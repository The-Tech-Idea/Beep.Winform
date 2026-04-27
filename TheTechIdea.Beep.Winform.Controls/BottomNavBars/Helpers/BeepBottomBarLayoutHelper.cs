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
        private List<Rectangle> _cachedIconRects = new List<Rectangle>();
        private List<Rectangle> _cachedLabelRects = new List<Rectangle>();
        private Rectangle _cachedIndicatorRect = Rectangle.Empty;
        public float CtaWidthFactor { get; set; } = 1.6f;
        public float SelectedWidthFactor { get; set; } = 1.3f;
        private bool _dirty = true;

        public int MinTouchTargetWidth { get; set; } = 48;
        public int MinTouchTargetHeight { get; set; } = 48;

        public bool IsOverflow { get; private set; }
        public int VisibleItemCount { get; private set; }

        public void InvalidateLayout() => _dirty = true;

        public void EnsureLayout(Rectangle bounds, List<SimpleItem> items, int ctaIndex, int selectedIndex = -1)
        {
            if (!_dirty && _bounds == bounds) return;
            _bounds = bounds;
            _cachedItemRects.Clear();
            _cachedIconRects.Clear();
            _cachedLabelRects.Clear();

            if (items == null || items.Count == 0)
            {
                IsOverflow = false;
                VisibleItemCount = 0;
                return;
            }

            int padding = 8;
            int count = items.Count;
            int availableWidth = Math.Max(1, _bounds.Width - padding * 2);
            int itemHeight = Math.Max(MinTouchTargetHeight, _bounds.Height - padding * 2);

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

            float baseWidth = (float)availableWidth / totalFactor;
            var itemWidths = new int[count];
            int consumed = 0;
            for (int i = 0; i < count; i++)
            {
                int w = Math.Max(MinTouchTargetWidth, (int)Math.Round(baseWidth * factors[i]));
                itemWidths[i] = w;
                consumed += w;
            }

            int remaining = availableWidth - consumed;
            if (remaining != 0 && count > 0)
            {
                itemWidths[count - 1] = itemWidths[count - 1] + remaining;
            }

            IsOverflow = false;
            VisibleItemCount = count;

            int left = _bounds.Left + padding;
            int top = _bounds.Top + (_bounds.Height - itemHeight) / 2;

            for (int i = 0; i < count; i++)
            {
                var itemRect = new Rectangle(left, top, itemWidths[i], itemHeight);
                _cachedItemRects.Add(itemRect);

                int iconSize = Math.Min(24, itemHeight / 3);
                int iconX = itemRect.Left + (itemRect.Width - iconSize) / 2;
                int iconY = itemRect.Top + 6;
                _cachedIconRects.Add(new Rectangle(iconX, iconY, iconSize, iconSize));

                int labelY = iconY + iconSize + 2;
                int labelHeight = itemRect.Bottom - labelY - 4;
                _cachedLabelRects.Add(new Rectangle(itemRect.Left + 4, labelY, itemRect.Width - 8, labelHeight));

                left += itemWidths[i];
            }

            if (ctaIndex >= 0 && ctaIndex < _cachedItemRects.Count)
            {
                var ctaRect = _cachedItemRects[ctaIndex];
                int totalInnerWidth = _cachedItemRects.Count > 0 ? (_cachedItemRects[_cachedItemRects.Count - 1].Right - _cachedItemRects[0].Left) : 0;
                int desiredCenter = _bounds.Left + availableWidth / 2 + padding;
                int currentCenter = ctaRect.Left + ctaRect.Width / 2;
                int delta = desiredCenter - currentCenter;
                int minShift = _bounds.Left + padding - _cachedItemRects[0].Left;
                int maxShift = (_bounds.Right - padding) - _cachedItemRects[_cachedItemRects.Count - 1].Right;
                int allowedDelta = Math.Min(maxShift, Math.Max(minShift, delta));
                if (allowedDelta != 0)
                {
                    for (int i = 0; i < _cachedItemRects.Count; i++)
                    {
                        var rr = _cachedItemRects[i];
                        rr.Offset(allowedDelta, 0);
                        _cachedItemRects[i] = rr;

                        var ir = _cachedIconRects[i];
                        ir.Offset(allowedDelta, 0);
                        _cachedIconRects[i] = ir;

                        var lr = _cachedLabelRects[i];
                        lr.Offset(allowedDelta, 0);
                        _cachedLabelRects[i] = lr;
                    }
                }
            }

            _cachedIndicatorRect = new Rectangle(_cachedItemRects[0].Left + 8, _cachedItemRects[0].Top + 6, Math.Max(16, _cachedItemRects[0].Width - 16), _cachedItemRects[0].Height - 12);
            if (ctaIndex >= 0 && ctaIndex < _cachedItemRects.Count)
            {
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

        public Rectangle GetIconRect(int index)
        {
            if (index >= 0 && index < _cachedIconRects.Count) return _cachedIconRects[index];
            return Rectangle.Empty;
        }

        public Rectangle GetLabelRect(int index)
        {
            if (index >= 0 && index < _cachedLabelRects.Count) return _cachedLabelRects[index];
            return Rectangle.Empty;
        }

        public IReadOnlyList<Rectangle> GetItemRectangles() => _cachedItemRects.AsReadOnly();
        public IReadOnlyList<Rectangle> GetIconRectangles() => _cachedIconRects.AsReadOnly();
        public IReadOnlyList<Rectangle> GetLabelRectangles() => _cachedLabelRects.AsReadOnly();
        public Rectangle GetIndicatorRect() => _cachedIndicatorRect;
    }
}
