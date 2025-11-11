# Advanced Filtering System for BeepGridPro

## Overview

The Advanced Filtering System provides modern, Excel-like filtering capabilities for BeepGridPro using BeepControls and BeepiFormPro. It supports multi-column filtering, custom operators, filter persistence, quick filtering, and date range selection.

## Features

### ✨ Core Capabilities

1. **Multi-Column Filtering** - Filter on multiple columns simultaneously
2. **17 Filter Operators** - Comprehensive comparison operators for all data types
3. **AND/OR Logic** - Combine filters with logical operators
4. **Quick Filter Bar** - Excel-like instant filtering
5. **Date Range Picker** - Modern calendar-based date selection
6. **Filter Persistence** - Save and load filter configurations as JSON
7. **Modern UI** - Built entirely with BeepControls and BeepiFormPro

### 🎯 Filter Operators

#### Text Operators
- **Equals** (`=`) - Exact match
- **Not Equals** (`≠`) - Does not match
- **Contains** (`⊃`) - Contains substring
- **Not Contains** (`⊅`) - Does not contain substring
- **Starts With** (`^`) - Starts with substring
- **Ends With** (`$`) - Ends with substring
- **Regex** (`.*`) - Matches regular expression pattern
- **In** (`∈`) - In comma-separated list
- **Not In** (`∉`) - Not in comma-separated list

#### Numeric/Date Operators
- **Greater Than** (`>`)
- **Greater Than or Equal** (`≥`)
- **Less Than** (`<`)
- **Less Than or Equal** (`≤`)
- **Between** (`↔`) - Between two values (inclusive)
- **Not Between** (`↮`) - Not between two values

#### Common Operators
- **Is Empty** (`∅`) - Null or empty value
- **Is Not Empty** (`∄`) - Has a value

## Architecture

### Components

```
Filtering/
├── FilterOperator.cs              - Enum and extensions for all operators
├── FilterCriteria.cs              - Filter criterion and configuration models
├── BeepFilterRow.cs               - UI row for building filter expressions
├── BeepDateRangePicker.cs         - Modern date range picker control
├── BeepAdvancedFilterDialog.cs    - Multi-column filter dialog (BeepiFormPro)
├── BeepQuickFilterBar.cs          - Excel-like quick filter bar
├── GridFilterEngine.cs            - Filter application engine
└── README.md                      - This file
```

### Integration

- **BeepGridPro.Filtering.cs** - Partial class with filter methods and properties

## Usage

### Quick Filter

```csharp
// Enable quick filter bar
grid.ShowQuickFilterBar = true;

// Apply quick filter programmatically
grid.ApplyQuickFilter("search text", "ColumnName");

// Apply to all columns
grid.ApplyQuickFilter("search text");

// Clear filter
grid.ClearFilter();
```

### Advanced Filter Dialog

```csharp
// Show advanced filter dialog
grid.ShowAdvancedFilterDialog();

// Handle filter applied event
grid.FilterApplied += (s, e) =>
{
    Console.WriteLine($"Filter applied: {e.MatchingRowCount} rows match");
};

// Handle filter cleared event
grid.FilterCleared += (s, e) =>
{
    Console.WriteLine("Filter cleared");
};
```

### Programmatic Filtering

```csharp
// Create filter configuration
var config = new FilterConfiguration("My Filter")
{
    Logic = FilterLogic.And
};

// Add criteria
config.AddCriteria(new FilterCriteria(
    "CustomerName",
    FilterOperator.Contains,
    "Acme"
));

config.AddCriteria(new FilterCriteria(
    "OrderDate",
    FilterOperator.Between,
    new DateTime(2025, 1, 1),
    new DateTime(2025, 12, 31)
));

config.AddCriteria(new FilterCriteria(
    "Status",
    FilterOperator.In,
    "Active,Pending,Approved"
));

// Apply filter
grid.ActiveFilter = config;

// Check if filtered
if (grid.IsFiltered)
{
    Console.WriteLine($"Showing {grid.FilteredRowCount} of {grid.Data.Rows.Count} rows");
}
```

### Filter Persistence

```csharp
// Save filter configuration
grid.SaveFilterConfiguration("my_filter.json");

// Load filter configuration
grid.LoadFilterConfiguration("my_filter.json");

// Get all saved configurations from directory
var configs = grid.GetSavedFilterConfigurations("filters/");
foreach (var config in configs)
{
    Console.WriteLine($"Found filter: {config.Name}");
}
```

### Date Range Filtering

```csharp
// Create date range picker
var dateRangePicker = new BeepDateRangePicker();

// Set preset range
dateRangePicker.SetPresetRange(DateRangePreset.Last30Days);

// Get selected dates
var startDate = dateRangePicker.StartDate;
var endDate = dateRangePicker.EndDate;

// Apply to filter
if (dateRangePicker.IsValidRange)
{
    grid.AddFilterCriterion(new FilterCriteria(
        "OrderDate",
        FilterOperator.Between,
        startDate,
        endDate
    ));
}
```

## Filter Configuration Model

```csharp
public class FilterConfiguration
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<FilterCriteria> Criteria { get; set; }
    public FilterLogic Logic { get; set; }  // AND or OR
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

public class FilterCriteria
{
    public string ColumnName { get; set; }
    public FilterOperator Operator { get; set; }
    public object Value { get; set; }
    public object Value2 { get; set; }  // For Between operator
    public bool CaseSensitive { get; set; }
    public bool IsEnabled { get; set; }
}
```

## Examples

### Example 1: Customer Search

```csharp
// Quick search across all columns
grid.ApplyQuickFilter("microsoft");

// Search specific column
grid.ApplyQuickFilter("microsoft", "CompanyName");
```

### Example 2: Date Range Filter

```csharp
var config = new FilterConfiguration("Recent Orders")
{
    Logic = FilterLogic.And
};

config.AddCriteria(new FilterCriteria(
    "OrderDate",
    FilterOperator.GreaterThanOrEqual,
    DateTime.Today.AddDays(-30)
));

config.AddCriteria(new FilterCriteria(
    "Status",
    FilterOperator.NotEquals,
    "Cancelled"
));

grid.ActiveFilter = config;
```

### Example 3: Complex Multi-Column Filter

```csharp
var config = new FilterConfiguration("High Value Active Customers")
{
    Logic = FilterLogic.And
};

// Customer must be active
config.AddCriteria(new FilterCriteria(
    "Status",
    FilterOperator.Equals,
    "Active"
));

// Total orders > 10000
config.AddCriteria(new FilterCriteria(
    "TotalOrders",
    FilterOperator.GreaterThan,
    10000
));

// Company name starts with A-M
config.AddCriteria(new FilterCriteria(
    "CompanyName",
    FilterOperator.Regex,
    "^[A-M]"
));

// Region in specific list
config.AddCriteria(new FilterCriteria(
    "Region",
    FilterOperator.In,
    "North,South,East"
));

grid.ActiveFilter = config;
```

### Example 4: Save/Load Filters

```csharp
// Save current filter
if (grid.IsFiltered)
{
    var saveDialog = new SaveFileDialog
    {
        Filter = "Filter Config|*.json",
        FileName = "my_filter.json"
    };
    
    if (saveDialog.ShowDialog() == DialogResult.OK)
    {
        grid.SaveFilterConfiguration(saveDialog.FileName);
    }
}

// Load saved filter
var openDialog = new OpenFileDialog
{
    Filter = "Filter Config|*.json"
};

if (openDialog.ShowDialog() == DialogResult.OK)
{
    grid.LoadFilterConfiguration(openDialog.FileName);
}
```

## UI Components

### BeepFilterRow

Modern filter row control for building filter expressions:
- Column selector dropdown
- Operator selector with symbols and names
- Value input fields (1 or 2 based on operator)
- Remove button
- Auto-updates operators based on column data type

### BeepDateRangePicker

Date range selection control:
- Start and end date text inputs
- Calendar popup buttons
- "to" label between dates
- Preset ranges (Today, Last 7 Days, This Month, etc.)
- Validation for valid date ranges

### BeepAdvancedFilterDialog

Full-featured filter dialog using BeepiFormPro:
- Multi-row filter builder
- Add/remove filter rows dynamically
- AND/OR logic selector
- Save/load configurations
- Clear all filters
- Apply/Cancel buttons
- Modern styling with BeepiFormPro

### BeepQuickFilterBar

Excel-like quick filter bar:
- Global search across columns
- Column selector dropdown
- Real-time filtering as you type
- Filter count badge
- Clear filter button
- Advanced filter button
- Modern BeepControl styling

## Events

```csharp
// Filter applied
grid.FilterApplied += (s, e) =>
{
    Console.WriteLine($"Filter: {e.FilterConfiguration.Name}");
    Console.WriteLine($"Matches: {e.MatchingRowCount} rows");
};

// Filter cleared
grid.FilterCleared += (s, e) =>
{
    Console.WriteLine("All filters cleared");
};
```

## Properties

```csharp
// Check if filtered
bool isFiltered = grid.IsFiltered;

// Get filtered row count
int visibleRows = grid.FilteredRowCount;
int totalRows = grid.Data.Rows.Count;

// Get active filter
FilterConfiguration activeFilter = grid.ActiveFilter;

// Enable quick filter bar
grid.ShowQuickFilterBar = true;
```

## Best Practices

1. **Use Quick Filter for Simple Searches** - For single-column or all-column text searches
2. **Use Advanced Filter for Complex Queries** - Multiple columns, different operators, AND/OR logic
3. **Save Frequently Used Filters** - Use filter persistence for recurring filter needs
4. **Combine with Sorting** - Filters work alongside column sorting
5. **Performance** - Filters are applied efficiently using indexed lookups
6. **Case Sensitivity** - Text operators support case-sensitive option when needed
7. **Data Types** - Operators are automatically filtered based on column data type

## Performance Considerations

- Filters are applied in-memory for fast results
- Filter engine uses optimized comparisons
- Filtered rows are hidden, not removed from data source
- Layout recalculation is performed after filter application
- Regex filters may be slower on large datasets

## Future Enhancements

- [ ] Filter templates library
- [ ] Recent filters history
- [ ] Filter preview before apply
- [ ] Export filtered data
- [ ] Custom filter functions
- [ ] Filter statistics/summary
- [ ] Conditional formatting based on filters
- [ ] Filter groups for complex scenarios

## Integration with Other Features

- ✅ Works with column sorting
- ✅ Works with row selection
- ✅ Works with virtual scrolling
- ✅ Works with all grid styles
- ✅ Works with themes
- ✅ Works with data binding
- ✅ Works with custom cell renderers

## Troubleshooting

**Q: Filters not working?**
A: Ensure columns have proper DataType set and data source is bound

**Q: Date filters failing?**
A: Check date format in data source matches expected format

**Q: Regex filter errors?**
A: Validate regex pattern syntax before applying

**Q: Filter persistence not saving?**
A: Ensure write permissions on target directory

## See Also

- BeepGridPro Documentation
- BeepControls Documentation  
- BeepiFormPro Documentation
- Filter Configuration JSON Schema
