# 01 — Painter Contract

**Priority P0.** Partially done — see the correction below.

## Correction to this document's first version

The first version of this document stated that `ITabPainter.PaintTab` was dead with zero callers,
and proposed deleting it from the interface and all seven painters.

**That was wrong, and acting on it would have broken every painter.**

`BaseTabPainter.PaintTabItem` calls `PaintTab` directly:

```csharp
// BaseTabPainter.PaintTabItem
PaintTab(g, itemLayout.Bounds, item.Index, isSelected, isHovered, alpha);
DrawAdornments(g, itemLayout, alpha);
```

The call has **no receiver** — it is virtual dispatch on `this` — so a text search for `.PaintTab(`
finds nothing. `PaintTab` is in fact the per-style extension point: all seven painters override it,
and it is the only thing most of them override.

Two things led to the wrong conclusion, and both are worth recording:

1. **The source comment said so.** `PaintTab` was documented as *"Legacy paint overload – used by the
   current BeepTabs shell"* and `PaintTabItem` as a *"Phase 2 paint overload"*. A maintainer
   following those comments deletes the method the whole painter set depends on.
2. **A receiver-only text search cannot find a call on `this`.** The harness made the identical
   mistake, then made the opposite one when "fixed" with a negative lookbehind, which excluded every
   receiver-prefixed call instead. Text matching cannot distinguish declaration, override and
   invocation without parsing C#.

The reliable method is the one this repo already established during the BeepTree work: **delete the
member and let the compiler enumerate the truth.** That is now how this check is run, and the
harness reports mention counts as *informational* rather than asserting deadness it cannot measure.

## What was actually dead — DONE

`ITabPainter.PaintBackground` had no callers at all: its only occurrences were its own declaration
and the `BaseTabPainter` implementation. The `OnPaintBackground` hits nearby are the unrelated
WinForms override.

Removed from `ITabPainter` and `BaseTabPainter`; **the solution compiles with zero errors**, which is
the proof — not the grep.

## Documentation corrected — DONE

The misleading comments are replaced with what the members actually are:

| Member | Now documented as |
|---|---|
| `PaintTab` | the per-style extension point; where a painter expresses its style; explicitly *not* legacy, with a note on why it looks uncalled |
| `PaintTabItem` | the entry point the header host calls; delegates the shape to `PaintTab`, then draws adornments |
| `MeasureTab` | the measure side, carrying the rule that its font must be the font `PaintTab` draws with |

## Measure/draw font divergence — DONE

`BaseTabPainter` violated the very rule its own `MeasureTab` doc states:

```csharp
// MeasureTab  — sizes the tab
Font baseFont = TabFontHelpers.GetTabFont(Theme, item.IsSelected);

// DrawTextInBounds — paints the title
TextRenderer.DrawText(g, text, SystemFonts.DefaultFont, bounds, textColor, ...);
```

Consequences, all of which were live:

- theme fonts never reached a drawn tab title;
- a **selected** tab was measured with the selected (bold) font and drawn regular, so its width did
  not match its content;
- any theme whose font differs from the system default produced tabs either too wide or clipped.

`DrawTabItemContent` already resolved the correct font into `baseFont` one line above the call — it
just was not passed. `DrawTextInBounds` now takes the font as a parameter, documented as *must be the
font `MeasureTab` measured with*. The unused `isHorizontal` parameter it also took was removed.

The harness now fails if any painter names a system font as its draw font, so this cannot come back.

## Remaining work

1. **Two text renderers still exist.** `BaseTabPainter` has both `DrawTabText` (handles vertical tabs
   by rotating, resolves the theme font correctly) and `DrawTextInBounds` (horizontal only, bounds
   supplied). That is the duplicate-implementation pattern again and should be resolved once the
   contact sheet in [10](10-theming-and-painters.md) shows what each style actually needs — including
   whether vertical tabs render text rotated at all.
2. **Assess whether two override points are enough.** Each painter overrides `PaintTab` plus one
   other member. Whether a capsule, a card and an underline tab can all be expressed that way is
   answered by rendering them side by side, not by reading — see
   [10](10-theming-and-painters.md).
2. **Enforce the measure/draw font rule** in the harness, not only in a comment.

## Verification

- ✅ `PaintBackground` removed; solution builds with 0 errors.
- ✅ Harness: no bare catches, no stubs, no empty directories, no dead model surface.
- ⬜ Contact sheet proving the seven painters render distinctly ([10](10-theming-and-painters.md)).
