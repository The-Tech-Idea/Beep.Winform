# BeepNavigationWidget Theme Instructions

## Overview
The BeepNavigationWidget ApplyTheme function should properly apply navigation-specific theme properties to ensure consistent styling for menus, breadcrumbs, tabs, and navigation elements.

## Current ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    InitializePainter();
    Invalidate();
}
```

## Required Theme Properties to Apply

### Button/Navigation Colors
- `ButtonBackColor` - Background for navigation items
- `ButtonForeColor` - Text color for navigation items
- `ButtonBorderColor` - Border color for navigation items
- `ButtonHoverBackColor` - Background for hovered navigation items
- `ButtonHoverForeColor` - Text color for hovered navigation items
- `ButtonSelectedBackColor` - Background for active/selected navigation items
- `ButtonSelectedForeColor` - Text color for active navigation items
- `ButtonSelectedBorderColor` - Border color for active navigation items

### Typography Styles
- `ButtonFont` - Font for navigation text
- `ButtonHoverFont` - Font for hovered navigation text
- `ButtonSelectedFont` - Font for selected navigation text

### Panel/Surface Colors
- `PanelBackColor` - Background for navigation panels
- `SurfaceColor` - Surface color for navigation areas
- `BorderColor` - Border color for navigation sections

### Accent Colors
- `AccentColor` - Color for highlights and active states
- `PrimaryColor` - Primary navigation color
- `SecondaryColor` - Secondary navigation color

### General Theme Colors
- `BackColor` - Widget background
- `ForeColor` - Default text color
- `HighlightBackColor` - Background for highlights

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply navigation-specific theme colors
    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    // Update navigation item colors
    _navItemBackColor = _currentTheme.ButtonBackColor;
    _navItemForeColor = _currentTheme.ButtonForeColor;
    _navItemBorderColor = _currentTheme.ButtonBorderColor;
    _navItemHoverBackColor = _currentTheme.ButtonHoverBackColor;
    _navItemHoverForeColor = _currentTheme.ButtonHoverForeColor;
    _navItemSelectedBackColor = _currentTheme.ButtonSelectedBackColor;
    _navItemSelectedForeColor = _currentTheme.ButtonSelectedForeColor;
    
    // Update panel and surface colors
    _panelBackColor = _currentTheme.PanelBackColor;
    _surfaceColor = _currentTheme.SurfaceColor;
    _borderColor = _currentTheme.BorderColor;
    
    // Update accent colors
    _accentColor = _currentTheme.AccentColor;
    _primaryColor = _currentTheme.PrimaryColor;
    _highlightBackColor = _currentTheme.HighlightBackColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Keep BackColor and ForeColor as generic theme colors
2. Add navigation item color properties:
   - `_navItemBackColor` = `ButtonBackColor`
   - `_navItemForeColor` = `ButtonForeColor`
   - `_navItemBorderColor` = `ButtonBorderColor`
   - `_navItemHoverBackColor` = `ButtonHoverBackColor`
   - `_navItemHoverForeColor` = `ButtonHoverForeColor`
   - `_navItemSelectedBackColor` = `ButtonSelectedBackColor`
   - `_navItemSelectedForeColor` = `ButtonSelectedForeColor`
3. Add panel and layout colors:
   - `_panelBackColor` = `PanelBackColor`
   - `_surfaceColor` = `SurfaceColor`
   - `_borderColor` = `BorderColor`
4. Add accent and highlight colors:
   - `_accentColor` = `AccentColor`
   - `_primaryColor` = `PrimaryColor`
   - `_highlightBackColor` = `HighlightBackColor`
5. Ensure InitializePainter() is called to refresh painter with new theme
6. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepNavigationWidget may need additional properties for navigation styling:
- Navigation item colors: `_navItemBackColor`, `_navItemForeColor`, `_navItemBorderColor`, etc.
- Layout colors: `_panelBackColor`, `_surfaceColor`, `_borderColor`
- Accent colors: `_accentColor`, `_primaryColor`, `_highlightBackColor`

## Testing
- Verify navigation items use Button colors
- Verify hovered navigation items use ButtonHover colors
- Verify selected/active navigation items use ButtonSelected colors
- Verify navigation panels use PanelBackColor
- Verify navigation surfaces use SurfaceColor
- Verify borders use BorderColor
- Verify highlights use HighlightBackColor and AccentColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepNavigationWidget.theme.instructions.md