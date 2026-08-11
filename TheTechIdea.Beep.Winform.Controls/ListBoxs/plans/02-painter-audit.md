# ListBoxs — painter-by-painter audit

One row per painter. `desc` = draws a second line of text; `2-line` = uses the shared
`BaseListBoxPainter.DrawTitleAndSubtitle`; `styling` = calls into `BeepStyling` (which pins the
style's own bundled theme, not the application's); `raw px` = numeric literals **not** wrapped in
`Scale(...)`, i.e. sizes that do not follow DPI; `lits` = colour literals remaining.

| painter | loc | desc | 2-line | styling | raw px | lits |
|---|---:|:--:|:--:|---:|---:|---:|
| `AvatarListBoxPainter` | 189 | Y | own | 0 | 0 | 0 |
| `BaseListBoxPainter` | 900 | Y | Y | 0 | 18 | 0 |
| `BorderlessListBoxPainter` | 51 | · | · | 0 | 1 | 0 |
| `CardListPainter` | 125 | · | · | 0 | 5 | 0 |
| `CategoryChipsPainter` | 110 | · | · | 0 | 4 | 0 |
| `ChakraUIListBoxPainter` | 205 | Y | Y | 3 | 0 | 0 |
| `ChatListBoxPainter` | 108 | Y | own | 0 | 1 | 0 |
| `CheckboxListPainter` | 157 | Y | own | 0 | 5 | 0 |
| `ChipStyleListBoxPainter` | 180 | Y | own | 0 | 8 | 0 |
| `ColoredSelectionPainter` | 116 | Y | own | 3 | 4 | 0 |
| `CommandListBoxPainter` | 102 | Y | own | 0 | 8 | 0 |
| `CompactListPainter` | 54 | · | · | 0 | 0 | 0 |
| `ContactListBoxPainter` | 77 | Y | own | 0 | 1 | 0 |
| `CustomListPainter` | 121 | · | · | 3 | 1 | 0 |
| `ErrorStatesPainter` | 165 | Y | own | 3 | 4 | 0 |
| `FilledListBoxPainter` | 97 | · | · | 0 | 3 | 0 |
| `FilledStylePainter` | 106 | · | · | 3 | 1 | 0 |
| `FilterStatusPainter` | 155 | · | · | 3 | 2 | 0 |
| `GlassmorphismListBoxPainter` | 172 | Y | own | 0 | 32 | 0 |
| `GradientCardListBoxPainter` | 240 | Y | own | 0 | 3 | 0 |
| `GroupedListPainter` | 99 | · | · | 5 | 1 | 0 |
| `HeroUIListBoxPainter` | 135 | Y | own | 3 | 0 | 0 |
| `InfiniteScrollListBoxPainter` | 63 | Y | own | 0 | 1 | 0 |
| `LanguageSelectorPainter` | 26 | · | · | 3 | 1 | 0 |
| `MaterialOutlinedListBoxPainter` | 46 | · | · | 3 | 0 | 0 |
| `MinimalListBoxPainter` | 39 | · | · | 0 | 2 | 0 |
| `MultiSelectionTealPainter` | 101 | Y | Y | 3 | 2 | 0 |
| `NavigationRailListBoxPainter` | 111 | Y | own | 0 | 16 | 0 |
| `NeumorphicListBoxPainter` | 235 | Y | own | 0 | 10 | 0 |
| `NotificationListBoxPainter` | 112 | Y | own | 0 | 3 | 0 |
| `OutlinedCheckboxesPainter` | 88 | · | · | 3 | 2 | 0 |
| `OutlinedListBoxPainter` | 57 | · | · | 0 | 0 | 0 |
| `ProfileCardListBoxPainter` | 79 | Y | own | 0 | 2 | 0 |
| `RadioSelectionPainter` | 127 | Y | Y | 3 | 2 | 0 |
| `RaisedCheckboxesPainter` | 105 | · | · | 3 | 1 | 0 |
| `RekaUIListBoxPainter` | 199 | Y | own | 3 | 3 | 0 |
| `RoundedListBoxPainter` | 110 | · | · | 0 | 4 | 0 |
| `SearchableListPainter` | 10 | · | · | 0 | 0 | 0 |
| `SimpleListPainter` | 75 | · | · | 0 | 1 | 0 |
| `StandardListBoxPainter` | 156 | Y | own | 0 | 5 | 0 |
| `TeamMembersPainter` | 96 | · | · | 3 | 2 | 0 |
| `ThreeLineListBoxPainter` | 115 | Y | own | 0 | 2 | 0 |
| `TimelineListBoxPainter` | 246 | Y | own | 0 | 11 | 0 |
| `WithIconsListBoxPainter` | 25 | · | · | 0 | 0 | 0 |

## Per-painter findings

### `AvatarListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `BaseListBoxPainter`
**Fixed**
- paging-hint overlay drawn over the last row - REMOVED
- GetItemHeight keyed on BeepListItem.SubText while 9 painters draw SimpleItem.Description - one HasSecondLine() authority now
- avatar palette was 7 hard-coded Material hues commented 'theme-independent' - now indexes theme slots
- 19 `_theme?.X ?? literal` sites -> Theme.X
- added DrawTitleAndSubtitle + Theme accessor
**Open**
- **18 unscaled numeric literals** — sizes that will not follow DPI

### `CardListPainter`
**Fixed**
- white selected-row ink -> OnPrimaryColor; black shadows -> ShadowColor veils

### `CategoryChipsPainter`
**Fixed**
- close-disc white -> OnPrimaryColor; black shadow -> ShadowColor

### `ChakraUIListBoxPainter`
**Fixed**
- same overlapping two-line layout - now uses DrawTitleAndSubtitle
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `ChatListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `CheckboxListPainter`
**Fixed**
- (26,32,44) ink literals -> ListItemForeColor
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `ChipStyleListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- **8 unscaled numeric literals** — sizes that will not follow DPI

### `ColoredSelectionPainter`
**Fixed**
- grey (120,120,120) -> SecondaryTextColor; white tick -> OnPrimaryColor
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `CommandListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- **8 unscaled numeric literals** — sizes that will not follow DPI

### `ContactListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `CustomListPainter`
**Fixed**
- (60,60,60) ink -> ListItemForeColor
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `ErrorStatesPainter`
**Fixed**
- warning badge was 4 hard-coded ambers -> WarningColor veils
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `FilledListBoxPainter`
**Fixed**
- hover/surface/border greys -> ListItemHoverBackColor / PanelBackColor / BorderColor

### `FilledStylePainter`
**Fixed**
- white ink -> OnPrimaryColor; (74,144,226) tick -> AccentColor
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `FilterStatusPainter`
**Fixed**
- 5 status literals (amber/gray/dark red) -> WarningColor / SecondaryTextColor / ErrorColor
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `GlassmorphismListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- **32 unscaled numeric literals** — sizes that will not follow DPI

### `GradientCardListBoxPainter`
**Fixed**
- 6 static readonly web gradients - every theme drew the same purple-blue cards; built from theme pairs per call
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `GroupedListPainter`
**Open**
- calls `BeepStyling` 5× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `HeroUIListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `InfiniteScrollListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `LanguageSelectorPainter`
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `MaterialOutlinedListBoxPainter`
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `MultiSelectionTealPainter`
**Fixed**
- same overlapping two-line layout - now uses DrawTitleAndSubtitle
- Color.Teal hover and a hard-coded teal (13,148,136) -> Theme.AccentColor
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `NavigationRailListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- **16 unscaled numeric literals** — sizes that will not follow DPI

### `NeumorphicListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- **10 unscaled numeric literals** — sizes that will not follow DPI

### `NotificationListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `OutlinedCheckboxesPainter`
**Fixed**
- hard-coded red (220,53,69) -> ErrorColor
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `ProfileCardListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `RadioSelectionPainter`
**Fixed**
- split-in-half two-line arithmetic -> DrawTitleAndSubtitle
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `RaisedCheckboxesPainter`
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `RekaUIListBoxPainter`
**Fixed**
- title drawn with VerticalCenter across the FULL row while the subtitle took the bottom half - they overlapped and the subtitle was sliced; now measured and stacked
- GetPreferredItemHeight was a flat 36 regardless of a second line - GetItemHeight override added
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `StandardListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `TeamMembersPainter`
**Open**
- calls `BeepStyling` 3× — the row background therefore comes from the **style's** bundled theme, not the application's (this is why dark themes still render light rows)

### `ThreeLineListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights

### `TimelineListBoxPainter`
**Open**
- draws a second line with **its own layout** — the same shape that sliced subtitles in the four that were fixed; not visibly clipping in the contact sheet, but unverified at other row heights
- **11 unscaled numeric literals** — sizes that will not follow DPI

