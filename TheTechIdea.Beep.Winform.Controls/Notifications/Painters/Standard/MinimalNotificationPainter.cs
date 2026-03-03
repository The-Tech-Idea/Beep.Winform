using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Zero-decoration minimal notification painter.
    /// <list type="bullet">
    ///   <item>Fully transparent background, no border</item>
    ///   <item>No icon, no progress, no close button decoration</item>
    ///   <item>Title + message only, maximum readability</item>
    ///   <item>Useful as an overlay HUD or status text</item>
    /// </list>
    /// </summary>
    public sealed class MinimalNotificationPainter : NotificationPainterBase
    {
        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            // No background — fully transparent presentation
            // (form's TransparencyKey or alpha-blending is set by the manager)
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            // Icon suppressed in full-minimal mode unless explicitly requested
            if (string.IsNullOrEmpty(data.IconPath)) return;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawIcon(g, iconRect, data.IconPath, colors.IconColor, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            // High contrast text: dark on light, white on dark (infer from back colour)
            Color textColor = data.CustomForeColor ?? Color.FromArgb(30, 30, 30);
            DrawTitle(g, rect, title, textColor);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            Color textColor = data.CustomForeColor.HasValue
                ? Color.FromArgb(190, data.CustomForeColor.Value)
                : Color.FromArgb(80, 80, 80);
            DrawMessage(g, rect, message, textColor);
        }

        // Progress bar: single line, no decoration
        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(Color.FromArgb(80, 80, 80)))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
