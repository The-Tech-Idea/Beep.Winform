using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Microsoft Fluent 2 Design System standard layout
    /// Follows Microsoft's latest Fluent 2 design guidelines
    /// </summary>
    public sealed class Fluent2StandardLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Fluent 2 Standard";
        
        public override string Description => 
            "Microsoft Fluent 2 design system with subtle effects and modern spacing";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Modern;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Fluent 2 standard row height
            grid.RowHeight = 44;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Fluent 2 uses subtle grid lines
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // No row stripes in Fluent 2 standard
            grid.Render.ShowRowStripes = false;
            
            // Clean header styling
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Fluent 2 padding
            grid.Render.HeaderCellPadding = 8;
            
            // Subtle effects
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Fluent);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            return NavigationPainterFactory.CreatePainter(navigationStyle.Fluent);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Fluent 2 header height
            return 48;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Fluent 2 navigation height
            return 56;
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Works best with Modern and Flat styles
            return gridStyle == BeepGridStyle.Modern || 
                   gridStyle == BeepGridStyle.Flat ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

