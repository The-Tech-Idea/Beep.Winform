using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Metro shadow painter - NO shadows (flat design signature)
    /// Metro design principle: Flat UI with no depth effects
    /// Returns path unchanged for consistency with other painters
    /// </summary>
    public static class MetroShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // Metro: NO shadows - flat design is the signature
            // StyleShadows.HasShadow(Metro) returns false
            // Return path unchanged
            return path;
        }
    }
}
