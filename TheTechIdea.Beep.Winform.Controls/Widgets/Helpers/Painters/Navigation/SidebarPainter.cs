using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

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
        private WidgetContext _lastCtx;
        private int _flatItemIndex; // index among only items
        private readonly List<int> _itemAreaIndices = new();
        private int _sectionJump; // number of rows in one section jump (estimate)

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
            int pad = 8;
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

            int y = ctx.ContentRect.Y + 36;
            int maxItemsInSection = 0;
            foreach (var g in groups)
            {
                string header = g.Item1;
                var items = g.Item2;
                maxItemsInSection = Math.Max(maxItemsInSection, items?.Count ?? 0);
                var headerRect = new Rectangle(ctx.ContentRect.X + 8, y, ctx.ContentRect.Width - 16, 20);
                _areas.Add((headerRect, $"Sidebar_Header_{header}"));
                y += 24;
                foreach (var item in items)
                {
                    var itemRect = new Rectangle(ctx.ContentRect.X + 8, y, ctx.ContentRect.Width - 16, 24);
                    _areas.Add((itemRect, $"Sidebar_Item_{item}"));
                    _itemAreaIndices.Add(_areas.Count - 1);
                    y += 26;
                }
                y += 8;
            }

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
            using var bg = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bg, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            using var headFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Bold);
            using var itemFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);

            // Toggle button
            bool hvT = IsAreaHovered("Sidebar_Toggle");
            using (var br = new SolidBrush(Color.FromArgb(hvT ? 24 : 12, Theme?.PrimaryColor ?? Color.SteelBlue)))
                g.FillEllipse(br, _toggleRect);
            using (var pen = new Pen(Theme?.PrimaryColor ?? Color.SteelBlue, 1.5f))
            {
                int cx = _toggleRect.X + _toggleRect.Width / 2;
                int cy = _toggleRect.Y + _toggleRect.Height / 2;
                if (_collapsed)
                {
                    g.DrawLine(pen, cx - 3, cy, cx + 3, cy);
                    g.DrawLine(pen, cx, cy - 3, cx, cy + 3);
                }
                else
                {
                    g.DrawLine(pen, cx - 3, cy, cx + 3, cy);
                }
            }

            // Current selection id
            string? activeId = ctx.SidebarSelectionId;

            // Sections and items
            for (int i = 0; i < _areas.Count; i++)
            {
                var (rect, id) = _areas[i];
                bool hv = IsAreaHovered(id);
                bool isHeader = id.StartsWith("Sidebar_Header_");
                bool isSelected = (!isHeader && activeId != null && id == activeId) || (!isHeader && _itemAreaIndices.IndexOf(i) == _flatItemIndex);

                if (isHeader)
                {
                    using var h = new SolidBrush(Color.FromArgb(hv ? 12 : 8, Theme?.PrimaryColor ?? Color.SteelBlue));
                    g.FillRectangle(h, rect);
                    string text = id.Substring("Sidebar_Header_".Length);
                    using var brush = new SolidBrush(Color.FromArgb(170, Theme?.ForeColor ?? Color.Black));
                    g.DrawString(text, headFont, brush, rect.X, rect.Y + 2);
                }
                else
                {
                    using var h = new SolidBrush(Color.FromArgb(isSelected ? 18 : (hv ? 12 : 6), Theme?.PrimaryColor ?? Color.SteelBlue));
                    g.FillRoundedRectangle(h, rect, 4);
                    string text = id.Substring("Sidebar_Item_".Length);
                    using var brush = new SolidBrush(Color.FromArgb(isSelected ? 220 : 180, Theme?.ForeColor ?? Color.Black));
                    var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(text, itemFont, brush, Rectangle.Inflate(rect, -6, 0), fmt);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            foreach (var (rect, id) in _areas)
            {
                if (IsAreaHovered(id))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.SteelBlue, 1f);
                    if (id.StartsWith("Sidebar_Header_")) g.DrawRectangle(pen, rect);
                    else g.DrawRoundedRectangle(pen, rect, 4);
                }
            }
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
            foreach (var (rect, id) in _areas)
            {
                string capture = id;
                owner.AddHitArea(id, rect, null, () =>
                {
                    if (!capture.StartsWith("Sidebar_Header_"))
                    {
                        ctx.SidebarSelectionId = capture;
                    }
                    notifyAreaHit?.Invoke(capture, rect);
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
