using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material Design background painter - Google's material design language
    /// Clean surface with subtle elevation hint (top highlight)
    /// </summary>
    public static class MaterialBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Material: clean white/light surface
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Material);

            // Solid background with strong Material-style state feedback
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);

            // Material elevation hint: subtle top highlight
            BackgroundPainterHelpers.PaintTopHighlight(g, path, 12);
        }
    }
}
