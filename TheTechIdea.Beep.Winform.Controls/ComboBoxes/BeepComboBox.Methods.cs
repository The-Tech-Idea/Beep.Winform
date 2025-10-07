using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepComboBox
    {
        #region Public Methods
        
        /// <summary>
        /// Shows the dropdown popup
        /// </summary>
        public void ShowPopup()
        {
            if (_isPopupOpen || _popupForm == null || _listItems.Count == 0)
                return;
            
            _isPopupOpen = true;
            
            // Calculate popup position
            Point screenLocation = PointToScreen(new Point(0, Height));
            
            // Set popup properties
            _popupForm.Location = screenLocation;
            _popupForm.Width = Width;
            _popupForm.MaxHeight = MaxDropdownHeight;
            _popupForm.SelectedItem = _selectedItem;
            
            // Show popup
            _popupForm.Show(this);
            
            PopupOpened?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
        
        /// <summary>
        /// Closes the dropdown popup
        /// </summary>
        public void ClosePopup()
        {
            if (!_isPopupOpen || _popupForm == null)
                return;
            
            _popupForm.Hide();
            _isPopupOpen = false;
            
            PopupClosed?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
        
        /// <summary>
        /// Toggles the dropdown popup
        /// </summary>
        public void TogglePopup()
        {
            if (_isPopupOpen)
            {
                ClosePopup();
            }
            else
            {
                ShowPopup();
            }
        }
        
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
            
            if (_popupForm != null && ApplyThemeToChilds)
            {
                // Apply theme to popup if needed
            }
            
            Invalidate();
        }
        
        #endregion
    }
}
