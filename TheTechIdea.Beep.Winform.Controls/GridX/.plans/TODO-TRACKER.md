# GridPro TODO Tracker

Date: 2026-06-06 | Revised: 2026-06-06 | Status: Active
Legend: P0=Critical P1=High P2=Medium P3=Low

---

## Quick Wins (existing grid fixes)

| ID | Task | Pri | Status | Est |
|----|------|-----|--------|-----|
| Q1 | Fix grid hover effect: `CanBeHovered = false` (already in constructor) | P1 | DONE | - |
| Q2 | Fix navigation control overlap with toolbar | P1 | TODO | 0.25d |
| Q3 | Fix toolbar icon theme colors via ImagePainter | P1 | TODO | 0.25d |
| Q4 | Add RowHoverOpacity, NavigatorVisibilityMode, IconColorOverride props | P2 | TODO | 0.25d |
| Q5 | BeepDateDropDown direct dropdown for filter date editor | P2 | TODO | 0.5d |

---

## Phase 0: Centralized Data Controller (IN PROGRESS)

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 0.1 | Create `GridDataController` class (row ownership, key→index map, Bind, RowAdded, RowRemoved, RowUpdated, CellChanged, CommitToSource) | P0 | **DONE** | 1.5d | `Controllers/GridDataController.cs` (648 lines) |
| 0.2 | Wire DataController into BeepGridPro constructor + GridDataHelper delegate | P0 | **DONE** | 0.25d | BeepGridPro.cs, GridDataHelper.cs |
| 0.3 | Wire INotifyCollectionChanged → controller delta methods (add/remove/replace) | P0 | **DONE** | 0.25d | OnCollectionChanged handles Add/Remove/Replace |
| 0.4 | Wire BindingSource.ListChanged → controller methods | P0 | TODO | 0.5d | GridNavigatorHelper.cs |
| 0.5 | Wire UoW events → controller (replace RefreshBinding) | P0 | TODO | 0.5d | GridUnitOfWorkBinder.cs |
| 0.6 | Wire Navigator.InsertNew/DeleteCurrent → controller | P1 | TODO | 0.25d | GridNavigatorHelper.cs |
| 0.7 | Wire EditHelper.EndEdit → controller.CellChanged / CommitToSource | P1 | TODO | 0.25d | GridEditHelper.cs |
| 0.8 | Remove full-rebind calls from helpers (replace with controller calls) | P1 | TODO | 0.25d | Multiple files |
| 0.9 | Maintain selection/scroll through delta operations | P1 | TODO | 0.5d | Selection + Scroll helpers |
| 0.10 | Build and verify all paths | P0 | TODO | 0.5d | Build |

---

## Phase 1: Renderer Interface & Registry

| ID | Task | Pri | Status | Est |
|----|------|-----|--------|-----|
| 1.1 | Define `ICellContentRenderer` interface | P1 | TODO | 0.25d |
| 1.2 | Create `CellRendererRegistry` static class | P1 | TODO | 0.25d |
| 1.3-1.7 | Extract all 5 existing renderers from CellContent switch | P1 | TODO | 1.25d |
| 1.8 | Replace CellContent.cs switch with registry call | P1 | TODO | 0.25d |
| 1.9 | Wire OnCellClick into GridInputHelper | P1 | TODO | 0.25d |
| 1.10 | Unit test: existing cells render same | P1 | TODO | 0.5d |

---

## Phase 2-7: New Renderers, SmartCellAdapter, Auto-Discovery, Missing Controls, Tests

(Unchanged from previous plan — see original for full list)

---

## Dependency Graph

```
Phase 0 (Data Controller) ── MUST COMPLETE FIRST
        │
        ├── Phase 4 (Data Management) ── now mostly covered by Phase 0
        ├── Phase 1 (Renderer Interface)
        ├── Phase 2 (New Renderers)
        ├── Phase 3 (SmartCellAdapter)
        ├── Phase 5 (Auto-Discovery)
        └── Phase 7 (Tests)
```

---

## Progress Summary

| Phase | Tasks | Done | In Progress | Blocked |
|-------|-------|------|-------------|---------|
| Quick Wins | 5 | 1 | 0 | 0 |
| Phase 0: Data Controller | 10 | 3 | 7 | 0 |
| Phase 1: Renderer Interface | 10 | 0 | 0 | 0 |
| Phase 2: New Renderers | 9 | 0 | 0 | 0 |
| Phase 3: SmartCellAdapter | 5 | 0 | 0 | 0 |
| Phase 4: Data Management | 8 | 0 | 0 | 0 |
| Phase 5: Auto-Discovery | 6 | 0 | 0 | 0 |
| Phase 6: Missing Controls | 4 | 0 | 0 | 0 |
| Phase 7: Testing & Docs | 6 | 0 | 0 | 0 |
| **Total** | **63** | **4** | **7** | **0** |

Last Updated: 2026-06-06
