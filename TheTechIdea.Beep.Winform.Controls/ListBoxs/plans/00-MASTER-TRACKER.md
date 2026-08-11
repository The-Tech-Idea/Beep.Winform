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
| 03 | Pointers / cursor affordances | **bug** | ☑ done |
| 04 | The `?? Color.X` fallback layer (329 sites) | **rule 4** | ☐ open |
| 05 | Selection colours stamped into properties | **bug** | ☐ open |
| 06 | Neumorphic's luminance shifts | refactor | ☐ open |
| 07 | Row backgrounds come from ControlStyle, not the theme | **bug** | ☐ open |
| — | [Painter-by-painter audit](02-painter-audit.md) | reference | ☑ current |

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

### Two faults in the probe, not the code

Worth recording because both would have read as defects: the style check asserted every list draws
**4** rows, which no style with a 72–120px row can do inside a 220px viewport (it now compares
against the number of rows that actually fit); and the pointer check found an empty layout cache,
because that cache is built during **painting** and `Invalidate()` alone does not build it.

### Still open

- **18** painters draw `item.Description` with their own layout rather than
  `DrawTitleAndSubtitle` — not five, as previously written. They are listed individually in
  [02-painter-audit.md](02-painter-audit.md). None was visibly clipping in the contact sheet, but
  each holds its own copy of the arithmetic that broke the four that were fixed.
- **16** painters call `BeepStyling`, which is why dark themes still draw light rows — see stage 07.
- Unscaled numeric literals are concentrated in a few painters: `Glassmorphism` 32, `BaseListBox`
  18, `NavigationRail` 16, `Timeline` 11, `Neumorphic` 10. Those sizes do not follow DPI.
- Tall styles (`ProfileCard` 120px, `ThreeLineList` 88px, `NotificationList` 80px) clip their last
  row against the viewport rather than the row — not yet investigated.

## Stage 03 — pointers (done)

Two defects, both in `OnMouseMove`.

**The cursor was only updated when the hovered ITEM changed.** The assignment sat inside the
`newHoveredItem != _hoveredItem` block, so moving out of the search box and back onto the row you
were already hovering left the cursor as an **IBeam over a list row** — the block never ran,
because the item had not changed. It is now resolved on every move.

**A disabled row still offered the hand.** The affordance is a promise, and this one was false.
Group headers and separators are labels too, and now say so.

The decision is a function, `ResolveCursorFor(Point)`, not a side effect buried in a
change-detection block — a cursor is otherwise only observable by moving a real mouse, which no
check can do reliably. The probe asserts it directly: enabled row → `Hand`, disabled row →
`Default`, empty space → `Default`, plus a guard that the two rows do not resolve to the same
thing.

**Break-it-first:** removing the `IsEnabled` guard turns the disabled check *and* the
"two rows differ" guard red; restored, both go green. 50 checks, 0 failures.

### Not done

Per-hit-area cursors. `BeepListBoxHitTestHelper` registers `check_`, `icon_` and `text_` areas
alongside `row_`, so a checkbox or trailing action could show something distinct from the row. The
row-level answer is correct for every style today; splitting it further is a design decision about
what a checkbox should feel like, not a defect.

## Stages 04 + 05 — done

### 05: the selection colours are gone

`SelectionBackColor`, `SelectionBorderColor` and `FocusOutlineColor` were per-control colour
overrides that had to be seeded from the theme — and the seeding **stamped** the property, so it
was no longer `Color.Empty` and the guard never fired again. **Every later theme change was
ignored**: the selection stayed the first theme's primary colour for the life of the control. It
also made a caller's deliberate colour indistinguishable from a seeded one.

Deleted, along with both seeding blocks and the now-dead `!= Color.Empty` guards in
`BeepListBox.Accessibility.cs`. The painters read `Theme.PrimaryColor` / `Theme.AccentColor`
directly. **Public members removed:** the three properties above.

### 04: no colour literals left in any painter

**269 sites across 38 painters.** Two passes:

- **206** of the form `_theme?.Slot ?? Color.Something` (and 48 two-slot chains). That pattern
  silently substituted a literal — usually a light-theme grey — for the entire palette whenever the
  field was null. Replaced by `Theme.Slot`, backed by a new `BaseListBoxPainter.Theme` accessor
  (`_theme ?? BeepThemesManager.CurrentTheme`). There is always a theme.
- **63 + 21** standalone literals, by category rather than mechanically: selection ink →
  `OnPrimaryColor`; shadows → an alpha veil over `ShadowColor`; disabled and border greys →
  `DisabledForeColor` / `BorderColor` / `SecondaryTextColor`; brightness-based black/white picks →
  a pick between two theme slots; and the semantic ones kept their meaning through the semantic
  slot (`ErrorColor`, `WarningColor`).

Two decorative palettes were labelled as deliberate and were not:

- `DrawInitialsFallback` carried seven hard-coded Material hues commented *"intentional literal
  colors — decorative, theme-independent"*. Theme-independent is the defect: a dark or
  high-contrast theme got the same seven light pastels. It now indexes the theme's own semantic
  slots, still deterministically.
- `GradientCardListBoxPainter` held six `static readonly` web gradients, so every theme drew the
  same purple-blue and pink-peach cards. Built from theme pairs per call now.

**Verified:** 50 checks pass under both LightTheme and DarkTheme; contact sheets rendered for both
and eyeballed. The avatar discs visibly change colour between themes now, which they could not
before.

**Correction.** "Zero colour literals" was claimed one commit too early — a per-file re-count found
`Color.FromArgb(30, Color.Teal)` still in `MultiSelectionTealPainter`'s hover brush, which the
first grep's exclusion list had hidden. Fixed; the count is genuinely zero now, and
[02-painter-audit.md](02-painter-audit.md) carries the per-painter numbers the summary should have
had in the first place.

### Found while verifying — not fixed

**Row backgrounds do not follow the theme.** The DarkTheme contact sheet still draws light rows in
most styles. This is *not* the literals — those are gone. The painters fill item backgrounds
through `BeepStyling` / `ControlStyle`, and `ControlStyle` pins its own bundled theme, so the
palette in force is the style's rather than the application's. Same trap recorded elsewhere in this
repo. That is the next thing to chase and is larger than a colour sweep.
