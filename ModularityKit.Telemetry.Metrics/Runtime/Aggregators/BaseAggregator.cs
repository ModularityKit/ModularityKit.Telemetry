using System.Collections.Concurrent;
using ModularityKit.Telemetry.Metrics.Abstractions;
using ModularityKit.Telemetry.Metrics.Abstractions.Emits;
using ModularityKit.Telemetry.Metrics.Abstractions.Policy;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators;

/// <summary>
/// Base class for aggregating metric snapshots over time and periodically flushing them to an <see cref="IMetricEmitter"/>.
/// </summary>
/// <typeparam name="TKey">The type used to key aggregated metric data. Must be non-nullable.</typeparam>
/// <typeparam name="TData">The type representing individual aggregated data entries.</typeparam>
/// <remarks>
/// <see cref="BaseAggregator{TKey,TData}"/> provides a thread-safe mechanism to store and aggregate metrics,
/// automatically flush them at configured intervals, and emit them in batches.
/// Derived classes implement the aggregation logic via <see cref="Add"/> and snapshot building via <see cref="BuildSnapshots"/>.
/// </remarks>
internal abstract class BaseAggregator<TKey, TData> : IMetricAggregator, IAsyncDisposable where TKey : notnull
{
    protected readonly ConcurrentDictionary<TKey, TData> Store = new();
    private readonly IMetricEmitter _emitter;
    private readonly TimeSpan _flushInterval;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _backgroundTask;

    /// <summary>
    /// Initializes a new instance of <see cref="BaseAggregator{TKey,TData}"/>.
    /// </summary>
    /// <param name="emitter">The metric emitter used to flush snapshots.</param>
    /// <param name="flushInterval">Interval at which aggregated metrics are automatically flushed.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="emitter"/> is null.</exception>
    internal BaseAggregator(IMetricEmitter emitter, TimeSpan flushInterval)
    {
        _emitter = emitter ?? throw new ArgumentNullException(nameof(emitter));
        _flushInterval = flushInterval;
        _backgroundTask = Task.Run(FlushLoopAsync);
    }

    /// <summary>
    /// Adds <see cref="MetricSnapshot"/> to aggregator.
    /// Derived classes define how snapshots are aggregated.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to add.</param>
    public abstract void Add(MetricSnapshot snapshot);

    /// <summary>
    /// Continuously flushes metrics at the configured interval until cancellation.
    /// </summary>
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

    /// <summary>
    /// Flushes all aggregated snapshots to the <see cref="IMetricEmitter"/> and clears the store.
    /// </summary>
    /// <returns>The list of snapshots that were flushed.</returns>
    public async Task<IEnumerable<MetricSnapshot>> FlushAsync()
    {
        var snapshots = BuildSnapshots().ToList();
        Store.Clear();

        if (snapshots.Count > 0)
            await _emitter.EmitBatchAsync(snapshots);

        return snapshots;
    }

    /// <summary>
    /// Builds the list of metric snapshots to be flushed.
    /// Derived classes must implement this to convert aggregated data into <see cref="MetricSnapshot"/> instances.
    /// </summary>
    /// <returns>An enumerable of snapshots ready for emission.</returns>
    protected abstract IEnumerable<MetricSnapshot> BuildSnapshots();

    /// <summary>
    /// Disposes the aggregator asynchronously, cancelling the background flush loop and waiting for its completion.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        await _backgroundTask;
        _cts.Dispose();
    }
}
