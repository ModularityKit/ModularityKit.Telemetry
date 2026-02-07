using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Configurations;
using ModularityKit.Telemetry.Metrics.Abstractions.Emits;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Type;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Buffers;

/// <summary>
/// A buffered metric collector that queues <see cref="MetricSnapshot"/> instances and flushes them periodically
/// or in batches to an <see cref="IMetricCollector"/>.
/// </summary>
/// <remarks>
/// The buffer uses <see cref="Channel{T}"/> internally and supports multiple overflow strategies:
/// dropping oldest, dropping newest, or blocking the producer.  
/// It also supports cancellation without flushing, periodic flushes, and logging via <see cref="ILogger"/>.
/// </remarks>
internal sealed class MetricBuffer : IMetricBuffer
{
    private readonly Channel<MetricSnapshot> _channel;
    private readonly MetricConsumer _consumer;
    private readonly MetricsConfiguration _options;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _consumerTask;
    private readonly Task _flushTask;
    private volatile MetricBufferState _state = MetricBufferState.Running;
    private readonly ILogger _logger;

    public MetricBuffer(IMetricCollector collector, MetricsConfiguration options, ILogger<MetricBuffer>? logger = null)
    {
        _options = options;
        
        _logger = _options.UseFrameworkLogger && logger != null
            ? logger
            : NullLogger.Instance;

        var channelOptions = new BoundedChannelOptions(options.MetricBuffer.MaxQueueSize)
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = options.MetricBuffer.OverflowStrategy switch
            {
                BufferOverflowStrategy.DropOldest => BoundedChannelFullMode.DropOldest,
                BufferOverflowStrategy.DropNewest => BoundedChannelFullMode.DropWrite,
                BufferOverflowStrategy.BlockProducer => BoundedChannelFullMode.Wait,
                _ => BoundedChannelFullMode.DropOldest
            }
        };

        _channel = Channel.CreateBounded<MetricSnapshot>(channelOptions);
        _consumer = new(_channel.Reader, collector, _cts.Token, options.MetricBuffer.MaxBatchSize, _logger);

        _logger.LogBuffer(BufferLogType.Started, options.MetricBuffer.MaxQueueSize);

        _consumerTask = Task.Run(() => _consumer.RunAsync());
        _flushTask = Task.Run(PeriodicFlushAsync);
    }

    /// <summary>
    /// A buffered metric collector that queues <see cref="MetricSnapshot"/> instances and flushes them periodically
    /// or in batches to an <see cref="IMetricCollector"/>.
    /// </summary>
    /// <remarks>
    /// The buffer uses a <see cref="Channel{T}"/> internally and supports multiple overflow strategies:
    /// dropping oldest, dropping newest, or blocking the producer.  
    /// It also supports cancellation without flushing, periodic flushes, and logging via <see cref="ILogger"/>.
    /// </remarks>
    public ValueTask EnqueueAsync(MetricSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        if (_state != MetricBufferState.Running)
        {
            _logger.LogBuffer(BufferLogType.Rejected);
            return ValueTask.CompletedTask;
        }

        if (_channel.Writer.TryWrite(snapshot)) return ValueTask.CompletedTask;
        _logger.LogBuffer(BufferLogType.Full, _options.MetricBuffer.OverflowStrategy);
        return _channel.Writer.WriteAsync(snapshot, cancellationToken);
    }

    /// <summary>
    /// Periodically flushes queued metrics to the collector according to <see cref="MetricBuffer.FlushInterval"/>.
    /// </summary>
    private async Task PeriodicFlushAsync()
    {
        using var timer = new PeriodicTimer(_options.MetricBuffer.FlushInterval);
        try
        {
            while (_state == MetricBufferState.Running && !_cts.IsCancellationRequested)
            {
                await timer.WaitForNextTickAsync(_cts.Token);
                await _consumer.FlushAsync();
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Buffer] PeriodicFlushAsync exception");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_state == MetricBufferState.Stopped) return;
        _state = MetricBufferState.Draining;

        _channel.Writer.TryComplete();
        _logger.LogBuffer(BufferLogType.Draining);
        
        try
        {
            await Task.WhenAll(_consumerTask, _flushTask).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Buffer] Error during shutdown");
        }

        _state = MetricBufferState.Stopped;
        await _cts.CancelAsync();
        _cts.Dispose();
        _consumer.Dispose();
    }

    public async ValueTask CancelWithoutFlushAsync()
    {
        if (_state == MetricBufferState.Stopped) return;

        _state = MetricBufferState.Draining;

        await _cts.CancelAsync();
        _channel.Writer.TryComplete();

        _state = MetricBufferState.Stopped;
        _cts.Dispose();
    }
}
