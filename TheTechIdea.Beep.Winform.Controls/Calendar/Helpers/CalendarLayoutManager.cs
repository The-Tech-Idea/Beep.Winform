using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    internal class CalendarLayoutManager
    {
        private readonly BaseControl _owner;
        private readonly CalendarState _state;
        private readonly CalendarRects _rects;

        public CalendarLayoutManager(BaseControl owner, CalendarState state, CalendarRects rects)
        {
            _owner = owner;
            _state = state;
            _rects = rects;
        }

        public void UpdateLayout(Rectangle clientRect, int tallestSelectorButtonHeight, int sidebarWidth, int leftGutter = 10)
        {
            if (clientRect.Width <= 0 || clientRect.Height <= 0) return;

            int headerHeight = CalendarLayoutMetrics.HeaderHeight;
            int selectorHeight = Math.Max(CalendarLayoutMetrics.BaseViewSelectorHeight,
                                          tallestSelectorButtonHeight + CalendarLayoutMetrics.SelectorVerticalPadding * 2);

            _rects.HeaderRect = new Rectangle(clientRect.X, clientRect.Y, clientRect.Width, headerHeight);
            _rects.ViewSelectorRect = new Rectangle(clientRect.X, clientRect.Y + headerHeight, clientRect.Width, selectorHeight);

            int gridTop = _rects.ViewSelectorRect.Bottom + CalendarLayoutMetrics.SectionSpacing;
            int actualSidebarWidth = _state.ShowSidebar ? sidebarWidth : 0;

            // Apply left gutter only for time-based views (Week, Day) that need space for time labels
            // Month and List views should start at the left edge
            int actualLeftGutter = (_state.ViewMode == CalendarViewMode.Week || _state.ViewMode == CalendarViewMode.Day) 
                                 ? Math.Max(0, leftGutter) 
                                 : 0;

            int gridLeft = clientRect.X + actualLeftGutter;

            _rects.SidebarRect = new Rectangle(clientRect.Right - actualSidebarWidth, gridTop,
                                               actualSidebarWidth, clientRect.Bottom - gridTop);

            _rects.CalendarGridRect = new Rectangle(gridLeft, gridTop,
                                                    clientRect.Width - actualSidebarWidth - (gridLeft - clientRect.X), clientRect.Bottom - gridTop);
        }

        public CalendarRects Rects => _rects;
    }
}
