using System;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Helper class for tooltip style properties and recommendations
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class ToolTipStyleHelpers
    {
        /// <summary>
        /// Gets recommended border radius for tooltips based on ControlStyle
        /// </summary>
        public static int GetRecommendedBorderRadius(BeepControlStyle controlStyle)
        {
            return BeepStyling.GetRadius(controlStyle);
        }

        /// <summary>
        /// Gets recommended padding for tooltip content
        /// </summary>
        public static int GetRecommendedPadding(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Minimal => 8,
                BeepControlStyle.NotionMinimal => 8,
                _ => 12 // Material3, iOS15, Fluent2, etc.
            };
        }

        /// <summary>
        /// Gets recommended arrow size for tooltips
        /// </summary>
        public static int GetRecommendedArrowSize(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Minimal => 7,
                BeepControlStyle.NotionMinimal => 7,
                _ => 8 // Standard
            };
        }

        /// <summary>
        /// Gets recommended shadow offset for tooltips
        /// </summary>
        public static int GetRecommendedShadowOffset(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Minimal => 3,
                BeepControlStyle.NotionMinimal => 3,
                _ => 4 // Standard
            };
        }

        /// <summary>
        /// Gets recommended minimum width for tooltips
        /// </summary>
        public static int GetRecommendedMinWidth(BeepControlStyle controlStyle)
        {
            // A readability FLOOR, not a slab. These used to be 100/120/150, applied as a hard
            // minimum on the final size, so a one-word tooltip like "Save" - which measures about
            // 58px including padding - was inflated to 150px and looked nothing like its content.
            // A tooltip should hug its text; what controls a long tooltip's shape is the MAX
            // width (wrapping), not the min. These values only stop a degenerate sliver.
            return controlStyle switch
            {
                BeepControlStyle.NotionMinimal => 56,
                BeepControlStyle.Minimal => 60,
                _ => 64 // Standard
            };
        }

        /// <summary>
        /// Gets recommended maximum width for tooltips
        /// </summary>
        public static int GetRecommendedMaxWidth(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Minimal => 350,
                BeepControlStyle.NotionMinimal => 350,
                _ => 400 // Standard
            };
        }

        /// <summary>
        /// Gets recommended font size for tooltip text
        /// </summary>
        public static float GetRecommendedFontSize(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.NotionMinimal => 8.5f,
                BeepControlStyle.Minimal => 9f,
                _ => 9.5f // Standard
            };
        }

        /// <summary>
        /// Gets recommended title font size for tooltips
        /// </summary>
        public static float GetRecommendedTitleFontSize(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Minimal => 10f,
                BeepControlStyle.NotionMinimal => 10f,
                _ => 10.5f // Standard
            };
        }

        /// <summary>
        /// Gets recommended spacing between tooltip elements
        /// </summary>
        public static int GetRecommendedSpacing(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.NotionMinimal => 4,
                BeepControlStyle.Minimal => 6,
                _ => 8 // Standard
            };
        }

        /// <summary>
        /// Gets recommended offset from target element
        /// </summary>
        public static int GetRecommendedOffset(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Minimal => 7,
                BeepControlStyle.NotionMinimal => 7,
                _ => 8 // Standard
            };
        }
    }
}
