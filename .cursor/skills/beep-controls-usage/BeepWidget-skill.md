# BeepWidget System Skill

## Overview
The Beep Widget system provides 10+ specialized dashboard widget controls, each with multiple visual styles and dedicated painters.

## API Rule
- Use strongly typed widget models only.
- Do not use `Dictionary<string, object>` or `object` as control input/output contracts.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Widgets;
```

---

## BeepMetricWidget (6 Styles)

**Purpose**: Display KPIs, metrics, and numeric indicators.

### MetricWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `SimpleValue` | Number and label only |
| `ValueWithTrend` | Number + trend indicator |
| `ProgressMetric` | Number with progress bar |
| `GaugeMetric` | Circular gauge display |
| `ComparisonMetric` | Two values side-by-side |
| `CardMetric` | Card-style with icon |

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `MetricWidgetStyle` | Visual style |
| `Title` | `string` | Metric label |
| `Value` | `string` | Main metric value |
| `Units` | `string` | Units (%, $, users) |
| `TrendValue` | `string` | Trend text (+12.5%) |
| `TrendDirection` | `string` | up/down/neutral |
| `TrendPercentage` | `double` | Trend value |
| `ShowTrend` | `bool` | Display trend |
| `ShowIcon` | `bool` | Display icon |
| `IconPath` | `string` | Icon image path |
| `AccentColor` | `Color` | Accent color |
| `TrendColor` | `Color` | Trend indicator color |

### Events
- `ValueClicked`, `TrendClicked`, `IconClicked`

### Example
```csharp
var metric = new BeepMetricWidget
{
    Style = MetricWidgetStyle.ValueWithTrend,
    Title = "Total Sales",
    Value = "$125,400",
    TrendValue = "+15%",
    TrendDirection = "up",
    ShowTrend = true
};
```

---

## BeepChartWidget (7 Styles)

**Purpose**: Chart and data visualization.

### ChartWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `BarChart` | Vertical/horizontal bars |
| `LineChart` | Line/area charts |
| `PieChart` | Pie/donut charts |
| `GaugeChart` | Gauge/speedometer |
| `Sparkline` | Mini trend line |
| `HeatmapChart` | Calendar/grid heatmap |
| `CombinationChart` | Multiple chart types |

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `ChartWidgetStyle` | Chart type |
| `Title` | `string` | Chart title |
| `Values` | `List<double>` | Data values |
| `Labels` | `List<string>` | Data labels |
| `Colors` | `List<Color>` | Series colors |
| `ShowLegend` | `bool` | Show legend |
| `ShowGrid` | `bool` | Show grid lines |
| `MinValue` | `double` | Scale minimum |
| `MaxValue` | `double` | Scale maximum |

### Events
- `ChartClicked`, `DataPointClicked`, `LegendClicked`

### Example
```csharp
var chart = new BeepChartWidget
{
    Style = ChartWidgetStyle.BarChart,
    Title = "Monthly Revenue",
    Values = new List<double> { 10, 25, 30, 45, 20, 35 },
    Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
    ShowLegend = true
};
```

---

## BeepCalendarWidget (10 Styles)

**Purpose**: Calendar, scheduling, and time management.

### CalendarWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `DateGrid` | Calendar grid layout |
| `TimeSlots` | Available time slot picker |
| `EventCard` | Event display card |
| `CalendarView` | Full calendar month view |
| `ScheduleCard` | Schedule/appointment display |
| `DatePicker` | Date selection interface |
| `TimelineView` | Timeline-based calendar |
| `WeekView` | Weekly calendar display |
| `EventList` | List of upcoming events |
| `AvailabilityGrid` | Availability/booking grid |

### CalendarViewMode Enum
`Month`, `Week`, `Day`, `Year`, `Agenda`

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `CalendarWidgetStyle` | Visual style |
| `SelectedDate` | `DateTime` | Current selected date |
| `DisplayMonth` | `DateTime` | Displayed month/year |
| `Events` | `List<CalendarEvent>` | Calendar events |
| `TimeSlots` | `List<TimeSlot>` | Available time slots |
| `SelectedDates` | `List<DateTime>` | Multi-select dates |
| `ViewMode` | `CalendarViewMode` | View mode |
| `ShowWeekends` | `bool` | Show weekends |
| `ShowToday` | `bool` | Highlight today |
| `ShowEvents` | `bool` | Show events |
| `AllowMultiSelect` | `bool` | Allow multi-select |
| `WorkingHoursStart` | `int` | Work hours start (24h) |
| `WorkingHoursEnd` | `int` | Work hours end (24h) |
| `FirstDayOfWeek` | `DayOfWeek` | First day of week |

### Data Models
```csharp
public class CalendarEvent
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Color Color { get; set; }
    public string Type { get; set; }
    public string Location { get; set; }
    public bool IsAllDay { get; set; }
    public bool IsRecurring { get; set; }
    public List<string> Attendees { get; set; }
}

public class TimeSlot
{
    public string Label { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsBooked { get; set; }
    public bool IsWorkingHours { get; set; }
}
```

### Events
- `DateSelected`, `EventClicked`, `TimeSlotClicked`, `MonthChanged`, `ViewModeChanged`

---

## BeepFormWidget (10 Styles)

**Purpose**: Data entry, validation, and form management.

### FormWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `FieldGroup` | Related inputs grouped |
| `ValidationPanel` | Form section with validation |
| `FormSection` | Organized form with title |
| `InputCard` | Styled input container |
| `FormStep` | Multi-step progression |
| `FieldSet` | Traditional fieldset styling |
| `InlineForm` | Horizontal form layout |
| `CompactForm` | Space-efficient design |
| `ValidatedInput` | Single input with validation |
| `FormSummary` | Form data summary |

### FormFieldType Enum
`Text`, `Email`, `Password`, `Number`, `Phone`, `Date`, `Dropdown`, `Checkbox`, `Radio`, `TextArea`, `File`, `Color`, `Range`, `Hidden`

### FormLayout Enum
`Vertical`, `Horizontal`, `Grid`, `Inline`, `Stacked`

### FormValidationMode Enum
`OnSubmit`, `OnBlur`, `OnChange`, `Realtime`

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `FormWidgetStyle` | Visual style |
| `Fields` | `List<FormField>` | Form fields |
| `ValidationResults` | `List<ValidationResult>` | Validation results |
| `ShowValidation` | `bool` | Show validation indicators |
| `ShowRequired` | `bool` | Show required indicators |
| `IsReadOnly` | `bool` | Read-only mode |
| `ShowProgress` | `bool` | Show progress (multi-step) |
| `CurrentStep` | `int` | Current step (1-based) |
| `TotalSteps` | `int` | Total steps |
| `Layout` | `FormLayout` | Field layout |
| `ValidationMode` | `FormValidationMode` | When to validate |

### Methods
- `ValidateForm()` - Validate all fields
- `ResetForm()` - Reset to defaults
- `GetFormData()` - Returns `List<FormFieldValueEntry>`
- `SetFormData(IEnumerable<FormFieldValueEntry>)` - Set typed field values

### Data Models
```csharp
public class FormField
{
    public string Name { get; set; }
    public string Label { get; set; }
    public FormFieldType Type { get; set; }
    public FormFieldValue Value { get; set; }
    public FormFieldValue DefaultValue { get; set; }
    public FormFieldAttributes Attributes { get; set; }
    public string Placeholder { get; set; }
    public string HelpText { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsVisible { get; set; }
    public List<string> Options { get; set; } // Dropdown/radio
    public int MaxLength { get; set; }
    public string ValidationPattern { get; set; }
}

public class ValidationResult
{
    public string FieldName { get; set; }
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public ValidationSeverity Severity { get; set; }
}

public class FormFieldValueEntry
{
    public string FieldName { get; set; }
    public FormFieldValue Value { get; set; }
}
```

### Events
- `FieldChanged`, `ValidationChanged`, `StepChanged`, `FormSubmitted`, `FormReset`

---

## BeepDashboardWidget (6 Styles)

**Purpose**: Multiple data visualizations combined.

### DashboardWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `MultiMetric` | Multiple KPIs in one widget |
| `ChartGrid` | Multiple small charts |
| `TimelineView` | Chronological display |
| `ComparisonGrid` | Side-by-side comparisons |
| `StatusOverview` | System status dashboard |
| `AnalyticsPanel` | Complex analytics layout |

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `DashboardWidgetStyle` | Visual style |
| `Metrics` | `List<DashboardMetric>` | Dashboard metrics |
| `ShowTitle` | `bool` | Show title |
| `Columns` | `int` | Grid columns |
| `Rows` | `int` | Grid rows |

### Data Model
```csharp
public class DashboardMetric
{
    public string Title { get; set; }
    public string Value { get; set; }
    public string Trend { get; set; }
    public Color Color { get; set; }
}
```

---

## BeepListWidget (6 Styles)

**Purpose**: Data lists, feeds, and tables.

### ListWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `ActivityFeed` | Timeline-style activities |
| `DataTable` | Structured data table |
| `RankingList` | Ordered ranking list |
| `StatusList` | Items with status indicators |
| `ProfileList` | User/profile listings |
| `TaskList` | Checklist/todo style |

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `ListWidgetStyle` | Visual style |
| `Items` | `List<ListItem>` | Data items |
| `Columns` | `List<string>` | Column names |
| `ShowHeader` | `bool` | Show header |
| `AllowSelection` | `bool` | Enable selection |
| `SelectedIndex` | `int` | Selected item index |
| `MaxVisibleItems` | `int` | Max visible items |

### Events
- `ItemClicked`, `ItemSelected`, `HeaderClicked`

---

## BeepControlWidget (10 Styles)

**Purpose**: Interactive input controls using typed value models.

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `ControlWidgetStyle` | Visual style |
| `ToggleValue` | `ToggleValue` | Toggle model |
| `SliderValue` | `SliderValue` | Slider model |
| `RangeValue` | `RangeValue` | Range selector model |
| `DateValue` | `DateValue` | Date model |
| `ColorValue` | `ColorValue` | Color model |
| `NumberValue` | `NumberValue` | Number model |
| `SelectedOption` | `string` | Selected option |
| `SearchText` | `string` | Search text |
| `Options` | `List<string>` | Option labels |
| `CheckboxOptions` | `List<CheckboxOption>` | Typed checkbox options |

### Events
- `ValueChanged`, `ControlClicked`, `OptionSelected`

---

## BeepNavigationWidget (10 Styles)

**Purpose**: Navigation elements and step indicators.

### NavigationWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `Breadcrumb` | Breadcrumb navigation |
| `StepIndicator` | Multi-step process indicator |
| `TabContainer` | Tab navigation |
| `Pagination` | Page navigation |
| `MenuBar` | Horizontal menu bar |
| `SidebarNav` | Sidebar navigation |
| `WizardSteps` | Wizard step navigation |
| `ProcessFlow` | Process flow indicator |
| `TreeNavigation` | Tree-style navigation |
| `QuickActions` | Quick action buttons |

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `NavigationWidgetStyle` | Visual style |
| `Items` | `List<NavigationItem>` | Navigation items |
| `CurrentIndex` | `int` | Active item index |
| `ShowIcons` | `bool` | Show icons |
| `IsHorizontal` | `bool` | Horizontal layout |

### Data Model
```csharp
public class NavigationItem
{
    public string Text { get; set; }
    public string IconPath { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsVisible { get; set; } = true;
}
```

---

## BeepFinanceWidget (10 Styles)

**Purpose**: Financial data, portfolios, and transactions.

### FinanceWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `PortfolioCard` | Investment portfolio display |
| `CryptoWidget` | Cryptocurrency stats |
| `TransactionCard` | Financial transaction display |
| `BalanceCard` | Account balance showcase |
| `FinancialChart` | Financial charts |
| `PaymentCard` | Payment method display |
| `InvestmentCard` | Investment tracking card |
| `ExpenseCard` | Expense category display |
| `RevenueCard` | Revenue tracking display |
| `BudgetWidget` | Budget progress tracking |

### FinanceTrend Enum
`Up`, `Down`, `Neutral`

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `FinanceWidgetStyle` | Visual style |
| `PrimaryValue` | `decimal` | Primary value |
| `SecondaryValue` | `decimal` | Secondary value |
| `Percentage` | `decimal` | Change percentage |
| `Currency` | `string` | Currency code (USD, EUR, BTC) |
| `CurrencySymbol` | `string` | Currency symbol ($, €, ₿) |
| `Trend` | `FinanceTrend` | Trend direction |
| `FinanceItems` | `List<FinanceItem>` | Finance items |
| `ShowCurrency` | `bool` | Show currency |
| `ShowPercentage` | `bool` | Show percentage |
| `ShowTrend` | `bool` | Show trend |
| `AccountNumber` | `string` | Account number |
| `CardType` | `string` | Card type |

### Data Model
```csharp
public class FinanceItem
{
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Value { get; set; }
    public decimal Percentage { get; set; }
    public FinanceTrend Trend { get; set; }
    public string Currency { get; set; }
    public DateTime Date { get; set; }
}
```

---

## BeepSocialWidget (10 Styles)

**Purpose**: User profiles, teams, messaging, social interactions.

### SocialWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `ProfileCard` | User profile display card |
| `TeamMembers` | Team member avatar grid |
| `MessageCard` | Communication/message display |
| `ActivityStream` | Social activity feed |
| `UserList` | Contact/user listing |
| `ChatWidget` | Chat interface component |
| `CommentThread` | Comment/reply thread |
| `SocialFeed` | Social media style feed |
| `UserStats` | User statistics display |
| `ContactCard` | Contact information card |

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `SocialWidgetStyle` | Visual style |
| `UserName` | `string` | User name |
| `UserRole` | `string` | User role/title |
| `UserStatus` | `string` | Online/Away/Busy/Offline |
| `UserAvatar` | `string` | Avatar image path |
| `AvatarImage` | `Image` | Avatar image |
| `SocialItems` | `List<SocialItem>` | Social items |
| `ShowStatus` | `bool` | Show status indicator |
| `ShowAvatar` | `bool` | Show avatar |
| `OnlineCount` | `int` | Online user count |
| `TotalCount` | `int` | Total user count |

### Data Model
```csharp
public class SocialItem
{
    public string Name { get; set; }
    public string Role { get; set; }
    public string Status { get; set; } = "Offline";
    public string Avatar { get; set; }
    public Image AvatarImage { get; set; }
    public string LastSeen { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsOnline { get; set; }
}
```

---

## BeepMediaWidget (10 Styles)

**Purpose**: Images, avatars, icons, and galleries.

### MediaWidgetStyle Enum
| Value | Description |
|-------|-------------|
| `ImageCard` | Card with background image |
| `AvatarGroup` | Clustered profile pictures |
| `IconCard` | Large icon with label |
| `MediaGallery` | Image carousel/gallery |
| `ProfileCard` | User profile with photo |
| `ImageOverlay` | Image with text overlay |
| `PhotoGrid` | Grid of photos/thumbnails |
| `MediaViewer` | Single media item display |
| `AvatarList` | List of avatars with details |
| `IconGrid` | Grid of icons with labels |

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Style` | `MediaWidgetStyle` | Visual style |
| `ImagePath` | `string` | Main image path |
| `Image` | `Image` | Main image |
| `ShowOverlay` | `bool` | Show overlay |
| `OverlayText` | `string` | Overlay text |
| `MediaItems` | `List<MediaItem>` | Media items |

### Data Model
```csharp
public class MediaItem
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string ImagePath { get; set; }
    public Image Image { get; set; }
    public bool IsAvatar { get; set; }
    public Color AccentColor { get; set; }
}
```

---

## Common Widget Features

All widgets inherit from `BaseControl` and share:
- **Theme Integration**: Use `ApplyTheme()` for theme colors
- **Hit Areas**: Interactive regions with click handlers
- **Painter System**: Dedicated painters per style
- **BeepEventDataArgs**: Event data container

## Related Controls
- `BeepCard` - Card containers (40+ styles)
- `BeepChart` - Standalone charts
- `BeepCalendar` - Full calendar control
