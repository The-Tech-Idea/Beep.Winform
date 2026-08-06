# Stage 06 — The field paints itself; the popup hand-positions

**Kind:** refactor · **Files:** `BeepListofValuesBox.Composition.cs`, `BeepLovPopup.cs` · **Status: done.**

## The field is composed

`PaintValueArea` drew the display value and its key badge straight onto the control: measuring text,
building a rounded path, filling it, and picking a contrasting foreground by luminance. All of that is
what a `BeepLabel` does by existing.

The field is now a three-column `TableLayoutPanel` — **key box | badge | display value** — positioned
into `GetAdjustedContentRect`. Not docked: `BaseControl` owns the border, background and the trailing
dropdown icon, and docking over it would cover the icon that opens the popup.

Removed with the paint pass: `PaintValueArea`, `BuildRoundedPath`, `ScaleLogicalX`, `ScaleLogicalY`, the
`DrawContent` override, and the `Draw(Graphics, Rectangle)` override.

**`Draw` deserves a note.** It is `BaseControl`'s extension point for rendering an *unparented* control
into a rectangle, which a composed control cannot do — its children are what render. `AppBars` uses that
extension point for its own components; nothing in the solution rendered a LOV that way, so removing it
costs nothing. A composed control renders through `DrawToBitmap` instead.

### The 22 % split is computed once

`(int)(width * 0.22)` was evaluated independently in `AdjustLayout` and again in `PaintValueArea`. The
key box and the value area it sat beside lined up only because the two expressions happened to be
identical — a drift waiting for one of them to be edited. It is one `KeyColumnPercent` constant on one
`ColumnStyle` now.

### What composition buys here

The displayed value is a control, so it is in the accessibility tree and a test can read it. The
placeholder is set on the label while `SelectedDisplayValue` stays empty, so a caller reading that
property never gets `"Select a value…"` back as though it were data.

## Verification

Four controls: `TableLayoutPanel`, `BeepTextBox`, and two `BeepLabel`s. A control carries `Research`;
the badge is its own label; `ShowKeyBadge` toggles it off **and back on**; an empty field shows the
placeholder while reporting no display value.

**The badge check was wrong first.** It matched any visible control whose text was the key — and the
key *text box* carries the key by design, so it reported the badge present whether or not it was.
Counting visible `BeepLabel`s carrying the key is the discriminating version, and toggling back on is
what stops a fix that simply never shows the badge from passing.

## The popup is composed too

`PositionHeaderControls` and `PositionFooterControls` set bounds by arithmetic on every resize — close
button width subtracted from panel width, search width derived from that, buttons walked leftwards from
the right edge. Both are gone:

| | |
|---|---|
| header | `TableLayoutPanel`, 2 columns — search `100%`, close `AutoSize` |
| footer | `TableLayoutPanel`, 3 columns — count `100%`, cancel and OK `AutoSize` |

`_countLabel` was a plain `Label` with a hand-set `ForeColor` and `Color.Transparent` background; it is
a `BeepLabel` now and themes itself with the other children.

`_loadingOverlay` keeps its manual positioning on purpose. It is an overlay that has to cover the grid
region, not a participant in the layout — docking it would make it a row.

## Verification

Field: four controls (`TableLayoutPanel`, `BeepTextBox`, two `BeepLabel`s); a control carries
`Research`; the badge is its own label; `ShowKeyBadge` toggles off **and back on**; an empty field
shows the placeholder while reporting no display value.

Popup: two `TableLayoutPanel`s in the tree.
