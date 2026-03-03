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
    public sealed class VerticalTimelineStepperPainter : IStepperPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _labelFont;
        private Font _numberFont;

        public string Name => "VerticalTimeline";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _owner = owner;
            _theme = theme;
            _labelFont = labelFont ?? stepFont;
            _numberFont = numberFont ?? stepFont ?? labelFont;
        }

        public StepperLayoutResult ComputeLayout(Rectangle clientRect, IReadOnlyList<StepModel> steps, Orientation orientation, StepperStyleConfig styleConfig)
        {
            var result = new StepperLayoutResult { ContentRect = clientRect };
            if (steps == null || steps.Count == 0)
            {
                return result;
            }

            int count = steps.Count;
            int spacing = styleConfig?.RecommendedStepSpacing ?? 26;
            Size node = styleConfig?.RecommendedButtonSize ?? new Size(28, 28);
            int timelineX = clientRect.Left + 20;
            int totalHeight = (node.Height * count) + (spacing * (count - 1));
            int startY = clientRect.Top + (clientRect.Height - totalHeight) / 2;

            for (int i = 0; i < count; i++)
            {
                int y = startY + (i * (node.Height + spacing));
                var stepRect = new Rectangle(timelineX, y, node.Width, node.Height);
                result.StepRects.Add(stepRect);

                if (i > 0)
                {
                    var prev = result.StepRects[i - 1];
                    int cx = prev.Left + (prev.Width / 2);
                    result.ConnectorRects.Add(new Rectangle(cx - 1, prev.Bottom, 2, stepRect.Top - prev.Bottom));
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
            bool active = stepIndex == context.SelectedIndex;
            bool focused = stepIndex == context.FocusedIndex;
            bool hovered = stepIndex == context.HoveredIndex;

            Color nodeFill = active
                ? StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors)
                : step.State switch
                {
                    StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                    StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                    StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                    StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                    _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
                };
            Color nodeBorder = StepperThemeHelpers.GetStepBorderColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            Color nodeText = active
                ? Color.White
                : StepperThemeHelpers.GetStepTextColor(context.Theme ?? _theme, context.UseThemeColors, step.State);

            using (var fill = new SolidBrush(nodeFill))
            using (var border = new Pen(nodeBorder, StepperAccessibilityHelpers.GetAccessibleBorderWidth(active ? 2 : 1)))
            {
                g.FillEllipse(fill, stepRect);
                g.DrawEllipse(border, stepRect);
            }

            if (hovered && !active)
            {
                using var hover = new SolidBrush(Color.FromArgb(18, Color.White));
                g.FillEllipse(hover, stepRect);
            }

            if (focused)
            {
                Rectangle focus = Rectangle.Inflate(stepRect, 3, 3);
                using var focusPen = new Pen((context.Theme ?? _theme)?.PrimaryColor ?? Color.DodgerBlue, 2f);
                g.DrawEllipse(focusPen, focus);
            }

            if (step.State == StepState.Completed)
            {
                StepperIconHelpers.PaintCheckmarkIcon(g, stepRect, nodeText, 1f);
            }
            else
            {
                string marker = (stepIndex + 1).ToString();
                Font markerFont = _numberFont ?? context.NumberFont ?? SystemFonts.DefaultFont;
                var markerSize = g.MeasureString(marker, markerFont);
                float mx = stepRect.Left + ((stepRect.Width - markerSize.Width) / 2f);
                float my = stepRect.Top + ((stepRect.Height - markerSize.Height) / 2f);
                using var markerBrush = new SolidBrush(nodeText);
                g.DrawString(marker, markerFont, markerBrush, mx, my);
            }

            DrawLabelCard(g, context, stepIndex, stepRect, step);
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            Color baseColor = StepperThemeHelpers.GetConnectorLineColor(context.Theme ?? _theme, context.UseThemeColors, context.Steps[fromIndex].State);
            int thickness = StepperAccessibilityHelpers.GetAccessibleConnectorLineWidth(context.StyleConfig?.RecommendedConnectorLineWidth ?? 3);
            using var pen = new Pen(baseColor, thickness);
            int x = connectorRect.Left + (connectorRect.Width / 2);
            g.DrawLine(pen, x, connectorRect.Top, x, connectorRect.Bottom);
        }

        private void DrawLabelCard(Graphics g, StepPainterContext context, int stepIndex, Rectangle stepRect, StepModel step)
        {
            Font labelFont = _labelFont ?? context.LabelFont ?? SystemFonts.DefaultFont;
            string title = step.Text ?? $"Step {stepIndex + 1}";
            string subtitle = step.Subtitle ?? string.Empty;

            float titleHeight = g.MeasureString(title, labelFont).Height;
            float subtitleHeight = string.IsNullOrWhiteSpace(subtitle) ? 0f : g.MeasureString(subtitle, labelFont).Height;
            int subStepCount = (step.HasSubSteps && step.SubSteps != null) ? step.SubSteps.Count : 0;
            int subStepHeight = subStepCount > 0 ? DpiScalingHelper.ScaleValue(14 * subStepCount, _owner) : 0;
            int cardLeft = stepRect.Right + 10;
            int cardTop = stepRect.Top - 2;
            int cardHeight = (int)System.Math.Ceiling(titleHeight + subtitleHeight + subStepHeight + 10f);
            int cardWidth = System.Math.Max(120, context.DrawingRect.Right - cardLeft - 8);
            Rectangle cardRect = new Rectangle(cardLeft, cardTop, cardWidth, cardHeight);

            Color cardFill = step.State switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
            };
            Color cardBorder = StepperThemeHelpers.GetStepBorderColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            int padding = DpiScalingHelper.ScaleValue(6, _owner);
            int rowHeight = DpiScalingHelper.ScaleValue(14, _owner);

            using (var fill = new SolidBrush(Color.FromArgb(210, cardFill)))
            using (var border = new Pen(cardBorder, 1f))
            {
                g.FillRectangle(fill, cardRect);
                g.DrawRectangle(border, cardRect);
            }

            using var textBrush = new SolidBrush(StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, context.UseThemeColors, step.State));
            float titleX = cardRect.Left + padding;
            float titleY = cardRect.Top + 4;
            g.DrawString(title, labelFont, textBrush, titleX, titleY);
            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                g.DrawString(subtitle, labelFont, textBrush, titleX, titleY + titleHeight);
            }

            if (step.HasSubSteps && step.SubSteps != null && step.SubSteps.Count > 0)
            {
                int subIndent = DpiScalingHelper.ScaleValue(10, _owner);
                int markerSize = DpiScalingHelper.ScaleValue(4, _owner);
                int x = cardRect.Left + padding + subIndent;
                float y = titleY + titleHeight + subtitleHeight + DpiScalingHelper.ScaleValue(4, _owner);

                using var linePen = new Pen(cardBorder, 1f);
                g.DrawLine(linePen, x - subIndent / 2f, y, x - subIndent / 2f, y + (step.SubSteps.Count * rowHeight));

                for (int i = 0; i < step.SubSteps.Count; i++)
                {
                    var subStep = step.SubSteps[i];
                    float rowY = y + (i * rowHeight);
                    var markerRect = new Rectangle(x - markerSize - 1, (int)rowY + (rowHeight / 2) - (markerSize / 2), markerSize, markerSize);
                    using (var markerBrush = new SolidBrush(cardBorder))
                    {
                        g.FillEllipse(markerBrush, markerRect);
                    }

                    string subText = subStep?.Text ?? $"Sub-step {i + 1}";
                    g.DrawString(subText, labelFont, textBrush, x + DpiScalingHelper.ScaleValue(4, _owner), rowY);
                }
            }
        }
    }
}
