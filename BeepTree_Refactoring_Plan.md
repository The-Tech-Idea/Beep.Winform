# BeepTree Refactoring Plan

## Overview
Refactor the monolithic `BeepTree.cs` (2045 lines) into a modular architecture using:
- **Partial Classes**: Split responsibilities into separate files
- **Helper Classes**: Extract reusable logic into dedicated helpers
- **Painter System**: Implement style-based rendering with BeepStyling and StyledImagePainter
- **BaseControl Integration**: Leverage existing HitTest and Input helpers
- **BeepControlStyle**: Support multiple visual styles (Material3, iOS15, Fluent2, etc.)

## Current Architecture Analysis

### File Structure
- **Single File**: `BeepTree.cs` (2045 lines)
- **Inherits From**: `BaseControl`
- **Dependencies**: BeepButton, BeepImage, BeepCheckBoxBool, BeepScrollBar

### Current Responsibilities
1. **Data Management**: _nodes, _visibleNodes, node traversal
2. **Layout**: RecalculateLayoutCache, node positioning, virtualization
3. **Rendering**: DrawContent, DrawNodeRecursive, DrawNodeFromCache
4. **Scrolling**: Vertical/horizontal scrollbars, offset management
5. **Hit Testing**: LocalHitTest, mouse interaction
6. **Events**: 60+ events for node interactions
7. **Theme Management**: ApplyTheme, font handling
8. **DPI Scaling**: GetScaledBoxSize, GetScaledImageSize, etc.

### Key Features
- **Virtualization**: Only renders visible nodes
- **Expandable Nodes**: Toggle expand/collapse
- **Checkboxes**: Optional node checkboxes
- **Icons**: Node icons via ImagePath
- **Multi-Select**: Optional multi-selection support
- **Custom Renderers**: BeepButton, BeepImage, BeepCheckBoxBool

## Refactored Architecture

### 1. Partial Class Structure

#### BeepTree.cs (Main Entry)
**Purpose**: Main class declaration, constructor, disposal
**Lines**: ~50-100
**Content**:
- Namespace and class declaration
- Constructor with initialization
- Dispose pattern
- Component initialization

#### BeepTree.Core.cs
**Purpose**: Core fields, initialization, data management
**Lines**: ~200-300
**Content**:
- Private fields (_nodes, _visibleNodes, _selectedNode)
- Renderer instances (_toggleRenderer, _checkRenderer, _iconRenderer, _textRenderer)
- Cache fields (_layoutCache, _measurementCache)
- Helper instances (BeepTreeHelper, BeepTreeLayoutHelper, BeepTreePainter)
- DPI scaling fields
- Initialization methods

#### BeepTree.Properties.cs
**Purpose**: All public/browsable properties
**Lines**: ~300-400
**Content**:
- **Node Data Properties**: Nodes, SelectedNode, SelectedNodes, ClickedNode
- **Display Properties**: ShowCheckBox, ShowIcons, TextAlignment
- **Scrolling Properties**: ShowVerticalScrollBar, ShowHorizontalScrollBar
- **Performance Properties**: VirtualizeLayout, VirtualizationBufferRows
- **Style Properties**: ControlStyle (BeepControlStyle enum)
- **Font Properties**: TextFont, UseThemeFont, UseScaledFont
- **Selection Properties**: AllowMultiSelect
- **Icon Properties**: PlusIcon, MinusIcon

#### BeepTree.Events.cs
**Purpose**: Event declarations and invocation methods
**Lines**: ~200-250
**Content**:
- 60+ event declarations (NodeSelected, NodeExpanded, NodeChecked, etc.)
- Protected OnXXX methods for event raising
- Event argument classes if needed

#### BeepTree.Methods.cs
**Purpose**: Public API methods for tree manipulation
**Lines**: ~200-300
**Content**:
- **Node Management**: AddNode, RemoveNode, ClearNodes, FindNode
- **Expansion**: ExpandAll, CollapseAll, ExpandTo
- **Selection**: SelectNode, DeselectNode, ClearSelection
- **Visibility**: EnsureVisible, ScrollToNode
- **Data Operations**: RefreshNodes, RebuildVisible
- **Search**: FindItemByGuid, TraverseAll

#### BeepTree.Drawing.cs
**Purpose**: Rendering logic, painter management
**Lines**: ~200-300
**Content**:
- `DrawContent` override
- `CreatePainter` factory method for style-based painters
- Drawing helper methods
- Painter cache management
- Integration with BeepTreePainter

#### BeepTree.Scrolling.cs
**Purpose**: Scrollbar management and scrolling logic
**Lines**: ~150-200
**Content**:
- ScrollBar initialization
- UpdateScrollBars method
- Scroll event handlers
- Offset management (_xOffset, _yOffset)
- Virtual size calculation

### 2. Helper Classes

#### BeepTreeHelper.cs
**Purpose**: Tree data structure operations
**Location**: `Trees/Helpers/BeepTreeHelper.cs`
**Lines**: ~200-300
**Responsibilities**:
- Node traversal algorithms (DFS, BFS)
- Node search and filtering
- Parent-child relationship management
- Node state management (expanded, checked, selected)
- GUID-based node lookup
- Visible node list rebuilding

**Key Methods**:
```csharp
public class BeepTreeHelper
{
    private readonly BeepTree _owner;
    
    public BeepTreeHelper(BeepTree owner);
    
    // Traversal
    public IEnumerable<SimpleItem> TraverseAll();
    public IEnumerable<SimpleItem> TraverseVisible();
    public SimpleItem FindByGuid(string guid);
    public SimpleItem FindByPredicate(Func<SimpleItem, bool> predicate);
    
    // Visibility
    public List<NodeInfo> BuildVisibleNodeList();
    public void RebuildVisibleCache();
    
    // State
    public void ExpandAll();
    public void CollapseAll();
    public void ExpandTo(SimpleItem item);
    public void SetNodeChecked(SimpleItem item, bool isChecked);
    
    // Measurement
    public int GetNodeLevel(SimpleItem item);
    public int GetVisibleNodeCount();
}
```

#### BeepTreeLayoutHelper.cs
**Purpose**: Layout calculation and caching
**Location**: `Trees/Helpers/BeepTreeLayoutHelper.cs`
**Lines**: ~250-350
**Responsibilities**:
- Layout cache management (NodeInfo structs)
- Text measurement using TextRenderer
- Row height calculation
- Indent calculation
- Rectangle calculation for node parts (toggle, check, icon, text)
- Virtualization logic (viewport intersection)
- DPI-aware scaling

**Key Methods**:
```csharp
public class BeepTreeLayoutHelper
{
    private readonly BeepTree _owner;
    private Dictionary<string, NodeInfo> _layoutCache;
    
    public BeepTreeLayoutHelper(BeepTree owner);
    
    // Layout
    public void RecalculateLayout();
    public void RecalculateLayoutForViewport(int viewportTop, int viewportBottom);
    public NodeInfo CalculateNodeLayout(SimpleItem item, int level, int y);
    
    // Measurement
    public Size MeasureText(string text, Font font);
    public int CalculateRowHeight(SimpleItem item, Size textSize);
    public int CalculateIndent(int level);
    
    // Virtualization
    public (int startIndex, int endIndex) GetViewportNodeRange();
    public bool IsNodeInViewport(NodeInfo node);
    
    // Cache
    public NodeInfo GetCachedLayout(SimpleItem item);
    public void InvalidateCache();
    public void InvalidateNode(SimpleItem item);
}
```

#### BeepTreeHitTestHelper.cs
**Purpose**: Hit testing and mouse interaction
**Location**: `Trees/Helpers/BeepTreeHitTestHelper.cs`
**Lines**: ~150-200
**Responsibilities**:
- Hit testing for node parts (toggle, checkbox, icon, text, row)
- Mouse hover detection
- Click detection and routing
- Integration with BaseControl._hitTest helper

**Key Methods**:
```csharp
public class BeepTreeHitTestHelper
{
    private readonly BeepTree _owner;
    private readonly BeepTreeLayoutHelper _layoutHelper;
    
    public BeepTreeHitTestHelper(BeepTree owner, BeepTreeLayoutHelper layoutHelper);
    
    // Hit Testing
    public bool HitTest(Point point, out string hitName, out SimpleItem item, out Rectangle rect);
    public SimpleItem GetNodeAt(Point point);
    public string GetHitPartType(Point point);
    
    // Hover
    public void UpdateHover(Point mousePosition);
    public void ClearHover();
    
    // Helpers
    public Rectangle TransformToViewport(Rectangle contentRect);
    public Point TransformToContent(Point viewportPoint);
}
```

### 3. Painter System

#### ITreePainter.cs (Interface)
**Purpose**: Contract for tree painters
**Location**: `Trees/Painters/ITreePainter.cs`
**Lines**: ~50-80

```csharp
public interface ITreePainter
{
    void Initialize(BeepTree owner, IBeepTheme theme);
    void Paint(Graphics g, BeepTree owner, Rectangle bounds);
    void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected);
    void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren);
    void PaintCheckbox(Graphics g, Rectangle checkRect, bool isChecked);
    void PaintIcon(Graphics g, Rectangle iconRect, string imagePath);
    void PaintText(Graphics g, Rectangle textRect, string text, bool isSelected, bool isHovered);
    void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected);
    int GetPreferredRowHeight(SimpleItem item, Font font);
}
```

#### BaseTreePainter.cs (Abstract Base)
**Purpose**: Common painter functionality
**Location**: `Trees/Painters/BaseTreePainter.cs`
**Lines**: ~200-300
**Content**:
- Common drawing utilities
- Theme color extraction
- Font management
- DPI scaling helpers
- Default implementations

#### Concrete Painters (26 styles):
1. **StandardTreePainter.cs** - Classic Windows tree
2. **Material3TreePainter.cs** - Material Design 3
3. **MaterialYouTreePainter.cs** - Material You
4. **iOS15TreePainter.cs** - iOS 15 style
5. **MacOSBigSurTreePainter.cs** - macOS Big Sur
6. **Fluent2TreePainter.cs** - Fluent Design 2
7. **Windows11MicaTreePainter.cs** - Windows 11 Mica
8. **MinimalTreePainter.cs** - Minimal style
9. **NotionMinimalTreePainter.cs** - Notion-inspired
10. **VercelCleanTreePainter.cs** - Vercel clean
11. **NeumorphismTreePainter.cs** - Neumorphism
12. **GlassAcrylicTreePainter.cs** - Glass/Acrylic
13. **DarkGlowTreePainter.cs** - Dark with glow
14. **GradientModernTreePainter.cs** - Modern gradient
15. **BootstrapTreePainter.cs** - Bootstrap style
16. **TailwindCardTreePainter.cs** - Tailwind CSS
17. **StripeDashboardTreePainter.cs** - Stripe dashboard
18. **FigmaCardTreePainter.cs** - Figma style
19. **DiscordStyleTreePainter.cs** - Discord style
20. **AntDesignTreePainter.cs** - Ant Design
21. **ChakraUITreePainter.cs** - Chakra UI
22. **PillRailTreePainter.cs** - Pill rail style

**Each Painter Structure** (~100-150 lines):
```csharp
public class Material3TreePainter : BaseTreePainter
{
    public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
    {
        // Material 3 specific rendering
    }
    
    public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
    {
        // Material 3 node with ripple effect
        // Elevation shadows
        // Material color scheme
    }
    
    public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren)
    {
        // Material chevron icons
        // Rotation animation hints
    }
    
    // ... other overrides
}
```

#### BeepTreePainterFactory.cs
**Purpose**: Create painters based on ControlStyle
**Location**: `Trees/Painters/BeepTreePainterFactory.cs`
**Lines**: ~50-100

```csharp
public static class BeepTreePainterFactory
{
    private static Dictionary<BeepControlStyle, ITreePainter> _painterCache = new();
    
    public static ITreePainter CreatePainter(BeepControlStyle style, BeepTree owner, IBeepTheme theme)
    {
        if (_painterCache.TryGetValue(style, out var cached))
        {
            cached.Initialize(owner, theme);
            return cached;
        }
        
        ITreePainter painter = style switch
        {
            BeepControlStyle.Material3 => new Material3TreePainter(),
            BeepControlStyle.MaterialYou => new MaterialYouTreePainter(),
            BeepControlStyle.iOS15 => new iOS15TreePainter(),
            // ... all 26 styles
            _ => new StandardTreePainter()
        };
        
        painter.Initialize(owner, theme);
        _painterCache[style] = painter;
        return painter;
    }
    
    public static void ClearCache() => _painterCache.Clear();
}
```

### 4. NodeInfo Structure

**Location**: `Trees/Models/NodeInfo.cs`
**Purpose**: Layout cache structure for visible nodes

```csharp
public struct NodeInfo
{
    public SimpleItem Item { get; set; }
    public int Level { get; set; }
    public int Y { get; set; }
    public int RowHeight { get; set; }
    public int RowWidth { get; set; }
    public Size TextSize { get; set; }
    
    // Content-space rectangles (before scroll offset)
    public Rectangle RowRectContent { get; set; }
    public Rectangle ToggleRectContent { get; set; }
    public Rectangle CheckRectContent { get; set; }
    public Rectangle IconRectContent { get; set; }
    public Rectangle TextRectContent { get; set; }
    public Rectangle Bounds { get; set; }
}
```

## Integration Strategy

### 1. BaseControl HitTest Helper
```csharp
// In BeepTree.Core.cs - use existing BaseControl._hitTest
protected override void OnMouseDown(MouseEventArgs e)
{
    // Use BeepTreeHitTestHelper which integrates with _hitTest
    if (_hitTestHelper.HitTest(e.Location, out var hitName, out var item, out var rect))
    {
        HandleNodeClick(hitName, item, e.Button);
    }
}
```

### 2. BaseControl Input Helper
```csharp
// In BeepTree.Core.cs - leverage _input from BaseControl
public BeepTree()
{
    // Input helper already initialized in BaseControl
    // Subscribe to keyboard events
    _input.UpArrowKeyPressed += OnUpArrowKey;
    _input.DownArrowKeyPressed += OnDownArrowKey;
    _input.LeftArrowKeyPressed += OnLeftArrowKey;
    _input.RightArrowKeyPressed += OnRightArrowKey;
    _input.EnterKeyPressed += OnEnterKey;
}

private void OnUpArrowKey(object sender, EventArgs e)
{
    SelectPreviousNode();
}
```

### 3. BeepStyling Integration
```csharp
// In painter classes
public override void PaintNodeBackground(Graphics g, Rectangle bounds, bool isHovered, bool isSelected)
{
    if (isSelected)
    {
        // Use BeepStyling for consistent styling
        var bgColor = BeepStyling.GetThemeColor("Primary");
        using (var brush = new SolidBrush(bgColor))
        {
            g.FillRectangle(brush, bounds);
        }
    }
    else if (isHovered)
    {
        var hoverColor = BeepStyling.GetThemeColor("Surface");
        using (var brush = new SolidBrush(Color.FromArgb(20, hoverColor)))
        {
            g.FillRectangle(brush, bounds);
        }
    }
}
```

### 4. StyledImagePainter Integration
```csharp
// In BaseTreePainter.cs
public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
{
    if (string.IsNullOrEmpty(imagePath))
        return;
    
    // Use StyledImagePainter for consistent image rendering with caching
    StyledImagePainter.Paint(g, iconRect, imagePath, _owner.ControlStyle);
}
```

### 5. ControlStyle Property
```csharp
// In BeepTree.Properties.cs
private BeepControlStyle _controlStyle = BeepControlStyle.Material3;
private ITreePainter _treePainter;

[Browsable(true)]
[Category("Appearance")]
[Description("The visual style/painter to use for rendering the tree.")]
[DefaultValue(BeepControlStyle.Material3)]
public BeepControlStyle ControlStyle
{
    get => _controlStyle;
    set
    {
        if (_controlStyle != value)
        {
            _controlStyle = value;
            _treePainter = BeepTreePainterFactory.CreatePainter(_controlStyle, this, _currentTheme);
            Invalidate();
        }
    }
}
```

## Implementation Steps

### Phase 1: Create Infrastructure (Day 1)
1. ✅ Create `BeepTree_Refactoring_Plan.md`
2. Create folder structure:
   - `Trees/Helpers/`
   - `Trees/Painters/`
   - `Trees/Models/`
3. Create `NodeInfo.cs` struct
4. Create `ITreePainter.cs` interface
5. Create `BaseTreePainter.cs` abstract class

### Phase 2: Extract Helpers (Day 1-2)
1. Create `BeepTreeHelper.cs`
   - Extract traversal methods
   - Extract node search methods
   - Extract state management
2. Create `BeepTreeLayoutHelper.cs`
   - Extract RecalculateLayoutCache
   - Extract measurement logic
   - Extract virtualization logic
3. Create `BeepTreeHitTestHelper.cs`
   - Extract LocalHitTest
   - Extract hover detection
   - Integrate with BaseControl._hitTest

### Phase 3: Create Partial Classes (Day 2)
1. Create `BeepTree.cs` (main)
2. Create `BeepTree.Core.cs`
   - Move fields
   - Create helper instances
   - Move initialization
3. Create `BeepTree.Properties.cs`
   - Move all properties
   - Add ControlStyle property
4. Create `BeepTree.Events.cs`
   - Move all 60+ events
   - Create OnXXX methods
5. Create `BeepTree.Methods.cs`
   - Move public API methods
6. Create `BeepTree.Drawing.cs`
   - Move DrawContent
   - Create painter integration
7. Create `BeepTree.Scrolling.cs`
   - Move scrollbar logic

### Phase 4: Create Painters (Day 3-5)
1. Create `StandardTreePainter.cs` (default)
2. Create `Material3TreePainter.cs`
3. Create `iOS15TreePainter.cs`
4. Create remaining 19 painters
5. Create `BeepTreePainterFactory.cs`

### Phase 5: Integration & Testing (Day 5-6)
1. Test each painter style
2. Verify BaseControl integration
3. Test hit testing
4. Test keyboard navigation
5. Test virtualization
6. Test scrolling
7. Performance testing

### Phase 6: Cleanup (Day 6)
1. Remove old `BeepTree.cs` after verification
2. Update documentation
3. Add XML comments
4. Code review

## File Structure After Refactoring

```
Trees/
├── BeepTree.cs                          (50-100 lines)
├── BeepTree.Core.cs                     (200-300 lines)
├── BeepTree.Properties.cs               (300-400 lines)
├── BeepTree.Events.cs                   (200-250 lines)
├── BeepTree.Methods.cs                  (200-300 lines)
├── BeepTree.Drawing.cs                  (200-300 lines)
├── BeepTree.Scrolling.cs                (150-200 lines)
├── Helpers/
│   ├── BeepTreeHelper.cs                (200-300 lines)
│   ├── BeepTreeLayoutHelper.cs          (250-350 lines)
│   └── BeepTreeHitTestHelper.cs         (150-200 lines)
├── Painters/
│   ├── ITreePainter.cs                  (50-80 lines)
│   ├── BaseTreePainter.cs               (200-300 lines)
│   ├── BeepTreePainterFactory.cs        (50-100 lines)
│   ├── StandardTreePainter.cs           (100-150 lines)
│   ├── Material3TreePainter.cs          (100-150 lines)
│   ├── MaterialYouTreePainter.cs        (100-150 lines)
│   ├── iOS15TreePainter.cs              (100-150 lines)
│   ├── MacOSBigSurTreePainter.cs        (100-150 lines)
│   ├── Fluent2TreePainter.cs            (100-150 lines)
│   ├── Windows11MicaTreePainter.cs      (100-150 lines)
│   ├── MinimalTreePainter.cs            (100-150 lines)
│   ├── NotionMinimalTreePainter.cs      (100-150 lines)
│   ├── VercelCleanTreePainter.cs        (100-150 lines)
│   ├── NeumorphismTreePainter.cs        (100-150 lines)
│   ├── GlassAcrylicTreePainter.cs       (100-150 lines)
│   ├── DarkGlowTreePainter.cs           (100-150 lines)
│   ├── GradientModernTreePainter.cs     (100-150 lines)
│   ├── BootstrapTreePainter.cs          (100-150 lines)
│   ├── TailwindCardTreePainter.cs       (100-150 lines)
│   ├── StripeDashboardTreePainter.cs    (100-150 lines)
│   ├── FigmaCardTreePainter.cs          (100-150 lines)
│   ├── DiscordStyleTreePainter.cs       (100-150 lines)
│   ├── AntDesignTreePainter.cs          (100-150 lines)
│   ├── ChakraUITreePainter.cs           (100-150 lines)
│   └── PillRailTreePainter.cs           (100-150 lines)
└── Models/
    └── NodeInfo.cs                       (50-80 lines)
```

## Benefits

### 1. Maintainability
- **Separation of Concerns**: Each file has single responsibility
- **Easier Navigation**: Find code by function, not by line number
- **Reduced Complexity**: Each file is <400 lines

### 2. Extensibility
- **New Styles**: Add new painter without touching tree logic
- **New Features**: Add to specific partial class
- **Easy Testing**: Test helpers independently

### 3. Reusability
- **Helpers**: Can be used by other tree-like controls
- **Painters**: Share styling across controls
- **Layout Logic**: Reusable measurement and positioning

### 4. Performance
- **Cached Painters**: Create once, reuse
- **Lazy Loading**: Only load painters for used styles
- **Optimized Measurement**: TextRenderer without Graphics objects

### 5. Consistency
- **BeepStyling**: Consistent theming across all controls
- **StyledImagePainter**: Consistent image rendering
- **BaseControl Integration**: Leverage existing infrastructure

## Testing Strategy

### Unit Tests
- Test each helper class independently
- Test layout calculations
- Test hit testing logic
- Test painter rendering (pixel comparison)

### Integration Tests
- Test painter switching
- Test theme changes
- Test scrolling with different painters
- Test keyboard navigation

### Performance Tests
- Large tree (10,000+ nodes)
- Rapid style switching
- Rapid scrolling
- Memory profiling

## Migration Path

### For Existing Code Using BeepTree
- ✅ **100% Backward Compatible**
- All existing properties preserved
- All existing events preserved
- All existing methods preserved
- Only new feature: `ControlStyle` property

### For New Code
- Recommended: Use `ControlStyle` property for visual styling
- Recommended: Leverage BaseControl input helpers
- Recommended: Use theme colors via BeepStyling

## Success Criteria

1. ✅ Zero breaking changes to public API
2. ✅ All 60+ events working
3. ✅ All 26 painters implemented
4. ✅ Performance equal or better than original
5. ✅ Code coverage >80%
6. ✅ All existing tests passing
7. ✅ Documentation complete
8. ✅ Designer support working

## Timeline

- **Day 1**: Infrastructure + Helpers (8 hours)
- **Day 2**: Partial classes + Initial painters (8 hours)
- **Day 3-4**: Complete all 26 painters (16 hours)
- **Day 5**: Integration + Testing (8 hours)
- **Day 6**: Cleanup + Documentation (8 hours)
- **Total**: 48 hours (6 working days)

## Notes

- Use TextRenderer.MeasureText (no Graphics object needed)
- Cache painters for performance
- Use StyledImagePainter for all image rendering
- Integrate with BaseControl._hitTest and _input
- Follow BeepListBox refactoring pattern as reference
- Keep NodeInfo as struct for performance
- Maintain virtualization for large trees
- Preserve all existing functionality
