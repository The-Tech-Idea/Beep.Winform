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
    /// Painter for Full login view type
    /// Logo; Row with Avatar & Title; Subtitle; Username; Password; Row with Login, Forgot, Register
    /// </summary>
    public class FullLoginPainter : BaseLoginPainter
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

            // 1) Logo - centered, 60x60
            var logoSize = LoginIconHelpers.GetIconSize("logo");
            var logoPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, logoSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Logo", new Rectangle(logoPos.X, logoPos.Y, logoSize.Width, logoSize.Height));
            currentY += logoSize.Height + margin;

            // 2) Avatar - centered, 60x60
            var avatarSize = LoginIconHelpers.GetIconSize("avatar");
            var avatarPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, avatarSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Avatar", new Rectangle(avatarPos.X, avatarPos.Y, avatarSize.Width, avatarSize.Height));
            currentY += avatarSize.Height + margin;

            // 3) Title - centered, full width
            var titleSize = new Size(containerWidth - (metrics.ContainerPadding.Left * 2), 40);
            metrics.SetControlBounds("Title", new Rectangle(metrics.ContainerPadding.Left + margin, currentY, titleSize.Width, titleSize.Height));
            currentY += titleSize.Height + margin;

            // 4) Subtitle - centered, full width
            var subtitleSize = new Size(containerWidth - (metrics.ContainerPadding.Left * 2), 40);
            metrics.SetControlBounds("Subtitle", new Rectangle(metrics.ContainerPadding.Left + margin, currentY, subtitleSize.Width, subtitleSize.Height));
            currentY += subtitleSize.Height + margin;

            // 5) Username - centered, 300x30
            var usernameSize = new Size(300, 30);
            var usernamePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, usernameSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Username", new Rectangle(usernamePos.X, usernamePos.Y, usernameSize.Width, usernameSize.Height));
            currentY += usernameSize.Height + margin;

            // 6) Password - centered, 300x30
            var passwordSize = new Size(300, 30);
            var passwordPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, passwordSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Password", new Rectangle(passwordPos.X, passwordPos.Y, passwordSize.Width, passwordSize.Height));
            currentY += passwordSize.Height + margin;

            // 7) Login, Forgot, Register - side by side, centered
            var loginSize = new Size(100, 30);
            var linkSize = new Size(100, 20);
            int totalRowWidth = loginSize.Width + linkSize.Width + linkSize.Width + 20;
            int startX = metrics.ContainerPadding.Left + (containerWidth - totalRowWidth) / 2;
            
            metrics.SetControlBounds("LoginButton", new Rectangle(startX, currentY, loginSize.Width, loginSize.Height));
            metrics.SetControlBounds("ForgotPassword", new Rectangle(startX + loginSize.Width + 10, currentY + (loginSize.Height - linkSize.Height) / 2, linkSize.Width, linkSize.Height));
            metrics.SetControlBounds("RegisterLink", new Rectangle(startX + loginSize.Width + linkSize.Width + 20, currentY + (loginSize.Height - linkSize.Height) / 2, linkSize.Width, linkSize.Height));

            return metrics;
        }
    }
}

