# ADR-008: Metric Pipeline Interface (`IMetricPipeline`)

## Tag
#adr_008

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions.Pipeline

## Context

The ModularityKit telemetry system requires processing metrics before they are emitted to backends or consumers. Typical operations include:

- enriching metrics with additional tags (enrichment),
- filtering metrics based on names, tags, or values (filtering),
- aggregating multiple snapshots to reduce emission volume (aggregation),
- transforming metrics for downstream compliance.

Without consistent pipeline abstraction:

- each emission would need to duplicate metric processing logic,
- combining different operations in single chain would be difficult,
- implementation would be less testable and less declarative.

## Decision

### `IMetricPipeline`

**Responsibilities:**

- Defines single processing point for metric snapshot.
- Supports implementing transformations, enrichment, filtering, or other operations within the pipeline.
- The `Process` method accepts `MetricSnapshot` and returns:
    - the processed snapshot, or
    - `null` if snapshot should be discarded.

**Contract:**

- Pipelines can be composed of multiple middleware components.
- Pipeline implementations can leverage existing interfaces:
    - `IMetricEnricher` — snapshot enrichment,
    - `IMetricFilter` — snapshot filtering,
    - `IMetricAggregator` — snapshot aggregation.
- Pipelines are backend agnostic, focusing solely on snapshot processing.

## Design Rationale

- Separates the definition of processing from metric emission, following the **definition first, runtime later** principle.
- Enables easy testing and composition of different operations on snapshots.
- A consistent `Process` method simplifies collector and emitter implementations.
- Returning `null` as signal to discard snapshot provides simple and declarative way to implement filters.