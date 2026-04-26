
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Marquees.Helpers;
using TheTechIdea.Beep.Winform.Controls.Marquees;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;
using TheTechIdea.Beep.Winform.Controls.Marquees.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// A rich, themed marquee control that supports continuous, ping-pong, and news-ticker scroll
    /// modes; multiple visual styles (Card, Pill, StockTicker, NewsBanner, Minimal, Default); 
    /// fade-edge effects; pause-on-hover; and click/hover events per item.
    /// </summary>
    [ToolboxItem(true)]
    [Category("UI")]
    [Description("A control that displays a continuous, styled marquee of items.")]
    [DisplayName("Beep Marquee")]
    public class BeepMarquee : BaseControl
    {
        // ── Legacy component dictionary (kept for backward-compat) ────────────────
        private Dictionary<string, IBeepUIComponent> _marqueeComponents
            = new Dictionary<string, IBeepUIComponent>();

        // ── Rich item list ────────────────────────────────────────────────────────
        private List<MarqueeItem> _items = new List<MarqueeItem>();

        // ── Timer ─────────────────────────────────────────────────────────────────
        private Timer _timer;

        // ── Scroll state ──────────────────────────────────────────────────────────
        private float _scrollOffset   = 0f;
        private float _scrollOffsetY  = 0f;
        private float _scrollSpeed    = 2f;
        private bool  _pingPongForward = true;
        private float _cachedTotalWidth = 0f;
        private float _cachedTotalHeight = 0f;
        private bool _sizeCacheDirty = true;
        private HashSet<int> _displayedItems = new HashSet<int>();

        // ── NewsTicker state ──────────────────────────────────────────────────────
        private float _newsAlpha        = 1f;
        private bool  _newsFadingOut    = false;
        private int   _activeNewsIndex  = 0;
        private int   _newsHoldTicks    = 0;
        private const int NewsHoldTarget = 100;  // ticks to hold each item

        // ── Hit testing ───────────────────────────────────────────────────────────
        private List<(MarqueeItem item, RectangleF rect)> _hitRects
            = new List<(MarqueeItem, RectangleF)>();
        private int _hoveredItemIndex = -1;

        // ── Painter ───────────────────────────────────────────────────────────────
        private IMarqueeItemRenderer _painter;

        // ── Backing fields for properties ─────────────────────────────────────────
        private MarqueeScrollMode      _scrollMode    = MarqueeScrollMode.Continuous;
        private MarqueeScrollDirection _scrollDir     = MarqueeScrollDirection.RightToLeft;
        private MarqueeStyle           _marqueeStyle  = MarqueeStyle.Default;
        private bool _paused        = false;
        private bool _pauseOnHover  = true;
        private bool _fadeEdges     = false;
        private int  _fadeWidth     = 40;
        private int  _componentSpacing = 20;
        private int  _itemHeight    = 32;

        // ═════════════════════════════════════════════════════════════════════════
        //  Events
        // ═════════════════════════════════════════════════════════════════════════
        [Category("Marquee")]
        [Description("Fired when the user clicks an item.")]
        public event EventHandler<MarqueeItemEventArgs> ItemClicked;

        [Category("Marquee")]
        [Description("Fired when the mouse hovers over an item.")]
        public event EventHandler<MarqueeItemEventArgs> ItemHovered;

        [Category("Marquee")]
        [Description("Fired when an item first becomes visible.")]
        public event EventHandler<MarqueeItemEventArgs> ItemDisplayed;

        // ═════════════════════════════════════════════════════════════════════════
        //  Properties
        // ═════════════════════════════════════════════════════════════════════════

        [Category("Marquee")]
        [Description("Collection of rich marquee items.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<MarqueeItem> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<MarqueeItem>();
                _sizeCacheDirty = true;
                _displayedItems.Clear();
                Invalidate();
            }
        }

        [Category("Marquee")]
        [Description("Scroll mode: Continuous, PingPong, or NewsTicker.")]
        [DefaultValue(MarqueeScrollMode.Continuous)]
        public MarqueeScrollMode ScrollMode
        {
            get => _scrollMode;
            set { _scrollMode = value; _scrollOffset = 0; _scrollOffsetY = 0; Invalidate(); }
        }

        [Category("Marquee")]
        [Description("Direction of scrolling.")]
        [DefaultValue(MarqueeScrollDirection.RightToLeft)]
        public MarqueeScrollDirection ScrollDirection
        {
            get => _scrollDir;
            set { _scrollDir = value; _scrollOffset = 0; _scrollOffsetY = 0; Invalidate(); }
        }

        /// <summary>Backward-compatible alias for ScrollDirection.</summary>
        [Browsable(false)]
        public bool ScrollLeft
        {
            get => _scrollDir == MarqueeScrollDirection.RightToLeft;
            set => ScrollDirection = value ? MarqueeScrollDirection.RightToLeft
                                           : MarqueeScrollDirection.LeftToRight;
        }

        [Category("Marquee")]
        [Description("Visual style of the marquee items.")]
        [DefaultValue(MarqueeStyle.Default)]
        public MarqueeStyle MarqueeStyle
        {
            get => _marqueeStyle;
            set
            {
                _marqueeStyle = value;
                _painter = MarqueePainterFactory.Create(value);
                Invalidate();
            }
        }

        [Category("Marquee")]
        [Description("Pixels to advance per timer tick.")]
        [DefaultValue(2f)]
        public float ScrollSpeed
        {
            get => _scrollSpeed;
            set => _scrollSpeed = Math.Max(0, value);
        }

        [Category("Marquee")]
        [Description("Timer interval in milliseconds (~30 = 33 FPS).")]
        [DefaultValue(30)]
        public int ScrollInterval
        {
            get => _timer?.Interval ?? 30;
            set { if (_timer != null) _timer.Interval = Math.Max(1, value); }
        }

        [Category("Marquee")]
        [Description("Spacing in pixels between items.")]
        [DefaultValue(20)]
        public int ComponentSpacing
        {
            get => _componentSpacing;
            set { _componentSpacing = Math.Max(0, value); Invalidate(); }
        }

        [Category("Marquee")]
        [Description("Height of each item in pixels (used by vertical scroll and painters).")]
        [DefaultValue(32)]
        public int ItemHeight
        {
            get => _itemHeight;
            set { _itemHeight = Math.Max(8, value); Invalidate(); }
        }

        [Category("Marquee")]
        [Description("Pause scrolling when the mouse hovers over the control.")]
        [DefaultValue(true)]
        public bool PauseOnHover
        {
            get => _pauseOnHover;
            set => _pauseOnHover = value;
        }

        [Category("Marquee")]
        [Description("Draw gradient fade strips on the leading/trailing edges.")]
        [DefaultValue(false)]
        public bool FadeEdges
        {
            get => _fadeEdges;
            set { _fadeEdges = value; Invalidate(); }
        }

        [Category("Marquee")]
        [Description("Width in pixels of the fade-edge gradient strips.")]
        [DefaultValue(40)]
        public int FadeWidth
        {
            get => _fadeWidth;
            set { _fadeWidth = Math.Max(4, value); Invalidate(); }
        }

        /// <summary>True if the marquee is currently paused.</summary>
        [Browsable(false)]
        public bool IsPaused => _paused;

        // ═════════════════════════════════════════════════════════════════════════
        //  Constructor
        // ═════════════════════════════════════════════════════════════════════════
        public BeepMarquee() : base()
        {
            _painter = MarqueePainterFactory.Create(MarqueeStyle.Default);

            _timer = new Timer();
            _timer.Interval = 30;
            _timer.Tick += (s, e) => OnTimerTick();

            if (!DesignMode)
                _timer.Start();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode) _timer.Start();
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  Public API
        // ═════════════════════════════════════════════════════════════════════════

        /// <summary>Pause the marquee animation.</summary>
        public void Pause()  { _paused = true;  _timer.Stop(); }

        /// <summary>Resume the marquee animation.</summary>
        public void Resume() { _paused = false; _timer.Start(); }

        /// <summary>Add a rich item to the Items list.</summary>
        public void AddItem(MarqueeItem item)
        {
            if (item == null) return;
            _items.Add(item);
            _sizeCacheDirty = true;
            Invalidate();
        }

        /// <summary>Add a StockItem.</summary>
        public void AddStockItem(string symbol, decimal price, decimal change)
        {
            var s = new StockItem { Symbol = symbol, Price = price, Change = change };
            s.Text = symbol;
            AddItem(s);
        }

        /// <summary>Add a NewsItem.</summary>
        public void AddNewsItem(string headline, string category = "")
        {
            var n = new NewsItem { Text = headline, Category = category };
            AddItem(n);
        }

        /// <summary>Remove an item by Id.</summary>
        public void RemoveItem(string id)
        {
            _items.RemoveAll(i => i.Id == id);
            _sizeCacheDirty = true;
            Invalidate();
        }

        /// <summary>Clear all rich items.</summary>
        public void ClearItems()
        {
            _items.Clear();
            _scrollOffset = 0;
            _scrollOffsetY = 0;
            _sizeCacheDirty = true;
            _displayedItems.Clear();
            Invalidate();
        }

        // ── Legacy API (backward compat) ─────────────────────────────────────────
        public void AddMarqueeComponent(string key, IBeepUIComponent component)
        {
            _marqueeComponents[key] = component;
            _sizeCacheDirty = true;
            Invalidate();
        }

        public void RemoveMarqueeComponent(string key)
        {
            _marqueeComponents.Remove(key);
            _sizeCacheDirty = true;
            Invalidate();
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  Mouse interaction
        // ═════════════════════════════════════════════════════════════════════════
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (_pauseOnHover && !_paused) { _paused = true; _timer.Stop(); }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoveredItemIndex = -1;
            if (_pauseOnHover && _paused) { _paused = false; _timer.Start(); }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int prev = _hoveredItemIndex;
            _hoveredItemIndex = HitTest(e.Location);

            if (_hoveredItemIndex != prev)
            {
                Invalidate();
                if (_hoveredItemIndex >= 0 && _hoveredItemIndex < _items.Count)
                {
                    var item = _items[_hoveredItemIndex];
                    var rect = _hitRects.Count > _hoveredItemIndex
                        ? _hitRects[_hoveredItemIndex].rect
                        : RectangleF.Empty;
                    ItemHovered?.Invoke(this, new MarqueeItemEventArgs(item, _hoveredItemIndex,
                        Point.Round(rect.Location)));
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int idx = HitTest(e.Location);
            if (idx >= 0 && idx < _items.Count)
            {
                var item = _items[idx];
                var rect = _hitRects.Count > idx ? _hitRects[idx].rect : RectangleF.Empty;
                ItemClicked?.Invoke(this, new MarqueeItemEventArgs(item, idx,
                    Point.Round(rect.Location)));
            }
        }

        private int HitTest(Point pt)
        {
            for (int i = 0; i < _hitRects.Count; i++)
                if (_hitRects[i].rect.Contains(pt)) return i;
            return -1;
        }

        private void FireItemDisplayed(int index)
        {
            if (index < 0 || index >= _items.Count) return;
            if (_displayedItems.Contains(index)) return;
            _displayedItems.Add(index);
            var item = _items[index];
            ItemDisplayed?.Invoke(this, new MarqueeItemEventArgs(item, index, Point.Empty));
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  Timer tick — scroll logic
        // ═════════════════════════════════════════════════════════════════════════
        private void OnTimerTick()
        {
            if (DesignMode || _paused) return;

            switch (_scrollMode)
            {
                case MarqueeScrollMode.Continuous:
                    TickContinuous();
                    break;
                case MarqueeScrollMode.PingPong:
                    TickPingPong();
                    break;
                case MarqueeScrollMode.NewsTicker:
                    TickNewsTicker();
                    break;
            }

            bool vertical = _scrollDir == MarqueeScrollDirection.TopToBottom
                         || _scrollDir == MarqueeScrollDirection.BottomToTop;
            int dirtyMargin = 10;
            if (vertical)
            {
                Invalidate(new Rectangle(0, 0, Width, Height));
            }
            else
            {
                Invalidate(new Rectangle(0, 0, Width, Height));
            }
        }

        private void TickContinuous()
        {
            bool vertical = _scrollDir == MarqueeScrollDirection.TopToBottom
                         || _scrollDir == MarqueeScrollDirection.BottomToTop;

            if (vertical)
            {
                float delta = _scrollDir == MarqueeScrollDirection.BottomToTop
                    ? -_scrollSpeed : _scrollSpeed;
                _scrollOffsetY += delta;
                float total = GetTotalHeight();
                if (_scrollOffsetY < -total) _scrollOffsetY = 0;
                else if (_scrollOffsetY > total) _scrollOffsetY = 0;
            }
            else
            {
                float delta = _scrollDir == MarqueeScrollDirection.RightToLeft
                    ? -_scrollSpeed : _scrollSpeed;
                _scrollOffset += delta;
                float total = GetTotalWidth();
                if (_scrollOffset < -total) _scrollOffset = 0;
                else if (_scrollOffset > total) _scrollOffset = 0;
            }
        }

        private void TickPingPong()
        {
            bool vertical = _scrollDir == MarqueeScrollDirection.TopToBottom
                         || _scrollDir == MarqueeScrollDirection.BottomToTop;

            if (vertical)
            {
                float total = GetTotalHeight();
                float maxOffset = Math.Max(0, total - Height);
                float delta = _pingPongForward ? -_scrollSpeed : _scrollSpeed;
                _scrollOffsetY += delta;

                if (_scrollOffsetY <= -maxOffset)
                {
                    _scrollOffsetY = -maxOffset;
                    _pingPongForward = false;
                }
                else if (_scrollOffsetY >= 0)
                {
                    _scrollOffsetY = 0;
                    _pingPongForward = true;
                }
            }
            else
            {
                float total = GetTotalWidth();
                float maxOffset = Math.Max(0, total - Width);
                float delta = _pingPongForward ? -_scrollSpeed : _scrollSpeed;
                _scrollOffset += delta;

                if (_scrollOffset <= -maxOffset)
                {
                    _scrollOffset = -maxOffset;
                    _pingPongForward = false;
                }
                else if (_scrollOffset >= 0)
                {
                    _scrollOffset = 0;
                    _pingPongForward = true;
                }
            }
        }

        private void TickNewsTicker()
        {
            if (_items.Count == 0) return;

            if (_newsFadingOut)
            {
                _newsAlpha = Math.Max(0f, _newsAlpha - 0.04f);
                if (_newsAlpha <= 0f)
                {
                    _activeNewsIndex = (_activeNewsIndex + 1) % _items.Count;
                    _newsFadingOut = false;
                    _newsHoldTicks = 0;
                    _newsAlpha = 0f;
                    FireItemDisplayed(_activeNewsIndex);
                }
            }
            else
            {
                _newsAlpha = Math.Min(1f, _newsAlpha + 0.04f);
                if (_newsAlpha >= 1f)
                {
                    _newsHoldTicks++;
                    if (_newsHoldTicks >= NewsHoldTarget)
                        _newsFadingOut = true;
                }
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  Paint
        // ═════════════════════════════════════════════════════════════════════════
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Build render context
            var ctx = BuildContext(g);

            _hitRects.Clear();

            if (_scrollMode == MarqueeScrollMode.NewsTicker)
                PaintNewsTicker(g, ctx);
            else if (_scrollDir == MarqueeScrollDirection.BottomToTop ||
                     _scrollDir == MarqueeScrollDirection.TopToBottom)
                PaintVertical(g, ctx);
            else
                PaintHorizontal(g, ctx);

            // Fade edges overlay
            if (_fadeEdges && _painter is MarqueePainterBase pb)
                pb.DrawFadeEdges(g, ClientRectangle, ctx);
        }

        // ── Horizontal paint ─────────────────────────────────────────────────────
        private void PaintHorizontal(Graphics g, MarqueeRenderContext ctx)
        {
            if (_items.Count == 0) { PaintLegacy(g); return; }

            float totalW = GetTotalWidth();
            float startX = _scrollOffset;

            // Draw enough bands to cover visible area (left + right of start)
            float drawX = startX;
            while (drawX < Width)        { DrawBandH(g, ctx, drawX); drawX += totalW; }
            drawX = startX - totalW;
            while (drawX + totalW > 0)   { DrawBandH(g, ctx, drawX); drawX -= totalW; }
        }

        private void DrawBandH(Graphics g, MarqueeRenderContext ctx, float startX)
        {
            float cx = startX;
            int centerY = Height / 2;
            int ih = ctx.ItemHeight;
            int topY = centerY - ih / 2;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (!item.IsVisible) { cx += _componentSpacing; continue; }

                ctx.HoveredItemIndex  = _hoveredItemIndex;
                ctx.CurrentItemIndex  = i;

                var sz = _painter.Measure(g, item, ctx);
                var dest = new RectangleF(cx, topY, sz.Width, ih);

                if (dest.Right > 0 && dest.Left < Width)
                {
                    _painter.Draw(g, item, dest, ctx);
                    _hitRects.Add((item, dest));
                }

                cx += sz.Width + _componentSpacing;
            }
        }

        // ── Vertical paint ───────────────────────────────────────────────────────
        private void PaintVertical(Graphics g, MarqueeRenderContext ctx)
        {
            if (_items.Count == 0) return;

            float totalH = GetTotalHeight();
            float startY = _scrollOffsetY;
            float drawY  = startY;

            while (drawY < Height)     { DrawBandV(g, ctx, drawY); drawY += totalH; }
            drawY = startY - totalH;
            while (drawY + totalH > 0) { DrawBandV(g, ctx, drawY); drawY -= totalH; }
        }

        private void DrawBandV(Graphics g, MarqueeRenderContext ctx, float startY)
        {
            float cy = startY;
            int ih = ctx.ItemHeight;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (!item.IsVisible) { cy += _componentSpacing; continue; }

                ctx.HoveredItemIndex = _hoveredItemIndex;
                ctx.CurrentItemIndex = i;

                var sz   = _painter.Measure(g, item, ctx);
                var dest = new RectangleF(0, cy, Width, ih);

                if (dest.Bottom > 0 && dest.Top < Height)
                {
                    _painter.Draw(g, item, dest, ctx);
                    _hitRects.Add((item, dest));
                }

                cy += ih + _componentSpacing;
            }
        }

        // ── NewsTicker paint ─────────────────────────────────────────────────────
        private void PaintNewsTicker(Graphics g, MarqueeRenderContext ctx)
        {
            if (_items.Count == 0) return;

            int idx  = _activeNewsIndex % _items.Count;
            var item = _items[idx];

            ctx.NewsAlpha        = _newsAlpha;
            ctx.HoveredItemIndex = _hoveredItemIndex;
            ctx.CurrentItemIndex = idx;

            var sz   = _painter.Measure(g, item, ctx);
            int ih   = ctx.ItemHeight;
            int topY = (Height - ih) / 2;
            var dest = new RectangleF((Width - sz.Width) / 2f, topY, sz.Width, ih);

            _painter.Draw(g, item, dest, ctx);
            _hitRects.Add((item, dest));
        }

        // ── Legacy (IBeepUIComponent) fallback ───────────────────────────────────
        private void PaintLegacy(Graphics g)
        {
            float totalW = GetTotalComponentsWidth();
            float startX = _scrollOffset;
            float drawX  = startX;
            while (drawX < Width)       { DrawComponents(g, drawX); drawX += totalW; }
            drawX = startX - totalW;
            while (drawX + totalW > 0)  { DrawComponents(g, drawX); drawX -= totalW; }
        }

        private void DrawComponents(Graphics g, float startX)
        {
            float cx = startX;
            int cy = Height / 2;
            foreach (var kvp in _marqueeComponents)
            {
                var comp = kvp.Value;
                if (comp == null) continue;
                int topY = cy - comp.Height / 2;
                var rect = new Rectangle((int)cx, topY, comp.Width, comp.Height);
                comp.Draw(g, rect);
                cx += comp.Width + _componentSpacing;
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  Helpers
        // ═════════════════════════════════════════════════════════════════════════
        private MarqueeRenderContext BuildContext(Graphics g)
        {
            return new MarqueeRenderContext
            {
                Theme           = _currentTheme,
                UseThemeColors  = UseThemeColors,
                DefaultForeColor = ForeColor,
                DefaultBackColor = BackColor,
                ItemFont        = Font ?? SystemFonts.DefaultFont,
                OwnerControl    = this,
                Direction       = _scrollDir,
                ItemHeight      = _itemHeight,
                Padding         = _componentSpacing / 2,
                NewsAlpha       = _newsAlpha,
                FadeEdges       = _fadeEdges,
                FadeWidth       = _fadeWidth,
            };
        }

        private float GetTotalWidth()
        {
            if (_sizeCacheDirty || _cachedTotalWidth == 0f)
            {
                if (_items.Count == 0)
                {
                    _cachedTotalWidth = GetTotalComponentsWidth();
                }
                else
                {
                    float total = 0;
                    using var g = CreateGraphics();
                    var ctx = BuildContext(g);
                    foreach (var item in _items)
                    {
                        if (!item.IsVisible) { total += _componentSpacing; continue; }
                        total += _painter.Measure(g, item, ctx).Width + _componentSpacing;
                    }
                    _cachedTotalWidth = total <= 0 ? 1 : total;
                }
                _sizeCacheDirty = false;
            }
            return _cachedTotalWidth;
        }

        private float GetTotalHeight()
        {
            if (_sizeCacheDirty || _cachedTotalHeight == 0f)
            {
                float total = _items.Count * (_itemHeight + _componentSpacing);
                _cachedTotalHeight = total <= 0 ? 1 : total;
                _sizeCacheDirty = false;
            }
            return _cachedTotalHeight;
        }

        private float GetTotalComponentsWidth()
        {
            float total = 0;
            foreach (var kvp in _marqueeComponents)
                if (kvp.Value != null)
                    total += kvp.Value.Width + _componentSpacing;
            return total <= 0 ? 1 : total;
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  Layout / Theme
        // ═════════════════════════════════════════════════════════════════════════
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme != null && UseThemeColors)
                BackColor = MarqueeThemeHelpers.GetMarqueeBackgroundColor(_currentTheme, UseThemeColors);
            Invalidate();
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  Dispose
        // ═════════════════════════════════════════════════════════════════════════
        protected override void Dispose(bool disposing)
        {
            if (disposing) _timer?.Dispose();
            base.Dispose(disposing);
        }
    }
}
