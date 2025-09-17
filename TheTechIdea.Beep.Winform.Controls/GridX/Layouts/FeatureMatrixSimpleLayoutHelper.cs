using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Minimal feature matrix preset.
    /// </summary>
    public sealed class FeatureMatrixSimpleLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Sizing
            grid.RowHeight = 32;
            grid.ColumnHeaderHeight = 40;
            grid.ShowColumnHeaders = true;

            // Render flags
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 6;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;

            var cols = grid?.Columns?.ToList();
            if (cols == null || cols.Count == 0) return;

            var first = cols.FirstOrDefault(c => c.Visible);
            if (first != null)
            {
                first.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                first.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            }

            foreach (var c in cols.Where(x => x.Visible && x != first))
            {
                c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            }
        }
    }
}
