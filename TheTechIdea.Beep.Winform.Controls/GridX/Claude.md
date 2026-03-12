# Claude.md - BeepGridPro Implementation Guide

This file is for AI/code agents working inside `GridX`. Prefer this over older status docs when behavior questions come up.

## Source of Truth
- Public surface: `BeepGridPro.cs`, `BeepGridPro.Properties.cs`, `BeepGridPro.*.cs`
- Data model: `Models/BeepColumnConfig.cs`, `Models/BeepRowConfig.cs`, `Models/BeepCellConfig.cs`
- Behavior helpers: `GridX/Helpers/*.cs`
- Design-time surface: `Controls.Design.Server/Designers/BeepGridProDesigner.cs`

When docs or plans disagree with code, treat the code as authoritative.

## Mental Model
`BeepGridPro` is a thin coordinator over helper objects. It owns the helpers, public properties, and public methods, but most behavior lives here:

| Area | Primary files |
|------|---------------|
| Binding and rows | `GridDataHelper.cs`, `GridDataHelper.Rows.cs`, `GridDataHelper.ValueConversion.cs` |
| Layout | `GridLayoutHelper.cs` |
| Rendering | `GridRenderHelper.cs`, `GridRenderHelper.Rendering.cs`, `GridRenderHelper.CellContent.cs` |
| Input | `GridInputHelper.cs` |
| Selection | `GridSelectionHelper.cs` |
| Navigator and CRUD | `GridNavigatorHelper.cs` |
| Sort/filter | `GridSortFilterHelper.cs`, `BeepGridPro.Filtering.cs` |
| Dialogs | `GridDialogHelper.cs`, `GridDialogHelper.*.cs`, `BeepGridPro.Dialogs.cs` |
| Auto-size | `GridSizingHelper.cs` |
| UOW bridge | `GridUnitOfWorkBinder.cs` |

## High-Signal Invariants

### 1. Two filter systems exist
- `BeepGridPro.Filtering.cs` owns `ActiveFilter`, `ApplyQuickFilter`, `ShowAdvancedFilterDialog`, `FilterApplied`, and `FilterCleared`.
- `GridSortFilterHelper` owns the `SortFilter` pipeline used by header popup filtering and binding-source sort/filter attempts.
- They are related in the UI but not the same internal state. Do not document them as one unified engine.

### 2. UOW mode is interface-driven only
- `Uow` stores `object?`, but the setter only casts to `IUnitofWork` and `IUnitOfWorkWrapper`.
- There is no automatic `UnitofWork<T>` wrapping inside `BeepGridPro`.
- If a screen assigns a plain runtime UOW type, it must already satisfy one of those interfaces or be wrapped externally.

### 3. `LayoutPreset` enum is ahead of the property switch
- `GridLayoutPreset` contains modern values such as `Material3Surface`, `Fluent2Standard`, `TailwindProse`, and `AGGridAlpine`.
- `BeepGridPro.ApplyLayoutPreset(GridLayoutPreset)` currently switches only the legacy preset group.
- Direct class application through `grid.ApplyLayoutPreset(new SomeLayout())` works because the extension overload accepts `IGridLayoutPreset`.

### 4. `BeepGridStyle.Modern` is not implemented in `ApplyGridStyle()`
- The enum exists in `TheTechIdea.Beep.Vis.Modules`.
- The current switch in `BeepGridPro.ApplyGridStyle()` has no `Modern` branch.

### 5. Top filter panel owns click handling
- `GridInputHelper.HandleMouseDown()` returns early for any click inside `TopFilterRect`.
- Do not let new logic fall through from top-panel hit tests into cell selection.
- The inline quick search controls are real `BeepComboBox` and `BeepTextBox` controls painted as static images until activated.

## Public API Notes That Matter

### Binding
- `DataSource` clears the grid when set to `null`.
- `DataMember` is respected for `DataSet`, `DataViewManager`, root-object properties, and `BindingSource` resolution.
- `AutoGenerateColumns()` clears existing columns, re-adds system columns, and then reflects schema/properties.

### System columns
`EnsureSystemColumns()` guarantees:
- `Sel`: sticky checkbox, hidden unless `ShowCheckBox`
- `RowNum`: sticky row number
- `RowID`: sticky hidden row identity

These are unbound helper columns. Skip them in new sort/filter/auto-size/reorder features unless there is a specific reason not to.

### Selection
- Active-cell selection is tracked by `GridSelectionHelper`.
- Checkbox row selection is separate and is what powers `SelectedRows`, `SelectedRowIndices`, and `RowSelectionChanged`.
- `SelectionMode` is publicly exposed, but the current interaction code still behaves primarily like active-cell focus plus row-checkbox selection.

### Editing
- `GridEditHelper.BeginEdit()` is the main entry point from input handlers.
- `OnCellValueChanged()` triggers the public `CellValueChanged` event and also calls `RequestAutoSize(AutoSizeTriggerSource.EditCommit)`.
- If you add a new editor flow, preserve that callback path.

### CRUD
- In regular mode, CRUD goes through `BindingSource`.
- In UOW mode, navigator actions call UOW methods directly and then refresh rows through `GridNavigatorHelper` and `GridUnitOfWorkBinder`.
- Wrapper-only UOW mode forwards synthetic lifecycle events via `WrapperEventForwarded`.

## Rendering Rules
- Do not add child controls for cells, headers, navigator, or scrollbars.
- Do not patch `DrawContent()` unless the overall rendering pipeline is changing.
- Prefer changing helper code:
  - cell visuals: `GridRenderHelper`
  - filter panel visuals: `Painters/*FilterPanelPainter.cs`
  - header visuals: `Painters/*HeaderPainter.cs`
  - navigator visuals: `Painters/*NavigationPainter.cs`
- Always guard against `Rectangle.Empty` or zero-width/height regions before drawing.

## Layout Rules
- Call `Layout.Recalculate()` when a change affects geometry.
- Call `ScrollBars.UpdateBars()` after geometry or content-size changes.
- `SafeInvalidate(rect)` is preferred when a localized redraw is enough.
- `SafeRecalculate()` exists specifically to avoid recursive layout work while `Layout.IsCalculating` is true.

## Input Rules
- `GridInputHelper` handles resize, reorder, filter-panel actions, header icon hits, cell selection, and keyboard shortcuts.
- Column reorder is restricted to visible, non-sticky, non-system columns with `AllowReorder = true`.
- Double-click on a column border best-fits one column; `Ctrl` + double-click best-fits all visible columns.

## Common Mistakes to Avoid
- Do not state that all `GridLayoutPreset` enum values are property-wired.
- Do not state that `FilterApplied` fires for Excel-style popup filtering.
- Do not state that `Uow` accepts arbitrary runtime UOW objects without wrapping.
- Do not bypass `GridDataHelper.UpdateCellValue()` if the underlying row object must stay synchronized.
- Do not forget that `BindingSource.DataSourceChanged` triggers full column regeneration.

## Safe Change Patterns

### Add a new helper
1. Add the helper class under `GridX/Helpers`.
2. Inject `BeepGridPro` in the constructor.
3. Initialize it in the `BeepGridPro` constructor.
4. Keep cross-helper coordination through `_grid`.

### Add a new layout class
1. Implement `IGridLayoutPreset`.
2. Apply it directly with the extension overload first.
3. Only extend the `GridLayoutPreset` property switch if you want enum-based property support too.

### Add a new navigation or header painter
1. Implement the relevant painter interface/base class.
2. Register it in the factory.
3. Update any enum mappings and recommended-size logic.

## Useful Breakpoints
- `GridDataHelper.Bind`
- `GridDataHelper.RefreshRows`
- `GridNavigatorHelper.Save`
- `GridSortFilterHelper.Sort`
- `GridSortFilterHelper.FilterIn`
- `BeepGridPro.ApplyActiveFilter`
- `GridInputHelper.HandleMouseDown`
- `GridRenderHelper.Draw`

## Preferred Documentation Language
When you update docs for this area, be explicit about:
- what is public API
- what is helper/internal behavior
- what is implemented now
- what exists in enums or plan files but is not yet wired
