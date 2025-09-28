using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// ChatWidget - Chat interface painter
    /// </summary>
    internal sealed class ChatWidgetPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Title area
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            }
            
            // Chat content area
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad - 30 // Reserve space for input area
            );
            
            // Chat input area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Bottom - pad - 30,
                ctx.DrawingRect.Width - pad * 2,
                30
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            // Draw placeholder content for chat messages
            using var contentFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var contentBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            g.DrawString("Chat widget implementation pending...", contentFont, contentBrush, ctx.ContentRect);
            
            // Draw input area placeholder
            if (!ctx.FooterRect.IsEmpty)
            {
                using var inputBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
                using var inputPath = CreateRoundedPath(ctx.FooterRect, 4);
                g.FillPath(inputBrush, inputPath);
                
                using var placeholderFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Italic);
                using var placeholderBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString("Type a message...", placeholderFont, placeholderBrush, ctx.FooterRect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw message status indicators
        }
    }
}