# Caption Renderer Refactoring Plan
## Match BeepListBox Painter Pattern

### Current Problem
Caption renderers use old pattern with separate methods and callbacks. Need to match BeepListBox's clean painter pattern where everything is self-contained.

---

## Target Pattern (from BeepListBox)

```csharp
internal interface IListBoxPainter
{
    void Initialize(BeepListBox owner, IBeepTheme theme);
    void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect);
}
```

**Key Points:**
- Painter receives OWNER in Paint() method
- Painter accesses all properties through owner
- No callbacks, no delegates - just direct property access
- Paint() does EVERYTHING

---

## New ICaptionRenderer Pattern (Already Updated)

```csharp
internal interface ICaptionRenderer
{
    void Initialize(IBeepModernFormHost owner, IBeepTheme theme);
    void UpdateTheme(IBeepTheme theme);
    void SetShowSystemButtons(bool show);
    void Paint(Graphics g, IBeepModernFormHost owner, GraphicsPath captionBounds);
    bool OnMouseMove(Point location, out GraphicsPath invalidatedArea);
    void OnMouseLeave(out GraphicsPath invalidatedArea);
    bool OnMouseDown(Point location, Form form, out GraphicsPath invalidatedArea);
    bool HitTest(Point location);
}
```

---

## Required Changes for Each Renderer

### 1. **Fields to Remove**
```csharp
// OLD - Remove these
private Form _host;
private Func<IBeepTheme> _theme;
private Func<int> _captionHeight;
```

### 2. **Fields to Keep/Add**
```csharp
// NEW - Keep these
private IBeepModernFormHost _owner;  // Store owner reference
private IBeepTheme _theme;           // Store theme directly
private bool _showButtons = true;    // Button visibility
private RectangleF _closeRect, _maxRect, _minRect;  // Button positions
private bool _hoverClose, _hoverMax, _hoverMin;     // Hover states
```

### 3. **Initialize Method**
```csharp
// NEW Pattern
public void Initialize(IBeepModernFormHost owner, IBeepTheme theme)
{
    _owner = owner;
    _theme = theme;
}
```

### 4. **UpdateTheme Method**
```csharp
public void UpdateTheme(IBeepTheme theme)
{
    _theme = theme;
    _owner?.Invalidate();
}
```

### 5. **Paint Method Signature**
```csharp
// NEW - Paint receives owner
public void Paint(Graphics g, IBeepModernFormHost owner, GraphicsPath captionBounds)
{
    // Access properties through owner:
    var form = owner.AsForm;
    var theme = owner.CurrentTheme;
    float scale = form.DeviceDpi / 96f;
    
    // Get caption bounds
    var bounds = captionBounds.GetBounds();
    
    // 1. Paint caption background
    PaintCaptionBackground(g, bounds, theme);
    
    // 2. Paint title text
    PaintTitle(g, form, bounds, theme, scale);
    
    // 3. Paint system buttons (if enabled)
    if (_showButtons)
    {
        CalculateButtonPositions(bounds, scale);
        PaintSystemButtons(g, theme, form.WindowState, scale);
    }
}
```

### 6. **Access Owner Properties**
```csharp
// In Paint method, access everything through owner:
var form = owner.AsForm;
string title = form.Text;
Font titleFont = form.Font;
FormWindowState windowState = form.WindowState;
float dpiScale = form.DeviceDpi / 96f;
var theme = owner.CurrentTheme;
```

### 7. **Helper Methods to Add**
```csharp
private void PaintCaptionBackground(Graphics g, RectangleF bounds, IBeepTheme theme)
{
    // Paint gradient or solid background
    using var brush = new SolidBrush(theme?.AppBarBackColor ?? Color.FromArgb(240, 240, 240));
    g.FillRectangle(brush, bounds);
}

private void PaintTitle(Graphics g, Form form, RectangleF bounds, IBeepTheme theme, float scale)
{
    // Calculate title position (accounting for button space)
    int leftPad = (int)(8 * scale);
    int rightPad = _showButtons ? GetButtonsWidth(scale) : (int)(8 * scale);
    
    var titleRect = new RectangleF(
        bounds.Left + leftPad,
        bounds.Top,
        bounds.Width - leftPad - rightPad,
        bounds.Height);
    
    // Draw title
    Color titleColor = theme?.AppBarTitleForeColor ?? form.ForeColor;
    using var brush = new SolidBrush(titleColor);
    using var sf = new StringFormat
    {
        LineAlignment = StringAlignment.Center,
        Alignment = GetTitleAlignment() // Center for Mac/Gnome, Left for Windows
    };
    
    g.DrawString(form.Text, form.Font, brush, titleRect, sf);
}

private void CalculateButtonPositions(RectangleF bounds, float scale)
{
    int pad = (int)(8 * scale);
    float btnSize = Math.Max(24, bounds.Height - (2 * pad));
    float top = bounds.Top + (bounds.Height - btnSize) / 2f;
    
    // Right-aligned buttons (Windows style) or Left-aligned (Mac style)
    if (IsLeftAligned())
    {
        float x = bounds.Left + pad;
        _closeRect = new RectangleF(x, top, btnSize, btnSize);
        x += btnSize + pad;
        _minRect = new RectangleF(x, top, btnSize, btnSize);
        x += btnSize + pad;
        _maxRect = new RectangleF(x, top, btnSize, btnSize);
    }
    else
    {
        float x = bounds.Right - pad - btnSize;
        _closeRect = new RectangleF(x, top, btnSize, btnSize);
        x -= btnSize + pad;
        _maxRect = new RectangleF(x, top, btnSize, btnSize);
        x -= btnSize + pad;
        _minRect = new RectangleF(x, top, btnSize, btnSize);
    }
}

private void PaintSystemButtons(Graphics g, IBeepTheme theme, FormWindowState windowState, float scale)
{
    Color iconColor = theme?.AppBarTitleForeColor ?? Color.Black;
    using var pen = new Pen(iconColor, 1.5f * scale);
    
    // Minimize button
    if (_hoverMin)
    {
        using var hoverBrush = new SolidBrush(theme?.ButtonHoverBackColor ?? Color.FromArgb(30, Color.Gray));
        g.FillRectangle(hoverBrush, _minRect);
    }
    CaptionGlyphProvider.DrawMinimize(g, pen, Rectangle.Round(_minRect), scale);
    
    // Maximize/Restore button
    if (_hoverMax)
    {
        using var hoverBrush = new SolidBrush(theme?.ButtonHoverBackColor ?? Color.FromArgb(30, Color.Gray));
        g.FillRectangle(hoverBrush, _maxRect);
    }
    if (windowState == FormWindowState.Maximized)
        CaptionGlyphProvider.DrawRestore(g, pen, Rectangle.Round(_maxRect), scale);
    else
        CaptionGlyphProvider.DrawMaximize(g, pen, Rectangle.Round(_maxRect), scale);
    
    // Close button
    if (_hoverClose)
    {
        using var hoverBrush = new SolidBrush(theme?.ButtonErrorBackColor ?? Color.FromArgb(232, 17, 35));
        g.FillRectangle(hoverBrush, _closeRect);
        pen.Color = Color.White; // White X on red background
    }
    CaptionGlyphProvider.DrawClose(g, pen, Rectangle.Round(_closeRect), scale);
}

protected virtual StringAlignment GetTitleAlignment() => StringAlignment.Near;
protected virtual bool IsLeftAligned() => false; // Windows: right-aligned buttons
protected virtual int GetButtonsWidth(float scale)
{
    if (!_showButtons) return 0;
    int pad = (int)(8 * scale);
    int btnSize = (int)(32 * scale);
    return (3 * btnSize) + (4 * pad);
}
```

---

## Renderers to Update (17 files)

### High Priority (Core Styles)
1. ✅ **WindowsCaptionRenderer.cs** - Reference implementation
2. **MacLikeCaptionRenderer.cs** - Left-aligned buttons, center title
3. **ModernCaptionRenderer.cs** (if exists) - Default modern style
4. **MetroCaptionRenderer.cs** - Flat, minimal style

### Medium Priority (Linux DE Styles)
5. **GnomeCaptionRenderer.cs** - Center title, left buttons
6. **KdeCaptionRenderer.cs** - Standard buttons, blue accents
7. **CinnamonCaptionRenderer.cs** - Mint green accents
8. **ElementaryCaptionRenderer.cs** - macOS-like but unique

### Low Priority (Themed Styles)
9. **OfficeCaptionRenderer.cs** - Ribbon-style
10. **NeonCaptionRenderer.cs** - Glowing effects
11. **RetroCaptionRenderer.cs** - Pixelated/vintage
12. **GamingCaptionRenderer.cs** - RGB effects
13. **CorporateCaptionRenderer.cs** - Professional/minimal
14. **ArtisticCaptionRenderer.cs** - Creative gradients
15. **IndustrialCaptionRenderer.cs** - Dark, mechanical
16. **HighContrastCaptionRenderer.cs** - Accessibility
17. **SoftCaptionRenderer.cs** - Rounded, pastel

### Files in MiscCaptionRenderers.cs
18. Check if there are multiple renderers bundled

---

## Implementation Steps

### Phase 1: Update Reference Renderer
1. ✅ Update `WindowsCaptionRenderer.cs` to new pattern
2. Test with BeepiForm
3. Verify all interactions work (hover, click, resize)

### Phase 2: Update Core Renderers (Priority 1-4)
For each renderer:
1. Remove old fields (`Func<>` delegates)
2. Add new fields (`_owner`, `_theme`)
3. Update `Initialize()` signature
4. Rewrite `Paint()` to receive owner
5. Add helper methods (PaintCaptionBackground, PaintTitle, etc.)
6. Update button positioning for style
7. Test with BeepiForm

### Phase 3: Update Linux DE Renderers (Priority 5-8)
- Same steps as Phase 2
- Pay attention to left vs. right button alignment
- Center vs. left title alignment

### Phase 4: Update Themed Renderers (Priority 9-17)
- Same steps as Phase 2
- Add style-specific effects in helper methods

### Phase 5: Cleanup
1. Remove any old helper classes no longer used
2. Update documentation
3. Run full test suite

---

## Testing Checklist (Per Renderer)

- [ ] Form loads without errors
- [ ] Caption background renders correctly
- [ ] Title text displays correctly
- [ ] System buttons appear in correct position
- [ ] Hover effects work on all buttons
- [ ] Click close button closes form
- [ ] Click minimize button minimizes form
- [ ] Click maximize button maximizes form
- [ ] Click restore button (when maximized) restores form
- [ ] Theme changes apply correctly
- [ ] DPI scaling works correctly
- [ ] HitTest returns true for button areas

---

## Common Pitfalls to Avoid

1. ❌ **Don't store delegates** - Access owner properties directly
2. ❌ **Don't use callbacks** - Everything in Paint()
3. ❌ **Don't cache form properties** - Always read from owner
4. ✅ **Do store owner reference** - Access via `_owner.AsForm`
5. ✅ **Do calculate positions in Paint()** - DPI might change
6. ✅ **Do use helper methods** - Keep Paint() readable
7. ✅ **Do handle null theme gracefully** - Fallback colors

---

## Example: Before vs After

### BEFORE (Old Pattern - BAD)
```csharp
private Form _host;
private Func<IBeepTheme> _theme;
private Func<int> _captionHeight;

public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
{
    _host = host;
    _theme = themeProvider;
    _captionHeight = captionHeightProvider;
}

public void Paint(Graphics g, GraphicsPath captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
{
    var bounds = captionBounds.GetBounds();
    int height = _captionHeight(); // Callback!
    var currentTheme = _theme();    // Callback!
    // ... manual painting
}
```

### AFTER (New Pattern - GOOD)
```csharp
private IBeepModernFormHost _owner;
private IBeepTheme _theme;

public void Initialize(IBeepModernFormHost owner, IBeepTheme theme)
{
    _owner = owner;
    _theme = theme;
}

public void Paint(Graphics g, IBeepModernFormHost owner, GraphicsPath captionBounds)
{
    var form = owner.AsForm;         // Direct access!
    var theme = owner.CurrentTheme;  // Direct access!
    float scale = form.DeviceDpi / 96f;
    var bounds = captionBounds.GetBounds();
    
    // Paint everything
    PaintCaptionBackground(g, bounds, theme);
    PaintTitle(g, form, bounds, theme, scale);
    if (_showButtons)
    {
        CalculateButtonPositions(bounds, scale);
        PaintSystemButtons(g, theme, form.WindowState, scale);
    }
}
```

---

## Success Criteria

✅ All 17+ caption renderers updated to new pattern
✅ No compilation errors
✅ All renderers tested with BeepiForm
✅ All system button interactions working
✅ Theme switching working
✅ DPI scaling working
✅ No regression in existing functionality

---

## Next Steps

1. Start with WindowsCaptionRenderer (reference implementation)
2. Create template renderer with all helper methods
3. Copy template structure to other renderers
4. Customize per-renderer (button position, title alignment, effects)
5. Test each one
6. Document any issues found
