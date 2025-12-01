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
    /// Painter for Social login view type
    /// Title; Username; Password; Row with Remember & Login; Subtitle; Social buttons; Register
    /// </summary>
    public class SocialLoginPainter : BaseLoginPainter
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

            // 1) Title - top-left
            var titleSize = new Size(100, 30); // Approximate
            metrics.SetControlBounds("Title", new Rectangle(metrics.ContainerPadding.Left, currentY, titleSize.Width, titleSize.Height));
            currentY += titleSize.Height + margin;

            // 2) Subtitle - left aligned, wraps
            var subtitleSize = new Size(containerWidth, 0); // Auto-height with wrapping
            metrics.SetControlBounds("Subtitle", new Rectangle(metrics.ContainerPadding.Left, currentY, subtitleSize.Width, 40));
            currentY += 40 + margin;

            // 3) Card area for inputs (centered, 320px wide)
            int cardWidth = 320;
            int cardX = metrics.ContainerPadding.Left + (containerWidth - cardWidth) / 2;
            int cardInnerMargin = 10;
            int localY = currentY + cardInnerMargin;

            // Username inside card
            var usernameSize = new Size(cardWidth - cardInnerMargin * 2, 30);
            metrics.SetControlBounds("Username", new Rectangle(cardX + cardInnerMargin, localY, usernameSize.Width, usernameSize.Height));
            localY += usernameSize.Height + cardInnerMargin;

            // Password inside card
            var passwordSize = new Size(cardWidth - cardInnerMargin * 2, 30);
            metrics.SetControlBounds("Password", new Rectangle(cardX + cardInnerMargin, localY, passwordSize.Width, passwordSize.Height));
            localY += passwordSize.Height + cardInnerMargin;

            // Remember Me (left) and Forgot Password (right) inside card
            var checkboxSize = new Size(120, 20);
            metrics.SetControlBounds("RememberMe", new Rectangle(cardX + cardInnerMargin, localY - 5, checkboxSize.Width, checkboxSize.Height));
            
            var linkSize = new Size(120, 20);
            int forgotX = cardX + cardWidth - linkSize.Width - cardInnerMargin;
            metrics.SetControlBounds("ForgotPassword", new Rectangle(forgotX, localY, linkSize.Width, linkSize.Height));
            
            localY += Math.Max(checkboxSize.Height, linkSize.Height) + cardInnerMargin;
            int cardHeight = localY - currentY;
            currentY += cardHeight + margin;

            // 4) Login button - centered, 120x35
            var loginSize = new Size(120, 35);
            var loginPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, loginSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPos.X, loginPos.Y, loginSize.Width, loginSize.Height));
            currentY += loginSize.Height + margin;

            // 5) "— or —" separator - centered (will be created dynamically)
            var separatorSize = new Size(50, 20);
            var separatorPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, separatorSize.Width, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("Separator", new Rectangle(separatorPos.X, separatorPos.Y, separatorSize.Width, separatorSize.Height));
            currentY += separatorSize.Height + margin;

            // 6) Social buttons - Facebook, Twitter, Google (centered, 320x40 each)
            int socialWidth = cardWidth;
            int socialHeight = 40;
            
            var facebookPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, socialWidth, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("FacebookButton", new Rectangle(facebookPos.X, facebookPos.Y, socialWidth, socialHeight));
            currentY += socialHeight + margin;

            var twitterPos = LoginLayoutHelpers.CenterHorizontally(containerWidth, socialWidth, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("TwitterButton", new Rectangle(twitterPos.X, twitterPos.Y, socialWidth, socialHeight));
            currentY += socialHeight + margin;

            var googlePos = LoginLayoutHelpers.CenterHorizontally(containerWidth, socialWidth, currentY, metrics.ContainerPadding);
            metrics.SetControlBounds("GoogleButton", new Rectangle(googlePos.X, googlePos.Y, socialWidth, socialHeight));

            return metrics;
        }
    }
}

