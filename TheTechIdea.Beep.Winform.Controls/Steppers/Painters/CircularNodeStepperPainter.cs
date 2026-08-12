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
    public sealed class CircularNodeStepperPainter : IStepperPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _stepFont;
        private Font _labelFont;
        private Font _numberFont;

        public string Name => "CircularNode";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
            _owner = owner;
            _theme = theme;
            _stepFont = stepFont;
            _labelFont = labelFont;
            _numberFont = numberFont;
        }

        public StepperLayoutResult ComputeLayout(
            Rectangle clientRect,
            IReadOnlyList<StepModel> steps,
            Orientation orientation,
            StepperStyleConfig styleConfig)
        {
            var result = new StepperLayoutResult { ContentRect = clientRect };
            if (steps == null || steps.Count == 0)
            {
                return result;
            }

            int stepCount = steps.Count;
            int spacing = styleConfig?.RecommendedStepSpacing ?? 20;
            Size stepSize = styleConfig?.RecommendedButtonSize ?? new Size(32, 32);
            int stepTotalSize = orientation == Orientation.Horizontal ? stepSize.Width : stepSize.Height;

            // Distribute across the available width (F5 rule 3): the recommended ~20px pitch
            // left a 4-step bar clustered in the middle of a 900px control, labels touching.
            if (orientation == Orientation.Horizontal && stepCount > 1)
            {
                int edgeInset = Math.Max(stepTotalSize / 2, DpiScalingHelper.ScaleValue(56, _owner));
                int availW = clientRect.Width - (2 * edgeInset);
                spacing = Math.Max(DpiScalingHelper.ScaleValue(8, _owner),
                                   (availW - (stepTotalSize * stepCount)) / (stepCount - 1));
            }
            int totalLength = (stepTotalSize + spacing) * stepCount - spacing;
            Point startPoint = orientation == Orientation.Horizontal
                ? new Point(clientRect.Left + Math.Max(0, (clientRect.Width - totalLength) / 2), clientRect.Top + (clientRect.Height - stepSize.Height) / 2)
                : new Point(clientRect.Left + (clientRect.Width - stepSize.Width) / 2, clientRect.Top + (clientRect.Height - totalLength) / 2);

            for (int i = 0; i < stepCount; i++)
            {
                int x = orientation == Orientation.Horizontal
                    ? startPoint.X + i * (stepSize.Width + spacing)
                    : startPoint.X;
                int y = orientation == Orientation.Horizontal
                    ? startPoint.Y
                    : startPoint.Y + i * (stepSize.Height + spacing);

                var stepRect = new Rectangle(x, y, stepSize.Width, stepSize.Height);
                result.StepRects.Add(stepRect);

                if (i > 0)
                {
                    Rectangle previousRect = result.StepRects[i - 1];
                    Rectangle connector = orientation == Orientation.Horizontal
                        ? Rectangle.FromLTRB(previousRect.Right, previousRect.Top + previousRect.Height / 2 - 1, stepRect.Left, stepRect.Top + stepRect.Height / 2 + 1)
                        : Rectangle.FromLTRB(previousRect.Left + previousRect.Width / 2 - 1, previousRect.Bottom, stepRect.Left + stepRect.Width / 2 + 1, stepRect.Top);
                    result.ConnectorRects.Add(connector);
                }
            }

            return result;
        }

        public void Paint(Graphics g, StepPainterContext context)
        {
            if (context == null || context.StepRects == null || context.StepRects.Count == 0)
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
            bool isHovered = stepIndex == context.HoveredIndex;
            bool isFocused = stepIndex == context.FocusedIndex;
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

            Color fillColor = step.State switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(context.Theme),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(context.Theme),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(context.Theme),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(context.Theme),
                _ => StepperThemeHelpers.GetStepPendingColor(context.Theme)
            };

            using (var fillBrush = new SolidBrush(fillColor))
            {
                g.FillEllipse(fillBrush, stepRect);
            }

            if (isHovered)
            {
                using var hoverOverlay = new SolidBrush(Color.FromArgb(24, (context.Theme ?? _theme).StepperItemHoverBackColor));
                g.FillEllipse(hoverOverlay, stepRect);
            }

            if (stepIndex == context.SelectedIndex)
            {
                using var borderPen = new Pen(StepperThemeHelpers.GetStepBorderColor(context.Theme, step.State),
                    StepperAccessibilityHelpers.GetAccessibleBorderWidth(2));
                g.DrawEllipse(borderPen, stepRect);
            }

            if (isFocused)
            {
                int focusGap = 2;
                var focusRect = Rectangle.Inflate(stepRect, focusGap, focusGap);
                using var focusPen = new Pen((context.Theme ?? _theme).PrimaryColor, 2f);
                g.DrawEllipse(focusPen, focusRect);
            }

            if (context.AnimationStates != null && stepIndex < context.AnimationStates.Count)
            {
                var anim = context.AnimationStates[stepIndex];
                if (anim != null && anim.RippleActive && anim.RippleRadius > 0.5f && anim.RippleAlpha > 0)
                {
                    int diameter = (int)(anim.RippleRadius * 2f);
                    var rippleRect = new Rectangle(
                        anim.RippleCenter.X - (diameter / 2),
                        anim.RippleCenter.Y - (diameter / 2),
                        diameter,
                        diameter);
                    using var rippleBrush = new SolidBrush(Color.FromArgb(anim.RippleAlpha, (context.Theme ?? _theme).PrimaryColor));
                    g.FillEllipse(rippleBrush, rippleRect);
                }
            }

            if (step.State == StepState.Completed)
            {
                Color iconColor = StepperIconHelpers.GetIconColor(context.Theme, StepState.Completed);
                StepperIconHelpers.PaintCheckmarkIcon(g, stepRect, iconColor, 1f);
            }
            else
            {
                string text = (stepIndex + 1).ToString();
                Font font = _numberFont ?? _stepFont ?? _labelFont ?? SystemFonts.DefaultFont;
                SizeF textSize = g.MeasureString(text, font);
                float textX = stepRect.Left + (stepRect.Width - textSize.Width) / 2f;
                float textY = stepRect.Top + (stepRect.Height - textSize.Height) / 2f;
                using var textBrush = new SolidBrush(StepperThemeHelpers.GetStepTextColor(context.Theme, step.State));
                g.DrawString(text, font, textBrush, textX, textY);
            }

            // The title, through the one helper every painter uses - measured against the slot
            // this step owns, so a long title ellipsises instead of running into its neighbour.
            StepperLabelHelpers.DrawStepTitle(
                g, step, stepRect, context.Orientation,
                _labelFont ?? context.LabelFont ?? _stepFont ?? SystemFonts.DefaultFont,
                StepperThemeHelpers.GetStepLabelColor(context.Theme ?? _theme, step.State),
                StepperLabelHelpers.SlotWidth(context, stepIndex, stepRect),
                _owner);
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
            var fromState = context.Steps[fromIndex].State;
            Color trackColor = StepperThemeHelpers.GetConnectorLineColor(context.Theme, fromState);
            Color fillColor = StepperThemeHelpers.GetStepActiveColor(context.Theme);
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
                int midY = connectorRect.Top + connectorRect.Height / 2;
                g.DrawLine(trackPen, connectorRect.Left, midY, connectorRect.Right, midY);
                if (progress > 0f)
                {
                    int fillX = connectorRect.Left + (int)(connectorRect.Width * progress);
                    g.DrawLine(fillPen, connectorRect.Left, midY, fillX, midY);
                }
            }
            else
            {
                int midX = connectorRect.Left + connectorRect.Width / 2;
                g.DrawLine(trackPen, midX, connectorRect.Top, midX, connectorRect.Bottom);
                if (progress > 0f)
                {
                    int fillY = connectorRect.Top + (int)(connectorRect.Height * progress);
                    g.DrawLine(fillPen, midX, connectorRect.Top, midX, fillY);
                }
            }
        }
    }
}
