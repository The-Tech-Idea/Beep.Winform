using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// FeatureCard - Icon + title + description (like app features)
    /// </summary>
    internal sealed class FeatureCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = Inset(drawingRect, 8);

            // Icon centered at top
            int iconSize = 64;
            var iconArea = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, iconSize);
            ctx.ImageRect = AlignToContent(iconArea, new Size(iconSize, iconSize), ContentAlignment.MiddleCenter);

            var content = new Rectangle(ctx.DrawingRect.Left + pad, ctx.ImageRect.Bottom + pad, ctx.DrawingRect.Width - pad * 2, Math.Max(0, ctx.DrawingRect.Bottom - (ctx.ImageRect.Bottom + pad)));

            ctx.HeaderRect = new Rectangle(content.Left, content.Top, content.Width, HeaderHeight);
            ctx.ParagraphRect = new Rectangle(content.Left, ctx.HeaderRect.Bottom + 6, content.Width, Math.Max(0, content.Height - HeaderHeight - pad));

            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Optional accent circle behind icon
            using var accentBrush = new SolidBrush(Color.FromArgb(15, ctx.AccentColor));
            var accentCircle = new Rectangle(
                ctx.ImageRect.X - 10,
                ctx.ImageRect.Y - 10,
                ctx.ImageRect.Width + 20,
                ctx.ImageRect.Height + 20
            );
            g.FillEllipse(accentBrush, accentCircle);
        }
    }
}