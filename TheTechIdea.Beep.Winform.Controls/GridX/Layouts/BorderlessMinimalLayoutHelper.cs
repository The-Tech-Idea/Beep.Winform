using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Borderless minimal preset.
    /// </summary>
    public sealed class BorderlessMinimalLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Sizing
            grid.RowHeight = 34;
            grid.ColumnHeaderHeight = 42;
            grid.ShowColumnHeaders = true;

            // Render flags
            grid.Render.ShowGridLines = false;               // borderless
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = false;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 8;
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
                // keep left by default for this clean look
                c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            }
        }
    }
}
