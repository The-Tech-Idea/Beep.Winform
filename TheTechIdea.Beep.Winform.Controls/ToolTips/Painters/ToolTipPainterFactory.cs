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
        /// <summary>
        /// Return the appropriate painter for <paramref name="config"/>.
        /// </summary>
        public static IToolTipPainter GetPainter(ToolTipConfig config)
        {
            if (config == null)
                return new BeepStyledToolTipPainter();

            // Layout-variant wins first
            return config.LayoutVariant switch
            {
                ToolTipLayoutVariant.Preview  => new PreviewToolTipPainter(),
                ToolTipLayoutVariant.Tour     => new TourToolTipPainter(),
                // All other variants use the single unified styled painter
                _                             => new BeepStyledToolTipPainter()
            };
        }

        /// <summary>
        /// Convenience overload:  resolve by <see cref="ToolTipLayoutVariant"/> directly.
        /// </summary>
        public static IToolTipPainter GetPainter(ToolTipLayoutVariant variant)
        {
            return variant switch
            {
                ToolTipLayoutVariant.Preview => new PreviewToolTipPainter(),
                ToolTipLayoutVariant.Tour    => new TourToolTipPainter(),
                ToolTipLayoutVariant.Glass   => new GlassToolTipPainter(),
                _                            => new BeepStyledToolTipPainter()
            };
        }
    }
}
