using System.Reflection;
using ModularityKit.Telemetry.Metrics.Abstractions.Policy;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Enrichers;

/// <summary>
/// Enriches metric snapshots with service-related metadata.
/// </summary>
/// <remarks>
/// Adds tags such as the service name, version, environment, and a unique instance ID.
/// Useful for distinguishing metrics across different deployments, services, or instances.
/// </remarks>
public sealed class ServiceEnricher : IMetricEnricher
{
    private readonly string _serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown_service";
    private readonly string _serviceVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0";
    private readonly string _environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                                           ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                                           ?? "production";
    private readonly string _instanceId = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Enriches the given <see cref="snapshot"/> with service metadata tags.
    /// </summary>
    /// <param name="snapshot">The original metric snapshot to enrich.</param>
    /// <returns>
    /// A new <see cref="MetricSnapshot"/> containing the enriched tags: service name, version, environment, and instance ID.
    /// </returns>
    public MetricSnapshot Enrich(MetricSnapshot snapshot)
    {
        var enrichedTags = new Dictionary<string, object?>(snapshot.Tags)
        {
            ["service_name"] = _serviceName,
            ["service_version"] = _serviceVersion,
            ["environment"] = _environment,
            ["instance_id"] = _instanceId
        };

        return snapshot with { Tags = enrichedTags };
    }
}