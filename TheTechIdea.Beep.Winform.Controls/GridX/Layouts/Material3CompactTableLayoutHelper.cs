using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Material Design 3 Compact layout: dense but retains MD3 visual cues.
    /// </summary>
    public sealed class Material3CompactTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Material 3 Compact";
        public override string Description => "Dense Material Design 3 layout for maximum data density";
        public override LayoutCategory Category => LayoutCategory.Dense;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 22;
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
            grid.Render.HeaderCellPadding = 2;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        public override IPaintGridHeader GetHeaderPainter() => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Material);
        public override INavigationPainter GetNavigationPainter() => NavigationPainterFactory.CreatePainter(navigationStyle.Material);
    }
}
