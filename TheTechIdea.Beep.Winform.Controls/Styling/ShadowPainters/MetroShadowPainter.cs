using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Metro shadow painter - NO shadows (flat design signature)
    /// Metro/Modern UI design principle: Pure flat design with no depth effects
    /// Returns path unchanged - this is intentional
    /// </summary>
    public static class MetroShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // Metro: NO shadows - flat design is the core principle
            // StyleShadows.HasShadow(Metro) returns false
            // Always return path unchanged - shadows would break the Metro aesthetic
            return path;
        }
    }
}
