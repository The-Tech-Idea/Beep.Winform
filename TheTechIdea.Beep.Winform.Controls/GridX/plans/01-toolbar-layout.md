# 01 — Toolbar layout, search box, and what hides when

## Measured before changed

The toolbar was rendered at 1200, 820, 560, 380, 300, 240 and 200px and its computed rectangles
dumped, rather than reasoned about from the source. Three of the reported problems were real and one
widely-assumed one was not.

### The right-hand cluster ran off the end of the control

The worst of it, and invisible in a screenshot because the overrun is outside the control:

```
200px toolbar:  advanced = {X=186 .. 204}     overflow chevron = {X=210}
240px toolbar:  overflow chevron = {X=240}
```

A 200px toolbar drew its advanced button past its own right edge and its chevron 10px beyond that.

The cause was structural. `reservedRight` was computed correctly and used to derive a `rightLimit`
for the flexible sections — but the cluster itself was then positioned by the same running `x` that
the title and search advanced. Once those hit their minimums, `x` passed the reservation and nothing
stopped it.

**The cluster is now laid out right-to-left from `bounds.Right`.** Anchoring makes overrunning
impossible: whatever survives is the middle's budget, and if that is not enough the middle collapses
rather than the toolbar bleeding past its own bounds.

### Adjacent icon buttons had different hit heights

```
advanced / filter : 18 x 18
export buttons    : 18 x 32
```

Both were individually centred, which is why the code looked right, and they still read as
misaligned — the hit targets differ by 14px and an 18px-tall target is well under a comfortable
minimum. Every button in the cluster now gets the same box: icon-wide, full band height.

The width stays at the icon deliberately. A previous pass tried a 28px minimum and recorded that it
padded the whole strip out; that note is still true, so only the height was changed.

### Nothing ever hid

At every width from 1200 down to 380 the layout reported `IsOverflow=False` for every button and an
empty `OverflowButtonRect`. The export section was reserved from its visible count before anything
else was placed, so it always got its space and only the title and search absorbed the pressure —
the title shrinking from 131px to 127px across a three-fold width change while the search box was
crushed to its minimum.

Now the collapse order is real and each step is a defined outcome:

| pressure | what gives |
|---|---|
| plenty | title, search at max 300, all buttons |
| less | search shrinks toward its minimum |
| less still | title drops entirely once it cannot hold its 70px floor |
| less still | export buttons overflow **from the tail** into the chevron |
| least | search box disappears below 72px rather than becoming an unusable stub |

Overflowing from the tail needed care: placement is right-to-left, so the obvious loop kept the
*last* declared buttons and pushed the first into the chevron. Which buttons survive is decided
left-to-right, and only the placement runs backwards.

### The search box and its editor were already aligned

This was the reported problem I could not reproduce. The painter and `FilterEditorHelper` both inset
their text by `SearchIconWidth * dpiScale`, and the icon is laid out inside the box occupying exactly
that inset. They agree by construction.

There is one genuine but minor asymmetry left: the painter's text rectangle extends to
`bounds.Right` while the editor stops short by the corner radius, so painted text and edited text can
differ by a few pixels at the extreme right of a full field. Recorded rather than changed, because it
only shows with text long enough to reach the arc.

## Verified

Rectangles at every width are inside the control, and the degradation sequence is as tabulated:

```
1200px  title 131, search 300, advanced+3 exports, no chevron
 300px  title dropped, search 183, advanced+3 exports
 200px  title dropped, search 100, advanced+import, export/print in chevron at {172,3,18x32}
```

Solution builds with 0 errors. The three failing toolbar tests
(`ToolbarState_ActionButtons_Are_Visible_By_Default`,
`SetToolbarButtonVisible_Unknown_Key_Is_NoOp`, `ToolbarColor_HaveDefaultValues`) fail identically
with the change stashed — they are pre-existing, and concern visibility defaults and colours rather
than layout.

### Separators were computed and not drawn

`Separator1X` divides the middle of the toolbar from the filter cluster — and was gated on the
**action buttons** being visible. Those are hidden by default, so on a default toolbar the line
between the search box and the filter buttons was never drawn: the one place a separator is most
obviously wanted. Each separator is now gated on what it actually divides.

Its vertical extent came from the actions section, falling back to the search section. That fallback
became wrong the moment the search box could collapse — an empty rect gave `top == bottom == 0`, so
the separators would have been drawn as nothing in the top-left corner. The extent now comes from the
advanced button, which is the one element always placed.

### The search text area is single-sourced

The painter and the editor each computed the inner text rectangle, and disagreed at the right edge:
the painter ran to the box's edge while the editor stopped short of the corner arc, so text shifted
by a few pixels at the moment the editor opened or closed. Both now call
`BeepGridToolbarPainter.SearchTextArea`.

### The inline editor was positioned from the previous layout

Reported: after minimizing or maximizing, the search editor is on screen at the wrong place. Two
separate causes, and the second is the one that mattered.

**It was abandoned rather than put away.** `ResizeIfActive` returned early when the search box
rectangle was empty. Minimizing collapses the client area, the toolbar layout resets, the rectangle
goes empty - and the editor was left visible wherever it last sat. The same case now arises
legitimately when the toolbar is too narrow to keep the box. There is now a `SuspendForLayout` that
hides it and carries the typed text into `ToolbarState.SearchText`, so nothing is lost. It is
deliberately not `HideSearchEditor`, which treats a hide as the user cancelling and hands focus back
to the grid - both wrong when the window is simply minimizing.

**It was moved one layout too early.** The toolbar is laid out during *paint*
(`GridRenderHelper.Rendering`), but the editor was repositioned from `OnResize`, which runs before
that. So it was always placed from the previous geometry. Barely visible on a small drag; badly wrong
on a maximize, where the box moves across the screen:

```
maximized, before:  editor {X=594}   search box {X=3010}
maximized, after:   editor {X=3034}  search box {X=3010}
```

The editor is now synced immediately after the layout pass that positions the box, and `OnResize`
does not touch it. The call sits outside the paint block's `catch`, because failing to place a child
control is not a paint failure and should not be swallowed as one.

Asserted as an invariant rather than as the symptom - **editor visible implies a search box to sit
in** - across open, minimize, restore, maximize and a width narrow enough that the box is dropped on
purpose: 9 checks, all passing.

### The funnel is shown by default

`ShowFilterButton` defaulted to false, on the reasoning that the Advanced button and the per-column
header icons covered every case. In practice the funnel is the affordance users look for, and a
toolbar whose only filter control is a cogs icon reads as having no filtering at all. It is now on by
default; hosts wanting the sparser toolbar set `ShowFilterButton = false`, which is asserted
separately so the default is not the only supported value.

```
funnel   {X=879,Y=3,Width=18,Height=32}
advanced {X=901,Y=3,Width=18,Height=32}
```

Same band height, funnel to the left of advanced, both inside the separators.

`BeepGridProToolbarTests.Filter_Button_Is_Hidden_By_Default` encoded the old intent and was updated
rather than worked around - a deliberate behaviour change should move the test that documents it.

### Paint failures are reported instead of vanishing

Every one of the eight catches in `GridRenderHelper.Rendering` swallowed silently. The one that
logged used `Debug.WriteLine`, which is compiled out of Release - silent exactly where it matters.
A failure in any section left that band of the grid blank with nothing to say why.

`BeepGridPro.RenderError` now carries `{ Section, Exception }`, and all eight sections report through
it: Toolbar, TopFilterPanel, ColumnHeaders, Rows, Navigator, Selection, DragFeedback,
FocusIndicator. With no subscriber the behaviour is unchanged, so it cannot destabilise an existing
host.

**They still absorb, deliberately.** An exception escaping a paint handler does not reach the
caller: it travels the window procedure to `NativeWindow.Callback` and WinForms raises it as
unhandled, which becomes a modal dialog - on a minimized, off-screen or mid-drag window, an
invisible and unclickable one, so the application appears to hang. That mechanism cost three
misdiagnoses in the Docking program. Absorbing is right; absorbing *silently* was not.

Verified by causing a failure rather than by reading the catch - a channel that exists and is never
reached looks identical to one that works:

```
healthy paint:                0 reports
broken toolbar painter:       1 report -> Toolbar: NullReferenceException
grid still alive afterwards:  yes
```

## Still open

- [ ] 29 bare catches remain elsewhere in `GridX` outside the render pipeline. The render path is
      the one where silence is most costly, and it is now covered; the rest are worth a sweep.
