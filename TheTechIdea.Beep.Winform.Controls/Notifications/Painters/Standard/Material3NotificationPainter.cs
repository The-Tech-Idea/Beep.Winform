using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Google Material Design 3 notification painter.
    /// <list type="bullet">
    ///   <item>Tonal surface fill (type colour @ 12 % alpha)</item>
    ///   <item>Subtle border (type colour @ 25 % alpha)</item>
    ///   <item>Icon in filled circle badge (40 dp, type colour @ 15 % alpha background)</item>
    ///   <item>Title: TitleSmall / Body: BodyMedium — both from theme</item>
    ///   <item>Corner radius: 12 dp</item>
    ///   <item>Optional 4 dp left accent stripe</item>
    ///   <item>Bottom progress bar 4 dp</item>
    /// </list>
    /// </summary>
    public sealed class Material3NotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 12;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var opts   = CreateRenderOptions(data);
            var colors = GetColorsForType(data.Type, opts);
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Tonal surface fill
            Color back = Color.FromArgb(30, colors.IconColor);
            DrawBackground(g, bounds, back, Color.FromArgb(64, colors.BorderColor), radius);

            // Optional left accent stripe
            if (data.ShowAccentStripe)
            {
                int sw = S(4);
                Color sc = data.AccentStripeColor ?? colors.IconColor;
                using var sb = new SolidBrush(sc);
                g.FillRectangle(sb, bounds.X, bounds.Y + radius, sw, bounds.Height - radius * 2);
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);

            DrawCircleBadgeIcon(g, iconRect, ip,
                Color.FromArgb(40, colors.IconColor), colors.IconColor);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawTitle(g, rect, title, colors.ForeColor);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawMessage(g, rect, message, Color.FromArgb(190, colors.ForeColor));
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
