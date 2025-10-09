using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepComboBox
    {
        #region Mouse Events
        
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }
        
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            _isButtonHovered = false;
            Invalidate();
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Check if hovering over dropdown button
            bool wasButtonHovered = _isButtonHovered;
            _isButtonHovered = _dropdownButtonRect.Contains(e.Location);
            
            if (wasButtonHovered != _isButtonHovered)
            {
                Cursor = _isButtonHovered ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            // Hit testing is handled by BaseControl's hit test system
            // and the AddHitArea registered in the painter
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            
            // Scroll through items if dropdown is closed
            if (!_isDropdownOpen && _listItems.Count > 0)
            {
                int newIndex = _selectedItemIndex + (e.Delta > 0 ? -1 : 1);
                newIndex = Math.Max(0, Math.Min(newIndex, _listItems.Count - 1));
                
                if (newIndex != _selectedItemIndex)
                {
                    SelectedIndex = newIndex;
                }
            }
        }
        
        #endregion
        
        #region Keyboard Events
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (_isDropdownOpen)
                    {
                        // Context menu handles it
                    }
                    else if (_selectedItemIndex > 0)
                    {
                        SelectedIndex = _selectedItemIndex - 1;
                    }
                    e.Handled = true;
                    break;
                    
                case Keys.Down:
                    if (!_isDropdownOpen)
                    {
                        if (_selectedItemIndex < _listItems.Count - 1)
                        {
                            SelectedIndex = _selectedItemIndex + 1;
                        }
                        else
                        {
                            ShowDropdown();
                        }
                    }
                    e.Handled = true;
                    break;
                    
                case Keys.Enter:
                    if (_isDropdownOpen)
                    {
                        CloseDropdown();
                    }
                    else
                    {
                        ShowDropdown();
                    }
                    e.Handled = true;
                    break;
                    
                case Keys.Escape:
                    if (_isDropdownOpen)
                    {
                        CloseDropdown();
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.Space:
                    if (!IsEditable && !_isDropdownOpen)
                    {
                        ShowDropdown();
                        e.Handled = true;
                    }
                    break;
            }
        }
        
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            
            if (IsEditable && _isEditing)
            {
                // Handle text input for editable mode
                _inputText += e.KeyChar;
                Text = _inputText;
                
                // Trigger auto-complete if enabled
                if (AutoComplete)
                {
                    TriggerAutoComplete();
                }
                
                Invalidate();
            }
        }
        
        #endregion
        
        #region Resize Event
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InvalidateLayout();
        }
        
        #endregion
        
        #region Helper Methods for Events
        
        private void TriggerAutoComplete()
        {
            if (string.IsNullOrEmpty(_inputText) || _listItems.Count == 0)
                return;
            
            // Find first matching item
            foreach (var item in _listItems)
            {
                if (item.Text.StartsWith(_inputText, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedItem = item;
                    break;
                }
            }
        }
        
        #endregion
    }
}
