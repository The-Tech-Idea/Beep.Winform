using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Material Design 3 Surface layout
    /// Implements Google's Material 3 design system with surface container styling
    /// </summary>
    public sealed class Material3SurfaceLayout : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Material 3 Surface";
        
        public override string Description => 
            "Material Design 3 with surface container styling, generous padding, and subtle elevation";
        
        public override string Version => "1.0.0";
        
        public override LayoutCategory Category => LayoutCategory.Modern;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            // Material 3 standard row height (comfortable)
            grid.RowHeight = 52;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            // Material 3 is typically borderless within the surface
            grid.Render.ShowGridLines = false;
            grid.Render.GridLineStyle = DashStyle.Solid;
            
            // No row stripes in Material 3 surface variant
            grid.Render.ShowRowStripes = false;
            
            // Material 3 uses elevation instead of gradients
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            
            // Generous padding following Material 3 guidelines
            grid.Render.HeaderCellPadding = 16;
            
            // Material 3 elevation (subtle)
            grid.Render.UseElevation = true;
            
            // Not a card style (surface container)
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Custom Configuration
        
        protected override void CustomConfiguration(BeepGridPro grid)
        {
            // Material 3 specific customizations can go here
            // For example, setting corner radius if supported
            // grid.Render.CornerRadius = 12; // Material 3 medium corners
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            // Use Material header painter
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
            // Material 3 standard header height
            return 56;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Material 3 standard navigation height with DPI scaling
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return DpiScalingHelper.ScaleValue(64, dpiScale);
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

