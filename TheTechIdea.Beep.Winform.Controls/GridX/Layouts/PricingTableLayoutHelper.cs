using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public sealed class PricingTableLayoutHelper : BaseLayoutPreset
    {
        public override string Name => "Pricing Table";
        public override string Description => "Pricing comparison and subscription tier layout";
        public override LayoutCategory Category => LayoutCategory.Specialty;

        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 28;
            grid.ShowColumnHeaders = true;
        }

        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = true;
            grid.Render.HeaderCellPadding = 4;
            grid.Render.UseElevation = true;
            grid.Render.CardStyle = false;
        }

        protected override void CustomConfiguration(BeepGridPro grid)
        {
            // Center-align price/feature columns
            if (grid?.Data?.Columns != null)
            {
                foreach (var col in grid.Data.Columns)
                {
                    if (!col.IsSelectionCheckBox && !col.IsRowNumColumn && !col.IsRowID)
                    {
                        // First column (feature name) left-aligned
                        var isFirst = grid.Data.Columns.IndexOf(col) == 0;
                        if (isFirst)
                        {
                            col.HeaderTextAlignment = ContentAlignment.MiddleLeft;
                            col.CellTextAlignment = ContentAlignment.MiddleLeft;
                        }
                        else
                        {
                            // Price columns centered
                            col.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                            col.CellTextAlignment = ContentAlignment.MiddleCenter;
                        }
                    }
                }
            }
        }

        public override IPaintGridHeader GetHeaderPainter() 
            => HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Material);

        public override INavigationPainter GetNavigationPainter() 
            => NavigationPainterFactory.CreatePainter(navigationStyle.Material);

        public override int CalculateHeaderHeight(BeepGridPro grid) => 32;
        public override int CalculateNavigatorHeight(BeepGridPro grid) => 52;
    }
}
