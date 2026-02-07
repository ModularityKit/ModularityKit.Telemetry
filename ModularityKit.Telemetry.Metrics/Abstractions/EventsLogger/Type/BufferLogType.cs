namespace ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Type;

/// <summary>
/// Represents the type of log event for a metrics buffer operation.
/// </summary>
/// <remarks>
/// Used by <see cref="BufferLogger.LogBuffer"/> to categorize buffer log messages.
/// </remarks>
public enum BufferLogType
{
    /// <summary>
    /// The buffer has started.
    /// </summary>
    Started,

    /// <summary>
    /// The buffer has processed/flushed a batch of metrics.
    /// </summary>
    Flow,

    /// <summary>
    /// The buffer is draining remaining metrics before shutdown.
    /// </summary>
    Draining,

    /// <summary>
    /// The buffer has fully stopped.
    /// </summary>
    Stopped,

    /// <summary>
    /// The buffer is full and applying the configured overflow strategy.
    /// </summary>
    Full,

    /// <summary>
    /// A metric snapshot was rejected because the buffer is not running.
    /// </summary>
    Rejected
}