## Tag
#adr_001

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions

## Context

The ModularityKit telemetry system supports multiple metric types (Counter, Gauge, Histogram, Summary, Timer) that need to be recorded, emitted, and processed in a consistent and predictable manner. Each metric has a common set of metadata, such as:

- name,
- description,
- metric type,
- unit of measurement,
- logical scope,
- default tags.

Without unified abstraction for metric definitions, the system would face:

- inconsistent metric definitions across types,
- duplication of logic in registries, emitters, and collectors,
- difficulties integrating with telemetry pipelines and policies,
- reduced developer experience when defining new metrics.

It is necessary to introduce common contract and a base implementation to ensure consistency and reduce boilerplate.

## Decision

### IMetric

**Responsibilities:**

- Represent metric definition independently of its concrete type.
- Provide the common metadata required for registration and emission.
- Explicitly and unambiguously define the metric type (`MetricType`).
- Define the unit of measurement for the metric value.
- Assign the metric to a logical scope (`MetricScope`).
- Provide set of default tags applied to all metric emissions.

### MetricDefinition

**Responsibilities:**

- Serve as the canonical base implementation of the `IMetric` interface.
- Encapsulate the complete metadata of the metric in an immutable model.
- Provide shared input validation logic (e.g., null checks).
- Expose the fully qualified metric name (`FullName`) based on `MetricScope`.
- Serve as the base class for all concrete metric definitions (Counter, Gauge, Histogram, Summary, Timer).

### Contract

- All concrete metric types **inherit from `MetricDefinition`**.
- `MetricDefinition` implements `IMetric` and contains no runtime or emission logic.
- The class is an immutable `record`, which:
    - supports definition comparison,
    - facilitates testing,
    - ensures safety when sharing definitions.
- `FullName` is computed deterministically based on `MetricScope`.

## Design Rationale

- Combining `IMetric` + `MetricDefinition` separates **contract** from **canonical implementation**, preserving flexibility.
- The base class reduces boilerplate and enforces consistency across metric definitions.
- Using `record` reinforces the “definition over behavior” semantics.
- `MetricScope` allows hierarchical and contextual grouping of metrics without affecting emission mechanics.
- The architecture aligns with policy driven telemetry and validation/emission pipelines.