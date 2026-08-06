using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Cyberpunk palette over the classic taskbar renderer.
    /// </summary>
    public sealed class CyberpunkDockPainter : ClassicTaskbarDockPainter
    {
        /// <summary>Named for a cyberpunk palette. The palette is the reason the style was chosen, so it
        /// survives UseThemeColors.</summary>
        protected override bool IsNamedPalette => true;

        protected override Color? StyleBackgroundColor => Color.FromArgb(28, 14, 45);
        protected override Color? StyleBorderColor => Color.FromArgb(0, 255, 222);
        protected override float? StyleBackgroundOpacity => 0.92f;
    }
}
