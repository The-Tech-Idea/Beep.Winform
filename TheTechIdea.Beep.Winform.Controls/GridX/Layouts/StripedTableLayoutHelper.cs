using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class StripedTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Striped";
        public override string Description => "Zebra-striped rows for improved readability";
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
            grid.Render.ShowRowStripes = true; // Key difference
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 2;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() 
            => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Standard);

        public override INavigationPainter GetNavigationPainter() 
            => NavigationPainterFactory.CreatePainter(navigationStyle.Standard);

        public override int CalculateHeaderHeight(BeepGridPro grid) => 26;
        public override int CalculateNavigatorHeight(BeepGridPro grid) => 48;
    }
}
