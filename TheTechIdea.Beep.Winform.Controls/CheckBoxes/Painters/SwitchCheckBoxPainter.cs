using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// Switch-style checkbox painter (toggle-like appearance)
    /// </summary>
    public class SwitchCheckBoxPainter : CheckBoxPainterBase
    {
        public override void PaintCheckBox(Graphics g, Rectangle bounds, CheckBoxItemState state, CheckBoxRenderOptions options)
        {
            var (bgColor, borderColor, checkMarkColor, fgColor) = GetCheckBoxColors(state, options);
            (bgColor, borderColor) = ApplyInteractionStateColors(state, bgColor, borderColor);

            int radius = bounds.Height / 2;
            using (var trackPath = CreateRoundedPath(bounds, radius))
            {
                using (var trackBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(trackBrush, trackPath);
                }
                using (var trackPen = new Pen(borderColor, options.BorderWidth))
                {
                    g.DrawPath(trackPen, trackPath);
                }
            }

            Color thumbColor = state.IsDisabled
                ? ControlPaint.Light(CheckBoxThemeHelpers.GetForegroundColor(options.Theme, options.UseThemeColors), 0.35f)
                : CheckBoxThemeHelpers.GetForegroundColor(options.Theme, options.UseThemeColors);

            Rectangle thumbRect = CalculateThumbRect(bounds, state, options.BorderWidth);
            using (var thumbBrush = new SolidBrush(thumbColor))
            {
                g.FillEllipse(thumbBrush, thumbRect);
            }
            using (var thumbPen = new Pen(Color.FromArgb(60, borderColor), 1f))
            {
                g.DrawEllipse(thumbPen, thumbRect);
            }

            if (state.IsIndeterminate)
            {
                Rectangle markRect = new Rectangle(
                    thumbRect.X + thumbRect.Width / 4,
                    thumbRect.Y + thumbRect.Height / 2 - 1,
                    thumbRect.Width / 2,
                    2);
                using var markBrush = new SolidBrush(CheckBoxThemeHelpers.GetIndeterminateMarkColor(options.Theme, options.UseThemeColors));
                g.FillRectangle(markBrush, markRect);
            }

            if (state.IsFocused && !state.IsDisabled)
            {
                PaintFocusRing(g, bounds, options);
            }
        }

        private static Rectangle CalculateThumbRect(Rectangle bounds, CheckBoxItemState state, int borderWidth)
        {
            int inset = Math.Max(2, borderWidth + 1);
            int thumbSize = Math.Max(8, bounds.Height - (inset * 2));
            int y = bounds.Y + (bounds.Height - thumbSize) / 2;
            int left = bounds.X + inset;
            int right = bounds.Right - inset - thumbSize;
            int center = bounds.X + (bounds.Width - thumbSize) / 2;

            int x = state.IsChecked ? right : (state.IsIndeterminate ? center : left);
            return new Rectangle(x, y, thumbSize, thumbSize);
        }

        public override void PaintCheckMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options)
        {
            // Switch style uses thumb position as primary selected affordance.
        }

        public override void PaintIndeterminateMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options)
        {
            // Switch style indeterminate marker is drawn inside thumb.
        }

        public override void PaintText(Graphics g, Rectangle bounds, string text, CheckBoxRenderOptions options)
        {
            PaintStandardText(g, bounds, text, options);
        }
    }
}
