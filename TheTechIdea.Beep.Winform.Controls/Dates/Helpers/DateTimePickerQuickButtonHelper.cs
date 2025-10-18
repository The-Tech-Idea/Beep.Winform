using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Helpers
{
    /// <summary>
    /// Provides consistent quick button definitions (Today, Tomorrow, etc.) for painters and hit handlers.
    /// Guarantees uniform ordering so layouts, painters, and hit test logic stay in sync.
    /// </summary>
    internal static class DateTimePickerQuickButtonHelper
    {
        internal sealed class QuickButtonDefinition
        {
            public QuickButtonDefinition(string id, string label, DateTime targetDate)
            {
                Id = id;
                Label = label;
                TargetDate = targetDate;
            }

            /// <summary>
            /// Unique id used for hit testing (e.g., quick_today).
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// Display text shown on the button.
            /// </summary>
            public string Label { get; }

            /// <summary>
            /// Representative target date (start of week/month for range-oriented buttons).
            /// </summary>
            public DateTime TargetDate { get; }
        }

        /// <summary>
        /// Builds quick button definitions honoring picker properties (visibility, custom labels, first day of week).
        /// </summary>
        internal static List<QuickButtonDefinition> GetQuickButtonDefinitions(DateTimePickerProperties properties)
        {
            var props = properties ?? new DateTimePickerProperties();
            var buttons = new List<QuickButtonDefinition>();
            var today = DateTime.Today;

            if (props.ShowTodayButton)
            {
                buttons.Add(new QuickButtonDefinition(
                    "quick_today",
                    string.IsNullOrWhiteSpace(props.TodayButtonText) ? "Today" : props.TodayButtonText,
                    today));
            }

            if (props.ShowTomorrowButton)
            {
                buttons.Add(new QuickButtonDefinition(
                    "quick_tomorrow",
                    string.IsNullOrWhiteSpace(props.TomorrowButtonText) ? "Tomorrow" : props.TomorrowButtonText,
                    today.AddDays(1)));
            }

            if (props.ShowCustomQuickDates)
            {
                buttons.Add(new QuickButtonDefinition(
                    "quick_yesterday",
                    string.IsNullOrWhiteSpace(props.YesterdayButtonText) ? "Yesterday" : props.YesterdayButtonText,
                    today.AddDays(-1)));

                var startOfWeek = GetStartOfWeek(today, props.FirstDayOfWeek);
                buttons.Add(new QuickButtonDefinition(
                    "quick_this_week",
                    "This Week",
                    startOfWeek));

                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                buttons.Add(new QuickButtonDefinition(
                    "quick_this_month",
                    "This Month",
                    startOfMonth));
            }

            return buttons;
        }

        private static DateTime GetStartOfWeek(DateTime anchor, DatePickerFirstDayOfWeek firstDayOfWeek)
        {
            var startOfWeek = (DayOfWeek)(int)firstDayOfWeek;
            int diff = ((int)anchor.DayOfWeek - (int)startOfWeek + 7) % 7;
            return anchor.AddDays(-diff).Date;
        }
    }
}
