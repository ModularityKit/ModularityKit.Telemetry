using System.Diagnostics;
using System.Runtime.InteropServices;
using ModularityKit.Telemetry.Metrics.Abstractions.Policy;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Enrichers;

/// <summary>
/// Enriches metric snapshots with machine and runtime environment information.
/// </summary>
/// <remarks>
/// Adds tags such as machine name, OS description, processor count, .NET runtime version,
/// process memory usage, and system uptime.
/// </remarks>
public sealed class MachineInfoEnricher : IMetricEnricher
{
    /// <summary>
    /// Enriches the given metric snapshot with machine and environment details.
    /// </summary>
    /// <param name="snapshot">The original <see cref="MetricSnapshot"/> to enrich.</param>
    /// <returns>A new <see cref="MetricSnapshot"/> containing the enriched tags.</returns>
    public MetricSnapshot Enrich(MetricSnapshot snapshot)
    {
        var enrichedTags = new Dictionary<string, object?>(snapshot.Tags)
        {
            ["machine_name"] = Environment.MachineName,
            ["os_platform"] = RuntimeInformation.OSDescription,
            ["processor_count"] = Environment.ProcessorCount,
            ["dotnet_version"] = RuntimeInformation.FrameworkDescription,
            ["process_memory_mb"] = Math.Round(Process.GetCurrentProcess().WorkingSet64 / 1024.0 / 1024.0, 2),
            ["uptime_seconds"] = Math.Round(Environment.TickCount64 / 1000.0, 2)
        };

        return snapshot with { Tags = enrichedTags };
    }
}