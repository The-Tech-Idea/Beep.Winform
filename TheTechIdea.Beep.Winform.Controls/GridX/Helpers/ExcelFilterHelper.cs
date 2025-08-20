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
    // Non-invasive Excel-like filter integration helper
    // Does NOT modify BeepGridPro internals. Caller computes header hit and calls ShowForColumn.
    public static class ExcelFilterHelper
    {
        // Show filter popup for a column. screenLocation should be near the header cell bottom-left
        public static void ShowForColumn(BeepSimpleGridLike gridAdapter, int columnIndex, Point screenLocation)
        {
            if (gridAdapter == null) return;
            if (columnIndex < 0 || columnIndex >= gridAdapter.Columns.Count) return;

            var col = gridAdapter.Columns[columnIndex];
            var values = gridAdapter.Rows
                .Select(r => r.RowData?.GetType().GetProperty(col.ColumnName)?.GetValue(r.RowData))
                .Where(v => v != null)
                .Distinct()
                .ToList();

            var popup = new BeepGridFilterPopup(col.ColumnCaption ?? col.ColumnName, values);
            popup.SortRequested += (s, dir) =>
            {
                gridAdapter.Sort(col.ColumnName, dir);
            };
            popup.ClearRequested += (s, e) => gridAdapter.ClearFilter();
            popup.FilterApplied += (s, e) =>
            {
                gridAdapter.ApplyInFilter(col.ColumnName, e.SelectedValues);
            };

            popup.Location = screenLocation;
            var owner = gridAdapter.AsControl().FindForm();
            popup.Show(owner);
        }
    }

    // Adapter interface to avoid changing BeepGridPro API. Implement a thin wrapper alongside your grid.
    public interface BeepSimpleGridLike
    {
        IList<BeepColumnConfig> Columns { get; }
        System.ComponentModel.BindingList<BeepRowConfig> Rows { get; }
        void Sort(string columnName, SortDirection direction);
        void ClearFilter();
        void ApplyInFilter(string columnName, IEnumerable<object> selectedValues);
        Control AsControl();
    }
}
