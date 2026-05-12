using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar; // for CalendarViewMode, CalendarEvent

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    // Holds current calendar state
    internal class CalendarState
    {
        public DateTime CurrentDate { get; set; } = DateTime.Today;
        public CalendarViewMode ViewMode { get; set; } = CalendarViewMode.Month;
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public CalendarEvent SelectedEvent { get; set; }
        public DateTime? VisibleRangeStart { get; set; }
        public DateTime? VisibleRangeEnd { get; set; }
        public DateTime? HoveredDate { get; set; }
        public int? HoveredEventId { get; set; }
        public DateTime FocusedDate { get; set; } = DateTime.Today;
        public bool IsKeyboardFocusVisible { get; set; }
        public bool IsPointerDown { get; set; }
        public Point? PointerDownLocation { get; set; }
        public CalendarInteractionMode InteractionMode { get; set; } = CalendarInteractionMode.None;
        public CalendarInteractionTargetKind InteractionTargetKind { get; set; } = CalendarInteractionTargetKind.None;
        public int? InteractionEventId { get; set; }
        public bool ShowSidebar { get; set; } = false; // Default to false to avoid empty space
    }

    // Rectangles computed by layout
    internal class CalendarRects
    {
        public Rectangle HeaderRect;
        public Rectangle ViewSelectorRect;
        public Rectangle NavigationRect; // reserved in case we want separate area
        public Rectangle CalendarGridRect;
        public Rectangle SidebarRect;
    }

    // Metrics and constants
    internal static class CalendarLayoutMetrics
    {
        public const int HeaderHeight = CalendarTokens.HeaderHeight;
        public const int BaseViewSelectorHeight = CalendarTokens.ViewSelectorHeight;
        public const int SelectorVerticalPadding = CalendarTokens.SelectorVerticalPadding;
        public const int SectionSpacing = CalendarTokens.SectionSpacing;
        public const int OuterMargin = CalendarTokens.OuterMargin;
        public const int ControlSpacing = CalendarTokens.ControlSpacing;
        public const int HeaderTextSpacingFromNav = CalendarTokens.HeaderTextSpacing;
        public const int HeaderRightPadding = CalendarTokens.HeaderRightPadding;
        public const int EventInsetX = CalendarTokens.EventInsetX;
        public const int EventInsetY = CalendarTokens.EventInsetY;
        public const int MinEventHitHeight = CalendarTokens.MinEventHitHeight;
        public const int EventCornerRadius = CalendarTokens.EventCornerRadius;
        public const int EventAccentWidth = CalendarTokens.EventAccentWidth;
        public const int MaxEventsPerCell = CalendarTokens.MaxEventsPerCell;
        public const int ListRowHeight = CalendarTokens.ListRowHeight;
        public const int ListRowSpacing = CalendarTokens.ListRowSpacing;
        public const int SidebarPadding = CalendarTokens.SidebarPadding;
        public const int SidebarCardHeight = CalendarTokens.SidebarCardHeight;
        public const int SidebarCardGap = CalendarTokens.SidebarCardGap;
        public const int SidebarWidth = CalendarTokens.SidebarWidth;
        public const int DayHeaderHeight = CalendarTokens.DayHeaderHeight;
        public const int TimeSlotHeight = CalendarTokens.TimeSlotHeight;
        public const int TimeColumnWidth = CalendarTokens.TimeColumnWidth;
    }
}
