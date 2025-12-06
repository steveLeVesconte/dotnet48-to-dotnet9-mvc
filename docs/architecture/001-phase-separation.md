# ADR-001: Isolate Infrastructure from Platform Migration

## Status
Accepted

## Context
Most .NET Framework to .NET Core migrations combine cloud deployment and platform change in one phase. This compounds two high-risk changes.

## Decision
Separate into distinct phases:
- Phase 2: Deploy Framework 4.8 to Azure (infrastructure risk only)
- Phase 4: Migrate to .NET 9 (platform risk only)

## Consequences
**Positive**:
- Clear rollback points
- Can validate Azure works before changing platform
- If Phase 4 fails, can stay on Framework 4.8 in Azure

**Negative**:
- Slightly longer timeline (deploy twice)
- Some Azure config changes needed in Phase 4 (app settings)

## Rationale
Risk management beats speed. If deployment AND migration both fail, you can't tell which broke it.