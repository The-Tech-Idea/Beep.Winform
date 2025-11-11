# BeepFilter Architecture

## Overview
BeepFilter is a **generic, reusable filter UI component** that generates `FilterConfiguration` objects. It does NOT apply filters directly - each consuming control (like BeepGridPro) handles filter application using its own logic.

## Architecture Pattern

```
┌─────────────────┐
│   BeepFilter    │  ← Generic UI Component
│  (UI Builder)   │     - Takes EntityStructure
│                 │     - Displays filter UI (TagPills, GroupedRows, etc.)
│                 │     - Generates FilterConfiguration
└────────┬────────┘     - Raises FilterApplied event
         │
         │ FilterConfiguration
         │ (via event)
         ▼
┌─────────────────┐
│  BeepGridPro    │  ← Consuming Control
│ (Filter Applier)│     - Subscribes to BeepFilter.FilterApplied
│                 │     - Uses FilterEngine<ExpandoObject> to apply filter
└─────────────────┘     - Updates visible rows

┌─────────────────┐
│   BeepListBox   │  ← Another Consumer
│ (Filter Applier)│     - Subscribes to BeepFilter.FilterApplied
│                 │     - Uses FilterEngine<T> to apply filter
└─────────────────┘     - Updates visible items
```

## Responsibilities

### BeepFilter (UI Component)
**DOES:**
- ✅ Display filter UI (painters handle different styles)
- ✅ Bind to EntityStructure to get available columns
- ✅ Allow user to build filter criteria
- ✅ Generate `FilterConfiguration` object
- ✅ Raise `FilterApplied` event with configuration
- ✅ Support save/load of filter configurations

**DOES NOT:**
- ❌ Apply filters to data
- ❌ Know about grid rows or cells
- ❌ Handle data source directly
- ❌ Execute filter logic

### BeepGridPro (Consumer)
**DOES:**
- ✅ Subscribe to `BeepFilter.FilterApplied` event
- ✅ Use `FilterEngine<ExpandoObject>` to apply filters
- ✅ Update visible rows based on filter results
- ✅ Provide EntityStructure to BeepFilter

**Integration Example:**
```csharp
// Setup
var beepFilter = new BeepFilter();
beepFilter.EntityStructure = grid.EntityStructure;
beepFilter.FilterStyle = FilterStyle.TagPills;

// Subscribe to filter events
beepFilter.FilterApplied += (s, e) =>
{
    // Grid applies the filter using generic FilterEngine
    grid.ActiveFilter = beepFilter.ActiveFilter;
};

// User interacts with beepFilter UI
// → FilterApplied event fires
// → Grid.ActiveFilter setter applies filter via FilterEngine<ExpandoObject>
```

## File Organization

```
Filtering/
├── FilterStyle.cs              # Enums: FilterStyle, FilterDisplayMode, FilterPosition
├── FilterPainterMetrics.cs     # Layout metrics (no colors - from BeepStyling)
├── FilterOperator.cs           # 17 filter operators enum + extensions
├── FilterCriteria.cs           # FilterCriteria + FilterConfiguration models
├── FilterEngine.cs             # Generic filter engine (FilterEngine<T>)
│
├── IFilterPainter.cs           # Painter interface + FilterHitArea, FilterLayoutInfo
├── BaseFilterPainter.cs        # Base painter with common helpers
│
├── BeepFilter.cs               # Main control (inherits BaseControl)
├── BeepFilter.Properties.cs    # Properties (FilterStyle, DataSource, EntityStructure)
├── BeepFilter.Events.cs        # Events (FilterApplied, FilterChanged, etc.)
├── BeepFilter.HitTest.cs       # Hit testing logic
├── BeepFilter.Layout.cs        # Layout calculations
│
└── Painters/                   # (To be created)
    ├── TagPillsFilterPainter.cs
    ├── GroupedRowsFilterPainter.cs
    ├── QueryBuilderFilterPainter.cs
    └── ...

GridX/Filtering/                # Grid-specific filter UI components
├── BeepFilterRow.cs            # Filter row control (uses BeepControls)
├── BeepDateRangePicker.cs      # Date range picker
├── BeepAdvancedFilterDialog.cs # Advanced filter dialog (uses BeepiFormPro)
├── BeepQuickFilterBar.cs       # Excel-like quick filter bar
└── README.md
```

## Usage Patterns

### Pattern 1: Standalone BeepFilter
```csharp
var filter = new BeepFilter();
filter.EntityStructure = myEntityStructure;
filter.FilterStyle = FilterStyle.TagPills;

filter.FilterApplied += (s, e) =>
{
    var config = filter.ActiveFilter;
    // Apply to your data source however you want
    ApplyFilterToMyData(config);
};
```

### Pattern 2: BeepGridPro Integration
```csharp
// Grid uses FilterEngine<ExpandoObject> internally
grid.ActiveFilter = filterConfiguration; // Setter applies via FilterEngine

// Or use existing grid methods
grid.ShowAdvancedFilterDialog();
grid.ApplyQuickFilter("search text");
```

### Pattern 3: BeepFilter with Grid
```csharp
var beepFilter = new BeepFilter();
beepFilter.EntityStructure = grid.EntityStructure;
beepFilter.FilterStyle = FilterStyle.GroupedRows;

// Wire up event
beepFilter.FilterApplied += (s, e) =>
{
    grid.ActiveFilter = beepFilter.ActiveFilter; // Grid applies it
};

// Place filter UI wherever you want
panel.Controls.Add(beepFilter);
```

## Key Classes

### FilterConfiguration
```csharp
public class FilterConfiguration
{
    public string Name { get; set; }
    public List<FilterCriteria> Criteria { get; set; }
    public FilterLogic Logic { get; set; } // And/Or
}
```

### FilterCriteria
```csharp
public class FilterCriteria
{
    public string ColumnName { get; set; }
    public FilterOperator Operator { get; set; }
    public object Value { get; set; }
    public object Value2 { get; set; } // For Between
    public bool CaseSensitive { get; set; }
    public bool IsEnabled { get; set; }
}
```

### FilterOperator (17 operators)
- Equals, NotEquals
- Contains, NotContains, StartsWith, EndsWith
- GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual
- Between, NotBetween
- IsNull, IsNotNull
- Regex, In, NotIn

## Filter Styles (Interaction Patterns)

1. **TagPills** - Horizontal tag pills with X to remove
2. **GroupedRows** - Rows with AND/OR logic and grouping
3. **QueryBuilder** - Visual tree with If/Else branches
4. **DropdownMultiSelect** - Field/Operator/Value dropdowns
5. **InlineRow** - Single line per condition
6. **SidebarPanel** - Vertical sidebar (e-commerce style)
7. **QuickSearch** - Search bar with column selector
8. **AdvancedDialog** - Modal dialog with tabs

Visual styling (colors, borders, shadows) comes from **BeepStyling** and **BeepControlStyle** - NOT from FilterStyle!

## Events

### BeepFilter Events
- `FilterAdded` - New filter criterion added
- `FilterRemoved` - Filter criterion removed
- `FilterModified` - Filter criterion changed
- `FiltersCleared` - All filters cleared
- `FilterChanged` - Active configuration changed
- `FilterEditRequested` - User wants to edit a filter
- **`FilterApplied`** - User clicked Apply (consuming control handles this!)
- `GroupAdded` - Filter group added
- `GroupRemoved` - Filter group removed
- `ConfigurationSaveRequested` - User wants to save config
- `ConfigurationLoadRequested` - User wants to load config

### Consuming Control Responsibilities
When `FilterApplied` fires:
1. Get `ActiveFilter` from BeepFilter
2. Apply using your own logic (GridFilterEngine for grids)
3. Update your visible data
4. Optionally raise your own events

## Next Steps

### TODO
1. ✅ Core architecture defined
2. ✅ BeepFilter main control created (5 partials)
3. ✅ Painter interfaces defined
4. ⏳ Create painter implementations (TagPillsFilterPainter, etc.)
5. ⏳ Create FilterPainterFactory
6. ⏳ Create filter helpers (Layout, HitTest, Render, State)
7. ⏳ Test integration with BeepGridPro

### Design Principles
- **Separation of Concerns**: UI generation (BeepFilter) vs Filter application (consuming control)
- **Reusability**: BeepFilter works with any data source via EntityStructure
- **Extensibility**: New painters for new interaction patterns
- **Consistency**: All styling via BeepStyling/BeepControlStyle
- **Event-Driven**: Loose coupling via events
