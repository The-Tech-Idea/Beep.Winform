# Fix: Form Content Overlapping Caption Bar on Style Change

## Problem Description
When changing the form style at runtime, the form content would move and overlap the caption bar. This was particularly noticeable when switching between different FormStyle values (Modern, Metro, GNOME, etc.).

## Root Cause
The issue had two parts:

### 1. Incomplete Style Coverage in DisplayRectangle
The `DisplayRectangle` property override was checking for only specific styles:
```csharp
// OLD - Only checked 5 styles
if (ShowCaptionBar && (FormStyle == FormStyle.Modern || 
                        FormStyle == FormStyle.Minimal || 
                        FormStyle == FormStyle.Material || 
                        FormStyle == FormStyle.Fluent || 
                        FormStyle == FormStyle.MacOS))
```

This meant that the new styles (Metro, Metro2, GNOME, Glass, Cartoon, ChatBubble, Custom) would draw their caption bars, but the `DisplayRectangle` wouldn't be adjusted, causing child controls to render in the wrong position.

### 2. No Layout Recalculation on Style Change
When `ApplyFormStyle()` was called, it would change the painter and invalidate (redraw), but it didn't trigger a layout recalculation. This meant child controls wouldn't reposition themselves based on the updated `DisplayRectangle`.

## Solution

### Fix 1: Simplified DisplayRectangle Check
Changed the condition to check for ALL custom styles (everything except Classic):
```csharp
// NEW - Checks all custom form styles
if (ShowCaptionBar && FormStyle != FormStyle.Classic)
{
    int captionHeight = Math.Max(ScaleDpi(CaptionHeight), (int)(Font.Height * 2.5f));
    rect.Y += captionHeight;
    rect.Height -= captionHeight;
}
```

**Why this works:**
- Classic is the only style that uses `FormBorderStyle.Sizable` (native Windows border)
- All other styles use `FormBorderStyle.None` and custom caption painting
- Inverting the check (`!= Classic` instead of listing all custom styles) is more maintainable

### Fix 2: Force Layout Recalculation
Added `PerformLayout()` call in `ApplyFormStyle()`:
```csharp
private void ApplyFormStyle()
{
    switch (FormStyle)
    {
        // ... all style cases ...
    }
    
    // NEW: Force layout recalculation to reposition child controls
    if (!DesignMode)
    {
        PerformLayout();
    }
    
    Invalidate();
}
```

**Why this works:**
- `PerformLayout()` forces WinForms to recalculate control positions
- It queries `DisplayRectangle` again, getting the updated value
- Child controls reposition based on the new available area
- The `!DesignMode` check prevents issues in the Visual Studio designer

## How DisplayRectangle Works

The `DisplayRectangle` property defines the client area available for child controls:

```csharp
public override Rectangle DisplayRectangle
{
    get
    {
        var rect = base.DisplayRectangle;  // Get form's client rectangle
        
        // If we have a custom caption bar, reduce available space
        if (ShowCaptionBar && FormStyle != FormStyle.Classic)
        {
            int captionHeight = Math.Max(ScaleDpi(CaptionHeight), (int)(Font.Height * 2.5f));
            rect.Y += captionHeight;        // Start below caption
            rect.Height -= captionHeight;   // Reduce available height
        }
        
        return rect;
    }
}
```

### Before Fix:
```
┌─────────────────────────┐
│ Caption Bar (32px)      │ ← Painted by painter
├─────────────────────────┤
│                         │
│  Form Content           │ ← Controls start at Y=0 (OVERLAP!)
│  (DisplayRectangle)     │
│                         │
└─────────────────────────┘
```

### After Fix:
```
┌─────────────────────────┐
│ Caption Bar (32px)      │ ← Painted by painter
├─────────────────────────┤ ← DisplayRectangle.Y = 32
│                         │
│  Form Content           │ ← Controls start at Y=32 (CORRECT!)
│  (DisplayRectangle)     │
│                         │
└─────────────────────────┘
```

## Files Modified
- `BeepiFormPro.cs`:
  - Updated `DisplayRectangle` property (line ~120)
  - Updated `ApplyFormStyle()` method (line ~55)

## Testing Recommendations

### Test Case 1: Style Switching at Runtime
1. Create a form with some controls (buttons, labels, etc.)
2. Add buttons to switch between different FormStyle values
3. Verify controls stay below the caption bar when switching

### Test Case 2: Caption Height Variations
1. Test styles with different caption heights (Minimal=28px, Fluent=40px)
2. Verify controls adjust position correctly
3. Check that DPI scaling works correctly

### Test Case 3: ShowCaptionBar Toggle
1. Start with caption bar visible
2. Toggle `ShowCaptionBar = false`
3. Verify controls move up to fill the space
4. Toggle back to `true` and verify they move down again

### Test Case 4: All Styles
Test all 13 FormStyle values:
- ✓ Modern
- ✓ Minimal
- ✓ Material
- ✓ Fluent
- ✓ MacOS
- ✓ Glass
- ✓ Cartoon
- ✓ ChatBubble
- ✓ Metro
- ✓ Metro2
- ✓ GNOME
- ✓ Custom
- ✓ Classic (should NOT adjust DisplayRectangle)

## Benefits

### 1. Maintainability
- No need to update `DisplayRectangle` when adding new styles
- Simple logic: "If not Classic, adjust for caption bar"

### 2. Consistency
- All custom styles behave the same way
- No special cases to remember

### 3. Correctness
- Controls never overlap the caption bar
- Layout automatically adjusts on style change
- Works with DPI scaling

### 4. Performance
- `PerformLayout()` is efficient and only called when needed
- `!DesignMode` check prevents designer issues

## Related Code

### CaptionHeight Property
The caption height comes from the painter's metrics:
```csharp
public int CaptionHeight { get; set; } = 32;  // Default

// Each painter can override via FormPainterMetrics:
// Minimal: 28px
// Material: 36px
// Fluent: 40px
// etc.
```

### ScaleDpi Method
Ensures caption height scales with DPI:
```csharp
protected int ScaleDpi(int value)
{
    return (int)(value * _dpiScaleFactor);
}
```

## Future Considerations

### If Adding New FormStyle Values:
1. Add the case to `ApplyFormStyle()` switch
2. Create the corresponding painter
3. Set `FormBorderStyle.None` for custom painting
4. DisplayRectangle will automatically work (no changes needed!)

### If Changing Caption Bar Behavior:
- Update `ShowCaptionBar` property logic
- Ensure `PerformLayout()` is called when toggling
- Test with all styles

## Summary
The fix ensures that when changing form styles, the content area (`DisplayRectangle`) is properly adjusted to account for the caption bar, and child controls are repositioned correctly via `PerformLayout()`. The simplified check (`FormStyle != Classic`) is more maintainable and works with all current and future custom form styles.
