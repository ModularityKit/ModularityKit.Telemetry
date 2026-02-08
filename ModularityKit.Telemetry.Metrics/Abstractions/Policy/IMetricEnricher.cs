using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Policy;

/// <summary>
/// Enriches metric snapshots with additional information before emission.
/// </summary>
/// <remarks>
/// Metric enrichers allow adding or modifying tags, labels, or other metadata
/// on metric snapshots. This can include environment data, service identifiers,
/// or contextual information required by downstream telemetry systems.
/// </remarks>
public interface IMetricEnricher
{
    /// <summary>
    /// Enriches metric snapshot with additional tags or metadata.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to enrich.</param>
    /// <returns>
    /// The enriched <see cref="MetricSnapshot"/>. Implementations may return
    /// the original snapshot modified or a new instance.
    /// </returns>
    MetricSnapshot Enrich(MetricSnapshot snapshot);
}