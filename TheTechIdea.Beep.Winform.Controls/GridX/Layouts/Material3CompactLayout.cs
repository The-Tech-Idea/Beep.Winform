using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Material Design 3 Compact layout
    /// Optimized for data density while maintaining Material 3 design principles
    /// </summary>
    public sealed class Material3CompactLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Material 3 Compact";
        
        public override string Description => 
            "Material Design 3 optimized for data density with compact spacing";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Dense;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Material 3 compact row height
            grid.RowHeight = 40;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Compact variant can have subtle grid lines for better row definition
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // No row stripes
            grid.Render.ShowRowStripes = false;
            
            // Simpler header styling for compact view
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Reduced padding for compactness
            grid.Render.HeaderCellPadding = 12;
            
            // Subtle elevation
            grid.Render.UseElevation = true;
            
            // Not a card style
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            // Use Compact header painter for density
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Compact);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            // Use Compact navigation painter
            return NavigationPainterFactory.CreatePainter(navigationStyle.Compact);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Compact header height
            return 44;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Compact navigation height
            return 48;
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Works with Material, Modern, and Compact styles
            return gridStyle == BeepGridStyle.Material || 
                   gridStyle == BeepGridStyle.Modern ||
                   gridStyle == BeepGridStyle.Compact ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

