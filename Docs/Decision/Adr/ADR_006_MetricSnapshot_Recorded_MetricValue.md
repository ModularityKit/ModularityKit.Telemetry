# ADR-006: MetricSnapshot as Immutable Recorded Metric Value

## Tag
#adr_006

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions

## Context

In ModularityKit telemetry system, each metric can have multiple readings over time. Each reading should preserve:

- the metric value at a specific point in time,
- snapshot specific tags,
- the environment in which the metric was recorded,
- the timestamp of the reading.

Without a consistent snapshot model:

- aggregation and analysis of metrics would be difficult,
- default metric tags could be inconsistently applied,
- telemetry collectors could produce inconsistent data.

## Decision

### MetricSnapshot

`MetricSnapshot` is an immutable `record` that:

- stores reference to the `MetricDefinition`,
- records the numeric `Value`,
- holds snapshot specific tags `Tags`,
- records the measurement time `Timestamp` and environment `Environment`,
- exposes the metric `FullName`, including scope,
- provides convenient merging of default metric tags with snapshot tags via `GetAllTags()`

### Responsibilities

- Preserve full consistency and immutability of each reading.
- Provide simple access to all tags (`Metric.DefaultTags` + `Tags`).
- Allow unambiguous identification of metric reading within the telemetry pipeline.

### Usage

- Created directly when recording metric reading in an emitter or collector.
- Can be used for aggregation, filtering, and exporting telemetry data.

## Design Rationale

- Immutable `record` guarantees that snapshots cannot be modified after creation.
- Merging default metric tags with snapshot-specific tags simplifies consumption in the telemetry pipeline.
- Centralizing reading definitions in `MetricSnapshot` provides consistent model for collectors, aggregators, and telemetry policy pipelines.

---