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
    /// ValidationMessage - Modern validation message with appropriate styling based on validation type (interactive)
    /// </summary>
    internal sealed class ValidationMessagePainter : WidgetPainterBase
    {
        private BaseImage.ImagePainter _imagePainter;
        private Rectangle _dismissRect;
        private Rectangle _copyRect;
        private Rectangle _panelRect;

        public ValidationMessagePainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 14;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            _panelRect = ctx.DrawingRect;

            // Icon area
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 20, 20);

            // Right-side action buttons
            int actionTop = ctx.DrawingRect.Top + pad + 2;
            _dismissRect = new Rectangle(ctx.DrawingRect.Right - pad - 16, actionTop, 16, 16);
            bool enableCopy = !(ctx.CustomData.ContainsKey("DisableCopy") && (bool)ctx.CustomData["DisableCopy"]);
            _copyRect = enableCopy ? new Rectangle(_dismissRect.X - 20, actionTop, 16, 16) : Rectangle.Empty;

            // Content area
            int rightActionsWidth = (_copyRect.IsEmpty ? 0 : 20) + 20; // copy + dismiss spacing
            ctx.ContentRect = new Rectangle(
                ctx.IconRect.Right + 10,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 2 - rightActionsWidth,
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
                {
                    bgColor1 = Color.FromArgb(240, 255, 240);
                    bgColor2 = Color.FromArgb(220, 255, 220);
                    borderColor = Color.FromArgb(100, 34, 197, 94);
                    break;
                }
                case "warning":
                {
                    bgColor1 = Color.FromArgb(255, 248, 240);
                    bgColor2 = Color.FromArgb(255, 240, 220);
                    borderColor = Color.FromArgb(100, 245, 158, 11);
                    break;
                }
                case "info":
                {
                    bgColor1 = Color.FromArgb(240, 248, 255);
                    bgColor2 = Color.FromArgb(220, 240, 255);
                    borderColor = Color.FromArgb(100, 59, 130, 246);
                    break;
                }
                default: // error
                {
                    bgColor1 = Color.FromArgb(255, 240, 240);
                    bgColor2 = Color.FromArgb(255, 220, 220);
                    borderColor = Color.FromArgb(100, 239, 68, 68);
                    break;
                }
            }

            using var bgBrush = new LinearGradientBrush(
                ctx.DrawingRect,
                bgColor1,
                bgColor2,
                LinearGradientMode.Vertical
            );
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            using var borderPen = new Pen(borderColor, 1);
            g.DrawPath(borderPen, bgPath);
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 3, offset: 1);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            string validationType = ctx.CustomData.ContainsKey("ValidationType") ?
                ctx.CustomData["ValidationType"].ToString().ToLower() : "error";

            if (ctx.ShowIcon)
            {
                DrawValidationIcon(g, ctx.IconRect, validationType);
            }

            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                Color titleColor = GetValidationColor(validationType);
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(titleColor);
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.ContentRect.X, ctx.ContentRect.Y);
            }

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
                {
                    using var checkPen = new Pen(iconColor, 2)
                    {
                        StartCap = LineCap.Round,
                        EndCap = LineCap.Round
                    };
                    Point[] checkPoints = new Point[]
                    {
                        new Point(rect.X + 4, rect.Y + 10),
                        new Point(rect.X + 8, rect.Y + 14),
                        new Point(rect.X + 16, rect.Y + 6)
                    };
                    g.DrawLines(checkPen, checkPoints);
                    break;
                }

                case "warning":
                {
                    Point[] trianglePoints = new Point[]
                    {
                        new Point(rect.X + 10, rect.Y + 2),
                        new Point(rect.X + 18, rect.Y + 16),
                        new Point(rect.X + 2, rect.Y + 16)
                    };
                    using var triangleBrush = new SolidBrush(iconColor);
                    g.FillPolygon(triangleBrush, trianglePoints);
                    using var exclamationPen = new Pen(Color.White, 2);
                    g.DrawLine(exclamationPen, rect.X + 10, rect.Y + 6, rect.X + 10, rect.Y + 10);
                    using var dotBrush = new SolidBrush(Color.White);
                    g.FillEllipse(dotBrush, rect.X + 9, rect.Y + 12, 2, 2);
                    break;
                }

                case "info":
                {
                    using var circleBrush = new SolidBrush(iconColor);
                    g.FillEllipse(circleBrush, rect);
                    using var letterFont = new Font("Arial", 12, FontStyle.Bold);
                    using var letterBrush = new SolidBrush(Color.White);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("i", letterFont, letterBrush, rect, format);
                    break;
                }

                default: // error
                {
                    using var xPen = new Pen(iconColor, 2)
                    {
                        StartCap = LineCap.Round,
                        EndCap = LineCap.Round
                    };
                    g.DrawLine(xPen, rect.X + 4, rect.Y + 4, rect.X + 16, rect.Y + 16);
                    g.DrawLine(xPen, rect.X + 16, rect.Y + 4, rect.X + 4, rect.Y + 16);
                    break;
                }
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
            // Dismiss 'x'
            if (!_dismissRect.IsEmpty)
            {
                bool hover = IsAreaHovered("Validation_Dismiss");
                using var pen = new Pen(hover ? (Theme?.PrimaryColor ?? Color.Blue) : (Theme?.ForeColor ?? Color.Black), 1.5f);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawLine(pen, _dismissRect.Left + 3, _dismissRect.Top + 3, _dismissRect.Right - 3, _dismissRect.Bottom - 3);
                g.DrawLine(pen, _dismissRect.Right - 3, _dismissRect.Top + 3, _dismissRect.Left + 3, _dismissRect.Bottom - 3);
            }

            // Copy icon
            if (!_copyRect.IsEmpty)
            {
                bool hover = IsAreaHovered("Validation_Copy");
                using var brush = new SolidBrush(hover ? (Theme?.PrimaryColor ?? Color.Blue) : (Theme?.ForeColor ?? Color.Black));
                // simple clipboard icon
                var r = Rectangle.Inflate(_copyRect, -3, -3);
                using var pen = new Pen(brush.Color, 1.2f);
                g.DrawRectangle(pen, r);
                g.DrawRectangle(pen, new Rectangle(r.X + 2, r.Y - 2, r.Width - 4, 4));
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            if (!_dismissRect.IsEmpty)
            {
                owner.AddHitArea("Validation_Dismiss", _dismissRect, null, () =>
                {
                    ctx.CustomData["ValidationDismissed"] = true;
                    notifyAreaHit?.Invoke("Validation_Dismiss", _dismissRect);
                    Owner?.Invalidate();
                });
            }

            if (!_copyRect.IsEmpty)
            {
                owner.AddHitArea("Validation_Copy", _copyRect, null, () =>
                {
                    ctx.CustomData["ValidationCopyRequested"] = ctx.CustomData.ContainsKey("Message") ? ctx.CustomData["Message"] : ctx.Value;
                    notifyAreaHit?.Invoke("Validation_Copy", _copyRect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}