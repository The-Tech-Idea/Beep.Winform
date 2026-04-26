using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus
{
    public partial class BeepAccordionMenu
    {
        private int _focusedIndex = -1;
        private bool _isChildFocused = false;

        // Drag-drop state
        private bool _isDragging;
        private SimpleItem _draggedItem;
        private int _dragStartY;
        private int _dropInsertIndex = -1;
        private bool _dropAfter;
        private const int DragThreshold = 5;

        #region Keyboard Navigation
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Down:
                    NavigateToNextItem();
                    return true;
                case Keys.Up:
                    NavigateToPreviousItem();
                    return true;
                case Keys.Right:
                    if (_focusedIndex >= 0 && _focusedIndex < items.Count)
                    {
                        var headerItem = items[_focusedIndex];
                        if (headerItem.Children.Count > 0 && !expandedState[headerItem])
                        {
                            expandedState[headerItem] = true;
                            HeaderExpandedChanged?.Invoke(this, new BeepMouseEventArgs(headerItem.Text, headerItem));
                            Invalidate();
                        }
                    }
                    return true;
                case Keys.Left:
                    if (_focusedIndex >= 0 && _focusedIndex < items.Count)
                    {
                        var headerItem = items[_focusedIndex];
                        if (headerItem.Children.Count > 0 && expandedState[headerItem])
                        {
                            expandedState[headerItem] = false;
                            HeaderExpandedChanged?.Invoke(this, new BeepMouseEventArgs(headerItem.Text, headerItem));
                            Invalidate();
                        }
                    }
                    return true;
                case Keys.Enter:
                case Keys.Space:
                    if (_focusedIndex >= 0)
                    {
                        SelectFocusedItem();
                        return true;
                    }
                    break;
                case Keys.Home:
                    NavigateToFirstItem();
                    return true;
                case Keys.End:
                    NavigateToLastItem();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void NavigateToNextItem()
        {
            int maxAttempts = items.Count + GetTotalChildCount();
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                attempts++;
                if (_isChildFocused)
                {
                    var headerItem = GetCurrentHeaderItem();
                    if (headerItem != null)
                    {
                        var childIndex = GetChildIndexInHeader(headerItem, _focusedItem) + 1;
                        if (childIndex < headerItem.Children.Count)
                        {
                            _focusedItem = (SimpleItem)headerItem.Children[childIndex];
                            _focusedIndex = items.IndexOf(headerItem);
                            ScrollToItem(_focusedItem);
                            Invalidate();
                            return;
                        }
                        else
                        {
                            _isChildFocused = false;
                            _focusedIndex = items.IndexOf(headerItem) + 1;
                        }
                    }
                }
                else
                {
                    _focusedIndex++;
                    if (_focusedIndex >= items.Count)
                    {
                        _focusedIndex = 0;
                    }
                    var headerItem = items[_focusedIndex];
                    if (headerItem.Children.Count > 0 && expandedState[headerItem])
                    {
                        _isChildFocused = true;
                        _focusedItem = (SimpleItem)headerItem.Children[0];
                    }
                    else
                    {
                        _focusedItem = headerItem;
                        _isChildFocused = false;
                    }
                    ScrollToItem(_focusedItem);
                    Invalidate();
                    return;
                }
            }
        }

        private void NavigateToPreviousItem()
        {
            if (items.Count == 0) return;

            if (_focusedIndex < 0)
            {
                _focusedIndex = items.Count - 1;
                _focusedItem = items[_focusedIndex];
                _isChildFocused = false;
                Invalidate();
                return;
            }

            if (_isChildFocused)
            {
                var headerItem = GetCurrentHeaderItem();
                if (headerItem != null)
                {
                    var childIndex = GetChildIndexInHeader(headerItem, _focusedItem) - 1;
                    if (childIndex >= 0)
                    {
                        _focusedItem = (SimpleItem)headerItem.Children[childIndex];
                        ScrollToItem(_focusedItem);
                        Invalidate();
                        return;
                    }
                    else
                    {
                        _isChildFocused = false;
                        _focusedItem = headerItem;
                    }
                }
            }
            else
            {
                _focusedIndex--;
                if (_focusedIndex < 0)
                {
                    _focusedIndex = items.Count - 1;
                }
                var headerItem = items[_focusedIndex];
                if (headerItem.Children.Count > 0 && expandedState[headerItem])
                {
                    _isChildFocused = true;
                    _focusedItem = (SimpleItem)headerItem.Children[headerItem.Children.Count - 1];
                }
                else
                {
                    _focusedItem = headerItem;
                    _isChildFocused = false;
                }
                ScrollToItem(_focusedItem);
            }
            Invalidate();
        }

        private void NavigateToFirstItem()
        {
            if (items.Count > 0)
            {
                _focusedIndex = 0;
                _focusedItem = items[0];
                _isChildFocused = false;
                ScrollToItem(_focusedItem);
                Invalidate();
            }
        }

        private void NavigateToLastItem()
        {
            if (items.Count > 0)
            {
                _focusedIndex = items.Count - 1;
                _focusedItem = items[_focusedIndex];
                if (_focusedItem != null && _focusedItem.Children.Count > 0 && expandedState[_focusedItem])
                {
                    _isChildFocused = true;
                    _focusedItem = (SimpleItem)_focusedItem.Children[_focusedItem.Children.Count - 1];
                }
                else
                {
                    _isChildFocused = false;
                }
                ScrollToItem(_focusedItem);
                Invalidate();
            }
        }

        private void SelectFocusedItem()
        {
            if (_focusedItem != null)
            {
                SelectedItem = _focusedItem;
                ItemClick?.Invoke(this, new BeepMouseEventArgs(_focusedItem.Text, _focusedItem));
                Invalidate();
            }
        }

        private SimpleItem GetCurrentHeaderItem()
        {
            if (_focusedIndex >= 0 && _focusedIndex < items.Count)
                return items[_focusedIndex];
            return null;
        }

        private int GetTotalChildCount()
        {
            int count = 0;
            foreach (var item in items)
            {
                if (expandedState.ContainsKey(item) && expandedState[item])
                    count += item.Children.Count;
            }
            return count;
        }

        private int GetChildIndexInHeader(SimpleItem headerItem, SimpleItem childItem)
        {
            if (headerItem == null || childItem == null) return -1;
            return headerItem.Children.IndexOf(childItem);
        }

        private void ScrollToItem(SimpleItem item)
        {
            if (!_showScrollBar || item == null) return;

            int itemBottom = item.Y + item.Height - _scrollOffset;
            int itemTop = item.Y - _scrollOffset;

            if (itemBottom > Height - headerHeight)
            {
                ScrollTo(_scrollOffset + (itemBottom - (Height - headerHeight)));
            }
            else if (itemTop < 0)
            {
                ScrollTo(_scrollOffset + itemTop);
            }
        }
        #endregion

        #region Drag-Drop Reordering
        public event EventHandler<SimpleItemDragEventArgs> ItemReordered;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!AllowItemDragDrop || e.Button != MouseButtons.Left) return;

            _dragStartY = e.Y;
            _draggedItem = GetItemAtPoint(new Point(e.X, e.Y));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_isDragging && _draggedItem != null && Math.Abs(e.Y - _dragStartY) > DragThreshold)
            {
                _isDragging = true;
                DoDragDrop(_draggedItem, DragDropEffects.Move);
            }

            HitTestWithMouse();
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
            _draggedItem = null;
            _dropInsertIndex = -1;
            Invalidate();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            if (e.Data.GetDataPresent(typeof(SimpleItem)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            if (!AllowItemDragDrop || e.Data.GetDataPresent(typeof(SimpleItem)) == false)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;

            Point clientPoint = PointToClient(new Point(e.X, e.Y));
            SimpleItem targetItem = GetItemAtPoint(clientPoint);

            if (targetItem != null && targetItem != _draggedItem)
            {
                int targetIndex = items.IndexOf(targetItem);
                if (targetIndex >= 0)
                {
                    int draggedIndex = items.IndexOf(_draggedItem);
                    _dropInsertIndex = targetIndex;
                    _dropAfter = clientPoint.Y > targetItem.Y + targetItem.Height / 2;

                    if (_dropAfter && _dropInsertIndex < items.Count - 1)
                    {
                        _dropInsertIndex++;
                    }

                    Invalidate();
                }
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            if (!AllowItemDragDrop || _draggedItem == null || _dropInsertIndex < 0) return;

            int oldIndex = items.IndexOf(_draggedItem);
            int newIndex = _dropInsertIndex;

            if (oldIndex < 0 || oldIndex == newIndex || oldIndex == newIndex - 1) return;

            items.Remove(_draggedItem);

            if (newIndex > oldIndex)
                newIndex--;

            newIndex = Math.Min(newIndex, items.Count);
            items.Insert(newIndex, _draggedItem);

            ItemReordered?.Invoke(this, new SimpleItemDragEventArgs(_draggedItem, null, oldIndex, newIndex));

            _isDragging = false;
            _draggedItem = null;
            _dropInsertIndex = -1;
            Invalidate();
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            _dropInsertIndex = -1;
            Invalidate();
        }
        #endregion

        private SimpleItem GetItemAtPoint(Point point)
        {
            foreach (var item in items)
            {
                var rect = new Rectangle(item.X, item.Y, item.Width, item.Height);
                if (rect.Contains(point))
                {
                    return item;
                }

                if (expandedState.ContainsKey(item) && expandedState[item])
                {
                    foreach (SimpleItem child in item.Children)
                    {
                        var childRect = new Rectangle(child.X, child.Y, child.Width, child.Height);
                        if (childRect.Contains(point))
                        {
                            return child;
                        }
                    }
                }
            }
            return null;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                if (!isInitialized)
                {
                    InitializeMenu();
                    InitializeMenuItemState();
                    Invalidate();
                    isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepAccordionMenu: Error in OnHandleCreated: {ex.Message}");
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (isInitialized)
            {
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (HitTestWithMouse())
            {
                if (HitTestControl != null && HitTestControl.Name == "ToggleButton")
                {
                    isCollapsed = !isCollapsed;
                    StartAccordionAnimation();
                    ToggleClicked?.Invoke(this, new BeepMouseEventArgs("ToggleClicked", isCollapsed));
                }
                else if (HitTestControl != null && HitTestControl.Name.StartsWith("HeaderItem_"))
                {
                    HitTestControl.HitAction?.Invoke();
                }
                else if (HitTestControl != null && HitTestControl.Name.StartsWith("ChildItem_"))
                {
                    HitTestControl.HitAction?.Invoke();
                }

                Invalidate();
            }
        }
    }
}
