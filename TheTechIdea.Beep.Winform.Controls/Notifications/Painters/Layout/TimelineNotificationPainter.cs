using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Timeline entry notification painter.
    /// <list type="bullet">
    ///   <item>Vertical stripe on left (4 dp) representing the timeline track</item>
    ///   <item>Circle node on the stripe at midpoint (12 dp, type colour)</item>
    ///   <item>Right-side content: title, message, timestamp</item>
    ///   <item>No outer border; very transparent background</item>
    /// </list>
    /// </summary>
    public sealed class TimelineNotificationPainter : NotificationPainterBase
    {
        private const int TrackWidth = 4;
        private const int NodeSize   = 12;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            // Very faint tinted background, no border
            using var bg = new SolidBrush(Color.FromArgb(10, colors.IconColor));
            g.FillRectangle(bg, bounds);

            // Left timeline track
            int trackX = bounds.X + S(8);
            int trackW = S(TrackWidth);
            using var trackBrush = new SolidBrush(Color.FromArgb(80, colors.IconColor));
            g.FillRectangle(trackBrush, trackX, bounds.Y, trackW, bounds.Height);

            // Circle node at vertical centre
            int nodeS = S(NodeSize);
            int nodeX = trackX + (trackW - nodeS) / 2;
            int nodeY = bounds.Y + (bounds.Height - nodeS) / 2;
            using var nodeBrush = new SolidBrush(colors.IconColor);
            g.FillEllipse(nodeBrush, nodeX, nodeY, nodeS, nodeS);

            // White dot inside
            int innerS = nodeS / 2;
            int innerX = nodeX + (nodeS - innerS) / 2;
            int innerY = nodeY + (nodeS - innerS) / 2;
            using var innerBrush = new SolidBrush(Color.White);
            g.FillEllipse(innerBrush, innerX, innerY, innerS, innerS);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data) { }
        // Timeline uses the node dot — no separate icon

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
