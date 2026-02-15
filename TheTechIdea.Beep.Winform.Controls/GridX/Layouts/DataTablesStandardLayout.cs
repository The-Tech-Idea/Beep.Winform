using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// jQuery DataTables standard layout
    /// Classic DataTables styling with stripes and hover effects
    /// </summary>
    public sealed class DataTablesStandardLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "DataTables";
        
        public override string Description => 
            "jQuery DataTables style with striped rows and hover effects";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Web;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // DataTables standard row height
            grid.RowHeight = 36;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // DataTables classic grid lines
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // Classic zebra striping
            grid.Render.ShowRowStripes = true;
            
            // Simple header
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // DataTables padding
            grid.Render.HeaderCellPadding = 8;
            
            // No elevation
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.DataTables);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            return NavigationPainterFactory.CreatePainter(navigationStyle.DataTables);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // DataTables header height
            return 40;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // DataTables pagination with DPI scaling
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(48, dpiScale);
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // DataTables works with Bootstrap and standard styles
            return gridStyle == BeepGridStyle.Bootstrap || 
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

