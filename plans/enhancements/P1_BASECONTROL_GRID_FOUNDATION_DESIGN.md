# Phase 1: BaseControl and BeepGridPro foundation repair — Design Document

## 0. Problem Statement

The review identified that the shared foundation and the grid are the highest-risk integration points in the controls library. The current implementation relies on a large shared base contract plus a helper-heavy grid, and the risk is not random breakage so much as subtle state drift: 

- `BaseControl.cs:34-46` explicitly configures the base painting contract via `AllowBaseControlClear`, `IsContainerControl`, and `UseBaseMouseInputRouting`, but the effective behavior still depends on derived controls honoring those overrides correctly.
- `BaseControl.Methods.cs:27-80` and `BaseControl.LayoutManagement.cs:12-90` show a layout and painter flow where the base may still paint or recalculate sizing in ways that conflict with custom controls that manage their own rendering.
- `BeepGridPro.Properties.cs:54-158` rebinds `Uow` and `DataSource`, and `GridDataController.cs:12-61` owns the bind lifecycle. There are multiple transition points where stale data and stale layout state can survive rebinds, especially when switching between data sources and unit-of-work bindings.

This phase intentionally treats the following as out of scope: the intentionally customized BaseControl base 001 rendering behavior and the intentional grid painting disablement, per the user’s explicit direction.

## 1. Goals

1. Establish a consistent foundation contract for BaseControl so that derived controls can deliberately opt in or out of base painting without causing drift.
2. Make BeepGridPro rebind behavior deterministic when `DataSource`, `Uow`, `ClearGrid`, and `RefreshData` are used in sequence.
3. Prevent theme/invalidation churn from leaving stale UI state or incorrect render rectangles after grid and base activities change state.
4. Convert the architectural findings into an issue-by-issue backlog with acceptance criteria and a verified verification sequence.

## 2. Design

### Phase 1.A — BaseControl foundation contract guardrails

Issue: `BaseControl` mixes layout/painter/theme state into a broad shared surface, but the contract is only partially enforced. Derived controls can override the base painting policy, yet there is no explicit guarantee that those overrides remain consistent with the final layout and invalidation path.

Files likely involved:

- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.Methods.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.LayoutManagement.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.Events.cs`

Recommended change pattern:

1. Define a single, explicit contract for paint and layout overrides in the base class comments and validation path.
2. Ensure that class-level overrides are obeyed consistently before any base fill or layout recalc executes.
3. Add a dedicated guard around invalidation so low-level paint decisions do not silently fight a derived control’s own render pipeline.

Acceptance criteria:

- BaseControl can be used by controls that do custom rendering without implicit base clearing or layout collisions.
- The override contract is explicit and testable, not a side effect of a particular derived control.

### Phase 1.B — BeepGridPro rebind safety

Issue: the grid data flow is distributed across property setters and helper classes. The risk is not a single bad line but that state transitions are split across the property setter (`Uow` / `DataSource`), `GridDataController`, `Navigator`, and layout recalculation. This can leave stale rows, stale columns, or stale selection visibility after data-binding changes.

Files likely involved:

- `TheTechIdea.Beep.Winform.Controls/GridX/BeepGridPro.Properties.cs`
- `TheTechIdea.Beep.Winform.Controls/GridX/Controllers/GridDataController.cs`
- `TheTechIdea.Beep.Winform.Controls/GridX/BeepGridPro.cs`

Recommended change pattern:

1. Centralize the “rebind” flow into a single lifecycle that clears stale view state before reattaching a new source.
2. Make `DataSource`, `Uow`, and `ClearGrid` use a consistent sequence: clear stale view state → bind current source → recalc layout → invalidate once.
3. Avoid reusing stale `Data.Rows` or a stale binding source when switching from one source type to another.

Acceptance criteria:

- Setting `DataSource` to a new source replaces prior rows cleanly.
- Switching from `DataSource` to `Uow` mode does not leave the grid in a partially bound state.
- `ClearGrid` returns the grid to a known empty baseline without stale selection/state.

### Phase 1.C — Theme + invalidation stabilization

Issue: the grid and base control both participate in theme application and invalidation; when those paths are triggered from multiple places, it is easy to update only part of the state and leave mismatches in background, border, or painting rectangles.

Files likely involved:

- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.Methods.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.Properties.cs`
- `TheTechIdea.Beep.Winform.Controls/GridX/BeepGridPro.Invalidation.cs`

Recommended change pattern:

1. Reduce duplicate invalidation triggers to a single, deterministic refresh path per state change.
2. Ensure theme application does not overwrite runtime-specific grid state unless deliberately requested.
3. Keep `SafeInvalidate`/layout recalculation as the formal contract for repaint activity.

Acceptance criteria:

- Theme changes produce a single coherent repaint event.
- Grid invalidation after a data-source change does not repaint stale rows or stale geometry.

## 3. API surface

N/A for this phase. The work is internal contract cleanup and behavior stabilization; no public API redesign is required.

## 4. Files to change

| Action | File | Lines | Risk |
|--------|------|-------|------|
| Modify | `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.cs` | ~40 | medium |
| Modify | `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.Methods.cs` | ~30-80 | medium |
| Modify | `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.LayoutManagement.cs` | ~12-90 | medium |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/BeepGridPro.Properties.cs` | ~54-158 | high |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Controllers/GridDataController.cs` | ~12-61 | high |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/BeepGridPro.Invalidation.cs` | ~1-200 | medium |
| New | `TheTechIdea.Beep.Winform.Controls.Tests/BeepGridProFoundationRegressionTests.cs` | ~100 | medium |

## 5. Error handling matrix

| Operation | Before | After |
|-----------|--------|-------|
| BaseControl override contract violated | Painting and layout drift silently occur | Guarded validation and explicit override handling |
| Grid DataSource/Uow switch leaves stale rows | Rows or selection persist unexpectedly | Rebind sequence resets stale state before binding |
| Theme refresh triggers partial repaint | Mismatch between state and visuals | Single invalidation route after coherent state update |

## 6. Backward compatibility

Dev only; no backward compatibility guarantees are needed for this internal stabilization phase.

## 7. Verification

Per-task gate:

- `dotnet test TheTechIdea.Beep.Winform.Controls.Tests --filter "BaseControl or BeepGridPro"`

Per-phase gate:

- Re-run the grid-focused regression suite.
- Confirm that the intentional exceptions remain unchanged: BaseControl base 001 rendering stays allowed, and `BeepGridPro` painting remains intentionally disabled.

## 8. Risks

| Risk | Mitigation |
|------|------------|
| Over-correcting by fixing the intentional exceptions | Explicitly preserve the intentional out-of-scope exceptions in design and tests |
| Rebinding logic still split across multiple helpers | Centralize rebind sequencing in one code path before patching |
| Theme refresh regressions on unrelated controls | Keep the changes focused on base foundations and grid state, not unrelated visual styles |

## 9. Out of scope

- BaseControl base 001 rendering behavior.
- BeepGridPro intentionally disabled painting behavior.
- Broad library-wide refactors outside the foundation and grid state contract.
- New feature work unrelated to foundation correctness or grid stability.

## 10. Sub-task execution order

1. **P1.A.1** — Confirm and document the BaseControl override contract. Verify: BaseControl tests and design review confirm the override hierarchy remains explicit.
2. **P1.A.2** — Harden the base paint/layout guards against drift. Verify: targeted BaseControl regression checks.
3. **P1.B.1** — Trace and fix the `DataSource`/`Uow` rebind sequencing in `BeepGridPro.Properties.cs` and `GridDataController.cs`. Verify: grid rebind test passes.
4. **P1.B.2** — Normalize `ClearGrid` and `RefreshData` state reset behavior. Verify: empty-grid and refresh tests pass.
5. **P1.C.1** — Consolidate invalidation/theme update ordering. Verify: theme + repaint regression checks pass.
6. **P1.M.1** — Run the focused validation suite. Verify: `dotnet test ... --filter "BaseControl or BeepGridPro"` passes.
