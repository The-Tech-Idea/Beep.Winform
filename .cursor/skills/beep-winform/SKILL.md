---
name: beep-winform
description: Provides expert guidance for Beep.Winform Windows Forms control library development, including BaseControl architecture, painter pattern implementation, theme integration, control creation, hit testing, animation, and styling patterns. Use when creating or updating Beep controls, implementing painters, integrating themes, or working with Windows Forms controls in the Beep.Winform framework.
---

# Beep.Winform Development Guide

Expert guidance for developing Windows Forms controls using the Beep.Winform framework, a modern control library with Material Design integration, theme support, and painter-based rendering.

## Core Architecture

### BaseControl
Foundation class that all Beep controls inherit from:
- **Location**: `TheTechIdea.Beep.Winform.Controls/BaseControl/`
- **Key Responsibilities**: Paint pipeline, DPI scaling, theme integration, hit testing, Material Design layout
- **Key Methods**: `DrawContent()`, `ApplyTheme()`, `CalculateLayout()`, `OnPaint()`
- **Key Properties**: `DrawingRect`, `CurrentTheme`, `ScaleFactor`, `MaterialStyle`

### Painter Pattern (Strategy Pattern)
Visual rendering separated from control logic:
- **Interface**: `IControlNamePainter` - Defines drawing contract
- **Base Class**: `BaseControlNamePainter` - Provides helper methods
- **Concrete Painters**: One per visual style (Material3, iOS15, Fluent2, etc.)
- **Separation**: Painters only draw; no state mutation

### Theme System
Centralized color and styling management:
- **Manager**: `BeepThemesManager` - Theme management
- **Interface**: `IBeepTheme` - Theme contract
- **Integration**: `ApplyTheme()` method in all controls
- **Properties**: `UseThemeColors`, `CurrentTheme`, `AccentColor`

### File Structure Pattern
```
ControlName/
├── ControlName.cs                     # Main class (properties, events)
├── ControlName.Painters.cs            # Painter management
├── ControlName.Drawing.cs             # Paint logic
├── ControlName.Animation.cs           # Animation logic (optional)
├── Painters/
│   ├── IControlNamePainter.cs        # Painter interface
│   ├── BaseControlNamePainter.cs     # Base with helpers
│   ├── Material3Painter.cs           # Concrete painter 1
│   ├── iOS15Painter.cs               # Concrete painter 2
│   └── ...                           # More painters (16 styles)
├── Helpers/
│   ├── ControlNameRenderingHelper.cs # Rendering utilities
│   └── ControlNameAnimationHelper.cs # Animation utilities (optional)
└── ControlNameStyle.cs               # Style enum
```

## Control Creation Process

### Step 1: Create Main Control Class

**File:** `ControlName.cs`

```csharp
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ControlName))]
    [Category("Beep Controls")]
    [Description("Control description")]
    [DisplayName("Beep Control Name")]
    public partial class ControlName : BaseControl
    {
        #region Fields
        private ControlNameStyle _style = ControlNameStyle.Material3;
        private bool _useThemeColors = true;
        // Add other fields
        #endregion

        #region Constructor
        public ControlName()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
        }

        private void InitializeComponent()
        {
            // Subscribe to events
        }
        #endregion

        #region Public Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Visual style of the control.")]
        [DefaultValue(ControlNameStyle.Material3)]
        public ControlNameStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    InitializePainter();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors { get; set; } = true;
        #endregion

        #region Protected Methods
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (CurrentTheme != null)
            {
                BackColor = CurrentTheme.BackColor;
                ForeColor = CurrentTheme.ForeColor;
                // Apply theme to child components
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RefreshHitAreas();
        }
        #endregion
    }
}
```

### Step 2: Create Style Enum

**File:** `ControlNameStyle.cs`

```csharp
namespace TheTechIdea.Beep.Winform.Controls.YourNamespace
{
    public enum ControlNameStyle
    {
        Material3,
        iOS15,
        AntDesign,
        Fluent2,
        MaterialYou,
        Windows11Mica,
        MacOSBigSur,
        ChakraUI,
        TailwindCard,
        NotionMinimal,
        Minimal,
        VercelClean,
        StripeDashboard,
        DarkGlow,
        DiscordStyle,
        GradientModern
    }
}
```

### Step 3: Create Painter Interface

**File:** `Painters/IControlNamePainter.cs`

```csharp
using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace.Painters
{
    public interface IControlNamePainter
    {
        void Draw(ControlName control, Graphics g, Rectangle bounds);
        void DrawSelection(ControlName control, Graphics g, Rectangle selectedRect);
        void DrawHover(ControlName control, Graphics g, Rectangle hoverRect);
        void UpdateHitAreas(ControlName control, Rectangle bounds, 
            Action<string, Rectangle, Action> registerHitArea);
        string Name { get; }
    }
}
```

### Step 4: Create Base Painter

**File:** `Painters/BaseControlNamePainter.cs`

```csharp
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace.Painters
{
    public abstract class BaseControlNamePainter : IControlNamePainter
    {
        // Shared ImagePainter instance - DO NOT create new instances!
        private static readonly ImagePainter _sharedImagePainter = new ImagePainter();
        
        public abstract string Name { get; }
        public abstract void Draw(ControlName control, Graphics g, Rectangle bounds);

        public virtual void DrawSelection(ControlName control, Graphics g, Rectangle selectedRect)
        {
            // Default implementation
        }

        public virtual void DrawHover(ControlName control, Graphics g, Rectangle hoverRect)
        {
            // Default implementation
        }

        public virtual void UpdateHitAreas(ControlName control, Rectangle bounds, 
            Action<string, Rectangle, Action> registerHitArea)
        {
            // Default: register hit areas
        }

        #region Helper Methods
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

        protected virtual void DrawIcon(ControlName control, Graphics g, string imagePath, Rectangle iconRect)
        {
            try
            {
                _sharedImagePainter.ImagePath = imagePath;
                if (control.CurrentTheme != null)
                {
                    _sharedImagePainter.CurrentTheme = control.CurrentTheme;
                    _sharedImagePainter.ApplyThemeOnImage = true;
                }
                _sharedImagePainter.DrawImage(g, iconRect);
            }
            catch
            {
                // Fallback
            }
        }
        #endregion
    }
}
```

### Step 5: Create Concrete Painter

**File:** `Painters/Material3Painter.cs`

```csharp
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace.Painters
{
    public sealed class Material3Painter : BaseControlNamePainter
    {
        public override string Name => "Material3";

        public override void Draw(ControlName control, Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // CRITICAL: Check UseThemeColors setting
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
                // Preserve Material Design 3 identity
                backgroundColor = Color.FromArgb(255, 251, 254);
                foreColor = Color.FromArgb(28, 27, 31);
                accentColor = Color.FromArgb(103, 80, 164);
            }

            // Draw background
            using var bgBrush = new SolidBrush(backgroundColor);
            g.FillRectangle(bgBrush, bounds);

            // Draw control-specific content
            // Each painter implements this UNIQUELY
        }
    }
}
```

### Step 6: Painter Management

**File:** `ControlName.Painters.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.YourNamespace.Painters;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace
{
    public partial class ControlName
    {
        #region Painter Fields
        private IControlNamePainter _currentPainter;
        private Dictionary<string, (Rectangle rect, Action action)> _hitAreas = new();
        private int _hoveredItemIndex = -1;
        #endregion

        #region Painter Management
        private void InitializePainter()
        {
            _currentPainter = _style switch
            {
                ControlNameStyle.Material3 => new Material3Painter(),
                ControlNameStyle.iOS15 => new iOS15Painter(),
                // ... other styles
                _ => new Material3Painter()
            };
            RefreshHitAreas();
        }
        #endregion

        #region Hit Area Management
        private void RefreshHitAreas()
        {
            _hitAreas.Clear();
            if (_currentPainter != null && ClientRectangle.Width > 0)
            {
                _currentPainter.UpdateHitAreas(this, ClientRectangle, 
                    (name, rect, action) => _hitAreas[name] = (rect, action));
            }
        }

        private void UpdateHoverState(Point mouseLocation)
        {
            int previousHover = _hoveredItemIndex;
            _hoveredItemIndex = -1;

            foreach (var kvp in _hitAreas)
            {
                if (kvp.Value.rect.Contains(mouseLocation))
                {
                    if (kvp.Key.StartsWith("Item_") && 
                        int.TryParse(kvp.Key.Substring(5), out int index))
                    {
                        _hoveredItemIndex = index;
                        Cursor = Cursors.Hand;
                        break;
                    }
                }
            }

            if (_hoveredItemIndex < 0)
                Cursor = Cursors.Default;

            if (previousHover != _hoveredItemIndex)
                Invalidate();
        }

        private bool HandleHitAreaClick(Point mouseLocation)
        {
            foreach (var kvp in _hitAreas)
            {
                if (kvp.Value.rect.Contains(mouseLocation))
                {
                    kvp.Value.action?.Invoke();
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateHoverState(e.Location);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredItemIndex != -1)
            {
                _hoveredItemIndex = -1;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            HandleHitAreaClick(e.Location);
        }
        #endregion
    }
}
```

### Step 7: Drawing Implementation

**File:** `ControlName.Drawing.cs`

```csharp
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace
{
    public partial class ControlName
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            _currentPainter?.Draw(this, g, ClientRectangle);
        }
    }
}
```

## Critical Rules

### Always Follow These Rules

1. **Always inherit from BaseControl** - Never inherit directly from Control
2. **Always use StyledImagePainter** - For painting images and SVG
3. **Always use partial classes** - Split by functionality (Painters, Drawing, Animation)
4. **Always create distinct painters** - No base painter inheritance between styles
5. **Always support UseThemeColors** - Every painter must check this setting
6. **Always override ApplyTheme()** - Apply theme colors to control and children
7. **Always use BeepFontManager** - For getting fonts
8. **Always use BackgroundPainterFactory** - For painting backgrounds
9. **Always use BorderPainterFactory** - For painting borders
10. **Always use ShadowPainterFactory** - For painting shadows
11. **Always use HitTestHelper** - For hit testing
12. **Always use BeepStyling** - For applying FormStyle or ControlStyle
13. **Event detection in layout manager** - Painter only paints effects
14. **ImagePath is always string** - Never use Image objects directly

### Painter Pattern Rules

⚠️ **CRITICAL:**
- Each painter's `Draw()` method is UNIQUE - Material3 draws differently than iOS15
- Base class provides HELPERS only - Optional methods like `DrawMenuItems()`
- Painters call helpers BY CHOICE - Can use helpers OR implement custom
- No shared drawing logic - Each style defines its own layout and drawing
- One class per file - NEVER put multiple painters in one file
- UseThemeColors MUST be implemented - Check setting, use theme or style colors

### UseThemeColors Pattern (MANDATORY)

Every painter MUST implement:

```csharp
public override void Draw(ControlName control, Graphics g, Rectangle bounds)
{
    bool useThemeColors = control.UseThemeColors;
    Color backgroundColor, foreColor, accentColor;

    if (useThemeColors && control.CurrentTheme != null)
    {
        // Use theme's dedicated colors
        backgroundColor = control.CurrentTheme.BackColor;
        foreColor = control.CurrentTheme.ForeColor;
        accentColor = control.CurrentTheme.AccentColor;
    }
    else
    {
        // Use original style-specific colors (preserve identity!)
        backgroundColor = Color.FromArgb(255, 251, 254);  // MD3 Surface
        foreColor = Color.FromArgb(28, 27, 31);           // MD3 On Surface
        accentColor = Color.FromArgb(103, 80, 164);       // MD3 Primary
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
        
        // Propagate to child components
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

## Hit Testing Pattern

### Registering Hit Areas

```csharp
public virtual void UpdateHitAreas(ControlName control, Rectangle bounds, 
    Action<string, Rectangle, Action> registerHitArea)
{
    if (control.Items == null) return;

    int itemHeight = 44;
    int currentY = bounds.Top + 8;

    for (int i = 0; i < control.Items.Count; i++)
    {
        var itemRect = new Rectangle(bounds.Left + 4, currentY, bounds.Width - 8, itemHeight);
        int index = i; // IMPORTANT: Capture for lambda
        registerHitArea($"Item_{i}", itemRect, () => control.SelectItemByIndex(index));
        currentY += itemHeight + 4;
    }
}
```

### Hit Testing Flow

1. Control calls `RefreshHitAreas()` when layout changes
2. Painter's `UpdateHitAreas()` registers clickable rectangles
3. Mouse events check `_hitAreas` dictionary
4. Matching area triggers registered action
5. Control updates selection and raises events

## Animation Implementation (Optional)

### Animation Partial Class

**File:** `ControlName.Animation.cs`

```csharp
using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace
{
    public partial class ControlName
    {
        #region Animation Fields
        private Timer _animationTimer;
        private DateTime _animationStartTime;
        private int _animationDurationMs = 200;
        private bool _isAnimating = false;
        private bool _enableAnimation = true;
        #endregion

        #region Animation Properties
        [Browsable(true)]
        [Category("Animation")]
        [Description("Enable smooth animation.")]
        [DefaultValue(true)]
        public bool EnableAnimation
        {
            get => _enableAnimation;
            set => _enableAnimation = value;
        }

        [Browsable(true)]
        [Category("Animation")]
        [Description("Duration of animation in milliseconds.")]
        [DefaultValue(200)]
        public int AnimationDuration
        {
            get => _animationDurationMs;
            set => _animationDurationMs = Math.Max(0, value);
        }
        #endregion

        #region Animation Methods
        private void StartAnimation()
        {
            if (_isAnimating && _animationTimer != null)
                _animationTimer.Stop();

            _animationStartTime = DateTime.Now;
            _isAnimating = true;

            if (_animationTimer == null)
            {
                _animationTimer = new Timer();
                _animationTimer.Interval = 16; // ~60 FPS
                _animationTimer.Tick += AnimationTimer_Tick;
            }

            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - _animationStartTime).TotalMilliseconds;
            var progress = Math.Min(1.0, elapsed / _animationDurationMs);

            // Update animated value
            // Apply easing function

            if (progress >= 1.0)
            {
                _animationTimer.Stop();
                _isAnimating = false;
            }

            Invalidate();
        }

        private void DisposeAnimation()
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                _animationTimer.Tick -= AnimationTimer_Tick;
                _animationTimer.Dispose();
                _animationTimer = null;
            }
        }
        #endregion
    }
}
```

## Common Patterns

### Image Painting Pattern

```csharp
// Use shared ImagePainter instance (static readonly)
private static readonly ImagePainter _sharedImagePainter = new ImagePainter();

protected virtual void DrawIcon(ControlName control, Graphics g, string imagePath, Rectangle iconRect)
{
    try
    {
        _sharedImagePainter.ImagePath = imagePath; // Always string path
        if (control.CurrentTheme != null)
        {
            _sharedImagePainter.CurrentTheme = control.CurrentTheme;
            _sharedImagePainter.ApplyThemeOnImage = true;
        }
        _sharedImagePainter.DrawImage(g, iconRect);
    }
    catch
    {
        // Fallback drawing
    }
}
```

### Layout Calculation Pattern

```csharp
protected override void CalculateLayout()
{
    base.CalculateLayout();
    
    // Use DrawingRect (not ClientRectangle) for Material-aware layout
    var baseRect = DrawingRect;
    int padding = ScaleValue(8); // DPI-aware scaling
    
    // Calculate control-specific layout
    _contentRect = new Rectangle(
        baseRect.X + padding,
        baseRect.Y + padding,
        baseRect.Width - (padding * 2),
        baseRect.Height - (padding * 2)
    );
}
```

### DPI Scaling Pattern

```csharp
// Always use ScaleValue for sizes
int padding = ScaleValue(8);
int fontSize = ScaleValue(14);

// Use GetScaledFont for fonts
Font font = GetScaledFont("Segoe UI", fontSize, FontStyle.Regular);

// Use DrawingRect (already scaled) instead of ClientRectangle
Rectangle contentRect = DrawingRect;
```

## Anti-Patterns to Avoid

### ❌ DON'T:

1. **Don't create ImagePainter in loops**
   ```csharp
   // BAD
   for (int i = 0; i < items.Count; i++)
   {
       var painter = new ImagePainter(); // ❌ Creates many instances!
   }
   
   // GOOD
   private static readonly ImagePainter _sharedPainter = new ImagePainter();
   ```

2. **Don't hardcode colors**
   ```csharp
   // BAD
   g.FillRectangle(Brushes.Blue, rect); // ❌ Ignores themes!
   
   // GOOD
   Color blue = useThemeColors ? theme.AccentColor : Color.Blue;
   using var brush = new SolidBrush(blue);
   ```

3. **Don't forget lambda capture**
   ```csharp
   // BAD
   for (int i = 0; i < count; i++)
   {
       registerHitArea($"Item_{i}", rect, () => Select(i)); // ❌ All reference last i!
   }
   
   // GOOD
   for (int i = 0; i < count; i++)
   {
       int index = i; // ✅ Capture correctly
       registerHitArea($"Item_{index}", rect, () => Select(index));
   }
   ```

4. **Don't modify state in paint methods**
   ```csharp
   // BAD
   protected override void OnPaint(PaintEventArgs e)
   {
       _someField = CalculateValue(); // ❌ State change in paint!
   }
   
   // GOOD
   protected override void OnPaint(PaintEventArgs e)
   {
       var value = CalculateValue(); // ✅ Read-only calculation
   }
   ```

## Quick Reference Checklist

When creating a new control, verify:

- [ ] Main class inherits from BaseControl
- [ ] Partial classes for Painters, Drawing, Animation (if needed)
- [ ] Style enum with 16 options
- [ ] IPainter interface defined
- [ ] BasePainter with helper methods
- [ ] 16 concrete painter implementations (or as needed)
- [ ] All painters implement UseThemeColors pattern
- [ ] Hit testing via UpdateHitAreas
- [ ] Mouse events in Painters partial class
- [ ] Animation in separate partial class (if needed)
- [ ] Proper Dispose pattern
- [ ] Designer attributes (Browsable, Category, Description, DefaultValue)
- [ ] ToolboxItem, ToolboxBitmap, DisplayName attributes
- [ ] No hardcoded colors (use theme or style-specific)
- [ ] Shared ImagePainter instance (static readonly)
- [ ] Lambda capture done correctly
- [ ] RefreshHitAreas on layout changes
- [ ] SmoothingMode.AntiAlias for graphics
- [ ] DoubleBuffering enabled
- [ ] ApplyTheme() implemented and propagates to children

## Documentation Requirements

### README Files to Update

1. **Control README**: `ControlName/README.md` - Control-specific documentation
2. **BaseControl README**: `BaseControl/README.md` - BaseControl architecture
3. **Styling README**: `Styling/README.md` - Styling guidelines
4. **Theme README**: `ThemeManagement/README.md` - Theme system
5. **Main README**: `TheTechIdea.Beep.Winform.Controls/Readme.md` - Component list

### Documentation Update Triggers

Update README files when:
- Adding new public properties/methods
- Changing control behavior
- Adding theme integration
- Modifying layout logic
- Adding new dependencies
- Changing inheritance hierarchy

## Reference Implementation

**BeepSideBar** (`TheTechIdea.Beep.Winform.Controls.SideBar.BeepSideBar`) demonstrates perfect implementation:
- Complete painter architecture (16 styles)
- Smooth sliding animation (200ms, ease-out cubic)
- Full theme integration with UseThemeColors
- Hit testing for all menu items
- Hover and selection effects
- Clean partial class separation

**Use BeepSideBar as the gold standard for new controls!**

## Key File Locations

- **BaseControl**: `TheTechIdea.Beep.Winform.Controls/BaseControl/`
- **Theme Management**: `TheTechIdea.Beep.Winform.Controls/ThemeManagement/`
- **Styling**: `TheTechIdea.Beep.Winform.Controls/Styling/`
- **Image Painters**: `TheTechIdea.Beep.Winform.Controls/Styling/ImagePainters/`
- **Instructions**: `.github/instructions/`

## Related Documentation

- **Control Creation**: `.github/instructions/CreateUpdateBeepControl.instructions.md`
- **Co-pilot Instructions**: `.github/instructions/co-pilot.instructions.md`
- **Claude Instructions**: `.github/instructions/claude-instructions.md`
- **Cursor Instructions**: `.github/instructions/cursor-instructions.md`
- **Rules**: `.cursor/rules/mycontrolsonly.mdc`
