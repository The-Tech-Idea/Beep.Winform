using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Lovs.Helpers
{
    /// <summary>
    /// Helper class for mapping BeepControlStyle to LOV styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class LovStyleHelpers
    {
        /// <summary>
        /// Gets border radius for LOV control based on control style
        /// </summary>
        public static int GetBorderRadius(BeepControlStyle controlStyle, int controlHeight)
        {
            return BeepStyling.GetRadius(controlStyle);
        }

        /// <summary>
        /// Gets recommended padding for LOV control
        /// </summary>
        public static Padding GetRecommendedPadding(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => new Padding(4, 2, 4, 2),
                BeepControlStyle.iOS15 => new Padding(5, 3, 5, 3),
                BeepControlStyle.Fluent2 => new Padding(4, 2, 4, 2),
                BeepControlStyle.Minimal => new Padding(3, 1, 3, 1),
                _ => new Padding(4, 2, 4, 2)
            };
        }

        /// <summary>
        /// Gets recommended minimum height for LOV control
        /// </summary>
        public static int GetRecommendedMinimumHeight(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 30,
                BeepControlStyle.iOS15 => 34,
                BeepControlStyle.Fluent2 => 32,
                BeepControlStyle.Minimal => 28,
                _ => 30
            };
        }

        /// <summary>
        /// Gets recommended minimum width for LOV control
        /// </summary>
        public static int GetRecommendedMinimumWidth(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 200,
                BeepControlStyle.iOS15 => 220,
                BeepControlStyle.Fluent2 => 210,
                BeepControlStyle.Minimal => 180,
                _ => 200
            };
        }

        /// <summary>
        /// Gets recommended key textbox width ratio (as percentage of total width)
        /// </summary>
        public static float GetKeyTextBoxWidthRatio(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 0.2f, // 20%
                BeepControlStyle.iOS15 => 0.25f, // 25%
                BeepControlStyle.Fluent2 => 0.22f, // 22%
                BeepControlStyle.Minimal => 0.18f, // 18%
                _ => 0.2f
            };
        }

        /// <summary>
        /// Gets recommended button width ratio (as percentage of total width)
        /// </summary>
        public static float GetButtonWidthRatio(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 0.1f, // 10%
                BeepControlStyle.iOS15 => 0.12f, // 12%
                BeepControlStyle.Fluent2 => 0.11f, // 11%
                BeepControlStyle.Minimal => 0.09f, // 9%
                _ => 0.1f
            };
        }

        /// <summary>
        /// Gets recommended spacing between textboxes and button
        /// </summary>
        public static int GetRecommendedSpacing(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 5,
                BeepControlStyle.iOS15 => 6,
                BeepControlStyle.Fluent2 => 5,
                BeepControlStyle.Minimal => 4,
                _ => 5
            };
        }

        /// <summary>
        /// Gets icon size ratio for button (as percentage of button size)
        /// </summary>
        public static float GetIconSizeRatio(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 0.6f,
                BeepControlStyle.iOS15 => 0.65f,
                BeepControlStyle.Fluent2 => 0.6f,
                BeepControlStyle.Minimal => 0.7f,
                _ => 0.6f
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for LOV control
        /// </summary>
        public static bool ShouldShowShadow(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => false, // LOV typically doesn't need shadows
                BeepControlStyle.Fluent2 => false,
                BeepControlStyle.iOS15 => false,
                BeepControlStyle.Minimal => false,
                _ => false
            };
        }
    }
}
