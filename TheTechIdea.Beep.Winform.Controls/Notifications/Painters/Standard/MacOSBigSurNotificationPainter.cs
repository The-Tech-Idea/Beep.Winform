using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// macOS Big Sur / Ventura notification sheet painter.
    /// <list type="bullet">
    ///   <item>Sheet-style light surface: Apple #F5F5F7 @ 96 %</item>
    ///   <item>Subtle #E0E0E0 border</item>
    ///   <item>12 dp corner radius</item>
    ///   <item>App icon (large 40dp left) + app name above title in caption font </item>
    ///   <item>Actions rendered as macOS-style link buttons (no border)</item>
    /// </list>
    /// </summary>
    public sealed class MacOSBigSurNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 12;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // macOS sheet surface
            Color back   = Color.FromArgb(245, 245, 245, 247);
            Color border = Color.FromArgb(230, 224, 224, 224);
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);

            // macOS uses large app icon with no background container
            DrawIcon(g, iconRect, ip, colors.IconColor, S(4));
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            // App name (source) above title
            if (!string.IsNullOrEmpty(data.Source))
            {
                int capH     = S(13);
                var srcRect  = new Rectangle(rect.X, rect.Y, rect.Width, capH);
                Font capFont = CaptionFont ?? SystemFonts.DefaultFont;
                TextRenderer.DrawText(g, data.Source, capFont, srcRect,
                    Color.FromArgb(160, 60, 60, 70),
                    TextFormatFlags.Left | TextFormatFlags.Top);
                rect = new Rectangle(rect.X, rect.Y + capH + S(1), rect.Width, rect.Height - capH - S(1));
            }
            DrawTitle(g, rect, title, Color.FromArgb(29, 29, 31));
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(100, 100, 108));

        public override void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data)
        {
            if (actionsRect.IsEmpty || actions == null) return;

            // macOS-style link buttons: text only, type colour, no border
            var colors  = GetColorsForType(data.Type, CreateRenderOptions(data));
            int btnSpacing = S(16);
            int btnW  = (actionsRect.Width - btnSpacing * (actions.Length - 1)) / actions.Length;
            int x     = actionsRect.X;
            Font f    = ButtonFont ?? SystemFonts.DefaultFont;

            foreach (var action in actions)
            {
                var btnRect = new Rectangle(x, actionsRect.Y, btnW, actionsRect.Height);
                Color btnColor = action.IsPrimary ? colors.IconColor : Color.FromArgb(140, 120, 120, 135);
                TextRenderer.DrawText(g, action.Text, f, btnRect, btnColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                x += btnW + btnSpacing;
            }
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(35, colors.IconColor)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
