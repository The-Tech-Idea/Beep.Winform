using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Windows 11 Mica shadow painter
    /// Mica effect uses transparency, not shadows
    /// Very minimal shadow for depth hint when needed
    /// </summary>
    public static class Windows11MicaShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            
            // Mica relies on background blur, not shadows
            // Very minimal shadow only when absolutely needed
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Mica: Extremely subtle shadow (almost none)
            // Increased opacity slightly for visibility
            float opacity = state switch
            {
                ControlState.Hovered => 0.25f,
                ControlState.Pressed => 0.1f,
                ControlState.Focused => 0.2f,
                ControlState.Disabled => 0.05f,
                _ => 0.15f
            };

            if (opacity <= 0) return path;

            // Mica shadow - use darker theme color instead of pure black
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }
            else
            {
                // Use darker gray for more realistic shadows
                shadowColor = Color.FromArgb(30, 30, 30);
            }

            // Use soft shadow (Mica minimalism)
            return ShadowPainterHelpers.PaintSoftShadow(
                g, path, radius,
                0, 2, shadowColor, opacity, 4);
        }
    }
}
