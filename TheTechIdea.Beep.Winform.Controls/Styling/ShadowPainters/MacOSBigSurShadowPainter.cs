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
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // macOS Big Sur UX: Soft shadows with vibrancy and state changes
            if (!StyleShadows.HasShadow(style)) return;

            // macOS shadow vibrancy based on state
            float shadowOpacity = 0.2f; // Base macOS subtle shadow
            if (state == ControlState.Hovered)
                shadowOpacity = 0.28f; // More vibrant on hover
            else if (state == ControlState.Focused)
                shadowOpacity = 0.24f; // Moderate increase on focus
            else if (state == ControlState.Pressed)
                shadowOpacity = 0.12f; // Reduced on press

            // Use StyleShadows for consistent macOS shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, shadowOpacity, blur / 3);
        }
    }
}
