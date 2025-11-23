using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Helpers
{
    internal class BottomBarHitTestHelper
    {
        private readonly TheTechIdea.Beep.Winform.Controls.Base.BaseControl _owner;
        private readonly ControlHitTestHelper _hitTestHelper;
        private List<SimpleItem> _items = new List<SimpleItem>();
        private List<Rectangle> _itemRectangles = new List<Rectangle>();
        private int _hoveredIndex = -1;
        private int _focusedIndex = -1;

        public BottomBarHitTestHelper(object owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (owner is not TheTechIdea.Beep.Winform.Controls.Base.BaseControl typedOwner)
                throw new ArgumentException("owner must be a BaseControl", nameof(owner));
            _owner = typedOwner;
            // Use owner's control hit test helper so registration is centralized
            _hitTestHelper = _owner._hitTest;
            _hitTestHelper.HitDetected += OnHitDetected;
        }

        public int HoveredIndex => _hoveredIndex;
        public int FocusedIndex { get => _focusedIndex; set => _focusedIndex = value; }

        public event EventHandler IndexChanged;
        public event EventHandler<ItemClickEventArgs> ItemClicked;

        public void UpdateItems(List<SimpleItem> items, List<Rectangle> rectangles)
        {
            _items = items ?? new List<SimpleItem>();
            _itemRectangles = rectangles ?? new List<Rectangle>();
            _hitTestHelper.ClearHitList();
            for (int i = 0; i < Math.Min(_items.Count, _itemRectangles.Count); i++)
            {
                var item = _items[i];
                var rect = _itemRectangles[i];
                _hitTestHelper.AddHitArea($"BottomBarItem_{i}", rect, null, () => HandleItemClick(i, MouseButtons.Left));
            }
        }

        private void HandleItemClick(int index, MouseButtons button)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                _focusedIndex = index;
                ItemClicked?.Invoke(this, new ItemClickEventArgs(index, item, button));
                _owner.Invalidate();
            }
        }

        private void OnHitDetected(object sender, EventArgs e)
        {
            // Additional handling if needed
        }

        public void Clear()
        {
            _items.Clear();
            _itemRectangles.Clear();
            _hitTestHelper.ClearHitList();
            _hoveredIndex = -1;
            _focusedIndex = -1;
        }

        public void HandleMouseMove(Point location)
        {
            int newHoverIndex = FindItemAt(location);
            if (newHoverIndex != _hoveredIndex)
            {
                _hoveredIndex = newHoverIndex;
                IndexChanged?.Invoke(this, EventArgs.Empty);
                _owner.Invalidate();
            }
        }

        public void HandleMouseLeave()
        {
            _hoveredIndex = -1;
            _owner.Invalidate();
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
}
