using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Dark Glow notification painter — premium dark mode with coloured glow border.
    /// <list type="bullet">
    ///   <item>Dark surface: #1A1A2E</item>
    ///   <item>2 dp coloured glow border (type colour, semi-transparent)</item>
    ///   <item>Outer glow effect (simulated with a slightly larger semi-transparent path)</item>
    ///   <item>12 dp corners</item>
    ///   <item>Icon in glowing circle</item>
    ///   <item>White title, slightly muted message</item>
    /// </list>
    /// </summary>
    public sealed class DarkGlowNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 12;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Outer glow — expanded rect, very transparent
            int glowOffset = S(4);
            var glowRect   = Rectangle.Inflate(bounds, glowOffset, glowOffset);
            using (var glowPath = CreateRoundedPath(glowRect, radius + glowOffset))
            using (var glowBrush = new SolidBrush(Color.FromArgb(35, colors.IconColor)))
                g.FillPath(glowBrush, glowPath);

            // Main dark surface
            Color back   = Color.FromArgb(26, 26, 46);   // #1A1A2E
            DrawBackground(g, bounds, back, Color.Transparent, radius);

            // Coloured border (glow effect)
            using var borderPath = CreateRoundedPath(bounds, radius);
            using var borderPen  = new Pen(Color.FromArgb(180, colors.IconColor), S(2));
            g.DrawPath(borderPen, borderPath);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);

            // Glowing circle background
            using (var glowB = new SolidBrush(Color.FromArgb(50, colors.IconColor)))
                g.FillEllipse(glowB, iconRect);
            using (var glowP = new Pen(Color.FromArgb(120, colors.IconColor), S(2)))
                g.DrawEllipse(glowP, iconRect);

            int inset = S(5);
            var inner = new Rectangle(iconRect.X + inset, iconRect.Y + inset,
                iconRect.Width - inset * 2, iconRect.Height - inset * 2);
            DrawIcon(g, inner, ip, Color.FromArgb(230, colors.IconColor), 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, Color.FromArgb(240, 240, 255));  // near-white

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(160, 160, 190)); // muted blue-gray

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(45, 45, 65)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
            {
                using var grad = new LinearGradientBrush(
                    new PointF(rect.X, rect.Y), new PointF(rect.X + pw, rect.Y),
                    Color.FromArgb(180, colors.IconColor), colors.IconColor);
                g.FillRectangle(grad, rect.X, rect.Y, pw, rect.Height);
            }
        }
    }
}
