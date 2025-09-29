using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// ProgressAlert - Progress with message
    /// </summary>
    internal sealed class ProgressAlertPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ProgressAlertPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Content area for message
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 3 - 8
            );

            // Progress bar area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                8
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 8);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            // Progress icon
            var iconRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "activity", iconRect,
                Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.9f);

            // Progress message with modern typography
            using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);

            var textRect = new Rectangle(ctx.ContentRect.X + 28, ctx.ContentRect.Y,
                ctx.ContentRect.Width - 28, ctx.ContentRect.Height);
            var format = new StringFormat { LineAlignment = StringAlignment.Center };

            string progressText = !string.IsNullOrEmpty(ctx.Title) ? ctx.Title : "Processing...";
            g.DrawString(progressText, titleFont, titleBrush, textRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Modern progress bar with gradient
            float progress = (float)(ctx.TrendPercentage / 100.0);
            var progressRect = ctx.FooterRect;

            // Background track
            using var trackBrush = new SolidBrush(Color.FromArgb(30, Color.Gray));
            using var trackPath = CreateRoundedPath(progressRect, progressRect.Height / 2);
            g.FillPath(trackBrush, trackPath);

            // Progress fill
            if (progress > 0)
            {
                var fillWidth = (int)(progressRect.Width * progress);
                var fillRect = new Rectangle(progressRect.X, progressRect.Y, fillWidth, progressRect.Height);

                using var fillPath = CreateRoundedPath(fillRect, fillRect.Height / 2);
                var progressColor = ctx.AccentColor != Color.Empty ? ctx.AccentColor : (Theme != null ? Theme.PrimaryColor : Color.FromArgb(33, 150, 243));

                using var fillBrush = new LinearGradientBrush(fillRect,
                    progressColor,
                    Color.FromArgb(Math.Min(255, progressColor.R + 30),
                                 Math.Min(255, progressColor.G + 30),
                                 Math.Min(255, progressColor.B + 30)),
                    LinearGradientMode.Horizontal);
                g.FillPath(fillBrush, fillPath);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}