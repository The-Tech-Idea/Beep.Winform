# BeepTree Skill

## Overview
`BeepTree` is a hierarchical tree control with painter-based rendering, multi-select, checkboxes, virtualization for large datasets, and theme integration.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Trees;
```

## TreeStyle
Tree visual style determined by painter (via TreeStyle property):
- Material3, Fluent2, iOS, Minimal, Classic

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `TreeStyle` | `TreeStyle` | Visual style/painter |
| `Nodes` | `List<SimpleItem>` | Root nodes (hierarchical via Children) |
| `SelectedNode` | `SimpleItem` | Currently selected node |
| `SelectedNodes` | `List<SimpleItem>` | Multiple selected nodes |
| `ShowCheckBox` | `bool` | Show checkboxes on nodes |
| `AllowMultiSelect` | `bool` | Enable multi-selection |
| `TextFont` | `Font` | Node text font |
| `TextAlignment` | `TextAlignment` | Label alignment |
| `VirtualizeLayout` | `bool` | Enable virtualization (perf) |
| `VirtualizationBufferRows` | `int` | Extra rows to pre-render |
| `ShowVerticalScrollBar` | `bool` | Show vertical scroll |
| `ShowHorizontalScrollBar` | `bool` | Show horizontal scroll |

## Theme Properties (read-only)
| Property | Returns from Theme |
|----------|-------------------|
| `TreeBackColor` | Tree background |
| `TreeForeColor` | Tree foreground |
| `TreeNodeSelectedBackColor` | Selected node bg |
| `TreeNodeSelectedForeColor` | Selected node fg |
| `TreeNodeHoverBackColor` | Hovered node bg |
| `TreeNodeHoverForeColor` | Hovered node fg |

## Usage Examples

### Basic Tree
```csharp
var tree = new BeepTree
{
    TreeStyle = TreeStyle.Material3,
    UseThemeColors = true
};

tree.Nodes.Add(new SimpleItem
{
    Text = "Root",
    Children = new List<SimpleItem>
    {
        new SimpleItem { Text = "Child 1" },
        new SimpleItem { Text = "Child 2" }
    }
});
```

### With Checkboxes
```csharp
var tree = new BeepTree
{
    ShowCheckBox = true,
    AllowMultiSelect = true
};
```

### Handle Selection
```csharp
tree.NodeSelected += (s, e) =>
{
    var node = tree.SelectedNode;
    Console.WriteLine($"Selected: {node?.Text}");
};
```

### Virtualization (large trees)
```csharp
var tree = new BeepTree
{
    VirtualizeLayout = true,
    VirtualizationBufferRows = 5
};
```

## Methods
| Method | Description |
|--------|-------------|
| `RefreshTree()` | Rebuild and redraw |

## Architecture
- Partial classes: Core, Properties, Events, Drawing, Scrolling, Layout, Methods
- Helpers: BeepTreeHelper, BeepTreeLayoutHelper, BeepTreeHitTestHelper
- Painter factory for TreeStyle
