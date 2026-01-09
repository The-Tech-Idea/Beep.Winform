using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers
{
    /// <summary>
    /// Helper class for mapping RadioGroupRenderStyle to BeepControlStyle and determining styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class RadioGroupStyleHelpers
    {
        /// <summary>
        /// Maps RadioGroupRenderStyle to appropriate BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForRenderStyle(RadioGroupRenderStyle renderStyle)
        {
            return renderStyle switch
            {
                RadioGroupRenderStyle.Material or RadioGroupRenderStyle.Card or RadioGroupRenderStyle.Chip
                    => BeepControlStyle.Material3,
                RadioGroupRenderStyle.Circular or RadioGroupRenderStyle.Checkbox
                    => BeepControlStyle.Material3,
                RadioGroupRenderStyle.Flat
                    => BeepControlStyle.Minimal,
                RadioGroupRenderStyle.Button or RadioGroupRenderStyle.Segmented or RadioGroupRenderStyle.Pill
                    => BeepControlStyle.Material3,
                RadioGroupRenderStyle.Toggle
                    => BeepControlStyle.Material3,
                RadioGroupRenderStyle.Tile
                    => BeepControlStyle.Material3,
                _ => BeepControlStyle.Material3 // Default to Material3
            };
        }

        /// <summary>
        /// Gets border radius for radio items based on render style and control style
        /// </summary>
        public static int GetBorderRadius(RadioGroupRenderStyle renderStyle, BeepControlStyle controlStyle)
        {
            // Base radius from control style
            int baseRadius = BeepStyling.GetRadius(controlStyle);

            // Adjust based on render style
            return renderStyle switch
            {
                RadioGroupRenderStyle.Pill
                    => baseRadius * 3, // Very rounded for pill style
                RadioGroupRenderStyle.Chip or RadioGroupRenderStyle.Card
                    => baseRadius * 2, // Very rounded for chip/card styles
                RadioGroupRenderStyle.Button or RadioGroupRenderStyle.Segmented
                    => baseRadius, // Standard rounding
                RadioGroupRenderStyle.Circular or RadioGroupRenderStyle.Checkbox or RadioGroupRenderStyle.Material
                    => 0, // No rounding for circular/checkbox/material (they use circles)
                RadioGroupRenderStyle.Flat
                    => 0, // No rounding for flat
                _ => baseRadius // Default
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for render style
        /// </summary>
        public static bool ShouldShowShadow(RadioGroupRenderStyle renderStyle, BeepControlStyle controlStyle)
        {
            // Material styles typically use shadows
            if (controlStyle == BeepControlStyle.Material3 || controlStyle == BeepControlStyle.Material)
            {
                return renderStyle switch
                {
                    RadioGroupRenderStyle.Card or RadioGroupRenderStyle.Tile
                        => true,
                    RadioGroupRenderStyle.Flat or RadioGroupRenderStyle.Circular or RadioGroupRenderStyle.Checkbox
                        => false,
                    _ => false
                };
            }

            return renderStyle switch
            {
                RadioGroupRenderStyle.Card or RadioGroupRenderStyle.Tile => true,
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
        /// Gets shadow offset for render style
        /// </summary>
        public static Point GetShadowOffset(RadioGroupRenderStyle renderStyle, BeepControlStyle controlStyle, int elevation = 2)
        {
            if (!ShouldShowShadow(renderStyle, controlStyle))
                return Point.Empty;

            return renderStyle switch
            {
                RadioGroupRenderStyle.Card
                    => new Point(0, elevation), // Subtle shadow
                RadioGroupRenderStyle.Tile
                    => new Point(0, elevation * 2), // More pronounced
                _ => new Point(0, elevation)
            };
        }

        /// <summary>
        /// Gets border width for radio items
        /// </summary>
        public static int GetBorderWidth(RadioGroupRenderStyle renderStyle, BeepControlStyle controlStyle, bool isSelected, bool isFocused)
        {
            int baseWidth = 1;

            if (isFocused)
            {
                baseWidth = 2; // Thicker border for focused
            }

            return renderStyle switch
            {
                RadioGroupRenderStyle.Flat or RadioGroupRenderStyle.Circular or RadioGroupRenderStyle.Checkbox
                    => 0, // No border for flat/circular/checkbox
                _ => baseWidth
            };
        }

        /// <summary>
        /// Gets recommended item height for render style
        /// </summary>
        public static int GetRecommendedItemHeight(RadioGroupRenderStyle renderStyle)
        {
            return renderStyle switch
            {
                RadioGroupRenderStyle.Tile
                    => 80, // Larger for tile style
                RadioGroupRenderStyle.Card
                    => 60, // Medium for card style
                RadioGroupRenderStyle.Button or RadioGroupRenderStyle.Segmented or RadioGroupRenderStyle.Pill
                    => 40, // Standard for button styles
                _ => 40 // Default
            };
        }

        /// <summary>
        /// Gets recommended item spacing for render style
        /// </summary>
        public static int GetRecommendedItemSpacing(RadioGroupRenderStyle renderStyle)
        {
            return renderStyle switch
            {
                RadioGroupRenderStyle.Tile
                    => 12, // More spacing for tiles
                RadioGroupRenderStyle.Card
                    => 10, // Medium spacing for cards
                RadioGroupRenderStyle.Segmented
                    => 0, // No spacing for segmented (connected)
                _ => 8 // Default
            };
        }

        /// <summary>
        /// Gets recommended padding for render style
        /// </summary>
        public static Padding GetRecommendedPadding(RadioGroupRenderStyle renderStyle)
        {
            return renderStyle switch
            {
                RadioGroupRenderStyle.Tile
                    => new Padding(16), // More padding for tiles
                RadioGroupRenderStyle.Card
                    => new Padding(12), // Medium padding for cards
                RadioGroupRenderStyle.Button or RadioGroupRenderStyle.Segmented or RadioGroupRenderStyle.Pill
                    => new Padding(12, 8, 12, 8), // Horizontal/vertical padding
                _ => new Padding(8) // Default
            };
        }
    }
}
