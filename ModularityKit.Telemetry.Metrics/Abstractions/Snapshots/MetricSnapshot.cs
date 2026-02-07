using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

/// <summary>
/// Represents single recorded metric value at specific point in time.
/// </summary>
/// <remarks>
/// <see cref="MetricSnapshot"/> captures the metric definition, its numeric value,
/// associated tags, environment, and timestamp. It also provides a convenient way
/// to combine the metric's default tags with snapshot-specific tags via <see cref="GetAllTags"/>.
/// </remarks>
public sealed record MetricSnapshot
{
    /// <summary>
    /// The metric definition associated with this snapshot.
    /// </summary>
    public MetricDefinition Metric { get; init; }

    /// <summary>
    /// The recorded value of the metric.
    /// </summary>
    public double Value { get; init; }

    /// <summary>
    /// Tags specific to this snapshot.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Tags { get; init; }

    /// <summary>
    /// The timestamp when the metric was recorded.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// The environment name where the metric was recorded.
    /// </summary>
    public string Environment { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricSnapshot"/> record.
    /// </summary>
    /// <param name="metric">The metric definition.</param>
    /// <param name="value">The value to record.</param>
    /// <param name="tags">Optional snapshot-specific tags.</param>
    /// <param name="environment">The environment name.</param>
    /// <param name="timestamp">Optional timestamp (defaults to UTC now).</param>
    /// <exception cref="ArgumentNullException">Thrown if metric or environment is null.</exception>
    public MetricSnapshot(
        MetricDefinition metric,
        double value,
        IReadOnlyDictionary<string, object?>? tags,
        string environment,
        DateTimeOffset? timestamp = null)
    {
        Metric = metric ?? throw new ArgumentNullException(nameof(metric));
        Value = value;
        Tags = tags ?? new Dictionary<string, object?>();
        Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        Timestamp = timestamp ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets the fully qualified name of the metric, including its scope.
    /// </summary>
    public string FullName => Metric.FullName;

    /// <summary>
    /// Combines the metric's default tags with snapshot-specific tags.
    /// </summary>
    /// <returns>A read-only dictionary of all tags.</returns>
    public IReadOnlyDictionary<string, object?> GetAllTags()
    {
        var combined = new Dictionary<string, object?>(Metric.DefaultTags);
        foreach (var tag in Tags)
        {
            combined[tag.Key] = tag.Value;
        }
        return combined;
    }
}
