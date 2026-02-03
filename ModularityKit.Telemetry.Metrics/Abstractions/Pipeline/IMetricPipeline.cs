namespace ModularityKit.Telemetry.Metrics.Abstractions.Pipeline;

/// <summary>
/// Defines processing pipeline for metric snapshots.
/// </summary>
/// <remarks>
/// Metric pipelines allow metric snapshots to be transformed, filtered, or enriched
/// before being emitted to downstream consumers or telemetry backends. Implementations
/// can modify snapshots, drop them, or forward them unmodified.
/// </remarks>
public interface IMetricPipeline
{
    /// <summary>
    /// Processes single metric snapshot through the pipeline.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to process.</param>
    /// <returns>
    /// The processed <see cref="MetricSnapshot"/>, or <c>null</c> if the snapshot
    /// should be dropped.
    /// </returns>
    MetricSnapshot? Process(MetricSnapshot snapshot);
}