using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docks;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Keyboard Navigation and Accessibility
    /// </summary>
    public partial class BeepDock
    {
        private int _focusedIndex = -1;
        private bool _keyboardNavigationEnabled = true;

        /// <summary>
        /// Gets or sets whether keyboard navigation is enabled
        /// </summary>
        public bool KeyboardNavigationEnabled
        {
            get => _keyboardNavigationEnabled;
            set
            {
                _keyboardNavigationEnabled = value;
                TabStop = value;
            }
        }

        /// <summary>
        /// Gets or sets the currently focused item index for keyboard navigation
        /// </summary>
        public int FocusedIndex
        {
            get => _focusedIndex;
            set
            {
                if (value >= -1 && value < _items.Count)
                {
                    SetFocusedIndex(value);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Handles keyboard input for dock navigation
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!_keyboardNavigationEnabled || _items.Count == 0)
                return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.Left:
                case Keys.Up:
                    NavigatePrevious();
                    return true;

                case Keys.Right:
                case Keys.Down:
                    NavigateNext();
                    return true;

                case Keys.Home:
                    NavigateFirst();
                    return true;

                case Keys.End:
                    NavigateLast();
                    return true;

                case Keys.Enter:
                case Keys.Space:
                    ActivateFocusedItem();
                    return true;

                case Keys.Escape:
                    ClearFocus();
                    return true;

                case Keys.Tab:
                    if ((keyData & Keys.Shift) == Keys.Shift)
                        NavigatePrevious();
                    else
                        NavigateNext();
                    return true;

                // Number keys 1-9 for quick access
                case Keys.D1: SelectItemByIndex(0); return true;
                case Keys.D2: SelectItemByIndex(1); return true;
                case Keys.D3: SelectItemByIndex(2); return true;
                case Keys.D4: SelectItemByIndex(3); return true;
                case Keys.D5: SelectItemByIndex(4); return true;
                case Keys.D6: SelectItemByIndex(5); return true;
                case Keys.D7: SelectItemByIndex(6); return true;
                case Keys.D8: SelectItemByIndex(7); return true;
                case Keys.D9: SelectItemByIndex(8); return true;

                // Context menu
                case Keys.Apps:
                case Keys.F10 | Keys.Shift:
                    ShowContextMenuForFocusedItem();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void NavigatePrevious()
        {
            if (_items.Count == 0) return;

            if (_focusedIndex <= 0)
                SetFocusedIndex(_items.Count - 1);
            else
                SetFocusedIndex(_focusedIndex - 1);

            UpdateFocusVisuals();
        }

        private void NavigateNext()
        {
            if (_items.Count == 0) return;

            if (_focusedIndex >= _items.Count - 1)
                SetFocusedIndex(0);
            else
                SetFocusedIndex(_focusedIndex + 1);

            UpdateFocusVisuals();
        }

        private void NavigateFirst()
        {
            if (_items.Count == 0) return;
            SetFocusedIndex(0);
            UpdateFocusVisuals();
        }

        private void NavigateLast()
        {
            if (_items.Count == 0) return;
            SetFocusedIndex(_items.Count - 1);
            UpdateFocusVisuals();
        }

        private void ActivateFocusedItem()
        {
            if (_focusedIndex >= 0 && _focusedIndex < _items.Count)
            {
                var item = _items[_focusedIndex];
                SelectedItem = item;
                ItemClicked?.Invoke(this, new DockItemEventArgs(item, _focusedIndex));
            }
        }

        private void ClearFocus()
        {
            SetFocusedIndex(-1);
            Invalidate();
        }

        private void SelectItemByIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                SetFocusedIndex(index);
                ActivateFocusedItem();
            }
        }

        private void UpdateFocusVisuals()
        {
            // Update hover state for focused item
            for (int i = 0; i < _itemStates.Count; i++)
            {
                _itemStates[i].IsHovered = (i == _focusedIndex);
                _itemStates[i].IsFocused = (i == _focusedIndex);
            }

            // Scroll focused item into view if needed
            if (_focusedIndex >= 0 && _focusedIndex < _itemStates.Count)
            {
                var focusedState = _itemStates[_focusedIndex];
                // Could implement auto-scroll here if dock is scrollable
            }

            Invalidate();
        }

        private void ShowContextMenuForFocusedItem()
        {
            if (_focusedIndex >= 0 && _focusedIndex < _items.Count && _focusedIndex < _itemStates.Count)
            {
                var item = _items[_focusedIndex];
                var itemState = _itemStates[_focusedIndex];
                
                // Show context menu at item location
                var menuLocation = PointToScreen(new System.Drawing.Point(
                    itemState.Bounds.X + itemState.Bounds.Width / 2,
                    itemState.Bounds.Y + itemState.Bounds.Height
                ));

                ShowContextMenu(item, menuLocation);
            }
        }

        /// <summary>
        /// Shows a context menu for the specified item
        /// Override to provide custom context menu
        /// </summary>
        protected virtual void ShowContextMenu(Models.SimpleItem item, System.Drawing.Point location)
        {
            // Create default context menu
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Open", null, (s, e) => { SelectedItem = item; ItemClicked?.Invoke(this, new DockItemEventArgs(item)); });
            
            if (_config.EnableReorder)
            {
                contextMenu.Items.Add(new ToolStripSeparator());
                contextMenu.Items.Add("Move Up", null, (s, e) => MoveItemUp(item));
                contextMenu.Items.Add("Move Down", null, (s, e) => MoveItemDown(item));
            }

            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Remove from Dock", null, (s, e) => RemoveItem(item));

            contextMenu.Show(location);
        }

        private void MoveItemUp(Models.SimpleItem item)
        {
            int index = _items.IndexOf(item);
            if (index > 0)
            {
                _items.RemoveAt(index);
                _items.Insert(index - 1, item);
            }
        }

        private void MoveItemDown(Models.SimpleItem item)
        {
            int index = _items.IndexOf(item);
            if (index >= 0 && index < _items.Count - 1)
            {
                _items.RemoveAt(index);
                _items.Insert(index + 1, item);
            }
        }

        /// <summary>
        /// Provides accessibility information
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            // Set accessibility properties
            AccessibleName = "Dock";
            AccessibleRole = AccessibleRole.ToolBar;
            AccessibleDescription = $"Application dock with {_items.Count} items";
        }

        /// <summary>
        /// Updates accessibility information when items change
        /// </summary>
        private void UpdateAccessibility()
        {
            if (IsHandleCreated)
            {
                AccessibleDescription = $"Application dock with {_items.Count} items";
                
                // Announce changes to screen readers
                if (_focusedIndex >= 0 && _focusedIndex < _items.Count)
                {
                    var item = _items[_focusedIndex];
                    // Could use UI Automation to announce item changes
                }
            }
        }

        /// <summary>
        /// Handles focus events
        /// </summary>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            
            // Start keyboard navigation at first item if nothing is focused
            if (_focusedIndex < 0 && _items.Count > 0)
            {
                SetFocusedIndex(0);
                UpdateFocusVisuals();
            }
        }

        /// <summary>
        /// Handles focus loss
        /// </summary>
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            
            // Clear keyboard focus visuals but remember the index
            for (int i = 0; i < _itemStates.Count; i++)
            {
                _itemStates[i].IsHovered = false;
                _itemStates[i].IsFocused = false;
            }
            Invalidate();
        }

        /// <summary>
        /// Provides high contrast mode support
        /// </summary>
        private bool IsHighContrastMode()
        {
            return SystemInformation.HighContrast;
        }

        /// <summary>
        /// Gets high contrast colors
        /// </summary>
        private System.Drawing.Color GetHighContrastColor(System.Drawing.Color defaultColor, bool isForeground)
        {
            if (!IsHighContrastMode())
                return defaultColor;

            return isForeground
                ? Sc(SystemColors.HighlightText)
                : Sc(SystemColors.Highlight);
        }

        private static System.Drawing.Color Sc(System.Drawing.Color lightColor)
        {
            bool dark = SystemInformation.HighContrast || (TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.IsDarkTheme == true);
            if (!dark) return lightColor;
            return lightColor switch
            {
                var x when x == SystemColors.Window => System.Drawing.Color.FromArgb(30, 30, 30),
                var x when x == SystemColors.WindowText => System.Drawing.Color.White,
                var x when x == SystemColors.ControlText => System.Drawing.Color.White,
                var x when x == SystemColors.GrayText => System.Drawing.Color.FromArgb(150, 150, 155),
                var x when x == SystemColors.Highlight => System.Drawing.Color.FromArgb(0, 120, 215),
                var x when x == SystemColors.HighlightText => System.Drawing.Color.White,
                var x when x == SystemColors.Control => System.Drawing.Color.FromArgb(45, 45, 48),
                var x when x == SystemColors.ControlDark => System.Drawing.Color.FromArgb(70, 70, 75),
                var x when x == SystemColors.ControlLight => System.Drawing.Color.FromArgb(70, 70, 75),
                var x when x == SystemColors.ControlLightLight => System.Drawing.Color.FromArgb(60, 60, 65),
                var x when x == SystemColors.ActiveCaption => System.Drawing.Color.FromArgb(45, 45, 48),
                var x when x == SystemColors.Info => System.Drawing.Color.FromArgb(50, 50, 55),
                var x when x == SystemColors.InfoText => System.Drawing.Color.White,
                _ => lightColor
            };
        }
    }
}

