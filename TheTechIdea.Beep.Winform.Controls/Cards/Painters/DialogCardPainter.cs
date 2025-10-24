using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// DialogCard - Simple modal-Style (like confirmation dialogs)
    /// </summary>
    internal sealed class DialogCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad + 4;
            ctx.DrawingRect = drawingRect;

            int top = ctx.DrawingRect.Top + pad;
            if (ctx.ShowImage)
            {
                int icon = 48;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + (ctx.DrawingRect.Width - icon) / 2, top, icon, icon);
                top = ctx.ImageRect.Bottom + 16;
            }

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, top, ctx.DrawingRect.Width - pad * 2, HeaderHeight);
            ctx.ParagraphRect = new Rectangle(ctx.HeaderRect.Left, ctx.HeaderRect.Bottom + 12, ctx.HeaderRect.Width, 60);

            ctx.SecondaryButtonRect = new Rectangle(ctx.DrawingRect.Right - pad - 100 * 2 - 12, ctx.DrawingRect.Bottom - pad - ButtonHeight, 100, ButtonHeight);
            ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Right - pad - 100, ctx.DrawingRect.Bottom - pad - ButtonHeight, 100, ButtonHeight);

            ctx.ShowSecondaryButton = true;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Minimal accents for clean dialog appearance
        }
    }
}
