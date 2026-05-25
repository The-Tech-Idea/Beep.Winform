# Beep Docking Master Tracker

## Priority 0 - Current Blockers

- [ ] Fix `BeepDockspace` design-time header/tab click behavior.
- [ ] Clicking a dockspace tab/header selects and activates the corresponding `DockPanel`.
- [ ] Design-time drag from dockspace header/tab moves, stacks, or drops panels correctly.
- [ ] Fix `BeepDockspace.DockPosition` layout behavior.
- [ ] Verify dockspace resize in the Visual Studio designer.
- [ ] Verify runtime header click, close, pin, dropdown, and double-click behavior after design-time fixes.

## Priority 1 - Architecture Cleanup

- [ ] Audit `BeepDockspace.cs` against Krypton `KryptonSpace` and `KryptonDockspace`.
- [ ] Confirm `DockPanel` is content-only for docked panels.
- [ ] Remove any remaining incorrect dependency on `DockPanelDesigner` for dockspace-header interaction.
- [ ] Decide whether the dockspace header hit surface should be a private runtime child, a designer-only service, or a true dockspace cell abstraction.
- [ ] Ensure design-time child/control creation uses `IDesignerHost.CreateComponent` when serialization is required.

## Priority 2 - DockPosition

- [ ] Inspect `BeepDockingDesignerWiring.GetOrCreateDockspace`.
- [ ] Inspect `BeepDockingDesignerWiring.AddDockspaceToHostForm`.
- [ ] Inspect `BeepDockingDesignerWiring.ResizeDockspace`.
- [ ] Inspect `BeepDockingDesignerWiring.MovePanel`.
- [ ] Inspect `BeepDockingDesignerWiring.DropPanelAt`.
- [ ] Verify dockspace bounds for `Left`, `Right`, `Top`, `Bottom`, and `Fill`.
- [ ] Verify headers remain visible and clickable for every dock position.
- [ ] Verify designer serialization updates when `DockPosition` changes.

## Priority 3 - Krypton/DockPanelSuite Reference Study

- [ ] Read Krypton `Control Docking\KryptonDockspace.cs`.
- [ ] Read Krypton `Control Docking\KryptonSpace.cs`.
- [ ] Read Krypton `Elements Impl\KryptonDockingDockspace.cs`.
- [ ] Read DockPanelSuite dock pane/tab strip design-time behavior.
- [ ] Map Beep classes to Krypton concepts before modifying code.

## Priority 4 - Theme and Icons

- [ ] Keep close/pin/dropdown/tab icons in `DockingCaptionPainter`.
- [ ] Keep icon rendering path-based through `StyledImagePainter` and `SvgsUIcons`.
- [ ] Recolor icons based on active/inactive tab/header background for readability.
- [ ] Verify all header styles render readable text and icons.

## Required Verification

Do not mark a task complete unless all relevant checks are done:

- [ ] `dotnet build "TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" --no-restore -v:q /clp:ErrorsOnly`
- [ ] `dotnet build "TheTechIdea.Beep.Winform.Controls.Design.Server\TheTechIdea.Beep.Winform.Controls.Design.Server.csproj" --no-restore -v:q /clp:ErrorsOnly`
- [ ] Reopen Visual Studio designer after rebuild.
- [ ] Manually test dockspace header click.
- [ ] Manually test dockspace header drag.
- [ ] Manually test `DockPosition`.
- [ ] Manually test resize.
