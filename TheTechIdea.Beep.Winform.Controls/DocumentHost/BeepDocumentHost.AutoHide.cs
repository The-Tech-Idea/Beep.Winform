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

        // One strip per side (null until first document is hidden on that side)
        private BeepAutoHideStrip? _ahLeft, _ahRight, _ahTop, _ahBottom;

        // Currently-visible slide-out overlay
        private Panel?  _ahOverlay;
        private string? _ahActiveDocId;
        private System.Windows.Forms.Timer? _ahSlideTimer;

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

            string title    = panel.DocumentTitle;
            string? icon    = panel.IconPath;
            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            coordinator.Execute(DocumentHostOperationNames.AutoHideDocument, () =>
            {
                // Remove from the tab strip
                int idx = _tabStrip.Tabs.ToList().FindIndex(t => t.Id == documentId);
                if (idx >= 0) _tabStrip.RemoveTabAt(idx);

                if (_activeDocumentId == documentId)
                {
                    _activeDocumentId = null;
                    var next = _panels.Keys.FirstOrDefault(k => k != documentId && !_autoHideMap.ContainsKey(k));
                    if (next != null) SetActiveDocument(next);
                }

                // Detach from content area; keep panel alive but invisible
                _contentArea.Controls.Remove(panel);
                panel.Parent = this;
                panel.Visible = false;

                // Register
                _autoHideMap[documentId] = side;

                // Add button to the appropriate strip
                EnsureStrip(side).AddItem(documentId, title, icon);
            });
        }

        /// <summary>
        /// Returns an auto-hidden document to the normal tab strip.
        /// </summary>
        public void RestoreAutoHideDocument(string documentId)
        {
            if (!_autoHideMap.TryGetValue(documentId, out var side)) return;
            if (!_panels.TryGetValue(documentId, out var panel)) return;

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
                panel.Parent = _contentArea;
                panel.Visible = true;

                // Re-add to tab strip and activate
                _tabStrip.AddTab(documentId, panel.DocumentTitle, panel.IconPath, activate: true);
            });
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
                    Name    = $"_ah{side}",
                    Visible = false
                };
                slot.ItemClicked += OnAhStripItemClicked;
                Controls.Add(slot);
                slot.BringToFront();
            }
            return slot;
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

            // Remove panel from hidden parent and dock it inside the overlay
            this.Controls.Remove(panel);
            panel.Parent = _ahOverlay;
            panel.Dock   = DockStyle.Fill;
            panel.Visible = true;

            // Size the overlay (about 1/3 of client in the slide direction)
            var ca = _contentArea.Bounds;
            int ow = Math.Min(Math.Max(AhS(220), ca.Width  / 3), ca.Width  - AhS(8));
            int oh = Math.Min(Math.Max(AhS(160), ca.Height / 3), ca.Height - AhS(8));

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

            StartAhSlide(side, show: true);
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
            if (_ahOverlay != null) _ahOverlay.Visible = false;

            if (_ahActiveDocId != null && _panels.TryGetValue(_ahActiveDocId, out var p))
            {
                p.Parent  = this;
                p.Visible = false;
            }
            _ahActiveDocId = null;
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
                // Smooth ease-out
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

        internal event EventHandler<AhItemClickArgs>? ItemClicked;

        internal int ItemCount => _items.Count;

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
            if (hit != _hoverIndex) { _hoverIndex = hit; Invalidate(); }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverIndex >= 0) { _hoverIndex = -1; Invalidate(); }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int hit = HitTest(e.Location);
            if (hit >= 0 && hit < _items.Count)
                ItemClicked?.Invoke(this, new AhItemClickArgs(_items[hit].Id, _side));
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
