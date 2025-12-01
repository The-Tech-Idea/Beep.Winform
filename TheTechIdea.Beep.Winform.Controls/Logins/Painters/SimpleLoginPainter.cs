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
    /// Painter for Simple login view type
    /// Vertically stacked: Title, Avatar, Username, Password, [Remember, Forgot], Login
    /// </summary>
    public class SimpleLoginPainter : BaseLoginPainter
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

            // Paint background and border using BeepStyling
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

            // 1) Title - centered
            var titleSize = new Size(0, 0); // Auto-size
            var titlePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, 200, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Title", new Rectangle(titlePos.X, titlePos.Y, 200, 30));
            currentY += 30 + margin;

            // 2) Avatar - centered, 60x60
            var avatarSize = LoginIconHelpers.GetIconSize("avatar");
            var avatarPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, avatarSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Avatar", new Rectangle(avatarPos.X, avatarPos.Y, avatarSize.Width, avatarSize.Height));
            currentY += avatarSize.Height + margin;

            // 3) Username - centered, 250x30
            var usernameSize = LoginLayoutHelpers.GetControlSize("username", viewType);
            var usernamePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, usernameSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Username", new Rectangle(usernamePos.X, usernamePos.Y, usernameSize.Width, usernameSize.Height));
            currentY += usernameSize.Height + margin;

            // 4) Password - centered, 250x30
            var passwordSize = LoginLayoutHelpers.GetControlSize("password", viewType);
            var passwordPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, passwordSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Password", new Rectangle(passwordPos.X, passwordPos.Y, passwordSize.Width, passwordSize.Height));
            currentY += passwordSize.Height + margin;

            // 5) Remember Me (left) and Forgot Password (right) on same line
            int leftEdge = metrics.ContainerPadding.Left + (containerWidth - usernameSize.Width) / 2;
            var checkboxSize = new Size(120, 20); // Approximate
            metrics.SetControlBounds("RememberMe", new Rectangle(leftEdge, currentY, checkboxSize.Width, checkboxSize.Height));
            
            var linkSize = new Size(120, 20); // Approximate
            var linkPos = LoginLayoutHelpers.AlignRight(containerWidth, linkSize.Width, currentY, metrics.ContainerPadding, margin);
            metrics.SetControlBounds("ForgotPassword", new Rectangle(linkPos.X, linkPos.Y, linkSize.Width, linkSize.Height));
            
            currentY += Math.Max(checkboxSize.Height, linkSize.Height) + margin;

            // 6) Login button - centered, 100x30
            var loginSize = LoginLayoutHelpers.GetControlSize("loginbutton", viewType);
            var loginPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, loginSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPos.X, loginPos.Y, loginSize.Width, loginSize.Height));

            return metrics;
        }
    }
}

