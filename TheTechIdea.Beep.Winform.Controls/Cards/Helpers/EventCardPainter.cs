using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// EventCard - For displaying events, appointments, or time-based content
    /// </summary>
    internal sealed class EventCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            
            // Colored accent bar on left edge
            ctx.StatusRect = new Rectangle(
                ctx.DrawingRect.Left,
                ctx.DrawingRect.Top,
                4,
                ctx.DrawingRect.Height
            );
            
            // Date/time block on left
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left + 12,
                ctx.DrawingRect.Top + pad,
                60, 50
            );
            
            // Event content to the right
            int contentLeft = ctx.ImageRect.Right + 12;
            int contentWidth = ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - pad;
            
            // Event title
            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, 20);
            
            // Time/location info
            ctx.SubtitleRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 4, contentWidth, 16);
            
            // Event description
            ctx.ParagraphRect = new Rectangle(contentLeft, ctx.SubtitleRect.Bottom + 6, contentWidth, 30);
            
            // Tags for event categories
            ctx.TagsRect = new Rectangle(contentLeft, ctx.ParagraphRect.Bottom + 8, contentWidth - 80, 20);
            
            // RSVP/Action button in bottom-right
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    ctx.DrawingRect.Right - pad - 70,
                    ctx.DrawingRect.Bottom - pad - 24,
                    65, 20
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
            // Draw accent bar
            using var accentBrush = new SolidBrush(ctx.AccentColor);
            var accentPath = CreateRoundedPath(ctx.StatusRect, 2);
            g.FillPath(accentBrush, accentPath);
            
            // Draw date/time block
            if (!string.IsNullOrEmpty(ctx.StatusText)) // Date in StatusText
            {
                using var dateBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
                var dateRect = new Rectangle(ctx.ImageRect.X, ctx.ImageRect.Y, ctx.ImageRect.Width, ctx.ImageRect.Height);
                using var datePath = CreateRoundedPath(dateRect, 6);
                g.FillPath(dateBrush, datePath);
                
                // Draw date text
                using var dateFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var textBrush = new SolidBrush(Color.White);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, dateFont, textBrush, dateRect, format);
            }
            
            // Draw event category tags
            CardRenderingHelpers.DrawChips(g, Owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags);
        }
    }
}