namespace ModularityKit.Telemetry.Metrics.Abstractions.Configurations;

/// <summary>
/// Represents the root configuration for the metrics telemetry subsystem.
/// </summary>
/// <remarks>
/// This configuration aggregates all metric-related settings, including
/// buffering, aggregation, cardinality limits, and rate limiting. It is
/// typically bound from application configuration and applied during
/// telemetry initialization.
/// </remarks>
public sealed class MetricsConfiguration
{
    /// <summary>
    /// Logical environment name associated with emitted metrics.
    /// </summary>
    /// <remarks>
    /// Common values include <c>development</c>, <c>staging</c>, and <c>production</c>.
    /// The environment value may be emitted as a global tag or used for routing
    /// metrics to different backends.
    /// </remarks>
    public string Environment { get; init; } = "development";
    
    /// <remarks>
    /// If true, MetricBuffer will resolve ILogger&lt;MetricBuffer&gt; from the DI container.
    /// If false, MetricBuffer will not depend on any framework logger and logging will be no-op.
    /// </remarks>
    public bool UseFrameworkLogger { get; set; } = false;
    
    /// <summary>
    /// Enables or disables metric aggregation.
    /// </summary>
    /// <remarks>
    /// When enabled, multiple metric samples may be aggregated before emission
    /// (e.g. counters, histograms). This can reduce throughput at the cost of
    /// temporal resolution.
    /// </remarks>
    public bool EnableAggregation { get; set; } = false;
    
    /// <summary>
    /// Configuration for metric buffering behavior.
    /// </summary>
    public MetricBufferConfiguration MetricBuffer { get; init; } = new();
    
    /// <summary>
    /// Configuration for cardinality limiting of metric tags.
    /// </summary>
    public CardinalityLimitConfiguration CardinalityLimit { get; set; } = new();
    
    /// <summary>
    /// Configuration for rate limiting metric emission.
    /// </summary>
    public RateLimitConfiguration RateLimit { get; set; } = new();
}