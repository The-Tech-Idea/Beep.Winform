using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown
    {
        /// <summary>
        /// Handle text changes from base BeepTextBox
        /// </summary>
        private void BeepDateDropDown_TextChanged(object sender, EventArgs e)
        {
            // Trigger segment recalculation
            _segmentsNeedRecalculation = true;
            
            // Don't parse while user is still typing in certain modes
            if (!ValidateOnKeyPress && Focused)
                return;

            // Parse the text and update date(s)
            ParseAndUpdateDate(Text);
        }

        /// <summary>
        /// Override mouse down to handle dropdown button
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Check if click is on the image (calendar icon)
            if (_showDropDown && ImageVisible)
            {
                Rectangle imageRect = GetImageBounds();
                if (imageRect.Contains(e.Location))
                {
                    TogglePopup();
                    return;
                }
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Override key down for dropdown shortcuts
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // F4 or Alt+Down opens dropdown
            if (e.KeyCode == Keys.F4 || (e.Alt && e.KeyCode == Keys.Down))
            {
                if (_showDropDown)
                {
                    TogglePopup();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            // Escape closes dropdown or clears text
            if (e.KeyCode == Keys.Escape)
            {
                if (_isPopupOpen)
                {
                    ClosePopup();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }
                else if (_allowEmpty)
                {
                    Text = string.Empty;
                    _selectedDateTime = null;
                    _startDate = null;
                    _endDate = null;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Override lost focus to validate if configured
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            // Validate and parse on lost focus if enabled
            if (ValidateOnLostFocus && !string.IsNullOrWhiteSpace(Text))
            {
                ParseAndUpdateDate(Text);
            }

            // Close popup if open
            if (_isPopupOpen)
            {
                ClosePopup();
            }
        }

        /// <summary>
        /// Handle date selection from the calendar popup
        /// </summary>
        private void CalendarView_DateChanged(object sender, Models.DateTimePickerEventArgs e)
        {
            if (e?.Date.HasValue == true)
            {
                switch (_mode)
                {
                    case Models.DatePickerMode.Single:
                    case Models.DatePickerMode.SingleWithTime:
                        SelectedDateTime = e.Date.Value;
                        break;

                    case Models.DatePickerMode.Range:
                    case Models.DatePickerMode.RangeWithTime:
                        // For range, we need to handle start and end dates
                        // If start is not set, set it. If start is set, set end.
                        if (!_startDate.HasValue)
                        {
                            StartDate = e.Date.Value;
                        }
                        else if (!_endDate.HasValue || e.Date.Value != _startDate.Value)
                        {
                            EndDate = e.Date.Value;
                            
                            // Swap if end is before start
                            if (_endDate.Value < _startDate.Value)
                            {
                                var temp = _startDate.Value;
                                _startDate = _endDate;
                                _endDate = temp;
                                UpdateTextFromDateRange();
                            }
                            
                            // Close popup after range is complete
                            ClosePopup();
                        }
                        break;
                }
            }
            else
            {
                // Single date mode - close popup
                if (_mode == Models.DatePickerMode.Single || _mode == Models.DatePickerMode.SingleWithTime)
                {
                    ClosePopup();
                }
            }
        }

        /// <summary>
        /// Get bounds of the calendar icon/image
        /// </summary>
        private Rectangle GetImageBounds()
        {
            if (!ImageVisible || BeepImage == null)
                return Rectangle.Empty;

            // Calculate image bounds based on ImageAlign and TextImageRelation
            // This is a simplified calculation - actual bounds depend on layout
            int imgWidth = MaxImageSize.Width;
            int imgHeight = MaxImageSize.Height;

            Rectangle bounds = ClientRectangle;
            int x = bounds.Right - imgWidth - _dropdownPadding - BorderWidth;
            int y = bounds.Y + (bounds.Height - imgHeight) / 2;

            return new Rectangle(x, y, imgWidth, imgHeight);
        }
    }
}
