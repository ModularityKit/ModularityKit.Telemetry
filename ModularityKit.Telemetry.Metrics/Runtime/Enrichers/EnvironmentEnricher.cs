using ModularityKit.Telemetry.Metrics.Abstractions.Policy;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Enrichers;

/// <summary>
/// Enriches metric snapshots with runtime environment metadata.
/// </summary>
/// <remarks>
/// This enricher augments the snapshot by attaching infrastructure-level tags
/// describing the current host and execution context.
/// <para>
/// The original <see cref="MetricSnapshot"/> is not mutated — a new snapshot instance
/// is created with an extended tag set, preserving pipeline immutability guarantees.
/// </para>
/// <para>
/// Added tags:
/// <list type="bullet">
/// <item><c>host</c> — machine hostname</item>
/// <item><c>process_id</c> — current process identifier</item>
/// <item><c>os</c> — operating system platform</item>
/// <item><c>timestamp_utc</c> — enrichment timestamp (UTC)</item>
/// </list>
/// </para>
/// <para>
/// Note: this operation allocates a new tag dictionary per snapshot.  
/// It should therefore be placed after high-volume filters (sampling/dedup/delta)
/// to avoid unnecessary allocations.
/// </para>
/// </remarks>
public sealed class EnvironmentEnricher : IMetricEnricher
{
    /// <summary>
    /// Adds environment metadata to the provided metric snapshot.
    /// </summary>
    /// <param name="snapshot">The snapshot to enrich.</param>
    /// <returns>
    /// A new <see cref="MetricSnapshot"/> instance containing the original data
    /// and additional environment tags.
    /// </returns>
    public MetricSnapshot Enrich(MetricSnapshot snapshot)
    {
        var enrichedTags = new Dictionary<string, object?>(snapshot.Tags)
        {
            ["host"] = Environment.MachineName,
            ["process_id"] = Environment.ProcessId,
            ["os"] = Environment.OSVersion.Platform.ToString(),
            ["timestamp_utc"] = DateTime.UtcNow
        };

        return snapshot with { Tags = enrichedTags };
    }
}
