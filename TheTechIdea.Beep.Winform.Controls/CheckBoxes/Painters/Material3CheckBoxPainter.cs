using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// Material Design 3 style checkbox painter
    /// </summary>
    public class Material3CheckBoxPainter : CheckBoxPainterBase
    {
        public override void PaintCheckBox(Graphics g, Rectangle bounds, CheckBoxItemState state, CheckBoxRenderOptions options)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var (bgColor, borderColor, checkMarkColor, fgColor) = GetCheckBoxColors(state, options);
            (bgColor, borderColor) = ApplyInteractionStateColors(state, bgColor, borderColor);
            int effectiveRadius = Math.Max(4, options.BorderRadius);
            float borderWidth = state.IsChecked || state.IsIndeterminate
                ? Math.Max(1.6f, options.BorderWidth)
                : Math.Max(1.2f, options.BorderWidth);

            if (!state.IsChecked && !state.IsIndeterminate && !state.IsDisabled)
            {
                // Material-like resting state: low-emphasis container with crisp border.
                bgColor = Color.FromArgb(state.IsHovered ? 34 : 20, borderColor);
            }

            if (state.IsDisabled)
            {
                bgColor = Color.FromArgb(160, bgColor);
                borderColor = Color.FromArgb(170, borderColor);
            }

            // Paint background with rounded corners
            using (var path = CreateRoundedPath(bounds, effectiveRadius))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Paint border
                using (var pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }

                // Subtle inner highlight improves legibility on dense/dark themes.
                if ((state.IsChecked || state.IsIndeterminate) && !state.IsDisabled)
                {
                    Rectangle inner = Rectangle.Inflate(bounds, -1, -1);
                    using var innerPath = CreateRoundedPath(inner, Math.Max(2, effectiveRadius - 1));
                    using var innerPen = new Pen(Color.FromArgb(55, Color.White), 1f);
                    g.DrawPath(innerPen, innerPath);
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
            PaintStandardCheckMark(g, bounds, options);
        }

        public override void PaintIndeterminateMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options)
        {
            PaintStandardIndeterminateMark(g, bounds, options);
        }

        public override void PaintText(Graphics g, Rectangle bounds, string text, CheckBoxRenderOptions options)
        {
            PaintStandardText(g, bounds, text, options);
        }
    }
}
