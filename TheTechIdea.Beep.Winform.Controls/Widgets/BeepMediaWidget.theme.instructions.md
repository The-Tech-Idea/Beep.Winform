# BeepMediaWidget Theme Instructions

## Overview
The BeepMediaWidget ApplyTheme function should properly apply media-specific theme properties to ensure consistent styling for images, avatars, icons, and media content displays.

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

### Card/Surface Colors
- `CardBackColor` - Background for media cards
- `SurfaceColor` - Surface color for media containers
- `PanelBackColor` - Background for media panels

### Text Colors
- `CardTitleForeColor` - Color for media titles
- `CardTextForeColor` - Color for media descriptions
- `CardSubTitleForeColor` - Color for media subtitles/metadata

### Typography Styles
- `CardTitleFont` - Font for media titles
- `CardSubTitleFont` - Font for media subtitles
- `CardparagraphStyle` - Font for media descriptions

### Border and Accent Colors
- `BorderColor` - Border color for media containers
- `AccentColor` - Color for highlights and overlays
- `PrimaryColor` - Primary accent color
- `SecondaryColor` - Secondary accent color

### Interactive States
- `ButtonHoverBackColor` - Background for hovered media items
- `HighlightBackColor` - Background for selected/highlighted media

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

    // Apply media-specific theme colors
    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    // Update card and surface colors
    _cardBackColor = _currentTheme.CardBackColor;
    _surfaceColor = _currentTheme.SurfaceColor;
    _panelBackColor = _currentTheme.PanelBackColor;
    
    // Update text colors
    _titleForeColor = _currentTheme.CardTitleForeColor;
    _textForeColor = _currentTheme.CardTextForeColor;
    _subTitleForeColor = _currentTheme.CardSubTitleForeColor;
    
    // Update border and accent colors
    _borderColor = _currentTheme.BorderColor;
    _accentColor = _currentTheme.AccentColor;
    _primaryColor = _currentTheme.PrimaryColor;
    
    // Update interactive colors
    _hoverBackColor = _currentTheme.ButtonHoverBackColor;
    _highlightBackColor = _currentTheme.HighlightBackColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Keep BackColor and ForeColor as generic theme colors
2. Add card and surface color properties:
   - `_cardBackColor` = `CardBackColor`
   - `_surfaceColor` = `SurfaceColor`
   - `_panelBackColor` = `PanelBackColor`
3. Add text color properties:
   - `_titleForeColor` = `CardTitleForeColor`
   - `_textForeColor` = `CardTextForeColor`
   - `_subTitleForeColor` = `CardSubTitleForeColor`
4. Add border and accent colors:
   - `_borderColor` = `BorderColor`
   - `_accentColor` = `AccentColor`
   - `_primaryColor` = `PrimaryColor`
5. Add interactive state colors:
   - `_hoverBackColor` = `ButtonHoverBackColor`
   - `_highlightBackColor` = `HighlightBackColor`
6. Ensure InitializePainter() is called to refresh painter with new theme
7. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepMediaWidget may need additional properties for media styling:
- Layout colors: `_cardBackColor`, `_surfaceColor`, `_panelBackColor`
- Text colors: `_titleForeColor`, `_textForeColor`, `_subTitleForeColor`
- Interactive colors: `_borderColor`, `_accentColor`, `_primaryColor`, `_hoverBackColor`, `_highlightBackColor`

## Testing
- Verify media cards use CardBackColor
- Verify media surfaces use SurfaceColor
- Verify media titles use CardTitleForeColor
- Verify media descriptions use CardTextForeColor
- Verify media subtitles use CardSubTitleForeColor
- Verify borders use BorderColor
- Verify highlights and accents use AccentColor and PrimaryColor
- Verify hovered items use ButtonHoverBackColor
- Verify selected items use HighlightBackColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepMediaWidget.theme.instructions.md