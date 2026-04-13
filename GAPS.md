> [!NOTE]
> Missing or deferred behavior must fail fast and be tracked explicitly. No placeholder behavior should mask absent parity work.

# F# Gaps

## Current explicit gaps
- `fsharp-stakeholder.post-modern-core-pending`: `ai_governance`, `security_blockchain`, `health_protocol`, and `overlay_quantum` remain on grouped fallback; no deeper parity claims are made for those packets in this tranche.
- `fsharp-stakeholder.remote-publication-deferred`: the repo is intentionally local-only until remote creation and first publication are explicitly authorized.
- `fsharp-stakeholder.github-required-check-binding-deferred`: exact GitHub required checks cannot be bound until the repo has a remote and stable CI contexts.
- `fsharp-stakeholder.host-dotnet8-runtime-gap`: host-side `dotnet test` is blocked on this workstation because only the .NET 10 runtime is installed; Docker is the authoritative .NET 8 validation path here.
- `fsharp-stakeholder.codeql-unsupported-currently`: source CodeQL is not enabled for F# in this tranche because GitHub does not currently list F# as a supported CodeQL source language.
- `fsharp-stakeholder.flake-lock-pending`: `flake.nix` is present, but `flake.lock` cannot be generated until `nix` is available locally.

## Guardrail
- Do not present the repo as publication-complete until remote publication, stable GitHub CI contexts, and later-tranche gaps are handled explicitly.
