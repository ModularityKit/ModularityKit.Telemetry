namespace ModularityKit.Telemetry.Metrics.Abstractions.Buffers;

/// <summary>
/// Represents the internal lifecycle state of a metric buffer.
/// </summary>
/// <remarks>
/// This enum is used internally to coordinate buffer behavior during
/// normal operation, graceful shutdown, and forced termination.
/// </remarks>
internal enum MetricBufferState
{
    /// <summary>
    /// The buffer is actively accepting and processing metric snapshots.
    /// </summary>
    Running,

    /// <summary>
    /// The buffer is draining remaining snapshots but no longer accepts new ones.
    /// </summary>
    Draining,

    /// <summary>
    /// The buffer has stopped processing and released all resources.
    /// </summary>
    Stopped
}