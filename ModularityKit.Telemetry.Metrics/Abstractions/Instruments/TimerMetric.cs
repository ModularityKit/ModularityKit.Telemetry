using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

/// <summary>
/// Represents timer metric, used to measure the duration of operations.
/// </summary>
/// <remarks>
/// Timer metrics track elapsed time and emit values in a defined unit (milliseconds by default).
/// They can be used to monitor request latency, execution time, or other time-based metrics.
/// This record inherits from <see cref="MetricDefinition"/> and pre-configures the metric type
/// as <see cref="MetricType.Timer"/>.
/// </remarks>
internal sealed record TimerMetric : MetricDefinition
{
    /// <param name="name">The name of the metric.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="scope">The metric scope that defines where this metric is applicable.</param>
    /// <param name="defaultTags">Optional default tags applied to all emitted values.</param>
    internal TimerMetric(
        string name,
        string description,
        MetricScope scope,
        IReadOnlyDictionary<string, object?>? defaultTags = null)
        : base(name, description, MetricType.Timer, scope, "ms", defaultTags)
    {
    }
}