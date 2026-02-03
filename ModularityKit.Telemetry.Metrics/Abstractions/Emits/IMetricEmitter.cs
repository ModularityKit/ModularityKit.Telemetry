using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Emits;

/// <summary>
/// Emits metric data to the configured telemetry pipeline.
/// </summary>
/// <remarks>
/// Metric emitters are responsible for translating metric definitions and
/// snapshots into pipeline compatible representations and forwarding them
/// for further processing. Implementations should be lightweight,
/// thread-safe, and optimized for high-frequency usage.
/// </remarks>
public interface IMetricEmitter
{
    /// <summary>
    /// Emits a single metric asynchronously.
    /// </summary>
    /// <param name="metric">The metric definition to emit.</param>
    /// <param name="value">The value of the metric.</param>
    /// <param name="tags">Optional metric tags.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="ValueTask"/> that completes when the metric has been
    /// accepted by the pipeline.
    /// </returns>
    ValueTask EmitAsync(
        MetricDefinition metric,
        double value,
        IReadOnlyDictionary<string, object?>? tags = null,
        CancellationToken ct = default);

    /// <summary>
    /// Emits a batch of metric snapshots asynchronously.
    /// </summary>
    /// <param name="snapshots">The collection of metric snapshots to emit.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="ValueTask"/> that completes when the batch has been
    /// accepted by the pipeline.
    /// </returns>
    ValueTask EmitBatchAsync(
        IEnumerable<MetricSnapshot> snapshots,
        CancellationToken ct = default);
}