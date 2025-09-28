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
    /// InfoPanel - Modern information panel with info icon
    /// </summary>
    internal sealed class InfoPanelPainter : WidgetPainterBase
    {
        private BaseImage.ImagePainter _imagePainter;

        public InfoPanelPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);

            // Icon area (info circle)
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
            // Modern info blue background with subtle gradient
            using var bgBrush = new LinearGradientBrush(
                ctx.DrawingRect,
                Color.FromArgb(240, 248, 255), // Light blue
                Color.FromArgb(220, 240, 255), // Slightly darker blue
                LinearGradientMode.Vertical
            );

            // Rounded corners
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Info border
            using var borderPen = new Pen(Color.FromArgb(100, 59, 130, 246), 1); // Info blue border
            g.DrawPath(borderPen, bgPath);

            // Soft shadow
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 1);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw info circle icon
            if (ctx.ShowIcon)
            {
                DrawInfoIcon(g, ctx.IconRect);
            }

            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(59, 130, 246)); // Info blue
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.ContentRect.X, ctx.ContentRect.Y);
            }

            // Draw message
            if (ctx.CustomData.ContainsKey("Message"))
            {
                string message = ctx.CustomData["Message"].ToString();
                using var messageFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
                using var messageBrush = new SolidBrush(Color.FromArgb(120, 59, 130, 246));

                Rectangle messageRect = new Rectangle(
                    ctx.ContentRect.X,
                    ctx.ContentRect.Y + (ctx.ShowHeader ? 20 : 0),
                    ctx.ContentRect.Width,
                    ctx.ContentRect.Height - (ctx.ShowHeader ? 20 : 0)
                );

                g.DrawString(message, messageFont, messageBrush, messageRect);
            }
        }

        private void DrawInfoIcon(Graphics g, Rectangle rect)
        {
            // Draw info circle background
            using var circleBrush = new SolidBrush(Color.FromArgb(59, 130, 246)); // Info blue
            g.FillEllipse(circleBrush, rect);

            // Draw "i" letter
            using var letterFont = new Font("Arial", 14, FontStyle.Bold);
            using var letterBrush = new SolidBrush(Color.White);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("i", letterFont, letterBrush, rect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Add subtle accent lines or patterns
        }
    }
}