using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Policy;

/// <summary>
/// Determines whether metric snapshot should be processed or discarded.
/// </summary>
/// <remarks>
/// Metric filters provide a mechanism to include or exclude specific metrics
/// based on tags, names, values, or other criteria. They are useful for
/// controlling telemetry volume, implementing sampling, or enforcing policies
/// before metrics reach downstream systems.
/// </remarks>
public interface IMetricFilter
{
    /// <summary>
    /// Evaluates metric snapshot and determines if it should be processed.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the snapshot should be processed; <c>false</c> if it should be discarded.
    /// </returns>
    bool ShouldProcess(MetricSnapshot snapshot);
}