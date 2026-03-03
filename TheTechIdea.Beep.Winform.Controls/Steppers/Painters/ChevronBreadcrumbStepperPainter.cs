using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public sealed class ChevronBreadcrumbStepperPainter : IStepperPainter
    {
        private Font _labelFont;
        private Font _stepFont;
        private IBeepTheme _theme;

        public string Name => "ChevronBreadcrumb";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _stepFont = stepFont;
            _labelFont = labelFont;
            _theme = theme;
        }

        public StepperLayoutResult ComputeLayout(Rectangle clientRect, IReadOnlyList<StepModel> steps, Orientation orientation, StepperStyleConfig styleConfig)
        {
            var result = new StepperLayoutResult { ContentRect = clientRect };
            if (steps == null || steps.Count == 0)
            {
                return result;
            }

            int count = steps.Count;
            int totalLen = orientation == Orientation.Horizontal ? clientRect.Width : clientRect.Height;
            int crossLen = orientation == Orientation.Horizontal ? clientRect.Height : clientRect.Width;
            int stepLen = totalLen / System.Math.Max(1, count);

            for (int i = 0; i < count; i++)
            {
                int x = orientation == Orientation.Horizontal ? clientRect.Left + (i * stepLen) : clientRect.Left;
                int y = orientation == Orientation.Horizontal ? clientRect.Top : clientRect.Top + (i * stepLen);
                var rect = orientation == Orientation.Horizontal
                    ? new Rectangle(x, y, stepLen, crossLen)
                    : new Rectangle(x, y, crossLen, stepLen);
                result.StepRects.Add(rect);
            }

            return result;
        }

        public void Paint(Graphics g, StepPainterContext context)
        {
            if (context?.StepRects == null || context.StepRects.Count == 0)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            for (int i = 0; i < context.StepRects.Count; i++)
            {
                PaintStep(g, context, i, context.StepRects[i]);
            }
        }

        public void PaintStep(Graphics g, StepPainterContext context, int stepIndex, Rectangle stepRect)
        {
            var step = context.Steps[stepIndex];
            int crossLen = context.Orientation == Orientation.Horizontal ? stepRect.Height : stepRect.Width;
            int stepLen = context.Orientation == Orientation.Horizontal ? stepRect.Width : stepRect.Height;
            int arrowSize = System.Math.Max(4, crossLen / 4);
            bool first = stepIndex == 0;
            bool last = stepIndex == context.StepRects.Count - 1;

            Point[] pts;
            if (context.Orientation == Orientation.Horizontal)
            {
                int x = stepRect.Left;
                int y = stepRect.Top;
                if (first)
                {
                    pts = new[]
                    {
                        new Point(x, y),
                        new Point(x + stepLen - arrowSize, y),
                        new Point(x + stepLen, y + crossLen / 2),
                        new Point(x + stepLen - arrowSize, y + crossLen),
                        new Point(x, y + crossLen)
                    };
                }
                else if (last)
                {
                    pts = new[]
                    {
                        new Point(x, y),
                        new Point(x + stepLen, y),
                        new Point(x + stepLen, y + crossLen),
                        new Point(x, y + crossLen),
                        new Point(x + arrowSize, y + crossLen / 2)
                    };
                }
                else
                {
                    pts = new[]
                    {
                        new Point(x, y),
                        new Point(x + stepLen - arrowSize, y),
                        new Point(x + stepLen, y + crossLen / 2),
                        new Point(x + stepLen - arrowSize, y + crossLen),
                        new Point(x, y + crossLen),
                        new Point(x + arrowSize, y + crossLen / 2)
                    };
                }
            }
            else
            {
                pts = new[]
                {
                    new Point(stepRect.Left, stepRect.Top),
                    new Point(stepRect.Right, stepRect.Top),
                    new Point(stepRect.Right, stepRect.Bottom),
                    new Point(stepRect.Left, stepRect.Bottom)
                };
            }

            Color fill = step.State switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
            };

            using (var br = new SolidBrush(fill))
            {
                g.FillPolygon(br, pts);
            }

            if (stepIndex == context.HoveredIndex)
            {
                using var hoverOverlay = new SolidBrush(Color.FromArgb(20, Color.White));
                g.FillPolygon(hoverOverlay, pts);
            }

            using (var pen = new Pen((context.Theme ?? _theme)?.ShadowColor ?? Color.Gray, StepperAccessibilityHelpers.GetAccessibleBorderWidth(1)))
            {
                g.DrawPolygon(pen, pts);
            }

            if (stepIndex == context.FocusedIndex)
            {
                using var focusPen = new Pen((context.Theme ?? _theme)?.PrimaryColor ?? Color.DodgerBlue, 2f);
                g.DrawPolygon(focusPen, pts);
            }

            if (context.Orientation == Orientation.Horizontal)
            {
                string headerText = step.Text ?? string.Empty;
                string subText = step.Subtitle ?? string.Empty;
                Font headerFont = _labelFont ?? _stepFont ?? SystemFonts.DefaultFont;
                Font subFont = _stepFont ?? _labelFont ?? SystemFonts.DefaultFont;
                var headerSize = g.MeasureString(headerText, headerFont);
                var subSize = g.MeasureString(subText, subFont);
                float totalTextHeight = headerSize.Height + subSize.Height;
                float startY = stepRect.Top + (stepRect.Height - totalTextHeight) / 2f;
                float headerX = stepRect.Left + (stepRect.Width - headerSize.Width) / 2f;
                float subX = stepRect.Left + (stepRect.Width - subSize.Width) / 2f;
                Color fore = StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
                using var brush = new SolidBrush(fore);
                g.DrawString(headerText, headerFont, brush, headerX, startY);
                g.DrawString(subText, subFont, brush, subX, startY + headerSize.Height);
            }
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            // Breadcrumb chevrons are edge-connected; no explicit connector line needed.
        }
    }
}
