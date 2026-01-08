using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
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

            // Radial gradients could enhance embossed effect - add subtle radial for smoother neumorphic appearance
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                Color centerColor = ColorAccessibilityHelper.LightenColor(baseColor, 0.02f);
                Color edgeColor = baseColor;
                BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
            }
        }
    }
}
