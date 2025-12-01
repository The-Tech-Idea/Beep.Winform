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
    /// Painter for Modern login view type
    /// Title; Subtitle; Username; Password; Row with Remember & Forgot; Login
    /// </summary>
    public class ModernLoginPainter : BaseLoginPainter
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
            var padding = new Padding(15);
            var metrics = InitializeMetrics(bounds, padding);
            metrics.Margin = LoginLayoutHelpers.GetMargin(viewType);
            metrics.Spacing = LoginLayoutHelpers.GetSpacing(viewType);

            int margin = metrics.Margin;
            int currentY = metrics.ContainerPadding.Top + margin;
            int containerWidth = metrics.ContainerWidth;

            // 1) Title - centered
            var titleSize = new Size(200, 30); // Approximate
            var titlePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, titleSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Title", new Rectangle(titlePos.X, titlePos.Y, titleSize.Width, titleSize.Height));
            currentY += titleSize.Height + margin;

            // 2) Subtitle - centered, full width with wrapping
            var subtitleSize = new Size(containerWidth - (metrics.ContainerPadding.Left * 2), 40);
            var subtitlePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, subtitleSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Subtitle", new Rectangle(subtitlePos.X, subtitlePos.Y, subtitleSize.Width, subtitleSize.Height));
            currentY += subtitleSize.Height + margin;

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

            // 5) Remember Me - left aligned, full width
            var checkboxSize = new Size(containerWidth - (metrics.ContainerPadding.Left * 2), 40);
            metrics.SetControlBounds("RememberMe", new Rectangle(metrics.ContainerPadding.Left + margin, currentY, checkboxSize.Width, checkboxSize.Height));
            currentY += checkboxSize.Height + margin;

            // 6) Forgot Password - centered, full width
            var linkSize = new Size(containerWidth - (metrics.ContainerPadding.Left * 2), 40);
            var linkPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, linkSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("ForgotPassword", new Rectangle(linkPos.X, linkPos.Y, linkSize.Width, linkSize.Height));
            currentY += linkSize.Height + margin;

            // 7) Login button - centered, 100x30
            var loginSize = LoginLayoutHelpers.GetControlSize("loginbutton", viewType);
            var loginPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, loginSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPos.X, loginPos.Y, loginSize.Width, loginSize.Height));

            return metrics;
        }
    }
}

