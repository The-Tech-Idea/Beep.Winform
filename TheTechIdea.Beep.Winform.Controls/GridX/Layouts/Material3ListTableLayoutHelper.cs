using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Material 3 List layout: list-like appearance, no grid lines, subtle separators.
    /// </summary>
    public sealed class Material3ListTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Material 3 List";
        public override string Description => "List-style Material Design 3 layout with subtle row separators";
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
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Material);
        public override INavigationPainter GetNavigationPainter() => NavigationPainterFactory.CreatePainter(navigationStyle.Material);
    }
}
