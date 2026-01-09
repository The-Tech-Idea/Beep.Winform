using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Helpers
{
    /// <summary>
    /// Helper class for mapping NumericStyle and BeepControlStyle to numeric control styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class NumericStyleHelpers
    {
        /// <summary>
        /// Gets the recommended button width for a control style
        /// </summary>
        public static int GetButtonWidth(BeepControlStyle controlStyle, NumericSpinButtonSize buttonSize)
        {
            int baseWidth = controlStyle switch
            {
                BeepControlStyle.Material3 => 24,
                BeepControlStyle.iOS15 => 28,
                BeepControlStyle.Fluent2 => 26,
                BeepControlStyle.Minimal => 20,
                _ => 24
            };

            return buttonSize switch
            {
                NumericSpinButtonSize.Small => (int)(baseWidth * 0.75f),
                NumericSpinButtonSize.Standard => baseWidth,
                NumericSpinButtonSize.Large => (int)(baseWidth * 1.25f),
                NumericSpinButtonSize.ExtraLarge => (int)(baseWidth * 1.5f),
                _ => baseWidth
            };
        }

        /// <summary>
        /// Gets the recommended button height for a control style
        /// </summary>
        public static int GetButtonHeight(BeepControlStyle controlStyle, NumericSpinButtonSize buttonSize)
        {
            int baseHeight = controlStyle switch
            {
                BeepControlStyle.Material3 => 20,
                BeepControlStyle.iOS15 => 22,
                BeepControlStyle.Fluent2 => 21,
                BeepControlStyle.Minimal => 18,
                _ => 20
            };

            return buttonSize switch
            {
                NumericSpinButtonSize.Small => (int)(baseHeight * 0.75f),
                NumericSpinButtonSize.Standard => baseHeight,
                NumericSpinButtonSize.Large => (int)(baseHeight * 1.25f),
                NumericSpinButtonSize.ExtraLarge => (int)(baseHeight * 1.5f),
                _ => baseHeight
            };
        }

        /// <summary>
        /// Gets border radius for numeric control based on control style
        /// </summary>
        public static int GetBorderRadius(BeepControlStyle controlStyle, int controlHeight)
        {
            return BeepStyling.GetRadius(controlStyle);
        }

        /// <summary>
        /// Gets recommended padding for numeric control
        /// </summary>
        public static Padding GetRecommendedPadding(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => new Padding(8, 6, 8, 6),
                BeepControlStyle.iOS15 => new Padding(10, 8, 10, 8),
                BeepControlStyle.Fluent2 => new Padding(8, 6, 8, 6),
                BeepControlStyle.Minimal => new Padding(6, 4, 6, 4),
                _ => new Padding(8, 6, 8, 6)
            };
        }

        /// <summary>
        /// Gets recommended minimum height for numeric control
        /// </summary>
        public static int GetRecommendedMinimumHeight(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 36,
                BeepControlStyle.iOS15 => 40,
                BeepControlStyle.Fluent2 => 38,
                BeepControlStyle.Minimal => 32,
                _ => 36
            };
        }

        /// <summary>
        /// Gets recommended minimum width for numeric control
        /// </summary>
        public static int GetRecommendedMinimumWidth(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 100,
                BeepControlStyle.iOS15 => 120,
                BeepControlStyle.Fluent2 => 110,
                BeepControlStyle.Minimal => 90,
                _ => 100
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for numeric control
        /// </summary>
        public static bool ShouldShowShadow(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => true,
                BeepControlStyle.Fluent2 => true,
                BeepControlStyle.iOS15 => false,
                BeepControlStyle.Minimal => false,
                _ => true
            };
        }

        /// <summary>
        /// Gets shadow offset for numeric control
        /// </summary>
        public static Point GetShadowOffset(BeepControlStyle controlStyle, int elevation = 1)
        {
            if (!ShouldShowShadow(controlStyle))
                return Point.Empty;

            return controlStyle switch
            {
                BeepControlStyle.Material3 => new Point(0, elevation),
                BeepControlStyle.Fluent2 => new Point(0, elevation),
                _ => new Point(0, elevation)
            };
        }

        /// <summary>
        /// Gets icon size ratio for buttons (as percentage of button size)
        /// </summary>
        public static float GetIconSizeRatio(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 0.5f,
                BeepControlStyle.iOS15 => 0.55f,
                BeepControlStyle.Fluent2 => 0.5f,
                BeepControlStyle.Minimal => 0.6f,
                _ => 0.5f
            };
        }
    }
}
