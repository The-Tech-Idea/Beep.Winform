# BeepTextBox Refactoring Plan

## Overview
Refactor BeepTextBox from inner-control pattern to modern helper-based architecture with BaseControl inheritance.

## Current State Analysis
- **File Size**: 1,881 lines
- **Inner Control**: Heavy dependency on `_innerTextBox` (TextBox control)
- **Inheritance**: Currently inherits from BeepControl
- **Architecture**: Monolithic class with mixed responsibilities

## Target Architecture
- **Inheritance**: BaseControl (Material 3.0 capable)
- **Pattern**: Helper-based with partial classes
- **Rendering**: Direct GDI+ text rendering (no inner controls)
- **Structure**: Modular with clear separation of concerns

## Helper Classes (Already Created)
1. **BeepTextBoxInputHelper** - Keyboard/mouse input and validation
2. **BeepTextBoxDrawingHelper** - Text rendering and drawing operations
3. **BeepTextBoxCaretHelper** - Caret positioning and text selection

## Partial Classes Structure

### 1. BeepTextBox.Main.cs
**Purpose**: Main class definition, constructor, initialization
**Responsibilities**:
- Class declaration and inheritance from BaseControl
- Private fields (helpers, core properties)
- Constructor and initialization methods
- Helper integration and setup

**Key Methods**:
- Constructor
- InitializeComponent()
- InitializeHelpers()
- SetupControl()

### 2. BeepTextBox.Properties.cs
**Purpose**: All public properties adapted for helper-based architecture
**Categories**:
- **Text Properties**: Text, TextFont, TextAlignment, PlaceholderText
- **Behavior Properties**: Multiline, ReadOnly, PasswordChar, MaxLength
- **Selection Properties**: SelectionStart, SelectionLength, SelectedText
- **Appearance Properties**: BackColor, ForeColor, Border properties
- **Image Properties**: ImagePath, TextImageRelation, MaxImageSize
- **Masking Properties**: MaskFormat, CustomMask, Date/Time formats
- **Validation Properties**: OnlyDigits, OnlyCharacters
- **AutoComplete Properties**: AutoCompleteMode, AutoCompleteSource

### 3. BeepTextBox.Methods.cs
**Purpose**: All methods including masking, validation, and text manipulation
**Categories**:
- **Text Manipulation**: AppendText, Clear, SelectAll, ScrollToCaret
- **Masking Logic**: ApplyMaskFormat and all format-specific methods
- **Validation**: ValidateData, input validation methods
- **Helper Methods**: GetSingleLineHeight, UpdateMinimumSize, etc.
- **IBeepComponent**: Clear, Focus, SetFocus, GetValue, SetValue

**Masking Methods** (15 format types):
- ApplyCurrencyFormat, ApplyPercentageFormat, ApplyDateFormat
- ApplyTimeFormat, ApplyPhoneNumberFormat, ApplySSNFormat
- ApplyZipCodeFormat, ApplyIPAddressFormat, ApplyCreditCardFormat
- ApplyTimeSpanFormat, ApplyDecimalFormat, ApplyYearFormat
- ApplyMonthYearFormat, ApplyCustomFormat

### 4. BeepTextBox.Events.cs
**Purpose**: All event handlers and overrides for user interaction
**Categories**:
- **Keyboard Events**: OnKeyDown, OnKeyPress, OnKeyUp
- **Mouse Events**: OnMouseDown, OnMouseUp, OnMouseMove, OnMouseWheel
- **Focus Events**: OnGotFocus, OnLostFocus
- **Paint Events**: OnPaint, Draw method overrides
- **Theme Events**: ApplyTheme, AfterThemeApplied
- **Size Events**: OnResize, OnLayout
- **Custom Events**: SearchTriggered, TextChanged

## Migration Strategy

### Phase 1: Core Infrastructure
1. Create new BeepTextBox_New.cs with BaseControl inheritance
2. Implement helper integration in main partial class
3. Set up basic initialization and helper setup

### Phase 2: Properties Migration
1. Migrate all properties from old implementation
2. Adapt property getters/setters to use helpers instead of inner TextBox
3. Ensure proper attribute decorations and categories

### Phase 3: Methods Migration
1. Move all masking methods to Methods partial class
2. Implement text manipulation methods using helpers
3. Migrate IBeepComponent implementation

### Phase 4: Events Migration
1. Implement event handlers using helper classes
2. Set up proper event routing and delegation
3. Ensure theme application works correctly

### Phase 5: Drawing Implementation
1. Implement direct text rendering in drawing helper
2. Handle multiline text rendering
3. Implement password character masking
4. Handle image positioning and rendering

### Phase 6: Testing and Validation
1. Test all masking formats work correctly
2. Validate input restrictions (OnlyDigits, OnlyCharacters)
3. Test selection and caret positioning
4. Verify theme application and visual appearance
5. Test image integration and positioning

## Key Technical Changes

### 1. Remove Inner Control Dependency
```csharp
// OLD: Heavy dependency on inner TextBox
private TextBox _innerTextBox;

// NEW: Helper-based approach
private BeepTextBoxInputHelper _inputHelper;
private BeepTextBoxDrawingHelper _drawingHelper;
private BeepTextBoxCaretHelper _caretHelper;
```

### 2. Property Adaptation
```csharp
// OLD: Delegate to inner TextBox
public bool ReadOnly
{
    get => _innerTextBox.ReadOnly;
    set => _innerTextBox.ReadOnly = value;
}

// NEW: Use helper or direct implementation
public bool ReadOnly
{
    get => _readOnly;
    set
    {
        _readOnly = value;
        _inputHelper.SetReadOnly(value);
        Invalidate();
    }
}
```

### 3. Direct Text Rendering
```csharp
// OLD: Let inner TextBox handle rendering
// (inner TextBox renders itself)

// NEW: Direct GDI+ rendering
public override void Draw(Graphics graphics, Rectangle rectangle)
{
    _drawingHelper.DrawText(graphics, rectangle);
}
```

### 4. Event Handling
```csharp
// OLD: Hook into inner TextBox events
_innerTextBox.KeyPress += InnerTextBox_KeyPress;

// NEW: Handle events directly
protected override void OnKeyPress(KeyPressEventArgs e)
{
    _inputHelper.HandleKeyPress(e);
    base.OnKeyPress(e);
}
```

## Helper Class Responsibilities

### BeepTextBoxInputHelper
- Keyboard input processing and validation
- Mouse input handling
- Input restrictions (OnlyDigits, OnlyCharacters)
- Mask format validation during input

### BeepTextBoxDrawingHelper
- Text measurement and layout
- Direct GDI+ text rendering
- Password character masking
- Multiline text handling
- Image positioning and rendering

### BeepTextBoxCaretHelper
- Caret positioning and blinking
- Text selection management
- SelectionStart/SelectionLength properties
- Clipboard operations

## Benefits of New Architecture

1. **Performance**: Direct rendering eliminates inner control overhead
2. **Maintainability**: Clear separation of concerns with partial classes
3. **Extensibility**: Helper-based design allows easy feature addition
4. **Material Design**: BaseControl provides Material 3.0 styling capabilities
5. **Consistency**: Unified approach with other Beep controls
6. **Testability**: Isolated helper classes are easier to unit test

## Implementation Order

1. ✅ Create helper classes (BeepTextBoxInputHelper, BeepTextBoxDrawingHelper, BeepTextBoxCaretHelper)
2. 🔄 Create BeepTextBox_New.cs with BaseControl inheritance
3. ⏳ Implement BeepTextBox.Main.cs partial class
4. ⏳ Implement BeepTextBox.Properties.cs partial class
5. ⏳ Implement BeepTextBox.Methods.cs partial class (including all masking methods)
6. ⏳ Implement BeepTextBox.Events.cs partial class
7. ⏳ Test and validate all functionality
8. ⏳ Replace original BeepTextBox.cs with new implementation

## Success Criteria

- ✅ All original properties and methods preserved
- ✅ All masking formats work correctly
- ✅ Input validation functions properly
- ✅ Visual appearance matches original (or improved with Material Design)
- ✅ No inner control dependencies
- ✅ Proper BaseControl integration
- ✅ Helper classes handle their respective responsibilities
- ✅ Code is maintainable and extensible
