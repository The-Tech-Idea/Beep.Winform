# BeepMultiSplitter Skill

## Overview
`BeepMultiSplitter` is a resizable TableLayoutPanel wrapper with drag-to-resize, context menu, and design-time support.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `TableLayoutPanel` | `TableLayoutPanel` | Inner layout panel |
| `RowCount` | `int` | Number of rows |
| `ColumnCount` | `int` | Number of columns |
| `RowStyles` | Collection | Row sizing styles |
| `ColumnStyles` | Collection | Column sizing styles |
| `CellBorderStyle` | `TableLayoutPanelCellBorderStyle` | Cell border style |

## Usage Examples

### Basic Splitter
```csharp
var splitter = new BeepMultiSplitter
{
    RowCount = 2,
    ColumnCount = 2,
    CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
};

// Add controls to cells
splitter.TableLayoutPanel.Controls.Add(leftPanel, 0, 0);
splitter.TableLayoutPanel.Controls.Add(rightPanel, 1, 0);
```

### Configure Sizing
```csharp
// Percent-based columns
splitter.ColumnStyles.Clear();
splitter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
splitter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

// Fixed height row
splitter.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
```

## Features
- **Drag-to-Resize**: Drag column/row borders to resize
- **Context Menu**: Right-click to Add/Remove Row/Column
- **Drag-and-Drop**: Drop controls into cells
- **Design-Time Support**: Visual designer integration

## Context Menu Actions
- Add Row
- Remove Row
- Add Column
- Remove Column

## Related Controls
- `BeepPanel` - Container panel
