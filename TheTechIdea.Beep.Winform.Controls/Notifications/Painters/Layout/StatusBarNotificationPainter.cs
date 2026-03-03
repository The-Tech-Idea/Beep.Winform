using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Status bar notification painter — very thin, full-width single line.
    /// <list type="bullet">
    ///   <item>No border, no rounded corners</item>
    ///   <item>Type-colour 8 dp dot indicator on far left</item>
    ///   <item>Message text only (no title, no icon-badge)</item>
    ///   <item>Timestamp right-aligned in caption font</item>
    ///   <item>Very light type-tinted background</item>
    /// </list>
    /// </summary>
    public sealed class StatusBarNotificationPainter : NotificationPainterBase
    {
        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using var bg = new SolidBrush(Color.FromArgb(18, colors.IconColor));
            g.FillRectangle(bg, bounds);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            // Only the dot — iconRect from layout represents the dot area
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawTypeDot(g, iconRect, colors.IconColor);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data) { }
        // Status bar has no title

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            // Timestamp right-aligned if present
            if (data.Timestamp != default)
            {
                int tsW = S(50);
                var tsRect = new Rectangle(rect.Right - tsW, rect.Y, tsW, rect.Height);
                Font cf = CaptionFont ?? SystemFonts.DefaultFont;
                TextRenderer.DrawText(g, data.Timestamp.ToString("HH:mm"), cf, tsRect,
                    Color.FromArgb(130, 60, 60, 70),
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                rect = new Rectangle(rect.X, rect.Y, rect.Width - tsW - S(4), rect.Height);
            }
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawMessage(g, rect, message, colors.ForeColor);
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data) { }
    }
}
