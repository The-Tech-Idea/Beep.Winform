using System;
using System.Collections.Concurrent;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Factory that resolves the correct <see cref="IToolTipPainter"/> for a given
    /// <see cref="ToolTipConfig"/>.  Centralises the style-to-painter mapping so
    /// callers never need <c>if/switch</c> chains.
    /// </summary>
    public static class ToolTipPainterFactory
    {
        // B3: Pool one painter per variant to avoid allocating a new instance
        // on every tooltip show. Painters are stateless w.r.t. their inputs
        // (config, theme, bounds are passed per-call), so sharing is safe.
        // ConcurrentDictionary makes GetOrAdd atomic across threads.
        private static readonly ConcurrentDictionary<ToolTipLayoutVariant, IToolTipPainter> _pool = new();

        /// <summary>
        /// Return the appropriate painter for <paramref name="config"/>.
        /// </summary>
        public static IToolTipPainter GetPainter(ToolTipConfig config)
        {
            if (config == null)
                return GetPooled(ToolTipLayoutVariant.Simple);

            return GetPooled(config.LayoutVariant);
        }

        /// <summary>
        /// Convenience overload:  resolve by <see cref="ToolTipLayoutVariant"/> directly.
        /// </summary>
        public static IToolTipPainter GetPainter(ToolTipLayoutVariant variant)
        {
            return GetPooled(variant);
        }

        private static IToolTipPainter GetPooled(ToolTipLayoutVariant variant)
        {
            return _pool.GetOrAdd(variant, v => v switch
            {
                ToolTipLayoutVariant.Preview  => (IToolTipPainter)new PreviewToolTipPainter(),
                ToolTipLayoutVariant.Tour     => new TourToolTipPainter(),
                ToolTipLayoutVariant.Glass    => new GlassToolTipPainter(),
                _                             => new BeepStyledToolTipPainter()
            });
        }

        /// <summary>
        /// B3: Clear the pool. Call on theme/DPI change or application shutdown.
        /// </summary>
        public static void ClearPool()
        {
            _pool.Clear();
        }
    }
}
