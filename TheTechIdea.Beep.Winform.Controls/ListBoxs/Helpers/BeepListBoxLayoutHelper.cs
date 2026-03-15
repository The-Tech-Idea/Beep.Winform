using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers
{
    internal class ListItemInfo
    {
        public SimpleItem Item { get; set; }
        public Rectangle RowRect { get; set; }
        public Rectangle CheckRect { get; set; }
        public Rectangle IconRect { get; set; }
        public Rectangle TextRect { get; set; }
        public int Depth { get; set; }
        public bool HasChildren { get; set; }
        public Rectangle ChevronRect { get; set; }
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

        public void CalculateLayout(Control ownerControl = null)
        {
            var ctrl = ownerControl ?? (Control)_owner;
            _layoutCache.Clear();
            if (_owner.Width <= 0 || _owner.Height <= 0) return;

            var drawingRect = _owner.GetClientArea();
            if (drawingRect.IsEmpty)
            {
                drawingRect = _owner.DrawingRect;
            }
            var visibleItems = _owner.Helper.GetVisibleItems();
            if (visibleItems == null || visibleItems.Count == 0) return;
            
            int viewportTop = drawingRect.Top;
            int viewportBottom = drawingRect.Bottom;
            int runningVirtualY = 0;
            int x = drawingRect.Left;
            int w = drawingRect.Width;

            // Metrics (DPI-scaled, token-driven)
            int paddingX = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, ctrl);
            int paddingY = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingV, ctrl);
            int checkboxSize = DpiScalingHelper.ScaleValue(ListBoxTokens.CheckboxSize, ctrl);
            int checkboxArea = _owner.ShowCheckBox ? (checkboxSize + DpiScalingHelper.ScaleValue(ListBoxTokens.IconTextGap, ctrl)) : 0;
            int iconSize = DpiScalingHelper.ScaleValue(_owner.ImageSize, ctrl);
            int iconTextGap = DpiScalingHelper.ScaleValue(ListBoxTokens.IconTextGap, ctrl);
            int iconArea = iconSize > 0 ? (iconSize + iconTextGap) : 0;
            int borderWidth = (int)StyleBorders.GetBorderWidth(_owner.ControlStyle) * 2;

            foreach (var item in visibleItems)
            {
                int itemHeight = Math.Max(1, _owner.GetItemHeightForLayout(item) + borderWidth);
                int screenY = drawingRect.Top + runningVirtualY - _owner.YOffset;
                var row = new Rectangle(x, screenY, w, itemHeight);

                if (row.Bottom < viewportTop)
                {
                    runningVirtualY += itemHeight;
                    continue;
                }

                if (row.Top > viewportBottom)
                {
                    // We can stop populating cache once rows are below viewport, but keep tracking total height.
                    runningVirtualY += itemHeight;
                    continue;
                }

                // Hierarchy indentation
                int depth = _owner.ShowHierarchy ? _owner.Helper.GetItemDepth(item) : 0;
                bool hasChildren = _owner.ShowHierarchy ? _owner.Helper.ItemHasChildren(item) : false;
                int indentPx = depth * DpiScalingHelper.ScaleValue(ListBoxTokens.IndentStepPerLevel, ctrl);
                int chevronArea = hasChildren ? DpiScalingHelper.ScaleValue(ListBoxTokens.ChevronHitTarget, ctrl) : 0;
                int contentStartX = x + paddingX + indentPx + chevronArea;

                // Chevron rect (for hit testing and drawing)
                Rectangle chevronRect = Rectangle.Empty;
                if (hasChildren)
                {
                    int chevronSize = DpiScalingHelper.ScaleValue(ListBoxTokens.ChevronSize, ctrl);
                    int chevronX = x + paddingX + indentPx;
                    int chevronY = screenY + (itemHeight - chevronSize) / 2;
                    chevronRect = new Rectangle(chevronX, chevronY, chevronSize, chevronSize);
                }

                // Checkbox area (left, after indent + chevron)
                Rectangle checkRect = Rectangle.Empty;
                if (_owner.ShowCheckBox)
                {
                    int cy = screenY + (itemHeight - checkboxSize) / 2;
                    checkRect = new Rectangle(contentStartX, cy, checkboxSize, checkboxSize);
                }

                // Icon area (after checkbox)
                Rectangle iconRect = Rectangle.Empty;
                int iconLeft = contentStartX + checkboxArea;
                if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
                {
                    int iy = screenY + (itemHeight - iconSize) / 2;
                    iconRect = new Rectangle(iconLeft, iy, iconSize, iconSize);
                }

                // Text area (rest of row, with vertical padding)
                int textLeft = contentStartX + checkboxArea + (iconRect.IsEmpty ? 0 : iconArea);
                Rectangle textRect = new Rectangle(
                    textLeft,
                    screenY + paddingY,
                    Math.Max(0, x + w - textLeft - paddingX),
                    Math.Max(1, itemHeight - paddingY * 2)
                );

                _layoutCache.Add(new ListItemInfo
                {
                    Item = item,
                    RowRect = row,
                    CheckRect = checkRect,
                    IconRect = iconRect,
                    TextRect = textRect,
                    Depth = depth,
                    HasChildren = hasChildren,
                    ChevronRect = chevronRect
                });

                runningVirtualY += itemHeight;
            }

            // Keep full content height for scrollbar calculations even when only visible rows are cached.
            int totalHeight = Math.Max(0, runningVirtualY);

            // update owner's virtual size width and height
            try { _owner.UpdateVirtualSize(new Size(drawingRect.Width, Math.Max(0, totalHeight))); } catch { }
        }
    }
}
