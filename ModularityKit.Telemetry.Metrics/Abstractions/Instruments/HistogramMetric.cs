using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

/// <summary>
/// Represents histogram metric, which measures the distribution of values across predefined buckets.
/// </summary>
/// <remarks>
/// Histogram metrics are used to capture the distribution of values, such as request latency or
/// payload sizes. Each emitted value is placed into one of the configured buckets, allowing
/// calculation of percentiles, min/max, and averages. This record inherits from
/// <see cref="MetricDefinition"/> and pre-configures the metric type as
/// <see cref="MetricType.Histogram"/>.
/// </remarks>
internal sealed record HistogramMetric : MetricDefinition
{
    /// <summary>
    /// The bucket boundaries for histogram.
    /// </summary>
    /// <remarks>
    /// Values are placed into first bucket whose upper boundary is greater than or equal
    /// to the value. If no buckets are provided, a default set of latency-oriented buckets is used.
    /// </remarks>
    internal double[] Buckets { get; }
    
    /// <param name="name">The name of the metric.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="scope">The metric scope that defines where this metric is applicable.</param>
    /// <param name="unit">The unit of the metric (default is "ms").</param>
    /// <param name="buckets">Optional array of bucket boundaries. If null, defaults are applied.</param>
    /// <param name="defaultTags">Optional default tags applied to all emitted values.</param>
    internal HistogramMetric(
        string name,
        string description,
        MetricScope scope,
        string unit = "ms",
        double[]? buckets = null,
        IReadOnlyDictionary<string, object?>? defaultTags = null)
        : base(name, description, MetricType.Histogram, scope, unit, defaultTags)
    {
        Buckets = buckets ?? [10, 50, 100, 250, 500, 1000, 2500, 5000, 10000];
    }
}
