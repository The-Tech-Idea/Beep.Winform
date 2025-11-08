using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// Helper class to convert between DialogConfig and BeepControlStyle
    /// Similar to ToolTipStyleAdapter for consistent styling
    /// </summary>
    public static class DialogStyleAdapter
    {
        /// <summary>
        /// Get BeepControlStyle from DialogConfig
        /// Uses current BeepStyling style as fallback
        /// </summary>
        public static BeepControlStyle GetBeepControlStyle(DialogConfig config)
        {
            if (config == null)
                return BeepStyling.CurrentControlStyle;

            return config.Style;
        }

        /// <summary>
        /// Get icon path based on dialog icon type
        /// </summary>
        public static string GetIconPath(DialogConfig config)
        {
            if (config == null)
                return string.Empty;

            // Custom icon path takes precedence
            if (!string.IsNullOrEmpty(config.IconPath))
                return config.IconPath;

            // Map BeepDialogIcon to icon paths
            return config.IconType switch
            {
                BeepDialogIcon.Information => "TheTechIdea.Beep.Winform.Controls.GFX.SVG.information.svg",
                BeepDialogIcon.Warning => "TheTechIdea.Beep.Winform.Controls.GFX.SVG.warning.svg",
                BeepDialogIcon.Error => "TheTechIdea.Beep.Winform.Controls.GFX.SVG.error.svg",
                BeepDialogIcon.Question => "TheTechIdea.Beep.Winform.GFX.SVG.question.svg",
                BeepDialogIcon.Success => "TheTechIdea.Beep.Winform.Controls.GFX.SVG.success.svg",
                BeepDialogIcon.None => string.Empty,
                _ => "TheTechIdea.Beep.Winform.Controls.GFX.SVG.information.svg"
            };
        }

        /// <summary>
        /// Get semantic color for dialog icon based on type
        /// </summary>
        public static Color GetIconColor(DialogConfig config, IBeepTheme theme)
        {
            if (theme == null)
                return GetDefaultIconColor(config.IconType);

            return config.IconType switch
            {
                BeepDialogIcon.Information => theme.AccentColor,
                BeepDialogIcon.Warning => Color.FromArgb(255, 152, 0), // Orange
                BeepDialogIcon.Error => theme.ErrorColor,
                BeepDialogIcon.Question => theme.AccentColor,
                BeepDialogIcon.Success => theme.SuccessColor,
                _ => theme.ForeColor
            };
        }

        /// <summary>
        /// Get default icon color without theme
        /// </summary>
        private static Color GetDefaultIconColor(BeepDialogIcon iconType)
        {
            return iconType switch
            {
                BeepDialogIcon.Information => Color.FromArgb(33, 150, 243), // Blue
                BeepDialogIcon.Warning => Color.FromArgb(255, 152, 0), // Orange
                BeepDialogIcon.Error => Color.FromArgb(244, 67, 54), // Red
                BeepDialogIcon.Question => Color.FromArgb(33, 150, 243), // Blue
                BeepDialogIcon.Success => Color.FromArgb(76, 175, 80), // Green
                _ => Color.Black
            };
        }

        /// <summary>
        /// Get color scheme from DialogConfig and theme
        /// </summary>
        public static DialogColors GetColors(DialogConfig config, IBeepTheme theme)
        {
            var colors = new DialogColors();

            if (theme != null && config.UseBeepThemeColors)
            {
                colors.Background = config.BackColor ?? theme.BackgroundColor;
                colors.Foreground = config.ForeColor ?? theme.ForeColor;
                colors.Border = config.BorderColor ?? theme.BorderColor;
                colors.Accent = theme.AccentColor;
                colors.TitleBackground = theme.AccentColor;
                colors.TitleForeground = Color.White;
                colors.ButtonBackground = theme.ButtonBackColor;
                colors.ButtonForeground = theme.ButtonForeColor;
                colors.ButtonBorder = theme.ButtonBorderColor;
            }
            else
            {
                colors.Background = config.BackColor ?? Color.White;
                colors.Foreground = config.ForeColor ?? Color.Black;
                colors.Border = config.BorderColor ?? Color.FromArgb(189, 189, 189);
                colors.Accent = Color.FromArgb(33, 150, 243);
                colors.TitleBackground = Color.FromArgb(33, 150, 243);
                colors.TitleForeground = Color.White;
                colors.ButtonBackground = Color.FromArgb(240, 240, 240);
                colors.ButtonForeground = Color.Black;
                colors.ButtonBorder = Color.FromArgb(189, 189, 189);
            }

            // Custom shadow color
            if (config.ShadowColor.HasValue)
                colors.Shadow = config.ShadowColor.Value;
            else if (theme != null)
                colors.Shadow = Color.FromArgb(100, 0, 0, 0);
            else
                colors.Shadow = Color.FromArgb(80, 0, 0, 0);

            return colors;
        }

        /// <summary>
        /// Get button text for standard dialog buttons
        /// </summary>
        public static string GetButtonText(Vis.Modules.BeepDialogButtons button)
        {
            return button switch
            {
                Vis.Modules.BeepDialogButtons.Ok => "OK",
                Vis.Modules.BeepDialogButtons.Cancel => "Cancel",
                Vis.Modules.BeepDialogButtons.Yes => "Yes",
                Vis.Modules.BeepDialogButtons.No => "No",
                Vis.Modules.BeepDialogButtons.Abort => "Abort",
                Vis.Modules.BeepDialogButtons.Retry => "Retry",
                Vis.Modules.BeepDialogButtons.Ignore => "Ignore",
                Vis.Modules.BeepDialogButtons.Stop => "Stop",
                Vis.Modules.BeepDialogButtons.Continue => "Continue",
                _ => button.ToString()
            };
        }
    }

    /// <summary>
    /// Color scheme for dialogs
    /// </summary>
    public class DialogColors
    {
        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public Color Border { get; set; }
        public Color Shadow { get; set; }
        public Color Accent { get; set; }
        public Color TitleBackground { get; set; }
        public Color TitleForeground { get; set; }
        public Color ButtonBackground { get; set; }
        public Color ButtonForeground { get; set; }
        public Color ButtonBorder { get; set; }
    }
}
