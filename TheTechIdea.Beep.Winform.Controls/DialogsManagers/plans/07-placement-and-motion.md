# 07 — Placement & Motion

**Priority P1. Phase 4.**

## `DialogPlacementEngine` has zero callers

```csharp
// Helpers/DialogPlacementEngine.cs:7
internal static class DialogPlacementEngine
{
    public static Point Place(Form owner, Size size, DialogPlacementStrategy strategy)
```

Searched across the assembly: **no reference outside its own file.** Meanwhile there are **17** sites
setting `StartPosition` / `CenterParent` / centring by hand across the manager and the forms.

This is the seventh instance in this codebase of complete, plausible code that nothing calls. The
`BeepTabs` program found six — `BeepTabInputPolicy`, `BeepTabAccessibleObjectFactory`, the touch API,
`BeepTabRtlLayoutHelper`, `TabColorConfig`, `TabStyleConfig` — and three of those were duplicates of
a live seam that would have fought it had they ever been connected. The same question applies here
and has not yet been answered: **does `DialogPlacementEngine` agree with the 17 hand-rolled sites, or
would wiring it in change where dialogs appear?**

That question is answerable by measurement and must be answered before the engine is wired in.

`DialogMotionEngine` (114 lines) and `DialogMotionProfile` are in the same position and have not been
audited.

## What the reference products do

- **Radix / Floating UI** — one placement pipeline with ordered middleware
  (`offset → flip → shift → size → hide`); nothing positions a surface outside it.
- **Material 3 / Fluent 2** — dialogs are centred on the *window*, not the screen, and clamp to the
  viewport with a margin. Sheets dock to an edge through the same code path.
- **VS Code** — modals centre on the workbench; the Quick Pick anchors to the top-centre. Both go
  through one positioning service.

The `ToolTips` program in this repo found **three** placement engines that disagreed, and the fix was
a single `Resolve(anchor, size, placement, offset, viewportPadding)`. That is the target shape here.

## Work

1. **Establish agreement first.** For each of the 17 sites, compute where it puts a dialog and where
   `DialogPlacementEngine.Place` would put it. Record the differences before changing anything —
   wiring in an engine that disagrees would silently move every dialog.
2. **Route all placement through the engine**, or delete it if the measurement shows it is wrong and
   the scattered logic is right. One of the two must happen; both cannot stay.
3. **Handle the multi-monitor and clamp cases explicitly** — a dialog centred on an owner that
   straddles two monitors, and a dialog larger than the work area. These are the cases hand-rolled
   centring always gets wrong and a placement engine exists to get right.
4. **Audit `DialogMotionEngine`** for callers on the same basis.

## Verification

- ⬜ Probe: for each placement strategy, assert the dialog lands fully inside the owner's work area.
- ⬜ Probe: owner straddling two monitors — assert the dialog does not split across the seam.
- ⬜ Probe: dialog larger than the work area — assert it clamps rather than positioning off-screen.
- ⬜ Harness: no `StartPosition` or manual centring outside the placement engine.
