using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Fluent 2 Standard layout: spacious, subtle separators, minimal chrome.
    /// </summary>
    public sealed class Fluent2StandardTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Fluent 2 Standard";
        public override string Description => "Microsoft Fluent 2 layout with spacious rows and subtle separators";
        public override LayoutCategory Category => LayoutCategory.General;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 30;
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
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Fluent);
        public override INavigationPainter GetNavigationPainter() => NavigationPainterFactory.CreatePainter(navigationStyle.Fluent);
    }
}
