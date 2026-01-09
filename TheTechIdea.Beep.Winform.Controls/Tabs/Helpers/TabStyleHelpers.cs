using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Helper class for mapping TabStyle to BeepControlStyle and determining styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class TabStyleHelpers
    {
        /// <summary>
        /// Maps TabStyle to appropriate BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForTab(TabStyle tabStyle)
        {
            return tabStyle switch
            {
                TabStyle.Classic or TabStyle.Underline or TabStyle.Capsule
                    => BeepControlStyle.Material3,
                TabStyle.Minimal
                    => BeepControlStyle.Minimal,
                TabStyle.Segmented or TabStyle.Button
                    => BeepControlStyle.Material3,
                TabStyle.Card
                    => BeepControlStyle.Material3,
                _ => BeepControlStyle.Material3 // Default to Material3
            };
        }

        /// <summary>
        /// Gets border radius for tabs based on style and control style
        /// </summary>
        public static int GetBorderRadius(TabStyle style, BeepControlStyle controlStyle)
        {
            // Base radius from control style
            int baseRadius = BeepStyling.GetRadius(controlStyle);

            // Adjust based on tab style
            return style switch
            {
                TabStyle.Capsule
                    => baseRadius * 3, // Very rounded for capsule style
                TabStyle.Card
                    => baseRadius * 2, // Very rounded for card style
                TabStyle.Button
                    => baseRadius, // Standard rounding
                TabStyle.Classic or TabStyle.Underline or TabStyle.Minimal
                    => 0, // No rounding for classic/underline/minimal
                TabStyle.Segmented
                    => baseRadius / 2, // Slight rounding for segmented
                _ => baseRadius // Default
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for tab style
        /// </summary>
        public static bool ShouldShowShadow(TabStyle style, BeepControlStyle controlStyle)
        {
            // Material styles typically use shadows
            if (controlStyle == BeepControlStyle.Material3 || controlStyle == BeepControlStyle.Material)
            {
                return style switch
                {
                    TabStyle.Card
                        => true,
                    TabStyle.Classic or TabStyle.Underline or TabStyle.Minimal
                        => false,
                    _ => false
                };
            }

            return style switch
            {
                TabStyle.Card => true,
                _ => false
            };
        }

        /// <summary>
        /// Gets shadow color based on theme
        /// </summary>
        public static Color GetShadowColor(IBeepTheme theme, bool useThemeColors, int elevation = 2)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ShadowColor != Color.Empty)
                    return Color.FromArgb(Math.Min(255, elevation * 10), theme.ShadowColor);
            }

            return Color.FromArgb(Math.Min(255, elevation * 10), Color.Black);
        }

        /// <summary>
        /// Gets shadow offset for tab style
        /// </summary>
        public static Point GetShadowOffset(TabStyle style, BeepControlStyle controlStyle, int elevation = 2)
        {
            if (!ShouldShowShadow(style, controlStyle))
                return Point.Empty;

            return style switch
            {
                TabStyle.Card
                    => new Point(0, elevation), // Subtle shadow
                _ => new Point(0, elevation)
            };
        }

        /// <summary>
        /// Gets border width for tabs
        /// </summary>
        public static int GetBorderWidth(TabStyle style, BeepControlStyle controlStyle, bool isSelected)
        {
            int baseWidth = 1;

            if (isSelected)
            {
                baseWidth = 2; // Thicker border for selected
            }

            return style switch
            {
                TabStyle.Minimal or TabStyle.Underline
                    => 0, // No border for minimal/underline
                _ => baseWidth
            };
        }

        /// <summary>
        /// Gets recommended header height for tab style
        /// </summary>
        public static int GetRecommendedHeaderHeight(TabStyle style)
        {
            return style switch
            {
                TabStyle.Card
                    => 50, // Larger for card style
                TabStyle.Button or TabStyle.Capsule
                    => 40, // Medium for button/capsule styles
                TabStyle.Minimal
                    => 28, // Smaller for minimal
                _ => 30 // Default
            };
        }

        /// <summary>
        /// Gets recommended tab padding for tab style
        /// </summary>
        public static Padding GetRecommendedTabPadding(TabStyle style)
        {
            return style switch
            {
                TabStyle.Card
                    => new Padding(16, 12, 16, 12), // More padding for cards
                TabStyle.Button or TabStyle.Capsule
                    => new Padding(12, 8, 12, 8), // Standard padding
                TabStyle.Minimal
                    => new Padding(8, 6, 8, 6), // Less padding for minimal
                _ => new Padding(12, 8, 12, 8) // Default
            };
        }

        /// <summary>
        /// Gets underline/indicator thickness for tab style
        /// </summary>
        public static int GetIndicatorThickness(TabStyle style)
        {
            return style switch
            {
                TabStyle.Underline
                    => 3, // Thick underline
                TabStyle.Classic
                    => 2, // Medium indicator
                TabStyle.Minimal
                    => 1, // Thin indicator
                _ => 2 // Default
            };
        }
    }
}
