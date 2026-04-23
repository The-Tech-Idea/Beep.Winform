# BeepGridPro — Master Enhancement Plan

_Generated: 2026-04-22_
_Updated: 2026-04-22 — Phase 18: Unified Toolbar (Actions + Search + Filter combined)_

## Overview

This plan organizes all BeepGridPro enhancements into 18 phases across 4 tracks. Each phase has its own detailed document in `Enhancements/`.

## Tracks

| Track | Description | Phases |
|---|---|---|
| **Bug Fixes** | Correctness, rendering, and UI stability | 1-2 |
| **Data & Binding** | DataSource handling, performance, UoW sync | 3-9 |
| **Feature Additions** | New capabilities: selection, export, grouping, virtualization, unified toolbar | 10-13, 18 |
| **Polish & Quality** | Style wiring, layout presets, accessibility, editor framework | 14-17 |

---

## Phase Summary

### Track 1: Bug Fixes

| Phase | Title | Priority | Status | Doc |
|---|---|---|---|---|
| 1 | Filter Visibility Rendering Fix | P0 | Pending | `Enhancements/PHASE_001_FilterVisibilityFix.md` |
| 2 | Date Editor Direct Dropdown | P0 | Pending | `Enhancements/PHASE_002_DateEditorDirect.md` |

### Track 2: Data & Binding

| Phase | Title | Priority | Status | Doc |
|---|---|---|---|---|
| 3 | ObservableCollection Live Updates | P0 | Pending | `Enhancements/PHASE_003_ObservableCollection.md` |
| 4 | DataTable Cell-Level Fast Refresh | P0 | Pending | `Enhancements/PHASE_004_DataTableFastPath.md` |
| 5 | Schema Change Detection | P1 | Pending | `Enhancements/PHASE_005_SchemaChangeDetection.md` |
| 6 | UoW PostCommit/PostUpdate Cell Sync | P1 | Pending | `Enhancements/PHASE_006_UowPostCommitSync.md` |
| 7 | Deduplicate Position/Current Events | P1 | Pending | `Enhancements/PHASE_007_DeduplicatePositionEvents.md` |
| 8 | Dead Code Cleanup | P2 | Pending | `Enhancements/PHASE_008_DeadCodeCleanup.md` |
| 9 | Documentation & Tests | P3 | Pending | `Enhancements/PHASE_009_DocsAndTests.md` |

### Track 3: Feature Additions

| Phase | Title | Priority | Status | Doc |
|---|---|---|---|---|
| 10 | Selection Mode Strategies | P1 | Pending | `Enhancements/PHASE_010_SelectionModes.md` |
| 11 | Export Engine (Excel/CSV/PDF/HTML) | P2 | Pending | `Enhancements/PHASE_011_ExportEngine.md` |
| 12 | Row Grouping with Summaries | P2 | Pending | `Enhancements/PHASE_012_Grouping.md` |
| 13 | Large Dataset Virtualization | P2 | Pending | `Enhancements/PHASE_013_Virtualization.md` |
| 18 | Unified Toolbar (Actions + Search + Filter) | P1 | Pending | `Enhancements/PHASE_018_UnifiedToolbar.md` |

### Track 4: Polish & Quality

| Phase | Title | Priority | Status | Doc |
|---|---|---|---|---|
| 14 | Modern Style & Layout Preset Wiring | P2 | Pending | `Enhancements/PHASE_014_ModernStyleWiring.md` |
| 15 | LayoutPreset Enum-to-Class Wiring | P2 | Pending | `Enhancements/PHASE_015_LayoutPresetWiring.md` |
| 16 | Accessibility (UIA + Keyboard Nav) | P3 | Pending | `Enhancements/PHASE_016_Accessibility.md` |
| 17 | Editor Framework Refactor | P3 | Pending | `Enhancements/PHASE_017_EditorFramework.md` |

---

## Execution Order

### Sprint 1 — Critical Fixes (Phases 1-4)
These address silent data loss, rendering bugs, and performance regressions.

1. **Phase 1** — Filter visibility rendering fix
2. **Phase 2** — Date editor direct dropdown
3. **Phase 3** — ObservableCollection live updates
4. **Phase 4** — DataTable fast refresh path

### Sprint 2 — Data Correctness (Phases 5-9)
These fix remaining data binding issues and establish documentation baseline.

5. **Phase 5** — Schema change detection
6. **Phase 6** — UoW post-commit sync
7. **Phase 7** — Deduplicate position events
8. **Phase 8** — Dead code cleanup
9. **Phase 9** — Documentation & tests

### Sprint 3 — Core Features (Phases 10-13, 18)
These add the most-requested grid capabilities.

10. **Phase 10** — Selection mode strategies
11. **Phase 11** — Export engine
12. **Phase 12** — Row grouping
13. **Phase 13** — Virtualization
14. **Phase 18** — Unified toolbar (actions + search + filter in one bar)

### Sprint 4 — Polish (Phases 14-17)
These complete known gaps and improve developer/user experience.

16. **Phase 14** — Modern style wiring
17. **Phase 15** — Layout preset wiring
18. **Phase 16** — Accessibility
19. **Phase 17** — Editor framework refactor

---

## Risk Assessment

| Risk | Impact | Mitigation |
|---|---|---|
| Phases 1-2 break existing filter behavior | High | Phase 1 is pure additive (skip invisible rows); existing visible rows unchanged |
| Phase 3 subscription leak | Medium | Store reference, unsubscribe on ClearDataSource and rebind |
| Phase 4 DataTable fast path misses edge cases | Medium | Fallback to full rebind if column lookup fails |
| Phase 10 selection refactor breaks checkbox selection | High | Keep checkbox selection path separate; add strategy for it |
| Phase 11 export depends on external libraries | Medium | Use built-in .NET APIs first; optional NuGet for Excel |
| Phase 13 virtualization is complex | High | Start with row-only virtualization; add column virtualization later |
| Phase 18 unified toolbar breaks existing filter workflows | High | Keep old controls as fallback; new toolbar is opt-in via `ShowToolbar` property |

---

## Success Criteria

- All P0 phases pass without regression
- `BeepGridStyle.Modern` renders correctly
- All `GridLayoutPreset` enum values are property-wired
- `ObservableCollection<T>` mutations reflect in grid automatically
- DataTable cell edits do not trigger full rebind
- Filter `IsVisible` flag correctly hides rows
- Date cells open dropdown on first click
- Selection modes work independently
- Export produces valid files for all formats
- Grouping works with existing sort/filter
- Virtualization handles 100K+ rows smoothly
- Screen reader announces grid content
- Keyboard navigation covers all grid regions
- Unified toolbar renders actions + search + filter in one bar
- Toolbar uses painted SVG icons (no child controls at rest)
- Toolbar sizing/alignment works at all DPI settings
- Export/import/print buttons use `StyledImagePainter` with icon library

---

## Related Documents

- [Folder Plan](./FOLDER_PLAN.md)
- [TODO Tracker](./TODO_TRACKER.md)
- [Legacy: DataSource Enhancement Plan](./PLAN_DataSource_Enhancement.md)
- [Legacy: Filter & Date Editor Plan](./PLAN_GridPro_Filter_DateEditor.md)
- [Claude.md](./Claude.md) — Implementation guide
- [README.md](./README.md) — Overview
