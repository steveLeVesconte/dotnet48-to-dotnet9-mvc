# ADR-003: Fix Framework 4.8 Bugs Before Baseline

## Status
Accepted

## Context
Found two bugs during Phase 1 smoke testing:
1. Cart AJAX removal didn't work (jQuery load order)
2. Checkout validation didn't trigger (script bundling)

## Decision
Fixed both bugs BEFORE documenting baseline behavior.

## Consequences
**Positive**:
- Phase 4 validates against "correct Framework 4.8"
- Don't have to decide: "is this bug or migration issue?"
- Cleaner comparison (working vs working)

**Negative**:
- Technically changed seed project behavior
- Could argue "preserve bugs as baseline"

## Rationale
These were clear Framework 4.8 bugs (script ordering), not design choices. Migrating broken behavior would be confusing - you'd find the same bugs in .NET 9 and wonder if migration caused them.

Alternative considered: Document bugs as "known issues" but preserve them. Rejected because it makes Phase 4 harder (must distinguish "expected bug" from "new regression").