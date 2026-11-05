using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    public partial class BeepGridPro
    {
        /// <summary>
        /// Shows the column menu: sort, filter and clear for one column, in one place.
        /// </summary>
        /// <remarks>
        /// This is the single entry point behind the column menu button in the header. Commercial
        /// grids put sort and filter in the same menu rather than making the user find two separate
        /// icons, and it means a column offers the same actions whether or not the grid is wide
        /// enough to show a toolbar.
        /// </remarks>
        public void ShowColumnMenu(int columnIndex, Point screenLocation)
        {
            if (columnIndex < 0 || columnIndex >= Data.Columns.Count) return;
            var column = Data.Columns[columnIndex];
            if (column == null) return;

            var items = new List<SimpleItem>();

            if (column.AllowSort)
            {
                items.Add(CreateMenuItem("Sort Ascending", string.Empty, "col_sort_asc"));
                items.Add(CreateMenuItem("Sort Descending", string.Empty, "col_sort_desc"));
                if (column.IsSorted)
                    items.Add(CreateMenuItem("Clear Sort", string.Empty, "col_sort_clear"));
                items.Add(CreateMenuSeparator());
            }

            items.Add(CreateMenuItem("Filter…", string.Empty, "col_filter"));
            if (column.IsFiltered)
                items.Add(CreateMenuItem("Clear Filter", string.Empty, "col_filter_clear"));
            items.Add(CreateMenuSeparator());
            items.Add(CreateMenuItem("Auto-size Column", string.Empty, "col_autosize"));
            items.Add(CreateMenuItem(column.Sticked ? "Unpin Column" : "Pin Column", string.Empty, "col_pin"));
            bool grouped = GroupEngine.Descriptors.Any(d =>
                string.Equals(d.ColumnName, column.ColumnName, System.StringComparison.OrdinalIgnoreCase));
            items.Add(CreateMenuItem(grouped ? "Remove Grouping" : "Group by This Column", string.Empty, "col_group"));
            items.Add(CreateMenuItem("Hide Column", string.Empty, "col_hide"));
            items.Add(CreateMenuItem("Reset Columns", string.Empty, "col_reset"));
            items.Add(CreateMenuItem("Copy Column Name", string.Empty, "col_copy_name"));

            var selected = ShowContextMenu(items, screenLocation, multiSelect: false);
            if (selected?.Tag == null) return;

            // The menu is modal, so the hovered column can have moved on by the time it closes —
            // act on the column the menu was opened for, not on whatever is hovered now.
            Layout.HoveredHeaderColumnIndex = columnIndex;

            switch (selected.Tag.ToString())
            {
                case "col_sort_asc":
                    SetColumnSort(columnIndex, SortDirection.Ascending);
                    break;
                case "col_sort_desc":
                    SetColumnSort(columnIndex, SortDirection.Descending);
                    break;
                case "col_sort_clear":
                    SetColumnSort(columnIndex, SortDirection.None);
                    break;
                case "col_filter":
                    ShowColumnFilterPopup(columnIndex, screenLocation);
                    break;
                case "col_filter_clear":
                    SortFilter.Filter(column.ColumnName, string.Empty);
                    SortFilter.FilterIn(column.ColumnName, null);
                    RemoveFilterCriterion(column.ColumnName);
                    SafeInvalidate();
                    break;
                case "col_autosize":
                    BestFitColumn(columnIndex);
                    break;
                case "col_pin":
                    column.Sticked = !column.Sticked;
                    Layout.Recalculate();
                    SafeInvalidate();
                    break;
                case "col_group":
                    if (grouped)
                    {
                        GroupEngine.SetDescriptors(GroupEngine.Descriptors
                            .Where(d => !string.Equals(d.ColumnName, column.ColumnName,
                                System.StringComparison.OrdinalIgnoreCase)));
                    }
                    else
                    {
                        GroupBy(column.ColumnName);
                    }
                    break;
                case "col_hide":
                    column.Visible = false;
                    Layout.Recalculate();
                    SafeInvalidate();
                    break;
                case "col_reset":
                    ResetColumnLayout();
                    break;
                case "col_copy_name":
                    try { System.Windows.Forms.Clipboard.SetText(column.ColumnName); }
                    catch (System.Exception ex) { ReportOperationError("Column.CopyName", ex); }
                    break;
            }
        }

        private void ShowColumnFilterPopup(int columnIndex, Point screenLocation)
        {
            if (columnIndex < 0 || columnIndex >= Data.Columns.Count) return;
            var column = Data.Columns[columnIndex];
            if (column == null) return;

            var values = Data.Rows
                .Select(r => r.Cells.FirstOrDefault(c =>
                    string.Equals(c.ColumnName, column.ColumnName, System.StringComparison.OrdinalIgnoreCase))?.CellValue)
                .Where(v => v != null && v != System.DBNull.Value)
                .Distinct()
                .ToList();

            var popup = new Filters.BeepGridFilterPopup(column.ColumnCaption ?? column.ColumnName, values);
            popup.SortRequested += (_, direction) => SetColumnSort(columnIndex, direction);
            popup.ClearRequested += (_, __) =>
            {
                SortFilter.Filter(column.ColumnName, string.Empty);
                SortFilter.FilterIn(column.ColumnName, null);
                RemoveFilterCriterion(column.ColumnName);
                SafeInvalidate();
            };
            popup.FilterApplied += (_, args) =>
            {
                if (args.SelectedValues == null || args.SelectedValues.Count == 0)
                    SortFilter.FilterIn(column.ColumnName, null);
                else
                    SortFilter.FilterIn(column.ColumnName, args.SelectedValues);
            };
            popup.FormClosed += (_, __) => popup.Dispose();
            popup.Location = screenLocation;

            var owner = FindForm();
            if (owner != null) popup.Show(owner); else popup.Show();
        }

        private void ResetColumnLayout()
        {
            foreach (var column in Data.Columns)
            {
                if (column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID) continue;
                column.Visible = true;
                column.Sticked = false;
                column.DisplayOrder = column.Index;
            }
            Layout.Recalculate();
            SafeInvalidate();
        }

        private void ShowGridContextMenu(MouseEventArgs e)
        {
            var menuItems = new List<SimpleItem>
            {
                CreateMenuItemWithShortcut("Copy", "Ctrl+C", string.Empty, "copy"),
                CreateMenuItemWithShortcut("Copy with Headers", "Ctrl+Shift+C", string.Empty, "copy_headers"),
                CreateMenuItemWithShortcut("Cut", "Ctrl+X", string.Empty, "cut"),
                CreateMenuItemWithShortcut("Paste", "Ctrl+V", string.Empty, "paste"),
                CreateMenuSeparator(),
                CreateMenuItem("Select All", string.Empty, "select_all"),
                CreateMenuItem("Clear Selection", string.Empty, "clear_selection"),
                CreateMenuSeparator()
            };
            if (!ReadOnly)
            {
                menuItems.Add(CreateMenuItem("Insert New Row", string.Empty, "insert"));
                menuItems.Add(CreateMenuItem("Delete Selected Row(s)", string.Empty, "delete"));
                menuItems.Add(CreateMenuSeparator());
            }
            menuItems.Add(CreateMenuItem("Auto-size Columns", string.Empty, "autosize"));
            menuItems.Add(CreateMenuItem("Reset Column Order", string.Empty, "reset_order"));
            menuItems.Add(CreateMenuSeparator());
            menuItems.Add(CreateMenuItem("Export to Excel", string.Empty, "export_excel"));
            menuItems.Add(CreateMenuItem("Export to CSV", string.Empty, "export_csv"));

            var screenLocation = PointToScreen(e.Location);
            var selected = base.ShowContextMenu(menuItems, screenLocation, multiSelect: false);
            if (selected != null && selected.Tag != null)
            {
                var currentRowIndex = Selection.RowIndex;
                var currentRow = (currentRowIndex >= 0 && currentRowIndex < Data.Rows.Count)
                    ? Data.Rows[currentRowIndex]
                    : null;
                var selectedRows = Data.Rows.Where(r => r.IsSelected).ToList();
                var eventArgs = new Models.GridContextMenuEventArgs(selected, currentRow, selectedRows, currentRowIndex);
                GridContextMenuItemSelected?.Invoke(this, eventArgs);
                if (!eventArgs.Cancel)
                {
                    HandleContextMenuAction(selected.Tag.ToString() ?? string.Empty);
                }
                if (eventArgs.RefreshGrid)
                {
                    Invalidate();
                }
            }
        }

        private void HandleContextMenuAction(string? action)
        {
            if (string.IsNullOrEmpty(action)) return;
            switch (action)
            {
                case "copy":
                    CopyToClipboard(false, true); break;
                case "copy_headers":
                    CopyToClipboard(true, true); break;
                case "cut":
                    if (!ReadOnly) CutToClipboard(false, true); break;
                case "paste":
                    if (!ReadOnly) PasteFromClipboard(); break;
                case "select_all":
                    SelectAllRows(); break;
                case "clear_selection":
                    ClearSelection(); break;
                case "insert":
                    if (!ReadOnly) Navigator.InsertNew(); break;
                case "delete":
                    if (!ReadOnly) DeleteSelectedRows(); break;
                case "autosize":
                    AutoSizeColumns(); break;
                case "reset_order":
                    ResetColumnOrder(); break;
                case "export_excel":
                    ExportToExcel(); break;
                case "export_csv":
                    ExportToCsv(); break;
            }
        }

        private void SelectAllRows()
        {
            if (MultiSelect)
            {
                foreach (var row in Data.Rows) row.IsSelected = true;
                Invalidate();
            }
        }
        public void ClearSelection()
        {
            foreach (var row in Data.Rows) row.IsSelected = false;
            Invalidate();
        }
        private void AutoSizeColumns()
        {
            AutoResizeColumnsToFitContent();
        }
        private void ResetColumnOrder()
        {
            foreach (var column in Data.Columns) column.DisplayOrder = column.Index;
            Layout.Recalculate();
            Invalidate();
        }
        private async void ExportToExcel()
        {
            if (!ExportEngine.IsAvailable(Export.GridExportFormat.Excel))
            {
                var dialogManager = BeepDialogManager.Instance;
                dialogManager.SetHostForm(FindForm());
                dialogManager.NotifyFeaturePending("Export to Excel", dedupeKey: "grid-export-excel-pending");
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Export to Excel"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using var cancellation = new System.Threading.CancellationTokenSource();
                using var progressForm = new Export.GridExportProgressForm("Exporting Excel", cancellation);
                var owner = FindForm();
                if (owner != null) progressForm.Show(owner); else progressForm.Show();

                try
                {
                    await ExportToExcelAsync(
                        dialog.FileName,
                        progress: new Progress<Export.GridExportProgress>(progressForm.Report),
                        cancellationToken: cancellation.Token);
                    progressForm.MarkComplete();
                }
                catch (OperationCanceledException)
                {
                    // A cancelled export leaves the previous file untouched.
                }
                catch (Exception ex)
                {
                    ReportOperationError("Export.Excel", ex);
                    progressForm.Close();
                    MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressForm.Close();
                }
            }
        }

        private async void ExportToCsv()
        {
            if (!ExportEngine.IsAvailable(Export.GridExportFormat.Csv))
            {
                var dialogManager = BeepDialogManager.Instance;
                dialogManager.SetHostForm(FindForm());
                dialogManager.NotifyFeaturePending("Export to CSV", dedupeKey: "grid-export-csv-pending");
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Export to CSV"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using var cancellation = new System.Threading.CancellationTokenSource();
                using var progressForm = new Export.GridExportProgressForm("Exporting CSV", cancellation);
                var owner = FindForm();
                if (owner != null) progressForm.Show(owner); else progressForm.Show();

                try
                {
                    await ExportToCsvAsync(
                        dialog.FileName,
                        progress: new Progress<Export.GridExportProgress>(progressForm.Report),
                        cancellationToken: cancellation.Token);
                    progressForm.MarkComplete();
                }
                catch (OperationCanceledException)
                {
                    // A cancelled export leaves the previous file untouched.
                }
                catch (Exception ex)
                {
                    ReportOperationError("Export.Csv", ex);
                    progressForm.Close();
                    MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressForm.Close();
                }
            }
        }

        private void DeleteSelectedRows()
        {
            int selectedCount = Data.Rows.Count(r => r.IsSelected);

            if (selectedCount > 1 && MultiSelect)
            {
                var targets = Data.Rows
                    .Select((row, index) => new { row, index })
                    .Where(x => x.row.IsSelected)
                    .Select(x => x.index)
                    .OrderByDescending(index => index)
                    .ToList();

                foreach (int target in targets)
                {
                    if (target < 0 || target >= Data.Rows.Count) continue;

                    try
                    {
                        Selection.SelectCell(target, 0);
                        Navigator.DeleteCurrent();
                    }
                    catch (Exception ex)
                    {
                        ReportOperationError("DeleteSelectedRows", ex);
                        break;
                    }
                }

                ClearSelection();
                Invalidate();
            }
            else
            {
                Navigator.DeleteCurrent();
            }
        }
    }
}
