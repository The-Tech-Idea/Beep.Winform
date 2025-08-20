using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules; // SortDirection
using TheTechIdea.Beep.Winform.Controls.GridX.Filters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    // Extension to enable Excel-style filter with zero changes to BeepGridPro internals
    public static class BeepGridProFilterExtensions
    {
        public static void EnableExcelFilter(this BeepGridPro grid)
        {
            if (grid == null) return;
            // Avoid double subscription
            grid.MouseClick -= Grid_MouseClick;
            grid.MouseClick += Grid_MouseClick;
        }

        private static void Grid_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not BeepGridPro grid) return;
            // Guard header area
            if (!grid.ShowColumnHeaders) return;
            var layout = grid.Layout; // internal; accessible in same assembly
            if (layout == null || !layout.ShowColumnHeaders) return;
            if (!layout.HeaderRect.Contains(e.Location)) return;

            // Which header cell?
            int x = layout.HeaderRect.Left;
            int idx = -1;
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                int w = Math.Max(20, grid.Columns[i].Width);
                var rect = new Rectangle(x, layout.HeaderRect.Top, w, layout.HeaderRect.Height);
                if (rect.Contains(e.Location)) { idx = i; break; }
                x += w;
            }
            if (idx < 0) return;

            var screenPoint = grid.PointToScreen(new Point(x, layout.HeaderRect.Bottom));
            ShowPopupFor(grid, idx, screenPoint);
        }

        private static void ShowPopupFor(BeepGridPro grid, int columnIndex, Point screenLocation)
        {
            // Build values
            var col = grid.Columns[columnIndex];
            var values = grid.Rows
                .Select(r => r.RowData?.GetType().GetProperty(col.ColumnName)?.GetValue(r.RowData))
                .Where(v => v != null)
                .Distinct()
                .ToList();

            var popup = new BeepGridFilterPopup(col.ColumnCaption ?? col.ColumnName, values);
            popup.SortRequested += (s, dir) =>
            {
                Sort(grid, col.ColumnName, dir);
            };
            popup.ClearRequested += (s, e) =>
            {
                grid.RefreshGrid();
                grid.Layout.Recalculate();
                grid.Invalidate();
            };
            popup.FilterApplied += (s, e) =>
            {
                ApplyInFilter(grid, col.ColumnName, e.SelectedValues);
            };

            popup.Location = screenLocation;
            popup.Show(grid.FindForm());
        }

        // Simple in-memory sort using RowData reflection
        private static void Sort(BeepGridPro grid, string columnName, SortDirection direction)
        {
            Func<BeepRowConfig, object?> key = r => r.RowData?.GetType().GetProperty(columnName)?.GetValue(r.RowData);
            var ordered = direction switch
            {
                SortDirection.Ascending => grid.Rows.OrderBy(key).ToList(),
                SortDirection.Descending => grid.Rows.OrderByDescending(key).ToList(),
                _ => grid.Rows.ToList()
            };

            grid.Rows.Clear();
            int i = 0;
            foreach (var r in ordered)
            {
                r.DisplayIndex = i++;
                grid.Rows.Add(r);
            }
            grid.Layout.Recalculate();
            grid.Invalidate();
        }

        // Apply IN filter on selected values
        private static void ApplyInFilter(BeepGridPro grid, string columnName, IEnumerable<object> values)
        {
            var set = new HashSet<string>((values ?? Array.Empty<object>()).Select(v => v?.ToString() ?? string.Empty));
            var filtered = grid.Rows.Where(r => set.Contains(r.RowData?.GetType().GetProperty(columnName)?.GetValue(r.RowData)?.ToString() ?? string.Empty)).ToList();

            grid.Rows.Clear();
            int i = 0;
            foreach (var r in filtered)
            {
                r.DisplayIndex = i++;
                grid.Rows.Add(r);
            }
            grid.Layout.Recalculate();
            grid.Invalidate();
        }
    }
}
