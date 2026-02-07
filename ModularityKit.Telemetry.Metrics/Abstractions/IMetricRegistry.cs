using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Abstractions;

/// <summary>
/// Provides registry for metrics and scopes, supporting registration, retrieval, and querying.
/// </summary>
/// <remarks>
/// The <see cref="IMetricRegistry"/> manages the lifecycle of metric definitions and scopes.
/// It allows metrics to be registered, organized under scopes, and queried by name, scope, or
/// custom predicates. This ensures a single source of truth for metric definitions within
/// an application.
/// </remarks>
public interface IMetricRegistry
{
    /// <summary>
    /// Registers single metric definition.
    /// </summary>
    /// <param name="metric">The metric definition to register.</param>
    void Register(MetricDefinition metric);

    /// <summary>
    /// Registers metric scope and allows defining metrics within that scope.
    /// </summary>
    /// <param name="scope">The metric scope to register.</param>
    /// <param name="configure">Action to configure metrics within the scope using a builder.</param>
    void RegisterScope(MetricScope scope, Action<IMetricScopeBuilder> configure);

    /// <summary>
    /// Retrieves metric by its fully qualified name.
    /// </summary>
    /// <param name="name">The metric name.</param>
    /// <returns>The registered metric definition.</returns>
    MetricDefinition Get(string name);

    /// <summary>
    /// Attempts to retrieve a metric by name.
    /// </summary>
    /// <param name="name">The metric name.</param>
    /// <param name="metric">The metric definition if found; otherwise, null.</param>
    /// <returns>True if the metric exists; false otherwise.</returns>
    bool TryGet(string name, out MetricDefinition? metric);

    /// <summary>
    /// Retrieves all registered metric definitions.
    /// </summary>
    /// <returns>An enumerable of all metrics.</returns>
    IEnumerable<MetricDefinition> GetAll();

    /// <summary>
    /// Retrieves all metrics belonging to a specific scope.
    /// </summary>
    /// <param name="scopeName">The name of the scope.</param>
    /// <returns>An enumerable of metrics within the given scope.</returns>
    IEnumerable<MetricDefinition> GetByScope(string scopeName);

    /// <summary>
    /// Queries metrics using a custom predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter metrics.</param>
    /// <returns>An enumerable of metrics matching the predicate.</returns>
    IEnumerable<MetricDefinition> Query(Func<MetricDefinition, bool> predicate);
}
