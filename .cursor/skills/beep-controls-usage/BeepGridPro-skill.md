# BeepGridPro Skill

## Overview
Advanced data grid with multiple visual styles, navigation styles, and layout presets inspired by popular JavaScript frameworks.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.GridX;
```

---

## Key Properties

### Data Binding
| Property | Type | Description |
|----------|------|-------------|
| `DataSource` | `object` | Data source (IList, DataTable, etc.) |
| `DataMember` | `string` | Data member for complex sources |
| `Uow` | `object` | UnitOfWork for entity tracking |
| `Columns` | `BeepGridColumnConfigCollection` | Column definitions |
| `Rows` | `BindingList<BeepRowConfig>` | Row data |

### Appearance
| Property | Type | Description |
|----------|------|-------------|
| `GridStyle` | `BeepGridStyle` | Visual style preset |
| `NavigationStyle` | `navigationStyle` | Navigator appearance |
| `LayoutPreset` | `GridLayoutPreset` | Structural layout |
| `UsePainterNavigation` | `bool` | Modern vs legacy navigator |

### Layout
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RowHeight` | `int` | 18+ | Row height in pixels |
| `ColumnHeaderHeight` | `int` | 22+ | Header height |
| `ShowColumnHeaders` | `bool` | true | Show/hide headers |
| `ShowNavigator` | `bool` | true | Show/hide navigator |
| `ShowCheckBox` | `bool` | false | Show selection checkbox |

### Behavior
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AllowUserToResizeColumns` | `bool` | true | Column resize |
| `AllowUserToResizeRows` | `bool` | false | Row resize |
| `AllowColumnReorder` | `bool` | true | Drag column headers |
| `ReadOnly` | `bool` | false | Edit mode |
| `MultiSelect` | `bool` | true | Multiple row selection |
| `AutoSizeColumnsMode` | `DataGridViewAutoSizeColumnsMode` | None | Auto-sizing |

---

## BeepGridStyle Enum (12 Styles)
```csharp
Default, Bootstrap, Material, DataTables, AGGrid, Handsontable,
Kendo, DevExtreme, Wijmo, Syncfusion, Telerik, Custom
```

---

## navigationStyle Enum (12 Styles)
```csharp
None, Standard, Bootstrap, Material, DataTables, AGGrid,
Handsontable, Kendo, DevExtreme, Wijmo, Syncfusion, Telerik
```

---

## GridLayoutPreset Enum (12 Presets)
```csharp
Default, Clean, Dense, Striped, Borderless, HeaderBold,
MaterialHeader, Card, ComparisonTable, MatrixSimple, MatrixStriped, PricingTable
```

---

## Usage Examples

### Basic Binding
```csharp
var grid = new BeepGridPro
{
    GridStyle = BeepGridStyle.Bootstrap,
    NavigationStyle = navigationStyle.Bootstrap,
    LayoutPreset = GridLayoutPreset.Striped
};
grid.DataSource = myDataList;
```

### DataTable Binding
```csharp
grid.DataSource = myDataTable;
grid.DataMember = ""; // Optional table name
```

### Configure Columns
```csharp
grid.Columns["Name"].Width = 200;
grid.Columns["Name"].HeaderText = "Full Name";
grid.Columns["Age"].Visible = true;
grid.Columns["ID"].Visible = false;
```

### Navigation Setup
```csharp
grid.ShowNavigator = true;
grid.NavigationStyle = navigationStyle.Material;
grid.UsePainterNavigation = true; // Modern look
```

### Selection Handling
```csharp
grid.MultiSelect = true;
grid.ShowCheckBox = true;
// Access selected rows via grid.Rows where IsSelected
```

### Apply Layout Preset
```csharp
grid.LayoutPreset = GridLayoutPreset.Card;
// Or apply programmatically:
grid.ApplyLayoutPreset(GridLayoutPreset.Dense);
```

---

## Column Configuration

### BeepColumnConfig Properties
| Property | Type | Description |
|----------|------|-------------|
| `FieldName` | `string` | Data field binding |
| `HeaderText` | `string` | Display header |
| `Width` | `int` | Column width |
| `Visible` | `bool` | Visibility |
| `ReadOnly` | `bool` | Edit protection |
| `IsSelectionCheckBox` | `bool` | Checkbox column |
| `IsRowNumColumn` | `bool` | Row number column |
| `IsRowID` | `bool` | ID column |

---

## Events
- `CellValueChanged` - Cell edit completed
- `SelectionChanged` - Row selection changed
- `ColumnHeaderClick` - Header clicked (sorting)
- `RowDoubleClick` - Row double-clicked

---

## Filtering & Context Menu

### Built-in Context Menu
Right-click for: Copy, Paste, Delete, Export, Filter options

### Filtering
```csharp
grid.Filter("Name", "John"); // Simple filter
grid.ClearFilters(); // Remove all filters
```

## Related
- `BeepGridStyles` - Style definitions
- `GridLayoutPreset` - Layout helpers
- `BeepDataNavigator` - Navigation control
