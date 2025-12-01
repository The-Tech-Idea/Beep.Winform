using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in login controls
    /// </summary>
    public static class LoginFontHelpers
    {
        /// <summary>
        /// Gets the font for the title based on view type
        /// </summary>
        public static Font GetTitleFont(LoginViewType viewType)
        {
            float size = viewType switch
            {
                LoginViewType.Modern => 18f,
                LoginViewType.Social => 20f,
                LoginViewType.Full => 20f,
                _ => 16f
            };

            return BeepFontManager.GetFont(
                BeepFontManager.DefaultFontName,
                size,
                FontStyle.Bold);
        }

        /// <summary>
        /// Gets the font for the subtitle
        /// </summary>
        public static Font GetSubtitleFont(LoginViewType viewType)
        {
            float size = viewType switch
            {
                LoginViewType.Modern => 11f,
                LoginViewType.Social => 12f,
                _ => 12f
            };

            return BeepFontManager.GetFont(
                BeepFontManager.DefaultFontName,
                size,
                FontStyle.Regular);
        }

        /// <summary>
        /// Gets the font for input fields
        /// </summary>
        public static Font GetInputFont(LoginViewType viewType)
        {
            float size = viewType switch
            {
                LoginViewType.Compact => 11f,
                _ => 12f
            };

            return BeepFontManager.GetFont(
                BeepFontManager.DefaultFontName,
                size,
                FontStyle.Regular);
        }

        /// <summary>
        /// Gets the font for buttons
        /// </summary>
        public static Font GetButtonFont(LoginViewType viewType)
        {
            float size = viewType switch
            {
                LoginViewType.SocialView2 => 12f,
                LoginViewType.Social => 11f,
                _ => 12f
            };

            return BeepFontManager.GetFont(
                BeepFontManager.DefaultFontName,
                size,
                FontStyle.Bold);
        }

        /// <summary>
        /// Gets the font for links
        /// </summary>
        public static Font GetLinkFont(LoginViewType viewType)
        {
            return BeepFontManager.GetFont(
                BeepFontManager.DefaultFontName,
                11f,
                FontStyle.Regular);
        }

        /// <summary>
        /// Gets the font for checkbox labels
        /// </summary>
        public static Font GetCheckboxFont(LoginViewType viewType)
        {
            return BeepFontManager.GetFont(
                BeepFontManager.DefaultFontName,
                11f,
                FontStyle.Regular);
        }

        /// <summary>
        /// Applies font theme to all controls (to be called by painters)
        /// </summary>
        public static void ApplyFontTheme(LoginViewType viewType)
        {
            // This will be used by painters to apply fonts to child controls
            // Implementation details will be in the painters
        }
    }
}

