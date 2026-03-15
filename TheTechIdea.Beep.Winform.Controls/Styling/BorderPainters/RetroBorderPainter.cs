using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Retro border painter — Win95-style beveled frame that follows the passed
    /// GraphicsPath. Draws an outer highlight stroke (light) and inner shadow stroke
    /// (dark) at different insets to create the classic raised bevel effect.
    /// </summary>
    public static class RetroBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            const float outerWidth = 2f;
            const float innerWidth = 1f;

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path;

            var oldSmooth = g.SmoothingMode;
            var oldPixel = g.PixelOffsetMode;
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.None;

            Color light = Color.FromArgb(255, 255, 255);
            Color dark = Color.FromArgb(128, 128, 128);

            // Pressed state inverts the bevel (sunken effect)
            if (state == ControlState.Pressed)
            {
                Color temp = light;
                light = dark;
                dark = temp;
            }

            // Outer bevel — highlight stroke (follows path shape)
            using var outerInset = BorderPainterHelpers.CreateStrokeInsetPath(path, outerWidth / 2f);
            GraphicsPath outerTarget = (outerInset != null && outerInset.PointCount > 2) ? outerInset : path;
            var penLight = PaintersFactory.GetPen(light, outerWidth);
            g.DrawPath(penLight, outerTarget);

            // Inner bevel — shadow stroke (inset further)
            using var innerInset = BorderPainterHelpers.CreateStrokeInsetPath(path, outerWidth + innerWidth);
            GraphicsPath innerTarget = (innerInset != null && innerInset.PointCount > 2) ? innerInset : path;
            var penDark = PaintersFactory.GetPen(dark, innerWidth);
            g.DrawPath(penDark, innerTarget);

            // Accent ring — subtle inner border for depth
            using var accentInset = BorderPainterHelpers.CreateStrokeInsetPath(path, outerWidth + innerWidth + 2f);
            if (accentInset != null && accentInset.PointCount > 2)
            {
                var penAccent = PaintersFactory.GetPen(Color.FromArgb(180, 160, 160, 160), 1f);
                g.DrawPath(penAccent, accentInset);
            }

            g.SmoothingMode = oldSmooth;
            g.PixelOffsetMode = oldPixel;
            return path.CreateInsetPath(4f);
        }
    }
}
