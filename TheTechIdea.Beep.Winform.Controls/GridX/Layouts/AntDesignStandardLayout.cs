using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Ant Design standard table layout
    /// Follows Ant Design table component guidelines
    /// </summary>
    public sealed class AntDesignStandardLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Ant Design";
        
        public override string Description => 
            "Ant Design table layout with comfortable spacing and hover effects";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Web;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Ant Design standard row height (comfortable)
            grid.RowHeight = 54;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Ant Design has clean grid lines
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // No row stripes by default
            grid.Render.ShowRowStripes = false;
            
            // Clean flat header
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Ant Design generous padding
            grid.Render.HeaderCellPadding = 16;
            
            // No elevation (flat design)
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.AntDesign);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            return NavigationPainterFactory.CreatePainter(navigationStyle.AntDesign);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Ant Design header height
            return 54;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Ant Design pagination height with DPI scaling
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(64, dpiScale);
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Works with Flat and Modern styles
            return gridStyle == BeepGridStyle.Flat || 
                   gridStyle == BeepGridStyle.Modern ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

