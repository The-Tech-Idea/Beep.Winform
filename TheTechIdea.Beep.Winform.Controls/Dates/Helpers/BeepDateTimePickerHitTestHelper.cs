using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Helpers
{
    /// <summary>
    /// Helper class for managing hit testing and interactive areas in BeepDateTimePicker.
    /// Maps calendar areas (day cells, navigation buttons, quick buttons, etc.) to BaseControl's hit test system.
    /// </summary>
    internal class BeepDateTimePickerHitTestHelper
    {
        private readonly BeepDateTimePicker _owner;
        
        // Dictionary mapping hit area names to their rectangle bounds
        private Dictionary<string, Rectangle> _hitAreaMap;

        public BeepDateTimePickerHitTestHelper(BeepDateTimePicker owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _hitAreaMap = new Dictionary<string, Rectangle>();
        }

        #region Hit Area Registration

        /// <summary>
        /// Registers all interactive hit areas from the current layout to BaseControl's hit test system.
        /// This should be called after layout calculation and before/during painting.
        /// Handles different painter layouts including single month, dual month (for ranges), and multi-month displays.
        /// </summary>
        public void RegisterHitAreas(DateTimePickerLayout layout, DateTimePickerProperties props, DateTime displayMonth)
        {
            if (layout == null) return;

            // Clear existing hit areas
            _owner._hitTest.ClearHitList();
            _hitAreaMap.Clear();

            // Register navigation buttons (common across all layouts)
            RegisterNavigationButtons(layout);

            // Check if we have multiple calendar grids (e.g., for range pickers showing 2 months)
            if (layout.MonthGrids != null && layout.MonthGrids.Count > 0)
            {
                // Register multiple calendar grids (dual-month or multi-month layout)
                RegisterMultipleCalendarGrids(layout, displayMonth, props);
            }
            else
            {
                // Register single calendar grid (traditional single-month layout)
                RegisterDayCells(layout, displayMonth, props);
            }

            // Register time slots if showing time picker
            if (props.ShowTime)
            {
                RegisterTimeSlots(layout, props);
            }

            // Register quick action buttons
            if (props.ShowCustomQuickDates || props.ShowTodayButton || props.ShowTomorrowButton)
            {
                RegisterQuickButtons(layout);
            }

            // Register RangeWithTime spinner controls
            RegisterRangeTimeSpinners(layout);

            // Register clear button if enabled
            if (props.AllowClear && layout.ClearButtonRect != Rectangle.Empty)
            {
                RegisterClearButton(layout);
            }

            // Register week numbers if shown
            if (props.ShowWeekNumbers != DatePickerWeekNumbers.None)
            {
                RegisterWeekNumbers(layout);
            }

            // Register action buttons (Apply/Cancel)
            if (props.ShowApplyButton || props.ShowCancelButton)
            {
                RegisterActionButtons(layout);
            }

            // Register quarter buttons (for Quarterly mode)
            if (props.ShowQuarterButtons && layout.QuickDateButtons != null)
            {
                RegisterQuarterButtons(layout);
            }

            // Register month buttons (for MonthView mode)
            if (props.ShowMonthButtons && layout.QuickDateButtons != null)
            {
                RegisterMonthButtons(layout);
            }

            // Register year buttons (for YearView mode)
            if (props.ShowYearButtons && layout.QuickDateButtons != null)
            {
                RegisterYearButtons(layout);
            }

            // Register flexible range buttons (for FlexibleRange mode)
            if (layout.FlexibleButtons != null && layout.FlexibleButtons.Count > 0)
            {
                RegisterFlexibleRangeButtons(layout);
            }

            // Register filter buttons (for FilteredRange mode)
            if (layout.FilterButtons != null && layout.FilterButtons.Count > 0)
            {
                RegisterFilterButtons(layout);
            }
            
            // Register FilteredRange-specific areas
            RegisterFilteredRangeAreas(layout, displayMonth, props);
            
            // Register MonthView and YearView specific areas
            RegisterMonthYearViewAreas(layout);

            // Register tab buttons (for modes with tabs)
            RegisterTabButtons(layout);

            // Register today button
            if (props.ShowTodayButton && layout.TodayButtonRect != Rectangle.Empty)
            {
                RegisterTodayButton(layout);
            }
        }

        private void RegisterNavigationButtons(DateTimePickerLayout layout)
        {
            // Previous month button
            if (layout.PreviousButtonRect != Rectangle.Empty)
            {
                string hitName = "nav_previous";
                _hitAreaMap[hitName] = layout.PreviousButtonRect;
                // Registration only; no direct action callback
                _owner._hitTest.AddHitArea(hitName, layout.PreviousButtonRect, null, null);
            }

            // Next month button
            if (layout.NextButtonRect != Rectangle.Empty)
            {
                string hitName = "nav_next";
                _hitAreaMap[hitName] = layout.NextButtonRect;
                // Registration only; no direct action callback
                _owner._hitTest.AddHitArea(hitName, layout.NextButtonRect, null, null);
            }

            // Month/Year header (could be clickable for year/month selection in future)
            if (layout.TitleRect != Rectangle.Empty)
            {
                string hitName = "header_title";
                _hitAreaMap[hitName] = layout.TitleRect;
                _owner._hitTest.AddHitArea(hitName, layout.TitleRect, null, null);
            }
        }

        /// <summary>
        /// Registers day cells for a single calendar grid (traditional single-month layout).
        /// </summary>
        private void RegisterDayCells(DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties props)
        {
            if (layout.DayCellRects == null || layout.DayCellRects.Count == 0) return;

            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);

            // Determine the starting day of week
            int startDayOffset = ((int)firstDayOfMonth.DayOfWeek - (int)props.FirstDayOfWeek + 7) % 7;

            for (int i = 0; i < layout.DayCellRects.Count; i++)
            {
                var cellRect = layout.DayCellRects[i];
                if (cellRect.IsEmpty) continue;

                // Calculate the actual date for this cell
                int dayNumber = i - startDayOffset + 1;

                if (dayNumber >= 1 && dayNumber <= daysInMonth)
                {
                    DateTime cellDate = new DateTime(displayMonth.Year, displayMonth.Month, dayNumber);
                    
                    // Only register if date is within allowed range
                    if (_owner.IsDateInRange(cellDate))
                    {
                        string hitName = $"day_{cellDate:yyyy_MM_dd}";
                        _hitAreaMap[hitName] = cellRect;
                        
                        // Register with no direct action; handler will process clicks
                        _owner._hitTest.AddHitArea(hitName, cellRect, null, null);
                    }
                }
            }
        }

        /// <summary>
        /// Registers day cells for multiple calendar grids (dual-month or multi-month layout for ranges).
        /// Each grid can represent a different month.
        /// </summary>
        private void RegisterMultipleCalendarGrids(DateTimePickerLayout layout, DateTime baseDisplayMonth, DateTimePickerProperties props)
        {
            if (layout.MonthGrids == null || layout.MonthGrids.Count == 0) return;

            for (int gridIndex = 0; gridIndex < layout.MonthGrids.Count; gridIndex++)
            {
                var grid = layout.MonthGrids[gridIndex];
                if (grid == null || grid.DayCellRects == null || grid.DayCellRects.Count == 0) 
                    continue;

                // Calculate the month for this grid
                // Grid 0 = baseDisplayMonth, Grid 1 = baseDisplayMonth + 1 month, etc.
                DateTime gridMonth = baseDisplayMonth.AddMonths(gridIndex);
                var firstDayOfMonth = new DateTime(gridMonth.Year, gridMonth.Month, 1);
                int daysInMonth = DateTime.DaysInMonth(gridMonth.Year, gridMonth.Month);

                // Determine the starting day of week
                int startDayOffset = ((int)firstDayOfMonth.DayOfWeek - (int)props.FirstDayOfWeek + 7) % 7;

                // Register navigation buttons for this grid (if present)
                if (grid.PreviousButtonRect != Rectangle.Empty && gridIndex == 0) // Only first grid can go previous
                {
                    string hitName = $"nav_previous_grid{gridIndex}";
                    _hitAreaMap[hitName] = grid.PreviousButtonRect;
                    _owner._hitTest.AddHitArea(hitName, grid.PreviousButtonRect, null, null);
                }

                if (grid.NextButtonRect != Rectangle.Empty && gridIndex == layout.MonthGrids.Count - 1) // Only last grid can go next
                {
                    string hitName = $"nav_next_grid{gridIndex}";
                    _hitAreaMap[hitName] = grid.NextButtonRect;
                    _owner._hitTest.AddHitArea(hitName, grid.NextButtonRect, null, null);
                }

                // Register title/header for this grid
                if (grid.TitleRect != Rectangle.Empty)
                {
                    string hitName = $"header_title_grid{gridIndex}";
                    _hitAreaMap[hitName] = grid.TitleRect;
                    _owner._hitTest.AddHitArea(hitName, grid.TitleRect, null, null);
                }

                // Register day cells for this grid
                for (int i = 0; i < grid.DayCellRects.Count; i++)
                {
                    var cellRect = grid.DayCellRects[i];
                    if (cellRect.IsEmpty) continue;

                    // Calculate the actual date for this cell
                    int dayNumber = i - startDayOffset + 1;

                    if (dayNumber >= 1 && dayNumber <= daysInMonth)
                    {
                        DateTime cellDate = new DateTime(gridMonth.Year, gridMonth.Month, dayNumber);
                        
                        // Only register if date is within allowed range
                        if (_owner.IsDateInRange(cellDate))
                        {
                            string hitName = $"day_grid{gridIndex}_{cellDate:yyyy_MM_dd}";
                            _hitAreaMap[hitName] = cellRect;
                            
                            _owner._hitTest.AddHitArea(hitName, cellRect, null, null);
                        }
                    }
                }

                // Register week numbers for this grid if present
                if (props.ShowWeekNumbers != DatePickerWeekNumbers.None && grid.WeekNumberRects != null)
                {
                    for (int i = 0; i < grid.WeekNumberRects.Count; i++)
                    {
                        var weekRect = grid.WeekNumberRects[i];
                        if (weekRect.IsEmpty) continue;

                        string hitName = $"week_grid{gridIndex}_{i}";
                        _hitAreaMap[hitName] = weekRect;
                        _owner._hitTest.AddHitArea(hitName, weekRect, null, null);
                    }
                }
            }
        }

        private void RegisterTimeSlots(DateTimePickerLayout layout, DateTimePickerProperties props)
        {
            if (layout.TimeSlotRects == null || layout.TimeSlotRects.Count == 0) return;

            TimeSpan currentTime = TimeSpan.Zero;
            TimeSpan interval = props.TimeInterval;

            for (int i = 0; i < layout.TimeSlotRects.Count; i++)
            {
                var slotRect = layout.TimeSlotRects[i];
                if (slotRect.IsEmpty) continue;

                TimeSpan slotTime = currentTime;
                
                // Only register if time is within allowed range
                if (_owner.IsTimeInRange(slotTime))
                {
                    string hitName = $"time_{slotTime.Hours:D2}_{slotTime.Minutes:D2}";
                    _hitAreaMap[hitName] = slotRect;
                    
                    _owner._hitTest.AddHitArea(hitName, slotRect, null, null);
                }

                currentTime = currentTime.Add(interval);
                if (currentTime >= TimeSpan.FromHours(24)) break;
            }
        }

        private void RegisterQuickButtons(DateTimePickerLayout layout)
        {
            if (layout.QuickButtonRects == null || layout.QuickButtonRects.Count == 0) return;

            var quickButtons = GetQuickButtonLabels();

            for (int i = 0; i < Math.Min(layout.QuickButtonRects.Count, quickButtons.Count); i++)
            {
                var buttonRect = layout.QuickButtonRects[i];
                if (buttonRect.IsEmpty) continue;

                string buttonLabel = quickButtons[i];
                string hitName = $"quick_{buttonLabel.Replace(" ", "_").ToLower()}";
                _hitAreaMap[hitName] = buttonRect;
                
                _owner._hitTest.AddHitArea(hitName, buttonRect, null, null);
            }
        }

        private void RegisterRangeTimeSpinners(DateTimePickerLayout layout)
        {
            RegisterTimeSpinnerArea("time_start_hour_up", layout.StartTimeHourUpRect);
            RegisterTimeSpinnerArea("time_start_hour_down", layout.StartTimeHourDownRect);
            RegisterTimeSpinnerArea("time_start_minute_up", layout.StartTimeMinuteUpRect);
            RegisterTimeSpinnerArea("time_start_minute_down", layout.StartTimeMinuteDownRect);
            RegisterTimeSpinnerArea("time_end_hour_up", layout.EndTimeHourUpRect);
            RegisterTimeSpinnerArea("time_end_hour_down", layout.EndTimeHourDownRect);
            RegisterTimeSpinnerArea("time_end_minute_up", layout.EndTimeMinuteUpRect);
            RegisterTimeSpinnerArea("time_end_minute_down", layout.EndTimeMinuteDownRect);
        }

        private void RegisterTimeSpinnerArea(string name, Rectangle rect)
        {
            if (rect == Rectangle.Empty)
                return;

            _hitAreaMap[name] = rect;
            _owner._hitTest.AddHitArea(name, rect, null, null);
        }

        private void RegisterClearButton(DateTimePickerLayout layout)
        {
            if (layout.ClearButtonRect.IsEmpty) return;

            string hitName = "clear_button";
            _hitAreaMap[hitName] = layout.ClearButtonRect;
            _owner._hitTest.AddHitArea(hitName, layout.ClearButtonRect, null, null);
        }

        private void RegisterWeekNumbers(DateTimePickerLayout layout)
        {
            if (layout.WeekNumberRects == null || layout.WeekNumberRects.Count == 0) return;

            for (int i = 0; i < layout.WeekNumberRects.Count; i++)
            {
                var weekRect = layout.WeekNumberRects[i];
                if (weekRect.IsEmpty) continue;

                string hitName = $"week_{i}";
                _hitAreaMap[hitName] = weekRect;
                _owner._hitTest.AddHitArea(hitName, weekRect, null, null);
            }
        }

        private void RegisterActionButtons(DateTimePickerLayout layout)
        {
            // Apply button
            if (layout.ApplyButtonRect != Rectangle.Empty)
            {
                string hitName = "button_apply";
                _hitAreaMap[hitName] = layout.ApplyButtonRect;
                _owner._hitTest.AddHitArea(hitName, layout.ApplyButtonRect, null, null);
            }

            // Cancel button
            if (layout.CancelButtonRect != Rectangle.Empty)
            {
                string hitName = "button_cancel";
                _hitAreaMap[hitName] = layout.CancelButtonRect;
                _owner._hitTest.AddHitArea(hitName, layout.CancelButtonRect, null, null);
            }
        }

        private void RegisterQuarterButtons(DateTimePickerLayout layout)
        {
            if (layout.QuickDateButtons == null || layout.QuickDateButtons.Count == 0) return;

            foreach (var button in layout.QuickDateButtons)
            {
                if (button.Bounds.IsEmpty) continue;

                // Quarter buttons have keys like "Q1", "Q2", "Q3", "Q4"
                if (button.Key.StartsWith("Q"))
                {
                    string hitName = $"quarter_{button.Key}";
                    _hitAreaMap[hitName] = button.Bounds;
                    _owner._hitTest.AddHitArea(hitName, button.Bounds, null, null);
                }
            }
        }

        private void RegisterMonthButtons(DateTimePickerLayout layout)
        {
            if (layout.QuickDateButtons == null || layout.QuickDateButtons.Count == 0) return;

            for (int i = 0; i < layout.QuickDateButtons.Count; i++)
            {
                var button = layout.QuickDateButtons[i];
                if (button.Bounds.IsEmpty) continue;

                // Month buttons have numeric keys 0-11 or month names
                string hitName = $"month_{i}";
                _hitAreaMap[hitName] = button.Bounds;
                _owner._hitTest.AddHitArea(hitName, button.Bounds, null, null);
            }
        }

        private void RegisterYearButtons(DateTimePickerLayout layout)
        {
            if (layout.QuickDateButtons == null || layout.QuickDateButtons.Count == 0) return;

            foreach (var button in layout.QuickDateButtons)
            {
                if (button.Bounds.IsEmpty) continue;

                // Year buttons have year as key (e.g., "2025")
                if (int.TryParse(button.Key, out int year))
                {
                    string hitName = $"year_{button.Key}";
                    _hitAreaMap[hitName] = button.Bounds;
                    _owner._hitTest.AddHitArea(hitName, button.Bounds, null, null);
                }
            }
        }

        private void RegisterFlexibleRangeButtons(DateTimePickerLayout layout)
        {
            if (layout.FlexibleButtons == null || layout.FlexibleButtons.Count == 0) return;

            foreach (var kvp in layout.FlexibleButtons)
            {
                if (kvp.Value.IsEmpty) continue;

                string hitName = $"flexible_{kvp.Key.Replace(" ", "_").ToLower()}";
                _hitAreaMap[hitName] = kvp.Value;
                _owner._hitTest.AddHitArea(hitName, kvp.Value, null, null);
            }
        }

        private void RegisterFilterButtons(DateTimePickerLayout layout)
        {
            if (layout.FilterButtons == null || layout.FilterButtons.Count == 0) return;

            foreach (var kvp in layout.FilterButtons)
            {
                if (kvp.Value.IsEmpty) continue;

                string hitName = $"filter_{kvp.Key.Replace(" ", "_").ToLower()}";
                _hitAreaMap[hitName] = kvp.Value;
                _owner._hitTest.AddHitArea(hitName, kvp.Value, null, null);
            }
        }

        private void RegisterFilteredRangeAreas(DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties props)
        {
            // Sidebar
            if (layout.SidebarRect != Rectangle.Empty)
            {
                string hitName = "sidebar";
                _hitAreaMap[hitName] = layout.SidebarRect;
                _owner._hitTest.AddHitArea(hitName, layout.SidebarRect, null, null);
            }

            if (layout.FilterTitleRect != Rectangle.Empty)
            {
                string hitName = "filter_title";
                _hitAreaMap[hitName] = layout.FilterTitleRect;
                _owner._hitTest.AddHitArea(hitName, layout.FilterTitleRect, null, null);
            }

            // FilterButtonRects list (6 filter buttons)
            if (layout.FilterButtonRects != null)
            {
                for (int i = 0; i < layout.FilterButtonRects.Count; i++)
                {
                    if (layout.FilterButtonRects[i] == Rectangle.Empty) continue;
                    string hitName = $"filter_button_{i}";
                    _hitAreaMap[hitName] = layout.FilterButtonRects[i];
                    _owner._hitTest.AddHitArea(hitName, layout.FilterButtonRects[i], null, null);
                }
            }

            // Left calendar areas
            if (layout.LeftYearDropdownRect != Rectangle.Empty)
            {
                string hitName = "left_year_dropdown";
                _hitAreaMap[hitName] = layout.LeftYearDropdownRect;
                _owner._hitTest.AddHitArea(hitName, layout.LeftYearDropdownRect, null, null);
            }

            if (layout.LeftHeaderRect != Rectangle.Empty)
            {
                string hitName = "left_header";
                _hitAreaMap[hitName] = layout.LeftHeaderRect;
                _owner._hitTest.AddHitArea(hitName, layout.LeftHeaderRect, null, null);
            }

            if (layout.LeftDayNamesRect != Rectangle.Empty)
            {
                string hitName = "left_day_names";
                _hitAreaMap[hitName] = layout.LeftDayNamesRect;
                _owner._hitTest.AddHitArea(hitName, layout.LeftDayNamesRect, null, null);
            }

            if (layout.LeftCalendarGridRect != Rectangle.Empty)
            {
                string hitName = "left_calendar_grid";
                _hitAreaMap[hitName] = layout.LeftCalendarGridRect;
                _owner._hitTest.AddHitArea(hitName, layout.LeftCalendarGridRect, null, null);
            }

            // Left calendar day cells (42 cells)
            if (layout.LeftDayCellRects != null)
            {
                for (int i = 0; i < layout.LeftDayCellRects.Count; i++)
                {
                    if (layout.LeftDayCellRects[i] == Rectangle.Empty) continue;
                    string hitName = $"left_day_{i}";
                    _hitAreaMap[hitName] = layout.LeftDayCellRects[i];
                    _owner._hitTest.AddHitArea(hitName, layout.LeftDayCellRects[i], null, null);
                }
            }

            // Right calendar areas
            if (layout.RightYearDropdownRect != Rectangle.Empty)
            {
                string hitName = "right_year_dropdown";
                _hitAreaMap[hitName] = layout.RightYearDropdownRect;
                _owner._hitTest.AddHitArea(hitName, layout.RightYearDropdownRect, null, null);
            }

            if (layout.RightHeaderRect != Rectangle.Empty)
            {
                string hitName = "right_header";
                _hitAreaMap[hitName] = layout.RightHeaderRect;
                _owner._hitTest.AddHitArea(hitName, layout.RightHeaderRect, null, null);
            }

            if (layout.RightDayNamesRect != Rectangle.Empty)
            {
                string hitName = "right_day_names";
                _hitAreaMap[hitName] = layout.RightDayNamesRect;
                _owner._hitTest.AddHitArea(hitName, layout.RightDayNamesRect, null, null);
            }

            if (layout.RightCalendarGridRect != Rectangle.Empty)
            {
                string hitName = "right_calendar_grid";
                _hitAreaMap[hitName] = layout.RightCalendarGridRect;
                _owner._hitTest.AddHitArea(hitName, layout.RightCalendarGridRect, null, null);
            }

            // Right calendar day cells (42 cells)
            if (layout.RightDayCellRects != null)
            {
                for (int i = 0; i < layout.RightDayCellRects.Count; i++)
                {
                    if (layout.RightDayCellRects[i] == Rectangle.Empty) continue;
                    string hitName = $"right_day_{i}";
                    _hitAreaMap[hitName] = layout.RightDayCellRects[i];
                    _owner._hitTest.AddHitArea(hitName, layout.RightDayCellRects[i], null, null);
                }
            }

            // Time picker areas
            if (layout.TimePickerRowRect != Rectangle.Empty)
            {
                string hitName = "time_picker_row";
                _hitAreaMap[hitName] = layout.TimePickerRowRect;
                _owner._hitTest.AddHitArea(hitName, layout.TimePickerRowRect, null, null);
            }

            if (layout.FromLabelRect != Rectangle.Empty)
            {
                string hitName = "from_label";
                _hitAreaMap[hitName] = layout.FromLabelRect;
                _owner._hitTest.AddHitArea(hitName, layout.FromLabelRect, null, null);
            }

            if (layout.FromTimeInputRect != Rectangle.Empty)
            {
                string hitName = "from_time_input";
                _hitAreaMap[hitName] = layout.FromTimeInputRect;
                _owner._hitTest.AddHitArea(hitName, layout.FromTimeInputRect, null, null);
            }

            if (layout.ToLabelRect != Rectangle.Empty)
            {
                string hitName = "to_label";
                _hitAreaMap[hitName] = layout.ToLabelRect;
                _owner._hitTest.AddHitArea(hitName, layout.ToLabelRect, null, null);
            }

            if (layout.ToTimeInputRect != Rectangle.Empty)
            {
                string hitName = "to_time_input";
                _hitAreaMap[hitName] = layout.ToTimeInputRect;
                _owner._hitTest.AddHitArea(hitName, layout.ToTimeInputRect, null, null);
            }

            // Action buttons
            if (layout.ActionButtonRowRect != Rectangle.Empty)
            {
                string hitName = "action_button_row";
                _hitAreaMap[hitName] = layout.ActionButtonRowRect;
                _owner._hitTest.AddHitArea(hitName, layout.ActionButtonRowRect, null, null);
            }

            if (layout.ResetButtonRect != Rectangle.Empty)
            {
                string hitName = "reset_button";
                _hitAreaMap[hitName] = layout.ResetButtonRect;
                _owner._hitTest.AddHitArea(hitName, layout.ResetButtonRect, null, null);
            }

            if (layout.ShowResultsButtonRect != Rectangle.Empty)
            {
                string hitName = "show_results_button";
                _hitAreaMap[hitName] = layout.ShowResultsButtonRect;
                _owner._hitTest.AddHitArea(hitName, layout.ShowResultsButtonRect, null, null);
            }

            // Additional areas
            if (layout.MainContentRect != Rectangle.Empty)
            {
                string hitName = "main_content";
                _hitAreaMap[hitName] = layout.MainContentRect;
                _owner._hitTest.AddHitArea(hitName, layout.MainContentRect, null, null);
            }

            if (layout.DualCalendarContainerRect != Rectangle.Empty)
            {
                string hitName = "dual_calendar_container";
                _hitAreaMap[hitName] = layout.DualCalendarContainerRect;
                _owner._hitTest.AddHitArea(hitName, layout.DualCalendarContainerRect, null, null);
            }
        }

        private void RegisterMonthYearViewAreas(DateTimePickerLayout layout)
        {
            // MonthView specific areas
            if (layout.PreviousYearButtonRect != Rectangle.Empty)
            {
                string hitName = "previous_year_button";
                _hitAreaMap[hitName] = layout.PreviousYearButtonRect;
                _owner._hitTest.AddHitArea(hitName, layout.PreviousYearButtonRect, null, null);
            }

            if (layout.NextYearButtonRect != Rectangle.Empty)
            {
                string hitName = "next_year_button";
                _hitAreaMap[hitName] = layout.NextYearButtonRect;
                _owner._hitTest.AddHitArea(hitName, layout.NextYearButtonRect, null, null);
            }

            // MonthCellRects list (12 month cells)
            if (layout.MonthCellRects != null)
            {
                for (int i = 0; i < layout.MonthCellRects.Count; i++)
                {
                    if (layout.MonthCellRects[i] == Rectangle.Empty) continue;
                    string hitName = $"month_cell_{i}";
                    _hitAreaMap[hitName] = layout.MonthCellRects[i];
                    _owner._hitTest.AddHitArea(hitName, layout.MonthCellRects[i], null, null);
                }
            }

            // YearView specific areas
            if (layout.PreviousDecadeButtonRect != Rectangle.Empty)
            {
                string hitName = "previous_decade_button";
                _hitAreaMap[hitName] = layout.PreviousDecadeButtonRect;
                _owner._hitTest.AddHitArea(hitName, layout.PreviousDecadeButtonRect, null, null);
            }

            if (layout.NextDecadeButtonRect != Rectangle.Empty)
            {
                string hitName = "next_decade_button";
                _hitAreaMap[hitName] = layout.NextDecadeButtonRect;
                _owner._hitTest.AddHitArea(hitName, layout.NextDecadeButtonRect, null, null);
            }

            // YearCellRects list (12 year cells)
            if (layout.YearCellRects != null)
            {
                for (int i = 0; i < layout.YearCellRects.Count; i++)
                {
                    if (layout.YearCellRects[i] == Rectangle.Empty) continue;
                    string hitName = $"year_cell_{i}";
                    _hitAreaMap[hitName] = layout.YearCellRects[i];
                    _owner._hitTest.AddHitArea(hitName, layout.YearCellRects[i], null, null);
                }
            }
        }

        private void RegisterTabButtons(DateTimePickerLayout layout)
        {
            // Tab buttons (for FlexibleRange and other multi-tab modes)
            if (layout.TabExactRect != Rectangle.Empty)
            {
                string hitName = "tab_exact";
                _hitAreaMap[hitName] = layout.TabExactRect;
                _owner._hitTest.AddHitArea(hitName, layout.TabExactRect, null, null);
            }

            if (layout.TabFlexibleRect != Rectangle.Empty)
            {
                string hitName = "tab_flexible";
                _hitAreaMap[hitName] = layout.TabFlexibleRect;
                _owner._hitTest.AddHitArea(hitName, layout.TabFlexibleRect, null, null);
            }
        }

        private void RegisterTodayButton(DateTimePickerLayout layout)
        {
            if (layout.TodayButtonRect.IsEmpty) return;

            string hitName = "button_today";
            _hitAreaMap[hitName] = layout.TodayButtonRect;
            _owner._hitTest.AddHitArea(hitName, layout.TodayButtonRect, null, null);
        }

        #endregion

        // No click handlers here by design. This helper only registers areas and
        // provides hit test name/rect lookups. Actual click handling is performed
        // by the painter-specific IDateTimePickerHitHandler implementations.

        #region Hit Testing

        /// <summary>
        /// Performs hit test and returns the hit area information.
        /// </summary>
        public bool HitTest(Point point, out string hitName, out Rectangle hitRect)
        {
            hitName = string.Empty;
            hitRect = Rectangle.Empty;

            if (!_owner._hitTest.HitTest(point, out var hit)) 
                return false;

            hitName = hit.Name;
            
            if (_hitAreaMap.TryGetValue(hitName, out var rect))
            {
                hitRect = rect;
            }
            else
            {
                hitRect = hit.TargetRect;
            }

            return true;
        }

        /// <summary>
        /// Gets the rectangle for a specific hit area by name.
        /// </summary>
        public Rectangle GetHitAreaRect(string hitName)
        {
            return _hitAreaMap.TryGetValue(hitName, out var rect) ? rect : Rectangle.Empty;
        }

        /// <summary>
        /// Checks if a specific hit area exists.
        /// </summary>
        public bool HasHitArea(string hitName)
        {
            return _hitAreaMap.ContainsKey(hitName);
        }

        /// <summary>
        /// Gets all registered hit area names.
        /// </summary>
        public IEnumerable<string> GetAllHitAreaNames()
        {
            return _hitAreaMap.Keys;
        }

        /// <summary>
        /// Gets the hit area map for debugging or advanced scenarios.
        /// </summary>
        public IReadOnlyDictionary<string, Rectangle> GetHitAreaMap()
        {
            return _hitAreaMap;
        }

        #endregion

        #region Helper Methods

        private List<string> GetQuickButtonLabels()
        {
            var labels = new List<string>();

            // Get properties to determine which quick buttons to show
            var props = _owner.GetCurrentProperties();

            if (props.ShowTodayButton)
                labels.Add("Today");

            if (props.ShowTomorrowButton)
                labels.Add("Tomorrow");

            if (props.ShowCustomQuickDates)
            {
                labels.Add("Yesterday");
                labels.Add("This Week");
                labels.Add("This Month");
            }

            return labels;
        }

        #endregion
    }
}
