using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// KDE path painter - Breeze design system
    /// 4px radius (modern rounded)
    /// Breeze blue (#3DAEE9) fills
    /// Clean, flat KDE aesthetic
    /// </summary>
    public static class KdePathPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, ControlState state = ControlState.Normal)
        {
            // KDE Breeze blue
            Color breezeBlue = StyleColors.GetPrimary(BeepControlStyle.Kde); // #3DAEE9

            // Determine fill color based on state
            Color fillColor = state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(breezeBlue, 0.1f),
                ControlState.Pressed => ColorUtils.Darken(breezeBlue, 0.1f),
                ControlState.Selected => breezeBlue,
                ControlState.Focused => breezeBlue,
                _ => breezeBlue
            };

            // KDE: Clean flat fill (no gradients - modern minimalism)
            PathPainterHelpers.PaintSolidPath(g, path, fillColor);
        }
    }
}
