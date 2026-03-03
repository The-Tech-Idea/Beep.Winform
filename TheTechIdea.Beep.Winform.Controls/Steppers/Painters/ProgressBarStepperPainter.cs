using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public sealed class ProgressBarStepperPainter : IStepperPainter
    {
        private Font _labelFont;
        private Font _numberFont;
        private IBeepTheme _theme;

        public string Name => "ProgressBar";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _theme = theme;
            _labelFont = labelFont ?? stepFont;
            _numberFont = numberFont ?? stepFont;
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
            int nodeSize = System.Math.Max(16, (styleConfig?.RecommendedButtonSize.Width ?? 28));
            int totalLength = (nodeSize * count) + (spacing * (count - 1));
            int startX = clientRect.Left + (clientRect.Width - totalLength) / 2;
            int centerY = clientRect.Top + (clientRect.Height / 2);

            for (int i = 0; i < count; i++)
            {
                int x = startX + (i * (nodeSize + spacing));
                result.StepRects.Add(new Rectangle(x, centerY - (nodeSize / 2), nodeSize, nodeSize));
                if (i > 0)
                {
                    var prev = result.StepRects[i - 1];
                    var current = result.StepRects[i];
                    result.ConnectorRects.Add(new Rectangle(prev.Right, centerY - 1, current.Left - prev.Right, 2));
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
            float scale = 1f;
            if (context.AnimationStates != null && stepIndex < context.AnimationStates.Count && context.AnimationStates[stepIndex] != null)
            {
                scale = System.Math.Max(0.5f, context.AnimationStates[stepIndex].NodeScale);
            }
            if (System.Math.Abs(scale - 1f) > 0.001f)
            {
                int inflateX = (int)(stepRect.Width * (scale - 1f) / 2f);
                int inflateY = (int)(stepRect.Height * (scale - 1f) / 2f);
                stepRect = Rectangle.Inflate(stepRect, inflateX, inflateY);
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
                g.FillEllipse(brush, stepRect);
            }

            if (stepIndex == context.FocusedIndex)
            {
                var focusRect = Rectangle.Inflate(stepRect, 2, 2);
                using var pen = new Pen((context.Theme ?? _theme)?.PrimaryColor ?? Color.DodgerBlue, 2);
                g.DrawEllipse(pen, focusRect);
            }

            string marker = step.State == StepState.Completed ? "✓" : (stepIndex + 1).ToString();
            Font markerFont = _numberFont ?? context.NumberFont ?? SystemFonts.DefaultFont;
            var markerSize = g.MeasureString(marker, markerFont);
            float markerX = stepRect.Left + (stepRect.Width - markerSize.Width) / 2f;
            float markerY = stepRect.Top + (stepRect.Height - markerSize.Height) / 2f;
            using (var textBrush = new SolidBrush(StepperThemeHelpers.GetStepTextColor(context.Theme ?? _theme, context.UseThemeColors, step.State, Color.White)))
            {
                g.DrawString(marker, markerFont, textBrush, markerX, markerY);
            }

            Font labelFont = _labelFont ?? context.LabelFont ?? SystemFonts.DefaultFont;
            string label = step.Text ?? string.Empty;
            var labelSize = g.MeasureString(label, labelFont);
            float labelX = stepRect.Left + (stepRect.Width - labelSize.Width) / 2f;
            float labelY = stepRect.Bottom + 4;
            using (var labelBrush = new SolidBrush(StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, context.UseThemeColors, step.State)))
            {
                g.DrawString(label, labelFont, labelBrush, labelX, labelY);
            }
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            var fromState = context.Steps[fromIndex].State;
            Color trackColor = StepperThemeHelpers.GetConnectorLineColor(context.Theme ?? _theme, context.UseThemeColors, fromState);
            Color fillColor = StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors);
            int thickness = StepperAccessibilityHelpers.GetAccessibleConnectorLineWidth(context.StyleConfig?.RecommendedConnectorLineWidth ?? 2);
            using var trackPen = new Pen(trackColor, thickness);
            using var fillPen = new Pen(fillColor, thickness);

            float progress = 0f;
            if (context.AnimationStates != null && fromIndex < context.AnimationStates.Count && context.AnimationStates[fromIndex] != null)
            {
                progress = StepperAnimationEasing.Clamp01(context.AnimationStates[fromIndex].ConnectorFillProgress);
            }
            else if (fromState == StepState.Completed)
            {
                progress = 1f;
            }

            if (context.Orientation == Orientation.Horizontal)
            {
                int y = connectorRect.Top + (connectorRect.Height / 2);
                g.DrawLine(trackPen, connectorRect.Left, y, connectorRect.Right, y);
                if (progress > 0f)
                {
                    int fillX = connectorRect.Left + (int)(connectorRect.Width * progress);
                    g.DrawLine(fillPen, connectorRect.Left, y, fillX, y);
                }
            }
            else
            {
                int x = connectorRect.Left + (connectorRect.Width / 2);
                g.DrawLine(trackPen, x, connectorRect.Top, x, connectorRect.Bottom);
                if (progress > 0f)
                {
                    int fillY = connectorRect.Top + (int)(connectorRect.Height * progress);
                    g.DrawLine(fillPen, x, connectorRect.Top, x, fillY);
                }
            }
        }
    }
}
