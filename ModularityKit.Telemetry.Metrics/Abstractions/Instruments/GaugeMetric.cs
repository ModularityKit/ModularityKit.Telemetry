using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

/// <summary>
/// Represents gauge metric, which can increase or decrease over time.
/// </summary>
/// <remarks>
/// Gauge metrics are used to track values that can go up and down, such as
/// temperature, memory usage, or current queue length. This record inherits
/// from <see cref="MetricDefinition"/> and pre-configures the metric type
/// as <see cref="MetricType.Gauge"/>.
/// </remarks>
internal sealed record GaugeMetric : MetricDefinition
{
    /// <param name="name">The name of the metric.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="scope">The metric scope that defines where this metric is applicable.</param>
    /// <param name="unit">The unit of the metric (default is "1").</param>
    /// <param name="defaultTags">Optional default tags applied to all emitted values.</param>
    internal GaugeMetric(
        string name,
        string description,
        MetricScope scope,
        string unit = "1",
        IReadOnlyDictionary<string, object?>? defaultTags = null)
        : base(name, description, MetricType.Gauge, scope, unit, defaultTags)
    {
    }
}