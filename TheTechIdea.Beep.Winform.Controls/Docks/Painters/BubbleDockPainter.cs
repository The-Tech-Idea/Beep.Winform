using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Soft blue palette over the floating pill renderer.
    /// </summary>
    public sealed class BubbleDockPainter : FloatingDockPainter
    {
        protected override Color? StyleBackgroundColor => Color.FromArgb(230, 246, 251, 255);
        protected override Color? StyleBorderColor => Color.FromArgb(180, 206, 228, 244);
        protected override float? StyleBackgroundOpacity => 0.9f;
    }
}
