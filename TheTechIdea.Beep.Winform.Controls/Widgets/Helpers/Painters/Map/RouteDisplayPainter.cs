using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Map
{
    /// <summary>
    /// RouteDisplay - Route/path visualization painter
    /// </summary>
    internal sealed class RouteDisplayPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Route info header
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            // Route visualization area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                80
            );
            
            // Distance and time info
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
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var routes = ctx.MapRoutes?.Cast<MapRoute>().ToList() ?? new List<MapRoute>();
            var routeColor = ctx.RouteColor;

            // Draw route title and type
            using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);

            // Draw route type
            using var typeFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var typeBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            string routeType = routes.FirstOrDefault()?.RouteType ?? "Driving";
            g.DrawString($"Route Type: {routeType}", typeFont, typeBrush, ctx.HeaderRect.X, ctx.HeaderRect.Y + 16);

            // Draw route visualization
            DrawRouteVisualization(g, ctx.ContentRect, routes, routeColor);

            // Draw route statistics
            if (routes.Count > 0)
            {
                var route = routes[0];
                
                // Distance
                using var distanceFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var distanceBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                string distanceText = $"{route.Distance:F1} km";
                g.DrawString(distanceText, distanceFont, distanceBrush, ctx.FooterRect);

                // Estimated time
                using var timeFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var timeBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                string timeText = $"Est. Time: {route.EstimatedTime}";
                g.DrawString(timeText, timeFont, timeBrush, ctx.FooterRect.X, ctx.FooterRect.Y + 16);

                // Route status
                using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var statusBrush = new SolidBrush(Color.FromArgb(100, 150, 100));
                string statusText = "Active Route";
                var statusSize = TextUtils.MeasureText(g,statusText, statusFont);
                g.DrawString(statusText, statusFont, statusBrush, ctx.FooterRect.Right - statusSize.Width, ctx.FooterRect.Y);
            }
        }

        private void DrawRouteVisualization(Graphics g, Rectangle rect, List<MapRoute> routes, Color routeColor)
        {
            if (rect.Width < 20 || rect.Height < 20) return;

            // Draw map background
            using var mapBrush = new SolidBrush(Color.FromArgb(248, 248, 248));
            g.FillRectangle(mapBrush, rect);

            if (routes.Count > 0)
            {
                var route = routes[0];
                
                // Draw start marker
                var startRect = new Rectangle(rect.X + 8, rect.Y + rect.Height / 2 - 6, 12, 12);
                using var startBrush = new SolidBrush(Color.FromArgb(76, 175, 80)); // Green
                g.FillEllipse(startBrush, startRect);
                
                // Draw "S" for start
                using var startFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var startTextBrush = new SolidBrush(Color.White);
                var startFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("S", startFont, startTextBrush, startRect, startFormat);

                // Draw end marker
                var endRect = new Rectangle(rect.Right - 20, rect.Y + rect.Height / 2 - 6, 12, 12);
                using var endBrush = new SolidBrush(Color.FromArgb(244, 67, 54)); // Red
                g.FillEllipse(endBrush, endRect);
                
                // Draw "E" for end
                using var endFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var endTextBrush = new SolidBrush(Color.White);
                var endFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("E", endFont, endTextBrush, endRect, endFormat);

                // Draw route path
                DrawRoutePath(g, rect, routeColor);

                // Draw waypoints if any
                if (route.Waypoints.Count > 0)
                {
                    DrawWaypoints(g, rect, route.Waypoints.Count);
                }
            }

            // Draw border
            using var borderPen = new Pen(Color.FromArgb(200, 200, 200), 1);
            g.DrawRectangle(borderPen, rect);
        }

        private void DrawRoutePath(Graphics g, Rectangle rect, Color routeColor)
        {
            // Draw curved path from start to end
            var startPoint = new PointF(rect.X + 14, rect.Y + rect.Height / 2);
            var endPoint = new PointF(rect.Right - 14, rect.Y + rect.Height / 2);
            
            // Create curved path
            using var path = new GraphicsPath();
            var controlPoint1 = new PointF(startPoint.X + (endPoint.X - startPoint.X) / 3, startPoint.Y - 20);
            var controlPoint2 = new PointF(startPoint.X + 2 * (endPoint.X - startPoint.X) / 3, endPoint.Y + 15);
            
            path.AddBezier(startPoint, controlPoint1, controlPoint2, endPoint);
            
            using var routePen = new Pen(routeColor, 3);
            g.DrawPath(routePen, path);
            
            // Draw direction arrows along the path
            DrawRouteArrows(g, startPoint, endPoint, routeColor);
        }

        private void DrawRouteArrows(Graphics g, PointF start, PointF end, Color color)
        {
            // Draw a few direction arrows along the route
            int arrowCount = 3;
            for (int i = 1; i <= arrowCount; i++)
            {
                float t = (float)i / (arrowCount + 1);
                var arrowPoint = new PointF(
                    start.X + t * (end.X - start.X),
                    start.Y + (float)Math.Sin(t * Math.PI) * -10 // Slight curve
                );
                
                // Draw arrow
                using var arrowBrush = new SolidBrush(Color.FromArgb(200, color));
                var arrowSize = 6;
                var arrowRect = new Rectangle(
                    (int)arrowPoint.X - arrowSize / 2,
                    (int)arrowPoint.Y - arrowSize / 2,
                    arrowSize, arrowSize
                );
                
                // Draw triangle pointing right
                var arrowPoints = new PointF[]
                {
                    new PointF(arrowRect.X, arrowRect.Y),
                    new PointF(arrowRect.X, arrowRect.Bottom),
                    new PointF(arrowRect.Right, arrowRect.Y + arrowRect.Height / 2)
                };
                g.FillPolygon(arrowBrush, arrowPoints);
            }
        }

        private void DrawWaypoints(Graphics g, Rectangle rect, int waypointCount)
        {
            // Draw waypoint markers along the route
            using var waypointBrush = new SolidBrush(Color.FromArgb(255, 193, 7)); // Amber
            
            for (int i = 0; i < Math.Min(waypointCount, 3); i++)
            {
                float t = (float)(i + 1) / (waypointCount + 1);
                var waypointPoint = new PointF(
                    rect.X + 14 + t * (rect.Width - 28),
                    rect.Y + rect.Height / 2 + (float)Math.Sin(t * Math.PI) * -8
                );
                
                var waypointRect = new Rectangle(
                    (int)waypointPoint.X - 4,
                    (int)waypointPoint.Y - 4,
                    8, 8
                );
                g.FillEllipse(waypointBrush, waypointRect);
                
                // Draw waypoint number
                using var wpFont = new Font(Owner.Font.FontFamily, 6f, FontStyle.Bold);
                using var wpTextBrush = new SolidBrush(Color.White);
                var wpFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString((i + 1).ToString(), wpFont, wpTextBrush, waypointRect, wpFormat);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw route optimization indicators
        }
    }
}