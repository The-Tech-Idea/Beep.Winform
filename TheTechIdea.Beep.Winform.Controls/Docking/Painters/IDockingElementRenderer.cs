using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Common contract for every distinct docking element renderer (caption, tab strip,
    /// splitter, auto-hide strip, drag guide). Renderers are stateless and only paint the
    /// state resolved in the supplied <see cref="DockingPainterContext"/>; all hit-testing,
    /// hover/press/active detection and action raising belong to the matching layout manager.
    /// </summary>
    /// <remarks>
    /// House style: distinct renderers, no shared base painter. Each renderer pairs with a
    /// layout manager that computes geometry and resolves interaction state.
    /// </remarks>
    internal interface IDockingElementRenderer
    {
        /// <summary>
        /// Paints this element using the theme, palette, style and state in <paramref name="context"/>.
        /// Background/border/shadow come from the Beep painter factories so docking chrome
        /// matches form/control styling exactly.
        /// </summary>
        /// <param name="g">Target graphics surface.</param>
        /// <param name="context">Resolved render context (theme, colors, style, state, bounds).</param>
        void Paint(Graphics g, DockingPainterContext context);
    }
}
