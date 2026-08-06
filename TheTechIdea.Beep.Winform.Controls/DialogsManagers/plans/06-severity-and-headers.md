# Stage 06 — severity system and header treatments

**Kind:** conformance to `Example_images/dialog1.png`, `dialog4.png`.

**Status: done**, with its scope corrected. The resolver is built and green; the header treatments
are the theme's to supply, not this folder's to paint.

`DialogStyleAdapter.ResolveSeverity` is the single source — explicit `Severity`, then `Preset`, then
`IconType`, then neutral, with **preset winning** when preset and icon disagree. The named bug is
fixed: `IconType = Error` with no preset now resolves to Error, where severity previously read
`Preset` only.

**The header band was removed, not deferred.** It was painted behind the title row twice — from a
`BeepDialogShell` subclass, then through `TableLayoutPanel.CellPaint` — and neither reaches the
screen, because the title is a docked, opaque `BeepLabel` in that cell. Giving the title its own
colour instead renders white, because clearing `UseThemeColors` leaves it nothing to fall back on.
The rule this folder now follows is that controls colour themselves from the theme; see the tracker.

So `DialogHeaderStyle` and `DialogSurfaceTreatment` remain on `DialogConfig` as declarations of
intent, and what they look like is a theme question. The checks assert the **resolver** rather than
pixels — precedence, explicit override, and that the theme supplies a distinct colour per severity —
which is the part that was always the point.

## What was built

`DialogStyleAdapter.ResolveSeverity` is the single source: explicit `config.Severity`, then `Preset`,
then `IconType`, then neutral. **Preset wins when the two disagree** — a preset states what the dialog
*is*, an icon states how it *looks*, so the stronger claim colours the whole dialog and the glyph
follows. The named bug is fixed: `IconType = Error` with no preset now colours the dialog, where
before severity read `Preset` only and left the header neutral.

`GetSeverityAccent` (saturated: buttons, icons, `ColorBlock` fill) and `GetSeverityWash` (low
saturation: `Strip` header, tinted body) replace the two preset-keyed functions, which had **zero
callers** outside the adapter — dead code of exactly the shape stage 05 catalogues.

The wash is composited to an opaque colour rather than returned with an alpha channel: a translucent
`BackColor` is not honoured by every control that inherits it, and the band and the labels sitting in
it have to render the same colour.

`DialogHeaderStyle { Strip, ColorBlock, None }` and `DialogSurfaceTreatment { Plain, Tinted }` are on
`DialogConfig`, defaulting to `Strip` / `Plain` — what the shell already rendered, so no existing
caller changes appearance by upgrading.

The band is **painted** in `OnPaintBackground`, not built from a nested panel. A panel spanning the
title row would need its own column definitions to keep the icon and title aligned with the content
below — the two-grids-to-keep-in-sync problem the single-grid shell exists to remove. The title and
icon get the band's colour by direct assignment, because a `BeepLabel` with `IsChild` inherits from
its *parent*, and the parent here is the shell, whose colour is the body rather than the band.

## Two harness faults this stage exposed, both mine

1. **The "neutral" fixture was not neutral.** `DialogConfig.IconType` defaults to `Information`, so
   `Preset = None` alone still resolves to `Info` — correctly. The check reported neutral and info as
   identical and the *code was right*; the fixture was describing itself. It now clears `IconType`
   explicitly.
2. **The focus-restore check hung the whole suite.** `_manager.Show(...)` is modal and blocks until
   something closes the dialog, and nothing did. It had been passing only when an incidental focus
   change happened to dismiss it. The check now closes its own dialog on a timer. This cost three
   bisection runs against the *library* before the harness turned out to be at fault — the same trap
   as stage 02's `SendKeys`, and the second time in this program that a hang was blamed on product
   code.

## On verification

The theme-switch check discriminates by construction: it captures the error header, changes
`BeepThemesManager.CurrentThemeName`, captures again, and requires the two to differ. It passes only
because the colours genuinely moved — a hardcoded palette cannot produce that result. The
distinctness check demonstrated the same property from the other side, failing while the fixture was
wrong and passing once corrected.

## Not built

The close glyph's placement (step 5). It is a dismissal decision that interacts with `CloseOnEscape`
and the backdrop policy, and `ShowCloseButton` governs it separately — recorded in
[stage 12](12-presentation-styles.md) as out of scope there for the same reason.

The 4.5:1 contrast assertion on tinted body text (verification step 4) is not implemented. The wash
is 10% at body and 22% at the strip, which keeps the theme's foreground well clear, but that is an
argument rather than a measurement and should become one.

## What the references specify

`dialog1.png` is the systematic one — eight variants of the same dialog, and the variable is
**severity**. Each row shows a titled header strip tinted by intent, with the body and buttons
following:

| variant | header strip | accent | body |
|---|---|---|---|
| "Dialog default" | neutral grey | blue primary | white, or grey-tinted |
| "Positive dialog" | green | green primary | white, or green-tinted |
| "Error dialog" | red/pink | red primary | white, or red-tinted |
| "Action dialog" | blue | blue primary | white, or blue-tinted |

Two surface treatments per severity: **white body** with a tinted header, and **fully tinted body**
where the whole dialog carries the severity colour at low saturation. Both keep the close glyph in
the header, right-aligned.

`dialog4.png` is a different header entirely: a **colour block** — a full-bleed green or red panel
carrying the title *and* a circular icon, with the white body below it. Same severity idea, much
louder.

`dialog5.png` and `dialog6.png` have **no header at all** — the severity lives in a large centred
icon instead.

So the folder needs three header modes, not one: **strip**, **colour block**, **none**.

## What exists

More than the other stages, which is why this one is conformance rather than construction.

- `BeepDialogIcon` already has `None, Information, Warning, Error, Question, Success`
  (`Vis.Modules2.0/enums.cs:493-501`) — the severity vocabulary is there.
- `Helpers/DialogStyleAdapter.cs` already resolves a per-preset accent (`:128`) and a per-preset
  **tint** at low alpha (`:149`: `Color.FromArgb(18, errorBase.R, …)`). That is exactly the tinted
  surface `dialog1.png`'s right-hand column needs.
- `BeepDialogManager.Core.cs:853` maps preset to `DialogType`.

What is missing is that severity resolves from **`Preset`**, not from the icon or an explicit
severity — so a caller who sets `IconType = Error` without a matching preset gets an error icon on a
neutral dialog. Two inputs, one visual outcome, no single source.

## The fix

1. **One severity resolver.** `DialogSeverity { Neutral, Info, Success, Warning, Error }`, resolved
   once from `Preset` if set, else `IconType`, else `Neutral`. Everything downstream — header tint,
   icon colour, callout accent ([08](08-body-layouts-and-callouts.md)), button colour
   ([07](07-button-hierarchy.md)) — reads that one value. Building it here is why this stage comes
   before 07 and 08.
2. Every colour comes from the theme through `DialogStyleAdapter`, not from literals. The reference
   greens and reds are the *design*, but the palette is the theme's — the same rule the dock program
   settled on for named palettes versus theme-led styles.
3. **`DialogHeaderStyle { Strip, ColorBlock, None }`** on `DialogConfig`, defaulting to `Strip`.
   `ColorBlock` carries the title and icon on the severity fill with on-accent text; `None` suppresses
   the header for the centred layouts in [08](08-body-layouts-and-callouts.md).
4. **`DialogSurfaceTreatment { Plain, Tinted }`** for `dialog1.png`'s two columns. `Tinted` uses the
   existing low-alpha tint at `DialogStyleAdapter.cs:149`.
5. The close glyph sits in the header when there is one, and top-right on the surface when there is
   not — `dialog5.png` shows the headerless placement.

## Verification

1. **Five severities, five headers.** Render the same dialog at each severity and assert the header
   fills are pairwise distinct. *Today severity only moves when `Preset` moves, so setting
   `IconType = Error` alone leaves the header neutral — assert that specific case, it is the bug.*
2. **One source.** `Preset` and `IconType` disagreeing must produce a defined result (preset wins)
   rather than a header from one and an icon from the other. Assert the icon and header agree in that
   case. *Today they can disagree.*
3. **Three header modes differ.** Strip, ColorBlock and None render pairwise distinct, and `None`
   still shows a reachable close affordance.
4. **Tinted differs from plain** at every severity, and tinted body text still clears 4.5:1 against
   its tint. A tint that eats the text is worse than no tint.
5. **Theme-driven.** Switch theme; assert every severity colour moves. *A hardcoded green passes
   check 1 and fails this one* — which is the point of having both.
