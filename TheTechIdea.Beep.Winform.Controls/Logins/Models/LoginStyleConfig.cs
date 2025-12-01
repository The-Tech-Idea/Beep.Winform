using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Models
{
    /// <summary>
    /// Configuration for login control styling
    /// </summary>
    public class LoginStyleConfig
    {
        /// <summary>
        /// The control style to use
        /// </summary>
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Modern;

        /// <summary>
        /// The gradient type for modern styling
        /// </summary>
        public ModernGradientType GradientType { get; set; } = ModernGradientType.Subtle;

        /// <summary>
        /// Border radius in pixels
        /// </summary>
        public int BorderRadius { get; set; } = 12;

        /// <summary>
        /// Whether to use glassmorphism effect
        /// </summary>
        public bool UseGlassmorphism { get; set; } = false;

        /// <summary>
        /// Glassmorphism opacity (0.0 to 1.0)
        /// </summary>
        public float GlassmorphismOpacity { get; set; } = 0.1f;

        /// <summary>
        /// Whether to show shadow
        /// </summary>
        public bool ShowShadow { get; set; } = false;

        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;

        /// <summary>
        /// Foreground/text color
        /// </summary>
        public Color ForegroundColor { get; set; } = Color.Black;

        /// <summary>
        /// Title color
        /// </summary>
        public Color TitleColor { get; set; } = Color.Black;

        /// <summary>
        /// Subtitle color
        /// </summary>
        public Color SubtitleColor { get; set; } = Color.Gray;

        /// <summary>
        /// Input field background color
        /// </summary>
        public Color InputBackgroundColor { get; set; } = Color.White;

        /// <summary>
        /// Input field border color
        /// </summary>
        public Color InputBorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// Button background color
        /// </summary>
        public Color ButtonBackgroundColor { get; set; } = Color.FromArgb(0, 120, 215);

        /// <summary>
        /// Button text color
        /// </summary>
        public Color ButtonTextColor { get; set; } = Color.White;

        /// <summary>
        /// Link color
        /// </summary>
        public Color LinkColor { get; set; } = Color.FromArgb(0, 120, 215);

        /// <summary>
        /// Creates a default style config
        /// </summary>
        public static LoginStyleConfig CreateDefault()
        {
            return new LoginStyleConfig();
        }

        /// <summary>
        /// Creates a style config for a specific view type
        /// </summary>
        public static LoginStyleConfig CreateForViewType(LoginViewType viewType)
        {
            var config = new LoginStyleConfig();

            switch (viewType)
            {
                case LoginViewType.Simple:
                case LoginViewType.Compact:
                    config.GradientType = ModernGradientType.Subtle;
                    config.BorderRadius = 8;
                    break;

                case LoginViewType.Modern:
                    config.GradientType = ModernGradientType.Linear;
                    config.BorderRadius = 16;
                    config.UseGlassmorphism = true;
                    config.GlassmorphismOpacity = 0.1f;
                    break;

                case LoginViewType.Social:
                case LoginViewType.SocialView2:
                    config.GradientType = ModernGradientType.Mesh;
                    config.BorderRadius = 12;
                    break;

                case LoginViewType.Avatar:
                    config.GradientType = ModernGradientType.Radial;
                    config.BorderRadius = 20;
                    break;

                case LoginViewType.Extended:
                case LoginViewType.Full:
                    config.GradientType = ModernGradientType.Conic;
                    config.BorderRadius = 10;
                    break;

                case LoginViewType.Minimal:
                default:
                    config.GradientType = ModernGradientType.Subtle;
                    config.BorderRadius = 8;
                    break;
            }

            return config;
        }
    }
}

