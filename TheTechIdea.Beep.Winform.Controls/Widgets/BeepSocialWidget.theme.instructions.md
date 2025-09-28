# BeepSocialWidget Theme Instructions

## Overview
The BeepSocialWidget ApplyTheme function should properly apply social-specific theme properties to ensure consistent styling for user profiles, teams, messaging, and social feeds.

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

### Social/Card Colors
- `CardBackColor` - Background for social cards and posts
- `SurfaceColor` - Surface color for social content areas
- `PanelBackColor` - Background for social panels

### User/Author Colors
- `CardTitleForeColor` - Color for user names and titles
- `CardTextForeColor` - Color for social content and messages
- `CardSubTitleForeColor` - Color for timestamps and metadata

### Typography Styles
- `CardTitleFont` - Font for user names
- `CardSubTitleFont` - Font for timestamps and metadata
- `CardparagraphStyle` - Font for social content

### Interactive States
- `ButtonHoverBackColor` - Background for hovered social elements
- `HighlightBackColor` - Background for liked/selected content
- `AccentColor` - Color for engagement indicators (likes, comments)

### Status Colors
- `SuccessColor` - Color for positive social indicators
- `PrimaryColor` - Primary social accent color
- `SecondaryColor` - Secondary social accent color

### Avatar/Profile Colors
- `BorderColor` - Border color for avatars and profile images
- `OnBackgroundColor` - Text color on colored profile elements

### General Theme Colors
- `BackColor` - Widget background
- `ForeColor` - Default text color
- `BorderColor` - Fallback border color

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply social-specific theme colors
    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    // Update card and content colors
    _cardBackColor = _currentTheme.CardBackColor;
    _surfaceColor = _currentTheme.SurfaceColor;
    _panelBackColor = _currentTheme.PanelBackColor;
    
    // Update user and content text colors
    _userNameForeColor = _currentTheme.CardTitleForeColor;
    _contentForeColor = _currentTheme.CardTextForeColor;
    _metadataForeColor = _currentTheme.CardSubTitleForeColor;
    
    // Update interactive colors
    _hoverBackColor = _currentTheme.ButtonHoverBackColor;
    _highlightBackColor = _currentTheme.HighlightBackColor;
    _engagementColor = _currentTheme.AccentColor;
    
    // Update status and accent colors
    _positiveColor = _currentTheme.SuccessColor;
    _primaryColor = _currentTheme.PrimaryColor;
    _secondaryColor = _currentTheme.SecondaryColor;
    
    // Update avatar and profile colors
    _avatarBorderColor = _currentTheme.BorderColor;
    _onProfileColor = _currentTheme.OnBackgroundColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Keep BackColor and ForeColor as generic theme colors
2. Add card and content color properties:
   - `_cardBackColor` = `CardBackColor`
   - `_surfaceColor` = `SurfaceColor`
   - `_panelBackColor` = `PanelBackColor`
3. Add user and content text color properties:
   - `_userNameForeColor` = `CardTitleForeColor`
   - `_contentForeColor` = `CardTextForeColor`
   - `_metadataForeColor` = `CardSubTitleForeColor`
4. Add interactive color properties:
   - `_hoverBackColor` = `ButtonHoverBackColor`
   - `_highlightBackColor` = `HighlightBackColor`
   - `_engagementColor` = `AccentColor`
5. Add status and accent colors:
   - `_positiveColor` = `SuccessColor`
   - `_primaryColor` = `PrimaryColor`
   - `_secondaryColor` = `SecondaryColor`
6. Add avatar and profile colors:
   - `_avatarBorderColor` = `BorderColor`
   - `_onProfileColor` = `OnBackgroundColor`
7. Ensure InitializePainter() is called to refresh painter with new theme
8. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepSocialWidget may need additional properties for social styling:
- Layout colors: `_cardBackColor`, `_surfaceColor`, `_panelBackColor`
- Text colors: `_userNameForeColor`, `_contentForeColor`, `_metadataForeColor`
- Interactive colors: `_hoverBackColor`, `_highlightBackColor`, `_engagementColor`
- Status colors: `_positiveColor`, `_primaryColor`, `_secondaryColor`
- Profile colors: `_avatarBorderColor`, `_onProfileColor`

## Testing
- Verify social cards use CardBackColor
- Verify social surfaces use SurfaceColor
- Verify social panels use PanelBackColor
- Verify user names use CardTitleForeColor
- Verify social content uses CardTextForeColor
- Verify timestamps/metadata use CardSubTitleForeColor
- Verify hovered elements use ButtonHoverBackColor
- Verify liked/selected content use HighlightBackColor
- Verify engagement indicators use AccentColor
- Verify positive indicators use SuccessColor
- Verify primary accents use PrimaryColor
- Verify secondary accents use SecondaryColor
- Verify avatar borders use BorderColor
- Verify text on profiles use OnBackgroundColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepSocialWidget.theme.instructions.md