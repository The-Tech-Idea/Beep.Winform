# BeepTextBox DPI Scaling Updates Required

Based on Microsoft DPI guidance and BeepFontManager/DpiScalingHelper updates.

## Issues Identified

### ❌ Issue 1: Hardcoded Font Creation (Not DPI-Aware)
**File:** `BeepTextBox.Properties.cs` line 171
```csharp
// CURRENT (WRONG):
private Font _textFont = new Font("Segoe UI", 10);

// SHOULD BE:
private Font _textFont = new Font("Segoe UI", 10, GraphicsUnit.Point);
```
**Problem:** No GraphicsUnit specified defaults to `GraphicsUnit.Point`, but doesn't scale with DPI changes.

**Fix:** Initialize with BeepFontManager and add DPI change handler.

---

### ❌ Issue 2: Font Created in OnPaint (Performance + DPI)
**Files:** 
- `BeepTextBox.Drawing.cs` lines 41, 99
- `BeepTextBox.Methods.cs` line 52

```csharp
// CURRENT (WRONG):
using (var font = new Font(_textFont.FontFamily, _textFont.Size * 0.8f))
{
    // Draw character count
}
```

**Problems:**
1. Creates new Font on every paint → GDI overhead
2. Not DPI-aware (no scale factor applied)
3. Hardcoded 0.8f multiplier doesn't account for DPI

**Fix:** Cache scaled fonts or use BeepFontManager.GetFontForPainter()

---

### ❌ Issue 3: Missing DPI Change Handler
**File:** No OnDpiScaleChanged override exists

**Problem:** When user moves window between monitors with different DPI, fonts don't scale.

**Fix:** Override OnDpiScaleChanged to call ScaleFontForDpi()

---

### ❌ Issue 4: Font Cache Not Invalidated on DPI Change
**File:** `BeepTextBox.Core.cs` lines 195-203

**Problem:** Cached metrics like `_cachedTextHeightPx` and `_cachedMinHeightPx` become stale when DPI changes.

**Fix:** Invalidate caches in DPI change handler.

---

## Recommended Changes

### 1. Update Font Initialization
**File:** `BeepTextBox.Properties.cs`

```csharp
// Change line 171:
private Font _textFont = BeepFontManager.GetFont("Segoe UI", 10f, FontStyle.Regular);

// Update TextFont property setter (line 185):
set
{
    if (_textFont != value)
    {
        // Don't dispose if it came from BeepFontManager cache
        if (_textFont != null && !IsSystemOrCachedFont(_textFont))
        {
            _textFont?.Dispose();
        }
        _textFont = value ?? BeepFontManager.GetFont("Segoe UI", 10f);
        UseThemeFont = false;
        _helper?.InvalidateAllCaches();
        InvalidateFontCache(); // From BaseControl
        RecomputeMinHeight();
        InvalidateLayout();
    }
}

private bool IsSystemOrCachedFont(Font font)
{
    return font == Control.DefaultFont || 
           font == System.Drawing.SystemFonts.DefaultFont;
}
```

---

### 2. Use Cached DPI-Aware Fonts for Drawing
**File:** `BeepTextBox.Drawing.cs`

```csharp
// BEFORE (line 41):
using (var font = new Font(_textFont.FontFamily, _textFont.Size * 0.8f))
{
    var textSize = TextUtils.MeasureText(g, countText, font);
    // ...
}

// AFTER:
// Cache the smaller font at class level
private Font _characterCountFont;

private Font GetCharacterCountFont()
{
    if (_characterCountFont == null && _textFont != null)
    {
        float smallerSize = _textFont.SizeInPoints * 0.8f;
        _characterCountFont = BeepFontManager.GetFontForPainter(
            _textFont.Name, 
            smallerSize, 
            this, 
            _textFont.Style);
    }
    return _characterCountFont ?? _textFont;
}

// In DrawCharacterCount:
private void DrawCharacterCount(Graphics g)
{
    string countText = $"{_text.Length}/{_maxLength}";
    Font font = GetCharacterCountFont();
    var textSize = TextUtils.MeasureText(g, countText, font);
    var location = new PointF(Width - textSize.Width - 5, Height - textSize.Height - 2);
    
    Color textColor = _text.Length > _maxLength * 0.9 ? Color.Red : Color.Gray;
    using (var brush = new SolidBrush(textColor))
    {
        g.DrawString(countText, font, brush, location);
    }
}
```

---

### 3. Add DPI Change Handler
**File:** `BeepTextBox.Core.cs` (add new method)

```csharp
/// <summary>
/// Handle DPI changes per Microsoft guidance
/// </summary>
protected override void OnDpiScaleChanged(float oldScaleX, float oldScaleY, float newScaleX, float newScaleY)
{
    // Scale fonts if explicitly set (not inherited)
    int oldDpi = (int)(oldScaleX * 96);
    int newDpi = (int)(newScaleX * 96);
    
    if (_textFont != null && !UseThemeFont)
    {
        ScaleFontForDpi(oldDpi, newDpi);
    }
    
    // Invalidate cached fonts
    _characterCountFont?.Dispose();
    _characterCountFont = null;
    _lineNumberFont?.Dispose();
    _lineNumberFont = null;
    
    // Invalidate cached metrics
    _cachedTextHeightPx = -1;
    _cachedMinHeightPx = -1;
    RecomputeMinHeight();
    
    // Let base class handle rest
    base.OnDpiScaleChanged(oldScaleX, oldScaleY, newScaleX, newScaleY);
}
```

---

### 4. Update Theme Font Handling
**File:** `BeepTextBox.Theme.cs` (line 42)

```csharp
// CURRENT:
if (UseThemeFont)
{
    _textFont = BeepFontManager.ToFont(_currentTheme.LabelSmall);
}

// ENHANCED (add DPI awareness):
if (UseThemeFont)
{
    // Use GetFontForPainter for DPI-aware font
    var themeFont = BeepFontManager.ToFont(_currentTheme.LabelSmall);
    if (themeFont != null)
    {
        _textFont = BeepFontManager.GetFontForPainter(
            themeFont.Name,
            themeFont.SizeInPoints,
            this,
            themeFont.Style);
    }
}
```

---

### 5. Update Dispose
**File:** `BeepTextBox.Core.cs`

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _delayedUpdateTimer?.Stop();
        _delayedUpdateTimer?.Dispose();
        _animationTimer?.Stop();
        _animationTimer?.Dispose();
        _typingTimer?.Stop();
        _typingTimer?.Dispose();
        _incrementalSearchTimer?.Stop();
        _incrementalSearchTimer?.Dispose();
        
        _helper?.Dispose();
        
        // Dispose cached fonts
        if (_textFont != null && !IsSystemOrCachedFont(_textFont))
        {
            _textFont?.Dispose();
        }
        _characterCountFont?.Dispose();
        _lineNumberFont?.Dispose();
    }
    base.Dispose(disposing);
}
```

---

## Implementation Priority

### High Priority (Breaks DPI scaling)
1. ✅ Add OnDpiScaleChanged override
2. ✅ Use BeepFontManager.GetFont() for initialization
3. ✅ Cache character count font

### Medium Priority (Performance)
4. ✅ Remove Font creation in OnPaint
5. ✅ Invalidate font caches on theme change

### Low Priority (Polish)
6. ✅ Update documentation
7. ✅ Add DPI scaling example to README

---

## Testing Checklist

- [ ] Move window between 100% and 150% DPI monitors
- [ ] Verify text stays readable (no tiny fonts)
- [ ] Check character count font scales proportionally
- [ ] Verify no GDI handle leaks (use Performance Monitor)
- [ ] Test with UseThemeFont = true/false
- [ ] Verify font changes update correctly

---

## References

- Microsoft DPI Guidance: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms
- BaseControl.ScaleFontForDpi() documentation
- BeepFontManager.GetFontForPainter() documentation
