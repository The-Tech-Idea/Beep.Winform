# Function & Type Resolution Matrix

Maps EVERY Krypton namespace, class, method, property, event, enum, and parameter to its Beep equivalent.

## Namespace Map

| Krypton Namespace | Beep Namespace |
|-------------------|---------------|
| `Krypton.Toolkit` | `TheTechIdea.Beep.Winform.Controls` |
| `Krypton.Navigator` | `TheTechIdea.Beep.Winform.Controls.Docking` |
| `Krypton.Workspace` | `TheTechIdea.Beep.Winform.Controls.Docking` |
| `Krypton.Docking` | `TheTechIdea.Beep.Winform.Controls.Docking` |
| `ComponentFactory.Krypton.Toolkit` | (remove — use Beep equivalents) |

## Class Map — Base Infrastructure (Phase 0)

| Krypton | Beep | Base Class |
|---------|------|-----------|
| `VisualControlBase` | `BeepVisualControlBase` | `Control` |
| `VisualControlContainment` | `BeepVisualControlContainment` | `Control` |
| `VisualSimpleBase` | `BeepVisualSimpleBase` | `BeepVisualControlBase` |
| `VisualForm` | `BeepVisualForm` | `Form` |
| `ViewBase` | `BeepViewBase` | `Component` |
| `ViewComposite` | `BeepViewComposite` | `BeepViewBase` |
| `ViewDrawPanel` | `BeepViewDrawPanel` | `BeepViewComposite` |
| `ViewManager` | `BeepViewManager` | `Component` |
| `ViewLayoutDocker` | `BeepViewLayoutDocker` | `BeepViewComposite` |
| `ViewLayoutFill` | `BeepViewLayoutFill` | `BeepViewBase` |
| `ViewDrawCanvas` | `BeepViewDrawCanvas` | `BeepViewComposite` |
| `ViewDrawDocker` | `BeepViewDrawDocker` | `BeepViewDrawCanvas` |
| `ViewDrawButton` | `BeepViewDrawButton` | `BeepViewComposite` |
| `ViewLayoutSeparator` | `BeepViewLayoutSeparator` | `BeepViewComposite` |
| `PaletteBase` | `BeepPaletteBase` | `Component` |
| `PaletteRedirect` | `BeepPaletteRedirect` | `BeepPaletteBase` |
| `PaletteDoubleRedirect` | `BeepPaletteDoubleRedirect` | `BeepPaletteBase` |
| `PaletteBack` | `BeepPaletteBack` | `Component` |
| `PaletteBorder` | `BeepPaletteBorder` | `Component` |
| `PaletteContent` | `BeepPaletteContent` | `Component` |
| `PaletteDragDrop` | `BeepPaletteDragDrop` | (keep as-is reference) |
| `NeedPaintHandler` | `BeepNeedPaintHandler` | Delegate |
| `IRenderer` | `IBeepRenderer` | Interface |

## Class Map — Navigator (Phase 1)

| Krypton | Beep | Notes |
|---------|------|-------|
| `KryptonNavigator` | `BeepNavigator` | `BeepVisualControlContainment` |
| `KryptonPage` | `BeepPage` | `BeepVisualPanel` (new) |
| `KryptonPageCollection` | `BeepPageCollection` | `TypedCollection<BeepPage>` |
| `KryptonPageFlags` | `BeepPageFlags` | Enum, bit flags |
| `KryptonPageFlagsEventArgs` | `BeepPageFlagsEventArgs` | `EventArgs` |
| `UniqueNameEventArgs` | `BeepUniqueNameEventArgs` | same pattern |
| `UniqueNamesEventArgs` | `BeepUniqueNamesEventArgs` | same pattern |
| `CloseActionEventArgs` | `BeepCloseActionEventArgs` | same pattern |
| `CancelUniqueNameEventArgs` | `BeepCancelUniqueNameEventArgs` | same pattern |
| `KryptonContextMenu` | `BeepContextMenu` | `ContextMenuStrip` wrapper |
| `KryptonContextMenuItems` | (keep ContextMenuStrip) | Use native |
| `TypedCollection<T>` | `BeepTypedCollection<T>` | Generic collection |
| `ButtonSpec` | `BeepButtonSpec` | (simplify to wrapper) |
| `ButtonSpecNavigator` | `BeepButtonSpecNavigator` | (simplify) |
| `NavigatorMode` | `BeepNavigatorMode` | Enum |
| `CompactFlags` | `BeepCompactFlags` | Enum |
| `VisualPanel` | `BeepVisualPanel` | (extends Panel, simple) |

## Class Map — Workspace (Phase 2)

| Krypton | Beep | Notes |
|---------|------|-------|
| `KryptonWorkspace` | `BeepWorkspace` | `BeepVisualContainerControl` |
| `KryptonWorkspaceCell` | `BeepWorkspaceCell` | `BeepNavigator` + `IBeepWorkspaceItem` |
| `KryptonWorkspaceSequence` | `BeepWorkspaceSequence` | `Component` + `IBeepWorkspaceItem` |
| `KryptonWorkspaceCollection` | `BeepWorkspaceCollection` | `BeepTypedCollection<IBeepWorkspaceItem>` |
| `IWorkspaceItem` | `IBeepWorkspaceItem` | Interface |
| `StarSize` | `BeepStarSize` | Struct |
| `StarNumber` | `BeepStarNumber` | Struct |
| `SeparatorStyle` | `BeepSeparatorStyle` | Enum |
| `ViewDrawWorkspaceSeparator` | `BeepViewDrawWorkspaceSeparator` | `BeepViewComposite` |
| `WorkspaceCellEventArgs` | `BeepWorkspaceCellEventArgs` | `EventArgs` |
| `ActivePageChangedEventArgs` | `BeepActivePageChangedEventArgs` | `EventArgs` |
| `ActiveCellChangedEventArgs` | `BeepActiveCellChangedEventArgs` | `EventArgs` |
| `MaximizeRestoreEventArgs` | `BeepMaximizeRestoreEventArgs` | `EventArgs` |
| `XmlSavingEventArgs` | `BeepXmlSavingEventArgs` | `EventArgs` |
| `DragManager` | `BeepDragManager` | `Component` |

## Class Map — Docking (Phases 3-6)

| Krypton | Beep | Notes |
|---------|------|-------|
| `DockingElement` | `BeepDockingElement` | `Component` + `IBeepDockingElement` |
| `DockingElementOpenCollection` | `BeepDockingElementOpenCollection` | `BeepDockingElement` |
| `DockingElementClosedCollection` | `BeepDockingElementClosedCollection` | `BeepDockingElement` |
| `IDockingElement` | `IBeepDockingElement` | Interface |
| `DockingPropogateAction` | `BeepDockingPropogateAction` | Enum |
| `DockingPropogateIntState` | `BeepDockingPropogateIntState` | Enum |
| `DockingPropogateBoolState` | `BeepDockingPropogateBoolState` | Enum |
| `DockingPropogatePageList` | `BeepDockingPropogatePageList` | Enum |
| `DockingLocation` | `BeepDockingLocation` | Enum |
| `KryptonDockingManager` | `BeepDockingManager` | `BeepDockingElementOpenCollection` |
| `KryptonDockingControl` | `BeepDockingControl` | `BeepDockingElementClosedCollection` |
| `KryptonDockingEdge` | `BeepDockingEdge` | `BeepDockingElementClosedCollection` |
| `KryptonDockingEdgeDocked` | `BeepDockingEdgeDocked` | `BeepDockingElementClosedCollection` |
| `KryptonDockingEdgeAutoHidden` | `BeepDockingEdgeAutoHidden` | `BeepDockingElementClosedCollection` |
| `KryptonDockingDockspace` | `BeepDockingDockspace` | `BeepDockingSpace` |
| `KryptonDockingSpace` | `BeepDockingSpace` | `BeepDockingElementClosedCollection` |
| `KryptonDockingFloatspace` | `BeepDockingFloatspace` | `BeepDockingSpace` |
| `KryptonDockingFloatingWindow` | `BeepDockingFloatingWindow` | `BeepDockingElementClosedCollection` |
| `KryptonDockingFloating` | `BeepDockingFloating` | `BeepDockingElementClosedCollection` |
| `KryptonDockingAutoHiddenGroup` | `BeepDockingAutoHiddenGroup` | `BeepDockingElementClosedCollection` |
| `KryptonDockingWorkspace` | `BeepDockingWorkspace` | `BeepDockingSpace` |
| `KryptonDockingNavigator` | `BeepDockingNavigator` | `BeepDockingSpace` |
| `KryptonDockspace` | `BeepDockspace` | `BeepSpace` |
| `KryptonSpace` | `BeepSpace` | `BeepWorkspace` |
| `KryptonFloatspace` | `BeepFloatspace` | `BeepSpace` |
| `KryptonDockableWorkspace` | `BeepDockableWorkspace` | `BeepWorkspace` |
| `KryptonDockableNavigator` | `BeepDockableNavigator` | `BeepNavigator` |
| `KryptonFloatingWindow` | `BeepFloatingWindow` | `BeepVisualForm` |
| `KryptonDockspaceSeparator` | `BeepDockspaceSeparator` | `Control` |
| `KryptonAutoHiddenGroup` | `BeepAutoHiddenGroup` | `Panel` |
| `KryptonAutoHiddenPanel` | `BeepAutoHiddenPanel` | `Panel` |
| `KryptonAutoHiddenSlidePanel` | `BeepAutoHiddenSlidePanel` | `Panel` |
| `KryptonAutoHiddenProxyPage` | `BeepAutoHiddenProxyPage` | `BeepPage` |
| `KryptonStorePage` | `BeepStorePage` | `BeepPage` |
| `DockingDragManager` | `BeepDockingDragManager` | `BeepDragManager` |
| `DragTarget` | `BeepDragTarget` | (struct/class) |
| `DragTargetList` | `BeepDragTargetList` | `List<BeepDragTarget>` |
| `DragTargetControlEdge` | `BeepDragTargetControlEdge` | `BeepDragTarget` |
| `DragTargetWorkspaceCellEdge` | `BeepDragTargetWorkspaceCellEdge` | `BeepDragTarget` |
| `DragTargetWorkspaceEdge` | `BeepDragTargetWorkspaceEdge` | `BeepDragTarget` |
| `DragTargetFloatingWindow` | `BeepDragTargetFloatingWindow` | `BeepDragTarget` |
| `PageDragEndData` | `BeepPageDragEndData` | `EventArgs` |
| `DockspaceEventArgs` | `BeepDockspaceEventArgs` | `EventArgs` |
| `DockspaceCellEventArgs` | `BeepDockspaceCellEventArgs` | `EventArgs` |
| `DockspaceSeparatorResizeEventArgs` | `BeepDockspaceSeparatorResizeEventArgs` | `EventArgs` |
| `AutoHiddenGroupEventArgs` | `BeepAutoHiddenGroupEventArgs` | `EventArgs` |
| `CancelDropDownEventArgs` | `BeepCancelDropDownEventArgs` | `EventArgs` |

## Class Map — Designers (Phase 7)

| Krypton | Beep | Notes |
|---------|------|-------|
| `KryptonNavigatorDesigner` | `BeepNavigatorDesigner` | `ParentControlDesigner` |
| `KryptonWorkspaceDesigner` | `BeepWorkspaceDesigner` | `ParentControlDesigner` |
| `KryptonWorkspaceCellDesigner` | `BeepWorkspaceCellDesigner` | `BeepNavigatorDesigner` |
| `KryptonWorkspaceSequenceDesigner` | `BeepWorkspaceSequenceDesigner` | `ComponentDesigner` |
| `KryptonPageDesigner` | `BeepPageDesigner` | `ScrollableControlDesigner` |

## Mapping Parameters

### VisualControlBase Constructor
Krypton: (no args or various — read from source)
Beep: Same signature, just rename class references inside body

### KryptonNavigator Constructor
Krypton: `public KryptonNavigator()`
Beep: `public BeepNavigator()` — body unchanged except internal class refs

### KryptonWorkspaceCell Constructor
Krypton: `public KryptonWorkspaceCell() : this("50*,50*")` / `public KryptonWorkspaceCell(string starSize)`
Beep: `public BeepWorkspaceCell() : this("50*,50*")` / `public BeepWorkspaceCell(string starSize)`

### PropogateAction Signature
Krypton: `public override void PropogateAction(DockingPropogateAction action, int value)`
Beep: `public override void PropogateAction(BeepDockingPropogateAction action, int value)`

### ManageControl Signature
Krypton: `public void ManageControl(string name, Control c)`
Beep: `public void ManageControl(string name, Control c)`

### Page Collection
Krypton: `KryptonPageCollection`
Beep: `BeepPageCollection : BeepTypedCollection<BeepPage>`

## Krypton Base Class Dependency Tree (Everything that must be created)

```
Control
  └─ BeepVisualControlBase (base for all Beep controls)
       ├─ BeepVisualSimpleBase
       │    └─ BeepVisualControlContainment
       │         └─ BeepNavigator ← THE KEY CLASS
       │              └─ BeepWorkspaceCell
       │              └─ BeepDockableNavigator
       └─ BeepVisualForm
            └─ BeepFloatingWindow

Panel
  └─ BeepVisualPanel
       └─ BeepPage

Component
  └─ BeepPaletteBase
  ├─ BeepViewBase
  │    └─ BeepViewComposite
  │         ├─ BeepViewDrawPanel
  │         ├─ BeepViewLayoutDocker
  │         ├─ BeepViewLayoutFill
  │         ├─ BeepViewDrawCanvas
  │         │    └─ BeepViewDrawDocker
  │         ├─ BeepViewDrawButton
  │         ├─ BeepViewLayoutSeparator
  │         └─ BeepViewDrawWorkspaceSeparator
  ├─ BeepViewManager (manages View hierarchy rendering)
  ├─ BeepDockingElement
  │    ├─ BeepDockingElementOpenCollection
  │    │    └─ BeepDockingManager
  │    └─ BeepDockingElementClosedCollection
  │         ├─ BeepDockingSpace
  │         │    ├─ BeepDockingDockspace
  │         │    ├─ BeepDockingFloatspace
  │         │    ├─ BeepDockingWorkspace
  │         │    └─ BeepDockingNavigator
  │         ├─ BeepDockingControl
  │         ├─ BeepDockingEdge
  │         ├─ BeepDockingEdgeDocked
  │         ├─ BeepDockingEdgeAutoHidden
  │         ├─ BeepDockingAutoHiddenGroup
  │         ├─ BeepDockingFloatingWindow
  │         └─ BeepDockingFloating
  └─ BeepWorkspaceSequence

Control (standalone)
  └─ BeepDockspaceSeparator
  └─ BeepAutoHiddenPanel
  └─ BeepAutoHiddenSlidePanel
  └─ BeepAutoHiddenGroup
```
