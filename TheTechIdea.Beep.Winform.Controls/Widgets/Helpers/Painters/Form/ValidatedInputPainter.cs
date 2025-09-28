using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Form
{
    /// <summary>
    /// ValidatedInput - Single input with validation painter
    /// </summary>
    internal sealed class ValidatedInputPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            // Adjust for padding
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            // Calculate layout rectangles
            int labelHeight = 20;
            int inputHeight = 32;
            int validationHeight = 16;
            int spacing = 4;

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.X, ctx.DrawingRect.Y,
                                         ctx.DrawingRect.Width, labelHeight);

            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X, ctx.HeaderRect.Bottom + spacing,
                                          ctx.DrawingRect.Width, inputHeight);

            ctx.FooterRect = new Rectangle(ctx.DrawingRect.X, ctx.ContentRect.Bottom + spacing,
                                         ctx.DrawingRect.Width, validationHeight);

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Draw main background
            using var bgBrush = new SolidBrush(Theme?.TextBoxBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Draw input field background
            using var inputBrush = new SolidBrush(Theme?.TextBoxBackColor ?? Color.White);
            using var inputPath = CreateRoundedPath(ctx.ContentRect, 4);
            g.FillPath(inputBrush, inputPath);

            // Draw input border
            Color borderColor = GetBorderColor(ctx);
            using var borderPen = new Pen(borderColor, 1);
            g.DrawPath(borderPen, inputPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw label
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var labelFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
                using var labelBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.Black);
                g.DrawString(ctx.Title, labelFont, labelBrush, ctx.HeaderRect,
                           new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            }

            // Draw input field content
            DrawInputField(g, ctx);

            // Draw validation message
            DrawValidationMessage(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw focus indicator if interactive
            if (ctx.IsInteractive && Owner.Focused)
            {
                using var focusPen = new Pen(ctx.AccentColor, 2);
                var focusRect = Rectangle.Inflate(ctx.ContentRect, -1, -1);
                using var focusPath = CreateRoundedPath(focusRect, 4);
                g.DrawPath(focusPen, focusPath);
            }
        }

        private void DrawInputField(Graphics g, WidgetContext ctx)
        {
            var fields = ctx.CustomData.ContainsKey("Fields") ? ctx.CustomData["Fields"] as List<FormField> : null;
            if (fields == null || fields.Count == 0) return;

            var field = fields.First();
            string displayValue = field.Value?.ToString() ?? "";
            bool isRequired = field.IsRequired;

            // Draw placeholder or value
            using var textFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            Color textColor = string.IsNullOrEmpty(displayValue) ?
                (Theme?.PlaceholderColor ?? Color.Gray) :
                (Theme?.TextBoxForeColor ?? Color.Black);

            using var textBrush = new SolidBrush(textColor);

            var textRect = Rectangle.Inflate(ctx.ContentRect, -8, 0);
            string textToDraw = string.IsNullOrEmpty(displayValue) ? field.Placeholder ?? "Enter value..." : displayValue;

            g.DrawString(textToDraw, textFont, textBrush, textRect,
                       new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

            // Draw required indicator
            if (isRequired)
            {
                using var requiredBrush = new SolidBrush(Color.Red);
                g.DrawString("*", textFont, requiredBrush,
                           new PointF(ctx.ContentRect.Right - 12, ctx.ContentRect.Top + 2));
            }
        }

        private void DrawValidationMessage(Graphics g, WidgetContext ctx)
        {
            var validationResults = ctx.CustomData.ContainsKey("ValidationResults") as List<ValidationResult>;
            var fields = ctx.CustomData.ContainsKey("Fields") ? ctx.CustomData["Fields"] as List<FormField> : null;

            if (validationResults == null || fields == null || fields.Count == 0) return;

            var field = fields.First();
            var validation = validationResults.FirstOrDefault(v => v.FieldName == field.Name);

            if (validation != null && !validation.IsValid)
            {
                using var validationFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var validationBrush = new SolidBrush(GetValidationColor(ctx, validation.Severity));
                g.DrawString(validation.Message, validationFont, validationBrush, ctx.FooterRect,
                           new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            }
        }

        private Color GetBorderColor(WidgetContext ctx)
        {
            var validationResults = ctx.CustomData.ContainsKey("ValidationResults") as List<ValidationResult>;
            var fields = ctx.CustomData.ContainsKey("Fields") ? ctx.CustomData["Fields"] as List<FormField> : null;

            if (validationResults == null || fields == null || fields.Count == 0)
                return Theme?.TextBoxBorderColor ?? Color.FromArgb(200, 200, 200);

            var field = fields.First();
            var validation = validationResults.FirstOrDefault(v => v.FieldName == field.Name);

            if (validation != null && !validation.IsValid)
                return GetValidationColor(ctx, validation.Severity);

            if (ctx.IsInteractive && Owner.Focused)
                return ctx.AccentColor;

            return Theme?.TextBoxBorderColor ?? Color.FromArgb(200, 200, 200);
        }

        private Color GetValidationColor(WidgetContext ctx, ValidationSeverity severity)
        {
            switch (severity)
            {
                case ValidationSeverity.Error:
                    return ctx.CustomData.ContainsKey("ErrorColor") ? (Color)ctx.CustomData["ErrorColor"] : Color.Red;
                case ValidationSeverity.Warning:
                    return ctx.CustomData.ContainsKey("WarningColor") ? (Color)ctx.CustomData["WarningColor"] : Color.Orange;
                default:
                    return ctx.CustomData.ContainsKey("ValidColor") ? (Color)ctx.CustomData["ValidColor"] : Color.Green;
            }
        }
    }
}