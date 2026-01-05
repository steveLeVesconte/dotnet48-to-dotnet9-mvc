# Seed CLI Specs (Improved v2.3)

> Purpose: Create a **deterministic, migration-validation dataset** (users + carts + orders) in an existing legacy Music Store database pair (Identity DB + MusicStore DB), so you can run the **data integrity validation** queries before/after risky migration steps.

This v2 spec intentionally:
- **Requires databases and schema to already exist** (data-only tool).
- **Does NOT seed catalog** (Albums/Artists/Genres). Catalog must already exist.
- Uses **defensible transactional behavior** (per-database transactions).
- Adds a **hard, non-overridable safety gate** to prevent destructive use on production systems.
- Emphasizes **rich, step-oriented console logging** for debugging.

---

## 1. Scope

### In scope
- Seed **Identity DB**:
  - Ensure `Administrator` user exists (and `admin@musicstore.com`).
  - Ensure `test01` .. `test10` users exist.
  - Ensure required roles exist (if applicable).
- Seed **MusicStore DB**:
  - Create **20 Orders** + corresponding **OrderDetails**.
  - Create **8 CartIds**:
    - 4 GUID-like cart IDs (anonymous sessions)
    - 4 username-based cart IDs (associated with known seeded users)

### Out of scope
- Creating databases
- Creating schema / migrations
- Seeding catalog (Albums/Artists/Genres)
- Any production data migration or transformation logic

---

## 2. Preconditions (Fail Fast)

The CLI MUST fail fast (no changes) unless all preconditions below are satisfied.

### 2.1 Both databases exist and are reachable
- Identity DB connection succeeds.
- MusicStore DB connection succeeds.

### 2.2 Required schema exists
- Identity DB contains required tables for ASP.NET Identity (or the equivalent in your legacy setup).
- MusicStore DB contains: `Albums`, `Orders`, `OrderDetails`, `Carts` (or whatever your schema names are).

### 2.3 Catalog must already exist (no catalog seeding)
- MusicStore DB MUST have catalog rows present:
  - `Albums` > 0
  - `Artists` > 0
  - `Genres` > 0
- If any are empty/missing:
  - Exit with a clear error:
    - “Catalog is missing. This tool does not seed catalog. Restore a baseline DB or run the legacy initializer first.”


### 2.4 Freshness requirement for target tables (re-run prevention)
- MusicStore DB MUST have **empty** target tables before seeding:
  - `Orders` = 0
  - `OrderDetails` = 0 (recommended check)
  - `Carts` = 0
- If any of these tables contain rows, the CLI must fail fast and instruct the operator to restore/prepare a fresh baseline DB pair.

---

## 3. Hard Safety Gate (Non-Prod Marker)

**No override is allowed.**  
If the safety gate is not satisfied, the tool MUST refuse to run.

### 3.1 Marker table

The safety marker MUST exist in **each** target database the tool will touch:
- **Identity DB**
- **MusicStore DB**

Each target database MUST contain the marker table:

`dbo.__EnvironmentMarker`

**Operator-managed:** The Seed CLI MUST NOT create or modify this table. If it is missing, the CLI must fail.


Recommended schema:

| Column | Type | Notes |
|---|---:|---|
| MarkerId | int identity | PK |
| EnvironmentName | nvarchar(50) | Required. Must be `NONPROD` |
| AllowSeedCli | bit | Required. Must be `1` |
| ProjectSeedToken | nvarchar(200) | Required. Must match expected token |
| CreatedUtc | datetime2 | Audit |
| CreatedBy | nvarchar(200) | Audit |
| Notes | nvarchar(400) | Optional |

### 3.2 Required values
The CLI MUST check **both** DBs for a row satisfying:

- `EnvironmentName = 'NONPROD'`
- `AllowSeedCli = 1`
- `ProjectSeedToken = <ExpectedToken>`

Where `<ExpectedToken>` is a value shipped with the tool (e.g. config), such as:

- `dotnet48-to-dotnet9-mvc/validation-dataset`

### 3.4 Row cardinality and uniqueness

Expected cardinality is **one active marker row per database** for this tool.

Implementation guidance:
- The CLI may treat the gate as satisfied if it finds **at least one** row matching all required values.
- Recommended (optional) hardening: add a UNIQUE constraint to prevent duplicates, e.g. unique on `ProjectSeedToken` (or on `(EnvironmentName, ProjectSeedToken)`), so the marker behaves like a single authoritative row.

### 3.3 No override policy
- The CLI MUST NOT provide any `--force`, `--i-know-what-im-doing`, or equivalent escape hatch.
- If any DB fails the marker check, the CLI must:
  - log the failure
  - exit non-zero
  - make **zero changes** to either DB

### 3.4 Optional: additional safety heuristics (non-authoritative)
These may log warnings (but are **not** substitutes for the marker):
- If the server name appears to be an Azure SQL production host pattern
- If the database name contains `prod`, `production`, etc.
- If the login appears to be a production credential

Marker gate is the only authoritative safety check.

---

## 4. Run-once Semantics and Freshness Gates

Idempotency is **not** a goal for this tool. The intended operational model is:
- The legacy/baseline catalog seed process runs first (outside this tool).
- This Seed CLI then runs **once** against a freshly prepared DB pair.
- The CLI must **fail fast** if it detects signs of a prior run or a non-fresh target state.

### 4.1 Catalog presence checks (must already be seeded)

The CLI MUST verify the catalog exists before any writes:
- `Albums` row count > 0
- `Artists` row count > 0
- `Genres` row count > 0

If any are missing/empty, the CLI must exit with a clear error (no changes).

### 4.2 Re-run prevention via empty-table gates

To prevent accidental re-runs (and duplicate data), the CLI MUST fail fast if any of the following tables already contain rows:
- `Orders` row count > 0
- `OrderDetails` row count > 0 (recommended)
- `Carts` row count > 0

Rationale: This tool is meant to create a validation dataset once per freshly seeded baseline. If the target tables are not empty, the operator should restore/prepare a fresh baseline DB pair.

## 5. Transaction Strategy (Defensible)

### 5.1 Per-database transactions
- Seeding operations for **each database** are performed in a transaction for that database.
- Identity DB changes are committed/rolled back independently from MusicStore DB.

Rationale: There is no safe cross-DB atomic transaction in the general case, especially across servers/Azure SQL. This approach still provides strong consistency within each DB and good failure diagnostics.

### 5.2 Ordering
1. Validate both DB connections
2. Validate schema
3. Validate environment markers (both DBs)
4. Validate catalog exists (Albums > 0)
5. Seed Identity DB (transaction)
6. Seed MusicStore DB (transaction)

If Identity seeding fails, MusicStore seeding MUST NOT run.

---

## 6. Stable Key Lookups (No “hard-coded AlbumId assumptions”)

The tool MUST NOT assume `AlbumId` values.

Instead:
- Select albums using stable keys such as:
  - `Title` + `Artist.Name` (or equivalent)
  - or another stable unique attribute available in your dataset

Example approach:
- Query a known set of album titles and fetch their current IDs
- Fail fast if the required album set is not found

---

## 7. Logging Requirements (Console-First, Debug-Friendly)

Logging may be simple `Console.WriteLine`, but must be **structured and step-oriented**.

### 7.1 Start banner
Log:
- Tool name + version
- UTC start time
- Expected project token
- Sanitized connection targets (server + db name; no passwords)

### 7.2 Step logging format
Each major step logs:
- Step number / total (or step name)
- Target DB (`Identity` / `MusicStore`)
- Action
- Result (OK/FAIL/SKIP)
- Elapsed time

Example:

- `[STEP 3/9] DB=Identity ACTION=ValidateEnvironmentMarker RESULT=OK elapsedMs=12`
- `[STEP 6/9] DB=MusicStore ACTION=InsertOrders count=20 RESULT=OK elapsedMs=44`

### 7.3 Error logging
On failure, log:
- Step name
- Exception type + message
- Inner exception message(s) if present
- The last known successful step
- Any relevant IDs/keys (usernames, cart IDs, stable album keys) involved in the failed step

Never log secrets (passwords, connection strings with credentials).

### 7.4 Summary
At end, log:
- totals inserted/ensured
- whether run was NO-OP vs ADD-MISSING
- the seed marker values written
- total elapsed time

---

## 8. Dataset Definition (Concrete Targets)

### 8.1 Identity DB

- Administrator: **pre-existing (seeded by legacy SampleData.cs)**
  - The Seed CLI MUST NOT create or modify this account
  - Username: `Administrator`
  - Email: `admin@musicstore.com`

- Test users:
  - `test01` .. `test10`
  - Emails: `test01@musicstore.local` .. `test10@musicstore.local` (or similar)
- Passwords:
  - Use a known dev-only password from config/env var
  - Do not hardcode in source control

### 8.2 MusicStore DB
- Orders: 20
- OrderDetails:
  - Each order has 1..N details (define exact number; recommended deterministic, e.g., 3 lines per order)
- Carts: 8
  - 4 anonymous cart IDs (GUID-like deterministic strings)
  - 4 username cart IDs mapped to known users

### 8.3 Album selection for OrderDetails
- Define a small, stable set of album titles to use for order lines (e.g., 5–10 titles).
- Tool resolves their IDs at runtime via stable lookup.
- If not found, fail with clear message.

---

## 9. CLI Interface

### 9.1 Commands
Recommended:
- `seed` (default)
- `verify` (checks preconditions + marker gates + catalog existence; no writes)

### 9.2 Options
- `--identity-connection "<name or connection string>"`
- `--musicstore-connection "<name or connection string>"`
- `--verbose` (more step detail)
- `--commit-sha "<sha>"` (optional metadata for markers)

**No destructive override flags.**

---

## 10. Exit Codes

- `0`: success (including NO-OP)
- `1`: precondition failure (schema/catalog missing, connectivity)
- `2`: environment marker gate failure (treated distinctly and loudly)
- `3`: seeding failure (exception during writes)

---

## 11. Acceptance Criteria

- Tool refuses to run if either DB lacks a valid `dbo.__EnvironmentMarker` row.
- Tool refuses to run if catalog is missing/empty.
- Tool does not assume fixed `AlbumId` values.
- Tool fails fast if it detects a prior run (Orders/OrderDetails/Carts already contain rows).
- Tool uses per-database transactions.
- Logging makes it obvious:
  - what step it reached
  - what DB it was operating on
  - what it inserted/ensured
  - why it failed if it failed

---

## 12. Operator Setup (Manual, One-Time)

Before running the tool, an operator must add the non-prod marker rows to **BOTH** databases:

- Identity DB
- MusicStore DB

This step is intentionally manual and explicit to prevent accidental production usage. The Seed CLI MUST NOT create the marker table.

### 12.1 Recommended repo location for setup script

Add the following script to the repository at:

- `/scripts/create-environment-marker.sql`

### 12.2 Example T-SQL (create table + insert marker row)

> Replace the token value with the same `<ExpectedToken>` configured for the Seed CLI (e.g. `dotnet48-to-dotnet9-mvc/validation-dataset`).

```sql
/* create-environment-marker.sql
   Run this script in EACH target database (Identity DB and MusicStore DB).
*/

IF OBJECT_ID('dbo.__EnvironmentMarker', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.__EnvironmentMarker
    (
        MarkerId         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK___EnvironmentMarker PRIMARY KEY,
        EnvironmentName  NVARCHAR(50) NOT NULL,
        AllowSeedCli     BIT NOT NULL,
        ProjectSeedToken NVARCHAR(200) NOT NULL,
        CreatedUtc       DATETIME2(0) NOT NULL CONSTRAINT DF___EnvironmentMarker_CreatedUtc DEFAULT (SYSUTCDATETIME()),
        CreatedBy        NVARCHAR(200) NOT NULL,
        Notes            NVARCHAR(400) NULL
    );

    /* Optional hardening: prevent duplicates for this tool */
    CREATE UNIQUE INDEX UX___EnvironmentMarker_ProjectSeedToken
        ON dbo.__EnvironmentMarker(ProjectSeedToken);
END
GO

DECLARE @ExpectedToken NVARCHAR(200) = N'dotnet48-to-dotnet9-mvc/validation-dataset';
DECLARE @CreatedBy     NVARCHAR(200) = SUSER_SNAME();
DECLARE @Notes         NVARCHAR(400) = N'Explicit NONPROD gate for Seed CLI (manual operator action).';

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.__EnvironmentMarker
    WHERE EnvironmentName  = N'NONPROD'
      AND AllowSeedCli     = 1
      AND ProjectSeedToken = @ExpectedToken
)
BEGIN
    INSERT INTO dbo.__EnvironmentMarker(EnvironmentName, AllowSeedCli, ProjectSeedToken, CreatedBy, Notes)
    VALUES (N'NONPROD', 1, @ExpectedToken, @CreatedBy, @Notes);
END
GO
```

### 12.3 Operational notes

- Run the script once per DB. The `IF NOT EXISTS` keeps it safe to re-run.
- If the Seed CLI reports “marker gate failure,” fix the marker row(s) first. Do not add overrides to the tool.
