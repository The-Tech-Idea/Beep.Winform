using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepComboBox
    {
        #region Mouse Events
        
        protected override void OnMouseEnter(EventArgs e)
        {
            if (DesignMode) return;
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }
        
        protected override void OnMouseLeave(EventArgs e)
        {
            if (DesignMode) return;
            base.OnMouseLeave(e);
            _isHovered = false;
            _isButtonHovered = false;
            _clearButtonHovered = false;
            // ENH-15: hide overflow tooltip when cursor leaves the control
            _overflowTooltip?.SetToolTip(this, string.Empty);
            Invalidate();
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (DesignMode) return;
            base.OnMouseMove(e);
            
            // Check if hovering over dropdown button
            bool wasButtonHovered    = _isButtonHovered;
            bool wasClearHovered     = _clearButtonHovered;
            _isButtonHovered         = _dropdownButtonRect.Contains(e.Location);
            _clearButtonHovered      = !_clearButtonRect.IsEmpty && _clearButtonRect.Contains(e.Location);
            bool chipCloseHovered    = false;
            if (ChipCloseRects.Count > 0)
            {
                foreach (var rect in ChipCloseRects.Values)
                {
                    if (!rect.IsEmpty && rect.Contains(e.Location))
                    {
                        chipCloseHovered = true;
                        break;
                    }
                }
            }
            
            Cursor targetCursor = (_isButtonHovered || _clearButtonHovered || chipCloseHovered) ? Cursors.Hand : Cursors.Default;
            if (wasButtonHovered != _isButtonHovered || wasClearHovered != _clearButtonHovered || Cursor != targetCursor)
            {
                Cursor = targetCursor;
                Invalidate();
            }

            // ENH-15: show tooltip when display text overflows the text area
            if (_overflowTooltip != null && !_textAreaRect.IsEmpty && _textAreaRect.Contains(e.Location))
            {
                string displayText = _selectedItem?.Text ?? _inputText ?? Text ?? string.Empty;
                if (!string.IsNullOrEmpty(displayText))
                {
                    var measured = System.Windows.Forms.TextRenderer.MeasureText(
                        displayText, TextFont ?? Font);
                    string tip = measured.Width > _textAreaRect.Width ? displayText : string.Empty;
                    if (_overflowTooltip.GetToolTip(this) != tip)
                        _overflowTooltip.SetToolTip(this, tip);
                }
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Always call base so the designer can handle its own selection/focus logic.
            // Skip ALL our custom combo-box hit-testing at design time to prevent
            // ToggleDropdown / ShowInlineEditor from firing and causing flicker.
            if (DesignMode)
            {
                base.OnMouseDown(e);
                return;
            }
            if (_isLoading) return; // Disable interaction when loading

            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left) return;

            // Always recalculate layout before hit-testing on a mouse-down.
            // Focus-gain fires synchronously inside base.OnMouseDown above and can
            // shift DrawingRect (e.g. painter widens focused border), so we must
            // refresh the rects unconditionally to avoid stale hit areas.
            UpdateLayout();
            _needsLayoutUpdate = false;

            // ── Clear button (×) ────────────────────────────────────────────
            // Must be tested before text-area so it doesn't accidentally open the editor.
            if (!_clearButtonRect.IsEmpty && _clearButtonRect.Contains(e.Location))
            {
                HideInlineEditor(false);
                ClearSelection();
                return;
            }

            // ENH-19: chip × close buttons — remove clicked chip from selection
            if (ChipCloseRects.Count > 0)
            {
                foreach (var kvp in ChipCloseRects)
                {
                    if (!kvp.Value.IsEmpty && kvp.Value.Contains(e.Location))
                    {
                        var itemToRemove = _selectedItems?.Find(it =>
                            string.Equals(GetSimpleItemIdentity(it), kvp.Key, StringComparison.OrdinalIgnoreCase));
                        if (itemToRemove != null)
                        {
                            HideInlineEditor(false);
                            DeselectItem(itemToRemove);
                        }
                        return;
                    }
                }
            }

            // ENH-19b: chip body click — open popup and scroll to that item
            if (ChipBodyRects.Count > 0)
            {
                foreach (var kvp in ChipBodyRects)
                {
                    if (!kvp.Value.IsEmpty && kvp.Value.Contains(e.Location))
                    {
                        var itemToFocus = _selectedItems?.Find(it =>
                            string.Equals(GetSimpleItemIdentity(it), kvp.Key, StringComparison.OrdinalIgnoreCase))
                            ?? _listItems?.FirstOrDefault(it =>
                            string.Equals(GetSimpleItemIdentity(it), kvp.Key, StringComparison.OrdinalIgnoreCase));
                        if (itemToFocus != null)
                        {
                            HideInlineEditor(false);
                            ShowDropdown();
                            // After the popup is built, ask the host to focus the row for this item
                            _popupHost?.FocusItem(itemToFocus);
                        }
                        return;
                    }
                }
            }

            // ── Dropdown button (▲) ─────────────────────────────────────────
            // Only this zone opens/closes the dropdown list.
            if (!_dropdownButtonRect.IsEmpty && _dropdownButtonRect.Contains(e.Location))
            {
                HideInlineEditor(true);   // commit any open editor first
                ToggleDropdown();
                return;
            }

            // ── Text area ───────────────────────────────────────────────────
            // When NOT editable: clicking text area toggles the dropdown
            //   (standard combobox UX — click anywhere to open).
            // When editable: clicking text area shows the inline editor
            //   so the user can type a value.
            if (!_textAreaRect.IsEmpty && _textAreaRect.Contains(e.Location))
            {
                if (IsInlineEditorAllowed())
                {
                    ShowInlineEditor();
                }
                else
                {
                    ToggleDropdown();
                }
                return;
            }

            // ── Fallback: click anywhere else inside the control ─────────────
            // Treat it like clicking the dropdown button for accessibility.
            if (!IsInlineEditorAllowed())
            {
                ToggleDropdown();
            }
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (DesignMode) return;
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
                    if (!IsInlineEditorAllowed() && !_isDropdownOpen)
                    {
                        ShowDropdown();
                        e.Handled = true;
                    }
                    break;

                // ENH-09: Backspace removes the last chip in multi-select mode
                case Keys.Back:
                case Keys.Delete:
                    if (AllowMultipleSelection && _selectedItems.Count > 0 && !IsInlineEditorAllowed())
                    {
                        var last = _selectedItems[_selectedItems.Count - 1];
                        var updated = new System.Collections.Generic.List<SimpleItem>(_selectedItems);
                        updated.Remove(last);
                        SelectedItems = updated;
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
            
            if (IsInlineEditorAllowed() && _isEditing)
            {
                // The inline BeepTextBox handles its own text input and fires
                // InlineEditor_TextChanged — nothing extra needed here.
                return;
            }
            else if (!IsInlineEditorAllowed())
            {
                HandleSelectOnlyTypeAhead(e.KeyChar);
            }
        }
        
        #endregion
        
        #region Resize Event
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InvalidateLayout();

            // If inline editor is visible, reposition it — or hide if current type forbids it
            if (_inlineEditor != null && _inlineEditor.Visible)
            {
                if (!IsInlineEditorAllowed())
                {
                    HideInlineEditor(false);
                }
                else
                {
                    UpdateLayout();
                    _inlineEditor.Bounds = _textAreaRect;
                }
            }
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
