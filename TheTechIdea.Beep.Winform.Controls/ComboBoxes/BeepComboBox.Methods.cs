using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepComboBox
    {
        #region Public Methods
        
        /// <summary>
        /// Shows the dropdown menu
        /// </summary>
        public void ShowDropdown()
        {
            if (_isDropdownOpen || BeepContextMenu == null || _isLoading)
                return;

            _isDropdownOpen = true;
            TriggerChevronAnimation(true);
            SyncDropdownMetrics();

            // Clear and populate context menu with list items
            BeepContextMenu.ClearItems();

            if (_listItems.Count == 0)
            {
                // ENH-07: show a disabled placeholder row instead of silently doing nothing
                var placeholder = new SimpleItem
                {
                    Text        = EmptyStateText,
                    IsEnabled   = false,
                    IsVisible   = true
                };
                BeepContextMenu.AddItem(placeholder);
            }
            else
            {
                // ENH-18: Select-all / Clear-all row pinned at the top of multi-select lists
                if (AllowMultipleSelection && ShowSelectAll)
                {
                    bool allSelected = _listItems.Count > 0 &&
                                       (_selectedItems?.Count ?? 0) >= _listItems.Count;
                    var selectAllItem = new SimpleItem
                    {
                        Name      = "_selectall",
                        Text      = allSelected ? "Clear all" : "Select all",
                        IsEnabled = true,
                        IsVisible = true
                    };
                    BeepContextMenu.AddItem(selectAllItem);
                    // Add visual separator after the select-all row
                    BeepContextMenu.AddSeparator();
                }

                // ENH-06: Group items by GroupName, inserting disabled header rows
                // ENH-17: IsSeparator items become separator lines
                string lastGroup = null;
                foreach (var item in _listItems)
                {
                    // ENH-17: separator item
                    if (item.IsSeparator)
                    {
                        BeepContextMenu.AddSeparator();
                        continue;
                    }

                    // ENH-06: new group header
                    string group = item.GroupName;
                    if (!string.IsNullOrEmpty(group) && group != lastGroup)
                    {
                        lastGroup = group;
                        var header = new SimpleItem
                        {
                            Name      = "_grp_" + group,
                            Text      = group,
                            IsEnabled = false,
                            IsVisible = true
                        };
                        BeepContextMenu.AddItem(header);
                    }

                    // ENH-16: create a stripped copy when ShowOptionDescription is false
                    SimpleItem addItem = item;
                    if (!ShowOptionDescription && !string.IsNullOrEmpty(item.SubText))
                    {
                        addItem = new SimpleItem
                        {
                            ID          = item.ID,
                            GuidId      = item.GuidId,
                            Text        = item.Text,
                            Name        = item.Name,
                            ImagePath   = item.ImagePath,
                            IsEnabled   = item.IsEnabled,
                            IsVisible   = item.IsVisible,
                            IsCheckable = item.IsCheckable,
                            IsChecked   = item.IsChecked,
                            GroupName   = item.GroupName,
                            SubText     = null   // suppress description line
                        };
                    }
                    BeepContextMenu.AddItem(addItem);
                }
            }

            // Set multi-select behavior based on property
            BeepContextMenu.MultiSelect  = AllowMultipleSelection;
            BeepContextMenu.ShowCheckBox = AllowMultipleSelection;

            // ENH-08: propagate icon visibility setting
            BeepContextMenu.ShowImage = ShowStatusIcons;

            // ENH-13: honour MinDropdownWidth
            BeepContextMenu.MenuWidth = Math.Max(Width, Math.Max(ScaleLogicalX(160), MinDropdownWidth));

            // Multi-select behaviours
            BeepContextMenu.CloseOnItemClick = !AllowMultipleSelection;

            // Ensure searchbox state is up to date
            BeepContextMenu.ShowSearchBox = (ComboBoxType == ComboBoxes.ComboBoxType.SearchableDropdown) || ShowSearchInDropdown;

            // ENH-11: Auto-flip — open upward when not enough screen space below
            Point dropLocation;
            if (AutoFlip)
            {
                System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.FromControl(this).WorkingArea;
                Point below   = PointToScreen(new Point(0, Height));
                int   dropH   = Math.Min(BeepContextMenu.MaxHeight, (_listItems.Count + 1) * BeepContextMenu.MenuItemHeight + 20);
                bool  fitsBelow = (below.Y + dropH) <= workingArea.Bottom;
                dropLocation = fitsBelow
                    ? below
                    : PointToScreen(new Point(0, -dropH));
            }
            else
            {
                dropLocation = PointToScreen(new Point(0, Height));
            }

            // Show the context menu
            BeepContextMenu.Show(dropLocation, this);

            // ENH-12: notify accessibility clients that the control's state changed
            AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.SystemMenuPopupStart, -1);

            PopupOpened?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
        
        /// <summary>
        /// Closes the dropdown menu
        /// </summary>
        public void CloseDropdown()
        {
            if (!_isDropdownOpen || BeepContextMenu == null)
                return;
            
            // Use Hide() directly instead of Close().
            // With DestroyOnClose=false, Close() goes through OnFormClosing
            // which fires MenuClosing, cancels the close, then calls Hide()
            // anyway — causing duplicate events and leaving the parent form
            // in an inconsistent activation state.
            BeepContextMenu.Hide();
            _isDropdownOpen = false;
            TriggerChevronAnimation(false);

            // ENH-12: notify accessibility clients
            AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.SystemMenuPopupEnd, -1);

            PopupClosed?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
        
        /// <summary>
        /// Toggles the dropdown menu
        /// </summary>
        public void ToggleDropdown()
        {
            // Sync state with actual menu visibility. The context menu may have closed
            // itself (e.g. Deactivate when user clicked elsewhere) without raising
            // MenuClosed, leaving _isDropdownOpen stuck at true.
            if (_isDropdownOpen && (BeepContextMenu == null || !BeepContextMenu.Visible))
            {
                _isDropdownOpen = false;
            }

            if (_isDropdownOpen)
            {
                CloseDropdown();
            }
            else
            {
                ShowDropdown();
            }
        }
        
        // Legacy method names for backward compatibility
        public void ShowPopup() => ShowDropdown();
        public void ClosePopup() => CloseDropdown();
        public void TogglePopup() => ToggleDropdown();

        /// <summary>
        /// Clears the current selection and resets the control to empty state.
        /// </summary>
        public void ClearSelection()
        {
            SelectedItem   = null;
            _inputText     = string.Empty;
            Text           = string.Empty;
            Invalidate();
        }

        /// <summary>
        /// Removes a specific item from the multi-select selection set (ENH-19).
        /// Has no effect when the item is not currently selected.
        /// </summary>
        public void DeselectItem(SimpleItem item)
        {
            if (item == null) return;
            var current = SelectedItems ?? new System.Collections.Generic.List<SimpleItem>();
            if (!current.Contains(item)) return;
            var updated = new System.Collections.Generic.List<SimpleItem>(current);
            updated.Remove(item);
            SelectedItems = updated;
        }

        /// <summary>
        /// Starts or reverses the chevron rotation animation.
        /// Pass <c>true</c> when opening (0 → 180°), <c>false</c> when closing (180 → 0°).
        /// </summary>
        private void TriggerChevronAnimation(bool open)
        {
            _chevronAnimTarget = open ? 180f : 0f;

            if (!AnimateChevron || ReduceMotion)
            {
                _chevronAngle = _chevronAnimTarget;
                Invalidate();
                return;
            }

            if (_chevronTimer == null)
            {
                _chevronTimer = new System.Windows.Forms.Timer { Interval = 16 };
                _chevronTimer.Tick += (s, e) =>
                {
                    float delta = ChevronAnimStep;
                    if (_chevronAnimTarget > _chevronAngle)
                    {
                        _chevronAngle = Math.Min(_chevronAngle + delta, _chevronAnimTarget);
                    }
                    else
                    {
                        _chevronAngle = Math.Max(_chevronAngle - delta, _chevronAnimTarget);
                    }
                    Invalidate();
                    if (Math.Abs(_chevronAngle - _chevronAnimTarget) < 0.5f)
                    {
                        _chevronAngle = _chevronAnimTarget;
                        _chevronTimer.Stop();
                    }
                };
            }

            _chevronTimer.Start();
        }
        
        // ─── Inline Editor ───────────────────────────────────────────────────

        // Suppresses InlineEditor_TextChanged while we programmatically set the
        // editor text (e.g. on open). Without this the live-search fires immediately.
        private bool _suppressInlineSearch = false;

        /// <summary>
        /// Creates the BeepTextBox inline editor child control (once, lazily).
        /// Theme and font are applied here — never inside ShowInlineEditor — so
        /// there is no ApplyTheme cascade into the parent on every show.
        /// </summary>
        private void CreateInlineEditor()
        {
            if (_inlineEditor != null) return;

            _inlineEditor = new BeepTextBox
            {
                IsFrameless             = true,
                IsChild                 = true,
                ShowAllBorders          = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                TabStop                 = false,
                Visible                 = false
            };

            // Apply theme/font ONCE at creation — avoids cascade on every show
            if (_currentTheme != null)
            {
                _inlineEditor.BackColor = _currentTheme.TextBoxBackColor;
                _inlineEditor.ForeColor = _currentTheme.TextBoxForeColor;
            }
            // Font is inherited from parent — no explicit Font assignment (BeepButton pattern)

            _inlineEditor.LostFocus   += InlineEditor_LostFocus;
            _inlineEditor.KeyDown     += InlineEditor_KeyDown;
            _inlineEditor.TextChanged += InlineEditor_TextChanged;

            Controls.Add(_inlineEditor);
        }

        private void InlineEditor_LostFocus(object sender, EventArgs e)
        {
            // Small delay so that clicking the dropdown button does not
            // commit before ToggleDropdown() has a chance to run.
            if (!IsDisposed)
                BeginInvoke(new Action(() => HideInlineEditor(true)));
        }

        /// <summary>
        /// Live-search: as the user types, sync _inputText and filter the dropdown.
        /// </summary>
        private void InlineEditor_TextChanged(object sender, EventArgs e)
        {
            // Ignore programmatic text assignments (e.g. when opening the editor)
            if (_suppressInlineSearch || _inlineEditor == null) return;

            string typed = _inlineEditor.Text ?? string.Empty;

            // ENH-22: free-text tokenization — delimiter press creates a chip immediately
            if (AllowFreeText && AllowMultipleSelection && TokenDelimiters != null && typed.Length > 0)
            {
                char last = typed[typed.Length - 1];
                bool isDelim = false;
                foreach (char d in TokenDelimiters)
                    if (last == d) { isDelim = true; break; }

                if (isDelim)
                {
                    string token = typed.TrimEnd(TokenDelimiters).Trim();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var freeItem = new SimpleItem { Text = token, Name = token };
                        var updated = new System.Collections.Generic.List<SimpleItem>(
                            _selectedItems ?? new System.Collections.Generic.List<SimpleItem>());
                        if (!updated.Exists(x => string.Equals(x.Text, token, StringComparison.OrdinalIgnoreCase)))
                            updated.Add(freeItem);
                        SelectedItems = updated;
                    }
                    _suppressInlineSearch = true;
                    _inlineEditor.Text = string.Empty;
                    _suppressInlineSearch = false;
                    _inputText = string.Empty;
                    return;
                }
            }

            // Keep _inputText live so the painter can show it
            _inputText = typed;

            // Try exact/partial match — highlight but don’t commit yet
            if (!string.IsNullOrEmpty(typed))
            {
                // ENH-21: scored fuzzy filter — prefix=100, contains=50, subsequence=10
                var scoredMatches = new System.Collections.Generic.List<(int Score, SimpleItem Item)>();
                foreach (var item in _listItems)
                {
                    if (string.IsNullOrEmpty(item.Text)) continue;
                    int score = FuzzySearchScore(item.Text, typed);
                    if (score > 0)
                        scoredMatches.Add((score, item));
                }
                scoredMatches.Sort((a, b) => b.Score.CompareTo(a.Score));
                var matches = scoredMatches.ConvertAll(t => t.Item);

                if (matches.Count > 0)
                {
                    // Open (or refresh) the dropdown with filtered items
                    if (BeepContextMenu != null)
                    {
                        BeepContextMenu.ClearItems();
                        foreach (var m in matches)
                            BeepContextMenu.AddItem(m);

                        BeepContextMenu.MenuWidth = Math.Max(Width, ScaleLogicalX(160));
                        BeepContextMenu.ShowSearchBox  = false;
                        BeepContextMenu.CloseOnItemClick = true;
                        BeepContextMenu.MultiSelect      = false;

                        if (!_isDropdownOpen)
                        {
                            Point screenLoc = PointToScreen(new Point(0, Height));
                            BeepContextMenu.Show(screenLoc, this);
                            _isDropdownOpen = true;
                            PopupOpened?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
                else if (_isDropdownOpen)
                {
                    // Nothing matched — close the dropdown
                    CloseDropdown();
                }
            }
            else if (_isDropdownOpen)
            {
                // Empty input — close suggestions
                CloseDropdown();
            }

            // Repaint so the text under the editor stays in sync
            Invalidate();
        }

        private void InlineEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    HideInlineEditor(true);
                    break;

                case Keys.Escape:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    HideInlineEditor(false);
                    break;

                case Keys.Down:
                    e.Handled = true;
                    HideInlineEditor(true);
                    ShowDropdown();
                    break;
            }
        }

        /// <summary>
        /// Shows the inline BeepTextBox editor over the text area.
        /// Called when the user clicks the text portion of the combo box.
        /// </summary>
        public void ShowInlineEditor()
        {
            // Close dropdown if open
            if (_isDropdownOpen) CloseDropdown();

            CreateInlineEditor();
            if (_inlineEditor == null) return;

            // Ensure rects are fresh
            UpdateLayout();

            // Position over the text-area rect with enforced minimum size
            // so the editor is always usable even on compact controls.
            var editorBounds = _textAreaRect;
            if (editorBounds.Height < 20)
                editorBounds = new Rectangle(editorBounds.X, editorBounds.Y,
                    editorBounds.Width, Math.Max(20, Height));
            if (editorBounds.Width < 40)
                editorBounds = new Rectangle(editorBounds.X, editorBounds.Y,
                    Math.Max(40, Width / 2), editorBounds.Height);
            _inlineEditor.Bounds = editorBounds;

            // Pre-fill with current displayed text — suppress live-search during this
            _suppressInlineSearch = true;
            try
            {
                // Show what the painter shows: selected item text, else free-typed text
                _inlineEditor.Text = _selectedItem?.Text ?? _inputText;
            }
            finally
            {
                _suppressInlineSearch = false;
            }

            _inlineEditor.BringToFront();
            _inlineEditor.Show();
            _inlineEditor.Focus();

            _isEditing = true;
            Invalidate();
        }

        /// <summary>
        /// Hides the inline editor, optionally committing the typed text.
        /// </summary>
        public void HideInlineEditor(bool commitText)
        {
            if (_inlineEditor == null || !_inlineEditor.Visible) return;

            string typedText = _inlineEditor.Text ?? string.Empty;

            // Hide first so LostFocus does not re-enter
            _inlineEditor.Hide();
            _isEditing = false;

            if (commitText)
            {
                // Try exact/partial match against the list
                var match = _helper?.FindItemByText(typedText);

                if (match != null)
                {
                    // Matched a list item:
                    // - clear free-text so painter uses SelectedItem.Text
                    // - go through the property setter (updates index, Text, raises events)
                    _inputText = string.Empty;
                    SelectedItem = match;
                }
                else
                {
                    // Free-text — clear selection, keep typed text as display value
                    // Use property setter so index and events are properly reset
                    SelectedItem    = null;
                    _inputText      = typedText;
                    // Do NOT call Text = ... here; the painter reads _inputText directly
                }

                if (_isDropdownOpen) CloseDropdown();
            }
            else
            {
                // Escape — discard; restore display from whatever was selected before
                _inputText = string.Empty;   // painter will fall back to SelectedItem.Text
                if (_isDropdownOpen) CloseDropdown();
            }

            Focus();
            Invalidate();
        }

        /// <summary>
        /// Starts editing mode — now delegates to the inline editor.
        /// </summary>
        public void StartEditing()
        {
            ShowInlineEditor();
        }

        /// <summary>
        /// Stops editing mode — hides the inline editor and commits text.
        /// </summary>
        public void StopEditing()
        {
            HideInlineEditor(true);
        }
        
        /// <summary>
        /// Clears the selection
        /// </summary>
        public void Clear()
        {
            SelectedItem = null;
        }
        
        /// <summary>
        /// Resets the combo box to default state
        /// </summary>
        public void Reset()
        {
            SelectedItem = null;
            _inputText = string.Empty;
            HasError = false;
            ErrorText = string.Empty;
            Invalidate();
        }
        
        /// <summary>
        /// Selects an item by its text value
        /// </summary>
        public void SelectItemByText(string text)
        {
            if (string.IsNullOrEmpty(text) || _listItems.Count == 0)
                return;
            
            foreach (var item in _listItems)
            {
                if (string.Equals(item.Text, text, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedItem = item;
                    return;
                }
            }
        }
        
        /// <summary>
        /// Selects an item by its value
        /// </summary>
        public void SelectItemByValue(object value)
        {
            if (value == null || _listItems.Count == 0)
                return;
            
            foreach (var item in _listItems)
            {
                if (Equals(item.Item, value))
                {
                    SelectedItem = item;
                    return;
                }
            }
        }
        
        /// <summary>
        /// Gets the calculated text area rectangle
        /// </summary>
        public Rectangle GetTextAreaRect() => _textAreaRect;
        
        /// <summary>
        /// Gets the calculated dropdown button rectangle
        /// </summary>
        public Rectangle GetDropdownButtonRect() => _dropdownButtonRect;
        
        /// <summary>
        /// Gets the calculated image rectangle
        /// </summary>
        public Rectangle GetImageRect() => _imageRect;
        
        #endregion
        
        #region Internal Helper Methods
        
        internal void InvalidateComboBox()
        {
            Invalidate();
        }
        
        internal void UpdateLayoutAndInvalidate()
        {
            InvalidateLayout();
        }

        /// <summary>
        /// ENH-21: Scored fuzzy match used by the inline search filter.
        /// Returns 100 for a prefix match, 50 for a contains match,
        /// 10 for a subsequence (all characters of <paramref name="query"/> appear
        /// in order inside <paramref name="source"/>), or 0 for no match.
        /// </summary>
        private static int FuzzySearchScore(string source, string query)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(query)) return 0;

            // Prefix match
            if (source.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                return 100;

            // Contains match
            if (source.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                return 50;

            // Subsequence match
            int si = 0, qi = 0;
            string srcL = source.ToLower();
            string qryL = query.ToLower();
            while (si < srcL.Length && qi < qryL.Length)
            {
                if (srcL[si] == qryL[qi]) qi++;
                si++;
            }
            return qi == qryL.Length ? 10 : 0;
        }

        #endregion
        
        #region Override Methods
        
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme != null)
            {
                if (_currentTheme.ComboBoxBackColor != Color.Empty) BackColor = _currentTheme.ComboBoxBackColor;
                if (_currentTheme.ComboBoxForeColor != Color.Empty) ForeColor = _currentTheme.ComboBoxForeColor;
                if (_currentTheme.ComboBoxBorderColor != Color.Empty) BorderColor = _currentTheme.ComboBoxBorderColor;

                if (_currentTheme.ComboBoxHoverBackColor != Color.Empty) HoverBackColor = _currentTheme.ComboBoxHoverBackColor;
                if (_currentTheme.ComboBoxHoverForeColor != Color.Empty) HoverForeColor = _currentTheme.ComboBoxHoverForeColor;
                if (_currentTheme.ComboBoxHoverBorderColor != Color.Empty) HoverBorderColor = _currentTheme.ComboBoxHoverBorderColor;

                if (_currentTheme.ComboBoxSelectedBackColor != Color.Empty) SelectedBackColor = _currentTheme.ComboBoxSelectedBackColor;
                if (_currentTheme.ComboBoxSelectedForeColor != Color.Empty) SelectedForeColor = _currentTheme.ComboBoxSelectedForeColor;
                if (_currentTheme.ComboBoxSelectedBorderColor != Color.Empty) SelectedBorderColor = _currentTheme.ComboBoxSelectedBorderColor;

                // Keep inline editor colours in sync (but do NOT assign Font)
                if (_inlineEditor != null)
                {
                    _inlineEditor.BackColor = _currentTheme.TextBoxBackColor;
                    _inlineEditor.ForeColor = _currentTheme.TextBoxForeColor;
                }
            }

            // Only touch the private _textFont field — never assign control.Font
            // Use BeepThemesManager.ToFont() — preferred source per SKILL Rule 2.4
            if (UseThemeFont)
            {
                TypographyStyle comboStyle = _currentTheme?.ComboBoxItemFont;
                if (comboStyle != null)
                {
                    _textFont = BeepThemesManager.ToFont(comboStyle) ?? SystemFonts.DefaultFont;
                }
            }

            SyncDropdownMetrics();
            InvalidateOnce(); // batched — SKILL Rule 2.6
        }

        private void SyncDropdownMetrics()
        {
            if (BeepContextMenu == null)
                return;

            BeepContextMenu.Theme = Theme;

            // Never read Control.Font (this.Font) — use TextFont resolved from theme, then safe default
            Font dropdownFont = TextFont ?? SystemFonts.DefaultFont;
            if (UseThemeFont && _currentTheme?.ComboBoxListFont != null)
            {
                dropdownFont = BeepThemesManager.ToFont(_currentTheme.ComboBoxListFont) ?? SystemFonts.DefaultFont;
            }
            if (BeepContextMenu.TextFont != dropdownFont)
            {
                BeepContextMenu.TextFont = dropdownFont;
            }
            if (BeepContextMenu.ShortcutFont != dropdownFont)
            {
                BeepContextMenu.ShortcutFont = dropdownFont;
            }

            int textHeight;
            try
            {
                textHeight = TextRenderer.MeasureText(
                    "Ag",
                    dropdownFont,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding).Height;
            }
            catch
            {
                textHeight = Math.Max(16, (int)Math.Ceiling(dropdownFont.Size * 1.35f));
            }

            int targetItemHeight = Math.Max(28, textHeight + 10);
            if (BeepContextMenu.MenuItemHeight != targetItemHeight)
            {
                BeepContextMenu.MenuItemHeight = targetItemHeight;
            }

            int targetImageSize = Math.Max(16, Math.Min(textHeight, 24));
            if (BeepContextMenu.ImageSize != targetImageSize)
            {
                BeepContextMenu.ImageSize = targetImageSize;
            }

            int targetMaxHeight = Math.Max(140, MaxDropdownHeight);
            if (BeepContextMenu.MaxHeight != targetMaxHeight)
            {
                BeepContextMenu.MaxHeight = targetMaxHeight;
            }
        }
         
        #endregion
    }
}
