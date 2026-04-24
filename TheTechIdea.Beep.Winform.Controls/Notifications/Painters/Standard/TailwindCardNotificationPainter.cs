using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Tailwind CSS card-style notification painter.
    /// <list type="bullet">
    ///   <item>Pure white card with 1dp #E5E7EB border</item>
    ///   <item>8 dp corner radius</item>
    ///   <item>Icon in plain coloured box (no circle), flat</item>
    ///   <item>Title in gray-900, message in gray-600</item>
    ///   <item>Subtle drop-shadow approximated with a slightly darker border</item>
    /// </list>
    /// </summary>
    public sealed class TailwindCardNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 8;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            Color back   = data.CustomBackColor ?? colors.BackColor;
            Color border = colors.BorderColor;
            DrawBackground(g, bounds, back, border, radius);

            // Optional accent stripe
            if (data.ShowAccentStripe)
            {
                int sw = S(4);
                Color sc = data.AccentStripeColor ?? colors.IconColor;
                using var sb = new SolidBrush(sc);
                g.FillRectangle(sb, bounds.X, bounds.Y + radius, sw, bounds.Height - radius * 2);
            }
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawTitle(g, rect, title, colors.ForeColor);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Color msgColor = Color.FromArgb(185, colors.ForeColor);
            DrawMessage(g, rect, message, msgColor);
        }

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
