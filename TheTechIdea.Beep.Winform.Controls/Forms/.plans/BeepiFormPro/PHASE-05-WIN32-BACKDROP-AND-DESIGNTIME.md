# Phase 5 - Win32, Backdrop, And Design-Time Hardening

## Goal

Stabilize the low-level form lifecycle so custom non-client behavior, modern DWM effects, and Visual Studio design-time support stop competing with each other.

## Evidence From Current Code

- `BeepiFormPro.Win32.cs` returns early to `base.WndProc` in design mode, but also contains later design-time-specific branches inside runtime message handling.
- `BeepiFormPro.Backdrop.cs` applies multiple backdrop modes but does not clearly reset every path symmetrically when switching back to `None`.
- The backdrop interop helpers allocate unmanaged memory inside `try` blocks without a `finally`-style release guarantee if marshaling fails mid-path.
- `OnHandleCreated` in `BeepiFormPro.Win32.cs` performs DPI refresh, DWM non-client disable, backdrop application, and layout recalculation in one lifecycle step.
- `BeepiFormPro.cs` and `Designers/BeepiFormProDesigner.cs` both participate in design-time repaint orchestration, which risks duplicate or conflicting behavior.
- `WM_SIZE` in `BeepiFormPro.Win32.cs` updates only the window region, while the full layout, border-shape, managed-region, and repaint sync is deferred to `OnResizeEnd` and `WM_EXITSIZEMOVE`.
- The maximize actions in `BeepiFormPro.Core.cs` and `BeepiFormPro.Win32.cs` toggle `WindowState` directly without an explicit post-transition chrome refresh hook.

## Investigated Defect

### Maximize Or Restore Does Not Fully Repaint To The New Size

Likely root cause:

- Normal live resize is intentionally lightweight.
- The heavy refresh path lives in `RefreshChromeGeometryAfterBoundsSettled()`.
- That refresh is currently reached from `OnResizeEnd` and `WM_EXITSIZEMOVE`.
- Maximize and restore transitions go through `WindowState` changes and `WM_SIZE`, but `WM_SIZE` currently updates only the native window region.

Resulting risk:

- `CurrentLayout`, managed `Region`, cached border shape, and painter geometry can remain stale after maximize or restore.
- The form may show unpainted space, stale borders, or content not redrawn against the final maximized bounds until another repaint trigger occurs.

Current status:

- A targeted runtime fix has been applied in `BeepiFormPro.Win32.cs` so `WM_SIZE` triggers the full chrome resync for window-state transitions and non-normal bounds changes.
- The broader lifecycle cleanup in this phase is still needed so redraw, recalc, backdrop, and active-window logic become explicit and consistent instead of relying on one-off repairs.

## Scope

- Make runtime and design-time behavior use clearly separate decision paths.
- Harden backdrop interop, OS capability checks, and cleanup behavior.
- Centralize design-time repaint rules.
- Ensure handle creation, resize completion, and disposal all run symmetric cleanup/update logic.

## Deliverables

- One runtime Win32 flow and one design-time flow.
- Safe backdrop enable/disable helpers with consistent cleanup.
- A simplified designer refresh strategy.
- A handle lifecycle checklist covering create, resize, style change, theme change, and dispose.
- Explicit chrome redraw and chrome recalc operations that can be called independently.
- Active and inactive window-state repaint rules owned by one lifecycle path.

## Task Breakdown

- [ ] Remove unreachable or redundant design-time branches inside runtime message paths.
- [ ] Add an explicit maximize and restore refresh path that calls the same geometry sync used after resize settle, without reintroducing live-drag flicker.
- [ ] Decide whether the owning hook should be `WM_SIZE` with maximize-state filtering, `OnSizeChanged`, `OnClientSizeChanged`, or a dedicated `OnResize` transition guard.
- [ ] Wrap backdrop interop allocations in guaranteed cleanup logic.
- [ ] Add explicit disable/reset behavior for all supported DWM backdrop modes.
- [ ] Consolidate design-time repaint behavior between the form class and the designer class.
- [ ] Verify region updates are triggered only where necessary and never leave stale clip geometry behind.
- [ ] Add explicit active and inactive window transition handling for caption colors, borders, and non-client invalidation.
- [ ] Separate repaint-only operations from relayout and recomposition operations so future fixes do not over-invalidate.

## Risks

- Win32 cleanup mistakes can leave invisible border or caption regressions that only appear on specific Windows versions.
- Over-aggressive design-time invalidation can make the Visual Studio designer unstable again.

## Exit Criteria

- Runtime and designer message handling are clearly separated.
- Maximize and restore repaint the form correctly on the first transition.
- Backdrop transitions are symmetric and leak-safe.
- Resize, handle creation, and disposal no longer compete over region and non-client updates.