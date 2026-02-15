# BeepGridPro - Advanced Grid Control

## Overview

BeepGridPro is a high-performance, fully customizable data grid control for WinForms applications. It follows a clean architecture pattern with separation of concerns through helper-based design, featuring complete painter systems for both navigation bars and column headers, inspired by modern JavaScript frameworks while maintaining WinForms best practices.

## Architecture

### Core Design Principles

1. **Helper-Based Architecture**: Functionality is divided into focused helper classes, each responsible for a specific aspect of the grid
2. **Painter Pattern**: Visual rendering is delegated to specialized painter classes for maximum flexibility
3. **Layout Presets**: Pre-configured layout styles that can be easily applied and customized
4. **Theme Integration**: Deep integration with BeepThemesManager for consistent application styling
5. **Performance Optimization**: Double buffering, WM_SETREDRAW optimization, and smart invalidation

### Component Hierarchy

```
BeepGridPro (Main Control)
├── Helpers/ (Business Logic & State Management)
│   ├── GridLayoutHelper - Layout calculations and positioning
│   ├── GridDataHelper - Data binding and management
│   ├── GridRenderHelper - Main rendering coordinator
│   ├── GridSelectionHelper - Selection state management
│   ├── GridInputHelper - Mouse/keyboard input handling
│   ├── GridScrollHelper - Scroll position management
│   ├── GridScrollBarsHelper - Custom scrollbar rendering
│   ├── GridSortFilterHelper - Sorting and filtering logic
│   ├── GridEditHelper - Cell editing functionality
│   ├── GridThemeHelper - Theme application
│   ├── GridNavigatorHelper - Data navigation
│   ├── GridNavigationPainterHelper - Navigation bar rendering
│   ├── GridColumnHeadersPainterHelper - Column header rendering
│   ├── GridSizingHelper - Auto-sizing logic
│   └── GridDialogHelper - Dialog management
│
├── Painters/ (Visual Rendering)
│   ├── INavigationPainter - Navigation bar interface
│   ├── IPaintGridHeader - Header painter interface
│   ├── BaseNavigationPainter - Common navigation functionality
│   ├── MaterialNavigationPainter - Material Design style
│   ├── BootstrapNavigationPainter - Bootstrap style
│   ├── TailwindNavigationPainter - Tailwind CSS style
│   ├── AGGridNavigationPainter - AG Grid style
│   ├── DataTablesNavigationPainter - DataTables style
│   └── ... (12 navigation painters total)
│
├── Layouts/ (Layout Presets)
│   ├── IGridLayoutPreset - Layout preset interface
│   ├── DefaultTableLayoutHelper - Default layout
│   ├── MaterialHeaderTableLayoutHelper - Material style
│   ├── StripedTableLayoutHelper - Striped rows
│   ├── CardTableLayoutHelper - Card-based layout
│   └── ... (11 layout presets total)
│
├── Models/ (Data Models)
│   └── (Currently empty - models defined in parent Controls.Models)
│
├── Adapters/ (Data Adapters)
│   └── (For future data source adapters)
│
└── Filters/ (Filter Logic)
    └── (Filter-specific implementations)
```

## Folder Structure

### /Helpers
Contains all helper classes that manage grid functionality. Each helper is responsible for a specific concern and interacts with other helpers through the main BeepGridPro instance.

**Key Helpers:**
- **GridLayoutHelper**: Calculates all rectangles for headers, cells, navigation, and scrollbars
- **GridRenderHelper**: Coordinates all drawing operations and manages the rendering pipeline
- **GridNavigationPainterHelper**: Handles navigation bar rendering using painter pattern
- **GridColumnHeadersPainterHelper**: Handles column header rendering with sort/filter indicators

### /Painters
Contains painter implementations for different visual styles, using Strategy pattern for both navigation bars and column headers.

**Navigation Painters (12 styles):**
- Material, Bootstrap, Tailwind, Fluent, AG Grid, DataTables, Ant Design, Telerik, Compact, Minimal, Card, Standard

**Header Painters (12 styles):**
- MaterialHeaderPainter - Material Design with gradients and elevation
- BootstrapHeaderPainter - Bootstrap-inspired styling
- FluentHeaderPainter - Fluent Design (Microsoft) style
- AGGridHeaderPainter - AG Grid inspired modern styling
- DataTablesHeaderPainter - DataTables (jQuery) style
- AntDesignHeaderPainter - Ant Design clean professional look
- TelerikHeaderPainter - Telerik/Kendo UI inspired
- CompactHeaderPainter - Space-saving compact design
- MinimalHeaderPainter - Minimal icon-focused styling
- CardHeaderPainter - Card-based modern appearance
- TailwindHeaderPainter - Tailwind CSS inspired flat design
- StandardHeaderPainter - Classic Windows Forms style

**Factory Classes:**
- HeaderPainterFactory - Creates header painters based on navigationStyle
- NavigationPainterFactory - Creates navigation painters based on navigationStyle

**Future:**
- Cell painters for custom cell rendering

### /Layouts
Contains layout preset implementations that define structural and spacing characteristics of the grid. Layouts control spacing, stripes, headers, and other non-color aspects.

**Available Presets:**
- Default, Clean, Dense, Striped, Borderless, HeaderBold, MaterialHeader, Card, ComparisonTable, MatrixSimple, MatrixStriped, PricingTable

### /Models
Reserved for grid-specific data models and structures. Currently, models are defined in the parent `TheTechIdea.Beep.Winform.Controls.Models` namespace.

### /Adapters
Reserved for data source adapters to handle different data binding scenarios.

### /Filters
Reserved for filter-specific implementations and filter UI components.

## Key Features

### 1. Data Binding
- Supports `IBindingList`, `BindingList<T>`, `List<T>`, `DataTable`, and more
- Auto-column generation from data structure
- Complex data binding with `DataMember` support
- UnitOfWork pattern integration for enterprise scenarios

### 2. Visual Styles
- **12 Header Styles**: Full painter system for column headers matching 12 framework designs
- **12 Navigation Styles**: Material, Bootstrap, Tailwind, Fluent, AG Grid, DataTables, Ant Design, Telerik, Compact, Minimal, Card, Standard
- **10 Grid Styles**: Default, Clean, Bootstrap, Material, Flat, Compact, Corporate, Minimal, Card, Borderless
- **12 Layout Presets**: Structural presets with automatic header and navigation painter coordination
- **Coordinated Styling**: Layout presets automatically configure matching header and navigation painters

### 3. Performance Features
- Double buffering with WM_SETREDRAW optimization
- Visible row virtualization
- Smart invalidation (only redraw changed regions)
- BeginUpdate/EndUpdate for bulk operations
- Custom scrollbar rendering (no WinForms ScrollBar overhead)

### 4. Interactive Features
- Multi-row selection with checkboxes
- In-place cell editing
- Column resizing
- Sticky/frozen columns
- Sorting and filtering
- Top filter panel with title (controlled by `ShowTopFilterPanel` and `GridTitle`)
- Excel-like filter dropdowns
- Keyboard navigation (arrows, Page Up/Down, Home/End)

### 5. Navigation
- Painter-based navigation bars with 12 different styles
- Legacy button-based navigation for compatibility
- First/Previous/Next/Last navigation
- CRUD operations (Add, Delete, Save, Cancel)
- Page information display
- Record counter

## How It Works

### Rendering Pipeline

```
DrawContent(Graphics g)
    ├── UpdateDrawingRect() - Calculate available drawing area
    ├── Layout.EnsureCalculated() - Calculate all layout rectangles
    │   ├── Calculate HeaderRect
    │   ├── Calculate RowsRect
    │   ├── Calculate NavigatorRect
    │   └── LayoutCells() - Calculate all cell rectangles
    │
    ├── Render.Draw(g) - Main drawing coordinator
    │   ├── Draw background
    │   ├── Draw rows (visible only)
    │   │   ├── For each visible row
    │   │   │   ├── Draw row background (with stripes/hover effects)
    │   │   │   ├── Draw checkbox (if enabled)
    │   │   │   └── For each visible cell
    │   │   │       ├── Draw cell background
    │   │   │       └── Draw cell content (text/controls)
    │   │   └── Draw selection highlight
    │   │
    │   ├── GridColumnHeadersPainterHelper.DrawColumnHeaders(g)
    │   │   ├── Get header painter from HeaderPainterFactory (based on NavigationStyle)
    │   │   ├── Draw select-all checkbox
    │   │   ├── Call painter.PaintHeaders(g, headerRect, grid, theme)
    │   │   │   ├── For each visible column
    │   │   │   │   ├── Call painter.PaintHeaderCell(g, cellRect, column, index, grid, theme)
    │   │   │   │   ├── Draw header background (with hover/elevation effects)
    │   │   │   │   ├── Draw header text (styled per painter)
    │   │   │   │   ├── Draw sort indicator (if sorted)
    │   │   │   │   └── Draw filter icon (on hover)
    │   │   │   └── Draw sticky column separator
    │   │   └── painter.RegisterHeaderHitAreas(grid)
    │   │
    │   └── GridNavigationPainterHelper.DrawNavigatorArea(g)
    │       ├── If UsePainterNavigation
    │       │   ├── Get/Create navigation painter based on NavigationStyle
    │       │   └── Painter.PaintNavigation(g, navRect, grid, theme)
    │       └── Else (Legacy mode)
    │           ├── Layout buttons
    │           ├── Draw CRUD buttons
    │           ├── Draw navigation buttons
    │           └── Draw page info
    │
    └── ScrollBars.DrawScrollBars(g) - Custom scrollbars
        ├── Calculate thumb positions
        ├── Draw vertical scrollbar
        └── Draw horizontal scrollbar
```

### Layout Calculation Flow

```
GridLayoutHelper.Recalculate()
    ├── Get DrawingRect from BeepGridPro
    ├── Account for BorderThickness
    │
    ├── Calculate HeaderRect (if ShowColumnHeaders)
    │   └── Position: top of DrawingRect, height: ColumnHeaderHeight
    │
    ├── Calculate NavigatorRect (if ShowNavigator)
    │   ├── Get recommended height from NavigationPainterHelper
    │   └── Position: bottom of DrawingRect
    │
    ├── Calculate RowsRect
    │   └── Position: between HeaderRect and NavigatorRect
    │
    └── LayoutCells() - Calculate individual cell rectangles
        ├── Calculate sticky columns width
        ├── For each column
        │   ├── If sticky: position from left edge
        │   └── If not sticky: position with horizontal scroll offset
        │
        ├── Calculate SelectAllCheckRect (if ShowCheckBox)
        │
        └── For each visible row
            ├── Calculate row vertical position (accounting for scroll)
            ├── For each cell in row
            │   └── Create cell rectangle from column X and row Y
            └── Calculate row checkbox rectangle
```

### Input Handling Flow

```
User Input (Mouse/Keyboard)
    ├── GridInputHelper.HandleMouseDown(e)
    │   ├── Check if clicked on header
    │   │   ├── Check sort icon area → Toggle sort
    │   │   ├── Check filter icon area → Show filter menu
    │   │   ├── Check select-all checkbox → Toggle all rows
    │   │   └── Check column border → Start resize
    │   │
    │   ├── Check if clicked on cell
    │   │   ├── Check checkbox area → Toggle row selection
    │   │   └── Select cell → GridSelectionHelper.SelectCell()
    │   │
    │   ├── Check if clicked on navigator
    │   │   ├── Check hit areas registered by painters
    │   │   └── Execute action (First/Prev/Next/Last/Add/Delete/Save/Cancel)
    │   │
    │   └── Check if clicked on scrollbar
    │       └── GridScrollBarsHelper.HandleMouseDown()
    │
    ├── GridInputHelper.HandleMouseMove(e)
    │   ├── Track hover state for headers
    │   ├── Handle column resize dragging
    │   └── Handle scrollbar dragging
    │
    ├── GridInputHelper.HandleKeyDown(e)
    │   ├── Arrow keys → Navigate cells
    │   ├── Page Up/Down → Scroll pages
    │   ├── Home/End → Jump to first/last
    │   ├── Enter → Start cell editing
    │   └── Space → Toggle checkbox selection
    │
    └── GridScrollBarsHelper.HandleMouseWheel(e)
        ├── Calculate scroll delta
        ├── Update GridScrollHelper.VerticalOffset
        └── Invalidate to trigger redraw
```

### Style Application Flow

```
ApplyGridStyle(BeepGridStyle style)
    ├── Reset to theme defaults
    │
    ├── Based on GridStyle enum:
    │   ├── Default → Standard settings
    │   ├── Material → Elevation, gradient headers, 28px rows
    │   ├── Bootstrap → Striped rows, clean borders
    │   ├── Compact → 20px rows, 1px padding
    │   ├── Card → Elevation, no grid lines, card appearance
    │   └── ... (10 styles total)
    │
    ├── Update Render helper properties:
    │   ├── ShowGridLines
    │   ├── ShowRowStripes
    │   ├── GridLineStyle
    │   ├── UseHeaderGradient
    │   ├── UseElevation
    │   ├── CardStyle
    │   └── HeaderCellPadding
    │
    ├── Layout.Recalculate() → Update dimensions
    └── Invalidate() → Trigger redraw

ApplyLayoutPreset(IGridLayoutPreset preset)
    ├── Get preset implementation (DefaultTableLayoutHelper, etc.)
    ├── preset.Apply(grid)
    │   ├── ConfigureDimensions(grid)
    │   ├── ConfigurePainters(grid)
    │   │   ├── Get header painter from preset.GetHeaderPainter()
    │   │   ├── Get navigation painter from preset.GetNavigationPainter()
    │   │   └── Store painter references in grid.Tag for use during rendering
    │   ├── ConfigureHeights(grid)
    │   │   ├── grid.ColumnHeaderHeight = painter.CalculateHeaderHeight(grid)
    │   │   └── grid.Layout.NavigatorHeight = navPainter.RecommendedHeight
    │   ├── ConfigureVisualProperties(grid)
    │   ├── LayoutCommon.ApplyAlignmentHeuristics(grid)
    │   └── CustomConfiguration(grid)
    └── Invalidate() → Trigger redraw

NavigationStyle = navigationStyle.Material
    ├── GridNavigationPainterHelper.NavigationStyle = value
    ├── Clear current painter instance
    ├── NavigationPainterFactory.CreatePainter(style)
    │   └── Return new MaterialNavigationPainter()
    ├── Update Layout.NavigatorHeight from painter.RecommendedHeight
    ├── Layout.Recalculate()
    └── Invalidate()
```

## Usage Examples

### Basic Usage
```csharp
var grid = new BeepGridPro();
grid.DataSource = myDataList;
grid.Theme = BeepTheme.MaterialDark;
grid.GridStyle = BeepGridStyle.Material;
grid.NavigationStyle = navigationStyle.Material;
```

### Apply Layout Preset
```csharp
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
// or
grid.ApplyLayoutPreset(new MaterialHeaderTableLayoutHelper());
```

### Custom Styling
```csharp
grid.RowHeight = 32;
grid.ShowColumnHeaders = true;
grid.Render.ShowGridLines = true;
grid.Render.UseHeaderGradient = true;
grid.Render.ShowRowStripes = true;
```

### Data Operations
```csharp
// Bind to data
grid.DataSource = myBindingList;

// Get selected rows
var selected = grid.SelectedRows;

// Navigate
grid.MoveNext();
grid.MovePrevious();

// CRUD operations
grid.InsertNew();
grid.DeleteCurrent();
grid.Save();
```

## Extension Points

### Creating Custom Navigation Painter
1. Inherit from `BaseNavigationPainter`
2. Implement abstract methods
3. Register in `NavigationPainterFactory`
4. Add to `navigationStyle` enum

### Creating Custom Layout Preset
1. Implement `IGridLayoutPreset` interface
2. Implement `Apply(BeepGridPro grid)` method
3. Add to `GridLayoutPreset` enum
4. Add to `ApplyLayoutPreset` switch

### Creating Custom Cell Renderer
1. Implement `IBeepUIComponent`
2. Register in column configuration
3. Grid will use drawer pattern to render

## Performance Tips

1. **Use BeginUpdate/EndUpdate** for bulk operations
2. **Enable AutoSizeColumnsMode** only when needed
3. **Limit visible rows** using pagination
4. **Use sticky columns sparingly** (impacts scroll performance)
5. **Disable features** you don't need (ShowCheckBox, ShowNavigator, etc.)

## Dependencies

- **BaseControl**: Parent class providing theme integration and base functionality
- **BeepThemesManager**: Theme management and application
- **StyledImagePainter**: SVG and image rendering
- **BeepControls**: Various controls used for in-cell editing (BeepButton, BeepCheckBox, etc.)

## Future Enhancements

See ENHANCEMENT_PLAN.md for detailed roadmap including:
- Integration of column header painting into IGridLayoutPreset
- Integration of navigation painting into IGridLayoutPreset
- Header-specific painters per layout
- Enhanced layout customization API

## Related Documentation

- [Helpers/README.md](Helpers/README.md) - Detailed helper class documentation
- [Painters/README.md](Painters/README.md) - Painter pattern and implementations
- [Layouts/README.md](Layouts/README.md) - Layout preset system
- [BeepGridPro_README.md](BeepGridPro_README.md) - Main control documentation
- [ENHANCEMENT_PLAN.md](ENHANCEMENT_PLAN.md) - Future development roadmap
