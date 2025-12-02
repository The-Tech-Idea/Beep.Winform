using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class MatrixSimpleTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Matrix Simple";
        public override string Description => "Simple matrix display with centered cells";
        public override LayoutCategory Category => LayoutCategory.Matrix;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 24;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 2;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        protected override void CustomConfiguration(BeepGridPro grid)
        {
            // Center all cells for matrix display
            if (grid?.Data?.Columns != null)
            {
                foreach (var col in grid.Data.Columns)
                {
                    col.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    col.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
            }
        }

        public override IPaintGridHeader GetHeaderPainter() 
            => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Standard);

        public override INavigationPainter GetNavigationPainter() 
            => NavigationPainterFactory.CreatePainter(navigationStyle.Standard);

        public override int CalculateHeaderHeight(BeepGridPro grid) => 26;
        public override int CalculateNavigatorHeight(BeepGridPro grid) => 48;
    }
}
