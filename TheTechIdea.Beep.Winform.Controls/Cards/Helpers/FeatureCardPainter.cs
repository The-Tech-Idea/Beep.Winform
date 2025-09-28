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
            int pad = 20;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Icon at top center
            int iconSize = 64;
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left + (ctx.DrawingRect.Width - iconSize) / 2,
                ctx.DrawingRect.Top + pad,
                iconSize, iconSize
            );
            
            // Title below icon
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ImageRect.Bottom + 16,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            // Description below title
            ctx.ParagraphRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - (ctx.HeaderRect.Bottom + 8 - ctx.DrawingRect.Top) - pad * 2
            );
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 16, layers: 5, offset: 3);
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 16);
            g.FillPath(bgBrush, bgPath);
        }

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