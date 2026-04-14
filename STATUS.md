# fsharp-stakeholder Status

Last updated: 2026-04-13 CEST

- Role: `validated-wider-matrix`
- Parity class: `full-parity`
- Phase target: `first-push-ready-wider-matrix`
- Phase state: `complete`
- Phase completeness: `100%`
- Program state: `publication-held`
- Program completeness: `54%`
- Rewrite completeness: `54%`
- Functionality completeness: `48%`
- Branch: `main`
- Origin: `git@github.com:stakeholder-circus/fsharp-stakeholder.git`
- Upstream: `https://github.com/giacomo-b/rust-stakeholder`

## Blockers
- Remote creation and first push are intentionally deferred until at least 10 new full rewrites with tests are complete.
- Publication is held until the 10-rewrite threshold is met.
- Host-side `dotnet test` is blocked on this workstation because only the .NET 10 runtime is installed; Docker remains the authoritative validation path locally.
- `flake.lock` is now generated through the installed Nix toolchain.

## Next
- Keep the repo publication-held until the 10-rewrite publication threshold is met.
- Use the validated F# slice as the implementation template for `zig-stakeholder` and the remaining wider-matrix repos.

## Canonical references
- [`stakeholder-core/docs/program/rewrite-status-matrix.md`](/Users/davidsupan/shareholder/stakeholder-core/docs/program/rewrite-status-matrix.md)
- [`stakeholder-core/status/JOB_STATUS.md`](/Users/davidsupan/shareholder/stakeholder-core/status/JOB_STATUS.md)
