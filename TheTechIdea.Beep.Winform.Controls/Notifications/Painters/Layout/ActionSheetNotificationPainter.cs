using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Action Sheet notification painter — bottom sheet style with stacked actions.
    /// <list type="bullet">
    ///   <item>Light surface, sharp or slightly rounded bottom, 16 dp top corners</item>
    ///   <item>Title centred at top</item>
    ///   <item>Actions rendered as full-width stacked buttons with dividers</item>
    ///   <item>Cancel action rendered in red / muted at bottom</item>
    /// </list>
    /// </summary>
    public sealed class ActionSheetNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 16;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Top rounded only — sheet style
            Color back   = data.CustomBackColor ?? Color.FromArgb(248, 248, 248);
            Color border = Color.FromArgb(51, 229, 231, 235);
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data) { }
        // Action sheet has no icon

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            // Centred title + optional subtitle
            Font f = TitleFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, title, f, rect,
                Color.FromArgb(60, 60, 67),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            Font f = MessageFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, message, f, rect,
                Color.FromArgb(128, 128, 128),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }

        public override void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data)
        {
            if (actionsRect.IsEmpty || actions == null || actions.Length == 0) return;

            int btnH   = actionsRect.Height / actions.Length;
            int y      = actionsRect.Y;
            Font f     = ButtonFont ?? SystemFonts.DefaultFont;

            foreach (var action in actions)
            {
                var btnRect = new Rectangle(actionsRect.X, y, actionsRect.Width, btnH);
                bool isLast = y + btnH >= actionsRect.Bottom;

                // Divider above each (except first if == title bottom)
                if (y > actionsRect.Y)
                {
                    using var divPen = new Pen(Color.FromArgb(229, 231, 235), 1);
                    g.DrawLine(divPen, actionsRect.X, y, actionsRect.Right, y);
                }

                bool hovered  = btnRect.Contains(Cursor.Position);
                if (hovered)
                {
                    using var hb = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
                    g.FillRectangle(hb, btnRect);
                }

                Color c = isLast && !action.IsPrimary
                    ? Color.FromArgb(255, 59, 48)   // cancel in iOS red
                    : action.IsPrimary
                        ? Color.FromArgb(0, 122, 255)  // iOS blue
                        : Color.FromArgb(60, 60, 67);

                TextRenderer.DrawText(g, action.Text, f, btnRect, c,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                y += btnH;
            }
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data) { }
    }
}
