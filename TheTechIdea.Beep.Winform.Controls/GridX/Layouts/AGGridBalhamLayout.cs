using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// AG Grid Balham theme layout
    /// Professional theme with gradient headers and polished appearance
    /// </summary>
    public sealed class AGGridBalhamLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "AG Grid Balham";
        
        public override string Description => 
            "Professional AG Grid Balham theme with gradient headers and borders";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Enterprise;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // AG Grid Balham row height
            grid.RowHeight = 42;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Balham has prominent grid lines
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // Subtle alternating rows
            grid.Render.ShowRowStripes = true;
            
            // Balham signature: gradient header
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Professional padding
            grid.Render.HeaderCellPadding = 10;
            
            // No elevation (uses borders instead)
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
            // AG Grid Balham header height
            return 50;
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
            // Balham works with corporate and professional styles
            return gridStyle == BeepGridStyle.Corporate || 
                   gridStyle == BeepGridStyle.Modern ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

