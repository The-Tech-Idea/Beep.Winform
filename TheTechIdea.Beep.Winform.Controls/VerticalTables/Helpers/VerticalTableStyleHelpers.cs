using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers
{
    /// <summary>
    /// Helper class for mapping VerticalTablePainterStyle to BeepControlStyle and determining styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class VerticalTableStyleHelpers
    {
        /// <summary>
        /// Maps VerticalTablePainterStyle to appropriate BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForTable(VerticalTablePainterStyle tableStyle)
        {
            return tableStyle switch
            {
                VerticalTablePainterStyle.Style1 or VerticalTablePainterStyle.Style3 or VerticalTablePainterStyle.Style5
                    => BeepControlStyle.Material3,
                VerticalTablePainterStyle.Style2
                    => BeepControlStyle.Material3,
                VerticalTablePainterStyle.Style4
                    => BeepControlStyle.DarkGlow,
                VerticalTablePainterStyle.Style6
                    => BeepControlStyle.None,
                VerticalTablePainterStyle.Style7 or VerticalTablePainterStyle.Style8 or VerticalTablePainterStyle.Style9 or VerticalTablePainterStyle.Style10
                    => BeepControlStyle.Material3,
                VerticalTablePainterStyle.Style11
                    => BeepControlStyle.Material3,    // PricingCard — card-based with shadow
                VerticalTablePainterStyle.Style12
                    => BeepControlStyle.None,          // FeatureMatrix — grid with sticky headers
                VerticalTablePainterStyle.Style13
                    => BeepControlStyle.Material3,    // MinimalCompare — thin borders, clean
                VerticalTablePainterStyle.Style14
                    => BeepControlStyle.DarkGlow,     // DarkTerminal — dark bg + neon
                _ => BeepControlStyle.Material3 // Default to Material3
            };
        }

        /// <summary>
        /// Gets border radius for table cards based on style and control style
        /// </summary>
        public static int GetBorderRadius(VerticalTablePainterStyle style, BeepControlStyle controlStyle)
        {
            // Base radius from control style
            int baseRadius = BeepStyling.GetRadius(controlStyle);

            // Adjust based on table style
            return style switch
            {
                VerticalTablePainterStyle.Style1 or VerticalTablePainterStyle.Style3 or VerticalTablePainterStyle.Style5
                    => baseRadius * 2, // Very rounded for card styles
                VerticalTablePainterStyle.Style6
                    => 0, // No rounding for classic table
                VerticalTablePainterStyle.Style11
                    => baseRadius * 2, // PricingCard — very rounded cards
                VerticalTablePainterStyle.Style12
                    => 0,             // FeatureMatrix — grid, no rounding
                VerticalTablePainterStyle.Style13
                    => baseRadius,    // MinimalCompare — standard
                VerticalTablePainterStyle.Style14
                    => 0,             // DarkTerminal — sharp corners for terminal look
                VerticalTablePainterStyle.Style2 or VerticalTablePainterStyle.Style7
                    => baseRadius, // Standard rounding
                _ => baseRadius // Default
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for table style
        /// </summary>
        public static bool ShouldShowShadow(VerticalTablePainterStyle style, BeepControlStyle controlStyle)
        {
            // Material styles typically use shadows
            if (controlStyle == BeepControlStyle.Material3 || controlStyle == BeepControlStyle.Material)
            {
                return style switch
                {
                    VerticalTablePainterStyle.Style1 or VerticalTablePainterStyle.Style3 or VerticalTablePainterStyle.Style5
                        => true,
                    VerticalTablePainterStyle.Style6
                        => false, // Classic table doesn't use shadows
                    VerticalTablePainterStyle.Style11
                        => true,  // PricingCard uses shadows for elevation
                    VerticalTablePainterStyle.Style12
                        => false, // FeatureMatrix — grid, no shadow
                    VerticalTablePainterStyle.Style13
                        => false, // MinimalCompare — no shadows
                    VerticalTablePainterStyle.Style14
                        => false, // DarkTerminal uses glow, not shadows
                    _ => true
                };
            }

            return style switch
            {
                VerticalTablePainterStyle.Style6 => false,
                VerticalTablePainterStyle.Style12 => false,
                VerticalTablePainterStyle.Style13 => false,
                VerticalTablePainterStyle.Style14 => false, // glow instead
                _ => true
            };
        }

        /// <summary>
        /// Gets shadow color based on theme
        /// </summary>
        public static Color GetShadowColor(IBeepTheme theme, bool useThemeColors, int elevation = 4)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ShadowColor != Color.Empty)
                    return Color.FromArgb(Math.Min(255, elevation * 10), theme.ShadowColor);
            }

            return Color.FromArgb(Math.Min(255, elevation * 10), Color.Black);
        }

        /// <summary>
        /// Gets shadow offset for table style
        /// </summary>
        public static Point GetShadowOffset(VerticalTablePainterStyle style, BeepControlStyle controlStyle, int elevation = 4)
        {
            if (!ShouldShowShadow(style, controlStyle))
                return Point.Empty;

            return style switch
            {
                VerticalTablePainterStyle.Style1 or VerticalTablePainterStyle.Style3
                    => new Point(0, elevation), // Subtle shadow
                VerticalTablePainterStyle.Style5
                    => new Point(0, elevation * 2), // More pronounced
                _ => new Point(0, elevation)
            };
        }

        /// <summary>
        /// Gets border width for table cards
        /// </summary>
        public static int GetBorderWidth(VerticalTablePainterStyle style, BeepControlStyle controlStyle, bool isSelected, bool isFeatured)
        {
            int baseWidth = 1;

            if (isSelected || isFeatured)
            {
                baseWidth = 2; // Thicker border for selected/featured
            }

            return style switch
            {
                VerticalTablePainterStyle.Style6
                    => 0, // No border for classic table
                _ => baseWidth
            };
        }

        /// <summary>
        /// Gets recommended header height for table style
        /// </summary>
        public static int GetRecommendedHeaderHeight(VerticalTablePainterStyle style)
        {
            return style switch
            {
                VerticalTablePainterStyle.Style1 or VerticalTablePainterStyle.Style5
                    => 100, // Larger for card styles
                VerticalTablePainterStyle.Style6
                    => 40, // Smaller for classic table
                VerticalTablePainterStyle.Style11
                    => 120, // PricingCard — taller header for price display
                VerticalTablePainterStyle.Style12
                    => 48,  // FeatureMatrix — compact header
                VerticalTablePainterStyle.Style13
                    => 56,  // MinimalCompare — modest header
                VerticalTablePainterStyle.Style14
                    => 56,  // DarkTerminal — compact header
                _ => 80 // Default
            };
        }

        /// <summary>
        /// Gets recommended row height for table style
        /// </summary>
        public static int GetRecommendedRowHeight(VerticalTablePainterStyle style)
        {
            return style switch
            {
                VerticalTablePainterStyle.Style1 or VerticalTablePainterStyle.Style5
                    => 50, // Larger for card styles
                VerticalTablePainterStyle.Style6
                    => 30, // Smaller for classic table
                VerticalTablePainterStyle.Style11
                    => 52, // PricingCard — spacious rows
                VerticalTablePainterStyle.Style12
                    => 36, // FeatureMatrix — compact rows
                VerticalTablePainterStyle.Style13
                    => 36, // MinimalCompare — compact rows
                VerticalTablePainterStyle.Style14
                    => 34, // DarkTerminal — compact rows
                _ => 40 // Default
            };
        }

        /// <summary>
        /// Gets recommended column width for table style
        /// </summary>
        public static int GetRecommendedColumnWidth(VerticalTablePainterStyle style)
        {
            return style switch
            {
                VerticalTablePainterStyle.Style1 or VerticalTablePainterStyle.Style5
                    => 200, // Wider for card styles
                VerticalTablePainterStyle.Style6
                    => 120, // Narrower for classic table
                VerticalTablePainterStyle.Style11
                    => 220, // PricingCard — wide for feature comparisons
                VerticalTablePainterStyle.Style12
                    => 140, // FeatureMatrix — narrower columns
                VerticalTablePainterStyle.Style13
                    => 130, // MinimalCompare — compact
                VerticalTablePainterStyle.Style14
                    => 140, // DarkTerminal — compact
                _ => 150 // Default
            };
        }
    }
}
