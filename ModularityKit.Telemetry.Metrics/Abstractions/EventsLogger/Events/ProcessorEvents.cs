using Microsoft.Extensions.Logging;

namespace ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Events;

public static class ProcessorEvents
{
    // 200-299: Metric Processors
    public static readonly EventId SnapshotSkipped    = new(200, "SnapshotSkipped");  // e.g., filtered out, deduplicated, or delta = 0
    public static readonly EventId SnapshotProcessed  = new(201, "SnapshotProcessed"); // snapshot successfully processed
    public static readonly EventId SnapshotRouted     = new(202, "SnapshotRouted");   // snapshot forwarded to downstream/topology
}