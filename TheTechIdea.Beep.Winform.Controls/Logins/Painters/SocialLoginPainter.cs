using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Logins.Helpers;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Social login: title + subtitle (centred), username + password
    /// (full-width), login button (full-width), "or" divider with
    /// social buttons, forgot-password link.
    /// </summary>
    public class SocialLoginPainter : BaseLoginPainter
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

            // Title
            var tSz = new Size(inputW, 28);
            metrics.SetControlBounds("Title", new Rectangle(pad.Left, y, tSz.Width, tSz.Height));
            y += tSz.Height + sp;

            // Subtitle
            var sSz = new Size(inputW, 32);
            metrics.SetControlBounds("Subtitle", new Rectangle(pad.Left, y, sSz.Width, sSz.Height));
            y += sSz.Height + sp + 4;

            // Username
            var uSz = LoginLayoutHelpers.GetControlSize("username", containerW);
            var uPt = LoginLayoutHelpers.CenterH(containerW, uSz.Width, y, pad);
            metrics.SetControlBounds("Username", new Rectangle(uPt.X, uPt.Y, uSz.Width, uSz.Height));
            y += uSz.Height + sp;

            // Password
            var pSz = LoginLayoutHelpers.GetControlSize("password", containerW);
            var pPt = LoginLayoutHelpers.CenterH(containerW, pSz.Width, y, pad);
            metrics.SetControlBounds("Password", new Rectangle(pPt.X, pPt.Y, pSz.Width, pSz.Height));
            y += pSz.Height + LoginLayoutHelpers.ButtonTopMargin;

            // Login button
            var lSz = LoginLayoutHelpers.GetControlSize("loginbutton", containerW);
            var lPt = LoginLayoutHelpers.CenterH(containerW, lSz.Width, y, pad);
            metrics.SetControlBounds("LoginButton", new Rectangle(lPt.X, lPt.Y, lSz.Width, lSz.Height));
            y += lSz.Height + sp + 4;

            // Social buttons — stacked, full-width
            var sbSz = LoginLayoutHelpers.GetControlSize("socialbutton", containerW);
            int sbY = y;
            metrics.SetControlBounds("FacebookButton", new Rectangle(pad.Left, sbY, sbSz.Width, sbSz.Height));
            sbY += sbSz.Height + sp;
            metrics.SetControlBounds("TwitterButton", new Rectangle(pad.Left, sbY, sbSz.Width, sbSz.Height));
            sbY += sbSz.Height + sp;
            metrics.SetControlBounds("GoogleButton", new Rectangle(pad.Left, sbY, sbSz.Width, sbSz.Height));
            sbY += sbSz.Height + sp;

            // Forgot Password
            metrics.SetControlBounds("ForgotPassword",
                new Rectangle(pad.Left, sbY, inputW, 20));

            return metrics;
        }
    }
}
