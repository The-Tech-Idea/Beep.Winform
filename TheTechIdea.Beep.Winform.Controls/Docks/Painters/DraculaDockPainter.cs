using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Dracula palette over the classic taskbar renderer.
    /// </summary>
    public sealed class DraculaDockPainter : ClassicTaskbarDockPainter
    {
        /// <summary>Named for the Dracula colour scheme. The palette is the reason the style was chosen, so it
        /// survives UseThemeColors.</summary>
        protected override bool IsNamedPalette => true;

        protected override Color? StyleBackgroundColor => Color.FromArgb(40, 42, 54);   // Dracula background
        protected override Color? StyleBorderColor => Color.FromArgb(98, 114, 164);     // Dracula comment
        protected override float? StyleBackgroundOpacity => 0.94f;
    }
}
