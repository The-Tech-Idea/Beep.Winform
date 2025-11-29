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
            int alpha = state switch
            {
                ControlState.Hovered => 20,
                ControlState.Pressed => 8,
                ControlState.Focused => 15,
                ControlState.Disabled => 0,
                _ => 10
            };

            if (alpha == 0) return path;

            // Use subtle shadow (Mica minimalism)
            return ShadowPainterHelpers.PaintSubtleShadow(
                g, path, radius,
                1, alpha);
        }
    }
}
