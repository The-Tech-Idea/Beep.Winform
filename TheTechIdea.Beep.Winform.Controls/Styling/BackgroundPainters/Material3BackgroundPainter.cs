using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material3 background painter - Solid with10% white elevation highlight
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Material3BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Material3: Solid background with elevation-based lighting
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Material3);

            // Material3-specific state handling - NO HELPER FUNCTIONS
            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R + (int)(255 *0.15f)), Math.Min(255, baseColor.G + (int)(255 *0.15f)), Math.Min(255, baseColor.B + (int)(255 *0.15f))),
                ControlState.Pressed => Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - (int)(baseColor.R *0.12f)), Math.Max(0, baseColor.G - (int)(baseColor.G *0.12f)), Math.Max(0, baseColor.B - (int)(baseColor.B *0.12f))),
                ControlState.Selected => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R + (int)(255 *0.20f)), Math.Min(255, baseColor.G + (int)(255 *0.20f)), Math.Min(255, baseColor.B + (int)(255 *0.20f))),
                ControlState.Focused => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R + (int)(255 *0.08f)), Math.Min(255, baseColor.G + (int)(255 *0.08f)), Math.Min(255, baseColor.B + (int)(255 *0.08f))),
                ControlState.Disabled => Color.FromArgb(80, baseColor),
                _ => baseColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);

            // Add elevation highlight (10% white overlay) - consistent across states
            var elevationBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(25, Color.White));
            g.FillPath(elevationBrush, path);
        }
    }
}
