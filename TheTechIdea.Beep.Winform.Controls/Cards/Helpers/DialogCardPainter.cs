using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// DialogCard - Simple modal-style (like confirmation dialogs)
    /// </summary>
    internal sealed class DialogCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = 24;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Icon at top if present
            if (ctx.ShowImage)
            {
                int iconSize = 48;
                ctx.ImageRect = new Rectangle(
                    ctx.DrawingRect.Left + (ctx.DrawingRect.Width - iconSize) / 2,
                    ctx.DrawingRect.Top + pad,
                    iconSize, iconSize
                );
                
                ctx.HeaderRect = new Rectangle(
                    ctx.DrawingRect.Left + pad,
                    ctx.ImageRect.Bottom + 16,
                    ctx.DrawingRect.Width - pad * 2,
                    28
                );
            }
            else
            {
                ctx.HeaderRect = new Rectangle(
                    ctx.DrawingRect.Left + pad,
                    ctx.DrawingRect.Top + pad,
                    ctx.DrawingRect.Width - pad * 2,
                    28
                );
            }
            
            // Description
            ctx.ParagraphRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                60
            );
            
            // Buttons at bottom
            int buttonHeight = 36;
            int buttonWidth = 100;
            ctx.SecondaryButtonRect = new Rectangle(
                ctx.DrawingRect.Right - pad - buttonWidth * 2 - 12,
                ctx.DrawingRect.Bottom - pad - buttonHeight,
                buttonWidth, buttonHeight
            );
            
            ctx.ButtonRect = new Rectangle(
                ctx.DrawingRect.Right - pad - buttonWidth,
                ctx.DrawingRect.Bottom - pad - buttonHeight,
                buttonWidth, buttonHeight
            );
            
            ctx.ShowSecondaryButton = true;
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 20, layers: 8, offset: 6);
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 20);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Minimal accents for clean dialog appearance
        }
    }
}