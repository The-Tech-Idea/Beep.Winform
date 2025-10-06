# BeepGridPro Class Map

This document lists the key types involved in BeepGridPro and describes their responsibilities and interactions.

Top-level control
- `BeepGridPro` (Controls\GridX\BeepGridPro.cs)
  - Public grid control exposing properties, methods, and events.
  - Composition of helper classes: `GridLayoutHelper`, `GridDataHelper`, `GridRenderHelper`, `GridSelectionHelper`, `GridInputHelper`, `GridScrollHelper`, `GridScrollBarsHelper`, `GridSortFilterHelper`, `GridEditHelper`, `GridThemeHelper`, `GridNavigatorHelper`, `GridSizingHelper`, `GridDialogHelper`.
  - Key public APIs:
    - Data binding: `DataSource`, `DataMember`, `Columns`, `Rows`, `AutoGenerateColumns()`, `EnsureSystemColumns()`, `RefreshGrid()`
    - Layout/appearance: `RowHeight`, `ColumnHeaderHeight`, `ShowColumnHeaders`, `GridStyle`
    - Behavior: `AllowUserToResizeColumns`, `AllowUserToResizeRows`, `ReadOnly`, `MultiSelect`, `ShowCheckBox`, `AutoSizeColumnsMode`, `AutoSizeRowsToContent`, `RowAutoSizePadding`, `UseDpiAwareRowHeights`
    - Selection/editing: `SelectCell(r,c)`, `ShowCellEditor()`, `ShowFilterDialog()`, `ShowSearchDialog()`, `ShowColumnConfigDialog()`
    - Sorting/filtering: `ToggleColumnSort(colIndex)`
    - Navigator actions: `MoveFirst/Prev/Next/Last`, `InsertNew`, `DeleteCurrent`, `Save`, `Cancel`, `AttachNavigator()`
    - Helpers: `GetColumnByName/Caption/Index()`, `GetDictionaryColumns()`
    - Events: `RowSelectionChanged`, `CellValueChanged`, `SaveCalled`

Helpers (GridX/Helpers)
- `GridLayoutHelper`
  - Computes `HeaderRect`, `RowsRect`, `NavigatorRect`, header cell rects, selection checkbox rects.
  - Handles sticky columns and horizontal offsets; respects `_grid.Scroll`.
  - Calculates visible rows with variable row heights.

- `GridDataHelper`
  - Handles binding to various data sources and DataMember resolution.
  - Maintains `Rows` (BindingList of `BeepRowConfig`) and `Columns` (`BeepGridColumnConfigCollection`).
  - Auto-generates columns from DataTable schema or entity type and ensures system columns (`Sel`, `RowNum`, `RowID`).
  - Refreshes rows and updates page info (for navigator rendering).
  - Value normalization for editors (e.g., `SimpleItem`).

- `GridRenderHelper`
  - All drawing: background, headers, rows, selection, custom scrollbars (via `GridScrollBarsHelper.DrawScrollBars`), and owner-drawn navigator.
  - Header visuals: gradients, hover, sort and filter icons, padding, bold option.
  - Sticky columns rendering and stripe support.
  - Caches per-column drawer components for cell content (`IBeepUIComponent` based controls), delegates to `Draw()` of each.
  - Provides hit-test dictionaries for header icons and exposes `PageInfoLabel`.
  - Methods: `UpdatePageInfo`, internal helpers for text flags, icons, and navigator layout.

- `GridSelectionHelper`
  - Tracks active cell (`RowIndex`, `ColumnIndex`) and selection rectangle for highlighting.

- `GridInputHelper`
  - Handles mouse and keyboard input: column resizing, selection, checkboxes (row and header), header sort/icon clicks.
  - Coordinates with `GridScrollBarsHelper` for mouse wheel and scrollbar interactions.
  - Initiates dialog editor for editable cells.

- `GridScrollHelper`
  - Maintains pixel offsets and `FirstVisibleRowIndex` based on variable row heights.
  - Provides scrolling APIs used by input, scrollbars, and layout.

- `GridScrollBarsHelper`
  - Computes and draws custom scrollbars with draggable thumbs, paging clicks, and wheel handling.
  - Calculates necessity and sizes based on content extents and variable row heights.

- `GridSortFilterHelper`
  - In-memory sorting and simple text filtering on a column via reflection against `RowData`.

- `GridThemeHelper`
  - Applies theme colors/fonts from `BeepThemesManager` and current theme name.

- `GridNavigatorHelper`
  - Bridges `BeepBindingNavigator` and `BindingSource` to grid actions; also works without a visual navigator (owner-drawn case).
  - Wires navigator events to grid actions; exposes `Attach()`, `BindTo()`, movement and CRUD methods.

- `GridSizingHelper`
  - Column width and row height auto-sizing helpers; supports `DataGridViewAutoSizeColumnsMode` semantics.
  - `GetColumnWidth` measures header and cell text with padding; caps widths/heights.

- `GridDialogHelper`
  - Modeless overlay editor aligned to the cell (borderless form) with key navigation (Enter/Tab/arrows) and outside-click commit.
  - Filter/search/column-config modal dialogs.

Other helpers and infrastructure
- `BeepGridProFilterExtensions`
  - Extension to enable Excel-style filter behavior without modifying `BeepGridPro` internals; hooks grid mouse and shows `BeepGridFilterPopup`.

- `ExcelFilterHelper` + `BeepSimpleGridLike` adapter
  - Generic Excel-like popup invoker for any grid-like surface; used via `BeepGridProAdapter`.

- `BeepGridProAdapter` (GridX/Adapters)
  - Thin adapter that maps `BeepGridPro` to `BeepSimpleGridLike` to reuse `ExcelFilterHelper` without changing grid internals.

- `GridUnitOfWorkBinder`
  - Optional bridge to a UnitOfWork that exposes a `Units` property; subscribes to UoW events and rebinds grid on data changes.

- `MathCompat`
  - Internal helper exposing Math-like APIs in this namespace.

Filters UI (GridX/Filters)
- `BeepGridFilterPopup`
  - Lightweight popup with search box, Select All checkbox, value list, and sort/clear/apply buttons; raises events.

Data model types (from Controls.Models)
- `BeepColumnConfig`, `BeepRowConfig`, `BeepCellConfig`, `BeepGridColumnConfigCollection`, `SimpleItem` and theme types (`IBeepTheme`).
  - Columns expose appearance, behavior, editor type (`CellEditor`), sorting/filter flags, and system-column flags.
  - Rows contain `Cells`, `RowData`, `IsSelected`, `Height`, and rects used for hit-testing.
  - Cells track `CellValue`, `Rect`, editor metadata, and selection/dirty flags.

Interactions diagram (high level)
- BeepGridPro
  - DataSource/DataMember -> GridDataHelper.Bind -> Columns/Rows
  - Layout changes -> GridLayoutHelper.Recalculate -> Render.Draw
  - Scroll offsets -> GridScrollHelper + GridScrollBarsHelper
  - Mouse/keyboard -> GridInputHelper -> Selection/Edit/Sort/Filter -> GridDialogHelper/GridSortFilterHelper
  - Navigator -> GridNavigatorHelper
  - Auto-size -> GridSizingHelper
  - Theme/style -> GridThemeHelper + GridStyle presets within BeepGridPro
