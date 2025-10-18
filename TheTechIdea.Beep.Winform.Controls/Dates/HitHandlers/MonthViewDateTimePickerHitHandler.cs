using System;
using System.Drawing;
using System.Globalization;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Month View Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - 3x4 grid of month buttons (12 months: January through December)
    /// - Year navigation buttons at top (prev/next year)
    /// - Each month button represents entire month (e.g., clicking "March" = March 1-31)
    /// - Year header displayed prominently at center top
    /// - Grid layout: padding=20px, gap=12px between cells
    /// - Cell size calculated: cellWidth = (Width - gap*(cols-1)) / cols
    /// 
    /// INTERACTION MODEL:
    /// 1. Click year navigation buttons → change year, stay in view
    /// 2. Click month button → select first day of that month, close picker
    /// 3. Month hover → highlight single button
    /// 
    /// LAYOUT STRUCTURE:
    /// - Year header: Y+20, Height=50, navigation buttons 36x36
    /// - Month grid: Starts at Y+90, 3 columns x 4 rows
    /// - Navigation: PrevYear (X+20), NextYear (Right-36)
    /// </summary>
    internal class MonthViewDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;  // First day of selected month
        public DatePickerMode Mode => DatePickerMode.MonthView;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            
            // Check year combo box first (for direct year selection)
            if (layout.YearComboBoxRect != Rectangle.Empty && layout.YearComboBoxRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.YearComboBox;
                result.HitBounds = layout.YearComboBoxRect;
                return result;
            }

            // Check year navigation buttons using registered layout rectangles
            if (layout.PreviousYearButtonRect != Rectangle.Empty && layout.PreviousYearButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.PreviousYearButton;
                result.HitBounds = layout.PreviousYearButtonRect;
                return result;
            }
            
            if (layout.NextYearButtonRect != Rectangle.Empty && layout.NextYearButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.NextYearButton;
                result.HitBounds = layout.NextYearButtonRect;
                return result;
            }
            
            // Check month cells using registered layout rectangles
            if (layout.MonthCellRects != null)
            {
                for (int i = 0; i < layout.MonthCellRects.Count && i < 12; i++)
                {
                    if (layout.MonthCellRects[i].Contains(location))
                    {
                        int month = i + 1;  // 1-12
                        DateTime monthDate = new DateTime(displayMonth.Year, month, 1);
                        
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.MonthCell;
                        result.Date = monthDate;
                        result.HitBounds = layout.MonthCellRects[i];
                        result.CellIndex = i;
                        return result;
                    }
                }
            }
            
            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit) return false;
            
            // Year ComboBox click - show year selection dropdown
            if (hitResult.HitArea == DateTimePickerHitArea.YearComboBox)
            {
                ShowYearComboBox(owner, hitResult);
                return false; // Don't close the picker - let user interact with combo
            }

            // Year navigation
            if (hitResult.HitArea == DateTimePickerHitArea.PreviousYearButton)
            {
                owner.NavigateToPreviousYear();
                return false;  // Don't close
            }
            if (hitResult.HitArea == DateTimePickerHitArea.NextYearButton)
            {
                owner.NavigateToNextYear();
                return false;  // Don't close
            }
            
            // Month selection
            if (hitResult.HitArea == DateTimePickerHitArea.MonthCell && hitResult.Date.HasValue)
            {
                // Validate against MinDate/MaxDate
                DateTime selectedMonth = hitResult.Date.Value;  // First day of month
                
                if (!owner.IsDateInRange(selectedMonth))
                    return false;
                
                _selectedDate = selectedMonth;
                SyncToControl(owner);
                return true;  // Close on selection
            }
            
            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            
            // Year ComboBox hover
            if (hitResult.HitArea == DateTimePickerHitArea.YearComboBox)
            {
                hoverState.HoverArea = DateTimePickerHitArea.YearComboBox;
                hoverState.HoveredButton = "year_combo";
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Month cell hover
            else if (hitResult.HitArea == DateTimePickerHitArea.MonthCell)
            {
                hoverState.HoverArea = DateTimePickerHitArea.MonthCell;
                hoverState.HoveredDate = hitResult.Date;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.PreviousYearButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.PreviousYearButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.NextYearButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.NextYearButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
        }

        public void SyncFromControl(BeepDateTimePicker owner)
        {
            _selectedDate = owner.SelectedDate;
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Set first day of selected month
            owner.SelectedDate = _selectedDate;
            owner.RangeStartDate = null;
            owner.RangeEndDate = null;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
            owner.SelectedTime = null;
        }

        public bool IsSelectionComplete()
        {
            return _selectedDate.HasValue;
        }

        public void Reset()
        {
            _selectedDate = null;
        }

        /// <summary>
        /// Get localized month name for display
        /// </summary>
        private string GetMonthName(int month)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }

        /// <summary>
        /// Show year combo box dropdown for year selection
        /// Uses BeepComboBox with ShowDropdown() to display year options
        /// </summary>
        private void ShowYearComboBox(BeepDateTimePicker owner, DateTimePickerHitTestResult hitResult)
        {
            if (owner == null) return;

            int currentYear = owner.DisplayMonth.Year;
            int minYear = owner.MinDate.Year;
            int maxYear = owner.MaxDate.Year;

            // Create BeepComboBox using DateTimeComboBoxHelper
            var comboBox = DateTimeComboBoxHelper.CreateYearComboBox(minYear, maxYear, currentYear);
            
            // Set up selection change handler
            comboBox.SelectedIndexChanged += (s, e) =>
            {
                int? selectedYear = DateTimeComboBoxHelper.GetSelectedYear(comboBox);
                if (!selectedYear.HasValue) return;
                
                // Calculate month difference to navigate to selected year (keep same month)
                int monthDiff = ((selectedYear.Value - owner.DisplayMonth.Year) * 12);
                
                // Navigate to adjust DisplayMonth to the selected year
                if (monthDiff > 0)
                {
                    for (int i = 0; i < monthDiff; i++)
                        owner.NavigateToNextMonth();
                }
                else if (monthDiff < 0)
                {
                    for (int i = 0; i < Math.Abs(monthDiff); i++)
                        owner.NavigateToPreviousMonth();
                }
                
                owner.Invalidate();
                comboBox.CloseDropdown();
            };

            // Position the combo box at the year combo box rect location
            var screenPoint = owner.PointToScreen(new Point(
                hitResult.HitBounds.X, 
                hitResult.HitBounds.Y
            ));
            
            comboBox.Size = hitResult.HitBounds.Size;
            comboBox.Location = owner.PointToClient(screenPoint);
            
            // Add to owner's controls temporarily
            owner.Controls.Add(comboBox);
            comboBox.BringToFront();
            
            // Show the dropdown
            comboBox.ShowDropdown();
            
            // Remove when closed
            comboBox.PopupClosed += (s, e) =>
            {
                owner.Controls.Remove(comboBox);
                comboBox.Dispose();
            };
        }
    }
}
