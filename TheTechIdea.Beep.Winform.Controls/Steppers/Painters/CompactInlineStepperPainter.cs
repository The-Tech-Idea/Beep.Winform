using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public sealed class CompactInlineStepperPainter : IStepperPainter
    {
        private Font _stepFont;
        private IBeepTheme _theme;

        public string Name => "CompactInline";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _stepFont = stepFont ?? labelFont ?? numberFont;
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
            int segmentWidth = clientRect.Width / System.Math.Max(1, count);
            for (int i = 0; i < count; i++)
            {
                result.StepRects.Add(new Rectangle(clientRect.Left + (i * segmentWidth), clientRect.Top, segmentWidth, clientRect.Height));
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
                if (i < context.StepRects.Count - 1)
                {
                    PaintConnector(g, context, i, context.StepRects[i]);
                }
            }
        }

        public void PaintStep(Graphics g, StepPainterContext context, int stepIndex, Rectangle stepRect)
        {
            var step = context.Steps[stepIndex];
            Font textFont = _stepFont ?? context.StepFont ?? SystemFonts.DefaultFont;
            Color textColor = StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            string text = $"{stepIndex + 1}. {step.Text}";
            var textSize = g.MeasureString(text, textFont);
            float textX = stepRect.Left + (stepRect.Width - textSize.Width) / 2f;
            float textY = stepRect.Top + (stepRect.Height - textSize.Height) / 2f;

            if (stepIndex == context.HoveredIndex)
            {
                using var hover = new SolidBrush(Color.FromArgb(18, Color.White));
                g.FillRectangle(hover, stepRect);
            }

            using (var textBrush = new SolidBrush(textColor))
            {
                g.DrawString(text, textFont, textBrush, textX, textY);
            }

            if (stepIndex == context.SelectedIndex)
            {
                int underlineThickness = StepperAccessibilityHelpers.GetAccessibleBorderWidth(2);
                using var underlinePen = new Pen((context.Theme ?? _theme)?.PrimaryColor ?? Color.DodgerBlue, underlineThickness);
                g.DrawLine(underlinePen, stepRect.Left + 6, stepRect.Bottom - underlineThickness, stepRect.Right - 6, stepRect.Bottom - underlineThickness);
            }
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle fromRect)
        {
            int x = fromRect.Right - 2;
            int y = fromRect.Top + (fromRect.Height / 2);
            using var brush = new SolidBrush(StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors));
            g.DrawString(">", _stepFont ?? context.StepFont ?? SystemFonts.DefaultFont, brush, x, y - 8);
        }
    }
}
