using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepTree - Events partial class.
    /// Handles all mouse events, clicks, user interactions, hover effects, and context menus.
    /// </summary>
    public partial class BeepTree
    {
        // Stores the last right-clicked row's viewport Y (client coordinates), if available
        private int? _lastContextMenuViewportY;
        #region Mouse Event Handlers

        private void OnMouseDownHandler(object sender, MouseEventArgs e)
        {
            NodeMouseDown?.Invoke(this, new BeepMouseEventArgs("MouseDown", null));
            // Ensure DrawingRect is current for viewport transforms
            UpdateDrawingRect();
            Point point = e.Location;

            // Right-click context menu
            string htName; SimpleItem htItem; Rectangle htRect;
            bool hasHit = false;
            // Prefer BaseControl-based hit testing via helper
            if (_treeHitTestHelper != null && _treeHitTestHelper.HitTest(point, out htName, out htItem, out htRect))
                hasHit = true;
            else if (LocalHitTest(point, out htName, out htItem, out htRect))
                hasHit = true;

            if (e.Button == MouseButtons.Right && hasHit)
            {
                // Determine hit type and guid (format: "type_guid")
                var partsRC = htName.Split('_');
                if (partsRC.Length != 2)
                    return;
                var hitType = partsRC[0];
                var guid = partsRC[1];

                // Only trigger context menu for row, icon, or text areas
                if (!(hitType == "row" || hitType == "icon" || hitType == "text"))
                    return;

                var item = _treeHelper?.FindByGuid(guid);
                if (item != null)
                {
                    ClickedNode = item;
                    // Capture viewport Y from hit rectangle for precise popup placement
                    _lastContextMenuViewportY = htRect.Top;
                    var args = new BeepMouseEventArgs("RightClick", item);
                    NodeRightClicked?.Invoke(this, args);

                    var provider = SimpleItemFactory.GlobalMenuItemsProvider;
                    var menuItems = provider != null ? provider(item) : null;
                    if (menuItems != null && menuItems.Count > 0)
                    {
                        CurrentMenutems = new BindingList<SimpleItem>(menuItems);
                        TogglePopup();
                    }
                }
                return;
            }

            // Left or middle click on toggle/check/row
            if (hasHit)
            {
                var parts = htName.Split('_');
                if (parts.Length == 2)
                {
                    string type = parts[0], guid = parts[1];
                    var item = _treeHelper?.FindByGuid(guid);
                    if (item == null) return;

                    var args = new BeepMouseEventArgs(type, item);
                    switch (type)
                    {
                        case "toggle":
                            // COM-01: fire before-expand/collapse; callers can cancel
                            var cancelArgs = new BeepTreeNodeCancelEventArgs(item);
                            if (item.IsExpanded)
                                NodeBeforeCollapse?.Invoke(this, cancelArgs);
                            else
                                NodeBeforeExpand?.Invoke(this, cancelArgs);
                            if (cancelArgs.Cancel) break;

                            item.IsExpanded = !item.IsExpanded;
                            RebuildVisible();
                            UpdateScrollBars();

                            if (item.IsExpanded)
                                NodeExpanded?.Invoke(this, args);
                            else
                                NodeCollapsed?.Invoke(this, args);
                            break;

                        case "check":
                            item.IsChecked = !item.IsChecked;

                            if (item.IsChecked)
                                NodeChecked?.Invoke(this, args);
                            else
                                NodeUnchecked?.Invoke(this, args);
                            break;

                        case "row":
                        case "icon":
                            if (e.Button == MouseButtons.Left)
                            {
                                ClickedNode = item;
                                _lastClickedNode = item;
                                SelectedNode = item;

                                if (AllowMultiSelect)
                                {
                                    if (SelectedNodes.Contains(item))
                                    {
                                        SelectedNodes.Remove(item);
                                        item.IsSelected = false;
                                        NodeDeselected?.Invoke(this, args);
                                    }
                                    else
                                    {
                                        SelectedNodes.Add(item);
                                        item.IsSelected = true;
                                    }
                                }
                                else
                                {
                                    // Deselect all others
                                    foreach (var sel in SelectedNodes.ToList())
                                    {
                                        if (sel != item)
                                        {
                                            sel.IsSelected = false;
                                        }
                                    }
                                    SelectedNodes.Clear();

                                    item.IsSelected = true;
                                    SelectedNodes.Add(item);
                                    SelectedNode = item;
                                }

                                LeftButtonClicked?.Invoke(this, args);
                                NodeSelected?.Invoke(this, args);
                                OnSelectedItemChanged(item);
                            }
                            else if (e.Button == MouseButtons.Middle)
                            {
                                NodeMiddleClicked?.Invoke(this, args);
                            }
                            break;
                    }

                    Invalidate();
                }
            }
        }

        private void OnMouseUpHandler(object s, MouseEventArgs e)
        {
            NodeMouseUp?.Invoke(this, new BeepMouseEventArgs("MouseUp", null));
        }

        private void OnMouseMoveHandler(object s, MouseEventArgs e)
        {
            UpdateDrawingRect();
            // Only update hover; GetHover will invalidate small regions when changed
            GetHover();
            NodeMouseMove?.Invoke(this, new BeepMouseEventArgs("MouseMove", null));
        }

        private void OnMouseDoubleClickHandler(object s, MouseEventArgs e)
        {
            Point point = e.Location;
            if ((_treeHitTestHelper != null && _treeHitTestHelper.HitTest(point, out var hitName, out var hitItem, out var hitRect)) || LocalHitTest(point, out hitName, out hitItem, out hitRect))
            {
                NodeDoubleClicked?.Invoke(this, new BeepMouseEventArgs("NodeDoubleClick", hitItem));
            }
        }

        private void OnMouseHoverHandler(object? sender, EventArgs e)
        {
            GetHover();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Clear any hover state
            if (_lastHoveredItem != null)
            {
                if (!_lastHoveredRect.IsEmpty)
                    Invalidate(_lastHoveredRect);

                _lastHoveredItem = null;
                _lastHoveredRect = Rectangle.Empty;
            }

            NodeMouseLeave?.Invoke(this, new BeepMouseEventArgs("MouseLeave", null));
        }

        #endregion

        #region Keyboard Navigation

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_visibleNodes == null || _visibleNodes.Count == 0)
                return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    SelectPreviousNode();
                    e.Handled = true;
                    break;

                case Keys.Down:
                    SelectNextNode();
                    e.Handled = true;
                    break;

                case Keys.Left:
                    if (SelectedNode != null && SelectedNode.IsExpanded && SelectedNode.Children?.Count > 0)
                    {
                        SelectedNode.IsExpanded = false;
                        RebuildVisible();
                        UpdateScrollBars();
                        NodeCollapsed?.Invoke(this, new BeepMouseEventArgs("keyboard", SelectedNode));
                        Invalidate();
                    }
                    else if (SelectedNode?.ParentItem != null)
                    {
                        HighlightNode(SelectedNode.ParentItem);
                    }
                    e.Handled = true;
                    break;

                case Keys.Right:
                    if (SelectedNode != null && !(SelectedNode.IsExpanded) && SelectedNode.Children?.Count > 0)
                    {
                        SelectedNode.IsExpanded = true;
                        RebuildVisible();
                        UpdateScrollBars();
                        NodeExpanded?.Invoke(this, new BeepMouseEventArgs("keyboard", SelectedNode));
                        Invalidate();
                    }
                    e.Handled = true;
                    break;

                case Keys.Enter:
                case Keys.Space:
                    if (SelectedNode != null)
                    {
                        NodeSelected?.Invoke(this, new BeepMouseEventArgs("keyboard", SelectedNode));
                        OnSelectedItemChanged(SelectedNode);
                    }
                    e.Handled = true;
                    break;

                case Keys.Home:
                    if (_visibleNodes.Count > 0)
                        HighlightNode(_visibleNodes[0].Item);
                    e.Handled = true;
                    break;

                case Keys.End:
                    if (_visibleNodes.Count > 0)
                        HighlightNode(_visibleNodes[_visibleNodes.Count - 1].Item);
                    e.Handled = true;
                    break;

                case Keys.PageUp:
                {
                    int currentIndex = SelectedNode != null ? _visibleNodes.FindIndex(n => n.Item == SelectedNode) : 0;
                    int pageSize = Math.Max(1, DrawingRect.Height / Math.Max(1, GetScaledMinRowHeight()));
                    int targetIndex = Math.Max(0, currentIndex - pageSize);
                    HighlightNode(_visibleNodes[targetIndex].Item);
                    e.Handled = true;
                    break;
                }

                case Keys.PageDown:
                {
                    int currentIndex = SelectedNode != null ? _visibleNodes.FindIndex(n => n.Item == SelectedNode) : 0;
                    int pageSize = Math.Max(1, DrawingRect.Height / Math.Max(1, GetScaledMinRowHeight()));
                    int targetIndex = Math.Min(_visibleNodes.Count - 1, currentIndex + pageSize);
                    HighlightNode(_visibleNodes[targetIndex].Item);
                    e.Handled = true;
                    break;
                }
            }
        }

        #endregion

        #region Hover Detection

        private void GetHover()
        {
            Point mousePosition = PointToClient(MousePosition);

            // Find the node at the current mouse position
            if ((_treeHitTestHelper != null && _treeHitTestHelper.HitTest(mousePosition, out var hitName, out var hitItem, out var targetRect)) || LocalHitTest(mousePosition, out hitName, out hitItem, out targetRect))
            {
                string[] parts = hitName.Split('_');
                if (parts.Length == 2)
                {
                    string type = parts[0], guid = parts[1];
                    SimpleItem hoveredItem = _treeHelper?.FindByGuid(guid);

                    if (hoveredItem != null)
                    {
                        // If we're hovering over a different item than before
                        if (_lastHoveredItem != hoveredItem)
                        {
                            // Reset last hovered item
                            if (_lastHoveredItem != null)
                            {
                                _lastHoveredItem = null;

                                // Force redraw of previous area
                                if (!_lastHoveredRect.IsEmpty)
                                    Invalidate(_lastHoveredRect);
                            }

                            // Set hover state for current item
                            _lastHoveredItem = hoveredItem;
                            _lastHoveredRect = targetRect;

                            // Raise event with the hovered item
                            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseHover", hoveredItem);
                            NodeMouseHover?.Invoke(this, args);

                            // Redraw only the area of the hovered item
                            Invalidate(_lastHoveredRect);
                        }
                    }
                }
            }
            else
            {
                // Mouse isn't over any node
                if (_lastHoveredItem != null)
                {
                    _lastHoveredItem = null;

                    // Force redraw of previous hover area
                    if (!_lastHoveredRect.IsEmpty)
                        Invalidate(_lastHoveredRect);

                    _lastHoveredRect = Rectangle.Empty;
                    _lastClickedNode = null;
                }
            }
        }

        #endregion

        #region Local Hit Testing

        /// <summary>
        /// Local hit-test using cached content rectangles (no per-paint HitList building).
        /// Returns the type and item that was hit (toggle, check, icon, row).
        /// </summary>
        private bool LocalHitTest(Point p, out string name, out SimpleItem item, out Rectangle rect)
        {
            name = string.Empty;
            item = null;
            rect = Rectangle.Empty;

            if (_visibleNodes == null || _visibleNodes.Count == 0)
                return false;

            int viewportTop = _yOffset;
            int viewportBottom = _yOffset + DrawingRect.Height;

            // Find first node potentially visible
            int startIndex = 0;
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                var n = _visibleNodes[i];
                int bottom = n.Y + (n.RowHeight > 0 ? n.RowHeight : GetScaledMinRowHeight());
                if (bottom >= viewportTop)
                {
                    startIndex = i;
                    break;
                }
            }

            // Transform content rect to viewport via layout helper

            // Check each visible node
            for (int i = startIndex; i < _visibleNodes.Count; i++)
            {
                var n = _visibleNodes[i];
                if (n.Y > viewportBottom)
                    break;

                // Check if point is in the row's vertical range
                Rectangle rowVp = new Rectangle(
                    DrawingRect.Left,
                    DrawingRect.Top + (n.Y - _yOffset),
                    DrawingRect.Width,
                    n.RowHeight);

                if (!rowVp.Contains(p))
                    continue;

                // Check parts in priority order: toggle, check, icon, row

                // Toggle button
                var toggleVp = _layoutHelper.TransformToViewport(n.ToggleRectContent);
                if (!n.ToggleRectContent.IsEmpty && toggleVp.Contains(p))
                {
                    name = $"toggle_{n.Item.GuidId}";
                    item = n.Item;
                    rect = toggleVp;
                    return true;
                }

                // Checkbox
                if (ShowCheckBox && !n.CheckRectContent.IsEmpty)
                {
                    var checkVp = _layoutHelper.TransformToViewport(n.CheckRectContent);
                    if (checkVp.Contains(p))
                    {
                        name = $"check_{n.Item.GuidId}";
                        item = n.Item;
                        rect = checkVp;
                        return true;
                    }
                }

                // Icon
                if (!n.IconRectContent.IsEmpty)
                {
                    var iconVp = _layoutHelper.TransformToViewport(n.IconRectContent);
                    if (iconVp.Contains(p))
                    {
                        name = $"icon_{n.Item.GuidId}";
                        item = n.Item;
                        rect = iconVp;
                        return true;
                    }
                }

                // Row (catch-all for text and empty space)
                name = $"row_{n.Item.GuidId}";
                item = n.Item;
                rect = rowVp;
                return true;
            }

            return false;
        }

        #endregion

        #region Context Menu

        private bool _isPopupOpen;
        

        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }

        public void ShowPopup()
        {
            if (_isPopupOpen) return;
            if (CurrentMenutems == null || CurrentMenutems.Count == 0)
                return;

            // Compute screen location aligned with the clicked row
            int? viewportY = _lastContextMenuViewportY;
            if (viewportY == null && ClickedNode != null)
            {
                viewportY = DrawingRect.Top + Math.Max(0, ClickedNode.Y - _yOffset);
            }

            Point screenTopLeft = PointToScreen(Point.Empty);
            int targetY = screenTopLeft.Y + (viewportY ?? 0);
            int targetX = screenTopLeft.X + this.Width + 10; // small gap to the right of control
            Point screenLocation = new Point(targetX, targetY);

            _isPopupOpen = true;
            var selectedItem = base.ShowContextMenu(CurrentMenutems.ToList(), screenLocation, multiSelect: false );
            _isPopupOpen = false;
            _lastContextMenuViewportY = null; // reset after use

            if (selectedItem != null)
            {
                MenuItemSelected?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
            }
            Invalidate();
        }

        public void ClosePopup()
        {
            // No persistent popup to close when using BeepContextMenu per call
            _isPopupOpen = false;
            Invalidate();
        }

        public void ShowContextMenu(BindingList<SimpleItem> menuList)
        {
            CurrentMenutems = menuList;
            TogglePopup();
        }

        #endregion
    }
}
