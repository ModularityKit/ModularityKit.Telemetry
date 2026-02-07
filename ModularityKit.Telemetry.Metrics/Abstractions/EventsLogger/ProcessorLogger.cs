using Microsoft.Extensions.Logging;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Events;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Type;

namespace ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger;

public static class ProcessorLogger
{
    /// <summary>
    /// Logs processor-related events using <see cref="ProcessorLogType"/>.
    /// </summary>
    /// <param name="logger">The logger instance. Can be null.</param>
    /// <param name="type">The type of processor log.</param>
    /// <param name="args">Arguments to format into the log template.</param>
    public static void LogProcessor(this ILogger? logger, ProcessorLogType type, params object[] args)
    {
        var (eventId, template, level) = type switch
        {
            ProcessorLogType.Skipped   => (ProcessorEvents.SnapshotSkipped, "[Processor] Skipping snapshot: {Metric}", LogLevel.Debug),
            ProcessorLogType.Processed => (ProcessorEvents.SnapshotProcessed, "[Processor] Processed snapshot: {Metric}", LogLevel.Debug),
            ProcessorLogType.Routed    => (ProcessorEvents.SnapshotRouted, "[Processor] Routed snapshot {Metric} -> {Downstream}", LogLevel.Debug),
            _ => throw new ArgumentOutOfRangeException()
        };

        logger?.Log(level, eventId, template, args);
    }
}