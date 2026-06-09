using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Helpers
{
    /// <summary>
    /// Layout helpers for login painters.  All sizes are responsive
    /// to the container width — inputs and buttons fill the card,
    /// not a fixed 250 px or 100 px that looked tiny on modern displays.
    /// </summary>
    public static class LoginLayoutHelpers
    {
        /// <summary>Standard card padding (24px on all sides).</summary>
        public static Padding CardPadding => new Padding(24);

        /// <summary>Space between two consecutive elements (16px).</summary>
        public static int ElementSpacing => 16;

        /// <summary>Extra space above the login button (24px).</summary>
        public static int ButtonTopMargin => 24;

        /// <summary>
        /// Returns a full-width size for the given control type.
        /// Input fields and buttons always fill the card width
        /// (minus padding); titles/subtitles/links auto-size.
        /// </summary>
        public static Size GetControlSize(string controlType, int containerWidth, int padding = 24)
        {
            int w = containerWidth - padding * 2;
            if (w < 200) w = 200;
            return controlType?.ToLowerInvariant() switch
            {
                "username" => new Size(w, 40),
                "password" => new Size(w, 40),
                "loginbutton" => new Size(w, 44),
                "socialbutton" => new Size(w, 44),
                "avatar" => new Size(64, 64),
                "logo" => new Size(64, 64),
                _ => new Size(0, 0) // title / subtitle / checkbox / link auto-size
            };
        }

        /// <summary>Centers a control horizontally in the container.</summary>
        public static Point CenterH(int containerWidth, int controlWidth, int y, Padding pad)
            => new Point(pad.Left + (containerWidth - controlWidth) / 2, y);

        // ── legacy names (kept for backward compat with older painters) ──
        public static Point CenterHorizontally(int cw, int ctrlW, int y, Padding p)
            => CenterH(cw, ctrlW, y, p);
        public static Point AlignLeft(int cw, int ctrlW, int y, Padding p, int m = 0)
            => new Point(p.Left + m, y);
        public static Point AlignRight(int cw, int ctrlW, int y, Padding p, int m = 0)
            => new Point(p.Left + cw - ctrlW - m, y);
        public static Size GetControlSize(string t, LoginViewType _)
            => GetControlSize(t, 400, 24);

        /// <summary>Preferred card size per view type.</summary>
        public static Size GetPreferredSize(LoginViewType vt)
        {
            return vt switch
            {
                LoginViewType.Simple => new Size(380, 300),
                LoginViewType.Compact => new Size(340, 220),
                LoginViewType.Minimal => new Size(380, 280),
                LoginViewType.Social => new Size(400, 420),
                LoginViewType.SocialView2 => new Size(400, 460),
                LoginViewType.Modern => new Size(400, 360),
                LoginViewType.Avatar => new Size(400, 380),
                LoginViewType.Extended => new Size(440, 400),
                LoginViewType.Full => new Size(480, 460),
                _ => new Size(400, 360)
            };
        }

        // ── legacy overloads for callers that pass LoginViewType ──
        public static int GetMargin(LoginViewType _) => ElementSpacing;
        public static int GetSpacing(LoginViewType _) => ElementSpacing;
        public static Padding GetRecommendedPadding(LoginViewType _) => CardPadding;
        public static Size GetControlSize(string t, LoginViewType _)
            => GetControlSize(t, 400, 24);
    }
}
