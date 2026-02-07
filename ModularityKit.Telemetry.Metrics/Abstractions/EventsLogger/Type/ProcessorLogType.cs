namespace ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Type;

/// <summary>
/// Event IDs for metric processor logging.
/// </summary>
public enum ProcessorLogType
{
    Skipped,    // snapshot was skipped
    Processed,  // snapshot was processed
    Routed      // snapshot was forwarded to another processor / downstream
}