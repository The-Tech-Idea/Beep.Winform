using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// NeoBrutalist shadow painter - NO SHADOWS (CRITICAL SIGNATURE!)
    /// Neo-Brutalism principle: Flat, raw, aggressive design with NO depth effects
    /// Shadows would contradict the brutalist aesthetic
    /// Returns path unchanged
    /// </summary>
    public static class NeoBrutalistShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // NeoBrutalist: ABSOLUTELY NO SHADOWS - This is THE signature!
            // StyleShadows.HasShadow(NeoBrutalist) returns FALSE
            // Flat, raw, aggressive - shadows would ruin the aesthetic
            // Return path unchanged
            return path;
        }
    }
}
