using System;
using System.Drawing;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Draw using the legacy renderer system
        /// </summary>
        private void DrawWithLegacyRenderer(Graphics g, Rectangle contentRect)
        {
            var paintStopwatch = Stopwatch.StartNew();

            // Begin paint cycle for event caching
            _eventService?.BeginPaintCycle();

            var headerTextBounds = GetHeaderTextBounds();
            int headerLeft = Math.Max(0, headerTextBounds.Left - _rects.HeaderRect.X);
            int headerRight = Math.Max(0, _rects.HeaderRect.Right - headerTextBounds.Right);

            var ctx = new CalendarRenderContext(this, _currentTheme,
                HeaderFont, DayFont, EventFont, TimeFont, DaysHeaderFont,
                _state, _rects, _eventService, _categories, Resources,
                headerLeft, headerRight, GetDensityScale());

            _renderer.Draw(g, ctx);

            paintStopwatch.Stop();
            if (PerformanceMetrics.Enabled)
            {
                PerformanceMetrics.RecordPaint(paintStopwatch.Elapsed);
            }
        }
    }
}