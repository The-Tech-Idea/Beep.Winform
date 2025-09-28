# BeepMetricWidget Theme Instructions

## Overview
The BeepMetricWidget ApplyTheme function should properly apply metric/KPI-specific theme properties to ensure consistent styling for displaying key metrics, progress indicators, and performance data.

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

### Card-Specific Colors (Metrics are often card-based)
- `CardBackColor` - Background color for metric cards
- `CardTitleForeColor` - Color for metric titles
- `CardTextForeColor` - Color for metric values and labels
- `CardSubTitleForeColor` - Color for metric subtitles/descriptions

### Typography Styles
- `CardTitleFont` - Font for metric titles
- `CardSubTitleFont` - Font for metric subtitles
- `CardparagraphStyle` - Font for metric descriptions

### Progress/Status Colors
- `SuccessColor` - Color for positive metrics
- `WarningColor` - Color for warning metrics
- `ErrorColor` - Color for negative/error metrics
- `AccentColor` - Color for highlights and progress bars

### General Theme Colors
- `BackColor` - Widget background (fallback)
- `ForeColor` - Default text color (fallback)
- `BorderColor` - Border color
- `PrimaryColor` - Primary accent color

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply card-specific theme colors for metric display
    BackColor = _currentTheme.CardBackColor;
    ForeColor = _currentTheme.CardTextForeColor;
    
    // Update accent color for progress bars and highlights
    _accentColor = _currentTheme.AccentColor;
    
    // Update status colors for different metric states
    _successColor = _currentTheme.SuccessColor;
    _warningColor = _currentTheme.WarningColor;
    _errorColor = _currentTheme.ErrorColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Update BackColor to use `CardBackColor` instead of generic `BackColor`
2. Update ForeColor to use `CardTextForeColor` instead of generic `ForeColor`
3. Add `_accentColor` property and set to theme's `AccentColor`
4. Add status color properties and set to theme's `SuccessColor`, `WarningColor`, `ErrorColor`
5. Ensure InitializePainter() is called to refresh painter with new theme
6. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepMetricWidget may need additional color properties for different metric states:
- `_successColor` for positive metrics
- `_warningColor` for warning metrics  
- `_errorColor` for negative/error metrics

## Testing
- Verify metric card backgrounds use CardBackColor
- Verify metric titles use CardTitleForeColor
- Verify metric values use CardTextForeColor
- Verify progress bars and highlights use AccentColor
- Verify status indicators use appropriate Success/Warning/Error colors
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepMetricWidget.theme.instructions.md