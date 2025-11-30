using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.StylePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    /// <summary>
    /// Factory for creating calendar style painters based on BeepControlStyle
    /// </summary>
    public static class CalendarPainterFactory
    {
        private static readonly Dictionary<BeepControlStyle, Func<ICalendarStylePainter>> _painterFactories = 
            new Dictionary<BeepControlStyle, Func<ICalendarStylePainter>>
        {
            { BeepControlStyle.Material3, () => new MaterialCalendarPainter() },
            { BeepControlStyle.Material, () => new MaterialCalendarPainter() },
            { BeepControlStyle.MaterialYou, () => new MaterialCalendarPainter() },
            { BeepControlStyle.Minimal, () => new MinimalCalendarPainter() },
            { BeepControlStyle.NotionMinimal, () => new MinimalCalendarPainter() },
            { BeepControlStyle.VercelClean, () => new MinimalCalendarPainter() },
            // Add more mappings as painters are created
        };

        /// <summary>
        /// Gets a painter for the specified control style
        /// </summary>
        public static ICalendarStylePainter GetPainter(BeepControlStyle style)
        {
            if (_painterFactories.TryGetValue(style, out var factory))
            {
                return factory();
            }
            
            // Default to Material painter
            return new MaterialCalendarPainter();
        }

        /// <summary>
        /// Gets all available painter style names
        /// </summary>
        public static IEnumerable<string> GetAvailableStyles()
        {
            yield return "Material";
            yield return "Minimal";
            // Add more as painters are created
        }

        /// <summary>
        /// Creates a painter context with colors from StyleColors
        /// </summary>
        internal static CalendarPainterContext CreateContext(
            BeepControlStyle style,
            TheTechIdea.Beep.Vis.Modules.IBeepTheme theme,
            bool useThemeColors,
            Helpers.CalendarState state,
            Helpers.CalendarRects rects,
            Helpers.CalendarEventService eventService,
            System.Collections.Generic.List<EventCategory> categories,
            System.Drawing.Font headerFont,
            System.Drawing.Font dayFont,
            System.Drawing.Font eventFont,
            System.Drawing.Font timeFont,
            System.Drawing.Font daysHeaderFont)
        {
            var ctx = new CalendarPainterContext
            {
                State = state,
                Rects = rects,
                EventService = eventService,
                Categories = categories,
                HeaderFont = headerFont,
                DayFont = dayFont,
                EventFont = eventFont,
                TimeFont = timeFont,
                DaysHeaderFont = daysHeaderFont
            };

            // Set colors based on theme or StyleColors
            if (useThemeColors && theme != null)
            {
                ctx.BackgroundColor = theme.CalendarBackColor;
                ctx.ForegroundColor = theme.CalendarForeColor;
                ctx.BorderColor = theme.CalendarBorderColor;
                ctx.PrimaryColor = theme.PrimaryColor;
                ctx.SecondaryColor = theme.SecondaryColor;
                ctx.SelectedBackColor = theme.CalendarSelectedDateBackColor;
                ctx.SelectedForeColor = theme.CalendarSelectedDateForColor;
                ctx.HoverBackColor = theme.CalendarHoverBackColor;
                ctx.TodayBackColor = theme.CalendarHoverBackColor;
                ctx.TodayForeColor = theme.CalendarTodayForeColor;
                ctx.WeekendBackColor = theme.CalendarBackColor;
                ctx.OutOfMonthBackColor = System.Drawing.Color.FromArgb(248, 249, 250);
                ctx.OutOfMonthForeColor = System.Drawing.Color.FromArgb(160, 160, 160);
            }
            else
            {
                // Use StyleColors for consistent styling
                ctx.BackgroundColor = StyleColors.GetBackground(style);
                ctx.ForegroundColor = StyleColors.GetForeground(style);
                ctx.BorderColor = StyleColors.GetBorder(style);
                ctx.PrimaryColor = StyleColors.GetPrimary(style);
                ctx.SecondaryColor = StyleColors.GetSecondary(style);
                ctx.SelectedBackColor = StyleColors.GetPrimary(style);
                ctx.SelectedForeColor = System.Drawing.Color.White;
                ctx.HoverBackColor = System.Drawing.Color.FromArgb(30, StyleColors.GetPrimary(style));
                ctx.TodayBackColor = System.Drawing.Color.FromArgb(20, StyleColors.GetPrimary(style));
                ctx.TodayForeColor = StyleColors.GetPrimary(style);
                ctx.WeekendBackColor = System.Drawing.Color.FromArgb(252, ctx.BackgroundColor);
                ctx.OutOfMonthBackColor = System.Drawing.Color.FromArgb(248, 249, 250);
                ctx.OutOfMonthForeColor = System.Drawing.Color.FromArgb(160, 160, 160);
            }

            return ctx;
        }
    }
}

