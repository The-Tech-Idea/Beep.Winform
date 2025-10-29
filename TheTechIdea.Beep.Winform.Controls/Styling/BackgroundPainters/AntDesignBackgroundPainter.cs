using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling; // import factory

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Ant Design background painter - Solid white background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class AntDesignBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Ant Design: Clean white solid with border-aware tinting
            Color baseColor = StyleColors.GetBackground(BeepControlStyle.AntDesign);

            // AntDesign-specific state handling - NO HELPER FUNCTIONS
            // Unique border-aware subtle tinting for Ant Design
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // AntDesign hover: 8% lighter (noticeable but clean)
                    int hR = Math.Min(255, baseColor.R + (int)(255 * 0.08f));
                    int hG = Math.Min(255, baseColor.G + (int)(255 * 0.08f));
                    int hB = Math.Min(255, baseColor.B + (int)(255 * 0.08f));
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // AntDesign pressed: 12% darker (strong press feedback)
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.12f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.12f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.12f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // AntDesign selected: 12% lighter (bold selection)
                    int sR = Math.Min(255, baseColor.R + (int)(255 * 0.12f));
                    int sG = Math.Min(255, baseColor.G + (int)(255 * 0.12f));
                    int sB = Math.Min(255, baseColor.B + (int)(255 * 0.12f));
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // AntDesign focused: 6% lighter (gentle focus)
                    int fR = Math.Min(255, baseColor.R + (int)(255 * 0.06f));
                    int fG = Math.Min(255, baseColor.G + (int)(255 * 0.06f));
                    int fB = Math.Min(255, baseColor.B + (int)(255 * 0.06f));
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // AntDesign disabled: 90 alpha
                    stateColor = Color.FromArgb(90, baseColor);
                    break;
                default: // Normal
                    stateColor = baseColor;
                    break;
            }

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
