# ADR-007: Metric Policy Pipeline Interfaces

## Tag
#adr_007

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions.Policy

## Context

In ModularityKit telemetry system, once metric readings (`MetricSnapshot`) are recorded, they must be processed before being emitted to collectors or external systems. This processing includes:

- aggregating values (sums, averages, counts),
- enriching snapshots with additional tags or metadata,
- filtering unnecessary or redundant metrics,
- implementing intermediate pipeline logic (middleware).

Without clear interfaces for this processing:

- collector logic would become hard to maintain and inconsistent,
- the ordering and determinism of metric transformations would be unreliable,
- testing and composing telemetry pipelines would be difficult.

## Decision

### IMetricAggregator

- Aggregates individual metric snapshots for batching or summarization.
- Methods:
    - `Add(MetricSnapshot snapshot)` -  adds snapshot to aggregator.
    - `FlushAsync()` - asynchronously returns the collection of aggregated snapshots.
- Responsible for reducing emission load and preparing statistics.

### IMetricEnricher

- Enriches snapshots with additional information before emission.
- Can modify tags or metadata, eg, environment identifiers, service, or application context.
- Methods:
    - `Enrich(MetricSnapshot snapshot)` - returns an enriched snapshot.

### IMetricFilter

- Determines whether snapshot should be processed or discarded.
- Enables filtering, sampling, or volume control policies.
- Methods:
    - `ShouldProcess(MetricSnapshot snapshot)` - returns `true` if the snapshot should be processed.

### IMetricMiddleware

- Represents an intermediate component in the metric processing pipeline.
- Each middleware receives snapshot and a `next` delegate to pass snapshot downstream.
- Properties and methods:
    - `Process(MetricSnapshot snapshot, Func<MetricSnapshot, MetricSnapshot?> next)` - processes or discards the snapshot.
    - `Order` - defines execution order relative to other middleware.
- Enables implementation of cross cutting concerns such as filtering, enrichment, aggregation, or logging.

## Design Rationale

- Explicit interface separation enables modularity and simplifies pipeline testing.
- Middleware ensures deterministic ordering and composability of snapshot operations.
- Aggregators and enrichers optimize emission and provide contextual enrichment.
- Filters enable volume control and telemetry policies without impacting the rest of the pipeline.