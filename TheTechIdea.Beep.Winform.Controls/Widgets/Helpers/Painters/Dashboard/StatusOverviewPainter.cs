using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Dashboard
{
    /// <summary>
    /// StatusOverview - System status dashboard
    /// </summary>
    internal sealed class StatusOverviewPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 12, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);

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

            // Draw status overview
            if (ctx.CustomData.ContainsKey("Metrics"))
            {
                var services = (List<Dictionary<string, object>>)ctx.CustomData["Metrics"];
                DrawStatusOverview(g, ctx.ContentRect, services);
            }
        }

        private void DrawStatusOverview(Graphics g, Rectangle rect, List<Dictionary<string, object>> services)
        {
            if (!services.Any()) return;

            int serviceHeight = Math.Min(40, rect.Height / Math.Max(services.Count, 1));
            using var serviceFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);

            for (int i = 0; i < services.Count; i++)
            {
                var service = services[i];
                int y = rect.Y + i * serviceHeight;

                // Service row background (alternating)
                if (i % 2 == 1)
                {
                    using var rowBrush = new SolidBrush(Color.FromArgb(10, Color.Gray));
                    g.FillRectangle(rowBrush, rect.X, y, rect.Width, serviceHeight);
                }

                // Status indicator
                string status = service.ContainsKey("Title") ? service["Title"].ToString() : "Unknown";
                Color statusColor = GetServiceStatusColor(status);

                var statusRect = new Rectangle(rect.X + 8, y + serviceHeight / 2 - 8, 16, 16);
                using var statusBrush = new SolidBrush(statusColor);
                g.FillEllipse(statusBrush, statusRect);

                // Service name
                var nameRect = new Rectangle(rect.X + 32, y, rect.Width - 120, serviceHeight);
                if (service.ContainsKey("Value"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(service["Value"].ToString(), serviceFont, nameBrush, nameRect, nameFormat);
                }

                // Status text
                var statusTextRect = new Rectangle(rect.Right - 80, y, 80, serviceHeight);
                using var statusTextBrush = new SolidBrush(statusColor);
                var statusFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(status, statusFont, statusTextBrush, statusTextRect, statusFormat);
            }
        }

        private Color GetServiceStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "running" or "healthy" or "online" => Color.Green,
                "stopped" or "error" or "offline" => Color.Red,
                "warning" or "degraded" => Color.Orange,
                "starting" or "loading" => Color.Blue,
                _ => Color.Gray
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw overall system health indicator or summary stats
        }
    }
}
