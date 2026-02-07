using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModularityKit.Telemetry.Metrics.Abstractions.Emits;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Type;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Buffers;

/// <summary>
/// Consumes metric snapshots from a <see cref="ChannelReader{MetricSnapshot}"/>, batches them, 
/// and flushes to an <see cref="IMetricCollector"/>.
/// </summary>
/// <remarks>
/// <see cref="MetricConsumer"/> is designed to be thread-safe and allocation-friendly.
/// It uses an <see cref="ArrayPool{MetricSnapshot}"/> for batching and a <see cref="SemaphoreSlim"/> 
/// to synchronize flush operations. Supports both periodic and on-demand flushing.
/// </remarks>
internal sealed class MetricConsumer(
    ChannelReader<MetricSnapshot> reader,
    IMetricCollector collector,
    CancellationToken cancellationToken,
    int batchSize = 100,
    ILogger? logger = null)
    : IDisposable
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger? _logger = logger ?? NullLogger.Instance;
    private MetricSnapshot[]? _batchBuffer = ArrayPool<MetricSnapshot>.Shared.Rent(batchSize);
    private int _batchCount;
    private long _itemsProcessed;
    

    /// <summary>
    /// Starts consuming snapshots from the channel.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task RunAsync()
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!await reader.WaitToReadAsync(cancellationToken))
                    break;

                await _lock.WaitAsync(cancellationToken);
                try
                {
                    ReadBatchInline();
                    await FlushBatchInternalAsync();
                }
                finally
                {
                    _lock.Release();
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            await DrainAsync();
        }
    }

    /// <summary>
    /// Reads available items from the channel into the batch buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadBatchInline()
    {
        if (_batchBuffer == null) return;

        var buffer = _batchBuffer.AsSpan();
        while (_batchCount < buffer.Length && reader.TryRead(out var snapshot))
        {
            buffer[_batchCount++] = snapshot;
        }
    }

    /// <summary>
    /// Flushes any accumulated batch to the collector.
    /// </summary>
    public async ValueTask FlushAsync()
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            ReadBatchInline();
            await FlushBatchInternalAsync();
            _logger.LogBuffer(BufferLogType.Flow, _itemsProcessed, Interlocked.Read(ref _itemsProcessed));
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Internal implementation for flushing the batch to the collector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask FlushBatchInternalAsync()
    {
        if (_batchBuffer == null || _batchCount == 0) return;

        var count = _batchCount;
        var batch = new ReadOnlyMemory<MetricSnapshot>(_batchBuffer, 0, count);
    
        try
        {
            await collector.RecordBatchAsync(batch, cancellationToken);
            Interlocked.Add(ref _itemsProcessed, count);

            _logger.LogBuffer(BufferLogType.Flow, count, Interlocked.Read(ref _itemsProcessed));
        }
        catch (Exception ex)
        {
            _logger!.LogError(ex, "[Metrics Buffer] Error flushing batch");
        }
        finally
        {
            Array.Clear(_batchBuffer, 0, count);
            _batchCount = 0;
        }
    }


    /// <summary>
    /// Drains any remaining snapshots from the channel and flushes them.
    /// </summary>
    private async ValueTask DrainAsync()
    {
        await _lock.WaitAsync(CancellationToken.None);
        try
        {
            if (_batchBuffer == null) return;

            while (reader.TryRead(out var snapshot))
            {
                _batchBuffer[_batchCount++] = snapshot;
                if (_batchCount >= _batchBuffer.Length)
                    await FlushBatchInternalAsync();
            }

            if (_batchCount > 0)
                await FlushBatchInternalAsync();

            _logger.LogBuffer(BufferLogType.Stopped, Interlocked.Read(ref _itemsProcessed));
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Releases the buffer and other resources.
    /// </summary>
    public void Dispose()
    {
        var buffer = Interlocked.Exchange(ref _batchBuffer, null);
        if (buffer != null)
        {
            ArrayPool<MetricSnapshot>.Shared.Return(buffer);
        }
        _lock.Dispose();
    }
}
