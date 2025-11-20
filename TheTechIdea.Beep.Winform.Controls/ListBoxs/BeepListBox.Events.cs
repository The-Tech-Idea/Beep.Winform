using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Event handlers for BeepListBox
    /// </summary>
    public partial class BeepListBox
    {
        #region Mouse Events

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // route to centralized input
            _input?.OnMouseMove(e.Location);

            if (_listBoxPainter == null)
            {
                _listBoxPainter = CreatePainter(_listBoxType);
                _listBoxPainter?.Initialize(this, _currentTheme);
                _needsLayoutUpdate = true;
            }
            if (_listBoxPainter.Style != ControlStyle)
            {
                _listBoxPainter.Style = ControlStyle;
            }
            // Hover over search area
            if (_showSearch && _searchAreaRect.Contains(e.Location))
            {
                Cursor = Cursors.IBeam;
                return;
            }

            // Determine hovered item using layout cache
            var layoutCache = _layout.GetCachedLayout();
            SimpleItem newHoveredItem = null;
            if (layoutCache != null && layoutCache.Count > 0)
            {
                foreach (var info in layoutCache)
                {
                    if (info.RowRect.Contains(e.Location))
                    {
                        newHoveredItem = info.Item;
                        break;
                    }
                }
            }

            if (newHoveredItem != _hoveredItem)
            {
                // Preserve previous hover progress so we can fade it out
                _prevHoveredItem = _hoveredItem;
                _prevHoverProgress = _hoverProgress;
                _hoveredItem = newHoveredItem;
                Cursor = _hoveredItem != null ? Cursors.Hand : Cursors.Default;
                Invalidate();

                // Start hover animation
                if (EnableHoverAnimation)
                {
                    _hoverTarget = _hoveredItem != null;
                    if (!_hoverAnimationTimer.Enabled) _hoverAnimationTimer.Start();
                }
            }
            else
            {
                // Ensure animation target is correct even when re-hovering same item
                if (EnableHoverAnimation && _hoveredItem != null)
                {
                    _hoverTarget = true;
                    if (!_hoverAnimationTimer.Enabled) _hoverAnimationTimer.Start();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredItem != null)
            {
                _prevHoveredItem = _hoveredItem;
                _prevHoverProgress = _hoverProgress;
                _hoveredItem = null;
                Cursor = Cursors.Default;
                Invalidate();
                if (EnableHoverAnimation)
                {
                    _hoverTarget = false;
                    if (!_hoverAnimationTimer.Enabled) _hoverAnimationTimer.Start();
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_listBoxPainter == null)
            {
                _listBoxPainter = CreatePainter(_listBoxType);
                _listBoxPainter?.Initialize(this, _currentTheme);
                _needsLayoutUpdate = true;
            }

            // search area click
            if (_showSearch && _searchAreaRect.Contains(e.Location))
            {
                return;
            }

            _input?.OnClick();

            // Centralized hit-test
            if (_hitHelper != null && _hitHelper.HitTest(e.Location, out var hitName, out var hitItem, out var hitRect))
            {
                var parts = hitName.Split('_');
                var kind = parts.Length > 0 ? parts[0] : string.Empty;
                switch (kind)
                {
                    case "check":
                        if (_showCheckBox)
                        {
                            ToggleItemCheckbox(hitItem);
                            return;
                        }
                        break;
                    case "icon":
                    case "text":
                    case "row":
                        HandleItemClick(hitItem, e, hitRect);
                        return;
                }
            }

            // Fallback manual scan using layout cache
            var layoutCache = _layout.GetCachedLayout();
            if (layoutCache != null && layoutCache.Count > 0)
            {
                foreach (var info in layoutCache)
                {
                    if (info.RowRect.Contains(e.Location))
                    {
                        HandleItemClick(info.Item, e, info.RowRect);
                        break;
                    }
                }
            }
        }

        private void HandleItemClick(SimpleItem item, MouseEventArgs e, Rectangle itemRect)
        {
            if (item == null) return;

            // If clicking checkbox area for painters that support it
            if (_showCheckBox && _listBoxPainter.SupportsCheckboxes())
            {
                Rectangle checkboxRect = new Rectangle(
                    itemRect.Left + 8,
                    itemRect.Y + (itemRect.Height - 16) / 2,
                    16,
                    16);

                if (checkboxRect.Contains(e.Location))
                {
                    ToggleItemCheckbox(item);
                    return;
                }
            }

            // Multi-select and modifier-aware click handling
            var modifiers = Control.ModifierKeys;
            bool shift = (modifiers & Keys.Shift) == Keys.Shift;
            bool ctrl = (modifiers & Keys.Control) == Keys.Control;

            if (SelectionMode == BeepListBox.SelectionMode.MultiSimple || SelectionMode == BeepListBox.SelectionMode.MultiExtended)
            {
                var visibleItems = _helper.GetVisibleItems();
                if (SelectionMode == BeepListBox.SelectionMode.MultiExtended && shift && _anchorItem != null && _listItems.Count > 0)
                {
                    int targetIndex = visibleItems.IndexOf(item);
                    if (targetIndex >= 0)
                    {
                        // Range select
                        int anchorIdx = visibleItems.IndexOf(_anchorItem);
                        SelectRange(visibleItems, anchorIdx, targetIndex);
                    }
                }
                else if (ctrl)
                {
                    // Toggle selection
                    ToggleSelection(item);
                }
                else
                {
                    if (SelectionMode == BeepListBox.SelectionMode.MultiSimple)
                    {
                        // Toggle selection on click (MultiSimple)
                        ToggleSelection(item);
                        SelectedItem = item; // focus
                    }
                    else
                    {
                        // MultiExtended default behavior: clear previous selection and set focus
                        ClearSelection();
                        AddToSelection(item);
                        SelectedItem = item;
                    }
                }
            }
            else
            {
                SelectedItem = item;
            }
            OnItemClicked(item);

            // If hosted in a popup, notify and close
            var hostForm = FindForm() as BeepPopupForm;
            if (hostForm != null)
            {
                hostForm.NotifySelectedItemChanged(item);
                hostForm.DialogResult = DialogResult.OK;
            }
        }

        #endregion

        #region Keyboard Events

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_listItems == null || _listItems.Count == 0) return;

            var visibleItems = _helper.GetVisibleItems();
            if (visibleItems.Count == 0) return;

            bool shift = e.Shift;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveSelectionUp(visibleItems, shift);
                    e.Handled = true;
                    break;

                case Keys.Down:
                    MoveSelectionDown(visibleItems, shift);
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                {
                    if (visibleItems.Count == 0) break;
                    int itemHeight = _listBoxPainter.GetPreferredItemHeight();
                    Rectangle clientArea = GetClientArea();
                    int page = Math.Max(1, clientArea.Height / itemHeight) - 1;
                    int currentIndex = _selectedItem == null ? 0 : visibleItems.IndexOf(_selectedItem);
                    int newIndex = Math.Max(0, currentIndex - page);
                    if (newIndex >= 0 && newIndex < visibleItems.Count)
                    {
                        if (shift && (SelectionMode == BeepListBox.SelectionMode.MultiExtended))
                        {
                            int anchorIdx = visibleItems.IndexOf(_anchorItem);
                            SelectRange(visibleItems, anchorIdx < 0 ? newIndex : anchorIdx, newIndex);
                        }
                        else if (shift && _showCheckBox)
                        {
                            RangeCheck(visibleItems, newIndex);
                        }
                        SelectedItem = visibleItems[newIndex];
                        if (!shift) _anchorItem = SelectedItem;
                        EnsureItemVisible(SelectedItem);
                    }
                    e.Handled = true;
                }
                break;

                case Keys.PageDown:
                {
                    if (visibleItems.Count == 0) break;
                    int itemHeight = _listBoxPainter.GetPreferredItemHeight();
                    Rectangle clientArea = GetClientArea();
                    int page = Math.Max(1, clientArea.Height / itemHeight) - 1;
                    int currentIndex = _selectedItem == null ? 0 : visibleItems.IndexOf(_selectedItem);
                    int newIndex = Math.Min(visibleItems.Count - 1, currentIndex + page);
                    if (newIndex >= 0 && newIndex < visibleItems.Count)
                    {
                        if (shift && (SelectionMode == BeepListBox.SelectionMode.MultiExtended))
                        {
                            int anchorIdx = visibleItems.IndexOf(_anchorItem);
                            SelectRange(visibleItems, anchorIdx < 0 ? newIndex : anchorIdx, newIndex);
                        }
                        else if (shift && _showCheckBox)
                        {
                            RangeCheck(visibleItems, newIndex);
                        }
                        SelectedItem = visibleItems[newIndex];
                        if (!shift) _anchorItem = SelectedItem;
                        EnsureItemVisible(SelectedItem);
                    }
                    e.Handled = true;
                }
                break;

                case Keys.Home:
                    if (shift && (SelectionMode == BeepListBox.SelectionMode.MultiExtended) && visibleItems.Count > 0)
                    {
                        int target = 0;
                        int anchorIdx = visibleItems.IndexOf(_anchorItem);
                        SelectRange(visibleItems, anchorIdx < 0 ? 0 : anchorIdx, target);
                        SelectedItem = visibleItems[target];
                    }
                    else
                    {
                        SelectedItem = visibleItems[0];
                        // Update anchor for non-shift selection
                        _anchorItem = SelectedItem;
                    }
                    e.Handled = true;
                    break;

                case Keys.End:
                    if (shift && (SelectionMode == BeepListBox.SelectionMode.MultiExtended) && visibleItems.Count > 0)
                    {
                        int target = visibleItems.Count - 1;
                        int anchorIdx = visibleItems.IndexOf(_anchorItem);
                        SelectRange(visibleItems, anchorIdx < 0 ? target : anchorIdx, target);
                        SelectedItem = visibleItems[target];
                    }
                    else
                    {
                        SelectedItem = visibleItems[visibleItems.Count - 1];
                        _anchorItem = SelectedItem;
                    }
                    e.Handled = true;
                    break;

                case Keys.Space:
                    if (_showCheckBox && _selectedItem != null)
                    {
                        ToggleItemCheckbox(_selectedItem);
                        e.Handled = true;
                        break;
                    }
                    // Space toggles selection membership if MultiSelect mode is active and there is a selected item
                    if ((SelectionMode == BeepListBox.SelectionMode.MultiExtended) && _selectedItem != null)
                    {
                        ToggleSelection(_selectedItem);
                        e.Handled = true;
                    }
                    break;

                case Keys.Enter:
                    if (_selectedItem != null)
                    {
                        _input?.OnClick();
                        OnItemClicked(_selectedItem);
                        var hostForm = FindForm() as BeepPopupForm;
                        if (hostForm != null)
                        {
                            hostForm.NotifySelectedItemChanged(_selectedItem);
                            hostForm.DialogResult = DialogResult.OK;
                        }
                        e.Handled = true;
                    }
                    break;

                case Keys.A:
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control && (SelectionMode == BeepListBox.SelectionMode.MultiExtended || SelectionMode == BeepListBox.SelectionMode.MultiSimple))
                    {
                        SelectAll();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void MoveSelectionUp(System.Collections.Generic.List<SimpleItem> visibleItems, bool shift)
        {
            if (_selectedItem == null)
            {
                SelectedItem = visibleItems[visibleItems.Count - 1];
            }
            else
            {
                int currentIndex = visibleItems.IndexOf(_selectedItem);
                if (currentIndex > 0)
                {
                    int newIndex = currentIndex - 1;
                    if (shift)
                    {
                        if (SelectionMode == BeepListBox.SelectionMode.MultiExtended)
                        {
                            int anchorIdx = visibleItems.IndexOf(_anchorItem);
                            SelectRange(visibleItems, anchorIdx < 0 ? newIndex : anchorIdx, newIndex);
                        }
                        else if (_showCheckBox)
                        {
                            RangeCheck(visibleItems, newIndex);
                        }
                    }
                    SelectedItem = visibleItems[newIndex];
                    if (!shift)
                    {
                        _anchorItem = SelectedItem;
                    }
                    EnsureItemVisible(SelectedItem);
                }
            }
        }

        private void MoveSelectionDown(System.Collections.Generic.List<SimpleItem> visibleItems, bool shift)
        {
            if (_selectedItem == null)
            {
                SelectedItem = visibleItems[0];
            }
            else
            {
                int currentIndex = visibleItems.IndexOf(_selectedItem);
                if (currentIndex < visibleItems.Count - 1)
                {
                    int newIndex = currentIndex + 1;
                    if (shift)
                    {
                        if (SelectionMode == BeepListBox.SelectionMode.MultiExtended)
                        {
                            int anchorIdx = visibleItems.IndexOf(_anchorItem);
                            SelectRange(visibleItems, anchorIdx < 0 ? newIndex : anchorIdx, newIndex);
                        }
                        else if (_showCheckBox)
                        {
                            RangeCheck(visibleItems, newIndex);
                        }
                    }
                    SelectedItem = visibleItems[newIndex];
                    if (!shift)
                    {
                        _anchorItem = SelectedItem;
                    }
                    EnsureItemVisible(SelectedItem);
                }
            }
        }

        private void RangeCheck(System.Collections.Generic.List<SimpleItem> visibleItems, int newIndex)
        {
            if (visibleItems == null || visibleItems.Count == 0) return;

            int currentIndex = _selectedItem == null ? -1 : visibleItems.IndexOf(_selectedItem);
            if (currentIndex < 0)
            {
                // No current selection - just check newIndex
                SetItemCheckbox(visibleItems[newIndex], true);
                _anchorItem = visibleItems[newIndex];
                return;
            }

            if (_anchorItem == null) _anchorItem = _selectedItem;

            int anchorIdx = visibleItems.IndexOf(_anchorItem);
            if (anchorIdx < 0) anchorIdx = currentIndex;

            int start = Math.Min(anchorIdx, newIndex);
            int end = Math.Max(anchorIdx, newIndex);

            for (int i = start; i <= end; i++)
            {
                if (_showCheckBox)
                {
                    SetItemCheckbox(visibleItems[i], true);
                }
                if (SelectionMode == BeepListBox.SelectionMode.MultiSimple || SelectionMode == BeepListBox.SelectionMode.MultiExtended || MultiSelect)
                {
                    if (!_selectedItems.Contains(visibleItems[i])) _selectedItems.Add(visibleItems[i]);
                }
            }
        }

        private void SelectRange(System.Collections.Generic.List<SimpleItem> visibleItems, int startIndex, int endIndex)
        {
            if (visibleItems == null || visibleItems.Count == 0) return;
            int start = Math.Max(0, Math.Min(startIndex, endIndex));
            int end = Math.Min(visibleItems.Count - 1, Math.Max(startIndex, endIndex));
            bool ctrl = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            if (!ctrl)
            {
                // In regular shift-range selection (without Ctrl), clear existing selection
                ClearSelection();
            }
            if (_showCheckBox)
            {
                for (int i = start; i <= end; i++) SetItemCheckbox(visibleItems[i], true);
            }
            if (SelectionMode == BeepListBox.SelectionMode.MultiSimple || SelectionMode == BeepListBox.SelectionMode.MultiExtended || MultiSelect)
            {
                for (int i = start; i <= end; i++) if (!_selectedItems.Contains(visibleItems[i])) _selectedItems.Add(visibleItems[i]);
            }
            _anchorItem = visibleItems[end];
            RequestDelayedInvalidate();
        }

        #endregion

        #region Mouse Wheel

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            // If we have a vertical scrollbar visible, scroll accordingly
            if (_verticalScrollBar?.Visible == true)
            {
                int delta = -e.Delta / 120 * _verticalScrollBar.SmallChange;
                int newValue = Math.Max(_verticalScrollBar.Minimum,
                                       Math.Min(_verticalScrollBar.Maximum - _verticalScrollBar.LargeChange,
                                               _yOffset + delta));
                if (newValue != _yOffset)
                {
                    _yOffset = newValue;
                    _verticalScrollBar.Value = newValue;
                    try { _layoutHelper?.CalculateLayout(); _hitHelper?.RegisterHitAreas(); } catch { }
                    Invalidate();
                }
            }
            else
            {
                Invalidate();
            }
        }

        #endregion

        #region Resize

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _needsLayoutUpdate = true;
            RequestDelayedInvalidate();
        }

        #endregion
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Event handlers for BeepListBox
    /// </summary>
    public partial class BeepListBox
    {
        #region Mouse Events

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // route to centralized input
            _input?.OnMouseMove(e.Location);

            if (_listBoxPainter == null)
            {
                _listBoxPainter = CreatePainter(_listBoxType);
                _listBoxPainter?.Initialize(this, _currentTheme);
                _needsLayoutUpdate = true;
            }
            if (_listBoxPainter.Style != ControlStyle)
            {
                _listBoxPainter.Style = ControlStyle;
            }
            // Hover over search area
            if (_showSearch && _searchAreaRect.Contains(e.Location))
            {
                Cursor = Cursors.IBeam;
                return;
            }

            // Determine hovered item using layout cache
            var layoutCache = _layout.GetCachedLayout();
            SimpleItem newHoveredItem = null;
            if (layoutCache != null && layoutCache.Count > 0)
            {
                foreach (var info in layoutCache)
                {
                    if (info.RowRect.Contains(e.Location))
                    {
                        newHoveredItem = info.Item;
                        break;
                    }
                }
            }

                if (newHoveredItem != _hoveredItem)
                {
                    // Preserve previous hover progress so we can fade it out
                    _prevHoveredItem = _hoveredItem;
                    _prevHoverProgress = _hoverProgress;
                    _hoveredItem = newHoveredItem;
                    Cursor = _hoveredItem != null ? Cursors.Hand : Cursors.Default;
                    Invalidate();

                    // Start hover animation
                    if (EnableHoverAnimation)
                    {
                        _hoverTarget = _hoveredItem != null;
                        if (!_hoverAnimationTimer.Enabled) _hoverAnimationTimer.Start();
                    }
            }
                else
                {
                    // Ensure animation target is correct even when re-hovering same item
                    if (EnableHoverAnimation && _hoveredItem != null)
                    {
                        _hoverTarget = true;
                        if (!_hoverAnimationTimer.Enabled) _hoverAnimationTimer.Start();
                    }
                }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredItem != null)
            {
                _prevHoveredItem = _hoveredItem;
                _prevHoverProgress = _hoverProgress;
                _hoveredItem = null;
                Cursor = Cursors.Default;
                Invalidate();
                if (EnableHoverAnimation)
                {
                    _hoverTarget = false;
                    if (!_hoverAnimationTimer.Enabled) _hoverAnimationTimer.Start();
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_listBoxPainter == null)
            {
                _listBoxPainter = CreatePainter(_listBoxType);
                _listBoxPainter?.Initialize(this, _currentTheme);
                _needsLayoutUpdate = true;
            }

            // search area click
            if (_showSearch && _searchAreaRect.Contains(e.Location))
            {
                return;
            }

            _input?.OnClick();

            // Centralized hit-test
            if (_hitHelper != null && _hitHelper.HitTest(e.Location, out var hitName, out var hitItem, out var hitRect))
            {
                var parts = hitName.Split('_');
                var kind = parts.Length > 0 ? parts[0] : string.Empty;
                switch (kind)
                {
                    case "check":
                        if (_showCheckBox)
                        {
                            ToggleItemCheckbox(hitItem);
                            return;
                        }
                        break;
                    case "icon":
                    case "text":
                    case "row":
                        HandleItemClick(hitItem, e, hitRect);
                        return;
                }
            }

            // Fallback manual scan using layout cache
            var layoutCache = _layout.GetCachedLayout();
            if (layoutCache != null && layoutCache.Count > 0)
            {
                foreach (var info in layoutCache)
                {
                    if (info.RowRect.Contains(e.Location))
                    {
                        HandleItemClick(info.Item, e, info.RowRect);
                        break;
                    }
                }
            }
        }

        private void HandleItemClick(SimpleItem item, MouseEventArgs e, Rectangle itemRect)
        {
            if (item == null) return;

            // If clicking checkbox area for painters that support it
            if (_showCheckBox && _listBoxPainter.SupportsCheckboxes())
            {
                Rectangle checkboxRect = new Rectangle(
                    itemRect.Left + 8,
                    itemRect.Y + (itemRect.Height - 16) / 2,
                    16,
                    16);

                if (checkboxRect.Contains(e.Location))
                {
                    ToggleItemCheckbox(item);
                    return;
                }
            }

            // Multi-select and modifier-aware click handling
            var modifiers = Control.ModifierKeys;
            bool shift = (modifiers & Keys.Shift) == Keys.Shift;
            bool ctrl = (modifiers & Keys.Control) == Keys.Control;

            if (SelectionMode == BeepListBox.SelectionMode.MultiSimple || SelectionMode == BeepListBox.SelectionMode.MultiExtended)
            {
                if (SelectionMode == BeepListBox.SelectionMode.MultiExtended && shift && _anchorItem != null && _listItems.Count > 0)
                {
                    var visibleItems = _helper.GetVisibleItems();
                    int targetIndex = visibleItems.IndexOf(item);
                    if (targetIndex >= 0)
                    {
                        // Range select
                        int anchorIdxVisible = visibleItems.IndexOf(_anchorItem);
                        SelectRange(visibleItems, anchorIdxVisible, targetIndex);
                    }
                }
                else if (ctrl)
                {
                    // Toggle selection
                    ToggleSelection(item);
                }
                else
                {
                    if (SelectionMode == BeepListBox.SelectionMode.MultiSimple)
                    {
                        // Toggle selection on click (MultiSimple)
                        ToggleSelection(item);
                        SelectedItem = item; // focus
                    }
                    else
                    {
                        // MultiExtended default behavior: clear previous selection and set focus
                        ClearSelection();
                        AddToSelection(item);
                        SelectedItem = item;
                    }
                }
            }
            else
            {
                SelectedItem = item;
            }
            OnItemClicked(item);

            // If hosted in a popup, notify and close
            var hostForm = FindForm() as BeepPopupForm;
            if (hostForm != null)
            {
                hostForm.NotifySelectedItemChanged(item);
                hostForm.DialogResult = DialogResult.OK;
            }
        }

        #endregion

        #region Keyboard Events

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_listItems == null || _listItems.Count == 0) return;

            var visibleItems = _helper.GetVisibleItems();
            if (visibleItems.Count == 0) return;

            bool shift = e.Shift;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveSelectionUp(visibleItems, shift);
                    e.Handled = true;
                    break;

                case Keys.Down:
                    MoveSelectionDown(visibleItems, shift);
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                {
                    if (visibleItems.Count == 0) break;
                    int itemHeight = _listBoxPainter.GetPreferredItemHeight();
                    Rectangle clientArea = GetClientArea();
                    int page = Math.Max(1, clientArea.Height / itemHeight) - 1;
                    int currentIndex = _selectedItem == null ? 0 : visibleItems.IndexOf(_selectedItem);
                    int newIndex = Math.Max(0, currentIndex - page);
                    if (newIndex >= 0 && newIndex < visibleItems.Count)
                    {
                        if (shift && _showCheckBox)
                        {
                            RangeCheck(visibleItems, newIndex);
                        }
                        SelectedItem = visibleItems[newIndex];
                        EnsureItemVisible(SelectedItem);
                    }
                    e.Handled = true;
                }
                break;

                case Keys.PageDown:
                {
                    if (visibleItems.Count == 0) break;
                    int itemHeight = _listBoxPainter.GetPreferredItemHeight();
                    Rectangle clientArea = GetClientArea();
                    int page = Math.Max(1, clientArea.Height / itemHeight) - 1;
                    int currentIndex = _selectedItem == null ? 0 : visibleItems.IndexOf(_selectedItem);
                    int newIndex = Math.Min(visibleItems.Count - 1, currentIndex + page);
                    if (newIndex >= 0 && newIndex < visibleItems.Count)
                    {
                        if (shift && _showCheckBox)
                        {
                            RangeCheck(visibleItems, newIndex);
                        }
                        SelectedItem = visibleItems[newIndex];
                        EnsureItemVisible(SelectedItem);
                    }
                    e.Handled = true;
                }
                break;

                case Keys.Home:
                    if (shift && MultiSelect && visibleItems.Count > 0)
                    {
                        int target = 0;
                        int anchorIdxVisible = visibleItems.IndexOf(_anchorItem);
                        SelectRange(visibleItems, anchorIdxVisible < 0 ? 0 : anchorIdxVisible, target);
                        SelectedItem = visibleItems[target];
                    }
                    else
                    {
                        SelectedItem = visibleItems[0];
                    }
                    e.Handled = true;
                    break;

                case Keys.End:
                    if (shift && MultiSelect && visibleItems.Count > 0)
                    {
                        int target = visibleItems.Count - 1;
                        int anchorIdxVisible2 = visibleItems.IndexOf(_anchorItem);
                        SelectRange(visibleItems, anchorIdxVisible2 < 0 ? target : anchorIdxVisible2, target);
                        SelectedItem = visibleItems[target];
                    }
                    else
                    {
                        SelectedItem = visibleItems[visibleItems.Count - 1];
                    }
                    e.Handled = true;
                    break;

                case Keys.Space:
                    if (_showCheckBox && _selectedItem != null)
                    {
                        ToggleItemCheckbox(_selectedItem);
                        e.Handled = true;
                        break;
                    }
                    // Space toggles selection membership if MultiSelect mode is active and there is a selected item
                    if ((SelectionMode == BeepListBox.SelectionMode.MultiSimple || SelectionMode == BeepListBox.SelectionMode.MultiExtended) && _selectedItem != null)
                    {
                        ToggleSelection(_selectedItem);
                        e.Handled = true;
                    }
                    break;
                    break;

                case Keys.Enter:
                    if (_selectedItem != null)
                    {
                        _input?.OnClick();
                        OnItemClicked(_selectedItem);
                        var hostForm = FindForm() as BeepPopupForm;
                        if (hostForm != null)
                        {
                            hostForm.NotifySelectedItemChanged(_selectedItem);
                            hostForm.DialogResult = DialogResult.OK;

                    case Keys.A:
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control && (SelectionMode == BeepListBox.SelectionMode.MultiExtended || SelectionMode == BeepListBox.SelectionMode.MultiSimple))
                        {
                            SelectAll();
                            e.Handled = true;
                        }
                        break;
                        }
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void MoveSelectionUp(System.Collections.Generic.List<SimpleItem> visibleItems, bool shift)
        {
            if (_selectedItem == null)
            {
                SelectedItem = visibleItems[visibleItems.Count - 1];
            }
            else
            {
                int currentIndex = visibleItems.IndexOf(_selectedItem);
                if (currentIndex > 0)
                {
                    int newIndex = currentIndex - 1;
                    if (shift)
                    {
                        if (SelectionMode == BeepListBox.SelectionMode.MultiExtended)
                        {
                            int anchorIdxVisible3 = visibleItems.IndexOf(_anchorItem);
                            SelectRange(visibleItems, anchorIdxVisible3 < 0 ? newIndex : anchorIdxVisible3, newIndex);
                        }
                        else if (_showCheckBox)
                        {
                            RangeCheck(visibleItems, newIndex);
                        }
                    }
                    SelectedItem = visibleItems[newIndex];
                    EnsureItemVisible(SelectedItem);
                }
            }
        }

        private void MoveSelectionDown(System.Collections.Generic.List<SimpleItem> visibleItems, bool shift)
        {
            if (_selectedItem == null)
            {
                SelectedItem = visibleItems[0];
            }
            else
            {
                int currentIndex = visibleItems.IndexOf(_selectedItem);
                if (currentIndex < visibleItems.Count - 1)
                {
                    int newIndex = currentIndex + 1;
                    if (shift)
                    {
                        if (SelectionMode == BeepListBox.SelectionMode.MultiExtended)
                        {
                            int anchorIdxVisible4 = visibleItems.IndexOf(_anchorItem);
                            SelectRange(visibleItems, anchorIdxVisible4 < 0 ? newIndex : anchorIdxVisible4, newIndex);
                        }
                        else if (_showCheckBox)
                        {
                            RangeCheck(visibleItems, newIndex);
                        }
                    }
                    SelectedItem = visibleItems[newIndex];
                    EnsureItemVisible(SelectedItem);
                }
            }
        }

        private void RangeCheck(System.Collections.Generic.List<SimpleItem> visibleItems, int newIndex)
        {
            if (visibleItems == null || visibleItems.Count == 0) return;

            int currentIndex = _selectedItem == null ? -1 : visibleItems.IndexOf(_selectedItem);
            if (currentIndex < 0)
            {
                // No current selection - just check newIndex
                SetItemCheckbox(visibleItems[newIndex], true);
                _anchorItem = visibleItems[newIndex];
                return;
            }

            if (_anchorItem == null) _anchorItem = visibleItems[currentIndex];
            int anchorIdx = visibleItems.IndexOf(_anchorItem);
            if (anchorIdx < 0) anchorIdx = currentIndex;
            int start = Math.Min(anchorIdx, newIndex);
            int end = Math.Max(anchorIdx, newIndex);

            for (int i = start; i <= end; i++)
            {
                if (_showCheckBox)
                {
                    SetItemCheckbox(visibleItems[i], true);
                }
                if (MultiSelect)
                {
                    if (!_selectedItems.Contains(visibleItems[i])) _selectedItems.Add(visibleItems[i]);
                }
            }
        }

        private void SelectRange(System.Collections.Generic.List<SimpleItem> visibleItems, int startIndex, int endIndex)
        {
            if (visibleItems == null || visibleItems.Count == 0) return;
            int start = Math.Max(0, Math.Min(startIndex, endIndex));
            int end = Math.Min(visibleItems.Count - 1, Math.Max(startIndex, endIndex));
            bool ctrl = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            if (!ctrl && !(_showCheckBox && MultiSelect))
            {
                // In regular shift-range selection (without Ctrl), clear existing selection
                ClearSelection();
            }
            if (_showCheckBox)
            {
                for (int i = start; i <= end; i++) SetItemCheckbox(visibleItems[i], true);
            }
            if (SelectionMode == BeepListBox.SelectionMode.MultiSimple || SelectionMode == BeepListBox.SelectionMode.MultiExtended || MultiSelect)
            {
                for (int i = start; i <= end; i++) if (!_selectedItems.Contains(visibleItems[i])) _selectedItems.Add(visibleItems[i]);
            }
            _anchorItem = visibleItems[end];
            RequestDelayedInvalidate();
        }

        #endregion

        #region Mouse Wheel

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            // If we have a vertical scrollbar visible, scroll accordingly
            if (_verticalScrollBar?.Visible == true)
            {
                int delta = -e.Delta / 120 * _verticalScrollBar.SmallChange;
                int newValue = Math.Max(_verticalScrollBar.Minimum,
                                       Math.Min(_verticalScrollBar.Maximum - _verticalScrollBar.LargeChange,
                                               _yOffset + delta));
                if (newValue != _yOffset)
                {
                    _yOffset = newValue;
                    _verticalScrollBar.Value = newValue;
                    try { _layoutHelper?.CalculateLayout(); _hitHelper?.RegisterHitAreas(); } catch { }
                    Invalidate();
                }
            }
            else
            {
                Invalidate();
            }
        }

        #endregion

        #region Resize

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _needsLayoutUpdate = true;
            RequestDelayedInvalidate();
        }

        #endregion
    }
}
