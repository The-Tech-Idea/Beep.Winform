# ListBoxs — review and enhancement

Master tracker for `TheTechIdea.Beep.Winform.Controls/ListBoxs/`.
**64 C# files, ~12,000 lines: `BeepListBox` (8 partials) plus 47 painters, 3 helpers, 5 enums.**

## Census

| rule | count | note |
|---|---|---|
| `useThemeColors` flag | **0** | clean — unusual for this repo |
| bare `catch { }` | **21** | ☑ fixed in batch 1 |
| `BeepLog` calls | **0 → 20** | nothing in 12k lines reported anything |
| literal `Color.Xxx` | 219 | almost all `?? Color.White` style fallbacks after a theme slot |
| literal `Color.FromArgb(r,g,b)` | 110 | same shape, plus a hard-coded avatar palette |
| `Color.Empty` guards | 23 | incl. selection colours stamped into properties |
| luminance shifts / blends | 12 | `NeumorphicListBoxPainter` is built on them |
| literal `new Font("…")` | 0 | clean |
| `DpiScalingHelper` | 101 | genuinely used, unlike most folders at this stage |

## A finding that wasn't

A first pass reported "14 named styles fall through to `StandardListBoxPainter`". That was wrong:
`ListBoxType.cs` declares **five** enums, and the unmatched names (`Comfortable`, `Single`,
`MultiSimple`, `Contains`, `StartsWith`, `Auto`, `TitleOnly`, `TitleSubtext`,
`LeadingIconTrailingMeta`, `AvatarSecondaryAction`) belong to `ListDensityMode`,
`SelectionModeEnum`, `ListSearchMode` and `ListRowPreset` — not to `ListBoxType`. **Every
`ListBoxType` value maps to a painter**, and all four other enums have real consumers. Recorded
because the instrument, not the code, was at fault.

## Stages

| # | Stage | Kind | Status |
|---|---|---|---|
| 01 | Nothing reported: 21 bare catches | **rule 1** | ☑ done |
| 02 | Sizing and alignment across the 43 styles | **review** | ◐ two defects fixed |
| 03 | Pointers / cursor affordances | **review** | ☐ open |
| 04 | The `?? Color.X` fallback layer (329 sites) | **rule 4** | ☐ open |
| 05 | Selection colours stamped into properties | **bug** | ☐ open |
| 06 | Neumorphic's luminance shifts | refactor | ☐ open |

## Stage 01 — done

21 bare catches, zero reports, in 12,000 lines. Now:

- **Layout recalculation** (9 sites across Core/Events/Keyboard/Methods) — `FailureOnce` on one
  shared key, because these run from many paths and a per-call report would bury the first.
- **Accessibility notifications** (3) — one key each for selection, reorder and focus, so a screen
  reader going quiet is attributable to which notification failed.
- **Clipboard** (2) — `Failure`, not `…Once`: these are user-initiated, so every failure matters.
- **Painter preferred height** (1) — `FallbackOnce` keyed by painter type; it falls back to
  `MenuItemHeight`, and knowing *which* painter threw is the whole point.
- **Virtual size** (1) — scrolling silently breaks without it.
- **Paging-hint overlay** (1, in a paint path) — `FailureOnce`.
- **Three deleted rather than reported.** Assigning `Color` properties cannot throw, and neither
  can `16f / Math.Max(1f, x)`. Two of those catches wrapped only property assignments; the
  `_hoverAnimationStep` one wrapped a float division with the divisor floored at 1. Per CLAUDE.md,
  "catching is not error handling if nothing throws" — the division's catch is gone entirely, and
  the two colour-seeding ones now report because the *theme lookup* above them can fail.

## Stage 02 — sizing and alignment (partly done)

All 43 styles rendered with the same four items and eyeballed as a contact sheet
(`ListProbe`, `%TEMP%\ListShots\contact-sheet.png`). Two defects, both systemic.

### 1. A "PgUp / PgDn" hint painted over the last row — removed

`BaseListBoxPainter` drew the string at the bottom-right of the drawing rect whenever the content
overflowed, with **no space reserved and no background**, straight across the last visible item.
It showed in roughly twenty of the forty-three styles. A scrollbar already says there is more to
see, and says it without defacing a row. Deleted.

### 2. Two-line rows sliced their subtitle in half — fixed, and de-duplicated

Four painters (`RekaUI`, `ChakraUI`, `MultiSelectionTeal`, `RadioSelection`) each carried their
own copy of the same broken arithmetic: draw the title into the **full** row height with
`VerticalCenter`, then put the subtitle in the **bottom half**. The two overlap, and half a row is
less than the subtitle's own font needs — so every subtitle was cut through horizontally.

All four now call one `BaseListBoxPainter.DrawTitleAndSubtitle`, which stacks the two lines from
their **measured font heights** and centres the block. If a row is too short for both, the top edge
wins so the title stays whole rather than both lines being half-cut.

### 3. The measure and the paint keyed on different properties

`GetItemHeight` grew the row for `BeepListItem.SubText` — assigned in **two** places in the entire
repo — while **nine** painters draw `SimpleItem.Description`. Any list built the ordinary way was
measured as one line and drawn as two. There is now one `HasSecondLine(item)` authority that both
sides use.

**Note it is still inert by default:** `AutoItemHeight` defaults to `false`, so `GetItemHeight` is
never called and every row uses the flat per-style `GetPreferredItemHeight()`. The layout fix above
is what actually repairs the rendering, because it makes two lines fit inside the style's own
compact height. Whether `AutoItemHeight` should default to `true` is a behavioural change and is
left as a decision, not taken quietly.

### Still open

- The remaining five painters that draw `item.Description` have not been converted to
  `DrawTitleAndSubtitle`; they were not visibly clipping in the sheet, but they hold their own
  copies of the layout.
- Tall styles (`ProfileCard` 120px, `ThreeLineList` 88px, `NotificationList` 80px) clip their last
  row against the viewport rather than the row — not yet investigated.

## Stage 03 — pointers (open)

Only three `Cursor` assignments exist in the whole folder (`BeepListBox.Events.cs:26,51,101`):
`IBeam` while searching, `Hand` over an item, `Default` on leave. Nothing sets a cursor for the
checkbox, radio, avatar, chevron or trailing-action hit areas that several painters draw, so a
clickable affordance and dead space feel identical under the mouse. `BeepListBoxHitTestHelper`
already registers those areas, which is what a fix would read.
