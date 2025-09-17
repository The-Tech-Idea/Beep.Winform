using System;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Bold header + compact body preset (matches Image 3 style).
    /// </summary>
    public sealed class DepartmentsRecordBoldLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Sizing
            grid.RowHeight = 30;              // compact rows
            grid.ColumnHeaderHeight = 44;     // bold header
            grid.ShowColumnHeaders = true;

            // Render flags
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = false;
            grid.Render.UseBoldHeaderText = true;
            grid.Render.HeaderCellPadding = 8;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;

            var cols = grid?.Columns?.ToList();
            if (cols == null || cols.Count == 0) return;

            // Alignment heuristics
            var firstVisible = cols.FirstOrDefault(c => c.Visible);
            if (firstVisible != null)
            {
                firstVisible.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                firstVisible.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter; // assume #/checkbox
            }

            foreach (var c in cols.Where(x => x.Visible))
            {
                var name = (c.ColumnCaption ?? c.ColumnName ?? string.Empty).ToLowerInvariant();
                if (name.Contains("status"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else if (name.Contains("action") || name.Contains("actions") || name.Contains("operate") || name.Contains("ops"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else if (name.Contains("name") || name.Contains("employee") || name.Contains("department"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }
                else if (c != firstVisible)
                {
                    // default left for the rest
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }
            }
        }
    }
}
