using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// iOS-style rounded checkbox painter
    /// </summary>
    public class iOSCheckBoxPainter : CheckBoxPainterBase
    {
        public override void PaintCheckBox(Graphics g, Rectangle bounds, CheckBoxItemState state, CheckBoxRenderOptions options)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var (bgColor, borderColor, checkMarkColor, fgColor) = GetCheckBoxColors(state, options);
            (bgColor, borderColor) = ApplyInteractionStateColors(state, bgColor, borderColor);
            int radius = Math.Max(8, bounds.Height / 3);

            // Paint background with rounded corners (iOS style - more rounded)
            using (var path = CreateRoundedPath(bounds, radius))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Paint border
                using (var pen = new Pen(borderColor, options.BorderWidth))
                {
                    g.DrawPath(pen, path);
                }

                if (!state.IsDisabled)
                {
                    Rectangle glossRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, Math.Max(2, bounds.Height / 2));
                    using var glossPath = CreateRoundedPath(glossRect, radius);
                    using var glossBrush = new SolidBrush(Color.FromArgb(35, Color.White));
                    g.FillPath(glossBrush, glossPath);
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
            // iOS style - thicker check mark
            var optionsThick = options;
            optionsThick.CheckMarkThickness = Math.Max(2, options.CheckMarkThickness + 1);
            PaintStandardCheckMark(g, bounds, optionsThick);
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
