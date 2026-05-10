# BeepiFormPro Enhancement And Fix Plan

This planning set replaces the older single-file modernization narrative for `BeepiFormPro`.
It is based on a direct audit of the current source surface in `Forms/ModernForm`, including the core partial classes, Win32 and backdrop code, designer support, managers, painter contracts, painter factory, metrics cache, and painter implementations.

## Planning Intent

- Separate actual code debt from older docs that already marked large parts of the work as complete.
- Sequence the work so correctness and lifecycle issues are fixed before visual expansion.
- Keep painter individuality while reducing duplicated layout and interaction logic.
- Leave a durable tracker so future sessions can resume without losing phase status.

## Current Diagnosis

1. The runtime lifecycle is overloaded.
   The constructor in `BeepiFormPro.cs` mixes startup styling, region updates, child-control designer hooks, and global theme synchronization.

2. Theme and style state is not fully coherent.
   `Theme`, `ThemeName`, `CurrentTheme`, `PaintersFactory`, and `FormPainterMetrics` all participate in state changes, but they do not share one clear invalidation model.

3. Layout and hit-testing contracts are inconsistent across painters.
   Several painters register different hit-area names and duplicate caption/button geometry with different hardcoded values.

4. Win32, backdrop, and designer safety concerns are still mixed into the rendering flow.
   The code is trying to balance custom non-client behavior, design-time rendering, and modern DWM effects at the same time.

5. Documentation is ahead of reality.
   Existing docs describe the painter layer as fully complete, but the live code still contains unresolved contract, cache, and lifecycle work.

6. Maximize and restore appear to bypass the full repaint geometry path.
   `WM_SIZE` updates the window region, but the heavier refresh path is deferred to `OnResizeEnd` and `WM_EXITSIZEMOVE`, which are drag-resize oriented and do not reliably cover maximize or restore transitions.

## Phase Order

1. `PHASE-01-BASELINE-AND-CONTRACT-RESET.md`
2. `PHASE-02-THEME-CACHE-AND-STATE.md`
3. `PHASE-03-LAYOUT-HITTEST-AND-INPUT.md`
4. `PHASE-04-PAINTER-CONSOLIDATION.md`
5. `PHASE-05-WIN32-BACKDROP-AND-DESIGNTIME.md`
6. `PHASE-06-VALIDATION-DOCS-AND-SAMPLES.md`

## Validation Assets

- `DESIGNTIME-VALIDATION-MATRIX.md`
- `RUNTIME-VALIDATION-MATRIX.md`

## Audit Assets

- `CAPTION-BUTTON-WIDTH-AUDIT.md`
- `CAPTION-BUTTON-WIDTH-REFACTOR-PROPOSAL.md`

## Supporting Research

- `EXTERNAL-BENCHMARKS.md` captures relevant practices observed in Krypton Toolkit, MaterialSkin, DevExpress, Telerik, and Syncfusion.
- Use that note as a filter for candidate features, not as a reason to widen scope prematurely.

## Execution Rules

- Complete Phase 1 before making broad painter changes.
- Do not treat existing `plan.md` or painter summary docs as the source of truth for completion state.
- Prefer composition helpers and shared contracts over deep painter inheritance.
- Keep implementation changes grouped by phase so validation can be narrow and reversible.
- Update `TODO-TRACKER.md` whenever a phase starts, changes scope, or completes.

## Audit Anchors

The following files were the main evidence points behind this plan:

- `BeepiFormPro.cs`
- `BeepiFormPro.Core.cs`
- `BeepiFormPro.Drawing.cs`
- `BeepiFormPro.Events.cs`
- `BeepiFormPro.Theme.cs`
- `BeepiFormPro.Win32.cs`
- `BeepiFormPro.Backdrop.cs`
- `PaintersFactory.cs`
- `FormPainterMetrics.cs`
- `Designers/BeepiFormProDesigner.cs`
- `Painters/IFormPainter.cs`
- representative painter implementations including `MinimalFormPainter.cs`, `ModernFormPainter.cs`, and `MacOSFormPainter.cs`

## Working Note

This is a planning baseline only. No runtime fixes are included in this document set yet.