using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Microsoft Fluent Design 2 notification painter.
    /// <list type="bullet">
    ///   <item>Layered acrylic: near-white fill @ 93 % with subtle noise texture (approximated)</item>
    ///   <item>Very faint 1 dp border in light gray</item>
    ///   <item>8 dp corner radius</item>
    ///   <item>Icon in soft-rounded square badge (fluent style)</item>
    ///   <item>Bottom-only thin accent line (type colour)</item>
    /// </list>
    /// </summary>
    public sealed class Fluent2NotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 8;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Acrylic-ish near-white surface
            Color back   = Color.FromArgb(237, 237, 237, 237);
            Color border = Color.FromArgb(38, 0, 0, 0);
            DrawBackground(g, bounds, back, border, radius);

            // Bottom accent line
            int lineH = S(3);
            var lineRect = new Rectangle(bounds.X, bounds.Bottom - lineH, bounds.Width, lineH);
            int clipR = Math.Min(radius, lineH);
            using var lp = CreateRoundedPath(lineRect, clipR);
            using var lb = new SolidBrush(Color.FromArgb(200, colors.IconColor));
            g.FillPath(lb, lp);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);

            // Fluent soft-rounded square
            int r = S(6);
            using var path = CreateRoundedPath(iconRect, r);
            using (var bg = new SolidBrush(Color.FromArgb(45, colors.IconColor)))
                g.FillPath(bg, path);

            int inset = S(4);
            var inner = new Rectangle(iconRect.X + inset, iconRect.Y + inset,
                iconRect.Width - inset * 2, iconRect.Height - inset * 2);
            DrawIcon(g, inner, ip, colors.IconColor, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, Color.FromArgb(32, 31, 30));

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(130, 124, 119));

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(40, colors.IconColor)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
