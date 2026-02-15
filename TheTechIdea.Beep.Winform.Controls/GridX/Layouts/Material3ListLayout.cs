using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Material Design 3 List layout
    /// List-style layout following Material 3 list components
    /// </summary>
    public sealed class Material3ListLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Material 3 List";
        
        public override string Description => 
            "Material Design 3 list-style layout with horizontal dividers and hover effects";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Modern;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Material 3 list item height (2-line variant)
            grid.RowHeight = 64;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Material 3 lists typically have horizontal dividers only
            grid.Render.ShowGridLines = false; // No vertical lines
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // No row stripes in Material 3 lists
            grid.Render.ShowRowStripes = false;
            
            // List-style header
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = true; // Lists often have prominent headers
            
            // Material 3 list item padding
            grid.Render.HeaderCellPadding = 16;
            
            // Lists typically don't have elevation (they're on the surface)
            grid.Render.UseElevation = false;
            
            // Not a card style
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Custom Configuration
        
        protected override void CustomConfiguration(BeepGridPro grid)
        {
            // Material 3 list-specific customizations
            // Could add ripple effects, item hover backgrounds, etc.
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            // Use Material header painter with list modifications
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Material);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            // Use Material navigation painter
            return NavigationPainterFactory.CreatePainter(navigationStyle.Material);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Material 3 list header height
            return 48;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Material 3 navigation height with DPI scaling
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(56, dpiScale);
        }
        
        #endregion
        
        #region Compatibility
        
        public override bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Works best with Material style
            return gridStyle == BeepGridStyle.Material || 
                   gridStyle == BeepGridStyle.Modern ||
                   gridStyle == BeepGridStyle.Default;
        }
        
        #endregion
    }
}

