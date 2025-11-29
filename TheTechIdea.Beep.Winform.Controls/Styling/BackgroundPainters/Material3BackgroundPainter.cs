using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material3 background painter - Google's Material Design 3 / Material You
    /// Tonal surfaces with dynamic color and elevation-based lighting
    /// </summary>
    public static class Material3BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Material3: tonal surface color
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Material3);

            // M3 uses strong, noticeable state changes (tonal elevation)
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);

            // Material3 elevation: white tonal overlay
            var elevationBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, Color.White));
            g.FillPath(elevationBrush, path);
        }
    }
}
