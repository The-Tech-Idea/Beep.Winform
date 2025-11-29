using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Retro background painter - classic computing aesthetic
    /// Gray background with CRT scanlines and dithering pattern
    /// Nostalgic old-school computing feel
    /// </summary>
    public static class RetroBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Retro: classic system gray
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xD8, 0xD8, 0xD8);

            // Solid background with state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);

            var bounds = Rectangle.Round(path.GetBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            using (var smoothScope = new BackgroundPainterHelpers.SmoothingScope(g, SmoothingMode.None))
            {
                // CRT scanlines
                BackgroundPainterHelpers.PaintScanlineOverlay(g, bounds, 
                    Color.FromArgb(20, 0, 0, 0), 3);

                // Dithering pattern (classic computing texture)
                using (var hatchBrush = new HatchBrush(
                    HatchStyle.Percent50, 
                    Color.FromArgb(10, 0, 0, 0), 
                    Color.Transparent))
                {
                    g.FillRectangle(hatchBrush, bounds);
                }
            }
        }
    }
}
