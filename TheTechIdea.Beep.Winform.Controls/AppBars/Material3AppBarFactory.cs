using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AppBars
{
    /// <summary>
    /// Helper class to create common app bar configurations following Material 3 patterns
    /// </summary>
    public static class Material3AppBarFactory
    {
        /// <summary>
        /// Creates a standard top app bar for main application windows
        /// </summary>
        public static BeepMaterial3AppBar CreateStandardAppBar(string title, bool showNavigation = true)
        {
            var appBar = new BeepMaterial3AppBar
            {
                Title = title,
                Variant = Material3AppBarVariant.Small,
                ShowDivider = true
            };

            if (showNavigation)
            {
                appBar.NavigationIconPath = AppBarIcons.Menu;
            }

            return appBar;
        }

        /// <summary>
        /// Creates a center-aligned app bar for secondary screens
        /// </summary>
        public static BeepMaterial3AppBar CreateCenterAlignedAppBar(string title, bool showBackButton = true)
        {
            var appBar = new BeepMaterial3AppBar
            {
                Title = title,
                Variant = Material3AppBarVariant.CenterAligned
            };

            if (showBackButton)
            {
                appBar.NavigationIconPath = AppBarIcons.Back;
            }

            return appBar;
        }

        /// <summary>
        /// Creates a search-focused app bar with search functionality
        /// </summary>
        public static BeepMaterial3AppBar CreateSearchAppBar(string title = "Search")
        {
            var appBar = new BeepMaterial3AppBar
            {
                Title = title,
                Variant = Material3AppBarVariant.Small,
                ShowSearch = true
            };

            // Add common search-related action
            appBar.SetActionButton(1, AppBarIcons.Search);
            
            return appBar;
        }

        /// <summary>
        /// Creates an app bar with common business application actions
        /// </summary>
        public static BeepMaterial3AppBar CreateBusinessAppBar(string title)
        {
            var appBar = new BeepMaterial3AppBar
            {
                Title = title,
                Variant = Material3AppBarVariant.Small,
                NavigationIconPath = AppBarIcons.Menu,
                ShowDivider = true
            };

            // Common business actions
            appBar.SetActionButton(1, AppBarIcons.Search); 
            appBar.SetActionButton(2, AppBarIcons.Notifications); 

            // Add common overflow menu items
            appBar.AddOverflowMenuItem("Settings", AppBarIcons.Settings);
            appBar.AddOverflowMenuItem("Help", AppBarIcons.Help);

            return appBar;
        }

        /// <summary>
        /// Creates a minimal app bar for dialogs or secondary windows
        /// </summary>
        public static BeepMaterial3AppBar CreateDialogAppBar(string title)
        {
            var appBar = new BeepMaterial3AppBar
            {
                Title = title,
                Variant = Material3AppBarVariant.CenterAligned,
                NavigationIconPath = AppBarIcons.Close
            };

            return appBar;
        }
    }

    /// <summary>
    /// Common icon paths used in app bars
    /// </summary>
    public static class AppBarIcons
    {
        public const string Menu = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.menu.svg";
        public const string Back = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.005-back arrow.svg";
        public const string Close = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg";
        public const string Search = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg";
        public const string Notifications = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.093-waving.svg";
        public const string Profile = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.025-user.svg";
        public const string Settings = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.settings.svg";
        public const string Help = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.help.svg";
        public const string More = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.016-more-vertical.svg";
    }
}