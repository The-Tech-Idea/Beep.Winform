using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    internal sealed class FormMdiHelper : IDisposable
    {
        // --------- Events (useful hooks from BeepiForm) ----------
        public event EventHandler NewTabClicked; // “+” tab
        public event EventHandler<TabReorderedEventArgs> TabReordered;
        public event EventHandler<FormEventArgs> TabPinned;
        public event EventHandler<FormEventArgs> TabUnpinned;

        // --------- Public options  ----------
        public TabAlignment Alignment { get; set; } = TabAlignment.Top;
        public bool ShowCloseButtons { get; set; } = true;
        public bool MiddleClickToClose { get; set; } = true;
        public bool AllowDragReorder { get; set; } = true;
        public bool AllowPinning { get; set; } = true;
        public bool ShowBadges { get; set; } = true;
        public bool ShowNewTabButton { get; set; } = true;

        private int _tabHeight = 32;
        [DefaultValue(32)]
        public int TabHeight
        {
            get => _tabHeight;
            set { _tabHeight = Math.Max(24, value); _host.Invalidate(); }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                if (_enabled)
                {
                    EnsureMdiContainer();
                    HookEvents();
                    _registerPaddingProvider?.Invoke(AdjustPadding);
                    _overlayRegistry.Add(PaintOverlay);
                }
                else
                {
                    UnhookEvents();
                }
                _host.Invalidate();
            }
        }

        // --------- Infra ----------
        private readonly IBeepModernFormHost _host;
        private readonly FormOverlayPainterRegistry _overlayRegistry;
        private readonly Action<FormCaptionBarHelper.PaddingAdjuster> _registerPaddingProvider;

        private bool _enabled = false;
        private bool _disposed = false;

        // layout + hit-test caches
        private readonly Dictionary<Form, Rectangle> _tabRects = new();
        private readonly Dictionary<Form, Rectangle> _closeRects = new();
        private readonly Dictionary<Form, int> _lastMeasuredWidths = new();
        private Rectangle _plusRect = Rectangle.Empty;
        private int _tabPaddingX = 14;
        private int _tabGap = 2;
        private int _scrollOffset = 0;
        private int _totalTabsWidth = 0;

        // order, pins, badges
        private readonly List<Form> _tabOrder = new();            // master order (persists across rebuilds)
        private readonly HashSet<Form> _pinned = new();           // pinned tabs
        private readonly Dictionary<Form, BadgeInfo> _badges = new();

        // hover
        private Form _hoverTab = null;
        private bool _hoverOnClose = false;
        private bool _hoverOnPin = false;

        // drag-reorder
        private bool _dragging = false;
        private Form _draggedTab = null;
        private int _dragStartX = 0;
        private int _dragCurrentX = 0;
        private int _insertionIndex = -1;

        // context menu
        private ContextMenuStrip _menu;
        private Form _menuTarget;

        private sealed class BadgeInfo
        {
            public int Count;
            public Color Back;
            public Color Fore;
        }

        public FormMdiHelper(
            IBeepModernFormHost host,
            FormOverlayPainterRegistry overlayRegistry,
            Action<FormCaptionBarHelper.PaddingAdjuster> registerPaddingProvider)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _overlayRegistry = overlayRegistry ?? throw new ArgumentNullException(nameof(overlayRegistry));
            _registerPaddingProvider = registerPaddingProvider ?? throw new ArgumentNullException(nameof(registerPaddingProvider));
        }

        private void EnsureMdiContainer()
        {
            var form = _host.AsForm;
            if (!form.IsMdiContainer)
                form.IsMdiContainer = true;
        }

        private void HookEvents()
        {
            var form = _host.AsForm;
            form.MdiChildActivate += OnMdiChildActivate;
            form.ControlAdded += OnControlAddedOrRemoved;
            form.ControlRemoved += OnControlAddedOrRemoved;
        }

        private void UnhookEvents()
        {
            var form = _host.AsForm;
            form.MdiChildActivate -= OnMdiChildActivate;
            form.ControlAdded -= OnControlAddedOrRemoved;
            form.ControlRemoved -= OnControlAddedOrRemoved;
        }

        private void OnMdiChildActivate(object? sender, EventArgs e)
        {
            SyncTabOrder();
            _host.Invalidate();
        }

        private void OnControlAddedOrRemoved(object? sender, ControlEventArgs e)
        {
            SyncTabOrder();
            _host.Invalidate();
        }

        // Keep a persistent ordering list. Add new children to end, remove missing.
        private void SyncTabOrder()
        {
            var form = _host.AsForm;
            var current = (form.MdiChildren ?? Array.Empty<Form>()).ToList();
            var currentSet = new HashSet<Form>(current);

            // Remove vanished or disposed forms
            for (int i = _tabOrder.Count - 1; i >= 0; i--)
            {
                var f = _tabOrder[i];
                if (f == null || f.IsDisposed || !currentSet.Contains(f))
                    _tabOrder.RemoveAt(i);
            }

            // Append any new children at the end (preserves existing order)
            foreach (var c in current)
                if (!_tabOrder.Contains(c))
                    _tabOrder.Add(c);
        }


        private void AdjustPadding(ref Padding p)
        {
            if (!_enabled) return;
            if (Alignment == TabAlignment.Top) p.Top += _tabHeight;
            else p.Bottom += _tabHeight;
        }

        // -------------------- Mouse API (forward from BeepiForm) --------------------
        public void OnMouseMove(MouseEventArgs e)
        {
            if (!_enabled) return;

            var area = GetStripRect();
            if (!area.Contains(e.Location))
            {
                if (_hoverTab != null || _hoverOnClose || _hoverOnPin)
                {
                    _hoverTab = null; _hoverOnClose = false; _hoverOnPin = false;
                    _host.Invalidate();
                }
                if (_dragging)
                {
                    _dragCurrentX = e.X;
                    UpdateInsertionIndex();
                    _host.Invalidate();
                }
                return;
            }

            if (_dragging)
            {
                _dragCurrentX = e.X;
                UpdateInsertionIndex();
                _host.Invalidate();
                return;
            }

            // hover logic
            _hoverTab = null; _hoverOnClose = false; _hoverOnPin = false;
            foreach (var kv in _tabRects)
            {
                if (!kv.Value.Contains(e.Location)) continue;

                _hoverTab = kv.Key;

                if (ShowCloseButtons && _closeRects.TryGetValue(kv.Key, out var cr) && cr.Contains(e.Location))
                    _hoverOnClose = true;

                // Pin hotspot: small 12x12 square left of text area (we’ll reuse close rect idea mirrored)
                var pinRect = GetPinRect(kv.Value);
                if (AllowPinning && pinRect.Contains(e.Location))
                    _hoverOnPin = true;

                break;
            }
            _host.Invalidate();
        }

        public void OnMouseLeave()
        {
            if (!_enabled) return;
            if (_hoverTab != null || _hoverOnClose || _hoverOnPin)
            {
                _hoverTab = null; _hoverOnClose = false; _hoverOnPin = false;
                _host.Invalidate();
            }
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            if (!_enabled) return;

            var area = GetStripRect();
            if (!area.Contains(e.Location))
                return;

            if (e.Button == MouseButtons.Middle && MiddleClickToClose)
            {
                if (_hoverTab != null && !_hoverTab.IsDisposed)
                {
                    try { _hoverTab.Close(); } catch { }
                    return;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                if (_hoverTab != null)
                {
                    _menuTarget = _hoverTab;
                    ShowContextMenu(e.Location);
                }
                return;
            }

            if (e.Button != MouseButtons.Left) return;

            // “+” tab
            if (ShowNewTabButton && _plusRect.Contains(e.Location))
            {
                NewTabClicked?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Pin click toggles pin
            if (AllowPinning && _hoverTab != null && _hoverOnPin)
            {
                TogglePin(_hoverTab);
                _host.Invalidate();
                return;
            }

            // Start drag or activate
            foreach (var kv in _tabRects)
            {
                if (!kv.Value.Contains(e.Location)) continue;

                var form = kv.Key;
                if (ShowCloseButtons && _closeRects.TryGetValue(form, out var cr) && cr.Contains(e.Location))
                {
                    try { form.Close(); } catch { }
                    return;
                }

                _draggedTab = form;
                _dragStartX = e.X;
                _dragCurrentX = e.X;
                _dragging = AllowDragReorder;
                if (!AllowDragReorder)
                {
                    try { if (!form.IsDisposed) form.Activate(); } catch { }
                }
                return;
            }
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            if (!_enabled) return;

            if (_dragging)
            {
                _dragging = false;
                var oldIdx = GetVisualOrder().IndexOf(_draggedTab);
                var newIdx = _insertionIndex;
                if (newIdx >= 0 && oldIdx >= 0 && newIdx != oldIdx)
                {
                    ApplyReorder(oldIdx, newIdx);
                    TabReordered?.Invoke(this, new TabReorderedEventArgs(_draggedTab, oldIdx, newIdx));
                }
                _draggedTab = null;
                _insertionIndex = -1;
                _host.Invalidate();
            }
        }

        public void OnMouseWheel(MouseEventArgs e)
        {
            if (!_enabled) return;
            var area = GetStripRect();
            if (!area.Contains(_host.AsForm.PointToClient(Control.MousePosition))) return;

            int delta = Math.Sign(e.Delta) * 60;
            _scrollOffset -= delta;
            ClampScroll();
            _host.Invalidate();
        }

        // -------------------- Public helpers (badges API) --------------------
        public void SetBadge(Form form, int count, Color? back = null, Color? fore = null)
        {
            if (form == null || form.IsDisposed) return;
            if (count <= 0)
            {
                _badges.Remove(form);
                _host.Invalidate();
                return;
            }
            var theme = _host.CurrentTheme;
            var b = back ?? (theme?.BadgeBackColor != Color.Empty ? theme.BadgeBackColor : Color.Firebrick);
            var f = fore ?? (theme?.BadgeForeColor != Color.Empty ? theme.BadgeForeColor : Color.White);
            _badges[form] = new BadgeInfo { Count = count, Back = b, Fore = f };
            _host.Invalidate();
        }

        public void ClearBadge(Form form)
        {
            if (form == null) return;
            if (_badges.Remove(form)) _host.Invalidate();
        }

        // -------------------- Painting --------------------
        private void PaintOverlay(Graphics g)
        {
            if (!_enabled) return;

            var strip = GetStripRect();
            if (strip.Width <= 0 || strip.Height <= 0) return;

            var theme = _host.CurrentTheme;
            Color barBack = theme?.AppBarBackColor != Color.Empty ? theme.AppBarBackColor : SystemColors.ControlDark;
            Color border = theme?.BorderColor != Color.Empty ? theme.BorderColor : SystemColors.ControlDark;
            Color tabBack = theme?.PanelBackColor != Color.Empty ? theme.PanelBackColor : ControlPaint.Light(barBack, .05f);
            Color tabActive = theme?.ButtonBackColor != Color.Empty ? theme.ButtonBackColor : ControlPaint.Light(barBack, .2f);
            Color tabHover = theme?.ButtonHoverBackColor != Color.Empty ? theme.ButtonHoverBackColor : ControlPaint.Light(barBack, .15f);
            Color text = theme?.AppBarTitleForeColor != Color.Empty ? theme.AppBarTitleForeColor : SystemColors.ControlText;

            using (var barBrush = new SolidBrush(barBack))
            using (var borderPen = new Pen(border))
            {
                g.FillRectangle(barBrush, strip);
                g.DrawLine(borderPen, strip.Left, strip.Bottom - 1, strip.Right, strip.Bottom - 1);
            }

            // order
            var ordered = GetVisualOrder();
            var form = _host.AsForm;
            var active = form.ActiveMdiChild;

            // measure all widths
            _tabRects.Clear(); _closeRects.Clear(); _lastMeasuredWidths.Clear();
            _totalTabsWidth = 0;

            using var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, LineAlignment = StringAlignment.Center };

            // Pre-measure
            foreach (var f in ordered)
            {
                var title = string.IsNullOrEmpty(f.Text) ? f.Name : f.Text;
                var size = TextUtils.MeasureText(g,title, form.Font).ToSize();
                int extra = (_tabPaddingX * 2) + (ShowCloseButtons ? 18 : 0);
                if (AllowPinning) extra += 18;
                if (ShowBadges && _badges.ContainsKey(f)) extra += 24;
                int w = Math.Max(60, size.Width + extra);
                _lastMeasuredWidths[f] = w;
                _totalTabsWidth += w + _tabGap;
            }

            // space for “+” tab
            int plusWidth = ShowNewTabButton ? Math.Max(28, _tabHeight - 6) : 0;
            _totalTabsWidth += ShowNewTabButton ? plusWidth + _tabGap : 0;

            ClampScroll();

            // Draw tabs: inactive first, active last (for border overlap clarity)
            int x = strip.Left - _scrollOffset;
            int y = strip.Top;

            IEnumerable<Form> drawOrder = ordered.Where(f => f != active);
            if (active != null) drawOrder = drawOrder.Concat(new[] { active });

            foreach (var f in drawOrder)
            {
                int w = _lastMeasuredWidths[f];
                var rect = new Rectangle(x, y + 1, w, strip.Height - 2);

                // During drag, let the dragged tab render at its current offset; others compress around insertion marker
                Rectangle paintRect = rect;
                if (_dragging && f == _draggedTab)
                {
                    paintRect.X += (_dragCurrentX - _dragStartX);
                }

                x += w + _tabGap;

                bool isActive = f == active;
                bool isHover = _hoverTab == f;

                Color fill = isActive ? tabActive : (isHover ? tabHover : tabBack);
                using (var b = new SolidBrush(fill))
                using (var p = new Pen(border))
                {
                    g.FillRectangle(b, paintRect);
                    g.DrawRectangle(p, paintRect.X, paintRect.Y, paintRect.Width - 1, paintRect.Height - 1);
                }

                // Icon (optional)
                int contentLeft = paintRect.Left + _tabPaddingX;
                if (f.Icon != null)
                {
                    var iconRect = new Rectangle(contentLeft, paintRect.Top + (paintRect.Height - 16) / 2, 16, 16);
                    try { g.DrawIcon(f.Icon, iconRect); } catch { }
                    contentLeft = iconRect.Right + 6;
                }

                // Pin glyph
                Rectangle pinRect = Rectangle.Empty;
                if (AllowPinning)
                {
                    pinRect = GetPinRect(paintRect);
                    DrawPinGlyph(g, pinRect, _pinned.Contains(f), isHover && _hoverOnPin && _hoverTab == f, text);
                }

                // Text
                var title = string.IsNullOrEmpty(f.Text) ? f.Name : f.Text;
                int rightLimit = paintRect.Right - 8;
                if (ShowCloseButtons) rightLimit -= 18;
                if (ShowBadges && _badges.ContainsKey(f)) rightLimit -= 22;
                if (AllowPinning) contentLeft = Math.Max(contentLeft, pinRect.Right + 6);

                var textRect = Rectangle.FromLTRB(contentLeft, paintRect.Top, rightLimit, paintRect.Bottom);
                using var tb = new SolidBrush(text);
                g.DrawString(title, form.Font, tb, textRect, sf);

                // Badge
                if (ShowBadges && _badges.TryGetValue(f, out var badge))
                {
                    var br = new Rectangle(rightLimit + 2, paintRect.Top + (paintRect.Height - 16) / 2, 20, 16);
                    DrawBadge(g, br, badge);
                }

                // Close button
                if (ShowCloseButtons)
                {
                    var closeRect = new Rectangle(paintRect.Right - 18, paintRect.Top + (paintRect.Height - 14) / 2, 14, 14);
                    var hoverClose = isHover && _hoverOnClose && _hoverTab == f;
                    using var pen = new Pen(hoverClose ? ControlPaint.Dark(text) : text, 1.5f);
                    if (hoverClose)
                    {
                        using var hb = new SolidBrush(Color.FromArgb(30, text));
                        g.FillEllipse(hb, closeRect);
                    }
                    DrawCloseGlyph(g, pen, closeRect);
                    _closeRects[f] = closeRect;
                }

                _tabRects[f] = rect; // store original rect for hit-testing (not paintRect)
            }

            // “+” new tab
            _plusRect = Rectangle.Empty;
            if (ShowNewTabButton)
            {
                var plus = new Rectangle(x, y + 1, plusWidth, strip.Height - 2);
                using var b = new SolidBrush(tabBack);
                using var p = new Pen(border);
                g.FillRectangle(b, plus);
                g.DrawRectangle(p, plus.X, plus.Y, plus.Width - 1, plus.Height - 1);
                DrawPlusGlyph(g, plus, text);
                _plusRect = plus;
            }

            // Insertion marker during drag
            if (_dragging && _insertionIndex >= 0)
            {
                int markerX = GetInsertionMarkerX(ordered, strip);
                using var lp = new Pen(border, 2f);
                g.DrawLine(lp, markerX, strip.Top + 4, markerX, strip.Bottom - 4);
            }
        }

        private Rectangle GetStripRect()
        {
            var client = _host.AsForm.ClientRectangle;
            return Alignment == TabAlignment.Top
                ? new Rectangle(0, 0, client.Width, _tabHeight)
                : new Rectangle(0, client.Height - _tabHeight, client.Width, _tabHeight);
        }

        private void ClampScroll()
        {
            int maxOverflow = Math.Max(0, _totalTabsWidth - GetStripRect().Width);
            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, maxOverflow));
        }

        // Visual order = pinned (by _tabOrder), then unpinned (by _tabOrder)
        private List<Form> GetVisualOrder()
        {
            var existing = _host.AsForm.MdiChildren?.ToHashSet() ?? new HashSet<Form>();
            var vis = _tabOrder.Where(f => existing.Contains(f)).ToList();
            var left = vis.Where(f => _pinned.Contains(f)).ToList();
            var right = vis.Where(f => !_pinned.Contains(f)).ToList();
            left.AddRange(right);
            return left;
        }

        private Rectangle GetPinRect(Rectangle tabRect)
        {
            // small square left padding
            return new Rectangle(tabRect.Left + 6, tabRect.Top + (tabRect.Height - 12) / 2, 12, 12);
        }

        private static void DrawCloseGlyph(Graphics g, Pen pen, Rectangle r)
        {
            int pad = 3;
            g.DrawLine(pen, r.Left + pad, r.Top + pad, r.Right - pad, r.Bottom - pad);
            g.DrawLine(pen, r.Left + pad, r.Bottom - pad, r.Right - pad, r.Top + pad);
        }

        private static void DrawPlusGlyph(Graphics g, Rectangle r, Color color)
        {
            using var pen = new Pen(color, 1.6f);
            var cx = r.Left + r.Width / 2;
            var cy = r.Top + r.Height / 2;
            g.DrawLine(pen, cx - 5, cy, cx + 5, cy);
            g.DrawLine(pen, cx, cy - 5, cx, cy + 5);
        }

        private static void DrawPinGlyph(Graphics g, Rectangle r, bool pinned, bool hover, Color stroke)
        {
            // simple pin: triangle head + stem. If pinned => rotate-ish by drawing “head filled”.
            using var pen = new Pen(hover ? ControlPaint.Dark(stroke) : stroke, 1.3f);
            if (!pinned)
            {
                g.DrawLine(pen, r.Left + 6, r.Top + 2, r.Left + 6, r.Bottom - 3);           // stem
                g.DrawLine(pen, r.Left + 2, r.Top + 4, r.Right - 2, r.Top + 4);             // head bar
                g.DrawLine(pen, r.Left + 6, r.Bottom - 3, r.Left + 4, r.Bottom);            // foot
            }
            else
            {
                // “filled” head look
                Point p1 = new Point(r.Left + 6, r.Top + 2);
                Point p2 = new Point(r.Left + 2, r.Top + 6);
                Point p3 = new Point(r.Right - 2, r.Top + 6);
                using var br = new SolidBrush(Color.FromArgb(140, stroke));
                g.FillPolygon(br, new[] { p1, p2, p3 });
                g.DrawLine(pen, r.Left + 6, r.Top + 6, r.Left + 6, r.Bottom - 3);
                g.DrawLine(pen, r.Left + 6, r.Bottom - 3, r.Left + 4, r.Bottom);
            }
        }

        private static void DrawBadge(Graphics g, Rectangle r, BadgeInfo b)
        {
            using var br = new SolidBrush(b.Back);
            using var pen = new Pen(ControlPaint.Dark(b.Back));
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var rr = RRect(r, 8);
            g.FillPath(br, rr);
            g.DrawPath(pen, rr);
            using var fbr = new SolidBrush(b.Fore);
            g.DrawString(b.Count.ToString(), SystemFonts.DefaultFont, fbr, r, sf);
            rr.Dispose();
        }

        private static System.Drawing.Drawing2D.GraphicsPath RRect(Rectangle r, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // -------------------- Drag-reorder helpers --------------------
        private void UpdateInsertionIndex()
        {
            if (!_dragging || _draggedTab == null) { _insertionIndex = -1; return; }

            var ordered = GetVisualOrder();
            if (!ordered.Contains(_draggedTab)) { _insertionIndex = -1; return; }

            // Build edge list based on measured widths
            var strip = GetStripRect();
            int x = strip.Left - _scrollOffset;
            var edges = new List<int>();
            foreach (var f in ordered)
            {
                int w = _lastMeasuredWidths.TryGetValue(f, out var ww) ? ww : 80;
                edges.Add(x);
                x += w + _tabGap;
            }
            // If “+” is visible, allow dropping before it (at end). So add a trailing edge.
            edges.Add(x);

            // Dragged center
            int draggedCenterX = _tabRects.TryGetValue(_draggedTab, out var rc)
                ? rc.Left + (rc.Width / 2) + (_dragCurrentX - _dragStartX)
                : _dragCurrentX;

            // Find nearest edge
            int idx = 0;
            int bestDist = int.MaxValue;
            for (int i = 0; i < edges.Count; i++)
            {
                int d = Math.Abs(draggedCenterX - edges[i]);
                if (d < bestDist) { bestDist = d; idx = i; }
            }

            // Constrain: pinned tabs can reorder only within pinned segment; unpinned among unpinned.
            int pinnedCount = ordered.Count(f => _pinned.Contains(f));
            int currentIndex = ordered.IndexOf(_draggedTab);
            bool isPinned = _pinned.Contains(_draggedTab);

            if (isPinned)
            {
                idx = Math.Max(0, Math.Min(idx, pinnedCount));
            }
            else
            {
                idx = Math.Max(pinnedCount, Math.Min(idx, ordered.Count));
            }

            _insertionIndex = idx;
        }

        private int GetInsertionMarkerX(List<Form> ordered, Rectangle strip)
        {
            int x = strip.Left - _scrollOffset;
            for (int i = 0; i < ordered.Count; i++)
            {
                if (i == _insertionIndex) return x;
                x += (_lastMeasuredWidths.TryGetValue(ordered[i], out var w) ? w : 80) + _tabGap;
            }
            return x;
        }

        private void ApplyReorder(int oldIndex, int newIndex)
        {
            var ordered = GetVisualOrder();
            if (oldIndex < 0 || oldIndex >= ordered.Count) return;
            if (newIndex < 0 || newIndex > ordered.Count) return;

            var moved = ordered[oldIndex];
            ordered.RemoveAt(oldIndex);
            if (newIndex > ordered.Count) newIndex = ordered.Count;
            ordered.Insert(newIndex, moved);

            // Rebuild master _tabOrder preserving pin set:
            var repinnedLeft = ordered.Where(f => _pinned.Contains(f)).ToList();
            var normalRight = ordered.Where(f => !_pinned.Contains(f)).ToList();
            _tabOrder.Clear();
            _tabOrder.AddRange(repinnedLeft);
            _tabOrder.AddRange(normalRight);
        }

        // -------------------- Context menu --------------------
        private void ShowContextMenu(Point clientPos)
        {
            if (_menu == null)
            {
                _menu = new ContextMenuStrip();
                var close = new ToolStripMenuItem("Close", null, (s, e) => { try { _menuTarget?.Close(); } catch { } });
                var closeOthers = new ToolStripMenuItem("Close Others", null, (s, e) => CloseOthers(_menuTarget));
                var closeAll = new ToolStripMenuItem("Close All", null, (s, e) => CloseAll());
                var pin = new ToolStripMenuItem("Pin", null, (s, e) => { if (_menuTarget != null) TogglePin(_menuTarget); });
                _menu.Items.AddRange(new ToolStripItem[] { close, closeOthers, closeAll, new ToolStripSeparator(), pin });
                _menu.Opening += (s, e) =>
                {
                    if (_menuTarget == null) return;
                    var isPinned = _pinned.Contains(_menuTarget);
                    (_menu.Items[4] as ToolStripMenuItem).Text = isPinned ? "Unpin" : "Pin";
                };
            }
            _menu.Show(_host.AsForm, clientPos);
        }

        private void TogglePin(Form f)
        {
            if (!_pinned.Contains(f)) { _pinned.Add(f); TabPinned?.Invoke(this, new FormEventArgs(f)); }
            else { _pinned.Remove(f); TabUnpinned?.Invoke(this, new FormEventArgs(f)); }
        }

        public void CloseAll()
        {
            foreach (var f in _host.AsForm.MdiChildren.ToArray())
                try { f.Close(); } catch { }
        }

        public void CloseOthers(Form keep)
        {
            foreach (var f in _host.AsForm.MdiChildren)
                if (f != keep) { try { f.Close(); } catch { } }
        }

        // -------------------- Utils --------------------
        private Rectangle GetStripRectClamped() => GetStripRect();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { UnhookEvents(); } catch { }
            try { _menu?.Dispose(); } catch { }
        }
    }

    // -------- Event args types ----------
    internal sealed class FormEventArgs : EventArgs
    {
        public Form Form { get; }
        public FormEventArgs(Form f) => Form = f;
    }

    internal sealed class TabReorderedEventArgs : EventArgs
    {
        public Form Form { get; }
        public int OldIndex { get; }
        public int NewIndex { get; }
        public TabReorderedEventArgs(Form form, int oldIndex, int newIndex)
        {
            Form = form; OldIndex = oldIndex; NewIndex = newIndex;
        }
    }
}
