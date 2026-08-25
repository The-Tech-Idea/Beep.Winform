using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Painters
{
    /// <summary>
    /// Shared rendering foundation for date-picker painters. Centralizes theme-token
    /// resolution (colors and typography), DPI scaling, and the common calendar chrome
    /// (header, navigation buttons, day-name row, and day cells) so every mode renders
    /// with the same theme-driven, accessible styling.
    /// </summary>
    public abstract class DateTimePickerPainterBase : IDateTimePickerPainter
    {
        protected readonly BeepDateTimePicker _owner;
        protected IBeepTheme _theme;

        protected DateTimePickerPainterBase(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public abstract DatePickerMode Mode { get; }
        public abstract void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState);
        public abstract void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState);
        public abstract void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed);
        public abstract void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState);
        public abstract void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed);
        public abstract void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState);
        public abstract void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed);
        public abstract void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth);
        public abstract void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState);
        public abstract void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered);
        public abstract DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties);
        public abstract Size GetPreferredSize(DateTimePickerProperties properties);
        public abstract Size GetMinimumSize(DateTimePickerProperties properties);

        public virtual void UpdateTheme(IBeepTheme theme) => _theme = theme;

        #region Theme Tokens

        protected Color AccentColor => _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
        protected Color TextColor => _theme?.CalendarForeColor ?? Color.Black;
        protected Color SecondaryTextColor => _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(128, 128, 128);
        protected Color HoverColor => _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
        protected Color TodayColor => _theme?.CalendarTodayForeColor != Color.Empty ? _theme.CalendarTodayForeColor : AccentColor;
        protected Color BackgroundColor => _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;
        protected Color BorderColor => _theme?.CalendarBorderColor ?? Color.FromArgb(200, 200, 200);
        protected Color TitleColor => _theme?.CalendarTitleForColor ?? TextColor;

        protected Font TitleFont => ResolveFont(_theme?.CalendarTitleFont, 12f, FontStyle.Bold);
        protected Font CellFont => ResolveFont(_theme?.CalendarUnSelectedFont, 10f, FontStyle.Regular);
        protected Font SelectedFont => ResolveFont(_theme?.CalendarSelectedFont, 10f, FontStyle.Regular);
        protected Font DayNamesFont => ResolveFont(_theme?.CalendarUnSelectedFont, 10f, FontStyle.Regular);

        /// <summary>Scales a design-time pixel value for the owner's current DPI.</summary>
        protected int S(int px) => DpiScalingHelper.ScaleValue(px, _owner);

        protected Font ResolveFont(TypographyStyle style, float fallbackSize, FontStyle fallbackStyle)
        {
            if (style != null)
            {
                var font = BeepThemesManager.ToFont(style);
                if (font != null) return font;
            }

            return new Font(_theme?.FontName ?? "Segoe UI", fallbackSize, fallbackStyle);
        }

        #endregion

        #region Shared Painting

        public virtual void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false)
        {
            cellBounds.Inflate(-S(2), -S(2));

            Color textColor = TextColor;

            if (isSelected)
            {
                using (var brush = new SolidBrush(AccentColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
            }
            else if (isPressed)
            {
                using (var brush = new SolidBrush(ShiftLuminance(AccentColor, -0.1f)))
                {
                    g.FillEllipse(brush, cellBounds);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(HoverColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
            }

            if (isToday && !isSelected)
            {
                using (var pen = new Pen(TodayColor, S(2)))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

            if (isDisabled)
            {
                textColor = Muted(TextColor, 96);
            }

            using (var font = CellFont)
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(date.Day.ToString(), font, brush, cellBounds, format);
            }
        }

        public virtual void PaintHeader(Graphics g, Rectangle headerBounds, string headerText, bool showNavigation, bool isHovered)
        {
            using (var font = TitleFont)
            using (var brush = new SolidBrush(TitleColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(headerText, font, brush, headerBounds, format);
            }
        }

        public virtual void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed)
        {
            if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(HoverColor))
                {
                    g.FillEllipse(brush, buttonBounds);
                }
            }

            using (var pen = new Pen(TitleColor, S(2)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = buttonBounds.X + buttonBounds.Width / 2;
                int centerY = buttonBounds.Y + buttonBounds.Height / 2;
                int size = S(5);

                if (isNext)
                {
                    g.DrawLine(pen, centerX - 2, centerY - size, centerX + 3, centerY);
                    g.DrawLine(pen, centerX + 3, centerY, centerX - 2, centerY + size);
                }
                else
                {
                    g.DrawLine(pen, centerX + 2, centerY - size, centerX - 3, centerY);
                    g.DrawLine(pen, centerX - 3, centerY, centerX + 2, centerY + size);
                }
            }
        }

        public virtual void PaintDayNamesHeader(Graphics g, Rectangle headerBounds, DatePickerFirstDayOfWeek firstDayOfWeek)
        {
            using (var font = DayNamesFont)
            {
                string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
                int startDay = (int)firstDayOfWeek;
                int cellWidth = headerBounds.Width / 7;

                for (int i = 0; i < 7; i++)
                {
                    int dayIndex = (startDay + i) % 7;
                    var cellRect = new Rectangle(
                        headerBounds.X + i * cellWidth,
                        headerBounds.Y,
                        cellWidth,
                        headerBounds.Height);

                    using (var brush = new SolidBrush(SecondaryTextColor))
                    {
                        var format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawString(dayNames[dayIndex].Substring(0, 2), font, brush, cellRect, format);
                    }
                }
            }
        }

        #endregion

        #region Color Utilities

        protected static Color Muted(Color color, int alpha = 128) => Color.FromArgb(alpha, color);

        protected static Color ShiftLuminance(Color color, float amount)
        {
            ColorToHsl(color, out float h, out float s, out float l);
            l = Math.Max(0, Math.Min(1, l + amount));
            return ColorFromHsl(h, s, l);
        }

        protected static void ColorToHsl(Color color, out float h, out float s, out float l)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;
            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));
            l = (max + min) / 2.0f;

            if (max == min)
            {
                h = s = 0;
            }
            else
            {
                float d = max - min;
                s = l > 0.5f ? d / (2.0f - max - min) : d / (max + min);
                if (max == r) h = (g - b) / d + (g < b ? 6 : 0);
                else if (max == g) h = (b - r) / d + 2;
                else h = (r - g) / d + 4;
                h /= 6.0f;
            }
        }

        protected static Color ColorFromHsl(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = HueToRgb(p, q, h + 1.0f / 3.0f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0f / 3.0f);
            }

            return Color.FromArgb(
                Math.Max(0, Math.Min(255, (int)(r * 255 + 0.5f))),
                Math.Max(0, Math.Min(255, (int)(g * 255 + 0.5f))),
                Math.Max(0, Math.Min(255, (int)(b * 255 + 0.5f))));
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6 * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6;
            return p;
        }

        #endregion
    }
}
