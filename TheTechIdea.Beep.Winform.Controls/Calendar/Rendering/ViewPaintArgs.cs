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

        // ── Resolved palette ──────────────────────────────────────────────────
        // No initialisers, deliberately (the Badges rule): every one of these is filled by
        // ResolveThemeColors from the assigned theme, falling back to BeepThemesManager.CurrentTheme.
        // The literal defaults that used to sit here were a second palette that showed through
        // whenever resolution was skipped - hardcoded Material colours under every other theme.
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BorderColor { get; set; }
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }
        public Color TodayBackColor { get; set; }
        public Color TodayForeColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color SelectedBackColor { get; set; }
        public Color SelectedForeColor { get; set; }
        public Color WeekendBackColor { get; set; }
        public Color OutOfMonthBackColor { get; set; }
        public Color OutOfMonthForeColor { get; set; }
        public Color TitleForeColor { get; set; }
        public Color DaysHeaderForeColor { get; set; }
        public Color HoverForeColor { get; set; }
        public Color SuccessColor { get; set; }
        public Color WarningColor { get; set; }
        public Color ErrorColor { get; set; }
        public Color AccentColor { get; set; }

        // ── Style metrics (resolved from ControlStyle) ────────────────────────
        public CalendarStyleMetrics Metrics { get; set; } = CalendarStyleMetrics.Material3();

        /// <summary>
        /// The fill for an event block: its category's colour when it has one, otherwise a colour
        /// from the theme's own hue slots, chosen stably per event.
        /// </summary>
        /// <remarks>
        /// The fallback was <see cref="Color.Gray"/>, so every uncategorised event rendered as a flat
        /// grey slab — nothing like the reference designs (sampleimages/c1..c7), where block colours
        /// vary per event. The palette is the theme's slots verbatim (see <see cref="EventPalette"/>),
        /// and the pick is hashed from the event's identity so an event keeps its colour across
        /// repaints and view switches.
        /// </remarks>
        public Color GetEventFill(CalendarEvent evt)
        {
            if (evt != null && Categories != null)
            {
                for (int i = 0; i < Categories.Count; i++)
                {
                    if (Categories[i].Id == evt.CategoryId) return Categories[i].Color;
                }
            }

            var palette = EventPalette;
            int index = evt == null ? 0 : Math.Abs(($"{evt.Id}|{evt.Title}").GetHashCode()) % palette.Length;
            return palette[index];
        }

        /// <summary>
        /// Text colour for content painted on an event fill: whichever of the theme's foreground or
        /// background reads against it. A choice between two theme colours — never a computed one.
        /// </summary>
        public Color GetEventInk(Color fill) =>
            CalendarDrawingPrimitives.GetContrastingForeground(fill, ForegroundColor, BackgroundColor);

        /// <summary>
        /// The palette uncategorised events draw from: the theme's own hue slots, verbatim —
        /// primary, secondary, accent, success, warning, error. The theme author chose these;
        /// nothing here modifies them.
        /// </summary>
        private Color[] EventPalette
        {
            get
            {
                return new[] { PrimaryColor, SecondaryColor, AccentColor, SuccessColor, WarningColor, ErrorColor };
            }
        }

        /// <summary>Resolve a category color, falling back to the theme's primary.</summary>
        public Color GetCategoryColor(int categoryId)
        {
            if (Categories != null)
            {
                for (int i = 0; i < Categories.Count; i++)
                {
                    if (Categories[i].Id == categoryId) return Categories[i].Color;
                }
            }
            // The palette's first slot, not Color.Gray - the last non-theme colour in this file.
            return PrimaryColor;
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

            // There is always a theme - BeepThemesManager guarantees a current one. No null
            // checks, no per-slot fallbacks: the slots are assigned as the theme defines them, and a
            // wrong-looking colour is the THEME's bug, fixed in the theme. (User directive; the
            // guard-and-fallback version quietly substituted colours and made theme bugs invisible.)
            effectiveTheme ??= BeepThemesManager.CurrentTheme;

            BackgroundColor = effectiveTheme.CalendarBackColor;
            ForegroundColor = effectiveTheme.CalendarForeColor;
            BorderColor = effectiveTheme.CalendarBorderColor;
            PrimaryColor = effectiveTheme.PrimaryColor;
            SecondaryColor = effectiveTheme.SecondaryColor;
            TodayBackColor = effectiveTheme.CalendarSelectedDateBackColor;
            TodayForeColor = effectiveTheme.CalendarTodayForeColor;
            HoverBackColor = effectiveTheme.CalendarHoverBackColor;
            HoverForeColor = effectiveTheme.CalendarHoverForeColor;
            SelectedBackColor = effectiveTheme.CalendarSelectedDateBackColor;
            SelectedForeColor = effectiveTheme.CalendarSelectedDateForColor;
            WeekendBackColor = effectiveTheme.CalendarBackColor;
            OutOfMonthBackColor = effectiveTheme.CalendarBackColor;
            OutOfMonthForeColor = effectiveTheme.DisabledForeColor;
            TitleForeColor = effectiveTheme.CalendarTitleForColor;
            DaysHeaderForeColor = effectiveTheme.CalendarDaysHeaderForColor;
            SuccessColor = effectiveTheme.SuccessColor;
            WarningColor = effectiveTheme.WarningColor;
            ErrorColor = effectiveTheme.ErrorColor;
            AccentColor = effectiveTheme.AccentColor;
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
