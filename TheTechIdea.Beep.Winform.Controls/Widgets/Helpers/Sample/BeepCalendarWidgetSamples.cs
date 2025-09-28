using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepCalendarWidget with all calendar styles
    /// </summary>
    public static class BeepCalendarWidgetSamples
    {
        /// <summary>
        /// Creates a date grid calendar widget
        /// Uses DateGridPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateDateGridWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.DateGrid,
                Title = "Date Selector",
                Subtitle = "Pick a Date",
                SelectedDate = DateTime.Today.AddDays(5),
                DisplayMonth = DateTime.Today,
                ShowWeekends = true,
                ShowToday = true,
                Size = new Size(280, 200),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a time slots calendar widget
        /// Uses TimeSlotsPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateTimeSlotsWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.TimeSlots,
                Title = "Available Times",
                Subtitle = "Book Appointment",
                SelectedDate = DateTime.Today.AddDays(1),
                WorkingHoursStart = 9,
                WorkingHoursEnd = 17,
                Size = new Size(300, 250),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates an event card calendar widget
        /// Uses EventCardPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateEventCardWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.EventCard,
                Title = "Next Event",
                Subtitle = "Upcoming Schedule",
                SelectedDate = DateTime.Today,
                ShowEvents = true,
                Size = new Size(320, 180),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a full calendar view widget
        /// Uses CalendarViewPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateCalendarViewWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.CalendarView,
                Title = "Monthly Calendar",
                Subtitle = "Schedule Overview",
                SelectedDate = DateTime.Today,
                DisplayMonth = DateTime.Today,
                ShowEvents = true,
                ShowToday = true,
                ShowWeekends = true,
                Size = new Size(400, 350),
                AccentColor = Color.FromArgb(156, 39, 176)
            };
        }

        /// <summary>
        /// Creates a schedule card calendar widget
        /// Uses ScheduleCardPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateScheduleCardWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.ScheduleCard,
                Title = "Daily Schedule",
                Subtitle = "Today's Agenda",
                SelectedDate = DateTime.Today,
                ViewMode = CalendarViewMode.Day,
                ShowEvents = true,
                Size = new Size(300, 200),
                AccentColor = Color.FromArgb(244, 67, 54)
            };
        }

        /// <summary>
        /// Creates a date picker calendar widget
        /// Uses DatePickerPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateDatePickerWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.DatePicker,
                Title = "Select Date",
                Subtitle = "Date Input",
                SelectedDate = DateTime.Today.AddDays(3),
                Size = new Size(280, 120),
                AccentColor = Color.FromArgb(255, 152, 0)
            };
        }

        /// <summary>
        /// Creates a timeline view calendar widget
        /// Uses TimelineViewPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateTimelineViewWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.TimelineView,
                Title = "Project Timeline",
                Subtitle = "Milestone Tracker",
                SelectedDate = DateTime.Today,
                ViewMode = CalendarViewMode.Week,
                ShowEvents = true,
                Size = new Size(400, 200),
                AccentColor = Color.FromArgb(103, 58, 183)
            };
        }

        /// <summary>
        /// Creates a week view calendar widget
        /// Uses WeekViewPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateWeekViewWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.WeekView,
                Title = "Week Schedule",
                Subtitle = "Weekly Overview",
                SelectedDate = DateTime.Today,
                ViewMode = CalendarViewMode.Week,
                ShowEvents = true,
                ShowWeekends = true,
                Size = new Size(450, 250),
                AccentColor = Color.FromArgb(0, 150, 136)
            };
        }

        /// <summary>
        /// Creates an event list calendar widget
        /// Uses EventListPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateEventListWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.EventList,
                Title = "Upcoming Events",
                Subtitle = "Next 7 Days",
                SelectedDate = DateTime.Today,
                ShowEvents = true,
                Size = new Size(320, 280),
                AccentColor = Color.FromArgb(63, 81, 181)
            };
        }

        /// <summary>
        /// Creates an availability grid calendar widget
        /// Uses AvailabilityGridPainter.cs
        /// </summary>
        public static BeepCalendarWidget CreateAvailabilityGridWidget()
        {
            return new BeepCalendarWidget
            {
                Style = CalendarWidgetStyle.AvailabilityGrid,
                Title = "Availability",
                Subtitle = "Booking Calendar",
                SelectedDate = DateTime.Today,
                WorkingHoursStart = 8,
                WorkingHoursEnd = 18,
                ShowWeekends = false,
                Size = new Size(350, 300),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Gets all calendar widget samples
        /// </summary>
        public static BeepCalendarWidget[] GetAllSamples()
        {
            return new BeepCalendarWidget[]
            {
                CreateDateGridWidget(),
                CreateTimeSlotsWidget(),
                CreateEventCardWidget(),
                CreateCalendarViewWidget(),
                CreateScheduleCardWidget(),
                CreateDatePickerWidget(),
                CreateTimelineViewWidget(),
                CreateWeekViewWidget(),
                CreateEventListWidget(),
                CreateAvailabilityGridWidget()
            };
        }
    }
}