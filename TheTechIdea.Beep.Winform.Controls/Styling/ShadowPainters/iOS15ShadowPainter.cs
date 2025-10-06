using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// iOS 15 shadow painter
    /// Uses subtle shadows consistent with iOS design
    /// </summary>
    public static class iOS15ShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // iOS15 UX: Subtle blur effects, not traditional shadows
            if (!StyleShadows.HasShadow(style)) return;

            // iOS uses blur effects - adjust intensity based on state
            float blurIntensity = 0.15f; // Base subtle blur
            if (state == ControlState.Hovered)
                blurIntensity = 0.25f; // Slightly more prominent on hover
            else if (state == ControlState.Focused)
                blurIntensity = 0.20f; // Moderate increase on focus
            else if (state == ControlState.Pressed)
                blurIntensity = 0.08f; // Reduced on press (pressed in effect)

            // Use StyleShadows for consistent iOS blur behavior
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, blurIntensity, blur / 3);
        }
    }
}
