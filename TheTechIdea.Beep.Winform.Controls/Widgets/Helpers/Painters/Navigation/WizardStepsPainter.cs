using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// WizardSteps - Visualizes a wizard's steps with clear current/complete states.
    /// </summary>
    internal sealed class WizardStepsPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public WizardStepsPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + 8, ctx.DrawingRect.Y + 8,
                ctx.DrawingRect.Width - 16, ctx.DrawingRect.Height - 16);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var path = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, path);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.NavigationItems?.Cast<NavigationItem>().ToList() ?? CreateSampleSteps();

            // Support both CurrentIndex and ActiveIndex keys
            int currentIndex = ctx.CurrentIndex;

            if (!items.Any()) return;

            DrawSteps(g, ctx, items, currentIndex);
        }

        private List<NavigationItem> CreateSampleSteps()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Start", IsActive = true },
                new NavigationItem { Text = "Details", IsActive = false },
                new NavigationItem { Text = "Review", IsActive = false },
                new NavigationItem { Text = "Finish", IsActive = false },
            };
        }

        private void DrawSteps(Graphics g, WidgetContext ctx, List<NavigationItem> items, int currentIndex)
        {
            int stepRadius = 14;
            int stepDiameter = stepRadius * 2;
            int availableWidth = ctx.ContentRect.Width - stepDiameter * items.Count;
            int gap = items.Count > 1 ? availableWidth / (items.Count - 1) : 0;
            int centerY = ctx.ContentRect.Y + ctx.ContentRect.Height / 2 - 6;

            var primary = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            var complete = Color.FromArgb(76, 175, 80);
            var pending = Color.FromArgb(189, 189, 189);

            using var penComplete = new Pen(complete, 3);
            using var penPrimary = new Pen(primary, 3);
            using var penPending = new Pen(pending, 2);
            using var font = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);

            // Draw connectors first
            for (int i = 0; i < items.Count - 1; i++)
            {
                int x1 = ctx.ContentRect.X + i * (stepDiameter + gap) + stepDiameter;
                int x2 = ctx.ContentRect.X + (i + 1) * (stepDiameter + gap);
                var statePen = i < currentIndex ? penComplete : penPending;
                g.DrawLine(statePen, x1 + 2, centerY, x2 - 2, centerY);
            }

            // Draw steps
            for (int i = 0; i < items.Count; i++)
            {
                int cx = ctx.ContentRect.X + i * (stepDiameter + gap);
                var stepRect = new Rectangle(cx, centerY - stepRadius, stepDiameter, stepDiameter);

                bool isCompleted = i < currentIndex;
                bool isCurrent = i == currentIndex;

                if (isCompleted)
                {
                    using var b = new SolidBrush(complete);
                    g.FillEllipse(b, stepRect);
                    g.DrawEllipse(penComplete, stepRect);
                    // Check mark
                    var iconRect = new Rectangle(stepRect.X + 6, stepRect.Y + 6, stepDiameter - 12, stepDiameter - 12);
                    _imagePainter.DrawSvg(g, "check", iconRect, Color.White, 0.95f);
                }
                else if (isCurrent)
                {
                    using var b = new SolidBrush(primary);
                    g.FillEllipse(b, stepRect);
                    g.DrawEllipse(penPrimary, stepRect);
                    using var brush = new SolidBrush(Color.White);
                    using var numFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                    var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString((i + 1).ToString(), numFont, brush, stepRect, fmt);
                }
                else
                {
                    using var b = new SolidBrush(Color.FromArgb(245, 245, 245));
                    g.FillEllipse(b, stepRect);
                    g.DrawEllipse(penPending, stepRect);
                    using var brush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString((i + 1).ToString(), font, brush, stepRect, fmt);
                }

                // Label
                var label = items[i].Text ?? string.Empty;
                if (!string.IsNullOrEmpty(label))
                {
                    var labelColor = isCurrent ? primary : (isCompleted ? complete : Color.FromArgb(120, Color.Black));
                    using var labelBrush = new SolidBrush(labelColor);
                    var labelRect = new Rectangle(stepRect.X - 20, stepRect.Bottom + 6, stepDiameter + 40, 18);
                    var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(label, font, labelBrush, labelRect, fmt);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // No-op for now
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}
