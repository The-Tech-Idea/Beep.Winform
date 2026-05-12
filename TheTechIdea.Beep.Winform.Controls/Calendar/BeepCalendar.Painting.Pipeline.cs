using System.Drawing;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Draw using the new style painter system
        /// </summary>
        private void DrawWithPainter(Graphics g, Rectangle contentRect)
        {
            var paintStopwatch = Stopwatch.StartNew();

            // Begin paint cycle for event caching
            _eventService?.BeginPaintCycle();

            // Create painter context with StyleColors
            var painterCtx = CalendarPainterFactory.CreateContext(
                _calendarStyle,
                _currentTheme,
                UseThemeColors,
                _state,
                _rects,
                _eventService,
                _categories,
                HeaderFont,
                DayFont,
                EventFont,
                TimeFont,
                DaysHeaderFont
            );

            // Paint background
            _stylePainter.PaintBackground(g, contentRect, painterCtx);

            // Paint header
            var headerTextBounds = GetHeaderTextBounds();
            _stylePainter.PaintHeader(g, _rects.HeaderRect, string.Empty, painterCtx);
            DrawPainterHeaderText(g, painterCtx, GetHeaderText(), headerTextBounds);

            // Paint view selector
            _stylePainter.PaintViewSelector(g, _rects.ViewSelectorRect, painterCtx);

            // Paint main calendar based on view mode
            DrawCalendarViewWithPainter(g, painterCtx);

            // Paint sidebar
            if (_state.ShowSidebar && _rects.SidebarRect.Width > 0)
            {
                DrawSidebarWithPainter(g, painterCtx);
            }

            paintStopwatch.Stop();
            if (PerformanceMetrics.Enabled)
            {
                PerformanceMetrics.RecordPaint(paintStopwatch.Elapsed);
            }
        }

    }
}