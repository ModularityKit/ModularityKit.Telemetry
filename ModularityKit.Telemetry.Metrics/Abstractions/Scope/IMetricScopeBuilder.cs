namespace ModularityKit.Telemetry.Metrics.Abstractions.Scope;

/// <summary>
/// Provides fluent builder for defining metrics within a specific scope.
/// </summary>
/// <remarks>
/// Metric scopes group related metrics together and allow declarative creation
/// of counters, gauges, histograms, summaries, and timers. Each method adds a
/// metric definition to the scope and returns the builder for chaining.
/// </remarks>
public interface IMetricScopeBuilder
{
    /// <summary>
    /// Adds counter metric to the scope.
    /// </summary>
    /// <param name="name">The name of the counter.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="unit">The unit of the metric (default is "1").</param>
    /// <returns>The builder for fluent chaining.</returns>
    IMetricScopeBuilder Counter(string name, string description, string unit = "1");

    /// <summary>
    /// Adds gauge metric to the scope.
    /// </summary>
    /// <param name="name">The name of the gauge.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="unit">The unit of the metric (default is "1").</param>
    /// <returns>The builder for fluent chaining.</returns>
    IMetricScopeBuilder Gauge(string name, string description, string unit = "1");

    /// <summary>
    /// Adds histogram metric to the scope.
    /// </summary>
    /// <param name="name">The name of the histogram.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="unit">The unit of the metric (default is "ms").</param>
    /// <param name="buckets">Optional bucket boundaries for the histogram.</param>
    /// <returns>The builder for fluent chaining.</returns>
    IMetricScopeBuilder Histogram(string name, string description, string unit = "ms", double[]? buckets = null);

    /// <summary>
    /// Adds summary metric to the scope.
    /// </summary>
    /// <param name="name">The name of the summary.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <param name="unit">The unit of the metric (default is "1").</param>
    /// <param name="quantiles">Optional quantiles to track in the summary.</param>
    /// <returns>The builder for fluent chaining.</returns>
    IMetricScopeBuilder Summary(string name, string description, string unit = "1", double[]? quantiles = null);

    /// <summary>
    /// Adds timer metric to the scope.
    /// </summary>
    /// <param name="name">The name of the timer.</param>
    /// <param name="description">A short description of the metric.</param>
    /// <returns>The builder for fluent chaining.</returns>
    IMetricScopeBuilder Timer(string name, string description);
}