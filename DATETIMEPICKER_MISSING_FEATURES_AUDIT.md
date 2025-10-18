# DateTimePicker Painters - Missing Features Audit

## Executive Summary
Review of all DateTimePicker painters and hit handlers identified **3 critical missing features**:

1. **BeepComboBox Integration** - Year combo boxes have TODO placeholders, not showing dropdowns
2. **Reset Button Missing** - DualCalendar and other range pickers lack Reset functionality
3. **FilteredRange Year/Time Pickers** - Has TODOs for year and time selection

---

## 1. Missing BeepComboBox Implementations

### ✅ **DualCalendarDateTimePickerHitHandler** - COMPLETED
- **Status**: ✅ Fixed
- **Implementation**: Uses `DateTimeComboBoxHelper.CreateYearComboBox()` + `ShowDropdown()`
- **Location**: Lines 305-383
- **Features**:
  - Creates BeepComboBox with year list
  - Adds to owner controls temporarily
  - Shows dropdown via `comboBox.ShowDropdown()`
  - Removes control on close via `PopupClosed` event
  - Handles dual calendar gridIndex properly

### ❌ **YearViewDateTimePickerHitHandler** - NEEDS IMPLEMENTATION
- **File**: `YearViewDateTimePickerHitHandler.cs`
- **Issue**: Line 109 has TODO comment
- **Current Code**:
  ```csharp
  // Year ComboBox click - show year selection dropdown
  if (hitResult.HitArea == DateTimePickerHitArea.YearComboBox)
  {
      var comboBox = DateTimeComboBoxHelper.CreateYearComboBox(minYear, maxYear, currentYear);
      // TODO: Implement actual BeepComboBox dropdown integration
      return false; // Don't close the picker - let user interact with combo
  }
  ```
- **Fix Required**: Implement same pattern as DualCalendar
  - Add to owner.Controls
  - Call ShowDropdown()
  - Handle PopupClosed event

### ❌ **MonthViewDateTimePickerHitHandler** - NEEDS IMPLEMENTATION
- **File**: `MonthViewDateTimePickerHitHandler.cs`
- **Issue**: Line 107 has TODO comment
- **Current Code**:
  ```csharp
  // Year ComboBox click - show year selection dropdown
  if (hitResult.HitArea == DateTimePickerHitArea.YearComboBox)
  {
      var comboBox = DateTimeComboBoxHelper.CreateYearComboBox(minYear, maxYear, currentYear);
      // TODO: Implement actual BeepComboBox dropdown integration
      return false; // Don't close the picker - let user interact with combo
  }
  ```
- **Fix Required**: Same as YearView

### ❌ **FilteredRangeDateTimePickerHitHandler** - NEEDS IMPLEMENTATION
- **File**: `FilteredRangeDateTimePickerHitHandler.cs`
- **Issue**: Line 305 has TODO for year selection
- **Current Code**:
  ```csharp
  if (hitResult.HitArea == DateTimePickerHitArea.YearComboBox)
  {
      // TODO: Show year selection dropdown
      return false;
  }
  ```
- **Fix Required**: Complete implementation

---

## 2. Missing Reset Button Functionality

### ✅ **FilteredRangeDateTimePickerPainter** - HAS RESET BUTTON
- **Status**: ✅ Reference Implementation
- **Painter**: Lines 354-355, 780+
  - Paints Reset button
  - Handles hover/press states
  - Calculates ResetButtonRect in layout
- **Handler**: Has Reset button click handling
- **Pattern to Follow**: Use this as template for other range pickers

### ❌ **DualCalendarDateTimePickerPainter** - MISSING RESET BUTTON
- **File**: `DualCalendarDateTimePickerPainter.cs`
- **Issue**: No Reset button painting or layout
- **Status**: Partially started in handler (hit testing only)
- **Required Changes**:
  1. **Painter**:
     - Add `PaintResetButton()` method
     - Add ResetButtonRect calculation in `CalculateLayout()`
     - Call PaintResetButton from main Paint method
  2. **Handler** (DualCalendarDateTimePickerHitHandler.cs):
     - ✅ Hit testing already added (line 119-129)
     - ❌ Add to HandleClick() - reset _start/_end on click
     - ❌ Add to UpdateHoverState() - show hover effect

### 🔍 **Other Range Pickers - REVIEW NEEDED**
Need to check these range-based pickers for Reset button:
- **RangeDateTimePickerPainter**
- **RangeWithTimeDateTimePickerPainter**
- **FlexibleRangeDateTimePickerPainter**

---

## 3. FilteredRange Additional TODOs

### ❌ **FilteredRangeDateTimePickerHitHandler** - Time Picker
- **File**: `FilteredRangeDateTimePickerHitHandler.cs`
- **Issue**: Line 272 - TODO for time picker dialog
- **Current Code**:
  ```csharp
  if (hitResult.HitArea == DateTimePickerHitArea.StartTimeButton || 
      hitResult.HitArea == DateTimePickerHitArea.EndTimeButton)
  {
      // TODO: Show time picker dialog
      return false;
  }
  ```
- **Fix Required**: Implement time picker dialog (likely using BeepTimePicker if available)

---

## Implementation Priority

### 🔴 **HIGH PRIORITY** (User Requested)
1. **DualCalendar Reset Button** - Complete the partial implementation
2. **YearView BeepComboBox** - Fix TODO, make year dropdown work
3. **MonthView BeepComboBox** - Fix TODO, make year dropdown work

### 🟡 **MEDIUM PRIORITY**
4. **FilteredRange Year ComboBox** - Complete TODO implementation
5. **Other Range Pickers Reset Button** - Audit and add if needed

### 🟢 **LOW PRIORITY**
6. **FilteredRange Time Picker** - Implement time selection dialog

---

## Implementation Pattern

### **BeepComboBox ShowDropdown Pattern** (Use this template)
```csharp
private void ShowYearComboBox(BeepDateTimePicker owner, DateTimePickerHitTestResult hitResult)
{
    if (owner == null) return;
    
    int currentYear = owner.DisplayMonth.Year;
    int minYear = owner.MinDate.Year;
    int maxYear = owner.MaxDate.Year;

    // Create BeepComboBox using helper
    var comboBox = DateTimeComboBoxHelper.CreateYearComboBox(minYear, maxYear, currentYear);
    
    // Set up selection change handler
    comboBox.SelectedIndexChanged += (s, e) =>
    {
        int? selectedYear = DateTimeComboBoxHelper.GetSelectedYear(comboBox);
        if (!selectedYear.HasValue) return;
        
        // Navigate to selected year
        // (Implementation varies by mode)
        
        owner.Invalidate();
        comboBox.CloseDropdown();
    };

    // Position the combo box at the year combo box rect location
    var screenPoint = owner.PointToScreen(new Point(
        hitResult.HitBounds.X, 
        hitResult.HitBounds.Y
    ));
    
    comboBox.Size = hitResult.HitBounds.Size;
    comboBox.Location = owner.PointToClient(screenPoint);
    
    // Add to owner's controls temporarily
    owner.Controls.Add(comboBox);
    comboBox.BringToFront();
    
    // Show the dropdown
    comboBox.ShowDropdown();
    
    // Remove when closed
    comboBox.PopupClosed += (s, e) =>
    {
        owner.Controls.Remove(comboBox);
        comboBox.Dispose();
    };
}
```

### **Reset Button Pattern** (Use FilteredRange as reference)

**In Painter:**
```csharp
// Paint Reset button
bool resetHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.ResetButton) == true;
bool resetPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.ResetButton) == true;
PaintActionButton(g, layout.ResetButtonRect, "Reset", false, resetHovered, resetPressed);

// In CalculateLayout()
layout.ResetButtonRect = new Rectangle(
    bounds.X + (bounds.Width - 120) / 2,  // Centered
    bounds.Bottom - 90,                    // Near bottom
    120,                                   // Width
    36                                     // Height
);
```

**In HitHandler:**
```csharp
// In HitTest()
if (layout.ResetButtonRect != Rectangle.Empty && layout.ResetButtonRect.Contains(location))
{
    result.IsHit = true;
    result.HitArea = DateTimePickerHitArea.ResetButton;
    result.HitBounds = layout.ResetButtonRect;
    return result;
}

// In HandleClick()
if (hitResult.HitArea == DateTimePickerHitArea.ResetButton)
{
    Reset();  // Clear selection
    SyncToControl(owner);
    return false;  // Don't close
}

// In UpdateHoverState()
else if (hitResult.HitArea == DateTimePickerHitArea.ResetButton)
{
    hoverState.HoverArea = DateTimePickerHitArea.ResetButton;
    hoverState.HoverBounds = hitResult.HitBounds;
}
```

---

## Files Requiring Changes

### Immediate Changes (High Priority)
1. ✅ `DualCalendarDateTimePickerHitHandler.cs` - BeepComboBox DONE
2. ❌ `DualCalendarDateTimePickerPainter.cs` - Add Reset button painting
3. ❌ `DualCalendarDateTimePickerHitHandler.cs` - Complete Reset button handling
4. ❌ `YearViewDateTimePickerHitHandler.cs` - Fix BeepComboBox TODO
5. ❌ `MonthViewDateTimePickerHitHandler.cs` - Fix BeepComboBox TODO

### Medium Priority Changes
6. ❌ `FilteredRangeDateTimePickerHitHandler.cs` - Complete year combo TODO
7. ❌ Review: RangeDateTimePickerPainter (check for Reset button need)
8. ❌ Review: RangeWithTimeDateTimePickerPainter (check for Reset button need)
9. ❌ Review: FlexibleRangeDateTimePickerPainter (check for Reset button need)

---

## Next Steps

**Recommended Order:**
1. Fix YearViewDateTimePickerHitHandler BeepComboBox (copy DualCalendar pattern)
2. Fix MonthViewDateTimePickerHitHandler BeepComboBox (copy DualCalendar pattern)
3. Complete DualCalendar Reset button (HandleClick + UpdateHoverState)
4. Add DualCalendar Reset button painting (Painter + CalculateLayout)
5. Fix FilteredRange year combo box TODO
6. Audit other range pickers for Reset button needs
