# Toolchain contract

This repository now contains a locally validated first-push-candidate slice.

## Active commands
- `python3 scripts/validate_scaffold.py`
- `dotnet tool restore --tool-manifest .config/dotnet-tools.json`
- `dotnet fantomas src tests --check`
- `dotnet build src/FsharpStakeholder/FsharpStakeholder.fsproj`
- `dotnet build tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj`
- `dotnet test tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj`
- `docker build -t fsharp-stakeholder .`
- `docker run --rm fsharp-stakeholder --list-values`

## Validation profile on this workstation
- Fantomas is installed as a local dotnet tool via `.config/dotnet-tools.json`.
- Native build passes for src and tests.
- Host-side `dotnet test` is blocked because the machine does not have the .NET 8 runtime installed.
- Docker is the mandatory publishability gate here and currently passes:
  - image build
  - .NET 8 test suite (`14` passed)
  - `--list-values`
  - representative focused-family JSON smokes
  - deterministic same-seed comparison
  - experimental-provider fail-fast

## GitHub Actions shape
- `ci-native`
  - `ubuntu-latest`
  - `windows-latest`
  - `macos-latest`
  - local-tool restore, format, build, and test steps
- `docker-smoke`
  - `ubuntu-latest`
  - Docker build plus runtime smokes
- `actionlint`
- `dependency-review`
- Source CodeQL is intentionally not enabled for F# in this tranche.

## Current limitation
- `flake.lock` is now generated locally through the installed Nix toolchain.
