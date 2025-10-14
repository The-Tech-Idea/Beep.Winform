using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// ScheduleCard - Schedule/appointment display painter with hit areas and hover accents
    /// </summary>
    internal sealed class ScheduleCardPainter : WidgetPainterBase
    {
        private readonly List<(Rectangle rect, int index)> _eventRects = new();
        private Rectangle _headerRectCache;
        private Rectangle _statusRectCache;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);

            // Header (date)
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                30
            );

            // Content for events
            int contentTop = ctx.HeaderRect.Bottom + 13; // aligns with previous startY logic
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                Math.Max(0, ctx.DrawingRect.Bottom - contentTop - pad)
            );

            // Status area (top-right small capsule)
            _statusRectCache = new Rectangle(ctx.DrawingRect.Right - 40, ctx.DrawingRect.Top + 12, 30, 20);

            _headerRectCache = ctx.HeaderRect;
            _eventRects.Clear();
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            DrawScheduleHeader(g, ctx);
            DrawScheduleEvents(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) 
        {
            DrawStatusIndicators(g, ctx);

            // Hover accents
            if (IsAreaHovered("Schedule_Header"))
            {
                using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRectangle(hover, _headerRectCache);
            }
            if (IsAreaHovered("Schedule_Status"))
            {
                using var pen = new Pen(Theme?.PrimaryColor ?? Color.Blue, 1.2f);
                g.DrawRectangle(pen, _statusRectCache);
            }
        }

        private void DrawScheduleHeader(Graphics g, WidgetContext ctx)
        {
            // Draw date header (allow override via CustomData["ScheduleDate"])
            var date = ctx.CustomData.ContainsKey("ScheduleDate") ? Convert.ToDateTime(ctx.CustomData["ScheduleDate"]) : DateTime.Today;
            var headerRect = ctx.HeaderRect;

            using var dateFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var dayFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            using var dateBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.Black);
            using var dayBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));

            // Main date
            var dateText = date.ToString("MMM dd");
            var dateSize = TextUtils.MeasureText(g,dateText, dateFont);
            g.DrawString(dateText, dateFont, dateBrush, headerRect.Left, headerRect.Top);

            // Day of week
            var dayText = date.ToString("dddd");
            g.DrawString(dayText, dayFont, dayBrush, headerRect.Left, headerRect.Top + dateSize.Height);
        }

        private struct ScheduleItem
        {
            public string Time;
            public string Title;
            public string Type;
            public string Priority;
            public Color? Color;
        }

        private List<ScheduleItem> GetScheduleItems(WidgetContext ctx)
        {
            if (ctx.CustomData.TryGetValue("ScheduleEvents", out var raw) && raw is IEnumerable<Dictionary<string, object>> dicts)
            {
                var list = new List<ScheduleItem>();
                foreach (var d in dicts)
                {
                    list.Add(new ScheduleItem
                    {
                        Time = d.TryGetValue("Time", out var t) ? t?.ToString() ?? string.Empty : string.Empty,
                        Title = d.TryGetValue("Title", out var ti) ? ti?.ToString() ?? string.Empty : string.Empty,
                        Type = d.TryGetValue("Type", out var ty) ? ty?.ToString() ?? string.Empty : string.Empty,
                        Priority = d.TryGetValue("Priority", out var p) ? p?.ToString() ?? "low" : "low",
                        Color = d.TryGetValue("Color", out var c) && c is Color col ? col : null
                    });
                }
                return list;
            }

            return new List<ScheduleItem>
            {
                new() { Time = "09:00", Title = "Daily Standup", Type = "meeting", Priority = "high" },
                new() { Time = "10:30", Title = "Code Review", Type = "review", Priority = "medium" },
                new() { Time = "14:00", Title = "Client Demo", Type = "demo", Priority = "high" },
                new() { Time = "16:00", Title = "Team Sync", Type = "meeting", Priority = "low" }
            };
        }

        private void DrawScheduleEvents(Graphics g, WidgetContext ctx)
        {
            var events = GetScheduleItems(ctx);
            int eventHeight = 35;
            int padding = 12;
            int startY = ctx.ContentRect.Y;

            using var timeFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Bold);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Theme?.AccentColor ?? Color.Blue);
            using var titleBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.Black);

            _eventRects.Clear();
            for (int i = 0; i < events.Count && startY + i * (eventHeight + 8) + eventHeight <= ctx.ContentRect.Bottom; i++)
            {
                var evt = events[i];
                var eventRect = new Rectangle(
                    ctx.ContentRect.Left,
                    startY + i * (eventHeight + 8),
                    ctx.ContentRect.Width,
                    eventHeight
                );
                _eventRects.Add((eventRect, i));

                bool hovered = IsAreaHovered($"Schedule_Event_{i}");
                if (hovered)
                {
                    using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                    using var path = CreateRoundedPath(Rectangle.Inflate(eventRect, 2, 2), 4);
                    g.FillPath(hover, path);
                }

                // Time column
                var timeRect = new Rectangle(eventRect.Left, eventRect.Top, 50, eventHeight);
                g.DrawString(evt.Time, timeFont, timeBrush, timeRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                // Title
                var titleRect = new Rectangle(eventRect.Left + 60, eventRect.Top + 5, eventRect.Width - 80, 18);
                g.DrawString(evt.Title, titleFont, titleBrush, titleRect);

                // Type (subtitle)
                var typeRect = new Rectangle(eventRect.Left + 60, eventRect.Top + 20, eventRect.Width - 80, 12);
                using (var typeFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Italic))
                using (var typeBrush = new SolidBrush(Color.FromArgb(100, Theme?.ForeColor ?? Color.Black)))
                {
                    g.DrawString(evt.Type, typeFont, typeBrush, typeRect);
                }

                // Priority indicator (right stripe)
                Color priorityColor = evt.Priority switch
                {
                    "high" => Color.FromArgb(244, 67, 54),
                    "medium" => Color.FromArgb(255, 193, 7),
                    _ => Color.FromArgb(76, 175, 80)
                };
                if (evt.Color.HasValue)
                    priorityColor = evt.Color.Value;

                var priorityRect = new Rectangle(eventRect.Right - 8, eventRect.Top + 8, 4, eventHeight - 16);
                using var priorityBrush = new SolidBrush(priorityColor);
                g.FillRectangle(priorityBrush, priorityRect);
            }
        }

        private void DrawStatusIndicators(Graphics g, WidgetContext ctx)
        {
            // Overall schedule status, allow override via CustomData["Completed"] and ["Total"]
            int completed = ctx.CustomData.TryGetValue("Completed", out var c) && int.TryParse(c?.ToString(), out var ci) ? ci : _eventRects.Count;
            int total = ctx.CustomData.TryGetValue("Total", out var t) && int.TryParse(t?.ToString(), out var ti) ? ti : _eventRects.Count;
            total = Math.Max(total, completed);

            using var statusFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Bold);
            using var statusBrush = new SolidBrush(Color.FromArgb(76, 175, 80));

            g.DrawString($"{completed}/{total}", statusFont, statusBrush, _statusRectCache,
                       new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            // Completion bar under status
            int barWidth = _statusRectCache.Width + 4;
            var completionRect = new Rectangle(_statusRectCache.Left - 2, _statusRectCache.Bottom + 2, barWidth, 2);
            using var completionBrush = new SolidBrush(Color.FromArgb(40, 76, 175, 80));
            g.FillRectangle(completionBrush, completionRect);
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            // Header
            if (!_headerRectCache.IsEmpty)
            {
                owner.AddHitArea("Schedule_Header", _headerRectCache, null, () =>
                {
                    ctx.CustomData["HeaderClicked"] = true;
                    notifyAreaHit?.Invoke("Schedule_Header", _headerRectCache);
                    Owner?.Invalidate();
                });
            }

            // Events
            for (int i = 0; i < _eventRects.Count; i++)
            {
                int idx = _eventRects[i].index;
                var rect = _eventRects[i].rect;
                string name = $"Schedule_Event_{idx}";
                owner.AddHitArea(name, rect, null, () =>
                {
                    ctx.CustomData["SelectedScheduleIndex"] = idx;
                    notifyAreaHit?.Invoke(name, rect);
                    Owner?.Invalidate();
                });
            }

            // Status
            if (!_statusRectCache.IsEmpty)
            {
                owner.AddHitArea("Schedule_Status", _statusRectCache, null, () =>
                {
                    ctx.CustomData["StatusClicked"] = true;
                    notifyAreaHit?.Invoke("Schedule_Status", _statusRectCache);
                    Owner?.Invalidate();
                });
            }
        }
    }
}