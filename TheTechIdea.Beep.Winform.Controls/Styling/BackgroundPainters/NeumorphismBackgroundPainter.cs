using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Neumorphism background painter - soft 3D embossed effect
    /// Inner highlight that inverts on press for tactile feedback
    /// </summary>
    public static class NeumorphismBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Neumorphism: soft gray base
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Neumorphism);

            // Use the neumorphic helper
            BackgroundPainterHelpers.PaintNeumorphicBackground(g, path, baseColor, state);
        }
    }
}
