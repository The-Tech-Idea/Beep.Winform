// BeepDocumentHost.AutoHide.cs
// 3.3 Auto-Hide Side Panels — tool-window-style collapsible strips.
//
// Usage:
//   host.AutoHideDocument("docId", AutoHideSide.Left);   // collapses the doc into the left strip
//   host.RestoreAutoHideDocument("docId");                // pops it back into the tab strip
//
// Architecture:
//   • One BeepAutoHideStrip (thin custom Panel) per side, created lazily.
//   • Panels are positioned by PositionAutoHideStrips(), called from RecalculateLayout().
//   • Clicking a strip button slides an overlay Panel in/out over the content area.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────────

        // documentId → side it is auto-hidden on
        private readonly Dictionary<string, AutoHideSide> _autoHideMap
            = new(StringComparer.Ordinal);

        // documentId → last flyout size (width for Left/Right, height for Top/Bottom)
        // Populated when the overlay is closed (G7 — remember last auto-hide size)
        private readonly Dictionary<string, int> _ahLastSize
            = new(StringComparer.Ordinal);

        // One strip per side (null until first document is hidden on that side)
        private BeepAutoHideStrip? _ahLeft, _ahRight, _ahTop, _ahBottom;

        // Currently-visible slide-out overlay
        private Panel?  _ahOverlay;
        private string? _ahActiveDocId;
        private System.Windows.Forms.Timer? _ahSlideTimer;
        private System.Windows.Forms.Timer? _ahFocusTimer;   // delays focus-loss collapse

        // Thickness of each side strip in logical pixels
        private const int AhStripThickness = 24;

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Collapses the specified document into a thin icon strip on the given side.
        /// The document panel is preserved but hidden; it slides back out on hover/click.
        /// </summary>
        public void AutoHideDocument(string documentId, AutoHideSide side)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return;
            if (_autoHideMap.ContainsKey(documentId)) return;       // already auto-hidden
            if (!CanAutoHideNow()) return;                          // policy check
            PushUndoState();

            string title    = panel.DocumentTitle;
            string? icon    = panel.IconPath;
            var previousState = GetDocumentDockState(documentId);
            var ownerGroup = GetGroupForDocument(documentId);

            // Record the source group for re-dock hints on restore (P4-002)
            var ahDesc = _documents?.FirstOrDefault(d => d.Id == documentId);
            if (ahDesc != null && ownerGroup != null)
                ahDesc.PreviousGroupId = ownerGroup.GroupId;

            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            coordinator.Execute(DocumentHostOperationNames.AutoHideDocument, () =>
            {
                // Remove from the tab strip
                int idx = ownerGroup.TabStrip.Tabs.ToList().FindIndex(t => t.Id == documentId);
                if (idx >= 0) ownerGroup.TabStrip.RemoveTabAt(idx);

                if (_activeDocumentId == documentId)
                {
                    _activeDocumentId = null;
                    var next = _panels.Keys.FirstOrDefault(k => k != documentId && !_autoHideMap.ContainsKey(k));
                    if (next != null) SetActiveDocument(next);
                }

                // Detach from content area; keep panel alive but invisible
                ownerGroup.ContentArea.Controls.Remove(panel);
                panel.Parent = this;
                panel.Visible = false;

                // Register
                _autoHideMap[documentId] = side;

                // Add button to the appropriate strip
                EnsureStrip(side).AddItem(documentId, title, icon);
            });

            OnDocumentDockStateChanged(documentId, title, previousState, DocumentDockState.AutoHide, side);
        }

        /// <summary>
        /// Returns an auto-hidden document to the normal tab strip.
        /// </summary>
        public void RestoreAutoHideDocument(string documentId)
        {
            if (!_autoHideMap.TryGetValue(documentId, out var side)) return;
            if (!_panels.TryGetValue(documentId, out var panel)) return;
            PushUndoState();
            var previousState = GetDocumentDockState(documentId);
            var ownerGroup = GetGroupForDocument(documentId);

            // Close the overlay if it is showing this document
            if (_ahActiveDocId == documentId)
                CloseAhOverlay(animate: false);

            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            coordinator.Execute(DocumentHostOperationNames.RestoreAutoHideDocument, () =>
            {
                // Remove from side strip
                GetStrip(side)?.RemoveItem(documentId);
                _autoHideMap.Remove(documentId);

                // Re-parent to content area
                this.Controls.Remove(panel);
                panel.Parent = ownerGroup.ContentArea;
                panel.Visible = true;

                // Re-add to tab strip and activate
                ownerGroup.TabStrip.AddTab(documentId, panel.DocumentTitle, panel.IconPath, activate: false);
                SetActiveDocument(documentId);
            });

            OnDocumentDockStateChanged(documentId, panel.DocumentTitle, previousState, DocumentDockState.Docked, side);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Strip management
        // ─────────────────────────────────────────────────────────────────────

        private BeepAutoHideStrip EnsureStrip(AutoHideSide side)
        {
            ref BeepAutoHideStrip? slot = ref GetStripRef(side);
            if (slot == null)
            {
                slot = new BeepAutoHideStrip(side, _currentTheme)
                {
                    Name       = $"_ah{side}",
                    Visible    = false,
                    HoverDelay = _autoHideHoverDelay
                };
                slot.ItemClicked += OnAhStripItemClicked;
                Controls.Add(slot);
                slot.BringToFront();
            }
            return slot;
        }

        /// <summary>
        /// Updates the hover-trigger delay on all existing auto-hide strips.
        /// Called automatically when <see cref="AutoHideHoverDelay"/> is changed at runtime.
        /// </summary>
        private void UpdateStripHoverDelays()
        {
            foreach (var strip in new[] { _ahLeft, _ahRight, _ahTop, _ahBottom })
            {
                if (strip != null)
                    strip.HoverDelay = _autoHideHoverDelay;
            }
        }

        private BeepAutoHideStrip? GetStrip(AutoHideSide side) => side switch
        {
            AutoHideSide.Left   => _ahLeft,
            AutoHideSide.Right  => _ahRight,
            AutoHideSide.Top    => _ahTop,
            _                   => _ahBottom
        };

        private ref BeepAutoHideStrip? GetStripRef(AutoHideSide side)
        {
            switch (side)
            {
                case AutoHideSide.Left:   return ref _ahLeft;
                case AutoHideSide.Right:  return ref _ahRight;
                case AutoHideSide.Top:    return ref _ahTop;
                default:                  return ref _ahBottom;
            }
        }

        // Called from RecalculateLayout() in Layout.cs
        private void PositionAutoHideStrips()
        {
            if (_contentArea == null) return;

            int t  = AhS(AhStripThickness);
            var ca = _contentArea.Bounds;

            void Position(BeepAutoHideStrip? strip, AutoHideSide side)
            {
                if (strip == null) return;
                if (strip.ItemCount == 0) { strip.Visible = false; return; }

                strip.Visible = true;
                switch (side)
                {
                    case AutoHideSide.Left:   strip.SetBounds(ca.Left, ca.Top, t, ca.Height); break;
                    case AutoHideSide.Right:  strip.SetBounds(ca.Right - t, ca.Top, t, ca.Height); break;
                    case AutoHideSide.Top:    strip.SetBounds(ca.Left, ca.Top, ca.Width, t); break;
                    case AutoHideSide.Bottom: strip.SetBounds(ca.Left, ca.Bottom - t, ca.Width, t); break;
                }
            }

            Position(_ahLeft,   AutoHideSide.Left);
            Position(_ahRight,  AutoHideSide.Right);
            Position(_ahTop,    AutoHideSide.Top);
            Position(_ahBottom, AutoHideSide.Bottom);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Overlay show / hide
        // ─────────────────────────────────────────────────────────────────────

        private void OnAhStripItemClicked(object? sender, AhItemClickArgs e)
        {
            if (_ahActiveDocId == e.DocumentId)
                CloseAhOverlay(animate: true);
            else
                ShowAhOverlay(e.DocumentId, e.Side);
        }

        private void ShowAhOverlay(string documentId, AutoHideSide side)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return;

            CloseAhOverlay(animate: false);
            _ahActiveDocId = documentId;

            if (_ahOverlay == null)
            {
                _ahOverlay = new Panel { BackColor = _currentTheme?.PanelBackColor ?? ColorUtils.MapSystemColor(SystemColors.Control) };
                Controls.Add(_ahOverlay);
            }

            _ahOverlay.BackColor = _currentTheme?.PanelBackColor ?? ColorUtils.MapSystemColor(SystemColors.Control);

            // Clear any leftover controls from the previous flyout (header buttons, etc.).
            // The document panel was already returned to this.Controls by FinaliseAhClose.
            _ahOverlay.Controls.Clear();

            // ── Flyout header: title + Pin + Close ───────────────────────────
            var header = BuildAhFlyoutHeader(documentId, panel.DocumentTitle);
            _ahOverlay.Controls.Add(header);

            // Remove panel from hidden parent and dock it inside the overlay
            this.Controls.Remove(panel);
            panel.Parent  = _ahOverlay;
            panel.Dock    = DockStyle.Fill;
            panel.Visible = true;

            // Size the overlay — use remembered size when available (G7), else default to 1/3
            var ca = _contentArea.Bounds;
            bool isVertical = side == AutoHideSide.Left || side == AutoHideSide.Right;
            int defaultW = Math.Min(Math.Max(AhS(220), ca.Width  / 3), ca.Width  - AhS(8));
            int defaultH = Math.Min(Math.Max(AhS(160), ca.Height / 3), ca.Height - AhS(8));
            int remembered = _ahLastSize.TryGetValue(documentId, out var ls) ? ls : -1;
            int ow = isVertical
                ? (remembered > 0 ? Math.Min(Math.Max(remembered, AhS(80)), ca.Width  - AhS(8)) : defaultW)
                : defaultW;
            int oh = !isVertical
                ? (remembered > 0 ? Math.Min(Math.Max(remembered, AhS(60)), ca.Height - AhS(8)) : defaultH)
                : defaultH;

            switch (side)
            {
                case AutoHideSide.Left:
                    _ahOverlay.SetBounds(ca.Left - ow, ca.Top, ow, ca.Height);
                    break;
                case AutoHideSide.Right:
                    _ahOverlay.SetBounds(ca.Right, ca.Top, ow, ca.Height);
                    break;
                case AutoHideSide.Top:
                    _ahOverlay.SetBounds(ca.Left, ca.Top - oh, ca.Width, oh);
                    break;
                default: // Bottom
                    _ahOverlay.SetBounds(ca.Left, ca.Bottom, ca.Width, oh);
                    break;
            }

            _ahOverlay.Visible = true;
            _ahOverlay.BringToFront();

            // ── Focus-loss collapse: collapse the flyout when focus leaves it ─
            AttachAhFocusLossCollapse();

            StartAhSlide(side, show: true);
        }

        /// <summary>
        /// Builds the thin header bar shown at the top of the auto-hide flyout.
        /// Contains the document title, a Pin button (restores to tab strip), and
        /// a Close button (collapses the flyout with animation).
        /// </summary>
        private Panel BuildAhFlyoutHeader(string documentId, string title)
        {
            int headerH = AhS(28);
            Color backColor = _currentTheme?.PanelBackColor ?? ColorUtils.MapSystemColor(SystemColors.Control);
            Color foreColor = _currentTheme?.PanelForeColor ?? ColorUtils.MapSystemColor(SystemColors.ControlText);
            Color sepColor  = _currentTheme?.BorderColor    ?? ColorUtils.MapSystemColor(SystemColors.ControlDark);

            var header = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = headerH,
                BackColor = backColor,
                Padding   = new Padding(4, 0, 0, 0),
            };

            // Close button (rightmost)
            var btnClose = new Button
            {
                Text      = "✕",
                Width     = headerH,
                Height    = headerH,
                Dock      = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = foreColor,
                Cursor    = Cursors.Hand,
                TabStop   = false,
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (_, _) => CloseAhOverlay(animate: true);

            // Pin button (to the left of Close — restores document to tab strip)
            var btnPin = new Button
            {
                Text      = "📌",
                Width     = headerH,
                Height    = headerH,
                Dock      = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = foreColor,
                Cursor    = Cursors.Hand,
                TabStop   = false,
            };
            btnPin.FlatAppearance.BorderSize = 0;
            var capturedId = documentId;
            btnPin.Click += (_, _) => RestoreAutoHideDocument(capturedId);

            // Title label (fills remaining space)
            var lblTitle = new Label
            {
                Text      = title,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = foreColor,
                BackColor = Color.Transparent,
            };

            // Separator line at bottom of header
            header.Paint += (_, pe) =>
            {
                using var pen = new Pen(sepColor, 1f);
                pe.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width - 1, header.Height - 1);
            };

            // Add in this order: Close (right), Pin (right), Title (fill)
            header.Controls.Add(lblTitle);
            header.Controls.Add(btnPin);
            header.Controls.Add(btnClose);

            return header;
        }

        private void CloseAhOverlay(bool animate)
        {
            if (_ahActiveDocId == null) return;

            if (!animate)
            {
                _ahSlideTimer?.Stop();
                FinaliseAhClose();
                return;
            }

            if (_autoHideMap.TryGetValue(_ahActiveDocId, out var side))
                StartAhSlide(side, show: false);
            else
                FinaliseAhClose();
        }

        private void FinaliseAhClose()
        {
            // Stop any running focus-loss timer
            _ahFocusTimer?.Stop();
            _ahFocusTimer?.Dispose();
            _ahFocusTimer = null;

            if (_ahOverlay != null) _ahOverlay.Visible = false;

            if (_ahActiveDocId != null && _panels.TryGetValue(_ahActiveDocId, out var p))
            {
                // Remember current overlay size before hiding (G7)
                if (_ahOverlay != null && _autoHideMap.TryGetValue(_ahActiveDocId, out var closingSide))
                {
                    bool isV = closingSide == AutoHideSide.Left || closingSide == AutoHideSide.Right;
                    _ahLastSize[_ahActiveDocId] = isV ? _ahOverlay.Width : _ahOverlay.Height;
                }

                p.Parent  = this;
                p.Visible = false;
            }
            _ahActiveDocId = null;
        }

        /// <summary>
        /// Subscribes to Leave events on the overlay and all its children so
        /// that the flyout collapses automatically when focus moves elsewhere.
        /// A 600 ms debounce timer prevents flicker during child control transitions.
        /// </summary>
        private void AttachAhFocusLossCollapse()
        {
            if (_ahOverlay == null) return;

            // Re-use a single timer; reset on every Leave event.
            _ahFocusTimer?.Stop();
            _ahFocusTimer?.Dispose();
            _ahFocusTimer = new System.Windows.Forms.Timer { Interval = 600 };
            _ahFocusTimer.Tick += (_, _) =>
            {
                _ahFocusTimer!.Stop();

                // Do nothing if focus is still inside the overlay or in the host itself.
                var focused = FindForm()?.ActiveControl;
                if (focused != null)
                {
                    Control? c = focused;
                    while (c != null)
                    {
                        if (c == _ahOverlay || c == this) return;
                        c = c.Parent;
                    }
                }

                if (_ahActiveDocId != null)
                    CloseAhOverlay(animate: true);
            };

            // Wire Leave on the overlay and every control it contains.
            void AttachLeave(Control ctrl)
            {
                ctrl.Leave += OnAhControlLeave;
                foreach (Control child in ctrl.Controls)
                    AttachLeave(child);
            }

            AttachLeave(_ahOverlay);
        }

        private void OnAhControlLeave(object? sender, EventArgs e)
        {
            // Start / restart the debounce timer so we only collapse after focus has
            // truly left the flyout (not just moved between child controls).
            if (_ahFocusTimer != null)
            {
                _ahFocusTimer.Stop();
                _ahFocusTimer.Start();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Slide animation
        // ─────────────────────────────────────────────────────────────────────

        private void StartAhSlide(AutoHideSide side, bool show)
        {
            _ahSlideTimer?.Stop();
            _ahSlideTimer?.Dispose();
            _ahSlideTimer = null;

            const int Steps = 8;
            int step = 0;
            var ov = _ahOverlay;
            if (ov == null) return;

            int startX = ov.Left, startY = ov.Top;
            var ca = _contentArea.Bounds;
            int ow = ov.Width, oh = ov.Height;

            int endX = side == AutoHideSide.Left  ? ca.Left
                     : side == AutoHideSide.Right ? ca.Right - ow
                     : startX;
            int endY = side == AutoHideSide.Top    ? ca.Top
                     : side == AutoHideSide.Bottom ? ca.Bottom - oh
                     : startY;

            if (!show)
            {
                (startX, endX) = (endX, startX);
                (startY, endY) = (endY, startY);
            }

            var timer = new System.Windows.Forms.Timer { Interval = 16 };
            _ahSlideTimer = timer;

            timer.Tick += (_, _) =>
            {
                step++;
                float t = Math.Min(1f, step / (float)Steps);
                // Quadratic ease-out: fast start, decelerates to rest — correct for panel slide
                float ease = 1f - (1f - t) * (1f - t);

                ov.Left = startX + (int)((endX - startX) * ease);
                ov.Top  = startY + (int)((endY - startY) * ease);

                if (step >= Steps)
                {
                    timer.Stop();
                    if (!show) FinaliseAhClose();
                }
            };
            timer.Start();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Dispose hook (called from BeepDocumentHost.cs Dispose)
        // ─────────────────────────────────────────────────────────────────────

        private void DisposeAutoHide()
        {
            _ahSlideTimer?.Stop();
            _ahSlideTimer?.Dispose();
            _ahOverlay?.Dispose();
            _ahLeft?.Dispose();
            _ahRight?.Dispose();
            _ahTop?.Dispose();
            _ahBottom?.Dispose();
            _ahLastSize.Clear();
        }

        // ─────────────────────────────────────────────────────────────────────
        // DPI helper (BeepDocumentHost has no global S(); define locally)
        // ─────────────────────────────────────────────────────────────────────

        private int AhS(int logical)
        {
            int dpi = IsHandleCreated ? DeviceDpi : 96;
            if (dpi <= 0) dpi = 96;
            return (int)(logical * (dpi / 96f));
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BeepAutoHideStrip — lightweight custom-drawn side strip
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Event raised when the user clicks an auto-hide tab button.</summary>
    internal sealed class AhItemClickArgs : EventArgs
    {
        public string       DocumentId { get; }
        public AutoHideSide Side       { get; }
        public AhItemClickArgs(string id, AutoHideSide side) { DocumentId = id; Side = side; }
    }

    /// <summary>
    /// Thin custom-drawn strip that shows collapsed document tabs on one side of the host.
    /// </summary>
    internal sealed class BeepAutoHideStrip : Control
    {
        private readonly AutoHideSide                 _side;
        private readonly List<(string Id, string Title, string? Icon)> _items = new();
        private IBeepTheme? _currentTheme;
        private int _hoverIndex = -1;

        // Hover-trigger delay
        private int    _hoverDelay;               // 0 = click-only
        private System.Windows.Forms.Timer? _hoverTimer;
        private int    _pendingHoverIndex = -1;   // index waiting for the timer

        internal event EventHandler<AhItemClickArgs>? ItemClicked;

        internal int ItemCount => _items.Count;

        /// <summary>
        /// Milliseconds to wait while hovering before the flyout is opened automatically.
        /// Set to 0 (default) for click-only behaviour.
        /// </summary>
        internal int HoverDelay
        {
            get => _hoverDelay;
            set
            {
                _hoverDelay = Math.Max(0, value);
                // If a timer exists, update its interval immediately
                if (_hoverTimer != null)
                    _hoverTimer.Interval = Math.Max(1, _hoverDelay);
            }
        }

        internal BeepAutoHideStrip(AutoHideSide side, IBeepTheme? theme)
        {
            _side  = side;
            _currentTheme = theme;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint, true);

            Cursor = Cursors.Hand;
        }

        internal void AddItem(string id, string title, string? iconPath)
        {
            _items.Add((id, title, iconPath));
            Invalidate();
        }

        internal void RemoveItem(string id)
        {
            _items.RemoveAll(i => i.Id == id);
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int hit = HitTest(e.Location);
            if (hit != _hoverIndex)
            {
                _hoverIndex = hit;
                Invalidate();

                // Hover-trigger: restart timer on every new hit
                StopHoverTimer();
                if (hit >= 0 && _hoverDelay > 0)
                {
                    _pendingHoverIndex = hit;
                    StartHoverTimer();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            StopHoverTimer();
            if (_hoverIndex >= 0) { _hoverIndex = -1; Invalidate(); }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            StopHoverTimer();
            int hit = HitTest(e.Location);
            if (hit >= 0 && hit < _items.Count)
                ItemClicked?.Invoke(this, new AhItemClickArgs(_items[hit].Id, _side));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hoverTimer?.Stop();
                _hoverTimer?.Dispose();
                _hoverTimer = null;
            }
            base.Dispose(disposing);
        }

        private void StartHoverTimer()
        {
            if (_hoverTimer == null)
            {
                _hoverTimer = new System.Windows.Forms.Timer
                {
                    Interval = Math.Max(1, _hoverDelay)
                };
                _hoverTimer.Tick += OnHoverTimerTick;
            }
            _hoverTimer.Interval = Math.Max(1, _hoverDelay);
            _hoverTimer.Start();
        }

        private void StopHoverTimer()
        {
            _hoverTimer?.Stop();
            _pendingHoverIndex = -1;
        }

        private void OnHoverTimerTick(object? sender, EventArgs e)
        {
            StopHoverTimer();
            if (_pendingHoverIndex < 0 || _pendingHoverIndex >= _items.Count) return;
            ItemClicked?.Invoke(this,
                new AhItemClickArgs(_items[_pendingHoverIndex].Id, _side));
        }

        private int HitTest(Point pt)
        {
            bool vertical = _side == AutoHideSide.Left || _side == AutoHideSide.Right;
            for (int i = 0; i < _items.Count; i++)
            {
                var r = GetItemRect(i);
                if (r.Contains(pt)) return i;
            }
            return -1;
        }

        private Rectangle GetItemRect(int index)
        {
            bool vertical = _side == AutoHideSide.Left || _side == AutoHideSide.Right;
            int step = vertical ? Height / Math.Max(1, _items.Count)
                                : Width  / Math.Max(1, _items.Count);
            int size = Math.Min(step, vertical ? Width : Height);

            return vertical
                ? new Rectangle(0, index * step, Width, size)
                : new Rectangle(index * step, 0, size, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            Color bg     = _currentTheme?.PanelBackColor    ?? ColorUtils.MapSystemColor(SystemColors.Control);
            Color border = _currentTheme?.BorderColor       ?? ColorUtils.MapSystemColor(SystemColors.ControlDark);
            Color fg     = _currentTheme?.ForeColor         ?? ColorUtils.MapSystemColor(SystemColors.ControlText);
            Color hover  = _currentTheme?.BackgroundColor   ?? ColorUtils.MapSystemColor(SystemColors.ControlLight);

            g.Clear(bg);

            // Draw border on the inner edge
            using (var pen = new Pen(border, 1f))
            {
                bool vertical = _side == AutoHideSide.Left || _side == AutoHideSide.Right;
                if (_side == AutoHideSide.Right)  g.DrawLine(pen, 0, 0, 0, Height);
                if (_side == AutoHideSide.Left)   g.DrawLine(pen, Width - 1, 0, Width - 1, Height);
                if (_side == AutoHideSide.Bottom) g.DrawLine(pen, 0, 0, Width, 0);
                if (_side == AutoHideSide.Top)    g.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }

            bool isVertical = _side == AutoHideSide.Left || _side == AutoHideSide.Right;

            for (int i = 0; i < _items.Count; i++)
            {
                var rect = GetItemRect(i);
                if (i == _hoverIndex)
                {
                    using var hb = new SolidBrush(hover);
                    g.FillRectangle(hb, rect);
                }

                var (id, title, _) = _items[i];

                if (isVertical)
                {
                    // Rotate text 90° for side strips
                    g.TranslateTransform(rect.Left, rect.Bottom);
                    g.RotateTransform(-90f);
                    using var sf = new StringFormat
                    {
                        Alignment     = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming      = StringTrimming.EllipsisCharacter,
                        FormatFlags   = StringFormatFlags.NoWrap
                    };
                    using var br = new SolidBrush(fg);
                    g.DrawString(title, Font, br,
                        new RectangleF(0, 0, rect.Height, rect.Width), sf);
                    g.ResetTransform();
                }
                else
                {
                    using var sf = new StringFormat
                    {
                        Alignment     = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming      = StringTrimming.EllipsisCharacter,
                        FormatFlags   = StringFormatFlags.NoWrap
                    };
                    using var br = new SolidBrush(fg);
                    g.DrawString(title, Font, br, rect, sf);
                }
            }
        }
    }
}
