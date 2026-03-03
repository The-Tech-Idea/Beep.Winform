using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Full-Width notification painter — spans the entire parent container width.
    /// Behaves like a horizontal announcement bar (think GitHub/GitLab warning banners).
    /// <list type="bullet">
    ///   <item>No border, no rounded corners</item>
    ///   <item>Solid type-colour background (or tinted)</item>
    ///   <item>Icon flat, white tint</item>
    ///   <item>Title white, message slightly muted white</item>
    ///   <item>Actions as inline white-bordered buttons on right</item>
    /// </list>
    /// </summary>
    public sealed class FullWidthNotificationPainter : NotificationPainterBase
    {
        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            // Solid fill — no rounding, full-width bar
            Color back = data.CustomBackColor ?? colors.IconColor;
            g.FillRectangle(new SolidBrush(back), bounds);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            // White flat icon on solid background
            string ip = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawIcon(g, iconRect, ip, Color.White, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            // White on coloured background
            Font f = TitleFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, title, f, rect, Color.White,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            Font f = MessageFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, message, f, rect, Color.FromArgb(220, 255, 255, 255),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        public override void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data)
        {
            if (actionsRect.IsEmpty || actions == null || actions.Length == 0) return;

            int btnSpacing = S(8);
            int btnWidth   = (actionsRect.Width - btnSpacing * (actions.Length - 1)) / actions.Length;
            int x          = actionsRect.X;
            Font f         = ButtonFont ?? SystemFonts.DefaultFont;

            foreach (var action in actions)
            {
                var btnRect = new Rectangle(x, actionsRect.Y, btnWidth, actionsRect.Height);
                // White-outlined buttons on solid background
                using var pen = new Pen(Color.FromArgb(180, 255, 255, 255), 1f);
                using var path = CreateRoundedPath(btnRect, S(4));
                g.DrawPath(pen, path);

                Color c = action.IsPrimary
                    ? Color.White
                    : Color.FromArgb(200, 255, 255, 255);
                TextRenderer.DrawText(g, action.Text, f, btnRect, c,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                x += btnWidth + btnSpacing;
            }
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            // Subtle white bar on solid background
            using (var bg = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(Color.White))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
