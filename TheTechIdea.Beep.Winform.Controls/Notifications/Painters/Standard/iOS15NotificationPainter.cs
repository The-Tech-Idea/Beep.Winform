using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Apple iOS 15 / HIG notification painter.
    /// <list type="bullet">
    ///   <item>Frosted-glass look: near-white fill with high alpha, blurred feel</item>
    ///   <item>Minimal border: light gray @ 30 % alpha</item>
    ///   <item>Large pill corners: 16 dp</item>
    ///   <item>Icon: flat rounded square (no circle) with type fill @ 20 %</item>
    ///   <item>App name/source rendered above title in caption font</item>
    ///   <item>Thin separator line below source</item>
    /// </list>
    /// </summary>
    public sealed class iOS15NotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 16;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Frosted glass — very light fill, very soft border
            Color back   = Color.FromArgb(240, 248, 248, 252);
            Color border = Color.FromArgb(76, 200, 200, 210);
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);

            // Rounded square badge (iOS style) instead of circle
            int r = S(8);
            using var path = CreateRoundedPath(iconRect, r);
            using (var bg = new SolidBrush(Color.FromArgb(50, colors.IconColor)))
                g.FillPath(bg, path);

            int inset = S(4);
            var inner = new Rectangle(iconRect.X + inset, iconRect.Y + inset,
                iconRect.Width - inset * 2, iconRect.Height - inset * 2);
            DrawIcon(g, inner, ip, colors.IconColor, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            // If there's a source, render it above in caption font then adjust rect
            if (!string.IsNullOrEmpty(data.Source))
            {
                int capH = S(14);
                var srcRect = new Rectangle(rect.X, rect.Y, rect.Width, capH);
                Font capFont = CaptionFont ?? SystemFonts.DefaultFont;
                TextRenderer.DrawText(g, data.Source.ToUpperInvariant(), capFont, srcRect,
                    Color.FromArgb(150, 60, 60, 80),
                    TextFormatFlags.Left | TextFormatFlags.Top);

                // Separator
                int sepY = srcRect.Bottom + S(2);
                using var pen = new Pen(Color.FromArgb(40, 100, 100, 120), 1);
                g.DrawLine(pen, rect.X, sepY, rect.Right, sepY);

                // Shift title below
                rect = new Rectangle(rect.X, sepY + S(3), rect.Width, rect.Height - capH - S(5));
            }
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawTitle(g, rect, title, Color.FromArgb(30, 30, 35));
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(80, 80, 90));

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            // Thin pill-shaped progress bar, type colour fill
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int r = rect.Height / 2;
            using (var bg = new SolidBrush(Color.FromArgb(40, colors.IconColor)))
            using (var bgPath = CreateRoundedPath(rect, r))
                g.FillPath(bg, bgPath);

            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
            {
                var fillRect = new Rectangle(rect.X, rect.Y, pw, rect.Height);
                using var fp = CreateRoundedPath(fillRect, r);
                using var fill = new SolidBrush(colors.IconColor);
                g.FillPath(fill, fp);
            }
        }
    }
}
