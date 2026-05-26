# Phase 3 — BeepSpace & BeepDockspace

> **CRITICAL**: Copy the EXACT code and logic from Krypton. Only change namespace/class/method names.
> Do NOT rewrite, simplify, stub, or reinterpret. Create EVERY class fully.
> No compatibility concerns. No legacy.

## Prerequisites

- Phase 2 COMPLETED (zero build errors)

## Goal

Create the docking space layer: `BeepSpace` (the base workspace-for-docking), `BeepDockspace` (the docked edge control), `BeepDockspaceSeparator`, and the auto-hide proxy/store page infrastructure.

This replaces the old `BeepDockspace.cs` which had manual tab rendering and a custom designer.

## Krypton Source Files to Copy

### Task 3.1 — BeepSpace

**Source**: `Krypton.Docking/Control Docking/KryptonSpace.cs`

This is the base class that adds docking-specific behavior to the workspace:

| # | Action | Detail |
|---|--------|--------|
| 3.1.1 | Create | `Controls/Docking/Space/BeepSpace.cs` |
| 3.1.2 | Copy | Entire `KryptonSpace.cs` content |
| 3.1.3 | Rename class | `KryptonSpace` → `BeepSpace` |
| 3.1.4 | Rename base | `KryptonWorkspace` → `BeepWorkspace` |
| 3.1.5 | Rename all internal types | `KryptonPage` → `BeepPage`, `KryptonWorkspaceCell` → `BeepWorkspaceCell` |
| 3.1.6 | Rename events | `PageCloseClicked` → keep name, change EventArgs to `BeepUniqueNameEventArgs` |
| 3.1.7 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Space` |

### Task 3.2 — BeepDockspace (NEW rewrite)

**Source**: `Krypton.Docking/Control Docking/KryptonDockspace.cs`

This is a THIN wrapper around BeepSpace:

| # | Action | Detail |
|---|--------|--------|
| 3.2.1 | Create | `Controls/Docking/Space/BeepDockspace.cs` |
| 3.2.2 | Copy | Entire `KryptonDockspace.cs` content |
| 3.2.3 | Rename class | `KryptonDockspace` → `BeepDockspace` |
| 3.2.4 | Rename base | `KryptonSpace` → `BeepSpace` |
| 3.2.5 | Attributes | `[ToolboxItem(false)]`, `[DesignerCategory("code")]`, `[DesignTimeVisible(false)]` - NO custom designer |
| 3.2.6 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Space` |

### Task 3.3 — BeepDockspaceSeparator

**Source**: `Krypton.Docking/Control Docking/KryptonDockspaceSeparator.cs`

| # | Action | Detail |
|---|--------|--------|
| 3.3.1 | Create | `Controls/Docking/Space/BeepDockspaceSeparator.cs` |
| 3.3.2 | Copy entire content, rename class |
| 3.3.3 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Space` |

### Task 3.4 — BeepFloatspace

**Source**: `Krypton.Docking/Control Docking/KryptonFloatspace.cs`

| # | Action | Detail |
|---|--------|--------|
| 3.4.1 | Create | `Controls/Docking/Space/BeepFloatspace.cs` |
| 3.4.2 | Copy entire content, rename class |
| 3.4.3 | Rename base | `KryptonSpace` → `BeepSpace` |
| 3.4.4 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Space` |

### Task 3.5 — BeepDockableWorkspace and BeepDockableNavigator

**Source**: 
- `Krypton.Docking/Control Docking/KryptonDockableWorkspace.cs`
- `Krypton.Docking/Control Docking/KryptonDockableNavigator.cs`

| # | Action | Detail |
|---|--------|--------|
| 3.5.1 | Create | `Controls/Docking/Space/BeepDockableWorkspace.cs` |
| 3.5.2 | Copy, rename | `KryptonDockableWorkspace` → `BeepDockableWorkspace`, inherits `BeepWorkspace` |
| 3.5.3 | Create | `Controls/Docking/Space/BeepDockableNavigator.cs` |
| 3.5.4 | Copy, rename | `KryptonDockableNavigator` → `BeepDockableNavigator`, inherits `BeepNavigator` |

### Task 3.6 — BeepStorePage & BeepAutoHiddenProxyPage

**Source**:
- `Krypton.Docking/Control Docking/KryptonStorePage.cs`
- `Krypton.Docking/Control Docking/KryptonAutoHiddenProxyPage.cs`

| # | Action | Detail |
|---|--------|--------|
| 3.6.1 | Create | `Controls/Docking/Space/BeepStorePage.cs` |
| 3.6.2 | Copy, rename | `KryptonStorePage` → `BeepStorePage`, inherits `BeepPage` |
| 3.6.3 | Create | `Controls/Docking/AutoHidden/BeepAutoHiddenProxyPage.cs` |
| 3.6.4 | Copy, rename | `KryptonAutoHiddenProxyPage` → `BeepAutoHiddenProxyPage`, inherits `BeepPage` |

### Task 3.7 — Event Args for Docking

Copy all EventArgs files from `Krypton.Docking/`:

| # | Krypton File | Beep File | Location |
|---|-------------|-----------|----------|
| 3.7.1 | `DockspaceEventArgs.cs` | `BeepDockspaceEventArgs.cs` | `Events/` |
| 3.7.2 | `DockspaceCellEventArgs.cs` | `BeepDockspaceCellEventArgs.cs` | `Events/` |
| 3.7.3 | `DockspaceSeparatorResizeEventArgs.cs` | `BeepDockspaceSeparatorResizeEventArgs.cs` | `Events/` |
| 3.7.4 | `AutoHiddenGroupEventArgs.cs` | `BeepAutoHiddenGroupEventArgs.cs` | `Events/` |
| 3.7.5 | `CancelDropDownEventArgs.cs` | `BeepCancelDropDownEventArgs.cs` | `Events/` |
| 3.7.6 | `CloseRequestEventArgs.cs` | `BeepCloseRequestEventArgs.cs` | `Events/` |
| 3.7.7 | `CancelUniqueNameEventArgs.cs` | `BeepCancelUniqueNameEventArgs.cs` | `Events/` |
| 3.7.8 | `PageDragEndData.cs` | `BeepPageDragEndData.cs` | `Events/` |

## Delete Old File

| # | File to Delete | Reason |
|---|---------------|--------|
| D3.1 | `TheTechIdea.Beep.Winform.Controls/Docking/BeepDockspace.cs` | Replaced by new `BeepDockspace.cs` in `Docking/Space/` |

Do NOT delete any other old files yet. All deletions happen in Phase 8.

## Build Verification

```
dotnet build "TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj" -v:q /clp:ErrorsOnly
```

Fix ALL errors before Phase 4.
