using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// Button-style checkbox painter
    /// </summary>
    public class ButtonCheckBoxPainter : CheckBoxPainterBase
    {
        public override void PaintCheckBox(Graphics g, Rectangle bounds, CheckBoxItemState state, CheckBoxRenderOptions options)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var (bgColor, borderColor, checkMarkColor, fgColor) = GetCheckBoxColors(state, options);
            (bgColor, borderColor) = ApplyInteractionStateColors(state, bgColor, borderColor);

            Color surfaceColor = CheckBoxThemeHelpers.GetUncheckedBackgroundColor(options.Theme, options.UseThemeColors);
            Color effectiveBackground = state.IsChecked || state.IsIndeterminate
                ? bgColor
                : surfaceColor;
            if (state.IsHovered && !state.IsDisabled)
            {
                effectiveBackground = ControlPaint.Light(effectiveBackground, 0.06f);
            }
            float borderWidth = state.IsChecked || state.IsIndeterminate
                ? Math.Max(1.5f, options.BorderWidth)
                : Math.Max(1f, options.BorderWidth);

            using (var path = CreateRoundedPath(bounds, options.BorderRadius))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    brush.Color = effectiveBackground;
                    g.FillPath(brush, path);
                }

                // Paint border
                using (var pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }

                if ((state.IsChecked || state.IsIndeterminate) && !state.IsDisabled)
                {
                    using var innerGlow = new Pen(Color.FromArgb(60, checkMarkColor), 1f);
                    Rectangle inset = Rectangle.Inflate(bounds, -2, -2);
                    using var insetPath = CreateRoundedPath(inset, Math.Max(1, options.BorderRadius - 1));
                    g.DrawPath(innerGlow, insetPath);
                }
            }

            // Paint check mark or indeterminate mark
            if (state.IsChecked)
            {
                PaintCheckMark(g, bounds, options);
            }
            else if (state.IsIndeterminate)
            {
                PaintIndeterminateMark(g, bounds, options);
            }

            if (state.IsFocused && !state.IsDisabled)
            {
                PaintFocusRing(g, bounds, options);
            }
        }

        public override void PaintCheckMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options)
        {
            Rectangle iconBounds = Rectangle.Inflate(bounds, -Math.Max(2, bounds.Width / 6), -Math.Max(2, bounds.Height / 6));
            PaintStandardCheckMark(g, iconBounds, options);
        }

        public override void PaintIndeterminateMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options)
        {
            Rectangle iconBounds = Rectangle.Inflate(bounds, -Math.Max(2, bounds.Width / 6), -Math.Max(2, bounds.Height / 6));
            PaintStandardIndeterminateMark(g, iconBounds, options);
        }

        public override void PaintText(Graphics g, Rectangle bounds, string text, CheckBoxRenderOptions options)
        {
            PaintStandardText(g, bounds, text, options);
        }
    }
}
