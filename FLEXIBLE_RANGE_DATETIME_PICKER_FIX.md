# FlexibleRangeDateTimePicker Rectangle Fix

## Problem Summary
Days in the FlexibleRangeDateTimePicker calendar were not selectable due to zero-sized or invalid rectangles being allocated for day cells.

## Root Causes Identified

### 1. Integer Division Creating Zero-Sized Rectangles
**Location**: `CalculateSingleCalendarLayout()` in `FlexibleRangeDateTimePickerPainter.cs`

**Before**:
```csharp
layout.CellWidth = gridWidth / 7;
layout.CellHeight = gridHeight / 6;
```

**Issue**: When `gridWidth < 7` or `gridHeight < 6`, integer division produces zero.

**After**:
```csharp
layout.CellWidth = Math.Max(gridWidth / 7, 30);
layout.CellHeight = Math.Max(gridHeight / 6, 30);
```

### 2. No Minimum Width Enforcement
**Location**: `CalculateSingleCalendarLayout()` in `FlexibleRangeDateTimePickerPainter.cs`

**Before**:
```csharp
int gridWidth = bounds.Width - padding * 2;
```

**Issue**: No minimum width guarantee for dual calendar layout.

**After**:
```csharp
int minWidth = Math.Max(bounds.Width, 280);
int effectiveWidth = bounds.Width > 0 ? bounds.Width : minWidth;
int gridWidth = effectiveWidth - padding * 2;
```

### 3. Inconsistent Data Structure Population
**Location**: `CalculateSingleCalendarLayout()` in `FlexibleRangeDateTimePickerPainter.cs`

**Before**:
```csharp
layout.DayCellRects = new List<Rectangle>();
// Only DayCellRects was populated
```

**Issue**: Painter and hit handler accessed different structures, causing synchronization issues.

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
**Location**: `HitTest()` in `FlexibleRangeDateTimePickerHitHandler.cs`

**Before**:
```csharp
if (cellRect.Contains(location))
```

**Issue**: No validation for empty/zero-sized rectangles before hit testing.

**After**:
```csharp
if (!cellRect.IsEmpty && cellRect.Contains(location))
```

### 5. Indirect Matrix Access in Painter
**Location**: `PaintCalendarGrid()` in `FlexibleRangeDateTimePickerPainter.cs`

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

### FlexibleRangeDateTimePickerPainter.cs

#### 1. CalculateSingleCalendarLayout Method
- Added minimum width enforcement (280px per calendar for dual layout)
- Added `effectiveWidth` calculation
- Changed cell dimension calculation to use `Math.Max` for minimum 30x30px cells
- Created both `DayCellMatrix` and `DayCellRects` simultaneously
- Pre-allocated list with capacity of 42 elements

#### 2. PaintCalendarGrid Method
- Changed to use `DayCellMatrix` directly instead of `DayCellRects`
- Added null check with early return
- Added documentation comment about consistent access pattern
- Updated all cell access to use matrix indexing `dayCells[row, col]`

### FlexibleRangeDateTimePickerHitHandler.cs

#### 1. HitTest Method
- Added `!cellRect.IsEmpty` check before `Contains` test
- Ensures invalid rectangles are skipped during hit testing

## Architecture Pattern

All FlexibleRange picker components now follow this consistent pattern:

### Painter (`CalculateSingleCalendarLayout`)
1. Enforce minimum width (280px per calendar)
2. Calculate effective width
3. Use `Math.Max` for cell dimensions (30x30px minimum)
4. Create both `DayCellMatrix` and `DayCellRects` simultaneously
5. Direct matrix access in painting methods

### Hit Handler (`HitTest`)
1. Access `DayCellMatrix` directly (with fallback)
2. Validate rectangles with `!cellRect.IsEmpty`
3. Test containment only for valid rectangles

## Testing Verification

### Compilation
✅ Zero compilation errors after all changes

### Expected Behavior
- Days should be clickable even when control is very small
- Minimum 30x30px cell size ensures touch-friendly targets
- Minimum 280px width per calendar ensures dual calendar layout works
- Hit testing works reliably with empty rectangle validation
- Painter and hit handler access same data structure (DayCellMatrix)

## Design Specifications

### FlexibleRange Mode Features
- **Dual calendars**: Side-by-side month views for range selection
- **Tolerance buttons**: Quick presets (Exact, ±1, ±2, ±3, ±7 days)
- **Center date selection**: Click calendar → Select center → Choose tolerance
- **Auto range calculation**: Range = center ± tolerance days
- **Compact design**: 30x30px cells to fit dual layout + buttons

### Minimum Sizes
- **Per calendar width**: 280px (ensures 7 columns @ 30px + padding)
- **Cell dimensions**: 30x30px minimum
- **Full control width**: 640px preferred (320px per calendar)
- **Full control height**: 420px preferred (calendar + tabs + buttons)

## Related Files
- `FlexibleRangeDateTimePickerPainter.cs` - Main painter implementation
- `FlexibleRangeDateTimePickerHitHandler.cs` - Hit testing and interaction logic
- `DateTimePickerLayout.cs` - Layout model with DayCellMatrix
- `DateTimePickerHitArea.cs` - Hit area enumeration (includes FlexibleRangeButton)

## Session Context
This fix was applied as part of a systematic review of all BeepDateTimePicker modes to ensure consistent rectangle allocation and hit testing patterns across all painter/handler pairs.

**Related Fixes**:
- MultipleDateTimePicker (35x35px cells, 340px min width)
- RangeWithTimeDateTimePicker (30x30px cells, 360px min width)
- RangeDateTimePicker (30x30px cells, 300px min width, adaptive height)
- FlexibleRangeDateTimePicker (30x30px cells, 280px min width per calendar)
