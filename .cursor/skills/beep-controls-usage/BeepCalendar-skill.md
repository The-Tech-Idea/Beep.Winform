# BeepCalendar Skill

## Overview
`BeepCalendar` provides a Material Design calendar control with date selection and range support.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Calendar;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `SelectedDate` | `DateTime` | Selected date |
| `MinDate` | `DateTime` | Minimum selectable date |
| `MaxDate` | `DateTime` | Maximum selectable date |
| `UseThemeColors` | `bool` | Use theme colors |

## Usage Examples

### Basic Calendar
```csharp
var calendar = new BeepCalendar
{
    SelectedDate = DateTime.Today,
    UseThemeColors = true
};
```

### Handle Date Selection
```csharp
calendar.DateSelected += (s, e) =>
{
    DateTime selected = e.SelectedDate;
    // Process selection
};
```

### Date Range
```csharp
var calendar = new BeepCalendar
{
    MinDate = DateTime.Today,
    MaxDate = DateTime.Today.AddMonths(6)
};
```

## Events
| Event | Description |
|-------|-------------|
| `DateSelected` | Date selected |
| `MonthChanged` | Month navigation |
| `YearChanged` | Year navigation |

## Related Controls
- `BeepDatePicker` - Date input field with popup
- `BeepTimePicker` - Time selection
