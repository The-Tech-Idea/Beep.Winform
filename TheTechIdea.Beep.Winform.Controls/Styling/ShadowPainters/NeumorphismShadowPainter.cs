using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Shadow painter for Neumorphism style
    /// Draws dual shadows (light top-left, dark bottom-right)
    /// </summary>
    public static class NeumorphismShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            // Neumorphism UX: Dual shadow system (light top-left, dark bottom-right) with state support
            if (!StyleShadows.UsesDualShadows(style)) return path;

            // Adjust shadow intensity based on state
            float shadowIntensity = 1.0f; // Base neumorphic intensity
            switch (state)
            {
                case ControlState.Hovered:
                    shadowIntensity = 1.3f; // More pronounced on hover
                    break;
                case ControlState.Pressed:
                    shadowIntensity = 0.7f; // Less pronounced on press (pressed in effect)
                    break;
                case ControlState.Focused:
                    shadowIntensity = 1.1f; // Slightly more on focus
                    break;
                case ControlState.Disabled:
                    shadowIntensity = 0.5f; // Much reduced when disabled
                    break;
                default: // Normal
                    break;
            }

            int offsetY = (int)(StyleShadows.GetShadowOffsetY(style) * shadowIntensity);
            int offsetX = (int)(StyleShadows.GetShadowOffsetX(style) * shadowIntensity);
            Color shadowColor = StyleShadows.GetShadowColor(style);
            Color highlightColor = StyleShadows.GetNeumorphismHighlight(style);

            // Adjust alpha based on intensity
            shadowColor = Color.FromArgb((int)(shadowColor.A * shadowIntensity), shadowColor);
            highlightColor = Color.FromArgb((int)(highlightColor.A * shadowIntensity), highlightColor);

            // Use the helper method for neumorphic shadows
            return ShadowPainterHelpers.PaintNeumorphicShadow(g, path, radius, shadowColor);
        }
    }
}
