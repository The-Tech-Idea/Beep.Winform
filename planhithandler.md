# Hit Handler Implementation Plan
## Painter-by-Painter Analysis and Refactoring

This document provides a detailed analysis of each DateTimePicker painter's unique design and the corresponding hit handler logic required. Each painter has distinct UI elements, interaction areas, and event handling needs.

---

## 1. SingleDateTimePickerPainter
**Mode**: `DatePickerMode.Single`

### Visual Design
- Standard calendar grid (7x6 cells)
- Month/year header with navigation buttons (prev/next)
- Day names header
- Circular day cells with hover/selection states
- Today indicator (circular border)

### Interactive Areas
1. **Previous Month Button** - Top left corner
2. **Next Month Button** - Top right corner
3. **Day Cells** - 42 cells in 7x6 grid
4. **Month/Year Header** - Could trigger month/year picker (future)

### Hit Handler Logic Required
- **HitTest**: Map point to specific day cell using grid calculation
- **HandleClick**: 
  - Navigation buttons → Change display month
  - Day cell → Set SelectedDate, close dropdown
- **UpdateHoverState**: Track hovered day cell, navigation buttons
- **IsSelectionComplete**: Return true when SelectedDate is set
- **SyncFromControl**: Load current SelectedDate
- **SyncToControl**: Update owner's SelectedDate

### Unique Characteristics
- Single click selection (immediate close)
- Circular cell highlighting
- Simple, straightforward interaction model

---

## 2. SingleWithTimeDateTimePickerPainter
**Mode**: `DatePickerMode.SingleWithTime`

### Visual Design
- Calendar section (top 70%)
- Time spinner section (bottom 30%)
- Hour/minute spinners with up/down arrows
- AM/PM toggle buttons

### Interactive Areas
1. **Calendar Section** - Same as Single mode
2. **Hour Spinner Up/Down** - Increment/decrement hours
3. **Minute Spinner Up/Down** - Increment/decrement minutes
4. **AM/PM Toggle** - Switch between AM/PM
5. **Confirm Button** - Apply selection and close

### Hit Handler Logic Required
- **HitTest**: Distinguish between calendar area vs time spinner area
- **HandleClick**:
  - Calendar → Select date, show time spinners
  - Hour up/down → Adjust hours (1-12)
  - Minute up/down → Adjust minutes (0, 15, 30, 45)
  - AM/PM → Toggle time period
  - Confirm → Finalize datetime, close
- **UpdateHoverState**: Track all spinner buttons separately
- **IsSelectionComplete**: Both date AND time must be set

### Unique Characteristics
- Two-step interaction (date first, then time)
- Time spinner requires increment/decrement logic
- Doesn't close until Confirm button clicked

---

## 3. RangeDateTimePickerPainter
**Mode**: `DatePickerMode.Range`

### Visual Design
- Standard calendar grid
- Two-step selection (start date, then end date)
- Range highlighting between dates
- Visual feedback for range-in-progress

### Interactive Areas
1. **Navigation Buttons**
2. **Day Cells** - Click once for start, click again for end
3. **Range Display** - Shows selected range

### Hit Handler Logic Required
- **HitTest**: Standard day cell grid
- **HandleClick**:
  - First click → Set RangeStartDate
  - Second click → Set RangeEndDate (validate > start)
  - Navigation → Adjust month
- **UpdateHoverState**: Show preview range from start to hover
- **IsSelectionComplete**: Both RangeStartDate AND RangeEndDate set
- **State Management**: Track selection phase (waiting for start vs waiting for end)

### Unique Characteristics
- Two-click interaction model
- Range preview on hover
- Visual feedback for partial selection

---

## 4. RangeWithTimeDateTimePickerPainter
**Mode**: `DatePickerMode.RangeWithTime`

### Visual Design
- Calendar with range selection
- Time spinners for start time
- Time spinners for end time
- Confirm button

### Interactive Areas
1. **Calendar Section** - Range selection
2. **Start Time Spinners** - Hour/minute/AM-PM for start
3. **End Time Spinners** - Hour/minute/AM-PM for end
4. **Confirm Button**

### Hit Handler Logic Required
- **HitTest**: Calendar + two separate time spinner zones
- **HandleClick**:
  - Calendar → Select range
  - Start spinners → Adjust start time
  - End spinners → Adjust end time (validate > start time)
  - Confirm → Apply and close
- **IsSelectionComplete**: Range dates + both times set

### Unique Characteristics
- Combines range selection with dual time pickers
- Time validation (end must be after start)
- Multi-zone interaction

---

## 5. MultipleDateTimePickerPainter
**Mode**: `DatePickerMode.Multiple`

### Visual Design
- Calendar with checkboxes in cells
- Selected dates highlighted
- Counter showing selected count
- Clear All button

### Interactive Areas
1. **Navigation Buttons**
2. **Day Cells with Checkboxes** - Toggle selection
3. **Clear All Button** - Reset selection
4. **Done Button** - Finalize selection

### Hit Handler Logic Required
- **HitTest**: Day cells with checkbox zones
- **HandleClick**:
  - Day cell → Toggle date in SelectedDates HashSet
  - Clear All → Empty SelectedDates
  - Done → Close dropdown
- **UpdateHoverState**: Track checkbox hover states
- **IsSelectionComplete**: At least one date selected
- **State Management**: Maintain HashSet of selected dates

### Unique Characteristics
- Toggle selection (not replace)
- Multiple dates can be selected
- Explicit "Done" button required
- HashSet for efficient lookups

---

## 6. AppointmentDateTimePickerPainter
**Mode**: `DatePickerMode.Appointment`

### Visual Design
- Calendar on left (55% width)
- Scrollable time slot list on right (45% width)
- Hourly slots from 8 AM to 8 PM
- Vertical separator

### Interactive Areas
1. **Calendar Section** - Date selection
2. **Time Slot List** - Scrollable hourly slots
3. **Individual Time Slots** - Clickable hourly blocks
4. **Scroll Indicator** - If slots extend beyond view

### Hit Handler Logic Required
- **HitTest**: Split screen detection (calendar vs time slot panel)
- **HandleClick**:
  - Calendar → Select date
  - Time slot → Set SelectedTime
  - Must have both date + time to complete
- **UpdateHoverState**: Track time slot hover separately from calendar
- **IsSelectionComplete**: Date + time slot selected
- **Scroll Handling**: Track scroll position for time slots

### Unique Characteristics
- Split panel layout
- Scrollable time slot list
- Appointment-specific hourly granularity
- Two distinct interaction zones

---

## 7. TimelineDateTimePickerPainter
**Mode**: `DatePickerMode.Timeline`

### Visual Design
- Horizontal timeline bar with range
- Draggable start/end handles
- Month markers along timeline
- Mini calendar for reference
- Date labels showing current range

### Interactive Areas
1. **Timeline Track** - Background bar
2. **Start Handle** - Draggable left handle
3. **End Handle** - Draggable right handle
4. **Range Fill** - Selected period visualization
5. **Mini Calendar** - Optional calendar view

### Hit Handler Logic Required
- **HitTest**: Detect handles, timeline track, mini calendar
- **HandleClick**:
  - Start handle → Begin drag operation
  - End handle → Begin drag operation
  - Timeline track → Quick jump to position
- **HandleDrag**: Update range while dragging handles
- **UpdateHoverState**: Show handle hover, drag cursor
- **IsSelectionComplete**: Valid range with start < end
- **Coordinate Mapping**: Convert pixel position to date

### Unique Characteristics
- Drag-based interaction (not click)
- Visual timeline representation
- Handle-based range adjustment
- Continuous feedback during drag

---

## 8. QuarterlyDateTimePickerPainter
**Mode**: `DatePickerMode.Quarterly`

### Visual Design
- Quick quarter buttons (Q1, Q2, Q3, Q4)
- Year selector
- Calendar for custom range
- Fiscal year configuration option

### Interactive Areas
1. **Q1-Q4 Buttons** - Quick quarter selection
2. **Year Selector** - Dropdown or spinners
3. **Calendar Grid** - Manual date selection
4. **Custom Range Option**

### Hit Handler Logic Required
- **HitTest**: Quarter buttons, year selector, calendar
- **HandleClick**:
  - Q1 → Jan 1 - Mar 31
  - Q2 → Apr 1 - Jun 30
  - Q3 → Jul 1 - Sep 30
  - Q4 → Oct 1 - Dec 31
  - Year selector → Change year
  - Calendar → Custom range
- **IsSelectionComplete**: Quarter + year selected
- **Quarter Calculation**: Convert quarter to date range

### Unique Characteristics
- Business/financial oriented
- Quick quarter selection
- Fiscal year support
- Hybrid button + calendar approach

---

## 9. CompactDateTimePickerPainter
**Mode**: `DatePickerMode.Compact`

### Visual Design
- Minimalist calendar with reduced padding
- Smaller fonts and cells
- No decorative elements
- Compressed navigation

### Interactive Areas
1. **Compact Navigation** - Smaller buttons
2. **Compact Day Cells** - Reduced size grid
3. **Compact Header** - Minimal text

### Hit Handler Logic Required
- **HitTest**: Same as Single but with smaller hit zones
- **HandleClick**: Standard single date selection
- **Precision**: Smaller cells require accurate hit testing
- **IsSelectionComplete**: Single date selected

### Unique Characteristics
- Space-optimized layout
- Smaller hit targets (need precise detection)
- Same logic as Single mode, different dimensions

---

## 10. ModernCardDateTimePickerPainter
**Mode**: `DatePickerMode.ModernCard`

### Visual Design
- Card-style container with shadow
- Quick date buttons at top (Today, Tomorrow, Next Week, Custom)
- Calendar below separator
- Modern rounded corners

### Interactive Areas
1. **Quick Date Buttons** (2x2 grid)
   - Today
   - Tomorrow
   - Next Week
   - Next Month
2. **Calendar Section** - Standard grid
3. **Custom Button** - Opens full calendar

### Hit Handler Logic Required
- **HitTest**: Quick button grid + calendar area
- **HandleClick**:
  - Today → Set to DateTime.Today, close
  - Tomorrow → Set to DateTime.Today.AddDays(1), close
  - Next Week → Set to DateTime.Today.AddDays(7), close
  - Next Month → Set to DateTime.Today.AddMonths(1), close
  - Calendar → Manual selection
- **QuickButtonHelper**: Use `DateTimePickerQuickButtonHelper.GetQuickButtonDefinitions()`
- **IsSelectionComplete**: Date set via button or calendar

### Unique Characteristics
- Quick action buttons
- Card aesthetic (shadows, rounded corners)
- Two interaction modes (quick vs manual)
- Helper class for button definitions

---

## 11. DualCalendarDateTimePickerPainter
**Mode**: `DatePickerMode.DualCalendar`

### Visual Design
- Two side-by-side calendars
- Left: Current month
- Right: Next month
- Range selection across both calendars
- Range info display at bottom

### Interactive Areas
1. **Left Calendar** - Current month with navigation
2. **Right Calendar** - Next month (auto-advances)
3. **Day Cells** (84 total: 42 per calendar)
4. **Navigation** - Only on left calendar
5. **Range Info Bar** - Shows selected range

### Hit Handler Logic Required
- **HitTest**: Determine which calendar (left vs right), then day cell
- **HandleClick**:
  - Left calendar navigation → Adjust both calendars
  - Day cell (either calendar) → Range selection logic
  - First click → Start date
  - Second click → End date
- **UpdateHoverState**: Show range preview across both calendars
- **IsSelectionComplete**: Valid range with both dates
- **Cross-Calendar Range**: Handle range spanning both months

### Unique Characteristics
- Dual calendar synchronization
- Cross-calendar range visualization
- Single navigation controls both views
- Range preview across panels

---

## 12. WeekViewDateTimePickerPainter
**Mode**: `DatePickerMode.WeekView`

### Visual Design
- Week number column on left
- Full week selection (entire row)
- Week-based navigation
- Selected week info display

### Interactive Areas
1. **Week Number Column** - Click to select entire week
2. **Calendar Rows** - Entire row is clickable
3. **Navigation** - Previous/next week
4. **Week Info Bar** - Shows week number and range

### Hit Handler Logic Required
- **HitTest**: Detect week number or calendar row
- **HandleClick**:
  - Week number → Select that entire week
  - Any day in row → Select entire week (7 days)
  - Navigation → Move by weeks
- **Week Calculation**: Get week start (FirstDayOfWeek) and end
- **IsSelectionComplete**: Week selected (7-day range)
- **Week Number Display**: Use CultureInfo for week numbering

### Unique Characteristics
- Row-based selection (not individual cells)
- Week number integration
- Week-centric navigation
- ISO week standard support

---

## 13. MonthViewDateTimePickerPainter
**Mode**: `DatePickerMode.MonthView`

### Visual Design
- 12 month buttons in 3x4 grid
- Year selector at top
- Selected month highlighted
- Quarter indicators

### Interactive Areas
1. **Year Navigation** - Prev/next year
2. **Month Buttons** (12 buttons: Jan-Dec)
3. **Year Display/Selector**

### Hit Handler Logic Required
- **HitTest**: Map to month button (1-12) or year navigation
- **HandleClick**:
  - Month button → Select entire month (first to last day)
  - Year navigation → Change year
- **Month Range**: Convert month selection to date range
- **IsSelectionComplete**: Month + year selected

### Unique Characteristics
- Month-level granularity (not days)
- Grid of month buttons
- Entire month selection
- Quarter visual grouping

---

## 14. YearViewDateTimePickerPainter
**Mode**: `DatePickerMode.YearView`

### Visual Design
- 12 year buttons in 3x4 grid
- Decade navigation (e.g., 2020-2031)
- Current year highlighted
- Decade range display

### Interactive Areas
1. **Decade Navigation** - Prev/next decade
2. **Year Buttons** (12 years per view)
3. **Decade Display**

### Hit Handler Logic Required
- **HitTest**: Map to year button or decade navigation
- **HandleClick**:
  - Year button → Select entire year (Jan 1 - Dec 31)
  - Decade navigation → Move by 12 years
- **Year Range**: Convert year to full date range
- **IsSelectionComplete**: Year selected

### Unique Characteristics
- Year-level granularity
- Decade-based navigation
- 12-year grid display
- Entire year selection

---

## 15. SidebarEventDateTimePickerPainter
**Mode**: `DatePickerMode.SidebarEvent`

### Visual Design
- Event list sidebar on left
- Calendar on right
- Time picker below calendar
- Event indicators on calendar dates

### Interactive Areas
1. **Event List** - Clickable event items
2. **Calendar Grid** - Date selection
3. **Time Picker** - Hour/minute/period
4. **Add Event Button**

### Hit Handler Logic Required
- **HitTest**: Sidebar vs calendar vs time picker zones
- **HandleClick**:
  - Event item → Load event date/time
  - Calendar → Select date for new event
  - Time picker → Set event time
  - Add event → Create event with date/time
- **Event Management**: Track events, display on calendar
- **IsSelectionComplete**: Date + time for event

### Unique Characteristics
- Event management integration
- Three-panel layout
- Event list synchronization
- Calendar event indicators

---

## 16. FlexibleRangeDateTimePickerPainter
**Mode**: `DatePickerMode.FlexibleRange`

### Visual Design
- Preset range buttons (Last 7 Days, Last 30 Days, etc.)
- Dual calendar for custom range
- Quick range vs custom toggle
- Apply/Cancel buttons

### Interactive Areas
1. **Preset Range Buttons** - Quick date ranges
2. **Custom Range Toggle** - Switch to manual
3. **Dual Calendar** - When in custom mode
4. **Apply/Cancel Buttons**

### Hit Handler Logic Required
- **HitTest**: Preset buttons vs calendar area
- **HandleClick**:
  - Preset button → Calculate range, set immediately
  - Custom toggle → Show dual calendar
  - Calendar → Range selection
  - Apply → Commit custom range
  - Cancel → Revert to previous
- **Range Presets**:
  - Today, Yesterday
  - Last 7 Days, Last 30 Days
  - This Week, This Month
  - Last Week, Last Month
- **IsSelectionComplete**: Preset selected OR custom range with Apply

### Unique Characteristics
- Preset + custom hybrid
- Apply/Cancel workflow
- Range preset calculations
- Toggle between modes

---

## 17. FilteredRangeDateTimePickerPainter
**Mode**: `DatePickerMode.FilteredRange`

### Visual Design
- Left sidebar (25% width) with filter buttons
- Main area (75% width) with:
  - Dual calendar with year dropdowns
  - Time pickers (start + end)
  - Action buttons (Reset, Show Results)

### Interactive Areas
1. **Filter Sidebar** (6 buttons):
   - Past Week
   - Past Month
   - Past 3 Months
   - Past 6 Months
   - Past Year
   - Past Century
2. **Year Selectors** - Dropdown for each calendar
3. **Dual Calendar** - Range selection
4. **Start Time Picker** - Hour/minute/period
5. **End Time Picker** - Hour/minute/period
6. **Reset Button** - Clear all selections
7. **Show Results Button** - Apply and close

### Hit Handler Logic Required
- **HitTest**: Sidebar vs main area, then sub-zones
- **HandleClick**:
  - Filter button → Calculate past range, highlight selection
  - Year selector → Change calendar year
  - Calendar → Manual range selection
  - Time spinners → Adjust start/end times
  - Reset → Clear all (today's date)
  - Show Results → Apply and close
- **Filter Calculations**:
  - Past Week: Today - 7 days to today
  - Past Month: Today - 30 days to today
  - Past 3 Months: Today - 90 days to today
  - Past 6 Months: Today - 180 days to today
  - Past Year: Today - 365 days to today
  - Past Century: Today - 36500 days to today
- **State Management**: Track selected filter, custom overrides
- **IsSelectionComplete**: Valid range with times OR filter selected

### Unique Characteristics
- Most complex painter
- Four distinct zones (sidebar, calendars, times, actions)
- Filter + manual hybrid
- Analytics/reporting oriented
- Year dropdown integration
- Dual time pickers
- Explicit apply action

---

## 18. HeaderDateTimePickerPainter
**Mode**: `DatePickerMode.Header`

### Visual Design
- Prominent header with large month/year display
- Navigation integrated into header
- Calendar below with emphasis on header
- Minimalist day grid

### Interactive Areas
1. **Large Header Area** - Month/year display
2. **Integrated Navigation** - Part of header design
3. **Calendar Grid** - Standard day cells

### Hit Handler Logic Required
- **HitTest**: Header navigation vs calendar grid
- **HandleClick**:
  - Header navigation → Change month
  - Day cell → Select date
- **IsSelectionComplete**: Single date selected

### Unique Characteristics
- Prominent header design
- Navigation as part of header aesthetic
- Same logic as Single mode with different layout
- Visual emphasis on header

---

## Implementation Priority

### Phase 1: Foundation Patterns (Implement First)
1. **SingleDateTimePickerHitHandler** - Base pattern for simple selection
2. **RangeDateTimePickerHitHandler** - Base pattern for range selection
3. **ModernCardDateTimePickerHitHandler** - Base pattern for quick buttons

### Phase 2: Enhanced Interactions
4. **SingleWithTimeDateTimePickerHitHandler** - Time spinner pattern
5. **MultipleDateTimePickerHitHandler** - Toggle selection pattern
6. **AppointmentDateTimePickerHitHandler** - Split panel pattern

### Phase 3: Specialized Views
7. **WeekViewDateTimePickerHitHandler** - Row selection pattern
8. **MonthViewDateTimePickerHitHandler** - Month grid pattern
9. **YearViewDateTimePickerHitHandler** - Year grid pattern

### Phase 4: Advanced Features
10. **TimelineDateTimePickerHitHandler** - Drag interaction pattern
11. **DualCalendarDateTimePickerHitHandler** - Dual panel pattern
12. **FilteredRangeDateTimePickerHitHandler** - Multi-zone complex pattern

### Phase 5: Variants
13. **CompactDateTimePickerHitHandler** - Size variant of Single
14. **HeaderDateTimePickerHitHandler** - Layout variant of Single
15. **QuarterlyDateTimePickerHitHandler** - Business calendar variant
16. **FlexibleRangeDateTimePickerHitHandler** - Preset range variant
17. **RangeWithTimeDateTimePickerHitHandler** - Time-enhanced range
18. **SidebarEventDateTimePickerHitHandler** - Event management variant

---

## Common Patterns Across Handlers

### Pattern 1: Basic Calendar Navigation
- Previous/Next month buttons (fixed positions)
- Header click (optional month/year picker)

### Pattern 2: Day Cell Grid Hit Testing
```csharp
// Common grid calculation (7 columns x 6 rows)
int cellWidth = gridWidth / 7;
int cellHeight = gridHeight / 6;
int col = (point.X - gridLeft) / cellWidth;
int row = (point.Y - gridTop) / cellHeight;
int cellIndex = row * 7 + col;
```

### Pattern 3: Time Spinner Interaction
- Up/Down buttons for hours/minutes
- Increment/decrement with wrapping
- AM/PM toggle

### Pattern 4: Range Selection State Machine
```
State: WaitingForStart -> WaitingForEnd -> Complete
- First click: Set start, transition to WaitingForEnd
- Second click: Set end, transition to Complete
- Hover: Show preview range
```

### Pattern 5: Quick Button Layouts
- Use `DateTimePickerQuickButtonHelper` for consistency
- Grid-based button arrangement
- Immediate selection and close

---

## Next Steps

1. **Review each painter's Paint methods** to identify exact UI element bounds
2. **Extract hit area constants** from each painter for handler to use
3. **Implement handlers one-by-one** following priority order
4. **Create unit tests** for each handler's hit testing logic
5. **Integrate with BeepDateTimePicker** for end-to-end testing
6. **Document interaction patterns** for future reference

---

## Notes for Implementation

### Layout Coordination
- Each handler needs access to the same `CalculateLayout()` method used by its painter
- Consider extracting layout calculation to shared helper class
- Hit areas must match painter's visual layout exactly

### State Synchronization
- `SyncFromControl()`: Load owner's current state into handler
- `SyncToControl()`: Update owner's state from handler
- Handler state must remain consistent with visual state

### Hover Feedback
- `UpdateHoverState()` creates `DateTimePickerHoverState` object
- Painter uses hover state to render hover effects
- Handler must track all hover areas (buttons, cells, spinners, etc.)

### Selection Validation
- Range end must be >= start
- Time end must be >= start (if same date)
- Week selection must be 7 consecutive days
- Month/year selections must be valid dates

### Event Management
- Each click generates appropriate events (DateChanged, RangeChanged, etc.)
- Selection complete should trigger SelectionCompleted event
- Hover changes should trigger visual updates only

---

**End of Plan Document**
