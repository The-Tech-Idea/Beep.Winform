using System;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers
{
    /// <summary>
    /// Accordion style enum
    /// </summary>
    public enum AccordionStyle
    {
        Material3,
        Modern,
        Classic,
        Minimal,
        iOS,
        Fluent2
    }

    /// <summary>
    /// Helper class for mapping AccordionStyle and BeepControlStyle to accordion styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class AccordionStyleHelpers
    {
        /// <summary>
        /// Maps AccordionStyle to BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForAccordion(AccordionStyle accordionStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => BeepControlStyle.Material3,
                AccordionStyle.Modern => BeepControlStyle.Material3,
                AccordionStyle.Classic => BeepControlStyle.Material3,
                AccordionStyle.Minimal => BeepControlStyle.Minimal,
                AccordionStyle.iOS => BeepControlStyle.iOS15,
                AccordionStyle.Fluent2 => BeepControlStyle.Fluent2,
                _ => BeepControlStyle.Material3
            };
        }

        /// <summary>
        /// Gets recommended item height for accordion style
        /// </summary>
        public static int GetRecommendedItemHeight(AccordionStyle accordionStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => 40,
                AccordionStyle.Modern => 44,
                AccordionStyle.Classic => 36,
                AccordionStyle.Minimal => 36,
                AccordionStyle.iOS => 44,
                AccordionStyle.Fluent2 => 40,
                _ => 40
            };
        }

        /// <summary>
        /// Gets recommended child item height
        /// </summary>
        public static int GetRecommendedChildItemHeight(AccordionStyle accordionStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => 32,
                AccordionStyle.Modern => 36,
                AccordionStyle.Classic => 30,
                AccordionStyle.Minimal => 30,
                AccordionStyle.iOS => 36,
                AccordionStyle.Fluent2 => 32,
                _ => 32
            };
        }

        /// <summary>
        /// Gets recommended indentation width for child items
        /// </summary>
        public static int GetRecommendedIndentation(AccordionStyle accordionStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => 20,
                AccordionStyle.Modern => 24,
                AccordionStyle.Classic => 16,
                AccordionStyle.Minimal => 16,
                AccordionStyle.iOS => 24,
                AccordionStyle.Fluent2 => 20,
                _ => 20
            };
        }

        /// <summary>
        /// Gets recommended spacing between items
        /// </summary>
        public static int GetRecommendedSpacing(AccordionStyle accordionStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => 2,
                AccordionStyle.Modern => 4,
                AccordionStyle.Classic => 1,
                AccordionStyle.Minimal => 0,
                AccordionStyle.iOS => 4,
                AccordionStyle.Fluent2 => 2,
                _ => 2
            };
        }

        /// <summary>
        /// Gets recommended border radius for accordion style
        /// </summary>
        public static int GetRecommendedBorderRadius(AccordionStyle accordionStyle, BeepControlStyle controlStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => BeepStyling.GetRadius(controlStyle),
                AccordionStyle.Modern => BeepStyling.GetRadius(controlStyle),
                AccordionStyle.Classic => 4,
                AccordionStyle.Minimal => 0,
                AccordionStyle.iOS => 8,
                AccordionStyle.Fluent2 => BeepStyling.GetRadius(controlStyle),
                _ => BeepStyling.GetRadius(controlStyle)
            };
        }

        /// <summary>
        /// Gets recommended padding for accordion control
        /// </summary>
        public static int GetRecommendedPadding(AccordionStyle accordionStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => 8,
                AccordionStyle.Modern => 12,
                AccordionStyle.Classic => 4,
                AccordionStyle.Minimal => 4,
                AccordionStyle.iOS => 12,
                AccordionStyle.Fluent2 => 8,
                _ => 8
            };
        }

        /// <summary>
        /// Gets recommended header height
        /// </summary>
        public static int GetRecommendedHeaderHeight(AccordionStyle accordionStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => 48,
                AccordionStyle.Modern => 52,
                AccordionStyle.Classic => 40,
                AccordionStyle.Minimal => 40,
                AccordionStyle.iOS => 52,
                AccordionStyle.Fluent2 => 48,
                _ => 48
            };
        }

        /// <summary>
        /// Gets recommended highlight width (left border indicator)
        /// </summary>
        public static int GetRecommendedHighlightWidth(AccordionStyle accordionStyle)
        {
            return accordionStyle switch
            {
                AccordionStyle.Material3 => 4,
                AccordionStyle.Modern => 4,
                AccordionStyle.Classic => 3,
                AccordionStyle.Minimal => 2,
                AccordionStyle.iOS => 4,
                AccordionStyle.Fluent2 => 4,
                _ => 4
            };
        }
    }
}
