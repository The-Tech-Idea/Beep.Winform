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
    public sealed class AlternatingTimelineStepperPainter : IStepperPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _numberFont;
        private Font _labelFont;
        private readonly List<Rectangle> _labelRects = new();

        public string Name => "AlternatingTimeline";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _owner = owner;
            _theme = theme;
            _numberFont = numberFont ?? stepFont ?? labelFont;
            _labelFont = labelFont ?? stepFont ?? numberFont;
        }

        public StepperLayoutResult ComputeLayout(Rectangle clientRect, IReadOnlyList<StepModel> steps, Orientation orientation, StepperStyleConfig styleConfig)
        {
            var result = new StepperLayoutResult { ContentRect = clientRect };
            _labelRects.Clear();
            if (steps == null || steps.Count == 0)
            {
                return result;
            }

            int count = steps.Count;
            int spacing = DpiScalingHelper.ScaleValue(styleConfig?.RecommendedStepSpacing ?? 26, _owner);
            Size node = DpiScalingHelper.ScaleSize(styleConfig?.RecommendedButtonSize ?? new Size(28, 28), _owner);
            int centerX = clientRect.Left + (clientRect.Width / 2);
            int totalHeight = (node.Height * count) + (spacing * (count - 1));
            int startY = clientRect.Top + ((clientRect.Height - totalHeight) / 2);
            int offsetX = DpiScalingHelper.ScaleValue(72, _owner);
            int labelWidth = DpiScalingHelper.ScaleValue(160, _owner);

            for (int i = 0; i < count; i++)
            {
                int y = startY + (i * (node.Height + spacing));
                int x = centerX - (node.Width / 2);
                var stepRect = new Rectangle(x, y, node.Width, node.Height);
                result.StepRects.Add(stepRect);

                bool left = i % 2 == 0;
                int labelX = left ? (centerX - offsetX - labelWidth) : (centerX + offsetX);
                _labelRects.Add(new Rectangle(labelX, y - 2, labelWidth, node.Height + DpiScalingHelper.ScaleValue(18, _owner)));

                if (i > 0)
                {
                    var prev = result.StepRects[i - 1];
                    result.ConnectorRects.Add(new Rectangle(centerX - 1, prev.Bottom, 2, stepRect.Top - prev.Bottom));
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
            bool selected = stepIndex == context.SelectedIndex;
            bool hovered = stepIndex == context.HoveredIndex;
            bool focused = stepIndex == context.FocusedIndex;

            Color fill = selected
                ? StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors)
                : step.State switch
                {
                    StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                    StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                    StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                    StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                    _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
                };
            Color border = StepperThemeHelpers.GetStepBorderColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            Color textColor = selected ? Color.White : StepperThemeHelpers.GetStepTextColor(context.Theme ?? _theme, context.UseThemeColors, step.State);

            using (var fillBrush = new SolidBrush(fill))
            using (var borderPen = new Pen(border, StepperAccessibilityHelpers.GetAccessibleBorderWidth(selected ? 2 : 1)))
            {
                g.FillEllipse(fillBrush, stepRect);
                g.DrawEllipse(borderPen, stepRect);
            }

            if (hovered && !selected)
            {
                using var hover = new SolidBrush(Color.FromArgb(16, Color.White));
                g.FillEllipse(hover, stepRect);
            }

            if (focused)
            {
                Rectangle focusRect = Rectangle.Inflate(stepRect, DpiScalingHelper.ScaleValue(3, _owner), DpiScalingHelper.ScaleValue(3, _owner));
                using var focusPen = new Pen((context.Theme ?? _theme)?.PrimaryColor ?? Color.DodgerBlue, 2f);
                g.DrawEllipse(focusPen, focusRect);
            }

            string marker = step.State == StepState.Completed ? "✓" : (stepIndex + 1).ToString();
            Font markerFont = _numberFont ?? context.NumberFont ?? SystemFonts.DefaultFont;
            var markerSize = g.MeasureString(marker, markerFont);
            float mx = stepRect.Left + ((stepRect.Width - markerSize.Width) / 2f);
            float my = stepRect.Top + ((stepRect.Height - markerSize.Height) / 2f);
            using var markerBrush = new SolidBrush(textColor);
            g.DrawString(marker, markerFont, markerBrush, mx, my);

            if (stepIndex < _labelRects.Count)
            {
                DrawLabelCard(g, context, step, _labelRects[stepIndex]);
                DrawLink(g, context, stepRect, _labelRects[stepIndex], stepIndex % 2 == 0);
            }
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            var fromState = context.Steps[fromIndex].State;
            Color color = StepperThemeHelpers.GetConnectorLineColor(context.Theme ?? _theme, context.UseThemeColors, fromState);
            using var pen = new Pen(color, StepperAccessibilityHelpers.GetAccessibleConnectorLineWidth(context.StyleConfig?.RecommendedConnectorLineWidth ?? 3));
            int x = connectorRect.Left + (connectorRect.Width / 2);
            g.DrawLine(pen, x, connectorRect.Top, x, connectorRect.Bottom);
        }

        private void DrawLabelCard(Graphics g, StepPainterContext context, StepModel step, Rectangle cardRect)
        {
            Color bg = step.State switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
            };
            Color border = StepperThemeHelpers.GetStepBorderColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            using (var fill = new SolidBrush(Color.FromArgb(220, bg)))
            using (var pen = new Pen(border, 1f))
            {
                g.FillRectangle(fill, cardRect);
                g.DrawRectangle(pen, cardRect);
            }

            Font font = _labelFont ?? context.LabelFont ?? SystemFonts.DefaultFont;
            Color labelColor = StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            using var brush = new SolidBrush(labelColor);
            string title = step.Text ?? "Step";
            string subtitle = step.Subtitle ?? string.Empty;
            g.DrawString(title, font, brush, cardRect.Left + DpiScalingHelper.ScaleValue(8, _owner), cardRect.Top + DpiScalingHelper.ScaleValue(4, _owner));
            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                g.DrawString(subtitle, font, brush, cardRect.Left + DpiScalingHelper.ScaleValue(8, _owner), cardRect.Top + DpiScalingHelper.ScaleValue(22, _owner));
            }
        }

        private void DrawLink(Graphics g, StepPainterContext context, Rectangle nodeRect, Rectangle cardRect, bool toLeft)
        {
            int y = nodeRect.Top + (nodeRect.Height / 2);
            int x1 = nodeRect.Left + (nodeRect.Width / 2);
            int x2 = toLeft ? cardRect.Right : cardRect.Left;
            using var pen = new Pen(StepperThemeHelpers.GetConnectorLineColor(context.Theme ?? _theme, context.UseThemeColors, StepState.Active), 1f)
            {
                DashStyle = DashStyle.Dot
            };
            g.DrawLine(pen, x1, y, x2, y);
        }
    }
}
