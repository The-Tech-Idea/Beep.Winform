# BeepGridX Skill

## Overview
Advanced data grid with sorting, filtering, editing, and virtual scrolling.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.GridX;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `DataSource` | `object` | Data binding |
| `Columns` | `List<GridColumn>` | Column definitions |
| `AllowEdit` | `bool` | Enable editing |
| `AllowSort` | `bool` | Enable sorting |
| `AllowFilter` | `bool` | Enable filtering |
| `AllowGrouping` | `bool` | Enable row grouping |
| `PageSize` | `int` | Rows per page |
| `ShowRowNumbers` | `bool` | Display row numbers |

## Events
| Event | Description |
|-------|-------------|
| `CellValueChanged` | Cell edited |
| `SelectionChanged` | Row selection changed |
| `RowDoubleClick` | Row double-clicked |

## Usage
```csharp
var grid = new BeepGridX
{
    AllowEdit = true,
    AllowSort = true,
    AllowFilter = true
};
grid.Columns.Add(new GridColumn { FieldName = "Name", Header = "Name" });
grid.Columns.Add(new GridColumn { FieldName = "Price", Header = "Price", Format = "C2" });
grid.DataSource = products;
```

## Related Controls
- `BeepSimpleGrid` - Lightweight grid
- `BeepVerticalTable` - Vertical property grid
