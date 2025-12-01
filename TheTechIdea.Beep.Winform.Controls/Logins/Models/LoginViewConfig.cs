using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Models
{
    /// <summary>
    /// Configuration for a specific login view type
    /// </summary>
    public class LoginViewConfig
    {
        /// <summary>
        /// The view type
        /// </summary>
        public LoginViewType ViewType { get; set; } = LoginViewType.Simple;

        /// <summary>
        /// Preferred size for this view type
        /// </summary>
        public Size PreferredSize { get; set; } = new Size(300, 300);

        /// <summary>
        /// Padding for the container
        /// </summary>
        public Padding Padding { get; set; } = new Padding(10);

        /// <summary>
        /// Whether to show the title
        /// </summary>
        public bool ShowTitle { get; set; } = true;

        /// <summary>
        /// Whether to show the subtitle
        /// </summary>
        public bool ShowSubtitle { get; set; } = false;

        /// <summary>
        /// Whether to show the avatar
        /// </summary>
        public bool ShowAvatar { get; set; } = false;

        /// <summary>
        /// Whether to show the logo
        /// </summary>
        public bool ShowLogo { get; set; } = false;

        /// <summary>
        /// Whether to show social login buttons
        /// </summary>
        public bool ShowSocialButtons { get; set; } = false;

        /// <summary>
        /// Whether to show remember me checkbox
        /// </summary>
        public bool ShowRememberMe { get; set; } = false;

        /// <summary>
        /// Whether to show forgot password link
        /// </summary>
        public bool ShowForgotPassword { get; set; } = false;

        /// <summary>
        /// Whether to show register link
        /// </summary>
        public bool ShowRegisterLink { get; set; } = false;

        /// <summary>
        /// Creates a default config for a view type
        /// </summary>
        public static LoginViewConfig CreateForViewType(LoginViewType viewType)
        {
            var config = new LoginViewConfig { ViewType = viewType };

            switch (viewType)
            {
                case LoginViewType.Simple:
                    config.PreferredSize = new Size(300, 300);
                    config.ShowTitle = true;
                    config.ShowAvatar = true;
                    config.ShowRememberMe = true;
                    config.ShowForgotPassword = true;
                    break;

                case LoginViewType.Compact:
                    config.PreferredSize = new Size(500, 200);
                    config.ShowTitle = true;
                    config.ShowForgotPassword = true;
                    config.ShowRegisterLink = true;
                    break;

                case LoginViewType.Minimal:
                    config.PreferredSize = new Size(300, 280);
                    config.ShowTitle = true;
                    config.ShowAvatar = true;
                    config.ShowRegisterLink = true;
                    break;

                case LoginViewType.Social:
                    config.PreferredSize = new Size(450, 500);
                    config.ShowTitle = true;
                    config.ShowSubtitle = true;
                    config.ShowRememberMe = true;
                    config.ShowForgotPassword = true;
                    config.ShowSocialButtons = true;
                    break;

                case LoginViewType.SocialView2:
                    config.PreferredSize = new Size(350, 450);
                    config.ShowTitle = true;
                    config.ShowSubtitle = true;
                    config.ShowForgotPassword = true;
                    config.ShowSocialButtons = true;
                    config.ShowRegisterLink = true;
                    break;

                case LoginViewType.Modern:
                    config.PreferredSize = new Size(400, 350);
                    config.ShowTitle = true;
                    config.ShowSubtitle = true;
                    config.ShowRememberMe = true;
                    config.ShowForgotPassword = true;
                    break;

                case LoginViewType.Avatar:
                    config.PreferredSize = new Size(300, 330);
                    config.ShowAvatar = true;
                    config.ShowTitle = true;
                    config.ShowForgotPassword = true;
                    config.ShowRegisterLink = true;
                    break;

                case LoginViewType.Extended:
                    config.PreferredSize = new Size(500, 250);
                    config.ShowTitle = true;
                    config.ShowSubtitle = true;
                    config.ShowRememberMe = true;
                    config.ShowForgotPassword = true;
                    config.ShowRegisterLink = true;
                    break;

                case LoginViewType.Full:
                    config.PreferredSize = new Size(500, 360);
                    config.ShowLogo = true;
                    config.ShowAvatar = true;
                    config.ShowTitle = true;
                    config.ShowSubtitle = true;
                    config.ShowRememberMe = true;
                    config.ShowForgotPassword = true;
                    config.ShowRegisterLink = true;
                    break;
            }

            return config;
        }
    }
}

