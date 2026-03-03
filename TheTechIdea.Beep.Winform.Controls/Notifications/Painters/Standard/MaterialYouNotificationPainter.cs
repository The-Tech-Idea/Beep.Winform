using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Google Material You (Material Design 3 dynamic colour) notification painter.
    /// <list type="bullet">
    ///   <item>Dynamic tonal surface: type colour base, very light tint</item>
    ///   <item>Bold 8 dp gradient header bar at top in type colour</item>
    ///   <item>28 dp pill-shaped corners</item>
    ///   <item>Large (40 dp) icon in circle badge inside or overlapping the header</item>
    ///   <item>Title uses TitleMedium / Message uses BodyMedium</item>
    /// </list>
    /// </summary>
    public sealed class MaterialYouNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 28;
        private const int HeaderHeight  = 8;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Body — tonal surface
            Color back = Color.FromArgb(25, colors.IconColor);
            DrawBackground(g, bounds, back, Color.Transparent, radius);

            // Top gradient bar
            int hh = S(HeaderHeight);
            var headerRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, hh + radius);
            using var path = CreateRoundedPath(headerRect, radius);
            // Clip to top rounded corners
            g.SetClip(path);
            using (var grad = new LinearGradientBrush(
                new PointF(bounds.X, bounds.Y), new PointF(bounds.Right, bounds.Y),
                colors.IconColor, Color.FromArgb(180, colors.IconColor.R, colors.IconColor.G, colors.IconColor.B)))
            {
                g.FillRectangle(grad, headerRect);
            }
            g.ResetClip();
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawCircleBadgeIcon(g, iconRect, ip,
                Color.FromArgb(50, colors.IconColor), colors.IconColor);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, Color.FromArgb(28, 27, 31));

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(110, 107, 110));

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int r = rect.Height / 2;
            using (var bg = new SolidBrush(Color.FromArgb(40, colors.IconColor)))
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
