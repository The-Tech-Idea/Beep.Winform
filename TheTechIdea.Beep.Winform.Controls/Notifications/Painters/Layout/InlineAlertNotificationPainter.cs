using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Inline alert painter — renders inside a form body, not floating.
    /// Shares Ant Design DNA but with 0 dp corners to blend with form layout.
    /// <list type="bullet">
    ///   <item>Light type-tinted fill, 4 dp left stripe</item>
    ///   <item>0 dp corner radius (full-width inline)</item>
    ///   <item>Flat icon</item>
    ///   <item>Title bold + message below</item>
    /// </list>
    /// </summary>
    public sealed class InlineAlertNotificationPainter : NotificationPainterBase
    {
        private const int StripeWidth = 4;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride; // default 0 for inline

            Color back   = Color.FromArgb(18, colors.IconColor);
            Color border = Color.FromArgb(60, colors.BorderColor);
            DrawBackground(g, bounds, back, border, radius);

            // Left stripe
            int sw = S(StripeWidth);
            using var sb = new SolidBrush(colors.IconColor);
            g.FillRectangle(sb, bounds.X, bounds.Y, sw, bounds.Height);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawIcon(g, iconRect, ip, colors.IconColor, 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawTitle(g, rect, title, colors.ForeColor);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawMessage(g, rect, message, Color.FromArgb(185, colors.ForeColor));
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(30, colors.IconColor)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
