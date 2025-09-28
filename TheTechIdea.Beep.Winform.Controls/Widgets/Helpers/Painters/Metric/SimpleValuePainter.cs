using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// SimpleValue - Just displays a number and label
    /// </summary>
    internal sealed class SimpleValuePainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Icon at top if shown
            if (ctx.ShowIcon)
            {
                int iconSize = 32;
                ctx.IconRect = new Rectangle(
                    ctx.DrawingRect.Left + pad,
                    ctx.DrawingRect.Top + pad,
                    iconSize, iconSize
                );
            }
            
            // Title below icon or at top
            int titleTop = ctx.ShowIcon ? ctx.IconRect.Bottom + 8 : ctx.DrawingRect.Top + pad;
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                titleTop,
                ctx.DrawingRect.Width - pad * 2,
                20
            );
            
            // Main value - center focus
            ctx.ValueRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            // Trend indicator below value if shown
            if (ctx.ShowTrend)
            {
                ctx.TrendRect = new Rectangle(
                    ctx.DrawingRect.Left + pad,
                    ctx.ValueRect.Bottom + 4,
                    ctx.DrawingRect.Width - pad * 2,
                    20
                );
            }
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var titleBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
            
            // Draw main value
            if (!string.IsNullOrEmpty(ctx.Value))
            {
                using var valueFont = new Font(Owner.Font.FontFamily, 18f, FontStyle.Bold);
                using var valueBrush = new SolidBrush(ctx.AccentColor);
                DrawValue(g, ctx.ValueRect, ctx.Value, ctx.Units, valueFont, valueBrush.Color);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw trend indicator
            if (ctx.ShowTrend && !string.IsNullOrEmpty(ctx.TrendValue))
            {
                // Draw trend arrow
                var trendArrowRect = new Rectangle(ctx.TrendRect.X, ctx.TrendRect.Y, 16, 16);
                DrawTrendArrow(g, trendArrowRect, ctx.TrendDirection, ctx.TrendColor);
                
                // Draw trend text
                var trendTextRect = new Rectangle(
                    trendArrowRect.Right + 4, 
                    ctx.TrendRect.Y, 
                    ctx.TrendRect.Width - 20, 
                    ctx.TrendRect.Height
                );
                using var trendFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var trendBrush = new SolidBrush(ctx.TrendColor);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.TrendValue, trendFont, trendBrush, trendTextRect, format);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Use BaseControl's hit area system like BeepAppBar does
            // No need for custom ClickableAreas list
            
            // Add custom hit areas if needed beyond what BeepMetricWidget already handles
            if (!ctx.HeaderRect.IsEmpty)
            {
                owner.AddHitArea("Title", ctx.HeaderRect, null, () => notifyAreaHit?.Invoke("Title", ctx.HeaderRect));
            }
            
            // The main Value and Trend areas are already handled by BeepMetricWidget.RefreshHitAreas()
            // This method is for painters to add additional custom interactive areas
        }
    }
}