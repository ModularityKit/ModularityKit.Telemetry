using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Configurations;

/// <summary>
/// Configures buffering behavior for metric emission.
/// </summary>
/// <remarks>
/// Metric buffering allows metrics to be accumulated and emitted in batches,
/// reducing overhead and improving throughput. This configuration controls
/// queue capacity, batching behavior, overflow handling, and flushing semantics.
/// </remarks>
public sealed class MetricBufferConfiguration
{
    /// <summary>
    /// Enables or disables metric buffering.
    /// </summary>
    /// <remarks>
    /// When disabled, metrics are emitted immediately without buffering.
    /// </remarks>
    public bool EnableBuffering { get; init; } = true;

    /// <summary>
    /// Maximum number of metric snapshots in a single batch.
    /// </summary>
    /// <remarks>
    /// Once this limit is reached, the buffer may trigger an immediate flush.
    /// </remarks>
    public int MaxBatchSize { get; init; } = 100;

    /// <summary>
    /// Maximum number of metric snapshots that can be queued.
    /// </summary>
    /// <remarks>
    /// Exceeding this limit causes the configured
    /// <see cref="OverflowStrategy"/> to be applied.
    /// </remarks>
    public int MaxQueueSize { get; init; } = 10_000;

    /// <summary>
    /// Interval at which buffered metrics are flushed automatically.
    /// </summary>
    /// <remarks>
    /// This interval applies only when buffering is enabled.
    /// </remarks>
    public TimeSpan FlushInterval { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Strategy used when the buffer queue reaches its capacity.
    /// </summary>
    public BufferOverflowStrategy OverflowStrategy { get; init; }
        = BufferOverflowStrategy.DropOldest;

    /// <summary>
    /// Indicates whether buffered metrics should be flushed on disposal.
    /// </summary>
    /// <remarks>
    /// When enabled, pending metrics are flushed during disposal.
    /// When disabled, buffered metrics are discarded.
    /// </remarks>
    public bool FlushOnDispose { get; init; } = true;
}
