using Microsoft.Extensions.Logging;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Emits;

namespace ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Events;

/// <summary>
/// Defines EventIds for metric emission operations.
/// </summary>
/// <remarks>
/// These EventIds are used when logging operations performed by an <see cref="IMetricEmitter"/>,
/// such as direct recording, batch emission, or enqueueing to a buffer.
/// Event IDs are in the range 200–202.
/// </remarks>
public static class MetricEmitterEvents
{
    /// <summary>
    /// Logged when a batch of metrics is emitted via <see cref="IMetricEmitter.EmitBatchAsync"/>.
    /// </summary>
    public static readonly EventId EmitBatch = new(200, "EmitBatch");

    /// <summary>
    /// Logged when a metric snapshot is enqueued into a <see cref="IMetricBuffer"/>.
    /// </summary>
    public static readonly EventId EnqueueBuffer = new(201, "EnqueueBuffer");

    /// <summary>
    /// Logged when a metric snapshot is recorded directly without buffering.
    /// </summary>
    public static readonly EventId DirectRecord = new(202, "DirectRecord");
}