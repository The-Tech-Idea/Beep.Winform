using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// StatusList - Items with status indicators and toggle hit areas
    /// </summary>
    internal sealed class StatusListPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _rowRects    = new();
        private readonly List<Rectangle> _statusRects = new();
        private WidgetContext? _lastCtx;
        private bool _wheelHooked;

        private Font? _titleFont;
        private Font? _nameFont;
        private Font? _statusFont;

        private const int PadDp        = 16;
        private const int HeaderHDp    = 24;
        private const int ItemHeightDp = 32;
        private const int StatusBadgeW = 90;
        private const int DotSizeDp    = 8;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            if (!_wheelHooked && Owner != null) { Owner.MouseWheel += OnMouseWheel; _wheelHooked = true; }
        }

        protected override void RebuildFonts()
        {
            _titleFont?.Dispose(); _nameFont?.Dispose(); _statusFont?.Dispose();
            _titleFont  = BeepThemesManager.ToFont(Theme?.LabelMedium  ?? new TypographyStyle { FontSize = 11f, FontWeight = FontWeight.Bold }, true);
            _nameFont   = BeepThemesManager.ToFont(Theme?.LabelSmall   ?? new TypographyStyle { FontSize = 9f }, true);
            _statusFont = BeepThemesManager.ToFont(Theme?.CaptionStyle ?? new TypographyStyle { FontSize = 8f }, true);
        }

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (_lastCtx == null) return;
            int maxY = Math.Max(0, _lastCtx.TotalContentHeight - _lastCtx.ContentRect.Height);
            _lastCtx.ScrollOffsetY = Math.Max(0, Math.Min(_lastCtx.ScrollOffsetY - e.Delta / 120 * Dp(ItemHeightDp) * 3, maxY));
            Owner?.Invalidate();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = Dp(PadDp);
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            ctx.HeaderRect  = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, Dp(HeaderHDp));
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + Dp(8), ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);

            _rowRects.Clear(); _statusRects.Clear();
            var items = ctx.ListItems;
            int stride    = Dp(ItemHeightDp);
            int badgeW    = Dp(StatusBadgeW);
            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    int y = ctx.ContentRect.Y + i * stride;
                    var row = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, stride);
                    _rowRects.Add(row);
                    _statusRects.Add(new Rectangle(row.Right - badgeW, y + Dp(4), badgeW - Dp(4), stride - Dp(8)));
                }
                ctx.TotalContentHeight = items.Count * stride;
            }
            else ctx.TotalContentHeight = 0;
            ClampScrollOffset(ctx);
            _lastCtx = ctx;
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            var bgBrush = PaintersFactory.GetSolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                var titleBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
                if (_titleFont != null) g.DrawString(ctx.Title, _titleFont, titleBrush, ctx.HeaderRect);
            }
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
                DrawStatusItems(g, ctx, ctx.ContentRect, items);
        }

        private void DrawStatusItems(Graphics g, WidgetContext ctx, Rectangle rect, List<ListItem> items)
        {
            if (!items.Any()) return;

            int stride   = Dp(ItemHeightDp);
            int dotSize  = Dp(DotSizeDp);
            int badgeW   = Dp(StatusBadgeW);
            var overrides = ctx.StatusOverrides;
            var nameBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(180, Theme?.ForeColor ?? Color.Black));
            var nameFormat   = new StringFormat { LineAlignment = StringAlignment.Center };
            var statusFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

            var savedClip = g.Clip;
            g.SetClip(rect);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + stride < rect.Y) continue;
                if (y > rect.Bottom) break;

                var rowRect  = new Rectangle(rect.X, y, rect.Width, stride);
                var statRect = new Rectangle(rect.Right - badgeW, y + Dp(4), badgeW - Dp(4), stride - Dp(8));

                if (IsAreaHovered($"StatusList_Row_{i}"))
                {
                    using var hoverBg = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(hoverBg, rowRect);
                }

                string status = overrides.FirstOrDefault(o => o.RowIndex == i)?.StatusText ?? (!string.IsNullOrEmpty(item.Status) ? item.Status : "Unknown");
                Color statusColor = GetStatusColor(status);
                var dotRect = new Rectangle(rowRect.X + Dp(8), y + stride / 2 - dotSize / 2, dotSize, dotSize);
                var dotBrush = PaintersFactory.GetSolidBrush(statusColor);
                g.FillEllipse(dotBrush, dotRect);

                var nameRect = new Rectangle(rowRect.X + Dp(24), y, rowRect.Width - badgeW - Dp(8), stride);
                if (!string.IsNullOrEmpty(item.Title) && _nameFont != null)
                    g.DrawString(item.Title, _nameFont, nameBrush, nameRect, nameFormat);

                var statusBrush = PaintersFactory.GetSolidBrush(statusColor);
                if (_statusFont != null)
                    g.DrawString(status, _statusFont, statusBrush, statRect, statusFormat);

                if (IsAreaHovered($"StatusList_Status_{i}"))
                {
                    var hoverPen = PaintersFactory.GetPen(Theme?.AccentColor ?? Color.Blue, 1);
                    g.DrawRectangle(hoverPen, statRect);
                }
            }

            g.Clip = savedClip;
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
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, IsAreaHovered("StatusList_Scroll"));
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
                    ctx.SelectedStatusRowIndex = idx;
                    notifyAreaHit?.Invoke($"StatusList_Row_{idx}", rect);
                    Owner?.Invalidate();
                });
                owner.AddHitArea($"StatusList_Status_{idx}", statRect, null, () =>
                {
                    // Toggle status override without mutating original items
                    var map = ctx.StatusOverrides;
                    var currentOverride = map.FirstOrDefault(o => o.RowIndex == idx);

                    string current;
                    if (currentOverride != null && !string.IsNullOrEmpty(currentOverride.StatusText)) current = currentOverride.StatusText;
                    else if (ctx.ListItems != null && idx < ctx.ListItems.Count && !string.IsNullOrEmpty(ctx.ListItems[idx].Status))
                        current = ctx.ListItems[idx].Status;
                    else current = "Unknown";

                    string next = NextStatus(current);
                    if (currentOverride == null)
                    {
                        map.Add(new StatusOverride { RowIndex = idx, StatusText = next });
                    }
                    else
                    {
                        currentOverride.StatusText = next;
                    }
                    ctx.ToggledStatusRowIndex = idx;
                    ctx.ToggledStatusNewValue = next;

                    notifyAreaHit?.Invoke($"StatusList_Status_{idx}", statRect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}
