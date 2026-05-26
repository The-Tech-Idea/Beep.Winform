# Phase 0 — Krypton Base Infrastructure

> **CRITICAL**: Copy the EXACT code and logic from Krypton. Only change namespace/class/method names.
> Do NOT rewrite, simplify, stub, or reinterpret. Create EVERY base class fully.
> No compatibility concerns. No legacy.

## Goal

Create ALL base classes that Krypton's Navigator, Workspace, and Docking systems depend on. Without these, nothing else compiles.

## Krypton Source Files to Copy

All files are under:
`C:\Users\f_ald\source\repos\The-Tech-Idea\Standard-Toolkit-master\Standard-Toolkit-master\Source\Krypton Components\Krypton.Toolkit\`

### Task 0.1 — Control Base Classes

| # | Krypton File | Beep File | Location |
|---|-------------|-----------|----------|
| 0.1.1 | `VisualControlBase.cs` | `BeepVisualControlBase.cs` | `Controls/Docking/Base/` |
| 0.1.2 | `VisualControlContainment.cs` | `BeepVisualControlContainment.cs` | `Controls/Docking/Base/` |
| 0.1.3 | `VisualSimpleBase.cs` | `BeepVisualSimpleBase.cs` | `Controls/Docking/Base/` |
| 0.1.4 | `VisualForm.cs` | `BeepVisualForm.cs` | `Controls/Docking/Base/` |
| 0.1.5 | `VisualPanel.cs` | `BeepVisualPanel.cs` | `Controls/Docking/Base/` |

**Process for each file**:
1. Open the Krypton source file
2. Copy its ENTIRE content
3. Rename namespace: `Krypton.Toolkit` → `TheTechIdea.Beep.Winform.Controls.Docking.Base`
4. Rename class: `VisualControlBase` → `BeepVisualControlBase`, etc.
5. Rename constructors to match new class name
6. Find-and-replace internal Krypton type references:
   - `Krypton.Toolkit.` → `TheTechIdea.Beep.Winform.Controls.Docking.Base.` (or appropriate sub-namespace)
   - `NeedPaintHandler` → `BeepNeedPaintHandler`
   - `PaletteBase` → `BeepPaletteBase`
   - etc. — use the [function resolution matrix](./function-resolution-matrix.md)
7. Any Krypton type that doesn't have a Beep equivalent yet: create it in this phase too

### Task 0.2 — View System Base Classes

| # | Krypton File | Beep File | Location |
|---|-------------|-----------|----------|
| 0.2.1 | `ViewBase.cs` | `BeepViewBase.cs` | `Controls/Docking/View/` |
| 0.2.2 | `ViewComposite.cs` | `BeepViewComposite.cs` | `Controls/Docking/View/` |
| 0.2.3 | `ViewDrawPanel.cs` | `BeepViewDrawPanel.cs` | `Controls/Docking/View/` |
| 0.2.4 | `ViewManager.cs` | `BeepViewManager.cs` | `Controls/Docking/View/` |
| 0.2.5 | `ViewLayoutDocker.cs` | `BeepViewLayoutDocker.cs` | `Controls/Docking/View/` |
| 0.2.6 | `ViewLayoutFill.cs` | `BeepViewLayoutFill.cs` | `Controls/Docking/View/` |
| 0.2.7 | `ViewDrawCanvas.cs` | `BeepViewDrawCanvas.cs` | `Controls/Docking/View/` |
| 0.2.8 | `ViewDrawDocker.cs` | `BeepViewDrawDocker.cs` | `Controls/Docking/View/` |
| 0.2.9 | `ViewDrawButton.cs` | `BeepViewDrawButton.cs` | `Controls/Docking/View/` |
| 0.2.10 | `ViewLayoutSeparator.cs` | `BeepViewLayoutSeparator.cs` | `Controls/Docking/View/` |

**Important**: Search the Krypton toolkit for ALL View* classes referenced by KryptonNavigator or KryptonWorkspace. Create every single one.

### Task 0.3 — Palette System

| # | Krypton File | Beep File | Location |
|---|-------------|-----------|----------|
| 0.3.1 | `PaletteBase.cs` | `BeepPaletteBase.cs` | `Controls/Docking/Palette/` |
| 0.3.2 | `PaletteRedirect.cs` | `BeepPaletteRedirect.cs` | `Controls/Docking/Palette/` |
| 0.3.3 | `PaletteDoubleRedirect.cs` | `BeepPaletteDoubleRedirect.cs` | `Controls/Docking/Palette/` |
| 0.3.4 | `PaletteBack.cs` | `BeepPaletteBack.cs` | `Controls/Docking/Palette/` |
| 0.3.5 | `PaletteBorder.cs` | `BeepPaletteBorder.cs` | `Controls/Docking/Palette/` |
| 0.3.6 | `PaletteContent.cs` | `BeepPaletteContent.cs` | `Controls/Docking/Palette/` |
| 0.3.7 | `PaletteDragDrop.cs` | `BeepPaletteDragDrop.cs` | `Controls/Docking/Palette/` |
| 0.3.8 | `IPaletteDragDrop.cs` | `IBeepPaletteDragDrop.cs` | `Controls/Docking/Palette/` |
| 0.3.9 | `IRenderer.cs` | `IBeepRenderer.cs` | `Controls/Docking/Palette/` |
| 0.3.10 | `RenderBase.cs` | `BeepRenderBase.cs` | `Controls/Docking/Palette/` |
| 0.3.11 | `RenderStandard.cs` | `BeepRenderStandard.cs` | `Controls/Docking/Palette/` |

### Task 0.4 — Utility Types, Events, Enums, Helpers

| # | Krypton File | Beep File | Location |
|---|-------------|-----------|----------|
| 0.4.1 | `TypedCollection.cs` | `BeepTypedCollection.cs` | `Controls/Docking/Collections/` |
| 0.4.2 | `BoolFlags31.cs` | `BeepBoolFlags31.cs` | `Controls/Docking/Helpers/` |
| 0.4.3 | `CommonHelper.cs` | `BeepCommonHelper.cs` | `Controls/Docking/Helpers/` |
| 0.4.4 | ALL EventArgs classes referenced by Navigator/Workspace | Create each | `Controls/Docking/Events/` |
| 0.4.5 | ALL enums referenced by Navigator/Workspace | Create each | `Controls/Docking/Enums/` |
| 0.4.6 | ALL interfaces referenced (`IWorkspaceItem`, etc.) | Create each | `Controls/Docking/Interfaces/` |

### Task 0.5 — Delegate Types

Find ALL delegate types used in the copied Krypton code (e.g., `NeedPaintHandler`) and create Beep equivalents:

| # | Krypton Delegate | Beep Delegate | 
|---|-----------------|---------------|
| 0.5.1 | `NeedPaintHandler` | `BeepNeedPaintHandler` |
| 0.5.2 | Any other delegates found | Rename with `Beep` prefix |

## Build Verification

After completing each task, build `TheTechIdea.Beep.Winform.Controls.csproj` with:
```
dotnet build "TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj" -v:q /clp:ErrorsOnly
```

Fix ALL errors before proceeding to the next task. Errors at this phase should only be:
- Missing references to other base classes being created in this phase
- Self-referential compilation errors (fixed as more files are added)

## Dependencies

Phase 0 must be COMPLETELY DONE (0 build errors) before starting Phase 1.
