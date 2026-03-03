// BeepDocumentQuickSwitch.cs
// Keyboard-driven document picker popup for BeepDocumentHost.
// Shown via Ctrl+P (or BeepDocumentHost.ShowQuickSwitch()).
// Provides type-to-filter + arrow-key navigation over open documents.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Lightweight floating popup that lets the user quickly switch to any
    /// open document by typing a substring of its title or id.
    /// </summary>
    internal sealed class BeepDocumentQuickSwitch : Form
    {
        // ── Controls ─────────────────────────────────────────────────────────

        private readonly TextBox  _search;
        private readonly ListBox  _list;
        private readonly Panel    _frame;

        // ── Data ─────────────────────────────────────────────────────────────

        private readonly IReadOnlyList<BeepDocumentTab> _allTabs;
        private readonly IBeepTheme?                    _theme;
        private List<BeepDocumentTab>                   _filtered = new();

        // ── Result ───────────────────────────────────────────────────────────

        /// <summary>The document id chosen by the user, or <c>null</c> if cancelled.</summary>
        internal string? SelectedDocumentId { get; private set; }

        // ── Layout constants ──────────────────────────────────────────────────

        private const int PopupWidth  = 420;
        private const int PopupHeight = 340;
        private const int SearchH     = 36;
        private const int Pad         = 8;
        private const int ItemH       = 30;

        // ════════════════════════════════════════════════════════════════════
        // Constructor
        // ════════════════════════════════════════════════════════════════════

        internal BeepDocumentQuickSwitch(
            IReadOnlyList<BeepDocumentTab> tabs,
            string?                        activeDocumentId,
            IBeepTheme?                    theme,
            Point                          screenPosition)
        {
            _allTabs = tabs;
            _theme   = theme;

            // ── Form setup ───────────────────────────────────────────────────
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            StartPosition   = FormStartPosition.Manual;
            Size            = new Size(PopupWidth, PopupHeight);
            Location        = screenPosition;
            KeyPreview      = true;

            BackColor = theme?.PanelBackColor ?? SystemColors.Window;

            // ── Outer frame panel ────────────────────────────────────────────
            _frame = new Panel
            {
                Dock        = DockStyle.Fill,
                Padding     = new Padding(Pad),
                BackColor   = theme?.PanelBackColor ?? SystemColors.Window
            };

            // ── Search box ───────────────────────────────────────────────────
            _search = new TextBox
            {
                Bounds         = new Rectangle(Pad, Pad, PopupWidth - Pad * 2, SearchH),
                Anchor         = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle    = BorderStyle.FixedSingle,
                Font           = new Font("Segoe UI", 11),
                PlaceholderText= "Type to filter…",
                BackColor      = theme?.BackgroundColor ?? SystemColors.Window,
                ForeColor      = theme?.ForeColor       ?? SystemColors.WindowText,
            };

            // ── Results list ─────────────────────────────────────────────────
            _list = new ListBox
            {
                Bounds         = new Rectangle(Pad, Pad + SearchH + 4, PopupWidth - Pad * 2,
                                               PopupHeight - SearchH - Pad * 3 - 4),
                Anchor         = AnchorStyles.Top | AnchorStyles.Bottom
                                | AnchorStyles.Left | AnchorStyles.Right,
                DrawMode       = DrawMode.OwnerDrawFixed,
                ItemHeight     = ItemH,
                BorderStyle    = BorderStyle.None,
                BackColor      = theme?.PanelBackColor ?? SystemColors.Window,
                ForeColor      = theme?.ForeColor ?? SystemColors.WindowText,
                ScrollAlwaysVisible = false,
                IntegralHeight = false,
            };

            _frame.Controls.Add(_search);
            _frame.Controls.Add(_list);
            Controls.Add(_frame);

            // ── Wire events ──────────────────────────────────────────────────
            _search.TextChanged  += OnSearchChanged;
            _list.DrawItem       += OnDrawItem;
            _list.MouseDoubleClick += (s, e) => CommitSelection();

            KeyDown  += OnKeyDown;
            _search.KeyDown += OnKeyDown;
            _list.KeyDown   += OnKeyDown;

            Deactivate += (s, e) => { SelectedDocumentId = null; Close(); };

            // ── Populate ─────────────────────────────────────────────────────
            PopulateList(string.Empty, activeDocumentId);
            _search.Focus();
        }

        // ── Search filter ────────────────────────────────────────────────────

        private void OnSearchChanged(object? sender, EventArgs e)
        {
            PopulateList(_search.Text, null);
        }

        private void PopulateList(string filter, string? preselectId)
        {
            _filtered = string.IsNullOrWhiteSpace(filter)
                ? _allTabs.ToList()
                : _allTabs.Where(t =>
                      t.Title.Contains(filter, StringComparison.OrdinalIgnoreCase)
                   || t.Id.Contains(filter, StringComparison.OrdinalIgnoreCase))
                  .ToList();

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var t in _filtered)
                _list.Items.Add(t.Title);
            _list.EndUpdate();

            // Pre-select — the active document (when no filter) or the first match
            if (_filtered.Count > 0)
            {
                int sel = 0;
                if (preselectId != null)
                {
                    int idx = _filtered.FindIndex(t => t.Id == preselectId);
                    if (idx >= 0) sel = idx;
                }
                _list.SelectedIndex = sel;
            }
        }

        // ── Keyboard handling ────────────────────────────────────────────────

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    SelectedDocumentId = null;
                    e.Handled = true;
                    Close();
                    break;

                case Keys.Enter:
                    CommitSelection();
                    e.Handled = true;
                    break;

                case Keys.Down:
                    if (_list.SelectedIndex < _list.Items.Count - 1)
                        _list.SelectedIndex++;
                    e.Handled = true;
                    break;

                case Keys.Up:
                    if (_list.SelectedIndex > 0)
                        _list.SelectedIndex--;
                    e.Handled = true;
                    break;

                case Keys.Tab:
                    // Tab with no filter → cycle downward
                    if (_list.Items.Count > 0)
                    {
                        int n = e.Shift
                            ? (_list.SelectedIndex - 1 + _list.Items.Count) % _list.Items.Count
                            : (_list.SelectedIndex + 1) % _list.Items.Count;
                        _list.SelectedIndex = n;
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void CommitSelection()
        {
            int idx = _list.SelectedIndex;
            if (idx >= 0 && idx < _filtered.Count)
                SelectedDocumentId = _filtered[idx].Id;
            Close();
        }

        // ── Owner-draw list items ────────────────────────────────────────────

        private void OnDrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _filtered.Count) return;

            var tab     = _filtered[e.Index];
            bool sel    = (e.State & DrawItemState.Selected) != 0;

            Color backCol = sel
                ? (_theme?.PrimaryColor ?? SystemColors.Highlight)
                : (_theme?.PanelBackColor ?? SystemColors.Window);
            Color foreCol = sel
                ? (_theme?.BackgroundColor ?? SystemColors.HighlightText)
                : (_theme?.ForeColor   ?? SystemColors.WindowText);
            Color dimCol  = sel
                ? Color.FromArgb(200, foreCol)
                : (_theme?.SecondaryTextColor ?? SystemColors.GrayText);

            e.Graphics.FillRectangle(new SolidBrush(backCol), e.Bounds);

            // Active indicator bar
            if (tab.IsActive)
            {
                using var bar = new SolidBrush(_theme?.PrimaryColor ?? Color.DodgerBlue);
                e.Graphics.FillRectangle(bar,
                    e.Bounds.Left, e.Bounds.Top + 4, 3, e.Bounds.Height - 8);
            }

            // Icon area placeholder (8 px left margin)
            int x = e.Bounds.Left + 12;

            // Modified dot
            if (tab.IsModified)
            {
                using var dot = new SolidBrush(Color.OrangeRed);
                e.Graphics.FillEllipse(dot,
                    x, e.Bounds.Top + (e.Bounds.Height - 8) / 2, 8, 8);
                x += 12;
            }

            // Title
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var titleRect = new Rectangle(
                x, e.Bounds.Top, e.Bounds.Width - x - 60, e.Bounds.Height);
            using var titleFmt = new StringFormat
            {
                Alignment     = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
                FormatFlags   = StringFormatFlags.NoWrap
            };
            using var titleBr = new SolidBrush(foreCol);
            e.Graphics.DrawString(tab.Title, Font, titleBr, titleRect, titleFmt);

            // Pinned badge
            if (tab.IsPinned)
            {
                using var pinBr = new SolidBrush(dimCol);
                using var fmt   = new StringFormat
                    { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("📌", Font, pinBr,
                    new Rectangle(e.Bounds.Right - 28, e.Bounds.Top, 24, e.Bounds.Height), fmt);
            }
        }

        // ── Paint border ────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Color borderCol = _theme?.BorderColor ?? SystemColors.ControlDark;
            using var pen = new Pen(borderCol, 1);
            e.Graphics.DrawRectangle(pen,
                0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
        }

        // ── WndProc: resizable via owner-drawn border ─────────────────────────

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // Prevent the shadow / rounded corners that look wrong for a picker
                cp.ClassStyle |= 0x20000; // CS_DROPSHADOW
                return cp;
            }
        }
    }
}
