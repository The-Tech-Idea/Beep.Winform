using System.Linq;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class ComparisonTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            grid.RowHeight = 26;
            grid.ColumnHeaderHeight = 34; // more room
            grid.ShowColumnHeaders = true;

            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = true;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = true; // emphasize
            grid.Render.HeaderCellPadding = 6; // breathing room
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;

            // Left column (feature names) left aligned, others centered
            var cols = grid.Columns?.Where(c => c.Visible).ToList();
            if (cols != null && cols.Count > 0)
            {
                var first = cols.First();
                foreach (var c in cols)
                {
                    if (c == first)
                    {
                        c.HeaderTextAlignment = ContentAlignment.MiddleLeft;
                        c.CellTextAlignment = ContentAlignment.MiddleLeft;
                    }
                    else
                    {
                        c.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                        c.CellTextAlignment = ContentAlignment.MiddleCenter;
                    }
                }
            }
        }
    }
}
