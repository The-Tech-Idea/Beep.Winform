using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Map
{
    /// <summary>
    /// LiveTracking - Real-time location tracking painter
    /// </summary>
    internal sealed class LiveTrackingPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Status indicator area
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                24, 24
            );
            
            // Tracking title and status
            ctx.HeaderRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3,
                24
            );
            
            // Live map display area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                80
            );
            
            // Current location and time
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.ContentRect.Bottom - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 2, offset: 1);
            
            // Gradient background for live tracking feel
            using var brush = new LinearGradientBrush(ctx.DrawingRect, 
                Color.FromArgb(250, 255, 250), 
                Color.FromArgb(240, 255, 240), 
                LinearGradientMode.Vertical);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(brush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var locations = ctx.MapMarkers ?? new List<MapMarker>();
            double latitude = ctx.Latitude;
            double longitude = ctx.Longitude;
            var lastUpdated = ctx.LastUpdated;

            // Draw live status indicator (pulsing green)
            using var statusBrush = new SolidBrush(Color.FromArgb(76, 175, 80)); // Green for live
            g.FillEllipse(statusBrush, ctx.IconRect);
            
            // Draw inner pulse effect
            var pulseRect = new Rectangle(ctx.IconRect.X + 6, ctx.IconRect.Y + 6, ctx.IconRect.Width - 12, ctx.IconRect.Height - 12);
            using var pulseBrush = new SolidBrush(Color.FromArgb(150, 139, 195, 74));
            g.FillEllipse(pulseBrush, pulseRect);

            // Draw "LIVE" text
            using var liveFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            using var liveBrush = new SolidBrush(Color.White);
            var liveFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("LIVE", liveFont, liveBrush, ctx.IconRect, liveFormat);

            // Draw tracking title
            using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect.X, ctx.HeaderRect.Y);
            
            using var statusFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var statusBrush2 = new SolidBrush(Color.FromArgb(100, 200, 100));
            g.DrawString("Tracking Active", statusFont, statusBrush2, ctx.HeaderRect.X, ctx.HeaderRect.Y + 14);

            // Draw live tracking map
            DrawLiveTrackingMap(g, ctx.ContentRect, locations, ctx.AccentColor);

            // Draw current location
            using var locationFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var locationBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            string locationText = $"Current: {latitude:F4}, {longitude:F4}";
            g.DrawString(locationText, locationFont, locationBrush, ctx.FooterRect);

            // Draw last update time
            using var timeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            string timeText = $"Last update: {lastUpdated:HH:mm:ss}";
            var timeSize = TextUtils.MeasureText(g,timeText, timeFont);
            g.DrawString(timeText, timeFont, timeBrush, ctx.FooterRect.Right - timeSize.Width, ctx.FooterRect.Y);
        }

        private void DrawLiveTrackingMap(Graphics g, Rectangle rect, List<MapMarker> locations, Color accentColor)
        {
            if (rect.Width < 20 || rect.Height < 20) return;

            // Draw map background with darker shade for live view
            using var mapBrush = new SolidBrush(Color.FromArgb(235, 245, 235));
            g.FillRectangle(mapBrush, rect);

            // Draw tracking path if multiple locations
            if (locations.Count > 1)
            {
                DrawTrackingPath(g, rect, locations, accentColor);
            }

            // Draw current location marker (larger and pulsing)
            int markerSize = 16;
            var markerRect = new Rectangle(
                rect.X + rect.Width / 2 - markerSize / 2,
                rect.Y + rect.Height / 2 - markerSize / 2,
                markerSize, markerSize
            );
            
            // Outer pulse ring
            using var pulseRingBrush = new SolidBrush(Color.FromArgb(100, accentColor));
            var pulseRingRect = new Rectangle(markerRect.X - 4, markerRect.Y - 4, markerRect.Width + 8, markerRect.Height + 8);
            g.FillEllipse(pulseRingBrush, pulseRingRect);
            
            // Main marker
            using var markerBrush = new SolidBrush(accentColor);
            g.FillEllipse(markerBrush, markerRect);

            // Inner dot
            var innerRect = new Rectangle(markerRect.X + 4, markerRect.Y + 4, markerRect.Width - 8, markerRect.Height - 8);
            using var innerBrush = new SolidBrush(Color.White);
            g.FillEllipse(innerBrush, innerRect);

            // Draw border
            using var borderPen = new Pen(Color.FromArgb(180, 180, 180), 1);
            g.DrawRectangle(borderPen, rect);
        }

        private void DrawTrackingPath(Graphics g, Rectangle rect, List<MapMarker> locations, Color accentColor)
        {
            if (locations.Count < 2) return;

            // Simulate path by drawing connected lines
            var points = new List<PointF>();
            for (int i = 0; i < Math.Min(locations.Count, 10); i++) // Limit to 10 points
            {
                float x = rect.X + (float)i / 9 * rect.Width;
                float y = rect.Y + rect.Height / 2 + (float)Math.Sin(i * 0.8) * rect.Height / 4;
                points.Add(new PointF(x, y));
            }

            if (points.Count > 1)
            {
                using var pathPen = new Pen(Color.FromArgb(150, accentColor), 3);
                pathPen.DashStyle = DashStyle.Dash;
                g.DrawLines(pathPen, points.ToArray());
                
                // Draw direction arrows
                for (int i = 1; i < points.Count; i += 2)
                {
                    DrawDirectionArrow(g, points[i - 1], points[i], accentColor);
                }
            }
        }

        private void DrawDirectionArrow(Graphics g, PointF start, PointF end, Color color)
        {
            // Simple arrow drawing
            using var arrowBrush = new SolidBrush(Color.FromArgb(180, color));
            var arrowSize = 4;
            var arrowRect = new Rectangle((int)end.X - arrowSize / 2, (int)end.Y - arrowSize / 2, arrowSize, arrowSize);
            g.FillEllipse(arrowBrush, arrowRect);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw additional live tracking indicators
        }
    }
}