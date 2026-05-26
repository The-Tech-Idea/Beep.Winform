using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Trees.Editors;

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

            // Check for breadcrumb click
            if (ShowBreadcrumb && e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < _breadcrumbItemRects.Count; i++)
                {
                    if (_breadcrumbItemRects[i].Contains(point))
                    {
                        var clickedNode = _breadcrumbItems[i];
                        if (clickedNode != null)
                        {
                            NavigateToBreadcrumbItem(clickedNode);
                            return;
                        }
                    }
                }
            }

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

                    //var provider = SimpleItemFactory.GlobalMenuItemsProvider;
                    //var menuItems = provider != null ? provider(item) : null;
                    //if (menuItems != null && menuItems.Count > 0)
                    //{
                    //    CurrentMenutems = new BindingList<SimpleItem>(menuItems);
                    //    TogglePopup();
                    //}
                }
                return;
            }

            // Multi-column: check for column header click or resize
            if (IsMultiColumn && ShowColumnHeaders && e.Button == MouseButtons.Left)
            {
                var headerBounds = new Rectangle(DrawingRect.X, DrawingRect.Y, DrawingRect.Width, ColumnHeaderHeight);
                if (headerBounds.Contains(point))
                {
                    int x = DrawingRect.X;
                    int colIndex = 0;
                    foreach (var column in Columns.GetVisibleColumns())
                    {
                        var colRect = new Rectangle(x, headerBounds.Y, column.Width, headerBounds.Height);

                        // Check for resize (near right edge)
                        if (Math.Abs(point.X - colRect.Right) <= RESIZE_HIT_TEST_MARGIN)
                        {
                            _isResizingColumn = true;
                            _resizingColumnIndex = colIndex;
                            _resizeStartX = point.X;
                            _resizeStartWidth = column.Width;
                            return;
                        }

                        // Check for filter button click
                        if (column.Filterable)
                        {
                            var filterRect = new Rectangle(colRect.Right - 20, colRect.Top + (colRect.Height - 16) / 2, 18, 18);
                            if (filterRect.Contains(point))
                            {
                                ShowColumnFilterPopup(column, colIndex, filterRect);
                                return;
                            }
                        }

                        // Check for header click
                        if (colRect.Contains(point))
                        {
                            OnColumnHeaderClick(column, colIndex, (ModifierKeys & Keys.Control) == Keys.Control);
                            return;
                        }
                        x += column.Width;
                        colIndex++;
                    }
                }
            }

            // Kinetic scrolling: start tracking on left click
            if (e.Button == MouseButtons.Left && EnableKineticScrolling)
            {
                _isKineticScrolling = true;
                _kineticScrollStartPoint = e.Location;
                _kineticScrollStartYOffset = _yOffset;
                _kineticVelocityY = 0;
                _kineticTimer?.Stop();
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

                            // Lazy load: if expanding and no children, fire NodesNeeded
                            if (!item.IsExpanded && LazyLoad && (item.Children == null || item.Children.Count == 0))
                            {
                                var neededArgs = new NodesNeededEventArgs(item);
                                NodesNeeded?.Invoke(this, neededArgs);
                                if (!neededArgs.ChildrenLoaded)
                                {
                                    // No children available, don't expand
                                    break;
                                }
                            }

                            bool expanding = !item.IsExpanded;
                            item.IsExpanded = expanding;

                            // Animate expand/collapse if enabled
                            if (EnableAnimations && _animationHelper != null)
                            {
                                var nodeInfo = _visibleNodes.FirstOrDefault(n => n.Item == item);
                                if (nodeInfo.Item != null)
                                {
                                    var bounds = _layoutHelper.TransformToViewport(nodeInfo.RowRectContent);
                                    _animationHelper.AnimateExpandCollapse(bounds, expanding);
                                }
                            }

                            RebuildVisible();
                            UpdateScrollBars();

                            if (item.IsExpanded)
                                NodeExpanded?.Invoke(this, args);
                            else
                                NodeCollapsed?.Invoke(this, args);

                            // Notify accessibility clients of state change
                            var treeAccessibleObject = AccessibilityObject as BeepTreeAccessibleObject;
                            treeAccessibleObject?.NotifyStateChanged(item);
                            break;

                        case "check":
                            if (EnableThreeStateCheckboxes)
                            {
                                // Cycle through states: Unchecked -> Checked -> Indeterminate -> Unchecked
                                if (!item.IsChecked && !item.IsIndeterminate)
                                {
                                    item.IsChecked = true;
                                    item.IsIndeterminate = false;
                                }
                                else if (item.IsChecked && !item.IsIndeterminate)
                                {
                                    item.IsChecked = false;
                                    item.IsIndeterminate = true;
                                }
                                else
                                {
                                    item.IsChecked = false;
                                    item.IsIndeterminate = false;
                                }

                                // Cascade to children
                                CascadeCheckState(item);

                                // Update parent states
                                UpdateParentCheckStates(item);
                            }
                            else
                            {
                                item.IsChecked = !item.IsChecked;
                                item.IsIndeterminate = false;
                            }

                            if (item.IsChecked)
                                NodeChecked?.Invoke(this, args);
                            else if (!item.IsIndeterminate)
                                NodeUnchecked?.Invoke(this, args);

                            // Notify accessibility clients of state change
                            var checkAccessibleObject = AccessibilityObject as BeepTreeAccessibleObject;
                            checkAccessibleObject?.NotifyStateChanged(item);
                            break;

                        case "row":
                        case "icon":
                            if (e.Button == MouseButtons.Left)
                            {
                                ClickedNode = item;
                                _lastClickedNode = item;
                                SelectedNode = item;

                                // Check for slow double-click to start editing
                                if (AllowEdit && item == _lastDoubleClickNode)
                                {
                                    var elapsed = DateTime.Now - _lastClickTime;
                                    if (elapsed.TotalMilliseconds >= SLOW_DOUBLE_CLICK_MIN_MS &&
                                        elapsed.TotalMilliseconds <= SLOW_DOUBLE_CLICK_MAX_MS)
                                    {
                                        BeginEdit(item);
                                        _lastDoubleClickNode = null;
                                        return;
                                    }
                                }

                                _lastClickTime = DateTime.Now;
                                _lastDoubleClickNode = item;

                                // Initiate drag and drop if enabled
                                if (AllowDragDrop && _dragDropManager != null)
                                {
                                    _dragDropManager.OnMouseDown(e, item);
                                }

                                if (AllowMultiSelect)
                                {
                                    bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
                                    bool shiftPressed = (ModifierKeys & Keys.Shift) == Keys.Shift;

                                    if (shiftPressed && _lastSelectedNode != null && _lastSelectedNode != item)
                                    {
                                        // Shift+Click: select range from last selected to current
                                        int anchorIndex = _visibleNodes.FindIndex(n => n.Item == _lastSelectedNode);
                                        int currentIndex = _visibleNodes.FindIndex(n => n.Item == item);

                                        if (anchorIndex >= 0 && currentIndex >= 0)
                                        {
                                            int start = Math.Min(anchorIndex, currentIndex);
                                            int end = Math.Max(anchorIndex, currentIndex);

                                            // Clear existing selection unless Ctrl is also held
                                            if (!ctrlPressed)
                                            {
                                                foreach (var sel in SelectedNodes)
                                                    sel.IsSelected = false;
                                                SelectedNodes.Clear();
                                            }

                                            for (int i = start; i <= end; i++)
                                            {
                                                var nodeItem = _visibleNodes[i].Item;
                                                if (!SelectedNodes.Contains(nodeItem))
                                                {
                                                    SelectedNodes.Add(nodeItem);
                                                    nodeItem.IsSelected = true;
                                                }
                                            }
                                        }
                                    }
                                    else if (ctrlPressed)
                                    {
                                        // Ctrl+Click: toggle individual node
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
                                        // Plain click: single selection even in multi-select mode
                                        foreach (var sel in SelectedNodes.ToList())
                                        {
                                            if (sel != item)
                                                sel.IsSelected = false;
                                        }
                                        SelectedNodes.Clear();

                                        item.IsSelected = true;
                                        SelectedNodes.Add(item);
                                        SelectedNode = item;
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
                                // NodeSelected and OnSelectedItemChanged are now fired by SelectedNode setter
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
            // End column resize
            if (_isResizingColumn)
            {
                _isResizingColumn = false;
                _resizingColumnIndex = -1;
                Cursor = Cursors.Default;
                RecalculateLayoutCache();
                UpdateScrollBars();
                Invalidate();
            }

            // End drag and drop
            if (AllowDragDrop && _dragDropManager != null)
            {
                _dragDropManager.OnMouseUp(e);
            }

            // End kinetic scrolling and start momentum
            if (EnableKineticScrolling && _isKineticScrolling)
            {
                _isKineticScrolling = false;
                if (Math.Abs(_kineticVelocityY) >= KINETIC_MIN_VELOCITY)
                {
                    _kineticTimer?.Start();
                }
            }

            NodeMouseUp?.Invoke(this, new BeepMouseEventArgs("MouseUp", null));
        }

        private void OnMouseMoveHandler(object s, MouseEventArgs e)
        {
            UpdateDrawingRect();

            // Drag and drop tracking
            if (AllowDragDrop && _dragDropManager != null)
            {
                _dragDropManager.OnMouseMove(e);
            }

            // Kinetic scrolling: drag to scroll
            if (EnableKineticScrolling && _isKineticScrolling && e.Button == MouseButtons.Left)
            {
                int deltaY = e.Y - _kineticScrollStartPoint.Y;
                int newYOffset = _kineticScrollStartYOffset - deltaY;

                // Calculate velocity for momentum (track actual movement per frame)
                int actualDeltaY = _yOffset - newYOffset;
                _kineticVelocityY = actualDeltaY;

                if (_verticalScrollBar?.Visible == true)
                {
                    int clampedY = Math.Max(_verticalScrollBar.Minimum,
                        Math.Min(_verticalScrollBar.Maximum - _verticalScrollBar.LargeChange, newYOffset));
                    if (clampedY != _yOffset)
                    {
                        _yOffset = clampedY;
                        _verticalScrollBar.Value = clampedY;
                        try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
                        Invalidate();
                    }
                }
                return;
            }

            // Column resize in progress
            if (_isResizingColumn && _resizingColumnIndex >= 0)
            {
                int delta = e.X - _resizeStartX;
                int newWidth = Math.Max(20, _resizeStartWidth + delta);

                var visibleColumns = Columns.GetVisibleColumns().ToList();
                if (_resizingColumnIndex < visibleColumns.Count)
                {
                    visibleColumns[_resizingColumnIndex].Width = newWidth;
                    RecalculateLayoutCache();
                    UpdateScrollBars();
                    Invalidate();
                }
                return;
            }

            // Check for column resize cursor
            if (IsMultiColumn && ShowColumnHeaders)
            {
                var headerBounds = new Rectangle(DrawingRect.X, DrawingRect.Y, DrawingRect.Width, ColumnHeaderHeight);
                if (headerBounds.Contains(e.Location))
                {
                    int x = DrawingRect.X;
                    int colIndex = 0;
                    foreach (var column in Columns.GetVisibleColumns())
                    {
                        var colRect = new Rectangle(x, headerBounds.Y, column.Width, headerBounds.Height);
                        // Check if near right edge
                        if (Math.Abs(e.X - colRect.Right) <= RESIZE_HIT_TEST_MARGIN && e.X >= colRect.Right - RESIZE_HIT_TEST_MARGIN)
                        {
                            Cursor = Cursors.VSplit;
                            return;
                        }
                        x += column.Width;
                        colIndex++;
                    }
                }
            }

            // Reset cursor if not resizing
            if (!_isResizingColumn)
            {
                Cursor = Cursors.Default;
            }

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

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (AllowDragDrop && _dragDropManager != null)
            {
                Point clientPoint = PointToClient(new Point(e.X, e.Y));
                _dragDropManager.OnDragOver(e, clientPoint);
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (AllowDragDrop && _dragDropManager != null)
            {
                Point clientPoint = PointToClient(new Point(e.X, e.Y));
                _dragDropManager.OnDragDrop(e, clientPoint);
            }
        }

        #endregion

        #region Keyboard Navigation

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Ctrl+F: Show find panel
            if (e.KeyCode == Keys.F && e.Control)
            {
                ShowFindPanel();
                e.Handled = true;
                return;
            }

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

                case Keys.F2:
                    if (AllowEdit && SelectedNode != null)
                    {
                        BeginEdit(SelectedNode);
                        e.Handled = true;
                    }
                    break;

                case Keys.Left:
                    if (SelectedNode != null && SelectedNode.IsExpanded && SelectedNode.Children?.Count > 0)
                    {
                        SelectedNode.IsExpanded = false;
                        RebuildVisible();
                        UpdateScrollBars();
                        NodeCollapsed?.Invoke(this, new BeepMouseEventArgs("keyboard", SelectedNode));
                        Invalidate();

                        // Notify accessibility clients
                        var leftAccessibleObject = AccessibilityObject as BeepTreeAccessibleObject;
                        leftAccessibleObject?.NotifyStateChanged(SelectedNode);
                    }
                    else if (SelectedNode?.ParentItem != null)
                    {
                        HighlightNode(SelectedNode.ParentItem);
                    }
                    e.Handled = true;
                    break;

                case Keys.Right:
                    if (SelectedNode != null && !SelectedNode.IsExpanded)
                    {
                        bool hasChildren = SelectedNode.Children != null && SelectedNode.Children.Count > 0;

                        // Lazy load: if no children, try to load them
                        if (!hasChildren && LazyLoad)
                        {
                            var neededArgs = new NodesNeededEventArgs(SelectedNode);
                            NodesNeeded?.Invoke(this, neededArgs);
                            hasChildren = neededArgs.ChildrenLoaded;
                        }

                        if (hasChildren)
                        {
                            SelectedNode.IsExpanded = true;
                            RebuildVisible();
                            UpdateScrollBars();
                            NodeExpanded?.Invoke(this, new BeepMouseEventArgs("keyboard", SelectedNode));
                            Invalidate();

                            // Notify accessibility clients
                            var rightAccessibleObject = AccessibilityObject as BeepTreeAccessibleObject;
                            rightAccessibleObject?.NotifyStateChanged(SelectedNode);
                        }
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
                    if ((ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        // Ctrl+Home: collapse all and select first root
                        CollapseAll();
                        if (_nodes.Count > 0)
                            HighlightNode(_nodes[0]);
                    }
                    else if (_visibleNodes.Count > 0)
                    {
                        HighlightNode(_visibleNodes[0].Item);
                    }
                    e.Handled = true;
                    break;

                case Keys.End:
                    if ((ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        // Ctrl+End: expand all and select last visible leaf
                        ExpandAll();
                        if (_visibleNodes.Count > 0)
                            HighlightNode(_visibleNodes[_visibleNodes.Count - 1].Item);
                    }
                    else if (_visibleNodes.Count > 0)
                    {
                        HighlightNode(_visibleNodes[_visibleNodes.Count - 1].Item);
                    }
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

                default:
                    // Type-ahead search: letter/number keys jump to matching node
                    if (IsTypeAheadKey(e.KeyCode) && !e.Control && !e.Alt)
                    {
                        char keyChar = GetKeyChar(e.KeyCode);
                        if (keyChar != '\0')
                        {
                            _typeAheadBuffer += keyChar;
                            _typeAheadTimer?.Stop();
                            _typeAheadTimer?.Start();

                            int startIndex = SelectedNode != null
                                ? _visibleNodes.FindIndex(n => n.Item == SelectedNode) + 1
                                : 0;

                            // Wrap around if needed
                            if (startIndex >= _visibleNodes.Count)
                                startIndex = 0;

                            // Search from startIndex to end, then from beginning to startIndex
                            SimpleItem match = null;
                            for (int i = startIndex; i < _visibleNodes.Count; i++)
                            {
                                if (_visibleNodes[i].Item.Text?.StartsWith(_typeAheadBuffer, StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    match = _visibleNodes[i].Item;
                                    break;
                                }
                            }

                            if (match == null && startIndex > 0)
                            {
                                for (int i = 0; i < startIndex; i++)
                                {
                                    if (_visibleNodes[i].Item.Text?.StartsWith(_typeAheadBuffer, StringComparison.OrdinalIgnoreCase) == true)
                                    {
                                        match = _visibleNodes[i].Item;
                                        break;
                                    }
                                }
                            }

                            if (match != null)
                            {
                                HighlightNode(match);
                            }

                            e.Handled = true;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Determines if a key code is valid for type-ahead search (letters, digits, space).
        /// </summary>
        private static bool IsTypeAheadKey(Keys keyCode)
        {
            return (keyCode >= Keys.A && keyCode <= Keys.Z) ||
                   (keyCode >= Keys.D0 && keyCode <= Keys.D9) ||
                   (keyCode >= Keys.NumPad0 && keyCode <= Keys.NumPad9) ||
                   keyCode == Keys.Space;
        }

        /// <summary>
        /// Converts a Keys enum value to its character representation for type-ahead.
        /// </summary>
        private static char GetKeyChar(Keys keyCode)
        {
            if (keyCode >= Keys.A && keyCode <= Keys.Z)
                return (char)('a' + (keyCode - Keys.A));
            if (keyCode >= Keys.D0 && keyCode <= Keys.D9)
                return (char)('0' + (keyCode - Keys.D0));
            if (keyCode >= Keys.NumPad0 && keyCode <= Keys.NumPad9)
                return (char)('0' + (keyCode - Keys.NumPad0));
            if (keyCode == Keys.Space)
                return ' ';
            return '\0';
        }

        #endregion

        #region Inline Editing

        /// <summary>
        /// Occurs when a node begins editing.
        /// </summary>
        public event EventHandler<BeepMouseEventArgs> NodeBeginEdit;

        /// <summary>
        /// Occurs when a node edit is validated.
        /// </summary>
        public event EventHandler<BeepTreeNodeValidatingEventArgs> NodeValidating;

        /// <summary>
        /// Occurs when a node edit is completed.
        /// </summary>
        public event EventHandler<BeepTreeNodeEndEditEventArgs> NodeEndEdit;

        /// <summary>
        /// Raises the NodeValidating event.
        /// </summary>
        internal void OnNodeValidating(BeepTreeNodeValidatingEventArgs args)
        {
            NodeValidating?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the NodeEndEdit event.
        /// </summary>
        internal void OnNodeEndEdit(SimpleItem node, BeepTreeColumn column, object value)
        {
            NodeEndEdit?.Invoke(this, new BeepTreeNodeEndEditEventArgs(node, column, value));
        }

        /// <summary>
        /// Begins editing the currently selected node.
        /// </summary>
        public bool BeginEdit()
        {
            return BeginEdit(SelectedNode);
        }

        /// <summary>
        /// Begins editing the specified node.
        /// </summary>
        public bool BeginEdit(SimpleItem node)
        {
            if (!AllowEdit || node == null)
                return false;

            NodeBeginEdit?.Invoke(this, new BeepMouseEventArgs("BeginEdit", node));
            return _cellEditor?.BeginEdit(node) ?? false;
        }

        /// <summary>
        /// Ends the current edit operation.
        /// </summary>
        public bool EndEdit(bool acceptChanges)
        {
            return _cellEditor?.EndEdit(acceptChanges) ?? false;
        }

        /// <summary>
        /// Cancels the current edit operation.
        /// </summary>
        public void CancelEdit()
        {
            _cellEditor?.CancelEdit();
        }

        #endregion

        #region Custom Draw Events

        /// <summary>
        /// Occurs when a node is about to be drawn. Set Handled = true to skip default drawing.
        /// </summary>
        public event EventHandler<BeepTreeCustomDrawNodeEventArgs> CustomDrawNode;

        /// <summary>
        /// Occurs when a node background is about to be drawn.
        /// </summary>
        public event EventHandler<BeepTreeCustomDrawNodeBackgroundEventArgs> CustomDrawNodeBackground;

        /// <summary>
        /// Occurs when a cell is about to be drawn.
        /// </summary>
        public event EventHandler<BeepTreeCustomDrawCellEventArgs> CustomDrawCell;

        /// <summary>
        /// Occurs when a column header is about to be drawn.
        /// </summary>
        public event EventHandler<BeepTreeCustomDrawColumnHeaderEventArgs> CustomDrawColumnHeader;

        /// <summary>
        /// Raises the CustomDrawNode event.
        /// </summary>
        internal bool OnCustomDrawNode(Graphics g, Rectangle bounds, SimpleItem node, NodeInfo nodeInfo, bool isSelected, bool isHovered, Action defaultDraw)
        {
            if (CustomDrawNode == null) return false;
            var args = new BeepTreeCustomDrawNodeEventArgs(g, bounds, node, nodeInfo, isSelected, isHovered, defaultDraw);
            CustomDrawNode.Invoke(this, args);
            return args.Handled;
        }

        /// <summary>
        /// Raises the CustomDrawNodeBackground event.
        /// </summary>
        internal bool OnCustomDrawNodeBackground(Graphics g, Rectangle bounds, SimpleItem node, bool isSelected, bool isHovered, Action defaultDraw)
        {
            if (CustomDrawNodeBackground == null) return false;
            var args = new BeepTreeCustomDrawNodeBackgroundEventArgs(g, bounds, node, isSelected, isHovered, defaultDraw);
            CustomDrawNodeBackground.Invoke(this, args);
            return args.Handled;
        }

        /// <summary>
        /// Raises the CustomDrawCell event.
        /// </summary>
        internal bool OnCustomDrawCell(Graphics g, Rectangle bounds, SimpleItem node, BeepTreeColumn column, int columnIndex, object value, bool isSelected, Action defaultDraw)
        {
            if (CustomDrawCell == null) return false;
            var args = new BeepTreeCustomDrawCellEventArgs(g, bounds, node, column, columnIndex, value, isSelected, defaultDraw);
            CustomDrawCell.Invoke(this, args);
            return args.Handled;
        }

        /// <summary>
        /// Raises the CustomDrawColumnHeader event.
        /// </summary>
        internal bool OnCustomDrawColumnHeader(Graphics g, Rectangle bounds, BeepTreeColumn column, int columnIndex, Action defaultDraw)
        {
            if (CustomDrawColumnHeader == null) return false;
            var args = new BeepTreeCustomDrawColumnHeaderEventArgs(g, bounds, column, columnIndex, defaultDraw);
            CustomDrawColumnHeader.Invoke(this, args);
            return args.Handled;
        }

        #endregion

        #region Breadcrumb Navigation

        /// <summary>
        /// Occurs when a breadcrumb item is clicked.
        /// </summary>
        public event EventHandler<BeepMouseEventArgs> BreadcrumbClicked;

        /// <summary>
        /// Navigates to the specified node in the breadcrumb path.
        /// </summary>
        public void NavigateToBreadcrumbItem(SimpleItem node)
        {
            if (node == null)
                return;

            // Select the node
            SelectedNode = node;

            // Ensure all ancestors are expanded
            var current = node.ParentItem;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.ParentItem;
            }

            RebuildVisible();
            UpdateScrollBars();
            ScrollToNode(node);
            Invalidate();

            BreadcrumbClicked?.Invoke(this, new BeepMouseEventArgs("BreadcrumbClick", node));
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

            Rectangle viewportArea = GetClientArea();
            int viewportTop = _yOffset;
            int viewportBottom = _yOffset + viewportArea.Height;

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
                    viewportArea.Left,
                    viewportArea.Top + (n.Y - _yOffset),
                    viewportArea.Width,
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
            Rectangle viewportArea = GetClientArea();

            // Compute screen location aligned with the clicked row
            int? viewportY = _lastContextMenuViewportY;
            if (viewportY == null && ClickedNode != null)
            {
                viewportY = viewportArea.Top + Math.Max(0, ClickedNode.Y - _yOffset);
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
