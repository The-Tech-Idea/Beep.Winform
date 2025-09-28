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
    /// StepIndicator - Multi-step process indicator with enhanced visual presentation
    /// </summary>
    internal sealed class StepIndicatorPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public StepIndicatorPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.CustomData.ContainsKey("Items") ? 
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleSteps();
            
            int currentIndex = ctx.CustomData.ContainsKey("CurrentIndex") ? 
                (int)ctx.CustomData["CurrentIndex"] : 1;
            
            if (!items.Any()) return;
            
            DrawModernStepIndicator(g, ctx, items, currentIndex);
        }

        private List<NavigationItem> CreateSampleSteps()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Setup", IsActive = false },
                new NavigationItem { Text = "Configuration", IsActive = true },
                new NavigationItem { Text = "Review", IsActive = false },
                new NavigationItem { Text = "Complete", IsActive = false }
            };
        }

        private void DrawModernStepIndicator(Graphics g, WidgetContext ctx, List<NavigationItem> items, int currentIndex)
        {
            int stepSize = 32;
            int stepSpacing = (ctx.ContentRect.Width - stepSize * items.Count) / Math.Max(items.Count - 1, 1);
            int y = ctx.ContentRect.Y + ctx.ContentRect.Height / 2;
            
            var primaryColor = ctx.AccentColor ?? Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            var successColor = Color.FromArgb(76, 175, 80);
            var pendingColor = Color.FromArgb(189, 189, 189);
            
            using var completedBrush = new SolidBrush(successColor);
            using var currentBrush = new SolidBrush(primaryColor);
            using var pendingBrush = new SolidBrush(pendingColor);
            using var textFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Medium);
            using var completedPen = new Pen(successColor, 2);
            using var currentPen = new Pen(primaryColor, 3);
            using var pendingPen = new Pen(pendingColor, 2);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int x = ctx.ContentRect.X + i * (stepSize + stepSpacing);
                
                // Step circle with shadow
                var shadowRect = new Rectangle(x + 1, y - stepSize / 2 + 1, stepSize, stepSize);
                using var shadowBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
                g.FillEllipse(shadowBrush, shadowRect);
                
                var stepRect = new Rectangle(x, y - stepSize / 2, stepSize, stepSize);
                
                bool isCompleted = i < currentIndex;
                bool isCurrent = i == currentIndex;
                
                if (isCompleted)
                {
                    g.FillEllipse(completedBrush, stepRect);
                    g.DrawEllipse(completedPen, stepRect);
                    
                    // Check icon
                    var iconRect = new Rectangle(stepRect.X + 8, stepRect.Y + 8, 16, 16);
                    _imagePainter.DrawSvg(g, "check", iconRect, Color.White, 1.0f);
                }
                else if (isCurrent)
                {
                    g.FillEllipse(currentBrush, stepRect);
                    g.DrawEllipse(currentPen, stepRect);
                    
                    // Current step number
                    using var stepBrush = new SolidBrush(Color.White);
                    using var stepFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                    string stepText = (i + 1).ToString();
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(stepText, stepFont, stepBrush, stepRect, format);
                }
                else
                {
                    g.FillEllipse(pendingBrush, stepRect);
                    g.DrawEllipse(pendingPen, stepRect);
                    
                    // Pending step number
                    using var stepBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    using var stepFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
                    string stepText = (i + 1).ToString();
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(stepText, stepFont, stepBrush, stepRect, format);
                }
                
                // Connection line (except for last step)
                if (i < items.Count - 1)
                {
                    var lineColor = i < currentIndex ? successColor : pendingColor;
                    using var linePen = new Pen(lineColor, 3);
                    int lineY = y;
                    g.DrawLine(linePen, stepRect.Right + 2, lineY, stepRect.Right + stepSpacing - 2, lineY);
                }
                
                // Step label
                if (!string.IsNullOrEmpty(item.Text))
                {
                    using var labelBrush = new SolidBrush(isCurrent ? primaryColor : 
                        isCompleted ? successColor : Color.FromArgb(120, Color.Black));
                    using var labelFont = new Font(Owner.Font.FontFamily, 8f, 
                        isCurrent || isCompleted ? FontStyle.Medium : FontStyle.Regular);
                    
                    var labelRect = new Rectangle(x - 20, stepRect.Bottom + 8, stepSize + 40, 20);
                    var labelFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(item.Text, labelFont, labelBrush, labelRect, labelFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw progress percentage
            var items = ctx.CustomData.ContainsKey("Items") ? 
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleSteps();
            int currentIndex = ctx.CustomData.ContainsKey("CurrentIndex") ? 
                (int)ctx.CustomData["CurrentIndex"] : 1;
                
            float progress = (float)currentIndex / Math.Max(items.Count - 1, 1);
            
            if (ctx.CustomData.ContainsKey("ShowProgress") && (bool)ctx.CustomData["ShowProgress"])
            {
                using var progressFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Medium);
                using var progressBrush = new SolidBrush(Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243));
                
                var progressText = $"{progress:P0}";
                var progressRect = new Rectangle(ctx.ContentRect.Right - 50, ctx.ContentRect.Y, 50, 20);
                var format = new StringFormat { Alignment = StringAlignment.Far };
                g.DrawString(progressText, progressFont, progressBrush, progressRect, format);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}