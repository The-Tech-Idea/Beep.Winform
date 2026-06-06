using System.Diagnostics;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
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
            // Fonts are pre-seeded from BeepCalendar's resolved font
            // properties (which ApplyThemeTypography already populated from
            // _currentTheme). ViewPaintArgs.ApplyThemeFonts() re-projects
            // them from the theme so theme switches take effect without an
            // explicit ApplyTheme call from the host.
            var args = new ViewPaintArgs
            {
                ControlStyle = _calendarStyle,
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
            args.ApplyTheme(_currentTheme);
            args.ApplyThemeFonts();

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

                var selectedCtx = new CalendarCellContext(
                    CalendarCellKind.SidebarDetailCard,
                    _state.SelectedEvent,
                    _state.CurrentDate,
                    _state.ViewMode, 0, 0);
                DrawCellComponent(g, detailsRect, "sidebar:detail", selectedCtx);

                if (_state.SelectedEvent != null)
                {
                    var evt = _state.SelectedEvent;
                    int lineHeight = args.DayFont.Height + 2;
                    int pad = 4;
                    var lineRect = new Rectangle(detailsRect.X + pad, detailsRect.Y + pad,
                        detailsRect.Width - pad * 2, lineHeight);

                    // Title
                    CalendarPainterHelpers.DrawText(g, evt.Title, args.EventFont,
                        args.ForegroundColor, lineRect, StringAlignment.Near, StringAlignment.Near,
                        centerVertically: false);
                    lineRect.Y += lineHeight + 2;

                    // Date range
                    string dateText = evt.IsAllDay
                        ? evt.StartTime.ToString("ddd M/d") + (evt.Duration.TotalDays > 1
                            ? " - " + evt.EndTime.AddDays(-1).ToString("ddd M/d")
                            : " (All day)")
                        : evt.StartTime.ToString("ddd M/d  h:mm tt") + (evt.StartTime.Date != evt.EndTime.Date
                            ? " - " + evt.EndTime.ToString("ddd M/d  h:mm tt")
                            : " - " + evt.EndTime.ToString("h:mm tt"));
                    CalendarPainterHelpers.DrawText(g, dateText, args.DayFont,
                        args.ForegroundColor, lineRect, StringAlignment.Near, StringAlignment.Near,
                        centerVertically: false);
                    lineRect.Y += lineHeight;

                    // Category
                    var cat = _categories.Find(c => c.Id == evt.CategoryId);
                    string catText = cat != null ? cat.Name : $"Category {evt.CategoryId}";
                    CalendarPainterHelpers.DrawText(g, catText, args.DayFont,
                        cat != null ? cat.Color : args.ForegroundColor,
                        lineRect, StringAlignment.Near, StringAlignment.Near,
                        centerVertically: false);
                    lineRect.Y += lineHeight;

                    // Location (if set)
                    if (!string.IsNullOrWhiteSpace(evt.Location))
                    {
                        CalendarPainterHelpers.DrawText(g, evt.Location, args.DayFont,
                            args.ForegroundColor, lineRect, StringAlignment.Near, StringAlignment.Near,
                            centerVertically: false);
                    }
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
