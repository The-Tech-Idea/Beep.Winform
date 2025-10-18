# DateTimePicker Painter & HitHandler Quick Reference

## Quick Start Guide

### For Each Painter/HitHandler Pair:

#### Step 1: Analyze Painter
```csharp
// Check CalculateLayout() method
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    
    // ✅ MUST populate ALL interactive rectangles:
    layout.HeaderRect = ...;
    layout.PreviousButtonRect = ...;
    layout.NextButtonRect = ...;
    layout.DayCellRects = ...;
    // ... etc.
    
    return layout;
}

// Check PaintCalendar() method
public void PaintCalendar(Graphics g, Rectangle bounds, ...)
{
    // ✅ MUST call CalculateLayout() FIRST
    var layout = CalculateLayout(bounds, properties);
    
    // Then paint using layout rectangles
    PaintHeader(g, layout.HeaderRect, ...);
    PaintNavigationButton(g, layout.PreviousButtonRect, ...);
    // ... etc.
}
```

#### Step 2: Verify HitTestHelper
```csharp
// BeepDateTimePicker.DrawContent() calls:
_hitHelper.RegisterHitAreas(_layout, props, _displayMonth);

// Which internally calls:
RegisterNavigationButtons(layout);
RegisterDayCells(layout, displayMonth, props);
RegisterTimeSlots(layout, props);
// ... etc.

// ✅ Verify registration methods exist for ALL your painter's areas
```

#### Step 3: Analyze HitHandler
```csharp
// Check HitTest() method
public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, ...)
{
    var result = new DateTimePickerHitTestResult();
    
    // ✅ Test ALL registered areas in priority order
    
    // 1. Navigation buttons (highest priority)
    if (layout.PreviousButtonRect.Contains(location))
    {
        result.IsHit = true;
        result.HitArea = "nav_previous";  // ✅ Must match registered name
        result.HitBounds = layout.PreviousButtonRect;
        return result;
    }
    
    // 2. Day cells
    if (layout.CalendarGridRect.Contains(location))
    {
        // Calculate which cell was hit
        DateTime date = GetDateForCell(...);
        result.IsHit = true;
        result.HitArea = $"day_{date:yyyy_MM_dd}";  // ✅ Must match registered name
        result.Date = date;
        result.HitBounds = cellRect;
        return result;
    }
    
    // 3. Other areas...
    
    return result;
}

// Check HandleClick() method
public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
{
    if (!hitResult.IsHit) return false;
    
    // ✅ Handle ALL hit area name patterns
    
    if (hitResult.HitArea == "nav_previous")
    {
        owner.NavigateToPreviousMonth();
        return false;  // ✅ Don't close on navigation
    }
    
    if (hitResult.HitArea.StartsWith("day_"))
    {
        owner.SelectDate(hitResult.Date.Value);
        owner.OnDateSelected(hitResult.Date.Value);
        return true;  // ✅ Close dropdown after date selection
    }
    
    // ... etc.
    
    return false;
}

// Check UpdateHoverState() method
public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
{
    if (!hitResult.IsHit)
    {
        hoverState.Clear();
        return;
    }
    
    // ✅ Map hit area name to DateTimePickerHitArea enum
    if (hitResult.HitArea == "nav_previous")
    {
        hoverState.HoveredArea = DateTimePickerHitArea.PreviousButton;
        hoverState.HoveredBounds = hitResult.HitBounds;
    }
    else if (hitResult.HitArea.StartsWith("day_"))
    {
        hoverState.HoveredArea = DateTimePickerHitArea.DayCell;
        hoverState.HoveredDate = hitResult.Date;
        hoverState.HoveredBounds = hitResult.HitBounds;
    }
    // ... etc.
}
```

---

## Common Patterns

### Pattern 1: Single-Month Calendar
```csharp
// Painter - CalculateLayout()
layout.DayCellRects = new List<Rectangle>();
for (int row = 0; row < 6; row++)
{
    for (int col = 0; col < 7; col++)
    {
        var cellRect = new Rectangle(x, y, cellWidth, cellHeight);
        layout.DayCellRects.Add(cellRect);
        x += cellWidth;
    }
    x = startX;
    y += cellHeight;
}

// HitTestHelper automatically handles via RegisterDayCells()

// HitHandler - HitTest()
for (int row = 0; row < 6; row++)
{
    for (int col = 0; col < 7; col++)
    {
        var cellRect = layout.DayCellRects[row * 7 + col];
        if (cellRect.Contains(location))
        {
            DateTime date = GetDateForCell(displayMonth, row, col, properties);
            result.HitArea = $"day_{date:yyyy_MM_dd}";
            result.Date = date;
            result.HitBounds = cellRect;
            return result;
        }
    }
}
```

### Pattern 2: Dual-Month Calendar (Range Pickers)
```csharp
// Painter - CalculateLayout()
layout.MonthGrids = new List<DateTimePickerMonthGrid>();

for (int monthIndex = 0; monthIndex < 2; monthIndex++)
{
    var grid = new DateTimePickerMonthGrid();
    grid.DayCellRects = new List<Rectangle>();
    
    // Calculate cells for this month
    for (int i = 0; i < 42; i++)
    {
        var cellRect = new Rectangle(...);
        grid.DayCellRects.Add(cellRect);
    }
    
    grid.PreviousButtonRect = ...;  // Only for first grid
    grid.NextButtonRect = ...;      // Only for last grid
    grid.TitleRect = ...;
    
    layout.MonthGrids.Add(grid);
}

// HitTestHelper automatically handles via RegisterMultipleCalendarGrids()

// HitHandler - HitTest()
for (int gridIndex = 0; gridIndex < layout.MonthGrids.Count; gridIndex++)
{
    var grid = layout.MonthGrids[gridIndex];
    DateTime gridMonth = displayMonth.AddMonths(gridIndex);
    
    for (int i = 0; i < grid.DayCellRects.Count; i++)
    {
        if (grid.DayCellRects[i].Contains(location))
        {
            DateTime date = CalculateDateForGrid(gridMonth, i, properties);
            result.HitArea = $"day_grid{gridIndex}_{date:yyyy_MM_dd}";
            result.Date = date;
            result.HitBounds = grid.DayCellRects[i];
            return result;
        }
    }
}
```

### Pattern 3: Time Slots
```csharp
// Painter - CalculateLayout()
layout.TimeSlotRects = new List<Rectangle>();
TimeSpan currentTime = TimeSpan.Zero;
TimeSpan interval = properties.TimeInterval;

while (currentTime < TimeSpan.FromHours(24))
{
    var slotRect = new Rectangle(x, y, width, slotHeight);
    layout.TimeSlotRects.Add(slotRect);
    currentTime += interval;
    y += slotHeight;
}

// HitTestHelper automatically handles via RegisterTimeSlots()

// HitHandler - HitTest()
if (layout.TimeSlotRects != null)
{
    TimeSpan currentTime = TimeSpan.Zero;
    for (int i = 0; i < layout.TimeSlotRects.Count; i++)
    {
        if (layout.TimeSlotRects[i].Contains(location))
        {
            result.HitArea = $"time_{currentTime:hhmm}";
            result.Time = currentTime;
            result.HitBounds = layout.TimeSlotRects[i];
            return result;
        }
        currentTime += properties.TimeInterval;
    }
}
```

### Pattern 4: Quick Buttons
```csharp
// Painter - CalculateLayout()
layout.QuickButtonRects = new Dictionary<string, Rectangle>();

if (properties.ShowTodayButton)
{
    layout.QuickButtonRects["today"] = new Rectangle(...);
}
if (properties.ShowTomorrowButton)
{
    layout.QuickButtonRects["tomorrow"] = new Rectangle(...);
}
// ... etc.

// HitTestHelper automatically handles via RegisterQuickButtons()

// HitHandler - HitTest()
if (layout.QuickButtonRects != null)
{
    foreach (var kvp in layout.QuickButtonRects)
    {
        if (kvp.Value.Contains(location))
        {
            result.HitArea = $"quick_{kvp.Key}";
            result.HitBounds = kvp.Value;
            return result;
        }
    }
}

// HitHandler - HandleClick()
if (hitResult.HitArea.StartsWith("quick_"))
{
    string buttonName = hitResult.HitArea.Substring(6);
    switch (buttonName)
    {
        case "today":
            owner.SelectDate(DateTime.Today);
            break;
        case "tomorrow":
            owner.SelectDate(DateTime.Today.AddDays(1));
            break;
        // ... etc.
    }
    return true;  // Close after quick selection
}
```

---

## Hit Area Naming Convention

### Format: `"{type}_{identifier}"`

**Navigation:**
- `"nav_previous"` → PreviousButton
- `"nav_next"` → NextButton
- `"nav_previous_grid0"` → Previous button for grid 0
- `"nav_next_grid1"` → Next button for grid 1

**Header:**
- `"header_title"` → Main header/title
- `"header_title_grid0"` → Header for grid 0

**Day Cells:**
- `"day_2025_10_15"` → Single-month day cell
- `"day_grid0_2025_10_15"` → Day cell in grid 0
- `"day_grid1_2025_11_15"` → Day cell in grid 1

**Time:**
- `"time_1430"` → 14:30 time slot
- `"time_spinner_start_hour"` → Start hour spinner
- `"time_spinner_end_minute"` → End minute spinner

**Buttons:**
- `"quick_today"` → Today quick button
- `"quick_tomorrow"` → Tomorrow quick button
- `"button_apply"` → Apply button
- `"button_cancel"` → Cancel button
- `"button_clear"` → Clear button
- `"button_create"` → Create event button

**Special:**
- `"week_0"` → Week number 0
- `"week_grid0_3"` → Week number in grid 0, row 3
- `"month_0"` → January button (MonthView)
- `"year_2025"` → Year button (YearView)
- `"quarter_Q1"` → Q1 button (Quarterly)
- `"handle_start"` → Timeline start handle
- `"handle_end"` → Timeline end handle
- `"filter_preset1"` → Filter preset button

---

## DateTimePickerHitArea Enum Mapping

When updating hover state, map hit area names to enum:

```csharp
if (hitArea == "nav_previous" || hitArea.StartsWith("nav_previous_grid"))
    → DateTimePickerHitArea.PreviousButton

if (hitArea == "nav_next" || hitArea.StartsWith("nav_next_grid"))
    → DateTimePickerHitArea.NextButton

if (hitArea.StartsWith("day_"))
    → DateTimePickerHitArea.DayCell

if (hitArea.StartsWith("time_") && !hitArea.Contains("spinner"))
    → DateTimePickerHitArea.TimeSlot

if (hitArea.Contains("spinner"))
    → DateTimePickerHitArea.TimeSpinner

if (hitArea.StartsWith("quick_"))
    → DateTimePickerHitArea.QuickButton

if (hitArea == "button_apply")
    → DateTimePickerHitArea.ApplyButton

if (hitArea == "button_cancel")
    → DateTimePickerHitArea.CancelButton

if (hitArea == "button_clear")
    → DateTimePickerHitArea.ClearButton

if (hitArea.StartsWith("week_"))
    → DateTimePickerHitArea.WeekNumber

if (hitArea.StartsWith("month_"))
    → DateTimePickerHitArea.MonthButton

if (hitArea.StartsWith("year_"))
    → DateTimePickerHitArea.YearButton

if (hitArea.StartsWith("quarter_"))
    → DateTimePickerHitArea.QuarterButton

if (hitArea.StartsWith("handle_"))
    → DateTimePickerHitArea.Handle

if (hitArea == "timeline_track")
    → DateTimePickerHitArea.TimelineTrack

if (hitArea.StartsWith("filter_"))
    → DateTimePickerHitArea.FilterButton

if (hitArea == "button_create")
    → DateTimePickerHitArea.CreateButton

if (hitArea.Contains("header") || hitArea.Contains("title"))
    → DateTimePickerHitArea.Header
```

---

## Date Calculation Helper

Calculate date from cell index in 7×6 grid:

```csharp
private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties props)
{
    // First day of the month
    var firstDay = new DateTime(displayMonth.Year, displayMonth.Month, 1);
    
    // Calculate offset based on FirstDayOfWeek setting
    int firstDayOfWeek = (int)props.FirstDayOfWeek;
    int dayOfWeek = (int)firstDay.DayOfWeek;
    int offset = (dayOfWeek - firstDayOfWeek + 7) % 7;
    
    // Cell index
    int cellIndex = row * 7 + col;
    
    // Day number (1-based, can be negative for previous month or > days for next month)
    int dayNumber = cellIndex - offset + 1;
    
    // Calculate the actual date
    DateTime cellDate = firstDay.AddDays(dayNumber - 1);
    
    return cellDate;
}
```

---

## Validation Checklist

### Painter Validation
```
✅ CalculateLayout() populates ALL interactive rectangles
✅ PaintCalendar() calls CalculateLayout() FIRST
✅ Every painted clickable element has a layout rectangle
✅ Multi-month layouts use MonthGrids collection
✅ XML documentation explains layout structure
```

### HitTestHelper Validation
```
✅ Registration methods exist for all painter's area types
✅ RegisterHitAreas() is called after CalculateLayout()
✅ All rectangles registered with _owner._hitTest.AddHitArea()
✅ Hit area names follow "{type}_{identifier}" convention
✅ Single-month and multi-month layouts handled correctly
```

### HitHandler Validation
```
✅ HitTest() tests ALL registered areas
✅ HitTest() tests areas in correct priority order
✅ HitTest() returns correct DateTimePickerHitTestResult
✅ HandleClick() processes ALL hit area name patterns
✅ HandleClick() updates owner state correctly
✅ HandleClick() returns correct close/stay-open behavior
✅ UpdateHoverState() maps to DateTimePickerHitArea enum
✅ Date calculation logic uses FirstDayOfWeek correctly
```

---

## Testing Commands

### Manual Test Script
```
1. Click navigation buttons → Month should change
2. Click day cells → Date should select
3. Click time slots (if visible) → Time should select
4. Click quick buttons → Should execute preset
5. Click apply/cancel → Should confirm/cancel
6. Click clear → Should clear selection
7. Hover elements → Should highlight correctly
8. Test edge cases: MinDate/MaxDate boundaries
9. Test with different FirstDayOfWeek settings
10. Test with different themes
```

### Visual Verification
```
1. Hover over element → Hit area should align perfectly
2. Click on edge of element → Should still register
3. Click between elements → Should not register
4. Multi-month: Click both grids → Both should work
5. Scroll time slots → Hit areas should update
```

---

## Common Issues & Quick Fixes

| Issue | Cause | Fix |
|-------|-------|-----|
| Click does nothing | Rectangle is Empty | Set rectangle in CalculateLayout() |
| Wrong date selected | Date calculation error | Check FirstDayOfWeek logic |
| Hover doesn't work | UpdateHoverState() not mapping | Add enum mapping |
| Multi-month broken | Using DayCellRects | Use MonthGrids collection |
| Hit area misaligned | Painting doesn't match layout | Sync paint with layout rects |
| Time slots don't work | TimeSlotRects not set | Populate in CalculateLayout() |
| Apply button missing | Rectangle not calculated | Add to CalculateLayout() |
| Navigation not registered | HitTestHelper missing call | Add RegisterNavigationButtons() |

---

**Quick Reference Version:** 1.0
**Last Updated:** October 17, 2025
