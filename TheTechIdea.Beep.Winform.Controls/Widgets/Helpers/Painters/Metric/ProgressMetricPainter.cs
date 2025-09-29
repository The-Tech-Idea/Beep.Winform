using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Metric
{
    /// <summary>
    /// ProgressMetric - Value with progress bar
    /// </summary>
    internal sealed class ProgressMetricPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ProgressMetricPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 18);
            ctx.ValueRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, 32);

            // Progress bar below value
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.ValueRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, 8);

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
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            // Draw title with icon
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var titleBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                
                // Progress icon
                var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y - 1, 14, 14);
                _imagePainter.DrawSvg(g, "activity", iconRect, 
                    Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.7f);
                
                var textRect = new Rectangle(ctx.HeaderRect.X + 18, ctx.HeaderRect.Y, 
                    ctx.HeaderRect.Width - 18, ctx.HeaderRect.Height);
                g.DrawString(ctx.Title, titleFont, titleBrush, textRect);
            }

            // Value with enhanced styling
            if (!string.IsNullOrEmpty(ctx.Value))
            {
                using var valueFont = new Font(Owner.Font.FontFamily, 18f, FontStyle.Bold);
                using var valueBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                
                string displayText = !string.IsNullOrEmpty(ctx.Units) ? $"{ctx.Value} {ctx.Units}" : ctx.Value;
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(displayText, valueFont, valueBrush, ctx.ValueRect, format);
            }

            // Modern progress bar
            DrawModernProgressBar(g, ctx);
        }

        private void DrawModernProgressBar(Graphics g, WidgetContext ctx)
        {
            var progressRect = ctx.ContentRect;
            
            // Background track
            using var trackPath = CreateRoundedPath(progressRect, progressRect.Height / 2);
            using var trackBrush = new SolidBrush(Color.FromArgb(30, Theme?.ForeColor ?? Color.Gray));
            g.FillPath(trackBrush, trackPath);
            
            // Progress value (default to 75% if not specified)
            float progress = 0.75f;
            if (ctx.CustomData.ContainsKey("Progress"))
            {
                if (ctx.CustomData["Progress"] is float progressValue)
                    progress = Math.Max(0f, Math.Min(1f, progressValue));
                else if (float.TryParse(ctx.CustomData["Progress"]?.ToString(), out var parsedProgress))
                    progress = Math.Max(0f, Math.Min(1f, parsedProgress / 100f));
            }
            
            // Progress fill with gradient
            if (progress > 0)
            {
                var fillWidth = (int)(progressRect.Width * progress);
                var fillRect = new Rectangle(progressRect.X, progressRect.Y, fillWidth, progressRect.Height);
                
                using var fillPath = CreateRoundedPath(fillRect, fillRect.Height / 2);
                var progressColor = ctx.AccentColor != Color.Empty ? ctx.AccentColor : (Theme != null ? Theme.PrimaryColor : Color.FromArgb(33, 150, 243));
                
                var lighter = Color.FromArgb(
                    Math.Min(255, progressColor.R + 30),
                    Math.Min(255, progressColor.G + 30),
                    Math.Min(255, progressColor.B + 30));
                using var fillBrush = new LinearGradientBrush(
                    new RectangleF(fillRect.X, fillRect.Y, fillRect.Width, fillRect.Height),
                    progressColor,
                    lighter,
                    LinearGradientMode.Vertical);
                g.FillPath(fillBrush, fillPath);
            }
            
            // Progress percentage text
            if (ctx.CustomData.ContainsKey("ShowPercentage") && 
                (bool)ctx.CustomData["ShowPercentage"])
            {
                using var percentFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var percentBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Black));
                
                var percentText = $"{progress:P0}";
                var textRect = new Rectangle(progressRect.Right + 8, progressRect.Y - 4, 50, progressRect.Height + 8);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(percentText, percentFont, percentBrush, textRect, format);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw progress bar
            DrawProgressBar(g, ctx.ContentRect, ctx.TrendPercentage, ctx.AccentColor, Color.FromArgb(30, Color.Gray));
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Progress bar is clickable
            if (!ctx.ContentRect.IsEmpty)
            {
                owner.AddHitArea("Progress", ctx.ContentRect, null, () => notifyAreaHit?.Invoke("Progress", ctx.ContentRect));
            }
        }
    }
}
