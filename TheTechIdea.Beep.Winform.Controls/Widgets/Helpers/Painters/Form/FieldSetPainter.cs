using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Form
{
    /// <summary>
    /// FieldSet - Traditional fieldset styling painter
    /// </summary>
    internal sealed class FieldSetPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Legend area (title)
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);
            }

            // Content area (inside the fieldset border)
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 4;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Draw fieldset border
            using var borderPen = new Pen(Theme?.BorderColor ?? Color.FromArgb(200, 200, 200), 1);
            var borderRect = new Rectangle(ctx.DrawingRect.Left + 6, ctx.DrawingRect.Top + 6,
                                         ctx.DrawingRect.Width - 12, ctx.DrawingRect.Height - 12);

            // Create path with gap for legend
            using var borderPath = CreateFieldsetPath(borderRect, ctx.HeaderRect);
            g.DrawPath(borderPen, borderPath);

            // Fill background
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw legend (title)
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                DrawLegend(g, ctx.HeaderRect, ctx.Title);
            }

            // Draw fieldset content
            DrawFieldsetContent(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw fieldset-specific accents like required indicators
            var fields = ctx.CustomData.ContainsKey("Fields") ?
                ctx.CustomData["Fields"] as List<FormField> : null;

            if (fields != null && fields.Any(f => f.IsRequired))
            {
                DrawRequiredIndicator(g, ctx);
            }
        }

        private void DrawLegend(Graphics g, Rectangle legendRect, string title)
        {
            // Draw legend background (white rectangle over the border)
            using var legendBgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            var legendBgRect = new Rectangle(legendRect.Left - 8, legendRect.Top - 2, legendRect.Width + 16, legendRect.Height + 4);
            g.FillRectangle(legendBgBrush, legendBgRect);

            // Draw legend text
            using var legendFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var legendBrush = new SolidBrush(Theme?.TextForeColor ?? Color.FromArgb(100, 100, 100));
            g.DrawString(title, legendFont, legendBrush, legendRect);
        }

        private void DrawFieldsetContent(Graphics g, WidgetContext ctx)
        {
            var fields = ctx.CustomData.ContainsKey("Fields") ?
                ctx.CustomData["Fields"] as List<FormField> : null;

            if (fields == null || fields.Count == 0)
            {
                // Draw placeholder content
                using var placeholderFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var placeholderBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
                g.DrawString("No fields defined", placeholderFont, placeholderBrush, ctx.ContentRect);
                return;
            }

            // Draw fields in a simple vertical layout
            int fieldHeight = 35;
            int currentY = ctx.ContentRect.Top;

            foreach (var field in fields)
            {
                if (currentY + fieldHeight > ctx.ContentRect.Bottom) break;

                var fieldRect = new Rectangle(ctx.ContentRect.Left, currentY, ctx.ContentRect.Width, fieldHeight);

                // Draw field label
                using var labelFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var labelBrush = new SolidBrush(Theme?.LabelColor ?? Color.Black);
                g.DrawString(field.Label, labelFont, labelBrush, fieldRect.Left, fieldRect.Top + 2);

                // Draw field value/input area
                var inputRect = new Rectangle(fieldRect.Left, fieldRect.Top + 14, fieldRect.Width, 18);
                using var inputBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
                using var inputPen = new Pen(Color.FromArgb(200, 200, 200), 1);

                g.FillRectangle(inputBrush, inputRect);
                g.DrawRectangle(inputPen, inputRect);

                // Draw field value
                string displayValue = field.Value?.ToString() ?? "";
                if (string.IsNullOrEmpty(displayValue) && !string.IsNullOrEmpty(field.Placeholder))
                {
                    displayValue = field.Placeholder;
                }

                using var valueFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                Color valueColor = string.IsNullOrEmpty(field.Value?.ToString()) ?
                    Color.Gray : (Theme?.LabelColor ?? Color.Black);
                using var valueBrush = new SolidBrush(valueColor);

                g.DrawString(displayValue, valueFont, valueBrush, inputRect.Left + 4, inputRect.Top + 2);

                // Draw required indicator
                if (field.IsRequired)
                {
                    using var requiredBrush = new SolidBrush(Color.Red);
                    g.DrawString("*", labelFont, requiredBrush, fieldRect.Right - 10, fieldRect.Top + 2);
                }

                currentY += fieldHeight + 2;
            }
        }

        private void DrawRequiredIndicator(Graphics g, WidgetContext ctx)
        {
            // Draw a small required indicator in the top-right corner
            var indicatorRect = new Rectangle(ctx.DrawingRect.Right - 20, ctx.DrawingRect.Top + 4, 12, 12);
            using var indicatorBrush = new SolidBrush(Color.FromArgb(200, Color.Red));
            g.FillEllipse(indicatorBrush, indicatorRect);

            using var indicatorFont = new Font("Arial", 8f, FontStyle.Bold);
            using var indicatorTextBrush = new SolidBrush(Color.White);
            g.DrawString("*", indicatorFont, indicatorTextBrush, indicatorRect,
                       new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private GraphicsPath CreateFieldsetPath(Rectangle borderRect, Rectangle legendRect)
        {
            var path = new GraphicsPath();

            // Calculate legend gap
            int legendCenterX = legendRect.Left + legendRect.Width / 2;
            int gapWidth = Math.Max(legendRect.Width + 16, 40); // Minimum gap width
            int gapStartX = legendCenterX - gapWidth / 2;
            int gapEndX = legendCenterX + gapWidth / 2;

            // Top line with gap for legend
            path.AddLine(borderRect.Left, borderRect.Top, gapStartX, borderRect.Top);
            path.AddLine(gapEndX, borderRect.Top, borderRect.Right, borderRect.Top);

            // Right line
            path.AddLine(borderRect.Right, borderRect.Top, borderRect.Right, borderRect.Bottom);

            // Bottom line
            path.AddLine(borderRect.Right, borderRect.Bottom, borderRect.Left, borderRect.Bottom);

            // Left line
            path.AddLine(borderRect.Left, borderRect.Bottom, borderRect.Left, borderRect.Top);

            return path;
        }
    }
}