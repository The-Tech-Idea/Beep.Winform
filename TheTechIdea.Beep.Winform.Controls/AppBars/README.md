# Beep AppBars - Complete Documentation

## Overview

The AppBars folder contains several specialized header/toolbar controls for Beep.Winform applications:

1. **BeepAppBar** - Feature-rich traditional application bar with logo, search, notifications, profile
2. **BeepMaterial3AppBar** - Material Design 3 compliant top app bar
3. **BeepWebHeaderAppBar** - Modern website-style header with horizontal tab navigation ✨ NEW

## Control Comparison

| Feature | BeepAppBar | Material3AppBar | WebHeaderAppBar |
|---------|-----------|-----------------|-----------------|
| **Purpose** | Traditional app toolbar | Material Design 3 compliance | Modern web headers |
| **Layout Style** | Horizontal toolbar | Single-row MD3 | Horizontal tabs + buttons |
| **Logo Support** | ✅ Yes | ✅ Yes | ✅ Yes |
| **Navigation** | Icons/menus | Navigation icon + actions | Tab navigation |
| **Search Box** | ✅ Yes | ✅ Yes | ✅ Yes |
| **Notifications** | ✅ Badge support | ❌ No | ✅ Badge-ready |
| **Profile Menu** | ✅ Yes | ❌ No | 🔶 Customizable |
| **Indicators** | N/A | N/A | 4 tab indicator styles |
| **Color Schemes** | Theme-based | Theme-based | 3 built-in + extensible |
| **Best For** | Desktop apps, rich toolbars | Material Design apps | Web-style apps, e-commerce |

## BeepAppBar

### Features
- Logo and title display
- Search box with autocomplete
- Notification button with badge
- Profile button with menu
- Theme button for theme switching
- Window control buttons (minimize, maximize, close)
- Form dragging support
- Fully customizable visibility

### Usage

```csharp
var appBar = new BeepAppBar
{
    Dock = DockStyle.Top,
    Title = "My Application",
    LogoImage = "MyApp.Logo.svg",
    ShowSearchBox = true,
    ShowProfileIcon = true
};

appBar.OnButtonClicked += (s, e) => 
{
    switch (e.ButtonName)
    {
        case "Profile":
            ShowProfileMenu();
            break;
        case "Notification":
            ShowNotifications();
            break;
    }
};

Controls.Add(appBar);
```

### Documentation
See `README_BeepAppBar.cs` comments in the source file for detailed API documentation.

---

## BeepMaterial3AppBar

### Features
- Material Design 3 compliance
- Multiple variants (Small, CenterAligned, Medium, Large)
- Navigation icon support
- Search field integration
- Action buttons (up to 3)
- Overflow menu support
- Theme integration
- Factory methods for common configurations

### Variants

```csharp
// Small variant (64dp)
var smallAppBar = new BeepMaterial3AppBar
{
    Variant = Material3AppBarVariant.Small
};

// Large variant (152dp)
var largeAppBar = new BeepMaterial3AppBar
{
    Variant = Material3AppBarVariant.Large
};
```

### Usage

```csharp
var appBar = new BeepMaterial3AppBar
{
    Dock = DockStyle.Top,
    Title = "My Application",
    NavigationIconPath = "menu-icon.svg",
    ShowSearch = true
};

// Set action buttons
appBar.SetActionButton(1, "search-icon.svg");
appBar.SetActionButton(2, "notifications-icon.svg");

// Add overflow menu
appBar.AddOverflowMenuItem("Settings", "settings-icon.svg");
appBar.AddOverflowMenuItem("Help", "help-icon.svg");

Controls.Add(appBar);
```

### Documentation
See `README_BeepMaterial3AppBar.md` for complete documentation.

---

## BeepWebHeaderAppBar ✨ NEW

### Features
- 🎨 **10+ Modern Color Schemes** - Extensive palette of contemporary designs
- 📑 **Horizontal Tab Navigation** - Clean tab-based layout
- 🎯 **4 Indicator Styles** - UnderlineSimple, UnderlineFull, PillBackground, SlidingUnderline
- 🔘 **Action Buttons** - Multiple styles (Solid, Outline, Minimal, Ghost)
- 🔍 **Search Integration** - Built-in search box
- 🎭 **Theme Support** - Full Beep theme integration
- 📐 **Responsive Layout** - Adapts to control size and DPI
- 🖼️ **Logo/Brand Support** - SVG icons via StyledImagePainter
- ⚡ **Painter-Based** - Efficient rendering architecture
- 🎬 **Animation Ready** - Framework for smooth transitions

### Architecture

The WebHeaderAppBar is built on five specialized painters:

```
BeepWebHeaderAppBar (Main Control)
├── WebHeaderColorScheme (3 color schemes)
├── WebHeaderLayoutCalculator (Responsive positioning)
├── WebHeaderTabPainter (Tab + 4 indicator styles)
├── WebHeaderActionButtonPainter (4 button styles)
└── Integration: StyledImagePainter + BeepStyling + Theme System
```

### Color Schemes

#### WebHeader1: Purple Minimal
```csharp
header.HeaderStyle = WebHeaderStyle.WebHeader1;
// Colors: Purple background, Yellow indicators
// Best for: Professional applications, modern SaaS
```

#### WebHeader2: Yellow Vibrant
```csharp
header.HeaderStyle = WebHeaderStyle.WebHeader2;
// Colors: Gold background, Purple indicators
// Best for: Bold, energetic brands
```

#### WebHeader3: Dark Professional
```csharp
header.HeaderStyle = WebHeaderStyle.WebHeader3;
// Colors: Dark background, Orange indicators
// Best for: E-commerce, sleek modern sites
```

### Usage

```csharp
var header = new BeepWebHeaderAppBar
{
    Dock = DockStyle.Top,
    HeaderStyle = WebHeaderStyle.WebHeader1,
    IndicatorStyle = TabIndicatorStyle.UnderlineFull,
    LogoImagePath = "logo.svg",
    LogoWidth = 40,
    HeaderHeight = 60
};

// Add navigation tabs
header.AddTab("Home", id: 1);
header.AddTab("Products", id: 2);
header.AddTab("Services", id: 3);
header.AddTab("Contact", id: 4);

// Add action buttons
header.AddActionButton("Search", 
    imagePath: "search-icon.svg",
    style: WebHeaderButtonStyle.Outline,
    id: 10);
header.AddActionButton("Login",
    style: WebHeaderButtonStyle.Solid,
    id: 11);

// Handle events
header.TabSelected += (s, e) =>
{
    Console.WriteLine($"Tab selected: {e.TabText}");
};

header.ActionButtonClicked += (s, e) =>
{
    Console.WriteLine($"Button clicked: {e.ButtonText}");
};

Controls.Add(header);
```

### Tab Indicator Styles

```csharp
// Centered line (½ width)
header.IndicatorStyle = TabIndicatorStyle.UnderlineSimple;

// Full width line
header.IndicatorStyle = TabIndicatorStyle.UnderlineFull;

// Rounded pill background
header.IndicatorStyle = TabIndicatorStyle.PillBackground;

// Smooth sliding animation
header.IndicatorStyle = TabIndicatorStyle.SlidingUnderline;
```

### Button Styles

```csharp
// Filled background (primary)
header.AddActionButton("Register", style: WebHeaderButtonStyle.Solid);

// Border only (secondary)
header.AddActionButton("Learn More", style: WebHeaderButtonStyle.Outline);

// Text only (tertiary)
header.AddActionButton("Help", style: WebHeaderButtonStyle.Minimal);

// Very subtle (quaternary)
header.AddActionButton("Info", style: WebHeaderButtonStyle.Ghost);
```

### Documentation
See `README_BeepWebHeaderAppBar.md` for complete API documentation.

---

## WebHeader Supporting Components

The WebHeaderAppBar is supported by several helper components:

### WebHeaderStyle.cs (240 lines)
**Enums & Types**
- `WebHeaderStyle` enum (WebHeader1, WebHeader2, WebHeader3)
- `TabIndicatorStyle` enum (4 indicator types)
- `WebHeaderButtonStyle` enum (4 button styles)
- `WebHeaderTab` class - Navigation tab data model
- `WebHeaderActionButton` class - Action button data model
- `WebHeaderConfig` class - Configuration settings
- Event args classes (TabSelectedEventArgs, ActionButtonClickedEventArgs, SearchChangedEventArgs)

### WebHeaderColorScheme.cs (280 lines)
**Color Management**
- 3 complete color schemes (WebHeader1, WebHeader2, WebHeader3)
- 17+ color properties per scheme
- State-aware helper methods (active, hover, inactive, disabled)
- Color blending and transparency utilities
- Button style awareness

### WebHeaderLayoutCalculator.cs (380 lines)
**Layout Engine**
- Responsive positioning calculations
- GraphicsPath creation for shapes
- Hit testing for mouse interaction
- DPI-aware layout
- Tab and button position calculation
- Layout validation

### WebHeaderTabPainter.cs (380 lines)
**Tab Rendering**
- Tab painting with state management
- 4 indicator style rendering
- Badge support for notifications
- Icon and text rendering
- Hover effects
- Animation support

### WebHeaderActionButtonPainter.cs (380 lines)
**Button Rendering**
- Button painting for 4 styles
- Split button support
- Button groups with separators
- Badge support
- Icon and text rendering
- Hover, pressed, disabled states

---

## Design Patterns Covered

The WebHeaderAppBar supports modern website header patterns seen in:

✅ **E-commerce** (Shoppy Store variants)
- Categories dropdown
- Search integration
- Cart button
- Multi-color variants

✅ **SaaS Applications** (Studiofok style)
- Clean, professional layout
- Navigation tabs
- User account access

✅ **Content Sites** (Blog layouts)
- Blog, Shop, Sale navigation
- Featured section
- Search capability

✅ **Enterprise Applications** (Dashboard style)
- Multiple navigation options
- Rich toolbar
- Status indicators

---

## File Structure

```
AppBars/
├── BeepAppBar.cs (2000+ lines)
├── BeepMaterial3AppBar.cs (400+ lines)
├── BeepWebHeaderAppBar.cs (700 lines) ✨ NEW
│
├── WebHeaderStyle.cs (240 lines)
├── WebHeaderColorScheme.cs (280 lines)
├── WebHeaderLayoutCalculator.cs (380 lines)
├── WebHeaderTabPainter.cs (380 lines)
├── WebHeaderActionButtonPainter.cs (380 lines)
│
├── README_BeepMaterial3AppBar.md
├── README_BeepWebHeaderAppBar.md ✨ NEW
│
├── WebHeaderAppBar_Plan.md (Design specifications)
├── WebHeaderAppBar_Progress.md (Implementation progress)
├── WebHeaderAppBar_QuickRef.md (Quick reference)
│
├── Helpers/
│   ├── BeepAppBarComponentFactory.cs
│   ├── BeepAppBarDragHelper.cs
│   ├── BeepAppBarDrawingHelper.cs
│   ├── BeepAppBarEventHelper.cs
│   ├── BeepAppBarLayoutHelper.cs
│   ├── BeepAppBarMasterHelper.cs
│   ├── BeepAppBarStateStore.cs
│   └── IBeepAppBarHost.cs
│
└── Material3AppBarFactory.cs
```

---

## Integration Guide

### Adding WebHeaderAppBar to Your Project

1. **Ensure all files are present**:
   - BeepWebHeaderAppBar.cs
   - WebHeaderStyle.cs
   - WebHeaderColorScheme.cs
   - WebHeaderLayoutCalculator.cs
   - WebHeaderTabPainter.cs
   - WebHeaderActionButtonPainter.cs

2. **Reference StyledImagePainter**:
   - Already integrated in AppBars folder
   - Automatically handles SVG painting

3. **Use in Designer or Code**:
   ```csharp
   var header = new BeepWebHeaderAppBar
   {
       Dock = DockStyle.Top,
       HeaderStyle = WebHeaderStyle.WebHeader1
   };
   Controls.Add(header);
   ```

4. **Theme Support**:
   ```csharp
   // Apply theme
   header.Theme = "DarkTheme";
   ```

---

## Performance Notes

- **Double Buffering**: Enabled for flicker-free rendering
- **Image Caching**: StyledImagePainter handles SVG caching
- **Layout Caching**: Calculated only on resize or property changes
- **Hit Testing**: O(n) where n = tabs + buttons (negligible for typical use)
- **DPI Scaling**: Automatic, no configuration needed

---

## Choosing the Right AppBar

| Use Case | Recommendation |
|----------|-----------------|
| Traditional WinForms app with toolbars | **BeepAppBar** |
| Material Design 3 compliance required | **BeepMaterial3AppBar** |
| Modern website-style header with tabs | **BeepWebHeaderAppBar** ✨ |
| E-commerce or retail app | **BeepWebHeaderAppBar** (Web style) |
| Dashboard/admin interface | **BeepAppBar** or **BeepWebHeaderAppBar** |
| Mobile-first responsive design | **BeepMaterial3AppBar** |

---

## Example Applications

### E-commerce Header (BeepWebHeaderAppBar)
```csharp
var header = new BeepWebHeaderAppBar
{
    Dock = DockStyle.Top,
    HeaderStyle = WebHeaderStyle.WebHeader3, // Dark theme
    LogoImagePath = "store-logo.svg"
};

header.AddTab("Home");
header.AddTab("Shop");
header.AddTab("Sale");
header.AddTab("Blog");
header.AddTab("Contact");

header.AddActionButton("Search", style: WebHeaderButtonStyle.Outline);
header.AddActionButton("Cart (5)", style: WebHeaderButtonStyle.Solid);
header.AddActionButton("Account", style: WebHeaderButtonStyle.Outline);
```

### SaaS Application Header (BeepAppBar)
```csharp
var appBar = new BeepAppBar
{
    Dock = DockStyle.Top,
    Title = "Dashboard",
    LogoImage = "app-logo.svg",
    ShowProfileIcon = true,
    ShowNotificationIcon = true,
    ShowThemeIcon = true
};
```

### Material Design App (BeepMaterial3AppBar)
```csharp
var appBar = new BeepMaterial3AppBar
{
    Dock = DockStyle.Top,
    Title = "Material Design App",
    Variant = Material3AppBarVariant.Small,
    NavigationIconPath = "menu-icon.svg",
    ShowSearch = true
};
```

---

## Support & Documentation

- **WebHeaderAppBar Documentation**: `README_BeepWebHeaderAppBar.md`
- **Material3AppBar Documentation**: `README_BeepMaterial3AppBar.md`
- **Implementation Plan**: `WebHeaderAppBar_Plan.md`
- **Quick Reference**: `WebHeaderAppBar_QuickRef.md`
- **Progress Notes**: `WebHeaderAppBar_Progress.md`

---

## Summary

The Beep AppBars collection provides three powerful header/toolbar controls:

| Control | Lines | Purpose | Complexity |
|---------|-------|---------|-----------|
| **BeepAppBar** | 2000+ | Rich application toolbar | Complex |
| **BeepMaterial3AppBar** | 400+ | Material Design compliance | Medium |
| **BeepWebHeaderAppBar** | 700 | Modern web headers | Medium |

**Total**: 3,100+ lines of production-quality code

---

*Beep AppBars - Professional UI Controls for WinForms Applications*
