2025-11-22 corrections spelling and capitalization



2025-11-21 corrections for "seed realistic data"

2025-11-21 micro changes after final polish prompt D:\_AI_workspace\prompts\dotnet48-to-dotnet9-mvc\2025-11-21-13-issues-re-validate

status: READY FOR USE

# Phase 1 GitHub Issues - Data Entry Document

**⚠️ IMPORTANT: This is a data entry aid only. Do NOT commit this file to the repository.**

**Version**: 3.1
 **Date**: November 21, 2025
 **Total Issues**: 19
 **Target Milestone**: Phase 1 - Testing & Observability

------

## Data Entry Instructions

### Issue Format

Each issue begins with metadata (Title, Labels, Priority, etc.) followed by the issue body content.

### Delimiter Convention

Issues are separated by:

```
--- END ISSUE #N ---
---
---
```

### What to Enter

Copy everything between `**Body**:` and the delimiter (`--- END ISSUE #N ---`) into the GitHub issue body.

### After Creating Issues

1. Use GitHub's native dependency linking: Type `#` followed by issue number
2. Add issues to the "Phase 1" milestone
3. Assign to yourself
4. Add to project board for tracking
5. Apply labels for filtering: `foundation`, `critical-path`, `testability`, `validation`

------

## Issue #1: Create and execute manual smoke test baseline

**Title**: Create and execute manual smoke test baseline

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `documentation`, `foundation`

**Priority**: P2 (Important)

**Risk**: Low

**Skills**: Documentation, Testing

**Body**:

```markdown
Create standardized smoke test procedures to establish baseline functionality before any code changes.

## Tasks

- [ ] Create `/docs/testing/smoke-test-procedures.md` with procedures for 9 key pages
- [ ] Create `/docs/test-accounts.md` with test credentials (add to .gitignore)
- [ ] Document pass/fail criteria and severity classification
- [ ] Execute smoke tests and save results to `/docs/baselines/phase1-smoke-test-results-YYYY-MM-DD.md`

## Acceptance Criteria

- [ ] All 9 pages tested with documented pass/fail criteria
- [ ] Test accounts documented separately (not in procedures file)
- [ ] Results file includes baseline times and test summary

## Definition of Done

- [ ] Documentation includes reproduction steps
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #1 ---

------

------

## Issue #2: Create xUnit integration test project

**Title**: Create xUnit integration test project

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `infrastructure`, `foundation`

**Priority**: P1 (Critical Path)

**Risk**: Low

**Skills**: Testing, .NET Framework

**Body**:

```markdown
Set up xUnit test project infrastructure to support integration testing against Framework 4.8.

## Tasks

- [ ] Create `MvcMusicStore.Tests` project in `/src/MvcMusicStore.Tests`
- [ ] Add NuGet packages (xUnit, xUnit.runner.visualstudio, Microsoft.NET.Test.Sdk)
- [ ] Write sample integration test that verifies database connection

## Acceptance Criteria

- [ ] `dotnet build` and `dotnet test` run successfully from solution root
- [ ] Sample test passes consistently
- [ ] No hardcoded paths (configuration is portable)
- [ ] Test structure documented in `/docs/testing/test-structure.md`

## Definition of Done

- [ ] Documentation includes reproduction steps
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #2 ---

------

------

## Issue #3: Integration tests for shopping cart

**Title**: Integration tests for shopping cart

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `critical-path`

**Priority**: P1 (Critical Path)

**Risk**: Medium

**Skills**: Testing, Database

**Dependencies**: Blocked by #2

**Body**:

```markdown
Create integration tests for shopping cart workflows to validate cart persistence survives platform migration.

## Tasks

- [ ] Create test infrastructure (as needed):
  - `DatabaseFixture.cs` - xUnit collection fixture for test database lifecycle
  - `DatabaseCollection.cs` - collection definition marker
  - `DatabaseIntegrationTestBase.cs` - base class with fresh context helpers
  - `TestHttpContext.cs` - fake HttpContext for session/user simulation
- [ ] Create `ShoppingCartIntegrationTests.cs` in `/src/MvcMusicStore.Tests/Integration/Business`
- [ ] Test: add, remove, update quantity, calculate totals
- [ ] Test both anonymous and authenticated user workflows
- [ ] Verify cart persistence across sessions

## Acceptance Criteria

- [ ] Tests pass with 100% success rate against unmodified Framework 4.8
- [ ] Both anonymous and authenticated scenarios covered
- [ ] Cart calculations validated (subtotal, quantities, item management)

## Definition of Done

- [ ] Tests passing locally via `dotnet test`
- [ ] PR merged to main (self-reviewed checklist complete)

## Notes

- Risk: Session state behavior differs between Framework 4.8 and ASP.NET Core
```

------

## --- END ISSUE #3 ---

------

------

## Issue #4: Integration tests for order placement

**Title**: Integration tests for order placement

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `critical-path`

**Priority**: P0 (Blocking)

**Risk**: High

**Skills**: Testing, Database

**Dependencies**: Blocked by #2

**Body**:

```markdown
Create integration tests for complete order workflow from cart to database persistence.

## Tasks

- [ ] Create `OrderPlacementIntegrationTests.cs` in `/src/MvcMusicStore.Tests/Integration`
- [ ] Test end-to-end: cart → checkout → database persistence
- [ ] Test promo code validation (valid: "FREE", invalid: rejection)
- [ ] Test order total calculations
- [ ] Test Order → OrderDetails relationship integrity
- [ ] Verify cart clearing after successful order

## Acceptance Criteria

- [ ] Complete order workflow tested through database persistence
- [ ] Promo code validation covered for valid and invalid cases
- [ ] Database relationships validated (Order has OrderDetails)
- [ ] Cart state verified post-checkout (empty after order)

## Definition of Done

- [ ] Tests passing locally via `dotnet test`
- [ ] PR merged to main (self-reviewed checklist complete)

## Notes

- Most complex business logic in application
- Comprehensive coverage prevents data integrity issues during migration
```

------

## --- END ISSUE #4 ---

------

------

## Issue #5: Integration tests for admin CRUD operations

**Title**: Integration tests for admin CRUD operations

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `authorization`

**Priority**: P1 (Critical Path)

**Risk**: Medium

**Skills**: Testing, Authorization

**Dependencies**: Blocked by #2

**Body**:

```markdown
Create integration tests for StoreManager CRUD operations and role-based authorization.

## Tasks

- [ ] Create `StoreManagerIntegrationTests.cs` in `/src/MvcMusicStore.Tests/Integration`
- [ ] Test authorization (admin access allowed, non-admin blocked)
- [ ] Test CRUD operations (Create, Read, Update, Delete albums)
- [ ] Test Genre and Artist relationships in album creation

## Acceptance Criteria

- [ ] Authorization tests verify admin role enforcement
- [ ] All CRUD operations tested with database persistence
- [ ] Genre and Artist foreign key relationships tested
- [ ] Both authorized and unauthorized access scenarios covered

## Definition of Done

- [ ] Tests passing locally via `dotnet test`
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #5 ---

------

------

## Issue #6: Create Selenium test project

**Title**: Create Selenium test project

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `ui-automation`, `foundation`

**Priority**: P1 (Critical Path)

**Risk**: Medium

**Skills**: Testing, Selenium, UI

**Dependencies**: Blocked by #2

**Body**:

```markdown
Set up Selenium/WebDriver infrastructure for automated UI testing.

## Tasks

- [ ] Create `MvcMusicStore.SeleniumTests` project
- [ ] Add NuGet packages: Selenium.WebDriver, Selenium.WebDriver.ChromeDriver, xUnit
- [ ] Create `SeleniumTestBase.cs` with WebDriver lifecycle management
- [ ] Implement test helpers (login, logout, navigation, cart setup, wait operations)
- [ ] Create `/PageObjects` folder with stub page objects for 9 pages
- [ ] Configure `app.config` (BaseUrl, HeadlessMode, ImplicitWaitSeconds, ScreenshotPath)
- [ ] Create sample smoke test that verifies home page loads

## Acceptance Criteria

- [ ] Selenium project builds successfully
- [ ] Sample test passes in headed and headless modes
- [ ] Screenshots captured on failure to `/TestResults/Screenshots`
- [ ] Page object stubs created for all 9 pages
- [ ] Setup documented in `/docs/testing/selenium-setup.md`

## Definition of Done

- [ ] Tests passing locally via `dotnet test`
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #6 ---

------

------

## Issue #7: Selenium tests for critical pages

**Title**: Selenium tests for critical pages

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `ui-automation`, `critical-path`

**Priority**: P1 (Critical Path)

**Risk**: Medium

**Skills**: Testing, Selenium, UI

**Dependencies**: Blocked by #6

**Body**:

```markdown
Create automated UI tests for 9 critical pages using moderate assertion coverage (3-5 assertions per page).

**Goal**: Verify pages load and core functionality works - not exhaustive element verification.

## Tasks

- [ ] Implement Page Objects for all 9 pages with appropriate locators
- [ ] Create test classes for: Home, Store Index, Store Browse, Store Details, Cart, Checkout, Admin, Login, Register
- [ ] Implement 15+ total tests (2-3 tests per page, 3-5 assertions each)
- [ ] Implement console error detection on all tests
- [ ] Verify tests pass consistently (3 consecutive runs, 100% pass rate)

## Acceptance Criteria

- [ ] All 9 page test classes implemented with Page Objects
- [ ] Each page has focused assertion coverage (3-5 assertions per test)
- [ ] Console error detection working
- [ ] Test suite completes in under 5 minutes
- [ ] 100% pass rate over 3 consecutive runs

## Definition of Done

- [ ] Tests passing locally via `dotnet test`
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #7 ---

------

------

## Issue #8: Implement constructor injection in AccountController

**Title**: Implement constructor injection in AccountController

**Milestone**: Phase 1

**Labels**: `phase-1`, `refactoring`, `code-change`, `testability`

**Priority**: P1 (Critical Path)

**Risk**: High

**Skills**: C#, Dependency Injection, ASP.NET Identity

**Body**:

```markdown
Refactor AccountController to enable unit testing by replacing direct OWIN context access with constructor injection.

**Why**: AccountController currently uses `HttpContext.GetOwinContext()` directly, which breaks in test environments. This refactor enables Issue #11 (unit tests).

## Tasks

- [ ] Add constructor accepting UserManager and SignInManager dependencies
- [ ] Remove direct instantiation via `HttpContext.GetOwinContext()`
- [ ] Update all action methods to use injected dependencies
- [ ] Test authentication flows manually (register, login, logout, password reset)

## Acceptance Criteria

- [ ] AccountController uses constructor injection
- [ ] All authentication functionality works identically
- [ ] Manual testing confirms: registration, login, logout all functional
- [ ] Ready for unit test implementation (Issue #11)

## Definition of Done

- [ ] Feature branch merged to main
- [ ] Manual testing completed
- [ ] Integration tests still passing

## Definition of Done

- [ ] Manual testing completed successfully
- [ ] Integration tests still passing
- [ ] PR merged to main (self-reviewed checklist complete)

## Notes

- CRITICAL: Phase 1's first code change. Must preserve 100% existing behavior.
```

## --- END ISSUE #8 ---

------

------

## Issue #9: Implement constructor injection in CheckoutController

**Title**: Implement constructor injection in CheckoutController

**Milestone**: Phase 1

**Labels**: `phase-1`, `refactoring`, `code-change`, `testability`

**Priority**: P1 (Critical Path)

**Risk**: High

**Skills**: C#, Dependency Injection, EF6

**Dependencies**: Blocked by #8

**Body**:

```markdown
Refactor CheckoutController to enable unit testing by replacing direct context instantiation with constructor injection.

**Why**: CheckoutController uses `new MusicStoreEntities()` directly, making it untestable. This refactor enables Issue #12 (unit tests).

## Tasks

- [ ] Add constructor accepting DbContext and UserManager dependencies
- [ ] Remove direct instantiation of `MusicStoreEntities()`
- [ ] Update all action methods to use injected dependencies
- [ ] Test order placement workflow end-to-end
- [ ] Verify promo code validation still works

## Acceptance Criteria

- [ ] CheckoutController uses constructor injection
- [ ] Checkout workflow functions identically
- [ ] Manual testing confirms order data persists correctly
- [ ] Ready for unit test implementation (Issue #12)

## Definition of Done

- [ ] Manual testing completed successfully
- [ ] Integration tests still passing (Issues #3, #4)
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #9 ---

------

------

## Issue #10: Implement constructor injection in StoreManagerController

**Title**: Implement constructor injection in StoreManagerController

**Milestone**: Phase 1

**Labels**: `phase-1`, `refactoring`, `code-change`, `testability`

**Priority**: P1 (Critical Path)

**Risk**: Medium

**Skills**: C#, Dependency Injection, EF6

**Dependencies**: Blocked by #8

**Body**:

```markdown
Refactor StoreManagerController to enable unit testing by replacing direct context instantiation with constructor injection.

**Why**: StoreManagerController uses `new MusicStoreEntities()` directly, making it untestable. This refactor enables Issue #13 (unit tests).

## Tasks

- [ ] Add constructor accepting DbContext dependency
- [ ] Remove direct instantiation of `MusicStoreEntities()`
- [ ] Update all CRUD action methods to use injected dependency
- [ ] Test admin workflows manually (create, edit, delete albums)
- [ ] Verify authorization still works (admin role required)

## Acceptance Criteria

- [ ] StoreManagerController uses constructor injection
- [ ] All CRUD operations work identically
- [ ] Manual testing confirms operations persist to database
- [ ] Authorization enforcement verified (admin access only)
- [ ] Ready for unit test implementation (Issue #13)

## Definition of Done

- [ ] Manual testing completed successfully
- [ ] Integration tests still passing (Issue #5)
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #10 ---

------

------

## Issue #11: Unit tests for AccountController

**Title**: Unit tests for AccountController

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `unit-tests`, `testability`

**Priority**: P2 (Important)

**Risk**: Low

**Skills**: Testing, Mocking, C#

**Dependencies**: Blocked by #8

**Body**:

```markdown
Create unit tests for AccountController business logic using mocked dependencies.

## Tasks

- [ ] Create `AccountControllerTests.cs` in `/src/MvcMusicStore.Tests/Unit`
- [ ] Mock UserManager and SignInManager (using Moq or NSubstitute)
- [ ] Test key scenarios: register valid, register duplicate, login valid, login invalid, logout

## Acceptance Criteria

- [ ] Unit tests run in isolation without database
- [ ] Tests focus on controller logic, not framework behavior
- [ ] All tests pass with 100% success rate
- [ ] Mocking strategy documented in `/docs/testing/unit-tests.md`

## Definition of Done

- [ ] Tests passing locally via `dotnet test`
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #11 ---

------

------

## Issue #12: Unit tests for CheckoutController

**Title**: Unit tests for CheckoutController

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `unit-tests`, `testability`

**Priority**: P2 (Important)

**Risk**: Low

**Skills**: Testing, Mocking, C#

**Dependencies**: Blocked by #9

**Body**:

```markdown
Create unit tests for CheckoutController business logic focusing on order creation and validation.

## Tasks

- [ ] Create `CheckoutControllerTests.cs` in `/src/MvcMusicStore.Tests/Unit`
- [ ] Mock DbContext and UserManager
- [ ] Test order creation, promo code validation, calculations
- [ ] Test edge cases (empty cart, invalid promo codes)

## Acceptance Criteria

- [ ] Unit tests run without database dependencies
- [ ] Order creation, promo validation, calculations tested
- [ ] Edge cases covered
- [ ] All tests pass with 100% success rate

## Definition of Done

- [ ] Tests passing locally via `dotnet test`
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #12 ---

------

------

## Issue #13: Unit tests for StoreManagerController

**Title**: Unit tests for StoreManagerController

**Milestone**: Phase 1

**Labels**: `phase-1`, `testing`, `unit-tests`, `testability`

**Priority**: P2 (Important)

**Risk**: Low

**Skills**: Testing, Mocking, C#

**Dependencies**: Blocked by #10

**Body**:

```markdown
Create unit tests for StoreManagerController CRUD logic and authorization.

## Tasks

- [ ] Create `StoreManagerControllerTests.cs` in `/src/MvcMusicStore.Tests/Unit`
- [ ] Mock DbContext
- [ ] Test CRUD operations (Create, Read, Update, Delete)
- [ ] Test authorization checks (admin role requirement)

## Acceptance Criteria

- [ ] Unit tests run without database dependencies
- [ ] CRUD operations and authorization tested
- [ ] All tests pass with 100% success rate

## Definition of Done

- [ ] Tests passing locally via `dotnet test`
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #13 ---

------

------

## Issue #14: Create data integrity validation script

**Title**: Create data integrity validation script

**Milestone**: Phase 1

**Labels**: `phase-1`, `baseline`, `validation`, `database`

**Priority**: P1 (Critical Path)

**Risk**: Low

**Skills**: SQL, Database

**Body**:

```markdown
Create SQL script to validate database integrity before and after migrations.

## Tasks

- [ ] Create `/scripts/validate-data-integrity.sql` with validation queries
- [ ] Add queries: row counts, FK integrity checks, critical data verification, checksums
- [ ] Execute script against current LocalDB
- [ ] Save results to `/docs/baselines/phase1-data-integrity.md`

## Acceptance Criteria

- [ ] Script runs without errors on current database
- [ ] Output includes: row counts for all tables, FK integrity, admin user verification
- [ ] Baseline results documented with expected values
- [ ] Script is repeatable (same state produces same output)

## Definition of Done

- [ ] Script tested and results documented
- [ ] PR merged to main (self-reviewed checklist complete)
```

------

## --- END ISSUE #14 ---

------

------

## Issue #15: Seed realistic test data for baselines

**Title**: Seed realistic test data for baselines

**Milestone**: Phase 1

**Labels**: `phase-1`, `data`, `baseline`, `foundation`

**Priority**: P1 (Critical Path)

**Risk**: Low

**Skills**: Database, EF6, Data Seeding

**Body**:

```markdown

Seed database with realistic data volumes to enable meaningful performance baselines and migration validation.

**Why**: Performance baselines (Issue #16) need realistic data volumes to be meaningful. Testing with 10 albums won't reveal N+1 query problems that appear with 400+. Additionally, Phase 4 migration validation needs existing data to verify that data integrity survives the .NET 9 platform change.

## Tasks

- [ ] Review `/docs/decisions/ADR-001-test-data-strategy.md` for requirements
- [ ] Use existing seed data from original MVC Music Store tutorial:
  - 462 albums (catalog data from original tutorial)
  - 303 artists (catalog data from original tutorial)
  - 15 genres (catalog data from original tutorial)
- [ ] Add supplemental test data not in original seed:
  - 10 test users including admin@musicstore.com (for auth testing)
  - 20 historical orders with line items (for migration validation)
- [ ] Implement via EF6 database initializer or SQL script
- [ ] Document seeding process in `/docs/testing/test-data.md`
- [ ] Execute data integrity validation script against current LocalDB

## Acceptance Criteria

- [ ] Database contains catalog data matching ADR-001 volumes
- [ ] Admin account functional (manual login test passed)
- [ ] Data integrity validated: All foreign key references valid
- [ ] Run this query returns 0: `SELECT COUNT(*) FROM Albums WHERE GenreId NOT IN (SELECT GenreId FROM Genres)`
- [ ] Seeding process documented with reproduction steps
- [ ] Data integrity validation script passes

## Definition of Done

- [ ] All acceptance criteria met
- [ ] Seeding process documented in `/docs/testing/test-data.md`
- [ ] PR merged to main (self-reviewed checklist complete)

## Notes

- Original 462 albums provide sufficient volume to surface performance issues
- Historical orders added beyond tutorial seed to test data migration (Phase 4)
```

------

## --- END ISSUE #15 ---

------

------

## Issue #16: Capture performance baselines

**Title**: Capture performance baselines

**Milestone**: Phase 1

**Labels**: `phase-1`, `baseline`, `performance`, `critical-path`, `validation`

**Priority**: P0 (Blocking)

**Risk**: Medium

**Skills**: Performance Testing, JMeter, Application Insights

**Dependencies**: Blocked by #15, Blocked by #18

**Body**:

```markdown
Capture performance baselines using JMeter and Application Insights to establish "no regression" proof for migration.

**Why JMeter + App Insights**: Provides consistent, reproducible timing without browser overhead. Industry standard for performance baselines.

## Tasks

### Setup
- [ ] Verify Issue #15: Realistic test data seeded (462 albums, 303 artists)
- [ ] Verify Issue #18: App Insights telemetry flowing to Azure
- [ ] Download Apache JMeter (latest stable)

### Create JMeter Test Plan
- [ ] Create `/scripts/phase1-baseline-load.jmx`
- [ ] Configure 8 HTTP requests (Home, Store Index, Store Browse, Store Details, Cart, Checkout, Admin, Login)
- [ ] Set parameters: 1 thread, 5 iterations per page, 2-second think time
- [ ] Add HTTP Cookie Manager and authentication steps

### Execute and Capture
- [ ] Run JMeter test plan against local application
- [ ] Wait 5 minutes for telemetry in App Insights
- [ ] Export from Azure Portal: min/avg/max/p95 times for all 8 pages
- [ ] Export top 10 slowest SQL queries from Dependencies view

### Document
- [ ] Create `/docs/baselines/phase1-performance.md` with results
- [ ] Include: date, data volume stats, JMeter version, reproduction steps
- [ ] Save test plan to `/scripts/phase1-baseline-load.jmx`

## Acceptance Criteria

- [ ] JMeter executes with 100% success rate (40/40 requests)
- [ ] Baseline includes min/avg/max/p95 for all 8 pages from App Insights
- [ ] Top 10 SQL queries documented with execution times
- [ ] Results committed at git tag `phase1-baseline-complete`
- [ ] Reproducible: Another developer could follow docs and get similar results

## Definition of Done

- [ ] Baseline document complete in `/docs/baselines/phase1-performance.md`
- [ ] Test plan committed to `/scripts/`
- [ ] Git tag `phase1-baseline-complete` created
- [ ] PR merged to main (self-reviewed checklist complete)

## Notes

- These baselines are your "no regression" proof point
```

------

## --- END ISSUE #16 ---

------

------

## Issue #17: Create database backup

**Title**: Create database backup

**Milestone**: Phase 1

**Labels**: `phase-1`, `backup`, `safety`, `database`, `foundation`

**Priority**: P2 (Important)

**Risk**: Low

**Skills**: Database, SQL Server

**Body**:

```markdown
Create validated backup of LocalDB databases before migration work begins.

## Tasks

- [ ] Create `/backups` directory (add to .gitignore)
- [ ] Backup MvcMusicStoreV2 database
- [ ] Backup MvcMusicStoreUsersV2 database
- [ ] Generate MD5 checksums of backup files
- [ ] Test restore both backups to temporary databases
- [ ] Document procedure in `/docs/operations/database-backup.md`

## Acceptance Criteria

- [ ] Both backup files created successfully
- [ ] Backup sizes reasonable (>1MB confirms data included)
- [ ] MD5 checksums captured in `/docs/baselines/phase1-backup-checksums.txt`
- [ ] Both backups successfully restored (validation passed)

## Definition of Done

- [ ] Backups created and validated
- [ ] Restore tested successfully
- [ ] Documentation complete
```

------

## --- END ISSUE #17 ---

------

------

## Issue #18: Instrument Application Insights

**Title**: Instrument Application Insights

**Milestone**: Phase 1

**Labels**: `phase-1`, `observability`, `telemetry`, `critical-path`, `foundation`

**Priority**: P1 (Critical Path)

**Risk**: Medium

**Skills**: Azure, Application Insights, Telemetry

**Dependencies**: None (but BLOCKS #16)

**Body**:

```markdown
Integrate Application Insights SDK to enable automated telemetry collection for baseline establishment.

**Why**: Issue #16 cannot capture baselines until App Insights is collecting telemetry. App Insights automatically records performance data when the application runs.

**Captures**: Page response times, SQL query durations, exceptions, custom events

**Security Note**: Instrumentation key must be set via environment variable, 
not committed to git. Standard practice for all Azure secrets.

## Tasks

- [ ] Install `Microsoft.ApplicationInsights.Web` NuGet package (Framework 4.8 compatible)
- [ ] Create Application Insights resource in Azure Portal (Free tier)
- [ ] Configure instrumentation key via environment variable (NOT Web.config)
- [ ] Add config key to Web.config with environment variable reference
- [ ] Value set to: "SET_VIA_ENVIRONMENT_VARIABLE"
- [ ] Implement env var reading in Global.asax.cs Application_Start()
- [ ] Test telemetry flow: Browse pages, verify in Azure Portal within 2-3 minutes
- [ ] Verify SQL dependency tracking working
- [ ] Document access in `/docs/operations/application-insights-access.md`

## Acceptance Criteria

- [ ] Telemetry visible in Azure Portal (Live Metrics and Performance views)
- [ ] SQL dependency tracking working (queries visible in Dependencies)
- [ ] Instrumentation key NOT in git (environment variable verified)
- [ ] Can query last hour of telemetry in Portal
- [ ] Ready for Issue #16 baseline capture

## Definition of Done

- [ ] Telemetry verified in Azure Portal
- [ ] No secrets in git (verified via git log)
- [ ] PR merged to main (self-reviewed checklist complete)

## Notes

- BLOCKS Issue #16 - performance baselines cannot be captured without this
```

------

## --- END ISSUE #18 ---

------

------

## Issue #19: Phase 1 exit criteria validation

**Title**: Phase 1 exit criteria validation

**Milestone**: Phase 1

**Labels**: `phase-1`, `validation`, `documentation`, `milestone`

**Priority**: P0 (Blocking)

**Risk**: Low

**Skills**: Documentation, Validation

**Dependencies**: Blocked by ALL previous Phase 1 issues (#1-#18)

**Body**:

```markdown
Validate all Phase 1 exit criteria met before proceeding to Phase 2.

## Tasks

- [ ] Verify all Phase 1 issues closed (view milestone progress in GitHub)
- [ ] Create `/docs/phase1-exit-criteria.md` document
- [ ] Run full test suite: `dotnet test` (expect 100% pass rate)
- [ ] Verify baselines captured: smoke tests, data integrity, performance
- [ ] Verify backups: created, checksums recorded, restore tested
- [ ] Verify observability: App Insights telemetry visible in Azure
- [ ] Document any deviations from plan with rationale
- [ ] Create git tag: `phase1-complete`

## Acceptance Criteria

- [ ] Exit criteria document includes validation evidence for all items
- [ ] All items complete OR exceptions documented with rationale
- [ ] Git tag `phase1-complete` created
- [ ] Ready to proceed to Phase 2

## Definition of Done

- [ ] Exit criteria document complete
- [ ] Git tag created
- [ ] Phase 1 declared complete
```

------

## --- END ISSUE #19 ---

------

------

## Data Entry Complete

**Total Issues Entered**: 19
 **Milestone**: Phase 1 - Testing & Observability

**Next Steps**:

1. Add all issues to "Phase 1" milestone
2. Apply strategic labels:
   - `foundation` → Issues #1, #2, #6, #15, #17, #18
   - `critical-path` → Issues #3, #4, #7, #16, #18
   - `testability` → Issues #8-13
   - `validation` → Issues #14, #16, #19
3. Set up dependency links using GitHub's linking feature
4. Assign issues to yourself
5. Create project board and add issues
6. Begin work following roadmap sequence

------

**Document Version**: 3.1
 **Last Updated**: November 21, 2025

**Changes from v3.0**:

- Removed all "in CI" references from Definition of Done sections
- Changed to "Tests passing locally via `dotnet test`" for Phase 1 reality
- Updated DoD for refactoring issues to focus on manual testing + integration test validation
- Updated DoD for documentation/validation issues to appropriate completion criteria
- Removed "CI-ready configuration" language from Issue #2 (CI/CD is Phase 2 work)
- Made all DoD sections reflect Phase 1 reality: local testing discipline before CI automation

**Changes from v2.0**:

- Fixed priority misalignments (#1: P2→P1, #6: P2→P1)
- Trimmed issue bodies by 40-50% (removed verbose sections)
- Removed all implementation guide references
- Tailored Definition of Done to task types
- Added strategic label recommendations (foundation, critical-path, testability, validation)
- Added "Why" context to refactoring issues (#8-10)
- Simplified Issue #19 prerequisite validation
- Removed redundant task/acceptance criteria items
- Made all issues scan-friendly for hiring managers (15-25 lines typical)