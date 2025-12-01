using TheTechIdea.Beep.Winform.Controls.Logins.Models;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Factory for creating login painters based on view type
    /// </summary>
    public static class LoginPainterFactory
    {
        /// <summary>
        /// Creates a painter instance for the specified view type
        /// </summary>
        /// <param name="viewType">The login view type</param>
        /// <returns>An ILoginPainter implementation for the view type</returns>
        public static ILoginPainter CreatePainter(LoginViewType viewType)
        {
            return viewType switch
            {
                LoginViewType.Simple => new SimpleLoginPainter(),
                LoginViewType.Compact => new CompactLoginPainter(),
                LoginViewType.Minimal => new MinimalLoginPainter(),
                LoginViewType.Social => new SocialLoginPainter(),
                LoginViewType.SocialView2 => new SocialView2LoginPainter(),
                LoginViewType.Modern => new ModernLoginPainter(),
                LoginViewType.Avatar => new AvatarLoginPainter(),
                LoginViewType.Extended => new ExtendedLoginPainter(),
                LoginViewType.Full => new FullLoginPainter(),
                _ => new SimpleLoginPainter() // Default fallback
            };
        }

        /// <summary>
        /// Gets the recommended style config for a view type
        /// </summary>
        /// <param name="viewType">The login view type</param>
        /// <returns>Recommended style configuration</returns>
        public static LoginStyleConfig GetRecommendedStyleConfig(LoginViewType viewType)
        {
            return LoginStyleConfig.CreateForViewType(viewType);
        }

        /// <summary>
        /// Gets the view configuration for a view type
        /// </summary>
        /// <param name="viewType">The login view type</param>
        /// <returns>View configuration</returns>
        public static LoginViewConfig GetViewConfig(LoginViewType viewType)
        {
            return LoginViewConfig.CreateForViewType(viewType);
        }
    }
}

