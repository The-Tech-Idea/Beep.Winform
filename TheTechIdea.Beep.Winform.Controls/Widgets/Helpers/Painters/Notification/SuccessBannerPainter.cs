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
    /// SuccessBanner - Modern success notification banner with checkmark icon
    /// </summary>
    internal sealed class SuccessBannerPainter : WidgetPainterBase
    {
        private BaseImage.ImagePainter _imagePainter;

        public SuccessBannerPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);

            // Icon area (checkmark)
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 24, 24);

            // Content area
            ctx.ContentRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3,
                ctx.DrawingRect.Height - pad * 2
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Modern success green background with subtle gradient
            using var bgBrush = new LinearGradientBrush(
                ctx.DrawingRect,
                Color.FromArgb(240, 255, 240), // Light green
                Color.FromArgb(220, 255, 220), // Slightly darker green
                LinearGradientMode.Vertical
            );

            // Rounded corners
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(100, 34, 197, 94), 1); // Success green border
            g.DrawPath(borderPen, bgPath);

            // Soft shadow
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 1);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw success checkmark icon
            if (ctx.ShowIcon)
            {
                DrawSuccessIcon(g, ctx.IconRect);
            }

            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(34, 197, 94)); // Success green
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.ContentRect.X, ctx.ContentRect.Y);
            }

            // Draw message
            if (ctx.CustomData.ContainsKey("Message"))
            {
                string message = ctx.CustomData["Message"].ToString();
                using var messageFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
                using var messageBrush = new SolidBrush(Color.FromArgb(120, 34, 197, 94));

                Rectangle messageRect = new Rectangle(
                    ctx.ContentRect.X,
                    ctx.ContentRect.Y + (ctx.ShowHeader ? 20 : 0),
                    ctx.ContentRect.Width,
                    ctx.ContentRect.Height - (ctx.ShowHeader ? 20 : 0)
                );

                g.DrawString(message, messageFont, messageBrush, messageRect);
            }
        }

        private void DrawSuccessIcon(Graphics g, Rectangle rect)
        {
            using var iconPen = new Pen(Color.FromArgb(34, 197, 94), 3); // Success green
            iconPen.StartCap = LineCap.Round;
            iconPen.EndCap = LineCap.Round;

            // Draw checkmark
            Point[] checkPoints = new Point[]
            {
                new Point(rect.X + 6, rect.Y + 12),
                new Point(rect.X + 10, rect.Y + 16),
                new Point(rect.X + 18, rect.Y + 8)
            };

            g.DrawLines(iconPen, checkPoints);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Add subtle accent lines or patterns
        }
    }
}