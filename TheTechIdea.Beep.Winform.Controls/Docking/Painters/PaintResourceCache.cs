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
