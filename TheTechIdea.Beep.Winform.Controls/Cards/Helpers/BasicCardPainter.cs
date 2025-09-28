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
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            
            // Simple layout
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            ctx.ParagraphRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3 - 40
            );
            
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    ctx.DrawingRect.Right - pad - 100,
                    ctx.DrawingRect.Bottom - pad - 32,
                    95, 28
                );
            }
            
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 12);
            g.FillPath(bgBrush, bgPath);
        }

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