using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// Modern flat design checkbox painter
    /// </summary>
    public class ModernCheckBoxPainter : CheckBoxPainterBase
    {
        public override void PaintCheckBox(Graphics g, Rectangle bounds, CheckBoxItemState state, CheckBoxRenderOptions options)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var (bgColor, borderColor, checkMarkColor, fgColor) = GetCheckBoxColors(state, options);
            (bgColor, borderColor) = ApplyInteractionStateColors(state, bgColor, borderColor);
            Color effectiveBackground = state.IsChecked || state.IsIndeterminate
                ? bgColor
                : ControlPaint.Light(CheckBoxThemeHelpers.GetUncheckedBackgroundColor(options.Theme, options.UseThemeColors), 0.04f);
            float borderWidth = state.IsChecked || state.IsIndeterminate
                ? Math.Max(1.4f, options.BorderWidth)
                : Math.Max(1f, options.BorderWidth);

            // Paint background with rounded corners
            using (var path = CreateRoundedPath(bounds, options.BorderRadius))
            {
                using (var brush = new SolidBrush(effectiveBackground))
                {
                    g.FillPath(brush, path);
                }

                // Paint border with slightly thicker line
                using (var pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
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
