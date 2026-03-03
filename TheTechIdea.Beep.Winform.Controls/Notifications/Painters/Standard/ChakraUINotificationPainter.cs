using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Chakra UI Alert component-inspired notification painter.
    /// <list type="bullet">
    ///   <item>Very light type-semantic tint (#FFF5F5, #F0FFF4, etc.) per type</item>
    ///   <item>Mandatory 4 dp left stripe (type colour, Chakra signature)</item>
    ///   <item>Thin matching border</item>
    ///   <item>6 dp corner radius</item>
    ///   <item>Icon circle badge, type colour</item>
    /// </list>
    /// </summary>
    public sealed class ChakraUINotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 6;

        private static Color GetSemanticBackground(NotificationType type) => type switch
        {
            NotificationType.Error   => Color.FromArgb(255, 245, 245),
            NotificationType.Warning => Color.FromArgb(255, 250, 235),
            NotificationType.Success => Color.FromArgb(240, 255, 244),
            NotificationType.System  => Color.FromArgb(237, 242, 247),
            _                        => Color.FromArgb(235, 248, 255),   // Info default
        };

        private static Color GetSemanticForeground(NotificationType type) => type switch
        {
            NotificationType.Error   => Color.FromArgb(130, 0, 0),
            NotificationType.Warning => Color.FromArgb(114, 60, 0),
            NotificationType.Success => Color.FromArgb(0, 92, 46),
            NotificationType.System  => Color.FromArgb(45, 55, 72),
            _                        => Color.FromArgb(8, 95, 164),
        };

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            Color back   = data.CustomBackColor ?? GetSemanticBackground(data.Type);
            Color border = Color.FromArgb(120, colors.BorderColor);
            DrawBackground(g, bounds, back, border, radius);

            // Mandatory left stripe
            int sw = S(4);
            using var sb = new SolidBrush(colors.IconColor);
            g.FillRectangle(sb, bounds.X, bounds.Y + radius, sw, bounds.Height - radius * 2);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawCircleBadgeIcon(g, iconRect, ip, Color.Transparent, colors.IconColor);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, data.CustomForeColor ?? GetSemanticForeground(data.Type));

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(210, data.CustomForeColor ?? GetSemanticForeground(data.Type)));

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
