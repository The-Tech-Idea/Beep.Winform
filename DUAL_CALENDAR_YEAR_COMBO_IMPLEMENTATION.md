# Dual Calendar Year Combo Box Implementation

## Overview
The DualCalendarDateTimePickerPainter displays **two side-by-side calendars** for range selection. Each calendar now has its own **independent year combo box** that correctly navigates while maintaining the dual calendar relationship.

## Architecture

### Calendar Layout
```
┌─────────────────────────────────────────────────┐
│  [◄] [2024 ▼]  [►]     │     [2024 ▼]          │
│  Su Mo Tu We Th Fr Sa   │  Su Mo Tu We Th Fr Sa │
│  Left Calendar          │  Right Calendar       │
│  (DisplayMonth)         │  (DisplayMonth + 1)   │
└─────────────────────────────────────────────────┘
```

- **Left Calendar (gridIndex = 0)**: Shows `DisplayMonth`
- **Right Calendar (gridIndex = 1)**: Shows `DisplayMonth.AddMonths(1)`
- **Navigation Buttons**: Only on left calendar (both prev/next affect both calendars)
- **Year Combo Boxes**: On BOTH calendars, independently clickable

## Key Components

### 1. DateTimePickerHoverState (IDateTimePickerPainter.cs)
**Added Properties**:
```csharp
public int HoveredGridIndex { get; set; } = -1;   // Tracks which calendar is hovered
public int PressedGridIndex { get; set; } = -1;   // Tracks which calendar is pressed
```

**Purpose**: Distinguish between hovering/clicking the left vs right year combo box.

### 2. DualCalendarDateTimePickerPainter.cs
**Updated Methods**:

#### PaintCalendar()
- Passes `gridIndex` (0 for left, 1 for right) to `PaintSingleCalendar()`
- Ensures each calendar knows its position

#### PaintSingleCalendar()
```csharp
private void PaintSingleCalendar(Graphics g, Rectangle bounds, DateTime displayMonth, 
    DateTimePickerProperties properties, DateTimePickerHoverState hoverState, 
    bool showNavigation, int gridIndex)
```
- Added `gridIndex` parameter
- Passes it to `PaintYearComboBox()`

#### PaintYearComboBox()
```csharp
private void PaintYearComboBox(Graphics g, Rectangle bounds, int selectedYear, 
    DateTimePickerHoverState hoverState, int gridIndex)
```
- Checks if hover/press matches THIS specific gridIndex
- Only highlights if `hoverState.HoveredGridIndex == gridIndex`

### 3. DualCalendarDateTimePickerHitHandler.cs
**Updated Methods**:

#### HitTest()
- Already returns `hitResult.GridIndex` from MonthGrids
- Each grid has its own `YearComboBoxRect`

#### UpdateHoverState()
```csharp
hoverState.HoveredGridIndex = hitResult.GridIndex;  // Track which calendar
```

#### ShowYearComboBox()
**Critical Logic**:
```csharp
// Which calendar was clicked?
int gridIndex = hitResult.GridIndex;  // 0 = left, 1 = right

// What month does that calendar show?
DateTime targetMonth = owner.DisplayMonth.AddMonths(gridIndex);

// User selects year X
int selectedYear = X;
DateTime newTargetMonth = new DateTime(selectedYear, targetMonth.Month, 1);

// Calculate desired DisplayMonth:
// - If left calendar (gridIndex 0): DisplayMonth = newTargetMonth
// - If right calendar (gridIndex 1): DisplayMonth = newTargetMonth - 1 month
DateTime desiredDisplayMonth = newTargetMonth.AddMonths(-gridIndex);

// Navigate to desiredDisplayMonth
```

## Example Scenarios

### Scenario 1: Click Left Calendar Year Combo Box
**Current State**:
- DisplayMonth = Jan 2024
- Left shows: Jan 2024
- Right shows: Feb 2024

**User Action**: Click left year combo → Select "2025"

**Calculation**:
```csharp
gridIndex = 0
targetMonth = Jan 2024 + 0 months = Jan 2024
selectedYear = 2025
newTargetMonth = Jan 2025
desiredDisplayMonth = Jan 2025 - 0 months = Jan 2025
monthDiff = Jan 2025 - Jan 2024 = +12 months
→ Navigate forward 12 months
```

**Result**:
- DisplayMonth = Jan 2025
- Left shows: Jan 2025 ✓
- Right shows: Feb 2025 ✓

### Scenario 2: Click Right Calendar Year Combo Box
**Current State**:
- DisplayMonth = Jan 2024
- Left shows: Jan 2024
- Right shows: Feb 2024

**User Action**: Click right year combo → Select "2025"

**Calculation**:
```csharp
gridIndex = 1
targetMonth = Jan 2024 + 1 month = Feb 2024
selectedYear = 2025
newTargetMonth = Feb 2025
desiredDisplayMonth = Feb 2025 - 1 month = Jan 2025
monthDiff = Jan 2025 - Jan 2024 = +12 months
→ Navigate forward 12 months
```

**Result**:
- DisplayMonth = Jan 2025
- Left shows: Jan 2025 ✓
- Right shows: Feb 2025 ✓

### Scenario 3: Year Boundary (Right Calendar)
**Current State**:
- DisplayMonth = Dec 2023
- Left shows: Dec 2023
- Right shows: Jan 2024 (different year!)

**User Action**: Click right year combo → Select "2026"

**Calculation**:
```csharp
gridIndex = 1
targetMonth = Dec 2023 + 1 month = Jan 2024
selectedYear = 2026
newTargetMonth = Jan 2026
desiredDisplayMonth = Jan 2026 - 1 month = Dec 2025
monthDiff = Dec 2025 - Dec 2023 = +24 months
→ Navigate forward 24 months
```

**Result**:
- DisplayMonth = Dec 2025
- Left shows: Dec 2025 ✓
- Right shows: Jan 2026 ✓

## Range Selection Behavior
The year combo boxes **do NOT interfere** with range selection:
- Range selection uses `DayCell` hit area
- Year combo box uses `YearComboBox` hit area
- Both can be used together seamlessly
- Clicking year combo does NOT reset selected range dates

## Visual Feedback
- **Hover**: Only the hovered combo box (left or right) shows hover effect
- **Press**: Only the pressed combo box shows pressed effect
- **Border**: Always visible on both combo boxes
- **Dropdown Arrow**: Both combo boxes show dropdown indicator

## Files Modified
1. **IDateTimePickerPainter.cs**
   - Added `HoveredGridIndex` and `PressedGridIndex` to `DateTimePickerHoverState`

2. **DualCalendarDateTimePickerPainter.cs**
   - Added `gridIndex` parameter throughout paint methods
   - Updated year combo box hover/press detection

3. **DualCalendarDateTimePickerHitHandler.cs**
   - Updated `ShowYearComboBox()` with correct navigation logic
   - Set `HoveredGridIndex` in `UpdateHoverState()`

## Testing Checklist
- [x] Left year combo shows correct year
- [x] Right year combo shows correct year (can be different if year boundary)
- [x] Click left year combo → Both calendars update correctly
- [x] Click right year combo → Both calendars update correctly
- [x] Hover only highlights the specific combo box being hovered
- [x] Year boundary cases (Dec/Jan) work correctly
- [x] Range selection still works after year changes
- [x] No compilation errors

## Benefits
✅ **Independent Control**: Each calendar's year can be changed separately
✅ **Maintains Relationship**: Right calendar always stays DisplayMonth + 1
✅ **Visual Clarity**: Users can see different years in each calendar
✅ **Range Selection**: Seamlessly works across year boundaries
✅ **Consistent Behavior**: Matches other painters with year combo boxes
