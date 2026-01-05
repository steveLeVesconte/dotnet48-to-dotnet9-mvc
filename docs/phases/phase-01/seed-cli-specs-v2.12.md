------

# 📄 seed-cli-spec-v2.12.md (human edit)



> Purpose: Create a **repeatable, migration-validation dataset** (users + carts + orders) in an existing legacy Music Store database pair (Identity DB + MusicStore DB), so you can run **data-integrity validation** queries before/after risky migration steps.
> This CLI runs only after the MvcMusicStore web app has already run and seeded both databases.
>
> Note: Throughout this document, I will refer to the two database by the arbitrary names "MvcMusicStore" and "MvcMusicStoreUsers".  It is understood that those names may be different depending on the user's choices when configuring the MvcMusicStore Web App to seed the databases.

------

## 1. Scope

### In Scope

- Seed **Identity DB**:
  - Add **10 users** (`test01`..`test10`)
  - All assigned to the role **Visitor**
  - All using a common password supplied as a required CLI parameter
- Seed **MusicStore DB**:
  - **20 Orders** + **60 OrderDetails**
  - **8 Carts** (4 anonymous, 4 username-based)

### Out of Scope

- Creating databases or schema
- Admin user creation
- Catalog (Albums/Artists/Genres) seeding
- Any production data movement or transformation

------

## 2. Preconditions (Fail Fast)

The CLI MUST fail immediately (no writes) unless:

### 2.1 Connectivity

- Identity DB reachable
- MusicStore DB reachable

### 2.2 Required Schema Exists

- Identity DB: ASP.NET Identity tables exist
- MusicStore DB tables exist:
   `Albums`, `Artists`, `Genres`, `Orders`, `OrderDetails`, `Carts`

### 2.3 Catalog Presence Required

Minimum rows:

- `Albums >= 460`
- `Artists >= 302`
- `Genres >= 14`

If any below threshold → exit with clear error.

### 2.4 Freshness Gates (Re-run Prevention)

Target tables MUST be empty except one Admin user:

| Table        | Expected rows before run |
| ------------ | ------------------------ |
| AspNetUsers  | **1** (Admin only)       |
| Orders       | **0**                    |
| OrderDetails | **0**                    |
| Carts        | **0**                    |

If violated → exit code 3.

Rationale: Guarantee dataset uniqueness and detect non-fresh environment.

------

## 3. Operational Model (Run-Once Semantics)

- **Not idempotent** but guards against re-run
- Expected use: 1 run per freshly baseline-seeded DB pair

------

## 4. Transaction Strategy

Per-database transactions:

1. Validate both DBs
2. Validate schema + catalog + freshness
3. Seed Identity DB (transaction)
4. Seed MusicStore DB (transaction)

If Identity DB seeding fails → MusicStore DB MUST NOT run.

No cross-DB transaction required or attempted.

------

## 5. Stable Key Lookups (No Hard-Coded AlbumIds)

### 5.1 Required Albums (Title Lookup)

All must be found case-insensitive:

1. For Those About To Rock We Salute You
2. Let There Be Rock
3. Faceless
4. The Best Of Buddy Guy - The Millenium Collection
5. Prenda Minha
6. Sozinho Remix Ao Vivo
7. Battlestar Galactica (Classic), Season 1
8. Aquaman
9. Greatest Hits
10. Instant Karma: The Amnesty International Campaign to Save Darfur

If any missing → exit code 2 Preconditions: connectivity/schema/catalog failure.

### 5.2 Rotation Strategy

Use modulo-10 rotation for deterministic distribution.

### 5.3 OrderDetails

- 3 per order
- Quantity = **1**
- UnitPrice = `Albums.Price` at seed time
- Total OrderDetails = **60**

### 5.4 Carts + Items

- 8 carts total
  - 4 anonymous: fixed literal GUID-style strings
  - 4 username carts: `test01`, `test02`, `test03`, `test04`
- 1 items per cart
- Quantity/Count = **1**
- DateCreated = **Base: 2024-01-01 plus {orderIndex} days**

Anonymous cart IDs (fixed literals):

```
cart-anon-00000000-0000-0000-0000-000000000001
cart-anon-00000000-0000-0000-0000-000000000002
cart-anon-00000000-0000-0000-0000-000000000003
cart-anon-00000000-0000-0000-0000-000000000004
```

------

## 6. Identity DB Seed Data

### Pre-existing Admin

- Username:  `admin@musicstore.com`
- Email: `admin@musicstore.com`
- Role: Admin
- MUST NOT be modified

### Test Users (Added)

| Username       | Email                   | Password                              | Role    |
| -------------- | ----------------------- | ------------------------------------- | ------- |
| test01..test10 | testXX@musicstore.local | (supplied as a required CLI argument) | Visitor |

### Role Requirement

- If **Visitor** role does not exist → CLI is not responsible to create this role - if the role does not exist, the CLI will naturally throw exception.  No special coding required.

### Password Hashing

- Use ASP.NET Identity 2.2.4 compatible password hashing

------

## 7. Order Data Template

- 20 Orders (IDs auto-generated)
- Assignment rotates test users:

```
Order 1 → test01
Order 2 → test02
…
Order 10 → test10
Order 11 → test01 (repeat)
```

Dates:

- Base: 2024-01-01
- Add `orderIndex` days

**Customer Info (Fixed Templates):**

```
FirstName: Test{orderIndex:D2}
LastName:  User
Address:   {orderIndex} Test Street
City:      Testville
State:     WA
PostalCode: {98000 + orderIndex}
Country:   USA
Phone:     555-0{orderIndex:D3}
Email:     test{userIndex:D2}@musicstore.local
Total:     0.00
```

Note: Known Limitation, the MvcMusicStore web app puts zero the the Orders.Total column.

------

## 8. Logging Requirements

### Always Verbose

No silent or non-verbose mode.

### Structured Logging Format

Each step logs:

- Step #
- DB target
- Action
- Result (OK/FAIL)
- Elapsed ms

### Error Logging

Include:

- Step name
- Exception type + message
- Missing album/user/table counts when relevant
- Never log secrets

### Exit Summary

Show totals created and exit code.

------

## 9. CLI Inputs

Required:

- `--identity-connection "<name or connection string>"`
- `--musicstore-connection "<name or connection string>"`
- `--initial-password "<password>"`  

Optional:

- `--commit-sha "<sha>"` (for metadata only) - will appear at least once in the log.

No destructive override flags.

------

## 10. Exit Codes

| Code | Meaning                                            |
| ---- | -------------------------------------------------- |
| 0    | Success                                            |
| 1    | Missing required input(s)                          |
| 2    | Preconditions: connectivity/schema/catalog failure |
| 3    | Preconditions: freshness/prod-protection failure   |
| 4    | Seeding error (exception during writes)            |

------

## 11. Acceptance Criteria (Unchanged)

✔ Catalog must exist before run
 ✔ Only 1 Admin user must exist before run
 ✔ Orders, OrderDetails, Carts must be empty
 ✔ Stable title lookups for Albums
 ✔ Per-DB transactions
 ✔ Repeatable data structure (count + pattern)
 ✔ Logs reveal step, errors, row counts
 ✔ Auto-increment IDs allowed to vary

------

## 12. Testing Strategy (Unchanged Focus)

- Gate enforcement unit tests
- Manual validation w/ LocalDB
- Row count checks
- Album lookup tests

------

### Document Status

**v2.12 — Ready for implementation**
 Last Updated: January 2026