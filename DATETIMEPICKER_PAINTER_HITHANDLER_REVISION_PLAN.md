# DateTimePicker Painter & HitHandler Revision Plan

## Executive Summary
This plan ensures **all painters properly register hit areas** via `BeepDateTimePickerHitTestHelper` during their `CalculateLayout` methods, and that their corresponding **hit handlers correctly process these areas** using the `DateTimePickerHitArea` enum.

## Current Architecture

### Flow Diagram
```
BeepDateTimePicker.DrawContent()
    ↓
1. Painter.CalculateLayout() → Creates DateTimePickerLayout with Rectangles
    ↓
2. _hitHelper.RegisterHitAreas(layout, props, displayMonth) → Registers all areas to BaseControl._hitTest
    ↓
3. Painter.PaintCalendar() → Paints visual elements (uses layout)
    ↓
4. User Interaction → BaseControl hit test system detects hits
    ↓
5. HitHandler.HitTest() → Maps location to DateTimePickerHitArea enum
    ↓
6. HitHandler.HandleClick() → Executes workflow logic
```

### Key Components
- **DateTimePickerLayout**: Stores Rectangle bounds for all interactive elements
- **BeepDateTimePickerHitTestHelper**: Registers layout rectangles to BaseControl's `_hitTest`
- **DateTimePickerHitArea enum**: Standardized area types (DayCell, NextButton, TimeSlot, etc.)
- **IDateTimePickerPainter**: Interface with `CalculateLayout()` and `PaintCalendar()` methods
- **IDateTimePickerHitHandler**: Interface with `HitTest()` and `HandleClick()` methods

---

## Painter/HitHandler Pairs Matrix

| # | Painter | HitHandler | Mode |
|---|---------|-----------|------|
| 1 | SingleDateTimePickerPainter | SingleDateTimePickerHitHandler | Single |
| 2 | SingleWithTimeDateTimePickerPainter | SingleWithTimeDateTimePickerHitHandler | SingleWithTime |
| 3 | RangeDateTimePickerPainter | RangeDateTimePickerHitHandler | Range |
| 4 | RangeWithTimeDateTimePickerPainter | RangeWithTimeDateTimePickerHitHandler | RangeWithTime |
| 5 | MultipleDateTimePickerPainter | MultipleDateTimePickerHitHandler | Multiple |
| 6 | AppointmentDateTimePickerPainter | AppointmentDateTimePickerHitHandler | Appointment |
| 7 | TimelineDateTimePickerPainter | TimelineDateTimePickerHitHandler | Timeline |
| 8 | QuarterlyDateTimePickerPainter | QuarterlyDateTimePickerHitHandler | Quarterly |
| 9 | CompactDateTimePickerPainter | CompactDateTimePickerHitHandler | Compact |
| 10 | ModernCardDateTimePickerPainter | ModernCardDateTimePickerHitHandler | ModernCard |
| 11 | DualCalendarDateTimePickerPainter | DualCalendarDateTimePickerHitHandler | DualCalendar |
| 12 | WeekViewDateTimePickerPainter | WeekViewDateTimePickerHitHandler | WeekView |
| 13 | MonthViewDateTimePickerPainter | MonthViewDateTimePickerHitHandler | MonthView |
| 14 | YearViewDateTimePickerPainter | YearViewDateTimePickerHitHandler | YearView |
| 15 | SidebarEventDateTimePickerPainter | SidebarEventDateTimePickerHitHandler | SidebarEvent |
| 16 | FlexibleRangeDateTimePickerPainter | FlexibleRangeDateTimePickerHitHandler | FlexibleRange |
| 17 | FilteredRangeDateTimePickerPainter | FilteredRangeDateTimePickerHitHandler | FilteredRange |
| 18 | HeaderDateTimePickerPainter | HeaderDateTimePickerHitHandler | Header |

---

## DateTimePickerHitArea Enum - Area Type Reference

```csharp
public enum DateTimePickerHitArea
{
    None,
    Header,              // Month/Year title area
    PreviousButton,      // Previous month/year nav
    NextButton,          // Next month/year nav
    DayCell,             // Individual calendar day
    TimeSlot,            // Time selection slot
    QuickButton,         // Today/Tomorrow/etc quick actions
    TimeButton,          // Time picker toggle
    TimeSpinner,         // Hour/minute spinner
    ApplyButton,         // Confirm/Apply action
    CancelButton,        // Cancel action
    WeekNumber,          // Week number cell
    DropdownButton,      // Dropdown toggle
    ClearButton,         // Clear selection
    ActionButton,        // Generic action button
    Handle,              // Drag handle (Timeline)
    TimelineTrack,       // Timeline bar track
    FilterButton,        // Filter/preset button
    CreateButton,        // Create event button
    MonthButton,         // Month selector
    YearButton,          // Year selector
    QuarterButton,       // Quarter selector (Q1-Q4)
    WeekRow,             // Entire week row
    GridButton,          // Generic grid button
    FlexibleRangeButton, // Flexible range preset
    TodayButton          // Today shortcut
}
```

---

## Revision Process (Per Painter/HitHandler Pair)

### Phase 1: Painter Revision
For each painter, verify and correct:

#### 1.1 CalculateLayout() Method
**Checklist:**
- [ ] Returns complete `DateTimePickerLayout` with ALL interactive rectangles populated
- [ ] Handles single-month vs. multi-month layouts (MonthGrids collection)
- [ ] Calculates bounds for:
  - Navigation buttons (PreviousButtonRect, NextButtonRect)
  - Header/title (TitleRect, HeaderRect)
  - Day cells (DayCellRects or MonthGrids[].DayCellRects)
  - Week numbers (WeekNumberRects) if applicable
  - Time slots (TimeSlotRects) if applicable
  - Quick buttons (QuickButtonRects) if applicable
  - Action buttons (ApplyButtonRect, CancelButtonRect, ClearButtonRect)
  - Mode-specific elements (see area mapping below)

#### 1.2 Layout Completeness
Ensure layout includes bounds for EVERY painted clickable element

#### 1.3 Documentation
Add XML comments explaining which hit areas the layout supports

---

### Phase 2: HitTestHelper Registration Verification
Verify that `BeepDateTimePickerHitTestHelper.RegisterHitAreas()` correctly handles the painter's layout:

**Current Registration Methods:**
- `RegisterNavigationButtons()` - Handles PreviousButton, NextButton, Header
- `RegisterDayCells()` - Single-month day cells
- `RegisterMultipleCalendarGrids()` - Multi-month day cells (dual/range calendars)
- `RegisterTimeSlots()` - Time picker slots
- `RegisterQuickButtons()` - Quick action buttons
- `RegisterRangeTimeSpinners()` - Hour/minute spinners
- `RegisterClearButton()` - Clear button
- `RegisterWeekNumbers()` - Week number column

**Action Items:**
- [ ] Add missing registration methods for new area types
- [ ] Ensure naming convention: `"area_type_identifier"` (e.g., `"day_2025_10_15"`, `"nav_previous"`)
- [ ] Map to correct DateTimePickerHitArea enum values

---

### Phase 3: HitHandler Revision
For each hit handler, verify and correct:

#### 3.1 HitTest() Method
**Checklist:**
- [ ] Tests layout rectangles in correct priority order (navigation → day cells → time slots → buttons)
- [ ] Returns `DateTimePickerHitTestResult` with:
  - `IsHit = true` when hit
  - `HitArea` name matching registered name
  - `HitBounds` with the rectangle
  - `Date`, `Time`, or other context as applicable
  - Proper enum mapping in result processing
- [ ] Handles both single-month and multi-month layouts (MonthGrids)
- [ ] Correctly calculates dates from cell indices using `FirstDayOfWeek`

#### 3.2 HandleClick() Method
**Checklist:**
- [ ] Processes all hit areas returned by HitTest()
- [ ] Updates owner control state correctly:
  - Navigation: `owner.NavigateToPreviousMonth()` / `owner.NavigateToNextMonth()`
  - Day cells: `owner.SelectDate(date)` or range selection logic
  - Time slots: `owner.SelectTime(time)`
  - Quick buttons: Execute preset logic
  - Action buttons: Confirm/cancel/clear actions
- [ ] Returns `true` to close dropdown when appropriate (after date selection)
- [ ] Returns `false` for navigation/preview actions
- [ ] Fires appropriate events: `DateSelected`, `RangeSelected`, etc.

#### 3.3 UpdateHoverState() Method
**Checklist:**
- [ ] Updates `DateTimePickerHoverState` with hovered area type
- [ ] Stores hovered date/time/bounds for painter to highlight
- [ ] Properly uses `DateTimePickerHitArea` enum

---

## Hit Area Mapping by Painter Mode

### 1. Single
**Areas:** Header, PreviousButton, NextButton, DayCell, TodayButton
**Layout:** Single calendar grid (7×6 cells)

### 2. SingleWithTime
**Areas:** Header, PreviousButton, NextButton, DayCell, TimeSlot, TimeButton
**Layout:** Calendar grid + time slot list

### 3. Range
**Areas:** Header, PreviousButton, NextButton, DayCell, ApplyButton, CancelButton
**Layout:** Single or dual calendar grid with range highlighting

### 4. RangeWithTime
**Areas:** Header, PreviousButton, NextButton, DayCell, TimeSpinner, ApplyButton, CancelButton
**Layout:** Dual calendar + start/end time spinners

### 5. Multiple
**Areas:** Header, PreviousButton, NextButton, DayCell (with checkbox), ApplyButton, CancelButton
**Layout:** Calendar grid with checkable cells

### 6. Appointment
**Areas:** Header, PreviousButton, NextButton, DayCell, TimeSlot
**Layout:** Calendar grid + hourly time slot list

### 7. Timeline
**Areas:** Handle (start/end), TimelineTrack, DayCell (mini calendar)
**Layout:** Timeline bar with draggable handles + mini calendar

### 8. Quarterly
**Areas:** Header, PreviousButton, NextButton, QuarterButton, YearButton
**Layout:** Year selector + Q1/Q2/Q3/Q4 buttons

### 9. Compact
**Areas:** Header, PreviousButton, NextButton, DayCell, TodayButton
**Layout:** Minimal single calendar (smaller cells, less padding)

### 10. ModernCard
**Areas:** Header, PreviousButton, NextButton, DayCell, QuickButton (Today/Tomorrow/etc.)
**Layout:** Card-style with quick date buttons

### 11. DualCalendar
**Areas:** Header (2), PreviousButton, NextButton, DayCell (2 grids)
**Layout:** Side-by-side month calendars for range selection

### 12. WeekView
**Areas:** Header, PreviousButton, NextButton, WeekRow, WeekNumber
**Layout:** Week-based calendar with full row selection

### 13. MonthView
**Areas:** Header, PreviousButton, NextButton, MonthButton (12 buttons)
**Layout:** 3×4 grid of month selectors

### 14. YearView
**Areas:** Header, PreviousButton, NextButton, YearButton (12 buttons)
**Layout:** Grid of year selectors

### 15. SidebarEvent
**Areas:** Header, PreviousButton, NextButton, DayCell, CreateButton, ActionButton
**Layout:** Left sidebar (date/events) + right calendar

### 16. FlexibleRange
**Areas:** Header, PreviousButton, NextButton, DayCell, FlexibleRangeButton, ApplyButton
**Layout:** Tab selector + calendar + flexible range presets

### 17. FilteredRange
**Areas:** Header, PreviousButton, NextButton, DayCell, FilterButton, TimeSlot, ApplyButton
**Layout:** Filter sidebar + dual calendar + time selection

### 18. Header
**Areas:** Header (large), DayCell
**Layout:** Large colored header + compact calendar below

---

## Validation Criteria

For each painter/hit handler pair:

### Painter Validation
1. ✅ `CalculateLayout()` returns layout with all necessary rectangles
2. ✅ `PaintCalendar()` calls `CalculateLayout()` first
3. ✅ All painted interactive elements have corresponding layout rectangles
4. ✅ Multi-month layouts use `MonthGrids` collection correctly

### HitTestHelper Validation
1. ✅ All layout rectangles are registered via `_owner._hitTest.AddHitArea()`
2. ✅ Hit area names follow convention: `"{type}_{identifier}"`
3. ✅ Registration methods handle single-month and multi-month layouts
4. ✅ DateTimePickerHitArea enum values are implicitly used via naming

### HitHandler Validation
1. ✅ `HitTest()` covers all registered hit areas
2. ✅ `HitTest()` returns correct `DateTimePickerHitTestResult` with context
3. ✅ `HandleClick()` processes all hit area types
4. ✅ `HandleClick()` updates owner state and fires events correctly
5. ✅ `UpdateHoverState()` uses `DateTimePickerHitArea` enum
6. ✅ Returns correct close/stay-open behavior

---

## Implementation Order (Priority)

### Tier 1 - Core Modes (Most Used)
1. **Single** - Foundation for others
2. **Compact** - Common in dropdowns
3. **SingleWithTime** - Extends Single
4. **Range** - Important for range selection

### Tier 2 - Advanced Modes
5. **RangeWithTime**
6. **DualCalendar**
7. **ModernCard**
8. **Appointment**

### Tier 3 - Specialized Modes
9. **Multiple**
10. **WeekView**
11. **MonthView**
12. **YearView**

### Tier 4 - Complex Modes
13. **Timeline**
14. **Quarterly**
15. **FlexibleRange**
16. **FilteredRange**
17. **SidebarEvent**
18. **Header**

---

## Testing Checklist (Per Pair)

### Functional Tests
- [ ] All navigation buttons respond correctly
- [ ] All day cells respond to clicks
- [ ] Date selection updates control state
- [ ] Time selection works (if applicable)
- [ ] Quick buttons execute actions
- [ ] Apply/Cancel buttons work
- [ ] Clear button works
- [ ] Range selection highlights correctly
- [ ] Multi-selection works (Multiple mode)
- [ ] Drag handles work (Timeline mode)

### Visual Tests
- [ ] Hover states display correctly
- [ ] Pressed states display correctly
- [ ] Selected dates highlight properly
- [ ] Hit areas align with visual elements
- [ ] No gaps or overlaps in hit detection

### Integration Tests
- [ ] Works with all BeepTheme styles
- [ ] Respects MinDate/MaxDate constraints
- [ ] FirstDayOfWeek setting works
- [ ] Custom formats display correctly
- [ ] Events fire with correct data

---

## Common Issues & Fixes

### Issue 1: Hit Area Not Registering
**Cause:** Layout rectangle is Empty or not populated
**Fix:** Ensure `CalculateLayout()` sets all rectangles before returning

### Issue 2: Hit Test Returns Wrong Date
**Cause:** Incorrect cell-to-date calculation
**Fix:** Use `FirstDayOfWeek` correctly and handle month boundaries

### Issue 3: Click Does Nothing
**Cause:** `HandleClick()` doesn't process hit area name
**Fix:** Add case for hit area name pattern (e.g., `"day_*"`, `"nav_*"`)

### Issue 4: Hover State Not Updating
**Cause:** `UpdateHoverState()` not called or incorrect enum mapping
**Fix:** Ensure hit handler updates hover state with correct `DateTimePickerHitArea`

### Issue 5: Multi-Month Layout Broken
**Cause:** Using `DayCellRects` instead of `MonthGrids` collection
**Fix:** Use `MonthGrids` for dual-calendar and range modes

---

## Documentation Requirements

For each painter:
- [ ] XML doc explaining layout structure
- [ ] XML doc listing supported hit areas
- [ ] Code comments in `CalculateLayout()` explaining rectangle calculations

For each hit handler:
- [ ] XML doc explaining hit test logic
- [ ] XML doc listing handled click actions
- [ ] Code comments explaining date/time calculations

---

## Success Criteria

✅ All 18 painter/hit handler pairs pass validation
✅ All hit areas properly registered using `BeepDateTimePickerHitTestHelper`
✅ All hit handlers use `DateTimePickerHitArea` enum correctly
✅ Zero click detection failures
✅ 100% visual-to-interactive alignment
✅ All modes work correctly with BaseControl's hit test system

---

## Revision Workflow (Per Pair)

```
1. READ painter's CalculateLayout()
   └─> Identify all interactive rectangles
   └─> Note any missing rectangles

2. READ painter's PaintCalendar()
   └─> Identify all painted interactive elements
   └─> Verify each has a layout rectangle

3. VERIFY BeepDateTimePickerHitTestHelper
   └─> Check registration methods cover all areas
   └─> Add missing registration methods if needed

4. READ hit handler's HitTest()
   └─> Verify covers all registered areas
   └─> Check date/time calculation logic
   └─> Ensure correct enum mapping

5. READ hit handler's HandleClick()
   └─> Verify processes all hit area names
   └─> Check owner state updates
   └─> Verify close/stay-open logic

6. READ hit handler's UpdateHoverState()
   └─> Verify uses DateTimePickerHitArea enum
   └─> Check hover state updates

7. UPDATE painter if needed
   └─> Fix CalculateLayout()
   └─> Add missing rectangles
   └─> Update documentation

8. UPDATE hit handler if needed
   └─> Fix HitTest()
   └─> Fix HandleClick()
   └─> Fix UpdateHoverState()
   └─> Update documentation

9. TEST the pair
   └─> Manual click testing
   └─> Visual alignment verification
   └─> Edge case testing

10. DOCUMENT completion
    └─> Mark validation checklist complete
    └─> Note any special considerations
```

---

## Notes

- **BeepDateTimePickerHitTestHelper** is the bridge between painter layouts and BaseControl's hit test system
- **DateTimePickerHitArea enum** provides standardized area type identification
- Painters MUST call `CalculateLayout()` before painting
- HitTestHelper MUST be called after `CalculateLayout()` and before `PaintCalendar()`
- All hit area names follow pattern: `"{type}_{identifier}"` (e.g., `"day_2025_10_15"`, `"nav_previous"`)
- Multi-month layouts use `MonthGrids` collection, not flat `DayCellRects`
- Hit handlers determine close/stay-open behavior via return value from `HandleClick()`

---

**Document Version:** 1.0
**Date:** October 17, 2025
**Status:** PLAN READY FOR EXECUTION
