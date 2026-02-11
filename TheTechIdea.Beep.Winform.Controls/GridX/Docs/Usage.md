# Using BeepGridPro

Quick binding
- Set `DataSource` to a `BindingSource`, any `IEnumerable<T>`, `DataTable`/`DataView`, or a root object with a property indicated by `DataMember`.
- Optionally set `DataMember` when using `DataSet` or a root object containing the actual list.
- Call `AutoGenerateColumns()` or configure `Columns` at design-time.

Example
```csharp
beepGridPro1.DataSource = customersBindingSource; // or a List<Customer>
beepGridPro1.DataMember = "Customers";          // optional
beepGridPro1.AutoGenerateColumns();
beepGridPro1.EnableExcelFilter();
```

Columns
- Columns live in `beepGridPro1.Columns` (`BeepGridColumnConfigCollection`).
- System columns are auto-inserted:
  - `Sel` (selection checkbox, sticked, width from `Layout.CheckBoxColumnWidth`)
  - `RowNum` (#, sticked, readonly)
  - `RowID` (hidden)
- To ensure they exist: `beepGridPro1.EnsureSystemColumns()`.
- To toggle selection column: set `beepGridPro1.ShowCheckBox`.
- To auto-generate from entity/schema: `beepGridPro1.AutoGenerateColumns()`.

Selection
- Row selection is checkbox-based. Header checkbox toggles all when `MultiSelect` is true.
- Active cell is highlighted separately and does not alter row selection.
- Programmatic select: `beepGridPro1.SelectCell(rowIndex, colIndex)`.
- Events: `RowSelectionChanged` receives row index and row object.

Editing
- Call `beepGridPro1.ShowCellEditor()` to start editing the active cell.
- Enter/F2 starts edit via keyboard when cell editable and not a checkbox.
- Editors are chosen from column `CellEditor` (Text, ComboBox, DateTime, CheckBox*, NumericUpDown, Image, Radio, ListBox, ListOfValue, ProgressBar, Button).
- Values from list editors (`SimpleItem`) are normalized to target property type automatically.
- After commit, `CellValueChanged` fires.

Sorting and filtering
- Click header text or sort icon to toggle sort direction (if `AllowSort`).
- Excel-style filter popup: `beepGridPro1.EnableExcelFilter()`. Hover header shows icons; click filter icon to open.
- Programmatic sort: `SortFilter.Sort(columnName, direction)`.
- Programmatic simple filter: `SortFilter.Filter(columnName, contains)`.

Scrolling
- Custom scrollbars with drag-to-scroll and paging clicks.
- Mouse wheel scrolling supported. Variable row heights are respected.

Styling and theming
- `GridStyle` presets change stripes, grid lines, gradients, header text weight, and padding.
- Themes via `Theme` property (name) from `BeepThemesManager` affect colors and fonts.
- Adjust row/header heights via `RowHeight`, `ColumnHeaderHeight`.

Navigator
- Owner-drawn navigator area (footer) with CRUD and record navigation.
- Attach an external `BeepBindingNavigator` via `AttachNavigator(navigator, dataSource)` or use no visual navigator (owner drawn only).
- Use actions: `MoveFirst/Prev/Next/Last`, `InsertNew`, `DeleteCurrent`, `Save`, `Cancel`.

UOW mode
- Assign `Uow` to either `IUnitofWork` or `IUnitOfWorkWrapper`.
- If a runtime `UnitofWork<T>` instance is assigned, grid wraps it in `UnitOfWorkWrapper` automatically.
- `Uow` mode is authoritative; `DataSource` value is retained and applied automatically when `Uow` is cleared.
- Grid UOW binding/events use typed contracts only (no reflection in `BeepGridPro` UOW path).
- Navigator actions map to UOW lifecycle (`Move*`, add/delete, `Commit`/`Rollback`) and refresh from `Units`.
- Wrapper-only mode forwards navigator lifecycle actions as `UnitofWorkParams` events to the grid binder (`PreCreate/Delete/Commit/Rollback` and matching `Post*`).

Auto-sizing
- `AutoSizeColumnsMode` supports the standard `DataGridViewAutoSizeColumnsMode` values.
- `AutoResizeColumnsToFitContent()` applies the chosen mode.
- `AutoSizeRowsToContent=true` enables row height recalculation; also triggered for AllCells modes.

Advanced
- Sticky columns: set `BeepColumnConfig.Sticked=true` to pin to the left side.
- Column width: `SetColumnWidth(columnName, width)`.
- Programmatic measurement: `GetColumnWidth(column, includeHeader, allRows)`.
- Dialog helpers: `ShowFilterDialog()`, `ShowSearchDialog()`, `ShowColumnConfigDialog()`.
