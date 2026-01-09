using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers
{
    /// <summary>
    /// Helper class for mapping BeepControlStyle to switch styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class SwitchStyleHelpers
    {
        /// <summary>
        /// Gets track size ratio (width:height) for control style
        /// iOS uses 51:31 (~1.65), Material uses 52:32 (~1.625)
        /// </summary>
        public static float GetTrackSizeRatio(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.iOS15
                    => 51f / 31f, // ~1.645 (iOS standard)
                BeepControlStyle.Material3
                    => 52f / 32f, // ~1.625 (Material standard)
                BeepControlStyle.Fluent2
                    => 50f / 30f, // ~1.667 (Fluent standard)
                BeepControlStyle.Minimal or BeepControlStyle.NeoBrutalist
                    => 48f / 28f, // ~1.714 (More compact)
                _ => 52f / 32f // Default Material ratio
            };
        }

        /// <summary>
        /// Gets thumb size as percentage of track height
        /// Typically 0.85-0.95 (85-95% of track height)
        /// </summary>
        public static float GetThumbSizeRatio(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.iOS15
                    => 0.90f, // 90% for iOS
                BeepControlStyle.Material3
                    => 0.875f, // 87.5% for Material
                BeepControlStyle.Fluent2
                    => 0.85f, // 85% for Fluent
                BeepControlStyle.Minimal or BeepControlStyle.NeoBrutalist
                    => 0.95f, // 95% for minimal (larger thumb)
                _ => 0.875f // Default Material ratio
            };
        }

        /// <summary>
        /// Gets border radius for switch track based on control style
        /// </summary>
        public static int GetTrackBorderRadius(BeepControlStyle controlStyle, int trackHeight)
        {
            // Most switch tracks are fully rounded (pill-shaped)
            return trackHeight / 2;
        }

        /// <summary>
        /// Gets animation duration in milliseconds for control style
        /// </summary>
        public static int GetAnimationDuration(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.iOS15
                    => 300, // iOS uses 300ms
                BeepControlStyle.Material3
                    => 200, // Material uses 200ms
                BeepControlStyle.Fluent2
                    => 250, // Fluent uses 250ms
                BeepControlStyle.Minimal or BeepControlStyle.NeoBrutalist
                    => 150, // Minimal uses faster 150ms
                _ => 200 // Default Material duration
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for thumb
        /// </summary>
        public static bool ShouldShowThumbShadow(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3
                    => true, // Material uses shadows
                BeepControlStyle.Fluent2
                    => true, // Fluent uses shadows
                BeepControlStyle.iOS15
                    => false, // iOS doesn't use shadows
                BeepControlStyle.Minimal or BeepControlStyle.NeoBrutalist
                    => false, // Minimal doesn't use shadows
                _ => true // Default to shadows
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
                    return Color.FromArgb(Math.Min(255, elevation * 20), theme.ShadowColor);
            }

            return Color.FromArgb(Math.Min(255, elevation * 20), Color.Black);
        }

        /// <summary>
        /// Gets shadow offset for thumb
        /// </summary>
        public static Point GetShadowOffset(BeepControlStyle controlStyle, int elevation = 2)
        {
            if (!ShouldShowThumbShadow(controlStyle))
                return Point.Empty;

            return controlStyle switch
            {
                BeepControlStyle.Material3
                    => new Point(0, elevation), // Subtle shadow
                BeepControlStyle.Fluent2
                    => new Point(0, elevation), // Subtle shadow
                _ => new Point(0, elevation)
            };
        }

        /// <summary>
        /// Gets recommended minimum size for switch style
        /// </summary>
        public static Size GetRecommendedMinimumSize(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.iOS15
                    => new Size(51, 31), // iOS standard
                BeepControlStyle.Material3
                    => new Size(52, 32), // Material standard
                BeepControlStyle.Fluent2
                    => new Size(50, 30), // Fluent standard
                BeepControlStyle.Minimal or BeepControlStyle.NeoBrutalist
                    => new Size(48, 28), // Minimal compact
                _ => new Size(52, 32) // Default Material
            };
        }

        /// <summary>
        /// Gets recommended padding for switch style
        /// </summary>
        public static Padding GetRecommendedPadding(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.iOS15
                    => new Padding(8), // iOS padding
                BeepControlStyle.Material3
                    => new Padding(8), // Material padding
                BeepControlStyle.Fluent2
                    => new Padding(10), // Fluent padding
                BeepControlStyle.Minimal or BeepControlStyle.NeoBrutalist
                    => new Padding(6), // Minimal padding
                _ => new Padding(8) // Default
            };
        }
    }
}
