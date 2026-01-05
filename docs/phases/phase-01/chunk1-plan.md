https://chatgpt.com/g/g-p-693cc11b81cc819185432d720734c6a2-multi-mode-prompt-stack/c/6945cc6a-c0a8-832f-a90b-497c1ddc6751





MODE: Execution / Coding (Chunk 1 only)

CONTEXT
We are building a one-time “Seed CLI” tool used to load a deterministic validation dataset into an existing legacy system.
This is Repo 1 / baseline-era tooling; avoid introducing new dependencies unless absolutely necessary.

TECH STACK CONSTRAINTS
- Mirror the existing legacy stack where applicable (EF6/Identity2 era). However Chunk 1 does not touch EF/DB.
- For Chunk 1: DO NOT add any new NuGet packages (no System.CommandLine, no Serilog, etc.).
- Use simple Console.WriteLine logging with a tiny helper, but structured.

CHUNK 1 GOAL
Implement CLI argument parsing and “echo back the plan” logging.
No database connections. No writes. No schema checks.

FUNCTIONAL REQUIREMENTS
1) Supported commands:
   - seed (default command if none provided)
   - verify
   - help (or --help / -h)

2) Supported options:
   - --identity-connection <value>
   - --musicstore-connection <value>
   - --verbose (flag)
   - --commit-sha <value> (optional)
   - --project-seed-token <value> (optional; if omitted use a default constant)

3) Parsing rules:
   - Order-independent options.
   - Unknown options => print usage + exit code 1.
   - Missing required options for seed/verify => print usage + exit code 1.
   - For help: print usage + exit code 0.

4) Logging / output requirements:
   - Start banner (tool name/version, UTC time, command, verbose true/false, commit sha, project token)
   - Sanitized connection “targets”:
     - NEVER print full connection strings.
     - Print only: Data Source / Server + Initial Catalog / Database if present.
     - If parsing fails, print “[unparsed]”.
   - Print “execution plan” steps (just names; no DB work):
     Example steps list (for seed):
       1) Validate args
       2) (future) Connect to both DBs
       3) (future) Validate schema
       4) (future) Validate __EnvironmentMarker gate
       5) (future) Validate catalog exists
       6) (future) Seed Identity DB (transaction)
       7) (future) Seed MusicStore DB (transaction)
       8) (future) Write seed markers
     For verify, similar but no seed steps.
   - End summary line: RESULT=OK|USAGE_ERROR and exit code.

5) Exit codes (Chunk 1 only):
   - 0 success (including help)
   - 1 usage/argument error

NON-GOALS (DO NOT IMPLEMENT)
- Any DB connectivity
- Any marker checks
- Any schema inspection
- Any seeding logic
- Any config file loading (appsettings) unless it’s already there; keep it simple for now

DELIVERABLES
- Provide complete code changes for:
  - Program.cs (or equivalent entry)
  - Any small helper files you add (e.g., CliOptions.cs, Logger.cs)
- Include a short “how to run” section with example commands and sample output.

REPO / FILE SCOPE
Assume a new console app project already exists with a minimal Program.cs.
Only modify/add files within this new SeedCli project folder.
Do not touch the legacy web app.

QUALITY BAR
- Code must be clear, minimal, testable-by-running.
- Defensive parsing; readable usage text.
- No secrets in logs.

NOW PRODUCE
1) The code (as complete file listings).
2) Example runs (seed, verify, help, error case) showing expected output.