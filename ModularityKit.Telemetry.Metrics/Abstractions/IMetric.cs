using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;
using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions;

/// <summary>
/// Represents the common interface for all metric definitions.
/// </summary>
/// <remarks>
/// The <see cref="IMetric"/> interface exposes the metadata for a metric,
/// including its name, description, type, unit, scope, and default tags.
/// All specific metric types (counter, gauge, histogram, summary, timer) implement this interface.
/// </remarks>
public interface IMetric
{
    /// <summary>
    /// The name of the metric.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A short description of the metric.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The type of the metric (Counter, Gauge, Histogram, Summary, Timer).
    /// </summary>
    MetricType Type { get; }

    /// <summary>
    /// The unit of measurement for the metric values.
    /// </summary>
    string Unit { get; }

    /// <summary>
    /// The scope in which the metric is defined.
    /// </summary>
    MetricScope Scope { get; }

    /// <summary>
    /// Default tags applied to all emitted values of this metric.
    /// </summary>
    IReadOnlyDictionary<string, object?> DefaultTags { get; }
}