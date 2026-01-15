# 📄 Manual Guard Validation — Chunk 2 (Connectivity & Schema)

**Document Type:** Manual validation record (fail-fast guards)  
**Applies To:** `seed-cli` — Chunk 2 (Steps 6–9)  
**Purpose:**  
Verify that database connectivity and required-schema guards behave correctly
*before* any data seeding logic is executed.

> These tests intentionally use **destructive or invalid inputs**
> (incorrect database names, renamed tables) against **non-production databases**
> to validate fail-fast behavior.

---

## Design Note

This chunk assumes successful CLI argument parsing and validation
(verified in Chunk 1).  
All failures in this document are **runtime precondition failures**
and must terminate execution with exit code **2**.

---

## Environment

- Tool: `seed-cli.exe`
- Runtime: .NET Framework 4.8
- Execution context: Local developer machine
- Databases:
  - Identity DB (legacy ASP.NET Identity)
  - MusicStore DB (MVC Music Store catalog)

---

## Test Matrix Summary

| Test ID | Scenario                              | Expected Exit Code | Result |
|--------:|---------------------------------------|-------------------:|--------|
| G-01    | Baseline success (all guards pass)    | 0                  | ⬜     |
| G-02    | Identity DB unreachable               | 2                  | ⬜     |
| G-03    | MusicStore DB unreachable             | 2                  | ⬜     |
| G-04    | Identity schema missing required table| 2                  | ⬜     |
| G-05    | MusicStore schema missing tables      | 2                  | ⬜     |

---

## G-01 — Baseline Success (All Guards Pass)

**Setup**

- Valid `--identity-connection`
- Valid `--musicstore-connection`
- All required tables present in both databases

**Invocation**

```text
seed-cli.exe 
  --identity-connection "<redacted>"
  --musicstore-connection "<redacted>"
  --initial-password "<redacted>"
```

**Expected Behavior**

- Connectivity checks succeed for both databases
- Required-schema checks succeed
- Exit code = **0**
- No secrets or sensitive identifiers logged

**Sample Log Excerpt (expected shape)**

```
step=1 db=CLI action="Parse arguments" result=OK elapsedMs=0
step=2 db=CLI action="Validate required inputs" result=OK elapsedMs=0
Arg presence: --initial-password=provided
Arg presence: --identity-connection=provided
Arg presence: --musicstore-connection=provided
step=3 db=CLI action="Log secret input presence (safe)" result=OK elapsedMs=0
Identity DB reachable OK
step=4 db=Identity action="Connectivity check (open + SELECT 1)" result=OK elapsedMs=69
MusicStore DB reachable OK
step=5 db=MusicStore action="Connectivity check (open + SELECT 1)" result=OK elapsedMs=1
step=6 db=Identity action="Schema check (required Identity tables exist)" result=OK elapsedMs=6
step=7 db=MusicStore action="Schema check (required core tables exist)" result=OK elapsedMs=2
STEP Preconditions connectivity/schema OK (identityOk=true musicstoreOk=true)

... (output truncated)
exited with code 0 (0x0).
```

**Observed Result**

- Exit code: 0
- Notes: worked as expected

------

## G-02 — Identity DB Unreachable

**Setup**

- Modify Identity DB name in `--identity-connection`
- MusicStore connection remains valid

**Invocation**

```
seed-cli.exe 
  --identity-connection "<redacted-invalid-db>"
  --musicstore-connection "<redacted>"
  --initial-password "<redacted>"
```

**Expected Behavior**

- Identity DB connectivity check fails
- MusicStore connectivity and all schema checks are **not executed**
- Sanitized exception message logged
- Exit code = **2**

**Sample Log Excerpt**

```
step=3 db=CLI action="Log secret input presence (safe)" result=OK elapsedMs=0
ERROR stepName="Identity DB connectivity"
exceptionType=System.Data.SqlClient.SqlException
message="Cannot open database '<redacted-db>' requested by the login. The login failed.
Login failed for user '<redacted-user>'."
step=4 db=Identity action="Connectivity check (open + SELECT 1)" result=FAIL elapsedMs=10619

... (output truncated)
exited with code 2 (0x2).
```

**Observed Result**

- Exit code: 2
- Notes: worked as expected; exception message sanitized

------

## G-03 — MusicStore DB Unreachable

**Setup**

- Identity DB connection valid
- Modify MusicStore DB name in `--musicstore-connection`

**Expected Behavior**

- Identity DB connectivity check succeeds
- MusicStore DB connectivity check fails
- Schema checks are **not executed**
- Sanitized exception message logged
- Exit code = **2**

**Sample Log Excerpt**

```
Identity DB reachable OK
step=4 db=Identity action="Connectivity check (open + SELECT 1)" result=OK elapsedMs=604
ERROR stepName="MusicStore DB connectivity"
exceptionType=System.Data.SqlClient.SqlException
message="Cannot open database '<redacted-db>' requested by the login. The login failed.
Login failed for user '<redacted-user>'."
step=5 db=MusicStore action="Connectivity check (open + SELECT 1)" result=FAIL elapsedMs=10027

... (output truncated)
exited with code 2 (0x2).
```

**Observed Result**

- Exit code: 2
- Notes: worked as expected; exception message sanitized

------

## G-04 — Identity Schema Missing Required Table

**Setup**

- Identity DB reachable
- Rename one required Identity table in SSMS
   (e.g., `SomeTableName` → `SomeTableName_DISABLED`)

**Expected Behavior**

- Both connectivity checks succeed
- Identity schema validation fails
- Missing table count and list logged
- Exit code = **2**

**Sample Log Excerpt**

```
step=5 db=MusicStore action="Connectivity check (open + SELECT 1)" result=OK elapsedMs=27
Identity DB missing required tables: count=1 list=[<table name redacted>]
step=6 db=Identity action="Schema check (required Identity tables exist)" result=FAIL elapsedMs=8

... (output truncated)
exited with code 2 (0x2).
```

**Observed Result**

- Exit code: 2
- Notes: worked as expected

------

## G-05 — MusicStore Schema Missing Required Tables

**Setup**

- MusicStore DB reachable
- Rename multiple required MusicStore tables
   (e.g., `SomeTableName` → `SomeTableName_DISABLED`)

**Expected Behavior**

- Identity connectivity and schema checks succeed
- MusicStore schema validation fails
- Missing table count and list logged
- Exit code = **2**

**Sample Log Excerpt**

```
step=6 db=Identity action="Schema check (required Identity tables exist)" result=OK elapsedMs=7
MusicStore DB missing required tables: count=3 list=[<table names redacted>]
step=7 db=MusicStore action="Schema check (required core tables exist)" result=FAIL elapsedMs=6

... (output truncated)
exited with code 2 (0x2).
```

**Observed Result**

- Exit code: 2
- Notes: worked as expected

------

## Validation Notes

- No raw secrets or sensitive identifiers were logged.
- Failure paths terminated immediately (fail-fast).
- Exit codes match the slice contract.
- Schema checks are schema-agnostic (table rename sufficient to trigger failure).

------

## Status

-  All cases executed
-  Results verified
-  Ready to proceed to Chunk 3

