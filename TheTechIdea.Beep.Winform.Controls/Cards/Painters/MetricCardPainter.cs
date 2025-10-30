using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// MetricCardPainter - For displaying metrics, charts, and activity data
    /// Optimized for data visualization and KPI displays
    /// </summary>
    internal sealed class MetricCardPainter : CardPainterBase
    {
        private Font _valueFont;
        private Font _trendFont;
        private Font _badgeFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _valueFont?.Dispose(); } catch { }
            try { _trendFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            _valueFont = new Font(Owner.Font.FontFamily, 22f, FontStyle.Bold);
            _trendFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            _badgeFont = new Font(Owner.Font.FontFamily, 7.5f, FontStyle.Bold);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Icon or small chart thumbnail
            if (ctx.ShowImage)
            {
                int iconSize = 36;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, iconSize, iconSize);
            }

            // Metric name/title
            int headerLeft = ctx.DrawingRect.Left + pad + (ctx.ShowImage ? 46 : 0);
            int headerWidth = ctx.DrawingRect.Width - pad * 2 - (ctx.ShowImage ? 46 : 0);
            ctx.HeaderRect = new Rectangle(headerLeft, ctx.DrawingRect.Top + pad, headerWidth, 18);

            // Large metric value
            int valueTop = ctx.HeaderRect.Bottom + 6;
            ctx.SubtitleRect = new Rectangle(ctx.DrawingRect.Left + pad, valueTop, ctx.DrawingRect.Width - pad * 2, 36);

            // Trend indicator (percentage change, arrows)
            ctx.RatingRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.SubtitleRect.Bottom + 4, 120, 16);

            // Secondary metric or comparison value
            ctx.ParagraphRect = new Rectangle(ctx.RatingRect.Right + 8, ctx.SubtitleRect.Bottom + 4, 
                Math.Max(60, ctx.DrawingRect.Right - pad - (ctx.RatingRect.Right + 8)), 16);

            // Badge for metric category or status
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Right - pad - 70, ctx.DrawingRect.Top + pad, 65, 18);
            }

            // Status bar for progress or threshold indicator
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Bottom - 6, ctx.DrawingRect.Width, 6);
            }

            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw large metric value with emphasis
            if (!string.IsNullOrEmpty(ctx.SubtitleText))
            {
                var valueBrush = PaintersFactory.GetSolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _valueFont, valueBrush, ctx.SubtitleRect, format);
            }

            // Draw trend indicator with color coding (up/down arrows, percentages)
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                // Determine trend color based on prefix
                Color trendColor = Color.Gray;
                string displayText = ctx.StatusText;
                
                if (ctx.StatusText.StartsWith("+") || ctx.StatusText.Contains("↑"))
                {
                    trendColor = Color.FromArgb(76, 175, 80); // Material Green
                }
                else if (ctx.StatusText.StartsWith("-") || ctx.StatusText.Contains("↓"))
                {
                    trendColor = Color.FromArgb(244, 67, 54); // Material Red
                }

                var trendBrush = PaintersFactory.GetSolidBrush(trendColor);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                
                // Draw trend with background pill
                var trendBounds = new Rectangle(ctx.RatingRect.X, ctx.RatingRect.Y, Math.Min(100, ctx.RatingRect.Width), ctx.RatingRect.Height);
                var pillBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(30, trendColor));
                using var path = CreateRoundedPath(trendBounds, 8);
                g.FillPath(pillBrush, path);
                g.DrawString(displayText, _trendFont, trendBrush, trendBounds, format);
            }

            // Draw category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }

            // Draw progress/threshold status bar
            if (ctx.ShowStatus)
            {
                var statusBrush = PaintersFactory.GetSolidBrush(ctx.StatusColor);
                g.FillRectangle(statusBrush, ctx.StatusRect);
            }
        }

        private GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
