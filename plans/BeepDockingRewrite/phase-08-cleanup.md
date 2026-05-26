# Phase 8 — Cleanup & Wiring

> **CRITICAL**: No compatibility concerns. No legacy. Delete old code completely.
> Every reference to old types must be replaced with new Beep equivalents.

## Prerequisites

- Phase 7 COMPLETED (zero build errors)

## Goal

Delete ALL old BeepDocking files, adapt remaining code that referenced them, and ensure the solution builds end-to-end.

## Task 8.1 — Delete Old Files

Delete the following files from `TheTechIdea.Beep.Winform.Controls/Docking/`:

| # | File | Reason |
|---|------|--------|
| 8.1.1 | `BeepDockspace.cs` (old) | Replaced by `Space/BeepDockspace.cs` |
| 8.1.2 | `BeepDockingManager.cs` (old) | Replaced by `Manager/BeepDockingManager.cs` |
| 8.1.3 | `Models/DockPanel.cs` | Replaced by `Navigator/BeepPage.cs` |
| 8.1.4 | `Models/DockGroup.cs` | Replaced by `BeepWorkspaceCell` groups |
| 8.1.5 | `Controllers/LayoutController.cs` | Replaced by `BeepWorkspace` layout |
| 8.1.6 | `Controllers/DockPanelLayoutEngine.cs` | Replaced by workspace layout |
| 8.1.7 | `Helpers/TabInteractionHandler.cs` | Replaced by `BeepNavigator` native tab handling |
| 8.1.8 | `Helpers/DockingCaptionPainter.cs` | Replaced by `BeepNavigator` tab rendering |
| 8.1.9 | `Models/DockingThemeColors.cs` | Themes handled by palette system in Phase 0 |
| 8.1.10 | `DockingPainters/` (entire folder) | Replaced by View paint system |
| 8.1.11 | `Enums/DockPosition.cs` (if exists) | Replaced by `BeepDockingLocation` enum |
| 8.1.12 | `Models/` (any remaining files) | Ported to new equivalents |

Delete from `TheTechIdea.Beep.Winform.Controls.Design.Server/Docking/`:

| # | File | Reason |
|---|------|--------|
| 8.1.13 | `Designers/BeepDockspaceDesigner.cs` | No custom dockspace designer |
| 8.1.14 | `Designers/DockPanelDesigner.cs` | Replaced by `BeepPageDesigner` |
| 8.1.15 | `Designers/BeepDockingManagerDesigner.cs` | No custom manager designer |
| 8.1.16 | `Infrastructure/BeepDockingDesignerWiring.cs` | No design-time serialization of docking layer |
| 8.1.17 | `Infrastructure/` (if only contains the above) | Remove empty folder |

## Task 8.2 — Update References in Remaining Code

Search the ENTIRE solution for references to deleted types:

```
dotnet build <solution>.sln
```

Then fix every error:

| Old Reference | New Reference |
|--------------|---------------|
| `using TheTechIdea.Beep.Winform.Controls.Docking;` (old BeepDockspace) | `using TheTechIdea.Beep.Winform.Controls.Docking.Space;` |
| `new BeepDockspace()` | `new BeepDockspace()` (same name, new namespace) |
| `DockPanel` | `BeepPage` |
| `dockPanel1.Key` | `page1.UniqueName` or `page1.Name` |
| `new DockPanel()` | `new BeepPage()` |
| `BeepDockingManager` (old) | `BeepDockingManager` (same name, new namespace `Docking.Manager`) |
| `DockPosition.Left` | `BeepDockingEdge.Left` or `DockingEdge.Left` (check Krypton enum) |
| `panel.DockPosition` | page is added to a cell, which is in a dockspace, which has an edge |
| `dockPanel.Title` | `page.Text` |
| `dockPanel.TabBounds` | Handled by `BeepNavigator` — no longer exposed |

## Task 8.3 — Update Form Code

Search for `BeepiFormPro` and any test forms that create `DockPanel` or `BeepDockspace` instances. Update them:

**Old pattern:**
```csharp
var panel = new DockPanel { Key = "toolbox", Title = "Toolbox" };
var dockspace = new BeepDockspace { DockPosition = DockPosition.Left };
dockspace.Controls.Add(panel);
manager.ManageControl(dockspace, DockPosition.Left);
```

**New pattern:**
```csharp
var page = new BeepPage { UniqueName = "toolbox", Text = "Toolbox" };
// BeepDockingManager handles everything:
manager.ManageControl(this);  // this = BeepiFormPro
manager.AddDockspace("Left", "Toolbox", new Size(200, 400));
manager.AddPageToDockspace("Left", page);
```

(NOTE: Exact API matches Krypton's `KryptonDockingManager` API — copy the pattern from there.)

## Task 8.4 — Final Build

```
dotnet build <solution>.sln -v:q /clp:ErrorsOnly
```

Fix ALL errors. There should be ZERO references to deleted types.

## Task 8.5 — Verify Designer Works

1. Open the solution in Visual Studio
2. Open a form that uses `BeepDockingManager`
3. Verify the manager appears in the component tray
4. Verify you can drop `BeepNavigator` and `BeepWorkspace` from toolbox (if applicable)
5. Verify no `GetHitTest` errors, no `ISelectionService` issues, no tab selection problems

## Completion

After Phase 8, the BeepDocking system should be a clean Krypton-port:
- All docking infrastructure runs at runtime
- No design-time serialization of dockspaces/panels
- No `GetHitTest` / `ISelectionService` / `DesignMode` issues
- Tab rendering handled by `BeepNavigator` natively
- Click-to-select handled by `BeepNavigator.SelectedPage` property
- Designer integration ONLY via `BeepNavigatorDesigner` for page management
