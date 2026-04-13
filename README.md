> [!IMPORTANT]
> This repository is part of a Codex-assisted rewrite experiment. All changes are manually reviewed, a human remains in the loop, and missing behavior is tracked explicitly rather than hidden. The project exists for fun, research, language learning, AI agent workflow/planning, interop experiments, and code review testing.

# fsharp-stakeholder

F# functional parity sibling under `stakeholder-circus`.

## Status
- Imported Rust history is preserved for attribution and auditability.
- Full classic-six plus full modern-core dedicated coverage is implemented locally.
- Deterministic normalized JSON, full `--list-values`, explicit experimental-provider fail-fast behavior, Docker packaging, and split GitHub Actions workflows are in place.
- Local validation is complete for the first-push bar on this workstation via the Docker-backed .NET 8 gate.
- This repo remains local-only and not for push until publication is explicitly authorized.

## Role
- Functional .NET parity sibling.
- Purpose: immutable-first, pipeline-oriented parity implementation with output parity goals against `dotnet-stakeholder` and traceability back to Rust, Java, and `stakeholder-core`.
- Program category: correctness, ecosystem reach

## Toolchain contract
- `dotnet tool restore`
- `dotnet fantomas --check`
- `dotnet build`
- `dotnet test`
- `docker build`
- Docker runtime smoke commands

## Current implementation slice
- Deterministic session generation with normalized JSON output.
- Full 2026+ generator-family registry in `--list-values`.
- Dedicated renderers for all classic-six families:
  - `code_analyzer`
  - `data_processing`
  - `jargon`
  - `metrics`
  - `network_activity`
  - `system_monitoring`
- Dedicated renderers for all modern-core families:
  - `agent_workflows`
  - `platform_engineering`
  - `observability_ai_runtime`
  - `delivery_preview_ops`
  - `supply_chain_security`
- Grouped fallback renderers for post-modern-core families.
- Explicit fail-fast handling for experimental provider flags.

## Validation snapshot
- `python3 scripts/validate_scaffold.py`: pass
- `dotnet tool restore --tool-manifest .config/dotnet-tools.json`: pass
- `dotnet fantomas src tests --check`: pass
- `dotnet build src/FsharpStakeholder/FsharpStakeholder.fsproj`: pass
- `dotnet build tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj`: pass
- host `dotnet test`: blocked on this workstation because only the .NET 10 runtime is installed
- Docker gate: pass (`docker build` ran the .NET 8 test suite with `14` passing tests, plus runtime smokes for `--list-values`, representative focused-family JSON output, deterministic same-seed output, and experimental-provider fail-fast)

## License and provenance
- `LICENSE` keeps the upstream MIT notice exactly as imported from `rust-stakeholder`.
- This tranche does not add a second derivative copyright line.
- AI/human contribution nuance is documented in the provenance docs instead of being overloaded into the license text.

## Current guardrail
- The first local publishability bar is met, but remote creation and push remain intentionally deferred.
- Post-modern-core families remain on grouped fallback and are tracked in `GAPS.md`.
- `flake.lock` generation remains pending until `nix` is available locally.

## Documentation
- [AI disclosure](AI_DISCLOSURE.md)
- [Parity](PARITY.md)
- [Explicit gaps](GAPS.md)
- [Remotes](docs/remotes.md)
- [Provenance](docs/provenance.md)
- [Toolchain](docs/toolchain.md)
