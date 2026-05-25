# Beep Docking Engine — Quick Reference Guide

**Status**: Phase 1 Foundation Complete (70-step plan, 9/70 steps = 13% complete)  
**Location**: `TheTechIdea.Beep.Winform.Controls/Docking/`  
**Compiled**: ✅ All ~2,257 lines compile without errors

---

## Architecture Overview

```
BeepDockingManager
  ├── MDI Client HWND (native Win32)
  ├── DockLayoutTree
  │   ├── Root DockGroup
  │   │   ├── DockGroup[Position=Left]
  │   │   │   ├── DockPanel[Key="toolwindow1"]
  │   │   │   ├── DockPanel[Key="toolwindow2"]
  │   │   │   └── Content controls
  │   │   ├── DockGroup[Position=Right]
  │   │   ├── DockGroup[Position=Fill]
  │   │   │   └── DockPanel[Key="document1"] (active)
  │   │   └── ...
  │   └── Panel Registry & Group Registry
  └── Event System
	  ├── PanelActivated
	  ├── PanelAdded
	  ├── PanelRemoved
	  └── ThemeChanged
```

---

## Files Created (Phase 1.1–1.3)

### Interop Layer (`Interop/`)

| File | Lines | Purpose |
|------|-------|---------|
| `MdiNativeApi.cs` | 520 | Win32 P/Invoke wrapper (RECT, WINDOWPLACEMENT, POINT, MDICREATESTRUCT; CreateWindowEx, DestroyWindow, SendMessage, SetWindowPos, GetWindowRect, BeginDeferWindowPos, DeferWindowPos, EndDeferWindowPos, etc.) |
| `MdiConstants.cs` | 247 | Constants (WM_*, WS_*, WS_EX_*, SW_*, SWP_*, HWND_*, GWL_*, MDICLIENT_CLASS) |
| `WindowBatchUpdater.cs` | 250 | Batched window position updates (DeferUpdate, DeferMove, DeferResize, EndUpdate, GetDiagnostics) |

### Data Models (`Models/`)

| File | Lines | Purpose |
|------|-------|---------|
| `DockingEnums.cs` | 45 | DockPosition, DockPanelState, SplitOrientation, TabStyle |
| `DockGroup.cs` | 280 | Hierarchical group model with panel/child management |
| `DockPanel.cs` | 145 | Single panel model with state and events |
| `DockLayoutTree.cs` | 280 | Versioned layout tree (schema v1 support for migration) |
| `PanelSerializationInfo.cs` | 120 | Serializable snapshot struct for persistence |

### Manager (`/`)

| File | Lines | Purpose |
|------|-------|---------|
| `BeepDockingManager.cs` | 370 | Orchestrator: MDI lifecycle, panel registry, layout tree, events |

---

## Key APIs

### Creating & Using the Manager

```csharp
// In Form.Load or constructor:
var manager = new BeepDockingManager(this);
manager.CreateMdiClient();

// Add panels
var panel1 = manager.AddPanel(
	"toolwindow1",
	"Properties",
	DockPosition.Right,
	propertiesControl
);

var panel2 = manager.AddPanel(
	"toolwindow2",
	"Toolbox",
	DockPosition.Left,
	toolboxControl
);

// Activate a panel
manager.ActivatePanel("toolwindow1");

// On form resize:
protected override void OnHandleCreated(EventArgs e)
{
	base.OnHandleCreated(e);
	manager.ResizeMdiClient();
}

protected override void OnResize(EventArgs e)
{
	base.OnResize(e);
	manager.ResizeMdiClient();
}

// Cleanup
protected override void OnFormClosing(FormClosingEventArgs e)
{
	manager.Dispose();
	base.OnFormClosing(e);
}
```

### Query Operations

```csharp
// Get a panel
var panel = manager.GetPanel("toolwindow1");

// Get all panels
var allPanels = manager.GetAllPanels();

// Get panels at a position
var leftPanels = manager.GetPanelsAtPosition(DockPosition.Left);

// Get diagnostics
Debug.Write(manager.GetDiagnostics());
Debug.Write(manager.LayoutTree.GetDiagnostics());
```

### Events

```csharp
manager.PanelActivated += (s, panel) => 
	Debug.WriteLine($"Panel activated: {panel.Key}");

manager.PanelAdded += (s, panel) => 
	Debug.WriteLine($"Panel added: {panel.Key}");

manager.PanelRemoved += (s, panel) => 
	Debug.WriteLine($"Panel removed: {panel.Key}");

manager.ThemeChanged += (s, e) => 
	Debug.WriteLine("Theme changed - invalidate paint");
```

---

## Next Phases

### Phase 1.4–1.5: Component & Painter Integration
- Create `BeepDockPanel` component for toolbox
- Implement `IDockingPainter` interface
- Create painter factory and adapter
- Implement painter caching

### Phase 1.6–1.8: Layout & Rendering
- Implement layout controller (rect computation)
- Implement renderer (painting engine)
- Create tab strip and content panel controls

### Phase 1.9–1.10: Serialization & Persistence
- Implement `DockLayoutSnapshot` model
- Implement `DockLayoutSerializer` (JSON)
- Support layout save/restore

### Phase 2: Designer Support
- Custom ComponentDesigner for `BeepDockingHost`
- Designer action lists and smart tags
- Code serialization to `InitializeComponent`
- Property grid enhancements

### Phase 3: Advanced Features
- Float windows (separate BeepiFormPro windows)
- Auto-hide strips
- Full test suite
- Performance benchmarks
- Documentation & examples

---

## Performance Considerations

- **Batch Updates**: `WindowBatchUpdater` groups multiple `SetWindowPos` calls to minimize redraws
- **Native MDI**: Uses Win32 MDI for raw performance vs. managed simulation
- **Painter Cache**: Beep painters will cache Font/Brush/Pen resources
- **Lazy Initialization**: MDI client created on-demand, not in constructor

---

## Debugging

Enable Debug output in Visual Studio Output window:

```csharp
Debug.WriteLine(manager.GetDiagnostics());
// Output:
// [BeepDockingManager] Initialized for host form: MainForm
// [BeepDockingManager] MDI client created: 0xABCD1234
// [BeepDockingManager] Panel added: toolwindow1
// [BeepDockingManager] Panel activated: toolwindow1
// [BeepDockingManager] MDI client resized to 1024x768
// [BeepDockingManager] Disposed
```

---

## Framework Compatibility

- ✅ .NET Framework 4.7.2+ (P/Invoke, GDI+)
- ✅ .NET 8+
- ✅ .NET 9+
- ✅ .NET 10+

All P/Invoke calls use standard Win32 APIs available in all frameworks.

---

## Integration Checklist

- [ ] Phase 1: Foundation ✅ 13% complete
- [ ] Phase 2: Painters & Rendering (~40% of work)
- [ ] Phase 3: Serialization & Designer (~30% of work)
- [ ] Phase 4: Advanced Features (~17% of work)

