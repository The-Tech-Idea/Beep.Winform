using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Form
{
    /// <summary>
    /// CompactForm - Space-efficient form design painter
    /// </summary>
    internal sealed class CompactFormPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            // Adjust for minimal padding
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);

            // Calculate compact layout - fields arranged in a grid or horizontal layout
            var fields = ctx.CustomData.ContainsKey("Fields") ? ctx.CustomData["Fields"] as List<FormField> : null;
            if (fields != null && fields.Count > 0)
            {
                int fieldHeight = 28;
                int fieldSpacing = 2;
                int labelWidth = (int)(ctx.DrawingRect.Width * 0.35); // 35% for labels
                int inputWidth = ctx.DrawingRect.Width - labelWidth - 8;

                for (int i = 0; i < fields.Count; i++)
                {
                    int y = ctx.DrawingRect.Y + (i * (fieldHeight + fieldSpacing));
                    var labelRect = new Rectangle(ctx.DrawingRect.X, y, labelWidth, fieldHeight);
                    var inputRect = new Rectangle(ctx.DrawingRect.X + labelWidth + 4, y, inputWidth, fieldHeight);

                    // Store in custom data for drawing
                    if (!ctx.CustomData.ContainsKey($"FieldLabelRect_{i}"))
                        ctx.CustomData[$"FieldLabelRect_{i}"] = labelRect;
                    if (!ctx.CustomData.ContainsKey($"FieldInputRect_{i}"))
                        ctx.CustomData[$"FieldInputRect_{i}"] = inputRect;
                }
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Draw subtle background
            using var bgBrush = new SolidBrush(Color.FromArgb(248, 248, 248));
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Draw field backgrounds
            var fields = ctx.CustomData.ContainsKey("Fields") ? ctx.CustomData["Fields"] as List<FormField> : null;
            if (fields != null)
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    if (ctx.CustomData.ContainsKey($"FieldInputRect_{i}"))
                    {
                        var inputRect = (Rectangle)ctx.CustomData[$"FieldInputRect_{i}"];
                        using var fieldBrush = new SolidBrush(Theme?.TextBoxBackColor ?? Color.White);
                        using var fieldPath = CreateRoundedPath(inputRect, 2);
                        g.FillPath(fieldBrush, fieldPath);

                        // Draw subtle border
                        using var borderPen = new Pen(Color.FromArgb(220, 220, 220), 1);
                        g.DrawPath(borderPen, fieldPath);
                    }
                }
            }
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var fields = ctx.CustomData.ContainsKey("Fields") ? ctx.CustomData["Fields"] as List<FormField> : null;
            if (fields == null) return;

            using var labelFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var valueFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.FromArgb(100, 100, 100));
            using var valueBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.Black);
            using var requiredBrush = new SolidBrush(Color.Red);

            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];

                // Draw label
                if (ctx.CustomData.ContainsKey($"FieldLabelRect_{i}"))
                {
                    var labelRect = (Rectangle)ctx.CustomData[$"FieldLabelRect_{i}"];
                    string labelText = field.Label;
                    if (field.IsRequired) labelText += "*";

                    g.DrawString(labelText, labelFont, labelBrush, labelRect,
                               new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }

                // Draw input value
                if (ctx.CustomData.ContainsKey($"FieldInputRect_{i}"))
                {
                    var inputRect = (Rectangle)ctx.CustomData[$"FieldInputRect_{i}"];
                    string displayValue = field.Value?.ToString() ?? "";
                    string textToDraw = string.IsNullOrEmpty(displayValue) ? field.Placeholder ?? "" : displayValue;

                    var textRect = Rectangle.Inflate(inputRect, -4, 0);
                    g.DrawString(textToDraw, valueFont, valueBrush, textRect,
                               new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw validation indicators
            var validationResults = ctx.CustomData.ContainsKey("ValidationResults")
                ? ctx.CustomData["ValidationResults"] as List<ValidationResult>
                : null;
            var fields = ctx.CustomData.ContainsKey("Fields") ? ctx.CustomData["Fields"] as List<FormField> : null;

            if (validationResults == null || fields == null) return;

            using var errorPen = new Pen(Color.Red, 1);
            using var warningPen = new Pen(Color.Orange, 1);

            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var validation = validationResults.FirstOrDefault(v => v.FieldName == field.Name);

                if (validation != null && !validation.IsValid)
                {
                    if (ctx.CustomData.ContainsKey($"FieldInputRect_{i}"))
                    {
                        var inputRect = (Rectangle)ctx.CustomData[$"FieldInputRect_{i}"];
                        var indicatorRect = new Rectangle(inputRect.Right - 8, inputRect.Top + 2, 6, 6);

                        using var indicatorBrush = new SolidBrush(
                            validation.Severity == ValidationSeverity.Error ? Color.Red : Color.Orange);
                        g.FillEllipse(indicatorBrush, indicatorRect);
                    }
                }
            }
        }
    }
}