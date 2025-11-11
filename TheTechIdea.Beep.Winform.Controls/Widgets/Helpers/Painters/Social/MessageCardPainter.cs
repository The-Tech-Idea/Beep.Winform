using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// MessageCard - Message display card painter
    /// </summary>
    internal sealed class MessageCardPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Header area for message title/sender
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Message content area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3 - 20
            );
            
            // Footer for timestamp/actions
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Bottom - pad - 20,
                ctx.DrawingRect.Width - pad * 2,
                20
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 2, offset: 1);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            
            // Draw accent border
            using var accentPen = new Pen(Color.FromArgb(40, ctx.AccentColor), 2);
            using var borderPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.DrawPath(accentPen, borderPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            string userName = ctx.UserName ?? "User";
            
            // Draw message header (sender)
            using var headerFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var headerBrush = new SolidBrush(ctx.AccentColor);
            g.DrawString($"Message from {userName}", headerFont, headerBrush, ctx.HeaderRect);
            
            // Draw message content
            string messageContent = ctx.Value ?? "Dear Client,\n\nJoin our online community. It helps\nin managing your projects.";
            using var contentFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var contentBrush = new SolidBrush(Color.FromArgb(160, Color.Black));
            g.DrawString(messageContent, contentFont, contentBrush, ctx.ContentRect);
            
            // Draw timestamp
            using var footerFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var footerBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            var footerFormat = new StringFormat { Alignment = StringAlignment.Far };
            g.DrawString("2 hours ago", footerFont, footerBrush, ctx.FooterRect, footerFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw message status indicators
        }
    }
}