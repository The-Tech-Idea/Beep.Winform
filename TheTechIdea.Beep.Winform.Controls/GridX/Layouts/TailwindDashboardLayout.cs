using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Tailwind CSS Dashboard layout
    /// Dashboard-optimized layout with Tailwind styling
    /// </summary>
    public sealed class TailwindDashboardLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Tailwind Dashboard";
        
        public override string Description => 
            "Dashboard-style grid with Tailwind CSS design patterns";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Web;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Tailwind dashboard row height
            grid.RowHeight = 40;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Tailwind dashboard has visible grid lines
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // Subtle row stripes
            grid.Render.ShowRowStripes = true;
            
            // Gradient header (Tailwind gray gradient)
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Dashboard padding
            grid.Render.HeaderCellPadding = 10;
            
            // Subtle elevation for cards
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Tailwind);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            return NavigationPainterFactory.CreatePainter(navigationStyle.Tailwind);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Tailwind dashboard header
            return 40;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Tailwind navigation
            return 48;
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Works with Flat, Modern, and Bootstrap styles
            return gridStyle == BeepGridStyle.Flat || 
                   gridStyle == BeepGridStyle.Modern ||
                   gridStyle == BeepGridStyle.Bootstrap ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

