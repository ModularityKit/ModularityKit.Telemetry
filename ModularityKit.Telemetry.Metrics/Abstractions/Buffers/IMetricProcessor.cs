using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Buffers;

/// <summary>
/// Represents a processor capable of handling metric snapshots.
/// </summary>
/// <remarks>
/// Processors implement logic to transform, filter, or route <see cref="MetricSnapshot"/> instances.
/// They can operate on single snapshots via <see cref="EmitAsync"/> or on batches via <see cref="EmitBatchAsync"/>.
/// This interface is designed for building metric pipelines with composable processing steps.
/// </remarks>
internal interface IMetricProcessor
{
    /// <summary>
    /// Processes a single metric snapshot asynchronously.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to process.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask EmitAsync(MetricSnapshot snapshot, CancellationToken ct = default);

    /// <summary>
    /// Processes a batch of metric snapshots asynchronously.
    /// </summary>
    /// <param name="snapshots">The collection of metric snapshots to process.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask EmitBatchAsync(IEnumerable<MetricSnapshot> snapshots, CancellationToken ct = default);
}