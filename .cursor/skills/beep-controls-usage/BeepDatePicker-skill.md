# BeepDateTimePicker Skill

## Overview
`BeepDateTimePicker` is a comprehensive date/time selector with 8 modes, range selection, time picker, quick buttons, week numbers, and constraints.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Dates;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
```

## DatePickerMode (8 modes)
```csharp
public enum DatePickerMode
{
    Single,         // Select single date
    Range,          // Select date range (start-end)
    Multiple,       // Select multiple dates
    DateTime,       // Date with time picker
    TimeOnly,       // Time picker only
    Appointment,    // Combined date + time slots
    FlexibleRange,  // Flexible start/end range
    FilteredRange   // Range with filtered dates
}
```

## Key Properties

### Date/Time Properties
| Property | Type | Description |
|----------|------|-------------|
| `SelectedDate` | `DateTime?` | Selected date |
| `SelectedTime` | `TimeSpan?` | Selected time |
| `SelectedDateTime` | `DateTime?` | Combined date + time |
| `SelectedDates` | `List<DateTime>` | Multiple selection list |
| `RangeStartDate` | `DateTime?` | Range start date |
| `RangeEndDate` | `DateTime?` | Range end date |
| `RangeStartTime` | `TimeSpan?` | Range start time |
| `RangeEndTime` | `TimeSpan?` | Range end time |

### Constraint Properties
| Property | Type | Description |
|----------|------|-------------|
| `MinDate` | `DateTime` | Minimum selectable date |
| `MaxDate` | `DateTime` | Maximum selectable date |
| `MinTime` | `TimeSpan` | Minimum selectable time |
| `MaxTime` | `TimeSpan` | Maximum selectable time |

### Mode & Format
| Property | Type | Description |
|----------|------|-------------|
| `Mode` | `DatePickerMode` | Picker mode |
| `Format` | `DatePickerFormat` | Date format |
| `FirstDayOfWeek` | `DatePickerFirstDayOfWeek` | Week start day |

### Feature Toggles
| Property | Type | Description |
|----------|------|-------------|
| `ShowTime` | `bool` | Show time picker |
| `ShowQuickButtons` | `bool` | Today, Tomorrow, etc. |
| `ShowWeekNumbers` | `bool` | Week numbers |
| `AllowClear` | `bool` | Allow clearing value |

### Time Properties
| Property | Type | Description |
|----------|------|-------------|
| `TimeIntervalMinutes` | `int` | Time interval (30) |
| `TimeStartHour` | `int` | Start hour (0-23) |

### Filter Properties
| Property | Type | Description |
|----------|------|-------------|
| `FilteredDates` | `List<DateTime>` | Disabled dates |
| `Holidays` | `List<DateTime>` | Holiday dates |
| `MaxMultipleSelectionCount` | `int?` | Max selections |

## Usage Examples

### Single Date
```csharp
var picker = new BeepDateTimePicker
{
    Mode = DatePickerMode.Single,
    MinDate = DateTime.Today,
    MaxDate = DateTime.Today.AddYears(1)
};
picker.DateChanged += (s, e) => Console.WriteLine($"Selected: {picker.SelectedDate}");
```

### Date Range
```csharp
var picker = new BeepDateTimePicker
{
    Mode = DatePickerMode.Range,
    ShowQuickButtons = true
};
picker.RangeChanged += (s, e) =>
{
    Console.WriteLine($"From: {picker.RangeStartDate} To: {picker.RangeEndDate}");
};
```

### Multiple Selection
```csharp
var picker = new BeepDateTimePicker
{
    Mode = DatePickerMode.Multiple,
    MaxMultipleSelectionCount = 5,
    ShowWeekNumbers = true
};
```

### Date + Time
```csharp
var picker = new BeepDateTimePicker
{
    Mode = DatePickerMode.DateTime,
    ShowTime = true,
    TimeIntervalMinutes = 15
};
```

### Appointment Mode
```csharp
var picker = new BeepDateTimePicker
{
    Mode = DatePickerMode.Appointment,
    TimeStartHour = 9,
    MinTime = TimeSpan.FromHours(9),
    MaxTime = TimeSpan.FromHours(17)
};
```

### With Filtered Dates
```csharp
var picker = new BeepDateTimePicker
{
    Mode = DatePickerMode.FilteredRange,
    Holidays = new List<DateTime>
    {
        new DateTime(2024, 12, 25),
        new DateTime(2024, 1, 1)
    }
};
```

## Events
| Event | Description |
|-------|-------------|
| `DateChanged` | Date selection changed |
| `TimeChanged` | Time selection changed |
| `RangeChanged` | Range selection changed |

## Related Controls
- `BeepDateDropDown` - Dropdown date selector
- `BeepTimePicker` - Standalone time picker
