using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class StripedTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            grid.RowHeight = 24;
            grid.ColumnHeaderHeight = 26;
            grid.ShowColumnHeaders = true;

            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = true;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 2;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;

            LayoutCommon.ApplyAlignmentHeuristics(grid);
        }
    }
}
