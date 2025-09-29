using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

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
        private WidgetContext _lastCtx;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            HookKeyboard();
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

        private void UpdateActive(int idx)
        {
            if (_itemsCount <= 0 || _lastCtx == null) return;
            _activeIndex = Math.Clamp(idx, 0, _itemsCount - 1);
            _lastCtx.CustomData["ActiveIndex"] = _activeIndex;
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
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - pad * 2);

            _itemRects.Clear();
            var items = ctx.CustomData.ContainsKey("Items") ? (List<string>)ctx.CustomData["Items"] : new List<string> { "Home", "Analytics", "Reports", "Settings" };
            _itemsCount = items.Count;
            _activeIndex = ctx.CustomData.ContainsKey("ActiveIndex") ? Math.Clamp((int)ctx.CustomData["ActiveIndex"], 0, Math.Max(0, _itemsCount - 1)) : 0;

            int itemHeight = 28;
            int visible = Math.Max(0, Math.Min(items.Count, ctx.ContentRect.Height / itemHeight));
            for (int i = 0; i < visible; i++)
            {
                _itemRects.Add(new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + i * itemHeight, ctx.ContentRect.Width, itemHeight));
            }
            _lastCtx = ctx;
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bg = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bg, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _lastCtx = ctx;
            var items = ctx.CustomData.ContainsKey("Items") ? (List<string>)ctx.CustomData["Items"] : new List<string> { "Home", "Analytics", "Reports", "Settings" };
            int active = _activeIndex;
            using var font = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);

            for (int i = 0; i < _itemRects.Count && i < items.Count; i++)
            {
                var rect = _itemRects[i];
                bool isActive = i == active;
                bool hv = IsAreaHovered($"Menu_Item_{i}");

                if (isActive || hv)
                {
                    using var layer = new SolidBrush(Color.FromArgb(isActive ? 20 : 8, Theme?.PrimaryColor ?? Color.SteelBlue));
                    g.FillRoundedRectangle(layer, rect, 6);
                }

                using var brush = new SolidBrush(isActive ? (Theme?.ForeColor ?? Color.Black) : Color.FromArgb(160, Theme?.ForeColor ?? Color.Black));
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                var textRect = Rectangle.Inflate(rect, -10, 0);
                g.DrawString(items[i], font, brush, textRect, fmt);

                // Right chevron for active
                if (isActive)
                {
                    using var pen = new Pen(Theme?.PrimaryColor ?? Color.SteelBlue, 2f);
                    g.DrawLine(pen, rect.Right - 10, rect.Y + rect.Height / 2 - 4, rect.Right - 6, rect.Y + rect.Height / 2);
                    g.DrawLine(pen, rect.Right - 10, rect.Y + rect.Height / 2 + 4, rect.Right - 6, rect.Y + rect.Height / 2);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            for (int i = 0; i < _itemRects.Count; i++)
            {
                if (IsAreaHovered($"Menu_Item_{i}"))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.SteelBlue, 1f);
                    g.DrawRoundedRectangle(pen, _itemRects[i], 6);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = i; var rect = _itemRects[i];
                owner.AddHitArea($"Menu_Item_{idx}", rect, null, () =>
                {
                    ctx.CustomData["ActiveIndex"] = idx;
                    _activeIndex = idx;
                    notifyAreaHit?.Invoke($"Menu_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            if (!_keysHooked || Owner == null) return;
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
    }
}
