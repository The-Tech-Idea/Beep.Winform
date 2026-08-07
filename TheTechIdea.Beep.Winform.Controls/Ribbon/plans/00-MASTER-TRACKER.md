# Ribbon — review tracker

`BeepRibbonControl` is 18 partial files plus Accessibility, Backstage, Customization, Gallery,
Rendering, Search, Tokens and Tooltips subfolders. 42 files, ~11,300 lines after the deletions below.

## Done

### Two full backup copies of the main control were in the source tree

`BeepRibbonControl.cs.backup` and `BeepRibbonControl.cs.bk` — byte-identical, 5,410 lines each,
10,820 lines between them, against 11,292 lines of live code. Both were tracked by git and referenced
by nothing. Deleted: `CLAUDE.md` rule 2 says delete the old thing rather than keep it beside the new
one, and git already holds the history.

They were not harmless. Both still contained the pre-split implementation, so a search across the
folder returned two stale hits for every real one — which is how they were found: a scan for silent
catches reported 34 sites, 20 of them in files that are never compiled.

### 15 silent catches now report through `BeepLog`

Every one was a bare `catch { }` or a `catch` whose body was only a comment. By file:

| file | sites | what was being hidden |
|---|---|---|
| `.Events.cs` | 4 | theme subscribe/unsubscribe, theme application, design-time build |
| `.Customization.cs` | 4 | save/load customization, save/load theme tokens |
| `.Search.cs` | 4 | async search, provider failure, save/load history |
| `.QuickAccess.cs` | 2 | save/load the quick access toolbar |
| `.Backstage.cs` | 1 | a consumer-supplied timestamp formatter throwing |
| `RibbonTheme.cs` | 1 | reading the current Beep theme |

Chosen deliberately per site rather than mechanically:

- **`Failure`** where the operation is lost — a save that writes nothing, a theme that never applies.
- **`Fallback`** where a degraded path really does succeed: the design-time placeholder, the built-in
  timestamp format, the local search index standing in for a failed provider.
- **`FallbackOnce`** in `RibbonTheme.SyncFromBeepTheme`, which runs on every theme resolve. The
  fallback is legitimate during early startup before the theme manager has a current theme, so a
  per-call message would bury a real failure.

Three were worth more than a mechanical fix:

- **`UnsubscribeThemeManager`** was commented `// no-op`. `BeepThemesManager.ThemeChanged` is static,
  so a failed detach keeps the ribbon alive for the life of the process. That is a leak.
- **`TrySubscribeThemeManager`** was commented `// best effort only`. On failure `_subscribedToThemeManager`
  stays false, so every later call retries and fails identically, and the ribbon never follows a theme
  change again.
- **The search provider catch** already raised `providerFailed: true` on its event. A boolean says
  only *that* it failed; the provider is the consumer's own code, so it now gets the exception too.

Verified: the multiline pattern `catch\s*(\([^)]*\))?\s*\{\s*(//[^\n]*\s*)*\}` finds 0 in `Ribbon/`
and still finds 20 in `SideBar/`, so the zero is a result rather than a pattern that matches nothing.

A note on that pattern, because it cost a wrong conclusion first: a single-line
`grep "catch\s*{\s*}"` reported **0 silent catches in Ribbon** and was believed for several minutes.
Every catch in this folder puts its brace on the following line, so the pattern could not have matched
any of them. It had to be multiline.

## Not examined

Recorded so this does not read as a finished review:

- **54 literal colour references** (`Color.FromArgb`, `Color.White`…) across the folder. Not yet
  checked against rule 3 — some will be legitimate semantic or fallback colours, some may not be.
- **Composition vs hand-painting** (rule 4). `Rendering/BeepRibbonPainter.cs` and
  `Gallery/RibbonGalleryRenderer.cs` paint directly; whether that is warranted here is unassessed.
- **No behavioural probe has been written**, so nothing in this folder has been verified by running it.
  Everything above is a static finding plus a clean build. Layout, keyboard/KeyTips, backstage,
  contextual tabs, minimise/restore and the gallery are all unexercised.

## Standing constraints

Per `CLAUDE.md`: report every catch through `BeepLog`; no stubs or legacy paths; nothing assigns
colours; compose from Beep controls; a check must be able to fail for the reason it was written.

## Behavioural probe (scratchpad `RibbonProbe`)

22 checks, asserted from the control's own model rather than from pixels. Two defects, both found by
the first run.

### `new BeepRibbonControl()` threw from its own constructor

`InitializeBackstageLayout` set `SplitterDistance = 180`, then `Panel1MinSize = 140`, then
`Panel2MinSize = 260`. A `SplitContainer` is 150px wide until a parent lays it out, and `Dock.Fill`
does not apply inside a constructor — so assigning `Panel2MinSize` re-validated `SplitterDistance`
against `Width - Panel2MinSize` = **-110**, an empty range, and WinForms threw. The control could not
be constructed at all.

Fixed by widening the container to a satisfiable size first, then the minimums, then the distance.
Order matters as much as width: setting the distance first validates it against limits that do not
exist yet.

**This is why the probe was worth writing.** The static pass over this folder found real problems and
a clean build, and none of it could have caught a control that cannot be instantiated.

### `AddCommandToQuickAccess(string)` accepted any string

It checked only for null/whitespace, the personalization flag, and duplicates — never the command
lookup. `AddCommandToQuickAccess("NoSuchCommand")` returned **true** and put the literal text in the
key list beside genuine GUID keys, where it survived a save/load round trip and could never resolve to
a command. A `true` return meant only "that string was not already in the list".

`ResolveQuickAccessKey` had the matching fault: on no match it returned the token unchanged, so a
stale key in a saved file was accepted too. It now takes a `strict` flag — strict for a caller adding
by name, lenient for file loading, where the ribbon may not be built yet and the key must be taken on
trust.

### One instrument error, corrected

The first run reported "3 tabs built — got 0" for build, merge and customization load. `RibbonTabs` is
an *input* collection (`Core.cs:63` converts it into `CommandItems` when that is empty), not the built
result. The control caught it: an **empty** ribbon also reported 0, so the check could not tell the
two cases apart. Assertions now read `CommandItems`.

### Covered

Build from SimpleItems, quick access add/reject/move/remove, save+load round trips for quick access,
customization and theme tokens, corrupt-file reporting, merge scope begin/merge/end, search history
persistence, minimise/restore.

### Still not covered

KeyTips, contextual tabs, the backstage UI itself, the gallery, accessibility, and every rendering
path. The 54 literal colour references remain unchecked against rule 3.

## The ribbon does not render as a ribbon — group layout is never applied

Found by rendering it, which neither the static pass nor the model-level probe did. With a Home tab of
three groups (Clipboard, Font, Paragraph), the tab content panel contains:

```
Panel {X=0,Y=0,Width=1264,Height=59}    <- one group, stretched full width
Panel {X=0,Y=0,Width=200,Height=100}    <- second group, same origin, default size
Panel {X=0,Y=0,Width=200,Height=100}    <- third group, same origin, default size
```

All three sit at **(0,0)**, stacked. Two never left the default `200x100`, so they were neither
positioned nor sized, and at 100px tall inside a 59px parent they are clipped too.

Everything visible in the render follows from this: only the last group's commands appear, and the
captions read `BBulllets` / `NNumbering` / `AAlign Lefft` — not a font defect but two overlapping
panels each drawing their own labels a pixel or two apart.

Beyond the layout itself, the tab content has none of a ribbon's structure: no group caption strip, no
separators between groups, no large/small command buttons, no icons. Commands render as bare text.

Order to fix:

1. Lay groups out left-to-right across the tab content area, each sized to its content.
2. Group chrome — caption under each group, separator between them.
3. Commands as buttons with icons and large/small variants, per `CLAUDE.md` rule 4 (compose from Beep
   controls; `BeepButton` for an action, `BeepImage` for an icon).
4. Theme the chrome — the flat blue tab band is unthemed.

Not started.

### CORRECTION to the section above

The diagnosis "all three groups sit at (0,0)" was **wrong**. Enumerating one level deeper showed those
three panels were the three *tabs'* content panels — Home's docked Fill and visible, Insert's and
View's at the default 200x100 and hidden, which is correct. Each held its own groups properly parented.

The real defect was one level down: `BeepRibbonGroup` had `Dock = DockStyle.Top` and `Stretch = true`,
so groups stacked **vertically**, each spanning the full ribbon width:

```
Paragraph {X=0,Y=0,  W=1264,H=48}
Font      {X=0,Y=48, W=1264,H=48}
Clipboard {X=0,Y=96, W=1264,H=48}
```

In a 59px-tall content panel only the first was visible and the rest were pushed off the bottom.

**Fixed:** `Dock = DockStyle.Left`, `Stretch = false`, and the group takes its width from its own
items. `AddGroup` also calls `SetChildIndex(group, 0)` — a left-docked child docks nearest the edge in
reverse child order, so appending would have put the last group at the far left and reversed the tab.

Now: `Clipboard {X=0,W=272}  Font {X=272,W=196}  Paragraph {X=468,W=233}`.

### Every command caption rendered twice

Visible once the groups were side by side. `OnRenderButtonBackground` passed `btn.Text` to
`Painter.PaintSmallButton`, which draws the label — and WinForms then called `OnRenderItemText`, which
drew it again. Two captions per command, a few dozen pixels apart ("Paste  Paste", "Bold  Bold").

Fixed by painting background only and letting `OnRenderItemText` own the text: it already applies the
theme colour, and the standard pipeline is what gets alignment, `TextImageRelation` and ellipsis right
for both button layouts.

## Still not a ribbon — remaining work

With groups flowing and captions single, what a real ribbon still needs:

1. **Group caption strips** — "Clipboard"/"Font"/"Paragraph" under each group. `BeepRibbonGroup.Text`
   is set and never drawn.
2. **Command icons.** Every button is text-only; `CreateCommandImage` is called but the probe's items
   carry no `ImagePath`, and nothing falls back to a default glyph.
3. **Commands overflow too eagerly.** Copy, Format Painter, Underline and Align Left are all behind the
   "More" chevron at a 1264px width, because only two 17px rows fit in a 59px content panel. A real
   ribbon fits three small rows or one large button.
4. **Group separators** between adjacent groups.
5. **Theming** — the flat blue band is the unthemed default, not a resolved theme.

## Metrics, taken from established ribbon implementations

Researched rather than invented, because "it looks like boxes" needs numbers to fix. Sources:
Fluent.Ribbon's own theme XAML (`Themes/Controls/RibbonGroupBox.xaml`), its sizing documentation, and
Microsoft's Fluent ribbon overview.

The two anchors that come straight out of Fluent.Ribbon's group template:

- the group's inter-group **separator is 55px tall** — that is the content region, above the caption
- **22** recurs as the row/state unit (`QuickAccess State Height = 22`)
- the group **dialog launcher is 16x16**, sitting on the caption strip

From those, the standard Office layout this control should target:

| element | value |
|---|---|
| small button row height | 22px, 16px icon, label beside it |
| small rows per group | 3 (3 x 22 = 66px content) |
| large button | spans all three rows (~66px tall), 32px icon above the label |
| group caption strip | ~22px, below the content, with the dialog launcher at its right |
| **group total height** | **~88px** (66 content + 22 caption) |
| ribbon content panel | ~90-95px |

### What this means for the current code

The content panel is **59px**. That is the whole reason every command renders as bare text: 59px fits
two 17px rows and leaves nothing for a caption strip, so `BeepRibbonGroup.Text` has nowhere to go and
large buttons cannot exist at all. `GetGroupHeight()` returns 40/48/56 by density — every one of them
below the 66px a three-row group needs, before the caption.

Concretely, to fix:

1. `GetGroupHeight()` -> content 66 (Compact 44 = 2 rows, Touch 88 = 3 x 30). Add a separate caption
   height of 22.
2. Raise the ribbon's default `Height` so the content host gets ~90px after the toolstrip (25), the
   contextual strip (18) and the tab strip (28) take theirs. 150 is not enough; ~165-170 is.
3. Draw the caption strip in `BeepRibbonGroup.OnPaint` (or the renderer) using `Text`, and reserve it
   out of the item layout area.
4. Give `AddCommandButton` a real large/small split: large = 32px icon above label spanning the
   content height; small = 16px icon beside label at 22px, three per column.
5. Draw a 1px separator between adjacent groups, inset from the caption strip.

Not started. This is the work that turns the boxes into a ribbon.

## Group metrics applied — but the ribbon height is still being overridden

Done:

- `BeepRibbonGroup.ContentHeight = 66` and `CaptionHeight = 22`, with `ContentFor(density)` as the one
  place a density maps to an item-area height.
- The group reserves the caption strip in `Padding.Bottom` and **draws** it in `OnPaint` from `Text`,
  which `AddGroup` had always set and nothing had ever rendered. A 1px separator on the trailing edge.
- Two methods were silently undoing this and are fixed: `ApplyDensity` reassigned `Height` to the bare
  40/48/56 content values, and `ApplyMetrics` reassigned `Padding` without the caption reservation —
  and `ApplyMetrics` runs from `ApplyTheme`, from `ApplyDensity` and from every `Add*Button`, so the
  items would have drawn straight over the group title.
- `GetGroupHeight()` now delegates to `BeepRibbonGroup.ContentFor`.
- `BeepRibbonControl` constructor asks for `71 + 66 + 22 = 159` instead of 130.

**Not working:** the control still reports `Height = 130` after construction, so the content panel is
still 59px and each group is still docked to 59px. Verified it is not a stale build — both the library
and the probe's copy of the DLL carry the same timestamp, and the constructor line is present in the
compiled source. Something downstream reassigns `Height`; `BeepRibbonControl.Minimized.cs` has
`CalculateMinimizedHeight()` and a `minimumExpandedHeight` clamp around lines 175-195 that is the
obvious suspect and has not been read yet.

Until that is found, the visual result is unchanged: groups remain 59px tall, so the caption strip has
no room and the large/small button split (step 4) cannot be attempted. **The rendering has not been
re-verified since these edits** — the last render still showed the old 130px layout.

Next: read `Minimized.cs` 170-200, find what reassigns `Height`, then re-render before going near the
button split.

## The structural lesson that was missed

`BeepRibbonGroup : ToolStrip` is why this renders as boxes, and no amount of height tuning fixes it.

A `ToolStrip` is a toolbar: one flow of items, overflowing into a chevron. That is what produced the
single row of bare text captions and the "More" button on every group — the chevron is the ToolStrip's
own overflow, and it will keep appearing at any height.

Fluent.Ribbon's `RibbonGroupBox` is not a toolbar. It is a container whose items live in a wrap panel
that fills **columns of three rows**: small controls stack three-high and then start a new column,
while large controls span the full content height alongside them. That column-wrap arrangement *is*
the ribbon look. The metrics recorded above (66px content, 22px caption, 22px rows, 32px/16px icons)
only mean something once the container arranges items that way.

So the real work is not more tuning of the existing class:

1. Replace the `ToolStrip` base with a `Panel` (or `BaseControl`) that owns its own layout.
2. Give it a column-wrap layout: fill three 22px rows top-to-bottom, then advance a column; a large
   item takes a whole column at 66px with a 32px icon above its label.
3. Host commands as `BeepButton` + `BeepImage` per `CLAUDE.md` rule 4, rather than `ToolStripButton` -
   which also removes the custom renderer, and with it the double-draw fixed earlier.
4. Keep the caption strip and separator already written in `OnPaint`; they carry over unchanged.

Everything committed today (group docking, caption strip, metrics, the double-draw fix) is a
prerequisite for that and none of it is wasted, but on its own it cannot produce a ribbon.

---

## DONE — the group is a column-wrap container, not a toolbar

All four numbered items above are landed, plus the height override that blocked them.

### The height override

`Ribbon/BeepRibbonControl.Fields.cs` initialised `_expandedRibbonHeight = 130` — the old default — and
`ApplyMinimizedState`'s **else** branch wrote it back to `Height` on *every* rebuild, not only after a
minimize. `BuildFromSimpleItems` ends in `ApplyMinimizedState`, so the first command anyone added
stomped the constructor's 159 back to 130. The field is now `0` until a real minimize captures a
height, and the else branch only restores a captured one.

Two DPI faults went with it. `DpiScalingHelper.GetDpiScaleFactor` returns `1.0` while a control has no
handle, so the constructor's `ScaleValue` was a no-op on every monitor. The chrome height is now
computed in `ApplyRibbonChromeMetrics`, called from `OnHandleCreated`, `OnDpiChangedAfterParent` and
the `Density` setter, and it only moves a height it set itself (`_appliedRibbonHeight`), so a designer
or host height survives.

### The container

`BeepRibbonGroup : Panel`. Items go into `GroupSlot`s and are placed by `ComputeLayout`, which fills
columns of `RowsFor(density)` rows: a small command takes one 22px row and the column is as wide as its
widest member; a large command closes the current column and takes a whole one at the full content
band; a separator closes the column and reserves a slot for a painted vertical rule. `Width` is an
*output* of that fill. `MeasureContentWidth()` runs the same computation without moving anything, which
is what the overflow decision is made on — a real measurement, not the old pile of magic numbers.

Commands are `BeepRibbonCommandButton : BeepButton`: `ImagePath` straight to the button (no
rasterise-to-Bitmap per rebuild), `MaxImageSize` 32 or 16 by size, `ImageAboveText`/`ImageBeforeText`
geometry, and a chevron for a command that opens a menu. Galleries are direct children — the
`ToolStripControlHost` wrapper and its undisposed lifetime are gone.

`MinTouchTargetWidth`, `IsPopupOpen`, `CloseChildPopup`, `PopupOpened`/`PopupClosed` had no writer or
reader anywhere while the group was a toolbar. Rather than delete them they were wired up:
`MinTouchTargetWidth` is the small-item floor at Touch density, and `TrackPopup` follows a command's
drop-down so the popup members mean something.

### Overflow

Both old mechanisms are gone. The ToolStrip chevron vanished with the base class. The hand-rolled
"More" button is now a real command control with an accessible name, a role and a key tip, holding the
leftovers in its own drop-down; the group is filled twice when it overflows, the second time with
`OverflowButtonWidth` reserved, so the affordance cannot itself be the thing that does not fit.
`ApplyResponsiveLayout` is no longer a duplicate build at the end of `BuildFromSimpleItems` — it is
what a (debounced) resize calls, which is what its name always claimed.

### Decisions taken, and the option not taken

- **`RibbonItemSize` was not re-declared.** `RibbonItems.cs` already had `Large/Medium/Small` for the
  designer model, and `RibbonButtonItem.Size` did nothing at all (`Text = Size == Large ? Text : Text`).
  It now flows to `SimpleItem.Data["RibbonItemSize"]` and the group honours it; `Medium` lays out as
  `Small`, because the grid has no third row height to give it.
- **Per-command size, not a per-group boolean.** `DetermineLayoutSize` asked whether `72 x n` fitted
  inside a width derived from the small-button estimates — circular, and it answered "small" almost
  everywhere. Replaced by `ResolveItemSize`: an explicit declaration wins, a gallery is always large,
  otherwise the leading command of a group is large and the rest stack.
- **Two command maps, not one keyed on `object`.** The quick access toolbar looks its items up by
  `ToolStripItem.Owner`; a single object-keyed dictionary would have cost that lookup its type.
- **`RibbonCommandInvokedEventArgs.Source` is now `object`.** No WinForms type covers both a group's
  controls and the quick access toolbar's items. Breaking change; no consumer in this solution.
- **Deleted:** `BeepRibbonPainter.PaintGroupPanel` and `PaintLargeButton` (both had zero callers and
  the large button is now a control), `DetermineLayoutSize`, `GetAvailableGroupWidth`,
  `EstimateCommandWidth`, `GetGroupHeight`, `GetLargeItemWidth`, `EstimateOverflowButtonWidth`,
  `BeepRibbonGroup.ApplyMetrics`, and `GetCommandRole`'s host-type tests.
- **`PaintGroupSeparator` and `PaintGroupTitle` were kept and made live** — the group's `OnPaint` calls
  them, which is what they were written for. `PaintGroupTitle`'s fixed 14px sub-rect was clipping the
  caption once it had a caller.

### Verified

A probe (65 assertions, all from the control tree) covers: 3 tabs from SimpleItems; 2 groups on Home;
the group is a `Panel`; commands are `BeepButton`s; large leading command at 66px with a 32px icon;
three small commands stacked at y 0/22/44 in one 22px-row column; a separator starting a new column;
group width equal to its columns; a gallery as a direct child; a chevron and a populated menu on a
command with children; a checkable command selected; accessible names and tab stops on every command
and a clean accessibility audit; key tips assigned to group commands; Compact/Touch densities;
14 commands placed at 1200px and overflowed with nothing lost at 430px, then restored on widening;
quick access add/reject/move/remove; quick access and customization save+load round trips; merge scope;
minimize/restore; and label contrast on the group surface.

Each load-bearing check was made to go red first. Restoring `_expandedRibbonHeight = 130` reproduced
the reported symptom exactly (ribbon 130, group crushed to 59) and turned "expanded ribbon fits a
group" and "group height is content + caption" red. Setting `RowsFor` to 99 turned "small rows are
22px", "stacked rows are at y 0/22/44" and all three overflow checks red. Note that "a rebuild does not
stomp the height" did **not** go red under the first break: it compares against a baseline captured
after the stomp, so it only catches a change *during* a rebuild — "expanded ribbon fits a group" is the
check that catches the defect.

**Not verified:** Simplified layout mode, RTL, the minimized tab popup's contents, the backstage, the
super-tooltip and quick-access right-click paths on the new controls (wired and compiling, never
exercised), the split-button gap (a command with children still only opens a menu; there is no
icon-half/arrow-half behaviour), and any DPI other than 96 — `ApplyRibbonChromeMetrics` is where that
now happens but no high-DPI run was made.

## The rewrite landed — verified by looking at it

`BeepRibbonGroup` is a `Panel` with a column-grid layout: small commands stack three 22px rows per
column, a large command takes a whole column at the 66px band with a 32px icon above its label, and
group width is an *output* of the fill. Commands are `BeepRibbonCommandButton : BeepButton` with
`ImagePath` (BeepImage renders and themes the SVG) — the ToolStrip renderer no longer touches groups,
and the rasterise-to-Bitmap image cycle is gone with it.

The `Height = 130` mystery: `_expandedRibbonHeight = 130` hard-coded in `Fields.cs:88`, applied by
`Minimized.cs:200` after the constructor asked for 159. A second copy of the old default. Removed.

Theming per `01-VISUAL-DESIGN.md`: `RibbonThemeMapper` now derives **one surface**
(`GroupBack = TabActiveBack`) — the group patchwork was three different derivations of the band
colour — and an **accent ladder** (hover 18%, checked 22%, pressed 30% accent blends; stronger in
dark). The old luminance-shift hovers were toolbar styling.

Verified by rendering (`ribbon-home/insert/narrow.png`) and reading the images: large+small icons at
32/16, captions under groups, separators, continuous surface, checked toggle in accent, overflow
"More" only when genuinely narrow (414px). Behavioural probe 65/65, including its own
can-this-fail controls.

Not verified visually: hover/pressed fills (static renders cannot show them; the assignments are
asserted, the pixels are not). Known nits: the tab strip does not repaint the active card when a
section is shown programmatically in the render harness; gallery tiles read slightly disabled.
