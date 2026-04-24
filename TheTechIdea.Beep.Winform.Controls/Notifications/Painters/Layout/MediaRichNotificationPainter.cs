using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Media-Rich notification painter — large embedded image above text.
    /// <list type="bullet">
    ///   <item>Top image zone (40 % of height, rounded top corners)</item>
    ///   <item>White body below image</item>
    ///   <item>Small icon in bottom-left of image layer — overlapping seam</item>
    ///   <item>12 dp corners</item>
    /// </list>
    /// </summary>
    public sealed class MediaRichNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 12;
        private const int ImageHeightPct = 40; // percentage of total height

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            Color backColor = data.CustomBackColor ?? colors.BackColor;
            Color borderColor = colors.BorderColor;
            DrawBackground(g, bounds, backColor, borderColor, radius);

            // Image zone background (type-colour tinted, top only)
            int imgH = bounds.Height * ImageHeightPct / 100;
            var imgRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, imgH + radius);
            using var clipPath = CreateRoundedPath(imgRect, radius);
            g.SetClip(clipPath);

            if (!string.IsNullOrEmpty(data.EmbeddedImagePath))
            {
                DrawBodyImage(g, new Rectangle(bounds.X, bounds.Y, bounds.Width, imgH + radius),
                    data.EmbeddedImagePath, radius);
            }
            else
            {
                // Placeholder gradient
                using var gradBrush = new LinearGradientBrush(
                    new PointF(bounds.X, bounds.Y), new PointF(bounds.X, bounds.Y + imgH),
                    Color.FromArgb(80, colors.IconColor),
                    Color.FromArgb(20, colors.IconColor));
                g.FillPath(gradBrush, clipPath);
            }
            g.ResetClip();
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawCircleBadgeIcon(g, iconRect, ip, colors.BackColor, colors.IconColor);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawTitle(g, rect, title, colors.ForeColor);
        }

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            Color msgColor = Color.FromArgb(185, colors.ForeColor);
            DrawMessage(g, rect, message, msgColor);
        }

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(229, 231, 235)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
