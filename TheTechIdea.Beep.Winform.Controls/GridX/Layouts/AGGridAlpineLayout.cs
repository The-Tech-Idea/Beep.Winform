using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// AG Grid Alpine theme layout
    /// Professional layout following AG Grid's Alpine theme
    /// </summary>
    public sealed class AGGridAlpineLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "AG Grid Alpine";
        
        public override string Description => 
            "Professional AG Grid Alpine theme with clean lines and modern styling";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Enterprise;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // AG Grid Alpine row height
            grid.RowHeight = 42;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // AG Grid has visible grid lines
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // Subtle row stripes
            grid.Render.ShowRowStripes = true;
            
            // Clean header with subtle effects
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // AG Grid padding
            grid.Render.HeaderCellPadding = 8;
            
            // No elevation
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.AGGrid);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            return NavigationPainterFactory.CreatePainter(navigationStyle.AGGrid);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // AG Grid Alpine header height
            return 48;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // AG Grid navigation with DPI scaling
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(52, dpiScale);
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // AG Grid works with professional styles
            return gridStyle == BeepGridStyle.Corporate || 
                   gridStyle == BeepGridStyle.Modern ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

