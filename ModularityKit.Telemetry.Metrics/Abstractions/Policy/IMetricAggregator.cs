namespace ModularityKit.Telemetry.Metrics.Abstractions.Policy;

/// <summary>
/// Aggregates metric snapshots for batch processing or summarization.
/// </summary>
/// <remarks>
/// Metric aggregators collect individual metric snapshots and combine them
/// according to aggregation rules (eg, sum, average, count) before flushing
/// them to downstream pipelines or telemetry backends. This helps reduce
/// emission overhead and supports statistical calculations.
/// </remarks>
public interface IMetricAggregator
{
    /// <summary>
    /// Adds metric snapshot to the aggregator.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to add.</param>
    void Add(MetricSnapshot snapshot);

    /// <summary>
    /// Flushes all aggregated snapshots asynchronously.
    /// </summary>
    /// <returns>
    /// collection of <see cref="MetricSnapshot"/> representing the aggregated metrics.
    /// </returns>
    Task<IEnumerable<MetricSnapshot>> FlushAsync();
}