# DateTimePicker Painters - Complete Missing Rectangles Audit

## Overview
This document audits ALL 18 DateTimePicker painters to identify missing rectangles in their `CalculateLayout()` methods by comparing what they paint vs what they calculate.

---

## AUDIT METHODOLOGY

For each painter:
1. ✅ Analyze `PaintCalendar()` and all Paint methods
2. ✅ List ALL interactive elements painted (buttons, cells, inputs, dropdowns)
3. ✅ Check what `CalculateLayout()` currently returns
4. ❌ Identify MISSING rectangles that should be in layout
5. 📋 Create implementation plan

---

## 1. SingleDateTimePickerPainter ✅ COMPLETE

**What it paints:**
- Header with month/year
- Previous/Next buttons
- Day names row
- Calendar grid (6x7 cells)
- Today button

**What CalculateLayout returns:**
- ✅ HeaderRect
- ✅ PreviousButtonRect, NextButtonRect
- ✅ DayNamesRect
- ✅ CalendarGridRect, DayCellMatrix[6,7]
- ✅ TodayButtonRect

**Status:** ✅ COMPLETE - All rectangles present

---

## 2. CompactDateTimePickerPainter ✅ COMPLETE

**What it paints:**
- Header with month/year
- Previous/Next buttons
- Day names row
- Compact calendar grid (6x7 cells)

**What CalculateLayout returns:**
- ✅ HeaderRect
- ✅ PreviousButtonRect, NextButtonRect
- ✅ DayNamesRect
- ✅ CalendarGridRect, DayCellMatrix[6,7]

**Status:** ✅ COMPLETE - All rectangles present

---

## 3. RangeDateTimePickerPainter ✅ COMPLETE

**What it paints:**
- Dual calendars side-by-side
- Each calendar: Header, Day names, Grid
- Apply/Cancel buttons

**What CalculateLayout returns:**
- ✅ MonthGrids (2 grids with HeaderRect, DayNamesRect, CalendarGridRect, DayCellRects)
- ✅ ApplyButtonRect, CancelButtonRect

**Status:** ✅ COMPLETE - All rectangles present

---

## 4. MonthViewDateTimePickerPainter ❌ STUB - CRITICAL

**What it paints:**
- Year header with navigation
- Previous/Next YEAR buttons
- Month grid (3x4 = 12 months)
- Each month cell clickable

**What CalculateLayout returns:**
- ❌ EMPTY LAYOUT

**Missing Rectangles:**
- HeaderRect (year display)
- PreviousYearButtonRect
- NextYearButtonRect
- MonthCellRects (List<Rectangle> with 12 cells)

**Status:** ❌ NEEDS FULL IMPLEMENTATION

---

## 5. YearViewDateTimePickerPainter ❌ STUB - CRITICAL

**What it paints:**
- Decade header (e.g., "2020 — 2029")
- Previous/Next DECADE buttons
- Year grid (4x3 = 12 years)
- Each year cell clickable

**What CalculateLayout returns:**
- ❌ EMPTY LAYOUT

**Missing Rectangles:**
- HeaderRect (decade display)
- PreviousDecadeButtonRect
- NextDecadeButtonRect
- YearCellRects (List<Rectangle> with 12 cells)

**Status:** ❌ NEEDS FULL IMPLEMENTATION

---

## 6. SingleWithTimeDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Calendar section (header, day names, grid)
- Time picker section:
  - Hour display with up/down spinners
  - Colon separator
  - Minute display with up/down spinners
  - AM/PM toggle

**What CalculateLayout returns:**
- ✅ HeaderRect, DayNamesRect, CalendarGridRect, DayCellMatrix
- ✅ TimePickerRect
- ⚠️ MISSING time component rectangles

**Missing Rectangles:**
- TimeHourRect
- TimeMinuteRect
- TimeColonRect
- TimeHourUpRect (spinner up button)
- TimeHourDownRect (spinner down button)
- TimeMinuteUpRect
- TimeMinuteDownRect
- TimeAMPMRect (AM/PM toggle button)

**Status:** ⚠️ NEEDS TIME PICKER RECTANGLES

---

## 7. RangeWithTimeDateTimePickerPainter ✅ COMPLETE

**What it paints:**
- Dual calendars
- Start time picker (hour/minute/spinners)
- End time picker (hour/minute/spinners)

**What CalculateLayout returns:**
- ✅ MonthGrids (2 calendars)
- ✅ StartTimeHourRect, StartTimeMinuteRect, StartTimeColonRect
- ✅ StartTimeHourUpRect, StartTimeHourDownRect
- ✅ StartTimeMinuteUpRect, StartTimeMinuteDownRect
- ✅ EndTimeHourRect, EndTimeMinuteRect, EndTimeColonRect
- ✅ EndTimeHourUpRect, EndTimeHourDownRect
- ✅ EndTimeMinuteUpRect, EndTimeMinuteDownRect

**Status:** ✅ COMPLETE - All rectangles present

---

## 8. MultipleDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Header with month/year
- Previous/Next buttons
- Day names row
- Calendar grid (6x7 cells)
- Selected date chips (pills showing selected dates)
- Clear all button

**What CalculateLayout returns:**
- ✅ HeaderRect, DayNamesRect, CalendarGridRect
- ✅ DayCellRects (List)
- ⚠️ SelectedDateChips present but may need update
- ✅ ClearButtonRect

**Missing Rectangles:**
- PreviousButtonRect
- NextButtonRect

**Status:** ⚠️ MISSING NAVIGATION BUTTONS

---

## 9. AppointmentDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Left mini calendar (month view)
- Right time slots panel (hourly slots 8 AM - 6 PM)
- Each time slot is clickable

**What CalculateLayout returns:**
- ✅ HeaderRect, DayNamesRect, CalendarGridRect, DayCellMatrix
- ✅ TimeSlotRects (List)

**Missing Rectangles:**
- PreviousButtonRect (for calendar)
- NextButtonRect (for calendar)
- TimeSlotPanelRect (container for time slots)

**Status:** ⚠️ MISSING NAVIGATION AND CONTAINER RECTS

---

## 10. TimelineDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Top timeline strip (showing weeks/months)
- Header with month/year
- Previous/Next buttons
- Calendar grid
- Event markers on timeline

**What CalculateLayout returns:**
- ✅ HeaderRect, TimelineRect, CalendarGridRect, DayCellMatrix
- ✅ EventMarkerRects (List)

**Missing Rectangles:**
- PreviousButtonRect
- NextButtonRect
- TimelineSegmentRects (clickable timeline segments)

**Status:** ⚠️ MISSING NAVIGATION AND TIMELINE SEGMENTS

---

## 11. QuarterlyDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Year selector dropdown
- Q1, Q2, Q3, Q4 quarter cards (4 large buttons)
- Selected range display

**What CalculateLayout returns:**
- ✅ HeaderRect
- ✅ YearSelectorRect
- ✅ QuickDateButtons (Q1-Q4)

**Missing Rectangles:**
- YearDropdownArrowRect (clickable dropdown arrow)
- SelectedRangeDisplayRect

**Status:** ⚠️ MISSING DROPDOWN ARROW AND DISPLAY AREA

---

## 12. ModernCardDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Large header card with selected date
- Quick date buttons (Today, Tomorrow, Next Week, etc.)
- Calendar grid
- Apply/Cancel buttons

**What CalculateLayout returns:**
- ✅ HeaderRect
- ✅ QuickDateButtons (List)
- ✅ CalendarGridRect, DayCellMatrix
- ✅ ActionButtonRects

**Missing Rectangles:**
- PreviousButtonRect (for calendar nav)
- NextButtonRect (for calendar nav)
- DayNamesRect

**Status:** ⚠️ MISSING NAVIGATION AND DAY NAMES

---

## 13. DualCalendarDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Two calendars side-by-side
- Each calendar has header, day names, grid
- No explicit navigation buttons (uses header clicks?)

**What CalculateLayout returns:**
- ✅ MonthGrids (2 grids)
- ✅ Each grid has: HeaderRect, DayNamesRect, CalendarGridRect, DayCellRects

**Missing Rectangles:**
- PreviousButtonRect (for left calendar?)
- NextButtonRect (for right calendar?)
- OR HeaderTitleRects if headers are clickable

**Status:** ⚠️ POSSIBLY MISSING NAVIGATION MECHANISM

---

## 14. WeekViewDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Week number column (left side)
- Header with month/year
- Previous/Next buttons
- Day names row
- Calendar grid with week rows highlighted

**What CalculateLayout returns:**
- ✅ HeaderRect
- ✅ WeekNumberColumnRect
- ✅ CalendarGridRect
- ✅ DayCellRects[6,7]
- ✅ WeekNumberRects (List)

**Missing Rectangles:**
- PreviousButtonRect
- NextButtonRect
- DayNamesRect
- WeekRowRects (clickable week rows)

**Status:** ⚠️ MISSING NAVIGATION, DAY NAMES, AND WEEK ROWS

---

## 15. SidebarEventDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Left sidebar (40%):
  - Large date display
  - Event list
  - "Create Event" button
- Right calendar (60%):
  - Month selector (12 months in grid)
  - Previous/Next buttons
  - Day names
  - Calendar grid

**What CalculateLayout returns:**
- ✅ Calculates calendar section only
- ✅ HeaderRect (month selector area)
- ✅ PreviousButtonRect, NextButtonRect
- ✅ DayNamesRect
- ✅ CalendarGridRect, DayCellMatrix

**Missing Rectangles:**
- SidebarRect (entire sidebar area)
- LargeDateDisplayRect
- EventListRect
- CreateEventButtonRect
- MonthSelectorRects (12 month buttons in header)

**Status:** ⚠️ MISSING ENTIRE SIDEBAR LAYOUT

---

## 16. FlexibleRangeDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Tab selector at top (Exact dates / ± 1 day / ± 2 days / etc.)
- Calendar area
- Quick date buttons at bottom (tolerance presets)

**What CalculateLayout returns:**
- ✅ CalendarGridRect
- ✅ DayCellRects (List)
- ✅ QuickDateButtons (List - tolerance buttons)

**Missing Rectangles:**
- TabSelectorRect (container)
- TabButtonRects (List - individual tab buttons)
- HeaderRect (calendar header)
- DayNamesRect
- PreviousButtonRect
- NextButtonRect

**Status:** ⚠️ MISSING TABS, NAVIGATION, AND HEADER

---

## 17. FilteredRangeDateTimePickerPainter ❌ CRITICAL - MOSTLY MISSING

**What it paints:**
- **Left Sidebar (25%):**
  - Title "Quick Filters"
  - 6 filter buttons (Past Week, Month, 3M, 6M, Year, Century)
- **Main Content (75%):**
  - Dual calendar section:
    - Left calendar: Year dropdown, Month/Year header, Day names, Grid
    - Right calendar: Year dropdown, Month/Year header, Day names, Grid
  - Time picker row:
    - "From:" label + time input
    - "To:" label + time input
  - Action buttons row:
    - "Reset Date" button
    - "Show Results" button (primary)

**What CalculateLayout returns:**
- ⚠️ Only returns LEFT calendar grid
- ✅ CalendarGridRect
- ✅ DayCellRects (List)

**Missing Rectangles:**
- **Sidebar:**
  - SidebarRect
  - FilterTitleRect
  - FilterButtonRects (List<Rectangle> - 6 buttons)
- **Main Content:**
  - MainContentRect
  - DualCalendarContainerRect
- **Left Calendar:**
  - LeftYearDropdownRect
  - LeftHeaderRect
  - LeftDayNamesRect
  - LeftCalendarGridRect (currently only this is partially calculated)
  - LeftDayCellRects
- **Right Calendar:**
  - RightYearDropdownRect
  - RightHeaderRect
  - RightDayNamesRect
  - RightCalendarGridRect
  - RightDayCellRects
- **Time Picker Row:**
  - TimePickerRowRect (container)
  - FromLabelRect
  - FromTimeInputRect
  - ToLabelRect
  - ToTimeInputRect
- **Action Buttons:**
  - ActionButtonRowRect (container)
  - ResetButtonRect
  - ShowResultsButtonRect

**Status:** ❌ CRITICAL - ONLY ~5% OF RECTANGLES CALCULATED

---

## 18. HeaderDateTimePickerPainter ⚠️ INCOMPLETE

**What it paints:**
- Large custom header area (80px) - user-defined content
- Calendar section below:
  - Month/Year header
  - Day names row
  - Calendar grid

**What CalculateLayout returns:**
- ✅ HeaderRect (calendar header, not the large custom header)
- ✅ DayNamesRect
- ✅ CalendarGridRect, DayCellMatrix

**Missing Rectangles:**
- CustomHeaderRect (the 80px top section)
- PreviousButtonRect
- NextButtonRect

**Status:** ⚠️ MISSING CUSTOM HEADER AND NAVIGATION

---

## SUMMARY BY STATUS

### ✅ COMPLETE (3 painters)
1. SingleDateTimePickerPainter
2. CompactDateTimePickerPainter
3. RangeWithTimeDateTimePickerPainter

### ⚠️ INCOMPLETE - MISSING SOME RECTANGLES (12 painters)
4. SingleWithTimeDateTimePickerPainter - Missing 8 time picker rects
5. MultipleDateTimePickerPainter - Missing 2 navigation buttons
6. AppointmentDateTimePickerPainter - Missing 3 rects
7. TimelineDateTimePickerPainter - Missing 3+ rects
8. QuarterlyDateTimePickerPainter - Missing 2 rects
9. ModernCardDateTimePickerPainter - Missing 3 rects
10. DualCalendarDateTimePickerPainter - Missing 2-4 rects
11. WeekViewDateTimePickerPainter - Missing 4 rects
12. SidebarEventDateTimePickerPainter - Missing 5 sidebar rects
13. FlexibleRangeDateTimePickerPainter - Missing 6+ rects
14. RangeDateTimePickerPainter - Actually complete ✅
15. HeaderDateTimePickerPainter - Missing 3 rects

### ❌ CRITICAL - STUB OR MOSTLY EMPTY (3 painters)
16. MonthViewDateTimePickerPainter - STUB (0% complete)
17. YearViewDateTimePickerPainter - STUB (0% complete)
18. FilteredRangeDateTimePickerPainter - CRITICAL (~5% complete)

---

## IMPLEMENTATION PRIORITY

### Priority 1: CRITICAL STUBS (Must implement full layout)
1. **MonthViewDateTimePickerPainter** - Empty stub
2. **YearViewDateTimePickerPainter** - Empty stub
3. **FilteredRangeDateTimePickerPainter** - Only has 1 calendar out of ~30 rectangles

### Priority 2: HIGH - Missing Core Navigation (Cannot navigate)
4. **MultipleDateTimePickerPainter** - No nav buttons
5. **AppointmentDateTimePickerPainter** - No nav buttons
6. **TimelineDateTimePickerPainter** - No nav buttons
7. **ModernCardDateTimePickerPainter** - No nav buttons
8. **WeekViewDateTimePickerPainter** - No nav buttons
9. **HeaderDateTimePickerPainter** - No nav buttons

### Priority 3: MEDIUM - Missing Interactive Elements
10. **SingleWithTimeDateTimePickerPainter** - Missing time picker hitboxes
11. **SidebarEventDateTimePickerPainter** - Missing sidebar hitboxes
12. **FlexibleRangeDateTimePickerPainter** - Missing tabs and nav
13. **QuarterlyDateTimePickerPainter** - Missing dropdown arrow
14. **DualCalendarDateTimePickerPainter** - Unclear navigation mechanism

---

## NEXT STEPS

1. ✅ Create this audit document
2. ⬜ For each painter, create detailed implementation plan
3. ⬜ Implement Priority 1 (Critical stubs) first
4. ⬜ Implement Priority 2 (Navigation missing)
5. ⬜ Implement Priority 3 (Interactive elements)
6. ⬜ Test each painter after implementation
7. ⬜ Verify hit testing works for all rectangles

---

**Last Updated:** October 18, 2025
**Total Painters Audited:** 18
**Complete:** 3 (17%)
**Incomplete:** 12 (67%)
**Critical:** 3 (17%)
