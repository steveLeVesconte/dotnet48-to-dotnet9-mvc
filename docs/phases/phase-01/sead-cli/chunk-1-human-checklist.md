Below are the two things you asked for:

1. a **human verification checklist for Chunk 1**, and
2. a clear recommendation for **where the Seed CLI belongs in the repo**, grounded in your existing folder structure. folder-tree

------

## 1) Human Verification Checklist — Chunk 1 (CLI parsing + echo plan)

This checklist is intentionally **manual and fast**. You should be able to complete it in ~5–10 minutes.

### A. Build & basic execution

-  `dotnet build` succeeds with **no new NuGet dependencies**
-  `dotnet run` (no args) runs and exits cleanly
-  Executable name and banner clearly identify this as **Seed CLI**, not the web app

------

### B. Help / usage behavior

-  `dotnet run -- --help` prints usage and exits **0**
-  `dotnet run -- help` prints the same usage and exits **0**
-  Usage text clearly lists:
  - commands (`seed`, `verify`)
  - required options
  - optional flags
-  Usage text is readable when skimmed (not wall-of-text)

------

### C. Argument parsing correctness

Run each command and verify **exit code + log clarity**:

#### Valid cases

-  `seed` with all required args → exits **0**
-  `verify` with all required args → exits **0**
-  Options work **order-independently**
-  `--verbose` toggles additional log detail

#### Invalid cases

-  Unknown option → usage printed, exit **1**
-  Missing required option → usage printed, exit **1**
-  Unknown command → usage printed, exit **1**

------

### D. Logging quality (this is the real value of Chunk 1)

Inspect the console output carefully:

-  Start banner includes:
  - tool name + version
  - UTC timestamp
  - command (`seed` or `verify`)
  - verbose on/off
  - commit SHA (or `(none)`)
  - project seed token (resolved value)
-  Connection targets are **sanitized**:
  - server + database name only
  - no credentials
  - if parsing fails, shows `[unparsed]` (not an exception dump)
-  “Execution plan” is printed clearly as **future steps**, not implied execution
-  Final summary line states:
  - RESULT=OK or RESULT=USAGE_ERROR
  - exit code

> Litmus test:
>  If Chunk 2 later fails, can you look at this output and *immediately* know whether the failure was before or after DB contact?
>  If yes → logging is good enough.

------

### E. Guardrails (even though Chunk 1 doesn’t enforce them yet)

-  No flags like `--force`, `--dangerous`, etc. exist
-  No connection attempt is made (verified by intentionally broken connection strings)
-  No config files silently override CLI arguments

------

### F. Regression confidence

-  You feel comfortable committing this chunk **alone**
-  You would be happy to hand this output to “future you” six months later
-  You are confident Chunk 2 can start without refactoring Chunk 1

If all boxes are checked → move on.

------

## 2) Where the Seed CLI belongs in the repo

Given your current structure:

```
docs/
src/
  MvcMusicStore/
  packages/
```

folder-tree

### Recommended placement (strongly preferred)

```
src/
  MvcMusicStore/
  SeedCli/
    SeedCli.csproj
    Program.cs
    ...
```

### Why this is the right place

**1) Same repo, different responsibility**

- `MvcMusicStore/` = legacy application baseline
- `SeedCli/` = admin / validation tooling
- Same repo keeps:
  - versioning aligned
  - baselines reproducible
  - migration narrative intact

**2) Avoids “scripts dumping ground”**
 Putting this under `docs/` or `scripts/` would:

- blur the line between documentation and executable tooling
- make it feel throwaway (it isn’t — it’s a guardrail)

**3) Future-proof without overengineering**
 Later repos (Repo 2/3) can:

- copy the CLI forward
- or replace it
   without rewriting history in Repo 1.

------

### Naming recommendation

Use something boring and explicit:

```
SeedCli
MusicStore.SeedCli
MigrationValidation.SeedCli
```

Avoid clever names. This tool earns trust by being obvious.

------

## Optional (but useful): README placement

Add a short README **inside `SeedCli/`**, not at repo root:

```
src/SeedCli/README.md
```

Contents:

- What this tool is for (1 paragraph)
- What it explicitly does NOT do
- How to run `verify` vs `seed`
- Pointer to `seed-cli-specs-improved-v2.1.md`

------

If you want, next we can:

- write the **exact commit message** for Chunk 1, or
- design the **Chunk 2 verification prompt** so it snaps cleanly onto this foundation.