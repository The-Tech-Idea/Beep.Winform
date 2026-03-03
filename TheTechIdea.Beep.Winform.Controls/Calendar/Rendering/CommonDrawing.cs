using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal static class CommonDrawing
    {
        public static void DrawHeader(Graphics g, CalendarRenderContext ctx, string headerText)
        {
            var rect = ctx.Rects.HeaderRect;
            using (var brush = new SolidBrush(ctx.Theme?.CalendarBackColor ?? ctx.Owner.BackColor))
                g.FillRectangle(brush, rect);

            int leftMargin = ctx.HeaderLeftMargin;
            int rightMargin = ctx.HeaderRightMargin;
            int availableLeft = rect.X + leftMargin;
            int availableRight = rect.Right - rightMargin;
            if (availableRight <= availableLeft)
            {
                availableLeft = rect.X;
                availableRight = rect.Right;
            }

            var textRect = new Rectangle(
                availableLeft,
                rect.Y,
                Math.Max(1, availableRight - availableLeft),
                rect.Height);
            using (var brush = new SolidBrush(ctx.Theme?.CalendarTitleForColor ?? ctx.Owner.ForeColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(headerText, ctx.HeaderFont, brush, textRect, sf);
            }
        }

        public static void DrawViewSelectorBackground(Graphics g, CalendarRenderContext ctx)
        {
            var rect = ctx.Rects.ViewSelectorRect;
            using (var brush = new SolidBrush(ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250)))
                g.FillRectangle(brush, rect);

            using (var pen = new Pen(ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
                g.DrawLine(pen, rect.X, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
        }

        public static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static Color GetCategoryColor(CalendarRenderContext ctx, CalendarEvent evt)
        {
            var category = ctx.Categories.FirstOrDefault(c => c.Id == evt.CategoryId);
            return category?.Color ?? Color.Gray;
        }

        public static int ScaleMetric(int baseValue, float densityScale)
        {
            var scale = densityScale <= 0 ? 1.0f : densityScale;
            return Math.Max(1, (int)Math.Round(baseValue * scale));
        }

        public static void DrawEventCard(
            Graphics g,
            CalendarRenderContext ctx,
            CalendarEvent evt,
            Rectangle rect,
            bool isSelected = false,
            bool includeDescription = false,
            bool includeActions = false)
        {
            var categoryColor = GetCategoryColor(ctx, evt);
            var cardBackColor = ctx.Theme?.CardBackColor ?? ctx.Theme?.CalendarBackColor ?? Color.White;
            var cardBorderColor = ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224);
            var foreColor = ctx.Theme?.CalendarForeColor ?? Color.Black;
            var subtleTextColor = ctx.Theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(120, foreColor);

            var effectiveBackColor = isSelected ? Blend(cardBackColor, categoryColor, 0.20f) : cardBackColor;
            var effectiveBorderColor = isSelected ? categoryColor : cardBorderColor;
            var effectiveRect = new Rectangle(rect.X, rect.Y, rect.Width, Math.Max(CalendarLayoutMetrics.MinEventHitHeight, rect.Height));

            using (var path = RoundedRect(effectiveRect, CalendarLayoutMetrics.EventCornerRadius))
            using (var backBrush = new SolidBrush(effectiveBackColor))
            {
                g.FillPath(backBrush, path);
            }

            using (var borderPen = new Pen(effectiveBorderColor, isSelected ? 2f : 1f))
            using (var path = RoundedRect(effectiveRect, CalendarLayoutMetrics.EventCornerRadius))
            {
                g.DrawPath(borderPen, path);
            }

            var accentRect = new Rectangle(effectiveRect.X, effectiveRect.Y, CalendarLayoutMetrics.EventAccentWidth, effectiveRect.Height);
            using (var accentBrush = new SolidBrush(categoryColor))
            {
                g.FillRectangle(accentBrush, accentRect);
            }

            var contentRect = new Rectangle(
                effectiveRect.X + CalendarLayoutMetrics.EventAccentWidth + 6,
                effectiveRect.Y + 4,
                Math.Max(12, effectiveRect.Width - CalendarLayoutMetrics.EventAccentWidth - 10),
                Math.Max(12, effectiveRect.Height - 8));

            using (var titleBrush = new SolidBrush(foreColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                var titleRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width, Math.Max(14, contentRect.Height / 2));
                g.DrawString(evt.Title, ctx.DayFont, titleBrush, titleRect, sf);
            }

            using (var timeBrush = new SolidBrush(subtleTextColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                var timeText = evt.IsAllDay ? "All day" : $"{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}";
                int timeY = contentRect.Y + Math.Max(14, contentRect.Height / 2) - 2;
                var timeRect = new Rectangle(contentRect.X, timeY, contentRect.Width, Math.Max(12, contentRect.Height / 2));
                g.DrawString(timeText, ctx.EventFont, timeBrush, timeRect, sf);
            }

            if (includeDescription && !string.IsNullOrWhiteSpace(evt.Description) && contentRect.Height > 34)
            {
                using (var descriptionBrush = new SolidBrush(Color.FromArgb(160, subtleTextColor)))
                using (var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                })
                {
                    var descRect = new Rectangle(contentRect.X, contentRect.Y + 30, contentRect.Width, contentRect.Height - 30);
                    g.DrawString(evt.Description, ctx.EventFont, descriptionBrush, descRect, sf);
                }
            }

            if (includeActions)
            {
                DrawListActionChips(g, ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244), ctx.Theme?.CalendarForeColor ?? Color.Black, ctx.EventFont, effectiveRect);
            }
        }

        public static void DrawEventCard(
            Graphics g,
            CalendarPainterContext ctx,
            CalendarEvent evt,
            Rectangle rect,
            bool isSelected = false,
            bool includeDescription = false,
            bool includeActions = false)
        {
            var categoryColor = ctx.GetCategoryColor(evt.CategoryId);
            var cardBackColor = ctx.BackgroundColor;
            var cardBorderColor = ctx.BorderColor;
            var foreColor = ctx.ForegroundColor;
            var subtleTextColor = Color.FromArgb(120, foreColor);

            var effectiveBackColor = isSelected ? Blend(cardBackColor, categoryColor, 0.20f) : cardBackColor;
            var effectiveBorderColor = isSelected ? categoryColor : cardBorderColor;
            var effectiveRect = new Rectangle(rect.X, rect.Y, rect.Width, Math.Max(CalendarLayoutMetrics.MinEventHitHeight, rect.Height));

            using (var path = RoundedRect(effectiveRect, CalendarLayoutMetrics.EventCornerRadius))
            using (var backBrush = new SolidBrush(effectiveBackColor))
            {
                g.FillPath(backBrush, path);
            }

            using (var borderPen = new Pen(effectiveBorderColor, isSelected ? 2f : 1f))
            using (var path = RoundedRect(effectiveRect, CalendarLayoutMetrics.EventCornerRadius))
            {
                g.DrawPath(borderPen, path);
            }

            var accentRect = new Rectangle(effectiveRect.X, effectiveRect.Y, CalendarLayoutMetrics.EventAccentWidth, effectiveRect.Height);
            using (var accentBrush = new SolidBrush(categoryColor))
            {
                g.FillRectangle(accentBrush, accentRect);
            }

            var contentRect = new Rectangle(
                effectiveRect.X + CalendarLayoutMetrics.EventAccentWidth + 6,
                effectiveRect.Y + 4,
                Math.Max(12, effectiveRect.Width - CalendarLayoutMetrics.EventAccentWidth - 10),
                Math.Max(12, effectiveRect.Height - 8));

            using (var titleBrush = new SolidBrush(foreColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                var titleRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width, Math.Max(14, contentRect.Height / 2));
                g.DrawString(evt.Title, ctx.DayFont, titleBrush, titleRect, sf);
            }

            using (var timeBrush = new SolidBrush(subtleTextColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                var timeText = evt.IsAllDay ? "All day" : $"{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}";
                int timeY = contentRect.Y + Math.Max(14, contentRect.Height / 2) - 2;
                var timeRect = new Rectangle(contentRect.X, timeY, contentRect.Width, Math.Max(12, contentRect.Height / 2));
                g.DrawString(timeText, ctx.EventFont, timeBrush, timeRect, sf);
            }

            if (includeDescription && !string.IsNullOrWhiteSpace(evt.Description) && contentRect.Height > 34)
            {
                using (var descriptionBrush = new SolidBrush(Color.FromArgb(160, subtleTextColor)))
                using (var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                })
                {
                    var descRect = new Rectangle(contentRect.X, contentRect.Y + 30, contentRect.Width, contentRect.Height - 30);
                    g.DrawString(evt.Description, ctx.EventFont, descriptionBrush, descRect, sf);
                }
            }

            if (includeActions)
            {
                DrawListActionChips(g, ctx.PrimaryColor, foreColor, ctx.EventFont, effectiveRect);
            }
        }

        public static void DrawMiniCalendarCard(Graphics g, CalendarRenderContext ctx, Rectangle bounds, DateTime displayMonth, DateTime selectedDate)
        {
            using (var backBrush = new SolidBrush(ctx.Theme?.CardBackColor ?? Color.White))
            using (var borderPen = new Pen(ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
            {
                g.FillRectangle(backBrush, bounds);
                g.DrawRectangle(borderPen, bounds);
            }

            var titleRect = new Rectangle(bounds.X + 8, bounds.Y + 6, bounds.Width - 16, 20);
            using (var titleBrush = new SolidBrush(ctx.Theme?.CalendarForeColor ?? Color.Black))
            {
                g.DrawString(displayMonth.ToString("MMMM yyyy"), ctx.DaysHeaderFont, titleBrush, titleRect, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
            }

            var subtitleRect = new Rectangle(bounds.X + 8, bounds.Y + 26, bounds.Width - 16, 16);
            using (var subtitleBrush = new SolidBrush(ctx.Theme?.CalendarDaysHeaderForColor ?? Color.Gray))
            {
                g.DrawString($"Selected: {selectedDate:ddd, MMM dd}", ctx.EventFont, subtitleBrush, subtitleRect, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
            }

            int cellSize = Math.Min(22, Math.Max(14, (bounds.Width - 16) / 7));
            int startY = bounds.Y + 46;
            string[] days = { "S", "M", "T", "W", "T", "F", "S" };

            for (int i = 0; i < 7; i++)
            {
                var dayRect = new Rectangle(bounds.X + 8 + i * cellSize, startY, cellSize, 14);
                using (var dayBrush = new SolidBrush(ctx.Theme?.CalendarDaysHeaderForColor ?? Color.Gray))
                {
                    g.DrawString(days[i], ctx.EventFont, dayBrush, dayRect, new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });
                }
            }

            var firstDay = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var firstCalendarDay = firstDay.AddDays(-(int)firstDay.DayOfWeek);
            var primary = ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244);

            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstCalendarDay.AddDays(week * 7 + day);
                    var cellRect = new Rectangle(bounds.X + 8 + day * cellSize, startY + 16 + week * cellSize, cellSize, cellSize);
                    bool isSelected = cellDate.Date == selectedDate.Date;
                    bool isToday = cellDate.Date == DateTime.Today;
                    bool isCurrentMonth = cellDate.Month == displayMonth.Month;

                    if (isSelected)
                    {
                        using (var fill = new SolidBrush(primary))
                        {
                            g.FillEllipse(fill, cellRect.X + 1, cellRect.Y + 1, cellRect.Width - 2, cellRect.Height - 2);
                        }
                    }
                    else if (isToday)
                    {
                        using (var pen = new Pen(primary, 1f))
                        {
                            g.DrawEllipse(pen, cellRect.X + 1, cellRect.Y + 1, cellRect.Width - 2, cellRect.Height - 2);
                        }
                    }

                    var textColor = isSelected
                        ? Color.White
                        : !isCurrentMonth
                            ? Color.FromArgb(120, ctx.Theme?.CalendarForeColor ?? Color.Black)
                            : ctx.Theme?.CalendarForeColor ?? Color.Black;

                    using (var dayBrush = new SolidBrush(textColor))
                    {
                        g.DrawString(cellDate.Day.ToString(), ctx.EventFont, dayBrush, cellRect, new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                }
            }
        }

        public static void DrawMiniCalendarCard(Graphics g, CalendarPainterContext ctx, Rectangle bounds, DateTime displayMonth, DateTime selectedDate)
        {
            using (var backBrush = new SolidBrush(ctx.BackgroundColor))
            using (var borderPen = new Pen(ctx.BorderColor))
            {
                g.FillRectangle(backBrush, bounds);
                g.DrawRectangle(borderPen, bounds);
            }

            var titleRect = new Rectangle(bounds.X + 8, bounds.Y + 6, bounds.Width - 16, 20);
            using (var titleBrush = new SolidBrush(ctx.ForegroundColor))
            {
                g.DrawString(displayMonth.ToString("MMMM yyyy"), ctx.DaysHeaderFont, titleBrush, titleRect, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
            }

            var subtitleRect = new Rectangle(bounds.X + 8, bounds.Y + 26, bounds.Width - 16, 16);
            using (var subtitleBrush = new SolidBrush(Color.FromArgb(150, ctx.ForegroundColor)))
            {
                g.DrawString($"Selected: {selectedDate:ddd, MMM dd}", ctx.EventFont, subtitleBrush, subtitleRect, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
            }

            int cellSize = Math.Min(22, Math.Max(14, (bounds.Width - 16) / 7));
            int startY = bounds.Y + 46;
            string[] days = { "S", "M", "T", "W", "T", "F", "S" };

            for (int i = 0; i < 7; i++)
            {
                var dayRect = new Rectangle(bounds.X + 8 + i * cellSize, startY, cellSize, 14);
                using (var dayBrush = new SolidBrush(Color.FromArgb(150, ctx.ForegroundColor)))
                {
                    g.DrawString(days[i], ctx.EventFont, dayBrush, dayRect, new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });
                }
            }

            var firstDay = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var firstCalendarDay = firstDay.AddDays(-(int)firstDay.DayOfWeek);

            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstCalendarDay.AddDays(week * 7 + day);
                    var cellRect = new Rectangle(bounds.X + 8 + day * cellSize, startY + 16 + week * cellSize, cellSize, cellSize);
                    bool isSelected = cellDate.Date == selectedDate.Date;
                    bool isToday = cellDate.Date == DateTime.Today;
                    bool isCurrentMonth = cellDate.Month == displayMonth.Month;

                    if (isSelected)
                    {
                        using (var fill = new SolidBrush(ctx.PrimaryColor))
                        {
                            g.FillEllipse(fill, cellRect.X + 1, cellRect.Y + 1, cellRect.Width - 2, cellRect.Height - 2);
                        }
                    }
                    else if (isToday)
                    {
                        using (var pen = new Pen(ctx.PrimaryColor, 1f))
                        {
                            g.DrawEllipse(pen, cellRect.X + 1, cellRect.Y + 1, cellRect.Width - 2, cellRect.Height - 2);
                        }
                    }

                    var textColor = isSelected
                        ? Color.White
                        : !isCurrentMonth
                            ? Color.FromArgb(120, ctx.ForegroundColor)
                            : ctx.ForegroundColor;

                    using (var dayBrush = new SolidBrush(textColor))
                    {
                        g.DrawString(cellDate.Day.ToString(), ctx.EventFont, dayBrush, cellRect, new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                }
            }
        }

        public static void DrawEventInsightsCard(Graphics g, CalendarRenderContext ctx, Rectangle bounds, CalendarEvent evt)
        {
            using (var backBrush = new SolidBrush(ctx.Theme?.CardBackColor ?? Color.White))
            using (var borderPen = new Pen(ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
            {
                g.FillRectangle(backBrush, bounds);
                g.DrawRectangle(borderPen, bounds);
            }

            var fore = ctx.Theme?.CalendarForeColor ?? Color.Black;
            var muted = ctx.Theme?.CalendarDaysHeaderForColor ?? Color.Gray;

            if (evt == null)
            {
                using (var titleBrush = new SolidBrush(fore))
                using (var mutedBrush = new SolidBrush(muted))
                {
                    g.DrawString("No event selected", ctx.DayFont, titleBrush, new Rectangle(bounds.X + 10, bounds.Y + 16, bounds.Width - 20, 24));
                    g.DrawString("Select an event or use + Create Event.", ctx.EventFont, mutedBrush, new Rectangle(bounds.X + 10, bounds.Y + 42, bounds.Width - 20, 36));
                    g.DrawString("Keyboard: Arrows move | Enter select", ctx.EventFont, mutedBrush, new Rectangle(bounds.X + 10, bounds.Bottom - 24, bounds.Width - 20, 16));
                }
                return;
            }

            var category = ctx.Categories.FirstOrDefault(c => c.Id == evt.CategoryId);
            var categoryColor = category?.Color ?? Color.Gray;

            int y = bounds.Y + 10;
            using (var titleBrush = new SolidBrush(fore))
            {
                g.DrawString(evt.Title, ctx.DayFont, titleBrush, new Rectangle(bounds.X + 10, y, bounds.Width - 20, 24));
            }
            y += 26;

            DrawMetadataRow(g, ctx, bounds, y, "Date", evt.StartTime.ToString("ddd, MMM dd yyyy"), muted);
            y += 18;
            DrawMetadataRow(g, ctx, bounds, y, "Time", evt.IsAllDay ? "All day" : $"{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}", muted);
            y += 18;
            DrawMetadataRow(g, ctx, bounds, y, "Category", category?.Name ?? "None", muted);
            y += 20;

            if (!string.IsNullOrWhiteSpace(evt.Location))
            {
                DrawMetadataRow(g, ctx, bounds, y, "Location", evt.Location, muted);
                y += 18;
            }

            if (!string.IsNullOrWhiteSpace(evt.Description))
            {
                using (var descBrush = new SolidBrush(Color.FromArgb(170, muted)))
                {
                    g.DrawString(evt.Description, ctx.EventFont, descBrush, new Rectangle(bounds.X + 10, y + 2, bounds.Width - 20, bounds.Bottom - y - 10));
                }
            }

            using (var chipBrush = new SolidBrush(Color.FromArgb(40, categoryColor)))
            using (var chipTextBrush = new SolidBrush(categoryColor))
            {
                var chipRect = new Rectangle(bounds.Right - 94, bounds.Y + 10, 84, 18);
                g.FillRectangle(chipBrush, chipRect);
                g.DrawString(category?.Name ?? "General", ctx.EventFont, chipTextBrush, chipRect, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
            }

            using (var hintBrush = new SolidBrush(Color.FromArgb(150, muted)))
            {
                g.DrawString("PgUp/PgDn navigate period", ctx.EventFont, hintBrush, new Rectangle(bounds.X + 10, bounds.Bottom - 20, bounds.Width - 20, 14));
            }
        }

        public static void DrawEventInsightsCard(Graphics g, CalendarPainterContext ctx, Rectangle bounds, CalendarEvent evt)
        {
            using (var backBrush = new SolidBrush(ctx.BackgroundColor))
            using (var borderPen = new Pen(ctx.BorderColor))
            {
                g.FillRectangle(backBrush, bounds);
                g.DrawRectangle(borderPen, bounds);
            }

            var fore = ctx.ForegroundColor;
            var muted = Color.FromArgb(150, ctx.ForegroundColor);

            if (evt == null)
            {
                using (var titleBrush = new SolidBrush(fore))
                using (var mutedBrush = new SolidBrush(muted))
                {
                    g.DrawString("No event selected", ctx.DayFont, titleBrush, new Rectangle(bounds.X + 10, bounds.Y + 16, bounds.Width - 20, 24));
                    g.DrawString("Select an event or use + Create Event.", ctx.EventFont, mutedBrush, new Rectangle(bounds.X + 10, bounds.Y + 42, bounds.Width - 20, 36));
                }
                return;
            }

            var category = ctx.Categories?.FirstOrDefault(c => c.Id == evt.CategoryId);
            var categoryColor = category?.Color ?? Color.Gray;

            int y = bounds.Y + 10;
            using (var titleBrush = new SolidBrush(fore))
            {
                g.DrawString(evt.Title, ctx.DayFont, titleBrush, new Rectangle(bounds.X + 10, y, bounds.Width - 20, 24));
            }
            y += 26;

            DrawMetadataRow(g, ctx.EventFont, fore, muted, bounds, y, "Date", evt.StartTime.ToString("ddd, MMM dd yyyy"));
            y += 18;
            DrawMetadataRow(g, ctx.EventFont, fore, muted, bounds, y, "Time", evt.IsAllDay ? "All day" : $"{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}");
            y += 18;
            DrawMetadataRow(g, ctx.EventFont, fore, muted, bounds, y, "Category", category?.Name ?? "None");
            y += 20;

            if (!string.IsNullOrWhiteSpace(evt.Location))
            {
                DrawMetadataRow(g, ctx.EventFont, fore, muted, bounds, y, "Location", evt.Location);
                y += 18;
            }

            if (!string.IsNullOrWhiteSpace(evt.Description))
            {
                using (var descBrush = new SolidBrush(Color.FromArgb(170, muted)))
                {
                    g.DrawString(evt.Description, ctx.EventFont, descBrush, new Rectangle(bounds.X + 10, y + 2, bounds.Width - 20, bounds.Bottom - y - 10));
                }
            }

            using (var chipBrush = new SolidBrush(Color.FromArgb(40, categoryColor)))
            using (var chipTextBrush = new SolidBrush(categoryColor))
            {
                var chipRect = new Rectangle(bounds.Right - 94, bounds.Y + 10, 84, 18);
                g.FillRectangle(chipBrush, chipRect);
                g.DrawString(category?.Name ?? "General", ctx.EventFont, chipTextBrush, chipRect, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
            }
        }

        private static void DrawMetadataRow(Graphics g, CalendarRenderContext ctx, Rectangle bounds, int y, string label, string value, Color muted)
        {
            using (var labelBrush = new SolidBrush(muted))
            using (var valueBrush = new SolidBrush(ctx.Theme?.CalendarForeColor ?? Color.Black))
            {
                g.DrawString($"{label}:", ctx.EventFont, labelBrush, new Rectangle(bounds.X + 10, y, 64, 16));
                g.DrawString(value, ctx.EventFont, valueBrush, new Rectangle(bounds.X + 76, y, bounds.Width - 86, 16));
            }
        }

        private static void DrawMetadataRow(Graphics g, Font font, Color fore, Color muted, Rectangle bounds, int y, string label, string value)
        {
            using (var labelBrush = new SolidBrush(muted))
            using (var valueBrush = new SolidBrush(fore))
            {
                g.DrawString($"{label}:", font, labelBrush, new Rectangle(bounds.X + 10, y, 64, 16));
                g.DrawString(value, font, valueBrush, new Rectangle(bounds.X + 76, y, bounds.Width - 86, 16));
            }
        }

        private static void DrawListActionChips(Graphics g, Color primary, Color fore, Font font, Rectangle cardRect)
        {
            var actionY = cardRect.Bottom - 22;
            var openRect = new Rectangle(cardRect.Right - 132, actionY, 56, 16);
            var editRect = new Rectangle(cardRect.Right - 70, actionY, 56, 16);

            using (var openBack = new SolidBrush(Color.FromArgb(34, primary)))
            using (var editBack = new SolidBrush(Color.FromArgb(24, fore)))
            using (var openText = new SolidBrush(primary))
            using (var editText = new SolidBrush(fore))
            {
                g.FillRectangle(openBack, openRect);
                g.FillRectangle(editBack, editRect);
                g.DrawString("Open", font, openText, openRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                g.DrawString("Edit", font, editText, editRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private static Color Blend(Color a, Color b, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            int r = (int)Math.Round(a.R + ((b.R - a.R) * amount));
            int g = (int)Math.Round(a.G + ((b.G - a.G) * amount));
            int bch = (int)Math.Round(a.B + ((b.B - a.B) * amount));
            return Color.FromArgb(r, g, bch);
        }
    }
}
