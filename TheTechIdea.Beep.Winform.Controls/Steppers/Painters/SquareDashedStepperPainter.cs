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
    public sealed class SquareDashedStepperPainter : IStepperPainter
    {
        private IBeepTheme _theme;
        private Font _textFont;

        public string Name => "SquareDashed";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _theme = theme;
            _textFont = stepFont ?? labelFont ?? numberFont;
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
            Size node = styleConfig?.RecommendedButtonSize ?? new Size(24, 24);
            int totalLength = ((orientation == Orientation.Horizontal ? node.Width : node.Height) * count) + (spacing * (count - 1));
            Point start = orientation == Orientation.Horizontal
                ? new Point(clientRect.Left + (clientRect.Width - totalLength) / 2, clientRect.Top + (clientRect.Height - node.Height) / 2)
                : new Point(clientRect.Left + (clientRect.Width - node.Width) / 2, clientRect.Top + (clientRect.Height - totalLength) / 2);

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
            bool selected = stepIndex == context.SelectedIndex;
            bool hovered = stepIndex == context.HoveredIndex;

            Color fill = step.State switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme ?? _theme, context.UseThemeColors),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme ?? _theme, context.UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(context.Theme ?? _theme, context.UseThemeColors)
            };
            Color border = StepperThemeHelpers.GetStepBorderColor(context.Theme ?? _theme, context.UseThemeColors, step.State);
            Color text = StepperThemeHelpers.GetStepTextColor(context.Theme ?? _theme, context.UseThemeColors, step.State);

            if (selected)
            {
                fill = StepperThemeHelpers.GetStepActiveColor(context.Theme ?? _theme, context.UseThemeColors);
                text = Color.White;
            }

            using (var brush = new SolidBrush(fill))
            using (var pen = new Pen(border, StepperAccessibilityHelpers.GetAccessibleBorderWidth(selected ? 2 : 1)))
            {
                g.FillRectangle(brush, stepRect);
                g.DrawRectangle(pen, stepRect);
            }

            if (hovered && !selected)
            {
                using var hover = new SolidBrush(Color.FromArgb(14, Color.White));
                g.FillRectangle(hover, stepRect);
            }

            string marker = step.State == StepState.Completed ? "✓" : (stepIndex + 1).ToString();
            Font font = _textFont ?? context.StepFont ?? SystemFonts.DefaultFont;
            var size = g.MeasureString(marker, font);
            float x = stepRect.Left + ((stepRect.Width - size.Width) / 2f);
            float y = stepRect.Top + ((stepRect.Height - size.Height) / 2f);
            using var textBrush = new SolidBrush(text);
            g.DrawString(marker, font, textBrush, x, y);
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            Color color = StepperThemeHelpers.GetConnectorLineColor(context.Theme ?? _theme, context.UseThemeColors, context.Steps[fromIndex].State);
            using var pen = new Pen(color, StepperAccessibilityHelpers.GetAccessibleConnectorLineWidth(2)) { DashStyle = DashStyle.Dash };
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
    }
}
