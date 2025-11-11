using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Form
{
    /// <summary>
    /// FieldGroup - Related inputs grouped together painter
    /// </summary>
    internal sealed class FieldGroupPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Form title header
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            // Field group content area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.HeaderRect.Bottom - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var fields = ctx.Fields ?? new List<FormField>();
            var validationResults = ctx.ValidationResults ?? new List<ValidationResult>();
            var showValidation = ctx.ShowValidation;
            var showRequired = ctx.ShowRequired;
            var validColor = ctx.ValidColor;
            var errorColor = ctx.ErrorColor;

            // Draw form title
            DrawFormHeader(g, ctx.HeaderRect, ctx.Title, ctx.Value, ctx.AccentColor);
            
            // Draw field group
            DrawFieldGroup(g, ctx.ContentRect, fields, validationResults, showValidation, showRequired, validColor, errorColor);
        }

        private void DrawFormHeader(Graphics g, Rectangle rect, string title, string subtitle, Color accentColor)
        {
            // Draw title
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(title, titleFont, titleBrush, rect.X, rect.Y);
            
            // Draw subtitle if provided
            if (!string.IsNullOrEmpty(subtitle))
            {
                using var subtitleFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
                using var subtitleBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                g.DrawString(subtitle, subtitleFont, subtitleBrush, rect.X, rect.Y + 16);
            }
        }

        private void DrawFieldGroup(Graphics g, Rectangle rect, List<FormField> fields, List<ValidationResult> validationResults, 
            bool showValidation, bool showRequired, Color validColor, Color errorColor)
        {
            if (fields.Count == 0) return;

            int fieldHeight = 45;
            int maxFields = Math.Min(fields.Count, rect.Height / fieldHeight);
            
            using var labelFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Color.FromArgb(160, Color.Black));
            using var valueBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            using var inputBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
            using var borderPen = new Pen(Color.FromArgb(200, 200, 200), 1);
            using var requiredBrush = new SolidBrush(errorColor);
            
            for (int i = 0; i < maxFields; i++)
            {
                var field = fields[i];
                var validation = validationResults.FirstOrDefault(v => v.FieldName == field.Name);
                
                var fieldRect = new Rectangle(
                    rect.X,
                    rect.Y + i * fieldHeight,
                    rect.Width,
                    fieldHeight - 5
                );
                
                // Draw field background
                DrawFieldBackground(g, fieldRect, field, validation, showValidation, validColor, errorColor);
                
                // Draw label with required indicator
                DrawFieldLabel(g, fieldRect, field, showRequired, labelFont, labelBrush, requiredBrush);
                
                // Draw input area
                DrawFieldInput(g, fieldRect, field, labelFont, valueBrush, inputBrush, borderPen);
                
                // Draw validation indicator
                if (showValidation && validation != null)
                {
                    DrawValidationIndicator(g, fieldRect, validation, validColor, errorColor);
                }
            }
        }

        private void DrawFieldBackground(Graphics g, Rectangle fieldRect, FormField field, ValidationResult validation, 
            bool showValidation, Color validColor, Color errorColor)
        {
            Color bgColor = Color.Transparent;
            
            if (showValidation && validation != null)
            {
                if (!validation.IsValid)
                {
                    bgColor = Color.FromArgb(20, errorColor);
                }
                else if (validation.IsValid && !string.IsNullOrEmpty(field.Value?.ToString()))
                {
                    bgColor = Color.FromArgb(20, validColor);
                }
            }
            
            if (bgColor != Color.Transparent)
            {
                using var bgBrush = new SolidBrush(bgColor);
                using var bgPath = CreateRoundedPath(fieldRect, 6);
                g.FillPath(bgBrush, bgPath);
            }
        }

        private void DrawFieldLabel(Graphics g, Rectangle fieldRect, FormField field, bool showRequired, 
            Font labelFont, Brush labelBrush, Brush requiredBrush)
        {
            var labelRect = new Rectangle(fieldRect.X + 4, fieldRect.Y + 2, fieldRect.Width - 8, 16);
            
            string labelText = field.Label;
            if (showRequired && field.IsRequired)
            {
                labelText += " *";
            }
            
            // Draw label
            var labelSize = TextUtils.MeasureText(g,field.Label, labelFont);
            g.DrawString(field.Label, labelFont, labelBrush, labelRect);
            
            // Draw required asterisk in red if needed
            if (showRequired && field.IsRequired)
            {
                g.DrawString(" *", labelFont, requiredBrush, labelRect.X + labelSize.Width - 5, labelRect.Y);
            }
        }

        private void DrawFieldInput(Graphics g, Rectangle fieldRect, FormField field, Font font, 
            Brush valueBrush, Brush inputBrush, Pen borderPen)
        {
            var inputRect = new Rectangle(
                fieldRect.X + 4,
                fieldRect.Y + 18,
                fieldRect.Width - 32,
                20
            );
            
            // Draw input background
            using var inputPath = CreateRoundedPath(inputRect, 4);
            g.FillPath(inputBrush, inputPath);
            g.DrawPath(borderPen, inputPath);
            
            // Draw field value or placeholder
            string displayText = field.Value?.ToString() ?? field.Placeholder;
            var textBrush = field.Value != null ? valueBrush : new SolidBrush(Color.FromArgb(120, Color.Gray));
            
            if (!string.IsNullOrEmpty(displayText))
            {
                var textRect = new Rectangle(inputRect.X + 6, inputRect.Y + 2, inputRect.Width - 12, inputRect.Height - 4);
                var textFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                
                // Handle password field display
                if (field.Type == FormFieldType.Password && field.Value != null)
                {
                    displayText = new string('�', field.Value.ToString().Length);
                }
                
                g.DrawString(displayText, font, textBrush, textRect, textFormat);
            }
            
            if (field.Value == null)
            {
                textBrush.Dispose();
            }
        }

        private void DrawValidationIndicator(Graphics g, Rectangle fieldRect, ValidationResult validation, 
            Color validColor, Color errorColor)
        {
            var indicatorRect = new Rectangle(
                fieldRect.Right - 20,
                fieldRect.Y + 20,
                16, 16
            );
            
            Color indicatorColor = validation.IsValid ? validColor : errorColor;
            string indicatorText = validation.IsValid ? "?" : "?";
            
            // Draw indicator circle
            using var indicatorBrush = new SolidBrush(Color.FromArgb(150, indicatorColor));
            g.FillEllipse(indicatorBrush, indicatorRect);
            
            // Draw indicator symbol
            using var symbolFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var symbolBrush = new SolidBrush(Color.White);
            var symbolFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(indicatorText, symbolFont, symbolBrush, indicatorRect, symbolFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw form progress or additional indicators
        }
    }
}