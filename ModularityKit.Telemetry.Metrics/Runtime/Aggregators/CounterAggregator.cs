using System.Collections.Generic;
using System.Linq;
using ModularityKit.Telemetry.Metrics.Abstractions;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Emits;
using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;
using ModularityKit.Telemetry.Metrics.Runtime.Aggregators.Data;
using ModularityKit.Telemetry.Metrics.Runtime.Processors;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators;

/// <summary>
/// Aggregates <see cref="CounterMetric"/> snapshots and periodically flushes them via <see cref="IMetricProcessor"/>.
/// </summary>
/// <remarks>
/// <see cref="CounterAggregator"/> maintains thread-safe counts for each counter metric and its associated tags.
/// Values are aggregated in <see cref="CounterData"/> instances keyed by <see cref="MetricKey"/>.
/// The aggregator flushes snapshots at the interval configured in the base <see cref="BaseAggregator{TKey,TData}"/> class.
/// </remarks>
internal sealed class CounterAggregator(IMetricProcessor processor, TimeSpan flushInterval)
    : BaseAggregator<MetricKey, CounterData>(processor, flushInterval)
{
    /// <summary>
    /// Adds <see cref="MetricSnapshot"/> to the aggregator.
    /// Only <see cref="CounterMetric"/> snapshots are processed.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to add.</param>
    public override void Add(MetricSnapshot snapshot)
    {
        if (snapshot.Metric is not CounterMetric counter) return;

        var key = new MetricKey(counter.Name, snapshot.Tags);
        var data = Store.GetOrAdd(key, _ => new CounterData(counter, snapshot.Environment, snapshot.Tags));
        data.Increment(snapshot.Value);
    }

    /// <summary>
    /// Builds the list of <see cref="MetricSnapshot"/> instances from the aggregated counter data.
    /// </summary>
    /// <returns>An enumerable of snapshots ready for emission.</returns>
    protected override IEnumerable<MetricSnapshot> BuildSnapshots()
    {
        return Store.Select(kvp => new MetricSnapshot(
            kvp.Value.Metric,
            kvp.Value.Value,
            kvp.Value.Tags,
            kvp.Value.Environment
        ));
    }
}
