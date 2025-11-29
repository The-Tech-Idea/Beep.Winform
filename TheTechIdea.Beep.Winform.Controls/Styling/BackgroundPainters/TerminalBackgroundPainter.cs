using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Terminal background painter - authentic CRT/terminal aesthetic
    /// Dark background with scanlines and subtle grid overlay
    /// </summary>
    public static class TerminalBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Terminal colors: deep black base with green accent
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0x0C, 0x0C, 0x0C); // Almost pure black
            Color accent = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.FromArgb(0x00, 0xFF, 0x00); // Classic terminal green

            // Paint solid background with state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state, 
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = Rectangle.Round(path.GetBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Scanline effect (CRT phosphor lines)
                BackgroundPainterHelpers.PaintScanlineOverlay(g, bounds, 
                    Color.FromArgb(8, 255, 255, 255), 2);

                // Grid overlay (subtle tech grid)
                BackgroundPainterHelpers.PaintGridOverlay(g, bounds, 
                    Color.FromArgb(5, accent), 20);
            }
        }
    }
}
