using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class HeaderBoldTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Header Bold";
        public override string Description => "Emphasized headers with bold text";
        public override LayoutCategory Category => LayoutCategory.General;

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
            grid.Render.UseBoldHeaderText = true; // Bold headers
            grid.Render.HeaderCellPadding = 3;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() 
            => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Standard);

        public override INavigationPainter GetNavigationPainter() 
            => NavigationPainterFactory.CreatePainter(navigationStyle.Standard);

        public override int CalculateHeaderHeight(BeepGridPro grid) => 28;
        public override int CalculateNavigatorHeight(BeepGridPro grid) => 48;
    }
}
