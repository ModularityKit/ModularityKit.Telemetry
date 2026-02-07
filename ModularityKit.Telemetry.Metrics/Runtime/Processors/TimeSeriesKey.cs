using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Runtime.Processors;

/// <summary>
/// Represents unique key for a time series, combining a metric definition, optional tags, and environment.
/// </summary>
/// <remarks>
/// Designed for high performance scenarios:
/// - Tags are frozen into a <see cref="FrozenDictionary{TKey, TValue}"/> to reduce allocations.
/// - Equality checks are optimized using reference equality for metric and environment,
///   and value equality for tags.
/// </remarks>
internal readonly struct TimeSeriesKey(
    MetricDefinition metric,
    IReadOnlyDictionary<string, object?>? tags,
    string environment)
    : IEquatable<TimeSeriesKey>
{
    private readonly MetricDefinition _metric = metric;
    private readonly FrozenDictionary<string, object?>? _tags = tags is null || tags.Count == 0
        ? null
        : tags.ToFrozenDictionary(); // one time alloc per series

    private readonly string _environment = environment;

    /// <summary>
    /// Determines whether this <see cref="TimeSeriesKey"/> is equal to another instance.
    /// </summary>
    /// <param name="other">The other <see cref="TimeSeriesKey"/> to compare.</param>
    /// <returns>True if equal, otherwise false.</returns>
    public bool Equals(TimeSeriesKey other)
    {
        if (!ReferenceEquals(_metric, other._metric)) return false;
        if (!ReferenceEquals(_environment, other._environment)) return false;

        if (_tags == null && other._tags == null) return true;
        if (_tags == null || other._tags == null) return false;
        if (_tags.Count != other._tags.Count) return false;

        foreach (var kv in _tags)
        {
            if (!other._tags.TryGetValue(kv.Key, out var v)) return false;
            if (!Equals(kv.Value, v)) return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether this <see cref="TimeSeriesKey"/> is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if equal, otherwise false.</returns>
    public override bool Equals(object? obj)
        => obj is TimeSeriesKey other && Equals(other);

    /// <summary>
    /// Returns a hash code for this <see cref="TimeSeriesKey"/>.
    /// </summary>
    /// <returns>An integer hash code.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(RuntimeHelpers.GetHashCode(_metric));
        hash.Add(_environment);

        if (_tags != null)
        {
            foreach (var kv in _tags)
            {
                hash.Add(kv.Key);
                hash.Add(kv.Value);
            }
        }

        return hash.ToHashCode();
    }
}
