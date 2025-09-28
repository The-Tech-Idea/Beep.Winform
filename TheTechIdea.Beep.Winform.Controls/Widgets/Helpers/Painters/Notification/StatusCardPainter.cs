using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// StatusCard - Status card with icon and modern styling
    /// </summary>
    internal sealed class StatusCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public StatusCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Icon area
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 24, 24);
            
            // Content area
            ctx.ContentRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - (ctx.IconRect.Right + 12 - ctx.DrawingRect.Left) - pad,
                ctx.DrawingRect.Height - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Modern card background with soft shadow
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 8);
            g.FillPath(bgBrush, bgPath);
            
            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(20, Color.Gray), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            // Get status information
            var status = GetStatusInfo(ctx);
            
            // Status icon with background circle
            var iconColor = GetStatusColor(status.Type);
            
            // Icon background circle
            using var iconBgBrush = new SolidBrush(Color.FromArgb(20, iconColor));
            g.FillEllipse(iconBgBrush, ctx.IconRect);
            
            // Status icon
            var iconRect = new Rectangle(ctx.IconRect.X + 4, ctx.IconRect.Y + 4, 16, 16);
            _imagePainter.DrawSvg(g, status.IconName, iconRect, iconColor, 1.0f);
            
            // Status title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Medium);
                using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                
                var titleRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 20);
                g.DrawString(ctx.Title, titleFont, titleBrush, titleRect);
            }
            
            // Status message
            if (!string.IsNullOrEmpty(ctx.Value))
            {
                using var messageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var messageBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Black));
                
                var messageRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + 24, 
                    ctx.ContentRect.Width, ctx.ContentRect.Height - 24);
                
                var format = new StringFormat { LineAlignment = StringAlignment.Near };
                g.DrawString(ctx.Value, messageFont, messageBrush, messageRect, format);
            }
        }

        private StatusInfo GetStatusInfo(WidgetContext ctx)
        {
            var statusType = ctx.CustomData.ContainsKey("StatusType") ? 
                ctx.CustomData["StatusType"].ToString().ToLower() : "info";
            
            return statusType switch
            {
                "success" => new StatusInfo { Type = "success", IconName = "check-circle" },
                "warning" => new StatusInfo { Type = "warning", IconName = "alert-triangle" },
                "error" => new StatusInfo { Type = "error", IconName = "x-circle" },
                "info" => new StatusInfo { Type = "info", IconName = "info" },
                "loading" => new StatusInfo { Type = "loading", IconName = "activity" },
                _ => new StatusInfo { Type = "info", IconName = "info" }
            };
        }

        private Color GetStatusColor(string statusType)
        {
            return statusType switch
            {
                "success" => Color.FromArgb(76, 175, 80),    // Green
                "warning" => Color.FromArgb(255, 193, 7),    // Amber
                "error" => Color.FromArgb(244, 67, 54),      // Red
                "info" => Color.FromArgb(33, 150, 243),      // Blue
                "loading" => Color.FromArgb(156, 39, 176),   // Purple
                _ => Color.FromArgb(158, 158, 158)           // Gray
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional progress indicator for loading status
            var status = GetStatusInfo(ctx);
            if (status.Type == "loading" && ctx.CustomData.ContainsKey("ShowProgress"))
            {
                var progressRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Bottom - 8, 
                    ctx.ContentRect.Width, 4);
                
                // Animated progress bar placeholder
                using var trackBrush = new SolidBrush(Color.FromArgb(30, Color.Gray));
                using var trackPath = CreateRoundedPath(progressRect, 2);
                g.FillPath(trackBrush, trackPath);
                
                // Progress fill (could be animated)
                var progress = ctx.CustomData.ContainsKey("Progress") ? 
                    (float)ctx.CustomData["Progress"] / 100f : 0.3f;
                
                if (progress > 0)
                {
                    var fillRect = new Rectangle(progressRect.X, progressRect.Y, 
                        (int)(progressRect.Width * progress), progressRect.Height);
                    using var fillBrush = new SolidBrush(GetStatusColor(status.Type));
                    using var fillPath = CreateRoundedPath(fillRect, 2);
                    g.FillPath(fillBrush, fillPath);
                }
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }

        private class StatusInfo
        {
            public string Type { get; set; }
            public string IconName { get; set; }
        }
    }
}