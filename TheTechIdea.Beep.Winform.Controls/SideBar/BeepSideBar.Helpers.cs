using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    public partial class BeepSideBar
    {
        #region Helper Methods
        /// <summary>
        /// Refreshes hit test areas for all visible menu items
        /// </summary>
        private void RefreshHitAreas()
        {
            // This will be used by painters for hit testing
            // The actual hit areas are calculated during painting
            InitializeExpandedState();
        }

        /// <summary>
        /// Gets the menu item at the specified point
        /// </summary>
        private SimpleItem GetItemAtPoint(Point point)
        {
            if (_items == null || _items.Count == 0)
                return null;

            int padding = 8;
            int currentY = DrawingRect.Top + padding;

            // Check toggle button area first
            if (_showToggleButton)
            {
                Rectangle toggleRect = new Rectangle(
                    DrawingRect.Left + padding,
                    currentY,
                    DrawingRect.Width - padding * 2,
                    _itemHeight
                );

                currentY += _itemHeight + 4;

                if (toggleRect.Contains(point))
                    return null; // Toggle button, not an item
            }

            // Check main items and their children
            foreach (var item in _items)
            {
                Rectangle itemRect = new Rectangle(
                    DrawingRect.Left + padding,
                    currentY,
                    DrawingRect.Width - padding * 2,
                    _itemHeight
                );

                if (itemRect.Contains(point))
                    return item;

                currentY += _itemHeight + 4;

                // Check children if expanded
                if (IsItemExpanded(item))
                {
                    var childItem = GetChildItemAtPoint(point, item, ref currentY, 1);
                    if (childItem != null)
                        return childItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Recursively checks child items at a point
        /// </summary>
        private SimpleItem GetChildItemAtPoint(Point point, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0)
                return null;

            int padding = 4;
            int indent = _indentationWidth * indentLevel;

            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(
                    DrawingRect.Left + indent + padding,
                    currentY,
                    DrawingRect.Width - indent - padding * 2,
                    _childItemHeight
                );

                if (childRect.Contains(point))
                    return child;

                currentY += _childItemHeight + 2;

                // Check nested children if expanded
                if (IsItemExpanded(child))
                {
                    var nestedChild = GetChildItemAtPoint(point, child, ref currentY, indentLevel + 1);
                    if (nestedChild != null)
                        return nestedChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the item that has an expand/collapse icon at the specified point
        /// </summary>
        private SimpleItem GetItemWithExpandIconAtPoint(Point point)
        {
            if (_items == null || _items.Count == 0 || _isCollapsed)
                return null;

            int padding = 8;
            int currentY = DrawingRect.Top + padding;

            // Skip toggle button area
            if (_showToggleButton)
            {
                currentY += _itemHeight + 4;
            }

            // Check main items
            foreach (var item in _items)
            {
                if (item.Children != null && item.Children.Count > 0)
                {
                    Rectangle expandIconRect = new Rectangle(
                        DrawingRect.Right - 24 - padding,
                        currentY + (_itemHeight - 16) / 2,
                        16,
                        16
                    );

                    if (expandIconRect.Contains(point))
                        return item;
                }

                currentY += _itemHeight + 4;

                // Check children if expanded
                if (IsItemExpanded(item))
                {
                    var childItem = GetChildItemWithExpandIconAtPoint(point, item, ref currentY, 1);
                    if (childItem != null)
                        return childItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Recursively checks child items for expand/collapse icon at point
        /// </summary>
        private SimpleItem GetChildItemWithExpandIconAtPoint(Point point, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0)
                return null;

            int padding = 4;
            int indent = _indentationWidth * indentLevel;

            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandIconRect = new Rectangle(
                        DrawingRect.Right - 20 - padding,
                        currentY + (_childItemHeight - 14) / 2,
                        14,
                        14
                    );

                    if (expandIconRect.Contains(point))
                        return (SimpleItem)child;
                }

                currentY += _childItemHeight + 2;

                // Check nested children if expanded
                if (IsItemExpanded((SimpleItem)child))
                {
                    var nestedChild = GetChildItemWithExpandIconAtPoint(point, (SimpleItem)child, ref currentY, indentLevel + 1);
                    if (nestedChild != null)
                        return nestedChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a point is within the toggle button area
        /// </summary>
        private bool IsPointInToggleButton(Point point)
        {
            if (!_showToggleButton)
                return false;

            int padding = 8;
            Rectangle toggleRect = new Rectangle(
                DrawingRect.Left + padding,
                DrawingRect.Top + padding,
                DrawingRect.Width - padding * 2,
                _itemHeight
            );

            return toggleRect.Contains(point);
        }

        /// <summary>
        /// Gets a flat list of all visible menu items (respecting expanded state)
        /// </summary>
        private List<SimpleItem> GetFlatItemList()
        {
            var flatList = new List<SimpleItem>();

            if (_items == null || _items.Count == 0)
                return flatList;

            foreach (var item in _items)
            {
                flatList.Add(item);

                if (IsItemExpanded(item))
                {
                    AddChildrenToFlatList(item, flatList);
                }
            }

            return flatList;
        }

        /// <summary>
        /// Recursively adds child items to flat list
        /// </summary>
        private void AddChildrenToFlatList(SimpleItem parentItem, List<SimpleItem> flatList)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0)
                return;

            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                flatList.Add(child);

                if (IsItemExpanded(child))
                {
                    AddChildrenToFlatList(child, flatList);
                }
            }
        }
        #endregion
    }
}
