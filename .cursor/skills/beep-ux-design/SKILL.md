---
name: beep-ux-design
description: UI/UX design standards for Beep.Winform dialogs, forms, and controls — consistent sizing, alignment, accessibility, theming, and layout patterns. Use when creating or editing any visual surface in the Beep desktop framework.
---

# Beep UI/UX Design Standards

Use this skill when creating dialogs, forms, or editing layouts for Beep.Winform controls.

## 1) Sizing Standards — `BeepLayoutMetrics`

Always use `BeepLayoutMetrics` (in `Layouts/Helpers/`) instead of hardcoded pixel values. All values must be scaled via `.Scale()` for DPI awareness.

| Token | Value | Usage |
|-------|-------|-------|
| `DialogSmall` | 420 × 320 | Login, simple prompts |
| `DialogMedium` | 600 × 460 | Property editors, form builders |
| `DialogLarge` | 840 × 560 | Data grids, complex wizards |
| `DialogPadding` | 12 px | Outer form/dialog padding |
| `ContainerPadding` | 8 px | Panel/group padding |
| `ButtonStripPd` | 10 px | Button bar padding |
| `LabelColumnWidth` | 130 px | Label column in 2-col forms |
| `TextRowHeight` | 35 px | Single-line field rows |
| `InterRowSpacing` | 5 px | Between form rows |
| `ButtonStandard` | 100 × 32 | Default button size |
| `ButtonSmall` | 80 × 28 | Icon/toolbar buttons |
| `ButtonLarge` | 130 × 36 | Primary CTA buttons |
| `ButtonToolbar` | 110 × 32 | Toolbar buttons |
| `MinTouchTarget` | 44 px | WCAG minimum touch size |
| `MinContrast` | 4.5 | WCAG AA text contrast |

## 2) Layout Patterns

### Form Layout (2-column)
```
TableLayoutPanel { ColumnCount=2, Padding=DialogPadding.ScalePadding() }
  Column 0: Absolute LabelColumnWidth.Scale() (label)
  Column 1: Percent 100F (input)
  Rows: AutoSize or Absolute TextRowHeight.Scale()
```

### Button Bar (footer)
```
FlowLayoutPanel { FlowDirection=RightToLeft, Padding=ButtonStripPd.ScalePadding() }
  Primary CTA (rightmost, ButtonLarge): OK, Save, Login
  Secondary (ButtonStandard): Cancel, Close
```

### Dialog Structure
```
Form { Size=DialogSmall.Scale(), FormBorderStyle=FixedDialog }
  TableLayoutPanel { Padding=DialogPadding.ScalePadding() }
    Row 0: Title label (TitleFontSize)
    Row 1: Subtitle (optional)
    Row 2: Error message (optional, red)
    Row 3-N: Form fields
    Row Last: Button bar
```

## 3) Accessibility Checklist

- [ ] TabIndex follows logical left-to-right, top-to-bottom order
- [ ] All controls without visible text have Caption / AccessibleName
- [ ] All interactive controls reachable by Tab
- [ ] Escape closes modal dialogs (CancelButton set)
- [ ] Enter triggers primary action (AcceptButton set)
- [ ] Focus indicators visible against all backgrounds
- [ ] Color not sole state differentiator (use icons + text)
- [ ] Touch targets >= MinTouchTarget.Scale()
- [ ] All pixel values through DpiScalingHelper or .Scale()

## 4) Theme-Aware Control Creation

Always use Beep controls, never plain WinForms:
- `new BeepButton { ... }` — NOT `new Button { ... }`
- `new BeepTextBox { ... }` — NOT `new TextBox { ... }`
- `new BeepLabel { ... }` — NOT `new Label { ... }`
- `new BeepCheckBoxBool { ... }` — NOT `new CheckBox { ... }`
- `new BeepComboBox { ... }` — NOT `new ComboBox { ... }`

Set `UseThemeColors = true` on all Beep controls to enable automatic theme application.

## 5) Anti-Patterns

- ❌ Hardcoded pixel values without DpiScalingHelper or .Scale()
- ❌ Plain WinForms controls where Beep equivalents exist
- ❌ Inconsistent padding within the same dialog
- ❌ Missing CancelButton or AcceptButton on modal dialogs
- ❌ Buttons without tooltips containing only icons
- ❌ Bare `catch { }` in UI event handlers
- ❌ `async void` without try-catch wrapping

## 6) Key Bug Fix

`StyleSpacing.GetPadding()` had a dead-code `return 1;` on line 16 that short-circuited all 36+ style-specific padding values. **Fixed in Session 21.** All padding now resolves correctly per `BeepControlStyle`.

## Related Skills
- [`beep-winform`](../beep-winform/SKILL.md) — core development rules
- [`datablock-connection-integration`](../datablock-connection-integration/SKILL.md) — block-connection wiring
- [`winform-integrated-ide`](../winform-integrated-ide/SKILL.md) — IDE extension patterns

## Detailed Reference
Use [`reference.md`](./reference.md) for per-control sizing examples and dialog templates.
