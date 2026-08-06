using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Terminal/console palette over the classic taskbar renderer.
    /// </summary>
    public sealed class TerminalDockPainter : ClassicTaskbarDockPainter
    {
        /// <summary>Named for a terminal palette. The palette is the reason the style was chosen, so it
        /// survives UseThemeColors.</summary>
        protected override bool IsNamedPalette => true;

        protected override Color? StyleBackgroundColor => Color.FromArgb(16, 22, 16);
        protected override Color? StyleBorderColor => Color.FromArgb(80, 220, 160);
        protected override float? StyleBackgroundOpacity => 0.96f;
    }
}
