using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class DenseTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Dense";
        public override string Description => "High-density layout for maximum data display";
        public override LayoutCategory Category => LayoutCategory.Dense;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 20;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = false;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 1;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() 
            => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Minimal);

        public override INavigationPainter GetNavigationPainter() 
            => NavigationPainterFactory.CreatePainter(navigationStyle.Compact);

        public override int CalculateHeaderHeight(BeepGridPro grid) => 24;
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(40, dpiScale);
        }
    }
}
