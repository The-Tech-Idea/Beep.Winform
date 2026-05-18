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

        public void UpdateLayout(Rectangle clientRect, int tallestSelectorButtonHeight, int sidebarWidth, int leftGutter = 10, float metricScale = 1.0f)
        {
            if (clientRect.Width <= 0 || clientRect.Height <= 0) return;

            int headerHeight = Scale(CalendarLayoutMetrics.HeaderHeight, metricScale);
            int selectorHeight = Math.Max(Scale(CalendarLayoutMetrics.BaseViewSelectorHeight, metricScale),
                                          tallestSelectorButtonHeight + Scale(CalendarLayoutMetrics.SelectorVerticalPadding, metricScale) * 2);
            headerHeight = Math.Min(headerHeight, Math.Max(1, clientRect.Height / 3));
            selectorHeight = Math.Min(selectorHeight, Math.Max(1, clientRect.Height - headerHeight));

            _rects.HeaderRect = new Rectangle(clientRect.X, clientRect.Y, clientRect.Width, headerHeight);
            _rects.ViewSelectorRect = new Rectangle(clientRect.X, clientRect.Y + headerHeight, clientRect.Width, selectorHeight);

            int gridTop = _rects.ViewSelectorRect.Bottom + Scale(CalendarLayoutMetrics.SectionSpacing, metricScale);
            int availableGridHeight = Math.Max(0, clientRect.Bottom - gridTop);
            int actualSidebarWidth = _state.ShowSidebar ? Math.Max(0, Math.Min(sidebarWidth, clientRect.Width - 1)) : 0;

            // Apply left gutter only for time-based views that need space for time labels
            // Month, Agenda, and List views should start at the left edge
            int actualLeftGutter = (_state.ViewMode == CalendarViewMode.Week || _state.ViewMode == CalendarViewMode.WorkWeek || _state.ViewMode == CalendarViewMode.Day) 
                                 ? Math.Max(0, leftGutter) 
                                 : 0;

            int gridLeft = clientRect.X + actualLeftGutter;

            _rects.SidebarRect = new Rectangle(clientRect.Right - actualSidebarWidth, gridTop,
                                               actualSidebarWidth, availableGridHeight);

            _rects.CalendarGridRect = new Rectangle(gridLeft, gridTop,
                                                    Math.Max(0, clientRect.Width - actualSidebarWidth - (gridLeft - clientRect.X)),
                                                    availableGridHeight);
        }

        private static int Scale(int value, float metricScale)
        {
            float scale = metricScale <= 0 ? 1.0f : metricScale;
            return Math.Max(1, (int)Math.Round(value * scale));
        }

        public CalendarRects Rects => _rects;
    }
}
