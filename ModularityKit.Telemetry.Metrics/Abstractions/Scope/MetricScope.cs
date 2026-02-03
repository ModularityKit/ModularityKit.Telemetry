namespace ModularityKit.Telemetry.Metrics.Abstractions.Scope;

/// <summary>
/// Represents logical scope for metrics, providing namespacing and metadata.
/// </summary>
/// <remarks>
/// Metric scopes are used to group related metrics under a common prefix, associate
/// them with an owner or service, and attach contextual metadata. They help organize
/// metrics and avoid naming collisions across different parts of an application.
/// </remarks>
public sealed record MetricScope
{
    /// <summary>
    /// The human-readable name of the metric scope.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The prefix applied to all metric names within this scope.
    /// </summary>
    private string Prefix { get; init; }

    /// <summary>
    /// The owner or service associated with this scope.
    /// </summary>
    public string Owner { get; init; }

    /// <summary>
    /// Optional metadata associated with the scope.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Metadata { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricScope"/> record.
    /// </summary>
    /// <param name="name">The scope name.</param>
    /// <param name="prefix">The prefix to prepend to metric names.</param>
    /// <param name="owner">The owner or service of the scope.</param>
    /// <param name="metadata">Optional metadata for the scope.</param>
    /// <exception cref="ArgumentNullException">Thrown if name, prefix, or owner is null.</exception>
    public MetricScope(
        string name, 
        string prefix, 
        string owner,
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Metadata = metadata ?? new Dictionary<string, object?>();
    }

    /// <summary>
    /// Returns the fully qualified metric name by combining the scope prefix with a metric name.
    /// </summary>
    /// <param name="metricName">The metric name to qualify.</param>
    /// <returns>The fully qualified metric name.</returns>
    public string GetFullName(string metricName) => $"{Prefix}.{metricName}";
}
