using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Windows 11 Mica-inspired notification painter.
    /// <list type="bullet">
    ///   <item>Mica surface: very-light blue-tinted white @ 80 % alpha</item>
    ///   <item>Ultra-faint 15 % gray border</item>
    ///   <item>8 dp corners (Win11 system notification radius)</item>
    ///   <item>Icon: flat with semi-transparent container</item>
    ///   <item>Timestamp rendered bottom-right in caption font</item>
    /// </list>
    /// </summary>
    public sealed class Windows11MicaNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 8;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Mica: near-white with very slight blue-gray tint
            Color back   = Color.FromArgb(204, 250, 250, 255);
            Color border = Color.FromArgb(38, 0, 0, 0);
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);

            // Flat icon with subtle container
            int r = S(6);
            using (var path = CreateRoundedPath(iconRect, r))
            using (var bg = new SolidBrush(Color.FromArgb(30, colors.IconColor)))
                g.FillPath(bg, path);

            int inset = S(4);
            var inner = new Rectangle(iconRect.X + inset, iconRect.Y + inset,
                iconRect.Width - inset * 2, iconRect.Height - inset * 2);
            DrawIcon(g, inner, ip, colors.IconColor, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, Color.FromArgb(28, 28, 30));

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            // Reserve bottom-right area for timestamp
            if (data.Timestamp != default)
            {
                int tsW = S(55);
                var tsRect = new Rectangle(rect.Right - tsW, rect.Bottom - S(16), tsW, S(16));
                Font cf = CaptionFont ?? SystemFonts.DefaultFont;
                TextRenderer.DrawText(g, data.Timestamp.ToString("HH:mm"), cf, tsRect,
                    Color.FromArgb(120, 28, 28, 30),
                    TextFormatFlags.Right | TextFormatFlags.Bottom);
                rect = new Rectangle(rect.X, rect.Y, rect.Width - tsW - S(4), rect.Height);
            }
            DrawMessage(g, rect, message, Color.FromArgb(95, 95, 100));
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(35, colors.IconColor)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
