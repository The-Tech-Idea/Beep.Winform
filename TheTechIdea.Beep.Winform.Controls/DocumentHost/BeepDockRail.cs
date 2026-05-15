// BeepDockRail.cs
// Visual edge-rail control that hosts named tool-window panels on one side
// of a BeepDocumentHost.
//
// A BeepDockRail lives on exactly one edge (Left / Right / Top / Bottom).
// It can host multiple DockPanelDescriptors simultaneously:
//   • Pinned panels   — show a visible header + content area (stacked).
//   • Auto-hidden panels — collapsed to a thin strip; click the button to
//                          show a slide-out overlay (reuses overlay pattern).
//
// BeepDockManager creates one BeepDockRail per edge as needed and registers it
// with the host via BeepDocumentHost.RegisterDockRail(edge, rail).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Edge rail that hosts tool-window panels on one side of a
    /// <see cref="BeepDocumentHost"/>.  One instance per edge.
    /// </summary>
    internal sealed class BeepDockRail : Control
    {
        // ─────────────────────────────────────────────────────────────────────
        // Constants
        // ─────────────────────────────────────────────────────────────────────

        private const int StripThickness = 24;   // thin strip pixel width when all panels collapsed
        private const int HeaderHeight   = 28;   // height of per-panel header bar
        private const int SplitterSize   = 4;    // between stacked pinned panels

        // ─────────────────────────────────────────────────────────────────────
        // Fields
        // ─────────────────────────────────────────────────────────────────────

        private readonly DockEdge _edge;

        // Ordered list so stacking order is deterministic
        private readonly List<string> _pinnedOrder     = new();
        private readonly List<string> _autoHiddenOrder = new();

        // Per-descriptor UI wrappers (only for pinned panels)
        private readonly Dictionary<string, PinnedPanelSection> _sections =
            new(StringComparer.Ordinal);

        // Thin strip buttons for auto-hidden panels
        private readonly Dictionary<string, StripButton> _stripButtons =
            new(StringComparer.Ordinal);

        // The overlay Panel shown when a strip button is clicked
        private Panel?  _overlay;
        private string? _overlayPanelId;

        // Theme colours (set by the host)
        private Color _backColor    = SystemColors.Control;
        private Color _foreColor    = SystemColors.ControlText;
        private Color _borderColor  = SystemColors.ControlDark;
        private Color _headerBack   = SystemColors.ControlDark;

        // ─────────────────────────────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Raised when the user pins a panel (makes it permanent).</summary>
        public event EventHandler<DockRailPanelEventArgs>? PanelPinned;

        /// <summary>Raised when the user closes / hides a panel.</summary>
        public event EventHandler<DockRailPanelEventArgs>? PanelClosed;

        // ─────────────────────────────────────────────────────────────────────
        // Constructor
        // ─────────────────────────────────────────────────────────────────────

        public BeepDockRail(DockEdge edge)
        {
            _edge = edge;

            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint  |
                ControlStyles.ResizeRedraw,
                true);

            BackColor = _backColor;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Properties
        // ─────────────────────────────────────────────────────────────────────

        public DockEdge Edge => _edge;

        /// <summary>True if at least one panel is pinned (permanently visible).</summary>
        public bool HasPinnedPanels => _pinnedOrder.Count > 0;

        /// <summary>True if at least one panel is in the auto-hide strip.</summary>
        public bool HasAutoHiddenPanels => _autoHiddenOrder.Count > 0;

        /// <summary>
        /// Pixel extent of the pinned area on the edge axis.
        /// For Left/Right this is a width; for Top/Bottom a height.
        /// Returns <see cref="StripThickness"/> when only auto-hidden panels exist.
        /// Returns 0 when the rail is completely empty.
        /// </summary>
        public int RailPixelSize
        {
            get
            {
                if (!HasPinnedPanels && !HasAutoHiddenPanels) return 0;
                if (!HasPinnedPanels) return ScaledThickness;

                // Sum of all pinned panel extents + splitters
                int total = 0;
                foreach (var id in _pinnedOrder)
                    if (_sections.TryGetValue(id, out var s)) total += s.DesiredExtent;
                if (_pinnedOrder.Count > 1)
                    total += (_pinnedOrder.Count - 1) * SplitterSize;
                if (HasAutoHiddenPanels)
                    total += ScaledThickness;
                return Math.Max(total, ScaledThickness);
            }
        }

        private int ScaledThickness => (int)(StripThickness * (IsHandleCreated ? DeviceDpi / 96f : 1f));

        // ─────────────────────────────────────────────────────────────────────
        // Theme
        // ─────────────────────────────────────────────────────────────────────

        public void ApplyTheme(BeepTheme? theme)
        {
            if (theme is null) return;
            _backColor   = theme.PanelBackColor;
            _foreColor   = theme.ForeColor;
            _borderColor = theme.BorderColor;
            _headerBack  = theme.PrimaryColor;
            BackColor    = _backColor;
            foreach (var s in _sections.Values) s.ApplyTheme(theme);
            foreach (var b in _stripButtons.Values) b.ApplyTheme(theme);
            Invalidate();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the descriptor as a pinned panel section (permanently visible).
        /// If it was previously auto-hidden, it is promoted.
        /// </summary>
        public void ShowPinned(DockPanelDescriptor desc)
        {
            // Remove from auto-hide strip if present
            if (_autoHiddenOrder.Remove(desc.Id))
            {
                if (_stripButtons.TryGetValue(desc.Id, out var btn))
                {
                    Controls.Remove(btn);
                    btn.Dispose();
                    _stripButtons.Remove(desc.Id);
                }
                CloseOverlay();
            }

            if (!_pinnedOrder.Contains(desc.Id))
                _pinnedOrder.Add(desc.Id);

            if (!_sections.ContainsKey(desc.Id))
            {
                var section = new PinnedPanelSection(desc, _edge, _backColor, _foreColor, _headerBack, _borderColor);
                section.PinClicked   += (_, e) => PanelPinned?.Invoke(this, new DockRailPanelEventArgs(e.Id));
                section.CloseClicked += (_, e) =>
                {
                    RemovePanel(e.Id);
                    PanelClosed?.Invoke(this, new DockRailPanelEventArgs(e.Id));
                };
                _sections[desc.Id] = section;
                Controls.Add(section);
            }

            Visible = true;
            ArrangeContent();
        }

        /// <summary>
        /// Collapses the descriptor into the auto-hide strip button.
        /// If it was previously pinned, the section is removed.
        /// </summary>
        public void ShowAutoHidden(DockPanelDescriptor desc)
        {
            // Remove from pinned sections if present
            if (_pinnedOrder.Remove(desc.Id))
            {
                if (_sections.TryGetValue(desc.Id, out var sec))
                {
                    // Return content to the descriptor before disposing section
                    if (desc.Content is not null)
                        sec.DetachContent();
                    Controls.Remove(sec);
                    sec.Dispose();
                    _sections.Remove(desc.Id);
                }
            }

            if (!_autoHiddenOrder.Contains(desc.Id))
                _autoHiddenOrder.Add(desc.Id);

            if (!_stripButtons.ContainsKey(desc.Id))
            {
                var btn = new StripButton(desc, _edge, _backColor, _foreColor);
                btn.Clicked += (_, e) => ToggleOverlay(e.Id, desc);
                _stripButtons[desc.Id] = btn;
                Controls.Add(btn);
            }

            Visible = true;
            ArrangeContent();
        }

        /// <summary>Completely removes a panel from this rail.</summary>
        public void HidePanel(string id)
        {
            RemovePanel(id);
            ArrangeContent();
            if (!HasPinnedPanels && !HasAutoHiddenPanels)
                Visible = false;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Layout
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ArrangeContent();
        }

        private void ArrangeContent()
        {
            if (!HasPinnedPanels && !HasAutoHiddenPanels) return;

            bool vertical   = _edge == DockEdge.Left || _edge == DockEdge.Right;
            int  w          = Width;
            int  h          = Height;
            int  stripPx    = HasAutoHiddenPanels ? ScaledThickness : 0;
            int  spaceForPinned = vertical ? h - stripPx : w - stripPx;

            // --- Distribute space among pinned sections ---
            if (_pinnedOrder.Count > 0)
            {
                int splitterTotal = (_pinnedOrder.Count - 1) * SplitterSize;
                int availableForContent = spaceForPinned - splitterTotal;
                int eachExtent = _pinnedOrder.Count > 0
                    ? Math.Max(60, availableForContent / _pinnedOrder.Count)
                    : 0;

                int pos = 0;
                for (int i = 0; i < _pinnedOrder.Count; i++)
                {
                    var id = _pinnedOrder[i];
                    if (!_sections.TryGetValue(id, out var sec)) continue;

                    sec.DesiredExtent = eachExtent;
                    if (vertical)
                        sec.SetBounds(0, pos, w, eachExtent);
                    else
                        sec.SetBounds(pos, 0, eachExtent, h);
                    sec.BringToFront();
                    pos += eachExtent;

                    // Splitter gap (no dedicated control needed for thin gap — just painted)
                    if (i < _pinnedOrder.Count - 1) pos += SplitterSize;
                }
            }

            // --- Position strip buttons in the strip area ---
            if (HasAutoHiddenPanels)
            {
                int btnSize = ScaledThickness;
                int pos = 0;
                foreach (var id in _autoHiddenOrder)
                {
                    if (!_stripButtons.TryGetValue(id, out var btn)) continue;
                    if (vertical)
                    {
                        // Strip is at the bottom of the rail (vertical)
                        btn.SetBounds(0, h - stripPx + pos, w, btnSize);
                    }
                    else
                    {
                        // Strip is at the right of the rail (horizontal)
                        btn.SetBounds(w - stripPx + pos, 0, btnSize, h);
                    }
                    btn.BringToFront();
                    pos += btnSize;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Draw outer border on the inner edge (the side adjacent to the content area)
            using var pen = new Pen(_borderColor, 1f);
            switch (_edge)
            {
                case DockEdge.Left:   g.DrawLine(pen, Width - 1, 0, Width - 1, Height - 1); break;
                case DockEdge.Right:  g.DrawLine(pen, 0, 0, 0, Height - 1);                 break;
                case DockEdge.Top:    g.DrawLine(pen, 0, Height - 1, Width - 1, Height - 1); break;
                case DockEdge.Bottom: g.DrawLine(pen, 0, 0, Width - 1, 0);                  break;
            }

            // Paint splitter gaps between pinned sections
            if (_pinnedOrder.Count > 1)
            {
                bool vertical = _edge == DockEdge.Left || _edge == DockEdge.Right;
                for (int i = 0; i < _pinnedOrder.Count - 1; i++)
                {
                    var id = _pinnedOrder[i];
                    if (!_sections.TryGetValue(id, out var sec)) continue;
                    int gapStart = vertical ? sec.Bottom : sec.Right;
                    using var splitterPen = new Pen(Color.FromArgb(80, _borderColor), SplitterSize);
                    if (vertical)
                        g.DrawLine(splitterPen, 0, gapStart + SplitterSize / 2, Width, gapStart + SplitterSize / 2);
                    else
                        g.DrawLine(splitterPen, gapStart + SplitterSize / 2, 0, gapStart + SplitterSize / 2, Height);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Overlay (auto-hide flyout within the rail area)
        // ─────────────────────────────────────────────────────────────────────

        private void ToggleOverlay(string id, DockPanelDescriptor desc)
        {
            if (_overlayPanelId == id)
            {
                CloseOverlay();
                return;
            }
            OpenOverlay(id, desc);
        }

        private void OpenOverlay(string id, DockPanelDescriptor desc)
        {
            CloseOverlay();
            if (desc.Content is null) return;

            _overlayPanelId = id;
            _overlay ??= new Panel { BackColor = _backColor };
            Controls.Add(_overlay);
            _overlay.Controls.Clear();
            _overlay.BringToFront();

            // Header
            var header = BuildOverlayHeader(id, desc);
            _overlay.Controls.Add(header);

            desc.Content.Dock = DockStyle.Fill;
            _overlay.Controls.Add(desc.Content);

            // Position: fills the non-strip portion of the rail
            bool vertical = _edge == DockEdge.Left || _edge == DockEdge.Right;
            int  strip    = ScaledThickness;
            if (vertical)
                _overlay.SetBounds(0, 0, Width, Height - strip);
            else
                _overlay.SetBounds(0, 0, Width - strip, Height);

            _overlay.Visible = true;
        }

        private Panel BuildOverlayHeader(string id, DockPanelDescriptor desc)
        {
            int headerH = (int)(HeaderHeight * (IsHandleCreated ? DeviceDpi / 96f : 1f));
            var header = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = headerH,
                BackColor = _headerBack,
            };

            var btnClose = new Button
            {
                Text      = "✕",
                Width     = headerH,
                Height    = headerH,
                Dock      = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = _headerBack,
                ForeColor = _foreColor,
                Cursor    = Cursors.Hand,
                TabStop   = false,
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (_, _) => CloseOverlay();

            var btnPin = new Button
            {
                Text      = "📌",
                Width     = headerH,
                Height    = headerH,
                Dock      = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = _headerBack,
                ForeColor = _foreColor,
                Cursor    = Cursors.Hand,
                TabStop   = false,
            };
            btnPin.FlatAppearance.BorderSize = 0;
            var capturedId = id;
            btnPin.Click += (_, _) =>
            {
                CloseOverlay();
                PanelPinned?.Invoke(this, new DockRailPanelEventArgs(capturedId));
            };

            var lbl = new Label
            {
                Text      = desc.Title,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = _foreColor,
                BackColor = Color.Transparent,
                Padding   = new Padding(4, 0, 0, 0),
            };

            header.Controls.Add(lbl);
            header.Controls.Add(btnPin);
            header.Controls.Add(btnClose);
            return header;
        }

        private void CloseOverlay()
        {
            if (_overlay is null) return;
            // Return content to its descriptor (detach before hiding)
            if (_overlayPanelId is not null &&
                _autoHiddenOrder.Contains(_overlayPanelId))
            {
                // Remove content from overlay — it lives in the descriptor, not the overlay
                foreach (Control c in _overlay.Controls.Cast<Control>().ToList())
                {
                    if (c is not Panel header)
                        _overlay.Controls.Remove(c);
                }
            }
            _overlay.Visible = false;
            _overlayPanelId  = null;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Internal helpers
        // ─────────────────────────────────────────────────────────────────────

        private void RemovePanel(string id)
        {
            _pinnedOrder.Remove(id);
            _autoHiddenOrder.Remove(id);

            if (_sections.TryGetValue(id, out var sec))
            {
                Controls.Remove(sec);
                sec.Dispose();
                _sections.Remove(id);
            }

            if (_stripButtons.TryGetValue(id, out var btn))
            {
                Controls.Remove(btn);
                btn.Dispose();
                _stripButtons.Remove(id);
            }

            if (_overlayPanelId == id)
                CloseOverlay();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var s in _sections.Values)  s.Dispose();
                foreach (var b in _stripButtons.Values) b.Dispose();
                _overlay?.Dispose();
            }
            base.Dispose(disposing);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Nested: PinnedPanelSection
        // ─────────────────────────────────────────────────────────────────────

        private sealed class PinnedPanelSection : Panel
        {
            private readonly DockPanelDescriptor _desc;
            private readonly DockEdge            _edge;
            private readonly Panel               _header;
            private readonly Label               _titleLabel;
            private int                          _desiredExtent;

            public event EventHandler<IdEventArgs>? PinClicked;
            public event EventHandler<IdEventArgs>? CloseClicked;

            public int DesiredExtent
            {
                get => _desiredExtent;
                set
                {
                    _desiredExtent = value;
                    if (_edge == DockEdge.Left || _edge == DockEdge.Right)
                        Height = value;
                    else
                        Width = value;
                }
            }

            public PinnedPanelSection(
                DockPanelDescriptor desc,
                DockEdge edge,
                Color back, Color fore, Color headerBack, Color border)
            {
                _desc  = desc;
                _edge  = edge;
                BackColor   = back;
                BorderStyle = BorderStyle.None;

                int headerH = 26;
                _header = new Panel { Dock = DockStyle.Top, Height = headerH, BackColor = headerBack };

                var btnClose = new Button
                {
                    Text      = "✕",
                    Width     = headerH,
                    Height    = headerH,
                    Dock      = DockStyle.Right,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = headerBack,
                    ForeColor = fore,
                    Cursor    = Cursors.Hand,
                    TabStop   = false,
                };
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Click += (_, _) => CloseClicked?.Invoke(this, new IdEventArgs(desc.Id));

                var btnPin = new Button
                {
                    Text      = "📌",
                    Width     = headerH,
                    Height    = headerH,
                    Dock      = DockStyle.Right,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = headerBack,
                    ForeColor = fore,
                    Cursor    = Cursors.Hand,
                    TabStop   = false,
                };
                btnPin.FlatAppearance.BorderSize = 0;
                btnPin.Click += (_, _) => PinClicked?.Invoke(this, new IdEventArgs(desc.Id));

                _titleLabel = new Label
                {
                    Text      = desc.Title,
                    Dock      = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = fore,
                    BackColor = Color.Transparent,
                    Padding   = new Padding(4, 0, 0, 0),
                };

                _header.Controls.Add(_titleLabel);
                _header.Controls.Add(btnPin);
                _header.Controls.Add(btnClose);

                Controls.Add(_header);

                if (desc.Content is not null)
                {
                    desc.Content.Dock = DockStyle.Fill;
                    Controls.Add(desc.Content);
                }
            }

            public void DetachContent()
            {
                if (_desc.Content is not null && Controls.Contains(_desc.Content))
                    Controls.Remove(_desc.Content);
            }

            public void ApplyTheme(BeepTheme theme)
            {
                BackColor         = theme.PanelBackColor;
                _header.BackColor = theme.PrimaryColor;
                _titleLabel.ForeColor = theme.ForeColor;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Nested: StripButton
        // ─────────────────────────────────────────────────────────────────────

        private sealed class StripButton : Control
        {
            private readonly DockPanelDescriptor _desc;
            private readonly DockEdge            _edge;
            private bool _hovered;

            public event EventHandler<IdEventArgs>? Clicked;

            private Color _backColor, _foreColor;

            public StripButton(DockPanelDescriptor desc, DockEdge edge, Color back, Color fore)
            {
                _desc      = desc;
                _edge      = edge;
                _backColor = back;
                _foreColor = fore;
                Cursor     = Cursors.Hand;
                BackColor  = back;
                SetStyle(
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint,
                    true);
            }

            public void ApplyTheme(BeepTheme theme)
            {
                _backColor = theme.PanelBackColor;
                _foreColor = theme.ForeColor;
                BackColor  = _backColor;
                Invalidate();
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                _hovered = true;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                _hovered = false;
                Invalidate();
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                Clicked?.Invoke(this, new IdEventArgs(_desc.Id));
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.Clear(_hovered
                    ? ControlPaint.Light(_backColor, 0.15f)
                    : _backColor);

                bool vertical = _edge == DockEdge.Left || _edge == DockEdge.Right;
                var  tf       = new StringFormat(StringFormatFlags.NoWrap);

                if (vertical)
                {
                    // Rotated text for left/right edges
                    tf.FormatFlags |= StringFormatFlags.DirectionVertical;
                    using var font  = new Font("Segoe UI", 8f);
                    using var brush = new SolidBrush(_foreColor);
                    var size = g.MeasureString(_desc.Title, font);
                    g.DrawString(_desc.Title, font, brush,
                        (Width  - size.Height) / 2f,
                        (Height - size.Width)  / 2f,
                        tf);
                }
                else
                {
                    using var font  = new Font("Segoe UI", 8f);
                    using var brush = new SolidBrush(_foreColor);
                    g.DrawString(_desc.Title, font, brush,
                        new RectangleF(0, 0, Width, Height), tf);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Nested event args
        // ─────────────────────────────────────────────────────────────────────

        private sealed class IdEventArgs : EventArgs
        {
            public string Id { get; }
            public IdEventArgs(string id) => Id = id;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Public event args used by BeepDockManager ← BeepDockRail
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Event args carrying the panel id from a <see cref="BeepDockRail"/> event.</summary>
    public sealed class DockRailPanelEventArgs : EventArgs
    {
        public string Id { get; }
        public DockRailPanelEventArgs(string id) => Id = id;
    }
}
