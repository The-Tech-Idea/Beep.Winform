# Beep.Winform Quick Reference

Quick reference for common patterns and operations in Beep.Winform control development.

## Control Creation Checklist

### Required Files
- [ ] `ControlName.cs` - Main control class inheriting from BaseControl
- [ ] `ControlName.Painters.cs` - Painter management and hit testing
- [ ] `ControlName.Drawing.cs` - Paint logic
- [ ] `ControlName.Animation.cs` - Animation (optional)
- [ ] `ControlNameStyle.cs` - Style enum
- [ ] `Painters/IControlNamePainter.cs` - Painter interface
- [ ] `Painters/BaseControlNamePainter.cs` - Base painter with helpers
- [ ] `Painters/Material3Painter.cs` - Concrete painters (16 styles)
- [ ] `ControlName/README.md` - Control documentation

## BaseControl Key Methods

```csharp
// Override these methods
protected override void DrawContent(Graphics g) { }
public override void ApplyTheme() { }
protected override void CalculateLayout() { }

// Use these properties
DrawingRect          // Material-aware layout rectangle (use instead of ClientRectangle)
CurrentTheme         // Current IBeepTheme instance
ScaleFactor          // DPI scaling factor
MaterialStyle        // Material Design style

// Use these helpers
ScaleValue(int)      // Scale value for DPI
GetScaledFont(...)   // Get DPI-scaled font
AddHitArea(...)      // Register clickable region
ClearHitList()       // Clear hit areas
```

## Painter Pattern

### Painter Interface
```csharp
public interface IControlNamePainter
{
    void Draw(ControlName control, Graphics g, Rectangle bounds);
    void DrawSelection(ControlName control, Graphics g, Rectangle selectedRect);
    void DrawHover(ControlName control, Graphics g, Rectangle hoverRect);
    void UpdateHitAreas(ControlName control, Rectangle bounds, 
        Action<string, Rectangle, Action> registerHitArea);
    string Name { get; }
}
```

### UseThemeColors Pattern (MANDATORY)
```csharp
public override void Draw(ControlName control, Graphics g, Rectangle bounds)
{
    bool useThemeColors = control.UseThemeColors;
    Color backgroundColor, foreColor, accentColor;

    if (useThemeColors && control.CurrentTheme != null)
    {
        backgroundColor = control.CurrentTheme.BackColor;
        foreColor = control.CurrentTheme.ForeColor;
        accentColor = control.CurrentTheme.AccentColor;
    }
    else
    {
        // Style-specific colors (preserve identity)
        backgroundColor = Color.FromArgb(255, 251, 254);
        foreColor = Color.FromArgb(28, 27, 31);
        accentColor = Color.FromArgb(103, 80, 164);
    }
    
    // Draw using selected colors
}
```

## Theme Integration

### ApplyTheme Pattern
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (CurrentTheme != null)
    {
        BackColor = CurrentTheme.BackColor;
        ForeColor = CurrentTheme.ForeColor;
        BorderColor = CurrentTheme.BorderColor;
        
        // Propagate to children
        foreach (Control child in Controls)
        {
            if (child is BaseControl beepChild)
            {
                beepChild.CurrentTheme = CurrentTheme;
                beepChild.ApplyTheme();
            }
        }
    }
}
```

### Theme Properties
```csharp
[Browsable(true)]
[Category("Appearance")]
[Description("Use theme colors instead of custom accent color.")]
[DefaultValue(true)]
public bool UseThemeColors { get; set; } = true;

[Browsable(true)]
[Category("Appearance")]
[Description("Custom accent color (used when UseThemeColors = false).")]
public Color AccentColor { get; set; } = Color.FromArgb(0, 120, 215);
```

## Hit Testing

### Registering Hit Areas
```csharp
public virtual void UpdateHitAreas(ControlName control, Rectangle bounds, 
    Action<string, Rectangle, Action> registerHitArea)
{
    for (int i = 0; i < control.Items.Count; i++)
    {
        var itemRect = new Rectangle(bounds.Left + 4, currentY, bounds.Width - 8, itemHeight);
        int index = i; // IMPORTANT: Capture for lambda
        registerHitArea($"Item_{i}", itemRect, () => control.SelectItemByIndex(index));
        currentY += itemHeight + 4;
    }
}
```

### Mouse Event Handling
```csharp
protected override void OnMouseMove(MouseEventArgs e)
{
    base.OnMouseMove(e);
    UpdateHoverState(e.Location);
}

protected override void OnMouseClick(MouseEventArgs e)
{
    base.OnMouseClick(e);
    HandleHitAreaClick(e.Location);
}
```

## Image Painting

### Shared ImagePainter Pattern
```csharp
// In BasePainter class
private static readonly ImagePainter _sharedImagePainter = new ImagePainter();

protected virtual void DrawIcon(ControlName control, Graphics g, string imagePath, Rectangle iconRect)
{
    _sharedImagePainter.ImagePath = imagePath; // Always string path
    if (control.CurrentTheme != null)
    {
        _sharedImagePainter.CurrentTheme = control.CurrentTheme;
        _sharedImagePainter.ApplyThemeOnImage = true;
    }
    _sharedImagePainter.DrawImage(g, iconRect);
}
```

## Animation

### Animation Setup
```csharp
private Timer _animationTimer;
private DateTime _animationStartTime;
private int _animationDurationMs = 200;
private bool _enableAnimation = true;

[Browsable(true)]
[Category("Animation")]
[Description("Enable smooth animation.")]
[DefaultValue(true)]
public bool EnableAnimation { get; set; } = true;

[Browsable(true)]
[Category("Animation")]
[Description("Duration of animation in milliseconds.")]
[DefaultValue(200)]
public int AnimationDuration { get; set; } = 200;
```

### Animation Timer
```csharp
private void AnimationTimer_Tick(object sender, EventArgs e)
{
    var elapsed = (DateTime.Now - _animationStartTime).TotalMilliseconds;
    var progress = Math.Min(1.0, elapsed / _animationDurationMs);
    
    // Update animated value with easing
    // Apply easing function (EaseOutCubic recommended)
    
    if (progress >= 1.0)
    {
        _animationTimer.Stop();
        _isAnimating = false;
    }
    
    Invalidate();
}
```

## Layout and DPI Scaling

### Layout Calculation
```csharp
protected override void CalculateLayout()
{
    base.CalculateLayout();
    
    // Use DrawingRect (not ClientRectangle)
    var baseRect = DrawingRect;
    int padding = ScaleValue(8); // DPI-aware scaling
    
    _contentRect = new Rectangle(
        baseRect.X + padding,
        baseRect.Y + padding,
        baseRect.Width - (padding * 2),
        baseRect.Height - (padding * 2)
    );
}
```

### DPI Scaling Helpers
```csharp
int padding = ScaleValue(8);              // Scale size value
Font font = GetScaledFont("Segoe UI", 14, FontStyle.Regular); // Scale font
Rectangle contentRect = DrawingRect;     // Already scaled rectangle
```

## Common Helper Methods

### Rounded Rectangle
```csharp
protected static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
{
    var path = new GraphicsPath();
    int d = Math.Max(0, Math.Min(radius * 2, Math.Min(rect.Width, rect.Height)));
    if (d <= 1) { path.AddRectangle(rect); return path; }
    var arc = new Rectangle(rect.X, rect.Y, d, d);
    path.AddArc(arc, 180, 90);
    arc.X = rect.Right - d; path.AddArc(arc, 270, 90);
    arc.Y = rect.Bottom - d; path.AddArc(arc, 0, 90);
    arc.X = rect.Left; path.AddArc(arc, 90, 90);
    path.CloseFigure();
    return path;
}
```

### Fill Rounded Rectangle
```csharp
protected static void FillRoundedRect(Graphics g, Rectangle rect, int radius, Color color)
{
    using var path = CreateRoundedPath(rect, radius);
    using var br = new SolidBrush(color);
    g.FillPath(br, path);
}
```

## Control Styles

### Standard 16 Styles
```csharp
public enum ControlNameStyle
{
    Material3,          // Material Design 3
    iOS15,              // iOS 15+
    AntDesign,          // Ant Design
    Fluent2,            // Microsoft Fluent 2
    MaterialYou,        // Material You (dynamic colors)
    Windows11Mica,      // Windows 11 Mica
    MacOSBigSur,        // macOS Big Sur
    ChakraUI,           // Chakra UI inspired
    TailwindCard,       // Tailwind CSS inspired
    NotionMinimal,      // Notion minimal style
    Minimal,            // Ultra-minimal
    VercelClean,        // Vercel clean monochrome
    StripeDashboard,    // Stripe dashboard style
    DarkGlow,           // Dark with neon glow
    DiscordStyle,       // Discord-inspired
    GradientModern      // Modern gradient style
}
```

## Designer Attributes

### Standard Attributes
```csharp
[ToolboxItem(true)]
[ToolboxBitmap(typeof(ControlName))]
[Category("Beep Controls")]
[Description("Control description")]
[DisplayName("Beep Control Name")]

[Browsable(true)]
[Category("Appearance")]
[Description("Property description")]
[DefaultValue(ControlNameStyle.Material3)]
public ControlNameStyle Style { get; set; }
```

## Control Styles Setup

### Double Buffering
```csharp
public ControlName()
{
    InitializeComponent();
    SetStyle(ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.AllPaintingInWmPaint |
             ControlStyles.UserPaint |
             ControlStyles.ResizeRedraw, true);
}
```

### Graphics Quality
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    var g = e.Graphics;
    g.SmoothingMode = SmoothingMode.AntiAlias;
    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
    
    // Draw content
}
```

## Critical Rules Summary

1. ✅ Always inherit from BaseControl
2. ✅ Always use StyledImagePainter for images
3. ✅ Always use partial classes (Painters, Drawing, Animation)
4. ✅ Always create distinct painters (no base painter inheritance)
5. ✅ Always implement UseThemeColors pattern in painters
6. ✅ Always override ApplyTheme()
7. ✅ Always use BeepFontManager for fonts
8. ✅ Always use BackgroundPainterFactory for backgrounds
9. ✅ Always use BorderPainterFactory for borders
10. ✅ Always use ShadowPainterFactory for shadows
11. ✅ Always use HitTestHelper for hit testing
12. ✅ Always use BeepStyling for styling
13. ✅ Event detection in layout manager, painter only paints
14. ✅ ImagePath is always string (never Image objects)
15. ✅ Use DrawingRect (not ClientRectangle)
16. ✅ Use ScaleValue for DPI scaling
17. ✅ Use shared ImagePainter instance (static readonly)
18. ✅ Capture lambda variables correctly
19. ✅ RefreshHitAreas on layout changes
20. ✅ Dispose animation timers properly

## Common Pitfalls

### ❌ DON'T:
- Create ImagePainter instances in loops
- Hardcode colors (use theme or style-specific)
- Forget lambda capture in hit areas
- Modify state in paint methods
- Use ClientRectangle instead of DrawingRect
- Forget to dispose animation timers
- Put multiple painters in one file
- Skip UseThemeColors implementation

### ✅ DO:
- Use shared static readonly ImagePainter
- Check UseThemeColors before using colors
- Capture loop variables for lambdas
- Keep paint methods read-only
- Use DrawingRect for layout
- Dispose timers in Dispose method
- One painter class per file
- Always implement UseThemeColors pattern

## File Organization

```
ControlName/
├── ControlName.cs                     # Main class
├── ControlName.Painters.cs            # Painter management
├── ControlName.Drawing.cs             # Paint logic
├── ControlName.Animation.cs           # Animation (optional)
├── ControlNameStyle.cs               # Style enum
├── Painters/
│   ├── IControlNamePainter.cs        # Interface
│   ├── BaseControlNamePainter.cs     # Base with helpers
│   ├── Material3Painter.cs           # Concrete painters
│   └── ...                           # More painters
├── Helpers/                           # Optional helpers
│   └── ControlNameRenderingHelper.cs
└── README.md                          # Documentation
```

## Reference Implementation

**BeepSideBar** is the gold standard:
- Location: `TheTechIdea.Beep.Winform.Controls.SideBar.BeepSideBar`
- Features: Complete painter architecture, animation, theme integration, hit testing
- Use as template for new controls
