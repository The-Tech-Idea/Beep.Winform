using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Arc/i3 palette over the minimal renderer.
    /// </summary>
    /// <remarks>
    /// The background opacity this painter used to write (<c>0.95f</c>) is deliberately not declared
    /// here. <see cref="MinimalDockPainter"/> fills at <c>0.05f</c> and never read the config value,
    /// so the write changed nothing about how Arc rendered - it only persisted 0.95 into the shared
    /// config, where the next style to paint picked it up. Declaring it would make Arc opaque, which
    /// is a visual change this stage has no mandate for; if Arc is meant to be opaque that is a
    /// deliberate design decision, not a side effect of removing a mutation.
    /// </remarks>
    public sealed class ArcDockPainter : MinimalDockPainter
    {
        /// <summary>Named for the Arc/i3 palette. The palette is the reason the style was chosen, so it
        /// survives UseThemeColors.</summary>
        protected override bool IsNamedPalette => true;

        protected override Color? StyleBackgroundColor => Color.FromArgb(244, 245, 247);
        protected override Color? StyleBorderColor => Color.FromArgb(220, 225, 230);
    }
}
