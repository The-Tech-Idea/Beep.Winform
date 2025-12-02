using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Configuration for responsive layout behavior
    /// Defines breakpoints and rules for adapting grid layout to different screen sizes
    /// </summary>
    public class ResponsiveConfig
    {
        /// <summary>
        /// Breakpoint for extra small screens (mobile portrait)
        /// </summary>
        public int BreakpointExtraSmall { get; set; } = 320;
        
        /// <summary>
        /// Breakpoint for small screens (mobile landscape, small tablets)
        /// </summary>
        public int BreakpointSmall { get; set; } = 640;
        
        /// <summary>
        /// Breakpoint for medium screens (tablets)
        /// </summary>
        public int BreakpointMedium { get; set; } = 768;
        
        /// <summary>
        /// Breakpoint for large screens (laptops, desktops)
        /// </summary>
        public int BreakpointLarge { get; set; } = 1024;
        
        /// <summary>
        /// Breakpoint for extra large screens (large desktops)
        /// </summary>
        public int BreakpointExtraLarge { get; set; } = 1280;
        
        /// <summary>
        /// Breakpoint for 2XL screens (ultra-wide monitors)
        /// </summary>
        public int Breakpoint2XL { get; set; } = 1536;
        
        /// <summary>
        /// Rules for column visibility at different breakpoints
        /// Key = column name, Value = visibility rules
        /// </summary>
        public Dictionary<string, ColumnVisibilityRule> ColumnVisibilityRules { get; set; } 
            = new Dictionary<string, ColumnVisibilityRule>();
        
        /// <summary>
        /// Whether to collapse navigation on small screens
        /// </summary>
        public bool CollapseNavigationOnSmall { get; set; } = false;
        
        /// <summary>
        /// Whether to hide column headers on extra small screens
        /// </summary>
        public bool HideHeadersOnExtraSmall { get; set; } = false;
        
        /// <summary>
        /// Whether to stack columns vertically on mobile
        /// </summary>
        public bool StackColumnsOnMobile { get; set; } = false;
        
        /// <summary>
        /// Minimum column width at any breakpoint
        /// </summary>
        public int MinimumColumnWidth { get; set; } = 60;
        
        /// <summary>
        /// Whether to enable horizontal scrolling on small screens
        /// </summary>
        public bool EnableHorizontalScrollOnSmall { get; set; } = true;
        
        /// <summary>
        /// Row height adjustments per breakpoint
        /// </summary>
        public Dictionary<ScreenSize, int> RowHeightAdjustments { get; set; } 
            = new Dictionary<ScreenSize, int>();
        
        /// <summary>
        /// Whether responsive behavior is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
    
    /// <summary>
    /// Column visibility rules for different screen sizes
    /// </summary>
    public class ColumnVisibilityRule
    {
        /// <summary>
        /// Priority level (higher = more important, shown on smaller screens)
        /// </summary>
        public int Priority { get; set; } = 5;
        
        /// <summary>
        /// Minimum screen size where this column is visible
        /// </summary>
        public ScreenSize MinimumScreenSize { get; set; } = ScreenSize.ExtraSmall;
        
        /// <summary>
        /// Whether column is visible at extra small screens (< 320px)
        /// </summary>
        public bool VisibleOnExtraSmall { get; set; } = false;
        
        /// <summary>
        /// Whether column is visible at small screens (< 640px)
        /// </summary>
        public bool VisibleOnSmall { get; set; } = true;
        
        /// <summary>
        /// Whether column is visible at medium screens (< 768px)
        /// </summary>
        public bool VisibleOnMedium { get; set; } = true;
        
        /// <summary>
        /// Whether column is visible at large screens (< 1024px)
        /// </summary>
        public bool VisibleOnLarge { get; set; } = true;
        
        /// <summary>
        /// Whether column is visible at extra large screens (< 1280px)
        /// </summary>
        public bool VisibleOnExtraLarge { get; set; } = true;
        
        /// <summary>
        /// Whether column is visible at 2XL screens (>= 1280px)
        /// </summary>
        public bool VisibleOn2XL { get; set; } = true;
        
        /// <summary>
        /// Checks if column should be visible at the given screen size
        /// </summary>
        public bool IsVisibleAt(ScreenSize screenSize)
        {
            return screenSize switch
            {
                ScreenSize.ExtraSmall => VisibleOnExtraSmall,
                ScreenSize.Small => VisibleOnSmall,
                ScreenSize.Medium => VisibleOnMedium,
                ScreenSize.Large => VisibleOnLarge,
                ScreenSize.ExtraLarge => VisibleOnExtraLarge,
                ScreenSize.TwoXL => VisibleOn2XL,
                _ => true
            };
        }
    }
    
    /// <summary>
    /// Screen size categories
    /// </summary>
    public enum ScreenSize
    {
        ExtraSmall = 0,  // < 320px
        Small = 1,       // 320-640px
        Medium = 2,      // 640-768px
        Large = 3,       // 768-1024px
        ExtraLarge = 4,  // 1024-1280px
        TwoXL = 5        // >= 1280px
    }
    
    /// <summary>
    /// Helper methods for creating common responsive configurations
    /// </summary>
    public static class ResponsiveConfigPresets
    {
        /// <summary>
        /// Creates a mobile-first responsive configuration
        /// Shows only essential columns on mobile, progressively more on larger screens
        /// </summary>
        public static ResponsiveConfig MobileFirst()
        {
            return new ResponsiveConfig
            {
                CollapseNavigationOnSmall = true,
                HideHeadersOnExtraSmall = false,
                EnableHorizontalScrollOnSmall = true,
                MinimumColumnWidth = 80,
                RowHeightAdjustments = new Dictionary<ScreenSize, int>
                {
                    { ScreenSize.ExtraSmall, 48 },  // Larger rows for touch
                    { ScreenSize.Small, 44 },
                    { ScreenSize.Medium, 40 },
                    { ScreenSize.Large, 36 }
                }
            };
        }
        
        /// <summary>
        /// Creates a desktop-optimized responsive configuration
        /// Assumes primarily desktop usage with some tablet support
        /// </summary>
        public static ResponsiveConfig DesktopFirst()
        {
            return new ResponsiveConfig
            {
                CollapseNavigationOnSmall = false,
                HideHeadersOnExtraSmall = false,
                EnableHorizontalScrollOnSmall = true,
                MinimumColumnWidth = 60
            };
        }
        
        /// <summary>
        /// Creates a tablet-optimized responsive configuration
        /// </summary>
        public static ResponsiveConfig TabletOptimized()
        {
            return new ResponsiveConfig
            {
                CollapseNavigationOnSmall = true,
                HideHeadersOnExtraSmall = false,
                EnableHorizontalScrollOnSmall = true,
                MinimumColumnWidth = 70,
                RowHeightAdjustments = new Dictionary<ScreenSize, int>
                {
                    { ScreenSize.Small, 44 },      // Touch-friendly
                    { ScreenSize.Medium, 42 },
                    { ScreenSize.Large, 40 }
                }
            };
        }
    }
}

