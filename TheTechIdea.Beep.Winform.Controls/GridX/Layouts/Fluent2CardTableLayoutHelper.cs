using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class Fluent2CardTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Fluent 2 Card";
        public override string Description => "Microsoft Fluent 2 card-based layout with elevated rows";
        public override LayoutCategory Category => LayoutCategory.Modern;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 32;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = false;
            grid.Render.ShowRowStripes = false;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 4;
            grid.Render.UseElevation = true;
            grid.Render.CardStyle = true;
        }

        public override IPaintGridHeader GetHeaderPainter() => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Fluent);
        public override INavigationPainter GetNavigationPainter() => NavigationPainterFactory.CreatePainter(navigationStyle.Fluent);
    }
}
