## Chunking plan for AI–human cooperation (Seed CLI)

### Chunk 0 — Project skeleton and wiring

**Goal:** Compile + run a no-op command.

- Create new console project (e.g., `SeedCli`)
- Add config plumbing (appsettings + env vars if you want)
- Decide logging approach (simple `Console.WriteLine` + a tiny `Log` helper)

**Deliverable:** `dotnet run -- seed --help` works, exits 0.

------

### Chunk 1 — CLI parsing + echo plan

*(Your suggested first chunk — yes.)*

**Goal:** Parse args, validate basic shape, and print a “run plan” without touching DBs.

- Commands: `seed` (default), `verify`
- Options:
  - identity connection (name or full string)
  - musicstore connection
  - `--verbose`
  - optional `--commit-sha`
- Sanitize and log connection targets (server + db; never secrets)
- Print step list you *intend* to run

**Deliverable:** Running with various args yields clear logs and correct exit codes for usage errors.

**Human role:** confirm the CLI UX feels right and the log lines contain the debugging context you’ll want later.

------

### Chunk 2 — “Go / No-Go” validation (no writes)

*(Your suggested second chunk — yes.)*

**Goal:** Implement all preconditions + safety gate checks, with rich logs.

- Connect to both DBs
- Check required tables exist (schema presence)
- Check `dbo.__EnvironmentMarker` exists + required row matches token
- Check catalog exists (`Albums` count > 0)
- Implement `verify` command to run these checks and exit
- Define exit codes:
  - `1` precondition failure
  - `2` environment marker gate failure

**Deliverable:** `verify` gives deterministic pass/fail with step logging.

**Human role:** run against local DBs and confirm failures are “actionable” (the message tells you exactly what to fix).

------

### Chunk 3 — Read-side helpers + stable album lookup

**Goal:** Build the reusable query helpers you’ll rely on during seeding.

- ADO.NET helpers (execute scalar, execute reader, execute nonquery)
- “Stable key lookup” for albums (title + artist, or whatever you choose)
- Fail-fast if required albums are missing
- Add structured logging for found IDs and missing keys

**Deliverable:** A `--dry-run` (optional) or `verify` extension that shows resolved album IDs.

**Human role:** choose the stable album keys set (5–10 titles) once you inspect your baseline catalog.

------

### Chunk 4 — Identity DB seeding (transactional)

**Goal:** Seed only test users/roles; do not touch Administrator.

- Transaction on Identity DB
- Ensure roles if needed
- Ensure `test01..test10` exist (idempotent)
- Write `__SeedToolMarker` for Identity DB

**Deliverable:** `seed` now changes Identity DB only (MusicStore portion still stubbed), logs counts.

**Human role:** verify it behaves correctly when rerun (no duplicates) and that password handling is acceptable.

------

### Chunk 5 — MusicStore DB seeding (transactional)

**Goal:** Seed carts + orders + orderdetails deterministically.

- Transaction on MusicStore DB
- Ensure 8 carts (4 anon deterministic GUID-like + 4 username carts)
- Insert 20 orders
- Insert deterministic orderdetails referencing resolved album IDs
- Write `__SeedToolMarker` for MusicStore DB

**Deliverable:** `seed` produces full dataset; rerun is no-op or add-missing.

**Human role:** run your data-integrity validation SQL before/after and confirm checksums/counts match expectations.

------

### Chunk 6 — “Operational polish”

**Goal:** Make it safe and pleasant for future-you.

- Better summaries
- More defensive sanitization
- Optional `--verbose` expansions (timings, per-step counts)
- Improve error messages (include step name + last successful step)
- README usage examples

**Deliverable:** a tool you can hand to “future repo phases” without re-explaining.

------

## Prompting format (repeatable template)

For each chunk, keep AI inputs tight:

1. **Goal + non-goals** (one screen)
2. **Interfaces** (args/options; function signatures)
3. **Constraints** (no secrets in logs, no overrides, etc.)
4. **Expected outputs** (sample log lines; exit codes)
5. **Files in scope** (only the files you want edited)

That’s the “human guardrail” that prevents one-shot mega-generation.

------

## Recommended development cadence

For each chunk:

1. AI generates code for that chunk only
2. You run locally, capture one failure log if it fails
3. AI fixes only that failure (no refactors)
4. Once green, tag/commit (small PR-style commits)

This matches your “run repeatedly for debugging; used once when correct” reality.