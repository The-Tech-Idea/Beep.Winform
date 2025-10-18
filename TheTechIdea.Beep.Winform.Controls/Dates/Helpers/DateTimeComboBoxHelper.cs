using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Helpers
{
    /// <summary>
    /// Helper class for creating and managing BeepComboBox controls
    /// for DateTimePicker components (years, months, hours, minutes)
    /// </summary>
    public static class DateTimeComboBoxHelper
    {
        /// <summary>
        /// Creates a BeepComboBox for year selection
        /// </summary>
        /// <param name="minYear">Minimum year to display</param>
        /// <param name="maxYear">Maximum year to display</param>
        /// <param name="selectedYear">Currently selected year</param>
        /// <returns>Configured BeepComboBox for year selection</returns>
        public static BeepComboBox CreateYearComboBox(int minYear, int maxYear, int selectedYear)
        {
            var comboBox = new BeepComboBox
            {
                ComboBoxType = ComboBoxType.Standard,
                IsEditable = false,
                Width = 120,
                Height = 32
            };

            // Generate year list
            var items = new BindingList<SimpleItem>();
            for (int year = minYear; year <= maxYear; year++)
            {
                items.Add(new SimpleItem
                {
                    ID = year,
                    Text = year.ToString(),
                    Name = year.ToString(),
                    Item = year
                });
            }

            comboBox.ListItems = items;

            // Set selected year
            if (selectedYear >= minYear && selectedYear <= maxYear)
            {
                comboBox.SelectItemByValue(selectedYear);
            }

            return comboBox;
        }

        /// <summary>
        /// Creates a BeepComboBox for month selection
        /// </summary>
        /// <param name="selectedMonth">Currently selected month (1-12)</param>
        /// <param name="useFullNames">Use full month names vs abbreviated</param>
        /// <returns>Configured BeepComboBox for month selection</returns>
        public static BeepComboBox CreateMonthComboBox(int selectedMonth, bool useFullNames = true)
        {
            var comboBox = new BeepComboBox
            {
                ComboBoxType = ComboBoxType.Standard,
                IsEditable = false,
                Width = 150,
                Height = 32
            };

            // Get month names from culture
            var monthNames = useFullNames
                ? System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                : System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;

            // Add months (excluding empty 13th element)
            var items = new BindingList<SimpleItem>();
            for (int i = 0; i < 12; i++)
            {
                items.Add(new SimpleItem
                {
                    ID = i + 1, // 1-based month index
                    Text = monthNames[i],
                    Name = monthNames[i],
                    Item = i + 1
                });
            }

            comboBox.ListItems = items;

            // Set selected month
            if (selectedMonth >= 1 && selectedMonth <= 12)
            {
                comboBox.SelectItemByValue(selectedMonth);
            }

            return comboBox;
        }

        /// <summary>
        /// Creates a BeepComboBox for hour selection
        /// </summary>
        /// <param name="is24Hour">Use 24-hour format vs 12-hour AM/PM</param>
        /// <param name="selectedHour">Currently selected hour (0-23)</param>
        /// <returns>Configured BeepComboBox for hour selection</returns>
        public static BeepComboBox CreateHourComboBox(bool is24Hour, int selectedHour)
        {
            var comboBox = new BeepComboBox
            {
                ComboBoxType = ComboBoxType.Standard,
                IsEditable = false,
                Width = 80,
                Height = 32
            };

            var items = new BindingList<SimpleItem>();

            if (is24Hour)
            {
                // 24-hour format: 00-23
                for (int hour = 0; hour < 24; hour++)
                {
                    items.Add(new SimpleItem
                    {
                        ID = hour,
                        Text = hour.ToString("D2"),
                        Name = hour.ToString("D2"),
                        Item = hour
                    });
                }

                comboBox.ListItems = items;

                if (selectedHour >= 0 && selectedHour < 24)
                {
                    comboBox.SelectItemByValue(selectedHour);
                }
            }
            else
            {
                // 12-hour format: 12, 1-11 AM/PM
                for (int hour = 0; hour < 24; hour++)
                {
                    int displayHour = hour == 0 ? 12 : (hour > 12 ? hour - 12 : hour);
                    string period = hour < 12 ? "AM" : "PM";
                    items.Add(new SimpleItem
                    {
                        ID = hour,
                        Text = $"{displayHour:D2} {period}",
                        Name = $"{displayHour:D2} {period}",
                        Item = hour
                    });
                }

                comboBox.ListItems = items;

                if (selectedHour >= 0 && selectedHour < 24)
                {
                    comboBox.SelectItemByValue(selectedHour);
                }
            }

            return comboBox;
        }

        /// <summary>
        /// Creates a BeepComboBox for minute selection
        /// </summary>
        /// <param name="interval">Minute interval (e.g., 1, 5, 15, 30)</param>
        /// <param name="selectedMinute">Currently selected minute (0-59)</param>
        /// <returns>Configured BeepComboBox for minute selection</returns>
        public static BeepComboBox CreateMinuteComboBox(int interval, int selectedMinute)
        {
            if (interval <= 0 || interval > 60)
                throw new ArgumentException("Interval must be between 1 and 60", nameof(interval));

            var comboBox = new BeepComboBox
            {
                ComboBoxType = ComboBoxType.Standard,
                IsEditable = false,
                Width = 80,
                Height = 32
            };

            var items = new BindingList<SimpleItem>();

            // Generate minute intervals
            for (int minute = 0; minute < 60; minute += interval)
            {
                items.Add(new SimpleItem
                {
                    ID = minute,
                    Text = minute.ToString("D2"),
                    Name = minute.ToString("D2"),
                    Item = minute
                });
            }

            comboBox.ListItems = items;

            // Find closest minute to selected
            int closestMinute = 0;
            int minDiff = 60;
            for (int minute = 0; minute < 60; minute += interval)
            {
                int diff = Math.Abs(minute - selectedMinute);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestMinute = minute;
                }
            }

            comboBox.SelectItemByValue(closestMinute);

            return comboBox;
        }

        /// <summary>
        /// Creates a BeepComboBox for fiscal year selection
        /// </summary>
        /// <param name="currentYear">Current year</param>
        /// <param name="rangeYears">Number of years before and after current year</param>
        /// <param name="selectedYear">Currently selected fiscal year</param>
        /// <returns>Configured BeepComboBox for fiscal year selection</returns>
        public static BeepComboBox CreateFiscalYearComboBox(int currentYear, int rangeYears, int selectedYear)
        {
            int minYear = currentYear - rangeYears;
            int maxYear = currentYear + rangeYears;

            var comboBox = new BeepComboBox
            {
                ComboBoxType = ComboBoxType.Standard,
                IsEditable = false,
                Width = 120,
                Height = 32
            };

            var items = new BindingList<SimpleItem>();

            // Generate fiscal year list with "FY" prefix
            for (int year = minYear; year <= maxYear; year++)
            {
                items.Add(new SimpleItem
                {
                    ID = year,
                    Text = $"FY {year}",
                    Name = $"FY {year}",
                    Item = year
                });
            }

            comboBox.ListItems = items;

            // Set selected year
            if (selectedYear >= minYear && selectedYear <= maxYear)
            {
                comboBox.SelectItemByValue(selectedYear);
            }

            return comboBox;
        }

        /// <summary>
        /// Creates a BeepComboBox for decade selection
        /// </summary>
        /// <param name="currentDecade">Current decade start year (e.g., 2020)</param>
        /// <param name="rangeDecades">Number of decades before and after current</param>
        /// <param name="selectedDecade">Currently selected decade start year</param>
        /// <returns>Configured BeepComboBox for decade selection</returns>
        public static BeepComboBox CreateDecadeComboBox(int currentDecade, int rangeDecades, int selectedDecade)
        {
            var comboBox = new BeepComboBox
            {
                ComboBoxType = ComboBoxType.Standard,
                IsEditable = false,
                Width = 120,
                Height = 32
            };

            var items = new BindingList<SimpleItem>();

            // Generate decade list
            int startDecade = currentDecade - (rangeDecades * 10);
            int endDecade = currentDecade + (rangeDecades * 10);

            for (int decade = startDecade; decade <= endDecade; decade += 10)
            {
                items.Add(new SimpleItem
                {
                    ID = decade,
                    Text = $"{decade}–{decade + 9}",
                    Name = $"{decade}–{decade + 9}",
                    Item = decade
                });
            }

            comboBox.ListItems = items;

            // Set selected decade
            comboBox.SelectItemByValue(selectedDecade);

            return comboBox;
        }

        /// <summary>
        /// Gets the selected year value from a year ComboBox
        /// </summary>
        public static int? GetSelectedYear(BeepComboBox comboBox)
        {
            if (comboBox?.SelectedItem == null)
                return null;

            // The Item property contains the actual year value
            if (comboBox.SelectedItem.Item is int year)
                return year;

            return null;
        }

        /// <summary>
        /// Gets the selected month value (1-12) from a month ComboBox
        /// </summary>
        public static int? GetSelectedMonth(BeepComboBox comboBox)
        {
            if (comboBox?.SelectedItem == null)
                return null;

            // The Item property contains the actual month value (1-12)
            if (comboBox.SelectedItem.Item is int month)
                return month;

            return null;
        }

        /// <summary>
        /// Gets the selected hour value (0-23) from an hour ComboBox
        /// </summary>
        public static int? GetSelectedHour(BeepComboBox comboBox, bool is24Hour)
        {
            if (comboBox?.SelectedItem == null)
                return null;

            // The Item property contains the actual hour value (0-23)
            if (comboBox.SelectedItem.Item is int hour)
                return hour;

            return null;
        }

        /// <summary>
        /// Gets the selected minute value (0-59) from a minute ComboBox
        /// </summary>
        public static int? GetSelectedMinute(BeepComboBox comboBox)
        {
            if (comboBox?.SelectedItem == null)
                return null;

            // The Item property contains the actual minute value
            if (comboBox.SelectedItem.Item is int minute)
                return minute;

            return null;
        }

        /// <summary>
        /// Gets the selected decade start year from a decade ComboBox
        /// </summary>
        public static int? GetSelectedDecade(BeepComboBox comboBox)
        {
            if (comboBox?.SelectedItem == null)
                return null;

            // The Item property contains the actual decade start year
            if (comboBox.SelectedItem.Item is int decade)
                return decade;

            return null;
        }
    }
}
