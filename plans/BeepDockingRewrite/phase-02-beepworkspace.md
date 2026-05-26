# Phase 2 — BeepWorkspace

> **CRITICAL**: Copy the EXACT code and logic from Krypton. Only change namespace/class/method names.
> Do NOT rewrite, simplify, stub, or reinterpret. Create EVERY class fully.
> No compatibility concerns. No legacy.

## Prerequisites

- Phase 1 COMPLETED (zero build errors)

## Goal

Create the Workspace layout engine. This replaces the old `LayoutController`, `DockPanelLayoutEngine`, and `DockGroup` systems. `BeepWorkspace` arranges cells (tab groups) with splitters.

## Krypton Source Files to Copy

All files under:
`C:\Users\f_ald\source\repos\The-Tech-Idea\Standard-Toolkit-master\Standard-Toolkit-master\Source\Krypton Components\Krypton.Workspace\Controls Workspace\`

### Task 2.1 — BeepWorkspaceCell

**Source**: `KryptonWorkspaceCell.cs`

```csharp
public class KryptonWorkspaceCell : KryptonNavigator, IWorkspaceItem
```

| # | Action | Detail |
|---|--------|--------|
| 2.1.1 | Create | `Controls/Docking/Workspace/BeepWorkspaceCell.cs` |
| 2.1.2 | Copy | Entire content |
| 2.1.3 | Rename class | `KryptonWorkspaceCell` → `BeepWorkspaceCell` |
| 2.1.4 | Rename base | `KryptonNavigator` → `BeepNavigator` |
| 2.1.5 | Rename interface | `IWorkspaceItem` → `IBeepWorkspaceItem` |
| 2.1.6 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Workspace` |

### Task 2.2 — BeepWorkspace

**Source**: `KryptonWorkspace.cs`

```csharp
public class KryptonWorkspace : VisualContainerControl, IDragTargetProvider
```

| # | Action | Detail |
|---|--------|--------|
| 2.2.1 | Create | `Controls/Docking/Workspace/BeepWorkspace.cs` |
| 2.2.2 | Copy | Entire content |
| 2.2.3 | Rename class | `KryptonWorkspace` → `BeepWorkspace` |
| 2.2.4 | Rename base | `VisualContainerControl` → `BeepVisualContainerControl` (create in Phase 0) |
| 2.2.5 | Rename all cell refs | `KryptonWorkspaceCell` → `BeepWorkspaceCell` |
| 2.2.6 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Workspace` |

### Task 2.3 — BeepWorkspaceSequence

**Source**: `KryptonWorkspaceSequence.cs`

```csharp
public class KryptonWorkspaceSequence : Component, IWorkspaceItem
```

| # | Action | Detail |
|---|--------|--------|
| 2.3.1 | Create | `Controls/Docking/Workspace/BeepWorkspaceSequence.cs` |
| 2.3.2 | Copy | Entire content |
| 2.3.3 | Rename class | `KryptonWorkspaceSequence` → `BeepWorkspaceSequence` |
| 2.3.4 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Workspace` |

### Task 2.4 — BeepWorkspaceCollection

**Source**: `KryptonWorkspaceCollection.cs`

| # | Action | Detail |
|---|--------|--------|
| 2.4.1 | Create | `Controls/Docking/Workspace/BeepWorkspaceCollection.cs` |
| 2.4.2 | Copy | Entire content |
| 2.4.3 | Rename | `KryptonWorkspaceCollection` → `BeepWorkspaceCollection` |
| 2.4.4 | Rename base | `TypedCollection<IWorkspaceItem>` → `BeepTypedCollection<IBeepWorkspaceItem>` |

### Task 2.5 — Supporting Types

Copy ALL workspace-related files from Krypton.Workspace:

| # | Krypton Source | Beep File | Location |
|---|---------------|-----------|----------|
| 2.5.1 | `StarSize.cs` | `BeepStarSize.cs` | `Workspace/` |
| 2.5.2 | `StarNumber.cs` | `BeepStarNumber.cs` | `Workspace/` |
| 2.5.3 | `IWorkspaceItem.cs` | `IBeepWorkspaceItem.cs` | `Interfaces/` |
| 2.5.4 | `SeparatorStyle.cs` enum | `BeepSeparatorStyle.cs` | `Enums/` |
| 2.5.5 | `CompactFlags.cs` enum | `BeepCompactFlags.cs` | `Enums/` |
| 2.5.6 | `ViewDrawWorkspaceSeparator.cs` | `BeepViewDrawWorkspaceSeparator.cs` | `View/` |
| 2.5.7 | `WorkspaceCellEventArgs.cs` | `BeepWorkspaceCellEventArgs.cs` | `Events/` |
| 2.5.8 | `ActivePageChangedEventArgs.cs` | `BeepActivePageChangedEventArgs.cs` | `Events/` |
| 2.5.9 | `ActiveCellChangedEventArgs.cs` | `BeepActiveCellChangedEventArgs.cs` | `Events/` |
| 2.5.10 | `MaximizeRestoreEventArgs.cs` | `BeepMaximizeRestoreEventArgs.cs` | `Events/` |
| 2.5.11 | `XmlSavingEventArgs.cs` | `BeepXmlSavingEventArgs.cs` | `Events/` |
| 2.5.12 | `CellPageNotify.cs` | `BeepCellPageNotify.cs` | `Workspace/` |

### Task 2.6 — DragManager Base

**Source**: `Krypton.Toolkit/DragManager.cs`

| # | Action | Detail |
|---|--------|--------|
| 2.6.1 | Create | `Controls/Docking/Dragging/BeepDragManager.cs` |
| 2.6.2 | Copy | Entire content, rename class to `BeepDragManager` |

## Build Verification

```
dotnet build "TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj" -v:q /clp:ErrorsOnly
```

Fix ALL errors before Phase 3.
