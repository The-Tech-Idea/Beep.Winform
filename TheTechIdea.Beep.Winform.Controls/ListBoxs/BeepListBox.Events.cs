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

            // Determine hovered item
            var visibleItems = _helper.GetVisibleItems();
            SimpleItem newHoveredItem = null;

            if (!_contentAreaRect.IsEmpty)
            {
                int itemHeight = _listBoxPainter.GetPreferredItemHeight();
                int currentY = _contentAreaRect.Top;

                foreach (var item in visibleItems)
                {
                    Rectangle itemRect = new Rectangle(
                        _contentAreaRect.Left,
                        currentY,
                        _contentAreaRect.Width,
                        itemHeight);

                    if (itemRect.Contains(e.Location))
                    {
                        newHoveredItem = item;
                        break;
                    }

                    currentY += itemHeight;
                    if (currentY >= _contentAreaRect.Bottom) break;
                }
            }

            if (newHoveredItem != _hoveredItem)
            {
                _hoveredItem = newHoveredItem;
                Cursor = _hoveredItem != null ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredItem != null)
            {
                _hoveredItem = null;
                Cursor = Cursors.Default;
                Invalidate();
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

            // Fallback manual scan
            var visibleItems = _helper.GetVisibleItems();
            if (!_contentAreaRect.IsEmpty && visibleItems.Count > 0)
            {
                int itemHeight = _listBoxPainter.GetPreferredItemHeight();
                int currentY = _contentAreaRect.Top;
                foreach (var item in visibleItems)
                {
                    var itemRect = new Rectangle(_contentAreaRect.Left, currentY, _contentAreaRect.Width, itemHeight);
                    if (itemRect.Contains(e.Location))
                    {
                        HandleItemClick(item, e, itemRect);
                        break;
                    }
                    currentY += itemHeight;
                    if (currentY >= _contentAreaRect.Bottom) break;
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

            SelectedItem = item;
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

            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveSelectionUp(visibleItems);
                    e.Handled = true;
                    break;

                case Keys.Down:
                    MoveSelectionDown(visibleItems);
                    e.Handled = true;
                    break;

                case Keys.Home:
                    SelectedItem = visibleItems[0];
                    e.Handled = true;
                    break;

                case Keys.End:
                    SelectedItem = visibleItems[visibleItems.Count - 1];
                    e.Handled = true;
                    break;

                case Keys.Space:
                    if (_showCheckBox && _selectedItem != null)
                    {
                        ToggleItemCheckbox(_selectedItem);
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
            }
        }

        private void MoveSelectionUp(System.Collections.Generic.List<SimpleItem> visibleItems)
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
                    SelectedItem = visibleItems[currentIndex - 1];
                }
            }
        }

        private void MoveSelectionDown(System.Collections.Generic.List<SimpleItem> visibleItems)
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
                    SelectedItem = visibleItems[currentIndex + 1];
                }
            }
        }

        #endregion

        #region Mouse Wheel

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Invalidate();
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
