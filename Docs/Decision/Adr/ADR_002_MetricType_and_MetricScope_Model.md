# ADR-002: Metric Type and Metric Scope Model

## Tag
#adr_002

## Status
Accepted

## Date
2026-02-02

## Scope

ModularityKit.Telemetry.Metrics.Abstractions
- `Instruments`
- `Scope`

## Context

The ModularityKit telemetry system supports various metric semantics that determine how emitted values are recorded, interpreted, and aggregated. At the same time, metrics must be logically grouped, deterministically named, and resilient to naming collisions across modules, services, and teams.

Without clearly defined:

- **Metric types** (defining the semantic meaning of data),
- **Metric scopes** (defining context, ownership, and namespacing),

the telemetry system would be:

- difficult to extend,
- prone to semantic inconsistencies,
- ambiguous for collectors and backends,
- organizationally hard to scale.

It is necessary to formally define both the metric semantics and their logical context.

## Decision

### MetricType

**Responsibilities:**

- Define the semantic meaning of a metric and how emitted values are interpreted.
- Inform the telemetry system how metric:
    - accumulates data,
    - aggregates values,
    - supports runtime operations (increment, observe, timing).
- Allow unambiguous differentiation between metric classes.

**Defined Types:**

- `Counter` - cumulative metric, strictly increasing.
- `Gauge` - instantaneous metric, variable over time.
- `Histogram` - distribution of values into buckets.
- `Summary` - statistical aggregates (min, max, sum, count).
- `Timer` - specialized metric for measuring operation duration.

### MetricScope

**Responsibilities:**

- Define the logical context of metric.
- Provide deterministic namespacing of metrics via a prefix.
- Indicate ownership (service/module/domain) of metrics.
- Carry contextual metadata shared across group of metrics.
- Prevent naming collisions between independent parts of the system.

**Behavioral Contract:**

- Each metric is assigned to exactly one `MetricScope`.
- `MetricScope` is responsible for constructing the fully qualified metric name (`GetFullName`).
- Scope prefix is the single source of truth for namespacing.
- Scope contains no runtime or emission logic.

### Integration with MetricDefinition

- `MetricType` and `MetricScope` are required elements of `MetricDefinition`.
- `MetricDefinition.FullName` is deterministically computed from `MetricScope`.
- Scope serves as a static definition context, not dynamic runtime state.

## Design Rationale

- An explicit `MetricType` model eliminates semantic ambiguity.
- Separating **semantics (MetricType)** from **context (MetricScope)** simplifies the architecture.
- `MetricScope` enables hierarchical and organizational grouping without affecting metric behavior.
- Scope based namespacing is simple, deterministic, and auditable.
- The model supports the growth of the telemetry system in multi-module and multi-service environments.
- Architecture remains compatible with policy driven and pipeline based telemetry.