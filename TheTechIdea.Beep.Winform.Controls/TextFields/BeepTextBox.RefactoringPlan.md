# BeepTextBox Refactoring Plan

## Overview
Refactor the existing BeepTextBox.cs (1881 lines) from inner-control pattern to modern helper-based architecture with BaseControl inheritance.

## Current Structure Analysis
The existing BeepTextBox.cs has the following major sections:
1. **Properties** (lines 20-634) - Text, display, behavior properties
2. **Format and Masking** (lines 300-444) - Mask format properties
3. **Image Properties** (lines 445-593) - Image handling properties
4. **AutoComplete Properties** (lines 594-634) - AutoComplete configuration
5. **Constructors** (lines 635-689) - Initialization logic
6. **Initialization** (lines 690-759) - Component setup
7. **Paint and Invalidate** (lines 760-805) - Drawing logic
8. **Size and Position** (lines 806-1049) - Layout management
9. **Mouse Events** (lines 1050-1065) - Mouse interaction
10. **Key Events** (lines 1066-1269) - Keyboard handling
11. **Search Events** (lines 1270-1279) - Search functionality
12. **Masking Logic** (lines 1280-1534) - Format implementation
13. **IBeepComponent Implementation** (lines 1535-1763) - Interface methods
14. **Format Strings** (lines 1764-1772) - Format constants
15. **Theme and Style** (lines 1773-1881) - Theming logic

## Refactoring Strategy

### 1. Architecture Changes
- **Base Class**: Change from `BeepControl` to `BaseControl` for Material 3.0 styling
- **Inner Control Removal**: Remove `_innerTextBox` and all references
- **Helper Pattern**: Implement helper classes for text handling
- **Partial Classes**: Split into manageable partial class files

### 2. Helper Classes to Create/Update
- **BeepTextBoxInputHelper**: Handle keyboard/mouse input
- **BeepTextBoxDrawingHelper**: Handle text rendering and drawing
- **BeepTextBoxCaretHelper**: Handle caret positioning and selection (ALREADY EXISTS)
- **BeepTextBoxMaskingHelper**: Handle text formatting and masking

### 3. Partial Class Structure

#### BeepTextBox.Main.cs
- Class declaration and inheritance
- Constructor and basic initialization
- Helper initialization
- Core properties (Text, Font, etc.)

#### BeepTextBox.Properties.cs
- All public properties from original file
- Property validation and change handling
- Backing fields

#### BeepTextBox.Methods.cs
- All public methods (AppendText, Clear, etc.)
- Masking and formatting methods
- Utility methods
- IBeepComponent implementation

#### BeepTextBox.Events.cs
- Event declarations
- Event handlers
- Keyboard/mouse event overrides
- Theme change handlers

### 4. Key Implementation Details

#### Text Rendering
- Replace inner TextBox with direct GDI+ text rendering
- Handle multiline text properly
- Support password character masking
- Implement proper text alignment

#### Caret Management
- Use existing BeepTextBoxCaretHelper
- Handle caret positioning
- Text selection management
- Mouse interaction for selection

#### Input Handling
- Keyboard input processing
- Character filtering (digits only, characters only)
- Mask format validation
- Clipboard operations

#### Masking System
- Extract all masking logic from original file
- Support all existing mask formats:
  - Currency, Percentage, Date, Time
  - Phone, Email, SSN, ZipCode
  - IPAddress, CreditCard, Custom
- Maintain backward compatibility

### 5. Migration Steps

#### Phase 1: Helper Classes
1. **BeepTextBoxInputHelper**
   - Extract keyboard/mouse handling logic
   - Implement input validation
   - Handle clipboard operations

2. **BeepTextBoxDrawingHelper**
   - Extract text rendering logic
   - Handle multiline text
   - Implement password masking
   - Support text alignment

3. **BeepTextBoxMaskingHelper**
   - Extract all masking methods
   - Implement format validation
   - Handle custom mask formats

#### Phase 2: Partial Classes
1. **Update BeepTextBox.Main.cs**
   - Change base class to BaseControl
   - Initialize helpers
   - Basic constructor

2. **Update BeepTextBox.Properties.cs**
   - Migrate all properties
   - Update property implementations
   - Remove inner TextBox references

3. **Update BeepTextBox.Methods.cs**
   - Migrate all methods
   - Update method implementations
   - Integrate with helpers

4. **Update BeepTextBox.Events.cs**
   - Migrate event handlers
   - Update event implementations
   - Integrate with helpers

#### Phase 3: Integration
1. Create new BeepTextBox.cs that combines all partial classes
2. Test compilation
3. Validate functionality
4. Ensure backward compatibility

### 6. Testing Requirements
- All existing properties work correctly
- Text input/output functions properly
- Masking works for all formats
- Selection and caret work correctly
- Keyboard shortcuts work (Ctrl+A, Ctrl+C, Ctrl+V, etc.)
- Mouse interaction works
- Theming integration works
- No breaking changes to public API

### 7. Files to Create/Update
1. **Helpers/**
   - BeepTextBoxInputHelper.cs (NEW)
   - BeepTextBoxDrawingHelper.cs (NEW)
   - BeepTextBoxMaskingHelper.cs (NEW)
   - BeepTextBoxCaretHelper.cs (UPDATE)

2. **Partial Classes/**
   - BeepTextBox.Main.cs (UPDATE)
   - BeepTextBox.Properties.cs (UPDATE)
   - BeepTextBox.Methods.cs (UPDATE)
   - BeepTextBox.Events.cs (UPDATE)

3. **Final Integration/**
   - BeepTextBox.cs (NEW - combines all partials)

### 8. Risk Mitigation
- Keep original file as backup
- Test each helper class individually
- Validate partial classes compile separately
- Ensure BaseControl integration works
- Test all masking formats
- Verify event handling works correctly

### 9. Success Criteria
- All partial classes compile without errors
- Helper classes function correctly
- New BeepTextBox.cs maintains all original functionality
- No breaking changes to existing API
- Improved maintainability and performance
- Better separation of concerns
