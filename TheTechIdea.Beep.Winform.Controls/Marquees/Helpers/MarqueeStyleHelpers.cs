using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Helpers
{
    /// <summary>
    /// Helper class for mapping BeepControlStyle to marquee styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class MarqueeStyleHelpers
    {
        /// <summary>
        /// Gets border radius for marquee control based on control style
        /// </summary>
        public static int GetBorderRadius(BeepControlStyle controlStyle, int controlHeight)
        {
            return BeepStyling.GetRadius(controlStyle);
        }

        /// <summary>
        /// Gets recommended padding for marquee control
        /// </summary>
        public static Padding GetRecommendedPadding(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => new Padding(8, 4, 8, 4),
                BeepControlStyle.iOS15 => new Padding(10, 6, 10, 6),
                BeepControlStyle.Fluent2 => new Padding(8, 4, 8, 4),
                BeepControlStyle.Minimal => new Padding(6, 3, 6, 3),
                _ => new Padding(8, 4, 8, 4)
            };
        }

        /// <summary>
        /// Gets recommended minimum height for marquee control
        /// </summary>
        public static int GetRecommendedMinimumHeight(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 40,
                BeepControlStyle.iOS15 => 44,
                BeepControlStyle.Fluent2 => 42,
                BeepControlStyle.Minimal => 36,
                _ => 40
            };
        }

        /// <summary>
        /// Gets recommended component spacing for marquee control
        /// </summary>
        public static int GetRecommendedComponentSpacing(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 20,
                BeepControlStyle.iOS15 => 24,
                BeepControlStyle.Fluent2 => 22,
                BeepControlStyle.Minimal => 16,
                _ => 20
            };
        }

        /// <summary>
        /// Gets recommended scroll speed for marquee control
        /// </summary>
        public static float GetRecommendedScrollSpeed(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 2.0f,
                BeepControlStyle.iOS15 => 2.5f,
                BeepControlStyle.Fluent2 => 2.2f,
                BeepControlStyle.Minimal => 1.8f,
                _ => 2.0f
            };
        }

        /// <summary>
        /// Gets recommended scroll interval (timer interval) for marquee control
        /// </summary>
        public static int GetRecommendedScrollInterval(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 30,
                BeepControlStyle.iOS15 => 25,
                BeepControlStyle.Fluent2 => 28,
                BeepControlStyle.Minimal => 35,
                _ => 30
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for marquee items
        /// </summary>
        public static bool ShouldShowShadow(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => false, // Marquee items typically don't need shadows
                BeepControlStyle.Fluent2 => false,
                BeepControlStyle.iOS15 => false,
                BeepControlStyle.Minimal => false,
                _ => false
            };
        }
    }
}
