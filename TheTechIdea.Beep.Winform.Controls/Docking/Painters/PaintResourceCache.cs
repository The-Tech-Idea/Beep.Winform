using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Thread-local cache of <see cref="SolidBrush"/> and <see cref="Pen"/> instances keyed by
    /// color value. Shared across renderers to eliminate per-paint allocations. Cleared when
    /// the theme changes (colors mutate) or on disposal.
    /// </summary>
    internal sealed class PaintResourceCache : IDisposable
    {
        private readonly Dictionary<int, SolidBrush> _brushes = new Dictionary<int, SolidBrush>();
        private readonly Dictionary<int, Pen> _pens = new Dictionary<int, Pen>();

        /// <summary>
        /// Returns the cached brush for <paramref name="color"/>, creating it on first use.
        /// </summary>
        /// <remarks>
        /// <b>The returned brush is borrowed, not owned — never wrap it in <c>using</c>.</b> The
        /// cache owns every instance and releases them in <see cref="Clear"/>. Disposing one here
        /// leaves a disposed object in the dictionary, and the next caller asking for the same
        /// color gets it back and fails with <c>ArgumentException: Parameter is not valid</c> on
        /// the first draw. That is not hypothetical: all 32 call sites across the caption,
        /// auto-hide and splitter renderers did exactly this, so painting a dockspace header threw
        /// as soon as any color was used twice — and the cache saved no allocations either, since
        /// every entry was destroyed immediately after being created.
        /// </remarks>
        public SolidBrush GetBrush(Color color)
        {
            int key = color.ToArgb();
            if (!_brushes.TryGetValue(key, out var brush))
            {
                brush = new SolidBrush(color);
                _brushes[key] = brush;
            }
            return brush;
        }

        /// <summary>
        /// Returns the cached pen for <paramref name="color"/> and <paramref name="width"/>.
        /// </summary>
        /// <remarks>Borrowed, not owned - see <see cref="GetBrush"/>. Never wrap in <c>using</c>.</remarks>
        public Pen GetPen(Color color, float width = 1f)
        {
            int key = color.ToArgb() ^ ((int)(width * 100) << 16);
            if (!_pens.TryGetValue(key, out var pen))
            {
                pen = new Pen(color, width);
                _pens[key] = pen;
            }
            return pen;
        }

        public void Clear()
        {
            foreach (var b in _brushes.Values) b.Dispose();
            foreach (var p in _pens.Values) p.Dispose();
            _brushes.Clear();
            _pens.Clear();
        }

        public void Dispose() => Clear();
    }
}
