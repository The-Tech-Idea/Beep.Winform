using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Microsoft Fluent 2 Card Grid layout
    /// Card-based layout with Fluent 2 design principles
    /// </summary>
    public sealed class Fluent2CardLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Fluent 2 Card";
        
        public override string Description => 
            "Fluent 2 card-based layout with elevation and rounded corners";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Modern;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Fluent 2 card row height (taller for card appearance)
            grid.RowHeight = 48;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Cards don't have grid lines
            grid.Render.ShowGridLines = false;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // No row stripes
            grid.Render.ShowRowStripes = false;
            
            // Fluent 2 header with subtle gradient
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Card padding
            grid.Render.HeaderCellPadding = 12;
            
            // Card-specific properties
            grid.Render.UseElevation = true;
            grid.Render.CardStyle = true;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Card);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            return NavigationPainterFactory.CreatePainter(navigationStyle.Card);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Fluent 2 card header height
            return 52;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Fluent 2 card navigation height with DPI scaling
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(60, dpiScale);
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Works with Modern and card-friendly styles
            return gridStyle == BeepGridStyle.Modern || 
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

