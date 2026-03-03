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
    public sealed class BadgeStatusStepperPainter : IStepperPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _numberFont;
        private Font _labelFont;

        public string Name => "BadgeStatus";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _owner = owner;
            _theme = theme;
            _numberFont = numberFont ?? stepFont;
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
            int spacing = DpiScalingHelper.ScaleValue(styleConfig?.RecommendedStepSpacing ?? 20, _owner);
            Size node = DpiScalingHelper.ScaleSize(styleConfig?.RecommendedButtonSize ?? new Size(34, 34), _owner);

            if (orientation == Orientation.Horizontal)
            {
                int totalLength = (node.Width * count) + (spacing * (count - 1));
                int startX = clientRect.Left + ((clientRect.Width - totalLength) / 2);
                int y = clientRect.Top + ((clientRect.Height - node.Height) / 2);
                for (int i = 0; i < count; i++)
                {
                    int x = startX + (i * (node.Width + spacing));
                    var rect = new Rectangle(x, y, node.Width, node.Height);
                    result.StepRects.Add(rect);
                    if (i > 0)
                    {
                        var prev = result.StepRects[i - 1];
                        result.ConnectorRects.Add(new Rectangle(prev.Right, prev.Top + (prev.Height / 2) - 1, rect.Left - prev.Right, 2));
                    }
                }
            }
            else
            {
                int totalLength = (node.Height * count) + (spacing * (count - 1));
                int startY = clientRect.Top + ((clientRect.Height - totalLength) / 2);
                int x = clientRect.Left + ((clientRect.Width - node.Width) / 2);
                for (int i = 0; i < count; i++)
                {
                    int y = startY + (i * (node.Height + spacing));
                    var rect = new Rectangle(x, y, node.Width, node.Height);
                    result.StepRects.Add(rect);
                    if (i > 0)
                    {
                        var prev = result.StepRects[i - 1];
                        result.ConnectorRects.Add(new Rectangle(prev.Left + (prev.Width / 2) - 1, prev.Bottom, 2, rect.Top - prev.Bottom));
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
            Color statusRing = step.State switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
            };

            using (var fillBrush = new SolidBrush(fill))
            using (var ringPen = new Pen(statusRing, StepperAccessibilityHelpers.GetAccessibleBorderWidth(active ? 3 : 2)))
            {
                g.FillEllipse(fillBrush, stepRect);
                g.DrawEllipse(ringPen, stepRect);
            }

            if (hovered && !active)
            {
                using var hover = new SolidBrush(Color.FromArgb(16, Color.White));
                g.FillEllipse(hover, stepRect);
            }

            if (focused)
            {
                Rectangle focus = Rectangle.Inflate(stepRect, DpiScalingHelper.ScaleValue(3, _owner), DpiScalingHelper.ScaleValue(3, _owner));
                using var focusPen = new Pen((context.Theme ?? _theme)?.PrimaryColor ?? Color.DodgerBlue, 2f);
                g.DrawEllipse(focusPen, focus);
            }

            string marker = step.State == StepState.Completed ? "✓" : (stepIndex + 1).ToString();
            Font font = _numberFont ?? context.NumberFont ?? SystemFonts.DefaultFont;
            using (var markerBrush = new SolidBrush(StepperThemeHelpers.GetStepTextColor(context.Theme ?? _theme, context.UseThemeColors, step.State, Color.White)))
            {
                var markerSize = g.MeasureString(marker, font);
                float mx = stepRect.Left + ((stepRect.Width - markerSize.Width) / 2f);
                float my = stepRect.Top + ((stepRect.Height - markerSize.Height) / 2f);
                g.DrawString(marker, font, markerBrush, mx, my);
            }

            DrawBadge(g, context, stepRect, step.BadgeCount);
            DrawLabel(g, context, stepIndex, stepRect, step.Text);
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            Color color = StepperThemeHelpers.GetConnectorLineColor(context.Theme ?? _theme, context.UseThemeColors, context.Steps[fromIndex].State);
            using var pen = new Pen(color, StepperAccessibilityHelpers.GetAccessibleConnectorLineWidth(context.StyleConfig?.RecommendedConnectorLineWidth ?? 2));
            if (context.Orientation == Orientation.Horizontal)
            {
                int midY = connectorRect.Top + (connectorRect.Height / 2);
                g.DrawLine(pen, connectorRect.Left, midY, connectorRect.Right, midY);
            }
            else
            {
                int midX = connectorRect.Left + (connectorRect.Width / 2);
                g.DrawLine(pen, midX, connectorRect.Top, midX, connectorRect.Bottom);
            }
        }

        private void DrawBadge(Graphics g, StepPainterContext context, Rectangle stepRect, int badgeCount)
        {
            if (badgeCount <= 0)
            {
                return;
            }

            int badgeSize = DpiScalingHelper.ScaleValue(16, _owner);
            Rectangle badgeRect = new Rectangle(stepRect.Right - (badgeSize / 2), stepRect.Top - (badgeSize / 2), badgeSize, badgeSize);
            Color badgeColor = StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors);

            using (var fill = new SolidBrush(badgeColor))
            using (var border = new Pen(Color.White, 1f))
            {
                g.FillEllipse(fill, badgeRect);
                g.DrawEllipse(border, badgeRect);
            }

            string text = badgeCount > 99 ? "99+" : badgeCount.ToString();
            Font label = _labelFont ?? context.LabelFont ?? SystemFonts.DefaultFont;
            var size = g.MeasureString(text, label);
            float x = badgeRect.Left + ((badgeRect.Width - size.Width) / 2f);
            float y = badgeRect.Top + ((badgeRect.Height - size.Height) / 2f);
            using var brush = new SolidBrush(Color.White);
            g.DrawString(text, label, brush, x, y);
        }

        private void DrawLabel(Graphics g, StepPainterContext context, int stepIndex, Rectangle stepRect, string labelText)
        {
            if (string.IsNullOrWhiteSpace(labelText))
            {
                return;
            }

            Font font = _labelFont ?? context.LabelFont ?? SystemFonts.DefaultFont;
            using var brush = new SolidBrush(StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, context.UseThemeColors, context.Steps[stepIndex].State));
            var size = g.MeasureString(labelText, font);

            if (context.Orientation == Orientation.Horizontal)
            {
                g.DrawString(labelText, font, brush, stepRect.Left + ((stepRect.Width - size.Width) / 2f), stepRect.Bottom + DpiScalingHelper.ScaleValue(6, _owner));
            }
            else
            {
                g.DrawString(labelText, font, brush, stepRect.Right + DpiScalingHelper.ScaleValue(8, _owner), stepRect.Top + ((stepRect.Height - size.Height) / 2f));
            }
        }
    }
}
