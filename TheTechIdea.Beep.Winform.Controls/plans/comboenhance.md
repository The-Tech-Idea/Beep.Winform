# BeepComboBox Enhancement Plan
**Reference designs:** Figma Community, Carbon IBM, Spectrum Adobe, Ant Design, Fluent 2, Atlassian  
**Last updated:** 2026-02-27  

---

## Progress Tracker

| ID | Enhancement | Phase | Priority | Status |
|----|-------------|-------|----------|--------|
| ENH-01 | Clear button (× icon) | 1 | P0 | ✅ Done |
| ENH-02 | Animated chevron rotation | 1 | P0 | ✅ Done |
| ENH-03 | Warning state (border + icon) | 1 | P0 | ✅ Done |
| ENH-04 | Read-only state styling | 1 | P0 | ✅ Done |
| ENH-05 | Size variants (sm / md / lg) | 1 | P0 | ✅ Done |
| ENH-06 | Option groups / headers | 3 | P2 | ✅ Done |
| ENH-07 | Empty-state placeholder row | 2 | P1 | ✅ Done |
| ENH-08 | Status icons in option rows | 2 | P1 | ✅ Done |
| ENH-09 | Full keyboard nav (Home/End/PgUp/PgDn) | 2 | P1 | ✅ Done |
| ENH-10 | MaxTagCount + overflow chip | 2 | P1 | ✅ Done |
| ENH-11 | Dropdown auto-flip (above/below) | 2 | P1 | ✅ Done |
| ENH-12 | ARIA / UIA accessibility annotations | 2 | P1 | ✅ Done |
| ENH-13 | MinDropdownWidth enforcement | 2 | P1 | ✅ Done |
| ENH-14 | Loading spinner overlay | 3 | P2 | ✅ Done |
| ENH-15 | ToolTip on overflow text | 3 | P2 | ✅ Done |
| ENH-16 | Option description sub-text | 4 | P3 | ✅ Done |
| ENH-17 | Divider / separator rows | 3 | P2 | ✅ Done |
| ENH-18 | Select-all checkbox (multi-select) | 3 | P2 | ✅ Done |
| ENH-19 | Chip-close propagates deselect | 3 | P2 | ✅ Done |
| ENH-20 | Reduce-motion toggle | 3 | P2 | ✅ Done |
| ENH-21 | Fuzzy / ranked search | 3 | P2 | ✅ Done |
| ENH-22 | Free-text tokenization | 4 | P3 | ✅ Done |
| ENH-23 | Skeleton loading state | 4 | P3 | ✅ Done |
| ENH-24 | RTL (right-to-left) layout | 4 | P3 | ✅ Done |

**Status legend:** ⬜ Not started · 🔄 In progress · ✅ Done · ❌ Blocked

---

## Cross-Cutting Requirements

### Icon system

All icons use `SvgsUI` path constants + `StyledImagePainter.PaintWithTint`.  
**No new IBeepTheme properties are needed** – all tints map to existing properties.

| Purpose | `SvgsUI` constant | Tint source |
|---------|-------------------|-------------|
| Chevron open/close | `SvgsUI.ChevronDown` | `ComboBoxForeColor` / `ComboBoxHoverForeColor` |
| Clear button | `SvgsUI.XCircle` | `ComboBoxForeColor` (hover: `ComboBoxHoverForeColor`) |
| Chip close | `SvgsUI.X` | `ComboBoxSelectedForeColor` |
| Error icon | `SvgsUI.AlertCircle` | `ErrorColor` |
| Warning icon | `SvgsUI.AlertTriangle` | `WarningColor` |
| Loading spinner | `SvgsUI.Loader` | `Color.FromArgb(178, ComboBoxForeColor)` |
| Row checkmark | `SvgsUI.Check` | `ComboBoxSelectedForeColor` |
| Multi-select checkbox | `SvgsUI.CheckSquare` / `SvgsUI.Square` / `SvgsUI.MinusSquare` | `ComboBoxSelectedForeColor` |

#### Shared helper – `DrawSvgIcon` (add to `BaseComboBoxPainter`)

```csharp
private static void DrawSvgIcon(
    Graphics g, Rectangle rect, string svgPath,
    Color tint, float opacity = 1f, float rotationDeg = 0f)
{
    if (rect.IsEmpty || string.IsNullOrEmpty(svgPath)) return;
    using var path = new GraphicsPath();
    path.AddRectangle(rect);
    if (rotationDeg != 0f)
    {
        var state = g.Save();
        float cx = rect.X + rect.Width / 2f, cy = rect.Y + rect.Height / 2f;
        g.TranslateTransform(cx, cy);
        g.RotateTransform(rotationDeg);
        g.TranslateTransform(-cx, -cy);
        StyledImagePainter.PaintWithTint(g, path, svgPath, tint, opacity);
        g.Restore(state);
    }
    else
    {
        StyledImagePainter.PaintWithTint(g, path, svgPath, tint, opacity);
    }
}
```

#### Icon tint helper – `GetIconTint` + `BeepComboBoxIconRole` enum

```csharp
public enum BeepComboBoxIconRole
{
    Chevron, ClearNormal, ClearHover,
    Error, Warning, Spinner,
    ChipClose, Checkmark
}

private Color GetIconTint(BeepComboBoxIconRole role, IBeepTheme theme, BeepComboBox owner)
    => role switch
    {
        BeepComboBoxIconRole.Chevron     => owner.IsButtonHovered
                                             ? theme.ComboBoxHoverForeColor
                                             : theme.ComboBoxForeColor,
        BeepComboBoxIconRole.ClearNormal => theme.ComboBoxForeColor,
        BeepComboBoxIconRole.ClearHover  => theme.ComboBoxHoverForeColor,
        BeepComboBoxIconRole.Error       => theme.ErrorColor,
        BeepComboBoxIconRole.Warning     => theme.WarningColor,
        BeepComboBoxIconRole.Spinner     => Color.FromArgb(178, theme.ComboBoxForeColor),
        BeepComboBoxIconRole.ChipClose   => theme.ComboBoxSelectedForeColor,
        BeepComboBoxIconRole.Checkmark   => theme.ComboBoxSelectedForeColor,
        _                                => theme.ComboBoxForeColor
    };
```

### Color mapping (zero new IBeepTheme properties)

| Visual need | Existing `IBeepTheme` property |
|-------------|-------------------------------|
| Warning border/icon | `WarningColor` |
| Warning bg tint | `Color.FromArgb(20, theme.WarningColor)` (computed) |
| Error border/icon | `ErrorColor` |
| Read-only background | `DisabledBackColor` |
| Read-only border | `InactiveBorderColor` |
| Read-only foreground | `DisabledForeColor` |
| Focus ring / active border | `ActiveBorderColor` |
| Success indicator | `SuccessColor` |

---

## Phase 1 — P0 (implement first)

### ENH-01 · Clear Button

**Goal:** Show an `×` icon button inside the control (trailing side) when a value is selected and `ShowClearButton = true`.

**Properties to add to `BeepComboBox`:**
```csharp
public bool ShowClearButton { get; set; } = false;
private bool _clearButtonHovered;
private Rectangle _clearButtonRect;
```

**Painter changes (`BaseComboBoxPainter`):**
1. In `CalculateLayout` — reserve `_clearButtonRect` (20 × 20) between text area and dropdown button when `ShowClearButton && !string.IsNullOrEmpty(owner.SelectedText)`.
2. In `DrawControl` — call `DrawSvgIcon(g, _clearButtonRect, SvgsUI.XCircle, GetIconTint(ClearNormal/ClearHover, ...), opacity: 0.7f)`.
3. In `BeepComboBox.Core.cs` `OnMouseMove` — detect hover over `_clearButtonRect`, set `_clearButtonHovered`, invalidate.
4. In `BeepComboBox.Core.cs` `OnMouseDown` — if click lands in `_clearButtonRect`, call `ClearSelection()`.

**`ClearSelection()` method:**
```csharp
public void ClearSelection()
{
    SelectedIndex = -1;
    SelectedValue = null;
    Text = string.Empty;
    OnSelectedIndexChanged(EventArgs.Empty);
    Invalidate();
}
```

**Design reference:** Ant Design Select — clear icon appears on hover; Carbon Combo Box — persistent `×` when cleared.

---

### ENH-02 · Animated Chevron Rotation

**Goal:** Chevron rotates 0 → 180° when dropdown opens, reverses on close.  Respects `ReduceMotion` flag.

**Properties:**
```csharp
public bool AnimateChevron { get; set; } = true;
public bool ReduceMotion   { get; set; } = false;
private float _chevronAngle = 0f;          // 0 = closed, 180 = open
private Timer _chevronTimer;
private const int ChevronAnimMs = 150;     // total animation duration
private const int ChevronTimerInterval = 16; // ~60 fps
```

**Logic:**
- On dropdown open: if `AnimateChevron && !ReduceMotion` → start timer incrementing `_chevronAngle` by `(180f / (ChevronAnimMs / ChevronTimerInterval))` per tick until 180; else snap to 180.
- On dropdown close: reverse.
- Pass `_chevronAngle` to `DrawSvgIcon(..., rotationDeg: _chevronAngle)`.

---

### ENH-03 · Warning State

**Goal:** A `Warning` validation state parallel to the existing `Error` state.

**Enum addition:**
```csharp
// In BeepEnums or inline in BeepComboBox:
public enum BeepComboBoxValidationState { None, Error, Warning, Success }
```

**Property:**
```csharp
public BeepComboBoxValidationState ValidationState { get; set; }
```

**Painter changes:**
- Border color: `ValidationState == Warning ? theme.WarningColor : (existing logic)`
- Background tint: fill a `Color.FromArgb(20, theme.WarningColor)` rectangle behind content area.
- Trail icon: `DrawSvgIcon(warningIconRect, SvgsUI.AlertTriangle, theme.WarningColor)`.

---

### ENH-04 · Read-Only State

**Goal:** Visual distinction when `ReadOnly = true` — hatched or flat background, no dropdown button interaction.

**Painter changes:**
```csharp
if (owner.ReadOnly)
{
    using var brush = new SolidBrush(theme.DisabledBackColor);
    g.FillRectangle(brush, contentRect);
    // Optionally draw subtle hatch:
    using var hatch = new HatchBrush(HatchStyle.LightUpwardDiagonal,
        Color.FromArgb(15, theme.DisabledForeColor), Color.Transparent);
    g.FillRectangle(hatch, contentRect);
    // Border uses InactiveBorderColor
    DrawBorder(g, bounds, theme.InactiveBorderColor, theme.BorderRadius);
    // Hide dropdown button
    return;
}
```

---

### ENH-05 · Size Variants

**Goal:** Three predefined size tokens: `Small` (24 px), `Medium` (32 px), `Large` (40 px).

**Enum + property:**
```csharp
public enum BeepComboBoxSize { Small, Medium, Large }
public BeepComboBoxSize SizeVariant { get; set; } = BeepComboBoxSize.Medium;
```

**In `ApplyLayoutDefaultsFromPainter`:**
```csharp
int baseHeight = SizeVariant switch
{
    BeepComboBoxSize.Small  => ScaleLogicalY(24),
    BeepComboBoxSize.Large  => ScaleLogicalY(40),
    _                       => ScaleLogicalY(32)
};
if (Height != baseHeight && !HeightSetExplicitly)
    Height = baseHeight;
```

Font size scales proportionally: Small = –1pt, Large = +1pt from `ComboBoxItemFont`.

---

## Phase 2 — P1

### ENH-07 · Empty-State Placeholder Row

When the filtered list is empty, render a single non-selectable row:  
`"No options"` (customizable via `EmptyStateText` property).  
Style: centered, `DisabledForeColor`, italic caption font.

---

### ENH-08 · Status Icons in Option Rows

Add optional `ImagePath` / `SvgsUI` icon per `BeepComboItem`.  
Painter allocates 20 px on leading side of each row; calls `DrawSvgIcon`.

---

### ENH-09 · Full Keyboard Navigation

| Key | Behaviour |
|-----|-----------|
| `Home` | Select first item |
| `End` | Select last item |
| `PageUp` | Move up by visible row count |
| `PageDown` | Move down by visible row count |
| `Escape` | Close dropdown, restore prior value |
| `Enter` | Confirm selection |
| `Backspace` | (multi-select) Remove last chip |

---

### ENH-10 · MaxTagCount + Overflow Chip

```csharp
public int MaxTagCount { get; set; } = 0; // 0 = unlimited
```

When selected chips exceed `MaxTagCount`, render a `+N` overflow chip using `ComboBoxSelectedBackColor`.

---

### ENH-11 · Dropdown Auto-Flip

Before opening dropdown, measure available screen space above and below.  
If insufficient space below → open upward.  
Property: `public bool AutoFlip { get; set; } = true;`

---

### ENH-12 · Accessibility (ARIA / UIA)

- Set `AccessibleRole = AccessibleRole.ComboBox`.
- Expose `AccessibleName`, `AccessibleDescription`.
- Raise `AccessibleEvents.StateChange` on open/close.
- Selected item → `AccessibleObject.Value`.

---

### ENH-13 · MinDropdownWidth

```csharp
public int MinDropdownWidth { get; set; } = 0;
```

Dropdown panel width = `Math.Max(Width, MinDropdownWidth)`.

---

## Phase 3 — P2

### ENH-06 · Option Groups / Headers

`BeepComboItem` gains `GroupName` property. Painter renders group header rows:  
semi-bold caption font, `DisabledForeColor`, with a horizontal rule below.  
Group headers are not selectable.

---

### ENH-14 · Loading Spinner Overlay

```csharp
public bool IsLoading { get; set; } = false;
private float _loadingRotationAngle = 0f;
private Timer _spinnerTimer;
```

When `IsLoading`: draw `SvgsUI.Loader` at 70 % opacity, rotating at 6°/tick via a 16 ms timer.  
Disable keyboard and mouse interaction.

---

### ENH-15 · Tooltip on Overflow Text

When display text is truncated (measured via `Graphics.MeasureString`), attach a `ToolTip` showing full text on hover.

---

### ENH-17 · Separator Rows

`BeepComboItem` gains `IsSeparator` bool.  
Painter renders a 1 px horizontal line using `InactiveBorderColor`; row is not selectable.

---

### ENH-18 · Select All (Multi-select)

When `IsMultiSelect` and list count > 0, add a pinned "Select all / Clear all" toggle row at list top.  
Uses `MinusSquare` icon when partially selected, `CheckSquare` when all selected, `Square` when none.

---

### ENH-19 · Chip-Close Propagates Deselect

When user clicks `×` on a chip, call `DeselectItem(item)` which updates backing `SelectedItems` and raises `SelectedIndexChanged`.

---

### ENH-20 · Reduce Motion

```csharp
public bool ReduceMotion { get; set; } = false;
```

When `true`: skip all timer-driven animations (chevron, spinner sweep) and snap to final state immediately.  
Default can be derived from `SystemInformation.IsMinimizeAnimationEnabled` inverse.

---

### ENH-21 · Fuzzy / Ranked Search

Current live-search filters by `StartsWith`. Upgrade to scored fuzzy match:
1. Exact prefix match → score 100.
2. Contains match → score 50.
3. Character-sequence subsequence match → score 10.

Sort items descending by score; items with score 0 are hidden.

---

## Phase 4 — P3

### ENH-16 · Option Description Sub-text

`BeepComboItem` gains `Description` string.  
Painter renders it in smaller font (`BodySmall` style) below the label, `DisabledForeColor`.  
Row height auto-expands to fit.

---

### ENH-22 · Free-text Tokenization

```csharp
public bool AllowFreeText { get; set; } = false;
public char[] TokenDelimiters { get; set; } = new[] { ',', ';' };
```

When user types a delimiter, current text is wrapped into a chip.

---

### ENH-23 · Skeleton Loading State

```csharp
public bool ShowSkeleton { get; set; } = false;
```

When `true`: render an animated shimmer gradient striped rectangle instead of content (Carbon IBM pattern).  
Uses a 2-second CSS-style animation via `System.Windows.Forms.Timer` + linear gradient sweep.

---

### ENH-24 · RTL Layout

```csharp
public bool IsRtl { get; set; } = false;
```

Flip all rect calculations: dropdown button moves to leading (left) side, text aligns right, chips flow right-to-left.  
`Graphics.Transform` + horizontal mirror applied once at start of `DrawControl`.

---

## Files to Edit

| File | Changes |
|------|---------|
| `ComboBoxes/BeepComboBox.Core.cs` | New properties, mouse/keyboard handlers for ENH-01/09/10/11 |
| `ComboBoxes/BeepComboBox.Methods.cs` | `ClearSelection`, `DeselectItem`, `SizeVariant` apply logic |
| `ComboBoxes/BeepComboBox.Events.cs` | New events: `OnCleared`, `OnTagCountExceeded` |
| `ComboBoxes/BeepComboBox.Designer.cs` | New browsable properties |
| `ComboBoxes/Painters/BaseComboBoxPainter.cs` | `DrawSvgIcon`, `GetIconTint`, `BeepComboBoxIconRole`, all draw changes |
| `ComboBoxes/Painters/*Painter.cs` | Override paint hooks per style |
| `DataStructures/BeepComboItem.cs` | `GroupName`, `Description`, `IsSeparator`, `ImagePath` fields |

---

## Done tracker (fill in as work completes)

| Date | ID | Notes |
|------|----|-------|
| 2026-02-27 | ENH-01 | `ShowClearButton` prop + `_clearButtonRect` layout + `ClearSelection()` + `DrawClearButton()` + mouse hit-testing |
| 2026-02-27 | ENH-02 | `AnimateChevron`/`ReduceMotion` props + `_chevronAngle` field + `TriggerChevronAnimation()` + SVG `DrawDropdownButton` |
| 2026-02-27 | ENH-03 | `BeepComboBoxValidationState` enum + `ValidationState` prop + background tint + `DrawValidationIcon()` |
| 2026-02-27 | ENH-04 | `IsReadOnly` (BaseControl) read in `Paint()` — `DisabledBackColor` fill + hatch brush overlay |
| 2026-02-27 | ENH-05 | `BeepComboBoxSize` enum + `SizeVariant` prop + height applied in `ApplyLayoutDefaultsFromPainter` |
| 2026-02-27 | ENH-07 | `EmptyStateText` prop + disabled placeholder row when list empty — guard removed from ShowDropdown |
| 2026-02-27 | ENH-08 | `ShowStatusIcons` prop → `BeepContextMenu.ShowImage`; uses existing `SimpleItem.ImagePath` |
| 2026-02-27 | ENH-09 | `Backspace` key case added to `OnKeyDown`; existing Home/End/PgUp/PgDn/Enter/Escape/Space already present |
| 2026-02-27 | ENH-10 | Already rendered by `MultiSelectChipsPainter` via `MaxDisplayChips` + `+N` counter chip |
| 2026-02-27 | ENH-11 | `AutoFlip` prop; `Screen.FromControl().WorkingArea` check in `ShowDropdown()` — opens upward when needed |
| 2026-02-27 | ENH-12 | `BeepComboBoxAccessibleObject` nested class + `AccessibilityNotifyClients` on open/close/select |
| 2026-02-27 | ENH-13 | `MinDropdownWidth` prop → `Math.Max(Width, Math.Max(ScaleLogicalX(160), MinDropdownWidth))` |
| 2026-02-27 | ENH-06 | `GroupName` added to `SimpleItem`; group header rows (disabled items) inserted in `ShowDropdown()` |
| 2026-02-27 | ENH-14 | `DrawSpinner()` in `BaseComboBoxPainter` — haze overlay + rotating `SvgsUI.Loader`; `LoadingRotationAngle` accessor |
| 2026-02-27 | ENH-15 | `_overflowTooltip` ToolTip field in Core; show/hide in `OnMouseMove`/`OnMouseLeave` based on text truncation |
| 2026-02-27 | ENH-17 | `IsSeparator` added to `SimpleItem`; `BeepContextMenu.AddSeparator()` called for separator items |
| 2026-02-27 | ENH-18 | `ShowSelectAll` prop; virtual `_selectall` item prepended in multi-select dropdown; handled in `OnContextMenuItemClicked` |
| 2026-02-27 | ENH-19 | `ChipCloseRects` dictionary in Core; `DrawChip` draws × button + records rect; `OnMouseDown` calls `DeselectItem()` |
| 2026-02-27 | ENH-20 | Already implemented in Phase 1 as `ReduceMotion` prop |
| 2026-02-27 | ENH-21 | `FuzzySearchScore()` helper — prefix=100 / contains=50 / subsequence=10; results sorted descending |
