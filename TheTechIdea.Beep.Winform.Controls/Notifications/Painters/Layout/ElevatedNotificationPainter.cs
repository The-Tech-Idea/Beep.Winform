using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Elevated card painter — 56 dp circle badge icon, drop-shadow approximation, TitleMedium font.
    /// Resembles DevExpress FlyoutPanel + Material 3 elevated card.
    /// </summary>
    public sealed class ElevatedNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 16;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Shadow approximation — offset rect, semi-transparent dark
            int shadowOffset = S(3);
            var shadowRect = new Rectangle(bounds.X + shadowOffset, bounds.Y + shadowOffset,
                bounds.Width, bounds.Height);
            using var shadowPath = CreateRoundedPath(shadowRect, radius);
            using var shadowBrush = new SolidBrush(Color.FromArgb(35, 0, 0, 0));
            g.FillPath(shadowBrush, shadowPath);

            // Main surface: light tonal
            Color back   = Color.FromArgb(20, colors.IconColor);
            Color border = Color.FromArgb(50, colors.BorderColor);
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            // Large circle badge
            DrawCircleBadgeIcon(g, iconRect, ip, Color.FromArgb(35, colors.IconColor), colors.IconColor);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            // Use TitleFont (larger than Standard)
            Font f = TitleFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, title, f, rect, colors.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.Top |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawMessage(g, rect, message, Color.FromArgb(185, colors.ForeColor));
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int r = rect.Height / 2;
            using (var bg  = new SolidBrush(Color.FromArgb(40, colors.IconColor)))
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
