# DateTimePicker Painters - CalculateLayout Missing Code Implementation Plan

## Overview
This document provides a comprehensive plan to implement missing layout calculations in DateTimePicker painters. The `CalculateLayout()` method must calculate and populate all Rectangle fields used during painting and hit testing.

## Status Summary

### ✅ COMPLETE IMPLEMENTATIONS (16 painters)
These painters have fully implemented `CalculateLayout()` methods with all required rectangles:

1. **SingleDateTimePickerPainter** ✅
   - HeaderRect, DayNamesRect, CalendarGridRect, DayCellMatrix[6,7]
   - PreviousButtonRect, NextButtonRect, TodayButtonRect

2. **CompactDateTimePickerPainter** ✅
   - HeaderRect, PreviousButtonRect, NextButtonRect
   - DayNamesRect, CalendarGridRect, DayCellMatrix[6,7]

3. **RangeDateTimePickerPainter** ✅
   - Dual calendar layout with MonthGrids (2 grids)
   - Each grid: HeaderRect, DayNamesRect, CalendarGridRect, DayCellRects
   - Navigation buttons, Apply/Cancel buttons

4. **SingleWithTimeDateTimePickerPainter** ✅
   - Calendar section: HeaderRect, DayNamesRect, CalendarGridRect, DayCellMatrix[6,7]
   - Time picker section: TimePickerRect with hour/minute spinners

5. **RangeWithTimeDateTimePickerPainter** ✅
   - Dual calendar layout
   - Start time: StartTimeHourRect, StartTimeMinuteRect, StartTimeColonRect
   - End time: EndTimeHourRect, EndTimeMinuteRect, EndTimeColonRect
   - 8 spinner buttons (up/down for start/end hour/minute)

6. **MultipleDateTimePickerPainter** ✅
   - HeaderRect, DayNamesRect, CalendarGridRect
   - DayCellRects (List), SelectedDateChips, ClearButtonRect

7. **AppointmentDateTimePickerPainter** ✅
   - HeaderRect, DayNamesRect, CalendarGridRect, DayCellMatrix[6,7]
   - TimeSlotRects (List) for hourly slots

8. **TimelineDateTimePickerPainter** ✅
   - HeaderRect, TimelineRect, CalendarGridRect, DayCellMatrix[6,7]
   - EventMarkerRects (List)

9. **QuarterlyDateTimePickerPainter** ✅
   - HeaderRect, YearSelectorRect
   - QuickDateButtons (List) for Q1-Q4 quarters

10. **ModernCardDateTimePickerPainter** ✅
    - HeaderRect, QuickDateButtons (List), CalendarGridRect
    - DayCellMatrix[6,7], ActionButtonRects

11. **DualCalendarDateTimePickerPainter** ✅
    - MonthGrids (2 grids for side-by-side calendars)
    - Each grid: HeaderRect, DayNamesRect, CalendarGridRect, DayCellRects

12. **WeekViewDateTimePickerPainter** ✅
    - HeaderRect, WeekNumberColumnRect, CalendarGridRect
    - DayCellRects[6,7], WeekNumberRects (List)

13. **SidebarEventDateTimePickerPainter** ✅
    - Split layout: Sidebar (40%) + Calendar (60%)
    - HeaderRect, PreviousButtonRect, NextButtonRect
    - DayNamesRect, CalendarGridRect, DayCellMatrix[6,7]

14. **FlexibleRangeDateTimePickerPainter** ✅
    - Tab selector, Calendar area, Quick buttons
    - CalendarGridRect, DayCellRects (List)
    - QuickDateButtons for tolerance presets

15. **FilteredRangeDateTimePickerPainter** ✅
    - Sidebar with filters, Dual calendar layout
    - CalendarGridRect, DayCellRects (List)

16. **HeaderDateTimePickerPainter** ✅
    - Custom header area (80px), Calendar section below
    - HeaderRect, DayNamesRect, CalendarGridRect, DayCellMatrix[6,7]

---

## ❌ INCOMPLETE IMPLEMENTATIONS (2 painters)

### 1. MonthViewDateTimePickerPainter - **CRITICAL FIX REQUIRED**

**Current State:**
```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    
    // Register all hit areas with BaseControl's hit test system  
    _owner.HitTestHelper?.RegisterHitAreas(layout, properties);
    
    return layout; // EMPTY LAYOUT!
}
```

**Paint Method Requirements Analysis:**
From `PaintCalendar()` method, we see:
1. Year header with navigation buttons (PaintYearHeader)
2. Month grid 3x4 layout (PaintMonthGrid)
3. Previous/Next year navigation buttons
4. 12 month cells in 4 rows x 3 columns

**Required Layout Rectangles:**
```csharp
- HeaderRect (year display area)
- PreviousYearButtonRect (left navigation)
- NextYearButtonRect (right navigation)
- MonthGridRect (3x4 grid container)
- MonthCellRects (12 rectangles or 4x3 matrix)
```

**Implementation Plan:**
```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    
    int padding = 20;
    int currentY = bounds.Y + padding;
    
    // 1. Year header with navigation (50px height)
    layout.HeaderRect = new Rectangle(
        bounds.X + padding, 
        currentY, 
        bounds.Width - padding * 2, 
        50
    );
    
    // 2. Navigation buttons (36x36 each)
    int buttonSize = 36;
    layout.PreviousButtonRect = new Rectangle(
        bounds.X + padding, 
        currentY + 2, 
        buttonSize, 
        buttonSize
    );
    
    layout.NextButtonRect = new Rectangle(
        bounds.Right - padding - buttonSize, 
        currentY + 2, 
        buttonSize, 
        buttonSize
    );
    
    currentY += 70; // Move past header
    
    // 3. Month grid area (3 cols x 4 rows)
    var monthGridRect = new Rectangle(
        bounds.X + padding, 
        currentY, 
        bounds.Width - padding * 2, 
        bounds.Height - currentY - padding
    );
    
    int rows = 4;
    int cols = 3;
    int gap = 12;
    
    int cellWidth = (monthGridRect.Width - gap * (cols - 1)) / cols;
    int cellHeight = (monthGridRect.Height - gap * (rows - 1)) / rows;
    
    // 4. Create month cell rectangles (12 months)
    layout.MonthCellRects = new List<Rectangle>();
    
    for (int month = 1; month <= 12; month++)
    {
        int row = (month - 1) / cols;
        int col = (month - 1) % cols;
        
        var cellRect = new Rectangle(
            monthGridRect.X + col * (cellWidth + gap),
            monthGridRect.Y + row * (cellHeight + gap),
            cellWidth,
            cellHeight
        );
        
        layout.MonthCellRects.Add(cellRect);
    }
    
    // Register all hit areas with BaseControl's hit test system
    _owner.HitTestHelper?.RegisterHitAreas(layout, properties);
    
    return layout;
}
```

**Hit Handler Updates Required:**
After implementing layout, update `MonthViewDateTimePickerHitHandler.HitTest()` to:
1. Check PreviousYearButton / NextYearButton hit areas
2. Check MonthButton hit areas (use enum DateTimePickerHitArea.MonthButton)
3. Return clicked month index in result

---

### 2. YearViewDateTimePickerPainter - **CRITICAL FIX REQUIRED**

**Current State:**
```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    
    // Register all hit areas with BaseControl's hit test system
    _owner.HitTestHelper?.RegisterHitAreas(layout, properties);
    
    return layout; // EMPTY LAYOUT!
}
```

**Paint Method Requirements Analysis:**
From `PaintCalendar()` method, we see:
1. Decade header with navigation (e.g., "2020 — 2029")
2. Year grid 4x3 layout showing 12 years (decade-1 to decade+10)
3. Previous/Next decade navigation buttons
4. 12 year cells in 4 rows x 3 columns

**Required Layout Rectangles:**
```csharp
- HeaderRect (decade range display area)
- PreviousDecadeButtonRect (left navigation)
- NextDecadeButtonRect (right navigation)
- YearGridRect (4x3 grid container)
- YearCellRects (12 rectangles or 4x3 matrix)
```

**Implementation Plan:**
```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    
    int padding = 20;
    int currentY = bounds.Y + padding;
    
    // 1. Decade header with navigation (50px height)
    layout.HeaderRect = new Rectangle(
        bounds.X + padding, 
        currentY, 
        bounds.Width - padding * 2, 
        50
    );
    
    // 2. Navigation buttons (36x36 each)
    int buttonSize = 36;
    layout.PreviousButtonRect = new Rectangle(
        bounds.X + padding, 
        currentY + 2, 
        buttonSize, 
        buttonSize
    );
    
    layout.NextButtonRect = new Rectangle(
        bounds.Right - padding - buttonSize, 
        currentY + 2, 
        buttonSize, 
        buttonSize
    );
    
    currentY += 70; // Move past header
    
    // 3. Year grid area (3 cols x 4 rows = 12 years)
    var yearGridRect = new Rectangle(
        bounds.X + padding, 
        currentY, 
        bounds.Width - padding * 2, 
        bounds.Height - currentY - padding
    );
    
    int rows = 4;
    int cols = 3;
    int gap = 12;
    
    int cellWidth = (yearGridRect.Width - gap * (cols - 1)) / cols;
    int cellHeight = (yearGridRect.Height - gap * (rows - 1)) / rows;
    
    // 4. Create year cell rectangles (12 years)
    layout.YearCellRects = new List<Rectangle>();
    
    for (int i = 0; i < 12; i++)
    {
        int row = i / cols;
        int col = i % cols;
        
        var cellRect = new Rectangle(
            yearGridRect.X + col * (cellWidth + gap),
            yearGridRect.Y + row * (cellHeight + gap),
            cellWidth,
            cellHeight
        );
        
        layout.YearCellRects.Add(cellRect);
    }
    
    // Register all hit areas with BaseControl's hit test system
    _owner.HitTestHelper?.RegisterHitAreas(layout, properties);
    
    return layout;
}
```

**Hit Handler Updates Required:**
After implementing layout, update `YearViewDateTimePickerHitHandler.HitTest()` to:
1. Check PreviousDecadeButton / NextDecadeButton hit areas
2. Check YearButton hit areas (may need new enum DateTimePickerHitArea.YearButton)
3. Return clicked year value in result

---

## DateTimePickerLayout Missing Properties

Check if `DateTimePickerLayout` class needs these additional properties:

```csharp
// For MonthViewDateTimePickerPainter
public List<Rectangle> MonthCellRects { get; set; }
public Rectangle PreviousYearButtonRect { get; set; }
public Rectangle NextYearButtonRect { get; set; }

// For YearViewDateTimePickerPainter
public List<Rectangle> YearCellRects { get; set; }
public Rectangle PreviousDecadeButtonRect { get; set; }
public Rectangle NextDecadeButtonRect { get; set; }
```

**Action Required:** Verify and add missing properties to `DateTimePickerLayout.cs`

---

## DateTimePickerHitArea Enum Updates

Check if enum needs these additional values:

```csharp
public enum DateTimePickerHitArea
{
    // ... existing values ...
    
    MonthButton,           // For MonthViewDateTimePickerPainter
    YearButton,            // For YearViewDateTimePickerPainter
    PreviousYearButton,    // Already added ✅
    NextYearButton,        // Already added ✅
    PreviousDecadeButton,  // Already added ✅
    NextDecadeButton,      // Already added ✅
}
```

**Status:** PreviousYearButton, NextYearButton, PreviousDecadeButton, NextDecadeButton already exist ✅
**Action Required:** Add MonthButton and YearButton if not present

---

## BeepDateTimePickerHitTestHelper Updates

Verify registration methods exist:

```csharp
// For MonthViewDateTimePickerPainter
public void RegisterMonthButton(Rectangle bounds, int monthIndex)
{
    _hitAreas.Add(new HitArea
    {
        Bounds = bounds,
        Area = DateTimePickerHitArea.MonthButton,
        Data = monthIndex
    });
}

// For YearViewDateTimePickerPainter
public void RegisterYearButton(Rectangle bounds, int year)
{
    _hitAreas.Add(new HitArea
    {
        Bounds = bounds,
        Area = DateTimePickerHitArea.YearButton,
        Data = year
    });
}
```

**Action Required:** Add these registration methods to `BeepDateTimePickerHitTestHelper.cs`

---

## Implementation Order

### Phase 1: Infrastructure Updates
1. ✅ Add missing properties to `DateTimePickerLayout.cs`
2. ✅ Add MonthButton and YearButton to `DateTimePickerHitArea` enum
3. ✅ Add RegisterMonthButton() and RegisterYearButton() to `BeepDateTimePickerHitTestHelper.cs`

### Phase 2: Painter Implementation
4. ⬜ Implement `MonthViewDateTimePickerPainter.CalculateLayout()`
5. ⬜ Implement `YearViewDateTimePickerPainter.CalculateLayout()`

### Phase 3: Hit Handler Updates
6. ⬜ Update `MonthViewDateTimePickerHitHandler.HitTest()` to use new layout
7. ⬜ Update `YearViewDateTimePickerHitHandler.HitTest()` to use new layout

### Phase 4: Testing
8. ⬜ Test MonthView picker - verify month selection and year navigation
9. ⬜ Test YearView picker - verify year selection and decade navigation
10. ⬜ Test hit testing - ensure all clickable areas respond correctly

---

## Validation Checklist

After implementation, verify:

- [ ] MonthViewDateTimePickerPainter layout has 12 month cell rectangles
- [ ] MonthViewDateTimePickerPainter has year navigation button rectangles
- [ ] YearViewDateTimePickerPainter layout has 12 year cell rectangles  
- [ ] YearViewDateTimePickerPainter has decade navigation button rectangles
- [ ] RegisterHitAreas() is called before returning layout
- [ ] Hit handlers can correctly identify clicked months/years
- [ ] Navigation buttons respond to clicks
- [ ] No compilation errors
- [ ] Paint methods render correctly using calculated layout

---

## Notes

1. **Layout Consistency**: Both painters follow similar patterns (3x4 grid, navigation buttons)
2. **Grid Layout**: 4 rows × 3 columns = 12 items (months or years)
3. **Gap Handling**: 12px gap between cells for visual spacing
4. **Button Size**: 36x36px for navigation buttons (consistent with other painters)
5. **Padding**: 20px padding around edges
6. **Header Height**: 50px for title display, 70px total with spacing

---

## Related Files

- `DateTimePickerLayout.cs` - Layout model class
- `DateTimePickerHitArea.cs` (enums.cs) - Hit area enum
- `BeepDateTimePickerHitTestHelper.cs` - Hit test registration
- `MonthViewDateTimePickerPainter.cs` - Month picker painter
- `YearViewDateTimePickerPainter.cs` - Year picker painter
- `MonthViewDateTimePickerHitHandler.cs` - Month picker hit handler
- `YearViewDateTimePickerHitHandler.cs` - Year picker hit handler

---

**Last Updated:** October 18, 2025
**Status:** Ready for implementation
**Priority:** HIGH - These are the only 2 incomplete painters blocking full hit test functionality
