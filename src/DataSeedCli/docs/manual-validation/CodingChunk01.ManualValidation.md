# 📄 Manual Guard Validation — Chunk 1 (CLI Argument Parsing)

**Document Type:** Manual validation record (fail-fast guards)  
**Applies To:** `seed-cli` — Chunk 1 (Steps 1–5)  
**Purpose:**  
Verify that CLI argument parsing and required-input validation behave correctly
*before* any database connectivity or seeding logic is introduced.

> These tests intentionally use **invalid or missing CLI arguments** to validate
> fail-fast behavior at the CLI boundary.

---

## Design Note

Argument parsing is **non-failing by design**.  
All semantic validation (missing required inputs, help behavior, exit codes)
is handled explicitly in the validation step.

---

## Environment

- Tool: `seed-cli.exe`
- Runtime: .NET Framework 4.8
- Execution context: Local developer machine

---

## Test Matrix Summary

| Test ID | Scenario                               | Expected Exit Code | Result |
|--------:|----------------------------------------|-------------------:|--------|
| A-01    | Baseline success (all args present)    | n/a                | ⬜     |
| A-02    | `--help` only                          | 0                  | ⬜     |
| A-03    | All required arguments missing         | 1                  | ⬜     |
| A-04    | Invalid arguments ignored              | n/a                | ⬜     |
| A-05    | Missing `--identity-connection`        | 1                  | ⬜     |
| A-06    | Missing `--musicstore-connection`      | 1                  | ⬜     |
| A-07    | Missing `--initial-password`           | 1                  | ⬜     |

---

## A-01 — Baseline Success (All Required Arguments Present)

**Setup**

- `--identity-connection` provided
- `--musicstore-connection` provided
- `--initial-password` provided

**Invocation**

```text
seed-cli.exe 
  --identity-connection "any string"
  --musicstore-connection "any string"
  --initial-password "any string"
```

**Expected Behavior**

- Argument parsing succeeds
- Required-input validation succeeds
- No exit occurs; execution proceeds to the next chunk
- No secrets logged

**Sample Log Excerpt (expected shape)**

```
step=1 db=CLI action="Parse arguments" result=OK elapsedMs=0
step=2 db=CLI action="Validate required inputs" result=OK elapsedMs=0
Arg presence: --initial-password=provided
Arg presence: --identity-connection=provided
Arg presence: --musicstore-connection=provided
```

**Observed Result**

- Exit: none (execution proceeds)
- Notes: worked as expected

------

## A-02 — `--help` Only

**Setup**

- Single argument: `--help`

**Invocation**

```
seed-cli.exe --help
```

**Expected Behavior**

- Usage information is printed
- Exit code = **0**

**Sample Log Excerpt**

```
step=1 db=CLI action="Parse arguments" result=OK elapsedMs=0
Usage:
  seed-cli --initial-password "<password>" --identity-connection "<connection string>" --musicstore-connection "<connection string>"

Options:
  --help                         Show this help and exit (code 0).
  --initial-password             Required unless --help.
  --identity-connection          Required unless --help. Treated as a literal connection string from CLI args only.
  --musicstore-connection        Required unless --help. Treated as a literal connection string from CLI args only.

Exit codes:
  0  Success
  1  Missing required input(s)

... (output truncated)
exited with code 0 (0x0).
```

**Observed Result**

- Exit code: 0
- Notes: worked as expected

------

## A-03 — All Required Arguments Missing

**Setup**

- No arguments provided

**Expected Behavior**

- Required-input validation fails
- Missing arguments are listed
- Usage information is printed
- Exit code = **1**

**Sample Log Excerpt**

```
step=1 db=CLI action="Parse arguments" result=OK elapsedMs=0
step=2 db=CLI action="Validate required inputs" result=FAIL elapsedMs=0
Missing required inputs: --initial-password, --identity-connection, --musicstore-connection
Usage:
  seed-cli --initial-password "<password>" --identity-connection "<connection string>" --musicstore-connection "<connection string>"

Exit codes:
  0  Success
  1  Missing required input(s)

... (output truncated)
exited with code 1 (0x1).
```

**Observed Result**

- Exit code: 1
- Notes: worked as expected

------

## A-04 — Invalid Arguments Ignored

**Setup**

- One or more unknown/invalid arguments provided
- All required arguments present

**Invocation**

```
seed-cli.exe 
  --some-invalid-argument
  --identity-connection "any string"
  --musicstore-connection "any string"
  --initial-password "any string"
```

**Expected Behavior**

- Invalid arguments are ignored
- Required-input validation succeeds
- No exit occurs; execution proceeds to the next chunk
- No secrets logged

**Sample Log Excerpt**

```
step=1 db=CLI action="Parse arguments" result=OK elapsedMs=0
step=2 db=CLI action="Validate required inputs" result=OK elapsedMs=0
Arg presence: --initial-password=provided
Arg presence: --identity-connection=provided
Arg presence: --musicstore-connection=provided
```

**Observed Result**

- Exit: none (execution proceeds)
- Notes: worked as expected

------

## A-05 — Missing `--identity-connection`

**Setup**

- All required arguments provided except `--identity-connection`

**Invocation**

```
seed-cli.exe 
  --musicstore-connection "any string"
  --initial-password "any string"
```

**Expected Behavior**

- Required-input validation fails
- Missing argument listed: `--identity-connection`
- Usage information is printed
- Exit code = **1**

**Sample Log Excerpt**

```
step=1 db=CLI action="Parse arguments" result=OK elapsedMs=0
step=2 db=CLI action="Validate required inputs" result=FAIL elapsedMs=0
Missing required inputs: --identity-connection

... (output truncated)
exited with code 1 (0x1).
```

**Observed Result**

- Exit code: 1
- Notes: worked as expected

------

## A-06 — Missing `--musicstore-connection`

**Setup**

- All required arguments provided except `--musicstore-connection`

**Invocation**

```
seed-cli.exe 
  --identity-connection "any string"
  --initial-password "any string"
```

**Expected Behavior**

- Required-input validation fails
- Missing argument listed: `--musicstore-connection`
- Usage information is printed
- Exit code = **1**

**Observed Result**

- Exit code: 1
- Notes: worked as expected

------

## A-07 — Missing `--initial-password`

**Setup**

- All required arguments provided except `--initial-password`

**Invocation**

```
seed-cli.exe 
  --identity-connection "any string"
  --musicstore-connection "any string"
```

**Expected Behavior**

- Required-input validation fails
- Missing argument listed: `--initial-password`
- Usage information is printed
- Exit code = **1**

**Observed Result**

- Exit code: 1
- Notes: worked as expected

------

## Validation Notes

- Argument parsing never throws exceptions.
- Required-input validation fails deterministically.
- Exit codes match the CLI contract.
- No secrets or sensitive identifiers are logged.

------

## Status

-  All cases executed
-  Results verified
-  Ready to proceed to Chunk 2