namespace ModularityKit.Telemetry.Metrics.Abstractions.Configurations;

/// <summary>
/// Configures cardinality limits for metric dimensions (tags).
/// </summary>
/// <remarks>
/// Cardinality limits protect telemetry systems from unbounded growth
/// caused by high cardinality tag values (e.g. user IDs, request IDs).
/// When enabled, the system enforces a maximum number of unique tag
/// combinations within defined time window.
/// </remarks>
public sealed class CardinalityLimitConfiguration
{
    /// <summary>
    /// Enables or disables cardinality limiting.
    /// </summary>
    /// <remarks>
    /// When disabled, no cardinality checks are applied and all tag
    /// combinations are accepted.
    /// </remarks>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Maximum number of unique tag combinations allowed.
    /// </summary>
    /// <remarks>
    /// Exceeding this limit may result in metrics being dropped,
    /// sampled, or aggregated depending on the active policy.
    /// </remarks>
    public int MaxCardinality { get; set; } = 1000;

    /// <summary>
    /// Interval after which cardinality tracking is reset.
    /// </summary>
    /// <remarks>
    /// This defines the time window used to evaluate unique tag
    /// combinations. Shorter intervals reduce memory usage but
    /// may allow higher effective cardinality over time.
    /// </remarks>
    public TimeSpan ResetInterval { get; set; } = TimeSpan.FromHours(1);
}