using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Helpers
{
    /// <summary>
    /// Helper class for login styling and theme application
    /// </summary>
    public static class LoginStyleHelpers
    {
        /// <summary>
        /// Gets the recommended BeepControlStyle for a view type
        /// </summary>
        public static BeepControlStyle GetViewTypeStyle(LoginViewType viewType)
        {
            return viewType switch
            {
                LoginViewType.Modern => BeepControlStyle.Modern,
                LoginViewType.Social => BeepControlStyle.GradientModern,
                LoginViewType.SocialView2 => BeepControlStyle.GradientModern,
                LoginViewType.Avatar => BeepControlStyle.Material3,
                LoginViewType.Extended => BeepControlStyle.FigmaCard,
                LoginViewType.Full => BeepControlStyle.TailwindCard,
                _ => BeepControlStyle.Minimal
            };
        }

        /// <summary>
        /// Gets the recommended gradient type for a view type
        /// </summary>
        public static ModernGradientType GetGradientTypeForView(LoginViewType viewType)
        {
            return viewType switch
            {
                LoginViewType.Simple or LoginViewType.Compact => ModernGradientType.Subtle,
                LoginViewType.Modern => ModernGradientType.Linear,
                LoginViewType.Social or LoginViewType.SocialView2 => ModernGradientType.Mesh,
                LoginViewType.Avatar => ModernGradientType.Radial,
                LoginViewType.Extended or LoginViewType.Full => ModernGradientType.Conic,
                _ => ModernGradientType.Subtle
            };
        }

        /// <summary>
        /// Gets the recommended border radius for a view type
        /// </summary>
        public static int GetBorderRadiusForView(LoginViewType viewType)
        {
            return viewType switch
            {
                LoginViewType.Simple or LoginViewType.Compact => 8,
                LoginViewType.Modern => 16,
                LoginViewType.Social or LoginViewType.SocialView2 => 12,
                LoginViewType.Avatar => 20,
                LoginViewType.Extended or LoginViewType.Full => 10,
                _ => 8
            };
        }

        /// <summary>
        /// Creates a style config for a view type
        /// </summary>
        public static LoginStyleConfig GetStyleConfigForView(LoginViewType viewType, IBeepTheme theme = null, bool useThemeColors = true)
        {
            var config = LoginStyleConfig.CreateForViewType(viewType);
            
            if (theme != null && useThemeColors)
            {
                config.BackgroundColor = theme.BackgroundColor;
                config.ForegroundColor = theme.ForeColor;
                config.TitleColor = BeepStyling.GetThemeColor("LoginTitle") != Color.Empty 
                    ? BeepStyling.GetThemeColor("LoginTitle") 
                    : theme.ForeColor;
                config.SubtitleColor = BeepStyling.GetThemeColor("LoginSubtitle") != Color.Empty 
                    ? BeepStyling.GetThemeColor("LoginSubtitle") 
                    : Color.Gray;
                config.InputBackgroundColor = BeepStyling.GetThemeColor("TextBoxBack") != Color.Empty 
                    ? BeepStyling.GetThemeColor("TextBoxBack") 
                    : theme.SurfaceColor;
                config.InputBorderColor = BeepStyling.GetThemeColor("TextBoxBorder") != Color.Empty 
                    ? BeepStyling.GetThemeColor("TextBoxBorder") 
                    : theme.BorderColor;
                config.ButtonBackgroundColor = BeepStyling.GetThemeColor("LoginButton") != Color.Empty 
                    ? BeepStyling.GetThemeColor("LoginButton") 
                    : theme.PrimaryColor;
                config.ButtonTextColor = theme.ForeColor;
                config.LinkColor = BeepStyling.GetThemeColor("LoginLink") != Color.Empty 
                    ? BeepStyling.GetThemeColor("LoginLink") 
                    : theme.AccentColor;
            }

            return config;
        }

        /// <summary>
        /// Applies view type styling to a login control
        /// </summary>
        public static void ApplyViewTypeStyling(BaseControl loginControl, LoginViewType viewType, LoginStyleConfig styleConfig)
        {
            if (loginControl == null) return;

            loginControl.ControlStyle = styleConfig.ControlStyle;
            loginControl.ModernGradientType = styleConfig.GradientType;
            loginControl.BorderRadius = styleConfig.BorderRadius;
            loginControl.UseGlassmorphism = styleConfig.UseGlassmorphism;
            loginControl.GlassmorphismOpacity = styleConfig.GlassmorphismOpacity;
            loginControl.ShowShadow = styleConfig.ShowShadow;
        }

        /// <summary>
        /// Applies styling to child controls based on view type
        /// Uses LoginControlHelpers to configure actual BeepTextBox and BeepButton instances
        /// </summary>
        public static void ApplyChildControlStyling(
            LoginViewType viewType, 
            LoginStyleConfig styleConfig,
            IBeepTheme theme = null,
            bool useThemeColors = true)
        {
            // This will be used by painters to style child controls
            // The actual implementation is in LoginControlHelpers.ApplyViewTypeToControls()
            // which configures BeepTextBox, BeepButton, BeepCircularButton, etc.
        }
    }
}

