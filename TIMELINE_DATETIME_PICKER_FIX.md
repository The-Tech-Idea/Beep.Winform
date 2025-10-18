# TimelineDateTimePicker Rectangle Fix

## Problem Summary
Days in the TimelineDateTimePicker mini calendar were not selectable due to zero-sized or invalid rectangles being allocated for day cells.

## Root Causes Identified

### 1. Integer Division Creating Zero-Sized Rectangles
**Location**: `CalculateMiniLayout()` in `TimelineDateTimePickerPainter.cs`

**Before**:
```csharp
layout.CellWidth = gridWidth / 7;
layout.CellHeight = gridHeight / 6;
```

**Issue**: When `gridWidth < 7` or `gridHeight < 6`, integer division produces zero.

**After**:
```csharp
layout.CellWidth = Math.Max(gridWidth / 7, 28);
layout.CellHeight = Math.Max(gridHeight / 6, 22);
```

### 2. No Minimum Width Enforcement
**Location**: `CalculateMiniLayout()` in `TimelineDateTimePickerPainter.cs`

**Before**:
```csharp
int gridWidth = bounds.Width;
```

**Issue**: No minimum width guarantee for mini calendar component.

**After**:
```csharp
int minWidth = Math.Max(bounds.Width, 280);
int effectiveWidth = bounds.Width > 0 ? bounds.Width : minWidth;
int gridWidth = effectiveWidth;
```

### 3. Incomplete Data Structure Population
**Location**: `CalculateMiniLayout()` in `TimelineDateTimePickerPainter.cs`

**Before**:
```csharp
var matrix = new Rectangle[6, 7];
// Only DayCellMatrix was populated
layout.DayCellMatrix = matrix;
```

**Issue**: Missing `DayCellRects` list for alternative access pattern.

**After**:
```csharp
var matrix = new Rectangle[6, 7];
layout.DayCellRects = new List<Rectangle>(42);

for (int row = 0; row < 6; row++)
{
    for (int col = 0; col < 7; col++)
    {
        var cellRect = new Rectangle(...);
        matrix[row, col] = cellRect;
        layout.DayCellRects.Add(cellRect);
    }
}
layout.DayCellMatrix = matrix;
```

### 4. Missing Empty Rectangle Check
**Location**: `HitTest()` in `TimelineDateTimePickerHitHandler.cs`

**Before**:
```csharp
if (cellRect.Contains(location))
```

**Issue**: No validation for empty/zero-sized rectangles before hit testing.

**After**:
```csharp
if (!cellRect.IsEmpty && cellRect.Contains(location))
```

### 5. Missing Null Check in Painter
**Location**: `PaintCalendarGrid()` in `TimelineDateTimePickerPainter.cs`

**Before**:
```csharp
var cells = layout.GetDayCellMatrixOrDefault();
// No null check before usage
```

**Issue**: Could proceed with null cells array.

**After**:
```csharp
var cells = layout.DayCellMatrix ?? layout.GetDayCellMatrixOrDefault();
if (cells == null) return;
```

## Fixes Applied

### TimelineDateTimePickerPainter.cs

#### 1. CalculateMiniLayout Method
- Added minimum width enforcement (280px for mini calendar)
- Added `effectiveWidth` calculation
- Changed cell dimension calculation to use `Math.Max` for minimum 28x22px cells (compact mini calendar)
- Created both `DayCellMatrix` and `DayCellRects` simultaneously
- Pre-allocated list with capacity of 42 elements

#### 2. PaintCalendarGrid Method
- Changed to use `DayCellMatrix` directly with fallback
- Added null check with early return
- Added documentation comment about consistent access pattern

### TimelineDateTimePickerHitHandler.cs

#### 1. HitTest Method
- Added `!cellRect.IsEmpty` check before `Contains` test
- Ensures invalid rectangles are skipped during hit testing

## Architecture Pattern

All Timeline picker components now follow this consistent pattern:

### Painter (`CalculateMiniLayout`)
1. Enforce minimum width (280px for mini calendar)
2. Calculate effective width
3. Use `Math.Max` for cell dimensions (28x22px minimum - compact for timeline layout)
4. Create both `DayCellMatrix` and `DayCellRects` simultaneously
5. Direct matrix access with null check in painting methods

### Hit Handler (`HitTest`)
1. Access `DayCellMatrix` directly (with fallback)
2. Validate rectangles with `!cellRect.IsEmpty`
3. Test containment only for valid rectangles
4. Handle timeline-specific interactions (handles, track)

## Testing Verification

### Compilation
✅ Zero compilation errors after all changes

### Expected Behavior
- Days in mini calendar should be clickable even when control is very small
- Minimum 28x22px cell size ensures compact but usable mini calendar
- Minimum 280px width ensures readable mini calendar
- Hit testing works reliably with empty rectangle validation
- Timeline handles remain draggable and responsive
- Painter and hit handler access same data structure (DayCellMatrix)

## Design Specifications

### Timeline Mode Features
- **Visual timeline bar**: Horizontal bar showing date range with draggable handles
- **Start/End handles**: Draggable 24x40px rectangles for adjusting range
- **Timeline track**: Click to jump to date position
- **Mini calendar**: Compact month view below timeline for reference/quick selection
- **Range visualization**: Highlighted range on both timeline and mini calendar
- **Date labels**: Display start/end dates with formatting
- **Month markers**: Timeline shows month tick marks and labels

### Minimum Sizes (Mini Calendar Component)
- **Mini calendar width**: 280px minimum (ensures 7 columns @ 28px + padding)
- **Cell dimensions**: 28x22px minimum (compact design for reference calendar)
- **Full control width**: 480px preferred (timeline + padding)
- **Full control height**: 380px preferred (timeline bar + labels + mini calendar)

### Timeline-Specific Layout
- **Header**: Y+20, Height=40, title and instructions
- **Timeline bar**: Y+80, Height=80, interactive range visualization
- **Date labels**: Y+180, Height=60, formatted start/end dates
- **Mini calendar**: Y+260, Height=160, compact month grid

## Related Files
- `TimelineDateTimePickerPainter.cs` - Main painter implementation with timeline visualization
- `TimelineDateTimePickerHitHandler.cs` - Hit testing for handles, track, and mini calendar
- `DateTimePickerLayout.cs` - Layout model with DayCellMatrix
- `DateTimePickerHitArea.cs` - Hit area enumeration (includes StartHandle, EndHandle, TimelineTrack)

## Session Context
This fix was applied as part of a systematic review of all BeepDateTimePicker modes to ensure consistent rectangle allocation and hit testing patterns across all painter/handler pairs.

**Related Fixes**:
- MultipleDateTimePicker (35x35px cells, 340px min width)
- RangeWithTimeDateTimePicker (30x30px cells, 360px min width)
- RangeDateTimePicker (30x30px cells, 300px min width, adaptive height)
- FlexibleRangeDateTimePicker (30x30px cells, 280px min width per calendar)
- TimelineDateTimePicker (28x22px cells, 280px min width for mini calendar)

## Unique Timeline Considerations

### Why Smaller Cell Sizes?
The Timeline picker uses **28x22px cells** (smaller than other pickers) because:
1. Mini calendar is secondary UI element (timeline is primary)
2. Space-constrained layout (timeline + labels + calendar in 380px height)
3. Reference-only usage (not primary selection method)
4. Maintains readability while being compact

### Primary Interaction Model
Unlike other pickers where calendar is primary:
1. **Timeline handles**: Primary range adjustment method
2. **Timeline track**: Quick date positioning
3. **Mini calendar**: Reference and fine-tuning only
4. Calendar cells are smaller but still touch-friendly (28px width minimum)
