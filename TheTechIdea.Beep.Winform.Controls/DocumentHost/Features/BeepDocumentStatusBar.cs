// BeepDocumentStatusBar.cs
// Rich segmented status bar for BeepDocumentHost.
// Segments (left-to-right):
//   Left   — document-type icon · encoding · line-ending mode
//   Center — cursor position (Line:Col) and selection character count
//   Right  — git branch name · notification badges · zoom level
//
// Each segment is clickable and opens a relevant ContextMenuStrip.
// Height is fixed at 22 px (DPI-scaled via the provided scale factor).
// The bar is theme-aware and repaints whenever ApplyTheme() is called.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    // ── Event args ────────────────────────────────────────────────────────────

    /// <summary>Raised when the user clicks a segment on <see cref="BeepDocumentStatusBar"/>.</summary>
    public sealed class StatusBarSegmentClickedEventArgs : EventArgs
    {
        /// <summary>Logical id of the clicked segment (e.g. "encoding", "cursor", "git").</summary>
        public string SegmentId { get; }
        public StatusBarSegmentClickedEventArgs(string segmentId) => SegmentId = segmentId;
    }

    // ── Data contracts ────────────────────────────────────────────────────────

    /// <summary>Information the host pushes to the status bar to keep it up-to-date.</summary>
    public sealed class StatusBarInfo
    {
        // Left segment
        public string  DocumentType { get; set; } = string.Empty;
        public string  Encoding     { get; set; } = "UTF-8";
        public string  LineEnding   { get; set; } = "CRLF";

        // Center segment
        public int     Line         { get; set; } = 1;
        public int     Column       { get; set; } = 1;
        public int     SelectionLen { get; set; } = 0;

        // Right segment
        public string? GitBranch    { get; set; }
        public int     Notifications{ get; set; } = 0;
        public int     ZoomPercent  { get; set; } = 100;
    }

    // ── Main control ──────────────────────────────────────────────────────────

    /// <summary>
    /// Horizontally segmented status bar that lives along the bottom edge of
    /// <see cref="BeepDocumentHost"/>.  Call <see cref="UpdateInfo"/> to refresh
    /// the displayed values; the bar repaints itself automatically.
    /// </summary>
    public sealed class BeepDocumentStatusBar : Control
    {
        // ── Layout constants ─────────────────────────────────────────────────

        private const int BarHeight        = 22;
        private const int SegmentPadding   = 8;
        private const int SeparatorWidth   = 1;
        private const int NotificationSize = 14;

        // ── State ────────────────────────────────────────────────────────────

        private StatusBarInfo _info = new();

        // Segment hit rects (populated in OnPaint)
        private Rectangle _leftRect;
        private Rectangle _centerRect;
        private Rectangle _rightRect;

        // Hot-tracking
        private string? _hotSegment;

        // Theme colours
        private Color _backColor  = Color.FromArgb(0x2d, 0x2d, 0x30);
        private Color _foreColor  = Color.FromArgb(0xcc, 0xcc, 0xcc);
        private Color _hotBack    = Color.FromArgb(0x3f, 0x3f, 0x46);
        private Color _sepColor   = Color.FromArgb(0x3f, 0x3f, 0x46);
        private Color _accentColor= Color.FromArgb(0x00, 0x7a, 0xcc);
        private Color _warnColor  = Color.FromArgb(0xff, 0xcc, 0x00);
        private float _dpiScale   = 1.0f;

        // Context menus (created on demand)
        private ContextMenuStrip? _leftMenu;
        private ContextMenuStrip? _centerMenu;
        private ContextMenuStrip? _rightMenu;

        // ── Events ───────────────────────────────────────────────────────────

        /// <summary>Fired when the user clicks any status bar segment.</summary>
        public event EventHandler<StatusBarSegmentClickedEventArgs>? SegmentClicked;

        /// <summary>Fired when the user selects a different encoding from the left-segment menu.</summary>
        public event EventHandler<string>? EncodingChangeRequested;

        /// <summary>Fired when the user selects a different line-ending from the left-segment menu.</summary>
        public event EventHandler<string>? LineEndingChangeRequested;

        /// <summary>Fired when the user selects a zoom level from the right-segment menu.</summary>
        public event EventHandler<int>? ZoomChangeRequested;

        // ── Construction ─────────────────────────────────────────────────────

        public BeepDocumentStatusBar()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw          |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.UserPaint, true);

            Dock   = DockStyle.Bottom;
            Height = Scale(BarHeight);

            Cursor = Cursors.Arrow;
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Refresh the displayed values.  Triggers a repaint.</summary>
        public void UpdateInfo(StatusBarInfo info)
        {
            ArgumentNullException.ThrowIfNull(info);
            _info = info;
            Invalidate();
        }

        /// <summary>Apply a colour palette from the host's theme.</summary>
        public void ApplyTheme(
            Color backColor,
            Color foreColor,
            Color hotBack,
            Color separatorColor,
            Color accentColor,
            Color warnColor)
        {
            _backColor   = backColor;
            _foreColor   = foreColor;
            _hotBack     = hotBack;
            _sepColor    = separatorColor;
            _accentColor = accentColor;
            _warnColor   = warnColor;
            Invalidate();
        }

        /// <summary>Update the DPI scale factor (call when the host moves to a new monitor).</summary>
        public void SetDpiScale(float scale)
        {
            if (scale <= 0f) return;
            _dpiScale = scale;
            Height    = Scale(BarHeight);
            Invalidate();
        }

        // ── Painting ─────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rc   = ClientRectangle;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Background
            using var bgBrush = new SolidBrush(_backColor);
            g.FillRectangle(bgBrush, rc);

            // Measure text widths to define the three segment bounds
            using var font = new Font("Segoe UI", 8.5f);
            int width      = rc.Width;

            string leftText   = BuildLeftText();
            string centerText = BuildCenterText();
            string rightText  = BuildRightText();

            // Fixed-width centre; left and right share the remainder equally
            SizeF centerSize  = g.MeasureString(centerText, font);
            int   centerW     = (int)centerSize.Width + SegmentPadding * 2;
            int   sideW       = (width - centerW - SeparatorWidth * 2) / 2;

            _leftRect   = new Rectangle(0,       0, sideW,                          rc.Height);
            _centerRect = new Rectangle(sideW + SeparatorWidth, 0, centerW,         rc.Height);
            _rightRect  = new Rectangle(sideW + SeparatorWidth + centerW + SeparatorWidth,
                                        0, width - sideW - SeparatorWidth - centerW - SeparatorWidth,
                                        rc.Height);

            // Draw hot-track backgrounds
            DrawSegmentBack(g, _leftRect,   _hotSegment == "left");
            DrawSegmentBack(g, _centerRect, _hotSegment == "center");
            DrawSegmentBack(g, _rightRect,  _hotSegment == "right");

            // Separators
            using var sepPen = new Pen(_sepColor);
            g.DrawLine(sepPen, _leftRect.Right,   rc.Top, _leftRect.Right,   rc.Bottom);
            g.DrawLine(sepPen, _rightRect.Left - 1, rc.Top, _rightRect.Left - 1, rc.Bottom);

            // Segment text
            using var textBrush = new SolidBrush(_foreColor);
            DrawSegmentText(g, font, textBrush, leftText,   _leftRect,   StringAlignment.Near);
            DrawSegmentText(g, font, textBrush, centerText, _centerRect, StringAlignment.Center);
            DrawRightSegment(g, font, textBrush, rightText);

            base.OnPaint(e);
        }

        private void DrawSegmentBack(Graphics g, Rectangle rc, bool isHot)
        {
            if (!isHot) return;
            using var br = new SolidBrush(_hotBack);
            g.FillRectangle(br, rc);
        }

        private void DrawSegmentText(
            Graphics g, Font font, SolidBrush br,
            string text, Rectangle rc, StringAlignment align)
        {
            if (string.IsNullOrEmpty(text)) return;

            var sf = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment     = align,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter
            };

            var padded = new Rectangle(
                rc.X + SegmentPadding,
                rc.Y,
                rc.Width  - SegmentPadding * 2,
                rc.Height);

            g.DrawString(text, font, br, padded, sf);
        }

        private void DrawRightSegment(Graphics g, Font font, SolidBrush textBr, string text)
        {
            if (_rightRect.Width <= 0) return;

            int x = _rightRect.Right - SegmentPadding;

            // Notification badge (drawn rightmost before zoom)
            if (_info.Notifications > 0)
            {
                int badgeSize = Scale(NotificationSize);
                x -= badgeSize + Scale(2);
                var badgeRc = new Rectangle(x, (_rightRect.Height - badgeSize) / 2, badgeSize, badgeSize);

                using var badgeBr = new SolidBrush(_warnColor);
                g.FillEllipse(badgeBr, badgeRc);

                string cnt = _info.Notifications > 9 ? "9+" : _info.Notifications.ToString();
                using var badgeFont = new Font("Segoe UI", 6.5f, FontStyle.Bold);
                using var whiteBr   = new SolidBrush(Color.Black);
                var bsf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(cnt, badgeFont, whiteBr, badgeRc, bsf);
                x -= Scale(4);
            }

            // Main right text (zoom + git branch)
            var sf = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment     = StringAlignment.Far,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter
            };

            var textRc = new Rectangle(_rightRect.X + SegmentPadding, _rightRect.Y,
                                       x - _rightRect.X - SegmentPadding * 2, _rightRect.Height);
            g.DrawString(text, font, textBr, textRc, sf);
        }

        // ── Text builders ────────────────────────────────────────────────────

        private string BuildLeftText()
        {
            var parts = new List<string>(3);

            if (!string.IsNullOrWhiteSpace(_info.DocumentType))
                parts.Add(_info.DocumentType);

            if (!string.IsNullOrWhiteSpace(_info.Encoding))
                parts.Add(_info.Encoding);

            if (!string.IsNullOrWhiteSpace(_info.LineEnding))
                parts.Add(_info.LineEnding);

            return string.Join("  ·  ", parts);
        }

        private string BuildCenterText()
        {
            string pos = $"Ln {_info.Line}, Col {_info.Column}";
            return _info.SelectionLen > 0
                ? $"{pos}  ({_info.SelectionLen} sel)"
                : pos;
        }

        private string BuildRightText()
        {
            var parts = new List<string>(2);

            if (!string.IsNullOrWhiteSpace(_info.GitBranch))
                parts.Add($"⎇  {_info.GitBranch}");

            parts.Add($"{_info.ZoomPercent}%");

            return string.Join("  ", parts);
        }

        // ── Mouse interaction ─────────────────────────────────────────────────

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            string? seg = HitSegment(e.Location);
            if (seg != _hotSegment)
            {
                _hotSegment = seg;
                Cursor      = seg != null ? Cursors.Hand : Cursors.Arrow;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hotSegment != null)
            {
                _hotSegment = null;
                Cursor      = Cursors.Arrow;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button != MouseButtons.Left) return;

            string? seg = HitSegment(e.Location);
            if (seg == null) return;

            SegmentClicked?.Invoke(this, new StatusBarSegmentClickedEventArgs(seg));

            switch (seg)
            {
                case "left":   ShowLeftMenu(e.Location);   break;
                case "center": ShowCenterMenu(e.Location); break;
                case "right":  ShowRightMenu(e.Location);  break;
            }
        }

        private string? HitSegment(Point p)
        {
            if (_leftRect.Contains(p))   return "left";
            if (_centerRect.Contains(p)) return "center";
            if (_rightRect.Contains(p))  return "right";
            return null;
        }

        // ── Context menus ─────────────────────────────────────────────────────

        private void ShowLeftMenu(Point location)
        {
            _leftMenu ??= BuildLeftMenu();
            _leftMenu.Show(this, location);
        }

        private void ShowCenterMenu(Point location)
        {
            _centerMenu ??= BuildCenterMenu();
            _centerMenu.Show(this, location);
        }

        private void ShowRightMenu(Point location)
        {
            _rightMenu ??= BuildRightMenu();
            _rightMenu.Show(this, location);
        }

        private ContextMenuStrip BuildLeftMenu()
        {
            var menu = new ContextMenuStrip();

            var encHeader = new ToolStripLabel("Encoding") { ForeColor = Color.Gray };
            menu.Items.Add(encHeader);
            menu.Items.Add(new ToolStripSeparator());

            foreach (string enc in new[] { "UTF-8", "UTF-8 BOM", "UTF-16 LE", "UTF-16 BE", "ASCII", "ISO-8859-1" })
            {
                string captured = enc;
                var item = new ToolStripMenuItem(captured)
                {
                    Checked = _info.Encoding == captured
                };
                item.Click += (_, _) => EncodingChangeRequested?.Invoke(this, captured);
                menu.Items.Add(item);
            }

            menu.Items.Add(new ToolStripSeparator());
            var leHeader = new ToolStripLabel("Line Ending") { ForeColor = Color.Gray };
            menu.Items.Add(leHeader);
            menu.Items.Add(new ToolStripSeparator());

            foreach (string le in new[] { "CRLF", "LF", "CR" })
            {
                string captured = le;
                var item = new ToolStripMenuItem(captured)
                {
                    Checked = _info.LineEnding == captured
                };
                item.Click += (_, _) => LineEndingChangeRequested?.Invoke(this, captured);
                menu.Items.Add(item);
            }

            return menu;
        }

        private ContextMenuStrip BuildCenterMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add(new ToolStripMenuItem("Go to Line…", null,
                (_, _) => SegmentClicked?.Invoke(this, new StatusBarSegmentClickedEventArgs("goto-line"))));
            menu.Items.Add(new ToolStripMenuItem("Go to Column…", null,
                (_, _) => SegmentClicked?.Invoke(this, new StatusBarSegmentClickedEventArgs("goto-column"))));
            return menu;
        }

        private ContextMenuStrip BuildRightMenu()
        {
            var menu = new ContextMenuStrip();

            var zoomHeader = new ToolStripLabel("Zoom") { ForeColor = Color.Gray };
            menu.Items.Add(zoomHeader);
            menu.Items.Add(new ToolStripSeparator());

            foreach (int pct in new[] { 50, 75, 100, 125, 150, 200 })
            {
                int captured = pct;
                var item = new ToolStripMenuItem($"{captured}%")
                {
                    Checked = _info.ZoomPercent == captured
                };
                item.Click += (_, _) => ZoomChangeRequested?.Invoke(this, captured);
                menu.Items.Add(item);
            }

            if (!string.IsNullOrWhiteSpace(_info.GitBranch))
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(new ToolStripMenuItem("Refresh Git Status", null,
                    (_, _) => SegmentClicked?.Invoke(this, new StatusBarSegmentClickedEventArgs("git-refresh"))));
            }

            return menu;
        }

        // ── Accessibility ─────────────────────────────────────────────────────

        protected override AccessibleObject CreateAccessibilityInstance()
            => new StatusBarAccessibleObject(this);

        private sealed class StatusBarAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepDocumentStatusBar _bar;

            public StatusBarAccessibleObject(BeepDocumentStatusBar bar) : base(bar) => _bar = bar;

            public override string? Name        => "Document Status Bar";
            public override string? Description => _bar.BuildCenterText();
            public override AccessibleRole Role => AccessibleRole.StatusBar;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private int Scale(int value) => (int)Math.Round(value * _dpiScale);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _leftMenu?.Dispose();
                _centerMenu?.Dispose();
                _rightMenu?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
