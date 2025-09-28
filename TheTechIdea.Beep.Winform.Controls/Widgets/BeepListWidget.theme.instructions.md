# BeepListWidget Theme Instructions

## Overview
The BeepListWidget ApplyTheme function should properly apply list/table-specific theme properties to ensure consistent styling for displaying data tables, feeds, rankings, and list-based content.

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

### List/Table-Specific Colors
- `BackColor` - Background color for the list area
- `ForeColor` - Default text color
- `PanelBackColor` - Background for list panels/cards
- `BorderColor` - Border color for list items
- `SurfaceColor` - Surface color for individual list items

### Row/Item States
- `HighlightBackColor` - Background for highlighted/selected items
- `ButtonHoverBackColor` - Background for hovered items
- `DisabledBackColor` - Background for disabled items

### Text Colors
- `ForeColor` - Default text color
- `DisabledForeColor` - Text color for disabled items
- `OnBackgroundColor` - Text color on colored backgrounds

### Accent Colors
- `AccentColor` - Color for highlights, badges, and emphasis
- `PrimaryColor` - Primary accent color
- `SecondaryColor` - Secondary accent color

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply list-specific theme colors
    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    // Update item background colors
    _itemBackColor = _currentTheme.SurfaceColor;
    _selectedItemBackColor = _currentTheme.HighlightBackColor;
    _hoverItemBackColor = _currentTheme.ButtonHoverBackColor;
    _disabledItemBackColor = _currentTheme.DisabledBackColor;
    
    // Update text colors
    _itemForeColor = _currentTheme.ForeColor;
    _disabledItemForeColor = _currentTheme.DisabledForeColor;
    
    // Update accent colors
    _accentColor = _currentTheme.AccentColor;
    _borderColor = _currentTheme.BorderColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Keep BackColor and ForeColor as generic theme colors (lists use standard backgrounds)
2. Add item-specific color properties and set them from theme:
   - `_itemBackColor` = `SurfaceColor`
   - `_selectedItemBackColor` = `HighlightBackColor`
   - `_hoverItemBackColor` = `ButtonHoverBackColor`
   - `_disabledItemBackColor` = `DisabledBackColor`
3. Add text color properties:
   - `_itemForeColor` = `ForeColor`
   - `_disabledItemForeColor` = `DisabledForeColor`
4. Add accent and border properties:
   - `_accentColor` = `AccentColor`
   - `_borderColor` = `BorderColor`
5. Ensure InitializePainter() is called to refresh painter with new theme
6. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepListWidget may need additional color properties for different item states:
- `_itemBackColor` for normal item backgrounds
- `_selectedItemBackColor` for selected items
- `_hoverItemBackColor` for hovered items
- `_disabledItemBackColor` for disabled items
- `_itemForeColor` for item text
- `_disabledItemForeColor` for disabled item text
- `_borderColor` for item borders

## Testing
- Verify list background uses BackColor
- Verify list items use SurfaceColor for background
- Verify selected items use HighlightBackColor
- Verify hovered items use ButtonHoverBackColor
- Verify disabled items use DisabledBackColor
- Verify text uses appropriate ForeColor/DisabledForeColor
- Verify borders use BorderColor
- Verify accents use AccentColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepListWidget.theme.instructions.md