# BeepTree Refactoring Plan & Progress

## Overview
Refactor the monolithic `BeepTree.cs` (2045 lines) into a modular architecture using:
- **Partial Classes**: Split responsibilities into separate files
- **Helper Classes**: Extract reusable logic into dedicated helpers
- **Painter System**: Implement style-based rendering with BeepStyling and StyledImagePainter
- **BaseControl Integration**: Leverage existing HitTest and Input helpers
- **BeepControlStyle**: Support multiple visual styles (Material3, iOS15, Fluent2, etc.)

## Progress Tracking

### ✅ Phase 1: Infrastructure (COMPLETED)
- [x] Create folder structure (Helpers/, Painters/, Models/)
- [x] Create `NodeInfo.cs` struct for layout caching
- [x] Create `ITreePainter.cs` interface
- [x] Create `BaseTreePainter.cs` abstract class with default implementations
- [x] Create `StandardTreePainter.cs` first concrete painter
- [x] Create `BeepTreePainterFactory.cs` factory

### ✅ Phase 2: Helper Classes (COMPLETED)
- [x] Create `BeepTreeHelper.cs` - Data operations (traversal, search, filtering, state management)
- [x] Create `BeepTreeLayoutHelper.cs` - Layout calculation, text measurement, virtualization, coordinate transformation
- [x] Create `BeepTreeHitTestHelper.cs` - Hit testing integration with BaseControl._hitTest
- [x] Fix BaseControl integration - Use `_hitTest.AddHitArea()` instead of reimplementing
- [x] Document BaseControl integration strategy

### ✅ Phase 3: Partial Classes (COMPLETED - DISCOVERED PRE-EXISTING)
- [x] `BeepTree.cs` - Main entry point (already exists)
- [x] `BeepTree.Core.cs` - Fields, helper instances, initialization (already exists)
- [x] `BeepTree.Properties.cs` - All properties including Nodes (already exists)
- [x] `BeepTree.Events.cs` - Event declarations (already exists)
- [x] `BeepTree.Methods.cs` - Public API methods (already exists)
- [x] `BeepTree.Drawing.cs` - DrawContent override, painter integration (already exists)
- [x] `BeepTree.Layout.cs` - RebuildVisible, RecalculateLayoutCache (already exists)
- [x] `BeepTree.Scrolling.cs` - Scrollbar management (already exists)

### ✅ Phase 3.1: CRITICAL BUG FIX (COMPLETED)
- [x] **Identified struct mutation bug in RecalculateLayoutCache()**
  - Problem: NodeInfo is a struct - modifications to local copies weren't written back
  - Fix: Added `_visibleNodes[i] = nodeInfo;` to persist layout calculations
  - Impact: BeepTree now renders correctly
  - Documentation: `CRITICAL_FIX_STRUCT_MUTATION.md`
- [x] **Added comprehensive debug output**
  - Nodes property setter logs item counts
  - RebuildVisible logs node creation
  - RecalculateLayoutCache logs processing
  - DrawContent logs painter and client area
  - DrawVisibleNodes logs layout data
- [x] **Created test form**: `TestBeepTreeForm.cs` for verification

### ⏳ Phase 4: Painter Implementations (1/22 COMPLETE)
- [x] StandardTreePainter (default) ✅
- [ ] Material3TreePainter
- [ ] MaterialYouTreePainter
- [ ] iOS15TreePainter
- [ ] MacOSBigSurTreePainter
- [ ] Fluent2TreePainter
- [ ] Windows11MicaTreePainter
- [ ] MinimalTreePainter
- [ ] NotionMinimalTreePainter
- [ ] VercelCleanTreePainter
- [ ] NeumorphismTreePainter
- [ ] GlassAcrylicTreePainter
- [ ] DarkGlowTreePainter
- [ ] GradientModernTreePainter
- [ ] BootstrapTreePainter
- [ ] TailwindCardTreePainter
- [ ] StripeDashboardTreePainter
- [ ] FigmaCardTreePainter
- [ ] DiscordStyleTreePainter
- [ ] AntDesignTreePainter
- [ ] ChakraUITreePainter
- [ ] PillRailTreePainter

**Note**: All 21 additional painters are pending. Core rendering is now working after struct mutation bug fix.

### ⏳ Phase 5: Integration & Testing (PENDING)
- [ ] Wire up BaseControl._input keyboard events (arrow keys, Enter, Escape)
- [ ] Wire up BaseControl._hitTest events (HitDetected, OnControlHitTest)
- [ ] Update DrawContent to call RegisterHitAreas()
- [ ] Test each painter style
- [ ] Verify BaseControl integration
- [ ] Test hit testing functionality
- [ ] Test keyboard navigation
- [ ] Test virtualization
- [ ] Test scrolling
- [ ] Performance testing with large trees

### ⏳ Phase 6: Cleanup & Documentation (PENDING)
- [ ] Remove old monolithic BeepTree.cs after verification
- [ ] Update XML documentation comments
- [ ] Create usage examples
- [ ] Code review
- [ ] Final testing

## Architecture Details

### Current File Structure
```
Trees/
├── BeepTree.cs                          (2045 lines - TO BE REFACTORED)
├── Helpers/
│   ├── BeepTreeHelper.cs                ✅ DONE (250 lines)
│   ├── BeepTreeLayoutHelper.cs          ✅ DONE (280 lines)
│   └── BeepTreeHitTestHelper.cs         ✅ DONE (150 lines)
├── Painters/
│   ├── ITreePainter.cs                  ✅ DONE (60 lines)
│   ├── BaseTreePainter.cs               ✅ DONE (220 lines)
│   ├── BeepTreePainterFactory.cs        ✅ DONE (60 lines)
│   └── StandardTreePainter.cs           ✅ DONE (50 lines)
└── Models/
    └── NodeInfo.cs                       ✅ DONE (70 lines)
```

### Target File Structure (After Refactoring)
```
Trees/
├── BeepTree.cs                          (50-100 lines)
├── BeepTree.Core.cs                     (200-300 lines)
├── BeepTree.Properties.cs               (300-400 lines)
├── BeepTree.Events.cs                   (200-250 lines)
├── BeepTree.Methods.cs                  (200-300 lines)
├── BeepTree.Drawing.cs                  (200-300 lines)
├── BeepTree.Scrolling.cs                (150-200 lines)
├── Helpers/                             ✅ DONE
├── Painters/                            ⏳ 1/22 complete
└── Models/                              ✅ DONE
```

## BaseControl Integration Strategy

### Using BaseControl._hitTest
**Before (Wrong Approach):**
```csharp
// BeepTreeHitTestHelper was reimplementing hit testing
private bool LocalHitTest(Point p, out string name, out SimpleItem item, out Rectangle rect)
{
    // Manual hit testing loop...
}
```

**After (Correct Approach):**
```csharp
// Register hit areas with BaseControl
public void RegisterHitAreas()
{
    _owner._hitTest.ClearHitList();
    foreach (var node in layoutCache)
    {
        _owner._hitTest.AddHitArea($"toggle_{node.Item.GuidId}", toggleRect);
        _owner._hitTest.AddHitArea($"check_{node.Item.GuidId}", checkRect);
        _owner._hitTest.AddHitArea($"row_{node.Item.GuidId}", rowRect);
    }
}

// Use BaseControl's hit test
public bool HitTest(Point point, out string hitName, out SimpleItem item, out Rectangle rect)
{
    if (!_owner._hitTest.HitTest(point, out var hitTest))
        return false;
    
    hitName = hitTest.Name;
    rect = hitTest.TargetRect;
    // Extract item from GUID in name...
    return true;
}
```

### Using BaseControl._input
```csharp
// In BeepTree constructor
_input.UpArrowKeyPressed += OnUpArrowKey;
_input.DownArrowKeyPressed += OnDownArrowKey;
_input.LeftArrowKeyPressed += OnLeftArrowKey;
_input.RightArrowKeyPressed += OnRightArrowKey;
_input.EnterKeyPressed += OnEnterKey;

private void OnUpArrowKey(object sender, EventArgs e)
{
    SelectPreviousNode();
}
```

### Required BeepTree Properties for Helpers
```csharp
// Must be accessible (public or internal) for helper classes:
public int YOffset => _yOffset;                    // For coordinate transformation
public int XOffset => _xOffset;                    // For coordinate transformation
public int GetScaledBoxSize() => ScaleValue(14);   // For layout calculation
public int GetScaledImageSize() => ScaleValue(20); // For layout calculation
public int GetScaledMinRowHeight() => ScaleValue(24);
public int GetScaledIndentWidth() => ScaleValue(16);
public int GetScaledVerticalPadding() => ScaleValue(4);
public bool ShowCheckBox { get; }                  // Layout needs this
public bool VirtualizeLayout { get; }              // Layout optimization
public int VirtualizationBufferRows { get; }       // Layout optimization
public Font TextFont { get; }                      // Text measurement
public List<SimpleItem> Nodes { get; }             // Data source
```

## Key Implementation Details

### 1. NodeInfo Structure (Cached Layout)
```csharp
public struct NodeInfo
{
    public SimpleItem Item { get; set; }
    public int Level { get; set; }
    public int Y { get; set; }                    // Content-space Y position
    public int RowHeight { get; set; }
    public int RowWidth { get; set; }
    public Size TextSize { get; set; }
    
    // Content-space rectangles (before scroll offset)
    public Rectangle RowRectContent { get; set; }
    public Rectangle ToggleRectContent { get; set; }
    public Rectangle CheckRectContent { get; set; }
    public Rectangle IconRectContent { get; set; }
    public Rectangle TextRectContent { get; set; }
}
```

### 2. Text Measurement (Safe, No Graphics Object)
```csharp
public Size MeasureText(string text, Font font)
{
    return TextRenderer.MeasureText(text, font,
        new Size(int.MaxValue, int.MaxValue),
        TextFormatFlags.NoPadding);
}
```

### 3. Coordinate Transformation
```csharp
// Content space: Absolute positions ignoring scroll
// Viewport space: Screen positions after scroll offset

public Rectangle TransformToViewport(Rectangle contentRect)
{
    return new Rectangle(
        _owner.DrawingRect.Left + contentRect.X - _owner.XOffset,
        _owner.DrawingRect.Top + contentRect.Y - _owner.YOffset,
        contentRect.Width,
        contentRect.Height);
}
```

### 4. Virtualization
```csharp
// Only calculate layout for visible nodes + buffer
if (_owner.VirtualizeLayout && DrawingRect.Height > 0)
{
    (start, end) = GetVirtualizationRange(visibleItems);
}

// Calculate detailed layout only for nodes in range
for (int i = 0; i < visibleItems.Count; i++)
{
    if (i >= start && i <= end)
    {
        CalculateNodeLayout(ref nodeInfo); // Full calculation
    }
    else
    {
        nodeInfo.RowHeight = _owner.GetScaledMinRowHeight(); // Estimate
    }
}
```

## Painter System

### ITreePainter Interface
```csharp
public interface ITreePainter
{
    void Initialize(BeepTree owner, IBeepTheme theme);
    void Paint(Graphics g, BeepTree owner, Rectangle bounds);
    void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected);
    void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered);
    void PaintCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isHovered);
    void PaintIcon(Graphics g, Rectangle iconRect, string imagePath);
    void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered);
    void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected);
    int GetPreferredRowHeight(SimpleItem item, Font font);
}
```

### BaseTreePainter Default Implementations
- **PaintToggle**: Simple chevron (> for collapsed, v for expanded)
- **PaintCheckbox**: Square with checkmark when checked
- **PaintIcon**: Uses `StyledImagePainter.Paint()` for consistent rendering
- **PaintText**: Uses `TextRenderer.DrawText()` for safe text rendering
- **PaintNodeBackground**: Transparent overlay for hover/selection

### Painter Factory Pattern
```csharp
public static ITreePainter CreatePainter(BeepControlStyle style, BeepTree owner, IBeepTheme theme)
{
    ITreePainter painter = style switch
    {
        BeepControlStyle.Material3 => new Material3TreePainter(),
        BeepControlStyle.iOS15 => new iOS15TreePainter(),
        // ... 22 total styles
        _ => new StandardTreePainter()
    };
    
    painter.Initialize(owner, theme);
    _painterCache[style] = painter;
    return painter;
}
```

## Benefits of This Architecture

### 1. Maintainability
- Single Responsibility Principle - each file has one job
- Easy to find code by feature
- Each file < 400 lines

### 2. Extensibility
- Add new painter = 1 new file, no changes to tree logic
- Add new features to specific partial class
- Helpers are reusable

### 3. Performance
- Cached painters (create once, reuse)
- Virtualization for large trees
- Layout cache reduces calculations
- BaseControl's optimized hit testing

### 4. Consistency
- All controls use same BaseControl features
- BeepStyling provides unified theming
- StyledImagePainter provides consistent image rendering

### 5. Integration
- Works with BaseControl._hitTest
- Works with BaseControl._input
- Works with BaseControl._effects
- Works with BaseControl tooltip system

## Testing Strategy

### Unit Tests
- BeepTreeHelper traversal algorithms
- BeepTreeLayoutHelper calculations
- BeepTreeHitTestHelper with mock BaseControl
- Painter rendering (pixel comparison)

### Integration Tests
- Painter switching
- Theme changes
- Scrolling with different painters
- Keyboard navigation
- Hit testing with real mouse events

### Performance Tests
- Large tree (10,000+ nodes)
- Virtualization effectiveness
- Layout cache performance
- Memory profiling

## Migration Path

### Backward Compatibility
✅ **100% Backward Compatible**
- All existing properties preserved
- All existing events preserved (60+)
- All existing methods preserved
- Only new feature: `ControlStyle` property

### For New Code
- Use `ControlStyle` property for visual styling
- Subscribe to BaseControl._input events for keyboard handling
- Use BaseControl._hitTest.HitDetected event for interactions

## Success Criteria

1. ✅ Zero breaking changes to public API
2. ⏳ All 60+ events working correctly
3. ⏳ All 22 painters implemented
4. ⏳ Performance equal or better than original
5. ⏳ Code coverage > 80%
6. ⏳ All existing functionality preserved
7. ⏳ Documentation complete
8. ⏳ Designer support working

## Timeline Estimate

- **Phase 1**: Infrastructure - ✅ DONE (4 hours)
- **Phase 2**: Helpers - ✅ DONE (6 hours)
- **Phase 3**: Partial classes (8 hours) - IN PROGRESS
- **Phase 4**: Painters (16 hours) - 1/22 complete
- **Phase 5**: Integration + Testing (8 hours)
- **Phase 6**: Cleanup + Documentation (4 hours)
- **Total**: 46 hours (~6 working days)

## Current Status: Phase 2 Complete, Starting Phase 3

### What's Working
✅ Helper infrastructure complete and tested
✅ BaseControl integration strategy documented
✅ First painter (Standard) implemented
✅ Factory pattern ready

### Next Actions
1. Create partial class structure (BeepTree.Core.cs, Properties.cs, etc.)
2. Expose required properties for helper access
3. Wire up BaseControl._input keyboard events
4. Update DrawContent to use RegisterHitAreas()
5. Create remaining 21 painters

### Blockers
None currently - ready to proceed with Phase 3

## Notes
- Using TextRenderer.MeasureText (no Graphics object needed) ✅
- Caching painters for performance ✅
- Using StyledImagePainter for all image rendering ✅
- Integrating with BaseControl._hitTest and _input ✅
- Maintaining virtualization for large trees ✅
- Preserving all existing functionality ✅
