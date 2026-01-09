using System;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Images.Helpers
{
    /// <summary>
    /// Helper class for mapping image styles and providing recommended properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class ImageStyleHelpers
    {
        /// <summary>
        /// Gets recommended corner radius for rounded shapes based on ControlStyle
        /// </summary>
        public static int GetRecommendedCornerRadius(BeepControlStyle controlStyle, Rectangle bounds)
        {
            int baseRadius = BeepStyling.GetRadius(controlStyle);
            // Scale based on image size
            int minDimension = Math.Min(bounds.Width, bounds.Height);
            return Math.Min(baseRadius, minDimension / 4);
        }

        /// <summary>
        /// Gets recommended base size for images
        /// </summary>
        public static int GetRecommendedBaseSize(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 24,
                BeepControlStyle.iOS15 => 28,
                BeepControlStyle.Fluent2 => 24,
                BeepControlStyle.Minimal => 20,
                _ => 24
            };
        }

        /// <summary>
        /// Gets recommended opacity for images based on state
        /// </summary>
        public static float GetRecommendedOpacity(bool isEnabled, bool isHovered = false)
        {
            if (!isEnabled)
                return 0.5f;
            if (isHovered)
                return 0.9f;
            return 1.0f;
        }

        /// <summary>
        /// Gets recommended scale factor for images
        /// </summary>
        public static float GetRecommendedScaleFactor(BeepControlStyle controlStyle)
        {
            return 1.0f; // Default scale
        }
    }
}
