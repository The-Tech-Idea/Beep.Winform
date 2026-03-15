# BeepAdvancedButton OnPaint Override Refactor Plan

## Executive Summary
Refactor `BeepAdvancedButton` to use direct `OnPaint` override pattern (like `BeepChevronButton`) instead of relying on `BaseControl.DrawContent()`. This enables full custom rendering control while maintaining theme colors, fonts, and BaseControl infrastructure.

---

## Problem Statement

### Current State
- `BeepAdvancedButton` relies on `BaseControl.DrawContent()` being called from `BaseControl.OnPaint()`
- Limited control over rendering pipeline and layering
- Cannot easily customize background, border, or content drawing order
- Harder to implement complex custom button styles (NewsBanner, Contact, LowerThird variants)

### Desired State
- Direct `OnPaint` override with full rendering control
- Maintain theme colors and fonts from `BaseControl`
- Keep all existing painters and variants working
- Enable truly custom button rendering while preserving theme integration

---

## Reference Pattern: BeepChevronButton

### Key Characteristics
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
    UpdateDrawingRect();
    ClearHitList();
    Draw(e.Graphics, DrawingRect);
}

protected override void DrawContent(Graphics g)
{
    // EMPTY - not used
}

public override void Draw(Graphics graphics, Rectangle rectangle)
{
    // Full custom drawing implementation
    // Uses theme colors: _currentTheme.ButtonBackColor, _currentTheme.ButtonHoverBackColor, etc.
    // Uses theme fonts: _textFont from ApplyTheme()
}
```

### Benefits
1. **Full control** over rendering pipeline
2. **Clear separation** between layout (`UpdateDrawingRect`) and rendering (`Draw`)
3. **Hit testing** integrated via `ClearHitList()` + `AddHitArea()`
4. **Theme integration** via `_currentTheme` and `ApplyTheme()`
5. **Animation support** via custom timers and state tracking

---

## Implementation Plan

### Phase 1: Create OnPaint Override Infrastructure

#### 1.1 Add OnPaint Override to BeepAdvancedButton.cs
**File:** `BeepAdvancedButton.cs`

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    if (!IsReadyForDrawing) return;
    
    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
    
    UpdateDrawingRect();
    ClearHitList();
    
    Draw(e.Graphics, DrawingRect);
}

protected override void DrawContent(Graphics g)
{
    // EMPTY - BeepAdvancedButton uses OnPaint/Draw pattern instead
}
```

**Reasoning:**
- Match `BeepChevronButton` pattern exactly
- `UpdateDrawingRect()` ensures layout is current
- `ClearHitList()` prepares for new hit area registration
- `Draw()` encapsulates all rendering logic

---

### Phase 2: Implement Draw Method

#### 2.1 Create Core Draw Method
**File:** `BeepAdvancedButton.cs` or new `BeepAdvancedButton.Drawing.cs`

```csharp
public override void Draw(Graphics graphics, Rectangle rectangle)
{
    if (rectangle.Width <= 0 || rectangle.Height <= 0) return;
    
    // Apply high-quality rendering if enabled
    if (EnableHighQualityRendering)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
    }
    
    // Step 1: Delegate to painter for custom rendering
    if (_currentPainter != null)
    {
        DrawUsingPainter(graphics, rectangle);
    }
    else
    {
        // Fallback: basic button rendering with theme colors
        DrawFallbackButton(graphics, rectangle);
    }
    
    // Step 2: Draw focus ring (if keyboard focus visible)
    if (_showFocusRing && _keyboardFocusVisible && Focused)
    {
        DrawFocusRing(graphics, rectangle);
    }
    
    // Step 3: Draw loading indicator (if loading)
    if (_isLoading)
    {
        DrawLoadingIndicator(graphics, rectangle);
    }
    
    // Step 4: Draw ripple effect (if active)
    if (_rippleActive && !ReduceMotion)
    {
        DrawRippleEffect(graphics, rectangle);
    }
}
```

---

#### 2.2 Implement Painter Integration
**File:** `BeepAdvancedButton.Drawing.cs`

```csharp
private void DrawUsingPainter(Graphics graphics, Rectangle rectangle)
{
    // Build paint contract with current state
    var contract = BuildPaintContract(rectangle);
    
    // Let painter do its work
    _currentPainter.Paint(graphics, contract);
    
    // Register hit areas from painter (if applicable)
    RegisterPainterHitAreas(rectangle);
}

private AdvancedButtonPaintContract BuildPaintContract(Rectangle rectangle)
{
    return new AdvancedButtonPaintContract
    {
        Bounds = rectangle,
        
        // State
        IsHovered = IsHovered,
        IsPressed = IsPressed,
        IsFocused = Focused,
        IsEnabled = Enabled,
        IsToggled = _isToggled,
        IsLoading = _isLoading,
        
        // Animation progress
        HoverProgress = _hoverProgress,
        PressProgress = _pressProgress,
        RippleProgress = _rippleProgress,
        LoadingRotation = _loadingRotationAngle,
        
        // Content
        Text = Text,
        IconLeft = _iconLeft,
        IconRight = _iconRight,
        ImagePath = ImagePath,
        
        // Theme colors (from BaseControl._currentTheme)
        BackgroundColor = GetBackgroundColor(),
        ForegroundColor = GetForegroundColor(),
        BorderColor = GetBorderColor(),
        
        // Theme fonts
        TextFont = _textFont,
        
        // Layout properties
        ButtonShape = _buttonShape,
        ButtonSize = _buttonSize,
        Padding = Padding,
        BorderRadius = BorderRadius,
        BorderThickness = BorderThickness,
        
        // Split button state
        LeftAreaHovered = _leftAreaHovered,
        RightAreaHovered = _rightAreaHovered,
        LeftAreaPressed = _leftAreaPressed,
        RightAreaPressed = _rightAreaPressed
    };
}
```

---

#### 2.3 Implement Theme Color Getters
**File:** `BeepAdvancedButton.Drawing.cs`

```csharp
/// <summary>
/// Get background color based on current state and theme
/// </summary>
private Color GetBackgroundColor()
{
    if (!Enabled)
        return _currentTheme?.DisabledBackColor ?? DisabledBackColor;
    
    if (IsPressed)
        return _currentTheme?.ButtonPressedBackColor ?? PressedBackColor;
    
    if (_isToggled)
        return _currentTheme?.ButtonSelectedBackColor ?? SelectedBackColor;
    
    if (IsHovered)
        return _currentTheme?.ButtonHoverBackColor ?? HoverBackColor;
    
    // Use theme or custom solid background
    return UseThemeColors 
        ? (_currentTheme?.ButtonBackColor ?? BackColor)
        : _solidBackground;
}

/// <summary>
/// Get foreground/text color based on current state and theme
/// </summary>
private Color GetForegroundColor()
{
    if (!Enabled)
        return _currentTheme?.DisabledForeColor ?? DisabledForeColor;
    
    if (IsPressed)
        return _currentTheme?.ButtonPressedForeColor ?? PressedForeColor;
    
    if (_isToggled)
        return _currentTheme?.ButtonSelectedForeColor ?? SelectedForeColor;
    
    if (IsHovered)
        return _currentTheme?.ButtonHoverForeColor ?? HoverForeColor;
    
    return UseThemeColors
        ? (_currentTheme?.ButtonForeColor ?? ForeColor)
        : _solidForeground;
}

/// <summary>
/// Get border color based on current state and theme
/// </summary>
private Color GetBorderColor()
{
    if (!Enabled)
        return _currentTheme?.DisabledBorderColor ?? DisabledBorderColor;
    
    if (Focused)
        return _currentTheme?.FocusBorderColor ?? FocusBorderColor;
    
    if (IsPressed)
        return _currentTheme?.ButtonPressedBorderColor ?? PressedBorderColor;
    
    if (IsHovered)
        return _currentTheme?.ButtonHoverBorderColor ?? HoverBorderColor;
    
    return _currentTheme?.BorderColor ?? BorderColor;
}
```

---

#### 2.4 Implement Fallback Drawing (No Painter)
**File:** `BeepAdvancedButton.Drawing.cs`

```csharp
/// <summary>
/// Fallback rendering when no painter is assigned
/// </summary>
private void DrawFallbackButton(Graphics graphics, Rectangle rectangle)
{
    using (GraphicsPath path = ButtonShapeHelper.CreateButtonPath(rectangle, _buttonShape, BorderRadius))
    {
        // Background
        Color bgColor = GetBackgroundColor();
        using (Brush bgBrush = new SolidBrush(bgColor))
        {
            graphics.FillPath(bgBrush, path);
        }
        
        // Border
        if (BorderThickness > 0)
        {
            Color borderColor = GetBorderColor();
            using (Pen borderPen = new Pen(borderColor, BorderThickness))
            {
                graphics.DrawPath(borderPen, path);
            }
        }
        
        // Text
        if (!string.IsNullOrEmpty(Text))
        {
            DrawButtonText(graphics, rectangle);
        }
        
        // Icon/Image
        if (!string.IsNullOrEmpty(ImagePath))
        {
            DrawButtonImage(graphics, rectangle);
        }
    }
}

private void DrawButtonText(Graphics graphics, Rectangle rectangle)
{
    Rectangle textRect = CalculateTextRect(rectangle);
    Color textColor = GetForegroundColor();
    
    TextFormatFlags flags = TextFormatFlags.HorizontalCenter 
        | TextFormatFlags.VerticalCenter 
        | TextFormatFlags.WordBreak 
        | TextFormatFlags.EndEllipsis;
    
    TextRenderer.DrawText(graphics, Text, _textFont, textRect, textColor, flags);
}

private void DrawButtonImage(Graphics graphics, Rectangle rectangle)
{
    // Use StyledImagePainter for consistent image rendering
    Rectangle imageRect = CalculateImageRect(rectangle);
    
    try
    {
        StyledImagePainter.Paint(graphics, imageRect, ImagePath);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error drawing button image: {ex.Message}");
    }
}
```

---

#### 2.5 Implement Effects Drawing
**File:** `BeepAdvancedButton.Drawing.cs`

```csharp
/// <summary>
/// Draw keyboard focus ring
/// </summary>
private void DrawFocusRing(Graphics graphics, Rectangle rectangle)
{
    Rectangle focusRect = rectangle;
    focusRect.Inflate(-_focusRingOffset, -_focusRingOffset);
    
    int focusRadius = BorderRadius + _focusRingRadiusDelta;
    
    using (GraphicsPath focusPath = ButtonShapeHelper.CreateButtonPath(
        focusRect, _buttonShape, focusRadius))
    {
        Color focusColor = _currentTheme?.FocusBorderColor ?? Color.RoyalBlue;
        using (Pen focusPen = new Pen(focusColor, _focusRingThickness))
        {
            focusPen.DashStyle = DashStyle.Dot;
            graphics.DrawPath(focusPen, focusPath);
        }
    }
}

/// <summary>
/// Draw loading spinner indicator
/// </summary>
private void DrawLoadingIndicator(Graphics graphics, Rectangle rectangle)
{
    int spinnerSize = Math.Min(rectangle.Width, rectangle.Height) / 3;
    Point center = new Point(
        rectangle.Left + rectangle.Width / 2,
        rectangle.Top + rectangle.Height / 2
    );
    
    Rectangle spinnerRect = new Rectangle(
        center.X - spinnerSize / 2,
        center.Y - spinnerSize / 2,
        spinnerSize,
        spinnerSize
    );
    
    using (Pen spinnerPen = new Pen(GetForegroundColor(), 2))
    {
        spinnerPen.StartCap = LineCap.Round;
        spinnerPen.EndCap = LineCap.Round;
        
        graphics.DrawArc(spinnerPen, spinnerRect, _loadingRotationAngle, 270);
    }
}

/// <summary>
/// Draw ripple click effect
/// </summary>
private void DrawRippleEffect(Graphics graphics, Rectangle rectangle)
{
    if (_rippleProgress <= 0) return;
    
    float maxRadius = (float)Math.Sqrt(rectangle.Width * rectangle.Width + rectangle.Height * rectangle.Height) / 2;
    float currentRadius = maxRadius * _rippleProgress;
    
    using (GraphicsPath clipPath = ButtonShapeHelper.CreateButtonPath(rectangle, _buttonShape, BorderRadius))
    {
        Region oldClip = graphics.Clip;
        graphics.SetClip(clipPath);
        
        int alpha = (int)((1 - _rippleProgress) * 60);
        Color rippleColor = Color.FromArgb(alpha, Color.White);
        
        using (Brush rippleBrush = new SolidBrush(rippleColor))
        {
            graphics.FillEllipse(rippleBrush,
                _rippleCenter.X - currentRadius,
                _rippleCenter.Y - currentRadius,
                currentRadius * 2,
                currentRadius * 2);
        }
        
        graphics.Clip = oldClip;
    }
}
```

---

### Phase 3: Update Painters

#### 3.1 Ensure Painters Use Theme Colors
All existing painters should use colors from `AdvancedButtonPaintContract` instead of hardcoded values.

**Example - Update SolidButtonPainter:**
```csharp
public override void Paint(Graphics g, AdvancedButtonPaintContract contract)
{
    Rectangle bounds = contract.Bounds;
    
    // Use theme colors from contract (which comes from BaseControl._currentTheme)
    Color bgColor = contract.BackgroundColor;
    Color fgColor = contract.ForegroundColor;
    Color borderColor = contract.BorderColor;
    
    // Apply state-based color variations
    if (contract.HoverProgress > 0)
    {
        bgColor = BlendColors(bgColor, Color.White, contract.HoverProgress * 0.1f);
    }
    
    // Rest of painting logic...
}
```

**Apply to all painters:**
- `SolidButtonPainter`
- `IconButtonPainter`
- `TextButtonPainter`
- `GhostButtonPainter`
- `FABButtonPainter`
- All NewsBanner variants
- All Contact variants
- All LowerThird variants
- All other custom painters

---

#### 3.2 Update ButtonPainterFactory
Ensure factory creates painters that respect theme colors:

```csharp
public static IAdvancedButtonPainter CreatePainter(
    AdvancedButtonStyle style,
    NewsBannerVariant newsBannerVariant,
    ContactVariant contactVariant,
    ChevronVariant chevronVariant)
{
    // Factory logic remains same, but all painters now receive
    // theme colors via AdvancedButtonPaintContract
    
    return style switch
    {
        AdvancedButtonStyle.Solid => new SolidButtonPainter(),
        AdvancedButtonStyle.Icon => new IconButtonPainter(),
        AdvancedButtonStyle.Text => new TextButtonPainter(),
        // ... etc
    };
}
```

---

### Phase 4: Update ApplyTheme

#### 4.1 Enhance Theme Application
**File:** `BeepAdvancedButton.cs`

```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    
    if (_currentTheme == null) return;
    
    // Apply theme colors to properties (used by GetBackgroundColor/GetForegroundColor)
    BackColor = _currentTheme.ButtonBackColor;
    ForeColor = _currentTheme.ButtonForeColor;
    HoverBackColor = _currentTheme.ButtonHoverBackColor;
    HoverForeColor = _currentTheme.ButtonHoverForeColor;
    PressedBackColor = _currentTheme.ButtonPressedBackColor;
    PressedForeColor = _currentTheme.ButtonPressedForeColor;
    SelectedBackColor = _currentTheme.ButtonSelectedBackColor;
    SelectedForeColor = _currentTheme.ButtonSelectedForeColor;
    DisabledBackColor = _currentTheme.DisabledBackColor;
    DisabledForeColor = _currentTheme.DisabledForeColor;
    BorderColor = _currentTheme.BorderColor;
    FocusBorderColor = _currentTheme.FocusBorderColor;
    
    // Apply theme font if UseThemeFont is true
    if (UseThemeFont && _currentTheme.ButtonFont != null)
    {
        _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonFont);
        Font = _textFont;
    }
    
    // Update solid colors to match theme (if using theme colors)
    if (UseThemeColors)
    {
        _solidBackground = _currentTheme.ButtonBackColor;
        _solidForeground = _currentTheme.ButtonForeColor;
    }
    
    Invalidate();
}
```

---

### Phase 5: Testing & Validation

#### 5.1 Test Matrix

| Test Case | Expected Behavior |
|-----------|-------------------|
| **Theme Change** | Colors update immediately across all button styles |
| **Font Change** | Text renders with new font when `UseThemeFont = true` |
| **Custom Colors** | Respect custom colors when `UseThemeColors = false` |
| **Hover State** | Theme hover colors apply smoothly |
| **Pressed State** | Theme pressed colors apply |
| **Disabled State** | Theme disabled colors apply |
| **Focus Ring** | Uses theme focus border color |
| **Loading** | Spinner uses theme foreground color |
| **Ripple** | Effect respects button shape |
| **Painters** | All 30+ painters render correctly with theme colors |

#### 5.2 Validation Checklist

- [ ] All button styles render correctly
- [ ] Theme colors propagate to all states
- [ ] Theme fonts apply when `UseThemeFont = true`
- [ ] Custom colors work when `UseThemeColors = false`
- [ ] Focus ring appears only on keyboard focus
- [ ] Loading indicator rotates smoothly
- [ ] Ripple effect clips to button shape
- [ ] Hit areas register correctly
- [ ] Split buttons work properly
- [ ] All painters function as expected
- [ ] No visual regressions from previous implementation
- [ ] Performance is acceptable (no excessive redraws)

---

### Phase 6: Documentation

#### 6.1 Update Code Comments
Add XML documentation to new methods:
- `OnPaint` override
- `Draw` method
- `DrawUsingPainter`
- `BuildPaintContract`
- All color getters
- All drawing helper methods

#### 6.2 Update Readme
**File:** `Buttons/BeepAdvancedButton/Readme.md`

Add section:
```markdown
## Rendering Architecture

### OnPaint Override Pattern

BeepAdvancedButton uses direct `OnPaint` override for full rendering control:

1. **OnPaint** - Entry point, sets up graphics state
2. **Draw** - Main rendering method, delegates to painter or fallback
3. **DrawUsingPainter** - Painter integration
4. **DrawFallback** - Basic rendering when no painter
5. **DrawEffects** - Focus ring, loading, ripple

### Theme Integration

- Theme colors accessed via `_currentTheme` from `BaseControl`
- Applied in `ApplyTheme()` override
- Used by `GetBackgroundColor()`, `GetForegroundColor()`, `GetBorderColor()`
- Painters receive theme colors via `AdvancedButtonPaintContract`

### Custom Colors

Set `UseThemeColors = false` to use custom `SolidBackground` and `SolidForeground` colors.
```

---

## Migration Path

### Backward Compatibility
✅ **No breaking changes** - all existing properties and methods remain
✅ **Painter API unchanged** - painters work with same contract
✅ **Event signatures unchanged** - no consumer code changes needed

### Performance Impact
- **Positive:** Direct OnPaint is more efficient than DrawContent callback
- **Neutral:** Same number of drawing operations
- **Positive:** Better control over invalidation timing

---

## Implementation Order

1. ✅ **Phase 1** - Add OnPaint override (1-2 hours)
2. ✅ **Phase 2** - Implement Draw method and helpers (3-4 hours)
3. ✅ **Phase 3** - Update painters for theme colors (2-3 hours)
4. ✅ **Phase 4** - Enhance ApplyTheme (1 hour)
5. ✅ **Phase 5** - Testing & validation (2-3 hours)
6. ✅ **Phase 6** - Documentation (1 hour)

**Total Estimated Time:** 10-14 hours

---

## Success Criteria

- ✅ BeepAdvancedButton renders identically to current implementation
- ✅ All button styles and variants work correctly
- ✅ Theme colors apply automatically
- ✅ Theme fonts apply when enabled
- ✅ Custom colors work when theme colors disabled
- ✅ Focus ring shows on keyboard navigation
- ✅ Loading indicator and ripple effects work
- ✅ Hit testing works for split buttons
- ✅ All painters render correctly
- ✅ No visual regressions
- ✅ Performance is maintained or improved

---

## Follow-Up Tasks

After successful implementation:

1. **Apply pattern to other button controls** that could benefit from OnPaint override
2. **Optimize painter caching** if performance bottlenecks are detected
3. **Add more theme properties** if needed for advanced customization
4. **Create custom painter examples** demonstrating full theme integration

---

## References

- **BeepChevronButton.cs** - Reference implementation
- **BaseControl.Properties.cs** - Theme and font properties
- **BeepThemesManager** - Theme color and font access
- **StyledImagePainter** - Image rendering helper
- **ButtonShapeHelper** - Shape path generation

---

**Plan Created:** 2025-01-XX  
**Status:** Ready for Implementation  
**Priority:** High  
**Complexity:** Medium  
