using System;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers
{
    /// <summary>
    /// CheckBox style enum
    /// </summary>
    public enum CheckBoxStyle
    {
        Material3,
        Modern,
        Classic,
        Minimal,
        iOS,
        Fluent2,
        Switch,
        Button
    }

    /// <summary>
    /// Helper class for mapping CheckBoxStyle and BeepControlStyle to checkbox styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class CheckBoxStyleHelpers
    {
        /// <summary>
        /// Maps CheckBoxStyle to BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForCheckBox(CheckBoxStyle checkBoxStyle)
        {
            return checkBoxStyle switch
            {
                CheckBoxStyle.Material3 => BeepControlStyle.Material3,
                CheckBoxStyle.Modern => BeepControlStyle.Material3,
                CheckBoxStyle.Classic => BeepControlStyle.Material3,
                CheckBoxStyle.Minimal => BeepControlStyle.Minimal,
                CheckBoxStyle.iOS => BeepControlStyle.iOS15,
                CheckBoxStyle.Fluent2 => BeepControlStyle.Fluent2,
                CheckBoxStyle.Switch => BeepControlStyle.Material3,
                CheckBoxStyle.Button => BeepControlStyle.Material3,
                _ => BeepControlStyle.Material3
            };
        }

        /// <summary>
        /// Gets recommended checkbox size for style
        /// </summary>
        public static int GetRecommendedCheckBoxSize(CheckBoxStyle checkBoxStyle)
        {
            return checkBoxStyle switch
            {
                CheckBoxStyle.Material3 => 20,
                CheckBoxStyle.Modern => 22,
                CheckBoxStyle.Classic => 18,
                CheckBoxStyle.Minimal => 16,
                CheckBoxStyle.iOS => 24,
                CheckBoxStyle.Fluent2 => 20,
                CheckBoxStyle.Switch => 20,
                CheckBoxStyle.Button => 20,
                _ => 20
            };
        }

        /// <summary>
        /// Gets recommended spacing between checkbox and text
        /// </summary>
        public static int GetRecommendedSpacing(CheckBoxStyle checkBoxStyle)
        {
            return checkBoxStyle switch
            {
                CheckBoxStyle.Material3 => 8,
                CheckBoxStyle.Modern => 10,
                CheckBoxStyle.Classic => 6,
                CheckBoxStyle.Minimal => 4,
                CheckBoxStyle.iOS => 12,
                CheckBoxStyle.Fluent2 => 8,
                CheckBoxStyle.Switch => 8,
                CheckBoxStyle.Button => 8,
                _ => 8
            };
        }

        /// <summary>
        /// Gets recommended border radius for checkbox style
        /// </summary>
        public static int GetRecommendedBorderRadius(CheckBoxStyle checkBoxStyle, BeepControlStyle controlStyle)
        {
            return checkBoxStyle switch
            {
                CheckBoxStyle.Material3 => BeepStyling.GetRadius(controlStyle),
                CheckBoxStyle.Modern => BeepStyling.GetRadius(controlStyle),
                CheckBoxStyle.Classic => 4,
                CheckBoxStyle.Minimal => 0,
                CheckBoxStyle.iOS => 6,
                CheckBoxStyle.Fluent2 => BeepStyling.GetRadius(controlStyle),
                CheckBoxStyle.Switch => BeepStyling.GetRadius(controlStyle),
                CheckBoxStyle.Button => BeepStyling.GetRadius(controlStyle),
                _ => BeepStyling.GetRadius(controlStyle)
            };
        }

        /// <summary>
        /// Gets recommended border width for checkbox style
        /// </summary>
        public static int GetRecommendedBorderWidth(CheckBoxStyle checkBoxStyle)
        {
            return checkBoxStyle switch
            {
                CheckBoxStyle.Material3 => 2,
                CheckBoxStyle.Modern => 2,
                CheckBoxStyle.Classic => 2,
                CheckBoxStyle.Minimal => 1,
                CheckBoxStyle.iOS => 2,
                CheckBoxStyle.Fluent2 => 2,
                CheckBoxStyle.Switch => 2,
                CheckBoxStyle.Button => 2,
                _ => 2
            };
        }

        /// <summary>
        /// Gets recommended check mark thickness for checkbox style
        /// </summary>
        public static int GetRecommendedCheckMarkThickness(CheckBoxStyle checkBoxStyle)
        {
            return checkBoxStyle switch
            {
                CheckBoxStyle.Material3 => 2,
                CheckBoxStyle.Modern => 2,
                CheckBoxStyle.Classic => 2,
                CheckBoxStyle.Minimal => 1,
                CheckBoxStyle.iOS => 3,
                CheckBoxStyle.Fluent2 => 2,
                CheckBoxStyle.Switch => 2,
                CheckBoxStyle.Button => 2,
                _ => 2
            };
        }

        /// <summary>
        /// Gets recommended padding for checkbox control
        /// </summary>
        public static int GetRecommendedPadding(CheckBoxStyle checkBoxStyle)
        {
            return checkBoxStyle switch
            {
                CheckBoxStyle.Material3 => 4,
                CheckBoxStyle.Modern => 6,
                CheckBoxStyle.Classic => 2,
                CheckBoxStyle.Minimal => 2,
                CheckBoxStyle.iOS => 6,
                CheckBoxStyle.Fluent2 => 4,
                CheckBoxStyle.Switch => 4,
                CheckBoxStyle.Button => 4,
                _ => 4
            };
        }
    }
}
