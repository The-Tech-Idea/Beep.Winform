using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Menu - Vertical menu list with hover/active states and icons (interactive)
    /// + Keyboard: Up/Down change active; Enter selects; Home/End jump; PageUp/Down jump by section size
    /// </summary>
    internal sealed class MenuPainter : WidgetPainterBase, IDisposable
    {
        private readonly List<Rectangle> _itemRects = new();
        private int _activeIndex;
        private int _itemsCount;
        private bool _keysHooked;
        private bool _wheelHooked;
        private WidgetContext _lastCtx;

        private Font? _itemFont;

        private const int ItemHeightDp = 28;
        private const int PadDp        = 8;
        private const int CornerDp     = 6;

        protected override void RebuildFonts()
        {
            _itemFont?.Dispose();
            _itemFont = BeepThemesManager.ToFont(Theme?.LabelSmall ?? new TypographyStyle { FontSize = 9f }, true);
        }

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            HookKeyboard();
            if (!_wheelHooked && Owner != null) { Owner.MouseWheel += OnMouseWheel; _wheelHooked = true; }
        }

        private void HookKeyboard()
        {
            if (_keysHooked || Owner == null) return;
            try
            {
                Owner._input.UpArrowKeyPressed += OnUp;
                Owner._input.DownArrowKeyPressed += OnDown;
                Owner._input.HomeKeyPressed += OnHome;
                Owner._input.EndKeyPressed += OnEnd;
                Owner._input.PageUpKeyPressed += OnPageUp;
                Owner._input.PageDownKeyPressed += OnPageDown;
                Owner._input.EnterKeyPressed += OnEnter;
                _keysHooked = true;
            }
            catch { }
        }

        private int PageSize => Math.Max(1, _itemRects.Count);

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (_lastCtx == null) return;
            int maxY = Math.Max(0, _lastCtx.TotalContentHeight - _lastCtx.ContentRect.Height);
            _lastCtx.ScrollOffsetY = Math.Max(0, Math.Min(_lastCtx.ScrollOffsetY - e.Delta / 120 * Dp(ItemHeightDp) * 3, maxY));
            Owner?.Invalidate();
        }

        private void UpdateActive(int idx)
        {
            if (_itemsCount <= 0 || _lastCtx == null) return;
            _activeIndex = Math.Clamp(idx, 0, _itemsCount - 1);
            _lastCtx.ActiveIndex = _activeIndex;
            Owner?.Invalidate();
        }

        private void OnUp(object? s, EventArgs e) => UpdateActive(_activeIndex - 1);
        private void OnDown(object? s, EventArgs e) => UpdateActive(_activeIndex + 1);
        private void OnHome(object? s, EventArgs e) => UpdateActive(0);
        private void OnEnd(object? s, EventArgs e) => UpdateActive(_itemsCount - 1);
        private void OnPageUp(object? s, EventArgs e) => UpdateActive(_activeIndex - PageSize);
        private void OnPageDown(object? s, EventArgs e) => UpdateActive(_activeIndex + PageSize);

        private void OnEnter(object? s, EventArgs e)
        {
            if (Owner == null) return;
            string id = $"Menu_Item_{_activeIndex}";
            try
            {
                var hit = Owner._hitTest?.HitList?.FirstOrDefault(h => h.Name == id);
                hit?.HitAction?.Invoke();
            }
            catch { }
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = Dp(PadDp);
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - pad * 2);

            _itemRects.Clear();
            var items = ctx.NavigationItems?.Select(n => n.Text).ToList() ?? new List<string> { "Home", "Analytics", "Reports", "Settings" };
            _itemsCount = items.Count;
            _activeIndex = ctx.ActiveIndex > 0 ? Math.Clamp(ctx.ActiveIndex, 0, Math.Max(0, _itemsCount - 1)) : 0;

            int stride = Dp(ItemHeightDp);
            for (int i = 0; i < items.Count; i++)
                _itemRects.Add(new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + i * stride, ctx.ContentRect.Width, stride));

            ctx.TotalContentHeight = items.Count * stride;
            ClampScrollOffset(ctx);
            _lastCtx = ctx;
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var bg = PaintersFactory.GetSolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bg, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _lastCtx = ctx;
            var items = ctx.NavigationItems?.Select(n => n.Text).ToList() ?? new List<string> { "Home", "Analytics", "Reports", "Settings" };
            int active = _activeIndex;
            int stride = Dp(ItemHeightDp);
            var corner = Dp(CornerDp);

            var activeBrush  = PaintersFactory.GetSolidBrush(Theme?.ForeColor ?? Color.Black);
            var normalBrush  = PaintersFactory.GetSolidBrush(Color.FromArgb(160, Theme?.ForeColor ?? Color.Black));
            var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

            var savedClip = g.Clip;
            g.SetClip(ctx.ContentRect);

            for (int i = 0; i < items.Count; i++)
            {
                int y = ctx.ContentRect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + stride < ctx.ContentRect.Y) continue;
                if (y > ctx.ContentRect.Bottom) break;

                var rect    = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, stride);
                bool isActive = i == active;
                bool hv = IsAreaHovered($"Menu_Item_{i}");

                if (isActive || hv)
                {
                    using var layer = new SolidBrush(Color.FromArgb(isActive ? 20 : 8, Theme?.PrimaryColor ?? Color.SteelBlue));
                    g.FillRoundedRectangle(layer, rect, corner);
                }

                var textRect = Rectangle.Inflate(rect, -Dp(10), 0);
                if (_itemFont != null)
                    g.DrawString(items[i], _itemFont, isActive ? activeBrush : normalBrush, textRect, fmt);

                if (isActive)
                {
                    var chevronPen = PaintersFactory.GetPen(Theme?.PrimaryColor ?? Color.SteelBlue, 2f);
                    int cx = rect.Right - Dp(10); int cy = rect.Y + rect.Height / 2;
                    g.DrawLine(chevronPen, cx, cy - Dp(4), cx + Dp(4), cy);
                    g.DrawLine(chevronPen, cx, cy + Dp(4), cx + Dp(4), cy);
                }
            }

            g.Clip = savedClip;
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, IsAreaHovered("Menu_Scroll"));
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            int stride = Dp(ItemHeightDp);
            var items = ctx.NavigationItems?.Select(n => n.Text).ToList() ?? new List<string> { "Home", "Analytics", "Reports", "Settings" };
            for (int i = 0; i < items.Count; i++)
            {
                int idx = i;
                int y = ctx.ContentRect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + stride < ctx.ContentRect.Y || y > ctx.ContentRect.Bottom) continue;
                var rect = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, stride);
                owner.AddHitArea($"Menu_Item_{idx}", rect, null, () =>
                {
                    ctx.ActiveIndex = idx;
                    _activeIndex = idx;
                    notifyAreaHit?.Invoke($"Menu_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            if (_keysHooked && Owner != null)
            {
                try
                {
                    Owner._input.UpArrowKeyPressed -= OnUp;
                    Owner._input.DownArrowKeyPressed -= OnDown;
                    Owner._input.HomeKeyPressed -= OnHome;
                    Owner._input.EndKeyPressed -= OnEnd;
                    Owner._input.PageUpKeyPressed -= OnPageUp;
                    Owner._input.PageDownKeyPressed -= OnPageDown;
                    Owner._input.EnterKeyPressed -= OnEnter;
                    _keysHooked = false;
                }
                catch { }
            }
            if (_wheelHooked && Owner != null) { Owner.MouseWheel -= OnMouseWheel; _wheelHooked = false; }
            _itemFont?.Dispose();
        }
    }
}
