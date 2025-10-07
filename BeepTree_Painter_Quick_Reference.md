# BeepTree Painter Quick Reference

## Using TreeStyle

### Setting a Style
```csharp
// In code
myTree.TreeStyle = TreeStyle.Material3;

// In designer
// Select "TreeStyle" property in Properties window
// Choose from dropdown: Standard, Material3, Fluent2, iOS15, etc.
```

### Available Styles (26 total)

#### Application-Specific
- `InfrastructureTree` - VMware vSphere style
- `PortfolioTree` - Jira/Atlassian Portfolio
- `FileManagerTree` - Google Drive/OneDrive  
- `ActivityLogTree` - Timeline/chronological
- `ComponentTree` - Figma/VS Code layers
- `FileBrowserTree` - Windows Explorer
- `DocumentTree` - Multi-level documents

#### Modern Frameworks
- `Material3` - Google Material Design 3
- `Fluent2` - Microsoft Fluent Design 2
- `iOS15` - Apple iOS 15
- `MacOSBigSur` - macOS Big Sur sidebar
- `NotionMinimal` - Notion database
- `VercelClean` - Vercel dashboard
- `Discord` - Discord channels
- `AntDesign` - Ant Design
- `ChakraUI` - Chakra UI
- `Bootstrap` - Bootstrap cards

#### Component Libraries
- `TailwindCard` - Tailwind CSS cards
- `DevExpress` - DevExpress professional
- `Syncfusion` - Syncfusion modern
- `Telerik` - Telerik polished

#### Layout-Specific
- `PillRail` - Pill-shaped navigation
- `StripeDashboard` - Stripe dashboard
- `FigmaCard` - Figma layers panel

#### Default
- `Standard` - Classic Windows Explorer

## Theme Colors Used by Painters

```csharp
// Tree-specific (separate from generic control colors)
TreeBackColor              // Tree background
TreeForeColor              // Normal text
TreeNodeSelectedBackColor  // Selected background
TreeNodeSelectedForeColor  // Selected text
TreeNodeHoverBackColor     // Hover background
TreeNodeHoverForeColor     // Hover text
AccentColor               // Highlights, chevrons
BorderColor               // Lines, separators
```

## Painter Methods

Each painter implements:
- `Paint()` - Overall tree background
- `PaintNodeBackground()` - Row selection/hover
- `PaintToggle()` - Expand/collapse indicator
- `PaintCheckbox()` - Checkbox rendering
- `PaintIcon()` - Node icon
- `PaintText()` - Node text
- `GetPreferredRowHeight()` - Row height calculation

## Creating Custom Painters

```csharp
public class MyCustomTreePainter : BaseTreePainter
{
    public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, 
        bool isHovered, bool isSelected)
    {
        // Use _theme.TreeNodeSelectedBackColor, etc.
        // Use _owner to access tree properties
    }
    
    // Override other methods as needed
}
```

Register in factory:
```csharp
// In BeepTreePainterFactory.CreatePainter()
TreeStyle.MyCustom => new MyCustomTreePainter(),
```

## File Organization

```
BeepTree (partial class)
├── BeepTree.cs               - Main class, events, fields
├── BeepTree.Properties.cs    - TreeStyle property
└── BeepTree.Drawing.cs       - Painter integration

Painters/
├── ITreePainter.cs           - Interface
├── BaseTreePainter.cs        - Base class
├── BeepTreePainterFactory.cs - Factory
├── StandardTreePainter.cs    - Default
├── Material3TreePainter.cs   - Material Design
├── Fluent2TreePainter.cs     - Fluent Design
└── [22 more painters...]

Models/
└── TreeStyle.cs              - Enum (26 values)
```

## Performance Tips

1. **Painter Caching** - Painters are cached automatically
2. **Theme Changes** - Painters refresh on ApplyTheme()
3. **Style Switching** - Fast (cached painter reused)
4. **Custom Painters** - Follow BaseTreePainter pattern

## Troubleshooting

### Painter not rendering?
- Check `TreeStyle` property is set
- Verify `InitializePainter()` called in constructor
- Check theme has tree-specific colors set

### Colors look wrong?
- Painters use tree-specific colors (TreeBackColor, etc.)
- Don't use generic control colors (BackColor, ForeColor)
- Call `ApplyTheme()` after changing theme

### Performance issues?
- Painters are cached (should be fast)
- Check viewport culling is working
- Verify RecalculateLayoutCache() not called too often

## Examples

### Example 1: Basic Usage
```csharp
var tree = new BeepTree();
tree.TreeStyle = TreeStyle.Material3;
tree.Theme = BeepThemesManager.MaterialDarkTheme;
tree.Nodes.Add(new SimpleItem { Text = "Root" });
```

### Example 2: Dynamic Style Switching
```csharp
private void StyleComboBox_SelectedIndexChanged(object sender, EventArgs e)
{
    var selected = (string)styleComboBox.SelectedItem;
    tree.TreeStyle = Enum.Parse<TreeStyle>(selected);
}
```

### Example 3: Theme-Aware
```csharp
tree.TreeStyle = TreeStyle.Discord;  // Dark-optimized
tree.Theme = BeepThemesManager.DarkTheme;

// Switch to light theme
tree.Theme = BeepThemesManager.LightTheme;  // Painter auto-adjusts
```

## Quick Migration from Old Code

**Before:**
```csharp
// Manual drawing in BeepTree
protected override void DrawContent(Graphics g)
{
    // Custom drawing code...
}
```

**After:**
```csharp
// Just set the style!
tree.TreeStyle = TreeStyle.Material3;
// Painter handles all drawing automatically
```

---

**Last Updated:** October 7, 2025  
**Version:** 1.0  
**Total Painters:** 26
