# Ribbon visual design — taken from sources, not invented

Extracted from real implementations' actual color/state definitions:

- **Fluent.Ribbon** `Themes/Controls/RibbonTabItem.xaml` (state templates) and
  `Themes/Themes/Theme.Template.xaml` (brush derivations) — the modern flat design.
- **RibbonWinForms** `RibbonProfesionalRendererColorTable.cs` — the Office 2007 gradient design,
  fetched as a *negative* reference: it shows what "dated" looks like in numbers.

## The two lessons that matter more than any single color

**1. Interactive states are an accent ladder, not grays.** Fluent.Ribbon derives every hot state from
the theme's accent, in tint/alpha steps:

| state | brush |
|---|---|
| button hover | `AccentLight2` (light accent tint) |
| button pressed | `AccentLight3` |
| toggle checked | `Accent20` (accent @ 20% alpha) |
| toggle checked+hover | `Accent40` |
| gallery item hover / selected / pressed | `AccentLight3` / `AccentLight2` / `AccentLight1` |
| textbox focus border | `AccentBase` |

Chrome (non-interactive) comes from a neutral gray ramp: `Control.Border = Gray6`,
`Separator = Gray7`, `DropDown.Border = Gray5`. Two tiers — accent for anything that responds,
gray for structure. Our current painter uses theme colors but has no accent ladder for states.

**2. Modern is flat; gradients are the 2007 look.** RibbonWinForms' base table is gradient pairs
everywhere (`TabContentNorth/South = #C8D9ED/#E7F2FF`, glossy button fills, a 7-layer caption
gradient). Fluent.Ribbon has none of that: flat fills only. Any `LinearGradientBrush` in our ribbon
chrome reads as 2007.

## Tab states (from Fluent.Ribbon's template, verbatim roles)

| state | foreground | background | extra |
|---|---|---|---|
| normal | `RibbonTabItem.Foreground` | none | 1px vertical separator on right edge, low opacity |
| hover | `MouseOver.Foreground` | `MouseOver.Background` (subtle) | gray underline (`Gray2`) |
| selected | `Selected.Foreground` (= `Gray1`, near-black on light) | `Active.Background` (= content background — tab visually joins the content band) | **animated accent underline**, 3px |

Notably: Fluent.Ribbon does **not** render the selected tab as a white "card" — it uses an
underline whose thickness animates in. The tab joins the content because both use the same
`Active.Background`. Office 2016+ itself colors the selected tab *text* with the app accent; either
treatment is source-legitimate, underline is what our reference implements.

## Group visuals

- Group background = the tab content background (groups are not boxes on a different color).
- Group caption: smaller, dimmed text (`GroupBox.Header.Foreground`), centered, in the 22px strip.
- Inter-group separator: 1px `Gray7`-role line, inset vertically — **not** a border around the group.
  A ribbon group has no outline in the modern design; the box look comes from drawing borders.

## Mapping onto our theme system (rule 3: nothing assigns colors)

Every value above must resolve through `RibbonTheme` (fed by `RibbonThemeMapper` from
`BeepThemesManager`). Required roles and their derivations:

| ribbon role | derive from theme | today |
|---|---|---|
| tab fore normal / hover / selected | `Text` dimmed / `Text` / `Text` full + accent underline | partial |
| tab active background | = content background (same slot, one source) | `TabActiveBack` exists |
| command hover fill | accent @ ~15–20% alpha over surface | **missing — no accent ladder** |
| command pressed fill | accent @ ~30% alpha | **missing** |
| command checked fill | accent @ 20% alpha + accent border | **missing** |
| group caption fore | secondary/disabled text slot | `DisabledText` used |
| separators / chrome borders | border slot at reduced alpha | `GroupBorder` used |
| focus border | accent, full | `FocusBorder` exists |

Gap: `RibbonTheme` has no hover/pressed/checked slots. Add them as *computed* properties derived
from the accent (alpha steps), so a theme change reflows the whole ladder automatically — exactly
how Fluent.Ribbon's template derives `Accent20/40` from `AccentBase`. No literal ARGB anywhere.

## Rejected on evidence

- Gradient fills anywhere in ribbon chrome (2007 signature).
- Borders around groups (produces "boxes"; separators only).
- Gray-based hover states (the sources use accent tints; gray hover is toolbar styling).
- A hardcoded selected-tab palette (2007 table hardcodes `#15428B` text for every state — the exact
  anti-pattern rule 3 exists to prevent).

Application order: after the group rewrite lands (its verify pass is running), wire the accent
ladder into `RibbonTheme` + painter, then re-render and compare against this spec.
