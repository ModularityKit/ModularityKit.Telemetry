using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;
using ModularityKit.Telemetry.Metrics.Runtime.Aggregators.Data;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators;

/// <summary>
/// Aggregates <see cref="HistogramMetric"/> snapshots and periodically flushes them via <see cref="IMetricProcessor"/>.
/// </summary>
/// <remarks>
/// <see cref="HistogramAggregator"/> maintains thread-safe histogram data for each metric and its associated tags.
/// Each histogram is represented by <see cref="HistogramData"/> instance keyed by <see cref="MetricKey"/>.
/// The aggregator flushes snapshots at the interval configured in <see cref="BaseAggregator{TKey,TData}"/>.
/// During flush, both the sum of values and individual bucket counts are emitted as separate snapshots.
/// </remarks>
internal sealed class HistogramAggregator(IMetricProcessor processor, TimeSpan flushInterval)
    : BaseAggregator<MetricKey, HistogramData>(processor, flushInterval)
{
    /// <summary>
    /// Adds <see cref="MetricSnapshot"/> to the aggregator.
    /// Only <see cref="HistogramMetric"/> snapshots are processed.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to add.</param>
    public override void Add(MetricSnapshot snapshot)
    {
        if (snapshot.Metric is not HistogramMetric histMetric)
            return;

        var key = new MetricKey(histMetric.Name, snapshot.Tags);
        var data = Store.GetOrAdd(key, _ => new HistogramData(histMetric, snapshot.Environment, snapshot.Tags));
        data.Record(snapshot.Value);
    }

    /// <summary>
    /// Builds the list of <see cref="MetricSnapshot"/> instances from the aggregated histogram data.
    /// Emits sum and bucket counts as separate snapshots.
    /// </summary>
    /// <returns>An enumerable of snapshots ready for emission.</returns>
    protected override IEnumerable<MetricSnapshot> BuildSnapshots()
    {
        foreach (var (_, data) in Store)
        {
            var scope = data.Scope!;
            var tags = data.Tags;

            // sum
            yield return new MetricSnapshot(
                new HistogramMetric(data.MetricName, "sum", scope, unit: "ms", data.Buckets),
                data.Sum,
                tags,
                data.Environment);

            // bucket counts
            for (var i = 0; i < data.Buckets.Length; i++)
            {
                var count = data.Counts[i];
                if (count > 0)
                {
                    yield return new MetricSnapshot(
                        new HistogramMetric(data.MetricName, $"bucket <= {data.Buckets[i]}", scope, unit: "ms", data.Buckets),
                        count,
                        tags,
                        data.Environment);
                }
            }

            // +Inf bucket
            if (data.Counts[^1] > 0)
            {
                yield return new MetricSnapshot(
                    new HistogramMetric(data.MetricName, $"bucket > {data.Buckets[^1]}", scope, unit: "ms", data.Buckets),
                    data.Counts[^1],
                    tags,
                    data.Environment);
            }
        }
    }
}
