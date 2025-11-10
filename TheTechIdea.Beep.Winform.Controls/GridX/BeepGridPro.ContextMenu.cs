using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    public partial class BeepGridPro
    {
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
                    if (!ReadOnly) Navigator.DeleteCurrent(); break;
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
        private void ClearSelection()
        {
            foreach (var row in Data.Rows) row.IsSelected = false;
            Invalidate();
        }
        private void AutoSizeColumns()
        {
            Sizing.AutoResizeColumnsToFitContent();
            Layout.Recalculate();
            Invalidate();
        }
        private void ResetColumnOrder()
        {
            foreach (var column in Data.Columns) column.DisplayOrder = column.Index;
            Layout.Recalculate();
            Invalidate();
        }
        private void ExportToExcel() { /* TODO */ }
        private void ExportToCsv() { /* TODO */ }
    }
}
