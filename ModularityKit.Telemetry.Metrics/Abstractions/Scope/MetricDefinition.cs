using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Scope;

/// <summary>
/// Base class for defining metrics within a scope.
/// </summary>
/// <remarks>
/// MetricDefinition encapsulates the core metadata for a metric, including its name,
/// description, type, unit, scope, and default tags. All specific metric types
/// (counter, gauge, histogram, summary, timer) derive from this record.
/// </remarks>
public abstract record MetricDefinition : IMetric
{
    /// <summary>
    /// The name of the metric.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// A short description of the metric.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// The type of the metric (Counter, Gauge, Histogram, Summary, Timer).
    /// </summary>
    public MetricType Type { get; init; }

    /// <summary>
    /// The unit of the metric values (default is "1").
    /// </summary>
    public string Unit { get; init; }

    /// <summary>
    /// The scope in which the metric is defined.
    /// </summary>
    public MetricScope Scope { get; init; }

    /// <summary>
    /// Optional default tags applied to all emitted values.
    /// </summary>
    public IReadOnlyDictionary<string, object?> DefaultTags { get; init; }
    
    /// <param name="name">The metric name.</param>
    /// <param name="description">A description of the metric.</param>
    /// <param name="type">The metric type.</param>
    /// <param name="scope">The metric scope.</param>
    /// <param name="unit">The unit of measurement.</param>
    /// <param name="defaultTags">Optional default tags.</param>
    /// <exception cref="ArgumentNullException">Thrown if name, description, or scope is null.</exception>
    protected MetricDefinition(
        string name,
        string description,
        MetricType type,
        MetricScope scope,
        string unit = "1",
        IReadOnlyDictionary<string, object?>? defaultTags = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Unit = unit ?? "1";
        DefaultTags = defaultTags ?? new Dictionary<string, object?>();
    }

    /// <summary>
    /// The fully qualified metric name, including scope.
    /// </summary>
    public string FullName => Scope.GetFullName(Name);
}
