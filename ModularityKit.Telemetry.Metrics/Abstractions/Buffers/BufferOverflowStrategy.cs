namespace ModularityKit.Telemetry.Metrics.Abstractions.Buffers;

/// <summary>
/// Defines strategies for handling buffer overflow when the metric buffer
/// reaches its capacity.
/// </summary>
/// <remarks>
/// Buffer overflow strategies determine how the system behaves when producers
/// emit metrics faster than they can be processed or flushed. The chosen
/// strategy affects data loss characteristics, backpressure, and overall
/// system stability.
/// </remarks>
public enum BufferOverflowStrategy
{
    /// <summary>
    /// Discards the oldest buffered metrics to make room for new ones.
    /// </summary>
    /// <remarks>
    /// This strategy favors recent data at the cost of historical accuracy.
    /// It is suitable for high throughput, real-time telemetry scenarios.
    /// </remarks>
    DropOldest,

    /// <summary>
    /// Discards newly emitted metrics when the buffer is full.
    /// </summary>
    /// <remarks>
    /// This strategy preserves existing buffered data but may lose the most
    /// recent measurements under sustained load.
    /// </remarks>
    DropNewest,

    /// <summary>
    /// Blocks the metric producer until buffer space becomes available.
    /// </summary>
    /// <remarks>
    /// This strategy applies backpressure to producers, preventing data loss
    /// but potentially increasing latency or affecting upstream components.
    /// </remarks>
    BlockProducer
}