using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Vercel dashboard dark-mode notification painter.
    /// <list type="bullet">
    ///   <item>Dark surface: #111111</item>
    ///   <item>Subtle #333333 border</item>
    ///   <item>6 dp corners</item>
    ///   <item>Icon: flat, white/light-gray tint</item>
    ///   <item>Title: crisp white; Message: gray-400</item>
    ///   <item>Thin type-colour bottom line for status indication</item>
    /// </list>
    /// </summary>
    public sealed class VercelCleanNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 6;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);
            Color back   = data.CustomBackColor ?? Color.FromArgb(17, 17, 17);
            Color border = Color.FromArgb(51, 51, 51);
            DrawBackground(g, bounds, back, border, radius);

            // Thin bottom accent
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int lineH = S(2);
            using var lb = new SolidBrush(Color.FromArgb(180, colors.IconColor));
            g.FillRectangle(lb, bounds.X, bounds.Bottom - lineH, bounds.Width, lineH);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            // On dark: tint toward type colour
            DrawIcon(g, iconRect, ip, Color.FromArgb(210, colors.IconColor.R, colors.IconColor.G, colors.IconColor.B), 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, Color.FromArgb(237, 237, 237));   // near-white

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(160, 163, 163, 163)); // gray-400

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(51, 51, 51)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
