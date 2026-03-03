using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public sealed class DotsStepperPainter : IStepperPainter
    {
        private IBeepTheme _theme;

        public string Name => "Dots";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
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
            int spacing = styleConfig?.RecommendedStepSpacing ?? 20;
            int dotSize = System.Math.Max(6, (styleConfig?.RecommendedButtonSize.Width ?? 20) / 3);
            int totalLength = (dotSize * count) + (spacing * (count - 1));

            if (orientation == Orientation.Horizontal)
            {
                int startX = clientRect.Left + (clientRect.Width - totalLength) / 2;
                int centerY = clientRect.Top + (clientRect.Height / 2);
                for (int i = 0; i < count; i++)
                {
                    int x = startX + (i * (dotSize + spacing));
                    result.StepRects.Add(new Rectangle(x, centerY - (dotSize / 2), dotSize, dotSize));
                    if (i > 0)
                    {
                        var prev = result.StepRects[i - 1];
                        result.ConnectorRects.Add(new Rectangle(prev.Right, centerY - 1, x - prev.Right, 2));
                    }
                }
            }
            else
            {
                int startY = clientRect.Top + (clientRect.Height - totalLength) / 2;
                int centerX = clientRect.Left + (clientRect.Width / 2);
                for (int i = 0; i < count; i++)
                {
                    int y = startY + (i * (dotSize + spacing));
                    result.StepRects.Add(new Rectangle(centerX - (dotSize / 2), y, dotSize, dotSize));
                    if (i > 0)
                    {
                        var prev = result.StepRects[i - 1];
                        result.ConnectorRects.Add(new Rectangle(centerX - 1, prev.Bottom, 2, y - prev.Bottom));
                    }
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

            for (int i = 0; i < context.ConnectorRects.Count; i++)
            {
                PaintConnector(g, context, i, context.ConnectorRects[i]);
            }

            for (int i = 0; i < context.StepRects.Count; i++)
            {
                PaintStep(g, context, i, context.StepRects[i]);
            }
        }

        public void PaintStep(Graphics g, StepPainterContext context, int stepIndex, Rectangle stepRect)
        {
            var step = context.Steps[stepIndex];
            bool active = stepIndex == context.SelectedIndex;
            bool hovered = stepIndex == context.HoveredIndex;

            Rectangle dotRect = stepRect;
            if (active)
            {
                dotRect = Rectangle.Inflate(stepRect, 2, 2);
            }
            else if (hovered)
            {
                dotRect = Rectangle.Inflate(stepRect, 1, 1);
            }

            Color fill = step.State switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
            };

            using (var brush = new SolidBrush(fill))
            {
                g.FillEllipse(brush, dotRect);
            }
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            var fromState = context.Steps[fromIndex].State;
            Color color = StepperThemeHelpers.GetConnectorLineColor(context.Theme ?? _theme, context.UseThemeColors, fromState);
            int thickness = StepperAccessibilityHelpers.GetAccessibleConnectorLineWidth(context.StyleConfig?.RecommendedConnectorLineWidth ?? 2);
            using var pen = new Pen(color, thickness);
            if (context.Orientation == Orientation.Horizontal)
            {
                int y = connectorRect.Top + (connectorRect.Height / 2);
                g.DrawLine(pen, connectorRect.Left, y, connectorRect.Right, y);
            }
            else
            {
                int x = connectorRect.Left + (connectorRect.Width / 2);
                g.DrawLine(pen, x, connectorRect.Top, x, connectorRect.Bottom);
            }
        }
    }
}
