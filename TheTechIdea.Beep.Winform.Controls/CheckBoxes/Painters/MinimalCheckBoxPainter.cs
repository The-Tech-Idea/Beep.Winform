using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// Minimal clean style checkbox painter
    /// </summary>
    public class MinimalCheckBoxPainter : CheckBoxPainterBase
    {
        public override void PaintCheckBox(Graphics g, Rectangle bounds, CheckBoxItemState state, CheckBoxRenderOptions options)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var (bgColor, borderColor, checkMarkColor, fgColor) = GetCheckBoxColors(state, options);
            (bgColor, borderColor) = ApplyInteractionStateColors(state, bgColor, borderColor);

            // Minimal style - only paint background when checked
            if (state.IsChecked || state.IsIndeterminate)
            {
                using (var brush = new SolidBrush(state.IsIndeterminate ? Color.FromArgb(80, bgColor) : bgColor))
                {
                    using var path = CreateRoundedPath(bounds, Math.Max(2, options.BorderRadius));
                    g.FillPath(brush, path);
                }
            }

            // Paint thin border
            using (var pen = new Pen(borderColor, Math.Max(1f, options.BorderWidth - 0.5f)))
            {
                Rectangle borderRect = Rectangle.Inflate(bounds, -1, -1);
                using var borderPath = CreateRoundedPath(borderRect, Math.Max(2, options.BorderRadius));
                g.DrawPath(pen, borderPath);
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
