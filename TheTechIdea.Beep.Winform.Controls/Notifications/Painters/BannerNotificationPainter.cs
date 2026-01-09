using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Banner notification painter - wide banner style across top/bottom
    /// </summary>
    public class BannerNotificationPainter : NotificationPainterBase
    {
        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var brush = new SolidBrush(colors.BackColor))
            {
                // Banner style - full rectangle, no rounded corners
                g.FillRectangle(brush, bounds);
            }

            // Banner border at top or bottom
            using (var pen = new Pen(colors.BorderColor, 3))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }
        }

        public override void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data)
        {
            if (titleRect.IsEmpty || string.IsNullOrEmpty(title))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var titleFont = new Font("Segoe UI", 11, FontStyle.Bold);

            TextRenderer.DrawText(g, title, titleFont, titleRect, colors.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data)
        {
            if (messageRect.IsEmpty || string.IsNullOrEmpty(message))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var messageColor = Color.FromArgb(180, colors.ForeColor);
            var messageFont = new Font("Segoe UI", 9);

            TextRenderer.DrawText(g, message, messageFont, messageRect, messageColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
