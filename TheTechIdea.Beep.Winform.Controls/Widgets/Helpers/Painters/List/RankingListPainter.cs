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
    /// RankingList - Ordered ranking list with per-row hit areas
    /// </summary>
    internal sealed class RankingListPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _rowRects = new();
        private WidgetContext? _lastCtx;
        private bool _wheelHooked;

        private Font? _titleFont;
        private Font? _nameFont;
        private Font? _rankFont;
        private Font? _valueFont;

        private const int PadDp        = 16;
        private const int HeaderHDp    = 24;
        private const int ItemHeightDp = 32;
        private const int RankBadgeDp  = 24;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            if (!_wheelHooked && Owner != null) { Owner.MouseWheel += OnMouseWheel; _wheelHooked = true; }
        }

        protected override void RebuildFonts()
        {
            _titleFont?.Dispose(); _nameFont?.Dispose(); _rankFont?.Dispose(); _valueFont?.Dispose();
            _titleFont = BeepThemesManager.ToFont(Theme?.LabelMedium  ?? new TypographyStyle { FontSize = 11f, FontWeight = FontWeight.Bold }, true);
            _nameFont  = BeepThemesManager.ToFont(Theme?.LabelSmall   ?? new TypographyStyle { FontSize = 9f }, true);
            _rankFont  = BeepThemesManager.ToFont(Theme?.LabelSmall   ?? new TypographyStyle { FontSize = 10f, FontWeight = FontWeight.Bold }, true);
            _valueFont = BeepThemesManager.ToFont(Theme?.CaptionStyle ?? new TypographyStyle { FontSize = 8f }, true);
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

            _rowRects.Clear();
            var items = ctx.ListItems;
            int stride = Dp(ItemHeightDp);
            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                    _rowRects.Add(new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + i * stride, ctx.ContentRect.Width, stride));
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
                DrawRankingItems(g, ctx, ctx.ContentRect, items);
        }

        private void DrawRankingItems(Graphics g, WidgetContext ctx, Rectangle rect, List<ListItem> items)
        {
            if (!items.Any()) return;

            int stride    = Dp(ItemHeightDp);
            int badgeW    = Dp(RankBadgeDp);
            var nameBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(180, Theme?.ForeColor ?? Color.Black));
            var valBrush  = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
            var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
            var valFormat  = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

            var savedClip = g.Clip;
            g.SetClip(rect);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + stride < rect.Y) continue;
                if (y > rect.Bottom) break;

                var rowRect = new Rectangle(rect.X, y, rect.Width, stride);

                if (IsAreaHovered($"RankingList_Row_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(hover, rowRect);
                }

                var rankRect = new Rectangle(rowRect.X, rowRect.Y + Dp(4), badgeW, stride - Dp(8));
                Color rankColor = i < 3 ? GetRankColor(i) : Color.FromArgb(100, Color.Gray);
                using var rankFill = new SolidBrush(rankColor);
                using var rankPath = CreateRoundedPath(rankRect, Dp(4));
                g.FillPath(rankFill, rankPath);
                var whiteBrush = PaintersFactory.GetSolidBrush(Color.White);
                var rankFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                if (_rankFont != null) g.DrawString((i + 1).ToString(), _rankFont, whiteBrush, rankRect, rankFormat);

                var contentRect = new Rectangle(rowRect.X + badgeW + Dp(8), rowRect.Y, rowRect.Width - badgeW - Dp(68), stride);
                var valueRect   = new Rectangle(rowRect.Right - Dp(60), rowRect.Y, Dp(60), stride);

                if (!string.IsNullOrEmpty(item.Title) && _nameFont != null)
                    g.DrawString(item.Title, _nameFont, nameBrush, contentRect, nameFormat);
                if (!string.IsNullOrEmpty(item.Subtitle) && _valueFont != null)
                    g.DrawString(item.Subtitle, _valueFont, valBrush, valueRect, valFormat);
            }

            g.Clip = savedClip;
        }

        private Color GetRankColor(int rank)
        {
            return rank switch
            {
                0 => Color.FromArgb(255, 193, 7),  // Gold
                1 => Color.FromArgb(158, 158, 158), // Silver
                2 => Color.FromArgb(205, 127, 50),  // Bronze
                _ => Color.Gray
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, IsAreaHovered("RankingList_Scroll"));
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _rowRects.Count; i++)
            {
                int idx = i;
                var rect = _rowRects[i];
                owner.AddHitArea($"RankingList_Row_{idx}", rect, null, () =>
                {
                    ctx.SelectedRankIndex = idx;
                    notifyAreaHit?.Invoke($"RankingList_Row_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}