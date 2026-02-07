using System.Collections.Concurrent;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Policy;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators;

internal abstract class BaseAggregator<TKey, TData> : IMetricAggregator where TKey : notnull
{
    protected readonly ConcurrentDictionary<TKey, TData> Store = new();

    private readonly IMetricProcessor _processor;
    private readonly TimeSpan _flushInterval;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _backgroundTask;
    private int _disposedValue;

    protected BaseAggregator(
        IMetricProcessor processor,
        TimeSpan flushInterval)
    {
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        _flushInterval = flushInterval;
        _backgroundTask = Task.Run(FlushLoopAsync);
    }

    public abstract void Add(MetricSnapshot snapshot);

    private async Task FlushLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_flushInterval, _cts.Token);
                await FlushAsync();
            }
            catch (TaskCanceledException) { break; }
            catch { await Task.Delay(TimeSpan.FromSeconds(5), _cts.Token); }
        }

        await FlushAsync();
    }

    public async Task<IEnumerable<MetricSnapshot>> FlushAsync()
    {
        var snapshots = BuildSnapshots().ToList();
        Store.Clear();

        if (snapshots.Count > 0)
        {
            await _processor.EmitBatchAsync(snapshots);
        }

        return snapshots;
    }

    protected abstract IEnumerable<MetricSnapshot> BuildSnapshots();

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposedValue, 1) == 1)
            return;

        try
        {
            await _cts.CancelAsync();
            await _backgroundTask.ConfigureAwait(false);
        }
        catch (ObjectDisposedException) { }
        catch (OperationCanceledException) { }
        finally
        {
            _cts.Dispose();
        }
    }
}
