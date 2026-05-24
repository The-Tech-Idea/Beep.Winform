# Phase 3: Runtime Window Positioning — Complete Implementation

## Overview

**Phase 3** implements the runtime layer that bridges calculated layout rectangles to actual MDI child window positioning. It manages panel lifecycle, renders tab chrome and splitters, handles mouse interactions, and coordinates window Z-order and visibility.

## Architecture

### Core Components

#### 1. **MdiPanelPositioner** (`Runtime/MdiPanelPositioner.cs`)
Maps abstract layout rectangles from the layout controller to concrete MDI child windows.

**Key Methods:**
- `ApplyLayout(Dictionary<string, Rectangle>, List<DockPanel>)` — Apply calculated layout to windows
- `CreateWindowForPanel(DockPanel, Rectangle)` — Create MDI child window using `WM_MDICREATE`
- `RepositionWindow(string, Rectangle)` — Reposition existing window via `SetWindowPos`
- `ShowWindow(string)`, `HideWindow(string)` — Manage window visibility
- `ActivatePanel(string)` — Activate a panel using `WM_MDIACTIVATE`
- `DestroyPanel(string)` — Destroy window and remove from tracking
- `UpdateZOrder(List<string>)` — Manage panel stacking order
- `GetDiagnostics()` — Retrieve window state info

**State Tracking:**
- `_panelToWindowMap`: Panel key → HWND mapping
- `_windowToPanelMap`: HWND → Panel key reverse mapping
- `_windowStates`: Per-panel window state (visibility, active, bounds)

#### 2. **WindowChrome** (`Runtime/WindowChrome.cs`)
Renders and manages tab strip UI, window decorations, splitters, and docking guides.

**Key Methods:**
- `DrawTabStrip(Graphics, Rectangle, DockGroup)` — Render tab strip for a group
- `DrawTab(Graphics, Rectangle, DockPanel, bool)` — Render individual tab
- `DrawChrome(Graphics, Rectangle)` — Draw window chrome/title bar
- `DrawSplitter(Graphics, Rectangle, bool)` — Draw splitter bar with grip
- `DrawDockingGuide(Graphics, Rectangle, DockPosition)` — Preview docking position
- `HitTestTab(Point)` — Find tab at point
- `HitTestCloseButton(Point, Rectangle)` — Find close button at point
- `InvalidateCache()`, `UpdateMetrics()` — Cache management
- `GetDiagnostics()` — Retrieve rendering state

**Rendering Strategy:**
- Tab backgrounds vary by active state
- Close buttons drawn with feedback on hover
- Splitter bars have grip indicators
- Docking guides use color overlay or crosshair patterns

#### 3. **PanelWindowManager** (`Runtime/PanelWindowManager.cs`)
Coordinates panel lifecycle and maintains panel activation state.

**Key Methods:**
- `CreatePanel(DockPanel, IntPtr)` — Register new panel for management
- `DestroyPanel(string)` — Unregister and destroy panel window
- `ActivatePanel(string)` — Bring panel to front and set as active
- `HidePanel(string)`, `ShowPanel(string)` — Toggle visibility
- `HandleTabClick(string)` — Process tab click events
- `HandleTabCloseClick(string)` — Process close button click
- `GetDiagnostics()` — Retrieve manager state

**Event Hooks:**
- `PanelCreated` — Panel window created
- `PanelDestroyed` — Panel window destroyed
- `PanelActivated` — Panel activated
- `PanelHidden` — Panel hidden

**State Types:**
- `WindowDescriptor` — Metadata for managed panel
- `WindowLifecycleState` — Created, Visible, Hidden, Destroyed
- `PanelEventArgs` — Event arguments

#### 4. **PositioningUtilities** (`Runtime/PositioningUtilities.cs`)
Utility methods for splitter interaction, drag operations, and size calculations.

**Constants:**
- `SPLITTER_WIDTH = 4`
- `SPLITTER_HEIGHT = 4`
- `SPLITTER_HIT_TOLERANCE = 2`
- `MIN_PANEL_SIZE = 50`
- `MAX_PANEL_SIZE = 5000`

**Key Methods:**
- `HitTestSplitter(Point, Rectangle, Orientation, int, string)` — Detect splitter intersection
- `UpdateCursorForSplitter(Orientation, bool)` — Change cursor based on splitter
- `CalculateDragResult(DragState, Size)` — Compute new panel sizes during drag
- `IsValidPanelSize(int)`, `ClampPanelSize(int)` — Size validation
- `CalculateSplitRatio(int, int)` — Compute split percentage
- `EnsureMinimumSize(Rectangle, Size)` — Enforce minimum dimensions
- `ClampToBounds(Rectangle, Rectangle)` — Constrain to container

**State Classes:**
- `SplitterHitTestResult` — Splitter intersection info
- `DragState` — Active drag operation state

### Integration with BeepDockingManager

**Manager Wiring:**
1. Constructor creates painter and layout controller
2. `CreateMdiClient()` instantiates `MdiPanelPositioner` and `PanelWindowManager` after MDI client HWND exists
3. `AddPanel()` registers panel with window manager
4. `RemovePanel()` unregisters and destroys window
5. `RecalculateLayout()` calls `_positioner.ApplyLayout()` with layout results
6. `Dispose()` cleans up all runtime managers

**Properties Added:**
- `Positioner` — Access to window positioning
- `Chrome` — Access to rendering layer
- `PanelWindowManager` — Access to lifecycle management

## Data Flow

```
DockingLayoutController.CalculateLayout()
	↓ returns Dictionary<string, Rectangle>
MdiPanelPositioner.ApplyLayout()
	├─ CreateWindowForPanel() [new panels]
	├─ RepositionWindow() [existing panels]
	├─ ShowWindow() / HideWindow()
	└─ UpdateZOrder()
		↓
	MDI Child Windows (actual HWND positioning)
		↓
WindowChrome (renders tabs, splitters, chrome)
		↓
	User Interaction (mouse events)
		↓
PositioningUtilities (hit-test, drag calculations)
		↓
	Split ratio adjustment
		↓
RecalculateLayout() [repeat]
```

## Native Win32 Integration

### P/Invoke Methods Used (from MdiNativeApi)
- `CreateWindowEx()` — Create MDI client
- `SendMessage(WM_MDICREATE, ...)` — Create MDI child window
- `SendMessage(WM_MDIACTIVATE, ...)` — Activate MDI child
- `SetWindowPos()` — Reposition window
- `ShowWindow()` — Show/hide window
- `DestroyWindow()` — Destroy window
- `GetClientRect()`, `GetWindowRect()` — Query bounds
- `UpdateWindow()`, `InvalidateRect()` — Rendering updates

### Window Creation Flow
1. `DockPanel` added to manager
2. `PanelWindowManager.CreatePanel()` called
3. `MdiPanelPositioner.CreateWindowForPanel()` sends `WM_MDICREATE`
4. MDI client creates child window (HWND returned)
5. HWND stored in `_panelToWindowMap`
6. Panel content (Control) parented to HWND (future phase)

## User Interaction Model

### Splitter Dragging
1. Mouse over splitter → `PositioningUtilities.HitTestSplitter()` detects
2. Cursor changes via `UpdateCursorForSplitter()`
3. Mouse down → `DragState` begins
4. Mouse move → `CalculateDragResult()` computes new sizes
5. Mouse up → `RecalculateLayout()` applies new split ratio
6. `ApplyLayout()` repositions windows

### Tab Interaction
1. Mouse over tab strip → `WindowChrome.HitTestTab()` detects
2. Tab click → `PanelWindowManager.HandleTabClick()` activates
3. `ActivatePanel()` sends `WM_MDIACTIVATE`
4. Close button click → `PanelWindowManager.HandleTabCloseClick()` dispatches

### Panel Activation
1. `ActivatePanel(panelKey)` called
2. `SendMessage(mdiClient, WM_MDIACTIVATE, childHwnd, 0)`
3. MDI system brings window to front
4. `PanelWindowManager` updates active state
5. `Chrome` redraws tabs with new active state

## State Management

### Window Lifecycle
```
Created (registered)
	↓
Visible (shown in layout)
	↓
Hidden (user or layout hides)
	↓
Visible (user restores)
	↓
Destroyed (removed from manager)
```

### Panel Window State
- `PanelKey` — Unique identifier
- `WindowHandle` — HWND of MDI child
- `State` — Current lifecycle state
- `IsVisible` — Window visibility flag
- `IsActive` — Active panel flag
- `LastBounds` — Last applied window bounds
- `CreatedAt` — Timestamp of creation
- `LastActivatedAt` — Timestamp of activation

## Validation & Constraints

### Size Constraints
- Minimum panel size: 50 pixels
- Maximum panel size: 5000 pixels
- Splitter width/height: 4 pixels
- Hit test tolerance: 2 pixels

### Split Ratio Constraints
- Minimum split: 10% (left/top panel gets at least 10%)
- Maximum split: 90% (right/bottom panel gets at least 10%)

### Bounds Constraints
- Panels cannot exceed container bounds
- Splitter dragging respects minimum sizes
- New panel rectangles clamped to container

## Testing

**Unit Tests** (`Tests/Docking/Runtime/PositioningTests.cs`):
- Vertical/horizontal splitter hit-testing
- Drag calculations with constraint validation
- Size clamping and validation
- Split ratio calculations
- Rectangle sizing and bounds adjustment

**Test Coverage:**
- Edge cases (drag to container edge, minimum size limits)
- Constraint enforcement (min/max sizes)
- State transitions (drag start/end)
- Boundary conditions (at container edges)

## Example Usage

```csharp
// Initialize
var example = new Phase3RuntimeExample();
example.Initialize(hostForm);
example.AddExamplePanels();

// Interact
example.ActivatePanel("LeftPanel");      // Bring to front
example.HidePanel("TopPanel");           // Hide panel
example.ShowPanel("TopPanel");           // Show panel
example.RemovePanel("RightPanel");       // Remove entirely

// Diagnostics
example.PrintDiagnostics();
```

## Known Limitations & Future Work

### Phase 3 Limitations
1. **Content Hosting**: Panel content controls are not yet parented to HWND (Phase 4)
2. **Rendering**: Chrome rendering is basic, no theme support yet (Phase 4)
3. **Mouse Events**: Splitter drag/tab click events not routed to chrome handler (Phase 4)
4. **Animation**: No transition animations during panel show/hide (Future)
5. **Persistence**: Layout state not persisted to config (Future)
6. **Undo/Redo**: No undo stack for layout changes (Future)

### Phase 4 Roadmap (Planned)
- Content control hosting and reparenting
- Advanced theme/painter integration
- Event routing and interaction handlers
- Chrome rendering with hover feedback
- Splitter drag preview/feedback
- Performance optimization (batch window updates)

### Phase 5 Roadmap (Planned)
- Design-time integration (`BeepDockingManagerDesigner`)
- Action list support
- Layout persistence
- Undo/redo stack
- Floating windows / undockable panels

## Diagnostics API

### Manager Diagnostics
```csharp
string diags = _dockingManager.GetDiagnostics();
// Includes: host form, HWND, panel count, layout tree state
```

### Positioner Diagnostics
```csharp
var positionerDiags = _dockingManager.Positioner.GetDiagnostics();
// Includes: managed panels, visible count, hidden count, active panel, window list
```

### Chrome Diagnostics
```csharp
var chromeDiags = _dockingManager.Chrome.GetDiagnostics();
// Includes: tab strip height, chrome height, cached tabs, tab bounds
```

### Manager Diagnostics
```csharp
var managerDiags = _dockingManager.PanelWindowManager.GetDiagnostics();
// Includes: total panels, visible/hidden count, active panel, descriptor list
```

## Performance Notes

### Optimization Points
1. **Batch Window Updates**: Use `BeginDeferWindowPos` / `EndDeferWindowPos` for multiple repositions
2. **Cache Invalidation**: Only invalidate cache when painter changes
3. **Hit Testing**: Optimize splitter detection with bounding checks first
4. **Z-Order**: Batch Z-order updates rather than individual `SetWindowPos` calls

### Measured Performance (Target)
- Panel add: < 5ms
- Window reposition: < 1ms per window
- Layout recalculation: < 10ms (for typical 4-8 panels)
- Splitter drag: 60 FPS with < 1ms drag delta calculations

## Architecture Decisions

### Why Separate Positioner from Manager?
- **Layout** is independent of **window management**
- **Rendering** is independent of both
- Allows testing layout without MDI windows
- Allows swapping renderers (future themes)

### Why WindowChrome is Separate?
- **Rendering layer** is abstracted from **positioning layer**
- Painters can be swapped without affecting window management
- Tab/chrome rendering can evolve independently
- Supports design-time and runtime rendering paths

### Why PositioningUtilities?
- **Reusable utilities** for splitter/drag calculations
- **Stateless** calculations enable testing
- **Constants** centralized for easy tuning
- **Decoupled** from manager/positioner implementation

## Integration Checklist

- [x] Create `MdiPanelPositioner` for window positioning
- [x] Create `WindowChrome` for tab/splitter rendering
- [x] Create `PanelWindowManager` for lifecycle management
- [x] Create `PositioningUtilities` for interaction calculations
- [x] Wire runtime layer into `BeepDockingManager`
- [x] Update `RecalculateLayout()` to apply window positions
- [x] Add unit tests for utilities and positioning
- [x] Create example demonstrating Phase 3 usage
- [x] Document architecture and data flow
- [ ] Phase 4: Content hosting and event routing
- [ ] Phase 4: Theme integration and advanced rendering
- [ ] Phase 5: Design-time support

## References

- **Phase 2 Layout**: `PHASE_2_LAYOUT_COMPLETE.md`
- **Native MDI API**: `Interop/MdiNativeApi.cs`
- **Painter Contract**: `Painters/IDockingPainter.cs`
- **Model Definitions**: `Models/DockPanel.cs`, `Models/DockGroup.cs`

---

**Phase 3 Status**: ✅ **COMPLETE**

All runtime window positioning infrastructure is implemented and integrated. The docking engine can now:
- Create and manage MDI child windows
- Apply calculated layout to concrete window positions
- Render tab chrome and splitters
- Track panel lifecycle and visibility
- Handle splitter drag interactions with constraint enforcement
- Provide diagnostic information about window state

Ready for Phase 4: Content hosting and event routing.
