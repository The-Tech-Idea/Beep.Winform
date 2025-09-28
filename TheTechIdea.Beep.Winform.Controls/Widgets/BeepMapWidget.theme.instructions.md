# BeepMapWidget Theme Instructions

## Overview
The BeepMapWidget ApplyTheme function should properly apply map-specific theme properties to ensure consistent styling for maps, location tracking, routes, and geographic displays.

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

### Map-Specific Colors
- `BackColor` - Background color for map area
- `SurfaceColor` - Surface color for map overlays
- `PanelBackColor` - Background for map control panels

### Geographic/Location Colors
- `AccentColor` - Color for location markers and points of interest
- `PrimaryColor` - Primary color for routes and paths
- `SecondaryColor` - Secondary color for additional map elements

### Text Colors
- `ForeColor` - Default text color for map labels
- `CardTitleForeColor` - Color for location titles
- `CardTextForeColor` - Color for location descriptions
- `OnBackgroundColor` - Text color on colored map elements

### Route/Navigation Colors
- `SuccessColor` - Color for completed routes or valid paths
- `WarningColor` - Color for warnings or cautions
- `ErrorColor` - Color for errors or blocked routes

### Interactive Elements
- `ButtonHoverBackColor` - Background for hovered map controls
- `HighlightBackColor` - Background for selected locations
- `BorderColor` - Border color for map panels and controls

### Typography Styles
- `CardTitleFont` - Font for location names
- `CardSubTitleFont` - Font for location details
- `ButtonFont` - Font for map control buttons

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply map-specific theme colors
    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    // Update map surface colors
    _mapBackColor = _currentTheme.BackColor;
    _surfaceColor = _currentTheme.SurfaceColor;
    _panelBackColor = _currentTheme.PanelBackColor;
    
    // Update location and marker colors
    _locationColor = _currentTheme.AccentColor;
    _routeColor = _currentTheme.PrimaryColor;
    _secondaryColor = _currentTheme.SecondaryColor;
    
    // Update status colors for routes/navigation
    _validRouteColor = _currentTheme.SuccessColor;
    _warningColor = _currentTheme.WarningColor;
    _errorColor = _currentTheme.ErrorColor;
    
    // Update text colors
    _titleForeColor = _currentTheme.CardTitleForeColor;
    _textForeColor = _currentTheme.CardTextForeColor;
    _labelForeColor = _currentTheme.OnBackgroundColor;
    
    // Update interactive colors
    _hoverBackColor = _currentTheme.ButtonHoverBackColor;
    _selectedBackColor = _currentTheme.HighlightBackColor;
    _borderColor = _currentTheme.BorderColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Keep BackColor and ForeColor as generic theme colors
2. Add map-specific color properties:
   - `_mapBackColor` = `BackColor`
   - `_surfaceColor` = `SurfaceColor`
   - `_panelBackColor` = `PanelBackColor`
3. Add location and route color properties:
   - `_locationColor` = `AccentColor` (markers, POIs)
   - `_routeColor` = `PrimaryColor` (routes, paths)
   - `_secondaryColor` = `SecondaryColor` (additional elements)
4. Add status colors for navigation:
   - `_validRouteColor` = `SuccessColor`
   - `_warningColor` = `WarningColor`
   - `_errorColor` = `ErrorColor`
5. Add text color properties:
   - `_titleForeColor` = `CardTitleForeColor`
   - `_textForeColor` = `CardTextForeColor`
   - `_labelForeColor` = `OnBackgroundColor`
6. Add interactive color properties:
   - `_hoverBackColor` = `ButtonHoverBackColor`
   - `_selectedBackColor` = `HighlightBackColor`
   - `_borderColor` = `BorderColor`
7. Ensure InitializePainter() is called to refresh painter with new theme
8. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepMapWidget may need additional properties for map styling:
- Map colors: `_mapBackColor`, `_surfaceColor`, `_panelBackColor`
- Location colors: `_locationColor`, `_routeColor`, `_secondaryColor`
- Status colors: `_validRouteColor`, `_warningColor`, `_errorColor`
- Text colors: `_titleForeColor`, `_textForeColor`, `_labelForeColor`
- Interactive colors: `_hoverBackColor`, `_selectedBackColor`, `_borderColor`

## Testing
- Verify map background uses BackColor
- Verify map surfaces use SurfaceColor
- Verify map panels use PanelBackColor
- Verify location markers use AccentColor
- Verify routes use PrimaryColor
- Verify valid routes use SuccessColor
- Verify warnings use WarningColor
- Verify errors use ErrorColor
- Verify location titles use CardTitleForeColor
- Verify location text uses CardTextForeColor
- Verify labels use OnBackgroundColor
- Verify hovered controls use ButtonHoverBackColor
- Verify selected locations use HighlightBackColor
- Verify borders use BorderColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepMapWidget.theme.instructions.md