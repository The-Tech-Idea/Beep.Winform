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

        /// <summary>
        /// Project the theme + style + control flags into the resolved color set above.
        ///
        /// Pattern (same as every other Beep control):
        /// 1. Re-resolve <see cref="Metrics"/> from <see cref="ControlStyle"/>
        ///    so per-style layout/visual constants are current.
        /// 2. If a theme is supplied, project <see cref="Theme.CalendarBackColor"/>
        ///    / <see cref="Theme.CalendarForeColor"/> / <see cref="Theme.CalendarBorderColor"/>
        ///    / <see cref="Theme.PrimaryColor"/> into the resolved palette.
        /// 3. If no theme is supplied, resolve the default theme for the
        ///    owning control's <c>ControlStyle</c> via
        ///    <c>BeepStyling.GetFormStyle</c> +
        ///    <c>BeepThemesManager.GetThemeNameForFormStyle</c> +
        ///    <c>BeepThemesManager.GetTheme</c> and use its colors.
        ///    This mirrors <c>BaseControl.Properties.cs:382</c>:
        ///    <c>Theme = BeepStyling.GetThemeStyle(_controlstyle);</c>.
        /// 4. Only as a final safety net (no theme resolvable) fall back to
        ///    built-in static defaults.
        /// </summary>
        public void ResolveThemeColors()
        {
            Metrics = CalendarStyleMetrics.For(ControlStyle);

            IBeepTheme effectiveTheme = Theme;
            if (effectiveTheme == null)
            {
                // Resolve the default theme for the owner's control style
                // (this is the same mapping BaseControl uses when its
                // ControlStyle property is set).
                BeepControlStyle ownerStyle = Owner?.ControlStyle ?? ControlStyle;
                FormStyle formStyle = BeepStyling.GetFormStyle(ownerStyle);
                try
                {
                    string themeName = BeepThemesManager.GetThemeNameForFormStyle(formStyle);
                    if (!string.IsNullOrEmpty(themeName))
                    {
                        effectiveTheme = BeepThemesManager.GetTheme(themeName);
                    }
                }
                catch
                {
                    /* fall through to defaults */
                }
            }

            if (UseThemeColors && effectiveTheme != null)
            {
                BackgroundColor = effectiveTheme.CalendarBackColor;
                ForegroundColor = effectiveTheme.CalendarForeColor;
                BorderColor = effectiveTheme.CalendarBorderColor;
                PrimaryColor = effectiveTheme.PrimaryColor;
                if (effectiveTheme is BeepTheme bt)
                {
                    SecondaryColor = bt.SecondaryColor;
                }
            }
            else
            {
                // No theme resolvable at all — fall back to safe built-in defaults.
                BackgroundColor = Color.White;
                ForegroundColor = Color.Black;
                BorderColor = Color.FromArgb(218, 220, 224);
                PrimaryColor = Color.FromArgb(103, 80, 164);
                SecondaryColor = Color.FromArgb(66, 133, 244);
            }
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
    /// Style-specific layout + visual metrics. Layout constants only — colors
    /// are NOT stored here. Colors and theme come from <see cref="IBeepTheme"/>
    /// (the same way every other Beep control does it), resolved from
    /// <c>BeepThemesManager.GetThemeNameForFormStyle(BeepStyling.GetFormStyle(style))</c>
    /// + <c>BeepThemesManager.GetTheme(name)</c>.
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

        /// <summary>
        /// Return the layout/visual metrics for the supplied control style.
        /// Colors and theme are NOT derived here — see
        /// <see cref="ViewPaintArgs.ResolveThemeColors"/> for theme-driven
        /// color resolution, which follows the same
        /// <c>BeepStyling.GetFormStyle → BeepThemesManager.GetTheme</c>
        /// pattern that every other Beep control uses.
        /// </summary>
        public static CalendarStyleMetrics For(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Material3(),
                BeepControlStyle.Material => Material3(),
                BeepControlStyle.MaterialYou => Material3(),
                BeepControlStyle.Minimal => Minimal(),
                BeepControlStyle.NotionMinimal => Minimal(),
                BeepControlStyle.VercelClean => Minimal(),
                _ => Material3()
            };
        }
    }
}
