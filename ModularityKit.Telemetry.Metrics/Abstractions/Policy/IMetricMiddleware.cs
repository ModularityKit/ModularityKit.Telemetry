using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Policy;

/// <summary>
/// Represents middleware component that can process or transform metric snapshots
/// in pipeline.
/// </summary>
/// <remarks>
/// Metric middleware allows implementing cross-cutting concerns such as filtering,
/// enrichment, aggregation, or logging in a composable chain. Each middleware
/// receives a snapshot and <c>next</c> delegate to forward it to the subsequent
/// middleware. The <see cref="Order"/> property determines execution order.
/// </remarks>
public interface IMetricMiddleware
{
    /// <summary>
    /// Processes metric snapshot and optionally forwards it to the next middleware.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to process.</param>
    /// <param name="next">Delegate to invoke the next middleware in the chain.</param>
    /// <returns>
    /// The processed <see cref="MetricSnapshot"/>, or <c>null</c> if the snapshot
    /// should be discarded.
    /// </returns>
    MetricSnapshot? Process(MetricSnapshot snapshot, Func<MetricSnapshot, MetricSnapshot?> next);

    /// <summary>
    /// Determines the order in which this middleware is executed relative to others.
    /// </summary>
    int Order { get; }
}