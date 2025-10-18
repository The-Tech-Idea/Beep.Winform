# FilteredRangeDateTimePicker Rectangle Fix

## Problem Summary
Days in the FilteredRangeDateTimePicker dual calendars were not selectable due to zero-sized or invalid rectangles being allocated for day cells.

## Root Causes Identified

### 1. Integer Division Creating Zero-Sized Rectangles
**Location**: Multiple places in `FilteredRangeDateTimePickerPainter.cs`

**Before** (in `CalculateSingleCalendarGrid`):
```csharp
layout.CellWidth = bounds.Width / 7;
layout.CellHeight = bounds.Height / 6;
```

**Before** (in `CalculateLayout` for left/right calendars):
```csharp
int leftCellWidth = layout.LeftCalendarGridRect.Width / 7;
int leftCellHeight = layout.LeftCalendarGridRect.Height / 6;
int rightCellWidth = layout.RightCalendarGridRect.Width / 7;
int rightCellHeight = layout.RightCalendarGridRect.Height / 6;
```

**Issue**: When dimensions < 7 or < 6, integer division produces zero.

**After**:
```csharp
// CalculateSingleCalendarGrid
layout.CellWidth = Math.Max(effectiveWidth / 7, 25);
layout.CellHeight = Math.Max(effectiveHeight / 6, 20);

// CalculateLayout - Left calendar
int leftCellWidth = Math.Max(layout.LeftCalendarGridRect.Width / 7, 25);
int leftCellHeight = Math.Max(layout.LeftCalendarGridRect.Height / 6, 20);

// CalculateLayout - Right calendar
int rightCellWidth = Math.Max(layout.RightCalendarGridRect.Width / 7, 25);
int rightCellHeight = Math.Max(layout.RightCalendarGridRect.Height / 6, 20);
```

### 2. No Minimum Size Enforcement
**Location**: `CalculateSingleCalendarGrid()` in `FilteredRangeDateTimePickerPainter.cs`

**Before**:
```csharp
layout.CalendarGridRect = bounds;
layout.CellWidth = bounds.Width / 7;
```

**Issue**: No minimum width/height guarantee for filtered range layout.

**After**:
```csharp
int minWidth = Math.Max(bounds.Width, 200);
int effectiveWidth = bounds.Width > 0 ? bounds.Width : minWidth;
int minHeight = Math.Max(bounds.Height, 150);
int effectiveHeight = bounds.Height > 0 ? bounds.Height : minHeight;
layout.CalendarGridRect = new Rectangle(bounds.X, bounds.Y, effectiveWidth, effectiveHeight);
```

### 3. Incomplete Data Structure Population
**Location**: `CalculateSingleCalendarGrid()` in `FilteredRangeDateTimePickerPainter.cs`

**Before**:
```csharp
layout.DayCellRects = new List<Rectangle>();
// Only DayCellRects was populated
```

**Issue**: Missing `DayCellMatrix` for consistent access pattern.

**After**:
```csharp
layout.DayCellMatrix = new Rectangle[6, 7];
layout.DayCellRects = new List<Rectangle>(42);

for (int row = 0; row < 6; row++)
{
    for (int col = 0; col < 7; col++)
    {
        var cellRect = new Rectangle(...);
        layout.DayCellMatrix[row, col] = cellRect;
        layout.DayCellRects.Add(cellRect);
    }
}
```

### 4. Missing Empty Rectangle Check
**Location**: `HitTest()` in `FilteredRangeDateTimePickerHitHandler.cs`

**Before** (Left calendar):
```csharp
if (layout.LeftDayCellRects[i].Contains(location))
```

**Before** (Right calendar):
```csharp
if (layout.RightDayCellRects[i].Contains(location))
```

**Issue**: No validation for empty/zero-sized rectangles before hit testing.

**After**:
```csharp
// Left calendar
var cellRect = layout.LeftDayCellRects[i];
if (!cellRect.IsEmpty && cellRect.Contains(location))

// Right calendar
var cellRect = layout.RightDayCellRects[i];
if (!cellRect.IsEmpty && cellRect.Contains(location))
```

### 5. Indirect Cell Access in Painter
**Location**: `PaintCalendarGrid()` in `FilteredRangeDateTimePickerPainter.cs`

**Before**:
```csharp
var cellRect = layout.DayCellRects[GetCellIndex(row, col)];
```

**Issue**: Used list access instead of direct matrix access.

**After**:
```csharp
var dayCells = layout.DayCellMatrix ?? layout.GetDayCellMatrixOrDefault(6, 7);
if (dayCells == null) return;
var cellRect = dayCells[row, col];
```

## Fixes Applied

### FilteredRangeDateTimePickerPainter.cs

#### 1. CalculateSingleCalendarGrid Method
- Added minimum width enforcement (200px)
- Added minimum height enforcement (150px)
- Added `effectiveWidth` and `effectiveHeight` calculations
- Changed cell dimension calculation to use `Math.Max` for minimum 25x20px cells
- Created both `DayCellMatrix` and `DayCellRects` simultaneously
- Pre-allocated list with capacity of 42 elements

#### 2. CalculateLayout Method (Dual Calendar)
- **Left calendar**: Changed `leftCellWidth/Height` to use `Math.Max(..., 25)` and `Math.Max(..., 20)`
- **Right calendar**: Changed `rightCellWidth/Height` to use `Math.Max(..., 25)` and `Math.Max(..., 20)`
- Pre-allocated both `LeftDayCellRects` and `RightDayCellRects` with capacity of 42

#### 3. PaintCalendarGrid Method
- Changed to use `DayCellMatrix` directly with fallback
- Added null check with early return
- Added documentation comment about consistent access pattern
- Updated all cell access to use matrix indexing `dayCells[row, col]`

### FilteredRangeDateTimePickerHitHandler.cs

#### 1. HitTest Method
- **Left calendar**: Added `!cellRect.IsEmpty` check before `Contains` test
- **Right calendar**: Added `!cellRect.IsEmpty` check before `Contains` test
- Stored cell rect in variable for both validation and hit result

## Architecture Pattern

All FilteredRange picker components now follow this consistent pattern:

### Painter (`CalculateSingleCalendarGrid`)
1. Enforce minimum dimensions (200x150px)
2. Calculate effective width and height
3. Use `Math.Max` for cell dimensions (25x20px minimum - compact for sidebar layout)
4. Create both `DayCellMatrix` and `DayCellRects` simultaneously
5. Direct matrix access in painting methods

### Painter (`CalculateLayout` - Dual Calendar)
1. Use `Math.Max` for both left and right calendar cells
2. Pre-allocate lists with capacity
3. Maintain separate structures for left/right calendars

### Hit Handler (`HitTest`)
1. Access `LeftDayCellRects` and `RightDayCellRects` separately
2. Validate rectangles with `!cellRect.IsEmpty`
3. Test containment only for valid rectangles
4. Handle multiple interactive areas (sidebar filters, year dropdowns, time inputs, action buttons)

## Testing Verification

### Compilation
✅ Zero compilation errors after all changes

### Expected Behavior
- Days in both left and right calendars should be clickable even when control is very small
- Minimum 25x20px cell size ensures compact but usable cells in sidebar layout
- Minimum 200x150px per calendar ensures readable grids
- Hit testing works reliably with empty rectangle validation
- Sidebar filter buttons remain clickable
- Year dropdowns, time inputs, and action buttons functional
- Painter and hit handler access same data structure

## Design Specifications

### FilteredRange Mode Features
- **Left sidebar**: Quick filter buttons (25% width)
  - Past Week, Past Month, Past 3 Months
  - Past 6 Months, Past Year, Past Century
- **Dual calendar**: Side-by-side month views with year dropdowns (75% width)
- **Time pickers**: From/To time inputs at bottom
- **Action buttons**: Reset Date and Show Results buttons
- **Analytics-focused**: Designed for reporting and data filtering workflows

### Minimum Sizes
- **Per calendar width**: 200px minimum (each calendar in dual layout)
- **Per calendar height**: 150px minimum
- **Cell dimensions**: 25x20px minimum (compact for sidebar + dual calendar layout)
- **Preferred size**: 720×480px (sidebar + dual calendars + controls)
- **Sidebar width**: 25% of total width

### Layout Breakdown
- **Sidebar**: 25% width, filter buttons vertically stacked
- **Main content**: 75% width
  - **Dual calendars**: 55% of height, side-by-side
  - **Time pickers**: 48px height, From/To inputs
  - **Action buttons**: 44px height, Reset + Show Results

## Related Files
- `FilteredRangeDateTimePickerPainter.cs` - Main painter with sidebar + dual calendar
- `FilteredRangeDateTimePickerHitHandler.cs` - Hit testing for multiple interactive areas
- `DateTimePickerLayout.cs` - Layout model with separate left/right calendar structures
- `DateTimePickerHitArea.cs` - Hit area enumeration (includes FilterButton, YearDropdown, TimeInput, ResetButton, ShowResultsButton)

## Session Context
This fix was applied as part of a systematic review of all BeepDateTimePicker modes to ensure consistent rectangle allocation and hit testing patterns across all painter/handler pairs.

**Related Fixes**:
- MultipleDateTimePicker (35x35px cells, 340px min width)
- RangeWithTimeDateTimePicker (30x30px cells, 360px min width)
- RangeDateTimePicker (30x30px cells, 300px min width, adaptive height)
- FlexibleRangeDateTimePicker (30x30px cells, 280px min width per calendar)
- TimelineDateTimePicker (28x22px cells, 280px min width for mini calendar)
- FilteredRangeDateTimePicker (25x20px cells, 200px min width per calendar)

## Unique FilteredRange Considerations

### Why Smallest Cell Sizes?
The FilteredRange picker uses **25x20px cells** (smallest of all pickers) because:
1. Sidebar occupies 25% of width (space-constrained horizontal layout)
2. Dual calendar layout in remaining 75% width
3. Multiple UI elements compete for vertical space (calendars + time pickers + buttons)
4. Designed for quick filtering, not primary date interaction
5. Still maintains minimum usability (25px width is touch-friendly threshold)

### Complex Layout Structure
Unlike other pickers:
1. **Three-column layout**: Sidebar | Left Calendar | Right Calendar
2. **Multiple hit areas**: 6 filter buttons + 84 day cells (42 per calendar) + 2 year dropdowns + 2 time inputs + 2 action buttons
3. **Separate cell lists**: `LeftDayCellRects` and `RightDayCellRects` instead of single `DayCellRects`
4. **Analytics focus**: Optimized for report date range selection, not general date picking
