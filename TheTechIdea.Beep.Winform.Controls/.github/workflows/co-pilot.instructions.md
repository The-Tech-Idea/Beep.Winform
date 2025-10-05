# Beep.Winform Control Creation Guidelines
**Version:** 2.0  
**Last Updated:** October 3, 2025  
**Status:** Official Standard for All Beep Controls

---

## Table of Contents
1. [Architecture Principles](#architecture-principles)
2. [Control Creation Process](#control-creation-process)
3. [File Structure Pattern](#file-structure-pattern)
4. [Painter Architecture](#painter-architecture)
5. [Animation Implementation](#animation-implementation)
6. [Theme Integration](#theme-integration)
7. [Hit Testing Pattern](#hit-testing-pattern)
8. [Helper Classes](#helper-classes)
9. [Code Examples](#code-examples)
10. [Testing Requirements](#testing-requirements)

---

## Architecture Principles

### Core Principles
1. **Separation of Concerns** - Each file has ONE responsibility
2. **Painter Pattern** - Visual styles are pluggable strategies
3. **Partial Classes** - Large controls split by functionality
4. **Theme Integration** - All colors/fonts from IBeepTheme
5. **Animation Support** - Smooth transitions for modern UX
6. **Hit Testing** - Centralized click detection
7. **Designer Support** - Full Property Grid integration

### File Organization Standard
```
ControlName/
├── ControlName.cs                     # Main class (properties, events)
├── ControlName.Painters.cs            # Painter management
├── ControlName.Drawing.cs             # Paint logic
├── ControlName.Animation.cs           # Animation logic (optional)
├── Painters/
│   ├── IControlNamePainter.cs        # Painter interface
│   ├── BaseControlNamePainter.cs     # Base with helpers
│   ├── Style1Painter.cs              # Concrete painter 1
│   ├── Style2Painter.cs              # Concrete painter 2
│   └── ...                           # More painters
├── Helpers/
│   ├── ControlNameRenderingHelper.cs # Rendering utilities
│   └── ControlNameAnimationHelper.cs # Animation utilities (optional)
└── ControlNameStyle.cs               # Style enum
```

---

## Control Creation Process

### Step 1: Define the Control Class

**File:** `ControlName.cs`

```csharp
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace
{
    /// <summary>
    /// ControlName - Brief description
    /// Clean implementation using the painter architecture
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ControlName))]
    [Category("Beep Controls")]
    [Description("Control description")]
    [DisplayName("Beep Control Name")]
    public partial class ControlName : BaseControl
    {
        #region Fields
        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private ControlNameStyle _style = ControlNameStyle.Material3;
        // Add other fields
        #endregion

        #region Events
        public event Action<SimpleItem> ItemClicked;
        public event PropertyChangedEventHandler PropertyChanged;
        // Add other events
        #endregion

        #region Constructor
        public ControlName()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            
            // Set defaults
        }

        private void InitializeComponent()
        {
            // Subscribe to events
            _items.ListChanged += Items_ListChanged;
        }
        #endregion

        #region Public Properties
        
        [Browsable(true)]
        [Category("Data")]
        [Description("The list of items.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        public BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                    _items.ListChanged -= Items_ListChanged;
                
                _items = value ?? new BindingList<SimpleItem>();
                _items.ListChanged += Items_ListChanged;
                
                RefreshHitAreas();
                Invalidate();
            }
        }

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

        // Add more properties as needed
        #endregion

        #region Protected Methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RefreshHitAreas();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Private Methods
        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            RefreshHitAreas();
            Invalidate();
        }

        /// <summary>
        /// Partial method for animation support
        /// </summary>
        partial void OnStateChanging(bool newValue);
        #endregion

        #region Public Methods
        // Add public API methods
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeAnimation(); // If animation exists
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
```

---

### Step 2: Create Style Enum

**File:** `ControlNameStyle.cs`

```csharp
namespace TheTechIdea.Beep.Winform.Controls.YourNamespace
{
    /// <summary>
    /// Visual styles for ControlName
    /// </summary>
    public enum ControlNameStyle
    {
        /// <summary>
        /// Material Design 3 style
        /// </summary>
        Material3,

        /// <summary>
        /// iOS 15+ style
        /// </summary>
        iOS15,

        /// <summary>
        /// Ant Design style
        /// </summary>
        AntDesign,

        /// <summary>
        /// Microsoft Fluent 2 style
        /// </summary>
        Fluent2,

        /// <summary>
        /// Material You (dynamic colors)
        /// </summary>
        MaterialYou,

        /// <summary>
        /// Windows 11 Mica style
        /// </summary>
        Windows11Mica,

        /// <summary>
        /// macOS Big Sur style
        /// </summary>
        MacOSBigSur,

        /// <summary>
        /// Chakra UI inspired
        /// </summary>
        ChakraUI,

        /// <summary>
        /// Tailwind CSS inspired
        /// </summary>
        TailwindCard,

        /// <summary>
        /// Notion minimal style
        /// </summary>
        NotionMinimal,

        /// <summary>
        /// Ultra-minimal style
        /// </summary>
        Minimal,

        /// <summary>
        /// Vercel clean monochrome
        /// </summary>
        VercelClean,

        /// <summary>
        /// Stripe dashboard style
        /// </summary>
        StripeDashboard,

        /// <summary>
        /// Dark with neon glow
        /// </summary>
        DarkGlow,

        /// <summary>
        /// Discord-inspired style
        /// </summary>
        DiscordStyle,

        /// <summary>
        /// Modern gradient style
        /// </summary>
        GradientModern
    }
}
```

---

### Step 3: Create Painter Interface

**File:** `Painters/IControlNamePainter.cs`

```csharp
using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace.Painters
{
    /// <summary>
    /// Strategy interface for painting ControlName in different visual styles.
    /// Implementations should only draw; no state should be mutated.
    /// </summary>
    public interface IControlNamePainter
    {
        /// <summary>
        /// Draw the complete control surface for the given bounds.
        /// </summary>
        void Draw(ControlName control, Graphics g, Rectangle bounds);

        /// <summary>
        /// Draw selection indicator for an item
        /// </summary>
        void DrawSelection(ControlName control, Graphics g, Rectangle selectedRect);

        /// <summary>
        /// Draw hover effect for an item
        /// </summary>
        void DrawHover(ControlName control, Graphics g, Rectangle hoverRect);

        /// <summary>
        /// Register hit areas for interactive elements
        /// </summary>
        void UpdateHitAreas(ControlName control, Rectangle bounds, Action<string, Rectangle, Action> registerHitArea);

        /// <summary>
        /// Friendly name for diagnostics and designer drop-downs.
        /// </summary>
        string Name { get; }
    }
}
```

---

### Step 4: Create Base Painter with Helpers

**File:** `Painters/BaseControlNamePainter.cs`

```csharp
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace.Painters
{
    /// <summary>
    /// Base class with helpers used by concrete painters.
    /// </summary>
    public abstract class BaseControlNamePainter : IControlNamePainter
    {
        // Reusable ImagePainter instance - DO NOT create new instances in loops!
        private static readonly ImagePainter _sharedImagePainter = new ImagePainter();
        
        public abstract string Name { get; }
        public abstract void Draw(ControlName control, Graphics g, Rectangle bounds);

        public virtual void DrawSelection(ControlName control, Graphics g, Rectangle selectedRect)
        {
            // Default implementation
            using var path = CreateRoundedPath(selectedRect, 10);
            using var br = new SolidBrush(Color.FromArgb(28, control.AccentColor));
            using var pen = new Pen(Color.FromArgb(120, control.AccentColor), 1f) { Alignment = PenAlignment.Inset };
            g.FillPath(br, path);
            g.DrawPath(pen, path);
        }

        public virtual void DrawHover(ControlName control, Graphics g, Rectangle hoverRect)
        {
            // Default implementation
            using var path = CreateRoundedPath(hoverRect, 8);
            using var br = new SolidBrush(Color.FromArgb(15, control.AccentColor));
            g.FillPath(br, path);
        }

        public virtual void UpdateHitAreas(ControlName control, Rectangle bounds, Action<string, Rectangle, Action> registerHitArea)
        {
            // Default: register hit areas for each item
            if (control.Items == null) return;

            int itemHeight = 44;
            int currentY = bounds.Top + 8;

            for (int i = 0; i < control.Items.Count; i++)
            {
                var item = control.Items[i];
                var itemRect = new Rectangle(bounds.Left + 4, currentY, bounds.Width - 8, itemHeight);
                int index = i; // Capture for lambda
                registerHitArea($"Item_{i}", itemRect, () => control.SelectItemByIndex(index));
                currentY += itemHeight + 4;
            }
        }

        #region Helper Methods

        /// <summary>
        /// Helper to draw icon using shared ImagePainter
        /// </summary>
        protected virtual void DrawIcon(ControlName control, Graphics g, SimpleItem item, Rectangle iconRect)
        {
            try
            {
                _sharedImagePainter.ImagePath = item.ImagePath;
                
                if (control.CurrentTheme != null)
                {
                    _sharedImagePainter.CurrentTheme = control.CurrentTheme;
                    _sharedImagePainter.ApplyThemeOnImage = true;
                }
                else
                {
                    _sharedImagePainter.ApplyThemeOnImage = false;
                }
                
                _sharedImagePainter.DrawImage(g, iconRect);
            }
            catch
            {
                // Fallback
                using (var pen = new Pen(Color.Gray, 1f))
                {
                    g.DrawRectangle(pen, iconRect);
                }
            }
        }

        protected static void FillRoundedRect(Graphics g, Rectangle rect, int radius, Color color)
        {
            using var path = CreateRoundedPath(rect, radius);
            using var br = new SolidBrush(color);
            g.FillPath(br, path);
        }

        protected static void StrokeRoundedRect(Graphics g, Rectangle rect, int radius, Color color, float width = 1f)
        {
            using var path = CreateRoundedPath(rect, radius);
            using var pen = new Pen(color, width) { Alignment = PenAlignment.Inset };
            g.DrawPath(pen, path);
        }

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

        #endregion
    }
}
```

---

### Step 5: Create Painter Management Partial Class

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
        private int _hoveredItemIndex = -1;
        private Dictionary<string, (Rectangle rect, Action action)> _hitAreas = new Dictionary<string, (Rectangle, Action)>();
        #endregion

        #region Painter Properties
        internal int HoveredItemIndex => _hoveredItemIndex;
        #endregion

        #region Painter Management
        private void InitializePainter()
        {
            _currentPainter = _style switch
            {
                ControlNameStyle.Material3 => new Material3Painter(),
                ControlNameStyle.iOS15 => new iOS15Painter(),
                ControlNameStyle.AntDesign => new AntDesignPainter(),
                ControlNameStyle.Fluent2 => new Fluent2Painter(),
                ControlNameStyle.MaterialYou => new MaterialYouPainter(),
                ControlNameStyle.Windows11Mica => new Windows11MicaPainter(),
                ControlNameStyle.MacOSBigSur => new MacOSBigSurPainter(),
                ControlNameStyle.ChakraUI => new ChakraUIPainter(),
                ControlNameStyle.TailwindCard => new TailwindCardPainter(),
                ControlNameStyle.NotionMinimal => new NotionMinimalPainter(),
                ControlNameStyle.Minimal => new MinimalPainter(),
                ControlNameStyle.VercelClean => new VercelCleanPainter(),
                ControlNameStyle.StripeDashboard => new StripeDashboardPainter(),
                ControlNameStyle.DarkGlow => new DarkGlowPainter(),
                ControlNameStyle.DiscordStyle => new DiscordStylePainter(),
                ControlNameStyle.GradientModern => new GradientModernPainter(),
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
                _currentPainter.UpdateHitAreas(this, ClientRectangle, (name, rect, action) =>
                {
                    _hitAreas[name] = (rect, action);
                });
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
                        Cursor = System.Windows.Forms.Cursors.Hand;
                        break;
                    }
                }
            }

            if (_hoveredItemIndex < 0)
            {
                Cursor = System.Windows.Forms.Cursors.Default;
            }

            if (previousHover != _hoveredItemIndex)
            {
                Invalidate();
            }
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
                Cursor = System.Windows.Forms.Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            HandleHitAreaClick(e.Location);
        }
        #endregion

        #region Internal Methods
        internal void SelectItemByIndex(int index)
        {
            if (_items != null && index >= 0 && index < _items.Count)
            {
                _selectedItem = _items[index];
                ItemClicked?.Invoke(_selectedItem);
                Invalidate();
            }
        }
        #endregion
    }
}
```

---

### Step 6: Create Drawing Partial Class

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

            // Use painter to draw
            _currentPainter?.Draw(this, g, ClientRectangle);
        }
    }
}
```

---

### Step 7: Create Animation Partial Class (Optional)

**File:** `ControlName.Animation.cs`

```csharp
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace
{
    /// <summary>
    /// ControlName - Animation functionality (Partial Class)
    /// </summary>
    public partial class ControlName
    {
        #region Animation Fields
        private Timer _animationTimer;
        private DateTime _animationStartTime;
        private int _animationDurationMs = 200;
        private int _animStartValue;
        private int _animTargetValue;
        private bool _isAnimating = false;
        private bool _enableAnimation = true;
        private AnimationEasing _easing = AnimationEasing.EaseOutCubic;
        #endregion

        #region Animation Events
        public event Action<bool> AnimationStarted;
        public event Action<bool> AnimationCompleted;
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

        [Browsable(true)]
        [Category("Animation")]
        [Description("Easing function for animation.")]
        [DefaultValue(AnimationEasing.EaseOutCubic)]
        public AnimationEasing Easing
        {
            get => _easing;
            set => _easing = value;
        }

        [Browsable(false)]
        public bool IsAnimating => _isAnimating;
        #endregion

        #region Animation Methods
        private void StartAnimation(bool newValue)
        {
            if (_isAnimating && _animationTimer != null)
            {
                _animationTimer.Stop();
            }

            _animStartValue = CurrentValue;
            _animTargetValue = newValue ? TargetValue : DefaultValue;
            _animationStartTime = DateTime.Now;
            _isAnimating = true;

            if (_animationTimer == null)
            {
                _animationTimer = new Timer();
                _animationTimer.Interval = 16; // ~60 FPS
                _animationTimer.Tick += AnimationTimer_Tick;
            }

            AnimationStarted?.Invoke(newValue);
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - _animationStartTime).TotalMilliseconds;
            var progress = Math.Min(1.0, elapsed / _animationDurationMs);
            var easedProgress = ApplyEasing(progress);

            // Update animated value
            var currentValue = _animStartValue + (int)((_animTargetValue - _animStartValue) * easedProgress);
            // Apply currentValue

            if (progress >= 1.0)
            {
                _animationTimer.Stop();
                _isAnimating = false;
                OnAnimationCompleted();
            }

            Invalidate();
        }

        private void OnAnimationCompleted()
        {
            AnimationCompleted?.Invoke(true);
            Invalidate();
        }

        private double ApplyEasing(double progress)
        {
            switch (_easing)
            {
                case AnimationEasing.Linear:
                    return progress;
                case AnimationEasing.EaseOut:
                    return 1 - Math.Pow(1 - progress, 2);
                case AnimationEasing.EaseIn:
                    return Math.Pow(progress, 2);
                case AnimationEasing.EaseOutCubic:
                    return 1 - Math.Pow(1 - progress, 3);
                case AnimationEasing.EaseInCubic:
                    return Math.Pow(progress, 3);
                case AnimationEasing.EaseInOutCubic:
                    return progress < 0.5
                        ? 4 * Math.Pow(progress, 3)
                        : 1 - Math.Pow(-2 * progress + 2, 3) / 2;
                default:
                    return progress;
            }
        }

        partial void OnStateChanging(bool newValue)
        {
            if (_enableAnimation && _animationDurationMs > 0)
            {
                StartAnimation(newValue);
            }
            else
            {
                // Instant change
                Invalidate();
            }
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

---

## Painter Architecture

### Painter Responsibilities

✅ **DO:**
- Draw visual elements (background, items, decorations)
- Implement visual style (colors, shapes, effects)
- Register hit areas for clickable regions
- Use theme colors when UseThemeColors = true
- Preserve unique style identity when UseThemeColors = false
- **EACH painter implements its OWN complete Draw() method**
- **Each painter has its OWN unique way of drawing items/elements**

❌ **DON'T:**
- Maintain state
- Handle mouse events directly
- Create new ImagePainter instances (use shared)
- Modify control properties
- Perform business logic
- **NEVER assume all painters draw items the same way**
- **NEVER force painters to use a single DrawItems() implementation**

### Critical Painter Pattern Rules

⚠️ **IMPORTANT:** Each style painter MUST have its OWN complete implementation:

1. **Each painter's Draw() method is unique** - Material3 draws differently than iOS15, which draws differently than Discord
2. **Base class provides HELPERS only** - Methods like `DrawMenuItems()`, `DrawMenuItem()`, `DrawMenuItemIcon()` are OPTIONAL helpers
3. **Painters call helpers BY CHOICE** - A painter can use helpers OR implement everything custom
4. **No shared drawing logic** - Each style defines how to layout and draw items
5. **One class per file** - NEVER put multiple painter classes in one file

### UseThemeColors Pattern

**CRITICAL:** Every painter MUST implement this pattern:

```csharp
public override void Draw(ControlName control, Graphics g, Rectangle bounds)
{
    // Check UseThemeColors setting
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
        // Material3 Example:
        backgroundColor = Color.FromArgb(255, 251, 254);  // MD3 Surface
        foreColor = Color.FromArgb(28, 27, 31);           // MD3 On Surface
        accentColor = Color.FromArgb(103, 80, 164);       // MD3 Primary
    }

    // Draw using selected colors
    using var bgBrush = new SolidBrush(backgroundColor);
    g.FillRectangle(bgBrush, bounds);
    
    // NOW DRAW ITEMS - Each painter implements this UNIQUELY!
    // Example: Call helper OR implement custom
    DrawMenuItems(control, g, bounds, hoveredIndex, selectedItem);
}
```

### Complete Painter Implementation Example

**File:** `Material3SideMenuPainter.cs` (One class per file!)

```csharp
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    /// <summary>
    /// Material3/Tonal surface painter with elevation and keyline.
    /// </summary>
    public sealed class Material3SideMenuPainter : BaseSideMenuPainter
    {
        public override string Name => "Material3";

        public override void Draw(BeepSideMenu menu, Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Check theme setting
            bool useThemeColors = menu.UseThemeColors;
            Color surface, primary, onSurface;

            if (useThemeColors && menu.CurrentTheme != null)
            {
                surface = menu.CurrentTheme.SideMenuBackColor;
                primary = menu.CurrentTheme.SideMenuSelectedBackColor;
                onSurface = menu.CurrentTheme.SideMenuForeColor;
            }
            else
            {
                // Preserve Material Design 3 identity
                surface = Color.FromArgb(255, 251, 254);
                primary = Color.FromArgb(103, 80, 164);
                onSurface = Color.FromArgb(28, 27, 31);
            }

            // 2. Draw background (unique to Material3)
            var tonal = Color.FromArgb(20, primary);
            using var bg = new SolidBrush(Blend(surface, tonal));
            g.FillRectangle(bg, bounds);

            // 3. Draw elevation shadow (unique to Material3)
            if (menu.EnableRailShadow)
            {
                using var shadow = new LinearGradientBrush(
                    new Rectangle(bounds.Right - 12, bounds.Top, 12, bounds.Height),
                    Color.FromArgb(60, 0, 0, 0), Color.Transparent, 
                    LinearGradientMode.Horizontal);
                g.FillRectangle(shadow, new Rectangle(bounds.Right - 12, bounds.Top, 12, bounds.Height));
            }

            // 4. Draw keyline (unique to Material3)
            using var keyline = new Pen(Color.FromArgb(50, onSurface), 1f);
            g.DrawLine(keyline, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);

            // 5. Draw all menu items - THIS IS WHERE PAINTER CHOOSES HOW
            // Option A: Use base class helper (recommended for consistency)
            int? hoveredIndex = menu.HoveredItemIndex >= 0 ? menu.HoveredItemIndex : (int?)null;
            SimpleItem selectedItem = menu.SelectedItem;
            DrawMenuItems(menu, g, bounds, hoveredIndex, selectedItem);
            
            // Option B: Implement completely custom drawing
            // foreach (var item in menu.Items) { /* custom draw */ }
        }

        public override void DrawSelection(BeepSideMenu menu, Graphics g, Rectangle selectedRect, bool isCollapsed)
        {
            // Material3 specific selection style
            bool useThemeColors = menu.UseThemeColors;
            Color primary = useThemeColors && menu.CurrentTheme != null
                ? menu.CurrentTheme.SideMenuSelectedBackColor
                : Color.FromArgb(103, 80, 164);

            int radius = isCollapsed ? selectedRect.Height / 2 : 12;
            using var path = CreateRoundedPath(selectedRect, radius);
            using var br = new SolidBrush(Color.FromArgb(60, primary));
            using var pen = new Pen(Color.FromArgb(160, primary), 1f);
            g.FillPath(br, path);
            g.DrawPath(pen, path);
        }
        
        private static Color Blend(Color a, Color b) { /* blend logic */ }
    }
}
```

### Example: Painter WITHOUT Using Helpers

```csharp
public override void Draw(BeepSideMenu menu, Graphics g, Rectangle bounds)
{
    // 1. Draw background unique to this style
    g.FillRectangle(myBrush, bounds);
    
    // 2. Draw items COMPLETELY CUSTOM - no helpers!
    int y = bounds.Top + 10;
    foreach (var item in menu.Items)
    {
        // My unique way of drawing items for THIS style
        var itemRect = new Rectangle(bounds.X + 5, y, bounds.Width - 10, 40);
        
        // Custom item background
        g.FillEllipse(mySpecialBrush, itemRect); // Circles instead of rectangles!
        
        // Custom icon placement
        DrawMyCustomIcon(g, item, new Point(itemRect.X + 10, itemRect.Y + 10));
        
        // Custom text styling
        DrawMyCustomText(g, item, new Point(itemRect.X + 50, itemRect.Y + 15));
        
        y += 45;
    }
}
```

### Key Takeaway

⚠️ **CRITICAL RULE:**
- Base class helpers are OPTIONAL
- Each painter OWNS its Draw() method
- Painters can share helpers OR go completely custom
- NEVER assume all painters must use the same drawing approach

---

## Animation Implementation

### Animation Principles

1. **Separate Partial Class** - Keep animation logic isolated
2. **60 FPS Target** - 16ms timer interval
3. **Easing Functions** - Smooth, natural motion
4. **Enable/Disable** - Support instant mode
5. **Proper Disposal** - Clean up timers

### Standard Animation Duration

- **Fast:** 150ms (quick interactions)
- **Normal:** 200ms (default, matches Discord/VS Code)
- **Slow:** 300ms (dramatic effects)

### Recommended Easing

- **EaseOutCubic** - Best for most animations (smooth deceleration)
- **EaseInOutCubic** - For reversible animations
- **Linear** - For progress indicators

---

## Theme Integration

### Theme Color Properties

```csharp
// Use these from IBeepTheme:
CurrentTheme.BackColor
CurrentTheme.ForeColor
CurrentTheme.AccentColor
CurrentTheme.PrimaryColor
CurrentTheme.SecondaryColor
CurrentTheme.BorderColor
CurrentTheme.DisabledForeColor
CurrentTheme.SelectedBackColor
CurrentTheme.SelectedForeColor
CurrentTheme.HoverBackColor
CurrentTheme.HoverForeColor

// For specific controls:
CurrentTheme.MenuItemBackColor
CurrentTheme.MenuItemForeColor
CurrentTheme.SideMenuBackColor
CurrentTheme.SideMenuForeColor
// ... etc
```

### Theme Property Pattern

```csharp
[Browsable(true)]
[Category("Appearance")]
[Description("Use theme colors instead of custom colors.")]
[DefaultValue(true)]
public bool UseThemeColors { get; set; } = true;

[Browsable(true)]
[Category("Appearance")]
[Description("Custom accent color (used when UseThemeColors = false).")]
public Color AccentColor { get; set; } = Color.FromArgb(0, 120, 215);
```

---

## Hit Testing Pattern

### Standard Implementation

```csharp
public virtual void UpdateHitAreas(ControlName control, Rectangle bounds, Action<string, Rectangle, Action> registerHitArea)
{
    if (control.Items == null) return;

    int itemHeight = 44;
    int currentY = bounds.Top + 8;

    for (int i = 0; i < control.Items.Count; i++)
    {
        var item = control.Items[i];
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

---

## Helper Classes

### Rendering Helper Pattern

**File:** `Helpers/ControlNameRenderingHelper.cs`

```csharp
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.YourNamespace.Helpers
{
    public static class ControlNameRenderingHelper
    {
        public static void DrawItem(
            Graphics g, SimpleItem item, Rectangle rect, 
            bool isHovered, bool isSelected, 
            Color backColor, Color foreColor, Font font)
        {
            // Common rendering logic
        }

        public static void DrawIcon(
            Graphics g, string iconPath, Rectangle iconRect,
            IBeepTheme theme)
        {
            // Icon rendering logic
        }

        // More helper methods
    }
}
```

---

## Testing Requirements

### Unit Tests Required

✅ **Functionality Tests:**
- [ ] Control instantiation
- [ ] Property changes trigger repaints
- [ ] Events fire correctly
- [ ] Items collection management

✅ **Painter Tests:**
- [ ] All 16 painters compile
- [ ] All painters draw without errors
- [ ] UseThemeColors = true uses theme colors
- [ ] UseThemeColors = false uses style colors
- [ ] Hit testing works for all painters

✅ **Animation Tests:**
- [ ] Animation completes successfully
- [ ] All easing functions work
- [ ] EnableAnimation = false works (instant mode)
- [ ] Disposal cleans up timers
- [ ] Rapid state changes don't crash

✅ **Theme Tests:**
- [ ] Light theme applies correctly
- [ ] Dark theme applies correctly
- [ ] Theme changes update immediately
- [ ] Custom accent color works

### Manual Testing Checklist

- [ ] Test in Designer (drag/drop, property grid)
- [ ] Test at different DPI settings
- [ ] Test with different font sizes
- [ ] Test hover effects
- [ ] Test click detection
- [ ] Test keyboard navigation
- [ ] Test with empty items list
- [ ] Test with large items list (100+)
- [ ] Test animation smoothness
- [ ] Test all 16 visual styles

---

## Complete Example: BeepSideBar

**Reference Implementation:** See `TheTechIdea.Beep.Winform.Controls.SideBar.BeepSideBar`

This control demonstrates perfect implementation of all patterns:

✅ **File Structure:**
```
SideBar/
├── BeepSideBar.cs                      # Main class
├── BeepSideBar.Painters.cs             # Painter management
├── BeepSideBar.Drawing.cs              # Paint logic
├── BeepSideBar.Animation.cs            # Animation (NEW!)
├── Painters/
│   ├── ISideMenuPainter.cs
│   ├── BaseSideMenuPainter.cs
│   ├── Material3SideMenuPainter.cs
│   ├── iOS15SideMenuPainter.cs
│   └── ... (16 painters total)
└── SideMenuStyle.cs
```

✅ **Features:**
- Complete painter architecture (16 styles)
- Smooth sliding animation (200ms, ease-out cubic)
- Full theme integration with UseThemeColors
- Hit testing for all menu items
- Hover and selection effects
- Clean partial class separation

**Use BeepSideBar as the gold standard for new controls!**

---

## Common Pitfalls to Avoid

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

2. **Don't hardcode colors in painters**
   ```csharp
   // BAD
   g.FillRectangle(Brushes.Blue, rect); // ❌ Ignores themes!
   
   // GOOD
   Color blue = useThemeColors ? theme.AccentColor : Color.Blue;
   using var brush = new SolidBrush(blue);
   g.FillRectangle(brush, rect);
   ```

3. **Don't put all code in one file**
   ```csharp
   // BAD - 3000 line single file ❌
   
   // GOOD - Organized partial classes ✅
   ```

4. **Don't forget lambda capture**
   ```csharp
   // BAD
   for (int i = 0; i < count; i++)
   {
       registerHitArea($"Item_{i}", rect, () => Select(i)); // ❌ All lambdas reference last i!
   }
   
   // GOOD
   for (int i = 0; i < count; i++)
   {
       int index = i; // ✅ Capture correctly
       registerHitArea($"Item_{index}", rect, () => Select(index));
   }
   ```

5. **Don't forget to dispose timers**
   ```csharp
   protected override void Dispose(bool disposing)
   {
       if (disposing)
       {
           DisposeAnimation(); // ✅ Clean up!
       }
       base.Dispose(disposing);
   }
   ```

---

## Quick Reference Checklist

When creating a new control, verify:

- [ ] Main class inherits from BaseControl
- [ ] Partial classes for Painters, Drawing, Animation
- [ ] Style enum with 16 options
- [ ] IPainter interface defined
- [ ] BasePainter with helper methods
- [ ] 16 concrete painter implementations
- [ ] All painters implement UseThemeColors pattern
- [ ] Hit testing via UpdateHitAreas
- [ ] Mouse events in Painters partial class
- [ ] Animation in separate partial class (if needed)
- [ ] Proper Dispose pattern
- [ ] Designer attributes (Browsable, Category, Description, DefaultValue)
- [ ] ToolboxItem, ToolboxBitmap, DisplayName attributes
- [ ] No hardcoded colors (use theme or style-specific)
- [ ] Shared ImagePainter instance
- [ ] Lambda capture done correctly
- [ ] RefreshHitAreas on layout changes
- [ ] SmoothingMode.AntiAlias for graphics
- [ ] DoubleBuffering enabled
- [ ] No errors in Designer
- [ ] All tests passing

---

## Version History

**v2.0 (2025-10-03)**
- Added animation architecture (partial class pattern)
- Added UseThemeColors enforcement
- Added BeepSideBar as reference implementation
- Added hit testing pattern details
- Added 16 standard painter styles
- Added comprehensive examples

**v1.0 (2024)**
- Initial painter architecture
- Basic theme integration
- File structure standards

---

## Support

For questions or clarifications:
1. Review BeepSideBar implementation
2. Check painter_hittesting_audit.md
3. Check beepsidebar_animation_implementation.md
4. Follow patterns exactly as documented

**Remember:** Consistency is key. All controls should follow these patterns exactly!
