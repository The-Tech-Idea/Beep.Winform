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
| Grouping | `GridGroupEngine.cs`, `GridGroupHeaderRenderer.cs`, `GridGroupSummaryRow.cs`, `DefaultGridGrouper.cs` |
| Virtualization | `GridRowVirtualizer.cs`, `GridColumnVirtualizer.cs`, `GridVirtualDataSource.cs`, `IVirtualDataSource.cs` |
| Keyboard / Accessibility | `GridKeyboardNavigator.cs`, `GridFocusManager.cs` |
| Editor framework | `GridEditHelper.cs`, `GridEditorFactory.cs`, `Editors/*.cs` |
| Export | `GridExportEngine.cs`, `IGridExporter.cs`, `GridCsvExporter.cs`, `GridJsonExporter.cs`, `GridHtmlExporter.cs`, `GridExcelExporterStub.cs`, `GridPdfExporterStub.cs` |

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

### 6. Grouping and sort pipeline integration
- When grouping is active, `GridSortFilterHelper.Sort()` does NOT globally reorder rows.
- Sorting a **group column** updates the descriptor and recomputes groups (affects group ordering).
- Sorting a **non-group column** calls `GroupEngine.SortWithinGroups()`, preserving group structure.
- `ApplyLocalSort()` is bypassed entirely when `_grid.GroupEngine.IsGrouped` is true.

### 7. Virtualization owns the visible window
- In virtual mode, `Data.Rows` contains ONLY the visible window (not all rows).
- `FirstVisibleRowIndex` returns `0` in virtual mode because `Data.Rows[0]` is always the first visible row.
- `GridRowVirtualizer.PublishToGrid()` materializes the window and `SyncGridRowCount()` triggers layout recalc.
- `GridVirtualDataSource` provides factory methods: `FromList()`, `FromDataTable()`, `FromDataView()`.

### 8. Editor framework uses factory pattern
- `GridEditHelper` no longer contains a big `CreateEditorForColumn()` switch.
- `GridEditorFactory` resolves `IGridEditor` instances by `BeepColumnType`.
- Each editor class (`BeepGridTextEditor`, `BeepGridComboBoxEditor`, etc.) encapsulates creation, setup, value access, and event handling.
- `IGridEditorEvents` bridges editor lifecycle callbacks back into `GridEditHelper.EndEdit()`.
- Custom editors can be registered at runtime via `GridEditorFactory.Register(BeepColumnType, IGridEditor)`.

### 9. Keyboard navigation delegates to `GridKeyboardNavigator`
- `GridInputHelper.HandleKeyDown` delegates navigation keys to `KeyboardNavigator`.
- `ProcessDialogKey` override in `BeepGridPro` intercepts `Tab`/`Shift+Tab` before WinForms moves focus out of the grid.
- `GridFocusManager` tracks `HasFocus` and draws a focus indicator via `DrawFocusIndicator` in the render pipeline.

### 10. Group summary rows are computed and rendered per group
- `GridGroupSummaryRow` stores aggregate values per column for each `GridGroup`.
- `GridGroupAggregateEngine.ComputeForGroup()` computes `Sum`, `Average`, `Count`, `Min`, `Max`, `First`, `Last`, `DistinctCount` using `BeepColumnConfig.AggregationType`.
- Summary rows participate in total height (`GetTotalContentHeight`) and per-row height-before (`GetTotalHeightBeforeRow`) calculations.
- `DrawSummaryRows()` renders summary rows in a dedicated pass after both sticky and scrolling column passes.
- Summary rows only appear when the group is expanded (`IsCollapsed == false`).

### 11. Accessibility uses `Control.ControlAccessibleObject` hierarchy
- `BeepGridPro.CreateAccessibilityInstance()` returns `GridAccessibleObject` (root, `Role = Table`).
- `GridRowAccessibleObject` (`Role = Row`) is a child of the root; children are cells.
- `GridCellAccessibleObject` (`Role = Cell`) exposes `Name` (column caption), `Value` (cell text), `Bounds`, and `State` (Selected/Focused).
- Navigation: `Next`/`Previous` move between cells; `Up`/`Down`/`Left`/`Right` navigate the grid.
- `HitTest` maps screen coordinates to the cell accessible object under the cursor.

### 12. Column virtualization skips off-screen scrolling columns
- `GridColumnVirtualizer` tracks a horizontal window of scrolling (non-sticky) columns.
- `EnableColumnVirtualization` activates it; disabled by default.
- `UpdateWindow()` is called from `GridLayoutHelper.Recalculate()` after horizontal scroll changes.
- `DrawRows()` and `DrawSummaryRowContent()` iterate only `FirstScrollingVisibleIndex`..`LastScrollingVisibleIndex` instead of all scrolling columns.
- Sticky columns are never virtualized (always rendered).

### 13. Export engine uses plugin discovery for heavy dependencies
- Built-in exporters (CSV, JSON, HTML) live in the main assembly and are always available.
- `Excel` and `Pdf` exporters are plugin-based to avoid heavy NuGet dependencies (ClosedXML, EPPlus, PdfSharpCore).
- `GridExcelExporterStub` and `GridPdfExporterStub` are registered by default with `IsAvailable = false`.
- `GridExportEngine.DiscoverPlugins()` scans `AppDomain.CurrentDomain.GetAssemblies()` for `IGridExporter` implementations.
- If a real plugin assembly is loaded (e.g., `TheTechIdea.Beep.Winform.Controls.GridX.Export.Excel`), it replaces the stub.
- UI menus check `ExportEngine.IsAvailable(format)` to gray out unavailable formats.

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
