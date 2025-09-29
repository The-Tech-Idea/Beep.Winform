using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// StatusList - Items with status indicators and toggle hit areas
    /// </summary>
    internal sealed class StatusListPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _rowRects = new();
        private readonly List<Rectangle> _statusRects = new();

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);

            _rowRects.Clear();
            _statusRects.Clear();
            if (ctx.CustomData.TryGetValue("Items", out var raw) && raw is List<Dictionary<string, object>> items)
            {
                int itemHeight = Math.Min(28, ctx.ContentRect.Height / Math.Max(items.Count, 1));
                for (int i = 0; i < items.Count; i++)
                {
                    int y = ctx.ContentRect.Y + i * itemHeight;
                    var row = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, itemHeight);
                    _rowRects.Add(row);
                    _statusRects.Add(new Rectangle(row.Right - 90, y + 4, 86, itemHeight - 8));
                }
            }
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            if (ctx.CustomData.TryGetValue("Items", out var raw) && raw is List<Dictionary<string, object>> items)
            {
                DrawStatusItems(g, ctx, ctx.ContentRect, items);
            }
        }

        private void DrawStatusItems(Graphics g, WidgetContext ctx, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(28, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);

            // Optional local overrides for status after toggle (kept in CustomData)
            var overrides = ctx.CustomData.ContainsKey("StatusOverrides") && ctx.CustomData["StatusOverrides"] is Dictionary<int, string> map
                ? map : null;
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                var rowRect = _rowRects.Count > i ? _rowRects[i] : new Rectangle(rect.X, y, rect.Width, itemHeight);
                var statRect = _statusRects.Count > i ? _statusRects[i] : new Rectangle(rect.Right - 90, y + 4, 86, itemHeight - 8);

                // Row hover background
                if (IsAreaHovered($"StatusList_Row_{i}"))
                {
                    using var hoverBg = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(hoverBg, rowRect);
                }

                // Status indicator dot
                string status = overrides != null && overrides.ContainsKey(i)
                    ? overrides[i]
                    : item.ContainsKey("Status") ? item["Status"].ToString() : "Unknown";
                Color statusColor = GetStatusColor(status);
                var dotRect = new Rectangle(rowRect.X + 8, y + itemHeight / 2 - 4, 8, 8);
                using var dotBrush = new SolidBrush(statusColor);
                g.FillEllipse(dotBrush, dotRect);

                // Item name
                var nameRect = new Rectangle(rowRect.X + 24, y, rowRect.Width - 120, itemHeight);
                if (item.ContainsKey("Name"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, nameRect, nameFormat);
                }

                // Status text (toggle target)
                using var statusBrush = new SolidBrush(statusColor);
                var statusFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(status, statusFont, statusBrush, statRect, statusFormat);

                // Hover cue over status zone
                if (IsAreaHovered($"StatusList_Status_{i}"))
                {
                    using var hover = new Pen(Theme?.AccentColor ?? Color.Blue, 1);
                    g.DrawRectangle(hover, statRect);
                }
            }
        }

        private Color GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "active" or "online" or "running" => Color.Green,
                "inactive" or "offline" or "stopped" => Color.Red,
                "busy" or "warning" or "pending" => Color.Orange,
                "idle" or "paused" => Color.Gray,
                _ => Color.Gray
            };
        }

        private string NextStatus(string status)
        {
            // Simple cycle Active -> Busy -> Inactive -> Active
            var s = status?.ToLowerInvariant() ?? string.Empty;
            if (s is "active" or "online" or "running") return "Busy";
            if (s is "busy" or "warning" or "pending") return "Inactive";
            if (s is "inactive" or "offline" or "stopped") return "Active";
            if (s is "idle" or "paused") return "Active";
            return "Active";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Legend or summary can be drawn here
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _rowRects.Count; i++)
            {
                int idx = i;
                var rect = _rowRects[i];
                var statRect = _statusRects[i];

                owner.AddHitArea($"StatusList_Row_{idx}", rect, null, () =>
                {
                    ctx.CustomData["SelectedStatusRowIndex"] = idx;
                    notifyAreaHit?.Invoke($"StatusList_Row_{idx}", rect);
                    Owner?.Invalidate();
                });
                owner.AddHitArea($"StatusList_Status_{idx}", statRect, null, () =>
                {
                    // Toggle in StatusOverrides without mutating original Items
                    if (!ctx.CustomData.ContainsKey("StatusOverrides") || ctx.CustomData["StatusOverrides"] is not Dictionary<int, string>)
                        ctx.CustomData["StatusOverrides"] = new Dictionary<int, string>();
                    var map = (Dictionary<int, string>)ctx.CustomData["StatusOverrides"];

                    // Determine current status (override -> items)
                    string current;
                    if (map.ContainsKey(idx)) current = map[idx];
                    else if (ctx.CustomData.TryGetValue("Items", out var raw) && raw is List<Dictionary<string, object>> items && idx < items.Count && items[idx].ContainsKey("Status"))
                        current = items[idx]["Status"].ToString();
                    else current = "Unknown";

                    string next = NextStatus(current);
                    map[idx] = next;
                    ctx.CustomData["ToggledStatusRowIndex"] = idx;
                    ctx.CustomData["ToggledStatusNewValue"] = next;

                    notifyAreaHit?.Invoke($"StatusList_Status_{idx}", statRect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}