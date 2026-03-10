using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    internal static class ProgressBarLayoutHelper
    {
        public static Rectangle GetPaintBounds(BeepProgressBar owner, Rectangle drawingRect)
        {
            if (owner == null)
            {
                return drawingRect;
            }

            var rect = drawingRect;
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return Rectangle.Empty;
            }

            int thickness = Math.Max(0, ProgressBarDpiHelpers.Scale(owner, owner.BorderThickness));
            int maxInset = Math.Min(rect.Width / 2, rect.Height / 2);
            thickness = Math.Min(thickness, maxInset);
            rect.Inflate(-thickness, -thickness);
            return rect;
        }

        public static int GetScaledCornerRadius(BeepProgressBar owner, Rectangle bounds)
        {
            if (owner == null)
            {
                return 0;
            }

            int desired = owner.IsRounded ? ProgressBarDpiHelpers.Scale(owner, owner.BorderRadius) : 0;
            if (desired <= 0)
            {
                return 0;
            }

            return Math.Min(desired, Math.Min(bounds.Width, bounds.Height) / 2);
        }
    }
}
