using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Logins.Helpers;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Full login layout: logo + avatar (centred), title + subtitle,
    /// full-width inputs, full-width login button, and links
    /// below (forgot / register centred).
    /// </summary>
    public class FullLoginPainter : BaseLoginPainter
    {
        public override LoginLayoutMetrics CalculateMetrics(
            Rectangle bounds, LoginViewType viewType, LoginStyleConfig styleConfig)
        {
            var pad = LoginLayoutHelpers.CardPadding;
            var metrics = InitializeMetrics(bounds, pad);
            int sp = LoginLayoutHelpers.ElementSpacing;
            int containerW = metrics.ContainerWidth;
            int inputW = Math.Max(200, containerW - pad.Horizontal);
            int y = pad.Top + sp;

            // Logo — centred
            var logoSize = LoginIconHelpers.GetIconSize("logo");
            var logoPt = LoginLayoutHelpers.CenterH(containerW, logoSize.Width, y, pad);
            metrics.SetControlBounds("Logo", new Rectangle(logoPt.X, logoPt.Y, logoSize.Width, logoSize.Height));
            y += logoSize.Height + sp;

            // Avatar — centred
            var avSize = LoginIconHelpers.GetIconSize("avatar");
            var avPt = LoginLayoutHelpers.CenterH(containerW, avSize.Width, y, pad);
            metrics.SetControlBounds("Avatar", new Rectangle(avPt.X, avPt.Y, avSize.Width, avSize.Height));
            y += avSize.Height + sp + 4;

            // Title
            var titleSz = new Size(inputW, 28);
            metrics.SetControlBounds("Title", new Rectangle(pad.Left, y, titleSz.Width, titleSz.Height));
            y += titleSz.Height + sp;

            // Subtitle
            var subSz = new Size(inputW, 32);
            metrics.SetControlBounds("Subtitle", new Rectangle(pad.Left, y, subSz.Width, subSz.Height));
            y += subSz.Height + sp + 4;

            // Username — full width
            var unameSz = LoginLayoutHelpers.GetControlSize("username", containerW, pad.Horizontal / 2);
            var unamePt = LoginLayoutHelpers.CenterH(containerW, unameSz.Width, y, pad);
            metrics.SetControlBounds("Username", new Rectangle(unamePt.X, unamePt.Y, unameSz.Width, unameSz.Height));
            y += unameSz.Height + sp;

            // Password — full width
            var pwSz = LoginLayoutHelpers.GetControlSize("password", containerW, pad.Horizontal / 2);
            var pwPt = LoginLayoutHelpers.CenterH(containerW, pwSz.Width, y, pad);
            metrics.SetControlBounds("Password", new Rectangle(pwPt.X, pwPt.Y, pwSz.Width, pwSz.Height));
            y += pwSz.Height + LoginLayoutHelpers.ButtonTopMargin;

            // Login button — full width
            var loginSz = LoginLayoutHelpers.GetControlSize("loginbutton", containerW, pad.Horizontal / 2);
            var loginPt = LoginLayoutHelpers.CenterH(containerW, loginSz.Width, y, pad);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPt.X, loginPt.Y, loginSz.Width, loginSz.Height));
            y += loginSz.Height + sp;

            // Forgot password + Register links side-by-side, centred
            var linkW = inputW / 2;
            var linkH = 20;
            int linkY = y;
            metrics.SetControlBounds("ForgotPassword", new Rectangle(pad.Left, linkY, linkW, linkH));
            metrics.SetControlBounds("RegisterLink", new Rectangle(pad.Left + linkW, linkY, linkW, linkH));

            return metrics;
        }
    }
}
