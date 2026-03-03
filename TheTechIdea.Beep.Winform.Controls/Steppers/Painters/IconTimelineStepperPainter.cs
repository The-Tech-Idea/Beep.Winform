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
    public sealed class IconTimelineStepperPainter : IStepperPainter
    {
        private IBeepTheme _theme;
        private Font _labelFont;
        private Font _numberFont;

        public string Name => "IconTimeline";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
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
            int spacing = styleConfig?.RecommendedStepSpacing ?? 24;
            Size node = styleConfig?.RecommendedButtonSize ?? new Size(30, 30);
            int totalLength = ((orientation == Orientation.Horizontal ? node.Width : node.Height) * count) + (spacing * (count - 1));
            Point start = orientation == Orientation.Horizontal
                ? new Point(clientRect.Left + (clientRect.Width - totalLength) / 2, clientRect.Top + 8)
                : new Point(clientRect.Left + 8, clientRect.Top + (clientRect.Height - totalLength) / 2);

            for (int i = 0; i < count; i++)
            {
                int x = orientation == Orientation.Horizontal ? start.X + (i * (node.Width + spacing)) : start.X;
                int y = orientation == Orientation.Horizontal ? start.Y : start.Y + (i * (node.Height + spacing));
                var rect = new Rectangle(x, y, node.Width, node.Height);
                result.StepRects.Add(rect);

                if (i > 0)
                {
                    var prev = result.StepRects[i - 1];
                    var connector = orientation == Orientation.Horizontal
                        ? new Rectangle(prev.Right, prev.Top + (prev.Height / 2) - 1, rect.Left - prev.Right, 2)
                        : new Rectangle(prev.Left + (prev.Width / 2) - 1, prev.Bottom, 2, rect.Top - prev.Bottom);
                    result.ConnectorRects.Add(connector);
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

            Color fill = active
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

            using (var fillBrush = new SolidBrush(fill))
            using (var borderPen = new Pen(border, StepperAccessibilityHelpers.GetAccessibleBorderWidth(active ? 2 : 1)))
            {
                g.FillEllipse(fillBrush, stepRect);
                g.DrawEllipse(borderPen, stepRect);
            }

            if (hovered && !active)
            {
                using var hover = new SolidBrush(Color.FromArgb(16, Color.White));
                g.FillEllipse(hover, stepRect);
            }

            if (focused)
            {
                Rectangle focusRect = Rectangle.Inflate(stepRect, 3, 3);
                using var focusPen = new Pen((context.Theme ?? _theme)?.PrimaryColor ?? Color.DodgerBlue, 2f);
                g.DrawEllipse(focusPen, focusRect);
            }

            if (step.State == StepState.Completed)
            {
                StepperIconHelpers.PaintCheckmarkIcon(g, stepRect, Color.White, 1f);
            }
            else if (!string.IsNullOrWhiteSpace(step.ImagePath))
            {
                string glyph = "\u25CF";
                var glyphFont = _numberFont ?? context.NumberFont ?? SystemFonts.DefaultFont;
                var glyphSize = g.MeasureString(glyph, glyphFont);
                float gx = stepRect.Left + ((stepRect.Width - glyphSize.Width) / 2f);
                float gy = stepRect.Top + ((stepRect.Height - glyphSize.Height) / 2f);
                using var glyphBrush = new SolidBrush(StepperThemeHelpers.GetStepTextColor(context.Theme ?? _theme, context.UseThemeColors, step.State, Color.White));
                g.DrawString(glyph, glyphFont, glyphBrush, gx, gy);
            }
            else
            {
                string marker = (stepIndex + 1).ToString();
                var markerFont = _numberFont ?? context.NumberFont ?? SystemFonts.DefaultFont;
                var markerSize = g.MeasureString(marker, markerFont);
                float mx = stepRect.Left + ((stepRect.Width - markerSize.Width) / 2f);
                float my = stepRect.Top + ((stepRect.Height - markerSize.Height) / 2f);
                using var markerBrush = new SolidBrush(StepperThemeHelpers.GetStepTextColor(context.Theme ?? _theme, context.UseThemeColors, step.State, Color.White));
                g.DrawString(marker, markerFont, markerBrush, mx, my);
            }

            string label = step.Text ?? $"Step {stepIndex + 1}";
            Font labelFont = _labelFont ?? context.LabelFont ?? SystemFonts.DefaultFont;
            Color labelColor = StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            var labelSize = g.MeasureString(label, labelFont);

            float labelX;
            float labelY;
            if (context.Orientation == Orientation.Horizontal)
            {
                labelX = stepRect.Left + ((stepRect.Width - labelSize.Width) / 2f);
                labelY = stepRect.Bottom + 6f;
            }
            else
            {
                labelX = stepRect.Right + 10f;
                labelY = stepRect.Top + ((stepRect.Height - labelSize.Height) / 2f);
            }

            using var labelBrush = new SolidBrush(labelColor);
            g.DrawString(label, labelFont, labelBrush, labelX, labelY);
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            var fromStep = context.Steps[fromIndex];
            Color track = StepperThemeHelpers.GetConnectorLineColor(context.Theme ?? _theme, context.UseThemeColors, fromStep.State);
            Color fill = StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors);

            int thickness = StepperAccessibilityHelpers.GetAccessibleConnectorLineWidth(context.StyleConfig?.RecommendedConnectorLineWidth ?? 3);
            using var trackPen = new Pen(track, thickness);
            using var fillPen = new Pen(fill, thickness);

            if (context.Orientation == Orientation.Horizontal)
            {
                int y = connectorRect.Top + (connectorRect.Height / 2);
                g.DrawLine(trackPen, connectorRect.Left, y, connectorRect.Right, y);

                float progress = 0f;
                if (context.AnimationStates != null && fromIndex < context.AnimationStates.Count && context.AnimationStates[fromIndex] != null)
                {
                    progress = StepperAnimationEasing.Clamp01(context.AnimationStates[fromIndex].ConnectorFillProgress);
                }
                else if (fromStep.State == StepState.Completed || fromStep.State == StepState.Active)
                {
                    progress = 1f;
                }

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

                float progress = 0f;
                if (context.AnimationStates != null && fromIndex < context.AnimationStates.Count && context.AnimationStates[fromIndex] != null)
                {
                    progress = StepperAnimationEasing.Clamp01(context.AnimationStates[fromIndex].ConnectorFillProgress);
                }
                else if (fromStep.State == StepState.Completed || fromStep.State == StepState.Active)
                {
                    progress = 1f;
                }

                if (progress > 0f)
                {
                    int fillY = connectorRect.Top + (int)(connectorRect.Height * progress);
                    g.DrawLine(fillPen, x, connectorRect.Top, x, fillY);
                }
            }
        }
    }
}
