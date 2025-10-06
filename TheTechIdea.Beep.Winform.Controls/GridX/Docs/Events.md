# Events and Integration

BeepGridPro events
- `RowSelectionChanged (object sender, BeepRowSelectedEventArgs e)`
  - Fired when a row's checkbox selection changes (including header Select All). `e.RowIndex` is -1 for bulk changes.

- `CellValueChanged (object sender, BeepCellEventArgs e)`
  - Fired after an editor commits a value to a cell and data source.

- `SaveCalled (object sender, EventArgs e)`
  - Fired when Save action is invoked from the navigator.

Integration tips
- To react to Save: persist BindingSource changes in the underlying repository/service.
- To react to selection: use `SelectedRows` or `SelectedRowIndices`.
- To programmatically edit: use `SelectCell(r,c)` followed by `ShowCellEditor()`.
- To alter columns at runtime: modify `Columns` and call `RefreshGrid()` or `Layout.Recalculate()` as appropriate.
