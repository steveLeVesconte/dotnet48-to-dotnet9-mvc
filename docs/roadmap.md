# Migration Roadmap

**Last Updated**: November 15, 2025  
**Status**: Phase 0 (Planning & Setup)

## Overview

This project migrates the MVC Music Store from .NET Framework 4.8 to .NET 9, deployed on Azure. The migration follows a seven-phase approach that prioritizes safety, learning, and enterprise-quality practices.

> 📘 **New here?** This roadmap provides the strategic overview. See [README.md](../README.md) for current project status and quick links.

**Source Application**: Clean-room MVC5 implementation based on Microsoft's MVC Music Store tutorial  
**Target Platform**: .NET 9 with Azure cloud deployment  
**Timeline**: 17-21 weeks (part-time)

---

## Migration Strategy

### Why Seven Phases?

1. **Build safety nets first** (Phase 1: Testing infrastructure)
2. **Validate in production environment early** (Phases 2-3: Azure deployment)
3. **Isolate the hardest work** (Phase 4: Platform migration)
4. **Add modern capabilities incrementally** (Phases 5-7)

### Key Principles

- **No code changes before testing infrastructure exists**
- **Prove it works in Azure before changing platforms**
- **One major risk at a time**
- **Baselines before and after each phase**

---

## Phase Summary

| Phase | Focus | Duration | Key Risk |
|-------|-------|----------|----------|
| 0 | Planning & Setup | 3-5 days | Low |
| 1 | Testing & Observability | 3 weeks | Low |
| 2 | Azure Migration (Framework 4.8) | 3-4 weeks | Medium |
| 3 | Azure Validation | 1 week | Low |
| 4 | Platform Migration to .NET 9 | 4-5 weeks | **HIGH** |
| 5 | Modern Auth & Secrets | 2 weeks | Medium |
| 6 | APIs & Event-Driven Architecture | 3 weeks | Medium |
| 7 | Production Readiness | 2-3 weeks | Medium |

**Total Timeline**: 17-21 weeks (4-5 months)

---

## Phase Highlights

### Phase 0: Planning & Setup *(Current)*

**Goal**: Establish repository structure and documentation before writing code.

**Deliverables**:

- Repository structure (`/src`, `/docs`, `/infrastructure`)
- Core documentation (README, roadmap, dependency-inventory)
- Install MvcMusicStore seed project with shallow, short smoke test

### Phase 1: Foundation & Testing

**Goal**: Create a safety net before making changes.

**Key Deliverables**:

- Manual smoke tests with baseline times
- Integration test suite for critical workflows
- Unit test coverage for key components  
- Automated Selenium tests (one test/page)
- Application Insights instrumentation
- Performance baselines captured
- Security scan baseline (SAST/SCA)

**Why First**: Can't migrate safely without tests. Tests must work with Framework 4.8 before attempting .NET 9 migration.

### Phase 2: Azure Migration (Framework 4.8)

**Goal**: Prove the application works in Azure *before* platform changes.

**Key Deliverables**:
- Azure App Service deployment (Framework 4.8)
- Azure SQL Database
- Infrastructure as Code (Bicep)
- CI/CD pipeline (GitHub Actions)
- Azure Key Vault integration

**Why Before .NET 9**: Separates cloud deployment issues from platform migration issues.

### Phase 3: Azure Validation

**Goal**: Establish cloud performance baselines.

**Key Deliverables**:
- Load testing in Azure
- Performance comparison (local vs. Azure)
- Security validation
- Environment configuration documentation

**Why Important**: Phase 4 comparisons need Azure baselines, not just local ones.

### Phase 4: Platform Migration to .NET 9

**Goal**: Migrate to .NET 9 while maintaining all functionality.

**Critical Changes**:
- ASP.NET MVC 5 → ASP.NET Core MVC
- Entity Framework 6 → Entity Framework Core
- ASP.NET Identity 2.x → ASP.NET Core Identity
- OWIN middleware → ASP.NET Core middleware
- Web.config → appsettings.json

**Why Highest Risk**: Requires replacing every System.Web.* dependency and all authentication infrastructure. Unlike other phases, these changes are interdependent and cannot be isolated—we must break everything at once to migrate the platform.

**Success Criteria**: All tests pass, performance matches baselines, zero functional regressions.

### Phase 5: Modern Auth & Secrets

**Goal**: Add enterprise-grade authentication and secrets management.

**Key Deliverables**:
- External OAuth (Microsoft, Google)
- Managed Identity for Azure services
- All secrets in Key Vault
- Improved password policies

### Phase 6: APIs & Event-Driven Architecture

**Goal**: Transform into a modern, scalable architecture.

**Key Deliverables**:
- RESTful Web APIs
- Azure Cache for Redis (distributed sessions)
- Azure Service Bus (async order processing)
- OpenAPI/Swagger documentation

### Phase 7: Production Readiness

**Goal**: Add production-grade observability and deployment options.

**Key Deliverables**:
- OpenTelemetry distributed tracing
- Health check endpoints
- Circuit breaker and retry patterns
- Container deployment (Azure Container Apps)
- Deployment comparison (App Service vs. Container Apps)

---

## Success Definitions

### Minimum Viable Success
Complete through Phase 5:
- ✅ Working .NET 9 application in Azure
- ✅ Modern authentication (OAuth)
- ✅ Professional CI/CD and testing
- ✅ Security best practices

### Target Success
Complete through Phase 7:
- ✅ Production-ready cloud-native application
- ✅ Multiple deployment models demonstrated
- ✅ Full observability and resilience patterns
- ✅ Enterprise-grade architecture

---

## Project Constraints & Priorities

**Learning First**: This is a skill-building project, not production software. Decisions favor learning modern practices over shortcuts.

**Portfolio Quality**: Documentation and code quality reflect professional standards for mid-tier enterprise environments (regional banks, state agencies).

**AI-Assisted**: Development uses AI assistance transparently.

**Solo Development**: Branching strategy and workflows reflect single-developer reality.

---

**Document Status**: Frozen Planning Document will not be updated unless there are drastic changes to the project scope  
