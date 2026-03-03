using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    /// <summary>
    /// Sprint 2 — Common contract for all marquee item renderers.
    /// Each painter implements a specific <see cref="MarqueeStyle"/> visual.
    /// </summary>
    public interface IMarqueeItemRenderer
    {
        /// <summary>Human-readable name for design-time display.</summary>
        string Name { get; }

        /// <summary>
        /// Measure the rendered size of <paramref name="item"/> without drawing it.
        /// </summary>
        Size Measure(System.Drawing.Graphics g, MarqueeItem item, MarqueeRenderContext ctx);

        /// <summary>
        /// Draw <paramref name="item"/> into <paramref name="dest"/>.
        /// The renderer should clip to <paramref name="dest"/> and not paint outside it.
        /// </summary>
        void Draw(System.Drawing.Graphics g, MarqueeItem item,
                  System.Drawing.RectangleF dest, MarqueeRenderContext ctx);
    }
}
