using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Tailwind CSS Prose layout
    /// Typography-focused layout inspired by Tailwind's prose classes
    /// </summary>
    public sealed class TailwindProseLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Tailwind Prose";
        
        public override string Description => 
            "Typography-focused layout with Tailwind CSS prose styling";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Web;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Tailwind prose comfortable row height
            grid.RowHeight = 36;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Tailwind prose style - minimal grid lines
            grid.Render.ShowGridLines = false;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // Very subtle row stripes
            grid.Render.ShowRowStripes = true;
            
            // Clean header without gradients
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Tailwind prose padding (generous)
            grid.Render.HeaderCellPadding = 12;
            
            // No elevation
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
            // Tailwind prose header
            return 44;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Tailwind navigation
            return 52;
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Works with Flat and Minimal styles
            return gridStyle == BeepGridStyle.Flat || 
                   gridStyle == BeepGridStyle.Minimal ||
                   gridStyle == BeepGridStyle.Modern ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

