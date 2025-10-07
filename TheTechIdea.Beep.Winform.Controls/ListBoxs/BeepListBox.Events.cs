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
            
            if (_listBoxPainter == null) return;
            
            // Check if hovering over search area
            if (_showSearch && _searchAreaRect.Contains(e.Location))
            {
                Cursor = Cursors.IBeam;
                return;
            }
            
            // Check which item we're hovering over
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
            
            if (_listBoxPainter == null) return;
            
            // Check if clicking in search area
            if (_showSearch && _searchAreaRect.Contains(e.Location))
            {
                // TODO: Handle search area click (show text input)
                return;
            }
            
            // Check which item was clicked
            var visibleItems = _helper.GetVisibleItems();
            
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
            // Check if clicking on checkbox
            if (_showCheckBox && _listBoxPainter.SupportsCheckboxes())
            {
                // Checkbox is typically on the left side
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
            
            // Regular item selection
            SelectedItem = item;
            OnItemClicked(item);
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
                        OnItemClicked(_selectedItem);
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
            
            // TODO: Implement scrolling support
            // For now, just invalidate to show updated state
            Invalidate();
        }
        
        #endregion
        
        #region Theme Changed
      
        
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
