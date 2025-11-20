using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers
{
    internal class ListItemInfo
    {
        public SimpleItem Item { get; set; }
        public Rectangle RowRect { get; set; }
        public Rectangle CheckRect { get; set; }
        public Rectangle IconRect { get; set; }
        public Rectangle TextRect { get; set; }
    }

    internal class BeepListBoxLayoutHelper
    {
        private readonly BeepListBox _owner;
        private readonly List<ListItemInfo> _layoutCache = new();

        public BeepListBoxLayoutHelper(BeepListBox owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public IReadOnlyList<ListItemInfo> GetCachedLayout() => _layoutCache;

        public void Clear() => _layoutCache.Clear();

        public void CalculateLayout()
        {
            _layoutCache.Clear();
            if (_owner.Width <= 0 || _owner.Height <= 0) return;

            var drawingRect = _owner.GetClientArea();
            if (drawingRect.IsEmpty)
            {
                drawingRect = _owner.DrawingRect;
            }
            var visibleItems = _owner.Helper.GetVisibleItems();
            if (visibleItems == null || visibleItems.Count == 0) return;

            // Use the owner's preferred item height exposed publicly
            int itemHeight = Math.Max(1, _owner.PreferredItemHeight);

            int y = drawingRect.Top - _owner.YOffset;
            int x = drawingRect.Left;
            int w = drawingRect.Width;

            // Metrics
            int paddingX = 8;
            int checkboxSize = Math.Min(18, Math.Max(14, _owner.ImageSize));
            int checkboxArea = _owner.ShowCheckBox ? (checkboxSize + paddingX) : 0;
            int iconSize = _owner.ImageSize;
            int iconArea = iconSize > 0 ? (iconSize + paddingX) : 0;
            itemHeight= (int)(itemHeight +StyleBorders.GetBorderWidth(_owner.ControlStyle)*2);
            foreach (var item in visibleItems)
            {
                var row = new Rectangle(x, y, w, itemHeight);

                // Checkbox area (left)
                Rectangle checkRect = Rectangle.Empty;
                if (_owner.ShowCheckBox)
                {
                    int cy = y + (itemHeight - checkboxSize) / 2;
                    checkRect = new Rectangle(x + paddingX, cy, checkboxSize, checkboxSize);
                }

                // Icon area (after checkbox)
                Rectangle iconRect = Rectangle.Empty;
                int iconLeft = x + paddingX + checkboxArea;
                if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
                {
                    int iy = y + (itemHeight - iconSize) / 2;
                    iconRect = new Rectangle(iconLeft, iy, iconSize, iconSize);
                }

                // Text area (rest of row)
                int textLeft = x + paddingX + checkboxArea + (iconRect.IsEmpty ? 0 : iconArea);
                Rectangle textRect = new Rectangle(textLeft, y, Math.Max(0, x + w - textLeft - paddingX), itemHeight);

                _layoutCache.Add(new ListItemInfo
                {
                    Item = item,
                    RowRect = row,
                    CheckRect = checkRect,
                    IconRect = iconRect,
                    TextRect = textRect
                });

                y += itemHeight;
                if (y >= drawingRect.Bottom) break;
            }

            // Compute virtual size and notify owner: height is absolute content height we computed starting at drawingRect.Top
            int totalHeight = 0;
            if (_layoutCache.Count > 0)
            {
                var last = _layoutCache[_layoutCache.Count - 1];
                totalHeight = (last.RowRect.Bottom - drawingRect.Top);
            }
            else
            {
                totalHeight = 0;
            }

            // update owner's virtual size width and height
            try { _owner.UpdateVirtualSize(new Size(drawingRect.Width, Math.Max(0, totalHeight))); } catch { }
        }
    }
}
