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
    /// SystemAlert - Modern system alert with gear/cog icon and professional styling
    /// </summary>
    internal sealed class SystemAlertPainter : WidgetPainterBase
    {
        private BaseImage.ImagePainter _imagePainter;

        public SystemAlertPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);

            // Icon area (system gear)
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
            // Modern system gray background with subtle gradient
            using var bgBrush = new LinearGradientBrush(
                ctx.DrawingRect,
                Color.FromArgb(250, 250, 250), // Light gray
                Color.FromArgb(240, 240, 240), // Slightly darker gray
                LinearGradientMode.Vertical
            );

            // Rounded corners
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // System border
            using var borderPen = new Pen(Color.FromArgb(100, 107, 114, 128), 1); // System gray border
            g.DrawPath(borderPen, bgPath);

            // Soft shadow
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 1);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw system gear icon
            if (ctx.ShowIcon)
            {
                DrawSystemIcon(g, ctx.IconRect);
            }

            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(107, 114, 128)); // System gray
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.ContentRect.X, ctx.ContentRect.Y);
            }

            // Draw message
            if (ctx.CustomData.ContainsKey("Message"))
            {
                string message = ctx.CustomData["Message"].ToString();
                using var messageFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
                using var messageBrush = new SolidBrush(Color.FromArgb(120, 107, 114, 128));

                Rectangle messageRect = new Rectangle(
                    ctx.ContentRect.X,
                    ctx.ContentRect.Y + (ctx.ShowHeader ? 20 : 0),
                    ctx.ContentRect.Width,
                    ctx.ContentRect.Height - (ctx.ShowHeader ? 20 : 0)
                );

                g.DrawString(message, messageFont, messageBrush, messageRect);
            }

            // Draw timestamp if available
            if (ctx.CustomData.ContainsKey("Timestamp"))
            {
                string timestamp = ctx.CustomData["Timestamp"].ToString();
                using var timeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var timeBrush = new SolidBrush(Color.FromArgb(80, 107, 114, 128));

                Rectangle timeRect = new Rectangle(
                    ctx.ContentRect.X,
                    ctx.ContentRect.Bottom - 14,
                    ctx.ContentRect.Width,
                    14
                );

                g.DrawString(timestamp, timeFont, timeBrush, timeRect);
            }
        }

        private void DrawSystemIcon(Graphics g, Rectangle rect)
        {
            // Draw gear/cog icon
            using var gearPen = new Pen(Color.FromArgb(107, 114, 128), 2);

            // Outer circle
            g.DrawEllipse(gearPen, rect.X + 2, rect.Y + 2, 20, 20);

            // Inner circle
            g.DrawEllipse(gearPen, rect.X + 6, rect.Y + 6, 12, 12);

            // Gear teeth
            int centerX = rect.X + 12;
            int centerY = rect.Y + 12;
            int toothLength = 4;

            for (int i = 0; i < 8; i++)
            {
                double angle = i * Math.PI / 4;
                int x1 = centerX + (int)(8 * Math.Cos(angle));
                int y1 = centerY + (int)(8 * Math.Sin(angle));
                int x2 = centerX + (int)((8 + toothLength) * Math.Cos(angle));
                int y2 = centerY + (int)((8 + toothLength) * Math.Sin(angle));

                g.DrawLine(gearPen, x1, y1, x2, y2);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Add subtle accent lines or status indicators
        }
    }
}