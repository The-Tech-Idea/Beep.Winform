using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.ContextMenus;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepComboBox
    {
        private bool _popupRefreshQueued;
        private bool _multiSelectCommitQueued;
        private System.Collections.Generic.List<SimpleItem> _pendingPopupSelection;

        #region Public Methods

        public override void SetValue(object value)
        {
            SelectedValue = value;
        }

        public override object GetValue()
        {
            return SelectedValue;
        }
        
        /// <summary>
        /// Shows the dropdown menu. Single-select path routes through
        /// <see cref="ContextMenuManager.ShowNonBlocking"/> so the chrome matches
        /// <see cref="TheTechIdea.Beep.Winform.Controls.Menus.BeepMenuBar"/>. The
        /// manager's non-blocking variant does not yet support multi-select, so
        /// the multi-select path keeps the direct <see cref="BeepContextMenu"/>
        /// instantiation. An empty list is a no-op in either path.
        /// </summary>
        public void ShowDropdown()
        {
            if (_isDropdownOpen || _isLoading)
                return;
            // Don't popup an empty list -- nothing for the user to pick.
            if (_listItems == null || _listItems.Count == 0)
                return;

            _isDropdownOpen = true;
            _popupSearchText = string.Empty;
            TriggerChevronAnimation(true);

            if (AllowMultipleSelection && UseApplyCancelFooter)
            {
                _popupSelectionSnapshot = new System.Collections.Generic.List<SimpleItem>(
                    SelectedItems ?? new System.Collections.Generic.List<SimpleItem>());
            }

            Point screenLocation = this.PointToScreen(new Point(0, Height));

            if (!AllowMultipleSelection)
            {
                // Single-select: route through the central menu manager so the chrome
                // is identical to BeepMenuBar. ShowNonBlocking already guards on
                // empty items and handles DestroyOnClose. The ComboBox-specific
                // overrides (ShowSearchBox, EnableAnimations, etc.) do not apply
                // because the manager's CreateMenu is in control of those flags.
                var snapshot = _listItems.ToList();
                var previous = _managerPopupHandle;
                if (previous != null) { try { previous.Dispose(); } catch { } }
                _managerPopupHandle = ContextMenuManager.ShowNonBlocking(
                    items: snapshot,
                    screenLocation: screenLocation,
                    owner: this,
                    style: FormStyle.Modern,
                    theme: this.Theme,
                    onItemSelected: OnManagerItemSelected,
                    maxImageSize: MaxImageSize);

                // Access + life-cycle events
                AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.StateChange, -1);
                AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.SystemMenuPopupStart, -1);
                PopupOpened?.Invoke(this, EventArgs.Empty);
                Invalidate();
                return;
            }

            // Multi-select path: keep the manual BeepContextMenu path because
            // ContextMenuManager.ShowNonBlocking does not support multi-select.
            // Dispose old menu if exists (like Menu does - always fresh instance)
            if (BeepContextMenu != null && !BeepContextMenu.IsDisposed)
            {
                BeepContextMenu.ItemClicked -= OnContextMenuItemClicked;
                BeepContextMenu.MenuClosed -= OnDropdownMenuClosed;
                BeepContextMenu.Dispose();
            }

            // Create NEW menu instance (exactly like ContextMenuManager.CreateMenu)
            BeepContextMenu = new BeepContextMenu
            {
                ContextMenuType = FormStyle.Modern,
                DestroyOnClose = true,  // Auto-dispose when closed (same as Menu)
                MultiSelect = AllowMultipleSelection,
                ShowCheckBox = AllowMultipleSelection,
                // Always show the search box in the dropdown, regardless
                // of ComboBoxType.  The user expects to be able to type
                // and filter any list — limiting it to only certain
                // types was a UX regression.  ShowSearchInDropdown is
                // kept as a "force display" override but the default
                // for all types is now true.
                ShowSearchBox = true,
                ShowImage = true,
                ShowSeparators = false,
                StartPosition = FormStartPosition.Manual,
                CloseOnFocusLost = true,
                EnableAnimations = false,  // No fade animation for dropdown (instant show)
                Theme = this.Theme,
                // Match the leading-image cap so the popup icons resize with
                // the host control exactly the way the leading icon does.
                MaxImageSize = this.MaxImageSize
            };

            // Wire events fresh
            BeepContextMenu.ItemClicked += OnContextMenuItemClicked;
            BeepContextMenu.MenuClosed += OnDropdownMenuClosed;

            // Populate items
            foreach (var item in _listItems)
            {
                if (AllowMultipleSelection)
                {
                    item.IsChecked = SelectedItems?.Exists(x => IsSameSimpleItem(x, item)) ?? false;
                }
                BeepContextMenu.MenuItems.Add(item);
            }

            // Position below the combo box
            BeepContextMenu.Width = Width;
            BeepContextMenu.RecalculateSize();
            BeepContextMenu.Show(screenLocation, this);

            AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.SystemMenuPopupStart, -1);

            PopupOpened?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Bridge between the manager-driven single-select popup and the existing
        /// <see cref="OnContextMenuItemClicked"/> handler. The manager invokes
        /// this on every item click; we translate the <see cref="SimpleItem"/>
        /// payload into the <see cref="MenuItemEventArgs"/> the existing handler
        /// already understands, so all selection logic stays in one place.
        /// </summary>
        private void OnManagerItemSelected(SimpleItem item)
        {
            if (item == null) return;
            OnContextMenuItemClicked(this, new TheTechIdea.Beep.Winform.Controls.ContextMenus.MenuItemEventArgs(item));
        }

        private void OnDropdownMenuClosed(object sender, BeepContextMenuClosedEventArgs e)
        {
            _isDropdownOpen = false;
            // Clear the type-ahead buffer so a fresh search starts on
            // the next keypress after the dropdown closes.
            _typeAheadBuffer = string.Empty;
            TriggerChevronAnimation(false);
            PopupClosed?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Called when <see cref="ContextMenuManager"/> raises <see cref="ContextMenuManager.MenuDismissed"/>
        /// for a menu whose owner is this combo. The single-select path goes through
        /// <see cref="ContextMenuManager.ShowNonBlocking"/> which does not give us a direct
        /// <see cref="BeepContextMenu.MenuClosed"/> event; the central dismissal event fills
        /// that gap and lets us share <see cref="OnDropdownMenuClosed"/>'s cleanup.
        /// </summary>
        private void OnManagerMenuDismissed(object sender, TheTechIdea.Beep.Winform.Controls.ContextMenus.MenuDismissedEventArgs e)
        {
            if (e == null || e.Owner != this) return;
            // Dispose the handle so the manager can free its tracking dict entry.
            var h = _managerPopupHandle;
            _managerPopupHandle = null;
            try { h?.Dispose(); } catch { /* non-fatal */ }
            // Reuse the manual path's close cleanup so behavior matches.
            OnDropdownMenuClosed(this, null);
        }
        
        /// <summary>
        /// Closes the dropdown menu
        /// </summary>
        public void CloseDropdown()
        {
            if (!_isDropdownOpen)
                return;

            if (BeepContextMenu != null && !BeepContextMenu.IsDisposed)
            {
                BeepContextMenu.Close();
            }
            // Dispose the manager-driven handle for the single-select path so the
            // manager frees its tracking entry. Dispose() is safe on an already-closed
            // menu; ShowNonBlocking also raises MenuDismissed which will null this
            // out, but we null it here too in case CloseDropdown is called from
            // outside the manager path (e.g. user pressed Escape on the form).
            var h = _managerPopupHandle;
            _managerPopupHandle = null;
            try { h?.Dispose(); } catch { /* non-fatal */ }

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
            // Sync state with actual menu visibility.
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
            if (!current.Exists(x => IsSameSimpleItem(x, item))) return;
            var updated = new System.Collections.Generic.List<SimpleItem>(current);
            int idx = updated.FindIndex(x => IsSameSimpleItem(x, item));
            if (idx >= 0)
            {
                updated.RemoveAt(idx);
            }
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
                ControlStyle            = Common.BeepControlStyle.None,
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
                if (!_isDropdownOpen)
                {
                    ShowDropdown();
                }

                // BeepContextMenu handles search internally when ShowSearchBox is enabled
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
            if (!IsInlineEditorAllowed()) return;

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
            if (value == null || value == DBNull.Value)
            {
                SelectedItem = null;
                return;
            }

            var resolved = ResolveItemFromValue(value);
            if (resolved != null)
            {
                SelectedItem = resolved;
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

        private void ApplyComboBoxTypeFromControlStyleIfNeeded()
        {
            if (_comboBoxTypeWasExplicitlySet)
            {
                return;
            }

            ComboBoxType mapped = ComboBoxVisualTokenCatalog.MapFromControlStyle(ControlStyle);
            if (_comboBoxType == mapped)
            {
                return;
            }

            _suppressComboBoxTypeExplicitTracking = true;
            try
            {
                ComboBoxType = mapped;
            }
            finally
            {
                _suppressComboBoxTypeExplicitTracking = false;
            }
        }
        
        internal void UpdateLayoutAndInvalidate()
        {
            InvalidateLayout();
        }

        private object ResolveSelectedValue(SimpleItem item)
        {
            if (item == null)
            {
                return null;
            }

            if (item.Value != null)
            {
                return item.Value;
            }

            if (item.Item != null)
            {
                return item.Item;
            }

            return !string.IsNullOrWhiteSpace(item.Text) ? item.Text : item.Name;
        }

        private void TryResolveSelectionFromCurrentValue()
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            object currentValue = _selectedItem != null ? ResolveSelectedValue(_selectedItem) : base.SelectedValue;
            if (currentValue == null && string.IsNullOrWhiteSpace(_inputText))
            {
                return;
            }

            var resolved = ResolveItemFromValue(currentValue ?? _inputText);
            if (resolved == null)
            {
                return;
            }

            if (_selectedItem != null && IsSameSimpleItem(_selectedItem, resolved))
            {
                _selectedItem = resolved;
                _selectedItemIndex = _listItems.IndexOf(resolved);
                _inputText = resolved.Text;
                Text = resolved.Text;
                return;
            }

            SelectedItem = resolved;
        }

        internal SimpleItem ResolveItemFromValue(object value)
        {
            if (value == null || value == DBNull.Value || _listItems == null || _listItems.Count == 0)
            {
                return null;
            }

            if (value is SimpleItem simpleItem)
            {
                foreach (var candidate in _listItems)
                {
                    if (candidate == null)
                    {
                        continue;
                    }

                    if (ReferenceEquals(candidate, simpleItem) || IsSameSimpleItem(candidate, simpleItem))
                    {
                        return candidate;
                    }

                    if (MatchesItemValue(candidate, simpleItem.Value) ||
                        MatchesItemValue(candidate, simpleItem.Item) ||
                        MatchesItemValue(candidate, simpleItem.Text) ||
                        MatchesItemValue(candidate, simpleItem.Name))
                    {
                        return candidate;
                    }
                }

                return null;
            }

            foreach (var candidate in _listItems)
            {
                if (MatchesItemValue(candidate, value))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static bool MatchesItemValue(SimpleItem item, object value)
        {
            if (item == null || value == null || value == DBNull.Value)
            {
                return false;
            }

            if (Equals(item.Value, value) || Equals(item.Item, value))
            {
                return true;
            }

            string valueText = Convert.ToString(value) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(valueText))
            {
                return false;
            }

            return string.Equals(item.Text, valueText, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(item.Name, valueText, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(Convert.ToString(item.Value), valueText, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(Convert.ToString(item.Item), valueText, StringComparison.OrdinalIgnoreCase);
        }

        internal static string GetSimpleItemIdentity(SimpleItem item)
        {
            if (item == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(item.GuidId))
            {
                return item.GuidId;
            }

            if (item.ID != 0)
            {
                return item.ID.ToString();
            }

            return !string.IsNullOrWhiteSpace(item.Text) ? item.Text : (item.Name ?? string.Empty);
        }

        internal static bool IsSameSimpleItem(SimpleItem left, SimpleItem right)
            => string.Equals(GetSimpleItemIdentity(left), GetSimpleItemIdentity(right), StringComparison.OrdinalIgnoreCase);

        private bool IsInlineEditorAllowed()
            => IsEditable || AllowFreeText || ComboBoxVisualTokenCatalog.SupportsInlineEditor(ComboBoxType);

        private void QueuePopupSelectionToggle(SimpleItem item, bool isChecked)
        {
            if (item == null || IsDisposed)
            {
                return;
            }

            _pendingPopupSelection ??= new System.Collections.Generic.List<SimpleItem>(
                SelectedItems ?? new System.Collections.Generic.List<SimpleItem>());

            int idx = _pendingPopupSelection.FindIndex(x => IsSameSimpleItem(x, item));
            if (isChecked)
            {
                if (idx < 0)
                {
                    _pendingPopupSelection.Add(item);
                }
            }
            else if (idx >= 0)
            {
                _pendingPopupSelection.RemoveAt(idx);
            }

            if (_multiSelectCommitQueued)
            {
                return;
            }

            _multiSelectCommitQueued = true;
            BeginInvoke(new Action(() =>
            {
                _multiSelectCommitQueued = false;
                if (IsDisposed)
                {
                    _pendingPopupSelection = null;
                    return;
                }

                if (_pendingPopupSelection != null)
                {
                    SelectedItems = new System.Collections.Generic.List<SimpleItem>(_pendingPopupSelection);
                    _pendingPopupSelection = null;
                }

                // Refresh dropdown if open
                if (BeepContextMenu != null && !BeepContextMenu.IsDisposed && BeepContextMenu.Visible)
                {
                    BeepContextMenu.Invalidate();
                }
            }));
        }

        private bool ShouldUsePrimaryActionFooter()
            => AllowMultipleSelection && UsePrimaryActionFooter && !UseApplyCancelFooter;

        private bool ShouldShowPopupFooter()
            => AllowMultipleSelection && (UseApplyCancelFooter || ShouldUsePrimaryActionFooter());

        private string ResolvePrimaryActionFooterText()
        {
            if (!ShouldUsePrimaryActionFooter())
            {
                return string.Empty;
            }

            string template = string.IsNullOrWhiteSpace(PrimaryActionFooterText)
                ? "Select {count} item(s)"
                : PrimaryActionFooterText;

            int selectedCount = SelectedItems?.Count ?? 0;
            return template.Replace("{count}", selectedCount.ToString());
        }

        /// <summary>
        /// Type-to-search for non-editable combos (DropDownList style).
        /// Opens the dropdown and selects the best match so the user
        /// sees matching items immediately.  Once the dropdown is open,
        /// the BeepContextMenu's own search box handles subsequent
        /// keystrokes.  This replaces the old silent-selection
        /// behaviour with the standard Windows ComboBox pattern of
        /// "open dropdown, scroll to match".
        /// </summary>
        private void HandleSelectOnlyTypeAhead(char typedChar)
        {
            if (char.IsControl(typedChar) || _listItems == null || _listItems.Count == 0)
                return;

            // Build the type-ahead buffer (cleared after 1.5s idle
            // or when the dropdown closes).
            _typeAheadBuffer = (_typeAheadBuffer ?? string.Empty) + typedChar;
            _typeAheadTimer?.Stop();
            _typeAheadTimer?.Start();

            // Open the dropdown if not already open — the
            // BeepContextMenu's search box receives focus and handles
            // all subsequent keystrokes directly.
            if (!_isDropdownOpen)
            {
                ShowDropdown();
            }

            // Also select the best match directly so the dropdown
            // scroll position lands on the matched item.
            SimpleItem best = null;
            int bestScore = 0;
            foreach (var item in _listItems)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Text))
                    continue;
                int score = FuzzySearchScore(item.Text, _typeAheadBuffer);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = item;
                }
            }

            if (best != null)
            {
                _selectedItem = best;
                _selectedItemIndex = _listItems.IndexOf(best);
                Text = best.Text;
            }
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

            InvalidateOnce(); // batched — SKILL Rule 2.6
        }

        #endregion
    }
}
