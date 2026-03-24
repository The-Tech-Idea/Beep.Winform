# RadioGroup Enhancement Plan

> **Version:** 2026-Q1  
> **Baseline Build:** Current RadioGroup implementation  
> **Standards:** Material Design 3, Figma Design System tokens, WCAG 2.2, WAI-ARIA 1.2  
> **Scope:** `BeepRadioGroup`, `BeepHierarchicalRadioGroup`, all renderers, helpers, and models

---

## Executive Summary

The RadioGroup control already has a strong foundation (11 render styles, DPI awareness, accessibility metadata, interaction-state pipeline). This plan focuses on **four pillars** that close the gap between the current WinForms implementation and what Figma/modern web teams consider production-ready:

1. **Visual fidelity** – MD3 motion tokens, Figma-compatible color roles, sub-pixel rendering
2. **Renderer completeness** – consistent icon plumbing, ripple/animation, subtext support across all 11 renderers
3. **Icon pipeline simplification** – remove `ResolveSvgSymbolPath`, pass strings directly to `StyledImagePainter`
4. **Developer ergonomics** – scrolling, grouping, search, data binding, and design-time improvements

---

## Phase 1 — Icon Pipeline Simplification (Priority: CRITICAL)

### Goal
Remove the `ResolveSvgSymbolPath` helper method from `RadioGroupIconHelpers` entirely.  
`StyledImagePainter` already resolves symbolic SVG names; the helper layer is redundant and adds reflection overhead.

### Files to Change

#### [MODIFY] `RadioGroup/Helpers/RadioGroupIconHelpers.cs`

**Current (problematic):**
```csharp
public static string GetItemIconPath(string? imagePath, string? fallbackIcon = null)
{
    if (!string.IsNullOrEmpty(imagePath))
        return ResolveSvgSymbolPath(imagePath);   // ← remove this call

    if (!string.IsNullOrEmpty(fallbackIcon))
        return ResolveSvgSymbolPath(fallbackIcon); // ← remove this call

    return SvgsUI.Circle ?? SvgsUI.Check ?? SvgsUI.BoxMultiple;
}

private static string ResolveSvgSymbolPath(string iconPath) { ... } // ← delete entirely
```

**Replacement:**
```csharp
/// <summary>
/// Returns the icon path as-is; StyledImagePainter handles all resolution.
/// </summary>
public static string GetItemIconPath(string? imagePath, string? fallbackIcon = null)
{
    if (!string.IsNullOrEmpty(imagePath))
        return imagePath;

    if (!string.IsNullOrEmpty(fallbackIcon))
        return fallbackIcon;

    return SvgsUI.Circle ?? SvgsUI.Check ?? SvgsUI.BoxMultiple;
}
```

> **Impact:** All 12 renderers call `RadioGroupIconHelpers.GetItemIconPath()` — zero caller changes required.

---

## Phase 2 — MD3 Color Token Alignment (Priority: HIGH)

### Goal
Align all renderers to the canonical Material Design 3 color token model used in Figma handoff specs.  
Introduce a shared `RadioGroupColorTokens` record that maps `IBeepTheme` → MD3 roles.

### New: `RadioGroup/Models/RadioGroupColorTokens.cs`

```csharp
/// <summary>
/// Material Design 3 color roles resolved from IBeepTheme or control style fallbacks.
/// Aligns with Figma token names used in component handoff.
/// </summary>
public sealed record RadioGroupColorTokens
{
    // Surface roles
    public Color Surface          { get; init; }
    public Color SurfaceVariant   { get; init; }
    public Color SurfaceContainer { get; init; }  // NEW — MD3 surface-container

    // Content roles
    public Color OnSurface        { get; init; }
    public Color OnSurfaceVariant { get; init; }
    public Color Outline          { get; init; }
    public Color OutlineVariant   { get; init; }  // NEW — lighter border for subtle items

    // Interactive roles
    public Color Primary          { get; init; }
    public Color OnPrimary        { get; init; }
    public Color PrimaryContainer { get; init; }  // NEW — selected item background
    public Color OnPrimaryContainer { get; init; }// NEW — text on selected card/chip

    // State layers (pre-computed with opacity)
    public Color HoverStateLayer  { get; init; }  // Primary @ 8%
    public Color FocusStateLayer  { get; init; }  // Primary @ 12%
    public Color PressStateLayer  { get; init; }  // Primary @ 12%

    // Semantic
    public Color Error            { get; init; }
    public Color Disabled         { get; init; }
    public Color DisabledContainer { get; init; } // NEW — disabled item background

    public static RadioGroupColorTokens FromTheme(IBeepTheme theme, bool useThemeColors, BeepControlStyle style)
    {
        // ... resolve from theme or StyleColors fallback
    }
}
```

### Updated: `RadioGroup/Helpers/RadioGroupThemeHelpers.cs`
- Add `ResolveTokens(IBeepTheme, bool, BeepControlStyle)` factory
- Deprecate individual `GetXxxColor()` methods in favour of token resolution
- All renderers switch to consuming a single `RadioGroupColorTokens` instance per paint cycle

---

## Phase 3 — Renderer Parity & Visual Polish (Priority: HIGH)

### 3.1 Subtext Support Across All Renderers

Currently `SubText` is only partially rendered (MaterialRadioRenderer, CardRadioRenderer). All renderers must support it uniformly.

**Affected renderers:**
- `FlatRadioRenderer` — add secondary text line below label
- `ChipRadioRenderer` — show SubText as tooltip-style below chip (max 1 line, ellipsis)
- `PillRadioRenderer` — same as Chip
- `SegmentedRadioRenderer` — show SubText below segment label at smaller font size
- `TileRadioRenderer` — already supports it; verify spacing
- `CircularRadioRenderer` — show below label at 80% font size

**Helper update:** `RadioGroupFontHelpers.GetSubtextFont()` — verify all callers pass DPI-aware size.

### 3.2 Pressed State / Ripple Animation (**NEW** for non-Material renderers)

Renderers currently lacking a pressed state overlay:

| Renderer | Gap |
|---|---|
| `FlatRadioRenderer` | No pressed darkening |
| `ChipRadioRenderer` | No scale-down feedback |
| `PillRadioRenderer` | No ripple |
| `ButtonRadioRenderer` | No depth shadow on press |
| `CheckboxRadioRenderer` | No fill animation on check |
| `ToggleRadioRenderer` | Thumb slide not frame-based |

**Implementation approach:**
- Add `AnimationProgress` field to `RadioItemState` (float 0..1) — filled by a shared timer in `BeepRadioGroup`
- Renderers use `AnimationProgress` to interpolate pressed overlay alpha or geometry offset
- `BeepRadioGroup` hosts a single `System.Windows.Forms.Timer` (16ms ≈ 60 fps) that advances progress values and calls `RequestVisualRefresh()` only while animations are active

### 3.3 Focus Ring — WCAG 2.2 Compliance

WCAG 2.2 SC 2.4.11 requires a focus indicator with minimum 3:1 contrast against adjacent colors, minimum 2px outline, and minimum area of perimeter × 2px.

**Changes:**
- `MaterialRadioRenderer.DrawFocusIndicator()` — increase pen width to `2.5f`, use offset outset by 2px
- All other renderers — add a `DrawFocusIndicator()` method following the same rule
- Add `IFocusAwareRenderer` optional interface so `BeepRadioGroup.OnPaint` can call it uniformly when keyboard focus is active

### 3.4 Error / Validation Visual

When `HasError == true`, renderers should draw:
- A `2dp` error-colored border around the whole item (for Card, Tile, Button styles)
- A colored underline / indicator for Flat and Material styles
- Red tint on the indicator dot/checkbox for Circular, Checkbox styles

**Pass `RadioGroupColorTokens.Error` to all renderers via `RadioItemState.Tag` or a new `IsError` flag on `RadioItemState`.**

---

## Phase 4 — Layout & Scrolling Enhancements (Priority: MEDIUM)

### 4.1 Virtual Scrolling for Large Item Lists

When `Items.Count > VirtualizationThreshold` (default 50), `RadioGroupLayoutHelper` switches to virtual scrolling mode:
- Only items within the visible clip region are laid out and rendered
- `BeepRadioGroup` shows a vertical scrollbar via `VScrollBar` child control or custom scroll thumb
- `ScrollOffset` property drives layout origin

**New property:** `int VirtualizationThreshold { get; set; } = 50;`

### 4.2 Wrap Layout Mode

Add `RadioGroupOrientation.Wrap` — items flow horizontally and wrap to the next row (like CSS `flex-wrap`). Useful for Chip and Pill styles.

**`RadioGroupLayoutHelper` change:**
```csharp
case RadioGroupOrientation.Wrap:
    LayoutWrap(items, drawingRect, out rects);
    break;
```

### 4.3 Column Gap / Row Gap Tokens

Replace single `ItemSpacing` with:
```csharp
public int ColumnGap { get; set; } = 8;  // horizontal gap between items
public int RowGap    { get; set; } = 8;  // vertical gap between rows
```
`ItemSpacing` becomes a facade that sets both to the same value for backward compat.

---

## Phase 5 — Data Binding & API Ergonomics (Priority: MEDIUM)

### 5.1 Generic Data Source Binding

```csharp
/// <summary>
/// Binds the radio group to an IEnumerable data source.
/// </summary>
public void DataBind<T>(
    IEnumerable<T> source,
    Func<T, string> textSelector,
    Func<T, string>? valueSelector = null,
    Func<T, string>? iconSelector  = null,
    Func<T, string>? subTextSelector = null)
```

Internally converts `T` items to `SimpleItem` and calls `Items = ...`.

### 5.2 `SelectedItem<T>` Typed Return

```csharp
public T GetSelectedItem<T>() where T : class;
public IEnumerable<T> GetSelectedItems<T>() where T : class;
```

Stores original source objects in `SimpleItem.Tag`, casts on retrieval.

### 5.3 Item Template Callback

```csharp
public Func<SimpleItem, RadioItemState, Graphics, Rectangle, bool> CustomItemPainter { get; set; }
```
When set and returns `true`, `BeepRadioGroup` skips default rendering for that item. Enables fully custom per-item rendering without registering a new `IRadioGroupRenderer`.

### 5.4 Grouping / Section Headers

Add `SimpleItem.IsHeader` and `SimpleItem.GroupName` to support visual section labels between items (non-selectable). Layout helper skips hit testing on header items.

---

## Phase 6 — Inline Search / Filter (Priority: MEDIUM)

When `Items.Count > SearchThreshold` (default 10), show an optional search box above the list:

```csharp
public bool ShowSearchBox { get; set; } = false;  // explicit opt-in
public int  SearchThreshold { get; set; } = 10;    // auto-enable when ShowSearchBox = true
public string SearchPlaceholderText { get; set; } = "Search...";
```

- Search box is a child `TextBox` aligned to the top of `DrawingRect`
- Live filtering hides non-matching items (sets `RadioItemState.IsEnabled = false` for visual dimming, not for selection)
- Filtered items retain selection state

---

## Phase 7 — Design-Time & Developer Experience (Priority: MEDIUM)

### 7.1 Smart Tag Additions

| Action | Effect |
|---|---|
| **Enable Search Box** | Sets `ShowSearchBox = true` |
| **Enable Wrap Layout** | Sets `Orientation = Wrap` |
| **Add Sample Items** | Populates 3 sample `SimpleItem` entries |
| **Toggle Multi-Select** | Flips `AllowMultipleSelection` |
| **Apply Error Style** | Calls `HasError = true; ErrorMessage = "Sample error"` |

### 7.2 Design-Time Preview Renderer

In design mode (`DesignMode == true`), draw placeholder items so the control does not appear blank. The designer renderer draws 3 items: "Option A" (selected), "Option B", "Option C (Disabled)".

### 7.3 `[TypeConverter]` for `RadioGroupColorTokens`

Add a `TypeConverter` and `[Editor]` attribute on `ColorProfile` so developers can expand and edit individual color tokens in the VS property grid.

---

## Phase 8 — Accessibility Hardening (Priority: HIGH)

### 8.1 WAI-ARIA Role Mapping

Expose correct roles via `AccessibleObject`:
- Radio group container → `role="radiogroup"` (already: `AccessibleRole.Grouping`) ✅
- Each item in single-select mode → `role="radio"` (already `AccessibleRole.RadioButton`) ✅
- Each item in multi-select mode → `role="checkbox"` — **Currently returns RadioButton — fix this**
- Header items (Phase 5.4) → `role="presentation"` / not focusable

**Fix:** `BeepRadioGroupItemAccessibleObject.Role` must return `AccessibleRole.CheckButton` when `AllowMultipleSelection == true`.

### 8.2 Live Region for Selection Announcements

When a selection changes, announce to screen readers via:
```csharp
AccessibilityNotifyClients(AccessibleEvents.Selection, index);
AccessibilityNotifyClients(AccessibleEvents.StateChange, index);
```
Currently only `DescriptionChange` and `ValueChange` are notified at the group level — insufficient for item-level AT traversal.

### 8.3 High Contrast Mode Support

Detect `SystemInformation.HighContrast` and substitute all custom colors with system colors:
- `SystemColors.ControlText` for text
- `SystemColors.Highlight` for selected indicator
- `SystemColors.GrayText` for disabled
- `SystemColors.Window` for surface
- Use solid 2px borders instead of semi-transparent state layers

Add to `RadioGroupThemeHelpers`:
```csharp
public static RadioGroupColorTokens ForHighContrast()
```

---

## Phase 9 — Performance & Internal Housekeeping (Priority: LOW)

### 9.1 Cached Geometry

`RadioGroupLayoutHelper` currently recomputes all rects on every resize. Cache computed rectangles and invalidate only when `_layoutDirty == true`. Already partially implemented — enforce strict cache guard across all code paths.

### 9.2 Renderer Paint Batching

When `Orientation == Grid` with 50+ items, group Draw calls by state layer type to reduce GDI+ context switches:
1. All backgrounds
2. All indicators
3. All text

### 9.3 Dispose Guards on Fonts/Pens

Audit all renderers for `Font` / `Pen` / `Brush` objects that are created per-render but never cached. Introduce renderer-level `_pen`, `_brush` fields with proper `Dispose()` in renderer cleanup.

### 9.4 `IRadioGroupRenderer` Interface Extension

Add optional cleanup contract:
```csharp
public interface IRadioGroupRenderer
{
    // ... existing ...
    void Cleanup(); // called by BeepRadioGroup.Dispose()
}
```

---

## Implementation Order

| Phase | Priority | Complexity | Risk | Duration Estimate |
|---|---|---|---|---|
| Phase 1 — Icon pipeline fix | CRITICAL | Low | Low | 0.5 day |
| Phase 2 — Color tokens | HIGH | Medium | Low | 1 day |
| Phase 3 — Renderer parity | HIGH | High | Medium | 3 days |
| Phase 8 — Accessibility | HIGH | Medium | Low | 1.5 days |
| Phase 4 — Layout/scroll | MEDIUM | High | Medium | 2 days |
| Phase 5 — Data binding | MEDIUM | Medium | Low | 1 day |
| Phase 6 — Inline search | MEDIUM | Medium | Low | 1 day |
| Phase 7 — Design-time | MEDIUM | Low | Low | 0.5 day |
| Phase 9 — Performance | LOW | Low | Low | 0.5 day |

**Total estimated effort:** ~11 development days

---

## File Change Summary

### New Files
| File | Purpose |
|---|---|
| `Models/RadioGroupColorTokens.cs` | MD3 color token record (Phase 2) |
| `Renderers/BaseRadioRenderer.cs` | Abstract base class with shared focus/error/ripple drawing (Phase 3) |

### Modified Files
| File | Changes |
|---|---|
| `Helpers/RadioGroupIconHelpers.cs` | Remove `ResolveSvgSymbolPath`, pass strings directly (Phase 1) |
| `Helpers/RadioGroupThemeHelpers.cs` | Add `ResolveTokens()`, `ForHighContrast()` (Phases 2, 8) |
| `Helpers/RadioGroupLayoutHelper.cs` | Add `Wrap` layout, `ColumnGap`/`RowGap`, virtual scroll (Phase 4) |
| `Helpers/RadioGroupFontHelpers.cs` | DPI-aware subtext font verification (Phase 3.1) |
| `Renderers/IRadioGroupRenderer.cs` | Add `Cleanup()`, `IFocusAwareRenderer` (Phases 3.3, 9.4) |
| `Renderers/MaterialRadioRenderer.cs` | Switch to `RadioGroupColorTokens`, focus ring fix (Phases 2, 3.3) |
| `Renderers/FlatRadioRenderer.cs` | Subtext, pressed state, focus ring, error border (Phase 3) |
| `Renderers/ChipRadioRenderer.cs` | Subtext, scale-down press, focus ring (Phase 3) |
| `Renderers/PillRadioRenderer.cs` | Subtext, ripple, focus ring (Phase 3) |
| `Renderers/ButtonRadioRenderer.cs` | Depth shadow on press, focus ring (Phase 3) |
| `Renderers/CheckboxRadioRenderer.cs` | Check animation, focus ring (Phase 3) |
| `Renderers/ToggleRadioRenderer.cs` | Frame-based thumb slide (Phase 3) |
| `Renderers/CircularRadioRenderer.cs` | Subtext support (Phase 3.1) |
| `Renderers/SegmentedRadioRenderer.cs` | Subtext support (Phase 3.1) |
| `Renderers/TileRadioRenderer.cs` | Error border, color token switch (Phases 2, 3.4) |
| `Renderers/CardRadioRenderer.cs` | Color token switch, error border polish (Phase 2) |
| `BeepRadioGroup.cs` | Animation timer, search box child, virtual scroll, `DataBind<T>`, accessibility (Phases 3.2, 4, 5, 6, 8) |
| `BeepRadioGroup.Properties.cs` | New props: `VirtualizationThreshold`, `ShowSearchBox`, `SearchThreshold`, `ColumnGap`, `RowGap` (Phases 4, 6) |
| `BeepRadioGroup.Drawing.cs` | Group decoration pass for search box and headers (Phases 5.4, 6) |

---

## Verification Plan

### Phase 1 — Icon pipeline
- Build solution and confirm zero compilation errors
- Visual test: set `item.ImagePath = "Circle"` — icon must render (StyledImagePainter resolves the name)
- Visual test: set `item.ImagePath = "C:\\icons\\custom.svg"` — file path must render
- Visual test: set `item.ImagePath = null` — default circle icon must appear

### Phase 2 — Color tokens
- Apply `DarkTheme` and `LightTheme` via `ApplyTheme()` — verify all roles update without restart
- Set `UseThemeColors = false` and verify `ColorProfile` overrides apply correctly
- Inspect property grid: `ColorProfile` sub-properties must be individually editable

### Phase 3 — Renderer parity
- Switch through all 11 styles; verify subtext renders in each
- Trigger keyboard focus; verify 2px focus ring visible in all renderers with minimum 3:1 contrast
- Set `HasError = true` — verify error border/indicator in all renderers
- Press and hold mouse on item — verify pressed animation in non-Material renderers

### Phase 8 — Accessibility
- Run **Accessibility Insights for Windows** against form hosting `BeepRadioGroup`
- Verify: in multi-select mode item role reports `CheckButton`, not `RadioButton`
- Enable High Contrast theme in Windows → verify control renders with system colors only
- Use Narrator to navigate: verify each item announces name + state (selected/not selected)

### Build Verification
```
msbuild TheTechIdea.Beep.Winform.Controls.csproj /p:Configuration=Debug /t:Build
```

---

## Notes

- All changes maintain **full backward compatibility** — no existing public API is removed
- `ResolveSvgSymbolPath` removal is **non-breaking** because `GetItemIconPath` return type and signature are unchanged
- Animation timer is `null` by default and only activated when at least one item is in transition — no idle CPU cost
- `DataBind<T>` is in addition to existing `Items` setter — existing code continues to work unchanged
