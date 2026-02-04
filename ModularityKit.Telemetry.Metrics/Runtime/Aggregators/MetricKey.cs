namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators;

/// <summary>
/// Represents unique key for identifying metric based on its name and tags.
/// </summary>
/// <remarks>
/// <see cref="MetricKey"/> is used internally to distinguish metric instances in aggregators or caches.
/// It computes a pre-calculated hash for performance and implements value-based equality considering
/// both the metric name and its associated tags.
/// </remarks>
internal readonly struct MetricKey : IEquatable<MetricKey>
{
    private readonly string _metricName;
    private readonly IReadOnlyDictionary<string, object?> _tags;
    private readonly int _hash;

    /// <summary>
    /// Initializes a new instance of <see cref="MetricKey"/> with the specified metric name and tags.
    /// </summary>
    /// <param name="metricName">The name of the metric.</param>
    /// <param name="tags">The tags associated with the metric.</param>
    public MetricKey(string metricName, IReadOnlyDictionary<string, object?> tags)
    {
        _metricName = metricName;
        _tags = tags;

        var hash = _metricName.GetHashCode();
        hash = _tags.OrderBy(k => k.Key)
            .Aggregate(hash, (current, kv)
                => HashCode.Combine(current, kv.Key.GetHashCode(), kv.Value?.GetHashCode() ?? 0));
        _hash = hash;
    }

    /// <summary>
    /// Determines whether this instance is equal to another <see cref="MetricKey"/>.
    /// </summary>
    /// <param name="other">The other metric key to compare.</param>
    /// <returns>True if the metric name and all tags match; otherwise, false.</returns>
    public bool Equals(MetricKey other)
    {
        if (_metricName != other._metricName) return false;
        if (_tags.Count != other._tags.Count) return false;

        foreach (var kv in _tags)
        {
            if (!other._tags.TryGetValue(kv.Key, out var value) || !Equals(kv.Value, value))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Determines whether this instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the object is <see cref="MetricKey"/> and equals this instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is MetricKey k && Equals(k);

    /// <summary>
    /// Returns the precomputed hash code of the metric key.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => _hash;
}
