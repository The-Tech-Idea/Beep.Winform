using System;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Helpers
{
    /// <summary>
    /// Helper class for mapping ChipStyle and BeepControlStyle to chip styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class ChipStyleHelpers
    {
        /// <summary>
        /// Maps ChipStyle to BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForChip(ChipStyle chipStyle)
        {
            return chipStyle switch
            {
                ChipStyle.Modern => BeepControlStyle.Material3,
                ChipStyle.Classic => BeepControlStyle.Material3,
                ChipStyle.Minimalist => BeepControlStyle.Minimal,
                ChipStyle.Colorful => BeepControlStyle.Material3,
                ChipStyle.Professional => BeepControlStyle.Fluent2,
                ChipStyle.Soft => BeepControlStyle.Material3,
                ChipStyle.HighContrast => BeepControlStyle.Material3,
                ChipStyle.Pill => BeepControlStyle.iOS15,
                ChipStyle.Likeable => BeepControlStyle.Material3,
                ChipStyle.Ingredient => BeepControlStyle.Material3,
                ChipStyle.Shaded => BeepControlStyle.Material3,
                ChipStyle.Avatar => BeepControlStyle.Material3,
                ChipStyle.Elevated => BeepControlStyle.Material3,
                ChipStyle.Smooth => BeepControlStyle.Material3,
                ChipStyle.Square => BeepControlStyle.Minimal,
                ChipStyle.Dashed => BeepControlStyle.Material3,
                ChipStyle.Bold => BeepControlStyle.Material3,
                _ => BeepControlStyle.Material3
            };
        }

        /// <summary>
        /// Gets border radius for chip based on chip shape and control style
        /// </summary>
        public static int GetBorderRadius(BeepControlStyle controlStyle, ChipShape chipShape, int chipHeight)
        {
            return chipShape switch
            {
                ChipShape.Pill => chipHeight / 2,
                ChipShape.Rounded => BeepStyling.GetRadius(controlStyle),
                ChipShape.Square => 2,
                ChipShape.Stadium => chipHeight / 2,
                _ => BeepStyling.GetRadius(controlStyle)
            };
        }

        /// <summary>
        /// Gets recommended chip height for chip size
        /// </summary>
        public static int GetRecommendedChipHeight(ChipSize chipSize)
        {
            return chipSize switch
            {
                ChipSize.Small => 24,
                ChipSize.Medium => 32,
                ChipSize.Large => 40,
                _ => 32
            };
        }

        /// <summary>
        /// Gets recommended horizontal padding for chip
        /// </summary>
        public static int GetRecommendedHorizontalPadding(ChipSize chipSize)
        {
            return chipSize switch
            {
                ChipSize.Small => 8,
                ChipSize.Medium => 12,
                ChipSize.Large => 16,
                _ => 12
            };
        }

        /// <summary>
        /// Gets recommended vertical padding for chip
        /// </summary>
        public static int GetRecommendedVerticalPadding(ChipSize chipSize)
        {
            return chipSize switch
            {
                ChipSize.Small => 4,
                ChipSize.Medium => 6,
                ChipSize.Large => 8,
                _ => 6
            };
        }

        /// <summary>
        /// Gets recommended gap between chips
        /// </summary>
        public static int GetRecommendedGap(ChipSize chipSize)
        {
            return chipSize switch
            {
                ChipSize.Small => 4,
                ChipSize.Medium => 6,
                ChipSize.Large => 8,
                _ => 6
            };
        }

        /// <summary>
        /// Gets recommended border width for chip style
        /// </summary>
        public static int GetRecommendedBorderWidth(ChipStyle chipStyle)
        {
            return chipStyle switch
            {
                ChipStyle.Minimalist => 0,
                ChipStyle.Classic => 2,
                ChipStyle.HighContrast => 2,
                ChipStyle.Professional => 1,
                ChipStyle.Dashed => 1,
                _ => 1
            };
        }

        /// <summary>
        /// Determines if borders should be shown for chip style
        /// </summary>
        public static bool ShouldShowBorders(ChipStyle chipStyle)
        {
            return chipStyle switch
            {
                ChipStyle.Minimalist => false,
                ChipStyle.Classic => true,
                ChipStyle.Professional => true,
                ChipStyle.HighContrast => true,
                ChipStyle.Dashed => true,
                _ => true
            };
        }

        /// <summary>
        /// Gets recommended corner radius for chip style
        /// </summary>
        public static int GetRecommendedCornerRadius(ChipStyle chipStyle, ChipShape chipShape, int chipHeight)
        {
            if (chipShape == ChipShape.Pill || chipShape == ChipShape.Stadium)
                return chipHeight / 2;

            return chipStyle switch
            {
                ChipStyle.Square => 2,
                ChipStyle.Pill => chipHeight / 2,
                ChipStyle.Smooth => 12,
                ChipStyle.Modern => 8,
                ChipStyle.Classic => 4,
                _ => 15
            };
        }
    }
}
