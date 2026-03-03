using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Notion-inspired text-first minimal notification painter.
    /// <list type="bullet">
    ///   <item>White with near-invisible #E9E9E7 border</item>
    ///   <item>4 dp subtle corners</item>
    ///   <item>Small leading emoji-style icon (no badge, no background)</item>
    ///   <item>Title and message in Notion palette: near-black, muted gray</item>
    ///   <item>Extremely sparse — no progress bar decoration, just a plain line</item>
    /// </list>
    /// </summary>
    public sealed class NotionMinimalNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 4;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);
            Color back   = data.CustomBackColor ?? Color.White;
            Color border = Color.FromArgb(233, 233, 231);   // Notion divider colour
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            // Small flat icon — Notion uses emoji, we use SVG without any container
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawIcon(g, iconRect, ip, Color.FromArgb(170, colors.IconColor), 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, Color.FromArgb(37, 37, 38));      // Notion near-black

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(155, 155, 155));// Notion muted gray

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            // Notion style: plain thin line, no fill colour difference
            using (var bg = new SolidBrush(Color.FromArgb(233, 233, 231)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(Color.FromArgb(55, 53, 47)))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
