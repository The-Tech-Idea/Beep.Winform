using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// HighContrast path painter - WCAG AAA accessibility compliance
    /// Pure colors for maximum contrast, 0px radius (sharp for clarity)
    /// Black (#000000) fills, yellow (#FFFF00) selection
    /// No gradients - flat fills for accessibility
    /// </summary>
    public static class HighContrastPathPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, ControlState state = ControlState.Normal)
        {
            // HighContrast: Pure colors for WCAG AAA compliance
            Color blackFill = StyleColors.GetPrimary(BeepControlStyle.HighContrast); // Black (#000000)
            Color yellowSelection = StyleColors.GetSelection(BeepControlStyle.HighContrast); // Yellow (#FFFF00)

            // Determine fill color based on state
            Color fillColor = state switch
            {
                ControlState.Selected => yellowSelection, // Yellow for max contrast
                ControlState.Focused => yellowSelection,  // Yellow for keyboard focus visibility
                _ => blackFill // Black for normal/hover/pressed
            };

            // Paint with flat solid color - NO gradients (accessibility requirement)
            PathPainterHelpers.PaintSolidPath(g, path, fillColor);
        }
    }
}
