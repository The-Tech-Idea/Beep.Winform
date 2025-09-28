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
    /// ValidationMessage - Modern validation message with appropriate styling based on validation type
    /// </summary>
    internal sealed class ValidationMessagePainter : WidgetPainterBase
    {
        private BaseImage.ImagePainter _imagePainter;

        public ValidationMessagePainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 14;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Icon area
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 20, 20);

            // Content area
            ctx.ContentRect = new Rectangle(
                ctx.IconRect.Right + 10,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Determine validation type and colors
            string validationType = ctx.CustomData.ContainsKey("ValidationType") ?
                ctx.CustomData["ValidationType"].ToString().ToLower() : "error";

            Color bgColor1, bgColor2, borderColor;

            switch (validationType)
            {
                case "success":
                    bgColor1 = Color.FromArgb(240, 255, 240);
                    bgColor2 = Color.FromArgb(220, 255, 220);
                    borderColor = Color.FromArgb(100, 34, 197, 94);
                    break;
                case "warning":
                    bgColor1 = Color.FromArgb(255, 248, 240);
                    bgColor2 = Color.FromArgb(255, 240, 220);
                    borderColor = Color.FromArgb(100, 245, 158, 11);
                    break;
                case "info":
                    bgColor1 = Color.FromArgb(240, 248, 255);
                    bgColor2 = Color.FromArgb(220, 240, 255);
                    borderColor = Color.FromArgb(100, 59, 130, 246);
                    break;
                default: // error
                    bgColor1 = Color.FromArgb(255, 240, 240);
                    bgColor2 = Color.FromArgb(255, 220, 220);
                    borderColor = Color.FromArgb(100, 239, 68, 68);
                    break;
            }

            // Modern background with subtle gradient
            using var bgBrush = new LinearGradientBrush(
                ctx.DrawingRect,
                bgColor1,
                bgColor2,
                LinearGradientMode.Vertical
            );

            // Rounded corners
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Border
            using var borderPen = new Pen(borderColor, 1);
            g.DrawPath(borderPen, bgPath);

            // Soft shadow
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 3, offset: 1);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Determine validation type
            string validationType = ctx.CustomData.ContainsKey("ValidationType") ?
                ctx.CustomData["ValidationType"].ToString().ToLower() : "error";

            // Draw appropriate icon
            if (ctx.ShowIcon)
            {
                DrawValidationIcon(g, ctx.IconRect, validationType);
            }

            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                Color titleColor = GetValidationColor(validationType);
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(titleColor);
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.ContentRect.X, ctx.ContentRect.Y);
            }

            // Draw message
            if (ctx.CustomData.ContainsKey("Message"))
            {
                string message = ctx.CustomData["Message"].ToString();
                Color messageColor = Color.FromArgb(120, GetValidationColor(validationType));
                using var messageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var messageBrush = new SolidBrush(messageColor);

                Rectangle messageRect = new Rectangle(
                    ctx.ContentRect.X,
                    ctx.ContentRect.Y + (ctx.ShowHeader ? 18 : 0),
                    ctx.ContentRect.Width,
                    ctx.ContentRect.Height - (ctx.ShowHeader ? 18 : 0)
                );

                g.DrawString(message, messageFont, messageBrush, messageRect);
            }
        }

        private void DrawValidationIcon(Graphics g, Rectangle rect, string validationType)
        {
            Color iconColor = GetValidationColor(validationType);

            switch (validationType)
            {
                case "success":
                    // Checkmark
                    using var checkPen = new Pen(iconColor, 2);
                    checkPen.StartCap = LineCap.Round;
                    checkPen.EndCap = LineCap.Round;
                    Point[] checkPoints = new Point[]
                    {
                        new Point(rect.X + 4, rect.Y + 10),
                        new Point(rect.X + 8, rect.Y + 14),
                        new Point(rect.X + 16, rect.Y + 6)
                    };
                    g.DrawLines(checkPen, checkPoints);
                    break;

                case "warning":
                    // Exclamation triangle
                    Point[] trianglePoints = new Point[]
                    {
                        new Point(rect.X + 10, rect.Y + 2),
                        new Point(rect.X + 18, rect.Y + 16),
                        new Point(rect.X + 2, rect.Y + 16)
                    };
                    using var triangleBrush = new SolidBrush(iconColor);
                    g.FillPolygon(triangleBrush, trianglePoints);
                    // Exclamation mark
                    using var exclamationPen = new Pen(Color.White, 2);
                    g.DrawLine(exclamationPen, rect.X + 10, rect.Y + 6, rect.X + 10, rect.Y + 10);
                    g.FillEllipse(new SolidBrush(Color.White), rect.X + 9, rect.Y + 12, 2, 2);
                    break;

                case "info":
                    // Info circle with "i"
                    using var circleBrush = new SolidBrush(iconColor);
                    g.FillEllipse(circleBrush, rect);
                    using var letterFont = new Font("Arial", 12, FontStyle.Bold);
                    using var letterBrush = new SolidBrush(Color.White);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("i", letterFont, letterBrush, rect, format);
                    break;

                default: // error
                    // X mark
                    using var xPen = new Pen(iconColor, 2);
                    xPen.StartCap = LineCap.Round;
                    xPen.EndCap = LineCap.Round;
                    g.DrawLine(xPen, rect.X + 4, rect.Y + 4, rect.X + 16, rect.Y + 16);
                    g.DrawLine(xPen, rect.X + 16, rect.Y + 4, rect.X + 4, rect.Y + 16);
                    break;
            }
        }

        private Color GetValidationColor(string validationType)
        {
            return validationType switch
            {
                "success" => Color.FromArgb(34, 197, 94),
                "warning" => Color.FromArgb(245, 158, 11),
                "info" => Color.FromArgb(59, 130, 246),
                _ => Color.FromArgb(239, 68, 68) // error
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Add subtle accent lines
        }
    }
}