using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class AntDesignCompactTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Ant Design Compact";
        public override string Description => "Ant Design compact table layout with minimal padding";
        public override LayoutCategory Category => LayoutCategory.Web;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 20;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = true;
            grid.Render.ShowRowStripes = false;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 1;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Compact);
        public override INavigationPainter GetNavigationPainter() => NavigationPainterFactory.CreatePainter(navigationStyle.Compact);
    }
}
