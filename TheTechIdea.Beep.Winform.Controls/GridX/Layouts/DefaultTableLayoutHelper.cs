using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class DefaultTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Dimensions
            grid.RowHeight = 24;
            grid.ColumnHeaderHeight = 26;
            grid.ShowColumnHeaders = true;

            // Render flags
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
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
