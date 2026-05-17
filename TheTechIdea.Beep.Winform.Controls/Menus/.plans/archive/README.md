# Menus Archive

This folder holds historical documents that predate the Menus & ContextMenus
revise/enhance program (see `Menus/.plans/MASTER-TODO-TRACKER.md`).

They are preserved for archaeology only. **Do not treat any of these as
active guidance.** The authoritative roadmap lives in the phase docs
alongside this folder; the authoritative styling contract lives in
`Menus/.plans/ADR-001-StylingStack.md`.

## Inventory

| File | Origin | Why archived |
|---|---|---|
| `2024-pre-revise-plan.md` | Originally `Menus/plan.md`. Aspirational refactor plan claiming "95% complete" for a `IMenuBarPainter` framework that was never wired into `BeepMenuBar` and was deleted in Phase 03 of the revise/enhance program. | Superseded by `MASTER-TODO-TRACKER.md` + the per-phase docs. |
| `FINAL_MENU_FIX_COMPLETE.md` | Originally `Menus/FINAL_MENU_FIX_COMPLETE.md`. Narrative of a previous "fix complete" milestone for menu rendering issues. | Useful as a snapshot of historical fixes; not current. |
| `MENU_COLOR_DEBUG_ANALYSIS.md` | Originally `Menus/MENU_COLOR_DEBUG_ANALYSIS.md`. Diagnostic write-up around theme colour resolution. | Diagnostics for an issue that was subsequently resolved; kept for reference. |
| `MENU_ENHANCEMENT_SUMMARY.md` | Originally `Menus/MENU_ENHANCEMENT_SUMMARY.md`. Pre-program summary of accumulated menu improvements. | Superseded by the per-phase `Menus-Phase-NN-*.md` docs which carry their own change manifests. |
| `MENU_STYLE_FIX_COMPLETE.md` | Originally `Menus/MENU_STYLE_FIX_COMPLETE.md`. Narrative of a previous style/theme fix milestone. | Same status as `FINAL_MENU_FIX_COMPLETE.md`. |

## When in doubt

If something here disagrees with `MASTER-TODO-TRACKER.md` or
`ADR-001-StylingStack.md`, **the tracker and ADR win**.
