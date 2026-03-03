using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public sealed class SegmentedTabStepperPainter : IStepperPainter
    {
        private Font _labelFont;
        private IBeepTheme _theme;

        public string Name => "SegmentedTab";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _theme = theme;
            _labelFont = labelFont ?? stepFont ?? numberFont;
        }

        public StepperLayoutResult ComputeLayout(Rectangle clientRect, IReadOnlyList<StepModel> steps, Orientation orientation, StepperStyleConfig styleConfig)
        {
            var result = new StepperLayoutResult { ContentRect = clientRect };
            if (steps == null || steps.Count == 0)
            {
                return result;
            }

            int count = steps.Count;
            int spacing = System.Math.Max(0, styleConfig?.RecommendedStepSpacing ?? 4);
            if (orientation == Orientation.Horizontal)
            {
                int totalSpacing = spacing * (count - 1);
                int segmentWidth = (clientRect.Width - totalSpacing) / count;
                for (int i = 0; i < count; i++)
                {
                    int x = clientRect.Left + (i * (segmentWidth + spacing));
                    result.StepRects.Add(new Rectangle(x, clientRect.Top, segmentWidth, clientRect.Height));
                }
            }
            else
            {
                int totalSpacing = spacing * (count - 1);
                int segmentHeight = (clientRect.Height - totalSpacing) / count;
                for (int i = 0; i < count; i++)
                {
                    int y = clientRect.Top + (i * (segmentHeight + spacing));
                    result.StepRects.Add(new Rectangle(clientRect.Left, y, clientRect.Width, segmentHeight));
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

            for (int i = 0; i < context.StepRects.Count; i++)
            {
                PaintStep(g, context, i, context.StepRects[i]);
            }
        }

        public void PaintStep(Graphics g, StepPainterContext context, int stepIndex, Rectangle stepRect)
        {
            var step = context.Steps[stepIndex];
            bool selected = stepIndex == context.SelectedIndex;
            bool focused = stepIndex == context.FocusedIndex;
            bool hovered = stepIndex == context.HoveredIndex;

            Color fill = selected
                ? StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors)
                : StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors);
            Color border = StepperThemeHelpers.GetStepBorderColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            Color text = selected
                ? StepperThemeHelpers.GetStepTextColor(context.Theme ?? _theme, context.UseThemeColors, StepState.Active, Color.White)
                : StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, context.UseThemeColors, step.State);

            using (var fillBrush = new SolidBrush(fill))
            {
                g.FillRectangle(fillBrush, stepRect);
            }

            if (hovered)
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(18, Color.White));
                g.FillRectangle(hoverBrush, stepRect);
            }

            int borderWidth = StepperAccessibilityHelpers.GetAccessibleBorderWidth(1);
            using (var borderPen = new Pen(border, borderWidth))
            {
                g.DrawRectangle(borderPen, stepRect);
            }

            if (focused)
            {
                var focusRect = Rectangle.Inflate(stepRect, 1, 1);
                using var focusPen = new Pen((context.Theme ?? _theme)?.PrimaryColor ?? Color.DodgerBlue, 2f);
                g.DrawRectangle(focusPen, focusRect);
            }

            string label = string.IsNullOrWhiteSpace(step.Text) ? $"Step {stepIndex + 1}" : step.Text;
            Font textFont = _labelFont ?? context.LabelFont ?? SystemFonts.DefaultFont;
            var textSize = g.MeasureString(label, textFont);
            float textX = stepRect.Left + (stepRect.Width - textSize.Width) / 2f;
            float textY = stepRect.Top + (stepRect.Height - textSize.Height) / 2f;
            using var textBrush = new SolidBrush(text);
            g.DrawString(label, textFont, textBrush, textX, textY);
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            // Segmented tabs are directly adjacent; no connector drawing.
        }
    }
}
