using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Material Design header layout with gradient and elevation
    /// Enhanced to use BaseLayoutPreset for automatic painter integration
    /// </summary>
    public sealed class MaterialHeaderTableLayoutHelper : BaseLayoutPreset
    {
        #region Metadata
        
        public override string Name => "Material Header";
        
        public override string Description => 
            "Material Design inspired with gradient headers and elevation";
        
        public override LayoutCategory Category => LayoutCategory.Modern;
        
        #endregion
        
        #region Dimensions
        
        protected override void ConfigureDimensions(BeepGridPro grid)
        {
            grid.RowHeight = 28;
            grid.ShowColumnHeaders = true;
        }
        
        #endregion
        
        #region Visual Properties
        
        protected override void ConfigureVisualProperties(BeepGridPro grid)
        {
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = true;  // subtle gradient
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 4;
            grid.Render.UseElevation = true; // slight elevation
            grid.Render.CardStyle = false;
        }
        
        #endregion
        
        #region Painters
        
        public override IPaintGridHeader GetHeaderPainter()
        {
            return HeaderPainterFactory.CreateHeaderPainter(navigationStyle.Material);
        }
        
        public override INavigationPainter GetNavigationPainter()
        {
            return NavigationPainterFactory.CreatePainter(navigationStyle.Material);
        }
        
        #endregion
        
        #region Height Calculations
        
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            return 32;
        }
        
        public override int CalculateNavigatorHeight(BeepGridPro grid)
        {
            return 56;
        }
        
        #endregion
    }
}
