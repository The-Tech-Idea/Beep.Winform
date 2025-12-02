using System;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Base class for layout presets providing common functionality and template method pattern
    /// </summary>
    public abstract class BaseLayoutPreset : IGridLayoutPreset
    {
        #region Metadata
        
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual string Version => "1.0.0";
        public abstract LayoutCategory Category { get; }
        
        #endregion
        
        #region Apply Method (Template Pattern)
        
        /// <summary>
        /// Applies the layout preset to the grid using template method pattern
        /// </summary>
        public virtual void Apply(BeepGridPro grid)
        {
            if (grid == null) return;
            
            // 1. Configure dimensions
            ConfigureDimensions(grid);
            
            // 2. Configure painters (automatic integration!)
            ConfigurePainters(grid);
            
            // 3. Configure heights (calculated from painters)
            ConfigureHeights(grid);
            
            // 4. Configure visual properties
            ConfigureVisualProperties(grid);
            
            // 5. Apply alignment heuristics
            LayoutCommon.ApplyAlignmentHeuristics(grid);
            
            // 6. Custom configuration hook
            CustomConfiguration(grid);
        }
        
        #endregion
        
        #region Abstract Configuration Methods
        
        /// <summary>
        /// Configure basic dimensions (row height, etc.)
        /// </summary>
        protected abstract void ConfigureDimensions(BeepGridPro grid);
        
        /// <summary>
        /// Configure visual properties (grid lines, effects, etc.)
        /// </summary>
        protected abstract void ConfigureVisualProperties(BeepGridPro grid);
        
        #endregion
        
        #region Virtual Configuration Methods
        
        /// <summary>
        /// Configure painters - override for custom painter selection
        /// </summary>
        protected virtual void ConfigurePainters(BeepGridPro grid)
        {
            // Get painters from layout
            var headerPainter = GetHeaderPainter();
            var navPainter = GetNavigationPainter();
            
            // Apply to grid (if grid has SetHeaderPainter/SetNavigationPainter methods)
            // For now, store references that can be used during rendering
            if (headerPainter != null)
            {
                // Store for later use
                grid.Tag = grid.Tag ?? new System.Collections.Generic.Dictionary<string, object>();
                if (grid.Tag is System.Collections.Generic.Dictionary<string, object> dict)
                {
                    dict["HeaderPainter"] = headerPainter;
                }
            }
            
            if (navPainter != null)
            {
                // Store navigation painter reference for later use
                // Navigation painter will be used when BeepGridPro supports it
                if (grid.Tag is System.Collections.Generic.Dictionary<string, object> dict)
                {
                    dict["NavigationPainter"] = navPainter;
                }
            }
        }
        
        /// <summary>
        /// Configure heights based on painter calculations
        /// </summary>
        protected virtual void ConfigureHeights(BeepGridPro grid)
        {
            grid.ColumnHeaderHeight = CalculateHeaderHeight(grid);
            
            // Navigator height
            int navHeight = CalculateNavigatorHeight(grid);
            if (grid.Layout != null)
            {
                grid.Layout.NavigatorHeight = navHeight;
            }
        }
        
        /// <summary>
        /// Hook for custom configuration - override to add specific behavior
        /// </summary>
        protected virtual void CustomConfiguration(BeepGridPro grid)
        {
            // Override in derived classes for custom behavior
        }
        
        #endregion
        
        #region Abstract Painter Methods
        
        /// <summary>
        /// Get the header painter for this layout
        /// </summary>
        public abstract IPaintGridHeader GetHeaderPainter();
        
        /// <summary>
        /// Get the navigation painter for this layout
        /// </summary>
        public abstract INavigationPainter GetNavigationPainter();
        
        #endregion
        
        #region Virtual Height Calculation Methods
        
        /// <summary>
        /// Calculate the appropriate header height for this layout
        /// </summary>
        public virtual int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Default calculation based on painter
            var painter = GetHeaderPainter();
            if (painter != null)
            {
                return painter.CalculateHeaderHeight(grid);
            }
            return 32; // Default fallback
        }
        
        /// <summary>
        /// Calculate the appropriate navigator height for this layout
        /// </summary>
        public virtual int CalculateNavigatorHeight(BeepGridPro grid)
        {
            // Default calculation based on painter
            var painter = GetNavigationPainter();
            if (painter != null)
            {
                return painter.RecommendedHeight;
            }
            return 48; // Default fallback
        }
        
        #endregion
        
        #region Compatibility Methods
        
        /// <summary>
        /// Check if this layout is compatible with a grid style
        /// </summary>
        public virtual bool IsCompatibleWith(BeepGridStyle gridStyle)
        {
            // Default: compatible with all styles
            return true;
        }
        
        /// <summary>
        /// Check if this layout is compatible with a theme
        /// </summary>
        public virtual bool IsCompatibleWith(IBeepTheme theme)
        {
            // Default: compatible with all themes
            return true;
        }
        
        #endregion
    }
}

