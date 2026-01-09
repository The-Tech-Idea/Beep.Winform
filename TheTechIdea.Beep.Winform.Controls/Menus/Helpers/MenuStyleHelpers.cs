using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Menus;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Helper class for mapping MenuBarStyle and BeepControlStyle to menu bar styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class MenuStyleHelpers
    {
        /// <summary>
        /// Maps MenuBarStyle to BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForMenuBar(MenuBarStyle menuBarStyle)
        {
            return menuBarStyle switch
            {
                MenuBarStyle.Classic => BeepControlStyle.None,
                MenuBarStyle.Modern => BeepControlStyle.Material3,
                MenuBarStyle.Material => BeepControlStyle.Material3,
                MenuBarStyle.Compact => BeepControlStyle.Minimal,
                MenuBarStyle.Breadcrumb => BeepControlStyle.Material3,
                MenuBarStyle.Tab => BeepControlStyle.Material3,
                MenuBarStyle.Fluent => BeepControlStyle.Fluent2,
                MenuBarStyle.Bubble => BeepControlStyle.Material3,
                MenuBarStyle.Floating => BeepControlStyle.Material3,
                MenuBarStyle.DropdownCategory => BeepControlStyle.Material3,
                MenuBarStyle.CardLayout => BeepControlStyle.Material3,
                MenuBarStyle.IconGrid => BeepControlStyle.Material3,
                MenuBarStyle.MultiRow => BeepControlStyle.Material3,
                _ => BeepControlStyle.Material3
            };
        }

        /// <summary>
        /// Gets border radius for menu bar based on control style
        /// </summary>
        public static int GetBorderRadius(BeepControlStyle controlStyle, int controlHeight)
        {
            return BeepStyling.GetRadius(controlStyle);
        }

        /// <summary>
        /// Gets recommended padding for menu bar
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
        /// Gets recommended minimum height for menu bar
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
        /// Gets recommended menu item height
        /// </summary>
        public static int GetRecommendedMenuItemHeight(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 32,
                BeepControlStyle.iOS15 => 36,
                BeepControlStyle.Fluent2 => 34,
                BeepControlStyle.Minimal => 28,
                _ => 32
            };
        }

        /// <summary>
        /// Gets recommended menu item spacing
        /// </summary>
        public static int GetRecommendedMenuItemSpacing(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 4,
                BeepControlStyle.iOS15 => 6,
                BeepControlStyle.Fluent2 => 5,
                BeepControlStyle.Minimal => 2,
                _ => 4
            };
        }

        /// <summary>
        /// Gets recommended icon size ratio (as percentage of menu item height)
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
        /// Determines if shadow should be shown for menu bar
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
    }
}
