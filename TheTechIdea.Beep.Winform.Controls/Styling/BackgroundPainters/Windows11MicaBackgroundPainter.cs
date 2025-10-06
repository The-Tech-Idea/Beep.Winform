using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Windows 11 Mica background painter - Vertical gradient (2% darker at bottom)
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Windows11MicaBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Windows 11 Mica: Subtle mica material with state-aware tinting
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Windows11Mica);
            Color primaryColor = useThemeColors ? theme.PrimaryColor : StyleColors.GetPrimary(BeepControlStyle.Windows11Mica);

            // Windows11Mica-specific state handling - NO HELPER FUNCTIONS
            // Unique Mica tint intensity changes for Windows 11 design
            Color stateColor;
            float micaTintStrength;
            
            switch (state)
            {
                case ControlState.Hovered:
                    // Windows11 hover: 8% tint with primary color
                    micaTintStrength = 0.08f;
                    int hR = (int)(baseColor.R * (1f - micaTintStrength) + primaryColor.R * micaTintStrength);
                    int hG = (int)(baseColor.G * (1f - micaTintStrength) + primaryColor.G * micaTintStrength);
                    int hB = (int)(baseColor.B * (1f - micaTintStrength) + primaryColor.B * micaTintStrength);
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Windows11 pressed: 12% darker with more tint
                    micaTintStrength = 0.12f;
                    int pR = (int)(baseColor.R * (1f - micaTintStrength) + primaryColor.R * micaTintStrength);
                    int pG = (int)(baseColor.G * (1f - micaTintStrength) + primaryColor.G * micaTintStrength);
                    int pB = (int)(baseColor.B * (1f - micaTintStrength) + primaryColor.B * micaTintStrength);
                    // Also darken 10%
                    pR = Math.Max(0, pR - (int)(pR * 0.10f));
                    pG = Math.Max(0, pG - (int)(pG * 0.10f));
                    pB = Math.Max(0, pB - (int)(pB * 0.10f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Windows11 selected: 10% tint (noticeable)
                    micaTintStrength = 0.10f;
                    int sR = (int)(baseColor.R * (1f - micaTintStrength) + primaryColor.R * micaTintStrength);
                    int sG = (int)(baseColor.G * (1f - micaTintStrength) + primaryColor.G * micaTintStrength);
                    int sB = (int)(baseColor.B * (1f - micaTintStrength) + primaryColor.B * micaTintStrength);
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Windows11 focused: 6% subtle tint
                    micaTintStrength = 0.06f;
                    int fR = (int)(baseColor.R * (1f - micaTintStrength) + primaryColor.R * micaTintStrength);
                    int fG = (int)(baseColor.G * (1f - micaTintStrength) + primaryColor.G * micaTintStrength);
                    int fB = (int)(baseColor.B * (1f - micaTintStrength) + primaryColor.B * micaTintStrength);
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Windows11 disabled: 2% tint, very dim
                    micaTintStrength = 0.02f;
                    int dR = (int)(baseColor.R * (1f - micaTintStrength) + primaryColor.R * micaTintStrength);
                    int dG = (int)(baseColor.G * (1f - micaTintStrength) + primaryColor.G * micaTintStrength);
                    int dB = (int)(baseColor.B * (1f - micaTintStrength) + primaryColor.B * micaTintStrength);
                    stateColor = Color.FromArgb(75, dR, dG, dB); // 75 alpha
                    break;
                default: // Normal
                    // Normal: 5% default Mica tint
                    micaTintStrength = 0.05f;
                    int nR = (int)(baseColor.R * (1f - micaTintStrength) + primaryColor.R * micaTintStrength);
                    int nG = (int)(baseColor.G * (1f - micaTintStrength) + primaryColor.G * micaTintStrength);
                    int nB = (int)(baseColor.B * (1f - micaTintStrength) + primaryColor.B * micaTintStrength);
                    stateColor = Color.FromArgb(baseColor.A, nR, nG, nB);
                    break;
            }

            // Create Mica gradient effect (2% darker at bottom)
            Color topColor = stateColor;
            int bottomR = Math.Max(0, stateColor.R - (int)(stateColor.R * 0.02f));
            int bottomG = Math.Max(0, stateColor.G - (int)(stateColor.G * 0.02f));
            int bottomB = Math.Max(0, stateColor.B - (int)(stateColor.B * 0.02f));
            Color bottomColor = Color.FromArgb(stateColor.A, bottomR, bottomG, bottomB);

            using (var brush = new LinearGradientBrush(bounds, topColor, bottomColor, 90f))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }
        }
    }
}
