using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Helpers
{
    internal class BeepBottomBarLayoutHelper
    {
        private Rectangle _bounds = Rectangle.Empty;
        private List<Rectangle> _cachedItemRects = new List<Rectangle>();
        private Rectangle _cachedIndicatorRect = Rectangle.Empty;
        public float CtaWidthFactor { get; set; } = 1.6f;
        public float SelectedWidthFactor { get; set; } = 1.3f;
        private bool _dirty = true;

        public void InvalidateLayout() => _dirty = true;

        public void EnsureLayout(Rectangle bounds, List<SimpleItem> items, int ctaIndex, int selectedIndex = -1)
        {
            if (!_dirty && _bounds == bounds) return;
            _bounds = bounds;
            _cachedItemRects.Clear();

            if (items == null || items.Count == 0) return;

            int padding = 8;
            int count = items.Count;
            int width = Math.Max(1, _bounds.Width - padding * 2);
            // compute per-item widths with factors applied to CTA and optionally selected item
            var factors = new float[count];
            float totalFactor = 0f;
            for (int i = 0; i < count; i++)
            {
                float f = 1f;
                if (i == ctaIndex) f *= CtaWidthFactor;
                if (i == selectedIndex) f *= SelectedWidthFactor;
                factors[i] = f;
                totalFactor += f;
            }
            float baseWidth = (float)width / totalFactor;
            var itemWidths = new int[count];
            int consumed = 0;
            for (int i = 0; i < count; i++)
            {
                int w = Math.Max(1, (int)Math.Round(baseWidth * factors[i]));
                itemWidths[i] = w;
                consumed += w;
            }
            // adjust last width to consume any remaining space due to rounding
            int remaining = width - consumed;
            if (remaining != 0 && count > 0)
            {
                itemWidths[count - 1] = itemWidths[count - 1] + remaining;
            }
            int left = _bounds.Left + padding;
            for (int i = 0; i < count; i++)
            {
                var r = new Rectangle(left, _bounds.Top + padding, itemWidths[i], _bounds.Height - padding * 2);
                _cachedItemRects.Add(r);
                left += itemWidths[i];
            }

            // default indicator width is item width - 16
            _cachedIndicatorRect = new Rectangle(_cachedItemRects[0].Left + 8, _cachedItemRects[0].Top + 6, _cachedItemRects[0].Width - 16, _cachedItemRects[0].Height - 12);
            // If CTA index is used, set indicator to that element's rect by default
            if (ctaIndex >= 0 && ctaIndex < _cachedItemRects.Count)
            {
                // CTA may be taller/bigger; center it
                var ctaRect = _cachedItemRects[ctaIndex];
                _cachedIndicatorRect = new Rectangle(ctaRect.Left + 6, ctaRect.Top + 2, ctaRect.Width - 12, ctaRect.Height - 4);
            }
            _dirty = false;
        }

        public Rectangle GetItemRect(int index)
        {
            if (index >= 0 && index < _cachedItemRects.Count) return _cachedItemRects[index];
            return Rectangle.Empty;
        }

        public IReadOnlyList<Rectangle> GetItemRectangles() => _cachedItemRects.AsReadOnly();
        public Rectangle GetIndicatorRect() => _cachedIndicatorRect;
    }
}
