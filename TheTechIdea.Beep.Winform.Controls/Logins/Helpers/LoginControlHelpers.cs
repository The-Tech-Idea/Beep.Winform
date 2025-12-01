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
            button.GradientEndColor = DarkenColor(bgColor, 0.8f);

            // Hover colors
            button.HoverBackColor = LightenColor(bgColor, 1.1f);
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

        /// <summary>
        /// Applies view type styling to all login controls
        /// </summary>
        public static void ApplyViewTypeToControls(
            BeepTextBox txtUsername,
            BeepTextBox txtPassword,
            BeepButton btnLogin,
            BeepButton btnGoogleLogin,
            BeepButton btnFacebookLogin,
            BeepButton btnTwitterLogin,
            BeepCircularButton btnLogo,
            BeepCircularButton btnAvatar,
            Control lblTitle,
            Control lblSubtitle,
            LinkLabel lnkForgotPassword,
            LinkLabel lnkRegister,
            LoginViewType viewType,
            LoginStyleConfig styleConfig,
            IBeepTheme theme,
            bool useThemeColors)
        {
            // Configure text boxes
            if (txtUsername != null)
            {
                ConfigureLoginTextBox(txtUsername, viewType, styleConfig, theme, useThemeColors, "Username or Email", false);
            }

            if (txtPassword != null)
            {
                ConfigureLoginTextBox(txtPassword, viewType, styleConfig, theme, useThemeColors, "Password", true);
            }

            // Configure buttons
            if (btnLogin != null)
            {
                ConfigureLoginButton(btnLogin, viewType, styleConfig, theme, useThemeColors, "Login", "login");
            }

            if (btnGoogleLogin != null)
            {
                ConfigureLoginButton(btnGoogleLogin, viewType, styleConfig, theme, useThemeColors, "Google", "google");
            }

            if (btnFacebookLogin != null)
            {
                ConfigureLoginButton(btnFacebookLogin, viewType, styleConfig, theme, useThemeColors, "Facebook", "facebook");
            }

            if (btnTwitterLogin != null)
            {
                ConfigureLoginButton(btnTwitterLogin, viewType, styleConfig, theme, useThemeColors, "Twitter", "twitter");
            }

            // Configure circular buttons
            if (btnLogo != null)
            {
                ConfigureCircularButton(btnLogo, viewType, styleConfig, theme, useThemeColors);
            }

            if (btnAvatar != null)
            {
                ConfigureCircularButton(btnAvatar, viewType, styleConfig, theme, useThemeColors);
            }

            // Configure labels
            if (lblTitle != null)
            {
                ConfigureLoginLabel(lblTitle, viewType, styleConfig, theme, useThemeColors, true);
            }

            if (lblSubtitle != null)
            {
                ConfigureLoginLabel(lblSubtitle, viewType, styleConfig, theme, useThemeColors, false);
            }

            // Configure links
            if (lnkForgotPassword != null)
            {
                ConfigureLoginLink(lnkForgotPassword, viewType, styleConfig, theme, useThemeColors);
            }

            if (lnkRegister != null)
            {
                ConfigureLoginLink(lnkRegister, viewType, styleConfig, theme, useThemeColors);
            }
        }

        /// <summary>
        /// Helper to darken a color
        /// </summary>
        private static Color DarkenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R * factor),
                (int)(color.G * factor),
                (int)(color.B * factor));
        }

        /// <summary>
        /// Helper to lighten a color
        /// </summary>
        private static Color LightenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R * factor)),
                Math.Min(255, (int)(color.G * factor)),
                Math.Min(255, (int)(color.B * factor)));
        }
    }
}

