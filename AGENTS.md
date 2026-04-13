# fsharp-stakeholder AGENTS

1. Preserve imported Rust history and explicit provenance docs; do not present this repo as greenfield work.
2. This repo is the active wider-matrix implementation lane and already has full classic-six plus full modern-core depth locally.
3. Keep the repo local-only until remote publication is explicitly authorized; no premature push or remote bootstrap.
4. Active commands:
   - `python3 scripts/validate_scaffold.py`
   - `dotnet tool restore --tool-manifest .config/dotnet-tools.json`
   - `dotnet fantomas src tests --check`
   - `dotnet build src/FsharpStakeholder/FsharpStakeholder.fsproj`
   - `dotnet build tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj`
   - `docker build -t fsharp-stakeholder .`
   - `docker run --rm fsharp-stakeholder --list-values`
5. Keep `origin` intended for `stakeholder-circus/fsharp-stakeholder` and `upstream` pointed at `https://github.com/giacomo-b/rust-stakeholder`.
6. Preserve the conservative MIT policy: keep the upstream license notice unchanged and carry AI/human authorship nuance in provenance docs, not in `LICENSE`.
7. Do not hide missing behavior behind placeholders; record it in `GAPS.md` instead.
