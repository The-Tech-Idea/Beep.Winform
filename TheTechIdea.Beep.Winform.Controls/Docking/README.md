# Beep Docking

This docking implementation is currently under active repair. Do not treat the existing dockspace design-time behavior as complete.

## Current Blockers

- `BeepDockspace` header/tabs are not reliably clickable in the Visual Studio WinForms designer.
- Clicking a dockspace tab/header must activate the matching `DockPanel` and select that panel in the designer.
- Dragging from a dockspace tab/header at design time must support move/stack/drop behavior.
- `BeepDockspace.DockPosition` needs review and correction. The enum is present, but layout/design-time behavior is not reliable enough.
- `DockPanel` must remain content-only for docked panels; it must not draw docked headers.

## Intended Architecture

The Beep docking design should follow Krypton’s methodology:

- `BeepDockspace` is the docking cell/space.
- `BeepDockspace` owns the docked header and tabs.
- `DockPanel` is a page/content surface inside a dockspace.
- `BeepDockingManager` maps dockspace page events to docking actions.
- Designer-server code must use the Visual Studio designer services for selection, creation, serialization, and undo/redo.

Krypton reference files:

- `Krypton.Docking\Control Docking\KryptonDockspace.cs`
- `Krypton.Docking\Control Docking\KryptonSpace.cs`
- `Krypton.Docking\Elements Impl\KryptonDockingDockspace.cs`

## Important Files

Runtime/control project:

- `BeepDockspace.cs`
- `BeepDockingManager.cs`
- `Models\DockPanel.cs`
- `Models\DockingEnums.cs`
- `Models\DockingEventArgs.cs`
- `Helpers\DockingCaptionPainter.cs`

Designer-server project:

- `..\..\TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\Designers\BeepDockspaceDesigner.cs`
- `..\..\TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\Designers\DockPanelDesigner.cs`
- `..\..\TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\Infrastructure\BeepDockingDesignerWiring.cs`
- `..\..\TheTechIdea.Beep.Winform.Controls.Design.Server\Docking\Infrastructure\BeepDockingTypeRoutingProvider.cs`

## Design-Time Rules

- Create serialized design-time controls/components through `IDesignerHost.CreateComponent`.
- Wrap `Controls` collection and property changes with `IComponentChangeService` when designer serialization is expected.
- Do not rely on a `DockPanelDesigner` to receive dockspace-header clicks. Panels are laid out below the header.
- Do not claim header click/drag is fixed until it is manually verified in Visual Studio after rebuilding and reopening the designer.

## Verification Checklist

- Build `TheTechIdea.Beep.Winform.Controls`.
- Build `TheTechIdea.Beep.Winform.Controls.Design.Server`.
- Reopen the test form in Visual Studio designer.
- Click every dockspace tab/header and verify the corresponding panel activates and is selected.
- Drag a panel from the dockspace tab/header and verify move/stack/drop behavior.
- Change `DockPosition` and verify the dockspace layout changes correctly.
- Resize a dockspace and verify panels stay below the header and headers remain visible/clickable.
- Verify runtime tab click, close, pin, dropdown, and double-click behavior.
