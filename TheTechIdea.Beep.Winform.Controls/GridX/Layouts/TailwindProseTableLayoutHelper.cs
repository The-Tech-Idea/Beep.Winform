using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Tailwind Prose layout: generous whitespace, readable typography, editorial feel.
    /// </summary>
    public sealed class TailwindProseTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Tailwind Prose";
        public override string Description => "Editorial layout with generous whitespace and readable typography";
        public override LayoutCategory Category => LayoutCategory.General;

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
            grid.Render.UseBoldHeaderText = true;
            grid.Render.HeaderCellPadding = 4;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Tailwind);
        public override INavigationPainter GetNavigationPainter() => NavigationPainterFactory.CreatePainter(navigationStyle.Tailwind);
    }
}
