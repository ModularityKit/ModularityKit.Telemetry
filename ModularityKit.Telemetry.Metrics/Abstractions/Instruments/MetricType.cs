namespace ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

/// <summary>
/// Defines the type of metric, which determines how its values are recorded and aggregated.
/// </summary>
/// <remarks>
/// Metric types inform the telemetry system how to interpret, process, and aggregate
/// emitted metric values. Different metric types support different operations, such as
/// cumulative counting, instantaneous readings, or value distributions.
/// </remarks>
public enum MetricType
{
    /// <summary>
    /// counter metric that can only increase over time.
    /// </summary>
    Counter,

    /// <summary>
    /// gauge metric that can increase or decrease over time.
    /// </summary>
    Gauge,

    /// <summary>
    /// histogram metric that measures the distribution of values across predefined buckets.
    /// </summary>
    Histogram,

    /// <summary>
    /// summary metric that tracks aggregate statistics such as min, max, sum, and count.
    /// </summary>
    Summary,

    /// <summary>
    /// timer metric used to measure the duration of operations.
    /// </summary>
    Timer
}