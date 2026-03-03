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
        /// <para>
        /// All rects are computed as direct subdivisions of <paramref name="drawingRect"/>
        /// with NO dependency on cached InnerPadding, DropdownButtonWidth, or DPI-scale
        /// fields.  This makes the layout immune to stale values after FormStyle / theme /
        /// focus changes — exactly the same principle BeepButton uses.
        /// </para>
        /// </summary>
        /// <summary>
        /// Minimum dropdown-button width (logical pixels, before DPI).
        /// Ensures the chevron is always clickable even on tiny controls.
        /// </summary>
        private const int MinButtonWidthLogical = 28;

        public void CalculateLayout(Rectangle drawingRect, out Rectangle textAreaRect, out Rectangle dropdownButtonRect, out Rectangle imageRect)
        {
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            {
                textAreaRect = Rectangle.Empty;
                dropdownButtonRect = Rectangle.Empty;
                imageRect = Rectangle.Empty;
                return;
            }

            // Use the actual drawing height to keep editor/text/button aligned with
            // the rendered border/content rect. Forcing a larger height here causes
            // vertical drift (controls appear shifted down) on some styles/scales.
            int usableHeight = Math.Max(1, drawingRect.Height);

            // ── 1. Dropdown button — fixed logical width, full height ────────
            //    Use the owner's stored width (already DPI-scaled by ApplyLayoutDefaults)
            //    but enforce a floor of MinButtonWidthLogical and clamp to 1/3 of width.
            int minBtnW   = _owner.ScaleLogicalX(MinButtonWidthLogical);
            int buttonWidth = Math.Max(minBtnW, _owner.DropdownButtonWidth);
            buttonWidth = Math.Min(buttonWidth, drawingRect.Width / 3);
            // Final safety: never below the logical minimum
            buttonWidth = Math.Max(minBtnW, buttonWidth);

            dropdownButtonRect = new Rectangle(
                drawingRect.Right - buttonWidth,
                drawingRect.Y,
                buttonWidth,
                usableHeight);

            // ── 2. Leading image (optional) ──────────────────────────────────
            imageRect = Rectangle.Empty;
            int imageConsumed = 0;   // horizontal space consumed by image + gap

            if (!string.IsNullOrEmpty(_owner.LeadingImagePath) || !string.IsNullOrEmpty(_owner.LeadingIconPath))
            {
                int iconInset  = Math.Max(2, usableHeight / 8);
                int iconSize   = Math.Max(8, usableHeight - iconInset * 2);
                int iconGap    = Math.Max(4, iconSize / 4);

                imageRect = new Rectangle(
                    drawingRect.X + iconInset,
                    drawingRect.Y + (usableHeight - iconSize) / 2,
                    iconSize,
                    iconSize);

                imageConsumed = iconSize + iconGap + iconInset;
            }

            // ── 3. Text area — everything between image and button ───────────
            int textX = drawingRect.X + imageConsumed;
            int textW = Math.Max(1, dropdownButtonRect.X - textX);

            textAreaRect = new Rectangle(
                textX,
                drawingRect.Y,
                textW,
                usableHeight);
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
        /// Get display text:
        /// 1. Selected item text  — when the user picked something from the list.
        /// 2. Free-typed input text — when the user typed something that didn't match
        ///    (or before the dropdown was used).
        /// 3. Placeholder          — when nothing has been typed or selected.
        /// </summary>
        public string GetDisplayText()
        {
            if (_owner.SelectedItem != null && !string.IsNullOrEmpty(_owner.SelectedItem.Text))
                return _owner.SelectedItem.Text;

            // Show whatever the user typed even if it doesn't match any item
            if (!string.IsNullOrEmpty(_owner.InputText))
                return _owner.InputText;

            return _owner.PlaceholderText ?? string.Empty;
        }
        
        /// <summary>
        /// Returns true only when nothing is typed and no item is selected
        /// (placeholder text is being shown).
        /// </summary>
        public bool IsShowingPlaceholder()
        {
            return (_owner.SelectedItem == null || string.IsNullOrEmpty(_owner.SelectedItem.Text))
                && string.IsNullOrEmpty(_owner.InputText);
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
                return _owner.DisabledForeColor != Color.Empty ? _owner.DisabledForeColor : (_owner._currentTheme?.DisabledForeColor ?? _owner.ForeColor);
            
            if (_owner.HasError)
                return _owner._currentTheme?.ComboBoxErrorForeColor ?? _owner._currentTheme?.ErrorColor ?? _owner.ForeColor;
            
            if (IsShowingPlaceholder())
                return _owner._currentTheme?.TextBoxPlaceholderColor ?? Color.FromArgb(160, _owner.ForeColor);
            
            return _owner.ForeColor;
        }
        
        /// <summary>
        /// Get the appropriate background color based on state
        /// </summary>
        public Color GetBackgroundColor()
        {
            if (!_owner.Enabled)
                return _owner.DisabledBackColor != Color.Empty ? _owner.DisabledBackColor : (_owner._currentTheme?.DisabledBackColor ?? _owner.BackColor);
            
            if (_owner.Focused)
                return _owner.BackColor;
            
            return _owner.BackColor;
        }
        
        /// <summary>
        /// Get the button hover color
        /// </summary>
        public Color GetButtonHoverColor()
        {
            return _owner._currentTheme?.ComboBoxHoverForeColor ?? (_owner.HoverForeColor != Color.Empty ? _owner.HoverForeColor : _owner.ForeColor);
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


