using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ServiceCardPainter - For services, features, and benefits display
    /// Icon-centric layout with title, description, and call-to-action
    /// </summary>
    internal sealed class ServiceCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Large centered icon at top
            if (ctx.ShowImage)
            {
                int iconSize = 56;
                ctx.ImageRect = new Rectangle(
                    ctx.DrawingRect.Left + (ctx.DrawingRect.Width - iconSize) / 2,
                    ctx.DrawingRect.Top + pad,
                    iconSize, iconSize);
            }

            // Service/feature title (centered)
            int titleTop = ctx.ShowImage ? ctx.ImageRect.Bottom + 12 : ctx.DrawingRect.Top + pad;
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, titleTop, 
                ctx.DrawingRect.Width - pad * 2, HeaderHeight + 4);

            // Service category or status badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                int badgeWidth = 90;
                ctx.BadgeRect = new Rectangle(
                    ctx.DrawingRect.Left + (ctx.DrawingRect.Width - badgeWidth) / 2,
                    ctx.HeaderRect.Bottom + 6,
                    badgeWidth, 20);
            }

            // Service description
            int descTop = ctx.HeaderRect.Bottom + (string.IsNullOrEmpty(ctx.BadgeText1) ? 8 : 32);
            int descHeight = Math.Max(40, ctx.DrawingRect.Height - (descTop - ctx.DrawingRect.Top) - pad * 2 - (ctx.ShowButton ? ButtonHeight + 12 : 0));
            ctx.ParagraphRect = new Rectangle(ctx.DrawingRect.Left + pad, descTop, 
                ctx.DrawingRect.Width - pad * 2, descHeight);

            // Call-to-action button (Learn More, Get Started, etc.)
            if (ctx.ShowButton)
            {
                int buttonWidth = Math.Min(ctx.DrawingRect.Width - pad * 2, 140);
                ctx.ButtonRect = new Rectangle(
                    ctx.DrawingRect.Left + (ctx.DrawingRect.Width - buttonWidth) / 2,
                    ctx.DrawingRect.Bottom - pad - ButtonHeight,
                    buttonWidth, ButtonHeight);
            }

            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw icon background circle (Material Design Style)
            if (ctx.ShowImage)
            {
                // Draw circular background for icon with accent color
                using var circleBrush = new SolidBrush(Color.FromArgb(25, ctx.AccentColor));
                int circleSize = ctx.ImageRect.Width + 16;
                var circleRect = new Rectangle(
                    ctx.ImageRect.Left - 8,
                    ctx.ImageRect.Top - 8,
                    circleSize, circleSize);
                g.FillEllipse(circleBrush, circleRect);
                
                // Draw subtle border
                using var circlePen = new Pen(Color.FromArgb(50, ctx.AccentColor), 2);
                g.DrawEllipse(circlePen, circleRect);
            }

            // Draw category/status badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                using var badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, badgeFont);
            }

            // Draw decorative accent line above button
            if (ctx.ShowButton)
            {
                int lineWidth = 40;
                int lineY = ctx.ButtonRect.Top - 12;
                using var accentPen = new Pen(ctx.AccentColor, 3);
                g.DrawLine(accentPen,
                    ctx.DrawingRect.Left + (ctx.DrawingRect.Width - lineWidth) / 2, lineY,
                    ctx.DrawingRect.Left + (ctx.DrawingRect.Width + lineWidth) / 2, lineY);
            }
        }
    }
}
