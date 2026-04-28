using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupIconRenderer
    {
        internal static int ScaleLogical(Graphics g, int logicalPixels)
        {
            if (logicalPixels <= 0)
            {
                return 0;
            }

            float dpi = g?.DpiX > 0 ? g.DpiX : 96f;
            return Math.Max(1, (int)Math.Round(logicalPixels * (dpi / 96f)));
        }

        internal static int ComputeAdaptiveIconSize(int containerWidth, int containerHeight, int minSize, int padding = 6)
        {
            int minSide = Math.Min(containerWidth, containerHeight);
            int maxByContainer = Math.Max(1, minSide - Math.Max(0, padding));
            int preferred = Math.Max(minSize, (int)Math.Round(minSide * 0.45f));
            return Math.Max(1, Math.Min(preferred, maxByContainer));
        }

        internal static void PaintRowImage(Graphics g, Rectangle rect, string imagePath, bool enabled, bool circular, Color disabledBackColor)
        {
            if (g == null || rect.IsEmpty || string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            if (!enabled)
            {
                StyledImagePainter.PaintDisabled(g, rect, imagePath, disabledBackColor);
                return;
            }

            if (circular)
            {
                GraphicsState state = g.Save();
                using var clipPath = new GraphicsPath();
                clipPath.AddEllipse(rect);
                g.SetClip(clipPath, CombineMode.Intersect);
                StyledImagePainter.Paint(g, rect, imagePath, BeepControlStyle.Minimal);
                g.Restore(state);
                return;
            }

            StyledImagePainter.Paint(g, rect, imagePath, BeepControlStyle.Minimal);
        }

        internal static void PaintCheckIcon(Graphics g, Rectangle rect, Color tint, float opacity = 0.9f)
        {
            if (g == null || rect.IsEmpty)
            {
                return;
            }

            using var path = new GraphicsPath();
            path.AddRectangle(rect);
            StyledImagePainter.PaintWithTint(g, path, SvgsUI.Check, tint, opacity);
        }
    }
}
