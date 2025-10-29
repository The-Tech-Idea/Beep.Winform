using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Fluent2 background painter - Solid clean background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Fluent2BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Fluent2: Clean solid background with acrylic-inspired opacity changes
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Fluent2);

            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R + (int)(255 *0.10f)), Math.Min(255, baseColor.G + (int)(255 *0.10f)), Math.Min(255, baseColor.B + (int)(255 *0.10f))),
                ControlState.Pressed => Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - (int)(baseColor.R *0.08f)), Math.Max(0, baseColor.G - (int)(baseColor.G *0.08f)), Math.Max(0, baseColor.B - (int)(baseColor.B *0.08f))),
                ControlState.Selected => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R + (int)(255 *0.14f)), Math.Min(255, baseColor.G + (int)(255 *0.14f)), Math.Min(255, baseColor.B + (int)(255 *0.14f))),
                ControlState.Focused => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R + (int)(255 *0.06f)), Math.Min(255, baseColor.G + (int)(255 *0.06f)), Math.Min(255, baseColor.B + (int)(255 *0.06f))),
                ControlState.Disabled => Color.FromArgb(70, baseColor),
                _ => baseColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
