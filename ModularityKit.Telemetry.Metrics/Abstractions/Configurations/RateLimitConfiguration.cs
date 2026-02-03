namespace ModularityKit.Telemetry.Metrics.Abstractions.Configurations;

/// <summary>
/// Configures rate limiting for metric emission.
/// </summary>
/// <remarks>
/// Rate limiting protects telemetry pipelines and downstream backends
/// from excessive metric throughput. It controls the maximum number of
/// metrics that can be emitted within a given time window and allows
/// short bursts when configured.
/// </remarks>
public sealed class RateLimitConfiguration
{
    /// <summary>
    /// Enables or disables rate limiting.
    /// </summary>
    /// <remarks>
    /// When disabled, metrics are emitted without throughput restrictions.
    /// </remarks>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Maximum number of metrics allowed per second.
    /// </summary>
    /// <remarks>
    /// This value defines the steady-state emission rate enforced
    /// by the rate limiter.
    /// </remarks>
    public int MaxMetricsPerSecond { get; set; } = 1000;

    /// <summary>
    /// Maximum number of metrics that can be emitted in a short burst.
    /// </summary>
    /// <remarks>
    /// Burst capacity allows temporary spikes in metric emission
    /// while still enforcing the long-term rate limit.
    /// </remarks>
    public int BurstSize { get; set; } = 100;
}