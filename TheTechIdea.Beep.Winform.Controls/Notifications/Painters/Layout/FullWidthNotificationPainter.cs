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
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            Color iconColor = GetContrastColor(colors.IconColor);
            DrawIcon(g, iconRect, ip, iconColor, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Font f = TitleFont ?? SystemFonts.DefaultFont;
            Color titleColor = GetContrastColor(colors.IconColor);
            TextRenderer.DrawText(g, title, f, rect, titleColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Font f = MessageFont ?? SystemFonts.DefaultFont;
            Color titleColor = GetContrastColor(colors.IconColor);
            Color msgColor = Color.FromArgb(200, titleColor);
            TextRenderer.DrawText(g, message, f, rect, msgColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        public override void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data)
        {
            if (actionsRect.IsEmpty || actions == null || actions.Length == 0) return;

            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int btnSpacing = S(8);
            int btnWidth   = (actionsRect.Width - btnSpacing * (actions.Length - 1)) / actions.Length;
            int x          = actionsRect.X;
            Font f         = ButtonFont ?? SystemFonts.DefaultFont;
            Color textColor = GetContrastColor(colors.IconColor);

            foreach (var action in actions)
            {
                var btnRect = new Rectangle(x, actionsRect.Y, btnWidth, actionsRect.Height);
                using var pen = new Pen(Color.FromArgb(180, textColor), 1f);
                using var path = CreateRoundedPath(btnRect, S(4));
                g.DrawPath(pen, path);

                Color c = action.IsPrimary
                    ? textColor
                    : Color.FromArgb(200, textColor);
                TextRenderer.DrawText(g, action.Text, f, btnRect, c,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                x += btnWidth + btnSpacing;
            }
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Color barColor = GetContrastColor(colors.IconColor);
            using (var bg = new SolidBrush(Color.FromArgb(60, barColor)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(barColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }

        private static Color GetContrastColor(Color background)
        {
            float luminance = (0.299f * background.R + 0.587f * background.G + 0.114f * background.B) / 255f;
            return luminance > 0.5f ? Color.FromArgb(28, 27, 31) : Color.White;
        }
    }
}
