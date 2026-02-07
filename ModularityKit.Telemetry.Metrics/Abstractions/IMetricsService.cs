using ModularityKit.Telemetry.Metrics.Abstractions.Scope;
using ModularityKit.Telemetry.Metrics.Runtime;

namespace ModularityKit.Telemetry.Metrics.Abstractions;

/// <summary>
/// Provides high level service for recording and managing metric data.
/// </summary>
/// <remarks>
/// <see cref="IMetricsService"/> abstracts operations for counters, gauges, and timers,
/// allowing clients to increment counters, set gauge values, or measure durations using timers.
/// This service integrates with the underlying metric registry, pipelines, and emitters.
/// </remarks>
public interface IMetricsService
{
    /// <summary>
    /// Increments counter metric by one.
    /// </summary>
    /// <param name="metric">The counter metric to increment.</param>
    /// <param name="tags">Optional tags to associate with this increment.</param>
    ValueTask IncrementAsync(MetricDefinition metric, IReadOnlyDictionary<string, object?>? tags = null);

    /// <summary>
    /// Increments counter metric by specified value.
    /// </summary>
    /// <param name="metric">The counter metric to increment.</param>
    /// <param name="value">The amount to increment by.</param>
    /// <param name="tags">Optional tags to associate with this increment.</param>
    ValueTask IncrementByAsync(MetricDefinition metric, double value, IReadOnlyDictionary<string, object?>? tags = null);

    /// <summary>
    /// Sets gauge metric to specific value.
    /// </summary>
    /// <param name="metric">The gauge metric to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="tags">Optional tags to associate with this value.</param>
    ValueTask SetAsync(MetricDefinition metric, double value, IReadOnlyDictionary<string, object?>? tags = null);

    /// <summary>
    /// Measures the duration of synchronous operation and emits it as a timer metric.
    /// </summary>
    /// <param name="metric">The timer metric to record.</param>
    /// <param name="tags">Optional tags to associate with this timing.</param>
    /// <returns>A <see cref="TimingScope"/> that stops the timer when disposed.</returns>
    TimingScope Time(MetricDefinition metric, IReadOnlyDictionary<string, object?>? tags = null);

    /// <summary>
    /// Measures the duration of an asynchronous operation and emits it as a timer metric.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="metric">The timer metric to record.</param>
    /// <param name="operation">The asynchronous operation to measure.</param>
    /// <param name="tags">Optional tags to associate with this timing.</param>
    /// <returns>The result of the operation.</returns>
    ValueTask<T> TimeAsync<T>(MetricDefinition metric, Func<Task<T>> operation, IReadOnlyDictionary<string, object?>? tags = null);
}
