# GridDataHelper SimpleItem Conversion Fix

## Problem
The `UpdateCellValue` method in `GridDataHelper` was throwing a `System.ArgumentException` when trying to update cells with values from dialog editors that returned `SimpleItem` objects:

```
System.ArgumentException: Object of type 'TheTechIdea.Beep.Winform.Controls.Models.SimpleItem' cannot be converted to type 'System.String'.
```

## Root Cause
When users edited cells using the dialog-based editor system, controls like `BeepComboBox` and `BeepListBox` return `SimpleItem` objects through their `GetValue()` method. The `GridDataHelper.UpdateCellValue()` method was trying to set these `SimpleItem` objects directly to string properties using reflection, which caused the type conversion error.

## Solution
Added a `NormalizeEditorValue` method to `GridDataHelper` that handles `SimpleItem` objects correctly by extracting the appropriate value based on the target property type:

### Key Changes

1. **Added value normalization**: The `UpdateCellValue` method now calls `NormalizeEditorValue` before setting values.

2. **SimpleItem handling**: For `SimpleItem` objects, the method extracts:
   - `si.Text` for string properties
   - `si.Value` or `si.Text` for numeric properties with conversion
   - `si.Text` for enum properties with parsing
   - `si.Value` or `si.Text` for DateTime properties with parsing

3. **Type-safe conversion**: Uses proper type conversion based on the target property type from reflection.

### Code Flow

```csharp
// Before (caused exception)
prop.SetValue(row.RowData, newValue); // newValue is SimpleItem

// After (works correctly)
var normalizedValue = NormalizeEditorValue(newValue, column, cell);
var convertedValue = ConvertValue(normalizedValue, prop.PropertyType);
prop.SetValue(row.RowData, convertedValue);
```

### Supported Conversions

- **SimpleItem ? String**: Uses `Text`, `Value.ToString()`, or `Item.ToString()`
- **SimpleItem ? Numeric**: Converts `Value` or `Text` to target numeric type
- **SimpleItem ? Enum**: Parses `Text` or `Value` to enum
- **SimpleItem ? DateTime**: Parses `Value` or `Text` to DateTime
- **Other types**: Direct conversion or pass-through

## Testing

The fix handles all dialog editor scenarios:
- ? BeepComboBox returning SimpleItem
- ? BeepListBox returning SimpleItem  
- ? BeepListofValuesBox returning SimpleItem
- ? BeepTextBox returning string
- ? BeepCheckBox returning bool
- ? BeepDatePicker returning DateTime

## Result

Dialog-based editing now works seamlessly without type conversion errors, providing the same robust value handling that was already implemented in `GridEditHelper.NormalizeEditorValue`.