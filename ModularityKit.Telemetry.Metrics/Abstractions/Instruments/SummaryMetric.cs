using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

/// <summary>
/// Represents summary metric, which tracks configurable quantiles over stream of values.
/// </summary>
/// <remarks>
/// Summary metrics are used to calculate statistics such as median, 90th percentile, or 99th percentile.
/// Each emitted value contributes to the calculation of the configured quantiles. This record
/// inherits from <see cref="MetricDefinition"/> and pre-configures the metric type as
/// <see cref="MetricType.Summary"/>.
/// </remarks>
internal sealed record SummaryMetric : MetricDefinition
{
    /// <summary>
    /// The quantiles to track in the summary.
    /// </summary>
    /// <remarks>
    /// Values should be between 0 and 1. If none are provided, defaults are applied
    /// (50th, 90th, 95th, 99th percentiles).
    /// </remarks>
    internal double[] Quantiles { get; init; }
    
    /// <param name="name">The name of the metric.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="scope">The metric scope that defines where this metric is applicable.</param>
    /// <param name="unit">The unit of the metric (default is "1").</param>
    /// <param name="quantiles">Optional array of quantiles to track.</param>
    /// <param name="defaultTags">Optional default tags applied to all emitted values.</param>
    internal SummaryMetric(
        string name,
        string description,
        MetricScope scope,
        string unit = "1",
        double[]? quantiles = null,
        IReadOnlyDictionary<string, object?>? defaultTags = null)
        : base(name, description, MetricType.Summary, scope, unit, defaultTags)
    {
        Quantiles = quantiles ?? [0.5, 0.9, 0.95, 0.99];
    }
}