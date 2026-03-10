using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
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
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var (bgColor, borderColor, checkMarkColor, fgColor) = GetCheckBoxColors(state, options);
            (bgColor, borderColor) = ApplyInteractionStateColors(state, bgColor, borderColor);

            // Switch-specific track styling: explicit checked/unchecked contrast.
            Color trackColor;
            if (state.IsChecked)
            {
                trackColor = bgColor;
            }
            else if (state.IsIndeterminate)
            {
                var checkedColor = CheckBoxThemeHelpers.GetCheckedBackgroundColor(options.Theme, options.UseThemeColors);
                var uncheckedColor = CheckBoxThemeHelpers.GetUncheckedBackgroundColor(options.Theme, options.UseThemeColors);
                trackColor = Color.FromArgb(
                    (checkedColor.R + uncheckedColor.R) / 2,
                    (checkedColor.G + uncheckedColor.G) / 2,
                    (checkedColor.B + uncheckedColor.B) / 2);
            }
            else
            {
                var uncheckedBase = CheckBoxThemeHelpers.GetUncheckedBackgroundColor(options.Theme, options.UseThemeColors);
                if (uncheckedBase == Color.Empty || uncheckedBase.A == 0)
                {
                    uncheckedBase = Color.FromArgb(30, borderColor);
                }
                trackColor = state.IsHovered ? ControlPaint.Light(uncheckedBase, 0.05f) : uncheckedBase;
            }

            if (state.IsDisabled)
            {
                trackColor = Color.FromArgb(170, trackColor);
                borderColor = Color.FromArgb(170, borderColor);
            }

            int radius = bounds.Height / 2;
            using (var trackPath = CreateRoundedPath(bounds, radius))
            {
                using (var trackBrush = new SolidBrush(trackColor))
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
            // Subtle shadow improves thumb separation from track.
            using (var shadowBrush = new SolidBrush(Color.FromArgb(35, 0, 0, 0)))
            {
                var shadowRect = new Rectangle(thumbRect.X, thumbRect.Y + 1, thumbRect.Width, thumbRect.Height);
                g.FillEllipse(shadowBrush, shadowRect);
            }
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
            // Micro-polish: slightly smaller thumb + symmetric edge gaps for a cleaner switch feel.
            int inset = Math.Max(2, borderWidth + 1);
            int visualInset = inset + 1;
            int thumbSize = Math.Max(8, bounds.Height - (visualInset * 2) - 1);
            int y = bounds.Y + (bounds.Height - thumbSize) / 2;
            int left = bounds.X + inset;
            int right = bounds.Right - inset - thumbSize;
            int center = bounds.X + (bounds.Width - thumbSize) / 2;

            // Indeterminate sits closer to center-left to avoid looking like a weak "checked" state.
            int indeterminate = left + Math.Max(1, (right - left) / 3);
            int x = state.IsChecked ? right : (state.IsIndeterminate ? indeterminate : left);
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
