using System;
using System.Drawing;
using System.Windows.Forms;

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
            if (_isDropdownOpen || BeepContextMenu == null || _listItems.Count == 0)
                return;
            
            _isDropdownOpen = true;
            
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
            BeepContextMenu.MenuWidth = Width;
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
            // Ensure the dropdown menu follows the current theme
            if (BeepContextMenu != null)
            {
                BeepContextMenu.Theme = this.Theme;
            }
            Invalidate();
        }
        
        #endregion
    }
}
