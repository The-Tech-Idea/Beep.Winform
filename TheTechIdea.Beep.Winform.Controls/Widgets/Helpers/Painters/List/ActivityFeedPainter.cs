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
    /// ActivityFeed - Timeline-Style activities with hover and per-item hit areas
    /// </summary>
    internal sealed class ActivityFeedPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _itemRects = new();
        private WidgetContext? _lastCtx;
        private bool _wheelHooked;

        // Cached fonts
        private Font? _titleFont;
        private Font? _nameFont;
        private Font? _timeFont;

        private const int PadDp        = 16;
        private const int HeaderHDp    = 24;
        private const int ItemHeightDp = 40;
        private const int DotSizeDp    = 8;
        private const int TimelinePadDp = 24;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            if (!_wheelHooked && Owner != null)
            {
                Owner.MouseWheel += OnMouseWheel;
                _wheelHooked = true;
            }
        }

        protected override void RebuildFonts()
        {
            _titleFont?.Dispose(); _nameFont?.Dispose(); _timeFont?.Dispose();
            _titleFont = BeepThemesManager.ToFont(Theme?.LabelMedium    ?? new TypographyStyle { FontSize = 11f, FontWeight = FontWeight.Bold }, true);
            _nameFont  = BeepThemesManager.ToFont(Theme?.LabelSmall     ?? new TypographyStyle { FontSize = 9f }, true);
            _timeFont  = BeepThemesManager.ToFont(Theme?.CaptionStyle   ?? new TypographyStyle { FontSize = 8f }, true);
        }

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (_lastCtx == null) return;
            int stride = Dp(ItemHeightDp);
            int maxY = Math.Max(0, _lastCtx.TotalContentHeight - _lastCtx.ContentRect.Height);
            _lastCtx.ScrollOffsetY = Math.Max(0, Math.Min(_lastCtx.ScrollOffsetY - e.Delta / 120 * stride * 3, maxY));
            Owner?.Invalidate();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = Dp(PadDp);
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top  + pad,
                ctx.DrawingRect.Width - pad * 2,
                Dp(HeaderHDp));

            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + Dp(8),
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);

            _itemRects.Clear();
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
            {
                int stride = Dp(ItemHeightDp);
                for (int i = 0; i < items.Count; i++)
                    _itemRects.Add(new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + i * stride, ctx.ContentRect.Width, stride));
                ctx.TotalContentHeight = items.Count * stride;
            }
            else
            {
                ctx.TotalContentHeight = 0;
            }
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
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                if (_titleFont != null)
                    g.DrawString(ctx.Title, _titleFont, titleBrush, ctx.HeaderRect, format);
            }

            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
                DrawActivityItems(g, ctx, ctx.ContentRect, items);
        }

        private void DrawActivityItems(Graphics g, WidgetContext ctx, Rectangle rect, List<ListItem> items)
        {
            if (!items.Any()) return;

            int stride     = Dp(ItemHeightDp);
            int dotSize    = Dp(DotSizeDp);
            int timelinePad = Dp(TimelinePadDp);
            var nameBrush  = PaintersFactory.GetSolidBrush(Color.FromArgb(180, Theme?.ForeColor ?? Color.Black));
            var timeBrush  = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray));
            var dotBrush   = PaintersFactory.GetSolidBrush(Theme?.AccentColor ?? Color.Blue);

            var savedClip = g.Clip;
            g.SetClip(rect);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + stride < rect.Y) continue;
                if (y > rect.Bottom) break;

                var itemRect = new Rectangle(rect.X, y, rect.Width, stride);

                if (IsAreaHovered($"ActivityFeed_Item_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(hover, itemRect);
                }

                var dotRect = new Rectangle(rect.X + Dp(8), y + stride / 2 - dotSize / 2, dotSize, dotSize);
                g.FillEllipse(dotBrush, dotRect);

                if (i < items.Count - 1)
                {
                    var linePen = PaintersFactory.GetPen(Color.FromArgb(50, Theme?.BorderColor ?? Color.Gray), 1);
                    g.DrawLine(linePen, dotRect.X + dotSize / 2, dotRect.Bottom, dotRect.X + dotSize / 2, y + stride);
                }

                int textX = rect.X + timelinePad;
                int textW = rect.Width - timelinePad - Dp(8);
                var contentRect = new Rectangle(textX, y + Dp(4), textW, stride - Dp(8));
                var nameRect    = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width * 2 / 3, contentRect.Height / 2);
                var timeRect    = new Rectangle(contentRect.Right - contentRect.Width / 3, contentRect.Y, contentRect.Width / 3, contentRect.Height / 2);
                var descRect    = new Rectangle(contentRect.X, contentRect.Y + contentRect.Height / 2, contentRect.Width, contentRect.Height / 2);

                if (!string.IsNullOrEmpty(item.Title) && _nameFont != null)
                    g.DrawString(item.Title, _nameFont, nameBrush, nameRect);

                if (item.Timestamp != default(DateTime) && _timeFont != null)
                {
                    var tf = new StringFormat { Alignment = StringAlignment.Far };
                    g.DrawString(item.Timestamp.ToString("HH:mm"), _timeFont, timeBrush, timeRect, tf);
                }

                if (!string.IsNullOrEmpty(item.Subtitle) && _timeFont != null)
                    g.DrawString(item.Subtitle, _timeFont, timeBrush, descRect);
            }

            g.Clip = savedClip;
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, IsAreaHovered("ActivityFeed_Scroll"));
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = i;
                var rect = _itemRects[i];
                owner.AddHitArea($"ActivityFeed_Item_{idx}", rect, null, () =>
                {
                    ctx.SelectedActivityIndex = idx;
                    notifyAreaHit?.Invoke($"ActivityFeed_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}