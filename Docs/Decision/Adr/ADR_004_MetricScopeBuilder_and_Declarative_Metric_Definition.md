# ADR-004: Metric Scope Builder and Declarative Metric Definition

## Tag
#adr_004

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions.Scope

## Context

Defining metrics in the telemetry system should be:

- declarative,
- readable,
- error resistant,
- semantically consistent within single scope.

Without dedicated metric building mechanism:

- definitions would be scattered and difficult to audit,
- logic for creating individual metric types would be repeated,
- developer experience (DX) would degrade, especially with larger metric sets,
- metric registration within scopes would be procedural and error-prone.

It is necessary to introduce fluent builder that allows declarative metric definition within single `MetricScope`.

## Decision

### IMetricScopeBuilder

**Responsibilities:**

- Provide fluent API for declaratively defining metrics within a single scope.
- Support creation of all metric types:
    - Counter
    - Gauge
    - Histogram
    - Summary
    - Timer
- Encapsulate the rules for creating metric definitions.
- Enforce consistency of metric definitions within the scope.

### Builder Contract

- Each method call:
    - creates metric definition,
    - assigns it to the current `MetricScope`,
    - registers it in the `IMetricRegistry`.
- The builder does not emit metrics or hold runtime state.
- The builder returns itself (`IMetricScopeBuilder`) to support fluent chaining.

### Supported Definitions

- `Counter`, `Gauge` - simple metrics with unambiguous semantics.
- `Histogram` - supports optional value distribution buckets.
- `Summary` - supports optional quantiles.
- `Timer` - specialized for measuring operation durations.

### Integration with Registry

- `IMetricScopeBuilder` is used exclusively in the context of `IMetricRegistry.RegisterScope`.
- The scope serves as a configuration and logical boundary for the builder.
- The builder delegates the actual registration of definitions to the registry.

## Design Rationale

- Fluent builder significantly improves developer experience.
- Declarative metric definitions enhance readability and configuration auditability.
- The builder enforces correct association of metrics with a scope.
- Centralizing metric creation logic reduces errors and boilerplate.
- The model is natural extension of the registry and scopes without violating separation of concerns.