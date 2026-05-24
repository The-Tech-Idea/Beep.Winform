# Phase 3 Component Index

## Quick Reference Guide for Phase 3: Runtime Window Positioning

### Core Runtime Components

| Component | File | Purpose | Key Classes |
|-----------|------|---------|------------|
| **Panel Positioner** | `Runtime/MdiPanelPositioner.cs` | Maps layout rectangles to MDI window positions | `MdiPanelPositioner`, `PanelWindowState` |
| **Window Chrome** | `Runtime/WindowChrome.cs` | Renders tabs, chrome, splitters, docking guides | `WindowChrome`, `TabItem` |
| **Panel Manager** | `Runtime/PanelWindowManager.cs` | Coordinates panel lifecycle and activation | `PanelWindowManager`, `WindowDescriptor` |
| **Positioning Utilities** | `Runtime/PositioningUtilities.cs` | Splitter interaction and drag calculations | `PositioningUtilities`, `SplitterHitTestResult`, `DragState` |

### Integration Point

| File | Changes | Purpose |
|------|---------|---------|
| **BeepDockingManager.cs** | +65 lines, 7 edits | Orchestrator integration |

### Supporting Files

| File | Lines | Purpose |
|------|-------|---------|
| `PHASE_3_RUNTIME_COMPLETE.md` | 450+ | Architecture and detailed documentation |
| `PHASE_3_SUMMARY.md` | 400+ | Completion summary and usage guide |
| `Tests/Docking/Runtime/PositioningTests.cs` | 360 | 20+ unit tests |
| `Examples/Phase3RuntimeExample.cs` | 320 | Usage example and demo form |

## Component Dependencies

```
BeepDockingManager
├── MdiPanelPositioner (runtime window positioning)
├── WindowChrome (rendering layer)
├── PanelWindowManager (lifecycle coordination)
├── PositioningUtilities (static utilities)
├── DockingLayoutController (Phase 2)
├── DockLayoutTree (layout registry)
└── IDockingPainter (painter abstraction)
```

## Public APIs by Component

### MdiPanelPositioner
```csharp
public void ApplyLayout(Dictionary<string, Rectangle>, List<DockPanel>)
public void CreateWindowForPanel(DockPanel, Rectangle)
public void RepositionWindow(string, Rectangle, DockPanel)
public void ShowWindow(string)
public void HideWindow(string)
public void ActivatePanel(string)
public void DestroyPanel(string)
public IntPtr? GetPanelWindow(string)
public string GetPanelForWindow(IntPtr)
public PositionerDiagnostics GetDiagnostics()
```

### WindowChrome
```csharp
public void DrawTabStrip(Graphics, Rectangle, DockGroup)
public void DrawTab(Graphics, Rectangle, DockPanel, bool)
public void DrawChrome(Graphics, Rectangle)
public void DrawSplitter(Graphics, Rectangle, bool)
public void DrawDockingGuide(Graphics, Rectangle, DockPosition)
public string HitTestTab(Point)
public string HitTestCloseButton(Point, Rectangle)
public void InvalidateCache()
public void UpdateMetrics(int, int)
public ChromeDiagnostics GetDiagnostics()
public void Dispose()
```

### PanelWindowManager
```csharp
public bool CreatePanel(DockPanel, IntPtr)
public bool DestroyPanel(string)
public bool ActivatePanel(string)
public bool HidePanel(string)
public bool ShowPanel(string)
public string GetActivePanel()
public List<string> GetManagedPanels()
public WindowDescriptor GetPanelDescriptor(string)
public void HandleTabClick(string)
public void HandleTabCloseClick(string)
public ManagerDiagnostics GetDiagnostics()

public event EventHandler<PanelEventArgs> PanelCreated
public event EventHandler<PanelEventArgs> PanelDestroyed
public event EventHandler<PanelEventArgs> PanelActivated
public event EventHandler<PanelEventArgs> PanelHidden
```

### PositioningUtilities (Static)
```csharp
// Constants
public const int SPLITTER_WIDTH = 4
public const int SPLITTER_HEIGHT = 4
public const int SPLITTER_HIT_TOLERANCE = 2
public const int MIN_PANEL_SIZE = 50
public const int MAX_PANEL_SIZE = 5000

// Methods
public static SplitterHitTestResult HitTestSplitter(Point, Rectangle, Orientation, int, string)
public static void UpdateCursorForSplitter(Orientation, bool)
public static void CalculateDragResult(DragState, Size)
public static bool IsValidPanelSize(int)
public static int ClampPanelSize(int)
public static int CalculateSplitRatio(int, int)
public static Rectangle EnsureMinimumSize(Rectangle, Size)
public static Rectangle ClampToBounds(Rectangle, Rectangle)
```

### BeepDockingManager (New Properties)
```csharp
public MdiPanelPositioner Positioner { get; }
public WindowChrome Chrome { get; }
public PanelWindowManager PanelWindowManager { get; }
```

## State Models

### PanelWindowState
Tracks state of a single window:
- `PanelKey: string` — Unique identifier
- `WindowHandle: IntPtr` — HWND
- `IsVisible: bool` — Visibility flag
- `IsActive: bool` — Active flag
- `LastBounds: Rectangle` — Last applied bounds

### WindowDescriptor
Metadata for managed panel:
- `PanelKey: string` — Panel identifier
- `Panel: DockPanel` — Panel object
- `WindowHandle: IntPtr` — HWND
- `State: WindowLifecycleState` — Current state
- `CreatedAt: DateTime` — Creation timestamp
- `LastActivatedAt: DateTime?` — Activation timestamp

### WindowLifecycleState
Enum with states: `Created`, `Visible`, `Hidden`, `Destroyed`

### SplitterHitTestResult
Hit test results:
- `HitSplitter: bool` — If splitter was hit
- `SplitterBounds: Rectangle` — Splitter bounds
- `SplitterOrientation: Orientation` — Vertical or horizontal
- `SplitterId: string` — Splitter identifier
- `LeftOrTopPanel: Rectangle` — Left/top panel bounds
- `RightOrBottomPanel: Rectangle` — Right/bottom panel bounds

### DragState
Tracks active drag:
- `IsDragging: bool` — Drag in progress
- `StartPosition: Point` — Drag start
- `CurrentPosition: Point` — Current position
- `SplitterInfo: SplitterHitTestResult` — Splitter being dragged
- `NewLeftOrTopSize: int` — New left/top size
- `NewRightOrBottomSize: int` — New right/bottom size
- `GetDragDelta(): Point` — Delta calculation
- `Reset(): void` — Reset state

## Native Win32 Integration

### P/Invoke Methods Used
- `CreateWindowEx` — Create MDI client window
- `SendMessage(WM_MDICREATE)` — Create MDI child window
- `SendMessage(WM_MDIACTIVATE)` — Activate MDI child
- `SetWindowPos` — Reposition window
- `ShowWindow` — Show/hide window
- `DestroyWindow` — Destroy window
- `GetClientRect`, `GetWindowRect` — Query bounds
- `UpdateWindow`, `InvalidateRect` — Rendering

### Window Messages
- `WM_MDICREATE` — Create MDI child
- `WM_MDIACTIVATE` — Activate MDI child
- `WM_MDIDESTROY` — Destroy MDI child

## Key Constants

| Constant | Value | Purpose |
|----------|-------|---------|
| `SPLITTER_WIDTH` | 4 | Visual width of vertical splitter |
| `SPLITTER_HEIGHT` | 4 | Visual height of horizontal splitter |
| `SPLITTER_HIT_TOLERANCE` | 2 | Pixels beyond splitter for hit test |
| `MIN_PANEL_SIZE` | 50 | Minimum panel dimension |
| `MAX_PANEL_SIZE` | 5000 | Maximum panel dimension |

## Data Flow Diagram

```
User Action
	↓
Form Mouse Event
	↓
Phase3RuntimeExample / Manager Handler
	↓
PositioningUtilities (hit test / drag calc)
	↓
PanelWindowManager / MdiPanelPositioner
	↓
P/Invoke SendMessage / SetWindowPos
	↓
MDI Client Window
	↓
MDI Child Windows
	↓
WindowChrome (renders tabs/chrome)
```

## Test Coverage

**File**: `Tests/Docking/Runtime/PositioningTests.cs`

| Test Class | Tests | Coverage |
|-----------|-------|----------|
| `PositioningUtilitiesTests` | 15+ | Splitter hit testing, drag, size validation |
| `PanelWindowManagerTests` | 2+ | Panel lifecycle |
| `WindowChromeTests` | 1+ | Rendering initialization |

**xUnit Test Methods**:
- `HitTestSplitter_VerticalSplitter_PointOnSplitter_ReturnsHit`
- `HitTestSplitter_VerticalSplitter_PointNotOnSplitter_ReturnsNoHit`
- `HitTestSplitter_HorizontalSplitter_PointOnSplitter_ReturnsHit`
- `CalculateDragResult_VerticalSplit_DragRight_IncreasesLeftPanel`
- `CalculateDragResult_VerticalSplit_DragLeft_DecreasesLeftPanel`
- `CalculateDragResult_HorizontalSplit_DragDown_IncreasesTopPanel`
- `CalculateDragResult_RespectMinimumSize`
- `IsValidPanelSize_WithinBounds_ReturnsTrue`
- `IsValidPanelSize_BelowMinimum_ReturnsFalse`
- `IsValidPanelSize_AboveMaximum_ReturnsFalse`
- `ClampPanelSize_BelowMinimum_ReturnsMimimum`
- `ClampPanelSize_AboveMaximum_ReturnsMaximum`
- `ClampPanelSize_WithinBounds_ReturnsValue`
- `CalculateSplitRatio_FiftyFifySplit_Returns50`
- `CalculateSplitRatio_ThirtySeventySplit_Returns30`
- `CalculateSplitRatio_Clamps_BetweenTenAndNinety`
- `EnsureMinimumSize_SmallerThanMinimum_EnlargesRectangle`
- `EnsureMinimumSize_LargerThanMinimum_KeepsRectangle`
- `ClampToBounds_RectangleOutsideContainer_ClampsToContainer`
- `ClampToBounds_RectangleInsideContainer_KeepsRectangle`

## Usage Quick Start

```csharp
// 1. Create and initialize manager
var manager = new BeepDockingManager(hostForm);
manager.CreateMdiClient();

// 2. Add panels
manager.AddPanel("left", "Explorer", DockPosition.Left, leftControl);
manager.AddPanel("main", "Editor", DockPosition.Fill, editorControl);

// 3. Subscribe to events
manager.PanelWindowManager.PanelActivated += (s, e) =>
	Console.WriteLine($"Panel activated: {e.PanelKey}");

// 4. Handle layout changes
hostForm.Resize += (s, e) => manager.ResizeMdiClient();

// 5. Interact with panels
manager.PanelWindowManager.ActivatePanel("left");
manager.PanelWindowManager.HidePanel("left");

// 6. Get diagnostics
var diags = manager.GetDiagnostics();
```

## Related Documentation

- `PHASE_2_LAYOUT_COMPLETE.md` — Layout calculation engine
- `PHASE_3_RUNTIME_COMPLETE.md` — Detailed Phase 3 architecture
- `PHASE_3_SUMMARY.md` — Completion summary
- `Interop/MdiNativeApi.cs` — Win32 P/Invoke declarations
- `Painters/IDockingPainter.cs` — Painting contract
- `Models/*.cs` — Data model definitions

## Next Phase: Phase 4

Phase 4 will add:
- Content control hosting (reparenting to MDI windows)
- Event routing from windows to chrome handlers
- Advanced theme/painter integration
- Splitter drag visual feedback
- Performance optimization

---

**Phase 3 Status**: ✅ COMPLETE and READY FOR PHASE 4

All components are implemented, tested, documented, and integrated.

