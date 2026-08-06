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

        /// <summary>
        /// Extra width given to the selected cell.
        /// </summary>
        /// <remarks>
        /// Defaults to 1.0 - equal cells. It was 1.3, which widened the selected item by 30% and
        /// reflowed the whole row every time the selection moved. None of the reference designs do
        /// that: the cells are a fixed grid and selection is shown with colour, a pill, or an
        /// indicator bar *inside* the cell. Where a reference appears to widen an item (a pill that
        /// contains both icon and label) the cell is still the same width - the pill is drawn within
        /// it.
        /// </remarks>
        public float SelectedWidthFactor { get; set; } = 1.0f;
        private bool _dirty = true;

        public int MinTouchTargetWidth { get; set; } = 48;
        public int MinTouchTargetHeight { get; set; } = 48;

        // The measured design spec (Example_images/dock8.png, corroborated by dock2.png):
        // 74px cells, 24px icons, 12px labels, centre-aligned as a group.

        /// <summary>Preferred cell width in logical pixels. Cells shrink below this only when the bar is too narrow.</summary>
        public int PreferredCellWidth { get; set; } = 74;

        /// <summary>Icon edge length in logical pixels.</summary>
        public int IconSize { get; set; } = 24;

        /// <summary>Gap between the top of the cell and the icon.</summary>
        public int IconTopPadding { get; set; } = 7;

        /// <summary>Gap between the icon and the label.</summary>
        public int IconLabelGap { get; set; } = 3;

        /// <summary>Device pixels per logical pixel, so the spec above is honoured on scaled displays.</summary>
        public float DpiScale { get; set; } = 1f;

        private int Scaled(int logical) =>
            DpiScale > 0f && Math.Abs(DpiScale - 1f) > 0.001f
                ? (int)Math.Round(logical * DpiScale)
                : logical;

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

            // Fixed-width cells, centred as a group - not stretched to fill. The bar can be any width;
            // the cells stay 74px and sit in the middle, which is what "Center Aligned" means on the
            // spec sheet. Stretching every cell to the bar's width made a wide desktop bar look
            // nothing like the reference, because the icons drifted apart with acres between them.
            int preferredCell = Scaled(PreferredCellWidth);
            int minCell = Math.Max(Scaled(MinTouchTargetWidth), 1);

            float cellUnit = preferredCell;
            float requested = 0f;
            for (int i = 0; i < count; i++) requested += cellUnit * factors[i];

            // Only shrink when the bar genuinely cannot hold the grid.
            if (requested > availableWidth)
            {
                cellUnit = Math.Max(minCell, availableWidth / Math.Max(0.0001f, totalFactor));
            }

            var itemWidths = new int[count];
            int consumed = 0;
            for (int i = 0; i < count; i++)
            {
                int w = Math.Max(minCell, (int)Math.Round(cellUnit * factors[i]));
                itemWidths[i] = w;
                consumed += w;
            }

            IsOverflow = consumed > availableWidth;
            VisibleItemCount = count;

            int left = _bounds.Left + padding + Math.Max(0, (availableWidth - consumed) / 2);
            int top = _bounds.Top + (_bounds.Height - itemHeight) / 2;

            int iconSize = Scaled(IconSize);
            int iconTop = Scaled(IconTopPadding);
            int iconGap = Scaled(IconLabelGap);

            for (int i = 0; i < count; i++)
            {
                var itemRect = new Rectangle(left, top, itemWidths[i], itemHeight);
                _cachedItemRects.Add(itemRect);

                // 24px, flat. This was Math.Min(24, itemHeight / 3), which on the control's own
                // default 72px bar produced an 18px icon and never reached 24 below an 88px bar -
                // so the icons were a quarter too small everywhere by construction.
                int drawIcon = Math.Min(iconSize, Math.Max(8, itemHeight - iconTop * 2));
                int iconX = itemRect.Left + (itemRect.Width - drawIcon) / 2;
                int iconY = itemRect.Top + iconTop;
                _cachedIconRects.Add(new Rectangle(iconX, iconY, drawIcon, drawIcon));

                int labelY = iconY + drawIcon + iconGap;
                int labelHeight = Math.Max(0, itemRect.Bottom - labelY - Scaled(4));
                _cachedLabelRects.Add(new Rectangle(itemRect.Left + Scaled(4), labelY, itemRect.Width - Scaled(8), labelHeight));

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

        /// <summary>
        /// Width the bar needs to show its whole grid: every cell plus the edge padding.
        /// </summary>
        /// <remarks>
        /// Used by the centred placement, where the panel sizes itself to its content instead of
        /// filling the parent. Derived from the laid-out cells rather than recomputed, so the two
        /// cannot disagree - a preferred width calculated separately from the layout is the kind of
        /// second implementation that drifts.
        /// </remarks>
        public int GetPreferredContentWidth(int itemCount, bool hasCta = false)
        {
            if (itemCount <= 0) return 16;

            // From the spec, not from the cells already laid out. Deriving it from the cached rects
            // creates a feedback loop: the panel sizes itself from a layout that was computed against
            // the panel's *previous* size, so the width converges on something a pixel or two off and
            // the cells never reach their 74px. The spec has no such dependency.
            float cell = Math.Max(Scaled(MinTouchTargetWidth), Scaled(PreferredCellWidth));

            // Cells that are wider than one unit: the CTA, and the selected cell in styles that draw
            // a pill around icon and label. Counted once each; when they are the same item this
            // slightly over-estimates, which costs a few pixels of margin and never clips.
            float units = itemCount
                        + (hasCta ? Math.Max(0f, CtaWidthFactor - 1f) : 0f)
                        + Math.Max(0f, SelectedWidthFactor - 1f);

            return (int)Math.Round(cell * units) + 16;
        }

        public IReadOnlyList<Rectangle> GetItemRectangles() => _cachedItemRects.AsReadOnly();
        public IReadOnlyList<Rectangle> GetIconRectangles() => _cachedIconRects.AsReadOnly();
        public IReadOnlyList<Rectangle> GetLabelRectangles() => _cachedLabelRects.AsReadOnly();
        public Rectangle GetIndicatorRect() => _cachedIndicatorRect;
    }
}
