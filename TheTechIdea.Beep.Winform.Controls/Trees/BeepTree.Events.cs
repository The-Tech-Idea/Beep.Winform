using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepTree - Events partial class.
    /// Handles all mouse events, clicks, user interactions, hover effects, and context menus.
    /// </summary>
    public partial class BeepTree
    {
        #region Mouse Event Handlers

        private void OnMouseDownHandler(object sender, MouseEventArgs e)
        {
            NodeMouseDown?.Invoke(this, new BeepMouseEventArgs("MouseDown", null));
            Point point = e.Location;

            // Right-click context menu
            if (e.Button == MouseButtons.Right && LocalHitTest(point, out string htName, out var htItem, out var htRect) && htName.StartsWith("row_"))
            {
                var guid = htName.Substring(4); // everything after "row_"
                var item = _treeHelper?.FindByGuid(guid);
                if (item != null)
                {
                    ClickedNode = item;
                    var args = new BeepMouseEventArgs("RightClick", item);
                    NodeRightClicked?.Invoke(this, args);

                    var menuItems = SimpleItemFactory.GlobalMenuItemsProvider(item);
                    if (menuItems != null && menuItems.Count > 0)
                    {
                        CurrentMenutems = new BindingList<SimpleItem>(menuItems);
                        TogglePopup();
                    }
                }
                return;
            }

            // Left or middle click on toggle/check/row
            if (LocalHitTest(point, out var hitName, out var hitItem, out var hitRect))
            {
                var parts = hitName.Split('_');
                if (parts.Length == 2)
                {
                    string type = parts[0], guid = parts[1];
                    var item = _treeHelper?.FindByGuid(guid);
                    if (item == null) return;

                    var args = new BeepMouseEventArgs(type, item);
                    switch (type)
                    {
                        case "toggle":
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
                                        NodeSelected?.Invoke(this, args);
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
            GetHover();
            NodeMouseMove?.Invoke(this, new BeepMouseEventArgs("MouseMove", null));
        }

        private void OnMouseDoubleClickHandler(object s, MouseEventArgs e)
        {
            Point point = e.Location;
            if (LocalHitTest(point, out var hitName, out var hitItem, out var hitRect))
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

        #region Hover Detection

        private void GetHover()
        {
            Point mousePosition = PointToClient(MousePosition);

            // Find the node at the current mouse position
            if (LocalHitTest(mousePosition, out var hitName, out var hitItem, out var targetRect))
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

            // Transform content rect to viewport
            Rectangle ToViewport(Rectangle rc) => new Rectangle(
                DrawingRect.Left + rc.X - _xOffset,
                DrawingRect.Top + rc.Y - _yOffset,
                rc.Width,
                rc.Height);

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
                var toggleVp = ToViewport(n.ToggleRectContent);
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
                    var checkVp = ToViewport(n.CheckRectContent);
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
                    var iconVp = ToViewport(n.IconRectContent);
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

        private BeepPopupListForm _menuDialog;
        private bool _isPopupOpen;

        [Browsable(false)]
        public BeepPopupListForm PopupListForm
        {
            get => _menuDialog;
            set => _menuDialog = value;
        }

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

            ClosePopup();

            _menuDialog = new BeepPopupListForm(CurrentMenutems.ToList());
            _menuDialog.Theme = Theme;
            _menuDialog.SelectedItemChanged += MenuDialog_SelectedItemChanged;

            // Show popup near the clicked node
            Point popupLocation = new Point(10, ClickedNode != null ? ClickedNode.Y : 0);
            SimpleItem selectedItem = _menuDialog.ShowPopup(Text, this, popupLocation, BeepPopupFormPosition.Right, false, true);
            _isPopupOpen = true;
            Invalidate();
        }

        public void ClosePopup()
        {
            if (!_isPopupOpen) return;

            if (_menuDialog != null)
            {
                _menuDialog.SelectedItemChanged -= MenuDialog_SelectedItemChanged;
                _menuDialog.CloseCascade();
                _menuDialog.Dispose();
                _menuDialog = null;
            }

            _isPopupOpen = false;
            Invalidate();
        }

        public void ShowContextMenu(BindingList<SimpleItem> menuList)
        {
            CurrentMenutems = menuList;
            TogglePopup();
        }

        private void MenuDialog_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            MenuItemSelected?.Invoke(this, e);
            ClosePopup();
        }

        #endregion
    }
}
