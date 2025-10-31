using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// macOS Big Sur shadow painter
    /// Uses soft shadows with vibrancy
    /// </summary>
    public static class MacOSBigSurShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // macOS Big Sur UX: Soft shadows with vibrancy and state changes
            if (!StyleShadows.HasShadow(style)) return path;

            // macOS shadow vibrancy based on state
            float shadowOpacity = 0.2f; // Base macOS subtle shadow
            if (state == ControlState.Hovered)
                shadowOpacity = 0.28f; // More vibrant on hover
            else if (state == ControlState.Focused)
                shadowOpacity = 0.24f; // Moderate increase on focus
            else if (state == ControlState.Pressed)
                shadowOpacity = 0.12f; // Reduced on press

            // Match MacOSFormPainter: gentle shadows with proper offset
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintDropShadow(
                g, path, radius,
                0, 2, 8, // offsetX, offsetY, blur
                Color.FromArgb((int)(shadowOpacity * 255), 0, 0, 0),
                1.0f);

            return remainingPath;
        }
    }
}
