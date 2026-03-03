using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Material Snackbar painter — single line, dark background, optional right action link.
    /// <list type="bullet">
    ///   <item>Dark surface: #323232 (Material Snackbar colour)</item>
    ///   <item>No title, no icon, no progress bar — message only</item>
    ///   <item>Right-aligned action link in type accent colour</item>
    ///   <item>Pill-shaped: radius = height / 2</item>
    /// </list>
    /// </summary>
    public sealed class SnackbarNotificationPainter : NotificationPainterBase
    {
        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = bounds.Height / 2;
            Color back = data.CustomBackColor ?? Color.FromArgb(50, 50, 50);
            DrawBackground(g, bounds, back, Color.Transparent, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data) { }
        // Snackbar has no icon

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data) { }
        // Snackbar has no title

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, data.CustomForeColor ?? Color.FromArgb(245, 245, 245));

        public override void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data)
        {
            if (actionsRect.IsEmpty || actions == null || actions.Length == 0) return;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Font f = ButtonFont ?? SystemFonts.DefaultFont;

            // Snackbar: single primary action only
            var action = actions[0];
            Color c = action.CustomColor ?? colors.IconColor;
            TextRenderer.DrawText(g, action.Text.ToUpperInvariant(), f, actionsRect, c,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data) { }
        // Snackbar uses duration timer only
    }
}
