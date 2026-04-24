using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Overlay notification painter — screen-centre modal style.
    /// <list type="bullet">
    ///   <item>Semi-transparent dark scrim (should be painted by BeepNotification on a separate layer)</item>
    ///   <item>White centred card with 20 dp corners</item>
    ///   <item>Title centred, large</item>
    ///   <item>Message centred below title</item>
    ///   <item>Actions centred, horizontal row</item>
    ///   <item>Drop shadow</item>
    /// </list>
    /// </summary>
    public sealed class OverlayNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 20;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Drop-shadow
            int so = S(6);
            var shadowRect = new Rectangle(bounds.X + so / 2, bounds.Y + so,
                bounds.Width, bounds.Height);
            using (var shadowPath = CreateRoundedPath(shadowRect, radius))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                g.FillPath(shadowBrush, shadowPath);

            // Card
            Color back   = data.CustomBackColor ?? colors.BackColor;
            Color border = colors.BorderColor;
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Font f = TitleFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, title, f, rect, colors.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Font f = MessageFont ?? SystemFonts.DefaultFont;
            Color msgColor = Color.FromArgb(185, colors.ForeColor);
            TextRenderer.DrawText(g, message, f, rect, msgColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int r = rect.Height / 2;
            using (var bg  = new SolidBrush(Color.FromArgb(229, 231, 235)))
            using (var bgP = CreateRoundedPath(rect, r))
                g.FillPath(bg, bgP);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
            {
                var fill = new Rectangle(rect.X, rect.Y, pw, rect.Height);
                using var fp = CreateRoundedPath(fill, r);
                using var fb = new SolidBrush(colors.IconColor);
                g.FillPath(fb, fp);
            }
        }
    }
}
