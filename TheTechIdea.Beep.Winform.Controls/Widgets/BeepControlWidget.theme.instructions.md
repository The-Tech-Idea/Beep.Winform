# BeepControlWidget Theme Instructions

## Overview
The BeepControlWidget ApplyTheme function should properly apply control-specific theme properties to ensure consistent styling for interactive controls like buttons, toggles, sliders, and form controls within widgets.

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

### Button/Control Colors
- `ButtonBackColor` - Background color for buttons
- `ButtonForeColor` - Text color for buttons
- `ButtonBorderColor` - Border color for buttons
- `ButtonHoverBackColor` - Background for hovered buttons
- `ButtonHoverForeColor` - Text color for hovered buttons
- `ButtonHoverBorderColor` - Border color for hovered buttons
- `ButtonSelectedBackColor` - Background for selected/active buttons
- `ButtonSelectedForeColor` - Text color for selected buttons
- `ButtonSelectedBorderColor` - Border color for selected buttons
- `ButtonPressedBackColor` - Background for pressed buttons
- `ButtonPressedForeColor` - Text color for pressed buttons
- `ButtonPressedBorderColor` - Border color for pressed buttons

### Typography Styles
- `ButtonFont` - Font for button text
- `ButtonHoverFont` - Font for hovered button text
- `ButtonSelectedFont` - Font for selected button text

### CheckBox/Radio Colors (if applicable)
- `CheckBoxBackColor` - Background for checkboxes
- `CheckBoxForeColor` - Text color for checkboxes
- `CheckBoxBorderColor` - Border color for checkboxes
- `CheckBoxCheckedBackColor` - Background for checked checkboxes
- `CheckBoxCheckedForeColor` - Text color for checked checkboxes
- `CheckBoxHoverBackColor` - Background for hovered checkboxes

### General Theme Colors
- `BackColor` - Widget background
- `ForeColor` - Default text color
- `AccentColor` - Accent color for active states
- `PrimaryColor` - Primary color
- `SecondaryColor` - Secondary color

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply control-specific theme colors
    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    // Update button colors
    _buttonBackColor = _currentTheme.ButtonBackColor;
    _buttonForeColor = _currentTheme.ButtonForeColor;
    _buttonBorderColor = _currentTheme.ButtonBorderColor;
    _buttonHoverBackColor = _currentTheme.ButtonHoverBackColor;
    _buttonHoverForeColor = _currentTheme.ButtonHoverForeColor;
    _buttonSelectedBackColor = _currentTheme.ButtonSelectedBackColor;
    _buttonSelectedForeColor = _currentTheme.ButtonSelectedForeColor;
    _buttonPressedBackColor = _currentTheme.ButtonPressedBackColor;
    
    // Update checkbox colors if applicable
    _checkBoxBackColor = _currentTheme.CheckBoxBackColor;
    _checkBoxForeColor = _currentTheme.CheckBoxForeColor;
    _checkBoxCheckedBackColor = _currentTheme.CheckBoxCheckedBackColor;
    
    // Update accent colors
    _accentColor = _currentTheme.AccentColor;
    _primaryColor = _currentTheme.PrimaryColor;
    _secondaryColor = _currentTheme.SecondaryColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Keep BackColor and ForeColor as generic theme colors
2. Add button-specific color properties and set them from theme:
   - `_buttonBackColor` = `ButtonBackColor`
   - `_buttonForeColor` = `ButtonForeColor`
   - `_buttonBorderColor` = `ButtonBorderColor`
   - `_buttonHoverBackColor` = `ButtonHoverBackColor`
   - `_buttonHoverForeColor` = `ButtonHoverForeColor`
   - `_buttonSelectedBackColor` = `ButtonSelectedBackColor`
   - `_buttonSelectedForeColor` = `ButtonSelectedForeColor`
   - `_buttonPressedBackColor` = `ButtonPressedBackColor`
3. Add checkbox-specific colors if the widget includes checkboxes:
   - `_checkBoxBackColor` = `CheckBoxBackColor`
   - `_checkBoxForeColor` = `CheckBoxForeColor`
   - `_checkBoxCheckedBackColor` = `CheckBoxCheckedBackColor`
4. Add general accent colors:
   - `_accentColor` = `AccentColor`
   - `_primaryColor` = `PrimaryColor`
   - `_secondaryColor` = `SecondaryColor`
5. Ensure InitializePainter() is called to refresh painter with new theme
6. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepControlWidget may need additional properties for different control states:
- Button colors: `_buttonBackColor`, `_buttonForeColor`, `_buttonBorderColor`, `_buttonHoverBackColor`, etc.
- Checkbox colors: `_checkBoxBackColor`, `_checkBoxForeColor`, `_checkBoxCheckedBackColor`, etc.
- General colors: `_accentColor`, `_primaryColor`, `_secondaryColor`

## Testing
- Verify buttons use ButtonBackColor, ButtonForeColor, ButtonBorderColor
- Verify hovered buttons use ButtonHover colors
- Verify selected buttons use ButtonSelected colors
- Verify pressed buttons use ButtonPressed colors
- Verify checkboxes use CheckBox colors appropriately
- Verify accents use AccentColor, PrimaryColor, SecondaryColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepControlWidget.theme.instructions.md