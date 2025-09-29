using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// FormStep - Multi-step form progression painter with enhanced visual presentation
    /// </summary>
    internal sealed class FormStepPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public FormStepPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Step progress indicator
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            // Step title and description
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                32
            );
            
            // Step content area
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
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            var currentStep = ctx.CustomData.ContainsKey("CurrentStep") ? (int)ctx.CustomData["CurrentStep"] : 2;
            var totalSteps = ctx.CustomData.ContainsKey("TotalSteps") ? (int)ctx.CustomData["TotalSteps"] : 4;
            var fields = ctx.CustomData.ContainsKey("Fields") ? 
                (List<FormField>)ctx.CustomData["Fields"] : new List<FormField>();
            var description = ctx.CustomData.ContainsKey("Description") ? 
                ctx.CustomData["Description"].ToString() : "Complete your personal information";

            // Draw modern step progress
            DrawModernStepProgress(g, ctx.HeaderRect, currentStep, totalSteps);
            
            // Draw step info with icons
            DrawStepInfo(g, ctx.IconRect, ctx.Title ?? "Step Information", ctx.Value, description, currentStep, totalSteps);
            
            // Draw step content preview
            DrawStepContent(g, ctx.ContentRect, fields, ctx.AccentColor);
        }

        private void DrawModernStepProgress(Graphics g, Rectangle rect, int currentStep, int totalSteps)
        {
            if (totalSteps <= 1) return;
            
            var progressRect = Rectangle.Inflate(rect, -10, -5);
            float stepWidth = (float)progressRect.Width / totalSteps;
            int stepSize = 32;
            int y = progressRect.Y + 4;
            
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            var completedColor = Color.FromArgb(76, 175, 80);
            var futureColor = Color.FromArgb(200, 200, 200);
            
            // Draw progress line background
            var lineRect = new Rectangle(progressRect.X + stepSize/2, y + stepSize/2 - 2, 
                progressRect.Width - stepSize, 4);
            using var bgBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
            g.FillRoundedRectangle(bgBrush, lineRect, 2);
            
            // Draw completed progress line
            if (currentStep > 1)
            {
                float completedWidth = ((float)(currentStep - 1) / (totalSteps - 1)) * lineRect.Width;
                var completedRect = new Rectangle(lineRect.X, lineRect.Y, (int)completedWidth, lineRect.Height);
                using var completedBrush = new SolidBrush(completedColor);
                g.FillRoundedRectangle(completedBrush, completedRect, 2);
            }
            
            // Draw step circles with icons
            for (int i = 1; i <= totalSteps; i++)
            {
                float x = progressRect.X + (i - 1) * stepWidth + (stepWidth - stepSize) / 2;
                var stepRect = new Rectangle((int)x, y, stepSize, stepSize);
                
                Color stepColor = i <= currentStep ? (i < currentStep ? completedColor : primaryColor) : futureColor;
                
                // Step circle
                using var stepBrush = new SolidBrush(stepColor);
                g.FillEllipse(stepBrush, stepRect);
                
                // Step icon
                var iconRect = Rectangle.Inflate(stepRect, -8, -8);
                string iconName = i <= currentStep ? (i < currentStep ? "check" : "user") : "circle";
                var iconColor = i <= currentStep ? Color.White : Color.FromArgb(120, Color.Gray);
                _imagePainter.DrawSvg(g, iconName, iconRect, iconColor, 0.9f);
                
                // Step number (if not completed)
                if (i > currentStep)
                {
                    using var stepFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
                    using var stepTextBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(i.ToString(), stepFont, stepTextBrush, stepRect, format);
                }
            }
        }

        private void DrawStepProgress(Graphics g, Rectangle rect, int currentStep, int totalSteps, Color accentColor)
        {
            if (totalSteps <= 1) return;
            
            float stepWidth = (float)rect.Width / totalSteps;
            int stepRadius = 12;
            int y = rect.Y + 8;
            
            using var completedBrush = new SolidBrush(accentColor);
            using var currentBrush = new SolidBrush(Color.FromArgb(200, accentColor));
            using var futureBrush = new SolidBrush(Color.FromArgb(220, 220, 220));
            using var linePen = new Pen(Color.FromArgb(200, 200, 200), 2);
            using var completedLinePen = new Pen(accentColor, 2);
            
            // Draw connecting lines
            for (int i = 0; i < totalSteps - 1; i++)
            {
                float x1 = rect.X + (i + 0.5f) * stepWidth + stepRadius;
                float x2 = rect.X + (i + 1.5f) * stepWidth - stepRadius;
                
                var pen = i < currentStep - 1 ? completedLinePen : linePen;
                g.DrawLine(pen, x1, y, x2, y);
            }
            
            // Draw step circles
            for (int i = 1; i <= totalSteps; i++)
            {
                float x = rect.X + (i - 0.5f) * stepWidth;
                var circleRect = new Rectangle((int)(x - stepRadius), y - stepRadius, stepRadius * 2, stepRadius * 2);
                
                Brush brush;
                if (i < currentStep)
                    brush = completedBrush;
                else if (i == currentStep)
                    brush = currentBrush;
                else
                    brush = futureBrush;
                
                g.FillEllipse(brush, circleRect);
                
                // Draw step number or checkmark
                using var stepFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
                using var stepTextBrush = new SolidBrush(Color.White);
                var stepFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                
                string stepText = i < currentStep ? "?" : i.ToString();
                g.DrawString(stepText, stepFont, stepTextBrush, circleRect, stepFormat);
            }
        }

        private void DrawStepInfo(Graphics g, Rectangle rect, string title, string subtitle, string description, 
            int currentStep, int totalSteps)
        {
            // Draw step title
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            string stepTitle = $"Step {currentStep}: {title}";
            g.DrawString(stepTitle, titleFont, titleBrush, rect.X, rect.Y);
            
            // Draw step subtitle
            if (!string.IsNullOrEmpty(subtitle))
            {
                using var subtitleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var subtitleBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                g.DrawString(subtitle, subtitleFont, subtitleBrush, rect.X, rect.Y + 16);
            }
            
            // Draw progress text
            using var progressFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var progressBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            string progressText = $"{currentStep} of {totalSteps} steps";
            var progressSize = g.MeasureString(progressText, progressFont);
            g.DrawString(progressText, progressFont, progressBrush, rect.Right - progressSize.Width, rect.Y + 2);
        }

        private void DrawStepContent(Graphics g, Rectangle rect, List<FormField> fields, Color accentColor)
        {
            if (fields.Count == 0)
            {
                // Draw placeholder content
                DrawPlaceholderContent(g, rect, accentColor);
                return;
            }
            
            // Draw step fields preview
            int fieldHeight = 28;
            int maxFields = Math.Min(fields.Count, rect.Height / fieldHeight);
            
            using var labelFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Color.FromArgb(160, Color.Black));
            
            for (int i = 0; i < maxFields; i++)
            {
                var field = fields[i];
                var fieldRect = new Rectangle(
                    rect.X,
                    rect.Y + i * fieldHeight,
                    rect.Width,
                    fieldHeight - 4
                );
                
                // Draw field indicator
                using var indicatorBrush = new SolidBrush(Color.FromArgb(150, accentColor));
                var indicatorRect = new Rectangle(fieldRect.X, fieldRect.Y + 8, 4, 12);
                g.FillRectangle(indicatorBrush, indicatorRect);
                
                // Draw field name
                g.DrawString(field.Label, labelFont, labelBrush, fieldRect.X + 12, fieldRect.Y + 2);
                
                // Draw field status
                string status = field.Value != null ? "Completed" : "Pending";
                using var statusBrush = new SolidBrush(field.Value != null ? Color.FromArgb(100, 150, 100) : Color.FromArgb(100, Color.Black));
                g.DrawString(status, labelFont, statusBrush, fieldRect.X + 12, fieldRect.Y + 16);
            }
        }

        private void DrawPlaceholderContent(Graphics g, Rectangle rect, Color accentColor)
        {
            // Draw step placeholder
            using var placeholderFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Italic);
            using var placeholderBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
            var placeholderFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            
            string placeholderText = "Step content will appear here...";
            g.DrawString(placeholderText, placeholderFont, placeholderBrush, rect, placeholderFormat);
            
            // Draw step icon
            var iconRect = new Rectangle(rect.X + rect.Width / 2 - 16, rect.Y + 20, 32, 32);
            using var iconBrush = new SolidBrush(Color.FromArgb(100, accentColor));
            g.FillEllipse(iconBrush, iconRect);
            
            using var iconFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
            using var iconTextBrush = new SolidBrush(Color.White);
            var iconFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("??", iconFont, iconTextBrush, iconRect, iconFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Navigation buttons
            var currentStep = ctx.CustomData.ContainsKey("CurrentStep") ? (int)ctx.CustomData["CurrentStep"] : 2;
            var totalSteps = ctx.CustomData.ContainsKey("TotalSteps") ? (int)ctx.CustomData["TotalSteps"] : 4;
            
            var buttonRect = new Rectangle(ctx.DrawingRect.Right - 100, ctx.DrawingRect.Bottom - 40, 90, 32);
            
            if (currentStep < totalSteps)
            {
                // Next button
                using var nextBrush = new SolidBrush(Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243));
                g.FillRoundedRectangle(nextBrush, buttonRect, 6);
                
                var iconRect = new Rectangle(buttonRect.X + 8, buttonRect.Y + 8, 16, 16);
                _imagePainter.DrawSvg(g, "arrow-right", iconRect, Color.White, 0.9f);
                
                using var nextFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var nextTextBrush = new SolidBrush(Color.White);
                var textRect = new Rectangle(iconRect.Right + 4, buttonRect.Y, 
                    buttonRect.Width - iconRect.Width - 12, buttonRect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Next", nextFont, nextTextBrush, textRect, format);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}