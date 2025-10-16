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
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // iOS15 UX: Subtle blur effects, not traditional shadows
            if (!StyleShadows.HasShadow(style)) return path;

            // iOS uses blur effects - adjust intensity based on state
            float blurIntensity = 0.15f; // Base subtle blur
            if (state == ControlState.Hovered)
                blurIntensity = 0.25f; // Slightly more prominent on hover
            else if (state == ControlState.Focused)
                blurIntensity = 0.20f; // Moderate increase on focus
            else if (state == ControlState.Pressed)
                blurIntensity = 0.08f; // Reduced on press (pressed in effect)

            // Paint shadows
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(g, path, radius, 0, 0, StyleShadows.GetShadowColor(style), blurIntensity, StyleShadows.GetShadowBlur(style) / 3);
            return remainingPath;
        }
    }
}
