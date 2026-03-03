using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Stripe Dashboard notification painter.
    /// <list type="bullet">
    ///   <item>Clean white card, subtle gray-200 border, 8 dp corners</item>
    ///   <item>Coloured dot indicator (10 dp circle) left of title — type colour</item>
    ///   <item>No icon badge — just the dot</item>
    ///   <item>Title in Stripe gray-900; message in gray-500</item>
    ///   <item>Stripe-style action link (text only, blue)</item>
    /// </list>
    /// </summary>
    public sealed class StripeDashboardNotificationPainter : NotificationPainterBase
    {
        private const int DefaultRadius = 8;
        private const int DotSize       = 10;

        public override void PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = data.CornerRadiusOverride > 0 ? data.CornerRadiusOverride : S(DefaultRadius);
            Color back   = data.CustomBackColor ?? Color.White;
            Color border = Color.FromArgb(229, 231, 235);
            DrawBackground(g, bounds, back, border, radius);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            // Stripe uses a plain colour dot, not an icon image
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            int ds = S(DotSize);
            var dotRect = new Rectangle(
                iconRect.X + (iconRect.Width  - ds) / 2,
                iconRect.Y + (iconRect.Height - ds) / 2,
                ds, ds);
            DrawTypeDot(g, dotRect, colors.IconColor);
        }

        public override void PaintTitle(Graphics g, Rectangle rect, string title, NotificationData data)
            => DrawTitle(g, rect, title, Color.FromArgb(15, 23, 42));      // Stripe slate-900

        public override void PaintMessage(Graphics g, Rectangle rect, string message, NotificationData data)
            => DrawMessage(g, rect, message, Color.FromArgb(100, 116, 139)); // slate-500

        public override void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data)
        {
            if (actionsRect.IsEmpty || actions == null) return;
            // Stripe link style: blue text, no border
            int btnSpacing = S(12);
            int btnW  = (actionsRect.Width - btnSpacing * (actions.Length - 1)) / actions.Length;
            int x     = actionsRect.X;
            Font f    = ButtonFont ?? SystemFonts.DefaultFont;
            Color primary   = Color.FromArgb(99, 102, 241);  // Stripe indigo
            Color secondary = Color.FromArgb(148, 163, 184);  // slate-400

            foreach (var action in actions)
            {
                var btnRect = new Rectangle(x, actionsRect.Y, btnW, actionsRect.Height);
                TextRenderer.DrawText(g, action.Text, f, btnRect,
                    action.IsPrimary ? primary : secondary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                x += btnW + btnSpacing;
            }
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
