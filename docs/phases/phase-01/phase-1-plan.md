# Phase 1: Foundation & Testing - Work Plan

**Goal**: Create a safety net and establish baselines before making any migration changes.

**Strategy**: Measure current state first, then build testing infrastructure, then add instrumentation. This sequence ensures baseline measurements capture the unmodified application.

------

## Task Groups

### Task Group 1: Baseline Current State

Document existing behavior and quirks to detect regressions during migration.

**Deliverables**:

1. **Manual smoke tests** - Document current behavior, bugs, and quirks
   - Test each controller action manually (13 test scenarios)
   - Document existing issues (not to be "fixed" during migration)
   - Output: `/docs/Smoke-Test-Procedure.md`, `/docs/baselines/phase1-smoke-test-results-2025-11-28.md`
3. **Performance baselines** - Capture current performance metrics
   - Use browser DevTools Network tab
   - Measure 3 key pages: Homepage (`/`), Album Detail (`/Store/Details/1`), Checkout (`/Checkout/AddressAndPayment`)
   - 5 runs per page, record averages
   - Output: `/docs/baselines/phase1-performance.csv`
4. **Security scan baseline** - Establish security posture
   - Enable GitHub Dependabot (Settings → Security)
   - Add CodeQL workflow (use GitHub's template)
   - Run both, screenshot results
   - Document known issues and migration plan
   - Output: `/docs/baselines/phase1-security.md`
5. **Database backup & restore (optional)** - Practice Azure migration skills
   - Take LocalDB backup: `BACKUP DATABASE MvcMusicStoreV2 TO DISK='C:\backups\phase1-baseline.bak'`
   - Test restore: `RESTORE DATABASE MvcMusicStoreV2_Test FROM DISK='C:\backups\phase1-baseline.bak'`
   - Document commands in `/docs/recovery-plan.md`
   - **Why**: Phase 2 Azure SQL migration uses same skills (BACPAC/backup)
   - **Skip if**: Time-constrained (database fully seeded via code, no backup needed for recovery)
6. **Data integrity validation script** - SQL script for migration validation
   - Record counts for all tables
   - Checksums for critical tables (Albums, Orders, Users)
   - Validation queries to run before/after migrations
   - Output: `/scripts/validate-data-integrity.sql`

**Success Criteria**:

- Baseline behavior documented (13 test scenarios)
- Known limitations preserved (not fixed):
  - Console warnings documented but not resolved
  - Visual quirks captured but not improved
  - Missing features noted but not added
- Performance metrics captured
- Security scan results documented
- Data integrity validation script created

------

### Task Group 2: Build Testing Infrastructure

Create automated tests to validate all future migration phases.

**Dependencies**: Task Group 1 must be complete (baseline documentation provides test scenarios)

**Deliverables**:

1. **Create test projects** - Set up testing infrastructure

   - Create xUnit test project: `/src/MvcMusicStore.Tests`
   - Add project reference to MvcMusicStore
   - Install required packages:
     - xUnit (test framework)
     - xUnit.runner.visualstudio (test runner)
     - Microsoft.NET.Test.Sdk (test SDK)
     - Moq (mocking framework for unit tests)
   - Verify test project builds and can reference main project
   - Output: Test project structure in place

2. **Constructor injection refactor** - Make controllers testable

   - Refactor controllers to use constructor injection for DbContext
   - Manual verification after each controller (5 min per controller)
   - This enables test database injection for integration tests

3. ********Seed migration validation data******** - Data to verify Azure/EF migrations

   **Purpose**: Populate all tables so Phase 2/4 migrations can be validated

   **Seed data additions** (to main application database):

   - 10 test users (provides data for AspNetUsers, AspNetUserRoles tables)
   - 20 historical orders with line items (provides data for Orders, OrderDetails)
   - Catalog data already exists: 462 albums, 303 artists, 15 genres

   **NOT used by integration tests** - Integration tests use isolated DatabaseFixture with minimal data (5 albums, 3 genres, 3 artists)

   **Validation approach**:

   - Data integrity script captures checksums for all tables
   - Phase 2: Compare Azure SQL checksums to Phase 1 baseline
   - Phase 4: Compare EF Core checksums to Phase 3 baseline

   Document expected counts and checksums:

   - Output: `/docs/baselines/phase1-data-manifest.md`

4. **Data integrity validation - Baseline #1**

   - Execute SQL script to capture initial state after seeding
   - Record counts for all tables (Albums, Artists, Genres, Users, Orders, OrderDetails, CartItems)
   - Calculate checksums for critical tables (Albums, Orders, Users using CHECKSUM_AGG)
   - Save baseline output: `/docs/baselines/phase1-data-validation-1-after-seed.txt`
   - This becomes source of truth for "what should exist"

5. **Test database strategy** - Document integration test approach

   - DatabaseFixture creates isolated LocalDB per test run (unique GUID in database name)
   - Minimal seed data per test (5 albums, 3 genres, 3 artists) - independent from main app seed data
   - Fresh DbContext per test to avoid change tracker conflicts
   - Tests clean up after themselves (database deleted in Dispose)
   - Output: `/docs/testing-strategy.md`

6. **Integration test suite** - Test critical user workflows

   - **Target**: 10 scenarios (6 controller-level + 4 data-level)

   **Controller-level integration tests** (test full request pipeline):

   - Guest: Browse → Add album to cart → View cart (verify cart count increases)
   - User: Login → Add to cart → Checkout with FREE code → Order created
   - Admin: Login → Create album → Verify appears in store
   - Admin: Login → Edit album → Verify changes reflected
   - Edge: Checkout with invalid promo code → Error message
   - Edge: Checkout with empty cart → Validation error

   **Data-level integration tests** (test business logic with real database):

   - `ShoppingCart.CreateOrder()` with items → OrderDetails created correctly
   - `ShoppingCart.MigrateCart()` → Anonymous cart merged with user cart
   - `ShoppingCart.GetTotal()` → Correct sum calculation with multiple items
   - `ShoppingCart.EmptyCart()` → All cart items removed from database

   **Requirements**:

   - All tests must be independent (no shared state between tests)
   - Target: <30 seconds total runtime
   - Output: Integration test suite passing in local environment

7. **Data integrity validation - Baseline #2**

   - Re-run the same SQL validation script after integration tests complete
   - Compare output to Baseline #1 (should match exactly)
   - If mismatch detected: Investigate test cleanup issues (tests should not leave data behind)
   - Save output: `/docs/baselines/phase1-data-validation-2-after-tests.txt`
   - This verifies: Tests are properly isolated and don't corrupt the main application database

8. **Unit test coverage** - Test business logic in isolation

   - **Target**: 60-70% line coverage of each target class (not global average)

   **Target classes for unit testing**:

   - `ShoppingCart`: GetCart, AddToCart, RemoveFromCart, EmptyCart, GetTotal, CreateOrder
   - `Order`: Create method, validation logic
   - `AccountController.MigrateShoppingCart`: Cart migration logic
   - Any extracted validation classes (e.g., PromoCodeValidator if extracted)

   **Testing approach**:

   - Use mocks/fakes for DbContext dependencies (no real database for unit tests)
   - Focus on business logic and calculations
   - Test edge cases and validation rules

   **How to measure coverage**:

   ```bash
   # Run tests with coverage collection
   dotnet test --collect:"XPlat Code Coverage"
   
   # Generate HTML coverage report
   dotnet tool install -g dotnet-reportgenerator-globaltool
   reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report
   
   # Open coverage-report/index.html and verify per-class coverage
   ```

   **Acceptable uncovered lines**:

   - Null checks that can't happen due to framework constraints
   - Exception handlers for impossible states (e.g., "cart total < 0" defensive check)
   - Logging statements
   - Defensive programming guards

   **Output**: Unit test suite with code coverage report

**Success Criteria**:

- Test project created and verified
- Constructor injection complete (all controllers)
- Test data seeded and documented
- Data validation Baseline #1 captured (after seed)
- Integration tests: 10 scenarios passing (6 controller-level + 4 data-level)
- Data validation Baseline #2 captured and matches Baseline #1
- Unit tests: 60-70% coverage of each target class (4-5 classes)

------

### Task Group 3: Add Instrumentation & Final Validation

Add observability and finalize Phase 1 baselines.

**Dependencies**: Task Group 2 must be complete (testing infrastructure validates instrumentation works)

**Deliverables**:

1. **Application Insights (basic setup)** - Add telemetry for before/after comparison

   - Install Application Insights SDK (timebox: 30 minutes)
   - Configure connection string in configuration file
   - Log one custom event (e.g., "OrderPlaced" in CheckoutController)
   - Capture 10 minutes of baseline telemetry (manual app usage)
   - Screenshot Azure portal showing custom event received
   - Output: `/docs/baselines/phase1-appinsights.md` with screenshots

2. **Data integrity validation - Final Baseline**

   - Re-run the same SQL validation script one final time
   - Compare output to Baseline #1 and Baseline #2 (all three should match)
   - If any mismatch: Investigate and resolve before Phase 1 completion
   - Save output: `/docs/baselines/phase1-data-validation-3-final.txt`
   - This is the official "Phase 1 complete" baseline
   - **This baseline will be compared against**:
     - Phase 2: After Azure SQL migration
     - Phase 4: After EF Core migration
   - Create comparison checklist in data manifest showing expected vs actual counts

3. **Playwright smoke tests (REQUIRED)** - Automated browser testing

   - **Purpose**: Catch deployment and front-end issues in Phases 2-4

     **Scope**: 4 critical flows only

     - Homepage loads and displays albums (static files work)
     - Can add album to cart (JavaScript + session state work)
     - Cart count updates after add (AJAX/page refresh works)
     - Checkout page renders for authenticated user (auth cookies work)

     **Time budget breakdown**:

     - Setup (NuGet, browser install, first test): 45 minutes
     - 4 tests at 30 minutes each: 2 hours
     - Buffer for surprises: 15 minutes
     - Total: 3 hours maximum

     **Per-test rule**: If any single test takes >30 min to stabilize, remove it

     **Why now**:

     - Phase 2 needs these to validate Azure deployment
     - Phase 4 needs these to validate front-end migration
     - Building after Phase 1 is too late (no baseline comparison)

     **Output**: Playwright test suite passing in local environment

4. **Phase 1 completion report** - Document deliverables and readiness

   - Summary of all baselines captured (3 data validation checkpoints + performance + security)
   - Test coverage achieved (integration + unit)
   - Known issues documented
   - Phase 2 readiness checklist
   - Output: `/docs/phase1-completion.md`

**Success Criteria**:

- Data validation Final Baseline captured (matches Baseline #1 and #2)
- Application Insights configured and baseline captured
- All Phase 1 documentation complete
- Ready to begin Phase 2 (Azure migration)

------

## Phase 1 Exit Criteria

Before proceeding to Phase 2, verify all of the following are complete:

### Testing Infrastructure

- Integration tests: 10 scenarios (6 controller-level + 4 data-level), all passing
- Unit tests: 60-70% coverage of each target class (4-5 classes)
- Tests run in <30 seconds (integration) + <10 seconds (unit)
- Test database strategy documented
- **Playwright tests**: 4 critical flows, <60s runtime

### Baselines Captured

- Performance metrics (3 pages, 5 runs each)
- Security scan results (Dependabot + CodeQL)
- Database backup tested and verified (optional)
- **Data integrity validation - 3 checkpoints**:
  - Baseline #1: After seed data added
  - Baseline #2: After integration tests (verifies test isolation)
  - Final Baseline: Phase 1 complete (this is the comparison baseline for Phases 2 & 4)
- Application Insights telemetry baseline

### Documentation Complete

- Baseline behavior documented (including current bugs/quirks)
- Test data manifest with expected counts
- Recovery plan with backup/restore commands (optional)
- Phase 1 completion report
- Data validation baselines saved and compared (all 3 should match)

------

## Why This Phase Cannot Be Skipped

Phase 4 platform migration (.NET Framework 4.8 → .NET 9) breaks everything simultaneously:

- ASP.NET MVC 5 → ASP.NET Core MVC
- Entity Framework 6 → Entity Framework Core
- ASP.NET Identity 2.x → ASP.NET Core Identity
- OWIN middleware → ASP.NET Core middleware
- Web.config → appsettings.json

Without this testing foundation, there's no way to verify functionality post-migration except manual testing (brittle, time-consuming, error-prone). The integration tests built here become the regression safety net for Phase 4.

------

## Risk Assessment

**Biggest risk**: Integration test scope creep **Mitigation**: Define 10 specific scenarios upfront (listed above)

**Second risk**: Application Insights rabbit hole **Mitigation**: 30-minute timebox for basic setup

**Third risk**: Unit test coverage perfectionism **Mitigation**: Target 60-70% per class (not 100%), document acceptable uncovered lines