using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Stripe dashboard shadow painter
    /// Uses Stripe's professional shadow system
    /// </summary>
    public static class StripeDashboardShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            // Stripe Dashboard UX: Professional shadow system with prominence
            if (!StyleShadows.HasShadow(style)) return;

            // Stripe shadow prominence based on state
            float shadowOpacity = 0.22f; // Base Stripe professional shadow
            int shadowLayers = 5;

            if (state == ControlState.Hovered)
            {
                shadowOpacity = 0.32f; // More prominent on hover
                shadowLayers = 7;
            }
            else if (state == ControlState.Focused)
            {
                shadowOpacity = 0.27f; // Moderate increase on focus
                shadowLayers = 6;
            }
            else if (state == ControlState.Pressed)
            {
                shadowOpacity = 0.17f; // Reduced on press
                shadowLayers = 4;
            }

            // Stripe elevation levels
            if (elevation >= MaterialElevation.Level4)
            {
                shadowOpacity += 0.08f; // Very prominent
                shadowLayers += 2;
            }
            else if (elevation >= MaterialElevation.Level3)
            {
                shadowOpacity += 0.05f; // More prominent
                shadowLayers += 1;
            }

            // Use StyleShadows for consistent Stripe shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, shadowOpacity, shadowLayers);
        }
    }
}
