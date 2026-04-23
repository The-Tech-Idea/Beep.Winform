using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// AG Grid Alpine layout: clean borders, light striping, familiar spreadsheet feel.
    /// </summary>
    public sealed class AGGridAlpineTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "AG Grid Alpine";
        public override string Description => "AG Grid Alpine-inspired layout with clean borders and light striping";
        public override LayoutCategory Category => LayoutCategory.General;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 26;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = true;
            grid.Render.ShowRowStripes = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 2;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Standard);
        public override INavigationPainter GetNavigationPainter() => NavigationPainterFactory.CreatePainter(navigationStyle.Standard);
    }
}
