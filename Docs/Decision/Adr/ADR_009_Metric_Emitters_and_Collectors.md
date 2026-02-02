# ADR-009: Metric Emitters and Collectors (`IMetricEmitter` & `IMetricCollector`)

## Tag
#adr_009

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions.Emits

## Context

ModularityKit telemetry system requires endpoints to record and transmit metrics to downstream pipelines or backends.

Key challenges include:

- consistently receiving metric snapshots from multiple scopes,
- supporting both single and batch emissions,
- integrating with pipelines that process snapshots (filters, enrichers, aggregators),
- optimizing for high frequency emissions and multithreaded scenarios.

Without dedicated emitter and collector abstractions:

- registration and emission logic would be duplicated across components,
- consistency and testability would be harder to maintain,
- there would be no clear separation between collecting and sending metrics.

## Decision

### `IMetricCollector`

**Responsibilities:**

- Records single or batch metric snapshots.
- Can perform additional operations such as filtering, aggregation, or batching before emission.
- Provides both synchronous (`Record`) and asynchronous (`RecordAsync`, `RecordBatchAsync`) APIs.
- Implementations must be efficient and resilient under frequent calls.

### `IMetricEmitter`

**Responsibilities:**

- Sends metrics to downstream pipeline.
- Supports single and batch snapshot emissions (`EmitAsync`, `EmitBatchAsync`).
- Must be lightweight, thread safe, and optimized for high frequency calls.
- Transforms `MetricDefinition` and `MetricSnapshot` into representations compatible with the pipeline.

### Contract

- Emitters and collectors operate independently of the pipeline type.
- Integration with `IMetricPipeline` occurs via passing snapshots.
- The public contract remains minimal and clean: only methods for snapshot registration and emission.

## Design Rationale

- Separates responsibilities: collectors gather snapshots, emitters forward them.
- Supports composition with pipelines: snapshots can be enriched, filtered, or aggregated before emission.
- Enables performance optimizations: batching, async processing, lightweight emissions.
- Interface separation facilitates testing and interchangeable implementations (e.g., mock collector, noop emitter, backend-specific emitter).
- Aligns with **definition first, runtime later**, where snapshots are defined and processed before actual emission.