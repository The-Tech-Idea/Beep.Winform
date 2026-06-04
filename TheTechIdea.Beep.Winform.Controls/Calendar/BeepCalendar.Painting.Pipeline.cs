using System.Diagnostics;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Single paint entry point. Builds a <see cref="ViewPaintArgs"/>
        /// snapshot for the current paint cycle, calls
        /// <see cref="ICalendarViewPainter.Layout"/> + <see cref="ICalendarViewPainter.Paint"/>
        /// on the per-view painter, then paints the chrome
        /// (background, header text, toolbar, sidebar) that lives outside
        /// the per-view grid.
        /// </summary>
        private void DrawWithPainter(Graphics g, Rectangle contentRect)
        {
            var paintStopwatch = Stopwatch.StartNew();
            _eventService?.BeginPaintCycle();

            // ── Build the view paint args bundle ────────────────────────────
            var args = new ViewPaintArgs
            {
                ControlStyle = _calendarStyle,
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                State = _state,
                Rects = _rects,
                Surface = _surfaceModel,
                EventService = _eventService,
                Events = _events,
                Categories = _categories,
                Resources = Resources,
                HeaderFont = HeaderFont,
                DayFont = DayFont,
                EventFont = EventFont,
                TimeFont = TimeFont,
                DaysHeaderFont = DaysHeaderFont,
                HoveredEventId = _hoveredEvent?.Id,
                HoveredDate = _hoveredDate,
                SelectedEvent = _state.SelectedEvent,
                Owner = this
            };
            args.ResolveThemeColors();

            // ── Background ──────────────────────────────────────────────────
            using (var backBrush = new SolidBrush(args.BackgroundColor))
                g.FillRectangle(backBrush, contentRect);

            // ── Sidebar (chrome) ───────────────────────────────────────────
            if (_state.ShowSidebar && _rects.SidebarRect.Width > 0)
            {
                PaintSidebarChrome(g, args);
            }

            // ── Per-view painter fills _rects.CalendarGridRect ─────────────
            _viewPainter.Layout(args);
            _viewPainter.Paint(g, args);

            // ── Header chrome: bar background + centered title text ────────
            using (var headerBack = new SolidBrush(args.BackgroundColor))
                g.FillRectangle(headerBack, _rects.HeaderRect);
            var headerTextBounds = _surfaceModel != null
                ? _surfaceModel.GetHeaderTextBounds()
                : GetHeaderTextBounds();
            PaintHeaderText(g, args, GetHeaderText(), headerTextBounds);

            // ── View selector / toolbar chrome ─────────────────────────────
            PaintToolbar(g);

            paintStopwatch.Stop();
            if (PerformanceMetrics.Enabled)
                PerformanceMetrics.RecordPaint(paintStopwatch.Elapsed);
        }

        private static void PaintHeaderText(Graphics g, ViewPaintArgs args, string headerText, Rectangle textRect)
        {
            if (textRect.Width <= 0 || textRect.Height <= 0 || string.IsNullOrEmpty(headerText)) return;
            CalendarPainterHelpers.DrawText(g, headerText,
                args.HeaderFont, args.ForegroundColor, textRect,
                StringAlignment.Center, StringAlignment.Center);
        }

        private void PaintSidebarChrome(Graphics g, ViewPaintArgs args)
        {
            var rect = _rects.SidebarRect;
            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            CalendarPainterHelpers.StrokeRoundedRect(g, rect, args.Metrics.CornerRadius, args.BorderColor);

            var miniRect = _surfaceModel != null
                ? _surfaceModel.GetSidebarMiniCalendarRect()
                : Rectangle.Empty;
            if (miniRect.Width > 20)
            {
                CalendarPainterHelpers.FillRoundedRect(g, miniRect, args.Metrics.CornerRadius, args.BackgroundColor);
                CalendarPainterHelpers.StrokeRoundedRect(g, miniRect, args.Metrics.CornerRadius, args.BorderColor);
                CalendarPainterHelpers.DrawText(g, _state.CurrentDate.ToString("MMMM yyyy"),
                    args.HeaderFont, args.ForegroundColor, miniRect,
                    StringAlignment.Center, StringAlignment.Center);
            }

            var detailsRect = _surfaceModel != null
                ? _surfaceModel.GetSidebarEventDetailsRect()
                : Rectangle.Empty;
            if (detailsRect.Width > 20)
            {
                CalendarPainterHelpers.FillRoundedRect(g, detailsRect, args.Metrics.CornerRadius, args.BackgroundColor);
                CalendarPainterHelpers.StrokeRoundedRect(g, detailsRect, args.Metrics.CornerRadius, args.BorderColor);
                if (_state.SelectedEvent != null)
                {
                    CalendarPainterHelpers.DrawText(g, _state.SelectedEvent.Title,
                        args.HeaderFont, args.ForegroundColor, detailsRect,
                        StringAlignment.Near, StringAlignment.Near, centerVertically: false);
                }
                else
                {
                    CalendarPainterHelpers.DrawText(g, "No event selected",
                        args.DayFont, args.ForegroundColor, detailsRect,
                        StringAlignment.Center, StringAlignment.Center);
                }
            }
        }
    }
}
