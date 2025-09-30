# BaseControl Painter-Only Refactor Plan

Objective
- Make painters the single source of truth for layout and rendering.
- Remove the need for `ControlPaintHelper` and `BaseControlMaterialHelper` from the runtime rendering pipeline.
- Ensure each painter defines: its layout (DrawingRect/ContentRect/BorderRect), background, borders, state layers, icons, labels, helper text, and main text.
- Integrate hit testing and input via `ControlHitTestHelper` and `ControlInputHelper` without triggering duplicate drawing.

Important convention
- `DrawingRect` must represent the inner drawing rectangle equivalent to `BaseControl.DrawingRect` for controls inheriting `BaseControl` (used by derived controls for content placement). Painters must set this accordingly in `UpdateLayout`.

Phased Plan

Phase 0 — Groundwork (no visual change)
Status: Done

Phase 1 — Expand painter contract (interface)
Status: Done

Phase 2 — Classic painter layout (move from ControlPaintHelper)
Status: Done (classic painter now computes rects and draws background/shadow/borders/labels/icons)

Phase 3 — Material painter layout/draw (merge BaseControlMaterialHelper)
Status: Done (material painter owns layout, elevation, state, borders, icons, label/supporting)

Phase 4 — Card painter
Status: In progress (rects exposed; still uses internal layout; text drawn centrally by BaseControl)

Phase 5 — NeoBrutalist painter
Status: In progress (rects exposed; still uses internal layout; text drawn centrally by BaseControl)

Phase 6 — BaseControl orchestration only
Status: Done (GetPreferredSize delegates; DrawingRect mirrored from painter; main text uses painter.ContentRect)

Phase 7 — Remove helpers from runtime paths
Status: Done
- Removed construction/usage of `BaseControlMaterialHelper` from BaseControl runtime code paths
- Removed `ControlPaintHelper.Draw()` usage from BaseControl paint pipeline
- ControlPaintHelper kept only for shared utilities (rounded path, gradients)
- BaseControl.Material.cs updated to use painter-based approach exclusively
- _materialHelper field kept for binary compatibility but remains null

Phase 8 — QA and cleanup
Next:
- Visual parity checks across states and painters.
- Remove legacy `DrawMaterialContent` and dead code.
- Update documentation to reflect painter-only architecture
