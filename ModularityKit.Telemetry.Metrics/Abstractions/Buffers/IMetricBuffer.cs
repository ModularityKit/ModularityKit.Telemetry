namespace ModularityKit.Telemetry.Metrics.Abstractions.Buffers;

/// <summary>
/// Represents a buffer for temporarily storing metric snapshots
/// before they are flushed or processed by a downstream pipeline.
/// </summary>
/// <remarks>
/// Metric buffers decouple metric producers from consumers by absorbing
/// bursts of telemetry data. Implementations may apply batching,
/// backpressure, or overflow strategies to manage load and throughput.
/// </remarks>
public interface IMetricBuffer : IAsyncDisposable
{
    /// <summary>
    /// Enqueues a metric snapshot into the buffer.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to enqueue.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="ValueTask"/> that completes when the snapshot has been
    /// accepted by the buffer or rejected according to the overflow strategy.
    /// </returns>
    ValueTask EnqueueAsync(
        MetricSnapshot snapshot,
        CancellationToken ct = default);
    
    /// <summary>
    /// Cancels buffer processing without flushing pending metrics.
    /// </summary>
    /// <remarks>
    /// Pending buffered metrics will be discarded. This is typically used
    /// during shutdown or failure scenarios where flushing is undesirable
    /// or unsafe.
    /// </remarks>
    ValueTask CancelWithoutFlushAsync();
}