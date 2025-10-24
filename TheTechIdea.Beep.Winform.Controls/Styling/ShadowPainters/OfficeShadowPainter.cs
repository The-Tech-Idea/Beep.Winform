using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Office shadow painter - Microsoft Office Ribbon UI subtle elevation
    /// Subtle shadows for professional depth, 8px blur with 2px offset
    /// Only visible on interaction states (hover, selected, focused)
    /// </summary>
    public static class OfficeShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // Office: Subtle shadows for professional elevation
            if (!StyleShadows.HasShadow(BeepControlStyle.Office)) return path;

            // Office: Only show shadow on interaction states (professional subtlety)
            float shadowOpacity = 0f;
            
            switch (state)
            {
                case ControlState.Hovered:
                    shadowOpacity = 0.08f; // Very subtle on hover
                    break;
                case ControlState.Focused:
                    shadowOpacity = 0.10f; // Slightly more on focus
                    break;
                case ControlState.Selected:
                    shadowOpacity = 0.12f; // Most visible when selected
                    break;
                case ControlState.Pressed:
                    shadowOpacity = 0.05f; // Minimal on press
                    break;
                default:
                    // Office: No shadow in normal state (clean ribbon appearance)
                    return path;
            }

            // Use StyleShadows for consistent Office shadows
            Color shadowColor = StyleShadows.GetShadowColor(BeepControlStyle.Office); // Subtle black
            int blur = StyleShadows.GetShadowBlur(BeepControlStyle.Office); // 8px
            int offsetY = StyleShadows.GetShadowOffsetY(BeepControlStyle.Office); // 2px below
            int offsetX = StyleShadows.GetShadowOffsetX(BeepControlStyle.Office); // 0px (centered)

            // Paint subtle soft shadow (Office professional Style)
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(
                g, path, radius, offsetX, offsetY, shadowColor, shadowOpacity, blur / 4);

            return remainingPath;
        }
    }
}
