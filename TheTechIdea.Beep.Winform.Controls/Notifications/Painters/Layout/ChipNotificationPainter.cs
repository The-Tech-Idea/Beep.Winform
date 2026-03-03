using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Chip notification painter — compact pill for inline tagging / brief status.
    /// <list type="bullet">
    ///   <item>Pill shape (radius = height / 2)</item>
    ///   <item>Tiny icon (14 dp) or dot on left</item>
    ///   <item>Single-line title only — centred</item>
    ///   <item>Light tinted fill, coloured border</item>
    /// </list>
    /// </summary>
    public sealed class ChipNotificationPainter : NotificationPainterBase
    {
        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = bounds.Height / 2;

            Color back   = Color.FromArgb(20, colors.IconColor);
            Color border = Color.FromArgb(120, colors.BorderColor);
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            // Chip uses a tiny flat icon — no badge
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawIcon(g, iconRect, ip, colors.IconColor, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Font f = CaptionFont ?? SystemFonts.DefaultFont; // Chip uses caption-size text
            TextRenderer.DrawText(g, title, f, rect, colors.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string msg, NotificationData data) { }
        // Chip has no message
        public override void PaintProgressBar(Graphics g, Rectangle rect, float p, NotificationData data) { }
        // No progress bar in chip
    }
}
