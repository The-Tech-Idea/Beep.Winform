using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Logins.Helpers;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Simple login: avatar (centred), username + password (full-width),
    /// login button (full-width), forgot password link.
    /// </summary>
    public class SimpleLoginPainter : BaseLoginPainter
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

            // Avatar — centred
            var avSz = LoginLayoutHelpers.GetControlSize("avatar", containerW);
            var avPt = LoginLayoutHelpers.CenterH(containerW, avSz.Width, y, pad);
            metrics.SetControlBounds("Avatar", new Rectangle(avPt.X, avPt.Y, avSz.Width, avSz.Height));
            y += avSz.Height + sp + 8;

            // Username — full-width
            var uSz = LoginLayoutHelpers.GetControlSize("username", containerW);
            var uPt = LoginLayoutHelpers.CenterH(containerW, uSz.Width, y, pad);
            metrics.SetControlBounds("Username", new Rectangle(uPt.X, uPt.Y, uSz.Width, uSz.Height));
            y += uSz.Height + sp;

            // Password — full-width
            var pSz = LoginLayoutHelpers.GetControlSize("password", containerW);
            var pPt = LoginLayoutHelpers.CenterH(containerW, pSz.Width, y, pad);
            metrics.SetControlBounds("Password", new Rectangle(pPt.X, pPt.Y, pSz.Width, pSz.Height));
            y += pSz.Height + LoginLayoutHelpers.ButtonTopMargin;

            // Login button — full-width
            var lSz = LoginLayoutHelpers.GetControlSize("loginbutton", containerW);
            var lPt = LoginLayoutHelpers.CenterH(containerW, lSz.Width, y, pad);
            metrics.SetControlBounds("LoginButton", new Rectangle(lPt.X, lPt.Y, lSz.Width, lSz.Height));
            y += lSz.Height + sp;

            // Forgot Password link
            metrics.SetControlBounds("ForgotPassword", new Rectangle(pad.Left, y, inputW, 20));

            return metrics;
        }
    }
}
