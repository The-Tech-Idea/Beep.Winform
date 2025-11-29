using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Nordic theme background painter - light Scandinavian design
    /// Clean, bright background with cool blue undertones
    /// </summary>
    public static class NordicBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Nordic light background (Scandinavian minimalism)
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xF2, 0xF5, 0xF8);
            
            // Cool blue accent
            Color accent = Color.FromArgb(0xA3, 0xB8, 0xC9);

            // Paint solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Subtle cool gradient overlay (natural light from above)
            var gradient = PaintersFactory.GetLinearGradientBrush(
                bounds, 
                Color.FromArgb(0, accent), 
                Color.FromArgb(25, accent), 
                LinearGradientMode.Vertical);
            g.FillPath(gradient, path);
        }
    }
}
