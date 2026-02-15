using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class BorderlessTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Borderless";
        public override string Description => "Clean borderless layout for modern UIs";
        public override LayoutCategory Category => LayoutCategory.Modern;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 24;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = false; // No borders
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = false;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 2;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() 
            => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Minimal);

        public override INavigationPainter GetNavigationPainter() 
            => NavigationPainterFactory.CreatePainter(navigationStyle.Minimal);

        public override int CalculateHeaderHeight(BeepGridPro grid) => 26;
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(48, dpiScale);
        }
    }
}
