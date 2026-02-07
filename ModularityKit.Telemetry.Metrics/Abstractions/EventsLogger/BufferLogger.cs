using Microsoft.Extensions.Logging;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Events;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Type;

namespace ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger;

public static class BufferLogger
{
    /// <summary>
    /// Logs buffer related event using specified <see cref="BufferLogType"/> and arguments.
    /// </summary>
    /// <param name="logger">The logger instance. Can be null, in which case no logging occurs.</param>
    /// <param name="type">The type of buffer event.</param>
    /// <param name="args">The arguments to format into the log message template.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="type"/> is not recognized.</exception>
    public static void LogBuffer(this ILogger? logger, BufferLogType type, params object[] args)
    {
        var (eventId, template, level) = type switch
        {
            BufferLogType.Started   => (BufferEvents.BufferStarted, "[Metrics Buffer] Started with capacity: {Capacity}", LogLevel.Information),
            BufferLogType.Flow      => (BufferEvents.BufferFlow, "[Metrics Buffer] Flushed {Count} items. Total processed: {Total}", LogLevel.Debug),
            BufferLogType.Draining  => (BufferEvents.BufferDraining, "[Metrics Buffer] Draining remaining items before shutdown...", LogLevel.Information),
            BufferLogType.Stopped   => (BufferEvents.BufferStopped, "[Metrics Buffer] Stopped. Final processed count: {Total}", LogLevel.Information),
            BufferLogType.Full      => (BufferEvents.BufferFull, "[Metrics Buffer] Buffer is full! Applying strategy: {Strategy}", LogLevel.Warning),
            BufferLogType.Rejected  => (BufferEvents.BufferRejected, "[Metrics Buffer] Reject snapshot - buffer not running", LogLevel.Warning),
            _ => throw new ArgumentOutOfRangeException()
        };
    
        logger.Log(level, eventId, template, args);
    }

}
