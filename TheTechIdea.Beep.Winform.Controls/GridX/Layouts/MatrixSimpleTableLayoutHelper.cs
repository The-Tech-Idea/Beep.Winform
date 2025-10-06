using System.Linq;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class MatrixSimpleTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            grid.RowHeight = 24;
            grid.ColumnHeaderHeight = 28;
            grid.ShowColumnHeaders = true;

            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = true;
            grid.Render.HeaderCellPadding = 3;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;

            LayoutCommon.ApplyAlignmentHeuristics(grid);
        }
    }
}
