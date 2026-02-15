using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class CardTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Card";
        public override string Description => "Card-based layout with elevation";
        public override LayoutCategory Category => LayoutCategory.Modern;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 28;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = false; // Cards don't have grid lines
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 4;
            grid.Render.UseElevation = true; // Card elevation
            grid.Render.CardStyle = true; // Enable card style
        }

        public override IPaintGridHeader GetHeaderPainter() 
            => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Card);

        public override INavigationPainter GetNavigationPainter() 
            => NavigationPainterFactory.CreatePainter(navigationStyle.Card);

        public override int CalculateHeaderHeight(BeepGridPro grid) => 32;
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(52, dpiScale);
        }
    }
}
