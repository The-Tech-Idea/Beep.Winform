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
    /// Painter for SocialView2 login view type
    /// Title; Username; Password; Forgot; Login; Subtitle; Social buttons; Register
    /// </summary>
    public class SocialView2LoginPainter : BaseLoginPainter
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

            // 4) Forgot Password - right aligned, full width
            var linkSize = new Size(containerWidth - (metrics.ContainerPadding.Left * 2), 40);
            var linkPos = LoginLayoutHelpers.AlignRight(containerWidth, linkSize.Width, currentY, metrics.ContainerPadding, margin);
            metrics.SetControlBounds("ForgotPassword", new Rectangle(linkPos.X, linkPos.Y, linkSize.Width, linkSize.Height));
            currentY += linkSize.Height + margin;

            // 5) Login button - centered, 150x40
            var loginSize = new Size(150, 40);
            var loginPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, loginSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPos.X, loginPos.Y, loginSize.Width, loginSize.Height));
            currentY += loginSize.Height + margin;

            // 6) Subtitle - centered, full width
            var subtitleSize = new Size(containerWidth - (metrics.ContainerPadding.Left * 2), 40);
            var subtitlePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, subtitleSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Subtitle", new Rectangle(subtitlePos.X, subtitlePos.Y, subtitleSize.Width, subtitleSize.Height));
            currentY += subtitleSize.Height + margin;

            // 7) Social buttons - Facebook and Google side-by-side
            int socialSize = 150;
            int socialHeight = 40;
            int totalSocialWidth = socialSize * 2 + 10;
            int socialStartX = metrics.ContainerPadding.Left + (containerWidth - totalSocialWidth) / 2;
            
            metrics.SetControlBounds("FacebookButton", new Rectangle(socialStartX, currentY, socialSize, socialHeight));
            metrics.SetControlBounds("GoogleButton", new Rectangle(socialStartX + socialSize + 10, currentY, socialSize, socialHeight));
            currentY += socialHeight + margin;

            // 8) Register Link - centered, full width
            var registerSize = new Size(containerWidth - (metrics.ContainerPadding.Left * 2), 40);
            var registerPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, registerSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("RegisterLink", new Rectangle(registerPos.X, registerPos.Y, registerSize.Width, registerSize.Height));

            return metrics;
        }
    }
}

