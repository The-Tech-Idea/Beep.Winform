# BeepNotificationWidget Theme Instructions

## Overview
The BeepNotificationWidget ApplyTheme function should properly apply notification-specific theme properties to ensure consistent styling for alerts, messages, banners, and notification displays.

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

### Status Colors (Critical for notifications)
- `SuccessColor` - Color for success notifications
- `WarningColor` - Color for warning notifications
- `ErrorColor` - Color for error notifications
- `AccentColor` - Color for info/general notifications

### Card/Panel Colors
- `CardBackColor` - Background for notification cards
- `CardTitleForeColor` - Color for notification titles
- `CardTextForeColor` - Color for notification text
- `CardSubTitleForeColor` - Color for notification subtitles

### Typography Styles
- `CardTitleFont` - Font for notification titles
- `CardSubTitleFont` - Font for notification subtitles
- `CardparagraphStyle` - Font for notification content

### Border and Surface Colors
- `BorderColor` - Border color for notifications
- `SurfaceColor` - Surface color for notification backgrounds
- `PanelBackColor` - Background for notification panels

### General Theme Colors
- `BackColor` - Widget background
- `ForeColor` - Default text color
- `OnBackgroundColor` - Text color on colored backgrounds

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply notification-specific theme colors
    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    // Update status colors for different notification types
    _successColor = _currentTheme.SuccessColor;
    _warningColor = _currentTheme.WarningColor;
    _errorColor = _currentTheme.ErrorColor;
    _infoColor = _currentTheme.AccentColor;
    
    // Update card colors for notification display
    _cardBackColor = _currentTheme.CardBackColor;
    _cardTitleForeColor = _currentTheme.CardTitleForeColor;
    _cardTextForeColor = _currentTheme.CardTextForeColor;
    
    // Update border and surface colors
    _borderColor = _currentTheme.BorderColor;
    _surfaceColor = _currentTheme.SurfaceColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Keep BackColor and ForeColor as generic theme colors
2. Add status color properties for different notification types:
   - `_successColor` = `SuccessColor`
   - `_warningColor` = `WarningColor`
   - `_errorColor` = `ErrorColor`
   - `_infoColor` = `AccentColor`
3. Add card-specific colors for notification content:
   - `_cardBackColor` = `CardBackColor`
   - `_cardTitleForeColor` = `CardTitleForeColor`
   - `_cardTextForeColor` = `CardTextForeColor`
4. Add border and surface colors:
   - `_borderColor` = `BorderColor`
   - `_surfaceColor` = `SurfaceColor`
5. Ensure InitializePainter() is called to refresh painter with new theme
6. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepNotificationWidget may need additional properties for notification styling:
- Status colors: `_successColor`, `_warningColor`, `_errorColor`, `_infoColor`
- Card colors: `_cardBackColor`, `_cardTitleForeColor`, `_cardTextForeColor`
- Layout colors: `_borderColor`, `_surfaceColor`

## Testing
- Verify success notifications use SuccessColor
- Verify warning notifications use WarningColor
- Verify error notifications use ErrorColor
- Verify info notifications use AccentColor
- Verify notification cards use CardBackColor
- Verify notification titles use CardTitleForeColor
- Verify notification text uses CardTextForeColor
- Verify borders use BorderColor
- Verify surfaces use SurfaceColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepNotificationWidget.theme.instructions.md