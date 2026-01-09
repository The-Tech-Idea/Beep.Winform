using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Dashboard
{
    /// <summary>
    /// TimelineView - Chronological events display with enhanced visual presentation
    /// </summary>
    internal sealed class TimelineViewPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public TimelineViewPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad + 20, ctx.HeaderRect.Bottom + 12, ctx.DrawingRect.Width - pad * 2 - 20, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 16, layers: 5, offset: 3);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
            
            // Draw timeline
            var events = ctx.Metrics;
            if (events != null && events.Count > 0)
            {
                DrawTimeline(g, ctx.ContentRect, events, ctx.AccentColor);
            }
        }

        private void DrawTimeline(Graphics g, Rectangle rect, List<DashboardMetric> events, Color accentColor)
        {
            if (!events.Any()) return;
            
            // Timeline line
            int timelineX = rect.X - 10;
            using var timelinePen = new Pen(Color.FromArgb(100, Color.Gray), 2);
            g.DrawLine(timelinePen, timelineX, rect.Top, timelineX, rect.Bottom);
            
            int eventHeight = Math.Min(60, rect.Height / Math.Max(events.Count, 1));
            using var titleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var descFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < events.Count; i++)
            {
                var eventData = events[i];
                int y = rect.Y + i * eventHeight;
                
                // Timeline node
                var nodeRect = new Rectangle(timelineX - 6, y + eventHeight / 2 - 6, 12, 12);
                using var nodeBrush = new SolidBrush(accentColor);
                g.FillEllipse(nodeBrush, nodeRect);
                
                // Event content
                var contentRect = new Rectangle(rect.X, y, rect.Width, eventHeight - 8);
                var titleRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width, contentRect.Height / 2);
                var descRect = new Rectangle(contentRect.X, titleRect.Bottom, contentRect.Width, contentRect.Height / 2);
                
                // Draw event title
                if (!string.IsNullOrEmpty(eventData.Title))
                {
                    using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    g.DrawString(eventData.Title, titleFont, titleBrush, titleRect);
                }
                
                // Draw event description or time
                if (!string.IsNullOrEmpty(eventData.Value))
                {
                    using var descBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
                    g.DrawString(eventData.Value, descFont, descBrush, descRect);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw time markers or event categories
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}