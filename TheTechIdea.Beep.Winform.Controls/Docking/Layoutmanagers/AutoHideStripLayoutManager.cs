using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers
{
    /// <summary>
    /// Immutable description of one auto-hide tab to lay out on an edge strip.
    /// </summary>
    internal sealed class AutoHideTabModel
    {
        public string Title { get; init; } = string.Empty;
        public string IconPath { get; init; } = string.Empty;
        public bool IsActive { get; init; }

        /// <summary>Back-reference so a hit-tested tab maps to the caller's panel.</summary>
        public object Tag { get; init; }
    }

    /// <summary>
    /// Computes auto-hide tab geometry for an edge strip and resolves hit-testing. Per house style
    /// the layout manager <b>detects</b> (geometry + hit-test); the matching
    /// <c>AutoHideStripRenderer</c> only paints the resolved layout. Tab length depends on the
    /// measured title plus icon, clamped to <see cref="TabMinLength"/>.
    /// </summary>
    internal sealed class AutoHideStripLayoutManager
    {
        public int TabSize { get; init; } = 22;
        public int TabMinLength { get; init; } = 60;
        public int TabPadding { get; init; } = 8;
        public int IconSize { get; init; } = 14;
        public int IconGap { get; init; } = 4;
        public int StartOffset { get; init; } = 2;
        public int TabSpacing { get; init; } = 2;

        private readonly List<KeyValuePair<AutoHideTabModel, Rectangle>> _tabRects = new();

        /// <summary>Computed tab rectangles in paint order.</summary>
        public IReadOnlyList<KeyValuePair<AutoHideTabModel, Rectangle>> TabRects => _tabRects;

        /// <summary>True when the strip runs horizontally (Top/Bottom edges).</summary>
        public bool Horizontal { get; private set; }

        /// <summary>
        /// Lays the tabs out along the strip. Geometry is independent of active state; the active
        /// flag rides along on each <see cref="AutoHideTabModel"/> for the renderer.
        /// </summary>
        public void Compute(bool horizontal, IReadOnlyList<AutoHideTabModel> tabs, Font font)
        {
            _tabRects.Clear();
            Horizontal = horizontal;

            if (tabs == null)
                return;

            int pos = StartOffset;
            foreach (var tab in tabs)
            {
                int textLen = TextRenderer.MeasureText(tab.Title ?? string.Empty, font).Width
                              + TabPadding * 2 + IconSize + IconGap;
                int len = Math.Max(textLen, TabMinLength);

                Rectangle r = horizontal
                    ? new Rectangle(pos, 0, len, TabSize)
                    : new Rectangle(0, pos, TabSize, len);

                _tabRects.Add(new KeyValuePair<AutoHideTabModel, Rectangle>(tab, r));
                pos += len + TabSpacing;
            }
        }

        /// <summary>Returns the tab model at a point, or null.</summary>
        public AutoHideTabModel HitTest(Point p)
        {
            foreach (var kv in _tabRects)
            {
                if (kv.Value.Contains(p))
                    return kv.Key;
            }
            return null;
        }
    }
}
