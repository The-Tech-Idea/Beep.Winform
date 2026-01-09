using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Helpers
{
    /// <summary>
    /// Helper class for mapping ToggleStyle to BeepControlStyle and determining styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class ToggleStyleHelpers
    {
        /// <summary>
        /// Maps ToggleStyle to appropriate BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForToggle(ToggleStyle toggleStyle)
        {
            return toggleStyle switch
            {
                ToggleStyle.MaterialPill or ToggleStyle.MaterialSquare or ToggleStyle.MaterialSlider or ToggleStyle.MaterialCheckbox or ToggleStyle.MaterialSquareButton
                    => BeepControlStyle.Material3,
                ToggleStyle.iOS
                    => BeepControlStyle.iOS15,
                ToggleStyle.Minimal
                    => BeepControlStyle.Minimal,
                ToggleStyle.ButtonStyle
                    => BeepControlStyle.Material3,
                ToggleStyle.CheckboxStyle
                    => BeepControlStyle.Material3,
                _ => BeepControlStyle.Material3 // Default to Material3
            };
        }

        /// <summary>
        /// Gets border radius for toggle track based on style and control style
        /// </summary>
        public static int GetBorderRadius(ToggleStyle style, BeepControlStyle controlStyle)
        {
            // Base radius from control style using BeepStyling
            int baseRadius = BeepStyling.GetRadius(controlStyle);

            // Adjust based on toggle style
            return style switch
            {
                ToggleStyle.Classic or ToggleStyle.iOS or ToggleStyle.MaterialPill
                    => baseRadius * 2, // Very rounded for pill styles
                ToggleStyle.RectangularLabeled or ToggleStyle.MaterialSquare or ToggleStyle.MaterialSquareButton
                    => baseRadius / 2, // Less rounded for square styles
                ToggleStyle.Minimal
                    => 0, // No rounding for minimal
                _ => baseRadius // Default
            };
        }

        /// <summary>
        /// Gets track shape for toggle style
        /// </summary>
        public static ToggleTrackShape GetTrackShape(ToggleStyle style)
        {
            return style switch
            {
                ToggleStyle.Classic or ToggleStyle.iOS or ToggleStyle.MaterialPill
                    => ToggleTrackShape.Pill,
                ToggleStyle.RectangularLabeled or ToggleStyle.MaterialSquare or ToggleStyle.MaterialSquareButton
                    => ToggleTrackShape.Rectangle,
                ToggleStyle.Minimal
                    => ToggleTrackShape.Rectangle,
                _ => ToggleTrackShape.RoundedRectangle
            };
        }

        /// <summary>
        /// Gets thumb shape for toggle style
        /// </summary>
        public static ToggleThumbShape GetThumbShape(ToggleStyle style)
        {
            return style switch
            {
                ToggleStyle.Classic or ToggleStyle.iOS or ToggleStyle.MaterialPill
                    => ToggleThumbShape.Circle,
                ToggleStyle.RectangularLabeled or ToggleStyle.MaterialSquare
                    => ToggleThumbShape.RoundedSquare,
                ToggleStyle.Minimal
                    => ToggleThumbShape.Square,
                _ => ToggleThumbShape.Circle
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for toggle style
        /// </summary>
        public static bool ShouldShowShadow(ToggleStyle style, BeepControlStyle controlStyle)
        {
            // Material styles typically use shadows
            if (controlStyle == BeepControlStyle.Material3)
            {
                return style switch
                {
                    ToggleStyle.Classic or ToggleStyle.iOS or ToggleStyle.MaterialPill
                        => true,
                    ToggleStyle.Minimal
                        => false,
                    _ => true
                };
            }

            // Other styles may or may not use shadows
            return style switch
            {
                ToggleStyle.Minimal => false,
                _ => true
            };
        }

        /// <summary>
        /// Gets shadow color based on theme
        /// </summary>
        public static Color GetShadowColor(IBeepTheme theme, bool useThemeColors, bool isOn)
        {
            if (useThemeColors && theme != null)
            {
                // Use theme shadow color if available
                if (theme.ShadowColor != Color.Empty)
                    return theme.ShadowColor;

                // Generate shadow from track color
                Color trackColor = ToggleThemeHelpers.GetToggleTrackColor(theme, useThemeColors, isOn);
                return Color.FromArgb(40, Color.Black);
            }

            // Default shadow
            return Color.FromArgb(40, Color.Black);
        }

        /// <summary>
        /// Gets shadow offset for toggle style
        /// </summary>
        public static Point GetShadowOffset(ToggleStyle style, BeepControlStyle controlStyle)
        {
            if (!ShouldShowShadow(style, controlStyle))
                return Point.Empty;

            return style switch
            {
                ToggleStyle.Classic or ToggleStyle.iOS
                    => new Point(0, 2), // Subtle shadow
                ToggleStyle.MaterialPill
                    => new Point(0, 1), // Very subtle
                _ => new Point(0, 1)
            };
        }

        /// <summary>
        /// Determines if gradient should be used for track
        /// </summary>
        public static bool ShouldUseGradient(ToggleStyle style, BeepControlStyle controlStyle)
        {
            return style switch
            {
                ToggleStyle.MaterialPill or ToggleStyle.MaterialSquare
                    => true,
                ToggleStyle.Classic or ToggleStyle.iOS
                    => controlStyle == BeepControlStyle.Material3,
                _ => false
            };
        }

        /// <summary>
        /// Gets gradient type for toggle style
        /// </summary>
        public static System.Drawing.Drawing2D.LinearGradientMode GetGradientType(ToggleStyle style)
        {
            return System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }

        /// <summary>
        /// Gets border width for toggle track
        /// </summary>
        public static int GetBorderWidth(ToggleStyle style, BeepControlStyle controlStyle, bool isHighContrast)
        {
            int baseWidth = 1;

            if (isHighContrast)
            {
                baseWidth = 2; // Thicker borders in high contrast
            }

            return style switch
            {
                ToggleStyle.Minimal
                    => 0, // No border for minimal
                ToggleStyle.Classic or ToggleStyle.iOS
                    => baseWidth,
                _ => baseWidth
            };
        }

        /// <summary>
        /// Gets border color for toggle track
        /// </summary>
        public static Color GetBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn,
            ToggleStyle style)
        {
            return ToggleThemeHelpers.GetToggleBorderColor(theme, useThemeColors, isOn);
        }

        /// <summary>
        /// Gets recommended size for toggle style
        /// </summary>
        public static Size GetRecommendedSize(ToggleStyle style)
        {
            return style switch
            {
                ToggleStyle.ButtonStyle
                    => new Size(100, 36), // Button-style needs more width
                ToggleStyle.LabeledTrack
                    => new Size(80, 32), // Needs space for labels
                ToggleStyle.RectangularLabeled
                    => new Size(90, 36),
                ToggleStyle.MaterialPill
                    => new Size(60, 28),
                _ => new Size(60, 28) // Default
            };
        }

        /// <summary>
        /// Gets minimum size for toggle style
        /// </summary>
        public static Size GetMinimumSize(ToggleStyle style)
        {
            return style switch
            {
                ToggleStyle.ButtonStyle
                    => new Size(60, 28),
                ToggleStyle.LabeledTrack
                    => new Size(50, 24),
                _ => new Size(40, 20) // Default minimum
            };
        }
    }
}
