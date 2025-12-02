using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class ComparisonTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Comparison Table";
        public override string Description => "Product comparison and feature matrix layout";
        public override LayoutCategory Category => LayoutCategory.Matrix;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 26;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = true; // Emphasized headers
            grid.Render.HeaderCellPadding = 3;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }

        protected override void CustomConfiguration(BeepGridPro grid)
        {
            // Center-align columns for comparison (override common alignment)
            if (grid?.Data?.Columns != null)
            {
                foreach (var col in grid.Data.Columns)
                {
                    if (!col.IsSelectionCheckBox && !col.IsRowNumColumn && !col.IsRowID)
                    {
                        col.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    }
                }
            }
        }

        public override IPaintGridHeader GetHeaderPainter() 
            => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Standard);

        public override INavigationPainter GetNavigationPainter() 
            => NavigationPainterFactory.CreatePainter(navigationStyle.Standard);

        public override int CalculateHeaderHeight(BeepGridPro grid) => 30;
        public override int CalculateNavigatorHeight(BeepGridPro grid) => 48;
    }
}
