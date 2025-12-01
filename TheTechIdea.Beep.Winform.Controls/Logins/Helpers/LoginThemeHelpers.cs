using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Helpers
{
    /// <summary>
    /// Helper class for handling theme-specific customizations for login controls
    /// </summary>
    public static class LoginThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the login control
        /// </summary>
        public static Color GetLoginBackgroundColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var color = BeepStyling.GetThemeColor("LoginPopoverBackground");
                return color != Color.Empty ? color : theme.BackgroundColor;
            }
            return Color.White;
        }

        /// <summary>
        /// Gets the text color for the login control
        /// </summary>
        public static Color GetLoginTextColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return theme.ForeColor;
            }
            return Color.Black;
        }

        /// <summary>
        /// Gets the input field background color
        /// </summary>
        public static Color GetInputBackgroundColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var color = BeepStyling.GetThemeColor("TextBoxBack");
                return color != Color.Empty ? color : theme.SurfaceColor;
            }
            return Color.White;
        }

        /// <summary>
        /// Gets the input field border color
        /// </summary>
        public static Color GetInputBorderColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var color = BeepStyling.GetThemeColor("TextBoxBorder");
                return color != Color.Empty ? color : theme.BorderColor;
            }
            return Color.LightGray;
        }

        /// <summary>
        /// Gets button colors based on button type
        /// </summary>
        public static (Color background, Color text) GetButtonColors(string buttonType, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return buttonType.ToLowerInvariant() switch
                {
                    "google" => (Color.FromArgb(219, 68, 55), Color.White),
                    "facebook" => (Color.FromArgb(66, 103, 178), Color.White),
                    "twitter" => (Color.FromArgb(29, 161, 242), Color.White),
                    "login" => (
                        BeepStyling.GetThemeColor("LoginButton") != Color.Empty 
                            ? BeepStyling.GetThemeColor("LoginButton") 
                            : theme.PrimaryColor,
                        theme.ForeColor),
                    _ => (theme.PrimaryColor, theme.ForeColor)
                };
            }

            return buttonType.ToLowerInvariant() switch
            {
                "google" => (Color.FromArgb(219, 68, 55), Color.White),
                "facebook" => (Color.FromArgb(66, 103, 178), Color.White),
                "twitter" => (Color.FromArgb(29, 161, 242), Color.White),
                _ => (Color.FromArgb(0, 120, 215), Color.White)
            };
        }

        /// <summary>
        /// Gets the link color
        /// </summary>
        public static Color GetLinkColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var color = BeepStyling.GetThemeColor("LoginLink");
                return color != Color.Empty ? color : theme.AccentColor;
            }
            return Color.FromArgb(0, 120, 215);
        }

        /// <summary>
        /// Gets the title color
        /// </summary>
        public static Color GetTitleColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var color = BeepStyling.GetThemeColor("LoginTitle");
                return color != Color.Empty ? color : theme.ForeColor;
            }
            return Color.Black;
        }

        /// <summary>
        /// Gets the subtitle color
        /// </summary>
        public static Color GetSubtitleColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var color = BeepStyling.GetThemeColor("LoginSubtitle");
                return color != Color.Empty ? color : Color.Gray;
            }
            return Color.Gray;
        }

        /// <summary>
        /// Applies theme colors to all child controls (to be called by painters)
        /// </summary>
        public static void ApplyThemeToControls(LoginStyleConfig styleConfig, IBeepTheme theme, bool useThemeColors)
        {
            // This will be used by painters to apply theme colors to child controls
            // Implementation details will be in the painters
        }
    }
}

