# Beep Date Controls Documentation

## Overview

The Beep Date Controls provide a comprehensive set of components for date and time selection in WinForms applications. The system consists of two main controls that work together:

1. **BeepDateDropDown** - A text-based date input control with dropdown calendar
2. **BeepDateTimePicker** - A visual calendar/time picker control

Both controls share a common architecture and use the **ReturnDateTimeType** enum as the single source of truth for determining what type of value is returned.

---

## Table of Contents

- [Architecture](#architecture)
- [Return Types](#return-types)
- [BeepDateDropDown Control](#beepdatedropdown-control)
- [BeepDateTimePicker Control](#beepdatetimepicker-control)
- [Mode Mapping](#mode-mapping)
- [Usage Examples](#usage-examples)
- [Bidirectional Sync](#bidirectional-sync)
- [Segment-Based Editing](#segment-based-editing)
- [Theming](#theming)
- [Best Practices](#best-practices)

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    User Application                         │
└─────────────────────────────────────────────────────────────┘
                              │
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                  ReturnDateTimeType (Enum)                  │
│              SINGLE SOURCE OF TRUTH                         │
│  • Date      • DateTime     • DateRange                     │
│  • DateTimeRange  • MultipleDates  • TimeOnly               │
└─────────────────────────────────────────────────────────────┘
                              │
                ┌─────────────┴─────────────┐
                ↓                           ↓
┌───────────────────────────┐   ┌───────────────────────────┐
│   BeepDateDropDown        │   │  BeepDateTimePicker       │
│   (Text Input + Popup)    │◄─►│  (Visual Calendar)        │
│                           │   │                           │
│  • Inherits BeepTextBox   │   │  • 18 Different Modes     │
│  • Masking & Validation   │   │  • Multiple Painters      │
│  • Segment Editing        │   │  • Themeable UI           │
│  • Inline Text Entry      │   │  • Time Selection         │
└───────────────────────────┘   └───────────────────────────┘
```

### Inheritance Hierarchy

```
BaseControl
    ↓
BeepTextBox (advanced text input with masking)
    ↓
BeepDateDropDown (date-specific functionality + dropdown)
```

### Key Components

- **DateTimePickerProperties** - Shared configuration class
- **DatePickerMode** - Enum defining visual/functional modes (18+ modes)
- **ReturnDateTimeType** - Enum defining what value type is returned
- **DatePickerModeMapping** - Maps each mode to its return type

---

## Return Types

The **ReturnDateTimeType** enum is the single source of truth that determines:
- What type of value the control returns
- How the text input is masked
- What UI elements are displayed
- How validation works

### Available Return Types

| Return Type | Returns | Time Included | Example |
|------------|---------|---------------|---------|
| **Date** | `DateTime?` | No (00:00:00) | `10/15/2025` |
| **DateTime** | `DateTime?` | Yes | `10/15/2025 2:30 PM` |
| **DateRange** | `(DateTime? start, DateTime? end)` | No | `10/15/2025 - 10/20/2025` |
| **DateTimeRange** | `(DateTime? start, DateTime? end)` | Yes | `10/15/2025 9:00 AM - 10/15/2025 5:00 PM` |
| **MultipleDates** | `DateTime[]` | No | `[10/15/2025, 10/16/2025, 10/20/2025]` |
| **TimeOnly** | `TimeSpan?` | N/A | `14:30:00` |

### Setting Return Type

```csharp
// Option 1: Set ReturnType directly (recommended)
datePicker.ReturnType = ReturnDateTimeType.DateTimeRange;
// Mode automatically becomes RangeWithTime (or Timeline, FlexibleRange, etc.)

// Option 2: Set Mode (ReturnType auto-syncs)
datePicker.Mode = DatePickerMode.Quarterly;
// ReturnType automatically becomes DateRange
```

---

## BeepDateDropDown Control

### Overview

**BeepDateDropDown** is a hybrid control that combines text input with a dropdown calendar. It inherits from **BeepTextBox**, providing advanced features like masking, validation, autocomplete, and segment-based editing.

### Key Features

#### 1. **Inline Text Editing**
- Type dates directly with automatic formatting
- Real-time validation
- Custom mask patterns
- Date shortcuts (Today, Tomorrow, etc.)

#### 2. **Dropdown Calendar**
- Visual date selection using BeepDateTimePicker
- F4 or Alt+Down to open
- Escape to close
- Automatic sync between text and calendar

#### 3. **Segment-Based Editing**
- Click on day, month, or year individually
- Hover highlights for visual feedback
- Tab/Shift+Tab to navigate segments
- Smart input validation per segment

#### 4. **Range Support**
- Single date or date range
- Date-only or date+time
- Configurable separator (default: " - ")
- Automatic validation (end >= start)

### Properties

#### Core Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| **ReturnType** | `ReturnDateTimeType` | `Date` | **SINGLE SOURCE OF TRUTH** - Defines what value type is returned |
| **Mode** | `DatePickerMode` | `Single` | Visual/functional mode (syncs with ReturnType) |
| **SelectedDateTime** | `DateTime?` | `null` | Currently selected date/time (for single modes) |
| **StartDate** | `DateTime?` | `null` | Start date for range modes |
| **EndDate** | `DateTime?` | `null` | End date for range modes |

#### Strongly-Typed Value Properties

| Property | Type | Description |
|----------|------|-------------|
| **DateValue** | `DateTime?` | Use for `ReturnType.Date` mode |
| **DateTimeValue** | `DateTime?` | Use for `ReturnType.DateTime` mode |
| **DateRangeValue** | `DateRange` | Use for `ReturnType.DateRange` mode |
| **DateTimeRangeValue** | `DateTimeRange` | Use for `ReturnType.DateTimeRange` mode |

#### Constraints

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| **MinDate** | `DateTime?` | `null` | Minimum allowed date |
| **MaxDate** | `DateTime?` | `null` | Maximum allowed date |
| **AllowEmpty** | `bool` | `true` | Allow null/empty values |

#### Appearance

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| **ShowDropDown** | `bool` | `true` | Show/hide dropdown calendar button |
| **ShowCalendarIcon** | `bool` | `true` | Show/hide calendar icon |
| **CalendarIconPath** | `string` | (default) | Path to custom calendar icon |
| **DateSeparator** | `string` | `" - "` | Separator for date ranges |
| **DateFormat** | `string` | `"MM/dd/yyyy"` | Date format string |
| **TimeFormat** | `string` | `"h:mm tt"` | Time format string |
| **DateTimeFormat** | `string` | `"MM/dd/yyyy h:mm tt"` | Combined format |

#### Behavior

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| **AllowInlineEdit** | `bool` | `true` | Allow typing dates directly |
| **EnableSegmentEditing** | `bool` | `true` | Enable clicking individual segments (day/month/year) |
| **ValidateOnKeyPress** | `bool` | `true` | Validate as user types |
| **ValidateOnLostFocus** | `bool` | `true` | Validate when control loses focus |

### Events

```csharp
// Date selection events
public event EventHandler<DateTimePickerEventArgs> SelectedDateTimeChanged;
public event EventHandler<DateRangeEventArgs> DateRangeChanged;

// Dropdown events
public event EventHandler DropDownOpened;
public event EventHandler DropDownClosed;

// Inherited from BeepTextBox
public event EventHandler TextChanged;
public event EventHandler TypingStarted;
public event EventHandler TypingStopped;
```

### Methods

```csharp
// Value management
bool HasValue()                    // Check if control has valid value
void ClearValue()                  // Clear value based on mode
string GetValueDescription()       // Get human-readable description
object Value { get; set; }         // Generic value property (auto-casts)

// Popup control
void ShowPopup()                   // Show dropdown calendar
void ClosePopup()                  // Close dropdown calendar
void TogglePopup()                 // Toggle dropdown state

// Date parsing & formatting
DateTime? ParseDate(string text)
(DateTime? start, DateTime? end) ParseDateRange(string text)
string FormatDate(DateTime? date)
string FormatDateRange(DateTime? start, DateTime? end)

// Validation
bool IsValidDate(string text)
bool IsValidDateRange(string text)
bool IsDateInRange(DateTime? date)

// Sync with BeepDateTimePicker
DateTimePickerProperties GetProperties()      // Get current state
void SetProperties(DateTimePickerProperties)  // Load state
```

### Usage Examples

#### Example 1: Simple Date Picker
```csharp
var datePicker = new BeepDateDropDown
{
    ReturnType = ReturnDateTimeType.Date,
    PlaceholderText = "Select a date",
    MinDate = DateTime.Today,
    MaxDate = DateTime.Today.AddYears(1)
};

datePicker.SelectedDateTimeChanged += (s, e) =>
{
    if (e.Date.HasValue)
        Console.WriteLine($"Selected: {e.Date:d}");
};
```

#### Example 2: Date-Time Picker
```csharp
var dateTimePicker = new BeepDateDropDown
{
    ReturnType = ReturnDateTimeType.DateTime,
    DateTimeFormat = "MM/dd/yyyy hh:mm tt",
    PlaceholderText = "MM/DD/YYYY HH:MM AM/PM"
};

DateTime? selectedDateTime = dateTimePicker.DateTimeValue;
if (selectedDateTime.HasValue)
{
    Console.WriteLine($"Date: {selectedDateTime.Value:d}");
    Console.WriteLine($"Time: {selectedDateTime.Value:t}");
}
```

#### Example 3: Date Range Picker
```csharp
var rangePicker = new BeepDateDropDown
{
    ReturnType = ReturnDateTimeType.DateRange,
    DateSeparator = " to ",
    PlaceholderText = "MM/DD/YYYY to MM/DD/YYYY"
};

rangePicker.DateRangeChanged += (s, e) =>
{
    if (e.StartDate.HasValue && e.EndDate.HasValue)
    {
        var range = rangePicker.DateRangeValue;
        Console.WriteLine($"Range: {range}");
        Console.WriteLine($"Days: {range.Days}");
    }
};
```

#### Example 4: Date-Time Range (Meeting Scheduler)
```csharp
var meetingPicker = new BeepDateDropDown
{
    ReturnType = ReturnDateTimeType.DateTimeRange,
    Mode = DatePickerMode.Timeline, // Or use default RangeWithTime
    DateSeparator = " → ",
    MinDate = DateTime.Today
};

var meeting = meetingPicker.DateTimeRangeValue;
if (meeting.IsValid)
{
    Console.WriteLine($"Meeting: {meeting.Start:g} to {meeting.End:g}");
    Console.WriteLine($"Duration: {meeting.TotalHours:F1} hours");
}
```

#### Example 5: Segment Editing
```csharp
var segmentPicker = new BeepDateDropDown
{
    ReturnType = ReturnDateTimeType.Date,
    EnableSegmentEditing = true,
    DateFormat = "MM/dd/yyyy"
};

// Users can:
// - Hover over day/month/year to see highlight
// - Click on a segment to select just that part
// - Type to change only that segment
// - Press Tab to move to next segment
// - See tooltips: "Day (DD)", "Month (MM)", "Year (YYYY)"
```

#### Example 6: Using Different Modes for Same ReturnType
```csharp
// All these modes return DateRange:
var standardRange = new BeepDateDropDown { Mode = DatePickerMode.Range };
var dualCalendar = new BeepDateDropDown { Mode = DatePickerMode.DualCalendar };
var quarterly = new BeepDateDropDown { Mode = DatePickerMode.Quarterly };

// All return DateRange, but with different visual styles
DateRange range1 = standardRange.DateRangeValue;
DateRange range2 = dualCalendar.DateRangeValue;
DateRange range3 = quarterly.DateRangeValue;
```

### Keyboard Shortcuts

| Key | Action |
|-----|--------|
| **F4** | Open/close dropdown calendar |
| **Alt+Down** | Open dropdown calendar |
| **Escape** | Close dropdown or clear value |
| **Tab** | Move to next segment (if segment editing enabled) |
| **Shift+Tab** | Move to previous segment |
| **Ctrl+Z** | Undo (inherited from BeepTextBox) |
| **Ctrl+Y** | Redo (inherited from BeepTextBox) |

---

## BeepDateTimePicker Control

### Overview

**BeepDateTimePicker** is a visual calendar control with 18+ different modes/painters. It serves as the popup calendar for BeepDateDropDown or can be used standalone.

### Available Modes

| Mode | ReturnType | Description |
|------|-----------|-------------|
| **Single** | `Date` | Standard calendar with single date selection |
| **SingleWithTime** | `DateTime` | Calendar + time picker |
| **Range** | `DateRange` | Date range selection |
| **RangeWithTime** | `DateTimeRange` | Date range + time selection |
| **Multiple** | `MultipleDates` | Multiple date selection with checkboxes |
| **Appointment** | `DateTime` | Calendar with time slot list |
| **Timeline** | `DateTimeRange` | Date range with visual timeline |
| **Quarterly** | `DateRange` | Quarterly range selector (Q1-Q4) |
| **Compact** | `Date` | Minimal compact calendar |
| **ModernCard** | `Date` | Modern card with quick buttons (Today, Tomorrow) |
| **DualCalendar** | `DateRange` | Side-by-side calendars for ranges |
| **WeekView** | `Date` | Week-based calendar |
| **MonthView** | `Date` | Month picker |
| **YearView** | `Date` | Year picker |
| **SidebarEvent** | `Date` | Calendar with event sidebar |
| **FlexibleRange** | `DateTimeRange` | Flexible range with tabs |
| **FilteredRange** | `DateTimeRange` | Range with filter sidebar |
| **Header** | `Date` | Prominent header calendar |

#### RangeWithTime Mode Highlights
- Dual calendar layout computes dedicated rectangles for start/end hour and minute spinners, enabling precise hit registration through `BeepDateTimePickerHitTestHelper`.
- `RangeWithTimeHitHandler` now responds to spinner hit areas (e.g., `time_start_hour_up`) to wrap hours/minutes and preserve an ordered range while users adjust times.
- Hover state surfaces spinner focus using `DateTimePickerHoverState.QuickButton`, keeping visual feedback consistent with other interactive elements.

### Properties

#### Core Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| **Mode** | `DatePickerMode` | `Single` | Visual/functional mode |
| **Properties** | `DateTimePickerProperties` | (new) | Configuration object |
| **Theme** | `BeepTheme` | (current) | Visual theme |

#### Date Selection (via Properties)

| Property | Type | Description |
|----------|------|-------------|
| **ReturnType** | `ReturnDateTimeType` | Type of value returned |
| **SelectedDate** | `DateTime?` | Selected date (for single modes) |
| **SelectedTime** | `TimeSpan?` | Selected time |
| **RangeStartDate** | `DateTime?` | Range start |
| **RangeEndDate** | `DateTime?` | Range end |
| **MinDate** | `DateTime?` | Minimum date |
| **MaxDate** | `DateTime?` | Maximum date |

#### Display Settings

| Property | Type | Description |
|----------|------|-------------|
| **Format** | `DatePickerFormat` | Date format (Long, Short, Custom) |
| **CalendarType** | `DatePickerCalendarType` | Gregorian, Hijri, Hebrew, etc. |
| **FirstDayOfWeek** | `DatePickerFirstDayOfWeek` | Sunday, Monday, etc. |
| **ShowWeekNumbers** | `DatePickerWeekNumbers` | Week number display |
| **ShowTime** | `bool` | Show time picker |
| **TimeFormat** | `string` | Time format string |
| **TimeInterval** | `TimeSpan` | Time slot interval |

#### Behavior

| Property | Type | Description |
|----------|------|-------------|
| **CloseOnSelection** | `bool` | Auto-close after selection |
| **AllowClear** | `bool` | Show clear button |
| **ShowApplyButton** | `bool` | Show apply button |
| **ShowCancelButton** | `bool` | Show cancel button |

### Events

```csharp
public event EventHandler<DateTimePickerEventArgs> DateChanged;
public event EventHandler<DateTimePickerEventArgs> SelectionChanged;
public event EventHandler ViewChanged;
```

### Methods

```csharp
void RefreshDisplay()              // Force UI update
void NavigateToDate(DateTime date) // Jump to specific date
void ClearSelection()              // Clear selected date(s)
```

### Usage Examples

#### Example 1: Standalone Calendar
```csharp
var calendar = new BeepDateTimePicker
{
    Mode = DatePickerMode.Single,
    Dock = DockStyle.Fill
};

calendar.DateChanged += (s, e) =>
{
    if (e.Date.HasValue)
        Console.WriteLine($"Selected: {e.Date:d}");
};

form.Controls.Add(calendar);
```

#### Example 2: Appointment Scheduler
```csharp
var appointmentCalendar = new BeepDateTimePicker
{
    Mode = DatePickerMode.Appointment,
    Properties = new DateTimePickerProperties
    {
        ReturnType = ReturnDateTimeType.DateTime,
        ShowTime = true,
        TimeInterval = TimeSpan.FromMinutes(30),
        AppointmentStartHour = TimeSpan.FromHours(8),
        AppointmentEndHour = TimeSpan.FromHours(18)
    }
};
```

#### Example 3: Quarterly Report Range
```csharp
var quarterlyPicker = new BeepDateTimePicker
{
    Mode = DatePickerMode.Quarterly,
    Properties = new DateTimePickerProperties
    {
        ReturnType = ReturnDateTimeType.DateRange,
        ShowQuarterButtons = true
    }
};

quarterlyPicker.DateChanged += (s, e) =>
{
    var props = quarterlyPicker.Properties;
    if (props.RangeStartDate.HasValue && props.RangeEndDate.HasValue)
    {
        Console.WriteLine($"Q Range: {props.RangeStartDate:d} to {props.RangeEndDate:d}");
    }
};
```

---

## Mode Mapping

The **DatePickerModeMapping** class defines which modes return which types:

```csharp
// Get return type for a mode
var returnType = DatePickerModeMapping.GetReturnType(DatePickerMode.Timeline);
// Returns: ReturnDateTimeType.DateTimeRange

// Get compatible modes for a return type
var modes = DatePickerModeMapping.GetCompatibleModes(ReturnDateTimeType.DateRange);
// Returns: [Range, DualCalendar, Quarterly]

// Get default mode for a return type
var mode = DatePickerModeMapping.GetDefaultMode(ReturnDateTimeType.DateTime);
// Returns: DatePickerMode.SingleWithTime

// Check compatibility
bool compatible = DatePickerModeMapping.IsCompatible(
    DatePickerMode.Timeline,
    ReturnDateTimeType.DateTimeRange
); // Returns: true
```

### Complete Mapping Table

| ReturnType | Compatible Modes |
|-----------|------------------|
| **Date** | Single, Compact, ModernCard, WeekView, MonthView, YearView, Header, SidebarEvent |
| **DateTime** | SingleWithTime, Appointment |
| **DateRange** | Range, DualCalendar, Quarterly |
| **DateTimeRange** | RangeWithTime, Timeline, FlexibleRange, FilteredRange |
| **MultipleDates** | Multiple |

---

## Bidirectional Sync

BeepDateDropDown and BeepDateTimePicker sync through **DateTimePickerProperties**:

### Flow Diagram

```
User Types in BeepDateDropDown
        ↓
Text parsed to date values
        ↓
ShowPopup() called
        ↓
SyncToCalendar() → Pushes ReturnType + values to calendar
        ↓
BeepDateTimePicker displays with correct mode
        ↓
User selects date in calendar
        ↓
SyncFromCalendar() → Reads values (not ReturnType!)
        ↓
BeepDateDropDown text updated
        ↓
Events fired
```

### Important Rules

1. **ReturnType flows ONE WAY**: BeepDateDropDown → BeepDateTimePicker
2. **Calendar is READ-ONLY** for ReturnType (never modifies it)
3. **Date values flow BOTH WAYS**: Text ↔ Calendar
4. **No circular updates**: Calendar changes don't affect ReturnType

### Sync Methods

```csharp
// In BeepDateDropDown
private void SyncToCalendar()      // Push state TO calendar
private void SyncFromCalendar()    // Read values FROM calendar

// Public methods
DateTimePickerProperties GetProperties()  // Get current state
void SetProperties(DateTimePickerProperties props) // Load state
```

---

## Segment-Based Editing

### Overview

Segment editing allows users to click on individual parts of a date (day, month, year, hour, minute) for precise editing.

### Features

- **Visual Highlights**: Hover over segments to see which part will be edited
- **Click to Select**: Click on day, month, or year to select just that segment
- **Smart Input**: Each segment validates input (only digits for dates, A/P/M for AM/PM)
- **Keyboard Navigation**: Tab through segments
- **Tooltips**: Shows "Day (DD)", "Month (MM)", etc. on hover
- **Range Support**: Works for both start and end dates in range mode

### Segment Types

```csharp
internal enum DateSegmentType
{
    Day,        // Day of month (01-31)
    Month,      // Month (01-12)
    Year,       // Year (2025, etc.)
    Hour,       // Hour (00-23 or 01-12)
    Minute,     // Minute (00-59)
    Second,     // Second (00-59)
    AmPm,       // AM/PM indicator
    Separator   // Separators like /, -, :
}
```

### Configuration

```csharp
var datePicker = new BeepDateDropDown
{
    EnableSegmentEditing = true,  // Enable feature
    DateFormat = "MM/dd/yyyy",    // Format determines segments
    ReturnType = ReturnDateTimeType.Date
};

// Segments calculated automatically:
// - Month segment at positions 0-1
// - Day segment at positions 3-4
// - Year segment at positions 6-9
```

### Keyboard Navigation

```csharp
// Tab: Move to next segment
// Shift+Tab: Move to previous segment
// Numbers: Type in current segment
// Escape: Cancel editing
```

---

## Theming

Both controls support **BeepTheme** for consistent styling:

```csharp
var theme = new BeepTheme
{
    CalendarBackColor = Color.White,
    CalendarForeColor = Color.Black,
    CalendarTitleBackColor = Color.DodgerBlue,
    CalendarSelectedBackColor = Color.CornflowerBlue,
    CalendarTodayBackColor = Color.LightBlue,
    AccentColor = Color.DodgerBlue
};

datePicker.Theme = theme;
calendar.Theme = theme;
```

### Theme Properties

- **CalendarBackColor**: Main background
- **CalendarForeColor**: Text color
- **CalendarTitleBackColor**: Header background
- **CalendarSelectedBackColor**: Selected date background
- **CalendarTodayBackColor**: Today's date background
- **AccentColor**: Highlights and focus

---

## Best Practices

### 1. Choose the Right ReturnType

```csharp
// ✅ Good: Clear intent
datePicker.ReturnType = ReturnDateTimeType.DateRange;

// ❌ Avoid: Setting mode without considering return type
datePicker.Mode = DatePickerMode.Range; // ReturnType auto-syncs, but less explicit
```

### 2. Use Strongly-Typed Properties

```csharp
// ✅ Good: Type-safe
DateRange range = datePicker.DateRangeValue;
if (range.IsValid)
{
    ProcessRange(range.Start.Value, range.End.Value);
}

// ❌ Avoid: Generic Value property with casting
var value = datePicker.Value;
if (value is (DateTime? start, DateTime? end) range)
{
    // More error-prone
}
```

### 3. Validate User Input

```csharp
// ✅ Good: Check constraints
datePicker.MinDate = DateTime.Today;
datePicker.MaxDate = DateTime.Today.AddMonths(6);
datePicker.AllowEmpty = false;

datePicker.SelectedDateTimeChanged += (s, e) =>
{
    if (!datePicker.HasValue())
    {
        MessageBox.Show("Please select a date");
        return;
    }
    
    if (!datePicker.IsDateInRange(e.Date))
    {
        MessageBox.Show("Date is out of range");
        return;
    }
};
```

### 4. Use Appropriate Modes

```csharp
// ✅ Good: Match mode to use case
var vacationPicker = new BeepDateDropDown
{
    Mode = DatePickerMode.DualCalendar, // Visual dual-calendar for ranges
    ReturnType = ReturnDateTimeType.DateRange
};

var appointmentPicker = new BeepDateDropDown
{
    Mode = DatePickerMode.Appointment, // Time slots for scheduling
    ReturnType = ReturnDateTimeType.DateTime
};

var quarterlyReport = new BeepDateDropDown
{
    Mode = DatePickerMode.Quarterly, // Q1-Q4 shortcuts
    ReturnType = ReturnDateTimeType.DateRange
};
```

### 5. Handle Events Properly

```csharp
// ✅ Good: Check for null and validate
datePicker.DateRangeChanged += (s, e) =>
{
    if (!e.StartDate.HasValue || !e.EndDate.HasValue)
        return;
    
    if (e.EndDate < e.StartDate)
    {
        MessageBox.Show("End date must be after start date");
        return;
    }
    
    // Process valid range
    ProcessDateRange(e.StartDate.Value, e.EndDate.Value);
};
```

### 6. Sync State When Needed

```csharp
// ✅ Good: Save/restore state
// Save
var state = datePicker.GetProperties();
SaveToDatabase(state);

// Restore
var savedState = LoadFromDatabase();
datePicker.SetProperties(savedState);
```

### 7. Use Segment Editing for Precision

```csharp
// ✅ Good: Enable for data entry forms
var invoiceDatePicker = new BeepDateDropDown
{
    ReturnType = ReturnDateTimeType.Date,
    EnableSegmentEditing = true,  // Users can tab through day/month/year
    DateFormat = "MM/dd/yyyy"
};
```

---

## Troubleshooting

### Issue: Calendar shows wrong dates after typing
**Solution**: Ensure `ValidateOnKeyPress` is true, or validate on `LostFocus`

### Issue: Range dates are swapped
**Solution**: The control automatically swaps if end < start. Check `DateRangeChanged` event.

### Issue: ReturnType and Mode out of sync
**Solution**: Set `ReturnType` and let `Mode` auto-sync, or use `DatePickerModeMapping.GetDefaultMode()`

### Issue: Segment editing not working
**Solution**: Check `EnableSegmentEditing = true` and ensure `DateFormat` is set correctly

### Issue: Calendar doesn't close after selection
**Solution**: Set `CloseOnSelection = true` in `DateTimePickerProperties`

---

## Version History

### Current Version
- ✅ ReturnDateTimeType as single source of truth
- ✅ 18+ DatePickerMode options
- ✅ Bidirectional sync between controls
- ✅ Segment-based editing
- ✅ Full range support (date and date-time)
- ✅ Mode mapping system
- ✅ Strongly-typed value properties
- ✅ Comprehensive validation

---

## See Also

- [BeepTextBox Documentation](../TextFields/README.md) - Base class for BeepDateDropDown
- [BeepTheme Documentation](../ThemeManagement/README.md) - Theming system
- [Styling Documentation](../Styling/README.md) - Custom styling options

---

## License

Copyright © The Tech Idea
