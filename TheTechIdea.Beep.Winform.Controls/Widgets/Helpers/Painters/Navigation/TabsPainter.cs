using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

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
        private bool _keysHooked;
        private bool _wheelHooked;
        private bool _tabsOverflow;
        private int _tabFirstVisible;
        private int _visibleTabCount;
        private int _tabWidth;
        private Font? _tabFont;
        private WidgetContext _lastCtx; // capture latest ctx for keyboard handlers

        protected override void RebuildFonts()
        {
            _tabFont?.Dispose();
            _tabFont = BeepThemesManager.ToFont(Theme?.LabelSmall ?? new TypographyStyle { FontSize = 9f, FontWeight = FontWeight.Bold }, true);
        }

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            HookKeyboard();
            if (!_wheelHooked && Owner != null)
            {
                Owner.MouseWheel += OnMouseWheel;
                _wheelHooked = true;
            }
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
            // Ensure active tab is visible
            if (_tabsOverflow && _activeIndex < _tabFirstVisible)
                _tabFirstVisible = _activeIndex;
            Owner?.Invalidate();
        }

        private void OnRight(object? s, EventArgs e)
        {
            if (_tabRects.Count == 0 || _lastCtx == null) return;
            var labels = (_lastCtx.Labels?.Count > 0) ? _lastCtx.Labels : new List<string> { "Tab 1", "Tab 2", "Tab 3" };
            _activeIndex = Math.Min(labels.Count - 1, _activeIndex + 1);
            _lastCtx.ActiveTabIndex = _activeIndex;
            // Ensure active tab is visible
            if (_tabsOverflow && _activeIndex >= _tabFirstVisible + _visibleTabCount)
                _tabFirstVisible = _activeIndex - _visibleTabCount + 1;
            Owner?.Invalidate();
        }

        private void OnHome(object? s, EventArgs e)
        {
            if (_tabRects.Count == 0 || _lastCtx == null) return;
            _activeIndex = 0; _tabFirstVisible = 0;
            _lastCtx.ActiveTabIndex = _activeIndex;
            Owner?.Invalidate();
        }

        private void OnEnd(object? s, EventArgs e)
        {
            if (_tabRects.Count == 0 || _lastCtx == null) return;
            var labels = (_lastCtx.Labels?.Count > 0) ? _lastCtx.Labels : new List<string> { "Tab 1", "Tab 2", "Tab 3" };
            _activeIndex = labels.Count - 1;
            if (_tabsOverflow) _tabFirstVisible = Math.Max(0, _activeIndex - _visibleTabCount + 1);
            _lastCtx.ActiveTabIndex = _activeIndex;
            Owner?.Invalidate();
        }

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (!_tabsOverflow || _lastCtx == null) return;
            var labels = (_lastCtx.Labels?.Count > 0) ? _lastCtx.Labels : new List<string> { "Tab 1", "Tab 2", "Tab 3" };
            int maxFirst = Math.Max(0, labels.Count - _visibleTabCount);
            if (e.Delta > 0) _tabFirstVisible = Math.Max(0, _tabFirstVisible - 1);
            else             _tabFirstVisible = Math.Min(maxFirst, _tabFirstVisible + 1);
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
            int pad = Dp(8);
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2, Dp(28));
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom,
                ctx.DrawingRect.Width - pad * 2,
                Math.Max(0, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad));

            _activeIndex = Math.Max(0, ctx.ActiveTabIndex);
            var labels = (ctx.Labels != null && ctx.Labels.Count > 0) ? ctx.Labels
                : new List<string> { "Tab 1", "Tab 2", "Tab 3" };
            int count = labels.Count;

            _tabRects.Clear();
            if (count > 0)
            {
                int minTabW = Dp(60);
                int arrowW   = Dp(24);
                int avail = ctx.HeaderRect.Width;
                int naturalW = avail / count;
                _tabWidth    = Math.Max(minTabW, naturalW);
                _tabsOverflow = count * _tabWidth > avail;

                if (_tabsOverflow)
                {
                    int usableW = avail - arrowW * 2;
                    _visibleTabCount = Math.Max(1, usableW / _tabWidth);
                    _tabWidth        = usableW / _visibleTabCount;
                }
                else
                {
                    _visibleTabCount = count;
                    _tabFirstVisible = 0;
                }

                // Clamp first-visible
                _tabFirstVisible = Math.Max(0, Math.Min(_tabFirstVisible, count - _visibleTabCount));

                int tabOffset = _tabsOverflow ? ctx.HeaderRect.X + arrowW : ctx.HeaderRect.X;
                for (int i = 0; i < _visibleTabCount && _tabFirstVisible + i < count; i++)
                {
                    _tabRects.Add(new Rectangle(
                        tabOffset + i * _tabWidth, ctx.HeaderRect.Y,
                        Math.Min(_tabWidth, ctx.HeaderRect.Right - (tabOffset + i * _tabWidth)),
                        ctx.HeaderRect.Height));
                }
            }
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
            var labels = (ctx.Labels != null && ctx.Labels.Count > 0) ? ctx.Labels : new List<string> { "Tab 1", "Tab 2", "Tab 3" };
            var font = _tabFont ?? SystemFonts.DefaultFont;

            // Clip to header (excludes arrow areas)
            var savedClip = g.Clip;
            if (_tabsOverflow) g.SetClip(ctx.HeaderRect);

            for (int i = 0; i < _tabRects.Count && _tabFirstVisible + i < labels.Count; i++)
            {
                var rect = _tabRects[i];
                int originalIdx = _tabFirstVisible + i;
                bool active = originalIdx == _activeIndex;
                bool hv = IsAreaHovered($"Tabs_Tab_{originalIdx}");

                // Background layer
                if (active || hv)
                {
                    var layer = PaintersFactory.GetSolidBrush(Color.FromArgb(active ? 24 : 10, Theme?.PrimaryColor ?? Color.SteelBlue));
                    g.FillRoundedRectangle(layer, rect, 6);
                }

                // Text
                var textBrush = PaintersFactory.GetSolidBrush(active
                    ? (Theme?.ForeColor ?? Color.Black)
                    : Color.FromArgb(160, Theme?.ForeColor ?? Color.Black));
                var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(labels[originalIdx], font, textBrush, rect, fmt);

                // Active underline
                if (active)
                {
                    var pen = PaintersFactory.GetPen(Theme?.PrimaryColor ?? Color.SteelBlue, 2f);
                    g.DrawLine(pen, rect.X + Dp(8), rect.Bottom - Dp(2), rect.Right - Dp(8), rect.Bottom - Dp(2));
                }
            }

            if (_tabsOverflow) g.Clip = savedClip;

            // Bottom divider
            var div = PaintersFactory.GetPen(Color.FromArgb(40, Theme?.BorderColor ?? Color.Gray), 1f);
            g.DrawLine(div, ctx.HeaderRect.Left, ctx.HeaderRect.Bottom, ctx.HeaderRect.Right, ctx.HeaderRect.Bottom);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Hover outline for tabs
            for (int i = 0; i < _tabRects.Count; i++)
            {
                int originalIdx = _tabFirstVisible + i;
                if (IsAreaHovered($"Tabs_Tab_{originalIdx}"))
                {
                    var pen = PaintersFactory.GetPen(Theme?.AccentColor ?? Color.SteelBlue, 1f);
                    g.DrawRoundedRectangle(pen, _tabRects[i], 6);
                }
            }

            // Overflow arrow buttons
            if (_tabsOverflow)
            {
                int arrowW = Dp(24);
                var leftRect  = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y, arrowW, ctx.HeaderRect.Height);
                var rightRect = new Rectangle(ctx.HeaderRect.Right - arrowW, ctx.HeaderRect.Y, arrowW, ctx.HeaderRect.Height);

                Color arrowBg = Theme?.CardBackColor ?? Color.FromArgb(235, 235, 235);
                bool canLeft  = _tabFirstVisible > 0;
                bool canRight = _lastCtx != null &&
                    _tabFirstVisible + _visibleTabCount < (_lastCtx.Labels?.Count ?? 3);

                // Left arrow
                var lBgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(canLeft ? 200 : 80, arrowBg));
                g.FillRectangle(lBgBrush, leftRect);
                if (canLeft)
                {
                    using var lPath = CreateRoundedPath(Rectangle.Inflate(leftRect, -Dp(6), -Dp(6)), 0);
                    StyledImagePainter.PaintWithTint(g, lPath, SvgsUI.ChevronLeft, Theme?.ForeColor ?? Color.Black, canLeft ? 0.9f : 0.3f);
                }

                // Right arrow
                var rBgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(canRight ? 200 : 80, arrowBg));
                g.FillRectangle(rBgBrush, rightRect);
                if (canRight)
                {
                    using var rPath = CreateRoundedPath(Rectangle.Inflate(rightRect, -Dp(6), -Dp(6)), 0);
                    StyledImagePainter.PaintWithTint(g, rPath, SvgsUI.ChevronRight, Theme?.ForeColor ?? Color.Black, canRight ? 0.9f : 0.3f);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _tabRects.Count; i++)
            {
                int idx = _tabFirstVisible + i;
                var rect = _tabRects[i];
                owner.AddHitArea($"Tabs_Tab_{idx}", rect, null, () =>
                {
                    ctx.ActiveTabIndex = idx;
                    _activeIndex = idx;
                    notifyAreaHit?.Invoke($"Tabs_Tab_{idx}", rect);
                    Owner?.Invalidate();
                });
            }

            if (_tabsOverflow && _lastCtx != null)
            {
                int arrowW  = Dp(24);
                var labels  = (_lastCtx.Labels?.Count > 0) ? _lastCtx.Labels : new List<string> { "Tab 1", "Tab 2", "Tab 3" };
                int maxFirst= Math.Max(0, labels.Count - _visibleTabCount);

                var leftRect  = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y, arrowW, ctx.HeaderRect.Height);
                var rightRect = new Rectangle(ctx.HeaderRect.Right - arrowW, ctx.HeaderRect.Y, arrowW, ctx.HeaderRect.Height);

                owner.AddHitArea("Tabs_ArrowLeft", leftRect, null, () =>
                {
                    _tabFirstVisible = Math.Max(0, _tabFirstVisible - 1);
                    notifyAreaHit?.Invoke("Tabs_ArrowLeft", leftRect);
                    Owner?.Invalidate();
                });
                owner.AddHitArea("Tabs_ArrowRight", rightRect, null, () =>
                {
                    _tabFirstVisible = Math.Min(maxFirst, _tabFirstVisible + 1);
                    notifyAreaHit?.Invoke("Tabs_ArrowRight", rightRect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            if (_wheelHooked && Owner != null)
            {
                Owner.MouseWheel -= OnMouseWheel;
                _wheelHooked = false;
            }
            if (!_keysHooked || Owner == null) { _tabFont?.Dispose(); return; }
            try
            {
                Owner._input.LeftArrowKeyPressed  -= OnLeft;
                Owner._input.RightArrowKeyPressed -= OnRight;
                Owner._input.EnterKeyPressed      -= OnEnter;
                Owner._input.HomeKeyPressed       -= OnHome;
                Owner._input.EndKeyPressed        -= OnEnd;
                _keysHooked = false;
            }
            catch { }
            _tabFont?.Dispose();
        }
    }
}
