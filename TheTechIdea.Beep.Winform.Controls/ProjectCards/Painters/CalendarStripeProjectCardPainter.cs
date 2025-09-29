using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    // Left calendar stripe with date and compact content at right (like calendar lanes in screenshot)
    internal sealed class CalendarStripeProjectCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.CalendarStripe);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var subtitle = GetString(p, BeepProjectCard.ParamSubtitle, owner.Subtitle);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            int daysLeft = (int)(p != null && p.TryGetValue(BeepProjectCard.ParamDaysLeft, out var v) && v is IConvertible ? Convert.ToInt32(v) : owner.DaysLeft);

            var inner = Inset(bounds, 10);

            // calendar stripe
            int stripeW = Math.Min(56, Math.Max(40, inner.Width / 6));
            var stripe = new Rectangle(inner.X, inner.Y, stripeW, inner.Height);
            using (var bg = new LinearGradientBrush(stripe, theme.PrimaryColor, Color.FromArgb(80, theme.PrimaryColor), LinearGradientMode.Vertical))
                g.FillRectangle(bg, stripe);

            // date text (days left)
            using var df = new Font("Segoe UI", 12f, FontStyle.Bold);
            using var sf = new Font("Segoe UI", 8f, FontStyle.Regular);
            var dayText = daysLeft > 0 ? daysLeft.ToString() : "0";
            var dayRect = new Rectangle(stripe.X, stripe.Y + 8, stripe.Width, 24);
            TextRenderer.DrawText(g, dayText, df, dayRect, Color.White, TextFormatFlags.HorizontalCenter);
            var lblRect = new Rectangle(stripe.X, dayRect.Bottom, stripe.Width, 16);
            TextRenderer.DrawText(g, "days", sf, lblRect, Color.White, TextFormatFlags.HorizontalCenter);

            // right content
            var content = new Rectangle(stripe.Right + 10, inner.Y, inner.Width - stripe.Width - 10, inner.Height);
            DrawTitle(g, new Rectangle(content.X, content.Y, content.Width, 24), theme, title);
            DrawSubtitle(g, new Rectangle(content.X, content.Y + 20, content.Width, 18), theme, subtitle);

            var bar = new Rectangle(content.X, content.Bottom - 10, content.Width, 6);
            using (var back = new SolidBrush(Color.FromArgb(40, theme.CardTextForeColor))) g.FillRectangle(back, bar);
            int w = (int)(bar.Width * (progress / 100f));
            using (var fore = new SolidBrush(theme.SecondaryColor.IsEmpty ? Color.MediumSeaGreen : theme.SecondaryColor)) g.FillRectangle(fore, new Rectangle(bar.X, bar.Y, w, bar.Height));
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            var inner = Inset(bounds, 10);
            int stripeW = Math.Min(56, Math.Max(40, inner.Width / 6));
            var stripe = new Rectangle(inner.X, inner.Y, stripeW, inner.Height);
            var content = new Rectangle(stripe.Right + 10, inner.Y, inner.Width - stripe.Width - 10, inner.Height);
            var bar = new Rectangle(content.X, content.Bottom - 10, content.Width, 6);
            register("Stripe", stripe);
            register("Content", content);
            register("ProgressBar", bar);
        }
    }
}
