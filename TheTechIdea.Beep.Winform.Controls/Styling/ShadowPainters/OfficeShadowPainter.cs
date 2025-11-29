using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Office shadow painter - Microsoft Office Ribbon UI
    /// Professional, subtle shadows for enterprise applications
    /// Only visible on interaction states (professional clean look)
    /// </summary>
    public static class OfficeShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Office: Shadow only on interaction (clean ribbon appearance)
            int alpha = state switch
            {
                ControlState.Hovered => 30,    // Subtle on hover
                ControlState.Pressed => 15,    // Minimal on press
                ControlState.Focused => 35,    // Moderate focus
                ControlState.Selected => 40,   // Most visible when selected
                ControlState.Disabled => 0,    // No shadow when disabled
                _ => 0                         // No shadow in normal state (clean look)
            };

            // No shadow in normal/disabled state
            if (alpha == 0) return path;

            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Use clean drop shadow (Office professional)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                2);
        }
    }
}
