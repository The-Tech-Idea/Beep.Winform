using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Tabs - Horizontal tab bar with modern web styling (interactive)
    /// + Keyboard: Left/Right arrows change active tab; Enter triggers current tab; Home/End jump first/last
    /// </summary>
    internal sealed class TabsPainter : WidgetPainterBase, IDisposable
    {
        private readonly List<Rectangle> _tabRects = new();
        private int _activeIndex;
        private BaseImage.ImagePainter _imagePainter = new BaseImage.ImagePainter();
        private bool _keysHooked;
        private WidgetContext _lastCtx; // capture latest ctx for keyboard handlers

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
                // Subscribe to arrow/enter/home/end via internal input helper
                Owner._input.LeftArrowKeyPressed += OnLeft;
                Owner._input.RightArrowKeyPressed += OnRight;
                Owner._input.EnterKeyPressed += OnEnter;
                Owner._input.HomeKeyPressed += OnHome;
                Owner._input.EndKeyPressed += OnEnd;
                _keysHooked = true;
            }
            catch { /* best-effort */ }
        }

        private void OnLeft(object? s, EventArgs e)
        {
            if (_tabRects.Count == 0 || _lastCtx == null) return;
            _activeIndex = Math.Max(0, _activeIndex - 1);
            _lastCtx.ActiveTabIndex = _activeIndex;
            Owner?.Invalidate();
        }

        private void OnRight(object? s, EventArgs e)
        {
            if (_tabRects.Count == 0 || _lastCtx == null) return;
            _activeIndex = Math.Min(_tabRects.Count - 1, _activeIndex + 1);
            _lastCtx.ActiveTabIndex = _activeIndex;
            Owner?.Invalidate();
        }

        private void OnHome(object? s, EventArgs e)
        {
            if (_tabRects.Count == 0 || _lastCtx == null) return;
            _activeIndex = 0;
            _lastCtx.ActiveTabIndex = _activeIndex;
            Owner?.Invalidate();
        }

        private void OnEnd(object? s, EventArgs e)
        {
            if (_tabRects.Count == 0 || _lastCtx == null) return;
            _activeIndex = _tabRects.Count - 1;
            _lastCtx.ActiveTabIndex = _activeIndex;
            Owner?.Invalidate();
        }

        private void OnEnter(object? s, EventArgs e)
        {
            if (Owner == null) return;
            // Trigger current tab hit action if present
            string id = $"Tabs_Tab_{_activeIndex}";
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
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 28);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom, ctx.DrawingRect.Width - pad * 2, Math.Max(0, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad));

            _activeIndex = Math.Max(0, ctx.ActiveTabIndex);

            // Build tab rects
            _tabRects.Clear();
            var labels = (ctx.Labels != null && ctx.Labels.Count > 0) ? ctx.Labels : new List<string> { "Tab 1", "Tab 2", "Tab 3" };
            int count = labels.Count;
            if (count > 0)
            {
                int tabWidth = Math.Max(60, ctx.HeaderRect.Width / count);
                int x = ctx.HeaderRect.X;
                for (int i = 0; i < count; i++)
                {
                    _tabRects.Add(new Rectangle(x, ctx.HeaderRect.Y, Math.Min(tabWidth, ctx.HeaderRect.Right - x), ctx.HeaderRect.Height));
                    x += tabWidth;
                }
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
            _imagePainter.CurrentTheme = Theme;
            _lastCtx = ctx;
            var labels = (ctx.Labels != null && ctx.Labels.Count > 0) ? ctx.Labels : new List<string> { "Tab 1", "Tab 2", "Tab 3" };
            using var font = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Bold);
            for (int i = 0; i < _tabRects.Count && i < labels.Count; i++)
            {
                var rect = _tabRects[i];
                bool active = i == _activeIndex;
                bool hv = IsAreaHovered($"Tabs_Tab_{i}");

                // Background layer
                if (active || hv)
                {
                    using var layer = new SolidBrush(Color.FromArgb(active ? 24 : 10, Theme?.PrimaryColor ?? Color.SteelBlue));
                    g.FillRoundedRectangle(layer, rect, 6);
                }

                // Text
                using var textBrush = new SolidBrush(active ? (Theme?.ForeColor ?? Color.Black) : Color.FromArgb(160, Theme?.ForeColor ?? Color.Black));
                var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(labels[i], font, textBrush, rect, fmt);

                // Active underline
                if (active)
                {
                    using var pen = new Pen(Theme?.PrimaryColor ?? Color.SteelBlue, 2f);
                    g.DrawLine(pen, rect.X + 8, rect.Bottom - 2, rect.Right - 8, rect.Bottom - 2);
                }
            }

            // Bottom divider
            using var div = new Pen(Color.FromArgb(40, Theme?.BorderColor ?? Color.Gray), 1);
            g.DrawLine(div, ctx.HeaderRect.Left, ctx.HeaderRect.Bottom, ctx.HeaderRect.Right, ctx.HeaderRect.Bottom);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Hover outline
            for (int i = 0; i < _tabRects.Count; i++)
            {
                if (IsAreaHovered($"Tabs_Tab_{i}"))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.SteelBlue, 1f);
                    g.DrawRoundedRectangle(pen, _tabRects[i], 6);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _tabRects.Count; i++)
            {
                int idx = i; var rect = _tabRects[i];
                owner.AddHitArea($"Tabs_Tab_{idx}", rect, null, () =>
                {
                    ctx.ActiveTabIndex = idx;
                    _activeIndex = idx;
                    notifyAreaHit?.Invoke($"Tabs_Tab_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            if (!_keysHooked || Owner == null) return;
            try
            {
                Owner._input.LeftArrowKeyPressed -= OnLeft;
                Owner._input.RightArrowKeyPressed -= OnRight;
                Owner._input.EnterKeyPressed -= OnEnter;
                Owner._input.HomeKeyPressed -= OnHome;
                Owner._input.EndKeyPressed -= OnEnd;
                _keysHooked = false;
            }
            catch { }
            _imagePainter?.Dispose();
        }
    }
}
