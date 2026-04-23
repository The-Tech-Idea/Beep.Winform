using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Material Design 3 Surface layout: elevated header, comfortable spacing, rounded feel.
    /// </summary>
    public sealed class Material3SurfaceTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Material 3 Surface";
        public override string Description => "Material Design 3 surface layout with elevated header and comfortable spacing";
        public override LayoutCategory Category => LayoutCategory.Modern;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 28;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = false;
            grid.Render.ShowRowStripes = false;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 4;
            grid.Render.UseElevation = true;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Material);
        public override INavigationPainter GetNavigationPainter() => NavigationPainterFactory.CreatePainter(navigationStyle.Material);
    }
}
