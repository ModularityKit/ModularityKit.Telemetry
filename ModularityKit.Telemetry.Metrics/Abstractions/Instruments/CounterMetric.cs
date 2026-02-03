using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

/// <summary>
/// Represents counter metric that can only increase.
/// </summary>
/// <remarks>
/// Counter metrics are used to track cumulative quantities, such as the
/// number of requests, processed items, or events. They cannot decrease
/// over time. This record inherits from <see cref="MetricDefinition"/>
/// and pre-configures the metric type as <see cref="MetricType.Counter"/>.
/// </remarks>
internal sealed record CounterMetric : MetricDefinition
{
    /// <param name="name">The name of the metric.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="scope">The metric scope that defines where this metric is applicable.</param>
    /// <param name="unit">The unit of the metric (default is "1").</param>
    /// <param name="defaultTags">Optional default tags applied to all emitted values.</param>
    internal CounterMetric(
        string name,
        string description,
        MetricScope scope,
        string unit = "1",
        IReadOnlyDictionary<string, object?>? defaultTags = null)
        : base(name, description, MetricType.Counter, scope, unit, defaultTags)
    {
    }
}