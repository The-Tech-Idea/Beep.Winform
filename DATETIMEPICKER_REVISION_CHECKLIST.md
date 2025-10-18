# DateTimePicker Painter & HitHandler Revision Checklist

## Overview
Track the revision status of all 18 painter/hit handler pairs to ensure proper hit area registration and event handling.

---

## Legend
- ⬜ Not Started
- 🔄 In Progress
- ✅ Complete
- ❌ Issues Found
- ⚠️ Needs Attention

---

## Tier 1 - Core Modes (Priority: HIGH)

### 1. Single Mode
**Files:**
- `SingleDateTimePickerPainter.cs`
- `SingleDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: All rectangles populated | ⬜ | |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Registration verified | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Click detection | ⬜ | |
| Testing: Visual alignment | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, TodayButton

---

### 2. Compact Mode
**Files:**
- `CompactDateTimePickerPainter.cs`
- `CompactDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: All rectangles populated | ⬜ | |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Registration verified | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Click detection | ⬜ | |
| Testing: Visual alignment | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, TodayButton

---

### 3. SingleWithTime Mode
**Files:**
- `SingleWithTimeDateTimePickerPainter.cs`
- `SingleWithTimeDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: All rectangles populated | ⬜ | Check TimeSlotRects |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Registration verified | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Time slot logic |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Click detection | ⬜ | |
| Testing: Time selection | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, TimeSlot, TimeButton

---

### 4. Range Mode
**Files:**
- `RangeDateTimePickerPainter.cs`
- `RangeDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: MonthGrids support | ⬜ | Dual-month layout |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Multi-grid registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | Multi-month logic |
| HitHandler: HandleClick() review | ⬜ | Range selection logic |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Range selection | ⬜ | |
| Testing: Multi-month nav | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, ApplyButton, CancelButton

---

## Tier 2 - Advanced Modes (Priority: MEDIUM)

### 5. RangeWithTime Mode
**Files:**
- `RangeWithTimeDateTimePickerPainter.cs`
- `RangeWithTimeDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: TimeSpinner rectangles | ⬜ | Start/end time spinners |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Spinner registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Spinner logic |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Time spinner clicks | ⬜ | |
| Testing: Range + time integration | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, TimeSpinner, ApplyButton, CancelButton

---

### 6. DualCalendar Mode
**Files:**
- `DualCalendarDateTimePickerPainter.cs`
- `DualCalendarDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: MonthGrids (2 months) | ⬜ | Side-by-side layout |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Dual-grid registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Both grids clickable | ⬜ | |
| Testing: Visual alignment | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header (2), PreviousButton, NextButton, DayCell (2 grids)

---

### 7. ModernCard Mode
**Files:**
- `ModernCardDateTimePickerPainter.cs`
- `ModernCardDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: QuickButton rectangles | ⬜ | Today/Tomorrow/etc. |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: QuickButton registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Quick button logic |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Quick buttons | ⬜ | |
| Testing: Card layout | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, QuickButton

---

### 8. Appointment Mode
**Files:**
- `AppointmentDateTimePickerPainter.cs`
- `AppointmentDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: TimeSlot list rectangles | ⬜ | Hourly slots |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: TimeSlot registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Time slot selection |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Time slot clicks | ⬜ | |
| Testing: Calendar + time integration | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, TimeSlot

---

## Tier 3 - Specialized Modes (Priority: MEDIUM)

### 9. Multiple Mode
**Files:**
- `MultipleDateTimePickerPainter.cs`
- `MultipleDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: Checkbox areas in cells | ⬜ | |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Registration verified | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Multi-selection toggle |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Multi-selection | ⬜ | |
| Testing: Apply/Cancel buttons | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell (checkable), ApplyButton, CancelButton

---

### 10. WeekView Mode
**Files:**
- `WeekViewDateTimePickerPainter.cs`
- `WeekViewDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: WeekRow rectangles | ⬜ | Full row selection |
| Painter: WeekNumber column | ⬜ | |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: WeekRow registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | Week row detection |
| HitHandler: HandleClick() review | ⬜ | Week selection logic |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Week row clicks | ⬜ | |
| Testing: Week number clicks | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, WeekRow, WeekNumber

---

### 11. MonthView Mode
**Files:**
- `MonthViewDateTimePickerPainter.cs`
- `MonthViewDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: MonthButton grid (3×4) | ⬜ | 12 month buttons |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: MonthButton registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Month selection |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Month button clicks | ⬜ | |
| Testing: Year navigation | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, MonthButton (12)

---

### 12. YearView Mode
**Files:**
- `YearViewDateTimePickerPainter.cs`
- `YearViewDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: YearButton grid | ⬜ | Year range buttons |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: YearButton registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Year selection |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Year button clicks | ⬜ | |
| Testing: Decade navigation | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, YearButton

---

## Tier 4 - Complex Modes (Priority: LOW)

### 13. Timeline Mode
**Files:**
- `TimelineDateTimePickerPainter.cs`
- `TimelineDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: Timeline track rectangle | ⬜ | Draggable bar |
| Painter: Handle rectangles | ⬜ | Start/end handles |
| Painter: Mini calendar | ⬜ | |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Timeline registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | Handle detection |
| HitHandler: HandleClick() review | ⬜ | Drag logic |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Handle dragging | ⬜ | |
| Testing: Track clicks | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Handle (start/end), TimelineTrack, DayCell (mini calendar)

---

### 14. Quarterly Mode
**Files:**
- `QuarterlyDateTimePickerPainter.cs`
- `QuarterlyDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: QuarterButton rectangles | ⬜ | Q1/Q2/Q3/Q4 |
| Painter: YearButton rectangle | ⬜ | |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Quarter registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Quarter selection |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Quarter buttons | ⬜ | |
| Testing: Year navigation | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, QuarterButton, YearButton

---

### 15. FlexibleRange Mode
**Files:**
- `FlexibleRangeDateTimePickerPainter.cs`
- `FlexibleRangeDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: Tab selector rectangles | ⬜ | |
| Painter: FlexibleRangeButton rects | ⬜ | Preset ranges |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Preset registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Preset logic |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Preset buttons | ⬜ | |
| Testing: Tab switching | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, FlexibleRangeButton, ApplyButton

---

### 16. FilteredRange Mode
**Files:**
- `FilteredRangeDateTimePickerPainter.cs`
- `FilteredRangeDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: FilterButton rectangles | ⬜ | Sidebar filters |
| Painter: Dual calendar support | ⬜ | |
| Painter: TimeSlot rectangles | ⬜ | |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Filter registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Filter + range logic |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Filter sidebar | ⬜ | |
| Testing: Calendar + time | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, FilterButton, TimeSlot, ApplyButton

---

### 17. SidebarEvent Mode
**Files:**
- `SidebarEventDateTimePickerPainter.cs`
- `SidebarEventDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: Sidebar layout | ⬜ | Date display + events |
| Painter: CreateButton rectangle | ⬜ | |
| Painter: ActionButton rectangles | ⬜ | Event actions |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Sidebar registration | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | Event actions |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Sidebar buttons | ⬜ | |
| Testing: Event list clicks | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header, PreviousButton, NextButton, DayCell, CreateButton, ActionButton

---

### 18. Header Mode
**Files:**
- `HeaderDateTimePickerPainter.cs`
- `HeaderDateTimePickerHitHandler.cs`

| Task | Status | Notes |
|------|--------|-------|
| Painter: CalculateLayout() review | ⬜ | |
| Painter: Large header rectangle | ⬜ | Colored header |
| Painter: Compact calendar layout | ⬜ | |
| Painter: Documentation updated | ⬜ | |
| HitTestHelper: Registration verified | ⬜ | |
| HitHandler: HitTest() review | ⬜ | |
| HitHandler: HandleClick() review | ⬜ | |
| HitHandler: UpdateHoverState() review | ⬜ | |
| Testing: Header clickable | ⬜ | |
| Testing: Compact calendar | ⬜ | |
| **OVERALL STATUS** | ⬜ | |

**Hit Areas:** Header (large), DayCell

---

## BeepDateTimePickerHitTestHelper Enhancements

| Enhancement | Status | Notes |
|-------------|--------|-------|
| RegisterQuarterButtons() method | ⬜ | For Quarterly mode |
| RegisterMonthButtons() method | ⬜ | For MonthView mode |
| RegisterYearButtons() method | ⬜ | For YearView mode |
| RegisterWeekRows() method | ⬜ | For WeekView mode |
| RegisterTimelineElements() method | ⬜ | For Timeline mode |
| RegisterFlexibleRangePresets() method | ⬜ | For FlexibleRange mode |
| RegisterFilterButtons() method | ⬜ | For FilteredRange mode |
| RegisterSidebarElements() method | ⬜ | For SidebarEvent mode |
| Documentation updates | ⬜ | XML docs for all methods |

---

## Global Issues Tracking

### Common Issues Found
| Issue | Affected Modes | Status | Resolution |
|-------|----------------|--------|------------|
| | | ⬜ | |
| | | ⬜ | |
| | | ⬜ | |

### Breaking Changes Required
| Change | Impact | Status | Notes |
|--------|--------|--------|-------|
| | | ⬜ | |
| | | ⬜ | |

---

## Progress Summary

**Tier 1 (Core):** 0/4 complete (0%)
**Tier 2 (Advanced):** 0/4 complete (0%)
**Tier 3 (Specialized):** 0/4 complete (0%)
**Tier 4 (Complex):** 0/6 complete (0%)

**Overall Progress:** 0/18 complete (0%)

---

## Notes & Decisions

### Design Decisions
1. All painters MUST call `CalculateLayout()` before painting
2. Hit area naming convention: `"{type}_{identifier}"`
3. Multi-month layouts use `MonthGrids` collection
4. Hit handlers determine dropdown close behavior

### Known Limitations
- (Document any discovered limitations here)

### Future Enhancements
- (Document planned improvements here)

---

**Last Updated:** October 17, 2025
**Status:** Ready for Revision
