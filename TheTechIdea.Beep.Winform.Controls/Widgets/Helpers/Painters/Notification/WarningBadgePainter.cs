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
    /// WarningBadge - Modern warning badge with exclamation triangle icon
    /// </summary>
    internal sealed class WarningBadgePainter : WidgetPainterBase
    {
        private BaseImage.ImagePainter _imagePainter;

        public WarningBadgePainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Icon area (warning triangle)
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 20, 20);

            // Content area
            ctx.ContentRect = new Rectangle(
                ctx.IconRect.Right + 8,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Modern warning orange background with subtle gradient
            using var bgBrush = new LinearGradientBrush(
                ctx.DrawingRect,
                Color.FromArgb(255, 248, 240), // Light orange
                Color.FromArgb(255, 240, 220), // Slightly darker orange
                LinearGradientMode.Vertical
            );

            // Rounded corners
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Warning border
            using var borderPen = new Pen(Color.FromArgb(100, 245, 158, 11), 1); // Warning orange border
            g.DrawPath(borderPen, bgPath);

            // Soft shadow
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 3, offset: 1);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw warning triangle icon
            if (ctx.ShowIcon)
            {
                DrawWarningIcon(g, ctx.IconRect);
            }

            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(245, 158, 11)); // Warning orange
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.ContentRect.X, ctx.ContentRect.Y);
            }

            // Draw message
            if (!string.IsNullOrEmpty(ctx.Message))
            {
                string message = ctx.Message;
                using var messageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var messageBrush = new SolidBrush(Color.FromArgb(120, 245, 158, 11));

                Rectangle messageRect = new Rectangle(
                    ctx.ContentRect.X,
                    ctx.ContentRect.Y + (ctx.ShowHeader ? 18 : 0),
                    ctx.ContentRect.Width,
                    ctx.ContentRect.Height - (ctx.ShowHeader ? 18 : 0)
                );

                g.DrawString(message, messageFont, messageBrush, messageRect);
            }
        }

        private void DrawWarningIcon(Graphics g, Rectangle rect)
        {
            // Draw warning triangle
            Point[] trianglePoints = new Point[]
            {
                new Point(rect.X + 10, rect.Y + 2),
                new Point(rect.X + 18, rect.Y + 16),
                new Point(rect.X + 2, rect.Y + 16)
            };

            using var triangleBrush = new SolidBrush(Color.FromArgb(245, 158, 11)); // Warning orange
            g.FillPolygon(triangleBrush, trianglePoints);

            // Draw exclamation mark
            using var exclamationPen = new Pen(Color.White, 2);
            g.DrawLine(exclamationPen, rect.X + 10, rect.Y + 6, rect.X + 10, rect.Y + 10);
            g.FillEllipse(new SolidBrush(Color.White), rect.X + 9, rect.Y + 12, 2, 2);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Add subtle accent lines
        }
    }
}