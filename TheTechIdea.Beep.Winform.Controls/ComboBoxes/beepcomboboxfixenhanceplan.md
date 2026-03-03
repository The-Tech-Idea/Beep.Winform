# BeepComboBox Painter — Fix & Enhancement Plan

> **Generated**: 2026-02-27  
> **Scope**: All painter classes in `ComboBoxes/Painters/`, plus `BeepComboBoxHelper` and `BeepComboBox.Drawing.cs`

---

## Painter Inventory

| Painter Class | File | ComboBoxType | Base |
|---|---|---|---|
| `StandardComboBoxPainter` | StandardComboBoxPainter.cs | Standard (6) | BaseComboBoxPainter |
| `MinimalComboBoxPainter` | MinimalComboBoxPainter.cs | Minimal (0) | BaseComboBoxPainter |
| `OutlinedComboBoxPainter` | OutlinedComboBoxPainter.cs | Outlined (1) | BaseComboBoxPainter |
| `RoundedComboBoxPainter` | RoundedComboBoxPainter.cs | Rounded (2) | BaseComboBoxPainter |
| `MaterialOutlinedComboBoxPainter` | MaterialOutlinedComboBoxPainter.cs | MaterialOutlined (3) | BaseComboBoxPainter |
| `FilledComboBoxPainter` | FilledComboBoxPainter.cs | Filled (4) | BaseComboBoxPainter |
| `BorderlessComboBoxPainter` | BorderlessComboBoxPainter.cs | Borderless (5) | BaseComboBoxPainter |
| `SmoothBorderPainter` | BorderVariantPainters.cs | SmoothBorder (16) | BaseComboBoxPainter |
| `DarkBorderPainter` | BorderVariantPainters.cs | DarkBorder (17) | BaseComboBoxPainter |
| `PillCornersComboBoxPainter` | BorderVariantPainters.cs | PillCorners (18) | BaseComboBoxPainter |
| `BlueDropdownPainter` | ColoredVariantPainters.cs | BlueDropdown (7) | OutlinedComboBoxPainter |
| `GreenDropdownPainter` | ColoredVariantPainters.cs | GreenDropdown (8) | OutlinedComboBoxPainter |
| `InvertedComboBoxPainter` | ColoredVariantPainters.cs | Inverted (9) | OutlinedComboBoxPainter |
| `ErrorComboBoxPainter` | ColoredVariantPainters.cs | Error (10) | OutlinedComboBoxPainter |
| `MultiSelectChipsPainter` | FeatureVariantPainters.cs | MultiSelectChips (11) | OutlinedComboBoxPainter |
| `SearchableDropdownPainter` | FeatureVariantPainters.cs | SearchableDropdown (12) | OutlinedComboBoxPainter |
| `WithIconsComboBoxPainter` | FeatureVariantPainters.cs | WithIcons (13) | OutlinedComboBoxPainter |
| `MenuComboBoxPainter` | FeatureVariantPainters.cs | Menu (14) | OutlinedComboBoxPainter |
| `CountrySelectorPainter` | FeatureVariantPainters.cs | CountrySelector (15) | WithIconsComboBoxPainter |
| **`BeepComboBoxPainter`** | BeepComboBoxPainter.cs | _(not used)_ | _(standalone)_ |
| **`ComboBoxPainterBase`** | ComboBoxPainterBase.cs | _(not used)_ | _(abstract)_ |

---

## Architecture Analysis

### Paint Pipeline
```
BeepComboBox.DrawContent(g)
  └── base.DrawContent(g)           ← BaseControl: draws border, shadow, background via painter system
  └── Paint(g, DrawingRect)
        └── if painter null → CreatePainter(type)
        └── UpdateLayout() if needed
        └── _comboBoxPainter.Paint(g, this, bounds)
              └── BaseComboBoxPainter.Paint()
                    ├── DrawBackground()   ← empty body (BaseControl already drew background)
                    ├── DrawTextArea()     ← hover/focus overlays on text area
                    ├── DrawLeadingImage() ← icon/image on left
                    ├── DrawText()         ← selected item text or placeholder
                    └── DrawDropdownButton() ← button bg + separator + arrow
        └── RegisterHitAreas()    ← hover detection only; actions=null (click handled in OnMouseDown)
```

### Key Design Fact
`BaseComboBoxPainter.Paint()` has `DrawBorder()` **commented out**:
```csharp
// DrawBorder(g, drawingRect);
```
**This means ALL `DrawBorder()` overrides in every painter are currently dead code.**  
The border is handled by `BaseControl`'s border painter system (Shadcn/Radix/NextJS/Linear painters) which already received the `PenAlignment.Inset` fix.

---

## BUG FIXES

### BUG-01 — `BeepComboBoxPainter.DrawSimpleArrow` modifies cached Pen (data corruption)
**File**: `BeepComboBoxPainter.cs`  
**Severity**: HIGH — modifies shared/cached `Pen` objects from `PaintersFactory`, corrupting rendering for other controls  
**Root Cause**:
```csharp
var pen = PaintersFactory.GetPen(arrowColor, penWidth);
pen.StartCap = LineCap.Round;   // ← modifies the cached pen!
pen.EndCap = LineCap.Round;
```
`PaintersFactory.GetPen` returns a cached/shared instance. Calling `.StartCap` / `.EndCap` on it mutates the shared object. Subsequent callers get a pen with unexpected caps.  
**Fix**:
```csharp
var basePen = PaintersFactory.GetPen(arrowColor, penWidth);
var pen = (System.Drawing.Pen)basePen.Clone();
try
{
    pen.StartCap = LineCap.Round;
    pen.EndCap = LineCap.Round;
    g.DrawLines(pen, arrowPoints);
}
finally { pen.Dispose(); }
```

---

### BUG-02 — `PenAlignment.Inset` on `GraphicsPath` (latent bug in `DrawBorder`)
**Files**: `OutlinedComboBoxPainter.cs`, `RoundedComboBoxPainter.cs`  
**Severity**: MEDIUM — currently dead code (DrawBorder not called), but latent bug if re-enabled  
**Root Cause**: GDI+ silently falls back from `PenAlignment.Inset` to `Center` for `GraphicsPath`, making half the stroke render outside the path bounds — thin/invisible right-side border (same issue fixed in `BorderPainterHelpers.PaintSimpleBorder`).  
**Fix** (if `DrawBorder` is ever re-enabled): Use the manual inset path approach identical to what was applied in `BorderPainterHelpers`:
```csharp
float halfStroke = borderWidth / 2f;
int radius = _owner.ScaleLogicalX(BorderRadiusLogical);
using var insetPath = path.CreateInsetPath(halfStroke, radius);
GraphicsPath drawTarget = (insetPath != null && insetPath.PointCount > 2) ? insetPath : path;
var pen = PaintersFactory.GetPen(borderColor, borderWidth);
pen.Alignment = PenAlignment.Center;
g.DrawPath(pen, drawTarget);
```
**Affected painters**: `OutlinedComboBoxPainter`, `RoundedComboBoxPainter` (and any other painter using `PenAlignment.Inset` on a `GraphicsPath`)

---

### BUG-03 — Arrow does not flip when dropdown is open
**File**: `BaseComboBoxPainter.cs` and all derived painters  
**Severity**: MEDIUM — visual regression; closed-state down-chevron always shown even when popup is open  
**Root Cause**: `DrawDropdownArrow(g, buttonRect, color)` has no `isOpen` parameter. The method always draws a down-pointing chevron.  
**Fix**: Add `bool isOpen` parameter to `DrawDropdownArrow` and pass `_owner.IsDropdownOpen`:
```csharp
// In BaseComboBoxPainter.cs - update signature
protected void DrawDropdownArrow(Graphics g, Rectangle buttonRect, Color arrowColor, bool isOpen = false)
{
    ...
    // When isOpen, flip the chevron (draw up-pointing)
    if (isOpen)
    {
        arrowPoints = new[] {
            new Point(centerX - arrowSize, centerY + arrowHalfHeight),
            new Point(centerX, centerY - arrowHalfHeight),
            new Point(centerX + arrowSize, centerY + arrowHalfHeight)
        };
    }
    ...
}

// In all DrawDropdownButton overrides:
DrawDropdownArrow(g, buttonRect, arrowColor, _owner.IsDropdownOpen);
```

---

### BUG-04 — `FilledComboBoxPainter`: No filled background; `DrawShadow()` is dead code
**File**: `FilledComboBoxPainter.cs`  
**Severity**: MEDIUM — "Filled" style looks identical to other styles; has no tinted background fill  
**Root Cause**:  
- `DrawBackground()` in `BaseComboBoxPainter` has an **empty body** — nothing fills the background.  
- `FilledComboBoxPainter.DrawShadow()` is defined but **never called**.  
- `GetRoundedRectPath()` inside `FilledComboBoxPainter` is only used by `DrawShadow()` → also dead code.  
**Fix**:
1. Override `DrawBackground` in `FilledComboBoxPainter` to render the tinted fill:
```csharp
protected override void DrawBackground(Graphics g, Rectangle rect)
{
    Color fillBase = _theme?.ComboBoxBackColor != Color.Empty
        ? _theme.ComboBoxBackColor
        : (_theme?.PrimaryColor ?? Color.LightGray);
    // Light alpha fill for Material Design "filled" look
    Color fillColor = Color.FromArgb(_owner.Focused ? 20 : 12, fillBase.R, fillBase.G, fillBase.B);
    var brush = PaintersFactory.GetSolidBrush(fillColor);
    using var path = GetRoundedRectPath(rect, BorderRadius);
    g.FillPath(brush, path);
}
```
2. Remove or mark `DrawShadow()` as `#pragma warning disable` / private dead code, or hook it into `DrawBackground`.

---

### BUG-05 — `DarkBorderPainter.DrawText()` hardcodes white text, ignores theme
**File**: `BorderVariantPainters.cs`  
**Severity**: LOW-MEDIUM — breaks theme compatibility; white text on light themes is invisible  
**Root Cause**: `textColor = Color.White;` unconditionally, bypassing theme's `ComboBoxForeColor`.  
**Fix**:
```csharp
protected override void DrawText(Graphics g, Rectangle textAreaRect)
{
    if (textAreaRect.IsEmpty) return;
    string displayText = _helper.GetDisplayText();
    if (string.IsNullOrEmpty(displayText)) return;

    Color textColor = _helper.IsShowingPlaceholder()
        ? Color.FromArgb(128, 200, 200, 200)
        : (_theme?.ComboBoxForeColor != Color.Empty
            ? _theme.ComboBoxForeColor
            : Color.FromArgb(230, 230, 230));  // near-white default for dark theme

    Font textFont = _owner.TextFont
        ?? BeepThemesManager.ToFont(_theme?.LabelFont)
        ?? PaintersFactory.GetFont("Segoe UI", 9f, FontStyle.Regular);

    TextRenderer.DrawText(g, displayText, textFont, textAreaRect, textColor,
        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
}
```

---

### BUG-06 — `InvertedComboBoxPainter.DrawText()` hardcodes white, null-unsafe on `TextFont`
**File**: `ColoredVariantPainters.cs`  
**Severity**: LOW — `_owner.TextFont` could be null, causing `NullReferenceException`  
**Root Cause**: `TextRenderer.DrawText(g, displayText, _owner.TextFont, ...)` — `TextFont` not null-checked.  
**Fix**: Same pattern as BUG-05: null-safe font resolution + theme-aware text color:
```csharp
Color textColor = _helper.IsShowingPlaceholder()
    ? Color.FromArgb(100, 255, 255, 255)
    : (_theme?.ComboBoxForeColor != Color.Empty ? _theme.ComboBoxForeColor : Color.White);

Font textFont = _owner.TextFont
    ?? BeepThemesManager.ToFont(_theme?.LabelFont)
    ?? PaintersFactory.GetFont("Segoe UI", 9f, FontStyle.Regular);
```

---

### BUG-07 — `ComboBoxPainterBase.DrawBackground()` hardcodes blue-tinted color
**File**: `ComboBoxPainterBase.cs`  
**Severity**: LOW — class appears unused (no painters derive from it directly), but the hardcoded `Color.FromArgb(240, 245, 255)` ignores all themes  
**Fix**: Either:  
(a) _(Preferred)_ Delete the `ComboBoxPainterBase` class entirely — it is superseded by `BaseComboBoxPainter`  
(b) Replace the hardcoded fill with theme-aware color:
```csharp
public virtual void DrawBackground(Graphics g, Rectangle rect)
{
    Color bg = _theme?.ComboBoxBackColor ?? _theme?.BackColor ?? SystemColors.Window;
    var brush = PaintersFactory.GetSolidBrush(bg);
    g.FillRectangle(brush, rect);
}
```

---

### BUG-08 — `BlueDropdownPainter` / `GreenDropdownPainter` button colors mismatch border
**File**: `ColoredVariantPainters.cs`  
**Severity**: LOW — border is hardcoded Blue/Green, but button hover/focus colors come from theme accent colors → visual mismatch  
**Fix**: Override `GetArrowColor()` and `DrawDropdownButton()` to use the fixed accent colors:
```csharp
// BlueDropdownPainter
private static readonly Color _accentColor = Color.FromArgb(66, 133, 244);

protected override Color GetArrowColor()
{
    if (!_owner.Enabled) return Color.FromArgb(120, _accentColor);
    if (_owner.Focused || _owner.IsButtonHovered) return _accentColor;
    return Color.FromArgb(180, _accentColor);
}
```
Same pattern for `GreenDropdownPainter` with `Color.FromArgb(52, 168, 83)`.

---

### BUG-09 — `MultiSelectChipsPainter`: `combined.Contains(a)` uses reference equality
**File**: `FeatureVariantPainters.cs`  
**Severity**: LOW — animating chips may show as duplicate chips if `SimpleItem` doesn't override `Equals()`  
**Fix**:
```csharp
// Replace reference-based Contains check with ID/text-based
foreach (var a in animating)
{
    if (!combined.Any(c => c.Id == a.Id || c.Text == a.Text))
        combined.Add(a);
}
```

---

## ENHANCEMENTS

### ENH-01 — Hover feedback missing in `MinimalComboBoxPainter`, `RoundedComboBoxPainter`, `FilledComboBoxPainter`, `BorderlessComboBoxPainter`
**Description**: These painters' `DrawDropdownButton` overrides only draw the arrow — no visual background change when the button is hovered. The user gets no feedback that the button is interactive.  
**Enhancement**: Add a subtle hover rectangle/pill behind the arrow, reusing the pattern from `StandardComboBoxPainter`:
```csharp
if (_owner.IsButtonHovered && _owner.Enabled)
{
    Color hoverFill = PathPainterHelpers.WithAlphaIfNotEmpty(
        _theme?.ComboBoxHoverBackColor ?? _theme?.PrimaryColor ?? Color.Empty, 60);
    if (hoverFill != Color.Empty && hoverFill.A > 0)
    {
        Rectangle visualRect = Rectangle.Inflate(buttonRect, -ScaleX(1), -ScaleY(1));
        using var path = GetRoundedRectPath(visualRect, ScaleX(4));
        g.FillPath(PaintersFactory.GetSolidBrush(hoverFill), path);
    }
}
```

---

### ENH-02 — Duplicate `GetRoundedRectPath` in every painter
**Description**: `GetRoundedRectPath` is copy-pasted into `StandardComboBoxPainter`, `OutlinedComboBoxPainter`, `RoundedComboBoxPainter`, `MaterialOutlinedComboBoxPainter`, `FilledComboBoxPainter`, `SmoothBorderPainter`, and `PillCornersComboBoxPainter` — 7 near-identical implementations, some with inconsistent `-1` off-by-ones.  
**Enhancement**: Move to `BaseComboBoxPainter` as a `protected` method (it already has `CreateRoundedPath` in `ComboBoxPainterBase` — consolidate):
```csharp
// In BaseComboBoxPainter.cs
protected GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
{
    var path = new GraphicsPath();
    if (radius <= 0) { path.AddRectangle(rect); return path; }
    int d = radius * 2;
    path.AddArc(rect.X, rect.Y, d, d, 180, 90);
    path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
    path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
    path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
    path.CloseFigure();
    return path;
}
```
Remove the `- 1` offsets that cause the inconsistency; let the inset path approach handle stroke alignment.

---

### ENH-03 — `MaterialOutlinedComboBoxPainter` floating label uses `DrawBorder` (dead code path)
**Description**: `DrawFloatingLabel()` is called from `DrawBorder()` which is never called. Floating label never renders.  
**Enhancement**: Move floating label drawing to an override of `DrawTextArea()` or add a dedicated `DrawDecorations()` hook after `DrawTextArea` in `BaseComboBoxPainter.Paint()`:
```csharp
// In BaseComboBoxPainter.Paint():
DrawTextArea(g, textAreaRect);
DrawDecorations(g, drawingRect);   // new virtual hook
```
```csharp
// In MaterialOutlinedComboBoxPainter:
protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
{
    if (!string.IsNullOrEmpty(_owner.LabelText))
    {
        Color labelColor = _owner.Focused
            ? (_theme?.PrimaryColor ?? Color.Empty)
            : PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.BorderColor ?? Color.Empty, 180);
        DrawFloatingLabel(g, drawingRect, labelColor);
    }
}
```

---

### ENH-04 — `SmoothBorderPainter` has no custom button drawing
**Description**: `SmoothBorderPainter` only customizes `DrawBorder` (dead code), so it renders identically to the default `BaseComboBoxPainter`. It should at least customize the button separator to match its smooth aesthetic.  
**Enhancement**: Override `DrawDropdownButton` to draw a smooth/subtle separator:
```csharp
protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
{
    // Gradient-fade separator instead of hard line
    int margin = ScaleY(8);
    using var brush = new LinearGradientBrush(
        new Point(buttonRect.Left, buttonRect.Top + margin),
        new Point(buttonRect.Left, buttonRect.Bottom - margin),
        Color.Transparent,
        PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.BorderColor ?? Color.Empty, 120));
    g.FillRectangle(brush, new Rectangle(buttonRect.Left, buttonRect.Top + margin, 1, buttonRect.Height - margin * 2));
    DrawDropdownArrow(g, buttonRect, GetArrowColor(), _owner.IsDropdownOpen);
}
```

---

### ENH-05 — `BeepComboBoxPainter` is unreachable dead code
**Description**: `BeepComboBoxPainter` does not implement `IComboBoxPainter`, is never instantiated by `CreatePainter()`, and duplicates logic from `BaseComboBoxPainter`. The class has the cached pen mutation bug (BUG-01).  
**Enhancement**: Either:  
(a) _(Preferred)_ Delete `BeepComboBoxPainter.cs` entirely — it duplicates `BaseComboBoxPainter` and is never used.  
(b) Convert it to implement `IComboBoxPainter` and wire it to a new `ComboBoxType.Beep` enum value if it was intended as a distinct style.

---

### ENH-06 — Clean up `DrawBorder` dead code documentation
**Description**: Every painter has a `DrawBorder()` override that's never called. Developers seeing these methods may be misled into thinking they affect border rendering.  
**Enhancement**: 
1. Add a prominent comment block in `BaseComboBoxPainter.Paint()`:
```csharp
// NOTE: DrawBorder() is intentionally NOT called here.
// Border rendering is handled by BaseControl's border painter system
// (Shadcn/Radix/NextJS/Linear painters in Styling/BorderPainters/).
// DrawBorder() overrides in derived classes are reserved for future
// use if per-type border customization is needed outside the global system.
```
2. Mark `DrawBorder` as `protected virtual void DrawBorder(...)` with a default no-op body (make it non-abstract) so it doesn't FORCE derived classes to implement it:
```csharp
// Change from:
protected abstract void DrawBorder(Graphics g, Rectangle rect);
// To:
protected virtual void DrawBorder(Graphics g, Rectangle rect) { /* reserved */ }
```

---

### ENH-07 — `ErrorComboBoxPainter`: Border color should respond to `_owner.HasError`
**Description**: `ErrorComboBoxPainter` always draws a red border, even when `_owner.HasError == false`. It's a dedicated error-state painter, but it should still respect the HasError flag or instead always be used intentionally only in error state.  
**Enhancement**: Add a comment + documentation, and consider linking `BeepComboBox.HasError = true` to auto-switch `ComboBoxType = Error` style, or simply note the intended usage:
```xml
/// <summary>
/// Permanently renders in error/validation-failed state with red border.
/// Intended to be assigned to BeepComboBox.ComboBoxType when validation fails.
/// For conditional error display, use HasError property with any other type.
/// </summary>
```

---

## PRIORITY SUMMARY

| # | ID | Description | File(s) | Priority | Status |
|---|---|---|---|---|---|
| 1 | BUG-01 | Cached Pen mutation in `BeepComboBoxPainter.DrawSimpleArrow` | ~~BeepComboBoxPainter.cs~~ (deleted) | 🔴 HIGH | ✅ Done |
| 2 | BUG-03 | Arrow never flips when dropdown is open | BaseComboBoxPainter.cs | 🟠 MEDIUM | ✅ Done |
| 3 | BUG-04 | `FilledComboBoxPainter` has no actual filled background | FilledComboBoxPainter.cs | 🟠 MEDIUM | ✅ Done |
| 4 | BUG-02 | `PenAlignment.Inset` on GraphicsPath (latent, dead code) | OutlinedComboBoxPainter.cs, RoundedComboBoxPainter.cs | 🟡 LOW-MEDIUM | ✅ Done |
| 5 | BUG-05 | `DarkBorderPainter` hardcoded white text | BorderVariantPainters.cs | 🟡 LOW-MEDIUM | ✅ Done |
| 6 | BUG-06 | `InvertedComboBoxPainter` null-unsafe TextFont | ColoredVariantPainters.cs | 🟡 LOW | ✅ Done |
| 7 | BUG-07 | `ComboBoxPainterBase` hardcoded blue bg (unused class) | ~~ComboBoxPainterBase.cs~~ (deleted) | 🟡 LOW | ✅ Done |
| 8 | BUG-08 | Blue/Green painter button colors mismatch border | ColoredVariantPainters.cs | 🟡 LOW | ✅ Done |
| 9 | BUG-09 | `MultiSelectChipsPainter` duplicate chip reference equality | FeatureVariantPainters.cs | 🟡 LOW | ✅ Done |
| 10 | ENH-01 | Add hover feedback to arrow-only button painters | Minimal, Rounded, Filled, Borderless painters | 🟠 MEDIUM | ✅ Done |
| 11 | ENH-02 | De-duplicate `GetRoundedRectPath` into base class | BaseComboBoxPainter.cs | 🟡 LOW | ✅ Done |
| 12 | ENH-03 | Fix floating label never renders (dead code path) | MaterialOutlinedComboBoxPainter.cs | 🟡 LOW | ✅ Done |
| 13 | ENH-04 | `SmoothBorderPainter` custom button drawing | BorderVariantPainters.cs | 🟢 NICE | ✅ Done |
| 14 | ENH-05 | Delete or repurpose `BeepComboBoxPainter` dead class | ~~BeepComboBoxPainter.cs~~ (deleted) | 🟡 LOW | ✅ Done |
| 15 | ENH-06 | Document `DrawBorder` dead code; make non-abstract | BaseComboBoxPainter.cs | 🟡 LOW | ✅ Done |
| 16 | ENH-07 | Document `ErrorComboBoxPainter` intended usage | ColoredVariantPainters.cs | 🟢 NICE | ✅ Done |

---

## IMPLEMENTATION ORDER

### Phase 1 — Critical Bugs (do first)
1. **BUG-01**: Fix cached Pen mutation in `BeepComboBoxPainter` → clone before modifying caps
2. **BUG-03**: Add `isOpen` parameter to `DrawDropdownArrow`; pass `_owner.IsDropdownOpen` everywhere
3. **BUG-04**: Override `DrawBackground` in `FilledComboBoxPainter`; remove dead `DrawShadow`

### Phase 2 — Correctness & Visual Fixes
4. **BUG-05/06**: Fix `DarkBorderPainter` and `InvertedComboBoxPainter` text color + null safety
5. **BUG-08**: Fix Blue/Green painter arrow & button hover color consistency
6. **ENH-01**: Add hover feedback to Minimal, Rounded, Filled, Borderless painters

### Phase 3 — Code Quality
7. **ENH-02**: De-duplicate `GetRoundedRectPath` into `BaseComboBoxPainter`
8. **ENH-06**: Document `DrawBorder` dead code; change from `abstract` to `virtual no-op`
9. **BUG-07**: Delete `ComboBoxPainterBase` (or mark obsolete)
10. **ENH-05**: Delete `BeepComboBoxPainter` (or repurpose)

### Phase 4 — Nice to Have
11. **ENH-03**: Fix floating label in `MaterialOutlinedComboBoxPainter`
12. **BUG-02**: Fix `PenAlignment.Inset` in dead `DrawBorder` code (defensive fix)
13. **ENH-04**: Custom `SmoothBorderPainter` button separator
14. **BUG-09**: `MultiSelectChipsPainter` text-based equality check
15. **ENH-07**: Add XML doc to `ErrorComboBoxPainter`

---

## NOTES

- `_owner.IsDropdownOpen` property needs to be verified / exposed from `BeepComboBox.Properties.cs` or `BeepComboBox.Core.cs`. If it's `_isPopupVisible` internally, expose it as `public bool IsDropdownOpen => _isPopupVisible;`.
- The `DrawBorder` code in painters is deliberately dead to avoid double-drawing with `BaseControl`'s border painter system. **Do not re-enable** `DrawBorder(g, drawingRect)` in `BaseComboBoxPainter.Paint()` unless the BaseControl border painters are bypassed for that ComboBoxType.
- `ComboBoxPainterBase` is entirely separate from `BaseComboBoxPainter`. No active painter inherits from `ComboBoxPainterBase` — it can be safely deleted.
- `BeepComboBoxPainter` is also unreachable — confirmed by tracing `CreatePainter()` switch statement in `BeepComboBox.Drawing.cs` which maps all ComboBoxType values and never returns a `BeepComboBoxPainter`.
