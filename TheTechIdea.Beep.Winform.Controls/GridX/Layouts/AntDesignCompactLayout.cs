using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Ant Design compact table layout
    /// Dense variant of Ant Design table for data-heavy displays
    /// </summary>
    public sealed class AntDesignCompactLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Ant Design Compact";
        
        public override string Description => 
            "Ant Design compact table for high-density data display";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Dense;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Ant Design compact row height
            grid.RowHeight = 40;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Compact still has grid lines
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // No row stripes in compact
            grid.Render.ShowRowStripes = false;
            
            // Minimal header styling
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Compact padding
            grid.Render.HeaderCellPadding = 12;
            
            // No elevation
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Compact);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            return NavigationPainterFactory.CreatePainter(navigationStyle.Compact);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Ant Design compact header
            return 44;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Compact navigation
            return 48;
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Works with Compact and Flat styles
            return gridStyle == BeepGridStyle.Compact || 
                   gridStyle == BeepGridStyle.Flat ||
                   gridStyle == BeepGridStyle.Modern ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

