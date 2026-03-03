using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public sealed class GradientMaterialStepperPainter : IStepperPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _stepFont;

        public string Name => "GradientMaterial";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _owner = owner;
            _theme = theme;
            _stepFont = stepFont ?? labelFont ?? numberFont;
        }

        public StepperLayoutResult ComputeLayout(Rectangle clientRect, IReadOnlyList<StepModel> steps, Orientation orientation, StepperStyleConfig styleConfig)
        {
            var result = new StepperLayoutResult { ContentRect = clientRect };
            if (steps == null || steps.Count == 0)
            {
                return result;
            }

            int count = steps.Count;
            int spacing = DpiScalingHelper.ScaleValue(styleConfig?.RecommendedStepSpacing ?? 14, _owner);
            int height = DpiScalingHelper.ScaleValue(styleConfig?.RecommendedButtonSize.Height ?? 44, _owner);
            int width = System.Math.Max(DpiScalingHelper.ScaleValue(86, _owner), (clientRect.Width - ((count - 1) * spacing)) / count);
            int totalWidth = (width * count) + ((count - 1) * spacing);
            int startX = clientRect.Left + ((clientRect.Width - totalWidth) / 2);
            int top = clientRect.Top + ((clientRect.Height - height) / 2);

            for (int i = 0; i < count; i++)
            {
                int x = startX + (i * (width + spacing));
                var stepRect = new Rectangle(x, top, width, height);
                result.StepRects.Add(stepRect);
                if (i > 0)
                {
                    var prev = result.StepRects[i - 1];
                    result.ConnectorRects.Add(new Rectangle(prev.Right, prev.Top, stepRect.Left - prev.Right, prev.Height));
                }
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
            bool selected = stepIndex == context.SelectedIndex;
            bool completed = step.State == StepState.Completed;
            bool hovered = stepIndex == context.HoveredIndex;
            int notch = System.Math.Max(DpiScalingHelper.ScaleValue(12, _owner), stepRect.Width / 7);
            var shape = BuildChevron(stepRect, notch, stepIndex > 0, stepIndex < context.Steps.Count - 1);

            Color leftColor = StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors);
            Color rightColor = step.State switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
            };

            if (completed || selected)
            {
                leftColor = StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors);
                rightColor = StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors);
            }

            using (shape)
            using (var gradient = new LinearGradientBrush(stepRect, leftColor, rightColor, LinearGradientMode.Horizontal))
            using (var border = new Pen(StepperThemeHelpers.GetStepBorderColor(context.Theme ?? _theme, context.UseThemeColors, step.State), StepperAccessibilityHelpers.GetAccessibleBorderWidth(selected ? 2 : 1)))
            {
                g.FillPath(gradient, shape);
                g.DrawPath(border, shape);
            }

            if (hovered && !selected)
            {
                using var hover = new SolidBrush(Color.FromArgb(16, Color.White));
                using var h = BuildChevron(stepRect, notch, stepIndex > 0, stepIndex < context.Steps.Count - 1);
                g.FillPath(hover, h);
            }

            string text = step.Text ?? $"Step {stepIndex + 1}";
            Font font = _stepFont ?? context.StepFont ?? SystemFonts.DefaultFont;
            using var textBrush = new SolidBrush(Color.White);
            var textSize = g.MeasureString(text, font);
            float tx = stepRect.Left + ((stepRect.Width - textSize.Width) / 2f);
            float ty = stepRect.Top + ((stepRect.Height - textSize.Height) / 2f);
            g.DrawString(text, font, textBrush, tx, ty);
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
        }

        private static GraphicsPath BuildChevron(Rectangle bounds, int notch, bool cutLeft, bool pointRight)
        {
            var path = new GraphicsPath();
            int left = bounds.Left;
            int top = bounds.Top;
            int right = bounds.Right;
            int bottom = bounds.Bottom;
            int midY = top + (bounds.Height / 2);
            int leftInset = cutLeft ? notch : 0;
            int rightTip = pointRight ? notch : 0;

            path.AddPolygon(new[]
            {
                new Point(left + leftInset, top),
                new Point(right - rightTip, top),
                new Point(right, midY),
                new Point(right - rightTip, bottom),
                new Point(left + leftInset, bottom),
                new Point(left, midY)
            });
            return path;
        }
    }
}
