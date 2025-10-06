using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class CardTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            grid.RowHeight = 28;
            grid.ColumnHeaderHeight = 32;
            grid.ShowColumnHeaders = true;

            grid.Render.ShowGridLines = false; // card uses borders instead
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 4;
            grid.Render.UseElevation = true;
            grid.Render.CardStyle = true; // outline for rows

            LayoutCommon.ApplyAlignmentHeuristics(grid);
        }
    }
}
