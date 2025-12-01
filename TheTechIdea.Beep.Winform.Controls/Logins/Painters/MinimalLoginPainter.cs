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
    /// Painter for Minimal login view type
    /// Row with Avatar (left) & Title (to right), then Username, Password, Register
    /// </summary>
    public class MinimalLoginPainter : BaseLoginPainter
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

            // 1) Avatar and Title side-by-side
            var avatarSize = new Size(60, 80);
            metrics.SetControlBounds("Avatar", new Rectangle(metrics.ContainerPadding.Left + margin, currentY, avatarSize.Width, avatarSize.Height));
            
            var titleSize = new Size(150, 30);
            metrics.SetControlBounds("Title", new Rectangle(metrics.ContainerPadding.Left + margin + avatarSize.Width + margin, currentY + (avatarSize.Height - titleSize.Height) / 2, titleSize.Width, titleSize.Height));
            currentY += avatarSize.Height + margin;

            // 2) Username - centered, 250x30
            var usernameSize = LoginLayoutHelpers.GetControlSize("username", viewType);
            var usernamePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, usernameSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Username", new Rectangle(usernamePos.X, usernamePos.Y, usernameSize.Width, usernameSize.Height));
            currentY += usernameSize.Height + margin;

            // 3) Password - centered, 250x30
            var passwordSize = LoginLayoutHelpers.GetControlSize("password", viewType);
            var passwordPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, passwordSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Password", new Rectangle(passwordPos.X, passwordPos.Y, passwordSize.Width, passwordSize.Height));
            currentY += passwordSize.Height + margin;

            // 4) Login button - centered
            var loginSize = LoginLayoutHelpers.GetControlSize("loginbutton", viewType);
            var loginPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, loginSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPos.X, loginPos.Y, loginSize.Width, loginSize.Height));
            currentY += loginSize.Height + margin;

            // 5) Register Link - centered
            var registerSize = new Size(200, 20);
            var registerPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, registerSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("RegisterLink", new Rectangle(registerPos.X, registerPos.Y, registerSize.Width, registerSize.Height));

            return metrics;
        }
    }
}

