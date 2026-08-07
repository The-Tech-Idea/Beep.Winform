using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Helpers
{
    internal class BottomBarHitTestHelper : IDisposable
    {
        private readonly TheTechIdea.Beep.Winform.Controls.Base.BaseControl _owner;
        private readonly ControlHitTestHelper _hitTestHelper;
        private List<SimpleItem> _items = new List<SimpleItem>();
        private List<Rectangle> _itemRectangles = new List<Rectangle>();
        private int _hoveredIndex = -1;
        private int _focusedIndex = -1;
        private bool _popupOpen = false;
        private int _popupParentIndex = -1;

        public BottomBarHitTestHelper(object owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (owner is not TheTechIdea.Beep.Winform.Controls.Base.BaseControl typedOwner)
                throw new ArgumentException("owner must be a BaseControl", nameof(owner));
            _owner = typedOwner;
            _hitTestHelper = _owner._hitTest;
            _hitTestHelper.HitDetected += OnHitDetected;
        }

        public int HoveredIndex => _hoveredIndex;
        public int FocusedIndex { get => _focusedIndex; set => _focusedIndex = value; }
        public ControlHitTestHelper ControlHitTest => _hitTestHelper;
        public bool PopupOpen => _popupOpen;
        public int PopupParentIndex => _popupParentIndex;

        public event EventHandler IndexChanged;
        public event EventHandler<ItemClickEventArgs> ItemClicked;
        public event EventHandler<PopupEventArgs> PopupRequested;
        public event EventHandler<PopupEventArgs> PopupClosed;

        /// <summary>
        /// Takes the item cells and rebuilds the hit areas from them.
        /// </summary>
        /// <remarks>
        /// The icon and label rectangles used to be passed in and stored as well. Their only reader
        /// was FindHitRegion, which nothing called - so two lists were copied on every layout pass to
        /// be cleared and copied again. Hit testing works on whole cells, which is what the touch
        /// target should be anyway.
        /// </remarks>
        public void UpdateItems(List<SimpleItem> items, List<Rectangle> rectangles)
        {
            _items = items ?? new List<SimpleItem>();
            _itemRectangles = rectangles ?? new List<Rectangle>();
            if (_focusedIndex >= _items.Count) _focusedIndex = -1;
            if (_hoveredIndex >= _items.Count) _hoveredIndex = -1;
            _hitTestHelper.ClearHitList();
            for (int i = 0; i < Math.Min(_items.Count, _itemRectangles.Count); i++)
            {
                var item = _items[i];
                var rect = _itemRectangles[i];
                int idx = i;
                _hitTestHelper.AddHitArea($"BottomBarItem_{i}", rect, null, () => HandleItemClick(idx, MouseButtons.Left));
            }
        }

        /// <summary>
        /// Replaces one item's clickable region, keeping the one click handler.
        /// </summary>
        /// <remarks>
        /// Painters whose style makes a cell bigger than its grid rectangle - the CTA circle, the
        /// selected pill or bubble - used to call AddHitArea themselves with the SAME key and a lambda
        /// straight to OnItemClicked. AddHitArea replaces by name, so that destroyed this helper's
        /// registration rather than extending it, and the cell stopped going through HandleItemClick:
        /// an item with Children in that slot raised no PopupRequested and never opened its submenu,
        /// and FocusedIndex was left pointing at the previously focused cell.
        ///
        /// Painters hand over a rectangle now; the handler stays here, so every cell on the bar
        /// behaves the same way whichever style painted it.
        /// </remarks>
        public void SetItemHitArea(int index, Rectangle rect)
        {
            if (index < 0 || index >= _items.Count) return;

            int idx = index;
            _hitTestHelper.AddHitArea($"BottomBarItem_{index}", rect, null, () => HandleItemClick(idx, MouseButtons.Left));
        }

        private void HandleItemClick(int index, MouseButtons button)
        {
            if (index < 0 || index >= _items.Count) return;

            var item = _items[index];

            if (item != null && item.Children != null && item.Children.Count > 0 && !_popupOpen)
            {
                _popupOpen = true;
                _popupParentIndex = index;
                _focusedIndex = index;
                PopupRequested?.Invoke(this, new PopupEventArgs(index, item, _itemRectangles[index]));
                InvalidateItem(index);
                return;
            }

            _focusedIndex = index;
            ItemClicked?.Invoke(this, new ItemClickEventArgs(index, item, button));
            InvalidateItem(index);
        }

        public void ClosePopup()
        {
            if (_popupOpen)
            {
                var item = _popupParentIndex >= 0 && _popupParentIndex < _items.Count ? _items[_popupParentIndex] : null;
                PopupClosed?.Invoke(this, new PopupEventArgs(_popupParentIndex, item, Rectangle.Empty));
                _popupOpen = false;
                _popupParentIndex = -1;
            }
        }

        private void OnHitDetected(object sender, EventArgs e)
        {
        }

        public void Clear()
        {
            _items.Clear();
            _itemRectangles.Clear();
            _hitTestHelper.ClearHitList();
            _hoveredIndex = -1;
            _focusedIndex = -1;
            _popupOpen = false;
            _popupParentIndex = -1;
        }

        public void Dispose()
        {
            _hitTestHelper.HitDetected -= OnHitDetected;
            Clear();
            IndexChanged = null;
            ItemClicked = null;
            PopupRequested = null;
            PopupClosed = null;
        }

        public void HandleMouseMove(Point location)
        {
            int newHoverIndex = FindItemAt(location);
            if (newHoverIndex != _hoveredIndex)
            {
                int oldHoverIndex = _hoveredIndex;
                _hoveredIndex = newHoverIndex;
                IndexChanged?.Invoke(this, EventArgs.Empty);
                InvalidateItem(oldHoverIndex);
                InvalidateItem(newHoverIndex);
            }
        }

        public void HandleMouseLeave()
        {
            int oldHoverIndex = _hoveredIndex;
            _hoveredIndex = -1;
            InvalidateItem(oldHoverIndex);
        }

        public void HandleMouseClick(Point location, MouseButtons button) => _hitTestHelper.HandleClick(location);
        public void HandleMouseDown(Point location, MouseEventArgs e) => _hitTestHelper.HandleMouseDown(location, e);
        public void HandleMouseUp(Point location, MouseEventArgs e) => _hitTestHelper.HandleMouseUp(location, e);

        public int FindItemAt(Point location)
        {
            for (int i = 0; i < _itemRectangles.Count; i++)
            {
                if (_itemRectangles[i].Contains(location)) return i;
            }
            return -1;
        }

        private void InvalidateItem(int index)
        {
            if (index < 0 || index >= _itemRectangles.Count)
            {
                return;
            }

            var rect = _itemRectangles[index];
            rect.Inflate(6, 6);
            _owner.Invalidate(rect);
        }
    }

    public class ItemClickEventArgs : EventArgs
    {
        public int Index { get; }
        public SimpleItem Item { get; }
        public MouseButtons Button { get; }

        public ItemClickEventArgs(int index, SimpleItem item, MouseButtons button)
        {
            Index = index; Item = item; Button = button;
        }
    }

    public class PopupEventArgs : EventArgs
    {
        public int ParentIndex { get; }
        public SimpleItem ParentItem { get; }
        public Rectangle AnchorRect { get; }

        public PopupEventArgs(int parentIndex, SimpleItem parentItem, Rectangle anchorRect)
        {
            ParentIndex = parentIndex;
            ParentItem = parentItem;
            AnchorRect = anchorRect;
        }
    }
}
