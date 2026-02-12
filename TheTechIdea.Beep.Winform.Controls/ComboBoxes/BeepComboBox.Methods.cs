using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

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
            if (_isDropdownOpen || BeepContextMenu == null || _listItems.Count == 0 || _isLoading)
                return;
            
            _isDropdownOpen = true;
            SyncDropdownMetrics();
            
            // Clear and populate context menu with list items
            BeepContextMenu.ClearItems();
            foreach (var item in _listItems)
            {
                BeepContextMenu.AddItem(item);
            }

            // Set multi-select behavior based on property
            BeepContextMenu.MultiSelect = AllowMultipleSelection;
            BeepContextMenu.ShowCheckBox = AllowMultipleSelection;
            
            // Calculate dropdown position
            Point screenLocation = PointToScreen(new Point(0, Height));
            
            // Set context menu width to match combo box
            BeepContextMenu.MenuWidth = Math.Max(Width, ScaleLogicalX(160));
            // Multi-select behaviors
            BeepContextMenu.CloseOnItemClick = !AllowMultipleSelection;
            
            // Ensure searchbox state is up to date
            BeepContextMenu.ShowSearchBox = (ComboBoxType== ComboBoxes.ComboBoxType.SearchableDropdown) || ShowSearchInDropdown;
            // Show the context menu
            BeepContextMenu.Show(screenLocation, this);
            
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
            
            BeepContextMenu.Close();
            _isDropdownOpen = false;
            
            PopupClosed?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
        
        /// <summary>
        /// Toggles the dropdown menu
        /// </summary>
        public void ToggleDropdown()
        {
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
        /// Starts editing mode (if editable)
        /// </summary>
        public void StartEditing()
        {
            if (!IsEditable) return;
            
            _isEditing = true;
            Focus();
            Invalidate();
        }
        
        /// <summary>
        /// Stops editing mode
        /// </summary>
        public void StopEditing()
        {
            _isEditing = false;
            Invalidate();
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
            }

            if (UseThemeFont)
            {
                TypographyStyle comboStyle = _currentTheme?.ComboBoxItemFont;
                if (comboStyle != null)
                {
                    _textFont = BeepFontManager.ToFont(comboStyle);
                }
                InvalidateLayout();
            }
            SyncDropdownMetrics();
            Invalidate();
        }

        protected override void OnDpiScaleChanged(float oldScaleX, float oldScaleY, float newScaleX, float newScaleY)
        {
            base.OnDpiScaleChanged(oldScaleX, oldScaleY, newScaleX, newScaleY);
            
            // When DPI changes, re-apply painter defaults to get properly scaled values
            // This ensures values scale correctly without compounding
            if (!_dropdownButtonWidthSetExplicitly || !_innerPaddingSetExplicitly)
            {
                _layoutDefaultsInitialized = false;
                ApplyLayoutDefaultsFromPainter(force: true);
            }
            
            InvalidateLayout();
            SyncDropdownMetrics();
        }

        private void SyncDropdownMetrics()
        {
            if (BeepContextMenu == null)
                return;

            BeepContextMenu.Theme = Theme;

            Font dropdownFont = TextFont ?? Font ?? SystemFonts.DefaultFont;
            if (UseThemeFont && _currentTheme?.ComboBoxListFont != null)
            {
                dropdownFont = BeepFontManager.ToFont(_currentTheme.ComboBoxListFont);
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
                textHeight = Math.Max(ScaleLogicalY(16), (int)Math.Ceiling(dropdownFont.Size * 1.35f));
            }

            int targetItemHeight = Math.Max(ScaleLogicalY(28), textHeight + ScaleLogicalY(10));
            if (BeepContextMenu.MenuItemHeight != targetItemHeight)
            {
                BeepContextMenu.MenuItemHeight = targetItemHeight;
            }

            int targetImageSize = Math.Max(ScaleLogicalY(16), Math.Min(textHeight, ScaleLogicalY(24)));
            if (BeepContextMenu.ImageSize != targetImageSize)
            {
                BeepContextMenu.ImageSize = targetImageSize;
            }

            int targetMaxHeight = Math.Max(ScaleLogicalY(140), ScaleLogicalY(MaxDropdownHeight));
            if (BeepContextMenu.MaxHeight != targetMaxHeight)
            {
                BeepContextMenu.MaxHeight = targetMaxHeight;
            }
        }
         
        #endregion
    }
}
