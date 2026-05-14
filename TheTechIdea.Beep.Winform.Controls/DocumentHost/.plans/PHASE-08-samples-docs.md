# Phase 08 — Samples, Docs, NuGet Readme, Tutorial

> **Owner:** _unassigned_  · **Status:** 🟩 Done (A–F complete) · **Predecessor:** all prior phases

## Why This Phase Exists

Even a perfect component is invisible without a polished sample, a clear README,
and a step-by-step tutorial. DevExpress wins not just on features but on
**documentation quality**.

## Tasks

### A. Sample form — `IdeShellDemo`

- [x] **A1** `IdeShellDemoForm.cs` created in `Beep.Winform.Default.Views` (code-only; no designer
  required since all wiring is explicit code — serves better as a tutorial reference).
- [x] **A2** Layout: `BeepDocumentManager` + `BeepTabbedView` + `MenuStrip` + `StatusStrip`.
  (`BeepDockManager` deferred to Phase 05.)
- [x] **A3** Documents: Welcome, Editor, Properties (PropertyGrid), Output (log ListBox).
  (`Properties` and `Output` appear as document tabs until Phase 05 ships.)
- [x] **A4** Wire dirty indicator on Editor — `panel.IsModified = true` on `TextChanged`;
  `StatusStrip` reflects the flag via `ActiveDocumentChanged`.
- [x] **A5** Wire workspace menu — `Workspaces` top-level menu with *Save Current As…*
  and dynamic switch entries driven by `BeepDocumentHost.GetAllWorkspaces()`.
- [x] **A6** Add to `MASTER-TODO-TRACKER.md` "Sample coverage" row → done.
  `IdeShellDemoForm` entry already present in tracker's Rolling Status Summary.

### B. README updates

- [x] **B1** Top-level `DocumentHost/Readme.md`: added *Quick Start* with the
  manager component (5-minute path). Also corrected SchemaVersion 2 → 3 in File Map.
- [x] **B2** Added `BeepDocumentManager.Readme.md` covering:
  - [x] Property reference
  - [x] Event reference
  - [x] Method reference
  - [x] Designer verbs
  - [x] Comparison table with DevExpress DocumentManager
- [x] **B3** Updated root `TheTechIdea.Beep.Winform.Controls/Readme.md` to point
  to `BeepDocumentManager` and the tutorial files.

### C. Tutorial — "Build a VS-style IDE shell in 10 minutes"

- [x] **C1** Markdown tutorial: `DocumentHost/Tutorials/01-IdeShell.md`. ✅
- [x] **C2** Step-by-step text descriptions for each step included in tutorial. ✅
- [ ] **C3** Mirror tutorial as an HTML page under `Help/`. (deferred — no art yet)

### D. NuGet readme

- [x] **D1** Added `<PackageReadmeFile>NUGET-README.md</PackageReadmeFile>` to csproj;
  created `NUGET-README.md` marketing readme highlighting the manager component. ✅
- [ ] **D2** `BeepDocumentManager.bmp` toolbox bitmap. (deferred — art asset needed)

### E. Migration guide

- [x] **E1** `DocumentHost/Tutorials/02-Migrate-from-host-only.md` created. ✅
- [x] **E2** `MainFrm_MDI` migration walked through as the concrete example. ✅

### F. Regression smoke test matrix

- [x] **F1** `DocumentHost/Tests/regression-matrix.md` created — 12 sections,
  50 test cases covering lifecycle, keyboard, split, float, persistence, workspaces,
  shell integration, themes, accessibility, designer, and disposal. ✅
- [x] **F2** Sign-off row included; track per release. ✅

## Acceptance Criteria

- ✅ `IdeShellDemoForm` opens, looks good, demonstrates every headline feature.
- ✅ Tutorial reads end-to-end without "go figure it out" gaps.
- ✅ NuGet README renders correctly on nuget.org.
- ✅ Regression matrix is committed and signed off for this release.

## Out of Scope

- Marketing video / GIFs (nice-to-have, separate task).
- Localisation of docs (English only for first cut).
