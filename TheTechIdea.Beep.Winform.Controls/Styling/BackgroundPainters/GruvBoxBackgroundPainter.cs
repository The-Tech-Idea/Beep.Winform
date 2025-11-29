using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// GruvBox theme background painter - retro groove color scheme
    /// Warm, earthy tones with a vintage CRT feel and subtle grain
    /// </summary>
    public static class GruvBoxBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // GruvBox dark hard background
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0x28, 0x28, 0x28); // bg0_h
            
            // GruvBox orange accent
            Color accent = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.FromArgb(0xFB, 0xB8, 0x6C); // orange bright

            // Paint solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = Rectangle.Round(path.GetBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Warm grain effect (retro CRT feel)
                BackgroundPainterHelpers.PaintScanlineOverlay(g, bounds, 
                    Color.FromArgb(12, accent), 3);

                // Warm top glow
                int glowHeight = Math.Max(1, bounds.Height / 6);
                var topRect = new Rectangle(bounds.Left, bounds.Top, bounds.Width, glowHeight);
                var glow = PaintersFactory.GetLinearGradientBrush(
                    topRect, 
                    Color.FromArgb(30, accent), 
                    Color.Transparent, 
                    LinearGradientMode.Vertical);
                g.FillRectangle(glow, topRect);
            }
        }
    }
}
