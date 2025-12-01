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
    /// Painter for Compact login view type
    /// Title on top; side-by-side Username & Password; then Login; then Forgot; then Register
    /// </summary>
    public class CompactLoginPainter : BaseLoginPainter
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
            var padding = new Padding(15);
            var metrics = InitializeMetrics(bounds, padding);
            metrics.Margin = LoginLayoutHelpers.GetMargin(viewType);
            metrics.Spacing = LoginLayoutHelpers.GetSpacing(viewType);

            int margin = metrics.Margin;
            int currentY = metrics.ContainerPadding.Top + margin;
            int containerWidth = metrics.ContainerWidth;

            // 1) Title - centered
            var titleSize = new Size(100, 30);
            var titlePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, titleSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Title", new Rectangle(titlePos.X, titlePos.Y, titleSize.Width, titleSize.Height));
            currentY += titleSize.Height + margin;

            // 2) Username and Password side-by-side
            var inputSize = new Size(200, 30);
            var (usernamePos, passwordPos) = LoginLayoutHelpers.PlaceSideBySide(containerWidth, inputSize, inputSize, currentY, metrics.ContainerPadding, margin);
            metrics.SetControlBounds("Username", new Rectangle(usernamePos.X, usernamePos.Y, inputSize.Width, inputSize.Height));
            metrics.SetControlBounds("Password", new Rectangle(passwordPos.X, passwordPos.Y, inputSize.Width, inputSize.Height));
            currentY += inputSize.Height + margin;

            // 3) Login button - centered
            var loginSize = LoginLayoutHelpers.GetControlSize("loginbutton", viewType);
            var loginPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, loginSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPos.X, loginPos.Y, loginSize.Width, loginSize.Height));
            currentY += loginSize.Height + margin;

            // 4) Forgot Password - centered
            var linkSize = new Size(120, 20);
            var linkPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, linkSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("ForgotPassword", new Rectangle(linkPos.X, linkPos.Y, linkSize.Width, linkSize.Height));
            currentY += linkSize.Height + margin;

            // 5) Register Link - centered
            var registerSize = new Size(100, 20);
            var registerPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, registerSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("RegisterLink", new Rectangle(registerPos.X, registerPos.Y, registerSize.Width, registerSize.Height));

            return metrics;
        }
    }
}

