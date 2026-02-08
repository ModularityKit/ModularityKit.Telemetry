using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Type;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Processors;

/// <summary>
/// Routes metric snapshots to set of downstream processors based on ilter function for each processor.
/// </summary>
/// <remarks>
/// This processor acts as a topology router: it evaluates each snapshot against all registered processor filters,
/// and forwards snapshots to processors whose filter returns <c>true</c>. Skipped and routed events are logged
/// using <see cref="ProcessorLogger"/>.
/// </remarks>
internal sealed class TopologyProcessor(
    IEnumerable<(IMetricProcessor processor, Func<MetricSnapshot, bool> filter)> processors,
    ILogger? logger = null)
    : IMetricProcessor
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    private readonly List<(IMetricProcessor Processor, Func<MetricSnapshot, bool> Filter)> _processors = processors.ToList();
    
    /// <summary>
    /// Emits single snapshot to the appropriate downstream processors.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to emit.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public ValueTask EmitAsync(MetricSnapshot snapshot, CancellationToken ct = default)
        => EmitBatchInternalAsync([snapshot], ct);

    /// <summary>
    /// Emits batch of snapshots to the appropriate downstream processors.
    /// </summary>
    /// <param name="snapshots">The collection of metric snapshots to emit.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public ValueTask EmitBatchAsync(IEnumerable<MetricSnapshot> snapshots, CancellationToken ct = default)
        => EmitBatchInternalAsync(snapshots, ct);

    /// <summary>
    /// Internal implementation that iterates through snapshots and routes them to processors according to their filters.
    /// Logs skipped and routed snapshots.
    /// </summary>
    /// <param name="snapshots">The snapshots to process.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    private async ValueTask EmitBatchInternalAsync(IEnumerable<MetricSnapshot> snapshots, CancellationToken ct)
    {
        foreach (var snapshot in snapshots)
        {
            foreach (var (processor, _) in _processors)
            {
                _logger.LogProcessor(ProcessorLogType.Routed, snapshot.Metric.Name, processor.GetType().Name);
                await processor.EmitAsync(snapshot, ct);
            }
        }
    }
}