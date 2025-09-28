using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// StatCard - For displaying key metrics, KPIs, and statistics
    /// </summary>
    internal sealed class StatCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = 20;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            
            // Icon in top-left
            if (ctx.ShowImage)
            {
                int iconSize = 32;
                ctx.ImageRect = new Rectangle(
                    ctx.DrawingRect.Left + pad,
                    ctx.DrawingRect.Top + pad,
                    iconSize, iconSize
                );
            }
            
            // Main value (large number) - center focus
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad + (ctx.ShowImage ? 40 : 0),
                ctx.DrawingRect.Width - pad * 2,
                36
            );
            
            // Label/description below value
            ctx.ParagraphRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                20
            );
            
            // Trend indicator (percentage change) in bottom-right
            ctx.SubtitleRect = new Rectangle(
                ctx.DrawingRect.Right - pad - 80,
                ctx.DrawingRect.Bottom - pad - 18,
                75, 16
            );
            
            // Status indicator (optional)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    ctx.DrawingRect.Left,  // Full width accent line
                    ctx.DrawingRect.Bottom - 4,
                    ctx.DrawingRect.Width,
                    4
                );
            }
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 10, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 10);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw main value with large, bold font
            if (!string.IsNullOrEmpty(ctx.SubtitleText)) // Main stat value in SubtitleText
            {
                using var valueFont = new Font(Owner.Font.FontFamily, 24f, FontStyle.Bold);
                using var valueBrush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, valueFont, valueBrush, ctx.HeaderRect, format);
            }
            
            // Draw trend indicator with color coding
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                Color trendColor = ctx.StatusText.StartsWith("+") ? Color.Green : 
                                   ctx.StatusText.StartsWith("-") ? Color.Red : Color.Gray;
                using var trendFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var trendBrush = new SolidBrush(trendColor);
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, trendFont, trendBrush, ctx.SubtitleRect, format);
            }
            
            // Draw status accent line at bottom
            if (ctx.ShowStatus)
            {
                using var statusBrush = new SolidBrush(ctx.StatusColor);
                g.FillRectangle(statusBrush, ctx.StatusRect);
            }
        }
    }
}