# DPI Scaling, Font Assignment & Redundant Paint Fix Plan

**Created:** 2026-03-02  
**Scope:** All Beep WinForms controls — BaseControl, BeepDatePicker, BeepDatePickerView, BeepComboBox and the shared infrastructure in `BeepThemesManager`, `BeepFontManager`, `FontListHelper`, `DpiScalingHelper`.  
**Reference standards:** Figma Design Tokens (W3C Draft), DevExpress Skins, Telerik RadControls, Fluent UI WinForms, Material Design 3 typography scale.  
**Priority:** High — the current multi-layer DPI multiplication is causing visual size regression across all themed controls on high-DPI monitors.

---

## 1. Executive Summary

The Beep control library currently applies DPI scaling in **four independent, uncoordinated places**. On a 144-DPI monitor (150%) a design-time 9 pt font ends up as a 27 pt visual — a 3× inflation — before it reaches the screen. Every theme-style change fires `ApplyTheme()`, which re-creates `Font` objects and calls `Invalidate()` unconditionally, causing the control to redraw for every property setter. The fixes below eliminate all but one DPI scaling code path, decouple logical font descriptors from physical `Font` object creation, and introduce a dirty-flag pattern to batch multiple property writes into a single repaint.

---

## 2. Diagnosed Root Causes

### 2.1 DPI Scaling Applied in Four Places Simultaneously

| Layer | Where | Behaviour |
|---|---|---|
| **WinForms AutoScale** | Parent `Form`/`UserControl` (`AutoScaleMode.Font` or `.Dpi`) | Scales every child's `this.Font` × DPI factor on `ScaleControl()`. Fires `OnFontChanged` on each control. |
| **BeepThemesManager.ToFont()** | `fontSize = style.FontSize * _dpiScaleFactor` | Multiplies the stored design-time point size by the *global* DPI factor, every call. |
| **BeepFontManager.GetFontForControl()** | `scaledSize = size * (control.DeviceDpi / 72f)` | Uses `DeviceDpi/72` — a pixel-conversion formula — then passes the result as if it were points, yielding 2× inflation at 144 DPI. |
| **DpiScalingHelper.ScaleFont()** | Called from `ScaleFontForDpi()` in derived controls | Mathematically a no-op for `GraphicsUnit.Point` fonts, but still a third scaling call site. |

**Combined effect at 150% DPI (144 dpi):**  
Design value = 9 pt →  
× 1.5 WinForms AutoScale = 13.5 pt (via `OnFontChanged`) →  
× 1.5 `BeepThemesManager._dpiScaleFactor` = 20.25 pt (via `ApplyTheme → ToFont`) →  
× 2.0 `DeviceDpi/72` formula = 40.5 pt final visual  
**Expected:** 13.5 pt (WinForms handles it; all others must not multiply again)

### 2.2 `this.Font` and `_textFont` Desynchronised

`ApplyTheme()` in most Beep controls assigns `_textFont = BeepThemesManager.ToFont(…)` but never sets `this.Font`. WinForms separately auto-scales `this.Font` via `ScaleControl()`. When `OnFontChanged` fires (from WinForms auto-scale), derived overrides re-capture `_textFont = this.Font`, clobbering the themed font. The control then paints with the unthemed WinForms-scaled font until the next `ApplyTheme()`, which rescales ToFont again — a fight that repeats every DPI change.

### 2.3 `BeepDatePicker.ApplyTheme()` Double Assignment

```csharp
// Line 907 (inside try):
_textFont = BeepThemesManager.ToFont(_currentTheme.DateFont);   // #1

// Line 930 (after Task.Run icon preload, always runs):
_textFont = BeepThemesManager.ToFont(_currentTheme.DateFont);   // #2 — redundant
```
Two identical calls, second always overwrites the first. Each `ToFont()` call allocates a new `Font` GDI object.

### 2.4 `BeepThemesManager.OnDpiScaleFactorChanged()` Fires Full Theme Reload

When DPI changes (user moves window to different monitor), `BeepThemesManager` calls `OnDpiScaleFactorChanged()` which fires `ThemeChanged`, causing every subscribed control to call `ApplyTheme()` → re-create `_textFont` → `Invalidate()`. For a form with 50 controls, this means 50 font re-creations and 50 redraws **per DPI change event**.

### 2.5 Every Property Setter Calls `Invalidate()` Independently

Setting a `Color`, a `Font`, a `PainterKind`, and `Text` on a control from code fires 4 independent `Invalidate()` calls, causing 4 paint cycles even though only 1 was needed.

### 2.6 Two Separate `EnableDpiFontScaling` Flags

`BeepThemesManager.EnableDpiFontScaling` (default `true`) and `FontListHelper.EnableDpiFontScaling` (default `false`) are independent. Callers may scale once or twice without knowing which path was taken.

---

## 3. Design Target — Commercial Standards Reference

### DevExpress Skin Model (Reference)
- `AppearanceObject` stores logical descriptors: `FontFamily`, `FontSize` (pt), `FontStyle`. No `Font` object.  
- Physical `Font` objects are created **once per unique (family, size, style) tuple** in a global `AppearanceDefaultFont` cache. Cache is keyed by logical descriptor; DPI is NOT part of the key.  
- Painters receive a `GraphicsCache` and call `graphics.GetFont(appearance)` which applies the device DPI once at draw time and never stores the result on the control.  
- `this.Font` on DevExpress editors is **always** the base ambient font; the skin appearance font is separate and does not override `this.Font`.

### Telerik RadControls (Reference)
- `RootRadElement.DpiScaleFactor` is a per-element scale computed from the element's host control `DeviceDpi`. It is applied once, inside `RadElement.Paint()`, to the cached layout metrics.  
- Theme tokens are stored as device-independent units. `px → dp → px` round-trips are forbidden.  
- A `ThemeComposite` is rebuilt only when the theme *name* changes, not on DPI changes.

### Fluent UI / Material Design 3 (Reference)
- **Type scale** defines 15 logical roles (Display Large → Label Small) each with a logical point size, weight, and tracking. These are **design tokens** — unitless design-time numbers.  
- Rendering maps roles to physical fonts once per render context. The application frame monitors `DisplayInformation.LogicalDpi` and updates the render context when it changes; individual components do not recalculate anything.

### Figma Design Token Standard (W3C Draft 2024)
```json
{
  "typography.body.medium": {
    "$type": "typography",
    "$value": { "fontFamily": "Roboto", "fontSize": "14", "fontWeight": "400", "lineHeight": "1.43" }
  }
}
```
`fontSize` is always a raw logical number (pt). The host applies the device pixel ratio **once** at the display boundary. Tokens are never multiplied by density inside the design system itself.

### Target Architecture for Beep

```
TypographyStyle (logical)      ← stored in theme, design-time pt values, NO DPI
    fontFamily: string
    fontSize: float (design points, e.g. 9f)
    fontStyle: FontStyle

BeepFontManager.GetPhysicalFont(TypographyStyle, Graphics)
    → applies ONE DPI conversion: pt → px via g.DpiY
    → caches by (family, round(px), style)
    → returns Font with GraphicsUnit.Pixel  ← never multiplied again

Painters / DrawContent
    → call GetPhysicalFont(style, g) at paint time
    → NEVER store result on control (no _textFont field)
    → NEVER set this.Font from theme
```

---

## 4. Phased Implementation Plan

---

### Phase 1 — Fix `BeepFontManager.GetFontForControl()` Formula (Quick Fix)

**File:** `FontManagement/BeepFontManager.cs`  
**Issue:** `scaledSize = sizeInPoints * (control.DeviceDpi / 72f)` uses `DeviceDpi/72` (a pt→px formula) as if the output were still in points, creating 2× inflation.

**Fix:**
```csharp
// WRONG (current):
float scaledSize = Math.Max(sizeInPoints * (control.DeviceDpi / 72.0f), 6.0f);

// CORRECT:
// Returns a pixel value; caller must create font with GraphicsUnit.Pixel
float scaledPx = Math.Max(sizeInPoints * (control.DeviceDpi / 96.0f), 6.0f);
// 96 is the reference DPI at which 1pt ≈ 1px in logical design space
// At 144 DPI: 9 * (144/96) = 13.5px — correct
```

Or, to stay in points (simplest change):
```csharp
// Keep output as points; WinForms will do pt→px automatically:
float scaledPt = Math.Max(sizeInPoints * (control.DeviceDpi / 96.0f), 6.0f);
return new Font(familyName, scaledPt, fontStyle, GraphicsUnit.Point);
```

**Acceptance:** At 144 DPI, a 9 pt design value renders as ~13.5 px (correct); previously rendered ~18 px (wrong).

---

### Phase 2 — Remove DPI Multiplication from `BeepThemesManager.ToFont()`

**File:** `ThemeManagement/BeepThemesManager.cs`  
**Issue:** `fontSize = style.FontSize * _dpiScaleFactor` multiplies the logical design-time size by the global DPI factor on every call. WinForms has already handled DPI for `this.Font`; painters that use `_textFont` for GDI drawing also get the unintended multiplication.

**Step 2a — Remove scale from ToFont(TypographyStyle)**
```csharp
// BEFORE:
public static Font ToFont(TypographyStyle style, bool applyDpiScaling = true)
{
    float fontSize = style.FontSize;
    if (applyDpiScaling && EnableDpiFontScaling && _dpiScaleFactor > 0)
        fontSize *= _dpiScaleFactor;
    …
}

// AFTER:
public static Font ToFont(TypographyStyle style)
{
    float fontSize = style.FontSize;   // logical pt — DPI-independent
    if (fontSize <= 0) fontSize = 9f;
    var familyName = string.IsNullOrWhiteSpace(style.FontFamily) ? "Segoe UI" : style.FontFamily;
    return BeepFontManager.GetOrCreate(familyName, fontSize, style.FontStyle)
           ?? SystemFonts.DefaultFont;
}
```

**Step 2b — Remove `applyDpiScaling` parameter from all three overloads**  
The boolean is no longer needed; remove it (or keep as deprecated no-op for binary compatibility).

**Step 2c — Remove `EnableDpiFontScaling` flag from `BeepThemesManager`**  
Merge with `FontListHelper.EnableDpiFontScaling` (already `false` by default). Delete the duplicate; the one in `FontListHelper` controls font loading; there is no concept of "scaling during font lookup".

**Step 2d — `ToFontForControl(TypographyStyle, Control)` — Apply DPI here only**  
This overload is intended for painters that need a device-specific font. Keep the DPI application here:
```csharp
public static Font ToFontForControl(TypographyStyle style, Control control)
{
    float dpiScale = control.DeviceDpi / 96.0f;   // 1.0 at 96 DPI
    float fontSize = style.FontSize * dpiScale;
    if (fontSize <= 0) fontSize = 9f;
    var familyName = string.IsNullOrWhiteSpace(style.FontFamily) ? "Segoe UI" : style.FontFamily;
    return BeepFontManager.GetOrCreate(familyName, fontSize, style.FontStyle)
           ?? SystemFonts.DefaultFont;
}
```
Rename to `ToPhysicalFont(TypographyStyle, Control)` to be explicit that the result is device-specific.

**Acceptance:** At 150% DPI, `ToFont(theme.DateFont)` where `DateFont.FontSize = 9` returns a 9 pt font. `ToPhysicalFont(theme.DateFont, control)` returns a 13.5 pt font (correct device rendering size).

---

### Phase 3 — Separate Logical Store from Physical Font in `ApplyTheme()`

**Principle (based on DevExpress / Fluent pattern):**  
`ApplyTheme()` stores a reference to the `TypographyStyle` (or its fields) — it does **not** create a `Font` object and does **not** assign `this.Font`. Physical `Font` objects are created inside `DrawContent(Graphics g)` where the `Graphics` DPI context is known.

**Step 3a — Add a logical font descriptor field to affected controls**
```csharp
// In BeepDatePicker (and all date controls):
private TypographyStyle _logicalTextStyle;  // set in ApplyTheme
```

**Step 3b — Update `ApplyTheme()` in `BeepDatePicker`**
```csharp
protected override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Store LOGICAL descriptor — no Font object created, no this.Font assignment
    _logicalTextStyle = _currentTheme.DateFont;

    // Colours
    BackColor = _currentTheme.CalendarBackColor;
    ForeColor = _currentTheme.CalendarForeColor;
    BorderColor = _currentTheme.CalendarBorderColor;

    // Kick one redraw (batched, see Phase 4)
    InvalidateOnce();
}
```
- **Remove**: Both `_textFont = BeepThemesManager.ToFont(…)` assignments (lines 907 and 930).
- **Remove**: The `try { } catch { }` around the first `ToFont` call (exception-hiding anti-pattern).

**Step 3c — Resolve physical font in `DrawContent()`**
```csharp
protected override void DrawContent(Graphics g)
{
    // Resolve physical font once per paint, in the correct DPI context
    using var textFont = BeepThemesManager.ToFontForControl(_logicalTextStyle ?? DefaultStyle, this);
    // pass textFont to painting helpers — do NOT store it back to a field
    DrawDateText(g, textFont);
    DrawDropDownButton(g);
}
```
> **Note on `using`:** If `BeepFontManager.GetOrCreate` is a cache (recommended), `using` is wrong — do not dispose a cached font. Wrap only if the font is freshly created for each call. The cache approach is preferred; see Phase 5.

**Step 3d — Remove `_textFont` field from `BeepDatePicker`**  
After step 3b/3c are complete, `_textFont` is no longer needed as a persistent field. Remove it to prevent future accidental re-introduction of the stale-font pattern.

**Step 3e — Update `OnFontChanged()` in `BeepDatePicker`**
```csharp
// BEFORE (dangerous):
_textFont = this.Font;   // captures WinForms-scaled font, may clobber themed font

// AFTER:
// Do nothing — this.Font is allowed to change via WinForms auto-scale.
// _logicalTextStyle is theme-driven; physical resolution happens in DrawContent.
protected override void OnFontChanged(EventArgs e)
{
    base.OnFontChanged(e);
    UpdateMinimumSize();    // recalculate minimum size using new base font metrics
    InvalidateOnce();
}
```

**Acceptance:** Opening VS designer; changing theme; changing DPI — never causes `_textFont` to be set. `private Font _textFont` field does not exist in the class.

---

### Phase 4 — Dirty-Flag Batched `Invalidate()` Pattern

**Problem:** A single property change → immediate `Invalidate()` → immediate `WM_PAINT` within the message loop. Setting 5 properties in a `BeginInit/EndInit` block fires 5 separate paint cycles.

**Solution (same pattern used by WinForms `DataGridView`, DevExpress `BaseStyleControl`):**

**Step 4a — Add `_paintDirty` flag and `InvalidateOnce()` to `BaseControl`**
```csharp
// BaseControl.cs:
private bool _paintDirty = false;

protected void InvalidateOnce()
{
    if (_paintDirty) return;      // already scheduled
    _paintDirty = true;
    BeginInvoke((Action)CommitInvalidate);   // post to message queue
}

private void CommitInvalidate()
{
    _paintDirty = false;
    Invalidate();   // single actual invalidation
}
```

**Step 4b — Replace `Invalidate()` calls in property setters with `InvalidateOnce()`**  
In `BaseControl.Properties.cs`, every property setter that currently ends with `Invalidate()` should call `InvalidateOnce()` instead. Exceptions:
- `Visible` — keep immediate `Invalidate()` (WinForms visibility protocol).
- `Bounds` / `Location` / `Size` — managed by `PerformLayout()`, not `Invalidate()`.

**Step 4c — `ApplyTheme()` should call `InvalidateOnce()`, not `Invalidate()`**  
This ensures that even if `ApplyTheme()` re-runs from several cascading property sets, only one repaint is queued.

**Step 4d — In `OnDpiChangedAfterParent()` — defer redraw**  
```csharp
// BEFORE:
Invalidate(true);  // immediate deep redraw on DPI change

// AFTER:
InvalidateOnce();  // batched; child controls post their own InvalidateOnce too
```

**Acceptance:** Profiling with `WM_PAINT` counter: changing theme (which sets colours, style, font style, painter kind) should produce **1** `WM_PAINT` per control, not 5+.

---

### Phase 5 — Font Cache Keyed on Logical (Not DPI-inflated) Values

**Problem:** `BeepFontManager` currently allows multiple entries for the same logical font at different inflated sizes, because the cache key includes the already-scaled size. When `_dpiScaleFactor` changes, the cache is cleared (`InvalidateFontCache()`) and all fonts are recreated — triggering GDI churn for 50 controls simultaneously.

**Step 5a — Cache by logical descriptor only**
```csharp
// Key: (family, logicalSizePt, fontStyle)  — DPI NOT in key
// The Font stored is created at 96 DPI (LogicalPt, GraphicsUnit.Point)
// Painters that need device-specific size call GetPhysicalFont(style, deviceDpi)
// which scales on the fly or maintains a secondary device-keyed sub-cache

private static readonly Dictionary<(string, float, FontStyle), Font> _logicalCache = new();
```

**Step 5b — `GetPhysicalFont(TypographyStyle, float deviceDpi)`**
```csharp
public static Font GetPhysicalFont(TypographyStyle style, float deviceDpi = 96f)
{
    float logicalPt = style.FontSize;
    float physicalPt = logicalPt * (deviceDpi / 96f);
    physicalPt = MathF.Round(physicalPt * 4) / 4f;    // round to 0.25 pt grid
    return GetOrCreate(style.FontFamily, physicalPt, style.FontStyle);
}
```

**Step 5c — DPI change clears only the device-specific sub-cache, not the logical cache**  
The logical cache is permanent (logical fonts don't change on DPI). Only the physical sub-cache (if any) needs eviction on DPI change.

**Step 5d — Unify `FontListHelper.EnableDpiFontScaling`**  
Remove `BeepThemesManager.EnableDpiFontScaling` (now irrelevant after Phase 2). Keep only `FontListHelper.EnableDpiFontScaling = false` (the loading-time flag) and document it as "controls whether the font-loading subsystem applies DPI to loaded resource fonts". Default remains `false`.

**Acceptance:** DPI change from 96 → 144 DPI: logical cache unchanged; physical font re-lookup is instantaneous (recalculates size, hits the `GetOrCreate` cache with rounded physical key). Zero GDI font allocations per paint cycle after warm-up.

---

### Phase 6 — Fix `BeepThemesManager.OnDpiScaleFactorChanged()` Cascade

**Problem:** When DPI changes, this method fires `ThemeChanged` which causes every subscribed control to run `ApplyTheme()`. This is the same cost as an application-wide theme switch — every control re-reads all colours, re-creates fonts, calls `Invalidate()`. For a 50-control form: 50 × (colour assignments + font creation + Invalidate).

**Step 6a — Emit `DpiChanged` event instead of `ThemeChanged`**
```csharp
// BeepThemesManager:
public static event EventHandler DpiChanged;

private static void OnDpiScaleFactorChanged()
{
    BeepFontManager.ClearPhysicalFontSubCache();   // Phase 5c
    PaintersFactory.ClearCache();
    DpiChanged?.Invoke(null, EventArgs.Empty);     // NOT ThemeChanged
}
```

**Step 6b — In `BaseControl`, handle `DpiChanged` differently from `ThemeChanged`**
```csharp
// BaseControl.cs:
BeepThemesManager.DpiChanged += OnGlobalDpiChanged;
BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;

private void OnGlobalDpiChanged(object sender, EventArgs e)
{
    // No ApplyTheme — theme colours don't change on DPI
    // Just recalculate layout metrics and re-draw
    UpdateDrawingRect();
    _painter?.UpdateLayout(this);
    PerformLayout();
    InvalidateOnce();
}

private void OnGlobalThemeChanged(object sender, EventArgs e)
{
    ApplyTheme();   // full re-theme only when theme actually changes
}
```

**Acceptance:** Move window to 150% DPI monitor: `ApplyTheme()` is NOT called. Controls redraw once via `OnDpiChangedAfterParent` (WinForms) and once more via `OnGlobalDpiChanged` (debounced). Font/colour values unchanged.

---

### Phase 7 — `this.Font` vs Painting Font — Clear Ownership Rules

**Rule (matches DevExpress / Telerik pattern):**

| Property | Owner | Purpose | Set by |
|---|---|---|---|
| `this.Font` (Control.Font) | WinForms | Text measurement for `GetPreferredSize`, `MeasureString`, accessibility | WinForms auto-scale only. NEVER set from `ApplyTheme()`. |
| `_logicalTextStyle` | Control | Design-time token — family, size (pt), weight | `ApplyTheme()` only. |
| Physical `Font` (local var) | Painter / DrawContent | GDI painting | Created inside `DrawContent(g)` or passed by painter. Dispose after use OR retrieve from cache. |

**Step 7a — Add `[DesignerSerializationVisibility(Never)]` to `Font` override in `BaseControl`**  
Prevent the designer from serializing `this.Font`; fonts come from the theme.

**Step 7b — Set `this.Font` once at construction to a neutral baseline**
```csharp
// BaseControl constructor:
this.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
// Do NOT change this.Font again in ApplyTheme().
```

**Step 7c — `SetFont()` in `BaseControl.Methods.cs` — Restrict scope**  
`SetFont()` currently sets `this.Font = BeepThemesManager.ToFont(…)`. This triggers `OnFontChanged`, which triggers `Invalidate()`, and fires the WinForms auto-scale cascade. Change `SetFont()` to only update `_logicalTextStyle`; remove all `this.Font =` assignments from it.

**Step 7d — Remove `SafeApplyFont()` call from `TextFont` property setter**  
`SafeApplyFont()` sets `this.Font = value` which fires `OnFontChanged` and `Invalidate()` unconditionally. The `TextFont` property is a design-time override; it should set `_logicalTextStyle` and call `InvalidateOnce()`.

**Acceptance:** In VS designer Property Grid: changing `Font` via the grid has no effect on visual (theme drives appearance). Setting `TextFont` via designer stores the override and redraws once.

---

### Phase 8 — `BaseControl.Properties.cs` Setter Audit

Go through every property setter in `BaseControl.Properties.cs` and apply the following rules:

| Current Code | Replace With | Condition |
|---|---|---|
| `Invalidate()` | `InvalidateOnce()` | Always |
| `Refresh()` | `InvalidateOnce()` | Unless explicitly needing synchronous paint |
| `SafeApplyFont(…)` | Update `_logicalTextStyle`; `InvalidateOnce()` | In TextFont setter |
| `UpdateBorderPainter(); Invalidate()` | `UpdateBorderPainter(); InvalidateOnce()` | In BorderPainter setter |
| `_painter = …; Invalidate()` | `_painter = …; InvalidateOnce()` | In PainterKind setter |

**Specific Properties to audit (from research):**

- [ ] `IsTransparentBackground`
- [ ] `BackColor`
- [ ] `BorderPainter`
- [ ] `PainterKind`
- [ ] `TextFont`
- [ ] `Text`
- [ ] `ExcludedPaintRectangles` (both Add and Clear)
- [ ] All colour properties (ForeColor, BorderColor, etc.)

---

### Phase 9 — Apply `BeginUpdate / EndUpdate` for Batch Theming

**Inspiration:** DevExpress `LookAndFeel.BeginUpdate()` / Telerik `SuspendThemeUpdate()`.

**Step 9a — Add `BeginUpdate() / EndUpdate()` to `BaseControl`**
```csharp
private int _updateCount = 0;

public void BeginUpdate()
{
    _updateCount++;
}

public void EndUpdate()
{
    if (--_updateCount == 0)
    {
        ApplyTheme();
        InvalidateOnce();
    }
}
```

**Step 9b — Wrap `ApplyTheme()` with update guard**
```csharp
protected virtual void ApplyTheme()
{
    if (_updateCount > 0) { _themeDirty = true; return; }
    // … actual apply …
    _themeDirty = false;
    InvalidateOnce();
}
```

**Step 9c — Use `BeginUpdate / EndUpdate` in form initialisation**
```csharp
// Form.InitializeComponent():
myDatePicker.BeginUpdate();
myDatePicker.Theme = "Material";
myDatePicker.DateFormatStyle = DateFormatStyle.ShortDate;
myDatePicker.HighlightToday = true;
myDatePicker.EndUpdate();   // single ApplyTheme + single repaint
```

**Acceptance:** Setting 5 properties fires `ApplyTheme()` once and `Invalidate()` once.

---

### Phase 10 — Control-Specific Fixes

#### 10.1 `BeepDatePicker` — Complete Fix Checklist

- [ ] Remove `private Font _textFont` field
- [ ] Add `private TypographyStyle _logicalTextStyle` field
- [ ] Rewrite `ApplyTheme()`: store `_logicalTextStyle`, no Font creation, no `this.Font =`
- [ ] Fix double assignment (lines 907 + 930) — remove both
- [ ] Remove `try { } catch { _textFont = this.Font; }` exception silence
- [ ] Update `OnFontChanged()`: remove `_textFont = this.Font` line
- [ ] Update `DrawContent()` / `DrawText()`: resolve font from `_logicalTextStyle` and `this.CreateGraphics().DpiY`
- [ ] Update `UpdateMinimumSize()`: use `TextRenderer.MeasureText(sampleText, SystemFonts.DefaultFont)` baseline, or resolve once with device DPI from `this.DeviceDpi`
- [ ] Apply `InvalidateOnce()` instead of `Invalidate()` in all property setters

#### 10.2 `BeepDatePickerView` — Same pattern as 10.1

- [ ] Find all `_textFont = BeepThemesManager.ToFont(…)` assignments
- [ ] Replace with logical style field + paint-time resolution

#### 10.3 `BeepDateTimePicker` — Same pattern

- [ ] Audit all partial files for `_textFont` assignments and `ToFont` calls
- [ ] Apply logical style pattern throughout

#### 10.4 `BeepComboBox` — Special case

- [ ] `BaseComboBoxPainter.cs` was recently modified — check for unintended `this.Font` assignment
- [ ] Ensure drop-down list font also derived from `_logicalTextStyle`, not from `this.Font`

#### 10.5 All remaining Beep controls (bulk pass)

Run the following grep across all `.cs` files in the solution:
```
grep -r "BeepThemesManager.ToFont(" --include="*.cs" -l
```
For each found file, apply the logical-style refactor.

---

### Phase 11 — `DpiScalingHelper` Cleanup

**Step 11a — Mark `ScaleFont(Font, float)` as `[Obsolete]`**  
The method is a no-op for `GraphicsUnit.Point` fonts (mathematically verified). No callers should use it after Phase 2. Add `[Obsolete("Font DPI scaling is now applied once in GetPhysicalFont(). Do not scale fonts manually.")]`.

**Step 11b — Mark `TryReplaceFontSafely()` as `[Obsolete]`**  
Now that `ApplyTheme()` no longer sets `this.Font`, there is no need for safe font replacement. Deprecate.

**Step 11c — Keep `GetDpiScaleFactor(Control)` and `RefreshScaleFactors()`**  
These are still needed for layout geometry (padding, border radius, icon sizing). They are valid utilities.

**Step 11d — Rename `GetDpiScaleFactor(Graphics)` to `GetRenderDpiScale(Graphics)`**  
Names should communicate purpose. This one is specifically for use inside `DrawContent(g)` to derive the physical DPI for font resolution.

---

## 5. Implementation Priority Order

```
Priority 1 (Immediate — stops the crunching):
  Phase 1 — Fix GetFontForControl formula (1 line change)
  Phase 2 — Remove DPI multiplication from ToFont() (3 files)
  Phase 3 — Fix BeepDatePicker double assignment (remove lines 907 + 930)

Priority 2 (Stops redundant repaints):
  Phase 4 — Dirty-flag InvalidateOnce() pattern
  Phase 6 — Separate DpiChanged from ThemeChanged
  Phase 8 — Property setter audit

Priority 3 (Architectural correctness):
  Phase 3 full — Logical/physical font separation
  Phase 5 — Logical font cache
  Phase 7 — this.Font ownership rules

Priority 4 (Polish & developer experience):
  Phase 9 — BeginUpdate/EndUpdate
  Phase 10 — Control-specific checklists
  Phase 11 — DpiScalingHelper cleanup
```

---

## 6. Acceptance Criteria — Full Suite

| Test | Expected |
|---|---|
| Form on 100% DPI (96): 9 pt design font | Renders ~12 px tall (normal) |
| Form on 150% DPI (144): 9 pt design font | Renders ~18 px tall (1.5× only) |
| Form on 200% DPI (192): 9 pt design font | Renders ~24 px tall (2× only) |
| Move window 100% → 150% DPI monitor | Controls redraw **once**, no crunching |
| Change theme via `BeepThemesManager.CurrentTheme` | Each control redraws **once** |
| Set 5 properties in a property grid | One repaint cycle total |
| `ApplyTheme()` call count per DPI change | 0 (DPI change does NOT call ApplyTheme) |
| `_textFont` field in `BeepDatePicker` | Does not exist |
| `BeepThemesManager.ToFont()` result at 150% DPI | Returns 9 pt (logical, unchanged) |
| `BeepThemesManager.ToPhysicalFont(style, control)` at 150% DPI | Returns 13.5 pt |
| VS Designer: load form at design-time | No font exceptions, no crunched sizes |
| VS Designer: change Beep control `Theme` property | Single repaint, correct size |

---

## 7. Files Impacted

| File | Phase | Change Type |
|---|---|---|
| `FontManagement/BeepFontManager.cs` | 1, 5 | Fix formula; add physical font method |
| `ThemeManagement/BeepThemesManager.cs` | 2, 6 | Remove DPI multiply; add DpiChanged event |
| `FontManagement/FontListHelper.cs` | 5d | Remove duplicate EnableDpiFontScaling |
| `Base/BaseControl.cs` | 4, 6, 7 | InvalidateOnce; DpiChanged handler; BeginUpdate |
| `Base/BaseControl.Properties.cs` | 4, 7, 8 | Replace Invalidate() with InvalidateOnce() |
| `Base/BaseControl.Methods.cs` | 7 | SetFont/SafeApplyFont refactor |
| `Helpers/DpiScalingHelper.cs` | 11 | Deprecate ScaleFont/TryReplaceFontSafely |
| `Dates/BeepDatePicker.cs` | 3, 10.1 | Remove _textFont; logical style; fix double assign |
| `Dates/BeepDatePickerView.cs` | 10.2 | Same as BeepDatePicker |
| `Dates/BeepDateTimePicker.*.cs` | 10.3 | Audit all partials |
| `ComboBoxes/Painters/BaseComboBoxPainter.cs` | 10.4 | Audit for ToFont calls |
| All controls with `BeepThemesManager.ToFont(` calls | 10.5 | Logical style refactor (bulk) |

---

## 8. Non-Goals (DPI Fix Scope Only)

- Changing the `TypographyStyle` data model schema (fonts are already stored as logical pt values — correct).
- Supporting non-Point `GraphicsUnit` fonts in the theme system.

---

## 9. Risks & Mitigations (DPI Fix)

| Risk | Mitigation |
|---|---|
| Removing `_textFont` breaks a control that relies on the field directly | Grep for `_textFont` in all files before removing; address each one |
| `ToFont()` signature change (removing `applyDpiScaling` param) breaks callers | Add `[Obsolete]` overload that delegates to new signature; remove in +1 release |
| `InvalidateOnce()` `BeginInvoke` causes subtle timing issues in synchronous test code | Provide `FlushPendingInvalidate()` test helper that calls `CommitInvalidate()` synchronously |
| Custom painters that cache `Font` objects from `ToFont()` at scale factor 1.5 | After Phase 2, all such caches hold logical fonts — they must call `GetPhysicalFont` to get device fonts; update painter interfaces |
| `BeginUpdate / EndUpdate` misuse (missing paired call) | Implement `IDisposable` scope: `using (control.Update()) { … }` pattern |

---

---

# PART 2 — UI/UX Enhancement Plan

**Based on:**
- Figma Design System Standards 2024–2025 (Auto Layout, Design Tokens W3C Spec, Variants, Interactive Components)
- Material Design 3 (Material You — dynamic colour, type scale, elevation, motion)
- DevExpress WinForms v24 (look-and-feel skins, state machine animation, per-pixel painting, accessibility)
- Telerik RadControls WinForms (themes, transitions, element hierarchy, animation framework)
- Infragistics Ignite UI WinForms (UltraGrid, UltraEditor patterns)
- Fluent UI (Windows 11 design language — rounded corners, Mica, acrylic, depth layers)
- Ant Design 5 (component variants, token-based spacing, state colours)

---

## UX-1. Design Token System — Figma W3C Standard

### Problem
The current `IBeepTheme` stores raw `Color`, `Font`, and size values. There is no **token layer** — every visual decision is a magic value scattered across themes. Changing "the brand colour" requires editing dozens of theme properties. DevExpress uses a **Skin + Paint Style** hierarchy; Telerik has a **ThemeResources.xml** token dictionary.

### Target Architecture (W3C Design Tokens Draft 2 + Figma naming)

```
Token Tier 1: Global tokens (raw values)
  color.blue.500  = #1976D2
  color.red.600   = #E53935
  space.4         = 4
  space.8         = 8
  radius.sm       = 4
  radius.md       = 8
  font.size.body  = 14
  font.weight.regular = 400

Token Tier 2: Semantic tokens (role-based aliases — reference global tokens)
  token.color.surface           → color.neutral.100
  token.color.on-surface        → color.neutral.900
  token.color.primary           → color.blue.500
  token.color.on-primary        → color.white
  token.color.error             → color.red.600
  token.color.outline           → color.neutral.400
  token.color.outline-variant   → color.neutral.200
  token.elevation.level1        → shadow(0,1,2,black,15%)
  token.elevation.level2        → shadow(0,2,6,black,20%)
  token.elevation.level3        → shadow(0,6,12,black,25%)

Token Tier 3: Component tokens (component-specific aliases — reference semantic tokens)
  button.color.background            → token.color.primary
  button.color.label                 → token.color.on-primary
  button.color.background.hover      → token.color.primary-container
  button.color.border.focused        → token.color.primary
  input.color.underline              → token.color.outline
  input.color.underline.focused      → token.color.primary
  input.color.background.error       → token.color.error-container
  datepicker.color.selected-day      → token.color.primary
  datepicker.color.today-ring        → token.color.primary
```

### Implementation Steps

**Step UX-1a — Add `BeepDesignTokens` static class**
```csharp
public static class BeepDesignTokens
{
    // Tier 1 raw palette (brand-agnostic)
    public static Color Palette_Blue500    = ColorTranslator.FromHtml("#1976D2");
    public static Color Palette_Blue700    = ColorTranslator.FromHtml("#1565C0");
    // … full Material 3 tonal palette

    // Tier 2 semantic aliases (updated when theme switches)
    public static Color Surface            = SystemColors.Control;
    public static Color OnSurface          = SystemColors.ControlText;
    public static Color Primary            = Color.FromArgb(25, 118, 210);
    public static Color OnPrimary          = Color.White;
    public static Color PrimaryContainer   = Color.FromArgb(187, 222, 251);
    public static Color OnPrimaryContainer = Color.FromArgb(13, 71, 161);
    public static Color Secondary          = Color.FromArgb(38, 166, 154);
    public static Color Error              = Color.FromArgb(229, 57, 53);
    public static Color ErrorContainer     = Color.FromArgb(255, 235, 238);
    public static Color Outline            = Color.FromArgb(189, 189, 189);
    public static Color OutlineVariant     = Color.FromArgb(238, 238, 238);
    public static Color Scrim              = Color.FromArgb(0, 0, 0);

    // Elevation shadow descriptors
    public static ElevationLevel[] ElevationLevels = { /* 0–5 */ };

    // Spacing grid (pt)
    public static readonly int[] SpaceGrid = { 0, 2, 4, 8, 12, 16, 20, 24, 32, 40, 48, 64 };

    // Update all semantic tokens from an IBeepTheme
    public static void Apply(IBeepTheme theme) { … }
}
```

**Step UX-1b — Add token export/import to `BeepThemesManager`**
- Load from `theme.tokens.json` (Figma token format via Style Dictionary)
- Support live hot-reload (file watcher, fires `ThemeChanged`)

**Step UX-1c — Update `IBeepTheme` with semantic token references**
- Replace `Color CalendarBackColor` → `string Token_CalendarBackground = "token.color.surface-container"`
- At load time, resolve token → actual `Color` via `BeepDesignTokens`
- This means a single brand-colour change propagates through all component tokens automatically

---

## UX-2. Typography Scale (Material Design 3 / Figma)

### Problem
Current themes have a few `TypographyStyle` entries (DateFont, ButtonFont, etc.) without a systematic scale. Sizes are arbitrary per control.

### Target: Material Design 3 Type Scale (15 roles)

| Token Role | Size (pt) | Weight | Line Height | Usage |
|---|---|---|---|---|
| `Display.Large` | 57 | 400 | 1.12 | Hero headings |
| `Display.Medium` | 45 | 400 | 1.16 | Section headings |
| `Display.Small` | 36 | 400 | 1.22 | Card titles |
| `Headline.Large` | 32 | 400 | 1.25 | Dialog titles |
| `Headline.Medium` | 28 | 400 | 1.29 | Panel headers |
| `Headline.Small` | 24 | 400 | 1.33 | Group headers |
| `Title.Large` | 22 | 400 | 1.27 | List headers |
| `Title.Medium` | 16 | 500 | 1.50 | Section labels |
| `Title.Small` | 14 | 500 | 1.43 | Column headers |
| `Body.Large` | 16 | 400 | 1.50 | Primary body text |
| `Body.Medium` | 14 | 400 | 1.43 | **Default — most controls** |
| `Body.Small` | 12 | 400 | 1.33 | Helper text, captions |
| `Label.Large` | 14 | 500 | 1.43 | Button labels |
| `Label.Medium` | 12 | 500 | 1.33 | Chip labels, tab labels |
| `Label.Small` | 11 | 500 | 1.45 | Badge, tooltip |

**Step UX-2a — Add `TypeScale` enum**
```csharp
public enum TypeScale
{
    DisplayLarge, DisplayMedium, DisplaySmall,
    HeadlineLarge, HeadlineMedium, HeadlineSmall,
    TitleLarge, TitleMedium, TitleSmall,
    BodyLarge, BodyMedium, BodySmall,     // BodyMedium = current default
    LabelLarge, LabelMedium, LabelSmall
}
```

**Step UX-2b — Add `TypeScaleDescriptor` lookup table in `BeepFontManager`**
```csharp
public static Font GetTypeScaleFont(TypeScale role)
    => _typeScaleTable[role]; // pre-warmed at Initialize()
```

**Step UX-2c — Map Beep control roles to type scale slots**
```
BeepButton label        → Label.Large  (14pt, 500)
BeepTextBox input       → Body.Medium  (14pt, 400)
BeepLabel               → Body.Medium  (14pt, 400)
BeepLabel with IsHeader → Title.Medium (16pt, 500)
BeepComboBox            → Body.Medium
BeepDatePicker          → Body.Medium
BeepGrid header         → Label.Large
BeepGrid cell           → Body.Small
BeepTooltip             → Label.Small
Badge text              → Label.Small
Helper text             → Body.Small
Error text              → Body.Small  (colour changes, not size)
```

---

## UX-3. Spacing & 8pt Grid System

### Problem
Controls use hardcoded padding values (`_padding => 3`, `_buttonWidth => 24`). Different controls use different arbitrary padding, breaking visual rhythm.

### Target (Figma Auto Layout / Material 3 Spacing)

**8pt grid:** All spacing values are multiples of 4pt. Preferred: 4, 8, 12, 16, 20, 24, 32.

```csharp
public static class BeepSpacing
{
    public const int Xs   = 4;   // icon gap, tight padding
    public const int Sm   = 8;   // inner padding (button, input)
    public const int Md   = 12;  // standard padding
    public const int Lg   = 16;  // section spacing
    public const int Xl   = 24;  // card padding
    public const int Xxl  = 32;  // dialog padding
    public const int Xxxl = 48;  // section gap

    // Control-specific standards
    public const int ButtonPaddingH  = 24; // horizontal 24pt (Material filled button)
    public const int ButtonPaddingV  = 10; // vertical   10pt
    public const int InputPaddingH   = 12;
    public const int InputPaddingV   = 8;
    public const int InputMinHeight  = 36; // matches ComboBox, DatePicker, TextBox
    public const int ButtonMinHeight = 36;
    public const int ChipHeight      = 32;
    public const int BadgeMinWidth   = 16;
}
```

**Step UX-3a** — Replace every hardcoded magic padding constant in all controls with `BeepSpacing.*` references.  
**Step UX-3b** — Add `SpacingScale` property to `IBeepTheme` (Compact=0.75×, Normal=1×, Comfortable=1.25×, Spacious=1.5×) — same as DevExpress "compact/normal/touch" density modes.

---

## UX-4. Control State Machine (DevExpress / Material 3 Pattern)

### Problem
Current controls track `IsHovered`, `IsPressed`, `IsSelected`, `IsDisabled` as separate boolean fields read independently during paint. There is no formal **state machine** — state transitions fire multiple Invalidate() calls.

### Target: 7-state finite state machine (matches DevExpress `ObjectState`, Material 3 state layer)

```csharp
[Flags]
public enum ControlVisualState
{
    Default   = 0,
    Hovered   = 1,
    Pressed   = 2,
    Focused   = 4,
    Selected  = 8,
    Disabled  = 16,
    Error     = 32
}
```

**State Layer Opacity (Material 3 standard):**

| State | Overlay opacity | Colour |
|---|---|---|
| Hovered | 8% | Primary |
| Focused | 12% | Primary |
| Pressed | 12% | Primary (ripple at click point) |
| Dragged | 16% | Primary |
| Selected | 12% | Primary |
| Error | 12% | Error |
| Disabled | 38% opacity on entire control | — |

**Step UX-4a — Replace boolean state fields in `BaseControl` with `_visualState : ControlVisualState`**

**Step UX-4b — Add `GetStateLayerColor(ControlVisualState, Color baseColor) : Color` to `BeepThemesManager`**
```csharp
public static Color GetStateLayerColor(ControlVisualState state, Color onColor)
{
    float alpha = state switch {
        _ when state.HasFlag(ControlVisualState.Pressed)  => 0.12f,
        _ when state.HasFlag(ControlVisualState.Focused)  => 0.12f,
        _ when state.HasFlag(ControlVisualState.Hovered)  => 0.08f,
        _ when state.HasFlag(ControlVisualState.Selected) => 0.12f,
        _ => 0f
    };
    return Color.FromArgb((int)(alpha * 255), onColor);
}
```

**Step UX-4c — Painters draw base colour + state layer over it**  
This matches Material 3's "state layer" pattern: one background colour + one overlay = all visual states from a single base token.

**Step UX-4d — State transitions do NOT call `Invalidate` individually**  
State change → `_visualState = newState; InvalidateOnce();`

---

## UX-5. Motion & Animation System (Telerik Animation Framework Pattern)

### Problem
State changes (hover, focus, press) are instant — no transition. Commercial products (DevExpress, Telerik, Syncfusion) animate state layer opacity, control height (accordion), and thumb position (toggle). The Beep splash animation already exists but is isolated per-control.

### Target: Central `BeepAnimationEngine` with easing library

**Step UX-5a — Add `BeepAnimationEngine` (lightweight, shared)**
```csharp
public static class BeepAnimationEngine
{
    // Queue a value interpolation; calls Action<float> on each frame
    public static IAnimationHandle Animate(
        float from, float to, int durationMs,
        EasingFunction easing, Action<float> onUpdate, Action onComplete = null);

    // Predefined durations (Material 3 motion tokens)
    public const int Duration_Short1  = 50;   // micro interactions
    public const int Duration_Short2  = 100;  // state layer appear
    public const int Duration_Short3  = 150;  // state layer disappear
    public const int Duration_Short4  = 200;  // simple expand
    public const int Duration_Medium1 = 250;  // standard transitions
    public const int Duration_Medium2 = 300;  // page-level transitions
    public const int Duration_Long1   = 350;
    public const int Duration_Long2   = 400;
}
```

**Step UX-5b — Standard easing curves (Material 3 motion)**
```
Emphasized       = cubic-bezier(0.2, 0, 0, 1)     — enter/expand
EmphasizedDecel  = cubic-bezier(0.05, 0.7, 0.1, 1) — enter screen
EmphasizedAccel  = cubic-bezier(0.3, 0, 0.8, 0.15) — exit screen
Standard         = cubic-bezier(0.2, 0, 0, 1)      — default
StandardDecel    = cubic-bezier(0, 0, 0, 1)        — enter panels
StandardAccel    = cubic-bezier(0.3, 0, 1, 1)      — exit panels
Linear           = t
```

**Step UX-5c — Animate state layer opacity in BaseControl**
- On `IsHovered = true`: animate `_stateLayerAlpha` 0 → 20 in 100ms (Standard easing)
- On `IsHovered = false`: animate `_stateLayerAlpha` 20 → 0 in 150ms
- On `IsPressed = true`: show ripple + animate 0 → 30 in 50ms
- Each frame calls `InvalidateOnce()` (already batched — no risk of over-painting)

**Step UX-5d — Reduce motion option**
```csharp
public static bool PrefersReducedMotion { get; set; }  // respect Windows accessibility flag
// If true: all durations = 0, no easing
```

---

## UX-6. Elevation & Shadow System (Material 3 / Fluent UI Depth)

### Problem
Beep controls use flat borders or a single `ShadowColor` property. Commercial UIs use a **tonal elevation** system where surfaces lifted above the background gain a coloured tint + drop shadow.

### Target: 6 elevation levels

| Level | Use Case | Shadow | Surface Tint |
|---|---|---|---|
| 0 | Flat content, text fields | None | 0% |
| 1 | Cards, raised controls | `0 1px 2px rgba(0,0,0,0.3)` | 5% primary |
| 2 | Chips, filled buttons | `0 1px 2px + 0 2px 6px` | 8% primary |
| 3 | Dropdowns, popups | `0 1px 3px + 0 4px 8px` | 11% primary |
| 4 | Dialog overlays | `0 2px 3px + 0 6px 10px` | 12% primary |
| 5 | Modals, app bars | `0 4px 4px + 0 8px 12px` | 14% primary |

**Step UX-6a — Add `ElevationLevel` property to `BaseControl` (default = 0)**

**Step UX-6b — Add `ElevationPainter` utility**
```csharp
public static void PaintElevation(Graphics g, Rectangle bounds, int level, Color primaryColor, bool isDarkMode)
```

**Step UX-6c — Surface tint in dark mode**  
In dark mode (Fluent UI and Material 3), elevated surfaces get a primary colour tint instead of a shadow so they maintain visual separation without harsh contrast:
- `surface + (5% primary at level 1)` through to `14% primary at level 5`

**Step UX-6d — Apply elevation to popup controls**
- `BeepPopupForm` → Level 3
- `BeepDialog` / modal → Level 4–5
- `BeepDropDown` list → Level 3
- Standard card/panel → Level 1
- Un-elevated flat controls → Level 0

---

## UX-7. Focus Ring System (WCAG 2.1 AA / Fluent UI)

### Problem
Focus is indicated inconsistently across Beep controls. Some use `BorderColor` change, most show nothing. WCAG 2.1 AA requires a **minimum 3:1 contrast ratio** between focused and unfocused state, with at least 2px visible indicator.

### Target: Fluent UI / Windows 11 focus ring

**Specification (Windows 11 design):**
- Outer ring: 2px, `token.color.primary`, drawn **outside** the control bounds (no layout impact)
- Inner ring: 1px offset gap in `token.color.surface` (creates halo effect)
- Corner radius matches the control's own border radius
- Focus ring NOT visible when using mouse (only keyboard focus) — matches Windows 11 `ShowFocusCues`

**Step UX-7a — Add `FocusRingPainter` utility**
```csharp
public static void PaintFocusRing(Graphics g, Rectangle bounds, int borderRadius,
    Color ringColor, Color gapColor, int ringThickness = 2, int gapSize = 2)
```

**Step UX-7b — `BaseControl.OnPaint` — draw focus ring last (above everything)**
```csharp
if (Focused && ShouldShowFocusRing())
    FocusRingPainter.Paint(g, ClientRectangle, BorderRadius, …);
```

**Step UX-7c — `ShouldShowFocusRing()` — keyboard-only visibility**
```csharp
private bool ShouldShowFocusRing()
    => Focused && !_mouseIsDown; // show only for keyboard navigation
```

---

## UX-8. Colour Mode — Light / Dark / System (Material 3 / DevExpress)

### Problem
Beep themes are static `IBeepTheme` instances. To switch dark/light mode the user must change the whole theme. DevExpress and Telerik auto-switch based on the Windows `AppsUseLightTheme` registry key.

### Target: Dual-tone theme architecture

**Step UX-8a — Add `IBeepTheme.DarkVariant : IBeepTheme` property**  
Each theme ships with a baked-in dark variant. `BeepThemesManager` auto-selects based on `ColorMode`.

**Step UX-8b — Add `ColorMode` enum and global property**
```csharp
public enum ColorMode { Light, Dark, System }
public static ColorMode ActiveColorMode { get; set; } = ColorMode.System;
```

**Step UX-8c — React to Windows `AppsUseLightTheme`**
```csharp
// Listen to registry change via SystemEvents.UserPreferenceChanged
SystemEvents.UserPreferenceChanged += (s, e) => {
    if (e.Category == UserPreferenceCategory.General)
        BeepThemesManager.RefreshSystemColorMode();
};
```

**Step UX-8d — Material 3 Dynamic Color generation**  
Given a brand `seed color`, generate a complete tonal palette (40 colours) using Material 3's HCT colour space algorithm. This lets any app get a correct full dark+light palette from one brand hex code.

---

## UX-9. Border & Corner Radius System (Figma / Fluent / Material)

### Problem
Border radius tokens are not consistent. Most controls hard-code radius values. Moving from "sharp" to "rounded" style requires editing every control.

### Target: Semantic radius tokens (matches DevExpress "shape" skin property)

| Token | Value | Usage |
|---|---|---|
| `radius.none`   | 0  | Legacy, flat style |
| `radius.xs`     | 2  | Small chips, badges |
| `radius.sm`     | 4  | Inputs, textboxes |
| `radius.md`     | 8  | Buttons, cards |
| `radius.lg`     | 12 | Dialogs, large panels |
| `radius.xl`     | 16 | Bottom sheets |
| `radius.full`   | 9999 | Pills, circular elements |

**Step UX-9a — Add `CornerRadius` token enum and lookup table in `BeepSpacing`**

**Step UX-9b — Add `ShapeFamily` to `IBeepTheme`**
```csharp
public enum ShapeFamily { None, Rounded, Extrapolated, Full }
// None = all 0; Rounded = standard table above; Extrapolated = 2× values; Full = full pills
```
Switching `ShapeFamily` updates all corner radius tokens without touching individual controls — same as Material 3's **Shape system** and DevExpress **Shape Skin** concept.

---

## UX-10. Control-Specific UX Enhancements

### UX-10.1 BeepButton

**Current gaps vs DevExpress / Material:**

| Feature | Current | Target |
|---|---|---|
| Variants | Single style | Filled, Outlined, Text/Ghost, Elevated, Tonal (Material 3 5 variants) |
| State layer | None | Animated overlay (UX-4 + UX-5) |
| Leading icon | Supported | Add trailing icon position |
| Loading state | Spinner replaces entire button | Spinner replaces label only; button stays accessible |
| Ripple origin | Center | Click point (Material ink ripple) |
| Size variants | Fixed | Small (28), Medium (36), Large (44) — matches DevExpress button size |
| Toggle button | Not available | Add `IsToggle` + `IsChecked` properties |
| Split button | Not available | Add `SplitButton` mode (main action + dropdown arrow — DevExpress standard) |
| Icon badge | Badge on top | Already exists — wire to semantic `BadgeColor` token |
| Keyboard shortcut display | None | Show underline mnemonic OR right-aligned shortcut hint (`Ctrl+S`) |

### UX-10.2 BeepTextBox / BeepInput

**Current gaps:**

| Feature | Current | Target |
|---|---|---|
| Floating label | Supported | Animate label float (UX-5): 150ms, 12pt→10pt, colour change |
| Character counter | None | `MaxLength` triggers counter badge `12 / 100` |
| Password strength meter | None | Inline coloured bar below input (Weak/Fair/Strong/Very Strong) |
| Prefix / suffix icons | Limited | Standardize leading/trailing icon slots with click events |
| Clear button | None | Auto-show × when text is present and control focused |
| Paste-as-link | None | Show rich paste button when clipboard contains URL |
| Input mask | Partial | Expand to phone, IBAN, credit card, IP address formats |
| Auto-complete dropdown | None | Inline dropdown with `SuggestItems` list — same as DevExpress `AutoComplete` |
| Validation state colours | Basic | Full `Default / Valid / Warning / Error / Success` colour tokens |

### UX-10.3 BeepDatePicker

**Current gaps:**

| Feature | Current | Target |
|---|---|---|
| Calendar header navigation | Basic | Month/Year picker with slide animation |
| Date range selection | Basic | Visual range highlight with gradient fill between start/end dates |
| Preset shortcuts | None | `Today`, `Yesterday`, `Last 7 Days`, `This Month`, `Custom` chip row at top |
| Week numbers | None | Optional column showing ISO week numbers (DevExpress feature) |
| Disabled date display | Grey skip | Strikethrough + tooltip explaining why disabled |
| Keyboard navigation | Partial | Arrow keys move focus highlight; Enter selects; Escape closes |
| Long-press on nav arrow | None | Hold → fast-forward month (DevExpress touch-friendly) |
| Time picker integration | Separate | Inline time spinner below calendar (hours:minutes scroll wheels) |
| Multi-language calendar | Culture only | Support RTL Hebrew/Arabic calendar grids |
| Today highlight ring | Outline | Use `radius.full` + `token.color.primary` ring 2px |
| Selected date | Filled | Use `token.color.primary` background + `token.color.on-primary` text |
| Hover day | None | State layer 8% primary overlay (UX-4) |

### UX-10.4 BeepComboBox / BeepDropDown

**Current gaps:**

| Feature | Current | Target |
|---|---|---|
| Search in dropdown | None | Inline text filter field at top of dropdown list (Ant Design Select) |
| Multi-select chips | None | Show selected items as removable chip tokens in the input area |
| Virtual scrolling | None | For 1000+ items — render only visible rows (DevExpress VirtualMode) |
| Group headers | None | Categorized list with sticky section headers |
| Item checkboxes | None | Multi-select with checkboxes per item |
| Custom item template | Fixed | `ItemTemplate` property with image + title + subtitle layout |
| Empty state | None | "No results" illustration + message when filter returns nothing |
| Dropdown animation | Instant | Slide-down + fade in 150ms (Standard easing) |
| Max visible items | Hardcoded | `MaxDropDownItems` property |
| `Creatable` mode | None | Allow user to type new value and create it (Select `creatable` — Ant Design) |

### UX-10.5 BeepGrid / BeepGridPro

**Current gaps:**

| Feature | Current | Target |
|---|---|---|
| Column resizing | Limited | Per-pixel drag with visual guide line |
| Column reordering | None | Drag-drop column headers with ghost preview |
| Frozen columns | None | First N columns sticky during horizontal scroll (DevExpress FixedColumn) |
| Row hover highlight | Basic | Animated row highlight using state layer (UX-4) |
| Alternating row colours | None | Even/odd row tint using `token.color.surface-variant` |
| Cell editing | None | Inline editor per cell type (text, date, checkbox, combobox) |
| Row expand / detail row | None | Expandable detail section below row (DevExpress Master-Detail) |
| Column sort indicators | None | Sort arrow + tonal background on sorted column header |
| Multi-select rows | None | Checkbox column + keyboard `Shift+Click` range select |
| Export toolbar | None | Export to CSV/Excel/PDF toolbar buttons (DevExpress standard) |
| Loading skeleton | None | Shimmer placeholder rows while data loads |
| Empty state | None | Icon + message when grid has zero rows |
| Density modes | Fixed | Compact / Normal / Comfortable row heights (UX-3 spacing scale) |

### UX-10.6 BeepPanel / BeepCard

**Current gaps:**

| Feature | Current | Target |
|---|---|---|
| Elevation shadow | None | Apply UX-6 elevation levels |
| Card variants | Single | Elevated, Filled, Outlined (Material 3 card variants) |
| Collapsible header | None | Animate collapse with UX-5 |
| Drag handle | None | Show grip icon when `IsDraggable = true` |
| Overflow menu | None | ⋮ icon → popup actions menu top-right |
| Divider between header and content | Hardcoded border | Token-driven divider using `token.color.outline-variant` |

### UX-10.7 BeepLabel / BeepText

**Current gaps:**

| Feature | Current | Target |
|---|---|---|
| Hyperlink mode | None | Underline + colour change + cursor + `LinkClicked` event |
| Truncation with tooltip | None | Auto-show full text tooltip when truncated with ellipsis |
| Copyable text | None | `IsCopyable` → show copy icon on hover + clipboard on click |
| Selection | None | `IsSelectable` → mouse text selection (like `SelectableLabel` in DevExpress) |

### UX-10.8 BeepTooltip

**Current gaps:**

| Feature | Current | Target |
|---|---|---|
| Rich content | Text only | Support icon + title + body + action button (Infragistics tooltip) |
| Delay control | Fixed | `ShowDelay`, `HideDelay` properties |
| Position awareness | Fixed | Auto-flip when near screen edge (Telerik target origin/alignment) |
| Elevation | None | Apply UX-6 Level 3 shadow |
| Arrow pointer | None | Small triangle pointing to target control |
| Max width wrapping | Fixed | `MaxTooltipWidth` with auto-wrap |

### UX-10.9 BeepNavBar / BeepSideMenu

**Current gaps:**

| Feature | Current | Target |
|---|---|---|
| Rail mode (collapsed) | Limited | Icon-only rail + expanded panel on hover/click (Material 3 Navigation Rail) |
| Badge on nav items | Supported | Animate badge appear using UX-5 |
| Active indicator | Colour only | Pill indicator behind active item (Material 3 Navigation Drawer active state) |
| Tooltip on rail items | None | Show item label as tooltip when in collapsed rail mode |
| Keyboard navigation | Partial | Arrow keys cycle items |
| Flyout submenu | Partial | Animated slide-in at 250ms |

---

## UX-11. Accessibility Enhancements (WCAG 2.1 AA / Section 508)

| Enhancement | Standard | Implementation |
|---|---|---|
| Focus ring visibility | WCAG 2.4.11 (AA) | UX-7 focus ring system |
| Colour contrast ratio ≥ 4.5:1 (text) | WCAG 1.4.3 | `BeepDesignTokens.ValidateContrast()` utility |
| Colour contrast ratio ≥ 3:1 (UI components) | WCAG 1.4.11 | Token validator at theme load |
| Do not use colour as the only error indicator | WCAG 1.4.1 | Error state = colour + icon + `ErrorText` |
| Keyboard-navigable all interactive controls | WCAG 2.1.1 | Tab order audit + `TabStop = true` on all interactive elements |
| Screen reader support (`AccessibleName`, `AccessibleDescription`) | Section 508 | Auto-generate from `Text`, `LabelText`, `HelperText` |
| Reduce motion support | WCAG 2.3.3 | `BeepAnimationEngine.PrefersReducedMotion` read from system |
| High-contrast mode support | Windows SYSPARAMS | `SystemInformation.HighContrast` → disable custom painting, use system colours |
| Minimum touch target 44×44 pt | WCAG 2.5.5 (AA) | `MinimumTouchTargetSize = 44` on all interactive controls with auto-padding |

---

## UX-12. Scrollbar Theming (Fluent / Material Pattern)

### Problem
WinForms scrollbars are native Win32 — they ignore the custom theme completely. DevExpress replaces them with fully-painted custom scrollbars.

### Target

**Step UX-12a — Add `BeepScrollBar` custom control** (replaces native scrollbar)
- Thumb: `token.color.on-surface-variant` (light), `token.color.surface-variant` (dark)
- Track: transparent, reveals on hover
- Thumb hover → brighten 8%
- Thumb width: 6px (collapsed), 10px (hovered) with 200ms animation
- Matches Windows 11 / Fluent UI scrollbar
- Auto-hide when not scrolling (fade out after 1200ms idle)
- Keyboard: Page Up/Down, Home/End

**Step UX-12b — `ScrollableBeepControl` base class**
- Contains `BeepScrollBar` H + V, and a viewport panel
- Drop-in replacement for any control that currently uses `AutoScroll = true`

---

## UX-13. Notification & Feedback Patterns

| Pattern | Description | Reference |
|---|---|---|
| **Toast / Snackbar** | 4s auto-dismiss bottom notification. Single action button optional. | Material 3 Snackbar, Ant Design Message |
| **Inline validation** | Real-time validation feedback as user types — not only on blur. | Ant Design Form validation |
| **Progress overlay** | Semi-transparent overlay with spinner on a specific container (not whole form). | DevExpress WaitIndicator on panel |
| **Skeleton screens** | Animated shimmer placeholders for async data (cards, grid rows, text lines). | Ant Design Skeleton |
| **Confirmation dialog** | Standardised confirmation dialog API: `BeepDialog.Confirm(title, body, confirmLabel, cancelLabel)`. | Telerik RadMessageBox |
| **Undo toast** | After delete, show `Item deleted. [Undo]` snackbar (Google Material pattern). | |
| **Step progress** | Multi-step wizard progress indicator: linear steps with status states (active / complete / error). | Ant Design Steps |

---

## UX-14. Commercial Parity Checklist (DevExpress v24 / Telerik / Infragistics)

| Feature | DevExpress | Telerik | Beep Current | Beep Target |
|---|---|---|---|---|
| Theme hot-swap at runtime | ✅ | ✅ | ✅ | ✅ Already done |
| Design token system | ✅ (Skin XML) | ✅ (ThemeResources) | ❌ | UX-1 |
| 15-role type scale | ✅ | ✅ | ❌ | UX-2 |
| 8pt spacing grid | ✅ | ✅ | ❌ | UX-3 |
| Animated state layers | ✅ | ✅ | Partial (splash) | UX-4 + UX-5 |
| Elevation / shadow | ✅ | ✅ | ❌ | UX-6 |
| Focus ring (keyboard-only) | ✅ | ✅ | ❌ | UX-7 |
| Dark / Light / System mode | ✅ | ✅ | Partial | UX-8 |
| Shape (corner radius) system | ✅ | ✅ | Partial | UX-9 |
| Button 5 variants | ✅ | ✅ | ❌ | UX-10.1 |
| Input floating label (animated) | ✅ | ✅ | Static | UX-10.2 |
| Inline search in ComboBox | ✅ | ✅ | ❌ | UX-10.4 |
| Multi-select chips | ✅ | ✅ | ❌ | UX-10.4 |
| Grid column reorder | ✅ | ✅ | ❌ | UX-10.5 |
| Grid frozen columns | ✅ | ✅ | ❌ | UX-10.5 |
| Custom scrollbars | ✅ | ✅ | ❌ | UX-12 |
| Skeleton screens | ✅ | Partial | ❌ | UX-13 |
| Toast / Snackbar | ✅ | ✅ | ❌ | UX-13 |
| WCAG 2.1 AA | ✅ | ✅ | Partial | UX-11 |
| Reduce motion | ✅ | ✅ | ❌ | UX-5d |
| High-contrast mode | ✅ | ✅ | ❌ | UX-11 |

---

## UX-15. Implementation Roadmap

```
Sprint 1 (Foundation — 2 weeks)
  UX-1   Design token data model + BeepDesignTokens class
  UX-2   TypeScale enum + BeepFontManager type scale table
  UX-3   BeepSpacing constants — replace hardcoded values

Sprint 2 (State & Motion — 2 weeks)
  UX-4   ControlVisualState enum + BaseControl state machine
  UX-5a/b BeepAnimationEngine + easing library
  UX-5c  Animate state layer in BaseControl

Sprint 3 (Visual System — 2 weeks)
  UX-6   ElevationPainter
  UX-7   FocusRingPainter + keyboard-only display
  UX-9   ShapeFamily + CornerRadius token table

Sprint 4 (Colour & Theme — 2 weeks)
  UX-8   Dark/Light/System color mode + DarkVariant theme property
  UX-1b/c Token import from Figma JSON

Sprint 5 (Controls — 4 weeks)
  UX-10.1 BeepButton 5 variants + animated state + split mode
  UX-10.2 BeepTextBox floating label animation + clear button
  UX-10.3 BeepDatePicker preset shortcuts + animated header + week numbers
  UX-10.4 BeepComboBox inline search + multi-select chips

Sprint 6 (Controls continued — 3 weeks)
  UX-10.5 BeepGrid column reorder + frozen columns + alternating rows
  UX-10.6 BeepCard elevation + variants
  UX-10.8 BeepTooltip rich content + arrow
  UX-10.9 BeepNavBar rail mode

Sprint 7 (Accessibility & Notifications — 2 weeks)
  UX-11  Accessibility audit + screen reader names
  UX-12  BeepScrollBar custom + ScrollableBeepControl
  UX-13  Toast / Snackbar + Skeleton + Confirmation dialog
```

---

*End of plan.*
