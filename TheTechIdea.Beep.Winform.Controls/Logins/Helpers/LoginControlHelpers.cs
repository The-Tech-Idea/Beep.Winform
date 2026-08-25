using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Helpers
{
    /// <summary>
    /// Helper class for configuring and styling BeepTextBox and BeepButton controls in login forms
    /// </summary>
    public static class LoginControlHelpers
    {
        /// <summary>
        /// Configures a BeepTextBox for use in login forms
        /// </summary>
        public static void ConfigureLoginTextBox(
            BeepTextBox textBox,
            LoginViewType viewType,
            LoginStyleConfig styleConfig,
            IBeepTheme theme,
            bool useThemeColors,
            string placeholderText = "",
            bool isPassword = false)
        {
            if (textBox == null) return;

            // Basic configuration
            textBox.IsChild = true;
            textBox.IsFrameless = false;
            textBox.IsRounded = true;
            textBox.IsShadowAffectedByTheme = false;
            textBox.IsBorderAffectedByTheme = false;
            textBox.Anchor = AnchorStyles.None;

            // Placeholder text
            if (!string.IsNullOrEmpty(placeholderText))
            {
                textBox.PlaceholderText = placeholderText;
            }

            // Password field
            if (isPassword)
            {
                textBox.PasswordChar = '*';
            }

            // Font
            textBox.Font = LoginFontHelpers.GetInputFont(viewType);

            // Styling
            textBox.ModernGradientType = styleConfig.GradientType;
            textBox.UseGradientBackground = true;

            // Theme colors
            if (useThemeColors && theme != null)
            {
                textBox.ForeColor = LoginThemeHelpers.GetLoginTextColor(theme, useThemeColors);
                textBox.BackColor = LoginThemeHelpers.GetInputBackgroundColor(theme, useThemeColors);
                textBox.BorderColor = LoginThemeHelpers.GetInputBorderColor(theme, useThemeColors);
            }
            else
            {
                textBox.ForeColor = styleConfig.ForegroundColor;
                textBox.BackColor = styleConfig.InputBackgroundColor;
                textBox.BorderColor = styleConfig.InputBorderColor;
            }
        }

        /// <summary>
        /// Configures a BeepButton for use in login forms
        /// </summary>
        public static void ConfigureLoginButton(
            BeepButton button,
            LoginViewType viewType,
            LoginStyleConfig styleConfig,
            IBeepTheme theme,
            bool useThemeColors,
            string buttonText = "",
            string buttonType = "login")
        {
            if (button == null) return;

            // Basic configuration
            button.IsRounded = true;
            button.IsBorderAffectedByTheme = false;
            button.Anchor = AnchorStyles.None;

            // Text
            if (!string.IsNullOrEmpty(buttonText))
            {
                button.Text = buttonText;
            }

            // Font
            button.Font = LoginFontHelpers.GetButtonFont(viewType);

            // Styling
            button.ModernGradientType = styleConfig.GradientType;
            button.UseGradientBackground = true;

            // Theme colors
            var (bgColor, textColor) = LoginThemeHelpers.GetButtonColors(buttonType, theme, useThemeColors);
            button.BackColor = bgColor;
            button.ForeColor = textColor;

            // Gradient colors for buttons
            button.GradientStartColor = bgColor;
            button.GradientEndColor = ColorUtils.GetDarkerColor(bgColor, 0.8f);

            // Hover colors
            button.HoverBackColor = ColorUtils.GetLighterColor(bgColor, 0.1f);
            button.HoverForeColor = textColor;
        }

        /// <summary>
        /// Configures a BeepCircularButton for use as logo or avatar
        /// </summary>
        public static void ConfigureCircularButton(
            BeepCircularButton button,
            LoginViewType viewType,
            LoginStyleConfig styleConfig,
            IBeepTheme theme,
            bool useThemeColors,
            string imagePath = "")
        {
            if (button == null) return;

            // Basic configuration
            button.AutoSize = false;
            button.IsFrameless = true;
            button.IsChild = true;
            button.Anchor = AnchorStyles.None;

            // Image path
            if (!string.IsNullOrEmpty(imagePath))
            {
                button.ImagePath = imagePath;
            }

            // Styling
            button.ModernGradientType = ModernGradientType.Radial;
            button.UseGradientBackground = true;

            // Theme colors
            if (useThemeColors && theme != null)
            {
                var color = BeepStyling.GetThemeColor("LoginLogoBackground");
                button.BackColor = color != Color.Empty ? color : theme.SurfaceColor;
            }
            else
            {
                button.BackColor = styleConfig.BackgroundColor;
            }
        }

        /// <summary>
        /// Configures a BeepLabel for use in login forms
        /// </summary>
        public static void ConfigureLoginLabel(
            Control label,
            LoginViewType viewType,
            LoginStyleConfig styleConfig,
            IBeepTheme theme,
            bool useThemeColors,
            bool isTitle = false)
        {
            if (label == null) return;

            // Font
            if (isTitle)
            {
                label.Font = LoginFontHelpers.GetTitleFont(viewType);
            }
            else
            {
                label.Font = LoginFontHelpers.GetSubtitleFont(viewType);
            }

            // Theme colors
            if (useThemeColors && theme != null)
            {
                if (isTitle)
                {
                    label.ForeColor = LoginThemeHelpers.GetTitleColor(theme, useThemeColors);
                }
                else
                {
                    label.ForeColor = LoginThemeHelpers.GetSubtitleColor(theme, useThemeColors);
                }
            }
            else
            {
                if (isTitle)
                {
                    label.ForeColor = styleConfig.TitleColor;
                }
                else
                {
                    label.ForeColor = styleConfig.SubtitleColor;
                }
            }
        }

        /// <summary>
        /// Configures a LinkLabel for use in login forms
        /// </summary>
        public static void ConfigureLoginLink(
            LinkLabel link,
            LoginViewType viewType,
            LoginStyleConfig styleConfig,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (link == null) return;

            // Basic configuration
            link.AutoSize = true;
            link.Anchor = AnchorStyles.None;

            // Font
            link.Font = LoginFontHelpers.GetLinkFont(viewType);

            // Theme colors
            if (useThemeColors && theme != null)
            {
                link.LinkColor = LoginThemeHelpers.GetLinkColor(theme, useThemeColors);
            }
            else
            {
                link.LinkColor = styleConfig.LinkColor;
            }
        }
    }
}

