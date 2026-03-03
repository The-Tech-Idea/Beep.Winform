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
    /// Sidebar - Collapsible left sidebar with section headers and items (interactive)
    /// + Keyboard: Up/Down to move selection across items; Enter to activate
    /// + Home/End to jump to first/last item; PageUp/PageDown to jump by section size
    /// Stores selected id in ctx.CustomData["SidebarSelection"].
    /// </summary>
    internal sealed class SidebarPainter : WidgetPainterBase, IDisposable
    {
        private readonly List<(Rectangle rect, string id)> _areas = new();
        private Rectangle _toggleRect;
        private bool _collapsed;
        private bool _keysHooked;
        private bool _wheelHooked;
        private WidgetContext? _lastCtx;
        private int _flatItemIndex; // index among only items
        private readonly List<int> _itemAreaIndices = new();
        private int _sectionJump; // number of rows in one section jump (estimate)

        // Cached fonts
        private Font? _headFont;
        private Font? _itemFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            HookKeyboard();
            HookMouseWheel();
        }

        private void HookMouseWheel()
        {
            if (_wheelHooked || Owner == null) return;
            try { Owner.MouseWheel += OnMouseWheel; _wheelHooked = true; }
            catch { }
        }

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (_lastCtx == null) return;
            int lineH = Dp(26); // ~item row stride
            _lastCtx.ScrollOffsetY = Math.Max(0,
                Math.Min(_lastCtx.ScrollOffsetY - e.Delta / 120 * lineH * 3,
                         Math.Max(0, _lastCtx.TotalContentHeight - _lastCtx.ContentRect.Height)));
            Owner?.Invalidate();
        }

        protected override void RebuildFonts()
        {
            _headFont?.Dispose();
            _itemFont?.Dispose();
            var hStyle = Theme?.SideMenuSubTitleFont ?? Theme?.LabelSmall ?? new TypographyStyle { FontSize = 9f, FontWeight = FontWeight.Bold };
            var iStyle = Theme?.SideMenuTextFont    ?? Theme?.LabelSmall ?? new TypographyStyle { FontSize = 9f };
            _headFont = BeepThemesManager.ToFont(hStyle, applyDpiScaling: true);
            _itemFont = BeepThemesManager.ToFont(iStyle, applyDpiScaling: true);
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

        private void OnUp(object? s, EventArgs e)
        {
            if (_itemAreaIndices.Count == 0 || _lastCtx == null) return;
            _flatItemIndex = Math.Max(0, _flatItemIndex - 1);
            UpdateSelectionFromIndex();
        }
        private void OnDown(object? s, EventArgs e)
        {
            if (_itemAreaIndices.Count == 0 || _lastCtx == null) return;
            _flatItemIndex = Math.Min(_itemAreaIndices.Count - 1, _flatItemIndex + 1);
            UpdateSelectionFromIndex();
        }
        private void OnHome(object? s, EventArgs e)
        {
            if (_itemAreaIndices.Count == 0 || _lastCtx == null) return;
            _flatItemIndex = 0;
            UpdateSelectionFromIndex();
        }
        private void OnEnd(object? s, EventArgs e)
        {
            if (_itemAreaIndices.Count == 0 || _lastCtx == null) return;
            _flatItemIndex = _itemAreaIndices.Count - 1;
            UpdateSelectionFromIndex();
        }
        private void OnPageUp(object? s, EventArgs e)
        {
            if (_itemAreaIndices.Count == 0 || _lastCtx == null) return;
            _flatItemIndex = Math.Max(0, _flatItemIndex - Math.Max(1, _sectionJump));
            UpdateSelectionFromIndex();
        }
        private void OnPageDown(object? s, EventArgs e)
        {
            if (_itemAreaIndices.Count == 0 || _lastCtx == null) return;
            _flatItemIndex = Math.Min(_itemAreaIndices.Count - 1, _flatItemIndex + Math.Max(1, _sectionJump));
            UpdateSelectionFromIndex();
        }
        private void OnEnter(object? s, EventArgs e)
        {
            if (Owner == null || _itemAreaIndices.Count == 0) return;
            var areaIdx = _itemAreaIndices[_flatItemIndex];
            string id = _areas[areaIdx].id;
            try
            {
                var hit = Owner._hitTest?.HitList?.FirstOrDefault(h => h.Name == id);
                hit?.HitAction?.Invoke();
            }
            catch { }
        }

        private void UpdateSelectionFromIndex()
        {
            var idx = _itemAreaIndices[_flatItemIndex];
            _lastCtx.SidebarSelectionId = _areas[idx].id;
            Owner?.Invalidate();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            _collapsed = ctx.IsCollapsed;
            int width = _collapsed ? 56 : Math.Max(160, ctx.DrawingRect.Width);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X, ctx.DrawingRect.Y, width, ctx.DrawingRect.Height);

            _areas.Clear();
            _itemAreaIndices.Clear();
            _toggleRect = new Rectangle(ctx.ContentRect.Right - 20, ctx.ContentRect.Y + 8, 16, 16);

            // Expect Groups as List<(string header, List<string> items)>
            var groups = ctx.NavigationGroups ?? new List<(string, List<string>)> {
                    ("Main", new List<string>{"Dashboard","Analytics","Reports"}),
                    ("Management", new List<string>{"Users","Settings"})
                };

            int headerH  = Dp(20);
            int headerGap = Dp(24);
            int itemH    = Dp(24);
            int itemGap  = Dp(26);
            int groupGap = Dp(8);
            int pad      = Dp(8);
            int y = ctx.ContentRect.Y + Dp(36);
            int maxItemsInSection = 0;
            foreach (var grp in groups)
            {
                string header = grp.Item1;
                var items = grp.Item2;
                maxItemsInSection = Math.Max(maxItemsInSection, items?.Count ?? 0);
                var headerRect = new Rectangle(ctx.ContentRect.X + pad, y, ctx.ContentRect.Width - pad * 2, headerH);
                _areas.Add((headerRect, $"Sidebar_Header_{header}"));
                y += headerGap;
                foreach (var item in items)
                {
                    var itemRect = new Rectangle(ctx.ContentRect.X + pad, y, ctx.ContentRect.Width - pad * 2, itemH);
                    _areas.Add((itemRect, $"Sidebar_Item_{item}"));
                    _itemAreaIndices.Add(_areas.Count - 1);
                    y += itemGap;
                }
                y += groupGap;
            }

            // Total scrollable content height
            ctx.TotalContentHeight = Math.Max(0, y - ctx.ContentRect.Y);
            ctx.TotalContentWidth  = 0;
            ClampScrollOffset(ctx);

            // estimate a section jump as the largest group size, at least 3
            _sectionJump = Math.Max(3, maxItemsInSection);

            // Initialize selection index from current selection id
            _flatItemIndex = 0;
            string? selId = ctx.SidebarSelectionId;
            if (!string.IsNullOrEmpty(selId))
            {
                int foundIdx = -1;
                for (int i = 0; i < _itemAreaIndices.Count; i++)
                {
                    if (_areas[_itemAreaIndices[i]].id == selId) { foundIdx = i; break; }
                }
                if (foundIdx >= 0) _flatItemIndex = foundIdx;
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
            var primary = Theme?.PrimaryColor ?? Color.SteelBlue;
            var fgColor = Theme?.ForeColor    ?? Color.Black;

            // ── Toggle button (fixed, not scrolled) ──
            bool hvT = IsAreaHovered("Sidebar_Toggle");
            using (var br = new SolidBrush(Color.FromArgb(hvT ? 24 : 12, primary)))
                g.FillEllipse(br, _toggleRect);
            using (var pen = new Pen(primary, 1.5f))
            {
                int cx = _toggleRect.X + _toggleRect.Width / 2;
                int cy = _toggleRect.Y + _toggleRect.Height / 2;
                int d  = Dp(3);
                if (_collapsed) { g.DrawLine(pen, cx - d, cy, cx + d, cy); g.DrawLine(pen, cx, cy - d, cx, cy + d); }
                else            { g.DrawLine(pen, cx - d, cy, cx + d, cy); }
            }

            // ── Scrollable sections/items ──
            string? activeId = ctx.SidebarSelectionId;
            var savedClip    = g.Clip;
            g.SetClip(ctx.ContentRect);

            using var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

            for (int i = 0; i < _areas.Count; i++)
            {
                var (baseRect, id) = _areas[i];
                // Apply scroll offset
                var rect = new Rectangle(baseRect.X, baseRect.Y - ctx.ScrollOffsetY, baseRect.Width, baseRect.Height);

                if (rect.Bottom < ctx.ContentRect.Y) continue;
                if (rect.Y      > ctx.ContentRect.Bottom) break;

                bool hv      = IsAreaHovered(id);
                bool isHeader = id.StartsWith("Sidebar_Header_");
                bool isSelected = (!isHeader && activeId != null && id == activeId)
                               || (!isHeader && _itemAreaIndices.IndexOf(i) == _flatItemIndex);

                if (isHeader)
                {
                    if (hv)
                    {
                        using var h = new SolidBrush(Color.FromArgb(12, primary));
                        g.FillRectangle(h, rect);
                    }
                    string htext = id.Substring("Sidebar_Header_".Length);
                    var hbrush   = PaintersFactory.GetSolidBrush(Color.FromArgb(170, fgColor));
                    if (_headFont != null)
                        g.DrawString(htext, _headFont, hbrush, rect.X, rect.Y + Dp(2));
                }
                else
                {
                    using var h = new SolidBrush(Color.FromArgb(isSelected ? 18 : (hv ? 12 : 6), primary));
                    g.FillRoundedRectangle(h, rect, Dp(4));
                    string itext  = id.Substring("Sidebar_Item_".Length);
                    var ibrush    = PaintersFactory.GetSolidBrush(Color.FromArgb(isSelected ? 220 : 180, fgColor));
                    if (_itemFont != null)
                        g.DrawString(itext, _itemFont, ibrush, Rectangle.Inflate(rect, -Dp(6), 0), fmt);
                }
            }

            g.Clip = savedClip;
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            bool scrollHov = IsAreaHovered("Sidebar_Scroll");
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, scrollHov);
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return; ClearOwnerHitAreas();
            owner.AddHitArea("Sidebar_Toggle", _toggleRect, null, () =>
            {
                bool collapsed = ctx.IsCollapsed;
                ctx.IsCollapsed = !collapsed;
                notifyAreaHit?.Invoke("Sidebar_Toggle", _toggleRect);
                Owner?.Invalidate();
            });
            foreach (var (baseRect, id) in _areas)
            {
                // Apply scroll offset to hit areas too
                var hitRect = new Rectangle(baseRect.X, baseRect.Y - ctx.ScrollOffsetY, baseRect.Width, baseRect.Height);
                if (hitRect.Bottom < ctx.ContentRect.Y || hitRect.Y > ctx.ContentRect.Bottom)
                    continue;
                string capture = id;
                owner.AddHitArea(id, hitRect, null, () =>
                {
                    if (!capture.StartsWith("Sidebar_Header_"))
                        ctx.SidebarSelectionId = capture;
                    notifyAreaHit?.Invoke(capture, hitRect);
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
                    Owner._input.UpArrowKeyPressed   -= OnUp;
                    Owner._input.DownArrowKeyPressed -= OnDown;
                    Owner._input.HomeKeyPressed      -= OnHome;
                    Owner._input.EndKeyPressed       -= OnEnd;
                    Owner._input.PageUpKeyPressed    -= OnPageUp;
                    Owner._input.PageDownKeyPressed  -= OnPageDown;
                    Owner._input.EnterKeyPressed     -= OnEnter;
                    _keysHooked = false;
                }
                catch { }
            }
            if (_wheelHooked && Owner != null)
            {
                try { Owner.MouseWheel -= OnMouseWheel; _wheelHooked = false; }
                catch { }
            }
            _headFont?.Dispose();
            _itemFont?.Dispose();
        }
    }
}
