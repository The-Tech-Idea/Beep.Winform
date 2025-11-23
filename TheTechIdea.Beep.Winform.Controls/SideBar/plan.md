Plan: Sidebar Painters Enhancement
===============================

Overview
--------
This plan consolidates duplicated sidebar painters, adds new painter variants inspired by the provided images (PillRail and FinSet), and ensures each painter leverages the global theme system (`BeepThemesManager`) and the global control styling (`BeepStyling`). The goal is to keep a consistent, extensible painter architecture in `BeepSideBar` while updating the mapping in `InitializePainter()`.

Goals
-----
- Consolidate similar painters where duplication exists (Minimal vs NotionMinimal).
- Add new painters: `PillRail` and `FinSet` (a pill-shaped selected highlight/rounded selection variant).
- Use `BeepStyling`/`BeepThemesManager` / `BaseSideBarPainter` helpers to ensure theme and control style compliance.
- Add `plan.md` documenting the details and implementation steps.
- Update the painter mapping in `BeepSideBar.Painters.cs` to register new styles and remove / consolidate duplicates.

Steps
-----
1. Add Plan.md (this document) as part of `SideBar` directory describing the plan and rollout.
2. Create `PillRailSideBarPainter.cs` and `FinSetSideBarPainter.cs` in `SideBar/Painters/`:
   - `PillRail`: small, vertical rail, circular icon layout, top and bottom actions, and compact visuals matching the pill rail images.
   - `FinSet`: larger, full expanded sidebar with pill-shaped selected items, improved spacing and typography.
   - Both should use theme colors via `context.Theme` and use `BeepStyling` for secondary layout where appropriate.
3. Consolidate duplicates:
   - Replace `MinimalSideBarPainter` usage with `NotionMinimalSideBarPainter` to avoid duplicates (update `InitializePainter()` switch).
   - Remove `MinimalSideBarPainter.cs` file from Painters folder.
4. Update `BeepSideBar.Painters.cs` switch to add mappings:
   - `BeepControlStyle.PillRail` -> `Painters.PillRailSideBarPainter`
   - Add a `BeepControlStyle.FinSet` entry (if not present), and map it -> `Painters.FinSetSideBarPainter`
   - Map `BeepControlStyle.Minimal` -> `NotionMinimalSideBarPainter` to consolidate.
5. Ensure new painters call `BaseSideBarPainter` helpers and theme-conforming `BeepStyling` where applicable.
6. Add minimal inline tests or run-time checks (manual) to ensure no build errors and behavior as expected.

Notes and Edge Cases
--------------------
- I will avoid removing painter classes that may be referenced by other code unless the mapping is updated.
- Because `BeepStyling` switches depend on `BeepControlStyle`, if a new enum entry is added we must update `BeepStyling` background/text mapping to support it—however, to reduce change size we will reuse existing background mapping for `FinSet` and `PillRail` where appropriate.

Next Steps
----------
1. Implement `PillRailSideBarPainter.cs` and `FinSetSideBarPainter.cs` using existing painter patterns with theme support.
2. Update `BeepSideBar.Painters.cs` mapping accordingly and remove `MinimalSideBarPainter.cs`.
3. Build and test visually in the sample app (manual) to ensure styles behave as expected.

Change Summary
--------------
- Added: `FinSetSideBarPainter.cs`, `PillRailSideBarPainter.cs`
- Removed: `MinimalSideBarPainter.cs` (consolidated to NotionMinimal)
 - Removed: `MinimalSideBarPainter.cs` (consolidated to NotionMinimal and fully deleted from repository)
- Updated: `BeepSideBar.Painters.cs` mapping

Implementation Notes
--------------------
- Added `DefaultItemImagePath` to `BeepSideBar` with default `Svgs.Menu` fallback.
- `ISideBarPainterContext` now exposes `DefaultImagePath` so painters can access it.
- `BaseSideBarPainter.GetIconPath()` returns `item.ImagePath` or `context.DefaultImagePath` when `ImagePath` is empty.
- All SideBar painters updated to use `GetIconPath(item, context)` so default icons are drawn when no explicit icon is set.

Cleanup note:
-- The `MinimalSideBarPainter.cs` source file was removed and consolidated to `NotionMinimalSideBarPainter`. All references have been updated to point at `NotionMinimal` and the source file is no longer present in the repo.

Testing
-------
- Small demo added to `WinformSampleApp.Form1` showing `BeepSideBar` with some items missing `ImagePath` to validate the fallback behavior.

If you'd like, I can also add a small screenshot test harness or a sample form that demonstrates each painter style side-by-side.

---
Timestamp: 2025-11-23
