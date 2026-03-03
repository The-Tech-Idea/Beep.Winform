using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Callout / speech-bubble notification painter.
    /// Paints a rounded rectangle with a small triangular pointer (tail) at the bottom
    /// that points toward the <see cref="NotificationData.AnchorControl"/>.
    /// Tail direction defaults to pointing downward.
    /// </summary>
    public sealed class CalloutNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 10;
        private const int TailHeight    = 10;
        private const int TailWidth     = 14;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);
            int tailH  = S(TailHeight);
            int tailW  = S(TailWidth);

            // Bubble body (excludes tail area at bottom)
            var bubbleRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height - tailH);

            // Build path: rounded rect + downward triangle tail
            using var path = new GraphicsPath();
            int d = Math.Min(radius * 2, Math.Min(bubbleRect.Width, bubbleRect.Height));
            path.AddArc(bubbleRect.X, bubbleRect.Y, d, d, 180, 90);
            path.AddArc(bubbleRect.Right - d, bubbleRect.Y, d, d, 270, 90);
            path.AddArc(bubbleRect.Right - d, bubbleRect.Bottom - d, d, d, 0, 90);

            // Tail triangle
            int tailCentreX = bubbleRect.X + bubbleRect.Width / 2;
            path.AddLine(bubbleRect.Right - d / 2, bubbleRect.Bottom,
                         tailCentreX + tailW / 2, bubbleRect.Bottom);
            path.AddLine(tailCentreX + tailW / 2, bubbleRect.Bottom,
                         tailCentreX, bounds.Bottom);
            path.AddLine(tailCentreX, bounds.Bottom,
                         tailCentreX - tailW / 2, bubbleRect.Bottom);

            path.AddArc(bubbleRect.X, bubbleRect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            Color back   = Color.FromArgb(20, colors.IconColor);
            using (var brush = new SolidBrush(back))
                g.FillPath(brush, path);

            using var pen = new Pen(Color.FromArgb(80, colors.BorderColor), 1f);
            g.DrawPath(pen, path);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
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
