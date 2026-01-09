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
            if (_isLoading) return; // Disable interaction when loading
            
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
            if (!Enabled) 
            {
                base.OnKeyDown(e);
                return;
            }

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
                    
                case Keys.Home:
                    if (!_isDropdownOpen && _listItems.Count > 0)
                    {
                        SelectedIndex = 0;
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.End:
                    if (!_isDropdownOpen && _listItems.Count > 0)
                    {
                        SelectedIndex = _listItems.Count - 1;
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.PageUp:
                    if (!_isDropdownOpen && _listItems.Count > 0)
                    {
                        int pageSize = Math.Max(1, _listItems.Count / 10); // 10% of list
                        int newIndex = Math.Max(0, _selectedItemIndex - pageSize);
                        SelectedIndex = newIndex;
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.PageDown:
                    if (!_isDropdownOpen && _listItems.Count > 0)
                    {
                        int pageSize = Math.Max(1, _listItems.Count / 10); // 10% of list
                        int newIndex = Math.Min(_listItems.Count - 1, _selectedItemIndex + pageSize);
                        SelectedIndex = newIndex;
                        e.Handled = true;
                    }
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
            
            base.OnKeyDown(e);
        }
        
        /// <summary>
        /// Handles dialog keys (Enter, Space, Tab, Arrow keys) for better dialog integration
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (!Enabled) return base.ProcessDialogKey(keyData);

            switch (keyData)
            {
                case Keys.Enter:
                case Keys.Space:
                    if (Focused || TabStop)
                    {
                        if (_isDropdownOpen)
                        {
                            CloseDropdown();
                        }
                        else
                        {
                            ShowDropdown();
                        }
                        return true;
                    }
                    break;
                case Keys.Down:
                    if (!_isDropdownOpen && _listItems.Count > 0)
                    {
                        if (_selectedItemIndex < _listItems.Count - 1)
                        {
                            SelectedIndex = _selectedItemIndex + 1;
                        }
                        else
                        {
                            ShowDropdown();
                        }
                        return true;
                    }
                    break;
                case Keys.Up:
                    if (!_isDropdownOpen && _selectedItemIndex > 0)
                    {
                        SelectedIndex = _selectedItemIndex - 1;
                        return true;
                    }
                    break;
                case Keys.Escape:
                    if (_isDropdownOpen)
                    {
                        CloseDropdown();
                        return true;
                    }
                    break;
            }

            return base.ProcessDialogKey(keyData);
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
            if (string.IsNullOrEmpty(_inputText) || _listItems.Count == 0 || !AutoComplete)
                return;
            
            // Check minimum length
            if (_inputText.Length < AutoCompleteMinLength)
                return;
            
            // Apply delay if configured
            if (AutoCompleteDelay > 0)
            {
                if (_autoCompleteDelayTimer == null)
                {
                    _autoCompleteDelayTimer = new Timer { Interval = AutoCompleteDelay };
                    _autoCompleteDelayTimer.Tick += (s, e) =>
                    {
                        _autoCompleteDelayTimer.Stop();
                        PerformAutoComplete();
                    };
                }
                _autoCompleteDelayTimer.Stop();
                _autoCompleteDelayTimer.Start();
                return;
            }
            
            PerformAutoComplete();
        }
        
        private void PerformAutoComplete()
        {
            if (string.IsNullOrEmpty(_inputText) || _listItems.Count == 0)
                return;
            
            var matches = FindAutoCompleteMatches(_inputText, AutoCompleteMode, MaxSuggestions);
            
            if (matches.Count > 0)
            {
                // Select first match
                SelectedItem = matches[0];
            }
        }
        
        /// <summary>
        /// Finds matching items based on auto-complete mode
        /// </summary>
        private System.Collections.Generic.List<SimpleItem> FindAutoCompleteMatches(string input, BeepAutoCompleteMode mode, int maxResults)
        {
            var results = new System.Collections.Generic.List<SimpleItem>();
            if (string.IsNullOrEmpty(input) || _listItems.Count == 0)
                return results;
            
            string searchText = input.ToLowerInvariant();
            
            foreach (var item in _listItems)
            {
                if (results.Count >= maxResults) break;
                
                if (string.IsNullOrEmpty(item.Text)) continue;
                
                string itemText = item.Text.ToLowerInvariant();
                bool matches = false;
                
                switch (mode)
                {
                    case BeepAutoCompleteMode.Prefix:
                        matches = itemText.StartsWith(searchText, StringComparison.OrdinalIgnoreCase);
                        break;
                        
                    case BeepAutoCompleteMode.Fuzzy:
                        matches = FuzzyMatch(itemText, searchText);
                        break;
                        
                    case BeepAutoCompleteMode.Full:
                        matches = itemText.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                        break;
                        
                    case BeepAutoCompleteMode.None:
                    default:
                        return results;
                }
                
                if (matches)
                {
                    results.Add(item);
                }
            }
            
            return results;
        }
        
        /// <summary>
        /// Simple fuzzy matching - checks if all characters in search text appear in order in item text
        /// </summary>
        private bool FuzzyMatch(string itemText, string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return true;
            if (string.IsNullOrEmpty(itemText)) return false;
            
            int searchIndex = 0;
            foreach (char c in itemText)
            {
                if (searchIndex < searchText.Length && char.ToLowerInvariant(c) == searchText[searchIndex])
                {
                    searchIndex++;
                    if (searchIndex >= searchText.Length)
                        return true;
                }
            }
            
            return searchIndex >= searchText.Length;
        }
        
        #endregion
    }
}
