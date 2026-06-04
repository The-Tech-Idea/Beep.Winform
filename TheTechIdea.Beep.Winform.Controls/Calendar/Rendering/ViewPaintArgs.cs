using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    /// <summary>
    /// All the inputs a view painter needs for a single paint / hit-test cycle.
    /// Bundled so view painters have a single argument instead of a long
    /// parameter list, and so new inputs can be added without breaking
    /// signatures.
    /// </summary>
    public sealed class ViewPaintArgs
    {
        // ── Style + Theme ─────────────────────────────────────────────────────
        /// <summary>Resolved control style (Material3, Minimal, etc.).</summary>
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        /// <summary>Active theme. May be null — callers should fall back to defaults.</summary>
        public IBeepTheme Theme { get; set; }

        /// <summary>True when the calendar should use the theme palette vs. its built-in defaults.</summary>
        public bool UseThemeColors { get; set; } = true;

        // ── State ─────────────────────────────────────────────────────────────
        public CalendarState State { get; set; }
        public CalendarRects Rects { get; set; }
        public CalendarSurfaceModel Surface { get; set; }

        // ── Data ──────────────────────────────────────────────────────────────
        public CalendarEventService EventService { get; set; }
        public List<CalendarEvent> Events { get; set; }
        public List<EventCategory> Categories { get; set; }
        public List<CalendarResource> Resources { get; set; }

        // ── Fonts ─────────────────────────────────────────────────────────────
        public Font HeaderFont { get; set; }
        public Font DayFont { get; set; }
        public Font EventFont { get; set; }
        public Font TimeFont { get; set; }
        public Font DaysHeaderFont { get; set; }

        // ── Interaction ───────────────────────────────────────────────────────
        public int? HoveredEventId { get; set; }
        public DateTime? HoveredDate { get; set; }
        public CalendarEvent SelectedEvent { get; set; }

        // ── Owner (for back/fore color fallbacks and density) ─────────────────
        public BeepCalendar Owner { get; set; }

        // ── Computed helpers (resolved once, cached) ──────────────────────────
        public Color BackgroundColor { get; set; } = Color.White;
        public Color ForegroundColor { get; set; } = Color.Black;
        public Color BorderColor { get; set; } = Color.FromArgb(218, 220, 224);
        public Color PrimaryColor { get; set; } = Color.FromArgb(103, 80, 164);
        public Color SecondaryColor { get; set; } = Color.FromArgb(66, 133, 244);
        public Color TodayBackColor { get; set; } = Color.FromArgb(103, 80, 164);
        public Color TodayForeColor { get; set; } = Color.White;
        public Color HoverBackColor { get; set; } = Color.FromArgb(241, 243, 244);
        public Color SelectedBackColor { get; set; } = Color.FromArgb(231, 224, 236);
        public Color SelectedForeColor { get; set; } = Color.FromArgb(103, 80, 164);
        public Color WeekendBackColor { get; set; } = Color.FromArgb(252, 252, 252);
        public Color OutOfMonthBackColor { get; set; } = Color.FromArgb(248, 249, 250);
        public Color OutOfMonthForeColor { get; set; } = Color.FromArgb(190, 190, 190);

        // ── Style metrics (resolved from ControlStyle) ────────────────────────
        public CalendarStyleMetrics Metrics { get; set; } = CalendarStyleMetrics.Material3();

        /// <summary>Resolve a category color, falling back to <see cref="Color.Gray"/>.</summary>
        public Color GetCategoryColor(int categoryId)
        {
            if (Categories == null) return Color.Gray;
            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i].Id == categoryId) return Categories[i].Color;
            }
            return Color.Gray;
        }

        /// <summary>Project the theme + style + control flags into the resolved color set above.</summary>
        public void ResolveThemeColors()
        {
            if (UseThemeColors && Theme != null)
            {
                BackgroundColor = Theme.CalendarBackColor;
                ForegroundColor = Theme.CalendarForeColor;
                BorderColor = Theme.CalendarBorderColor;
                PrimaryColor = Theme.PrimaryColor;
                if (Theme is BeepTheme bt)
                {
                    SecondaryColor = bt.SecondaryColor;
                }
            }
            Metrics = CalendarStyleMetrics.For(ControlStyle);
        }

        /// <summary>
        /// Set <see cref="Theme"/> and re-resolve the resolved color palette +
        /// <see cref="Metrics"/>. Does NOT touch fonts; call
        /// <see cref="ApplyThemeFonts"/> for that. Safe to call with
        /// <c>null</c> (resets to built-in defaults).
        /// </summary>
        public void ApplyTheme(IBeepTheme theme)
        {
            Theme = theme;
            if (theme == null)
            {
                UseThemeColors = false;
            }
            ResolveThemeColors();
        }

        /// <summary>
        /// Re-resolve <see cref="HeaderFont"/>, <see cref="DayFont"/>,
        /// <see cref="EventFont"/>, <see cref="TimeFont"/>, and
        /// <see cref="DaysHeaderFont"/> from the current
        /// <see cref="Theme"/>'s typography styles. Falls back to the
        /// existing font when the theme property is null.
        /// </summary>
        public void ApplyThemeFonts()
        {
            if (Theme == null) return;
            HeaderFont = BeepThemesManager.ToFont(Theme.CalendarTitleFont) ?? HeaderFont;
            DaysHeaderFont = BeepThemesManager.ToFont(Theme.DaysHeaderFont) ?? DaysHeaderFont;
            DayFont = BeepThemesManager.ToFont(Theme.DateFont) ?? DayFont;
            EventFont = BeepThemesManager.ToFont(Theme.CalendarSelectedFont) ?? EventFont;
            TimeFont = BeepThemesManager.ToFont(Theme.CalendarUnSelectedFont) ?? TimeFont;
        }
    }

    /// <summary>
    /// Style-specific layout + visual metrics. Each control style picks a
    /// preset; view painters consult these instead of hard-coding numbers.
    /// </summary>
    public sealed class CalendarStyleMetrics
    {
        public int HeaderHeight { get; set; } = 60;
        public int ViewSelectorHeight { get; set; } = 40;
        public int DayHeaderHeight { get; set; } = 30;
        public int TimeSlotHeight { get; set; } = 60;
        public int EventBarHeight { get; set; } = 18;
        public int EventSpacing { get; set; } = 2;
        public int CornerRadius { get; set; } = 4;
        public int CellPadding { get; set; } = 4;
        public int SidebarWidth { get; set; } = 300;
        public int TimeColumnWidth { get; set; } = 60;
        public int MaxEventsPerCell { get; set; } = 3;
        public int EventCornerRadius { get; set; } = 4;
        public int EventAccentWidth { get; set; } = 3;
        public int MinEventHitHeight { get; set; } = 18;
        public FontStyle TitleFontStyle { get; set; } = FontStyle.Regular;
        public FontStyle DayFontStyle { get; set; } = FontStyle.Regular;
        public bool ShowEventShadows { get; set; } = true;
        public bool ShowEventAccentStripe { get; set; } = true;
        public bool UseElevatedCards { get; set; } = true;

        public static CalendarStyleMetrics Material3() => new CalendarStyleMetrics
        {
            HeaderHeight = 64,
            ViewSelectorHeight = 48,
            DayHeaderHeight = 36,
            TimeSlotHeight = 60,
            EventBarHeight = 18,
            EventSpacing = 2,
            CornerRadius = 8,
            CellPadding = 4,
            SidebarWidth = 300,
            TimeColumnWidth = 60,
            MaxEventsPerCell = 3,
            EventCornerRadius = 6,
            EventAccentWidth = 3,
            MinEventHitHeight = 18,
            TitleFontStyle = FontStyle.Bold,
            DayFontStyle = FontStyle.Regular,
            ShowEventShadows = true,
            ShowEventAccentStripe = true,
            UseElevatedCards = true
        };

        public static CalendarStyleMetrics Minimal() => new CalendarStyleMetrics
        {
            HeaderHeight = 56,
            ViewSelectorHeight = 40,
            DayHeaderHeight = 28,
            TimeSlotHeight = 48,
            EventBarHeight = 16,
            EventSpacing = 2,
            CornerRadius = 0,
            CellPadding = 2,
            SidebarWidth = 280,
            TimeColumnWidth = 56,
            MaxEventsPerCell = 4,
            EventCornerRadius = 0,
            EventAccentWidth = 2,
            MinEventHitHeight = 16,
            TitleFontStyle = FontStyle.Regular,
            DayFontStyle = FontStyle.Regular,
            ShowEventShadows = false,
            ShowEventAccentStripe = true,
            UseElevatedCards = false
        };

        public static CalendarStyleMetrics For(BeepControlStyle style)
        {
            switch (style)
            {
                case BeepControlStyle.Material3:
                case BeepControlStyle.Material:
                case BeepControlStyle.MaterialYou:
                    return Material3();
                case BeepControlStyle.Minimal:
                case BeepControlStyle.NotionMinimal:
                case BeepControlStyle.VercelClean:
                    return Minimal();
                default:
                    return Material3();
            }
        }

        /// <summary>
        /// Resolve the default <see cref="IBeepTheme"/> for the supplied
        /// control style. Maps the style to its <see cref="FormStyle"/>
        /// via <see cref="BeepStyling.GetFormStyle(BeepControlStyle)"/>
        /// and then to a theme via
        /// <see cref="BeepThemesManager.GetThemeNameForFormStyle(FormStyle)"/>
        /// + <see cref="BeepThemesManager.GetTheme(string)"/>.
        /// </summary>
        public static IBeepTheme ResolveDefaultTheme(BeepControlStyle style)
        {
            try
            {
                FormStyle formStyle = BeepStyling.GetFormStyle(style);
                string themeName = BeepThemesManager.GetThemeNameForFormStyle(formStyle);
                if (!string.IsNullOrEmpty(themeName))
                {
                    return BeepThemesManager.GetTheme(themeName);
                }
            }
            catch
            {
                /* fall through */
            }
            try
            {
                return BeepThemesManager.GetDefaultTheme();
            }
            catch
            {
                return null;
            }
        }
    }
}
