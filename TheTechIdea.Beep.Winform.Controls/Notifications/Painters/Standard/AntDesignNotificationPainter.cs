using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Alibaba Ant Design notification painter (Alert component style).
    /// <list type="bullet">
    ///   <item>Very light type-tinted background (type fill @ 8 %) </item>
    ///   <item>Coloured left border stripe 4 dp (mandatory in Ant Design)</item>
    ///   <item>Sharp 4 dp corners</item>
    ///   <item>Icon flat, no badge — same colour as stripe</item>
    ///   <item>Title in medium weight; message in regular weight, slightly muted</item>
    /// </list>
    /// </summary>
    public sealed class AntDesignNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 4;
        private const int StripeWidth   = 4;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Very light tinted background
            Color back   = Color.FromArgb(20, colors.IconColor);
            Color border = Color.FromArgb(80, colors.BorderColor);
            DrawBackground(g, bounds, back, border, radius);

            // Mandatory left stripe (Ant Design signature)
            int sw = S(StripeWidth);
            using var stripeBrush = new SolidBrush(colors.IconColor);
            g.FillRectangle(stripeBrush, bounds.X, bounds.Y + radius, sw, bounds.Height - radius * 2);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            // Flat icon, no badge
            DrawIcon(g, iconRect, ip, colors.IconColor, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawTitle(g, rect, title, colors.ForeColor);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawMessage(g, rect, message, Color.FromArgb(185, colors.ForeColor));
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(30, colors.IconColor)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
