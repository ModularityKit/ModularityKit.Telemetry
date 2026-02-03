namespace ModularityKit.Telemetry.Metrics.Abstractions.Emits;

/// <summary>
/// Collects metric snapshots for telemetry purposes.
/// Responsible for recording, storing, and forwarding metric data to
/// monitoring or analytics backends.
/// </summary>
/// <remarks>
/// Metric collectors handle the ingestion of metric snapshots produced by
/// various scopes in the system. Implementations may batch, filter, or
/// transform snapshots before sending them to external telemetry systems.
/// </remarks>
public interface IMetricCollector
{
    /// <summary>
    /// Records a single metric snapshot.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to record.</param>
    void Record(MetricSnapshot snapshot);

    /// <summary>
    /// Records a single metric snapshot.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to record.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RecordAsync(MetricSnapshot snapshot, CancellationToken cancellationToken);

    /// <summary>
    /// Records a batch of metric snapshots.
    /// </summary>
    /// <param name="snapshots">The collection of metric snapshots to record.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RecordBatchAsync(IEnumerable<MetricSnapshot> snapshots, CancellationToken cancellationToken);
}
