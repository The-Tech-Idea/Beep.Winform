# BeepDockspace Fix Plan

## Root-Cause Analysis

The BeepDockspace has a **private `DockspaceHeaderHitSurface` inner control** (created in constructor, line 55-57 of `BeepDockspace.cs`) that overlays the header area. This child control has its own Win32 HWND and intercepts mouse events at runtime (correctly forwarding them to BeepDockspace methods). However, at **design time**, this child HWND creates a fatal problem:

1. The WinForms designer framework hit-tests the header surface child HWND.
2. Since `DockspaceHeaderHitSurface` is a `private sealed` class with no custom designer and no `[Designer]` attribute, the framework cannot route design-time clicks to it.
3. The `BeepDockspaceDesigner`'s `WndProc` override only receives messages directed to the **dockspace's HWND**, not the **header surface's HWND**.
4. The `IMessageFilter.PreFilterMessage` theoretically intercepts all messages, but its coordinate conversion (`Dockspace.PointToClient(screenPoint)`) may produce incorrect results when the source HWND is the header surface child.

**This single architectural issue causes all 5 reported blockers.**

### Blocker mapping

| Blocker | Root Cause |
|---------|-----------|
| 1. Header/tabs not clickable at design time | Header surface child HWND steals mouse messages; designer can't route them |
| 2. Tab click doesn't select DockPanel | Same as #1 — selection code exists but never reached |
| 3. Design-time drag from header fails | MouseMove/Up during drag filtered by wrong hwnd path in `PreFilterMessage` |
| 4. DockPosition layout broken | `OnComponentChanged` only listens for Bounds/Size, not DockPosition property changes |
| 5. DockPanel draws docked headers | Already correct (DockPanel.OnPaint is empty); verify and close |

---

## Fix Plan: 5 Phases

### Phase 1 — Eliminate `DockspaceHeaderHitSurface`

**File**: `TheTechIdea.Beep.Winform.Controls/Docking/BeepDockspace.cs`

The header surface is redundant because:
- DockPanels are laid out at y=HeaderHeight (26px), so they never overlap the header
- BeepDockspace already has `OnPaint` → `PaintHeader` for header drawing
- BeepDockspace already has `OnMouseDown`/`OnMouseMove`/`OnMouseDoubleClick`
- The only purpose of the header surface was `Cursor = Cursors.Hand` in the header area, which can be handled directly

**Actions**:

1. **Remove the `DockspaceHeaderHitSurface` inner class** (lines 925-965) entirely.

2. **Remove all references to `_headerSurface`**:
   - Remove `private readonly DockspaceHeaderHitSurface _headerSurface;` (line 35)
   - Remove from constructor:
     ```csharp
     _headerSurface = new DockspaceHeaderHitSurface(this);
     Controls.Add(_headerSurface);
     _headerSurface.BringToFront();
     ```
   - Remove `LayoutHeaderSurface()` method (lines 872-879)
   - Remove `_headerSurface?.BringToFront()` from `LayoutPanels()` (line 1068)
   - Remove `LayoutHeaderSurface()` call from `LayoutPanels()` (line 1067)

3. **Add direct mouse double-click handling** (replace what header surface forwarded):
   ```csharp
   protected override void OnMouseDoubleClick(MouseEventArgs e)
   {
       base.OnMouseDoubleClick(e);
       HandleHeaderDoubleClick(e.Location, e.Button);
   }
   ```

4. **Set Hand cursor in header area via OnMouseMove** (already exists at line 480-485 — verify it works without the header surface cursor override).

5. **Set TabStop in constructor** (already present).

**Expected result**: BeepDockspace now owns 100% of the header area as its own non-child surface. Mouse events hit the dockspace's HWND directly, and the designer's `WndProc` override will receive them.

---

### Phase 2 — Fix Design-Time Header Click (Tab Selection)

**File**: `TheTechIdea.Beep.Winform.Controls.Design.Server/Docking/Designers/BeepDockspaceDesigner.cs`

**Problem**: With the header surface removed, clicks on the header area hit the dockspace's HWND. But `GetHitTest` currently returns `true` for header points, meaning the designer framework treats the click as "container hit" (selecting the dockspace, not child panels). The dock panels below the header (y >= 26) already work for `GetHitTest → false → child designer`.

**Actions**:

1. **Fix `GetHitTest`**: Return `false` for ALL points. The header area should NOT prevent child panels from being hit-tested, and clicks on header should fall through to `WndProc`:
   ```csharp
   protected override bool GetHitTest(Point point)
   {
       return false; // Always pass through: let WndProc handle header clicks, child designers handle panels
   }
   ```

2. **Remove `IMessageFilter` implementation**. The `WndProc` override handles the header clicks now that there's no child HWND intercepting messages. Delete:
   - `IMessageFilter` from the class declaration
   - `PreFilterMessage` method
   - `Application.AddMessageFilter(this)` / `Application.RemoveMessageFilter(this)`
   - `ScreenPointFromMessage` static method

3. **Fix `WndProc` to properly handle header click activation**:
   - On `WM_LBUTTONDOWN` in header area: call `HandleDesignerHeaderMouseDown` which does `ActivateAndSelectTab`
   - On `WM_LBUTTONDBLCLK` in header area: call `Dockspace.HandleHeaderDoubleClick`
   - On `WM_RBUTTONDOWN` in header area: call `Dockspace.HandleHeaderMouseDown` for right-click context menu

   Current WndProc already does this but uses `ScreenPointFromMessage` for coordinate conversion. Use `ClientPointFromMessage` directly (which reads from lParam) since messages now arrive on the correct (dockspace) HWND.

4. **Simplify `HandleDesignerHeaderMouseDown`**: Remove `_dragPanel` capture logic here (move to Phase 3 for drag). For now, just activate and select:
   ```csharp
   DockPanel tab = Dockspace.HitTestTabAt(clientPoint);
   if (tab != null)
   {
       Dockspace.ActivatePanel(tab);
       SelectPanelInDesigner(tab);
       return true;
   }
   ```

5. **Verify** `OnDockspaceMouseDown` event handler (line 251-265) — this is now the same as header mouse down handled through WndProc. Keep as fallback.

**Expected result**: Clicking any visible tab in the header area selects the corresponding DockPanel in the Visual Studio designer (shown in PropertyGrid and with selection handles).

---

### Phase 3 — Fix Design-Time Tab Drag

**File**: `TheTechIdea.Beep.Winform.Controls.Design.Server/Docking/Designers/BeepDockspaceDesigner.cs`

**Problem**: Drag initiation needs `WM_MOUSEMOVE` and `WM_LBUTTONUP` during drag to be captured, even when the mouse leaves the dockspace area. Since we removed `IMessageFilter`, we need `Capture = true` to ensure mouse events continue arriving at the dockspace's WndProc.

**Actions**:

1. **Restore drag state in designer class** (re-add fields):
   ```csharp
   private DockPanel _dragPanel;
   private Point _dragStartClient;
   private bool _draggingTab;
   ```

2. **On header `WM_LBUTTONDOWN`**: After activating the tab, capture the mouse and enter potential-drag mode:
   ```csharp
   _dragPanel = tab;
   _dragStartClient = clientPoint;
   _draggingTab = false;
   Dockspace.Capture = true;
   ```

3. **On header `WM_MOUSEMOVE`**: If `_dragPanel != null`, check drag threshold:
   ```csharp
   int dx = Math.Abs(clientPoint.X - _dragStartClient.X);
   int dy = Math.Abs(clientPoint.Y - _dragStartClient.Y);
   if (!_draggingTab && (dx > SystemInformation.DragSize.Width || dy > SystemInformation.DragSize.Height))
   {
       _draggingTab = true;
       Dockspace.Cursor = Cursors.SizeAll;
   }
   ```

4. **On `WM_LBUTTONUP` during drag**: If `_draggingTab`:
   ```csharp
   Point screenPoint = Dockspace.PointToScreen(clientPoint);
   var args = Dockspace.RaiseBeforePageDrag(_dragPanel, screenPoint, _dragStartClient, Dockspace);
   if (!args.Cancel)
       BeepDockingDesignerWiring.DropPanelAt(_dragPanel, screenPoint, AsServiceProvider);
   ```
   Then release capture and reset state.

5. **Handle drag abort (`WM_CAPTURECHANGED`, `WM_KEYDOWN` Escape)**: Release drag state if capture is lost or Escape pressed.

**Expected result**: User can drag a tab from the header area, move it over other dockspaces (stacking) or to a new edge position, and drop it to reorganize panels — all at design time.

---

### Phase 4 — Fix `BeepDockspace.DockPosition` Layout

**Files**:
- `BeepDockspaceDesigner.cs` (requires change)
- `BeepDockingDesignerWiring.cs` (existing logic is sound but not triggered)

**Problem**: When the user changes `BeepDockspace.DockPosition` in the PropertyGrid at design time, `BeepDockspaceDesigner.OnComponentChanged` only triggers on `Bounds` or `Size` changes — **not** on `DockPosition` changes. So changing DockPosition has no visual effect until something triggers a layout recalculation.

Additionally, `BeepDockspace` itself doesn't set its `Control.Dock` property or Bounds based on `DockPosition`. The layout is solely driven by `BeepDockingDesignerWiring.RefreshHostLayout`, which positions dockspaces based on panel DockPosition, not dockspace DockPosition.

**Actions**:

1. **In `BeepDockspaceDesigner.OnComponentChanged`**: Add handling for `DockPosition` property change:
   ```csharp
   if (e.Member.Name == nameof(BeepDockspace.DockPosition))
   {
       try
       {
           _updatingDockPosition = true;
           BeepDockingDesignerWiring.RefreshHostLayout(Dockspace.Manager, AsServiceProvider);
       }
       finally
       {
           _updatingDockPosition = false;
       }
       return;
   }
   ```

2. **In `BeepDockspace`**: When `DockPosition` changes, also update `LayoutPanels()` to sync child panel positions:
   ```csharp
   set
   {
       if (_dockPosition == value) return;
       _dockPosition = value;
       SyncPanelDockingProperties();
       LayoutPanels();    // ADD THIS
       Invalidate();
   }
   ```

3. **In `BeepDockingDesignerWiring`**: `RefreshHostLayout` already handles the layout correctly — it groups panels by DockPosition and computes dockspace bounds. But it reads `panel.DockPosition`, not `dockspace.DockPosition`. Ensure the layout reads from the dockspace's own DockPosition when available:
   - In `SetStackBounds`, the dockspace's `DockPosition` is already set via `SetProperty`. ✓
   - In `RefreshHostLayout`, the grouping uses `p.DockPosition`. Consider also re-reading from the dockspace if panels are already assigned to one.

4. **Add `RefreshHostLayout` call in `BeepDockspaceDesigner.Initialize`**: After attaching to component, ensure initial layout is applied:
   ```csharp
   if (Dockspace.Manager != null)
       BeepDockingDesignerWiring.RefreshHostLayout(Dockspace.Manager, AsServiceProvider);
   ```

5. **Add `_updatingDockPosition` guard field** to prevent recursive layout updates.

**Expected result**: Changing `BeepDockspace.DockPosition` in the PropertyGrid at design time immediately repositions the dockspace to the correct edge and updates all child panel positions.

---

### Phase 5 — Verify DockPanel Remains Content-Only

**File**: `TheTechIdea.Beep.Winform.Controls/Docking/Models/DockPanel.cs`

**Status**: Already correct.

**Verification**:
- `DockPanel.OnPaint` (line 368-371): Only calls `base.OnPaint(e)` — no custom header/tab drawing ✓
- `DockPanel.ContentBounds` (line 101): Returns full `(0, 0, Width, Height)` — no header area deducted ✓
- No header hit-testing or mouse handling in DockPanel ✓
- Header rendering is entirely in `BeepDockspace.PaintHeader` ✓

**Action**: No code changes needed. Mark as verified and close.

---

### Phase 6 — Runtime Verification

After all design-time fixes, verify **runtime** behavior still works:

1. Rebuild both projects
2. Open test form in Visual Studio designer
3. Click tabs in header → verify DockPanel selects
4. Drag tab from header → verify panel moves/stacks/drops
5. Change DockPosition in PropertyGrid → verify dockspace repositions
6. Resize dockspace → verify panels stay below header
7. **Runtime**: Tab click, close button, pin button, dropdown menu, double-click all work
8. **Runtime**: Float/dock/auto-hide operations work through BeepDockingManager

---

## Files Changed Summary

| File | Phase | Change |
|------|-------|--------|
| `BeepDockspace.cs` | 1, 4 | Remove `DockspaceHeaderHitSurface`, add `OnMouseDoubleClick`, update `DockPosition` setter |
| `BeepDockspaceDesigner.cs` | 2, 3, 4 | Fix `GetHitTest`, remove `IMessageFilter`, fix WndProc drag, respond to `DockPosition` changes |
| `DockPanelDesigner.cs` | 5 | None (verify only) |

## Build & Verification Commands

```bash
dotnet build "TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj" --no-restore -v:q /clp:ErrorsOnly
dotnet build "TheTechIdea.Beep.Winform.Controls.Design.Server/TheTechIdea.Beep.Winform.Controls.Design.Server.csproj" --no-restore -v:q /clp:ErrorsOnly
```

After building both projects, **close and reopen Visual Studio** (or at least close and reopen the test form) before testing. The WinForms designer caches designer/control types aggressively.

---

## Krypton Reference Patterns

### KryptonDockspace.cs
- Thin subclass: `KryptonDockspace : KryptonSpace`
- No child controls created in constructor
- All header/tab behavior inherited from `KryptonSpace`

### KryptonSpace.cs
- Header/tab hit testing is part of the control itself (no separate header surface)
- Uses `ViewLayoutDocker` and `ViewDrawDocker` renderer tree for layout and painting
- Tab selection handled through `SelectedPageChanged` event
- Mouse events handled through ViewBase hierarchy, not separate child controls

### KryptonDockingDockspace.cs
- Wiring class that maps dockspace events to docking manager operations
- `OnDockspaceBeforePageDrag` → starts drag operation
- `OnDockspacePagesDoubleClicked` → float/restore toggle
- `OnDockspaceDropDownClicked` → context menu

**Key takeaway**: Krypton does NOT use a separate child control for header hit testing. The header is part of the space control's own surface, rendered and hit-tested through the View hierarchy.

---

## Risk Assessment

| Risk | Mitigation |
|------|------------|
| Removing header surface breaks runtime header painting | Header already painted by BeepDockspace.OnPaint; header surface just re-painted the same area redundantly |
| Removing header surface breaks runtime cursor | Cursor already handled by BeepDockspace.OnMouseMove |
| Removing IMessageFilter prevents drag outside dockspace | `Capture = true` ensures all mouse events stay on dockspace HWND during drag |
| Designer caches old type definitions | Rebuild both projects + reopen VS |
| DockPosition change triggers layout loop | `_updatingDockPosition` guard field prevents reentry |
