using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Configurations;
using ModularityKit.Telemetry.Metrics.Abstractions.Emits;
using ModularityKit.Telemetry.Metrics.Abstractions.EventsLogger.Events;
using ModularityKit.Telemetry.Metrics.Abstractions.Pipeline;
using ModularityKit.Telemetry.Metrics.Abstractions.Scope;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Emits;

/// <summary>
/// Emits metrics either directly to a collector or via an optional <see cref="IMetricBuffer"/>.
/// Supports optional preprocessing via a <see cref="IMetricPipeline"/> and logging of all operations.
/// </summary>
/// <remarks>
/// Use <see cref="EmitAsync"/> for single metrics or <see cref="EmitBatchAsync"/> for bulk emission.
/// If a buffer is provided, metrics are enqueued; otherwise, they are immediately recorded in the collector.
/// </remarks>
public sealed class MetricEmitter(
    MetricsConfiguration config,
    IMetricCollector collector,
    IMetricPipeline? pipeline = null,
    IMetricBuffer? buffer = null,
    ILogger? logger = null)
    : IMetricProcessor, IAsyncDisposable
{
    private readonly ILogger? _logger = logger ?? NullLogger.Instance;
    
    ValueTask IMetricProcessor.EmitAsync(MetricSnapshot snapshot, CancellationToken ct)
        => EmitInternal(snapshot, ct);
    
    ValueTask IMetricProcessor.EmitBatchAsync(IEnumerable<MetricSnapshot> snapshots, CancellationToken ct)
    {
        return EmitBatchInternalAsync(snapshots, ct);
    }
    
    private async ValueTask EmitBatchInternalAsync(IEnumerable<MetricSnapshot> snapshots, CancellationToken ct)
    {
        foreach (var snapshot in snapshots)
        {
            var processed = pipeline?.Process(snapshot) ?? snapshot;
            _logger!.LogDebug(MetricEmitterEvents.EmitBatch, "Processing metric in batch: {Metric}", processed.Metric.Name);
            await EmitInternal(processed, ct);
        }
    }

    
    /// <summary>
    /// Emits snapshot either via buffer or directly to collector.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to emit.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the emission operation.</returns>
    private ValueTask EmitInternal(MetricSnapshot snapshot, CancellationToken ct)
    {
        if (buffer != null)
        {
            _logger!.LogDebug(MetricEmitterEvents.EnqueueBuffer, "Enqueue to buffer: {Metric}", snapshot.Metric.Name);
            return buffer.EnqueueAsync(snapshot, ct);
        }

        _logger!.LogDebug(MetricEmitterEvents.DirectRecord, "Direct record to collector: {Metric}", snapshot.Metric.Name);
        return new ValueTask(collector.RecordAsync(snapshot, ct));
    }

    /// <summary>
    /// Disposes the emitter.
    /// If a buffer is present, it either flushes remaining metrics or cancels without flushing
    /// depending on <see>
    ///     <cref>MetricBuffer.FlushOnDispose</cref>
    /// </see>
    /// .
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (buffer == null) return;

        var disposeTask = config.MetricBuffer?.FlushOnDispose == true
            ? buffer.DisposeAsync()
            : buffer.CancelWithoutFlushAsync();

        await disposeTask;
    }
}
