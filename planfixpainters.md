# DateTimePicker Painter & HitHandler Revision Plan

## 📋 Executive Summary

This plan ensures that **all 18 DateTimePicker painters and their corresponding hit handlers** properly integrate with the hit testing system through `BeepDateTimePickerHitTestHelper` and use the `DateTimePickerHitArea` enum for standardized area identification.

### Problem Statement
- Painters must call `CalculateLayout()` to populate all interactive rectangles BEFORE painting
- `BeepDateTimePickerHitTestHelper` must register all areas to BaseControl's hit test system
- Hit handlers must process all registered areas using `DateTimePickerHitArea` enum
- Date/time calculations must be accurate and respect constraints

### Solution Approach
Systematic review of all 18 painter/hit handler pairs in 4 priority tiers, ensuring proper hit area registration, correct event handling, and visual-to-interactive alignment.

---

## 🏗️ Architecture Overview

### Component Flow
```
BeepDateTimePicker.DrawContent()
    ↓
1. Painter.CalculateLayout(bounds, properties)
   └─> Returns DateTimePickerLayout with ALL interactive Rectangles
    ↓
2. _hitHelper.RegisterHitAreas(layout, props, displayMonth)
   └─> Registers all areas to BaseControl._hitTest
   └─> Creates hit area map with naming convention
    ↓
3. Painter.PaintCalendar(g, bounds, properties, displayMonth, hoverState)
   └─> Uses layout rectangles to paint visual elements
    ↓
4. User Interaction (click/hover)
   └─> BaseControl hit test system detects location
    ↓
5. HitHandler.HitTest(location, layout, displayMonth, properties)
   └─> Maps location to DateTimePickerHitTestResult
   └─> Returns hit area name, date/time, bounds
    ↓
6. HitHandler.HandleClick(hitResult, owner)
   └─> Executes appropriate action
   └─> Updates owner state
   └─> Returns true to close dropdown or false to stay open
```

### Key Components

#### DateTimePickerLayout
Stores Rectangle bounds for all interactive elements:
- Single-month: `DayCellRects`, `PreviousButtonRect`, `NextButtonRect`, etc.
- Multi-month: `MonthGrids` collection with per-grid rectangles
- Time areas: `TimeSlotRects`, `StartTimeHourUpRect`, etc.
- Action areas: `ApplyButtonRect`, `CancelButtonRect`, `ClearButtonRect`

#### BeepDateTimePickerHitTestHelper
Bridges painter layouts to BaseControl's hit test system:
- `RegisterHitAreas()` - Main registration method
- `RegisterNavigationButtons()` - Navigation button registration
- `RegisterDayCells()` - Single-month day cells
- `RegisterMultipleCalendarGrids()` - Multi-month day cells
- `RegisterTimeSlots()` - Time picker slots
- `RegisterQuickButtons()` - Quick action buttons
- `RegisterRangeTimeSpinners()` - Hour/minute spinners
- `RegisterClearButton()` - Clear button
- `RegisterWeekNumbers()` - Week number column
- `RegisterActionButtons()` - Apply/Cancel buttons ✅ **NEW**
- `RegisterQuarterButtons()` - Quarter buttons (Q1-Q4) ✅ **NEW**
- `RegisterMonthButtons()` - Month selector buttons ✅ **NEW**
- `RegisterYearButtons()` - Year selector buttons ✅ **NEW**
- `RegisterFlexibleRangeButtons()` - Flexible range presets ✅ **NEW**
- `RegisterFilterButtons()` - Filter preset buttons ✅ **NEW**
- `RegisterTabButtons()` - Tab selector buttons ✅ **NEW**
- `RegisterTodayButton()` - Today button ✅ **NEW**

#### DateTimePickerHitArea Enum (25 Types)
```csharp
None, Header, PreviousButton, NextButton, DayCell, 
TimeSlot, QuickButton, TimeButton, TimeSpinner, 
ApplyButton, CancelButton, WeekNumber, DropdownButton, 
ClearButton, ActionButton, Handle, TimelineTrack, 
FilterButton, CreateButton, MonthButton, YearButton, 
QuarterButton, WeekRow, GridButton, FlexibleRangeButton, 
TodayButton
```

#### IDateTimePickerPainter Interface
```csharp
DatePickerMode Mode { get; }
void PaintCalendar(Graphics g, Rectangle bounds, ...);
DateTimePickerLayout CalculateLayout(Rectangle bounds, ...);
void PaintDayCell(Graphics g, Rectangle cellBounds, ...);
void PaintHeader(Graphics g, Rectangle headerBounds, ...);
void PaintNavigationButton(Graphics g, Rectangle buttonBounds, ...);
// ... other painting methods
```

#### IDateTimePickerHitHandler Interface
```csharp
DatePickerMode Mode { get; }
DateTimePickerHitTestResult HitTest(Point location, ...);
bool HandleClick(DateTimePickerHitTestResult hitResult, ...);
void UpdateHoverState(DateTimePickerHitTestResult hitResult, ...);
```

---

## 📊 18 Painter/HitHandler Pairs Matrix

| # | Painter | HitHandler | Mode | Priority | Hit Areas |
|---|---------|-----------|------|----------|-----------|
| 1 | SingleDateTimePickerPainter | SingleDateTimePickerHitHandler | Single | HIGH | Header, PreviousButton, NextButton, DayCell, TodayButton |
| 2 | CompactDateTimePickerPainter | CompactDateTimePickerHitHandler | Compact | HIGH | Header, PreviousButton, NextButton, DayCell, TodayButton |
| 3 | SingleWithTimeDateTimePickerPainter | SingleWithTimeDateTimePickerHitHandler | SingleWithTime | HIGH | Header, PreviousButton, NextButton, DayCell, TimeSlot, TimeButton |
| 4 | RangeDateTimePickerPainter | RangeDateTimePickerHitHandler | Range | HIGH | Header, PreviousButton, NextButton, DayCell, ApplyButton, CancelButton |
| 5 | RangeWithTimeDateTimePickerPainter | RangeWithTimeDateTimePickerHitHandler | RangeWithTime | MED | Header, PreviousButton, NextButton, DayCell, TimeSpinner, ApplyButton, CancelButton |
| 6 | DualCalendarDateTimePickerPainter | DualCalendarDateTimePickerHitHandler | DualCalendar | MED | Header (2), PreviousButton, NextButton, DayCell (2 grids) |
| 7 | ModernCardDateTimePickerPainter | ModernCardDateTimePickerHitHandler | ModernCard | MED | Header, PreviousButton, NextButton, DayCell, QuickButton |
| 8 | AppointmentDateTimePickerPainter | AppointmentDateTimePickerHitHandler | Appointment | MED | Header, PreviousButton, NextButton, DayCell, TimeSlot |
| 9 | MultipleDateTimePickerPainter | MultipleDateTimePickerHitHandler | Multiple | MED | Header, PreviousButton, NextButton, DayCell (checkable), ApplyButton, CancelButton |
| 10 | WeekViewDateTimePickerPainter | WeekViewDateTimePickerHitHandler | WeekView | MED | Header, PreviousButton, NextButton, WeekRow, WeekNumber |
| 11 | MonthViewDateTimePickerPainter | MonthViewDateTimePickerHitHandler | MonthView | MED | Header, PreviousButton, NextButton, MonthButton (12) |
| 12 | YearViewDateTimePickerPainter | YearViewDateTimePickerHitHandler | YearView | MED | Header, PreviousButton, NextButton, YearButton |
| 13 | TimelineDateTimePickerPainter | TimelineDateTimePickerHitHandler | Timeline | LOW | Handle (start/end), TimelineTrack, DayCell (mini calendar) |
| 14 | QuarterlyDateTimePickerPainter | QuarterlyDateTimePickerHitHandler | Quarterly | LOW | Header, PreviousButton, NextButton, QuarterButton, YearButton |
| 15 | FlexibleRangeDateTimePickerPainter | FlexibleRangeDateTimePickerHitHandler | FlexibleRange | LOW | Header, PreviousButton, NextButton, DayCell, FlexibleRangeButton, ApplyButton |
| 16 | FilteredRangeDateTimePickerPainter | FilteredRangeDateTimePickerHitHandler | FilteredRange | LOW | Header, PreviousButton, NextButton, DayCell, FilterButton, TimeSlot, ApplyButton |
| 17 | SidebarEventDateTimePickerPainter | SidebarEventDateTimePickerHitHandler | SidebarEvent | LOW | Header, PreviousButton, NextButton, DayCell, CreateButton, ActionButton |
| 18 | HeaderDateTimePickerPainter | HeaderDateTimePickerHitHandler | Header | LOW | Header (large), DayCell |

---

## 🎯 Critical Rules

### Rule 1: Painters MUST Call CalculateLayout() First
```csharp
public void PaintCalendar(Graphics g, Rectangle bounds, ...)
{
    // ✅ CORRECT: Call CalculateLayout FIRST
    var layout = CalculateLayout(bounds, properties);
    
    // Then use layout rectangles for painting
    PaintHeader(g, layout.HeaderRect, ...);
    PaintNavigationButton(g, layout.PreviousButtonRect, ...);
    PaintCalendarGrid(g, layout, ...);
}
```

### Rule 2: CalculateLayout() MUST Populate ALL Interactive Rectangles
```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, ...)
{
    var layout = new DateTimePickerLayout();
    
    // ✅ MUST set ALL interactive rectangles
    layout.HeaderRect = new Rectangle(...);
    layout.PreviousButtonRect = new Rectangle(...);
    layout.NextButtonRect = new Rectangle(...);
    layout.TitleRect = new Rectangle(...);
    layout.DayCellRects = new List<Rectangle>();
    // ... populate all cells
    
    // ❌ WRONG: Leaving rectangles as Empty
    // layout.ApplyButtonRect = Rectangle.Empty; // Will not be clickable!
    
    return layout;
}
```

### Rule 3: Hit Area Names MUST Follow Convention
```csharp
// Format: "{type}_{identifier}"

// ✅ CORRECT:
"nav_previous"
"nav_next"
"day_2025_10_15"
"day_grid0_2025_10_15"
"time_14_30"
"quick_today"
"button_apply"

// ❌ WRONG:
"previous"
"2025-10-15"
"todayButton"
"apply_button"
```

### Rule 4: Multi-Month Layouts MUST Use MonthGrids
```csharp
// ✅ CORRECT: For dual-month or range layouts
layout.MonthGrids = new List<DateTimePickerMonthGrid>();
for (int i = 0; i < 2; i++)
{
    var grid = new DateTimePickerMonthGrid();
    grid.DayCellRects = new List<Rectangle>();
    // ... populate grid
    layout.MonthGrids.Add(grid);
}

// ❌ WRONG: Don't use flat DayCellRects for multi-month
// layout.DayCellRects = new List<Rectangle>(); // Only for single-month!
```

### Rule 5: HitHandlers MUST Process ALL Registered Areas
```csharp
public DateTimePickerHitTestResult HitTest(Point location, ...)
{
    var result = new DateTimePickerHitTestResult();
    
    // ✅ Test in priority order
    
    // 1. Navigation (highest priority)
    if (layout.PreviousButtonRect.Contains(location)) { ... }
    
    // 2. Day cells
    if (layout.CalendarGridRect.Contains(location)) { ... }
    
    // 3. Time slots
    if (layout.TimeSlotRects != null) { ... }
    
    // 4. Action buttons (lowest priority)
    if (layout.ApplyButtonRect.Contains(location)) { ... }
    
    return result;
}
```

### Rule 6: UpdateHoverState() MUST Map to Enum
```csharp
public void UpdateHoverState(DateTimePickerHitTestResult hitResult, ...)
{
    if (!hitResult.IsHit)
    {
        hoverState.Clear();
        return;
    }
    
    // ✅ CORRECT: Map hit area name to enum
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

## 🔧 Revision Process (Per Painter/HitHandler Pair)

### Step 1: Analyze Painter's CalculateLayout()
**Checklist:**
- [ ] Returns complete `DateTimePickerLayout` with ALL interactive rectangles
- [ ] Handles single-month vs. multi-month layouts correctly
- [ ] Calculates bounds for:
  - Navigation buttons (PreviousButtonRect, NextButtonRect)
  - Header/title (TitleRect, HeaderRect)
  - Day cells (DayCellRects or MonthGrids[].DayCellRects)
  - Week numbers (WeekNumberRects) if applicable
  - Time slots (TimeSlotRects) if applicable
  - Quick buttons (QuickButtonRects) if applicable
  - Action buttons (ApplyButtonRect, CancelButtonRect, ClearButtonRect)
  - Mode-specific elements
- [ ] XML documentation explains layout structure

### Step 2: Verify Painter's PaintCalendar()
**Checklist:**
- [ ] Calls `CalculateLayout()` FIRST
- [ ] Uses layout rectangles for painting
- [ ] Every painted interactive element has a layout rectangle
- [ ] No painted elements without corresponding rectangles

### Step 3: Verify BeepDateTimePickerHitTestHelper
**Current Registration Methods:**
- `RegisterNavigationButtons()` - Navigation and header
- `RegisterDayCells()` - Single-month day cells
- `RegisterMultipleCalendarGrids()` - Multi-month day cells
- `RegisterTimeSlots()` - Time picker slots
- `RegisterQuickButtons()` - Quick action buttons
- `RegisterRangeTimeSpinners()` - Hour/minute spinners
- `RegisterClearButton()` - Clear button
- `RegisterWeekNumbers()` - Week number column

**Action Items:**
- [ ] Add missing registration methods for new area types (if needed)
- [ ] Verify naming convention: `"{type}_{identifier}"`
- [ ] Ensure all painter's rectangles are registered

### Step 4: Analyze HitHandler's HitTest()
**Checklist:**
- [ ] Tests layout rectangles in correct priority order
- [ ] Returns `DateTimePickerHitTestResult` with:
  - `IsHit = true` when hit
  - `HitArea` name matching registered name
  - `HitBounds` with the rectangle
  - `Date`, `Time`, or other context as applicable
- [ ] Handles both single-month and multi-month layouts
- [ ] Correctly calculates dates from cell indices using `FirstDayOfWeek`
- [ ] XML documentation explains hit test logic

### Step 5: Analyze HitHandler's HandleClick()
**Checklist:**
- [ ] Processes all hit areas returned by HitTest()
- [ ] Updates owner control state correctly:
  - Navigation: `owner.NavigateToPreviousMonth()` / `owner.NavigateToNextMonth()`
  - Day cells: `owner.SelectDate(date)` or range selection logic
  - Time slots: `owner.SelectTime(time)`
  - Quick buttons: Execute preset logic
  - Action buttons: Confirm/cancel/clear actions
- [ ] Returns `true` to close dropdown when appropriate
- [ ] Returns `false` for navigation/preview actions
- [ ] Fires appropriate events: `DateSelected`, `RangeSelected`, etc.
- [ ] XML documentation explains click handling logic

### Step 6: Analyze HitHandler's UpdateHoverState()
**Checklist:**
- [ ] Updates `DateTimePickerHoverState` with hovered area type
- [ ] Stores hovered date/time/bounds for painter to highlight
- [ ] Maps hit area names to `DateTimePickerHitArea` enum correctly

### Step 7: Test the Pair
**Functional Tests:**
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

**Visual Tests:**
- [ ] Hover states display correctly
- [ ] Pressed states display correctly
- [ ] Selected dates highlight properly
- [ ] Hit areas align with visual elements
- [ ] No gaps or overlaps in hit detection

**Integration Tests:**
- [ ] Works with all BeepTheme styles
- [ ] Respects MinDate/MaxDate constraints
- [ ] FirstDayOfWeek setting works
- [ ] Custom formats display correctly
- [ ] Events fire with correct data

### Step 8: Update Documentation
- [ ] Painter XML docs explain layout structure
- [ ] Painter XML docs list supported hit areas
- [ ] HitHandler XML docs explain hit test logic
- [ ] HitHandler XML docs list handled click actions
- [ ] Code comments explain complex calculations

### Step 9: Mark Complete in Checklist
- [ ] Update task checklist with ✅
- [ ] Note any issues or special considerations
- [ ] Update progress metrics

### Step 10: Move to Next Pair
- [ ] Review next pair in priority order
- [ ] Repeat steps 1-9

---

## 📝 Hit Area Naming Convention Reference

| Area Type | Name Pattern | Example | Enum |
|-----------|--------------|---------|------|
| Navigation | `nav_{direction}` | `nav_previous` | PreviousButton |
| Navigation (grid) | `nav_{direction}_grid{n}` | `nav_next_grid1` | NextButton |
| Header | `header_title` | `header_title` | Header |
| Header (grid) | `header_title_grid{n}` | `header_title_grid0` | Header |
| Day cell | `day_{yyyy_MM_dd}` | `day_2025_10_15` | DayCell |
| Day cell (grid) | `day_grid{n}_{yyyy_MM_dd}` | `day_grid0_2025_10_15` | DayCell |
| Time slot | `time_{HH}_{mm}` | `time_14_30` | TimeSlot |
| Time spinner | `time_{start/end}_{hour/minute}_{up/down}` | `time_start_hour_up` | TimeSpinner |
| Quick button | `quick_{name}` | `quick_today` | QuickButton |
| Apply button | `button_apply` | `button_apply` | ApplyButton |
| Cancel button | `button_cancel` | `button_cancel` | CancelButton |
| Clear button | `clear_button` | `clear_button` | ClearButton |
| Week number | `week_{n}` | `week_3` | WeekNumber |
| Week (grid) | `week_grid{n}_{row}` | `week_grid0_3` | WeekNumber |
| Month button | `month_{n}` | `month_0` (Jan) | MonthButton |
| Year button | `year_{yyyy}` | `year_2025` | YearButton |
| Quarter button | `quarter_{Q}` | `quarter_Q1` | QuarterButton |
| Handle | `handle_{start/end}` | `handle_start` | Handle |
| Timeline track | `timeline_track` | `timeline_track` | TimelineTrack |
| Filter button | `filter_{name}` | `filter_preset1` | FilterButton |
| Create button | `button_create` | `button_create` | CreateButton |
| Action button | `action_{name}` | `action_edit` | ActionButton |

---

## 💻 Code Patterns

### Pattern 1: Single-Month Calendar Layout
```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    int padding = 10;
    
    // Header with navigation
    layout.HeaderRect = new Rectangle(bounds.X + padding, bounds.Y + padding, bounds.Width - padding * 2, 40);
    layout.PreviousButtonRect = new Rectangle(layout.HeaderRect.X, layout.HeaderRect.Y, 30, 30);
    layout.NextButtonRect = new Rectangle(layout.HeaderRect.Right - 30, layout.HeaderRect.Y, 30, 30);
    layout.TitleRect = new Rectangle(layout.PreviousButtonRect.Right + 5, layout.HeaderRect.Y, 
        layout.NextButtonRect.X - layout.PreviousButtonRect.Right - 10, 30);
    
    // Day names header
    int dayNamesY = layout.HeaderRect.Bottom + 10;
    layout.DayNamesRect = new Rectangle(bounds.X + padding, dayNamesY, bounds.Width - padding * 2, 25);
    
    // Calendar grid (7 columns × 6 rows)
    int gridY = layout.DayNamesRect.Bottom + 5;
    int cellWidth = (bounds.Width - padding * 2) / 7;
    int cellHeight = 35;
    
    layout.CalendarGridRect = new Rectangle(bounds.X + padding, gridY, cellWidth * 7, cellHeight * 6);
    layout.DayCellRects = new List<Rectangle>();
    
    int x = layout.CalendarGridRect.X;
    int y = layout.CalendarGridRect.Y;
    
    for (int row = 0; row < 6; row++)
    {
        for (int col = 0; col < 7; col++)
        {
            var cellRect = new Rectangle(x, y, cellWidth, cellHeight);
            layout.DayCellRects.Add(cellRect);
            x += cellWidth;
        }
        x = layout.CalendarGridRect.X;
        y += cellHeight;
    }
    
    // Today button (optional)
    if (properties.ShowTodayButton)
    {
        layout.TodayButtonRect = new Rectangle(bounds.X + padding, y + 10, 100, 30);
    }
    
    return layout;
}
```

### Pattern 2: Dual-Month Calendar Layout (Range Pickers)
```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    int padding = 10;
    int gap = 20; // Gap between two months
    
    layout.MonthGrids = new List<DateTimePickerMonthGrid>();
    
    int monthWidth = (bounds.Width - padding * 2 - gap) / 2;
    
    for (int monthIndex = 0; monthIndex < 2; monthIndex++)
    {
        var grid = new DateTimePickerMonthGrid();
        int startX = bounds.X + padding + (monthWidth + gap) * monthIndex;
        int startY = bounds.Y + padding;
        
        // Header for this month
        grid.TitleRect = new Rectangle(startX, startY, monthWidth, 30);
        
        // Navigation buttons (only on edges)
        if (monthIndex == 0) // First month gets previous button
        {
            grid.PreviousButtonRect = new Rectangle(startX, startY, 30, 30);
        }
        if (monthIndex == 1) // Last month gets next button
        {
            grid.NextButtonRect = new Rectangle(startX + monthWidth - 30, startY, 30, 30);
        }
        
        // Day cells for this month
        int gridY = startY + 40;
        int cellWidth = monthWidth / 7;
        int cellHeight = 35;
        
        grid.DayCellRects = new List<Rectangle>();
        
        int x = startX;
        int y = gridY;
        
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                var cellRect = new Rectangle(x, y, cellWidth, cellHeight);
                grid.DayCellRects.Add(cellRect);
                x += cellWidth;
            }
            x = startX;
            y += cellHeight;
        }
        
        layout.MonthGrids.Add(grid);
    }
    
    // Apply/Cancel buttons below both months
    int buttonsY = bounds.Bottom - 50;
    layout.ApplyButtonRect = new Rectangle(bounds.Right - 180, buttonsY, 80, 35);
    layout.CancelButtonRect = new Rectangle(bounds.Right - 90, buttonsY, 80, 35);
    
    return layout;
}
```

### Pattern 3: HitTest with Date Calculation
```csharp
public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, 
    DateTime displayMonth, DateTimePickerProperties properties)
{
    var result = new DateTimePickerHitTestResult();
    
    // Test navigation buttons first (highest priority)
    if (layout.PreviousButtonRect.Contains(location))
    {
        result.IsHit = true;
        result.HitArea = "nav_previous";
        result.HitBounds = layout.PreviousButtonRect;
        return result;
    }
    
    if (layout.NextButtonRect.Contains(location))
    {
        result.IsHit = true;
        result.HitArea = "nav_next";
        result.HitBounds = layout.NextButtonRect;
        return result;
    }
    
    // Test day cells
    if (layout.DayCellRects != null && layout.CalendarGridRect.Contains(location))
    {
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                int cellIndex = row * 7 + col;
                var cellRect = layout.DayCellRects[cellIndex];
                
                if (cellRect.Contains(location))
                {
                    // Calculate the date for this cell
                    DateTime date = GetDateForCell(displayMonth, row, col, properties);
                    
                    result.IsHit = true;
                    result.HitArea = $"day_{date:yyyy_MM_dd}";
                    result.Date = date;
                    result.CellIndex = cellIndex;
                    result.HitBounds = cellRect;
                    return result;
                }
            }
        }
    }
    
    return result;
}

private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties props)
{
    var firstDay = new DateTime(displayMonth.Year, displayMonth.Month, 1);
    int firstDayOfWeek = (int)props.FirstDayOfWeek;
    int dayOfWeek = (int)firstDay.DayOfWeek;
    int offset = (dayOfWeek - firstDayOfWeek + 7) % 7;
    
    int cellIndex = row * 7 + col;
    int dayNumber = cellIndex - offset + 1;
    
    return firstDay.AddDays(dayNumber - 1);
}
```

### Pattern 4: HandleClick with State Updates
```csharp
public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
{
    if (!hitResult.IsHit)
        return false;
    
    // Handle navigation
    if (hitResult.HitArea == "nav_previous")
    {
        owner.NavigateToPreviousMonth();
        return false; // Don't close on navigation
    }
    
    if (hitResult.HitArea == "nav_next")
    {
        owner.NavigateToNextMonth();
        return false;
    }
    
    // Handle day cell selection
    if (hitResult.HitArea.StartsWith("day_") && hitResult.Date.HasValue)
    {
        DateTime selectedDate = hitResult.Date.Value;
        
        // Check if date is in range
        if (!owner.IsDateInRange(selectedDate))
            return false;
        
        // Update selection
        owner.SelectDate(selectedDate);
        
        // Fire event
        owner.OnDateSelected(selectedDate);
        
        // Close dropdown if configured
        return owner.GetCurrentProperties().CloseOnSelection;
    }
    
    // Handle quick buttons
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
            case "yesterday":
                owner.SelectDate(DateTime.Today.AddDays(-1));
                break;
        }
        
        return true; // Close after quick selection
    }
    
    // Handle action buttons
    if (hitResult.HitArea == "button_apply")
    {
        owner.ApplySelection();
        return true; // Close
    }
    
    if (hitResult.HitArea == "button_cancel")
    {
        owner.CancelSelection();
        return true; // Close
    }
    
    return false;
}
```

---

## ✅ Validation Criteria

### Painter Validation
1. ✅ `CalculateLayout()` returns layout with all necessary rectangles
2. ✅ `PaintCalendar()` calls `CalculateLayout()` first
3. ✅ All painted interactive elements have corresponding layout rectangles
4. ✅ Multi-month layouts use `MonthGrids` collection correctly
5. ✅ XML documentation explains layout structure and supported hit areas

### HitTestHelper Validation
1. ✅ All layout rectangles are registered via `_owner._hitTest.AddHitArea()`
2. ✅ Hit area names follow convention: `"{type}_{identifier}"`
3. ✅ Registration methods handle single-month and multi-month layouts
4. ✅ All painter-specific area types have registration methods

### HitHandler Validation
1. ✅ `HitTest()` covers all registered hit areas
2. ✅ `HitTest()` returns correct `DateTimePickerHitTestResult` with context
3. ✅ `HandleClick()` processes all hit area types
4. ✅ `HandleClick()` updates owner state and fires events correctly
5. ✅ `UpdateHoverState()` uses `DateTimePickerHitArea` enum
6. ✅ Returns correct close/stay-open behavior
7. ✅ Date/time calculations are accurate and respect FirstDayOfWeek

---

## 🚦 Implementation Order & Status

### Tier 1 - Core Modes (Priority: HIGH) - 0/4 Complete
1. ⬜ **Single** - Standard calendar (foundation for others)
2. ⬜ **Compact** - Minimal calendar for dropdowns
3. ⬜ **SingleWithTime** - Calendar + time picker
4. ⬜ **Range** - Date range selection

### Tier 2 - Advanced Modes (Priority: MEDIUM) - 0/4 Complete
5. ⬜ **RangeWithTime** - Range + time spinners
6. ⬜ **DualCalendar** - Side-by-side months
7. ⬜ **ModernCard** - Card with quick buttons
8. ⬜ **Appointment** - Calendar + hourly slots

### Tier 3 - Specialized Modes (Priority: MEDIUM) - 0/4 Complete
9. ⬜ **Multiple** - Multi-selection with checkboxes
10. ⬜ **WeekView** - Week-based selection
11. ⬜ **MonthView** - Month grid (3×4)
12. ⬜ **YearView** - Year grid

### Tier 4 - Complex Modes (Priority: LOW) - 0/6 Complete
13. ⬜ **Timeline** - Draggable timeline bar
14. ⬜ **Quarterly** - Quarter selectors (Q1-Q4)
15. ⬜ **FlexibleRange** - Range presets + calendar
16. ⬜ **FilteredRange** - Filters + dual calendar + time
17. ⬜ **SidebarEvent** - Sidebar + calendar + events
18. ⬜ **Header** - Large header + compact calendar

**Overall Progress:** 0/18 complete (0%)

---

## 🔍 Common Issues & Quick Fixes

| Symptom | Likely Cause | Check | Fix |
|---------|-------------|-------|-----|
| Click does nothing | Rectangle is Empty | CalculateLayout() | Populate rectangle with bounds |
| Wrong date selected | Date calc error | GetDateForCell() | Fix FirstDayOfWeek logic |
| Hover doesn't work | No enum mapping | UpdateHoverState() | Add enum mapping |
| Multi-month broken | Not using MonthGrids | Layout structure | Use MonthGrids collection |
| Misaligned hits | Paint != layout | Rectangle sync | Sync paint with layout rects |
| No time slots | Rects not populated | TimeSlotRects | Populate in CalculateLayout() |
| Missing button | Not calculated | CalculateLayout() | Add button rectangle |
| No navigation | Not registered | HitTestHelper | Verify RegisterNavigationButtons() |
| Date off by one | Offset calculation | Cell to date math | Check startDayOffset formula |
| Dropdown won't close | Always returns false | HandleClick() | Return true when appropriate |

---

## 🧪 Testing Strategy

### Per Mode Testing
```
1. Click all navigation buttons → Month should change
2. Click all day cells → Date should select
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

### Integration Tests
```
1. Test with all BeepTheme styles
2. Test MinDate/MaxDate constraints
3. Test FirstDayOfWeek settings (Sunday through Saturday)
4. Test custom formats
5. Test ReturnDateTimeType behaviors
6. Test event firing (DateSelected, RangeSelected, etc.)
```

---

## 📦 File Locations

### Painters
```
C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\
└── TheTechIdea.Beep.Winform.Controls\
    └── Dates\
        └── Painters\
            ├── SingleDateTimePickerPainter.cs
            ├── CompactDateTimePickerPainter.cs
            ├── SingleWithTimeDateTimePickerPainter.cs
            ├── RangeDateTimePickerPainter.cs
            ├── RangeWithTimeDateTimePickerPainter.cs
            ├── DualCalendarDateTimePickerPainter.cs
            ├── ModernCardDateTimePickerPainter.cs
            ├── AppointmentDateTimePickerPainter.cs
            ├── MultipleDateTimePickerPainter.cs
            ├── WeekViewDateTimePickerPainter.cs
            ├── MonthViewDateTimePickerPainter.cs
            ├── YearViewDateTimePickerPainter.cs
            ├── TimelineDateTimePickerPainter.cs
            ├── QuarterlyDateTimePickerPainter.cs
            ├── FlexibleRangeDateTimePickerPainter.cs
            ├── FilteredRangeDateTimePickerPainter.cs
            ├── SidebarEventDateTimePickerPainter.cs
            ├── HeaderDateTimePickerPainter.cs
            ├── IDateTimePickerPainter.cs
            └── DateTimePickerPainterFactory.cs
```

### Hit Handlers
```
C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\
└── TheTechIdea.Beep.Winform.Controls\
    └── Dates\
        └── HitHandlers\
            ├── SingleDateTimePickerHitHandler.cs
            ├── CompactDateTimePickerHitHandler.cs
            ├── SingleWithTimeDateTimePickerHitHandler.cs
            ├── RangeDateTimePickerHitHandler.cs
            ├── RangeWithTimeDateTimePickerHitHandler.cs
            ├── DualCalendarDateTimePickerHitHandler.cs
            ├── ModernCardDateTimePickerHitHandler.cs
            ├── AppointmentDateTimePickerHitHandler.cs
            ├── MultipleDateTimePickerHitHandler.cs
            ├── WeekViewDateTimePickerHitHandler.cs
            ├── MonthViewDateTimePickerHitHandler.cs
            ├── YearViewDateTimePickerHitHandler.cs
            ├── TimelineDateTimePickerHitHandler.cs
            ├── QuarterlyDateTimePickerHitHandler.cs
            ├── FlexibleRangeDateTimePickerHitHandler.cs
            ├── FilteredRangeDateTimePickerHitHandler.cs
            ├── SidebarEventDateTimePickerHitHandler.cs
            ├── HeaderDateTimePickerHitHandler.cs
            └── IDateTimePickerHitHandler.cs
```

### Helper
```
C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\
└── TheTechIdea.Beep.Winform.Controls\
    └── Dates\
        └── Helpers\
            └── BeepDateTimePickerHitTestHelper.cs
```

### Models
```
C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\
└── TheTechIdea.Beep.Winform.Controls\
    └── Dates\
        └── Models\
            ├── enums.cs (DateTimePickerHitArea, DatePickerMode, etc.)
            ├── DateTimePickerProperties.cs
            └── DateTimePickerModels.cs
```

---

## 🎯 Success Criteria

✅ All 18 painter/hit handler pairs pass validation  
✅ All hit areas properly registered using `BeepDateTimePickerHitTestHelper`  
✅ All hit handlers use `DateTimePickerHitArea` enum correctly  
✅ Zero click detection failures  
✅ 100% visual-to-interactive alignment  
✅ All modes work correctly with BaseControl's hit test system  
✅ All date/time calculations respect FirstDayOfWeek and constraints  
✅ All close/stay-open behaviors work as expected  
✅ All events fire with correct data  
✅ All themes render correctly  

---

## 💡 Pro Tips

1. **Work in order** - Complete Tier 1 first, then Tier 2, etc.
2. **Test frequently** - Test each mode immediately after fixing
3. **Reuse patterns** - Single mode is the template for most others
4. **Check alignment** - Hover should match painted areas exactly
5. **Ask for help** - Complex modes (Timeline, FilteredRange) may need discussion
6. **Document as you go** - Update progress immediately after completing tasks
7. **Use naming conventions religiously** - Consistency is critical for hit testing
8. **Validate before moving on** - Don't batch validation, do it per mode

---

## 📅 Next Steps

1. ✅ **Plan Created** - This document
2. ⬜ **Begin Tier 1** - Start with Single mode
3. ⬜ **Complete Single** - First complete implementation
4. ⬜ **Complete Compact** - Second mode
5. ⬜ **Complete SingleWithTime** - Third mode
6. ⬜ **Complete Range** - Fourth mode (completes Tier 1)
7. ⬜ **Begin Tier 2** - Move to advanced modes
8. ⬜ **Continue through all tiers** - Systematic completion
9. ⬜ **Final Testing** - Comprehensive integration tests
10. ⬜ **Documentation** - Final documentation updates

---

**Document Version:** 1.0  
**Created:** October 17, 2025  
**Status:** Ready for Implementation  
**Start with:** Tier 1 → Single Mode  

🚀 **Ready to begin!**
