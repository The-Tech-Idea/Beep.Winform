using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Logins.Helpers;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Painter for Extended login view type
    /// Title; Subtitle; Username; Password; Row with Login, Google, Facebook
    /// </summary>
    public class ExtendedLoginPainter : BaseLoginPainter
    {
        public override void Paint(
            Graphics g,
            GraphicsPath path,
            LoginViewType viewType,
            BeepControlStyle controlStyle,
            IBeepTheme theme,
            bool useThemeColors,
            LoginLayoutMetrics metrics,
            LoginStyleConfig styleConfig)
        {
            if (g == null || path == null) return;
            BeepStyling.PaintControl(g, path, controlStyle, theme, useThemeColors, ControlState.Normal);
        }

        public override LoginLayoutMetrics CalculateMetrics(
            Rectangle bounds,
            LoginViewType viewType,
            LoginStyleConfig styleConfig)
        {
            var padding = new Padding(10);
            var metrics = InitializeMetrics(bounds, padding);
            metrics.Margin = LoginLayoutHelpers.GetMargin(viewType);
            metrics.Spacing = LoginLayoutHelpers.GetSpacing(viewType);

            int margin = metrics.Margin;
            int currentY = metrics.ContainerPadding.Top + margin;
            int containerWidth = metrics.ContainerWidth;

            // 1) Title (left) and Social Icons (right) on same row
            var titleSize = new Size(100, 30);
            metrics.SetControlBounds("Title", new Rectangle(metrics.ContainerPadding.Left, currentY, titleSize.Width, titleSize.Height));
            
            int iconSize = 30;
            metrics.SetControlBounds("FacebookButton", new Rectangle(metrics.ContainerPadding.Left + containerWidth - (iconSize * 2) - 5, currentY, iconSize, iconSize));
            metrics.SetControlBounds("TwitterButton", new Rectangle(metrics.ContainerPadding.Left + containerWidth - iconSize, currentY, iconSize, iconSize));
            currentY += Math.Max(titleSize.Height, iconSize) + margin;

            // 2) Subtitle - left aligned, wraps
            var subtitleSize = new Size(containerWidth, 0); // Auto-height
            metrics.SetControlBounds("Subtitle", new Rectangle(metrics.ContainerPadding.Left, currentY, subtitleSize.Width, 30));
            currentY += 30 + margin;

            // 3) Username - centered, 300x30
            var usernameSize = new Size(300, 30);
            var usernamePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, usernameSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Username", new Rectangle(usernamePos.X, usernamePos.Y, usernameSize.Width, usernameSize.Height));
            currentY += usernameSize.Height + margin;

            // 4) Password - centered, 300x30
            var passwordSize = new Size(300, 30);
            var passwordPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, passwordSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Password", new Rectangle(passwordPos.X, passwordPos.Y, passwordSize.Width, passwordSize.Height));
            currentY += passwordSize.Height + margin;

            // 5) Remember Me (left) and Login Button (right) on same row
            var checkboxSize = new Size(120, 20);
            metrics.SetControlBounds("RememberMe", new Rectangle(metrics.ContainerPadding.Left, currentY + 2, checkboxSize.Width, checkboxSize.Height));
            
            var loginSize = new Size(80, 30);
            var loginPos = LoginLayoutHelpers.AlignRight(containerWidth, loginSize.Width, currentY - 5, metrics.ContainerPadding, 0);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPos.X, loginPos.Y, loginSize.Width, loginSize.Height));
            currentY += Math.Max(checkboxSize.Height, loginSize.Height) + margin;

            // 6) Sign Up label - centered
            var signUpSize = new Size(200, 20);
            var signUpPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, signUpSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("SignUpLabel", new Rectangle(signUpPos.X, signUpPos.Y, signUpSize.Width, signUpSize.Height));
            currentY += signUpSize.Height + margin;

            // 7) Forgot Password - centered
            var linkSize = new Size(120, 20);
            var linkPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, linkSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("ForgotPassword", new Rectangle(linkPos.X, linkPos.Y, linkSize.Width, linkSize.Height));

            return metrics;
        }
    }
}

