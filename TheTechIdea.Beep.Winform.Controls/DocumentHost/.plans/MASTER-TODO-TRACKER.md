# BeepDocumentHost — Master Todo Tracker

> **Mission:** make `BeepDocumentHost` the DevExpress-grade MDI/docking experience for
> the Beep WinForms stack — _easy in the designer_, _safe at runtime_, and _feature-parity
> with DevExpress DocumentManager + DockManager and Telerik RadDock_.
>
> This tracker indexes the 8 phase plans in this folder. Each phase has its own
> detailed checklist. Update **only** the per-phase files when working on a phase, then
> roll the summary up to this tracker.

## Phase Index

| #  | Phase | Status | File | Owner |
|----|-------|--------|------|-------|
| 01 | Stabilise designer + fix delete-crash       | � Done (A1–D2 implemented) | [PHASE-01-stabilise-designer.md](PHASE-01-stabilise-designer.md) | — |
| 02 | `BeepDocumentManager` non-visual component  | 🟩 Done (A1–E3 implemented) | [PHASE-02-document-manager-component.md](PHASE-02-document-manager-component.md) | — |
| 03 | View modes (Tabbed / NativeMdi / WindowsUI) | � Done (A1–C6, E1–E3 implemented; WindowsUI deferred) | [PHASE-03-view-modes.md](PHASE-03-view-modes.md) | — |
| 04 | Design-time `Documents` collection editor   | � Done (A1–D4, F1–F3; drop-onto-doc deferred) | [PHASE-04-documents-collection.md](PHASE-04-documents-collection.md) | — |
| 05 | Dock panels (tool windows) + DockManager   | 🟩 Done | [PHASE-05-dock-panels.md](PHASE-05-dock-panels.md) | — |
| 06 | Commands, shortcuts, Window menu, ribbon   | � Done (A1–A4, B2, B3, C1–C2, D1–D3, E1–E3 done ✅; F deferred — ribbon optional) | [PHASE-06-commands-and-menu.md](PHASE-06-commands-and-menu.md) | — |
| 07 | Persistence pack, workspaces, undo / redo   | � Done (A1–A3, B1–B2, C1–C2, D1–D2, E1–E2, F1–F2 done ✅) | [PHASE-07-persistence-pack.md](PHASE-07-persistence-pack.md) | — |
| 08 | Samples, docs, NuGet readme, tutorial      | � Done (A1–A6, B1–B3, C1–C2, D1, E1–E2, F1–F2 done ✅; C3/D2 deferred) | [PHASE-08-samples-docs.md](PHASE-08-samples-docs.md) | — |

Legend: 🟩 Done · 🟧 In progress / partial · 🟥 Not started · ⬜ Deferred

## High-Level Order of Work

```
Phase 01 ──┐
           ├─→ Phase 02 ──┐
                          ├─→ Phase 03 ──┐
                                         ├─→ Phase 06 ──┐
           Phase 04 ──────┘              │              │
                                         │              ├─→ Phase 08
           Phase 05 ─────────────────────┘              │
                                                        │
                                          Phase 07 ─────┘
```

- **Phase 01 must ship first.** Nothing else is safe until the designer stops crashing.
- **Phase 02 is the headline feature** — a DevExpress-style non-visual `BeepDocumentManager`
  component that lives in the form's component tray and orchestrates the visible host.
- **Phases 03–06 are parallelisable** after Phase 02 lands.
- **Phase 07 is mostly already implemented** — Phase 7 here is the wrap-and-validate pass.
- **Phase 08 closes the loop** with a polished demo and shipping docs.

## Cross-Phase Commercial-Parity Checklist

These are the headline features tracked across multiple phases. Each row links to the
phase that owns the implementation.

| Feature | DevExpress | Telerik | DockPanelSuite | Beep Phase |
|---------|-----------|---------|----------------|-----------|
| Non-visual manager component in tray | DocumentManager | RadDock host | — | **02** |
| Design-time **Document collection editor** | ✅ | ✅ | ✅ | **04** |
| **TabbedView** (default MDI) | ✅ | ✅ | ✅ | **03** |
| **NativeMdiView** (true `IsMdiContainer = true`) | ✅ | — | — | **03** |
| **WindowsUI** view (full-screen tile launcher) | ✅ | — | — | **03** deferred |
| Smart-tag inline pickers (TabStyle/Position/CloseMode) | ✅ | ✅ | — | **04** |
| Drop-onto-document at design time | ✅ | ✅ | — | **04** |
| Designer verbs: Add / Clear / Export / Shortcuts | ✅ | ✅ | — | **04** |
| `Documents` collection serialised in `.Designer.cs` | ✅ | ✅ | ✅ | **04** |
| Floating-window persistence | ✅ | ✅ | ✅ | **07** ✅ |
| Auto-hide tool panels (Left/Right/Top/Bottom) | ✅ | ✅ | ✅ | **05** ✅* |
| **DockPanel** sibling component (left/right/bottom rails) | ✅ DockManager | ✅ | ✅ | **05** |
| Theme integration | ✅ | ✅ | ✅ | **02** ✅* |
| Auto-populate `Window` menu | ✅ | ✅ | — | **06** ✅* |
| Keyboard shortcut customiser dialog | ✅ | — | — | **06** ✅* |
| Command palette (Ctrl+Shift+P) | — | — | — | **06** ✅ |
| Quick-switch (Ctrl+Tab) | ✅ | ✅ | ✅ | **06** ✅ |
| MRU / recent documents | ✅ | ✅ | ✅ | **07** ✅ |
| Pinned documents | ✅ | ✅ | ✅ | **07** ✅ |
| Preview tabs (italic, replace on next open) | ✅ VS | ✅ | — | **07** ✅ |
| Dirty / unsaved-changes guard | ✅ | ✅ | ✅ | **07** ✅ |
| Document templates (factory restore) | ✅ | ✅ | ✅ | **07** ✅ |
| Workspaces (named layout sets) | ✅ | ✅ | — | **07** ✅ |
| Undo / redo layout (`Ctrl+Z` / `Ctrl+Y`) | ✅ | — | — | **07** ✅ |
| Layout migration (v1 → v2) | ✅ | — | — | **07** ✅ |
| Drag dock-zone compass overlay | ✅ | ✅ | ✅ | **05** ✅ |
| Cross-host drag transfer | ✅ | ✅ | ✅ | **05** ✅ |
| Cloud sync (opt-in extension) | — | — | — | **07** ✅ (extension) |
| Mini-toolbar / status bar / breadcrumb (opt-in) | ✅ partial | — | — | **05** ✅ (extension) |
| Git status / diff panes (extension) | — | — | — | deferred |

\* = implemented as direct property on `BeepDocumentHost`; Phase 02 exposes the same
property via the new manager component so it works without a visible host.

## Rolling Status Summary

_Update this section at the end of each phase._

- **Runtime:** 96–98 % commercial-grade (confirmed by `CURRENT-STATE-AUDIT.md`)
- **Design-time:** 82 % — blocked by Phase 01 crash and Phase 02 manager gap
- **Sample coverage:** `MainFrm_MDI` (dirty-guard + editor doc) ✅; `IdeShellDemoForm` (full demo) ✅
- **Phase 06 completed** — Ctrl+Tab MRU cycling (E1) implemented; all A–E items done ✅
- **Phase 07 completed** — Layout migration v3 ✅; cloud-sync re-wired through manager ✅; `Cloud.Readme.md` written ✅
- **Phase 08 completed** — Quick Start docs (B1–B3) ✅; IDE shell tutorial (C1–C2) ✅; NuGet readme + `PackageReadmeFile` (D1) ✅; migration tutorial (E1–E2) ✅; regression matrix 50 cases (F1–F2) ✅. Deferred: C3 (HTML mirror), D2 (toolbox bitmap).

## How To Use This Tracker

1. **Pick a phase** — start with Phase 01 unless explicitly told otherwise.
2. **Open that phase's `.md` file** — work _only_ inside that file's checklist.
3. **Tick boxes** in the phase file as items complete.
4. **At end of phase**, set its status here (🟧 → 🟩) and update the
   _Rolling Status Summary_.
5. **Do not** add ad-hoc TODOs outside the phase files; if scope grows,
   create a new phase or amend an existing one.

## Historic Plan Files (Archive)

The following files were the original audit + roadmap. Keep them; new work supersedes
their TODOs but their analysis is still valid background reading.

- [CURRENT-STATE-AUDIT.md](CURRENT-STATE-AUDIT.md)
- [COMMERCIAL-REFERENCE-NOTES.md](COMMERCIAL-REFERENCE-NOTES.md)
- [IMPLEMENTATION-ROADMAP.md](IMPLEMENTATION-ROADMAP.md)
- [README.md](README.md)
