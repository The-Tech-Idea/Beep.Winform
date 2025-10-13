using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// BasicCard - Minimal card for general content
    /// </summary>
    internal sealed class BasicCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, HeaderHeight);
            int paragraphHeight = Math.Max(0, ctx.DrawingRect.Height - HeaderHeight - ButtonHeight - (pad * 3));
            ctx.ParagraphRect = new Rectangle(ctx.HeaderRect.Left, ctx.HeaderRect.Bottom + 6, ctx.HeaderRect.Width, paragraphHeight);

            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Right - pad - 100, ctx.DrawingRect.Bottom - pad - ButtonHeight, 95, ButtonHeight);
            }
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Do not draw container background/shadow here; BaseControl handles it.
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Minimal accent line at top
            using var accentPen = new Pen(ctx.AccentColor, 2);
            g.DrawLine(accentPen, 
                ctx.DrawingRect.Left + 16, ctx.DrawingRect.Top + 12,
                ctx.DrawingRect.Left + 56, ctx.DrawingRect.Top + 12);
        }
    }
}
