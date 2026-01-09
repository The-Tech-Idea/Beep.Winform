using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Toast notification painter - minimal toast style, auto-dismiss
    /// </summary>
    public class ToastNotificationPainter : NotificationPainterBase
    {
        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var brush = new SolidBrush(colors.BackColor))
            {
                // Toast style - rounded rectangle
                using (var path = CreateRoundedPath(bounds, 6))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public override void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data)
        {
            if (titleRect.IsEmpty || string.IsNullOrEmpty(title))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var titleFont = new Font("Segoe UI", 9, FontStyle.Regular);

            TextRenderer.DrawText(g, title, titleFont, titleRect, colors.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data)
        {
            // Toast typically doesn't show message, only title
        }

        public override void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data)
        {
            // Toast typically doesn't show actions
        }

        public override void PaintCloseButton(Graphics g, Rectangle closeButtonRect, bool isHovered, NotificationData data)
        {
            // Toast typically doesn't show close button
        }
    }
}
