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

## Stage 07 — theme reaches the painters (partly done)

### Root cause was not what stage 04 assumed

`BeepStyling.CurrentTheme` was a plain auto-property that **almost nothing set** — a repo-wide
search finds two writers, in GridX and ToolTips. Everywhere else it stayed **null**, and
`BeepStyling.GetColor` tests `UseThemeColors && CurrentTheme != null` before consulting the theme:
with null it fell straight through to `styleColorFunc(style)`, the style's own hard-coded palette.
The theme was never even asked. It now falls back to `BeepThemesManager.CurrentTheme`; an explicit
setter still wins, so the two existing writers are unaffected.

**This is library-wide**, not ListBoxs-only: every control that paints through
`PaintStyleBackground` was getting the style's literals instead of the application's theme.

### Painters use the list's own slots now

`ListBackColor`, `ListItemSelectedBackColor` and `ListItemSelectedForeColor` had **zero** uses
across 44 painters; the row surface was filled from the generic `BackgroundColor` (19 sites) and
selection from `PrimaryColor` (39). A theme sets the `List*` slots separately precisely so a list
need not be a panel and a selected row need not be the brand colour — and DarkTheme does differ.
Swept: surface and ink to `ListBackColor` / `ListItemForeColor` (24 sites), and the base painter's
selection to `ListItemSelectedBackColor` / `ListItemSelectedBorderColor`.

`BeepListBoxHelper.GetTextColor()` returned `_owner.ForeColor` — a plain WinForms property nothing
keeps in step with the theme, so it sat at the system near-black whatever theme was in force. It
reads `ListItemForeColor` now. `BeepListBox.ApplyTheme` also set neither `BackColor` nor
`ForeColor`; it sets both from the theme now.

### Verified

The 16 painters that draw through `BeepStyling` — MaterialOutlined, LanguageSelector, TeamMembers,
FilledStyle, Grouped, OutlinedCheckboxes, RaisedCheckboxes, MultiSelectionTeal, ColoredSelection,
RadioSelection, Custom, ErrorStates, FilterStatus, RekaUI, ChakraUI, HeroUI — render dark rows with
readable light text under DarkTheme. They rendered light rows before. 50 checks pass in both
themes.

### NOT fixed — the other ~25 styles still show a light surface

Standard, Minimal, Outlined, Rounded, Filled, Borderless, CategoryChips, SearchableList, WithIcons,
Compact, CardList, ChipStyle, Glassmorphism, Neumorphic, GradientCard, AvatarList, Timeline,
InfiniteScroll, CommandList, NavigationRail, ChatList, ContactList, ThreeLineList,
NotificationList and ProfileCard still draw a white surface under DarkTheme.

It is **not** the painters' fill colours — those now resolve to `ListBackColor` (30,30,30 in
DarkTheme). The surface is painted *before* the painter runs, by the control/`BaseControl` path,
and setting `BeepListBox.BackColor` from the theme did not change it. That path is the next thing
to find; it was not chased further here rather than guess at it.

**Do not read the light rows in the contact sheet as "the sweep failed"** — the sweep is visible in
the 16 styles above and in the avatar discs, which change colour with the theme in every style.

## Painter-by-painter - all 44, read individually

Every painter below was read line by line, then fixed. `[x]` is done and building; `[ ]` is
recorded and still open.

**Fixed across the folder in this pass:**

| fix | painters |
|---|---|
| `SecondLine(item)` - Description now feeds every second line that only read `SubText` | 13 painters, 28 sites |
| unselected rows now fill `ListBackColor` instead of being transparent | Avatar, Borderless, ChipStyle, Glassmorphism, GradientCard, Minimal, Neumorphic, Timeline |
| selection uses `ListItemSelectedBackColor` / `ListItemSelectedForeColor` | 19 painters, 23 sites |
| state read from the item instead of sniffed from its text | ColoredSelection, ErrorStates, OutlinedCheckboxes, RaisedCheckboxes |
| hardcoded English removed | ErrorStates (badge + message), InfiniteScroll (new `LoadMoreText` property) |
| live mouse reads removed from paint paths | CategoryChips, InfiniteScroll |
| square shadows now fill the rounded path | CardList (3), Rounded (1) |
| `O(n^2)` per-paint `IndexOf` replaced by a stable hash | GradientCard |
| stopped forcing a light surface on dark themes | Neumorphic |

`Notification` and `ProfileCard` deliberately keep the raw `SubText`: they use it *and*
`Description` for different lines, so the fallback would have printed the same string twice.

### AvatarListBoxPainter
- [x] **Second line dead for ordinary items** - reads item.SubText, never Description; SubText is assigned in 2 places repo-wide.
- [ ] **Avatar centred, text not** - avatar (H-av)/2, text pinned at Y+10 / Y+30; height is max(font+28,56), so they align only at the minimum.
- [ ] **Status dot requires BadgeText but never draws it** - gated on the text, paints only a dot from BadgeBackColor.
- [x] DrawItemBackground overridden **empty**; fill happens only when selected/hovered, so unselected rows are transparent.
- [x] *fixed*: the OnPrimaryColor-as-background fallback behind the dot is now ListBackColor.

### BaseListBoxPainter
- [x] *fixed*: the PgUp/PgDn overlay painted across the last row in ~20 styles - removed.
- [x] *fixed*: GetItemHeight keyed on SubText while 9 painters draw Description - one HasSecondLine authority now.
- [x] *fixed*: avatar palette was 7 Material hues commented 'theme-independent' - now theme slots.
- [ ] *added*: DrawTitleAndSubtitle, a Theme accessor, ListItemSelected* for selection.
- [ ] **18 unscaled pixel literals** remain.

### BorderlessListBoxPainter
- [x] **Draws no background at all** except a selected underline - every row transparent.
- [ ] SupportsSearch and SupportsCheckboxes are both false, so ShowCheckBox silently does nothing in this style.

### CardListPainter
- [x] **Square shadows behind a rounded card** - all three passes use FillRectangle while the card itself is a rounded path.
- [x] **No second line at all** - draws item.Text only; Description and SubText are both dropped.
- [ ] Image is a fixed Scale(44) in a Scale(60) row with Scale(8) padding - it fits exactly, with no room to grow.
- [x] Selection is a PrimaryColor gradient, not ListItemSelectedBackColor.

### CategoryChipsPainter
- [x] **Reads the live mouse inside paint** - PointToClient(Control.MousePosition) decides the close-button hover, so it renders differently under DrawToBitmap and ignores the hover state the control already tracks.
- [ ] **Measure and draw disagree by 4px a side** - width is text + Scale(16)*2 but the text is inset Scale(12).
- [ ] **Silently caps at 5 chips**, with nothing to say more are selected.
- [ ] Divider sits at chipY + Scale(32) while chips are Scale(24) tall - an unrelated constant.
- [ ] No GetPreferredItemHeight override.

### ChakraUIListBoxPainter
- [x] *fixed*: overlapping two-line layout now uses DrawTitleAndSubtitle.
- [ ] Paints through BeepStyling x3.

### ChatListBoxPainter
- [x] **Message line reads SubText** - a chat list built from SimpleItem shows names and no messages.
- [ ] **Avatar centred, text pinned** at Y+14 - they align only at the token row height.
- [ ] **Badge colour comes from a different property than Avatar's** - BeepListItem.BadgeColor here, item.BadgeBackColor there.
- [ ] Unread pill falls back to PrimaryColor where a semantic slot exists.

### CheckboxListPainter
- [ ] **Its 'checked' source differs from Standard's** - item.IsChecked here, _owner.SelectedItems.Contains there.
- [ ] Two spellings of DPI scaling interleaved in one method.
- [ ] Math.Max(12, ...) floor is **unscaled**.
- [x] Two-line path reads SubText only; selection is a PrimaryColor gradient.

### ChipStyleListBoxPainter
- [ ] **The close button is painted but not clickable** - no hit area is registered for it.
- [x] *fixed*: literal white in the 4-argument FromArgb(alpha,255,255,255) form - a white sheen over an accent chip on any dark theme.
- [x] *fixed*: the unselected chip's OnPrimaryColor-as-background fallback is now ListBackColor.
- [x] DrawItemBackground overridden **empty** - the row behind the chip is transparent.

### ColoredSelectionPainter
- [x] **Checkbox colour chosen by sniffing text** - item.Text.Contains('custom'). Renaming an item changes its colour.
- [x] *fixed*: two rects sized Width - currentX, mixing a width with an absolute X - text clipped early in any indented list.
- [x] *fixed*: a third literal-white form, FromArgb((int)(alpha*1.3f),255,255,255).
- [ ] Two-line layout is the split-in-half pattern.

### CommandListBoxPainter
- [x] *fixed*: **the shortcut chip was scaled twice** - ScaleValue applied to MeasureText(...).Width, already device pixels.
- [x] Shortcut reads SubText - invisible for ordinary items.
- [ ] Private RoundedRect duplicating GraphicsExtensions; Paint overrides only to call base.Paint.

### CompactListPainter
- [ ] Inherits Minimal but **does** fill unselected rows - unlike its parent.
- [x] Selection uses PrimaryColor.
- [ ] Padding is ItemPaddingH / 2 **before** scaling - integer division loses a pixel first.

### ContactListBoxPainter
- [ ] **The one painter that gets vertical centring right** - it counts its lines, computes totalTextH and centres the block. This is the pattern the others should copy.
- [x] Reads SubText and SubText2, so both extra lines are dead for ordinary items.

### CustomListPainter
- [ ] **A custom renderer bypasses the background entirely** - DrawItem calls CustomItemRenderer instead of DrawItemBackgroundEx, so a host-supplied renderer gets no hover, selection or focus unless it draws all three itself. Undocumented.
- [ ] Paints through BeepStyling x3.

### ErrorStatesPainter
- [x] **Error state decided by sniffing text** - Text.Contains('part-time') or Description.Contains('prohibited'). Demo strings deciding a semantic state.
- [x] **Hardcoded English drawn to screen** - 'Error state!' and 'Option now prohibited'.
- [ ] Same Width - currentX rect bug as ColoredSelection, at two sites.
- [x] *fixed*: 4 hard-coded ambers now WarningColor veils.

### FilledListBoxPainter
- [x] *fixed*: hover, surface and border greys now ListItemHoverBackColor / PanelBackColor / BorderColor; white ink now OnPrimaryColor.
- [x] Selection uses PrimaryColor.

### FilledStylePainter
- [x] *fixed*: white ink now OnPrimaryColor; the (74,144,226) tick now AccentColor.
- [ ] Paints through BeepStyling x3.

### FilterStatusPainter
- [x] **State decided by sniffing text** - Contains('error'), ('delivery'), ('payment'), ('alert') choose the colour.
- [x] *fixed*: 5 status literals now WarningColor / SecondaryTextColor / ErrorColor, and the OnPrimaryColor-as-background fallback now ListBackColor.

### GlassmorphismListBoxPainter
- [ ] **32 unscaled pixel literals** - the most in the folder.
- [ ] **More literal white** - FromArgb(80,255,255,255) for the sheen.
- [x] Sub-line reads SubText and uses the split-in-half rect.
- [ ] DrawItemBackground overridden empty - the glass panel is drawn in DrawItem.

### GradientCardListBoxPainter
- [x] **O(n squared) painting** - GetVisibleItems().IndexOf(item) runs a linear search per item, per paint, only to pick a gradient.
- [x] *fixed*: 6 static readonly web gradients - every theme drew the same purple-blue cards; built from theme pairs now.
- [x] Sub-line reads SubText, split-in-half rect; DrawItemBackground overridden empty.

### GroupedListPainter
- [ ] Leftover _theme?.SecondaryTextColor fallback chain.
- [ ] **Two different group-header paths** - one for BeepListItem.IsGroupHeader, another for item.Children.Count > 0, drawn by different methods with different colours.
- [ ] Paints through BeepStyling x5.

### HeroUIListBoxPainter
- [ ] **Description is used as a right-hand badge**, not a subtitle - the opposite meaning to every other painter.
- [ ] Leftover _theme?.LabelForeColor fallback chain.
- [ ] Paints through BeepStyling x3.

### InfiniteScrollListBoxPainter
- [x] **Reads the live mouse inside Paint** - PointToClient(Control.MousePosition) for the sentinel hover.
- [x] **The sentinel is drawn over the last row** - placed at drawingRect.Bottom - rowH with nothing reserved: the same defect as the PgUp/PgDn overlay that was removed.
- [x] **Hardcoded English** - 'Load more...'.

### LanguageSelectorPainter
- [ ] 26 loc; inherits WithIcons and only overrides the background to paint through BeepStyling.
- [ ] No GetPreferredItemHeight override.

### MaterialOutlinedListBoxPainter
- [ ] Paints through BeepStyling x3; the selected left bar uses PrimaryColor.
- [ ] Height is a bare Scale(48) rather than a token.

### MinimalListBoxPainter
- [x] **Fills nothing when a row is neither selected nor hovered** - transparent rows.
- [x] *fixed*: 2 Color.Empty fallbacks.

### MultiSelectionTealPainter
- [x] *fixed*: overlapping two-line layout now uses DrawTitleAndSubtitle; Color.Teal and a hard-coded teal now AccentColor.

### NavigationRailListBoxPainter
- [ ] **A hard-coded 14f font size** for the initials - ignores the theme's typography and DPI.
- [ ] **16 unscaled pixel literals**; RailItemHeight is a private const rather than a token.
- [ ] Paint overrides only to call base.Paint.

### NeumorphicListBoxPainter
- [x] **It deliberately forces a light surface** - if luminance is below 0.5 it lightens the base by 0.55. On any dark theme the whole style is lightened on purpose, which is why it stays pale in DarkTheme.
- [ ] Built on LightenColor / DarkenColor luminance shifts (10 uses).
- [x] Sub-line reads SubText, split-in-half rect; DrawItemBackground overridden empty.

### NotificationListBoxPainter
- [x] **A third meaning for SubText** - here it is the timestamp, while Description is the body. Chat uses SubText as the message; Command uses it as the shortcut.
- [ ] Title and body are pinned at Y+10 and +2 with a fixed Scale(34) body band - nothing is measured.

### OutlinedCheckboxesPainter
- [x] **Disabled state decided by sniffing text** - item.Text.Contains('disabled'), ignoring IsEnabled / IsDisabled.
- [x] *fixed*: hard-coded red (220,53,69) now ErrorColor.

### OutlinedListBoxPainter
- [ ] Fills unselected rows with ListBackColor - correct.
- [ ] Selection and hover use PrimaryColor / AccentColor rather than the ListItemSelected* slots.
- [ ] No GetPreferredItemHeight override.

### ProfileCardListBoxPainter
- [ ] Stacks with a running curY - correct - but the avatar is pinned at Y + Scale(8) and the bio band is a fixed Scale(28), so a long bio clips rather than growing the row.
- [x] Reads SubText **and** Description - the only painter using both.

### RadioSelectionPainter
- [x] *fixed*: split-in-half two-line arithmetic now uses DrawTitleAndSubtitle; 3 Color.Empty fallbacks.
- [ ] Paints through BeepStyling x3.

### RaisedCheckboxesPainter
- [x] **Disabled state sniffed from text**, twice - in GetItemTextColor and DrawRaisedCheckbox.
- [ ] The 'raised' accent is ErrorColor for every item, so the checkboxes are red regardless of meaning.

### RekaUIListBoxPainter
- [x] *fixed*: the title was VerticalCenter across the full row while the subtitle took the bottom half - they overlapped and the subtitle was sliced. Now measured and stacked, with a GetItemHeight override.
- [ ] Paints through BeepStyling x3.

### RoundedListBoxPainter
- [x] Fills unselected rows; selection uses PrimaryColor.
- [x] Shadow is a gradient over a **rectangle** behind a rounded path - the same square-shadow issue as CardList.

### SearchableListPainter
- [ ] **10 lines - it overrides nothing.** SupportsSearch returns true and it paints nothing of its own, so ListBoxType.SearchableList renders exactly as Standard.
- [ ] No GetPreferredItemHeight override.

### SimpleListPainter
- [x] Selection uses PrimaryColor; no GetPreferredItemHeight override.
- [x] *fixed*: white selected ink now OnPrimaryColor.

### StandardListBoxPainter
- [ ] The base style everything else inherits; fills unselected rows via DrawItemBackgroundEx.
- [ ] badgePad is a bare Scale(72) subtracted from the text width whether or not the badge is that wide.
- [ ] Selected ink is OnPrimaryColor while the fill is a PrimaryColor overlay - not the ListItemSelected* pair.
- [ ] No GetPreferredItemHeight override.

### TeamMembersPainter
- [ ] **Mixes scaled and unscaled units** - ImageSize is used raw if set, else Scale(28), then clamped between Scale(20) and Scale(40): a raw value compared against scaled bounds.
- [ ] Avatar is drawn on the **right**, unlike every other avatar style.

### ThreeLineListBoxPainter
- [ ] Own two/three-line layout rather than the shared helper.
- [x] Reads SubText and SubText2 - dead for ordinary items.

### TimelineListBoxPainter
- [ ] **11 unscaled pixel literals**; DrawItemBackground overridden empty, so rows are transparent.
- [x] Selection uses PrimaryColor; own two-line layout.
- [ ] The connector line and dots are positioned from constants independent of the row height, so they drift when the row grows.

### WithIconsListBoxPainter
- [ ] 25 lines; inherits everything else - no issues found.

### Patterns that repeat across painters

| pattern | painters |
|---|---|
| **state decided by sniffing item.Text** | ColoredSelection, ErrorStates, FilterStatus, OutlinedCheckboxes, RaisedCheckboxes |
| **SubText means three different things** | Chat = message, Command = shortcut, Notification = timestamp - and it is set in 2 places repo-wide, so all three are dead for ordinary items |
| **live mouse read inside paint** | CategoryChips, InfiniteScroll |
| **hardcoded English drawn to screen** | ErrorStates, InfiniteScroll |
| **square shadow behind a rounded card** | CardList, Rounded |
| **no unselected row fill** | Avatar, Borderless, ChipStyle, Glassmorphism, GradientCard, Minimal, Neumorphic, Timeline |
| **selection via PrimaryColor** | Card, Checkbox, Compact, Filled, GradientCard, MaterialOutlined, NavigationRail, Outlined, Rounded, Simple, Standard, Timeline |
| **own two-line layout** | 19 painters, marked individually above |
