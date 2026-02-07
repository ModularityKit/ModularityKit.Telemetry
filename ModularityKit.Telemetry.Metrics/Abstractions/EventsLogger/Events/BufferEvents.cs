using Microsoft.Extensions.Logging;

namespace ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Events;

public static class BufferEvents
{
    // 100-199: Buffer & Flow
    /// <summary>
    /// Event indicating that the metrics buffer has started.
    /// </summary>
    public static readonly EventId BufferStarted = new(100, "BufferStarted");

    /// <summary>
    /// Event indicating that the metrics buffer has flushed a batch of items.
    /// </summary>
    public static readonly EventId BufferFlow = new(101, "BufferFlow");

    /// <summary>
    /// Event indicating that the metrics buffer is draining remaining items before shutdown.
    /// </summary>
    public static readonly EventId BufferDraining = new(102, "BufferDraining");

    /// <summary>
    /// Event indicating that the metrics buffer has fully stopped.
    /// </summary>
    public static readonly EventId BufferStopped = new(103, "BufferStopped");

    /// <summary>
    /// Event indicating that the metrics buffer is full and applying its overflow strategy.
    /// </summary>
    public static readonly EventId BufferFull = new(104, "BufferFull");

    /// <summary>
    /// Event indicating that a snapshot was rejected because the buffer is not running.
    /// </summary>
    public static readonly EventId BufferRejected = new(105, "BufferRejected");
}