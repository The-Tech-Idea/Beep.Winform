using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Logins.Helpers;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Modern login layout: centered title + subtitle, full-width
    /// inputs and button, remember-me checkbox, forgot-password link.
    /// </summary>
    public class ModernLoginPainter : BaseLoginPainter
    {
        public override LoginLayoutMetrics CalculateMetrics(
            Rectangle bounds, LoginViewType viewType, LoginStyleConfig styleConfig)
        {
            var pad = LoginLayoutHelpers.CardPadding;
            var metrics = InitializeMetrics(bounds, pad);
            int sp = LoginLayoutHelpers.ElementSpacing;
            int containerW = metrics.ContainerWidth;
            int y = pad.Top + sp;
            int inputW = Math.Max(200, containerW - pad.Horizontal);

            // Title — centered, auto-sized by label
            var titleSz = new Size(inputW, 28);
            var titlePt = LoginLayoutHelpers.CenterH(containerW, titleSz.Width, y, pad);
            metrics.SetControlBounds("Title", new Rectangle(titlePt.X, titlePt.Y, titleSz.Width, titleSz.Height));
            y += titleSz.Height + sp;

            // Subtitle — full width (label wraps)
            var subSz = new Size(inputW, 32);
            var subPt = LoginLayoutHelpers.CenterH(containerW, subSz.Width, y, pad);
            metrics.SetControlBounds("Subtitle", new Rectangle(subPt.X, subPt.Y, subSz.Width, subSz.Height));
            y += subSz.Height + sp + 4;

            // Username — full width, 40px height
            var unameSz = LoginLayoutHelpers.GetControlSize("username", containerW, pad.Horizontal / 2);
            var unamePt = LoginLayoutHelpers.CenterH(containerW, unameSz.Width, y, pad);
            metrics.SetControlBounds("Username", new Rectangle(unamePt.X, unamePt.Y, unameSz.Width, unameSz.Height));
            y += unameSz.Height + sp;

            // Password — full width, 40px height
            var pwSz = LoginLayoutHelpers.GetControlSize("password", containerW, pad.Horizontal / 2);
            var pwPt = LoginLayoutHelpers.CenterH(containerW, pwSz.Width, y, pad);
            metrics.SetControlBounds("Password", new Rectangle(pwPt.X, pwPt.Y, pwSz.Width, pwSz.Height));
            y += pwSz.Height + sp;

            // Remember Me — left aligned
            var chkSz = new Size(inputW, 24);
            metrics.SetControlBounds("RememberMe", new Rectangle(pad.Left, y, chkSz.Width, chkSz.Height));
            y += chkSz.Height + sp;

            // Forgot Password link — centered
            var linkSz = new Size(inputW, 20);
            var linkPt = LoginLayoutHelpers.CenterH(containerW, linkSz.Width, y, pad);
            metrics.SetControlBounds("ForgotPassword", new Rectangle(linkPt.X, linkPt.Y, linkSz.Width, linkSz.Height));
            y += linkSz.Height + LoginLayoutHelpers.ButtonTopMargin;

            // Login button — full width, 44px height
            var loginSz = LoginLayoutHelpers.GetControlSize("loginbutton", containerW, pad.Horizontal / 2);
            var loginPt = LoginLayoutHelpers.CenterH(containerW, loginSz.Width, y, pad);
            metrics.SetControlBounds("LoginButton", new Rectangle(loginPt.X, loginPt.Y, loginSz.Width, loginSz.Height));

            return metrics;
        }
    }
}
