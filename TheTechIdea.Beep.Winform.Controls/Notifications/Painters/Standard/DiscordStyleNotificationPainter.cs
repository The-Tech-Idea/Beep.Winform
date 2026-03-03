using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Painters;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Discord-style dark notification painter with left coloured sidebar strip.
    /// <list type="bullet">
    ///   <item>Dark surface: #36393F (Discord dark channel background)</item>
    ///   <item>4 dp left strip — type colour (Discord uses brand colours)</item>
    ///   <item>No visible outer border</item>
    ///   <item>4 dp corners</item>
    ///   <item>Icon: flat, slightly muted type colour</item>
    ///   <item>Title in #FFFFFF; message in Discord secondary text #B9BBBE</item>
    /// </list>
    /// </summary>
    public sealed class DiscordStyleNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 4;
        private const int StripWidth    = 4;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);

            // Discord dark surface
            Color back = data.CustomBackColor ?? Color.FromArgb(54, 57, 63);
            DrawBackground(g, bounds, back, Color.Transparent, radius);

            // Left accent strip
            int sw = S(StripWidth);
            using var sb = new SolidBrush(colors.IconColor);
            g.FillRectangle(sb, bounds.X, bounds.Y + radius, sw, bounds.Height - radius * 2);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath : NotificationData.GetDefaultIconForType(data.Type);
            DrawIcon(g, iconRect, ip, Color.FromArgb(190, colors.IconColor), 0);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, Color.FromArgb(255, 255, 255));

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(185, 187, 190));  // #B9BBBE

        public override void PaintProgressBar(Graphics g, Rectangle rect, float progress, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            using (var bg = new SolidBrush(Color.FromArgb(47, 49, 54)))
                g.FillRectangle(bg, rect);
            int pw = (int)(rect.Width * (progress / 100f));
            if (pw > 0)
                using (var fill = new SolidBrush(colors.IconColor))
                    g.FillRectangle(fill, rect.X, rect.Y, pw, rect.Height);
        }
    }
}
