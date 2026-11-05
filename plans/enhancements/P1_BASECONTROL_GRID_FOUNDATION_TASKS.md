# Phase 1: BaseControl and BeepGridPro foundation repair — Task List

## Phase 1.A — BaseControl contract hygiene

| # | Task | Status |
|---|------|--------|
| 1.A.1 | Document the explicit BaseControl override contract for paint/layout behavior | ⬜ |
| 1.A.2 | Add or tighten the guard path that prevents base painting/layout drift when derived controls own rendering | ⬜ |
| 1.A.3 | Add a focused regression check around `AllowBaseControlClear` / `IsContainerControl` / `UseBaseMouseInputRouting` behavior | ⬜ |

## Phase 1.B — BeepGridPro rebinding safety

| # | Task | Status |
|---|------|--------|
| 1.B.1 | Audit the `DataSource` / `Uow` transitions and enforce one consistent rebind lifecycle | ⬜ |
| 1.B.2 | Reset stale grid rows, columns, and selection when `ClearGrid` or source replacement occurs | ⬜ |
| 1.B.3 | Verify `RefreshData` and in-/out-of-virtualization paths preserve the correct state after rebind | ⬜ |

## Phase 1.C — Invalidation + theme consistency

| # | Task | Status |
|---|------|--------|
| 1.C.1 | Consolidate the invalidation flow so theme and state updates happen in a single deterministic sequence | ⬜ |
| 1.C.2 | Validate borders, backgrounds, and layout rectangles are not left stale after data refresh or theme changes | ⬜ |

## Phase 1.M — Full focused validation

| # | Task | Status |
|---|------|--------|
| 1.M.1 | Run focused BaseControl + BeepGridPro tests | ⬜ |
| 1.M.2 | Confirm intentional exclusions remain unchanged (base 001 rendering, grid painting disabled) | ⬜ |
| 1.M.3 | Review the final backlog against the design doc and acceptance criteria | ⬜ |

## Issue-by-issue backlog summary

1. BaseControl override contract drift and layout/painter fights.
2. Grid source rebind sequencing producing stale rows/selection/layout.
3. Theme and invalidation churn causing partial or stale repaint state.
4. Missing focused regression coverage for the foundation and grid paths.

## Open decisions

1. Recommended: keep the intentional exceptions explicitly out of scope for this phase and treat them as release decisions rather than defects.
2. Recommended: validate using the existing Controls test project rather than adding a broad end-to-end suite.
