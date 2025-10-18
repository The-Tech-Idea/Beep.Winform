# DateTimePicker Painters - Priority 1 (CRITICAL) Implementation Plans

## Overview
Detailed implementation plans for the 3 CRITICAL painters that have stub or mostly empty `CalculateLayout()` methods.

---

# PAINTER 1: MonthViewDateTimePickerPainter

## Current Status
**Completeness:** 0% (Empty stub returning empty layout)

## Paint Analysis

### What PaintCalendar() does:
```csharp
public void PaintCalendar(Graphics g, Rectangle bounds, ...)
{
    int padding = 20;
    int currentY = bounds.Y + padding;
    
    // 1. Year header with navigation (50px height)
    var headerRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 50);
    PaintYearHeader(g, headerRect, displayMonth.Year, hoverState);
    currentY += 70;
    
    // 2. Month grid (3x4 = 12 months)
    var monthGridRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, bounds.Height - currentY - padding);
    PaintMonthGrid(g, monthGridRect, displayMonth.Year, hoverState);
}
```

### What PaintYearHeader() paints:
```csharp
- Year text centered (e.g., "2024")
- Previous year button (left, 36x36px)
- Next year button (right, 36x36px)
```

### What PaintMonthGrid() paints:
```csharp
- 12 month cells in 4 rows x 3 columns
- Gap of 12px between cells
- Each cell is clickable
```

## Required Layout Properties

### Check DateTimePickerLayout class for:
```csharp
public class DateTimePickerLayout
{
    // Need to add these if missing:
    public Rectangle PreviousYearButtonRect { get; set; }
    public Rectangle NextYearButtonRect { get; set; }
    public List<Rectangle> MonthCellRects { get; set; }
    
    // Already exists:
    public Rectangle HeaderRect { get; set; }
}
```

## Required Enum Values

### Check DateTimePickerHitArea enum for:
```csharp
public enum DateTimePickerHitArea
{
    PreviousYearButton,  // ✅ Already added
    NextYearButton,      // ✅ Already added
    MonthButton,         // ❓ Need to verify/add
}
```

## Required Registration Methods

### Check BeepDateTimePickerHitTestHelper for:
```csharp
public void RegisterPreviousYearButton(Rectangle bounds)  // ✅ Should exist
public void RegisterNextYearButton(Rectangle bounds)      // ✅ Should exist
public void RegisterMonthButton(Rectangle bounds, int month)  // ❓ Need to verify/add
```

## Complete CalculateLayout Implementation

```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    
    int padding = 20;
    int currentY = bounds.Y + padding;
    
    // ========================================
    // 1. YEAR HEADER SECTION (70px total)
    // ========================================
    
    // Year display area (text is centered here)
    layout.HeaderRect = new Rectangle(
        bounds.X + padding, 
        currentY, 
        bounds.Width - padding * 2, 
        50
    );
    
    // Previous year button (36x36, left aligned)
    int buttonSize = 36;
    layout.PreviousYearButtonRect = new Rectangle(
        bounds.X + padding, 
        currentY + 2, 
        buttonSize, 
        buttonSize
    );
    
    // Next year button (36x36, right aligned)
    layout.NextYearButtonRect = new Rectangle(
        bounds.Right - padding - buttonSize, 
        currentY + 2, 
        buttonSize, 
        buttonSize
    );
    
    currentY += 70; // Move past header (50 + 20 spacing)
    
    // ========================================
    // 2. MONTH GRID SECTION (3 cols x 4 rows)
    // ========================================
    
    var monthGridRect = new Rectangle(
        bounds.X + padding, 
        currentY, 
        bounds.Width - padding * 2, 
        bounds.Height - currentY - padding
    );
    
    int rows = 4;
    int cols = 3;
    int gap = 12;
    
    // Calculate individual cell size
    int cellWidth = (monthGridRect.Width - gap * (cols - 1)) / cols;
    int cellHeight = (monthGridRect.Height - gap * (rows - 1)) / rows;
    
    // Create 12 month cell rectangles
    layout.MonthCellRects = new List<Rectangle>();
    
    for (int month = 1; month <= 12; month++)
    {
        int row = (month - 1) / cols;  // 0-3
        int col = (month - 1) % cols;  // 0-2
        
        var cellRect = new Rectangle(
            monthGridRect.X + col * (cellWidth + gap),
            monthGridRect.Y + row * (cellHeight + gap),
            cellWidth,
            cellHeight
        );
        
        layout.MonthCellRects.Add(cellRect);
    }
    
    // ========================================
    // 3. REGISTER ALL HIT AREAS
    // ========================================
    _owner.HitTestHelper?.RegisterHitAreas(layout, properties);
    
    return layout;
}
```

## Hit Handler Implementation

Update `MonthViewDateTimePickerHitHandler.cs`:

```csharp
public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth)
{
    var result = new DateTimePickerHitTestResult();
    
    // Check previous year button
    if (layout.PreviousYearButtonRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.PreviousYearButton;
        result.HitBounds = layout.PreviousYearButtonRect;
        return result;
    }
    
    // Check next year button
    if (layout.NextYearButtonRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.NextYearButton;
        result.HitBounds = layout.NextYearButtonRect;
        return result;
    }
    
    // Check month cells
    if (layout.MonthCellRects != null)
    {
        for (int i = 0; i < layout.MonthCellRects.Count; i++)
        {
            if (layout.MonthCellRects[i].Contains(location))
            {
                result.HitArea = DateTimePickerHitArea.MonthButton;
                result.HitBounds = layout.MonthCellRects[i];
                result.Month = i + 1; // 1-12
                return result;
            }
        }
    }
    
    return result;
}
```

## Testing Checklist
- [ ] Layout calculates 3 rectangles (Header, PrevYear, NextYear)
- [ ] Layout calculates 12 month cell rectangles
- [ ] Clicking previous year button navigates to previous year
- [ ] Clicking next year button navigates to next year
- [ ] Clicking any month cell selects that month
- [ ] Visual rendering matches calculated layout
- [ ] No compilation errors

---

# PAINTER 2: YearViewDateTimePickerPainter

## Current Status
**Completeness:** 0% (Empty stub returning empty layout)

## Paint Analysis

### What PaintCalendar() does:
```csharp
public void PaintCalendar(Graphics g, Rectangle bounds, ...)
{
    int padding = 20;
    int currentY = bounds.Y + padding;
    
    // Calculate decade range
    int startYear = (displayMonth.Year / 10) * 10;
    int endYear = startYear + 9;
    
    // 1. Decade header (e.g., "2020 — 2029")
    var headerRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 50);
    PaintDecadeHeader(g, headerRect, startYear, endYear, hoverState);
    currentY += 70;
    
    // 2. Year grid (4x3 = 12 years, showing decade-1 to decade+10)
    var yearGridRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, bounds.Height - currentY - padding);
    PaintYearGrid(g, yearGridRect, startYear, hoverState);
}
```

### What PaintDecadeHeader() paints:
```csharp
- Decade range text centered (e.g., "2020 — 2029")
- Previous decade button (left, 36x36px)
- Next decade button (right, 36x36px)
```

### What PaintYearGrid() paints:
```csharp
- 12 year cells in 4 rows x 3 columns
- Shows years from (decade-1) to (decade+10)
- Gap of 12px between cells
- Each cell is clickable
```

## Required Layout Properties

### Check DateTimePickerLayout class for:
```csharp
public class DateTimePickerLayout
{
    // Need to add these if missing:
    public Rectangle PreviousDecadeButtonRect { get; set; }
    public Rectangle NextDecadeButtonRect { get; set; }
    public List<Rectangle> YearCellRects { get; set; }
    
    // Already exists:
    public Rectangle HeaderRect { get; set; }
}
```

## Required Enum Values

### Check DateTimePickerHitArea enum for:
```csharp
public enum DateTimePickerHitArea
{
    PreviousDecadeButton,  // ✅ Already added
    NextDecadeButton,      // ✅ Already added
    YearButton,            // ❓ Need to verify/add
}
```

## Required Registration Methods

### Check BeepDateTimePickerHitTestHelper for:
```csharp
public void RegisterPreviousDecadeButton(Rectangle bounds)  // ✅ Should exist
public void RegisterNextDecadeButton(Rectangle bounds)      // ✅ Should exist
public void RegisterYearButton(Rectangle bounds, int year)  // ❓ Need to verify/add
```

## Complete CalculateLayout Implementation

```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    
    int padding = 20;
    int currentY = bounds.Y + padding;
    
    // ========================================
    // 1. DECADE HEADER SECTION (70px total)
    // ========================================
    
    // Decade range display area (text is centered here)
    layout.HeaderRect = new Rectangle(
        bounds.X + padding, 
        currentY, 
        bounds.Width - padding * 2, 
        50
    );
    
    // Previous decade button (36x36, left aligned)
    int buttonSize = 36;
    layout.PreviousDecadeButtonRect = new Rectangle(
        bounds.X + padding, 
        currentY + 2, 
        buttonSize, 
        buttonSize
    );
    
    // Next decade button (36x36, right aligned)
    layout.NextDecadeButtonRect = new Rectangle(
        bounds.Right - padding - buttonSize, 
        currentY + 2, 
        buttonSize, 
        buttonSize
    );
    
    currentY += 70; // Move past header (50 + 20 spacing)
    
    // ========================================
    // 2. YEAR GRID SECTION (3 cols x 4 rows = 12 years)
    // ========================================
    
    var yearGridRect = new Rectangle(
        bounds.X + padding, 
        currentY, 
        bounds.Width - padding * 2, 
        bounds.Height - currentY - padding
    );
    
    int rows = 4;
    int cols = 3;
    int gap = 12;
    
    // Calculate individual cell size
    int cellWidth = (yearGridRect.Width - gap * (cols - 1)) / cols;
    int cellHeight = (yearGridRect.Height - gap * (rows - 1)) / rows;
    
    // Create 12 year cell rectangles
    // Note: Years displayed are decade-1 to decade+10
    // But we just create rectangles, actual year values handled in hit handler
    layout.YearCellRects = new List<Rectangle>();
    
    for (int i = 0; i < 12; i++)
    {
        int row = i / cols;  // 0-3
        int col = i % cols;  // 0-2
        
        var cellRect = new Rectangle(
            yearGridRect.X + col * (cellWidth + gap),
            yearGridRect.Y + row * (cellHeight + gap),
            cellWidth,
            cellHeight
        );
        
        layout.YearCellRects.Add(cellRect);
    }
    
    // ========================================
    // 3. REGISTER ALL HIT AREAS
    // ========================================
    _owner.HitTestHelper?.RegisterHitAreas(layout, properties);
    
    return layout;
}
```

## Hit Handler Implementation

Update `YearViewDateTimePickerHitHandler.cs`:

```csharp
public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth)
{
    var result = new DateTimePickerHitTestResult();
    
    // Check previous decade button
    if (layout.PreviousDecadeButtonRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.PreviousDecadeButton;
        result.HitBounds = layout.PreviousDecadeButtonRect;
        return result;
    }
    
    // Check next decade button
    if (layout.NextDecadeButtonRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.NextDecadeButton;
        result.HitBounds = layout.NextDecadeButtonRect;
        return result;
    }
    
    // Check year cells
    if (layout.YearCellRects != null)
    {
        // Calculate decade start
        int startYear = (displayMonth.Year / 10) * 10;
        
        for (int i = 0; i < layout.YearCellRects.Count; i++)
        {
            if (layout.YearCellRects[i].Contains(location))
            {
                // Years displayed: decade-1 to decade+10 (12 total)
                int year = startYear - 1 + i;
                
                result.HitArea = DateTimePickerHitArea.YearButton;
                result.HitBounds = layout.YearCellRects[i];
                result.Year = year;
                return result;
            }
        }
    }
    
    return result;
}
```

## Testing Checklist
- [ ] Layout calculates 3 rectangles (Header, PrevDecade, NextDecade)
- [ ] Layout calculates 12 year cell rectangles
- [ ] Clicking previous decade button navigates 10 years back
- [ ] Clicking next decade button navigates 10 years forward
- [ ] Clicking any year cell selects that year
- [ ] Year calculation correct (decade-1 to decade+10)
- [ ] Visual rendering matches calculated layout
- [ ] No compilation errors

---

# PAINTER 3: FilteredRangeDateTimePickerPainter

## Current Status
**Completeness:** ~5% (Only left calendar grid calculated, missing ~30 rectangles)

## Paint Analysis - Complete Breakdown

### What PaintCalendar() does:
```csharp
public void PaintCalendar(Graphics g, Rectangle bounds, ...)
{
    // Split into sidebar (25%) and main content (75%)
    int sidebarWidth = (int)(bounds.Width * 0.25f);
    var sidebarBounds = new Rectangle(bounds.X, bounds.Y, sidebarWidth, bounds.Height);
    var mainBounds = new Rectangle(bounds.X + sidebarWidth, bounds.Y, bounds.Width - sidebarWidth, bounds.Height);
    
    PaintFilterSidebar(g, sidebarBounds, hoverState);
    PaintMainContent(g, mainBounds, properties, displayMonth, hoverState);
}
```

### Sidebar Layout (25% width):
```csharp
PaintFilterSidebar():
- Title "Quick Filters" (24px height)
- 6 filter buttons (32px each + 8px spacing):
  1. "Past Week"
  2. "Past Month"
  3. "Past 3 Months"
  4. "Past 6 Months"
  5. "Past Year"
  6. "Past Century"
```

### Main Content Layout (75% width):
```csharp
PaintMainContent():
- Dual calendar section (65% of height):
  - Left calendar: Year dropdown + Month/Year header + Day names + Grid
  - Right calendar: Year dropdown + Month/Year header + Day names + Grid
- Time picker row (50px):
  - "From:" label + time input
  - "To:" label + time input
- Action buttons row (40px):
  - "Reset Date" button
  - "Show Results" button
```

### Dual Calendar Details:
```csharp
PaintDualCalendarWithYearSelector():
- Two calendars side by side (50% each with 16px gap)
- Each calendar has:
  - Year dropdown (28px height)
  - Month/Year header (24px height)
  - Day names row (20px height)
  - Calendar grid (remaining height)
```

## Required Layout Properties

### Need to add to DateTimePickerLayout class:
```csharp
public class DateTimePickerLayout
{
    // ===== SIDEBAR =====
    public Rectangle SidebarRect { get; set; }
    public Rectangle FilterTitleRect { get; set; }
    public List<Rectangle> FilterButtonRects { get; set; }
    
    // ===== MAIN CONTENT CONTAINERS =====
    public Rectangle MainContentRect { get; set; }
    public Rectangle DualCalendarContainerRect { get; set; }
    
    // ===== LEFT CALENDAR =====
    public Rectangle LeftYearDropdownRect { get; set; }
    public Rectangle LeftHeaderRect { get; set; }
    public Rectangle LeftDayNamesRect { get; set; }
    public Rectangle LeftCalendarGridRect { get; set; }
    public List<Rectangle> LeftDayCellRects { get; set; }
    
    // ===== RIGHT CALENDAR =====
    public Rectangle RightYearDropdownRect { get; set; }
    public Rectangle RightHeaderRect { get; set; }
    public Rectangle RightDayNamesRect { get; set; }
    public Rectangle RightCalendarGridRect { get; set; }
    public List<Rectangle> RightDayCellRects { get; set; }
    
    // ===== TIME PICKER ROW =====
    public Rectangle TimePickerRowRect { get; set; }
    public Rectangle FromLabelRect { get; set; }
    public Rectangle FromTimeInputRect { get; set; }
    public Rectangle ToLabelRect { get; set; }
    public Rectangle ToTimeInputRect { get; set; }
    
    // ===== ACTION BUTTONS =====
    public Rectangle ActionButtonRowRect { get; set; }
    public Rectangle ResetButtonRect { get; set; }
    public Rectangle ShowResultsButtonRect { get; set; }
}
```

## Required Enum Values

### Check DateTimePickerHitArea enum for:
```csharp
public enum DateTimePickerHitArea
{
    // Filter buttons
    FilterButton,  // ❓ Need to add
    
    // Year dropdowns
    YearDropdown,  // ❓ Need to add
    
    // Day cells (already exists)
    DayCell,  // ✅ Exists
    
    // Time inputs
    TimeInput,  // ❓ Need to add
    
    // Action buttons
    ResetButton,       // ❓ Need to add
    ShowResultsButton, // ❓ Need to add
}
```

## Complete CalculateLayout Implementation

```csharp
public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
{
    var layout = new DateTimePickerLayout();
    
    // ========================================
    // 1. SIDEBAR SECTION (25% width)
    // ========================================
    
    int sidebarWidth = (int)(bounds.Width * 0.25f);
    layout.SidebarRect = new Rectangle(bounds.X, bounds.Y, sidebarWidth, bounds.Height);
    
    int sidebarPadding = 12;
    int currentY = bounds.Y + sidebarPadding;
    
    // Filter title
    layout.FilterTitleRect = new Rectangle(
        layout.SidebarRect.X + sidebarPadding,
        currentY,
        layout.SidebarRect.Width - sidebarPadding * 2,
        24
    );
    currentY += 36;
    
    // Filter buttons (6 buttons)
    string[] filters = { "Past Week", "Past Month", "Past 3 Months", "Past 6 Months", "Past Year", "Past Century" };
    int buttonHeight = 32;
    int buttonSpacing = 8;
    
    layout.FilterButtonRects = new List<Rectangle>();
    for (int i = 0; i < filters.Length; i++)
    {
        var buttonBounds = new Rectangle(
            layout.SidebarRect.X + sidebarPadding,
            currentY,
            layout.SidebarRect.Width - sidebarPadding * 2,
            buttonHeight
        );
        layout.FilterButtonRects.Add(buttonBounds);
        currentY += buttonHeight + buttonSpacing;
    }
    
    // ========================================
    // 2. MAIN CONTENT SECTION (75% width)
    // ========================================
    
    int mainX = bounds.X + sidebarWidth;
    int mainWidth = bounds.Width - sidebarWidth;
    layout.MainContentRect = new Rectangle(mainX, bounds.Y, mainWidth, bounds.Height);
    
    int mainPadding = 12;
    currentY = bounds.Y + mainPadding;
    
    // ========================================
    // 3. DUAL CALENDAR SECTION (65% of main height)
    // ========================================
    
    int calendarHeight = (int)(layout.MainContentRect.Height * 0.65f);
    layout.DualCalendarContainerRect = new Rectangle(
        mainX + mainPadding,
        currentY,
        mainWidth - mainPadding * 2,
        calendarHeight
    );
    
    // Split into two calendars with 16px gap
    int calendarWidth = (layout.DualCalendarContainerRect.Width - 16) / 2;
    
    // LEFT CALENDAR
    var leftCalendarBounds = new Rectangle(
        layout.DualCalendarContainerRect.X,
        layout.DualCalendarContainerRect.Y,
        calendarWidth,
        calendarHeight
    );
    
    int calPadding = 6;
    int calY = leftCalendarBounds.Y;
    
    // Left year dropdown
    layout.LeftYearDropdownRect = new Rectangle(
        leftCalendarBounds.X + calPadding,
        calY,
        leftCalendarBounds.Width - calPadding * 2,
        28
    );
    calY += 34;
    
    // Left header (month/year text)
    layout.LeftHeaderRect = new Rectangle(
        leftCalendarBounds.X + calPadding,
        calY,
        leftCalendarBounds.Width - calPadding * 2,
        24
    );
    calY += 28;
    
    // Left day names
    layout.LeftDayNamesRect = new Rectangle(
        leftCalendarBounds.X + calPadding,
        calY,
        leftCalendarBounds.Width - calPadding * 2,
        20
    );
    calY += 24;
    
    // Left calendar grid
    int gridWidth = leftCalendarBounds.Width - calPadding * 2;
    int availableHeight = leftCalendarBounds.Bottom - calY - calPadding;
    layout.LeftCalendarGridRect = new Rectangle(
        leftCalendarBounds.X + calPadding,
        calY,
        gridWidth,
        availableHeight
    );
    
    // Left day cells (6x7 grid)
    layout.LeftDayCellRects = new List<Rectangle>();
    int cellWidth = gridWidth / 7;
    int cellHeight = availableHeight / 6;
    
    for (int row = 0; row < 6; row++)
    {
        for (int col = 0; col < 7; col++)
        {
            layout.LeftDayCellRects.Add(new Rectangle(
                layout.LeftCalendarGridRect.X + col * cellWidth,
                layout.LeftCalendarGridRect.Y + row * cellHeight,
                cellWidth,
                cellHeight
            ));
        }
    }
    
    // RIGHT CALENDAR (same structure as left)
    var rightCalendarBounds = new Rectangle(
        layout.DualCalendarContainerRect.X + calendarWidth + 16,
        layout.DualCalendarContainerRect.Y,
        calendarWidth,
        calendarHeight
    );
    
    calY = rightCalendarBounds.Y;
    
    // Right year dropdown
    layout.RightYearDropdownRect = new Rectangle(
        rightCalendarBounds.X + calPadding,
        calY,
        rightCalendarBounds.Width - calPadding * 2,
        28
    );
    calY += 34;
    
    // Right header
    layout.RightHeaderRect = new Rectangle(
        rightCalendarBounds.X + calPadding,
        calY,
        rightCalendarBounds.Width - calPadding * 2,
        24
    );
    calY += 28;
    
    // Right day names
    layout.RightDayNamesRect = new Rectangle(
        rightCalendarBounds.X + calPadding,
        calY,
        rightCalendarBounds.Width - calPadding * 2,
        20
    );
    calY += 24;
    
    // Right calendar grid
    layout.RightCalendarGridRect = new Rectangle(
        rightCalendarBounds.X + calPadding,
        calY,
        gridWidth,
        availableHeight
    );
    
    // Right day cells (6x7 grid)
    layout.RightDayCellRects = new List<Rectangle>();
    
    for (int row = 0; row < 6; row++)
    {
        for (int col = 0; col < 7; col++)
        {
            layout.RightDayCellRects.Add(new Rectangle(
                layout.RightCalendarGridRect.X + col * cellWidth,
                layout.RightCalendarGridRect.Y + row * cellHeight,
                cellWidth,
                cellHeight
            ));
        }
    }
    
    currentY = layout.DualCalendarContainerRect.Bottom + 10;
    
    // ========================================
    // 4. TIME PICKER ROW (50px height)
    // ========================================
    
    int timePickerHeight = 50;
    layout.TimePickerRowRect = new Rectangle(
        mainX + mainPadding,
        currentY,
        mainWidth - mainPadding * 2,
        timePickerHeight
    );
    
    int labelWidth = 50;
    int timePickerWidth = (layout.TimePickerRowRect.Width - labelWidth * 2 - 30) / 2;
    
    // From label
    layout.FromLabelRect = new Rectangle(
        layout.TimePickerRowRect.X,
        layout.TimePickerRowRect.Y,
        labelWidth,
        timePickerHeight
    );
    
    // From time input
    layout.FromTimeInputRect = new Rectangle(
        layout.FromLabelRect.Right,
        layout.TimePickerRowRect.Y + 5,
        timePickerWidth,
        timePickerHeight - 10
    );
    
    // To label
    int toX = layout.FromTimeInputRect.Right + 15;
    layout.ToLabelRect = new Rectangle(
        toX,
        layout.TimePickerRowRect.Y,
        labelWidth,
        timePickerHeight
    );
    
    // To time input
    layout.ToTimeInputRect = new Rectangle(
        layout.ToLabelRect.Right,
        layout.TimePickerRowRect.Y + 5,
        timePickerWidth,
        timePickerHeight - 10
    );
    
    currentY += timePickerHeight + 10;
    
    // ========================================
    // 5. ACTION BUTTONS ROW (40px height)
    // ========================================
    
    int actionHeight = 40;
    layout.ActionButtonRowRect = new Rectangle(
        mainX + mainPadding,
        currentY,
        mainWidth - mainPadding * 2,
        actionHeight
    );
    
    int buttonWidth = (layout.ActionButtonRowRect.Width - 12) / 2;
    
    // Reset Date button
    layout.ResetButtonRect = new Rectangle(
        layout.ActionButtonRowRect.X,
        layout.ActionButtonRowRect.Y,
        buttonWidth,
        actionHeight
    );
    
    // Show Results button
    layout.ShowResultsButtonRect = new Rectangle(
        layout.ActionButtonRowRect.X + buttonWidth + 12,
        layout.ActionButtonRowRect.Y,
        buttonWidth,
        actionHeight
    );
    
    // ========================================
    // 6. REGISTER ALL HIT AREAS
    // ========================================
    _owner.HitTestHelper?.RegisterHitAreas(layout, properties);
    
    return layout;
}
```

## Hit Handler Implementation

Update `FilteredRangeDateTimePickerHitHandler.cs` to check ALL rectangles:

```csharp
public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth)
{
    var result = new DateTimePickerHitTestResult();
    
    // Check filter buttons
    if (layout.FilterButtonRects != null)
    {
        for (int i = 0; i < layout.FilterButtonRects.Count; i++)
        {
            if (layout.FilterButtonRects[i].Contains(location))
            {
                result.HitArea = DateTimePickerHitArea.FilterButton;
                result.HitBounds = layout.FilterButtonRects[i];
                result.Data = i; // Filter index
                return result;
            }
        }
    }
    
    // Check left year dropdown
    if (layout.LeftYearDropdownRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.YearDropdown;
        result.HitBounds = layout.LeftYearDropdownRect;
        result.Data = 0; // Left calendar
        return result;
    }
    
    // Check right year dropdown
    if (layout.RightYearDropdownRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.YearDropdown;
        result.HitBounds = layout.RightYearDropdownRect;
        result.Data = 1; // Right calendar
        return result;
    }
    
    // Check left calendar day cells
    if (layout.LeftDayCellRects != null)
    {
        for (int i = 0; i < layout.LeftDayCellRects.Count; i++)
        {
            if (layout.LeftDayCellRects[i].Contains(location))
            {
                int row = i / 7;
                int col = i % 7;
                var date = GetDateFromCell(row, col, displayMonth, _owner.FirstDayOfWeek);
                result.HitArea = DateTimePickerHitArea.DayCell;
                result.Date = date;
                result.HitBounds = layout.LeftDayCellRects[i];
                result.Data = 0; // Left calendar
                return result;
            }
        }
    }
    
    // Check right calendar day cells
    if (layout.RightDayCellRects != null)
    {
        var nextMonth = displayMonth.AddMonths(1);
        for (int i = 0; i < layout.RightDayCellRects.Count; i++)
        {
            if (layout.RightDayCellRects[i].Contains(location))
            {
                int row = i / 7;
                int col = i % 7;
                var date = GetDateFromCell(row, col, nextMonth, _owner.FirstDayOfWeek);
                result.HitArea = DateTimePickerHitArea.DayCell;
                result.Date = date;
                result.HitBounds = layout.RightDayCellRects[i];
                result.Data = 1; // Right calendar
                return result;
            }
        }
    }
    
    // Check from time input
    if (layout.FromTimeInputRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.TimeInput;
        result.HitBounds = layout.FromTimeInputRect;
        result.Data = 0; // From time
        return result;
    }
    
    // Check to time input
    if (layout.ToTimeInputRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.TimeInput;
        result.HitBounds = layout.ToTimeInputRect;
        result.Data = 1; // To time
        return result;
    }
    
    // Check reset button
    if (layout.ResetButtonRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.ResetButton;
        result.HitBounds = layout.ResetButtonRect;
        return result;
    }
    
    // Check show results button
    if (layout.ShowResultsButtonRect.Contains(location))
    {
        result.HitArea = DateTimePickerHitArea.ShowResultsButton;
        result.HitBounds = layout.ShowResultsButtonRect;
        return result;
    }
    
    return result;
}
```

## Testing Checklist
- [ ] Layout calculates sidebar rectangle
- [ ] Layout calculates 6 filter button rectangles
- [ ] Layout calculates left calendar: dropdown, header, day names, grid, 42 day cells
- [ ] Layout calculates right calendar: dropdown, header, day names, grid, 42 day cells
- [ ] Layout calculates time picker row: 2 labels + 2 inputs
- [ ] Layout calculates action buttons: Reset + Show Results
- [ ] Total: ~100 rectangles calculated
- [ ] All filter buttons clickable
- [ ] Both year dropdowns clickable
- [ ] All day cells in both calendars clickable
- [ ] Both time inputs clickable
- [ ] Both action buttons clickable
- [ ] Visual rendering matches calculated layout
- [ ] No compilation errors

---

## IMPLEMENTATION ORDER

### Phase 1: Infrastructure (Do First)
1. Add missing properties to `DateTimePickerLayout.cs`
2. Add missing enum values to `DateTimePickerHitArea` enum
3. Add missing registration methods to `BeepDateTimePickerHitTestHelper.cs`

### Phase 2: Implement Painters (In Order)
4. MonthViewDateTimePickerPainter - Simplest (3 main rects + 12 cells)
5. YearViewDateTimePickerPainter - Similar complexity (3 main rects + 12 cells)
6. FilteredRangeDateTimePickerPainter - Most complex (~100 rectangles)

### Phase 3: Update Hit Handlers
7. MonthViewDateTimePickerHitHandler
8. YearViewDateTimePickerHitHandler
9. FilteredRangeDateTimePickerHitHandler

### Phase 4: Testing
10. Test each painter individually
11. Verify all hit areas respond correctly
12. Check visual rendering matches layout

---

**Last Updated:** October 18, 2025
**Priority:** CRITICAL - These 3 painters block full functionality
**Estimated Time:** 4-6 hours for all 3 painters
