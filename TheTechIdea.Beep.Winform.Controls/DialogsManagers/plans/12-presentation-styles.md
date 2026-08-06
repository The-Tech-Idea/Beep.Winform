# Stage 12 — Presentation styles from the reference images

**Kind:** conformance · **Depends on:** 06 (severity resolver), 07 (button hierarchy), 08 (body layout)

**Status: ◐ partial — the arrangements are built; the colour they also carry is the theme's.**
5 of 5 arrangement checks green; suite **42 passed / 1 failed**.

`DialogPresentation { TitleBar, Centred, HeroBand, Flat }` is on `DialogConfig`, defaulting to
`TitleBar` so nothing changes for a caller who never sets it. `Helpers/DialogPresentations.cs` applies
it by rearranging the grid the designer declared: the icon leaves its gutter for a row of its own
spanning both columns, text alignment changes, the actions move from the right edge to the centre, and
the hero and flat marks are enlarged.

Measured: the actions move from x=372 to x=255, and the icon from beside the title to above it and
centred. `TitleBar` is asserted unchanged, because a default that shifts is a regression.

**What is deliberately not here.** `dialog1.png`'s tinted header strip, `dialog4.png`'s saturated band
and the severity-coloured primaries are *colour*, and nothing in this folder colours anything — the
controls resolve their own from the theme. Making those appear means giving the theme the keys and
having the controls read them. This stage covers the geometry, which is the part the dialogs own.

**A measurement note.** The first version of the actions check compared the footer *panel's* centre,
which spans the full width in every presentation and reported 251 against 251 — a real behaviour
reported as a failure. It measures the buttons' own span now.


## Why this stage exists

Stages 06–08 treat the reference images as a single design and pull individual attributes out of them
— header tint, button colour, icon placement. Read together, `dialog1.png` … `dialog6.png` are not one
design with variations. They are **four distinct presentations of the same content**, and a dialog
system that can only render one of them cannot match the set.

The shell built during the layout pass renders exactly one: icon gutter, title row, content rows,
right-aligned actions. That is `dialog1.png`'s left column and nothing else in the folder.

## What the images actually show

Measured from the files in `Example_images/`, not from memory.

### A — Title-bar dialog (`dialog1.png`, both columns)

A tinted caption strip across the top carrying the title and a close affordance, then the body, then
actions at the bottom right.

- The strip's tint is the severity: neutral grey ("Dialog default"), green ("Positive dialog"), red
  ("Error dialog"), blue ("Action dialog"). The title text takes a darker shade of the same hue.
- Left column: white body, outlined circular icon at the left of the text block, `Secondary` +
  `Primary Action` at the bottom right.
- Right column: the **whole surface** takes the tint, there is no icon, and a single filled action
  sits at the bottom right. So the tint is not "header only" — it is a surface treatment with two
  intensities.
- The close glyph is present in all eight variants. Our dialogs set `ShowCaptionBar = false` and have
  no close affordance of their own.

### B — Centred confirmation (`dialog2.png`)

Icon centred above the text, title centred, message centred, actions centred as a pair of equal-width
buttons. The destructive action is filled red and sits **right** of the safe one ("No, keep it." then
"Yes, Delete!").

This is the layout Apple HIG and Material 3 both use for a destructive confirmation, and it is the
shape stage 04's typed confirmation belongs in.

### C — Hero band (`dialog4.png`)

A saturated band across the top third — green for success, red for failure — carrying a large centred
word ("Success!", "Whoops!") and a circular icon beneath it. The body is white, the message centred,
and a single outlined button is centred below it.

Note the inversion: here the *headline* lives in the coloured area and the message in the neutral
one, which is the opposite of A.

### D — Flat / oversized mark (`dialog6.png`)

A large flat check or cross occupying the upper half, a small bold label and message at the lower
left, and the action rendered as underlined uppercase text rather than a button.

## What this asks for

A `DialogPresentation` enum on `DialogConfig` — `TitleBar`, `Centred`, `HeroBand`, `Flat` — with
`TitleBar` the default, since that is what the shell renders today and no caller should change
appearance by upgrading.

The presentations differ in three things only:

| | icon placement | text alignment | action alignment |
|---|---|---|---|
| TitleBar | left gutter, spanning | left | right |
| Centred | above title, centred | centre | centre, equal widths |
| HeroBand | in the band, centred | centre | centre |
| Flat | upper half, oversized | left | left, text-style |

That is a property of **how the shell arranges its cells**, not of what a dialog contains. So this is
one change to `BeepDialogShell` — a presentation applied to the existing grid — and not four new
forms. Concretely: the icon moves between the gutter column and a spanning row above the title, and
`TextColumn` alignment plus the footer's anchor change with it.

`Flat` is the one that does not fit the grid cleanly, because its mark bleeds behind the text rather
than occupying a cell. It should be built last, and dropped if it costs more than it returns.

## Where the severity tint comes from

Stage 06 builds the severity resolver. This stage consumes it and adds nothing of its own — the tint
strip in A and the band in C are the same colour at two intensities, and both must come from
`BeepThemesManager`, not from constants. `IBeepTheme` already carries the per-severity dialog keys
(`DialogInformationButtonBackColor`, `DialogWarningButtonBackColor`, `DialogErrorButtonBackColor`,
and their hover siblings); the surface tint is those hues at low saturation.

**Do not hardcode the greens and reds out of the PNGs.** A theme-derived tint is the whole point, and
a screenshot is 8-bit sRGB after compression — sampling it produces a colour that belongs to no theme.

## Acceptance

Each is a render diff against the stage 11 corpus, plus one assertion that can fail:

1. `DialogPresentation.TitleBar` renders byte-identical to the current corpus. If it does not, this
   stage changed the default and that is a regression, not a feature.
2. For `Centred`: the icon's horizontal centre is within 1px of the title's, and of the message's.
   Fails today — the icon is in a gutter column, so its centre is left of both.
3. For `HeroBand`: the band's height is a fixed proportion of the dialog and its colour equals the
   theme's severity hue for the configured severity — not a literal.
4. Every presentation keeps the title and message sharing one left edge in the two left-aligned
   modes. This is the invariant the single-grid shell was built for, and a presentation that breaks
   it has reintroduced the nested-grid defect.
5. Switching presentation changes at least N distinct pixels between two renders of the same dialog.
   Without this the check passes when the enum is read and ignored — which is exactly the failure
   stages 03 and 05 exist to fix.

## What is explicitly out of scope

The close affordance visible in `dialog1.png`. Adding one interacts with `CloseOnEscape` (stage 02)
and the backdrop-dismiss policy (stage 03), and it is a dismissal decision rather than a presentation
one. It belongs with those stages, and this document should not be the reason it lands untracked.
