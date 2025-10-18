# Painter 2D Array to List<Rectangle> Migration Plan

## Date: October 16, 2025

## Painters Requiring Updates

All painters below use the old `Rectangle[6,7]` 2D array format and need to be updated to `List<Rectangle>` for compatibility with BeepDateTimePickerHitTestHelper.

### Critical (Dual/Multi-Month Layouts)
1. ✅ **DualCalendarDateTimePickerPainter** - Needs full rewrite with MonthGrids
2. ⚠️ **FlexibleRangeDateTimePickerPainter** - Uses 2D arrays for both calendars
3. ⚠️ **FilteredRangeDateTimePickerPainter** - Uses 2D arrays for both calendars

### High Priority (Single Month with 2D Arrays)
4. **HeaderDateTimePickerPainter** - Uses `Rectangle[6,7]`
5. **CompactDateTimePickerPainter** - Uses `Rectangle[6,7]`
6. **AppointmentDateTimePickerPainter** - Uses `Rectangle[6,7]`
7. **MultipleDateTimePickerPainter** - Uses `Rectangle[6,7]`

## Migration Pattern

### Old Code (2D Array):
```csharp
layout.DayCellRects = new Rectangle[6, 7];

for (int row = 0; row < 6; row++)
{
    for (int col = 0; col < 7; col++)
    {
        layout.DayCellRects[row, col] = new Rectangle(
            calendarRect.X + col * cellWidth,
            calendarRect.Y + row * cellHeight,
            cellWidth,
            cellHeight
        );
    }
}

// Usage:
var cellRect = layout.DayCellRects[row, col];
```

### New Code (List):
```csharp
layout.DayCellRects = new List<Rectangle>();

for (int row = 0; row < 6; row++)
{
    for (int col = 0; col < 7; col++)
    {
        layout.DayCellRects.Add(new Rectangle(
            calendarRect.X + col * cellWidth,
            calendarRect.Y + row * cellHeight,
            cellWidth,
            cellHeight
        ));
    }
}

// Usage:
int index = row * 7 + col;
var cellRect = layout.DayCellRects[index];
```

### Helper Method (Add to each painter):
```csharp
/// <summary>
/// Converts row/column grid coordinates to flat list index
/// </summary>
private int GetCellIndex(int row, int col)
{
    return row * 7 + col;
}
```

## Dual-Month Painters (Special Case)

For painters showing 2 months side-by-side (FlexibleRange, FilteredRange, DualCalendar):

### New Approach - Use MonthGrids:
```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    layout.MonthGrids = new List<CalendarMonthGrid>();
    
    // Left calendar
    var leftGrid = new CalendarMonthGrid();
    leftGrid.GridRect = new Rectangle(...);
    leftGrid.DayCellRects = new List<Rectangle>();
    for (int i = 0; i < 42; i++)  // 6 rows × 7 cols
    {
        leftGrid.DayCellRects.Add(new Rectangle(...));
    }
    layout.MonthGrids.Add(leftGrid);
    
    // Right calendar
    var rightGrid = new CalendarMonthGrid();
    rightGrid.GridRect = new Rectangle(...);
    rightGrid.DayCellRects = new List<Rectangle>();
    for (int i = 0; i < 42; i++)
    {
        rightGrid.DayCellRects.Add(new Rectangle(...));
    }
    layout.MonthGrids.Add(rightGrid);
    
    return layout;
}
```

## Migration Steps

For each painter:

1. **Find all `Rectangle[6,7]` declarations** → Change to `List<Rectangle>()`
2. **Find all `[row, col]` assignments** → Change to `.Add(...)` or `[index]`
3. **Find all `[row, col]` reads** → Change to `[GetCellIndex(row, col)]`
4. **Add helper method** `GetCellIndex(row, col)`
5. **Update HitTest method** to use list indexing
6. **Test painter** with BeepDateTimePicker

## Quick Reference

### Index Conversion Formula:
```
index = row * 7 + col

Examples:
- Row 0, Col 0 → index 0  (first cell, Sunday Week 1)
- Row 0, Col 6 → index 6  (last cell of Week 1, Saturday)
- Row 1, Col 0 → index 7  (first cell of Week 2)
- Row 5, Col 6 → index 41 (last cell, Week 6 Saturday)
```

### Reverse Formula (if needed):
```
row = index / 7
col = index % 7
```

## Verification Checklist

After updating each painter:

- [ ] CalculateLayout() populates List<Rectangle>, not 2D array
- [ ] All cell accesses use list indexing
- [ ] HitTest() works with list indexing
- [ ] Paint methods iterate correctly
- [ ] No compilation errors
- [ ] Hit test helper can register all cells
- [ ] Click interaction works
- [ ] Hover effects work

## Status Tracking

| Painter | Status | Notes |
|---------|--------|-------|
| DualCalendarDateTimePickerPainter | ⏳ Pending | Needs MonthGrids approach |
| FlexibleRangeDateTimePickerPainter | ⏳ Pending | Needs MonthGrids approach |
| FilteredRangeDateTimePickerPainter | ⏳ Pending | Needs MonthGrids approach |
| HeaderDateTimePickerPainter | ⏳ Pending | Simple list conversion |
| CompactDateTimePickerPainter | ⏳ Pending | Simple list conversion |
| AppointmentDateTimePickerPainter | ⏳ Pending | Simple list conversion |
| MultipleDateTimePickerPainter | ⏳ Pending | Simple list conversion |

## Next Actions

1. Start with single-month painters (simpler conversion)
2. Then tackle dual-month painters with MonthGrids
3. Test each painter after conversion
4. Update analysis document with results
