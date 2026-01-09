using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    /// <summary>
    /// Helper class for BeepComboBox - handles layout calculations and core logic
    /// </summary>
    internal class BeepComboBoxHelper : IDisposable
    {
        private readonly BeepComboBox _owner;
        private bool _disposed = false;
        
        public BeepComboBoxHelper(BeepComboBox owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
        
        #region Layout Calculation
        
        /// <summary>
        /// Calculate layout rectangles for text area, dropdown button, and image
        /// </summary>
        public void CalculateLayout(Rectangle drawingRect, out Rectangle textAreaRect, out Rectangle dropdownButtonRect, out Rectangle imageRect)
        {
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            {
                textAreaRect = Rectangle.Empty;
                dropdownButtonRect = Rectangle.Empty;
                imageRect = Rectangle.Empty;
                return;
            }
            
            // Apply inner padding
            var padding = _owner.InnerPadding;
            var workingRect = new Rectangle(
                drawingRect.X + padding.Left,
                drawingRect.Y + padding.Top,
                Math.Max(1, drawingRect.Width - padding.Horizontal),
                Math.Max(1, drawingRect.Height - padding.Vertical)
            );
            
            // Calculate dropdown button area (right side)
            int buttonWidth = _owner.DropdownButtonWidth;
            dropdownButtonRect = new Rectangle(
                workingRect.Right - buttonWidth,
                workingRect.Y,
                buttonWidth,
                workingRect.Height
            );
            
            // Calculate image area if leading image exists
            imageRect = Rectangle.Empty;
            int imageWidth = 0;
            
            if (!string.IsNullOrEmpty(_owner.LeadingImagePath) || !string.IsNullOrEmpty(_owner.LeadingIconPath))
            {
                // Calculate image size (16x16 default icon size with padding)
                imageWidth = Math.Min(24, workingRect.Height);
                imageRect = new Rectangle(
                    workingRect.X,
                    workingRect.Y + (workingRect.Height - imageWidth) / 2,
                    imageWidth,
                    imageWidth
                );
                imageWidth += 4; // Add spacing after image
            }
            
            // Calculate text area (remaining space between image and button)
            textAreaRect = new Rectangle(
                workingRect.X + imageWidth,
                workingRect.Y,
                Math.Max(1, workingRect.Width - imageWidth - buttonWidth),
                workingRect.Height
            );
        }
        
        #endregion
        
        #region Text Measurement
        
        /// <summary>
        /// Measure text size for layout purposes (using cached TextUtils)
        /// </summary>
        public Size MeasureText(string text, Font font)
        {
            if (string.IsNullOrEmpty(text) || font == null)
                return Size.Empty;
            
            SizeF sizeF = TextUtils.MeasureText(text, font, int.MaxValue);
            return new Size((int)sizeF.Width, (int)sizeF.Height);
        }
        
        /// <summary>
        /// Get display text (selected item text or placeholder)
        /// </summary>
        public string GetDisplayText()
        {
            if (_owner.SelectedItem != null && !string.IsNullOrEmpty(_owner.SelectedItem.Text))
            {
                return _owner.SelectedItem.Text;
            }
            
            return _owner.PlaceholderText ?? string.Empty;
        }
        
        /// <summary>
        /// Check if showing placeholder
        /// </summary>
        public bool IsShowingPlaceholder()
        {
            return _owner.SelectedItem == null || string.IsNullOrEmpty(_owner.SelectedItem.Text);
        }
        
        #endregion
        
        #region Item Management
        
        /// <summary>
        /// Find item by text (case-insensitive)
        /// </summary>
        public SimpleItem FindItemByText(string text)
        {
            if (string.IsNullOrEmpty(text) || _owner.ListItems.Count == 0)
                return null;
            
            foreach (var item in _owner.ListItems)
            {
                if (string.Equals(item.Text, text, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Find item by value
        /// </summary>
        public SimpleItem FindItemByValue(object value)
        {
            if (value == null || _owner.ListItems.Count == 0)
                return null;
            
            foreach (var item in _owner.ListItems)
            {
                if (Equals(item.Item, value))
                {
                    return item;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Get next item in the list
        /// </summary>
        public SimpleItem GetNextItem()
        {
            if (_owner.ListItems.Count == 0)
                return null;
            
            int currentIndex = _owner.SelectedIndex;
            int nextIndex = (currentIndex + 1) % _owner.ListItems.Count;
            
            return _owner.ListItems[nextIndex];
        }
        
        /// <summary>
        /// Get previous item in the list
        /// </summary>
        public SimpleItem GetPreviousItem()
        {
            if (_owner.ListItems.Count == 0)
                return null;
            
            int currentIndex = _owner.SelectedIndex;
            if (currentIndex < 0) currentIndex = 0;
            
            int prevIndex = currentIndex - 1;
            if (prevIndex < 0) prevIndex = _owner.ListItems.Count - 1;
            
            return _owner.ListItems[prevIndex];
        }
        
        #endregion
        
        #region Color Helpers
        
        /// <summary>
        /// Get the appropriate text color based on state
        /// </summary>
        public Color GetTextColor()
        {
            if (!_owner.Enabled)
                return Color.Gray;
            
            if (_owner.HasError)
                return _owner._currentTheme?.ErrorColor ?? Color.Red;
            
            if (IsShowingPlaceholder())
                return _owner._currentTheme?.TextBoxPlaceholderColor ?? Color.Gray;
            
            return _owner.ForeColor;
        }
        
        /// <summary>
        /// Get the appropriate background color based on state
        /// </summary>
        public Color GetBackgroundColor()
        {
            if (!_owner.Enabled)
                return Color.FromArgb(240, 240, 240);
            
            if (_owner.Focused)
                return _owner._currentTheme?.FocusIndicatorColor ?? _owner.BackColor;
            
            return _owner.BackColor;
        }
        
        /// <summary>
        /// Get the button hover color
        /// </summary>
        public Color GetButtonHoverColor()
        {
            return _owner._currentTheme?.ComboBoxHoverForeColor ?? Color.FromArgb(230, 230, 230);
        }
        
        #endregion
        
        #region Dispose
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                // Clean up managed resources
            }
            
            _disposed = true;
        }
        
        #endregion
    }
}
