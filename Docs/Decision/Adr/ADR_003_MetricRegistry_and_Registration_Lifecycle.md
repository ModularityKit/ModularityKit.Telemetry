# ADR-003: Metric Registry and Registration Lifecycle

## Tag
#adr_003

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions

## Context

The ModularityKit telemetry system requires central and deterministic mechanism to manage metric definitions. Metrics are defined statically, shared across emitters, collectors, and policy pipelines, and must serve as a **single source of truth** within the application.

Without dedicated registry:

- metric definitions can be duplicated or inconsistent,
- telemetry validation and policy enforcement lack a stable reference point,
- introspective and auditable management of metrics becomes difficult,
- registering metrics within scopes becomes ambiguous.

It is necessary to introduce an explicit metric registry abstraction responsible for the lifecycle and lookup of metric definitions.

## Decision

### IMetricRegistry

**Responsibilities:**

- Serve as the central registry for metric definitions.
- Provide unified mechanism for registering metrics and scopes.
- Enable deterministic metric lookup:    
    - by fully qualified name,
    - by scope,
    - via predicates.
- Ensure consistency and integrity of metric definitions within the application.

### Registration Model

- The registry supports two registration modes:
    - single metric registration (`Register`),
    - scope registration along with its metric definitions (`RegisterScope`).
- `RegisterScope` uses dedicated builder (`IMetricScopeBuilder`) for declaratively defining metrics within a scope.
- The registry stores only definitions (`MetricDefinition`), without runtime state or emitted data.

### Query and Access

- The registry provides:
    
    - direct retrieval of a metric (`Get`),
    - safe retrieval (`TryGet`),
    - enumeration of all metrics (`GetAll`),
    - filtering by scope (`GetByScope`),
    - predicate based queries (`Query`).
- Metric name is treated as a logical identifier, derived from `MetricScope`.

### Contract

- `IMetricRegistry` is responsible only for metric definitions.
- The registry does not emit metrics or store telemetry data.
- Metric definitions are immutable once registered.
- The registry is used by:
    - emitters,
    - collectors,
    - telemetry policy pipelines,
    - introspection and diagnostic layers.

## Design Rationale

- A central registry ensures consistency and auditability of metric definitions.
- Separating registration from emission simplifies architecture and testing.
- Scope based builders support DX friendly, declarative model for defining metrics.
- The query model enables introspection and integration with governance tools.
- The registry provides stable foundation for further mechanisms:
    - telemetry policies,
    - validation,
    - metric synchronization and export.