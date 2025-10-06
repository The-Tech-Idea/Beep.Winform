# Plan: Header and Navigation Painters Architecture

## Overview
This document outlines the architecture for the BeepGridPro painter system, which separates visual rendering into three distinct layers:

1. **Header Painter** - Toolbar above column headers
2. **Column Headers** - Rendered by grid itself
3. **Navigation Painter** - Pagination/toolbar at bottom

## Current Architecture

### Layer Separation

```
???????????????????????????????????????????????????????
? [Header Painter - IPaintGridHeader]                 ?
? Toolbar: Title, Search, Filters, Actions            ?
? "User Management" [Search] [Sort?] [Filter?] [+Add] ?
???????????????????????????????????????????????????????
? [Column Headers - Grid Native Rendering]            ?
? ? Name     | Email      | Status    | Actions      ?
???????????????????????????????????????????????????????
? [Data Rows - Grid Native Rendering]                 ?
? ? John     | john@...   | Active    | ...          ?
? ? Jane     | jane@...   | Inactive  | ...          ?
???????????????????????????????????????????????????????
? [Navigation Painter - IPaintGridNavigation]         ?
? Show: 7?    << < 1 2 3 4 > >>    Jump to: [_]      ?
???????????????????????????????????????????????????????
```

## Interfaces

### IPaintGridHeader
```csharp
public interface IPaintGridHeader
{
    void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid);
    void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column, 
                        int columnIndex, BeepGridPro grid);
    void RegisterHeaderHitAreas(BeepGridPro grid);
    string StyleName { get; }
}
```

**Responsibilities:**
- Paint toolbar section ABOVE column headers
- Does NOT paint individual column headers
- Registers hit areas for toolbar buttons
- Examples: Search box, filter dropdown, sort dropdown, action buttons

### IPaintGridNavigation
```csharp
public interface IPaintGridNavigation
{
    void PaintNavigation(Graphics g, Rectangle navigationRect, BeepGridPro grid);
    void RegisterNavigationHitAreas(BeepGridPro grid);
    void UpdatePageInfo(int currentPage, int totalPages, int totalRecords);
    int GetPreferredHeight();
    string StyleName { get; }
}
```

**Responsibilities:**
- Paint navigation/pagination toolbar at bottom
- Handle page controls, CRUD buttons, navigation
- Register hit areas for all navigation controls

## Implemented Painters

### Header Painters

#### 1. DefaultGridHeaderPainter
**Purpose:** Standard data table toolbar  
**Features:**
- Title or selection count display
- Search box with icon
- Filter dropdown
- Sort by dropdown
- Export button
- Add/New button (highlighted)
- Contextual actions (Share, Edit, Delete) when items selected

**Use Cases:**
- User management screens
- Data tables with CRUD operations
- Admin panels
- List views with filtering

**Configuration:**
```csharp
grid.HeaderPainter = new DefaultGridHeaderPainter();
```

#### 2. MinimalGridHeaderPainter (To Be Created)
**Purpose:** Clean, minimal toolbar  
**Planned Features:**
- Title only
- Optional search
- No action buttons
- Minimalist design

#### 3. CompactGridHeaderPainter (To Be Created)
**Purpose:** Space-efficient toolbar  
**Planned Features:**
- Icon-only buttons
- Condensed layout
- Dropdown menus for actions

### Navigation Painters

#### 1. DefaultGridNavigationPainter
**Purpose:** Traditional CRUD and paging controls  
**Features:**
- CRUD buttons (Insert, Delete, Save, Cancel)
- Record navigation (First, Previous, Next, Last)
- Record counter display
- Query, Filter, Print utilities
- Page info label

**Layout:**
```
[Insert][Delete][Save][Cancel]  |<<|<| 1 - 10 |>|>>|  [Query][Filter][Print] Page 1 of 5
```

#### 2. ToolbarNavigationPainter
**Purpose:** Modern data table pagination  
**Features:**
- Page size selector ("Show: 7")
- Page number buttons (1, 2, 3, 4...)
- First/Previous/Next/Last navigation
- Jump to page input
- Active page highlighting

**Layout:**
```
Show: 7?    << < [1] 2 3 4 > >>    Jump to: [___]
```

#### 3. CompactNavigationPainter (To Be Created)
**Purpose:** Minimal pagination  
**Planned Features:**
- Previous/Next only
- Current page display
- No CRUD buttons

#### 4. MinimalNavigationPainter (To Be Created)
**Purpose:** Ultra-minimal  
**Planned Features:**
- Record counter only
- Optional prev/next

## Event System

### GridPainterEventManager
Centralized event handling for all painter interactions.

**Standard Events:**
```csharp
// Header Events
GridPainterEvents.ClickedSortIcon
GridPainterEvents.ClickedFilterIcon
GridPainterEvents.ClickedHeaderCell

// Navigation Events
GridPainterEvents.NavFirst
GridPainterEvents.NavPrevious
GridPainterEvents.NavNext
GridPainterEvents.NavLast
GridPainterEvents.NavInsert
GridPainterEvents.NavDelete
GridPainterEvents.NavSave
GridPainterEvents.NavCancel
GridPainterEvents.NavQuery
GridPainterEvents.NavFilter
GridPainterEvents.NavPrint
GridPainterEvents.NavPageSizeChanged
GridPainterEvents.NavPageJump
```

**Usage:**
```csharp
// Register custom handler
grid.PainterEvents.RegisterEvent(GridPainterEvents.NavInsert, (sender, args) =>
{
    ShowAddUserDialog();
});

// Trigger from painter
grid.PainterEvents.TriggerEvent(GridPainterEvents.NavInsert);
```

## Implementation Plan

### Phase 1: Core Infrastructure ✅ COMPLETE
- [x] Create `IPaintGridHeader` interface
- [x] Create `IPaintGridNavigation` interface
- [x] Create `GridPainterEventManager`
- [x] Define `GridPainterEvents` constants
- [x] Create `GridPainterEventArgs` class

### Phase 2: Default Implementations ✅ COMPLETE
- [x] Implement `DefaultGridHeaderPainter`
- [x] Implement `DefaultGridNavigationPainter`
- [x] Implement `ToolbarNavigationPainter`
- [x] Test event triggering and handling
- [x] Integrate with BeepStyling for background painting
- [x] Use ControlStyle property from BaseControl

### Phase 3: Additional Painter Styles 🔄 IN PROGRESS
- [ ] Create `MinimalGridHeaderPainter`
- [ ] Create `CompactGridHeaderPainter`
- [ ] Create `CompactNavigationPainter`
- [ ] Create `MinimalNavigationPainter`
- [x] Document BeepStyling integration
- [x] Create ModernPainterGuide.md

### Phase 4: Themed Painters ?? PLANNED
- [ ] Create `MaterialHeaderPainter`
- [ ] Create `MaterialNavigationPainter`
- [ ] Create `BootstrapHeaderPainter`
- [ ] Create `BootstrapNavigationPainter`
- [ ] Create `AntDesignHeaderPainter`
- [ ] Create `AntDesignNavigationPainter`

### Phase 5: Specialized Painters ?? PLANNED
- [ ] Create `CardLayoutHeaderPainter`
- [ ] Create `KanbanHeaderPainter`
- [ ] Create `CalendarHeaderPainter`
- [ ] Create `TimelineNavigationPainter`

### Phase 6: Integration ?? PLANNED
- [ ] Add painter properties to `BeepGridPro`
- [ ] Update `GridLayoutHelper` to use painters
- [ ] Update `GridRenderHelper` to delegate to painters
- [ ] Add designer support for painter selection
- [ ] Create painter factory for easy instantiation

### Phase 7: Documentation & Examples ?? PLANNED
- [ ] Complete API documentation
- [ ] Create painter cookbook with examples
- [ ] Add screenshots for each painter style
- [ ] Create video tutorials
- [ ] Write migration guide from old system

## Design Patterns

### Strategy Pattern
Painters implement the Strategy pattern, allowing different rendering algorithms to be swapped at runtime.

```csharp
// Change header style
grid.HeaderPainter = new MinimalGridHeaderPainter();

// Change navigation style
grid.NavigationPainter = new CompactNavigationPainter();

// Change visual style (affects all painters)
grid.ControlStyle = BeepControlStyle.Material3;
grid.UseThemeColors = true;
```

### Facade Pattern
BeepStyling acts as a facade, simplifying access to complex styling subsystems.

```csharp
// Instead of manually managing brushes, pens, paths...
BeepStyling.CurrentTheme = theme;
BeepStyling.UseThemeColors = grid.UseThemeColors;
BeepStyling.PaintStyleBackground(g, rect, grid.ControlStyle);
```

### Observer Pattern
Event system uses Observer pattern for loose coupling between painters and grid logic.

```csharp
// Painter triggers event
grid.PainterEvents.TriggerEvent("CustomAction", args);

// Grid or application observes
grid.PainterEvents.RegisterEvent("CustomAction", HandleCustomAction);
```

### Factory Pattern
Painter factory simplifies creation and configuration.

```csharp
var painter = PainterFactory.Create(PainterStyle.Material, grid.Theme);
grid.HeaderPainter = painter;
```

## Best Practices

### 1. Separation of Concerns
- **Header Painter:** ONLY paints toolbar above headers
- **Column Headers:** Handled by grid's native rendering
- **Navigation Painter:** ONLY paints bottom toolbar/pagination

### 2. Event-Driven Architecture
- All user interactions trigger events
- Painters don't directly call grid methods
- Grid subscribes to events and handles business logic

### 3. Theme Integration
- Always respect `BeepTheme` colors
- Use theme fonts and sizes
- Support custom colors when specified

### 4. Hit Area Management
- Register all interactive regions as hit areas
- Use descriptive names for hit areas
- Clean up hit areas on repaint

### 5. Stateless Painting
- Store state in grid, not painter
- Painters should be reusable across grids
- No side effects during painting

## Usage Examples

### Basic Setup
```csharp
var grid = new BeepGridPro
{
    Dock = DockStyle.Fill,
    Theme = "BusinessProfessional"
};

// Set painters
grid.HeaderPainter = new DefaultGridHeaderPainter();
grid.NavigationPainter = new ToolbarNavigationPainter();

// Handle events
grid.PainterEvents.RegisterEvent(GridPainterEvents.NavInsert, (s, e) =>
{
    ShowAddRecordDialog();
});
```

### Custom Event Handling
```csharp
// Override default sort behavior
grid.PainterEvents.RegisterEvent(GridPainterEvents.ClickedSortIcon, (s, e) =>
{
    var column = grid.Data.Columns[e.ColumnIndex];
    CustomSort(column);
    e.Handled = true; // Prevent default
});
```

### Dynamic Painter Switching
```csharp
// Switch to minimal mode for presentations
void EnterPresentationMode()
{
    grid.HeaderPainter = new MinimalGridHeaderPainter();
    grid.NavigationPainter = new CompactNavigationPainter();
    grid.Refresh();
}

// Return to full mode
void ExitPresentationMode()
{
    grid.HeaderPainter = new DefaultGridHeaderPainter();
    grid.NavigationPainter = new DefaultGridNavigationPainter();
    grid.Refresh();
}
```

## File Organization

```
GridX/
??? Painters/
?   ??? IPaintGridHeader.cs
?   ??? IPaintGridNavigation.cs
?   ??? GridPainterEventManager.cs
?   ?
?   ??? Header/
?   ?   ??? DefaultGridHeaderPainter.cs ?
?   ?   ??? MinimalGridHeaderPainter.cs
?   ?   ??? CompactGridHeaderPainter.cs
?   ?   ??? MaterialHeaderPainter.cs
?   ?   ??? BootstrapHeaderPainter.cs
?   ?
?   ??? Navigation/
?   ?   ??? DefaultGridNavigationPainter.cs ?
?   ?   ??? ToolbarNavigationPainter.cs ?
?   ?   ??? CompactNavigationPainter.cs
?   ?   ??? MinimalNavigationPainter.cs
?   ?   ??? MaterialNavigationPainter.cs
?   ?
?   ??? Factory/
?       ??? PainterFactory.cs
?
??? Docs/
    ??? PainterSystem.md ?
    ??? PlanHeaderNavigationPainters.md ?
```

## Testing Strategy

### Unit Tests
- Test each painter in isolation
- Mock `BeepGridPro` for testing
- Verify hit area registration
- Test event triggering

### Integration Tests
- Test painter + grid interaction
- Verify event flow
- Test theme integration
- Test painter switching

### Visual Tests
- Screenshot comparisons
- Manual visual inspection
- Accessibility testing
- Responsive layout testing

## Performance Considerations

### Painting Optimization
- Cache frequently used brushes/pens
- Minimize GDI+ object creation
- Use clipping regions effectively
- Batch similar drawing operations

### Event Optimization
- Debounce rapid fire events
- Use event pooling for frequently triggered events
- Avoid heavy operations in event handlers

### Hit Area Optimization
- Clear and rebuild hit areas only when needed
- Use spatial indexing for large numbers of hit areas
- Coalesce overlapping hit areas

## Migration Guide

### From Old System
```csharp
// Old way (GridRenderHelper does everything)
grid.Render.DrawHeaders(g);
grid.Render.DrawNavigation(g);

// New way (Separate painters)
grid.HeaderPainter.PaintHeaders(g, headerRect, grid);
grid.NavigationPainter.PaintNavigation(g, navRect, grid);
```

### Event Migration
```csharp
// Old way (Direct method calls)
btnInsert.Click += (s, e) => grid.InsertNew();

// New way (Event system)
grid.PainterEvents.RegisterEvent(GridPainterEvents.NavInsert, (s, e) =>
{
    grid.InsertNew();
});
```

## Future Enhancements

### Planned Features
- [ ] Painter presets (combinations of header + navigation)
- [ ] Painter animation support
- [ ] Custom painter templates
- [ ] Painter state persistence
- [ ] Painter marketplace/sharing
- [ ] AI-generated painter suggestions
- [ ] Responsive painter layouts
- [ ] Touch-optimized painters for tablets

### Experimental Features
- [ ] Voice command integration
- [ ] Gesture support
- [ ] VR/AR painter rendering
- [ ] Real-time collaborative painters

## Resources

### Related Documentation
- [Painter System Overview](PainterSystem.md)
- [BeepGridPro Styling](Styling.md)
- [Event System](Events.md)
- [Theme Management](../ThemeManagement/README.md)

### Code Examples
- [Pricing Table Example](../Layouts/docs/PricingTableExample.cs)
- [Custom Painter Example](examples/CustomPainterExample.cs) (TBD)

### External References
- [GDI+ Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/graphics-overview-windows-forms)
- [WinForms Custom Painting](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-manually-render-buffered-graphics)

---

**Status:** Phase 2 Complete, Phase 3 In Progress  
**Last Updated:** 2024-01-XX  
**Maintainer:** BeepGridPro Team  
**Version:** 1.0
