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
