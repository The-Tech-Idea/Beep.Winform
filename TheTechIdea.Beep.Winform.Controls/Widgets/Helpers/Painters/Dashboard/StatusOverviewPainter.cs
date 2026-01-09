using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Dashboard
{
    /// <summary>
    /// StatusOverview - System status dashboard with hit areas per service row and hover accents
    /// </summary>
    internal sealed class StatusOverviewPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _rowRects = new();
        private Rectangle _headerRectCache;
        private Rectangle _contentRectCache;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 12, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);

            _headerRectCache = ctx.HeaderRect;
            _contentRectCache = ctx.ContentRect;
            _rowRects.Clear();

            // Precompute row rects if metrics count is known
            var services = ctx.Metrics;
            if (services != null && services.Count > 0)
            {
                int serviceHeight = Math.Min(40, ctx.ContentRect.Height / Math.Max(services.Count, 1));
                for (int i = 0; i < services.Count; i++)
                {
                    int y = ctx.ContentRect.Y + i * serviceHeight;
                    _rowRects.Add(new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, serviceHeight));
                }
            }

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
                using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            // Draw status overview
            var services = ctx.Metrics;
            if (services != null && services.Count > 0)
            {
                DrawStatusOverview(g, ctx.ContentRect, services);
            }
        }

        private void DrawStatusOverview(Graphics g, Rectangle rect, List<DashboardMetric> services)
        {
            if (!services.Any()) return;

            int serviceHeight = Math.Min(40, rect.Height / Math.Max(services.Count, 1));
            using var serviceFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
            using var statusFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Bold);

            for (int i = 0; i < services.Count; i++)
            {
                var service = services[i];
                int y = rect.Y + i * serviceHeight;
                var rowRect = new Rectangle(rect.X, y, rect.Width, serviceHeight);

                // Alternating background
                if (i % 2 == 1)
                {
                    using var rowBrush = new SolidBrush(Color.FromArgb(10, Theme?.ForeColor ?? Color.Gray));
                    g.FillRectangle(rowBrush, rowRect);
                }

                // Hover highlight
                if (IsAreaHovered($"StatusOverview_Row_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(hover, Rectangle.Inflate(rowRect, -2, -2));
                }

                // Status indicator
                string status = !string.IsNullOrEmpty(service.Title) ? service.Title : "Unknown";
                Color statusColor = GetServiceStatusColor(status);

                var statusDot = new Rectangle(rowRect.X + 8, rowRect.Y + rowRect.Height / 2 - 8, 16, 16);
                using var statusBrush = new SolidBrush(statusColor);
                g.FillEllipse(statusBrush, statusDot);

                // Service name (Value)
                var nameRect = new Rectangle(rowRect.X + 32, rowRect.Y, rowRect.Width - 120, rowRect.Height);
                if (!string.IsNullOrEmpty(service.Value))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Theme?.ForeColor ?? Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(service.Value, serviceFont, nameBrush, nameRect, nameFormat);
                }

                // Status text
                var statusTextRect = new Rectangle(rowRect.Right - 80, rowRect.Y, 80, rowRect.Height);
                using var statusTextBrush = new SolidBrush(statusColor);
                var statusFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(status, statusFont, statusTextBrush, statusTextRect, statusFormat);
            }
        }

        private Color GetServiceStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "running" or "healthy" or "online" => Color.FromArgb(76, 175, 80),
                "stopped" or "error" or "offline" => Color.FromArgb(244, 67, 54),
                "warning" or "degraded" => Color.FromArgb(255, 193, 7),
                "starting" or "loading" => Theme?.AccentColor ?? Color.Blue,
                _ => Color.FromArgb(158, 158, 158)
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional overall accents
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            if (!_headerRectCache.IsEmpty)
            {
                owner.AddHitArea("StatusOverview_Header", _headerRectCache, null, () =>
                {
                    ctx.StatusHeaderClicked = true;
                    notifyAreaHit?.Invoke("StatusOverview_Header", _headerRectCache);
                    Owner?.Invalidate();
                });
            }

            for (int i = 0; i < _rowRects.Count; i++)
            {
                int idx = i;
                var rect = _rowRects[i];
                owner.AddHitArea($"StatusOverview_Row_{idx}", rect, null, () =>
                {
                    ctx.SelectedStatusIndex = idx;
                    notifyAreaHit?.Invoke($"StatusOverview_Row_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}
