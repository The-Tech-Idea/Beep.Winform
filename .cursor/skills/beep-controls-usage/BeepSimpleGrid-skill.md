# BeepSimpleGrid Skill

## Overview
`BeepSimpleGrid` is a comprehensive data grid with virtual scrolling, column/row customization, filtering, sorting, selection, and data binding. ~10,000 lines of functionality.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
```

## Key Properties

### Data Properties
| Property | Type | Description |
|----------|------|-------------|
| `DataSource` | `object` | Data source (List, DataTable, IEnumerable) |
| `Entity` | `EntityStructure` | Entity metadata |
| `EntityName` | `string` | Entity name |
| `Columns` | `List<BeepColumnConfig>` | Column configuration |
| `Rows` | `BindingList<BeepRowConfig>` | Row collection |

### Layout Properties
| Property | Type | Description |
|----------|------|-------------|
| `RowHeight` | `int` | Row height (default: 25) |
| `ColumnHeaderHeight` | `int` | Header height (default: 40) |
| `DefaultColumnHeaderWidth` | `int` | Default column width (50) |
| `ShowVerticalGridLines` | `bool` | Show vertical lines |
| `ShowHorizontalGridLines` | `bool` | Show horizontal lines |
| `FitColumnToContent` | `bool` | Auto-resize columns |

### Visibility Properties
| Property | Type | Description |
|----------|------|-------------|
| `ShowHeaderPanel` | `bool` | Show title header |
| `ShowColumnHeaders` | `bool` | Show column headers |
| `ShowRowHeaders` | `bool` | Show row headers |
| `ShowRowNumbers` | `bool` | Show row numbers |
| `ShowNavigator` | `bool` | Show data navigator |
| `ShowFilter` | `bool` | Show filter panel |
| `ShowFooter` | `bool` | Show footer |
| `ShowAggregationRow` | `bool` | Show aggregation row |
| `ShowSortIcons` | `bool` | Show sort icons |
| `ShowCheckboxes` | `bool` | Show selection checkboxes |
| `ShowVerticalScrollBar` | `bool` | Show vertical scrollbar |
| `ShowHorizontalScrollBar` | `bool` | Show horizontal scrollbar |

### Title Properties
| Property | Type | Description |
|----------|------|-------------|
| `TitleText` | `string` | Grid title |
| `TitleHeaderImage` | `string` | Title icon path |
| `TitleTextFont` | `Font` | Title font |
| `TextImageRelation` | `TextImageRelation` | Title layout |

### Selection Properties
| Property | Type | Description |
|----------|------|-------------|
| `SelectedRows` | `List<int>` | Selected row indices |
| `SelectedGridRows` | `List<BeepRowConfig>` | Selected row objects |
| `SelectionColumnWidth` | `int` | Checkbox column width |

### Edit Properties
| Property | Type | Description |
|----------|------|-------------|
| `AllowUserToAddRows` | `bool` | Allow adding rows |
| `AllowUserToDeleteRows` | `bool` | Allow deleting rows |
| `IsEditorShown` | `bool` | Editor visible |

## Column Configuration (BeepColumnConfig)
```csharp
var column = new BeepColumnConfig
{
    ColumnName = "Name",          // Field name
    HeaderText = "Full Name",     // Display header
    Width = 150,                  // Column width
    Visible = true,               // Visibility
    ReadOnly = false,             // Editable
    ColumnType = BeepColumnType.Text,  // Column type
    SortDirection = SortDirection.None // Sorting
};
grid.Columns.Add(column);
```

## BeepColumnType Options
- `Text` - Plain text
- `Number` - Numeric values
- `DateTime` - Date/time picker
- `Boolean` / `CheckBox` - Checkbox
- `ComboBox` - Dropdown selection
- `Image` - Image display
- `Button` - Click button
- `Progress` - Progress bar
- `Link` - Hyperlink

## Usage Examples

### Basic Grid
```csharp
var grid = new BeepSimpleGrid
{
    TitleText = "Customers",
    ShowNavigator = true,
    ShowFilter = true,
    ShowCheckboxes = true
};
grid.DataSource = customers;
```

### Configure Columns
```csharp
var grid = new BeepSimpleGrid();
grid.Columns.Add(new BeepColumnConfig { ColumnName = "Id", Width = 50, ReadOnly = true });
grid.Columns.Add(new BeepColumnConfig { ColumnName = "Name", Width = 200 });
grid.Columns.Add(new BeepColumnConfig { ColumnName = "Email", Width = 250 });
grid.Columns.Add(new BeepColumnConfig { ColumnName = "Active", ColumnType = BeepColumnType.CheckBox, Width = 80 });
grid.DataSource = customers;
```

### With Selection
```csharp
grid.ShowCheckboxes = true;
grid.SelectedRowsChanged += (s, e) =>
{
    foreach (var row in grid.SelectedGridRows)
    {
        Console.WriteLine($"Selected: {row.DataItem}");
    }
};
```

### With Navigator
```csharp
var navigator = new BeepBindingNavigator();
grid.DataNavigator = navigator;
grid.ShowNavigator = true;
```

## Events
| Event | Description |
|-------|-------------|
| `SelectedRowsChanged` | Selection changed |

## Related Controls
- `BeepGridX` - Advanced grid features
- `BeepBindingNavigator` - Data navigation
- `BeepColumnConfig` - Column configuration
- `BeepRowConfig` - Row configuration
