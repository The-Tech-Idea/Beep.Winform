using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Gradient Modern notification painter — premium gradient header + white body card.
    /// <list type="bullet">
    ///   <item>Top gradient header (48 dp, type colour → lighter shade)</item>
    ///   <item>White body below the header</item>
    ///   <item>12 dp corners</item>
    ///   <item>Icon rendered on header, white tint</item>
    ///   <item>Title on header in white; message below in body, dark text</item>
    ///   <item>Very subtle drop shadow (extra semi-transparent border)</item>
    /// </list>
    /// </summary>
    public sealed class GradientModernNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 12;
        private const int HeaderHeight  = 44;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);
            int hh     = S(HeaderHeight);

            // Body (white card, full bounds)
            Color back   = Color.White;
            Color border = Color.FromArgb(30, 0, 0, 0);
            DrawBackground(g, bounds, back, border, radius);

            // Gradient header (top rounded only)
            var headerRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, hh + radius);
            using var clipPath = CreateRoundedPath(headerRect, radius);
            g.SetClip(clipPath);

            // Build gradient: type colour → lighter version
            Color gradStart = colors.IconColor;
            Color gradEnd   = Color.FromArgb(200,
                Math.Min(255, gradStart.R + 60),
                Math.Min(255, gradStart.G + 40),
                Math.Min(255, gradStart.B + 40));

            using var gradBrush = new LinearGradientBrush(
                new PointF(bounds.X, bounds.Y), new PointF(bounds.Right, bounds.Y),
                gradStart, gradEnd);
            g.FillPath(gradBrush, clipPath);
            g.ResetClip();
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            // Icon appears on the header in white
            string ip = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawIcon(g, iconRect, ip, Color.White, S(4));
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            // Title is on the gradient header — use white text
            int hh = S(HeaderHeight);
            bool onHeader = rect.Y < hh + S(DefaultRadius);

            Font f  = TitleFont ?? SystemFonts.DefaultFont;
            Color c = onHeader ? Color.White : Color.FromArgb(29, 29, 31);
            TextRenderer.DrawText(g, title, f, rect, c,
                TextFormatFlags.Left | TextFormatFlags.Top |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(80, 80, 90));

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(229, 231, 235)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
