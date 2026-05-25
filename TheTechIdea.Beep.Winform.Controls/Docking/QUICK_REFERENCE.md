# Beep Docking Quick Reference

## Current Status

The docking runtime and designer projects build, but the dockspace design-time experience is not complete.

Known failing areas:

- Dockspace header/tabs are still not reliably clickable in the Visual Studio designer.
- Header click does not reliably select/activate the intended `DockPanel`.
- Header drag/drop is not verified as working.
- `BeepDockspace.DockPosition` needs layout and serialization review.

## Core Ownership Rule

`BeepDockspace` owns docked header/tabs.

`DockPanel` is content/page only when hosted by a `BeepDockspace`.

Do not move docked header drawing or hit-testing into `DockPanel`.

## Current Runtime Files

| File | Role |
| --- | --- |
| `BeepDockspace.cs` | Dockspace/cell control, header/tab painting and intended hit testing |
| `BeepDockingManager.cs` | Docking manager and dockspace event bridge |
| `Models\DockPanel.cs` | Dockable page/content control |
| `Models\DockingEnums.cs` | Docking enums including `DockspaceHeaderStyle` |
| `Models\DockingEventArgs.cs` | Dockspace page event args |
| `Helpers\DockingCaptionPainter.cs` | SVG/path icon painting via Beep image painters |

## Current Designer Files

| File | Role |
| --- | --- |
| `TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\Designers\BeepDockspaceDesigner.cs` | Dockspace design-time selection, header hit-test, resize |
| `TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\Designers\DockPanelDesigner.cs` | DockPanel design-time verbs/actions; should not own dockspace header clicks |
| `TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\Infrastructure\BeepDockingDesignerWiring.cs` | Create, add, move, stack, resize dock panels/dockspaces |

## Krypton Mapping

| Krypton | Beep Target |
| --- | --- |
| `KryptonDockspace : KryptonSpace` | `BeepDockspace` |
| `KryptonWorkspaceCell` header/tab events | `BeepDockspace` page/header events |
| `SelectedPageChanged` | `SelectedPageChanged` |
| `PrimaryHeaderLeftClicked` | dockspace header/tab activation |
| `PrimaryHeaderRightClicked` / `ShowContextMenu` | dockspace dropdown/context menu |
| `TabDoubleClicked` / `PrimaryHeaderDoubleClicked` | floating/double-click request |
| `BeforePageDrag` | design/runtime dock drag request |

## Build Commands

```powershell
dotnet build "TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" --no-restore -v:q /clp:ErrorsOnly
dotnet build "TheTechIdea.Beep.Winform.Controls.Design.Server\TheTechIdea.Beep.Winform.Controls.Design.Server.csproj" --no-restore -v:q /clp:ErrorsOnly
```

## Must Verify In Visual Studio

Build success is not enough.

- Reopen the form designer after rebuilding.
- Click each dockspace tab/header.
- Confirm the clicked panel becomes visible and selected.
- Drag a tab/header and confirm docking move/stack/drop works.
- Change `DockPosition` and confirm actual layout updates.
