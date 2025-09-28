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
    /// ValidationPanel - Form section with validation display painter with enhanced visual presentation
    /// </summary>
    internal sealed class ValidationPanelPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ValidationPanelPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Form title and validation summary
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                32
            );
            
            // Validation messages area
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            // Form fields area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.IconRect.Bottom - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 2, offset: 1);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var fields = ctx.CustomData.ContainsKey("Fields") ? 
                (List<FormField>)ctx.CustomData["Fields"] : new List<FormField>();
            var validationResults = ctx.CustomData.ContainsKey("ValidationResults") ? 
                (List<ValidationResult>)ctx.CustomData["ValidationResults"] : new List<ValidationResult>();
            var errorColor = ctx.CustomData.ContainsKey("ErrorColor") ? (Color)ctx.CustomData["ErrorColor"] : Color.Red;
            var warningColor = ctx.CustomData.ContainsKey("WarningColor") ? (Color)ctx.CustomData["WarningColor"] : Color.Orange;
            var validColor = ctx.CustomData.ContainsKey("ValidColor") ? (Color)ctx.CustomData["ValidColor"] : Color.Green;

            // Draw title and validation summary
            DrawValidationHeader(g, ctx.HeaderRect, ctx.Title, validationResults, ctx.AccentColor, errorColor);
            
            // Draw validation messages
            DrawValidationMessages(g, ctx.IconRect, validationResults, errorColor, warningColor, validColor);
            
            // Draw simplified form fields
            DrawValidationFields(g, ctx.ContentRect, fields, validationResults, validColor, errorColor);
        }

        private void DrawValidationHeader(Graphics g, Rectangle rect, string title, List<ValidationResult> validationResults, 
            Color accentColor, Color errorColor)
        {
            // Count validation issues
            int errorCount = validationResults.Count(v => !v.IsValid);
            int totalFields = validationResults.Count;
            
            // Draw title
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(title, titleFont, titleBrush, rect.X, rect.Y);
            
            // Draw validation status
            using var statusFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            Color statusColor = errorCount == 0 ? accentColor : errorColor;
            using var statusBrush = new SolidBrush(statusColor);
            
            string statusText = errorCount == 0 ? 
                "? Form is valid" : 
                $"? {errorCount} error{(errorCount > 1 ? "s" : "")} found";
            
            var statusSize = g.MeasureString(statusText, statusFont);
            g.DrawString(statusText, statusFont, statusBrush, rect.Right - statusSize.Width, rect.Y);
            
            // Draw progress indicator
            DrawValidationProgress(g, rect, totalFields - errorCount, totalFields, accentColor, errorColor);
        }

        private void DrawValidationProgress(Graphics g, Rectangle rect, int validCount, int totalCount, 
            Color validColor, Color errorColor)
        {
            if (totalCount == 0) return;
            
            var progressRect = new Rectangle(rect.X, rect.Y + 20, rect.Width, 6);
            float progress = (float)validCount / totalCount;
            
            // Draw progress background
            using var bgBrush = new SolidBrush(Color.FromArgb(50, errorColor));
            using var bgPath = CreateRoundedPath(progressRect, 3);
            g.FillPath(bgBrush, bgPath);
            
            // Draw progress fill
            if (progress > 0)
            {
                var fillRect = new Rectangle(progressRect.X, progressRect.Y, 
                    (int)(progressRect.Width * progress), progressRect.Height);
                using var fillBrush = new SolidBrush(Color.FromArgb(150, validColor));
                using var fillPath = CreateRoundedPath(fillRect, 3);
                g.FillPath(fillBrush, fillPath);
            }
        }

        private void DrawValidationMessages(Graphics g, Rectangle rect, List<ValidationResult> validationResults, 
            Color errorColor, Color warningColor, Color validColor)
        {
            var errorMessages = validationResults.Where(v => !v.IsValid).Take(2).ToList();
            if (errorMessages.Count == 0)
            {
                // Draw success message
                DrawSuccessMessage(g, rect, validColor);
                return;
            }
            
            using var messageFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            int y = rect.Y;
            int lineHeight = 18;
            
            foreach (var error in errorMessages)
            {
                // Draw error icon
                using var iconBrush = new SolidBrush(errorColor);
                var iconRect = new Rectangle(rect.X, y + 2, 12, 12);
                g.FillEllipse(iconBrush, iconRect);
                
                using var iconFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var iconTextBrush = new SolidBrush(Color.White);
                var iconFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("!", iconFont, iconTextBrush, iconRect, iconFormat);
                
                // Draw error message
                using var messageBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                g.DrawString(error.Message, messageFont, messageBrush, rect.X + 18, y + 1);
                
                y += lineHeight;
                if (y >= rect.Bottom - lineHeight) break;
            }
        }

        private void DrawSuccessMessage(Graphics g, Rectangle rect, Color validColor)
        {
            // Draw success icon
            using var iconBrush = new SolidBrush(validColor);
            var iconRect = new Rectangle(rect.X, rect.Y + 2, 16, 16);
            g.FillEllipse(iconBrush, iconRect);
            
            using var iconFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var iconTextBrush = new SolidBrush(Color.White);
            var iconFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("?", iconFont, iconTextBrush, iconRect, iconFormat);
            
            // Draw success message
            using var messageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var messageBrush = new SolidBrush(Color.FromArgb(100, 150, 100));
            g.DrawString("All fields are valid and ready for submission", messageFont, messageBrush, rect.X + 22, rect.Y + 2);
        }

        private void DrawValidationFields(Graphics g, Rectangle rect, List<FormField> fields, 
            List<ValidationResult> validationResults, Color validColor, Color errorColor)
        {
            if (fields.Count == 0) return;

            int fieldHeight = 30;
            int maxFields = Math.Min(fields.Count, rect.Height / fieldHeight);
            
            using var labelFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            
            for (int i = 0; i < maxFields; i++)
            {
                var field = fields[i];
                var validation = validationResults.FirstOrDefault(v => v.FieldName == field.Name);
                
                var fieldRect = new Rectangle(
                    rect.X,
                    rect.Y + i * fieldHeight,
                    rect.Width,
                    fieldHeight - 2
                );
                
                // Draw validation status indicator
                Color statusColor = validation?.IsValid == true ? validColor : errorColor;
                string statusIcon = validation?.IsValid == true ? "?" : "?";
                
                using var statusBrush = new SolidBrush(statusColor);
                var statusRect = new Rectangle(fieldRect.X, fieldRect.Y + 6, 12, 12);
                g.FillEllipse(statusBrush, statusRect);
                
                using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var statusTextBrush = new SolidBrush(Color.White);
                var statusFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(statusIcon, statusFont, statusTextBrush, statusRect, statusFormat);
                
                // Draw field label
                g.DrawString(field.Label, labelFont, labelBrush, fieldRect.X + 18, fieldRect.Y + 2);
                
                // Draw field value preview
                string valuePreview = field.Value?.ToString() ?? "";
                if (field.Type == FormFieldType.Password && !string.IsNullOrEmpty(valuePreview))
                {
                    valuePreview = new string('�', valuePreview.Length);
                }
                else if (valuePreview.Length > 20)
                {
                    valuePreview = valuePreview.Substring(0, 17) + "...";
                }
                
                using var valueBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                g.DrawString(valuePreview, labelFont, valueBrush, fieldRect.X + 18, fieldRect.Y + 16);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw additional validation indicators
        }
    }
}