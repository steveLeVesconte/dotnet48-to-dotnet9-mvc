# ADR-002: Build Test Infrastructure Before Migration

## Status
Accepted

## Context
Could start migration immediately, but no automated way to verify behavior is preserved.

## Decision
Phase 1 dedicates time to:
- Document baseline behavior (smoke tests)
- Build integration test infrastructure
- Establish performance baselines

## Consequences
**Positive**:
- Phase 4 can run tests to verify migration
- Regression detection is automated, not manual
- Builds testing discipline for career growth

**Negative**:
- Delays visible progress (no .NET 9 code yet)
- Testing isn't as "exciting" as new features

## Rationale
Without tests, migration is guesswork. With tests, it's validation.