using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;
using ModularityKit.Telemetry.Metrics.Runtime.Aggregators.Data;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators;

/// <summary>
/// Aggregates <see cref="GaugeMetric"/> snapshots and periodically flushes them via <see cref="IMetricProcessor"/>.
/// </summary>
/// <remarks>
/// <see cref="GaugeAggregator"/> maintains thread-safe gauge values for each metric and its associated tags.
/// Each gauge is represented by <see cref="GaugeData"/> instance keyed by <see cref="MetricKey"/>.
/// The aggregator flushes snapshots at the interval configured in <see cref="BaseAggregator{TKey,TData}"/>.
/// </remarks>
internal sealed class GaugeAggregator(IMetricProcessor processor, TimeSpan flushInterval)
    : BaseAggregator<MetricKey, GaugeData>(processor, flushInterval)
{
    /// <summary>
    /// Adds <see cref="MetricSnapshot"/> to the aggregator.
    /// Only <see cref="GaugeMetric"/> snapshots are processed.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to add.</param>
    public override void Add(MetricSnapshot snapshot)
    {
        if (snapshot.Metric is not GaugeMetric gauge) 
            return;

        var key = new MetricKey(gauge.Name, snapshot.Tags);
        var data = Store.GetOrAdd(key, _ => new GaugeData(gauge, snapshot.Environment, snapshot.Tags));
        data.Set(snapshot.Value);
    }

    /// <summary>
    /// Builds the list of <see cref="MetricSnapshot"/> instances from the aggregated gauge data.
    /// </summary>
    /// <returns>An enumerable of snapshots ready for emission.</returns>
    protected override IEnumerable<MetricSnapshot> BuildSnapshots()
    {
        foreach (var kvp in Store)
        {
            yield return new MetricSnapshot(
                kvp.Value.Metric,
                kvp.Value.Value,
                kvp.Value.Tags,
                kvp.Value.Environment
            );
        }
    }
}
