using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Prominent notification painter - large icon, prominent text, centered layout
    /// </summary>
    public class ProminentNotificationPainter : NotificationPainterBase
    {
        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            if (iconRect.IsEmpty || string.IsNullOrEmpty(data.IconPath))
                return;

            // Use larger icon size for prominent layout
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var iconPath = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath
                : NotificationData.GetDefaultIconForType(data.Type);

            var iconColor = data.IconTint ?? colors.IconColor;

            // Expand icon rect for prominent style
            var expandedRect = new Rectangle(
                iconRect.X - 4,
                iconRect.Y - 4,
                iconRect.Width + 8,
                iconRect.Height + 8
            );

            StyledImagePainter.PaintWithTint(g, expandedRect, iconPath, iconColor, 1f, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data)
        {
            if (titleRect.IsEmpty || string.IsNullOrEmpty(title))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var titleFont = new Font("Segoe UI", 14, FontStyle.Bold);

            TextRenderer.DrawText(g, title, titleFont, titleRect, colors.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }

        public override void PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data)
        {
            if (messageRect.IsEmpty || string.IsNullOrEmpty(message))
                return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            var messageColor = Color.FromArgb(180, colors.ForeColor);
            var messageFont = new Font("Segoe UI", 10);

            TextRenderer.DrawText(g, message, messageFont, messageRect, messageColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }
    }
}
