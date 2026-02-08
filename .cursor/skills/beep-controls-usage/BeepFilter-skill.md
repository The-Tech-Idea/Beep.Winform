# BeepFilter Skill

## Overview
`BeepFilter` is a comprehensive filtering control with 8 interaction styles, 17 operators, and multiple display modes.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Filtering;
```

## FilterStyle (8 types)
```csharp
public enum FilterStyle
{
    TagPills,           // Tag/chip pills with X buttons
    GroupedRows,        // AND/OR logic groups
    QueryBuilder,       // If/Else visual tree
    DropdownMultiSelect,// Dropdown with checkboxes
    InlineRow,          // Column|Operator|Value rows
    SidebarPanel,       // E-commerce style sidebar
    QuickSearch,        // Single search with instant filter
    AdvancedDialog      // Modal with tabbed sections
}
```

## FilterOperator (17 operators)
| Operator | Description |
|----------|-------------|
| `Equals` | Exact match |
| `NotEquals` | Not equal |
| `Contains` | Substring match |
| `NotContains` | Not contains |
| `StartsWith` | Starts with |
| `EndsWith` | Ends with |
| `GreaterThan` | > comparison |
| `GreaterThanOrEqual` | >= comparison |
| `LessThan` | < comparison |
| `LessThanOrEqual` | <= comparison |
| `Between` | Range (inclusive) |
| `NotBetween` | Not in range |
| `IsNull` | Is null/empty |
| `IsNotNull` | Is not null |
| `Regex` | Pattern match |
| `In` | In list |
| `NotIn` | Not in list |

## FilterLogic
```csharp
public enum FilterLogic
{
    And,  // All must match
    Or    // Any must match
}
```

## FilterDisplayMode
```csharp
public enum FilterDisplayMode
{
    AlwaysVisible,  // Always shown
    Collapsible,    // Expandable
    OnHover,        // Show on hover
    Modal,          // Popup dialog
    SlideIn         // Slide-in panel
}
```

## FilterPosition
```csharp
public enum FilterPosition
{
    Top,      // Above grid
    Bottom,   // Below grid
    Left,     // Left panel
    Right,    // Right panel
    Floating, // Overlay
    Inline    // Embedded
}
```

## Usage Examples

### Tag Pills Filter
```csharp
var filter = new BeepFilter
{
    FilterStyle = FilterStyle.TagPills
};

filter.AddCriteria(new FilterCriteria
{
    ColumnName = "Status",
    Operator = FilterOperator.Equals,
    Value = "Active"
});

filter.FilterChanged += (s, e) => ApplyFilters();
```

### Quick Search
```csharp
var filter = new BeepFilter
{
    FilterStyle = FilterStyle.QuickSearch,
    DisplayMode = FilterDisplayMode.AlwaysVisible
};
```

### Grouped Filters
```csharp
var filter = new BeepFilter
{
    FilterStyle = FilterStyle.GroupedRows,
    FilterLogic = FilterLogic.And
};
```

## Type-Specific Operators
```csharp
// Get operators for data type
var ops = FilterOperatorExtensions.GetOperatorsForType(typeof(string));
// Returns: Equals, NotEquals, Contains, StartsWith, EndsWith, etc.

var numOps = FilterOperatorExtensions.GetOperatorsForType(typeof(int));
// Returns: Equals, NotEquals, GreaterThan, LessThan, Between, etc.
```

## Painters
- TagPillsFilterPainter
- GroupedRowsFilterPainter
- QueryBuilderFilterPainter
- DropdownMultiSelectFilterPainter
- InlineRowFilterPainter
- SidebarPanelFilterPainter
- QuickSearchFilterPainter
- AdvancedDialogFilterPainter

## Related Controls
- `BeepSimpleGrid` - Grid with filtering
- `BeepQueryandFilter` - Query builder
