using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Compact notification painter - inline icon and text, minimal spacing
    /// </summary>
    public class CompactNotificationPainter : NotificationPainterBase
    {
        public override void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data)
        {
            if (titleRect.IsEmpty || string.IsNullOrEmpty(title))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var titleFont = new Font("Segoe UI", 9, FontStyle.Bold);

            TextRenderer.DrawText(g, title, titleFont, titleRect, colors.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data)
        {
            if (messageRect.IsEmpty || string.IsNullOrEmpty(message))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var messageColor = Color.FromArgb(160, colors.ForeColor);
            var messageFont = new Font("Segoe UI", 8);

            TextRenderer.DrawText(g, message, messageFont, messageRect, messageColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
