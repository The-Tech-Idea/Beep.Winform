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
| 02 | Sizing and alignment across the 43 styles | **review** | ☐ open |
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

## Stage 02 — sizing and alignment (open)

The thing to check, and how it can fail: each painter implements both `GetItemHeight` /
`GetPreferredItemHeight` (measure) and its own draw. The repo's recurring defect is these two
disagreeing — measured at one height, drawn at another, so text clips or rows overlap. With 43
painters this needs a rendered contact sheet per style, eyeballed, not a count.

`AutoItemHeight` selects between `GetItemHeight(owner, item)` and `GetPreferredItemHeight()`, so
both paths need covering.

## Stage 03 — pointers (open)

Only three `Cursor` assignments exist in the whole folder (`BeepListBox.Events.cs:26,51,101`):
`IBeam` while searching, `Hand` over an item, `Default` on leave. Nothing sets a cursor for the
checkbox, radio, avatar, chevron or trailing-action hit areas that several painters draw, so a
clickable affordance and dead space feel identical under the mouse. `BeepListBoxHitTestHelper`
already registers those areas, which is what a fix would read.
