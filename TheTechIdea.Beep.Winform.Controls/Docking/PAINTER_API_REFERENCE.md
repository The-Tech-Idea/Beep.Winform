# Docking Painter API Reference

## Quick Start

```csharp
// Get painter from factory
var painter = DockingPainterFactory.GetPainter("Default");

// Update colors from current theme
painter.UpdateFromTheme();

// Paint tab strip
painter.DrawTabStrip(graphics, tabStripBounds, tabs, activeTabIndex);

// Paint individual tab
painter.DrawTab(graphics, tabBounds, tab, isActive: true, isHovered: false);

// Paint panel title bar
painter.DrawPanelChrome(graphics, chromeBounds, "Panel Title", icon, isDirty: false);

// Paint splitter
painter.DrawSplitter(graphics, splitterBounds, SplitterOrientation.Vertical);

// Paint docking guide (on drag)
painter.DrawDockingGuide(graphics, guideBounds, DockPosition.Left);
```

---

## Interface: `IDockingPainter`

```csharp
namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
	public interface IDockingPainter : IDisposable
	{
		// ... (see sections below)
	}
}
```

---

## Color Properties

Theme-aware colors for docking UI elements.

### Properties

```csharp
/// <summary>Background color for panels and unselected areas.</summary>
Color BackgroundColor { get; set; }

/// <summary>Foreground color for text and primary elements.</summary>
Color ForegroundColor { get; set; }

/// <summary>Color for borders, separators, and lines.</summary>
Color BorderColor { get; set; }

/// <summary>Hover state color for interactive elements.</summary>
Color HoverColor { get; set; }

/// <summary>Active/selected element color (Beep accent blue by default).</summary>
Color SelectedColor { get; set; }

/// <summary>Disabled element color.</summary>
Color DisabledColor { get; set; }
```

### Examples

```csharp
var painter = DockingPainterFactory.GetPainter("Default");

// Read colors
Color bgColor = painter.BackgroundColor;
Color selColor = painter.SelectedColor;

// Update colors (e.g., from theme)
painter.BackgroundColor = Color.FromArgb(240, 240, 240);
painter.SelectedColor = Color.FromArgb(0, 122, 255);  // Beep accent
```

---

## Font Properties

Fonts used for UI text and labels.

### Properties

```csharp
/// <summary>Font for general UI text (buttons, labels).</summary>
Font UIFont { get; set; }

/// <summary>Font for tab labels and panel titles.</summary>
Font TabFont { get; set; }
```

### Examples

```csharp
var painter = DockingPainterFactory.GetPainter("Default");

// Default: 9pt Segoe UI Regular
var uiFont = painter.UIFont;      // System.Drawing.Font

// Change to custom font
painter.UIFont = new Font("Arial", 10f, FontStyle.Regular);
```

### Default Values

| Property | Font | Size | Style | Notes |
|----------|------|------|-------|-------|
| `UIFont` | Segoe UI | 9pt | Regular | UI elements |
| `TabFont` | Segoe UI | 9pt | Regular | Tab labels |

---

## Paint Methods

### DrawTabStrip

Renders the entire tab strip (all tabs in a row).

```csharp
void DrawTabStrip(
	Graphics graphics,              // Drawing surface
	Rectangle bounds,               // Tab strip area
	TabInfo[] tabs,                 // Array of tab descriptors
	int activeTabIndex              // Index of active tab (-1 if none)
);
```

**Features:**
- ✅ Renders multiple tabs side-by-side
- ✅ Highlights active tab with `SelectedColor`
- ✅ Applies hover effect
- ✅ High-quality antialiased rendering

**Example:**

```csharp
var tabs = new[]
{
	new TabInfo { Title = "Panel 1", CanClose = true, IsDirty = false },
	new TabInfo { Title = "Panel 2", CanClose = true, IsDirty = true },
	new TabInfo { Title = "Settings", CanClose = false, IsDirty = false }
};

painter.DrawTabStrip(graphics, new Rectangle(0, 0, 400, 30), tabs, activeIndex: 0);
```

---

### DrawTab

Renders a single tab with all decorations.

```csharp
void DrawTab(
	Graphics graphics,              // Drawing surface
	Rectangle bounds,               // Tab bounds
	TabInfo tab,                    // Tab descriptor
	bool isActive,                  // Is this the active tab?
	bool isHovered                  // Is mouse hovering over tab?
);
```

**Features:**
- ✅ Renders tab background (active/inactive/hover)
- ✅ Renders title text with ellipsis
- ✅ Shows dirty indicator (colored dot)
- ✅ Shows close button (if `CanClose` is true)
- ✅ Proper text alignment and margins

**Example:**

```csharp
var tab = new TabInfo
{
	Title = "Document.txt",
	CanClose = true,
	IsDirty = true
};

painter.DrawTab(graphics, new Rectangle(0, 0, 100, 30), tab, 
	isActive: true, isHovered: false);
```

---

### DrawPanelChrome

Renders the panel's title bar (chrome).

```csharp
void DrawPanelChrome(
	Graphics graphics,              // Drawing surface
	Rectangle bounds,               // Chrome area (typically 20-24px height)
	string title,                   // Panel title text
	Image icon,                     // Icon (16x16 typical) - nullable
	bool isDirty                    // Unsaved changes indicator
);
```

**Features:**
- ✅ Renders title bar background
- ✅ Displays icon (left side)
- ✅ Renders title text (center/left)
- ✅ Dirty indicator if needed
- ✅ Proper spacing and alignment

**Example:**

```csharp
painter.DrawPanelChrome(graphics, new Rectangle(0, 0, 400, 24),
	title: "Properties",
	icon: Properties.Resources.SettingsIcon,
	isDirty: false);
```

---

### DrawSplitter

Renders a splitter bar (vertical or horizontal).

```csharp
void DrawSplitter(
	Graphics graphics,              // Drawing surface
	Rectangle bounds,               // Splitter bounds
	SplitterOrientation orientation // Horizontal or Vertical
);
```

**Orientation Enum:**

```csharp
public enum SplitterOrientation
{
	Horizontal,     // ─────── (divides top/bottom)
	Vertical        // │││││││ (divides left/right)
}
```

**Features:**
- ✅ Renders visual splitter line
- ✅ Adapts to orientation
- ✅ Hover effect for cursor feedback

**Example:**

```csharp
// Vertical splitter between left and right panels
painter.DrawSplitter(graphics, new Rectangle(200, 0, 4, 600),
	SplitterOrientation.Vertical);

// Horizontal splitter between top and bottom panels
painter.DrawSplitter(graphics, new Rectangle(0, 300, 800, 4),
	SplitterOrientation.Horizontal);
```

---

### DrawDockingGuide

Renders a docking guide indicator (shown during drag operations).

```csharp
void DrawDockingGuide(
	Graphics graphics,              // Drawing surface
	Rectangle bounds,               // Guide area
	DockPosition position           // Where panel would dock
);
```

**DockPosition Enum:**

```csharp
public enum DockPosition
{
	Left,       // Dock to left side
	Right,      // Dock to right side
	Top,        // Dock to top
	Bottom,     // Dock to bottom
	Fill,       // Fill entire area
	Floating    // Floating window
}
```

**Features:**
- ✅ Semi-transparent overlay (visual feedback)
- ✅ Colored border indicating dock position
- ✅ Used during drag operations
- ✅ High contrast for visibility

**Example:**

```csharp
// Show guide for left-docking
painter.DrawDockingGuide(graphics, new Rectangle(0, 0, 200, 600),
	DockPosition.Left);

// Show guide for tab insertion
painter.DrawDockingGuide(graphics, new Rectangle(100, 0, 80, 30),
	DockPosition.Fill);
```

---

## Layout Helper Methods

### GetTabStripPreferredSize

Calculates preferred size for tab strip.

```csharp
Size GetTabStripPreferredSize(
	TabInfo[] tabs,                 // Array of tabs
	int availableWidth              // Width constraint
);

// Returns: Size with calculated width and height (TabStripHeight typically 30-32px)
```

**Example:**

```csharp
var tabs = new[] { tab1, tab2, tab3 };
var preferredSize = painter.GetTabStripPreferredSize(tabs, 800);
// Returns: Size(800, 30)
```

---

### GetTabAtPoint

Hit-tests a point against tabs.

```csharp
int GetTabAtPoint(
	Point point,                    // Point to test
	Rectangle bounds,               // Tab strip bounds
	TabInfo[] tabs                  // Array of tabs
);

// Returns: Tab index (0-based), or -1 if not on any tab
```

**Example:**

```csharp
int tabIndex = painter.GetTabAtPoint(mousePoint, tabStripBounds, tabs);
if (tabIndex >= 0)
{
	// Mouse is over tab at index tabIndex
	ActivateTab(tabIndex);
}
else
{
	// Mouse is not over any tab
}
```

---

### GetTabCloseButtonRect

Gets bounds of close button for a tab.

```csharp
Rectangle GetTabCloseButtonRect(
	Rectangle tabBounds,            // Tab bounds
	TabInfo tab                     // Tab descriptor
);

// Returns: Close button rectangle, or Rectangle.Empty if no close button
```

**Example:**

```csharp
if (painter.GetTabCloseButtonRect(tabBounds, tab) is var closeRect && closeRect != Rectangle.Empty)
{
	if (closeRect.Contains(mousePoint))
	{
		CloseTab(tab);
	}
}
```

---

## Theme Integration

### UpdateFromTheme

Updates painter colors and fonts from current theme.

```csharp
void UpdateFromTheme();
```

**Purpose:**
- Called when theme changes
- Reloads colors/fonts from `BeepThemesManager`
- Updates painter state

**Example:**

```csharp
// When theme changes
BeepThemesManager.OnThemeChanged += () =>
{
	painter.UpdateFromTheme();
	InvalidateAllPaints();
};
```

---

### InvalidateCache

Clears any internal rendering caches.

```csharp
void InvalidateCache();
```

**Purpose:**
- Invalidates cached brushes/pens
- Forces redraw on next paint
- Called on theme change or property update

**Example:**

```csharp
painter.BackgroundColor = Color.Blue;
painter.InvalidateCache();
// Next paint will use new color
```

---

## TabInfo Struct

Descriptor for a single tab.

```csharp
public struct TabInfo
{
	/// <summary>Unique identifier for the tab.</summary>
	public string Key { get; set; }

	/// <summary>Display title of the tab.</summary>
	public string Title { get; set; }

	/// <summary>Can user close this tab?</summary>
	public bool CanClose { get; set; }

	/// <summary>Has unsaved changes?</summary>
	public bool IsDirty { get; set; }
}
```

**Example:**

```csharp
var tab = new TabInfo
{
	Key = "editor.txt",
	Title = "editor.txt",
	CanClose = true,
	IsDirty = false
};
```

---

## Factory: DockingPainterFactory

### GetPainter

Gets painter for theme.

```csharp
static IDockingPainter GetPainter(string themeName);
```

**Example:**

```csharp
var defaultPainter = DockingPainterFactory.GetPainter("Default");
var material3Painter = DockingPainterFactory.GetPainter("Material3");
```

---

### RegisterPainter

Registers custom painter for theme.

```csharp
static void RegisterPainter(string themeName, IDockingPainter painter);
```

**Example:**

```csharp
var customPainter = new MyCustomDockingPainter();
DockingPainterFactory.RegisterPainter("MyTheme", customPainter);
```

---

### ClearCache

Invalidates painter cache.

```csharp
static void ClearCache();
```

**Example:**

```csharp
// On theme change
BeepThemesManager.OnThemeChanged += () =>
{
	DockingPainterFactory.ClearCache();
};
```

---

## Complete Example

```csharp
public class DockingPanelRenderer : Control
{
	private IDockingPainter _painter;
	private TabInfo[] _tabs;
	private int _activeTabIndex = 0;

	public DockingPanelRenderer()
	{
		_painter = DockingPainterFactory.GetPainter("Default");
		_tabs = new[] { /* ... */ };

		SetStyle(ControlStyles.AllPaintingInWmPaint |
				 ControlStyles.UserPaint |
				 ControlStyles.DoubleBuffer, true);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		e.Graphics.Clear(_painter.BackgroundColor);

		// Tab strip (top 30 pixels)
		var tabStripRect = new Rectangle(0, 0, Width, 30);
		_painter.DrawTabStrip(e.Graphics, tabStripRect, _tabs, _activeTabIndex);

		// Content area
		var contentRect = new Rectangle(0, 30, Width, Height - 30);
		_painter.DrawPanelChrome(e.Graphics, 
			new Rectangle(contentRect.X, contentRect.Y, contentRect.Width, 24),
			_tabs[_activeTabIndex].Title, null, _tabs[_activeTabIndex].IsDirty);
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		var tabStripRect = new Rectangle(0, 0, Width, 30);
		int tabIndex = _painter.GetTabAtPoint(e.Location, tabStripRect, _tabs);
		if (tabIndex >= 0)
		{
			_activeTabIndex = tabIndex;
			Invalidate();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_painter?.Dispose();
		}
		base.Dispose(disposing);
	}
}
```

---

## Performance Notes

### Caching Strategy
- Painters are cached by theme name (not recreated per paint)
- Brush/pen objects are created once and reused
- `InvalidateCache()` only called on theme change

### Graphics Quality
Applied by default:
- `SmoothingMode.AntiAlias` — Smooth edges
- `InterpolationMode.HighQualityBicubic` — Image quality
- `PixelOffsetMode.HighQuality` — Precise pixel placement
- `CompositingQuality.HighQuality` — Smooth blending

### Optimization Tips
1. Don't recreate painters; reuse cached instances
2. Call `InvalidateCache()` only when necessary
3. Use `DoubleBuffer` on rendering controls
4. Batch multiple paint operations per `OnPaint` call

---

## Future Extensions

### Custom Painters
```csharp
public class Material3DockingPainter : IDockingPainter
{
	// Custom implementation
}

DockingPainterFactory.RegisterPainter("Material3", new Material3DockingPainter());
```

### Icon Rendering
```csharp
// Consider using StyledImagePainter for icons
StyledImagePainter.Paint(g, iconBounds, iconPath, style);
```

### Animation
```csharp
// Tab hover animation
// Splitter preview during drag
// Panel activation transition
```

---

## Summary

The `IDockingPainter` interface provides a complete, high-quality rendering system for docking UI. It follows Beep's existing patterns and integrates seamlessly with the theme system.

**Key Strengths:**
- ✅ Contract-driven design
- ✅ Theme-aware colors/fonts
- ✅ High-quality graphics
- ✅ Extensible via factory
- ✅ Resource-safe with IDisposable

