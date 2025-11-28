using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Form
{
    /// <summary>
    /// FormSection - Organized form section with title painter with enhanced visual presentation
    /// </summary>
    internal sealed class FormSectionPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public FormSectionPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            // Adjust for padding
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            // Calculate layout rectangles
            int titleHeight = 28;
            int contentSpacing = 8;
            int fieldHeight = 32;
            int fieldSpacing = 6;

            // Title area
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.X, ctx.DrawingRect.Y,
                                         ctx.DrawingRect.Width, titleHeight);

            // Content area for fields
            int contentTop = ctx.HeaderRect.Bottom + contentSpacing;
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X, contentTop,
                                          ctx.DrawingRect.Width,
                                          ctx.DrawingRect.Bottom - contentTop);

            // Calculate field positions
            var fields = ctx.Fields;
            if (fields != null && fields.Count > 0)
            {
                ctx.InlineFieldData ??= new Dictionary<string, object>();
                
                int currentY = ctx.ContentRect.Y;
                for (int i = 0; i < fields.Count; i++)
                {
                    var fieldRect = new Rectangle(ctx.ContentRect.X, currentY,
                                                ctx.ContentRect.Width, fieldHeight);

                    // Store field rectangles for drawing
                    ctx.InlineFieldData[$"FieldRect_{i}"] = fieldRect;
                    currentY += fieldHeight + fieldSpacing;
                }
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Draw main section background
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.Empty);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Draw section border
            using var borderPen = new Pen(Theme?.CardBorderColor ?? Theme?.BorderColor ?? Color.Empty, 1);
            g.DrawPath(borderPen, bgPath);

            // Draw title background separator
            using var titleBgBrush = new SolidBrush(Theme?.PanelBackColor ?? Color.Empty);
            g.FillRectangle(titleBgBrush, ctx.HeaderRect);

            // Draw separator line under title
            using var separatorPen = new Pen(ctx.AccentColor, 2);
            int separatorY = ctx.HeaderRect.Bottom - 1;
            g.DrawLine(separatorPen, ctx.HeaderRect.Left + 4, separatorY,
                     ctx.HeaderRect.Right - 4, separatorY);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw section title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.CardTextForeColor ?? Color.Empty);
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect,
                           new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            }

            // Draw form fields
            DrawFormFields(g, ctx);

            // Draw section description if available
            var description = ctx.Description;
            if (!string.IsNullOrEmpty(description))
            {
                using var descFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
                using var descBrush = new SolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));
                var descRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Bottom - 20,
                                           ctx.ContentRect.Width, 16);
                g.DrawString(description, descFont, descBrush, descRect);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw validation summary if there are errors
            var validationResults = ctx.ValidationResults;
            if (validationResults != null && validationResults.Any(v => !v.IsValid))
            {
                int errorCount = validationResults.Count(v => !v.IsValid);
                using var errorFont = new Font(Owner?.Font?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
                using var errorBrush = new SolidBrush(Color.Red);

                string errorText = $"{errorCount} field{(errorCount != 1 ? "s" : "")} need attention";
                var errorRect = new Rectangle(ctx.DrawingRect.Right - 120, ctx.HeaderRect.Top + 4, 110, 16);
                g.DrawString(errorText, errorFont, errorBrush, errorRect,
                           new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
            }

            // Draw progress indicator if applicable
            var showProgress = ctx.ShowProgress;
            if (showProgress)
            {
                var currentStep = ctx.CurrentStep;
                var totalSteps = ctx.TotalSteps;

                DrawProgressIndicator(g, ctx, currentStep, totalSteps);
            }
        }

        private void DrawFormFields(Graphics g, WidgetContext ctx)
        {
            var fields = ctx.Fields;
            var validationResults = ctx.ValidationResults;

            if (fields == null) return;

            for (int i = 0; i < fields.Count; i++)
            {
                if (ctx.InlineFieldData == null || !ctx.InlineFieldData.ContainsKey($"FieldRect_{i}")) continue;

                var field = fields[i];
                var fieldRect = (Rectangle)ctx.InlineFieldData[$"FieldRect_{i}"];

                // Draw field label
                using var labelFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var labelBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.Black);
                var labelRect = new Rectangle(fieldRect.X, fieldRect.Y, fieldRect.Width / 3, fieldRect.Height);
                g.DrawString(field.Label, labelFont, labelBrush, labelRect,
                           new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

                // Draw field value/input area
                var inputRect = new Rectangle(fieldRect.X + fieldRect.Width / 3 + 8, fieldRect.Y,
                                            fieldRect.Width * 2 / 3 - 8, fieldRect.Height);

                // Draw input background
                Color inputBgColor = Theme?.TextBoxBackColor ?? Color.Empty;
                using var inputBrush = new SolidBrush(inputBgColor);
                using var inputPath = CreateRoundedPath(inputRect, 3);
                g.FillPath(inputBrush, inputPath);

                // Draw input border with validation state
                Color borderColor = GetFieldBorderColor(ctx, field, validationResults);
                using var borderPen = new Pen(borderColor, 1);
                g.DrawPath(borderPen, inputPath);

                // Draw field value
                string displayValue = field.Value?.ToString() ?? "";
                string textToDraw = string.IsNullOrEmpty(displayValue) ? field.Placeholder ?? "" : displayValue;

                using var valueFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                Color textColor = string.IsNullOrEmpty(displayValue) ?
                    Color.Gray :
                    (Theme?.TextBoxForeColor ?? Color.Black);
                using var valueBrush = new SolidBrush(textColor);

                var textRect = Rectangle.Inflate(inputRect, -6, 0);
                g.DrawString(textToDraw, valueFont, valueBrush, textRect,
                           new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

                // Draw required indicator
                if (field.IsRequired)
                {
                    using var requiredBrush = new SolidBrush(Color.Red);
                    g.DrawString("*", labelFont, requiredBrush,
                               new PointF(labelRect.Right - 8, labelRect.Top + 2));
                }
            }
        }

        private void DrawProgressIndicator(Graphics g, WidgetContext ctx, int currentStep, int totalSteps)
        {
            if (totalSteps <= 1) return;

            int indicatorWidth = 60;
            int indicatorHeight = 4;
            var indicatorRect = new Rectangle(ctx.DrawingRect.Right - indicatorWidth - 12,
                                            ctx.HeaderRect.Top + 6, indicatorWidth, indicatorHeight);

            // Draw background
            using var bgBrush = new SolidBrush(Theme?.PanelBackColor ?? Color.Empty);
            g.FillRectangle(bgBrush, indicatorRect);

            // Draw progress
            float progressRatio = Math.Min((float)currentStep / totalSteps, 1.0f);
            int progressWidth = (int)(indicatorWidth * progressRatio);
            var progressRect = new Rectangle(indicatorRect.X, indicatorRect.Y, progressWidth, indicatorHeight);
            using var progressBrush = new SolidBrush(ctx.AccentColor);
            g.FillRectangle(progressBrush, progressRect);

            // Draw step text
            using var stepFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
            using var stepBrush = new SolidBrush(Color.FromArgb(120, Theme?.CardTextForeColor ?? Color.Empty));
            string stepText = $"{currentStep}/{totalSteps}";
            g.DrawString(stepText, stepFont, stepBrush,
                       new PointF(indicatorRect.Right + 4, indicatorRect.Top - 1));
        }

        private Color GetFieldBorderColor(WidgetContext ctx, FormField field, List<ValidationResult> validationResults)
        {
            if (validationResults != null)
            {
                var validation = validationResults.FirstOrDefault(v => v.FieldName == field.Name);
                if (validation != null && !validation.IsValid)
                {
                    switch (validation.Severity)
                    {
                        case ValidationSeverity.Error:
                            return ctx.ErrorColor;
                        case ValidationSeverity.Warning:
                            return ctx.WarningColor;
                        default:
                            return ctx.ValidColor;
                    }
                }
            }

            return Theme?.TextBoxBorderColor ?? Color.Empty;
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}