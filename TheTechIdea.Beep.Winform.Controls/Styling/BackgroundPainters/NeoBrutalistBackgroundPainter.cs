using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// NeoBrutalist background painter - Bold, raw, brutalist design
    /// Bright bold colors (yellow/magenta), flat fills, NO gradients
    /// Neo-Brutalism signature: Aggressive simplicity with thick borders
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class NeoBrutalistBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // NeoBrutalist: BOLD colors, NO gradients, flat aggressive design
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.NeoBrutalist);
            Color secondaryColor = useThemeColors ? theme.SecondaryColor : StyleColors.GetSecondary(BeepControlStyle.NeoBrutalist);

            // NeoBrutalist-specific state handling - BOLD flat color swaps (no subtlety!)
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // NeoBrutalist hover: Swap to secondary color (magenta)
                    stateColor = secondaryColor; // Bold magenta!
                    break;
                case ControlState.Pressed:
                    // NeoBrutalist pressed: Darken secondary (deeper magenta)
                    int pR = Math.Max(0, secondaryColor.R - 40);
                    int pG = Math.Max(0, secondaryColor.G - 40);
                    int pB = Math.Max(0, secondaryColor.B - 40);
                    stateColor = Color.FromArgb(secondaryColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // NeoBrutalist selected: Full secondary color (bold magenta)
                    stateColor = secondaryColor;
                    break;
                case ControlState.Focused:
                    // NeoBrutalist focused: Slightly lighter yellow
                    int fR = Math.Min(255, backgroundColor.R + 20);
                    int fG = Math.Min(255, backgroundColor.G + 20);
                    int fB = Math.Min(255, backgroundColor.B + 20);
                    stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // NeoBrutalist disabled: Gray (lose the bold color)
                    stateColor = Color.FromArgb(200, 200, 200);
                    break;
                default: // Normal
                    // NeoBrutalist normal: Bold yellow background
                    stateColor = backgroundColor;
                    break;
            }

            // Fill with FLAT color (NO gradients - NeoBrutalist signature!)
            using (var brush = new SolidBrush(stateColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
