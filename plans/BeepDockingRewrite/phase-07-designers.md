# Phase 7 — Designers

> **CRITICAL**: Copy the EXACT code and logic from Krypton. Only change namespace/class/method names.
> Do NOT rewrite, simplify, stub, or reinterpret. Create EVERY class fully.
> No compatibility concerns. No legacy.

## Prerequisites

- Phase 6 COMPLETED (zero build errors)
- `TheTechIdea.Beep.Winform.Controls.Design.Server` project exists and compiles

## Goal

Create minimal custom designers ONLY for user-facing controls. Krypton avoids designers for the docking layer itself — so does Beep.

NO designer for `BeepDockingManager`, `BeepDockspace`, `BeepDockingDockspace`, or any element tree classes. These are `[DesignerCategory("code")]` only.

### Task 7.1 — BeepNavigatorDesigner

**Source**: `Krypton.Navigator/Navigator/KryptonNavigatorDesigner.cs`

| # | Action | Detail |
|---|--------|--------|
| 7.1.1 | Create | `Design.Server/Docking/Designers/BeepNavigatorDesigner.cs` |
| 7.1.2 | Copy entire content |
| 7.1.3 | Rename class | `KryptonNavigatorDesigner` → `BeepNavigatorDesigner` |
| 7.1.4 | Rename ALL types | `KryptonPage` → `BeepPage`, `KryptonNavigator` → `BeepNavigator` |
| 7.1.5 | Designer verb names | "Add Page" / "Remove Page" / "Clear Pages" — keep same |
| 7.1.6 | `IDesignerHost.CreateComponent(typeof(BeepPage))` — uses `BeepPage` type |

### Task 7.2 — BeepWorkspaceCellDesigner

**Source**: `Krypton.Workspace/Workspace/KryptonWorkspaceCellDesigner.cs`

| # | Action | Detail |
|---|--------|--------|
| 7.2.1 | Create | `Design.Server/Docking/Designers/BeepWorkspaceCellDesigner.cs` |
| 7.2.2 | Copy entire content |
| 7.2.3 | Rename class | `KryptonWorkspaceCellDesigner` → `BeepWorkspaceCellDesigner` |
| 7.2.4 | Rename base | `KryptonNavigatorDesigner` → `BeepNavigatorDesigner` |
| 7.2.5 | `SelectionRules` | Keep `SelectionRules.None` (from Krypton, cell sizing is managed by workspace) |

### Task 7.3 — BeepWorkspaceDesigner

**Source**: `Krypton.Workspace/Workspace/KryptonWorkspaceDesigner.cs`

| # | Action | Detail |
|---|--------|--------|
| 7.3.1 | Create | `Design.Server/Docking/Designers/BeepWorkspaceDesigner.cs` |
| 7.3.2 | Copy entire content |
| 7.3.3 | Rename class | `KryptonWorkspaceDesigner` → `BeepWorkspaceDesigner` |
| 7.3.4 | Rename all types | `KryptonWorkspace` → `BeepWorkspace`, etc. |

### Task 7.4 — BeepWorkspaceSequenceDesigner

**Source**: `Krypton.Workspace/Workspace/KryptonWorkspaceSequenceDesigner.cs`

| # | Action | Detail |
|---|--------|--------|
| 7.4.1 | Create | `Design.Server/Docking/Designers/BeepWorkspaceSequenceDesigner.cs` |
| 7.4.2 | Copy entire content, rename class/types |

### Task 7.5 — BeepPageDesigner

**Source**: `Krypton.Navigator/Navigator/KryptonPageDesigner.cs`

| # | Action | Detail |
|---|--------|--------|
| 7.5.1 | Create | `Design.Server/Docking/Designers/BeepPageDesigner.cs` |
| 7.5.2 | Copy entire content |
| 7.5.3 | Rename class | `KryptonPageDesigner` → `BeepPageDesigner` |
| 7.5.4 | `SelectionRules.None | SelectionRules.Locked` — keep as Krypton |

### Task 7.6 — Designer Attributes

Apply `[Designer(...)]` attributes to the Beep controls:

| Control | Attribute |
|---------|----------|
| `BeepNavigator` | `[Designer(typeof(BeepNavigatorDesigner))]` |
| `BeepWorkspace` | `[Designer(typeof(BeepWorkspaceDesigner))]` |
| `BeepWorkspaceCell` | `[Designer(typeof(BeepWorkspaceCellDesigner))]` |
| `BeepWorkspaceSequence` | `[Designer(typeof(BeepWorkspaceSequenceDesigner))]` |
| `BeepPage` | `[Designer(typeof(BeepPageDesigner))]` |
| `BeepDockspace` | `[DesignerCategory("code")]` — NO custom designer |
| `BeepDockingManager` | `[DesignerCategory("code")]` — NO custom designer |

## Build Verification

```
dotnet build "TheTechIdea.Beep.Winform.Controls.Design.Server/TheTechIdea.Beep.Winform.Controls.Design.Server.csproj" -v:q /clp:ErrorsOnly
```
