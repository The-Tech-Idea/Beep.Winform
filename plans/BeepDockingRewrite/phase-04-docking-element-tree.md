# Phase 4 — Docking Element Tree

> **CRITICAL**: Copy the EXACT code and logic from Krypton. Only change namespace/class/method names.
> Do NOT rewrite, simplify, stub, or reinterpret. Create EVERY class fully.
> No compatibility concerns. No legacy.

## Prerequisites

- Phase 3 COMPLETED (zero build errors)

## Goal

Create the component-based docking element tree. This is the "brains" of the docking system — all elements are `Component` subclasses that form a tree from `BeepDockingManager` (root) down to `BeepDockingDockspace` (leaf). Nothing in this phase appears on the form surface — it's all runtime-managed.

## Key Architectural Point

`BeepDockingManager` is a **Component**, NOT a Control. It sits in the form's component tray. It does NOT appear in `designer.cs` with child control instantiation calls. ALL docking children (dockspaces, separators, floating windows) are created programmatically at runtime by calling `manager.ManageControl(panel1)`, `AddDockspace()`, etc.

This is THE reason Krypton avoids all designer issues — no `GetHitTest`, no `ISelectionService`, no `IDesignerHost.CreateComponent` for docking elements.

## Krypton Source Files to Copy

All files under:
`C:\Users\f_ald\source\repos\The-Tech-Idea\Standard-Toolkit-master\Standard-Toolkit-master\Source\Krypton Components\Krypton.Docking\`

### Task 4.1 — Base Elements

| # | Krypton File | Beep File | Location |
|---|-------------|-----------|----------|
| 4.1.1 | `Elements Base/DockingElement.cs` | `Docking/Elements/BeepDockingElement.cs` | Base abstract class |
| 4.1.2 | `Elements Base/DockingElementOpenCollection.cs` | `Docking/Elements/BeepDockingElementOpenCollection.cs` | Open collection base |
| 4.1.3 | `Elements Base/DockingElementClosedCollection.cs` | `Docking/Elements/BeepDockingElementClosedCollection.cs` | Closed collection base |
| 4.1.4 | `IDockingElement.cs` (interface) | `Docking/Interfaces/IBeepDockingElement.cs` | Core interface |

**Process for each**: Copy entire file, rename class/interface with `Beep` prefix.

### Task 4.2 — BeepDockingManager (NEW Component)

**Source**: `Krypton.Docking/Elements Impl/KryptonDockingManager.cs`

| # | Action | Detail |
|---|--------|--------|
| 4.2.1 | Create | `Controls/Docking/Manager/BeepDockingManager.cs` |
| 4.2.2 | Copy | Entire `KryptonDockingManager.cs` content |
| 4.2.3 | Rename class | `KryptonDockingManager` → `BeepDockingManager` |
| 4.2.4 | Base class | `DockingElementOpenCollection` → `BeepDockingElementOpenCollection` |
| 4.2.5 | Attributes | `[ToolboxItem(true)]`, `[DesignerCategory("code")]`, `[DesignTimeVisible(true)]` — no custom designer |
| 4.2.6 | Rename ALL internal types | `KryptonDockingControl` → `BeepDockingControl`, etc. |
| 4.2.7 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Manager` |

### Task 4.3 — BeepDockingControl

**Source**: `Krypton.Docking/Elements Impl/KryptonDockingControl.cs`

| # | Action | Detail |
|---|--------|--------|
| 4.3.1 | Create | `Controls/Docking/Elements/BeepDockingControl.cs` |
| 4.3.2 | Copy entire content, rename class |
| 4.3.3 | Namespace | `TheTechIdea.Beep.Winform.Controls.Docking.Elements` |

### Task 4.4 — Edge Infrastructure

| # | Source File | Beep File | Location |
|---|------------|-----------|----------|
| 4.4.1 | `KryptonDockingEdge.cs` | `BeepDockingEdge.cs` | `Elements/` |
| 4.4.2 | `KryptonDockingEdgeDocked.cs` | `BeepDockingEdgeDocked.cs` | `Elements/` |

### Task 4.5 — Docking Space Adaptors

| # | Source File | Beep File | Note |
|---|------------|-----------|------|
| 4.5.1 | `KryptonDockingSpace.cs` | `BeepDockingSpace.cs` | Abstract base for all space adapters |
| 4.5.2 | `KryptonDockingDockspace.cs` | `BeepDockingDockspace.cs` | Wraps `BeepDockspace` control |
| 4.5.3 | `KryptonDockingWorkspace.cs` | `BeepDockingWorkspace.cs` | Wraps workspace |
| 4.5.4 | `KryptonDockingNavigator.cs` | `BeepDockingNavigator.cs` | Wraps navigator |

### Task 4.6 — Enums

| # | Krypton File | Beep File | Location |
|---|-------------|-----------|----------|
| 4.6.1 | `DockingPropogateAction` enum | `BeepDockingPropogateAction.cs` | `Enums/` |
| 4.6.2 | `DockingPropogateIntState` enum | `BeepDockingPropogateIntState.cs` | `Enums/` |
| 4.6.3 | `DockingPropogateBoolState` enum | `BeepDockingPropogateBoolState.cs` | `Enums/` |
| 4.6.4 | `DockingPropogatePageList` enum | `BeepDockingPropogatePageList.cs` | `Enums/` |
| 4.6.5 | `DockingLocation` enum | `BeepDockingLocation.cs` | `Enums/` |
| 4.6.6 | `DockingEdge` enum | `BeepDockingEdge.cs` (enum, not class) | `Enums/` |
| 4.6.7 | `DockingHelper.cs` | `BeepDockingHelper.cs` | `Helpers/` |

## Build Verification

```
dotnet build "TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj" -v:q /clp:ErrorsOnly
```

Fix ALL errors before Phase 5.
