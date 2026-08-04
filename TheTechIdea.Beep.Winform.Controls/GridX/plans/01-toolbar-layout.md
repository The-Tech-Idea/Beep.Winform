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

## Still open

- [ ] `ShowFilterButton` defaults to **false**, so the funnel never appears and only the advanced
      (cogs) button is shown. That is a product decision rather than a defect, but it means the
      filter affordance most users look for is absent by default.
- [ ] The painter's search text rect should share the editor's right inset, so painted and edited
      text cannot disagree at the right edge.
- [ ] Separators are computed (`Separator1X`, `Separator2X`, `Separator3X`) — worth confirming the
      painter draws all three, since none were visible in the renders.
