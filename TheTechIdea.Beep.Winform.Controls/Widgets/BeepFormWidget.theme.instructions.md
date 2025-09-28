# BeepFormWidget Theme Instructions

## Overview
The BeepFormWidget ApplyTheme function should properly apply form-specific theme properties to ensure consistent styling for form fields, validation, and data entry components.

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

### Form Field Colors
- `TextBoxBackColor` - Background for text input fields
- `TextBoxForeColor` - Text color for input fields
- `TextBoxBorderColor` - Border color for input fields
- `TextBoxPlaceholderColor` - Color for placeholder text

### Validation States
- `TextBoxErrorBackColor` - Background for error state fields
- `TextBoxErrorPlaceholderColor` - Placeholder color for error fields
- `SuccessColor` - Color for valid fields
- `ErrorColor` - Color for invalid fields
- `WarningColor` - Color for warning/validation states

### Interactive States
- `TextBoxHoverBackColor` - Background for hovered fields
- `TextBoxHoverForeColor` - Text color for hovered fields
- `TextBoxSelectedBackColor` - Background for focused/selected fields
- `TextBoxSelectedForeColor` - Text color for focused fields

### Button/Control Colors
- `ButtonBackColor` - Background for form buttons
- `ButtonForeColor` - Text color for form buttons
- `ButtonBorderColor` - Border color for form buttons
- `ButtonHoverBackColor` - Background for hovered buttons
- `ButtonSelectedBackColor` - Background for active buttons

### Label and Text Colors
- `CardTitleForeColor` - Color for form labels and titles
- `CardTextForeColor` - Color for form descriptions
- `CardSubTitleForeColor` - Color for help text and hints

### Typography Styles
- `CardTitleFont` - Font for form labels
- `CardSubTitleFont` - Font for help text
- `ButtonFont` - Font for form buttons

### General Theme Colors
- `BackColor` - Form background
- `ForeColor` - Default text color
- `BorderColor` - Fallback border color
- `SurfaceColor` - Surface color for form sections

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply form-specific theme colors
    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    // Update input field colors
    _textBoxBackColor = _currentTheme.TextBoxBackColor;
    _textBoxForeColor = _currentTheme.TextBoxForeColor;
    _textBoxBorderColor = _currentTheme.TextBoxBorderColor;
    _placeholderColor = _currentTheme.TextBoxPlaceholderColor;
    
    // Update validation state colors
    _errorBackColor = _currentTheme.TextBoxErrorBackColor;
    _errorPlaceholderColor = _currentTheme.TextBoxErrorPlaceholderColor;
    _validColor = _currentTheme.SuccessColor;
    _invalidColor = _currentTheme.ErrorColor;
    _warningColor = _currentTheme.WarningColor;
    
    // Update interactive state colors
    _hoverBackColor = _currentTheme.TextBoxHoverBackColor;
    _hoverForeColor = _currentTheme.TextBoxHoverForeColor;
    _focusedBackColor = _currentTheme.TextBoxSelectedBackColor;
    _focusedForeColor = _currentTheme.TextBoxSelectedForeColor;
    
    // Update button colors
    _buttonBackColor = _currentTheme.ButtonBackColor;
    _buttonForeColor = _currentTheme.ButtonForeColor;
    _buttonBorderColor = _currentTheme.ButtonBorderColor;
    
    // Update label colors
    _labelForeColor = _currentTheme.CardTitleForeColor;
    _helpTextForeColor = _currentTheme.CardSubTitleForeColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Keep BackColor and ForeColor as generic theme colors
2. Add input field color properties:
   - `_textBoxBackColor` = `TextBoxBackColor`
   - `_textBoxForeColor` = `TextBoxForeColor`
   - `_textBoxBorderColor` = `TextBoxBorderColor`
   - `_placeholderColor` = `TextBoxPlaceholderColor`
3. Add validation color properties:
   - `_errorBackColor` = `TextBoxErrorBackColor`
   - `_errorPlaceholderColor` = `TextBoxErrorPlaceholderColor`
   - `_validColor` = `SuccessColor`
   - `_invalidColor` = `ErrorColor`
   - `_warningColor` = `WarningColor`
4. Add interactive state color properties:
   - `_hoverBackColor` = `TextBoxHoverBackColor`
   - `_hoverForeColor` = `TextBoxHoverForeColor`
   - `_focusedBackColor` = `TextBoxSelectedBackColor`
   - `_focusedForeColor` = `TextBoxSelectedForeColor`
5. Add button color properties:
   - `_buttonBackColor` = `ButtonBackColor`
   - `_buttonForeColor` = `ButtonForeColor`
   - `_buttonBorderColor` = `ButtonBorderColor`
6. Add label color properties:
   - `_labelForeColor` = `CardTitleForeColor`
   - `_helpTextForeColor` = `CardSubTitleForeColor`
7. Ensure InitializePainter() is called to refresh painter with new theme
8. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepFormWidget may need additional properties for form styling:
- Input colors: `_textBoxBackColor`, `_textBoxForeColor`, `_textBoxBorderColor`, `_placeholderColor`
- Validation colors: `_errorBackColor`, `_errorPlaceholderColor`, `_validColor`, `_invalidColor`, `_warningColor`
- Interactive colors: `_hoverBackColor`, `_hoverForeColor`, `_focusedBackColor`, `_focusedForeColor`
- Button colors: `_buttonBackColor`, `_buttonForeColor`, `_buttonBorderColor`
- Label colors: `_labelForeColor`, `_helpTextForeColor`

## Testing
- Verify input fields use TextBoxBackColor, TextBoxForeColor, TextBoxBorderColor
- Verify placeholders use TextBoxPlaceholderColor
- Verify error fields use TextBoxErrorBackColor and TextBoxErrorPlaceholderColor
- Verify valid fields show SuccessColor indicators
- Verify invalid fields show ErrorColor indicators
- Verify warning fields show WarningColor indicators
- Verify hovered fields use TextBoxHover colors
- Verify focused fields use TextBoxSelected colors
- Verify form buttons use Button colors
- Verify form labels use CardTitleForeColor
- Verify help text uses CardSubTitleForeColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepFormWidget.theme.instructions.md