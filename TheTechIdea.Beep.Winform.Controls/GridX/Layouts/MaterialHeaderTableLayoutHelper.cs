using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class MaterialHeaderTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            grid.RowHeight = 28;
            grid.ColumnHeaderHeight = 32;
            grid.ShowColumnHeaders = true;

            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = true;  // subtle gradient
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 4;
            grid.Render.UseElevation = true; // slight elevation
            grid.Render.CardStyle = false;

            LayoutCommon.ApplyAlignmentHeuristics(grid);
        }
    }
}
