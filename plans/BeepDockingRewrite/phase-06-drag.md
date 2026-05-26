# Phase 6 — Drag Manager

> **CRITICAL**: Copy the EXACT code and logic from Krypton. Only change namespace/class/method names.
> Do NOT rewrite, simplify, stub, or reinterpret. Create EVERY class fully.
> No compatibility concerns. No legacy.

## Prerequisites

- Phase 5 COMPLETED (zero build errors)

## Goal

Port the drag-and-drop system: drag manager, drag targets, drag feedback.

### Task 6.1 — BeepDockingDragManager

**Source**: `Krypton.Docking/Dragging/DockingDragManager.cs`

| # | Action | Detail |
|---|--------|--------|
| 6.1.1 | Create | `Controls/Docking/Dragging/BeepDockingDragManager.cs` |
| 6.1.2 | Copy entire content, rename class |
| 6.1.3 | Rename base | `DragManager` → `BeepDragManager` (from Phase 2.6) |
| 6.1.4 | Rename ALL types | `KryptonPage` → `BeepPage`, etc. |
| 6.1.5 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Dragging` |

### Task 6.2 — Drag Targets

Search `Krypton.Docking/Dragging/` for ALL drag target files:

| # | Krypton File | Beep File | 
|---|-------------|-----------|
| 6.2.1 | `DragTarget.cs` | `BeepDragTarget.cs` |
| 6.2.2 | `DragTargetList.cs` | `BeepDragTargetList.cs` |
| 6.2.3 | `DragTargetControlEdge.cs` | `BeepDragTargetControlEdge.cs` |
| 6.2.4 | `DragTargetWorkspaceCellEdge.cs` | `BeepDragTargetWorkspaceCellEdge.cs` |
| 6.2.5 | `DragTargetWorkspaceEdge.cs` | `BeepDragTargetWorkspaceEdge.cs` |
| 6.2.6 | `DragTargetFloatingWindow.cs` | `BeepDragTargetFloatingWindow.cs` |
| 6.2.7 | `DockingDragTargetProvider.cs` | `BeepDockingDragTargetProvider.cs` |

Copy each entirely, rename all classes/types with `Beep` prefix.

### Task 6.3 — Drag Feedback

Search for `DragFeedback` in Krypton.Navigator and copy:

| # | Krypton File | Beep File |
|---|-------------|-----------|
| 6.3.1 | `DragFeedback.cs` | `BeepDragFeedback.cs` or in `BeepNavigator.cs` field if inline |

## Build Verification

```
dotnet build "TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj" -v:q /clp:ErrorsOnly
```
