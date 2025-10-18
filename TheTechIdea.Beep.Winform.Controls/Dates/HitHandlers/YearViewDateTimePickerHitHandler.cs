using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Year View Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - 3x4 grid of year buttons (12 years per view)
    /// - Decade navigation at top (prev/next decade buttons: 36x36)
    /// - Shows startYear-1 to startYear+10 (e.g., 2019-2030 for decade 2020-2029)
    /// - Decade header displays range (e.g., "2020 — 2029")
    /// - Double chevron icons for decade navigation (10-year jumps)
    /// - Click year → select January 1 of that year, close
    /// 
    /// INTERACTION MODEL:
    /// 1. Click decade navigation → jump 10 years, stay in view
    /// 2. Click year button → select Jan 1 of that year, close picker
    /// 3. Year hover → highlight single button
    /// 
    /// LAYOUT STRUCTURE:
    /// - Decade header: Y+20, Height=50, navigation 36x36
    /// - Year grid: Starts at Y+90, 3 columns x 4 rows, gap=10px
    /// - StartYear calculation: (displayYear / 10) * 10
    /// </summary>
    internal class YearViewDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;  // January 1 of selected year
        public DatePickerMode Mode => DatePickerMode.YearView;

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

            // Check decade navigation buttons using registered layout rectangles
            if (layout.PreviousDecadeButtonRect != Rectangle.Empty && layout.PreviousDecadeButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.PreviousDecadeButton;
                result.HitBounds = layout.PreviousDecadeButtonRect;
                return result;
            }
            
            if (layout.NextDecadeButtonRect != Rectangle.Empty && layout.NextDecadeButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.NextDecadeButton;
                result.HitBounds = layout.NextDecadeButtonRect;
                return result;
            }
            
            // Check year cells using registered layout rectangles
            if (layout.YearCellRects != null)
            {
                // Calculate decade start (e.g., 2020 for year 2025)
                int startYear = (displayMonth.Year / 10) * 10;
                
                for (int i = 0; i < layout.YearCellRects.Count && i < 12; i++)
                {
                    if (layout.YearCellRects[i].Contains(location))
                    {
                        int year = startYear - 1 + i;  // startYear-1 to startYear+10
                        DateTime yearDate = new DateTime(year, 1, 1);
                        
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.YearCell;
                        result.Date = yearDate;
                        result.HitBounds = layout.YearCellRects[i];
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

            // Decade navigation (jump 10 years by navigating months)
            if (hitResult.HitArea == DateTimePickerHitArea.PreviousDecadeButton)
            {
                // Jump back 10 years (120 months)
                for (int i = 0; i < 120; i++)
                    owner.NavigateToPreviousMonth();
                return false;  // Don't close
            }
            if (hitResult.HitArea == DateTimePickerHitArea.NextDecadeButton)
            {
                // Jump forward 10 years (120 months)
                for (int i = 0; i < 120; i++)
                    owner.NavigateToNextMonth();
                return false;  // Don't close
            }
            
            // Year selection
            if (hitResult.HitArea == DateTimePickerHitArea.YearCell && hitResult.Date.HasValue)
            {
                // Validate against MinDate/MaxDate
                DateTime selectedYear = hitResult.Date.Value;  // January 1 of year
                
                if (!owner.IsDateInRange(selectedYear))
                    return false;
                
                _selectedDate = selectedYear;
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
            // Year cell hover
            else if (hitResult.HitArea == DateTimePickerHitArea.YearCell && hitResult.Date.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.YearCell;
                hoverState.HoveredDate = hitResult.Date.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.PreviousDecadeButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.PreviousDecadeButton;
                hoverState.HoveredButton = "nav_previous_decade";
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.NextDecadeButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.NextDecadeButton;
                hoverState.HoveredButton = "nav_next_decade";
                hoverState.HoverBounds = hitResult.HitBounds;
            }
        }

        public void SyncFromControl(BeepDateTimePicker owner)
        {
            _selectedDate = owner.SelectedDate;
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Set January 1 of selected year
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
        /// Calculate the start year of the decade for a given year
        /// E.g., 2025 → 2020, 2019 → 2010
        /// </summary>
        private int GetDecadeStart(int year)
        {
            return (year / 10) * 10;
        }

        /// <summary>
        /// Get the decade range string for display
        /// E.g., "2020 — 2029"
        /// </summary>
        private string GetDecadeRange(int year)
        {
            int startYear = GetDecadeStart(year);
            int endYear = startYear + 9;
            return $"{startYear} — {endYear}";
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
                
                // Calculate month difference to navigate to selected year
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
