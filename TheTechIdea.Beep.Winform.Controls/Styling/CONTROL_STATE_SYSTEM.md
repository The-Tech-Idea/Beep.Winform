# Control State System for Painters

## Overview
All painters (Background, Border, Button, Text, Shadow) need to support control states to provide visual feedback for user interactions.

---

## Control States

### ControlState Enum
```csharp
public enum ControlState
{
    Normal,      // Default state
    Hovered,     // Mouse is over the control
    Pressed,     // Mouse button is down
    Selected,    // Control is selected
    Disabled,    // Control is disabled/inactive
    Focused      // Control has keyboard focus
}
```

---

## State Visual Effects

### Normal
- Default appearance
- No modifications

### Hovered
- **Background**: 5% lighter
- **Overlay**: 20α white
- **Purpose**: Subtle feedback that control is interactive

### Pressed
- **Background**: 10% darker
- **Overlay**: 30α black
- **Purpose**: Clear feedback for active click/touch

### Selected
- **Background**: 8% lighter
- **Overlay**: 25α white
- **Purpose**: Show control is currently selected

### Disabled
- **Background**: 100α opacity (semi-transparent)
- **Overlay**: 80α gray
- **Purpose**: Show control is inactive

### Focused
- **Background**: 3% lighter
- **Overlay**: 15α white
- **Purpose**: Show keyboard focus without being too prominent

---

## Implementation Pattern

### Background Painter Signature
```csharp
public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
    BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
    ControlState state = ControlState.Normal)  // ← Add state parameter with default
{
    // 1. Get base color
    Color backgroundColor = useThemeColors ? theme.BackColor : Color.White;
    
    // 2. Apply state modification
    backgroundColor = BackgroundPainterHelpers.ApplyState(backgroundColor, state);
    
    // 3. Paint base background
    using (var brush = new SolidBrush(backgroundColor))
    {
        if (path != null)
            g.FillPath(brush, path);
        else
            g.FillRectangle(brush, bounds);
    }
    
    // 4. Add style-specific effects (gradients, highlights, etc.)
    // ... style-specific code here ...
    
    // 5. Apply state overlay (last layer)
    Color stateOverlay = BackgroundPainterHelpers.GetStateOverlay(state);
    if (stateOverlay != Color.Transparent)
    {
        using (var brush = new SolidBrush(stateOverlay))
        {
            if (path != null)
                g.FillPath(brush, path);
            else
                g.FillRectangle(brush, bounds);
        }
    }
}
```

### Border Painter Signature
```csharp
public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path,
    BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
    ControlState state = ControlState.Normal)  // ← Add state parameter
{
    // Get base border color
    Color borderColor = GetBorderColor(style, theme, useThemeColors);
    
    // Apply state modification
    borderColor = BackgroundPainterHelpers.ApplyState(borderColor, state);
    
    // Paint border
    using (var pen = new Pen(borderColor, GetBorderWidth(state)))
    {
        if (path != null)
            g.DrawPath(pen, path);
        else
            g.DrawRectangle(pen, bounds);
    }
}
```

### Button Painter Signature
```csharp
public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
    BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
    ControlState upState = ControlState.Normal,     // ← State for up button
    ControlState downState = ControlState.Normal)   // ← State for down button
{
    // Paint up button with its state
    PaintButton(g, upButtonRect, style, theme, useThemeColors, upState, true);
    
    // Paint down button with its state
    PaintButton(g, downButtonRect, style, theme, useThemeColors, downState, false);
}
```

### Text Painter Signature
```csharp
public static void Paint(Graphics g, Rectangle bounds, string text,
    BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
    ControlState state = ControlState.Normal)  // ← Add state parameter
{
    // Get base text color
    Color textColor = useThemeColors ? theme.TextColor : Color.Black;
    
    // Apply state modification (especially important for Disabled)
    if (state == ControlState.Disabled)
    {
        textColor = BackgroundPainterHelpers.WithAlpha(textColor, 100);
    }
    
    // Paint text
    using (var brush = new SolidBrush(textColor))
    {
        g.DrawString(text, font, brush, bounds, format);
    }
}
```

### Shadow Painter Signature
```csharp
public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path,
    BeepControlStyle style, IBeepTheme theme,
    ControlState state = ControlState.Normal)  // ← Add state parameter
{
    // Shadows might change based on state
    // - Normal: Standard shadow
    // - Hovered: Slightly larger shadow (elevation)
    // - Pressed: Smaller shadow (depressed)
    // - Disabled: No shadow or very faint
    
    if (state == ControlState.Disabled)
        return; // No shadow when disabled
        
    int shadowSize = GetShadowSize(state);
    // Paint shadow...
}
```

---

## Helper Methods

### BackgroundPainterHelpers

```csharp
/// <summary>
/// Apply state modification to a color
/// </summary>
public static Color ApplyState(Color baseColor, ControlState state)
{
    switch (state)
    {
        case ControlState.Hovered:
            return Lighten(baseColor, 0.05f);  // 5% lighter
        case ControlState.Pressed:
            return Darken(baseColor, 0.1f);    // 10% darker
        case ControlState.Selected:
            return Lighten(baseColor, 0.08f);  // 8% lighter
        case ControlState.Disabled:
            return WithAlpha(baseColor, 100);  // Semi-transparent
        case ControlState.Focused:
            return Lighten(baseColor, 0.03f);  // 3% lighter
        case ControlState.Normal:
        default:
            return baseColor;
    }
}

/// <summary>
/// Get overlay color for state (applied as top layer)
/// </summary>
public static Color GetStateOverlay(ControlState state)
{
    switch (state)
    {
        case ControlState.Hovered:
            return WithAlpha(Color.White, 20);   // Subtle white highlight
        case ControlState.Pressed:
            return WithAlpha(Color.Black, 30);   // Darkening effect
        case ControlState.Selected:
            return WithAlpha(Color.White, 25);   // Highlight selected
        case ControlState.Focused:
            return WithAlpha(Color.White, 15);   // Subtle focus hint
        case ControlState.Disabled:
            return WithAlpha(Color.Gray, 80);    // Gray out disabled
        case ControlState.Normal:
        default:
            return Color.Transparent;            // No overlay
    }
}
```

---

## Usage in Controls

### BeepTextBox Example
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    // Determine state
    ControlState state = ControlState.Normal;
    if (!Enabled)
        state = ControlState.Disabled;
    else if (_isPressed)
        state = ControlState.Pressed;
    else if (_isHovered)
        state = ControlState.Hovered;
    else if (Focused)
        state = ControlState.Focused;
    
    // Paint with state
    Material3BackgroundPainter.Paint(
        e.Graphics, 
        ClientRectangle, 
        null, 
        CurrentStyle, 
        Theme, 
        UseThemeColors,
        state  // ← Pass the state
    );
}
```

### BeepButton Example
```csharp
protected override void OnMouseEnter(EventArgs e)
{
    _isHovered = true;
    Invalidate(); // Repaint with Hovered state
}

protected override void OnMouseDown(MouseEventArgs e)
{
    _isPressed = true;
    Invalidate(); // Repaint with Pressed state
}

protected override void OnMouseUp(MouseEventArgs e)
{
    _isPressed = false;
    Invalidate(); // Repaint with Normal/Hovered state
}
```

---

## State Priority

When multiple states apply, use this priority order:
1. **Disabled** (highest priority - overrides all)
2. **Pressed**
3. **Selected**
4. **Focused**
5. **Hovered**
6. **Normal** (lowest priority - default)

### Example Logic
```csharp
ControlState GetCurrentState()
{
    if (!Enabled) return ControlState.Disabled;
    if (_isPressed) return ControlState.Pressed;
    if (_isSelected) return ControlState.Selected;
    if (Focused) return ControlState.Focused;
    if (_isHovered) return ControlState.Hovered;
    return ControlState.Normal;
}
```

---

## Style-Specific State Variations

### Material 3
- **Hovered**: Elevation increases (additional shadow)
- **Pressed**: Elevation decreases
- **Disabled**: Gray tint with reduced opacity

### iOS 15
- **Hovered**: Very subtle highlight (2%)
- **Pressed**: System blue tint overlay
- **Disabled**: 50% opacity

### Neumorphism
- **Normal**: Raised appearance
- **Pressed**: Inverted shadows (appears pressed in)
- **Disabled**: Flattened (no shadows)

### Dark Glow
- **Hovered**: Glow intensifies (higher alpha)
- **Pressed**: Glow pulses (animated)
- **Disabled**: Glow disappears

---

## TODO: Update All Painters

### BackgroundPainters (21 files) - ⏳ IN PROGRESS
- ✅ Material3BackgroundPainter.cs (updated)
- ✅ MaterialYouBackgroundPainter.cs (updated)
- ⏳ iOS15BackgroundPainter.cs (needs state parameter)
- ⏳ MacOSBigSurBackgroundPainter.cs (needs state parameter)
- ⏳ ... (17 more to update)

### BorderPainters (21 files) - ⏳ TODO
- Need to add state parameter to all

### ButtonPainters (21 files) - ⏳ TODO
- Need to add upState and downState parameters

### TextPainters (21 files) - ⏳ TODO
- Need to add state parameter (especially for Disabled)

### ShadowPainters (21 files) - ⏳ TODO
- Need to add state parameter (shadow changes with elevation)

---

## Benefits

1. **Consistent UX**: All controls respond to interactions the same way
2. **Accessibility**: Clear visual feedback for all states
3. **Maintainability**: Centralized state logic in helpers
4. **Flexibility**: Each style can customize state appearance
5. **Performance**: Efficient state-based repainting

---

## Next Steps

1. ✅ Add ControlState enum to BackgroundPainterHelpers
2. ✅ Add ApplyState() and GetStateOverlay() helper methods
3. ✅ Update Material3 and MaterialYou as examples
4. ⏳ Update remaining 19 BackgroundPainters
5. ⏳ Update all 21 BorderPainters
6. ⏳ Update all 21 ButtonPainters  
7. ⏳ Update all 21 TextPainters
8. ⏳ Update all 21 ShadowPainters
9. ⏳ Update BeepStyling.cs to pass state parameters
10. ⏳ Update BaseControl to track and pass states
