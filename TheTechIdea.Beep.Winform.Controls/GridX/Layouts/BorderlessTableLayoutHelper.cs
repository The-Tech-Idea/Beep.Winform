using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class BorderlessTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            grid.RowHeight = 24;
            grid.ColumnHeaderHeight = 26;
            grid.ShowColumnHeaders = true;

            grid.Render.ShowGridLines = false;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = false;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 2;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;

            LayoutCommon.ApplyAlignmentHeuristics(grid);
        }
    }
}
