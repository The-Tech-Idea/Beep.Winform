// BeepTabOverflowPopup.cs
// Polished, searchable, keyboard-navigable overflow dropdown for BeepDocumentTabStrip.
// Improvements over the plain ContextMenuStrip (Sprint 14.3):
//   • Fuzzy search (exact > starts-with > word-boundary > contains scoring)
//   • Group entries by BeepDocumentTab.DocumentCategory with section headers
//   • Pinned tabs section at the top
//   • Keyboard navigation: ↑↓ Page Up/Down, Enter to select, Esc to dismiss
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Borderless searchable popup that replaces the plain ContextMenuStrip overflow menu.
    /// </summary>
    internal sealed class BeepTabOverflowPopup : Form
    {
        // ── Model ─────────────────────────────────────────────────────────────

        private sealed class OverflowRow
        {
            public bool IsHeader    { get; init; }
            public bool IsSeparator { get; init; }
            public string? Header   { get; init; }
            public BeepDocumentTab? Tab  { get; init; }
            public int  OriginalIndex    { get; init; } = -1;
        }

        // ── Fields ────────────────────────────────────────────────────────────

        private readonly BeepDocumentTabStrip _strip;
        private readonly IBeepTheme?          _currentTheme;

        private readonly TextBox  _search;
        private readonly ListBox  _list;
        private readonly Panel    _border;

        private List<OverflowRow> _allRows   = new();
        private List<OverflowRow> _rows      = new();  // filtered

        // ── Constants ─────────────────────────────────────────────────────────

        private const int PopupWidth  = 320;
        private const int PopupHeight = 400;
        private const int SearchH     = 32;
        private const int ItemH       = 28;
        private const int HeaderH     = 22;
        private const int SepH        = 8;
        private const int Padding     = 6;

        // ── Constructor ───────────────────────────────────────────────────────

        internal BeepTabOverflowPopup(BeepDocumentTabStrip strip, IBeepTheme? theme)
        {
            _strip = strip;
            _currentTheme = theme;

            // Popup window setup
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            StartPosition   = FormStartPosition.Manual;
            TopMost         = true;
            Width           = PopupWidth;
            Height          = PopupHeight;
            BackColor       = ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window);

            // Thin themed border panel
            _border         = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = ThemeAwareColor(_currentTheme?.BorderColor, SystemColors.ControlDark),
                Padding   = new Padding(1)
            };
            Controls.Add(_border);

            // Inner layout: search at top, list below
            var inner = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window),
                Padding   = new Padding(0)
            };
            _border.Controls.Add(inner);

            _search = new TextBox
            {
                Dock        = DockStyle.Top,
                Height      = SearchH,
                BorderStyle = BorderStyle.None,
                Font        = new Font("Segoe UI", 10f),
                BackColor   = ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window),
                ForeColor   = ThemeAwareColor(_currentTheme?.ForeColor, SystemColors.WindowText),
                Padding     = new Padding(Padding, 0, Padding, 0)
            };
            SetPlaceholder(_search, "Search tabs…");
            _search.TextChanged += OnSearchChanged;
            _search.KeyDown     += OnKeyDown;
            inner.Controls.Add(_search);

            // Thin separator under search
            var sep = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 1,
                BackColor = ThemeAwareColor(_currentTheme?.BorderColor, SystemColors.ControlDark)
            };
            inner.Controls.Add(sep);

            _list = new ListBox
            {
                Dock        = DockStyle.Fill,
                DrawMode    = DrawMode.OwnerDrawVariable,
                ScrollAlwaysVisible = false,
                BorderStyle = BorderStyle.None,
                BackColor   = ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window),
                ForeColor   = ThemeAwareColor(_currentTheme?.ForeColor, SystemColors.WindowText),
                Font        = BeepFontManager.GetCachedFont("Segoe UI", 9.5f, FontStyle.Regular),
                ItemHeight  = ItemH
            };
            _list.MeasureItem    += OnMeasureItem;
            _list.DrawItem       += OnDrawItem;
            _list.MouseClick     += OnListClick;
            _list.KeyDown        += OnKeyDown;
            inner.Controls.Add(_list);

            // Close on deactivate (click outside)
            Deactivate += (s, e) => SafeClose();
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Populates from the current strip tabs and shows below the given screen point.</summary>
        internal void ShowBelow(Point screenPt)
        {
            BuildAllRows();
            ApplyFilter(string.Empty);
            Location = ConstrainToScreen(screenPt);
            Show();
            _search.Focus();
        }

        // ── Row building ──────────────────────────────────────────────────────

        private void BuildAllRows()
        {
            _allRows.Clear();
            var tabs = _strip.Tabs;

            var pinned    = tabs.Select((t, i) => (t, i)).Where(x => x.t.IsPinned).ToList();
            var unpinned  = tabs.Select((t, i) => (t, i)).Where(x => !x.t.IsPinned).ToList();

            // Section 1: pinned
            if (pinned.Count > 0)
            {
                _allRows.Add(new OverflowRow { IsHeader = true, Header = "📌 Pinned" });
                foreach (var (t, idx) in pinned)
                    _allRows.Add(new OverflowRow { Tab = t, OriginalIndex = idx });
                _allRows.Add(new OverflowRow { IsSeparator = true });
            }

            // Section 2+: group unpinned by DocumentCategory
            var groups = unpinned
                .GroupBy(x => x.t.DocumentCategory ?? string.Empty)
                .OrderBy(g => string.IsNullOrEmpty(g.Key) ? 1 : 0)   // named categories first
                .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            bool multiGroup = groups.Count > 1 || (groups.Count == 1 && !string.IsNullOrEmpty(groups[0].Key));

            foreach (var grp in groups)
            {
                if (multiGroup && !string.IsNullOrEmpty(grp.Key))
                    _allRows.Add(new OverflowRow { IsHeader = true, Header = grp.Key });

                foreach (var (t, idx) in grp)
                    _allRows.Add(new OverflowRow { Tab = t, OriginalIndex = idx });

                if (multiGroup && grp != groups.Last())
                    _allRows.Add(new OverflowRow { IsSeparator = true });
            }
        }

        private void ApplyFilter(string query)
        {
            query = query.Trim();

            if (string.IsNullOrEmpty(query))
            {
                _rows = new List<OverflowRow>(_allRows);
            }
            else
            {
                // Filter: keep section headers only if they have matching children
                var candidates = _allRows
                    .Where(r => r.Tab != null)
                    .Select(r => (row: r, score: FuzzyScore(r.Tab!.Title ?? "", query)))
                    .Where(x => x.score >= 0)
                    .OrderByDescending(x => x.score)
                    .ThenBy(x => x.row.Tab!.Title, StringComparer.OrdinalIgnoreCase)
                    .Select(x => x.row)
                    .ToList();
                _rows = candidates;
            }

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var r in _rows)
                _list.Items.Add(r);
            _list.EndUpdate();

            // Pre-select first real tab row
            int first = _rows.FindIndex(r => r.Tab != null && !r.IsHeader && !r.IsSeparator);
            if (first >= 0) _list.SelectedIndex = first;
        }

        // ── Fuzzy scoring (higher = better match; -1 = no match) ──────────────

        private static int FuzzyScore(string title, string query)
        {
            var t = title.ToLowerInvariant();
            var q = query.ToLowerInvariant();
            if (t == q)                                              return 4;
            if (t.StartsWith(q, StringComparison.Ordinal))          return 3;
            if (t.Split(new[]{' ','-','_','.'}, StringSplitOptions.RemoveEmptyEntries)
                  .Any(w => w.StartsWith(q, StringComparison.Ordinal))) return 2;
            if (t.Contains(q, StringComparison.Ordinal))             return 1;
            return -1;
        }

        // ── Owner-draw ────────────────────────────────────────────────────────

        private void OnMeasureItem(object? sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _rows.Count) return;
            var row = _rows[e.Index];
            e.ItemHeight = row.IsSeparator ? SepH : row.IsHeader ? HeaderH : ItemH;
        }

        private void OnDrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _rows.Count) return;
            var row = _rows[e.Index];
            var g   = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color back   = ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window);
            Color fore   = ThemeAwareColor(_currentTheme?.ForeColor, SystemColors.WindowText);
            Color accent = ThemeAwareColor(_currentTheme?.PrimaryColor, SystemColors.Highlight);
            Color subFore= ThemeAwareGrayText(_currentTheme?.PanelBackColor);

            var r = e.Bounds;

            // ── Separator ──────────────────────────────────────────────────
            if (row.IsSeparator)
            {
                g.FillRectangle(new SolidBrush(back), r);
                int my = r.Top + r.Height / 2;
                using var sep = new Pen(Color.FromArgb(40, fore));
                g.DrawLine(sep, r.Left + 8, my, r.Right - 8, my);
                return;
            }

            // ── Section header ─────────────────────────────────────────────
            if (row.IsHeader)
            {
                g.FillRectangle(new SolidBrush(back), r);
                using var hFnt = new Font("Segoe UI", 8f, FontStyle.Bold);
                using var hBr  = new SolidBrush(subFore);
                g.DrawString(row.Header, hFnt, hBr,
                    new RectangleF(r.Left + 8, r.Top, r.Width - 8, r.Height),
                    new StringFormat { LineAlignment = StringAlignment.Center });
                return;
            }

            // ── Tab row ────────────────────────────────────────────────────
            var tab = row.Tab!;
            bool selected = (e.State & DrawItemState.Selected) != 0;

            Color fill = selected ? accent : back;
            Color text = selected ? (ContrastColor(accent)) : fore;

            g.FillRectangle(new SolidBrush(fill), r);

            // Modified dot
            if (tab.IsModified)
            {
                int ds = 6;
                using var dotBr = new SolidBrush(_currentTheme?.WarningColor ?? Color.Orange);
                g.FillEllipse(dotBr, r.Left + 6, r.Top + (r.Height - ds) / 2, ds, ds);
            }

            int textLeft = r.Left + (tab.IsModified ? 18 : 8);

            // Title
            using var tFont = tab.IsActive ? new Font(_list.Font, FontStyle.Bold) : _list.Font;
            bool ownFont    = tab.IsActive;
            using var tBr   = new SolidBrush(text);
            var titleRect   = new RectangleF(textLeft, r.Top, r.Width - textLeft - 4, r.Height);
            using var fmt   = new StringFormat
            {
                Alignment     = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
                FormatFlags   = StringFormatFlags.NoWrap
            };
            g.DrawString(tab.Title, tFont, tBr, titleRect, fmt);
        }

        // Returns black or white depending on which contrasts better with the background
        private static Color ContrastColor(Color bg)
        {
            double lum = (0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B) / 255.0;
            return lum > 0.5 ? Color.Black : Color.White;
        }

        // ── Keyboard ──────────────────────────────────────────────────────────

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    SafeClose();
                    break;

                case Keys.Enter:
                    e.Handled = true;
                    ActivateSelected();
                    break;

                case Keys.Down:
                    e.Handled = true;
                    MoveFocus(+1);
                    break;

                case Keys.Up:
                    e.Handled = true;
                    MoveFocus(-1);
                    break;

                case Keys.PageDown:
                    e.Handled = true;
                    MoveFocus(+5);
                    break;

                case Keys.PageUp:
                    e.Handled = true;
                    MoveFocus(-5);
                    break;
            }
        }

        private void MoveFocus(int delta)
        {
            if (_rows.Count == 0) return;
            int start = _list.SelectedIndex < 0 ? (delta > 0 ? -1 : _rows.Count) : _list.SelectedIndex;
            int next  = start + delta;

            // Skip header/separator rows
            for (int tries = 0; tries < _rows.Count; tries++)
            {
                next = (next % _rows.Count + _rows.Count) % _rows.Count;
                var r = _rows[next];
                if (!r.IsHeader && !r.IsSeparator && r.Tab != null)
                {
                    _list.SelectedIndex = next;
                    return;
                }
                next += delta > 0 ? 1 : -1;
            }
        }

        private void ActivateSelected()
        {
            int si = _list.SelectedIndex;
            if (si < 0 || si >= _rows.Count) return;
            var row = _rows[si];
            if (row.Tab == null || row.IsHeader || row.IsSeparator) return;

            _strip.ActiveTabIndex = row.OriginalIndex;
            _strip.RaiseTabSelected(new TabEventArgs(row.OriginalIndex, row.Tab));
            SafeClose();
        }

        private void OnListClick(object? sender, MouseEventArgs e)
        {
            int si = _list.IndexFromPoint(e.Location);
            if (si < 0 || si >= _rows.Count) return;
            var row = _rows[si];
            if (row.Tab == null || row.IsHeader || row.IsSeparator) return;

            _strip.ActiveTabIndex = row.OriginalIndex;
            _strip.RaiseTabSelected(new TabEventArgs(row.OriginalIndex, row.Tab));
            SafeClose();
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private void OnSearchChanged(object? sender, EventArgs e)
        {
            string txt = _search.Text;
            // Strip placeholder text if non-empty user input
            if (txt == "Search tabs…") txt = string.Empty;
            ApplyFilter(txt);
        }

        private static void SetPlaceholder(TextBox tb, string placeholder)
        {
            tb.Text      = placeholder;
            tb.ForeColor = SystemColors.GrayText;
            tb.GotFocus  += (s, e) =>
            {
                if (tb.Text == placeholder)
                { tb.Text = ""; tb.ForeColor = tb.Parent?.ForeColor ?? SystemColors.WindowText; }
            };
            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                { tb.Text = placeholder; tb.ForeColor = SystemColors.GrayText; }
            };
        }

        private Point ConstrainToScreen(Point pt)
        {
            var screen = Screen.FromPoint(pt).WorkingArea;
            int x = pt.X;
            int y = pt.Y;
            if (x + Width  > screen.Right)  x = screen.Right  - Width;
            if (y + Height > screen.Bottom) y = pt.Y - Height;   // flip above
            if (x < screen.Left) x = screen.Left;
            if (y < screen.Top)  y = screen.Top;
            return new Point(x, y);
        }

        private void SafeClose()
        {
            if (!IsDisposed) Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { SafeClose(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _search.SelectAll();
        }

        // ── Theme-aware color fallbacks ──────────────────────────────────────

        private static Color ThemeAwareColor(Color? themeColor, Color lightColor)
        {
            if (themeColor.HasValue && themeColor.Value != Color.Empty)
                return themeColor.Value;
            return Sc(lightColor);
        }

        private static Color ThemeAwareGrayText(Color? refColor)
        {
            if (refColor.HasValue && IsDarkBackground(refColor.Value))
                return Color.FromArgb(150, 150, 155);
            return SystemColors.GrayText;
        }

        private static bool IsDarkBackground(Color c) => c.GetBrightness() < 0.5;

        private static Color Sc(Color lightColor)
        {
            return lightColor switch
            {
                var x when x == SystemColors.Window => Color.FromArgb(30, 30, 30),
                var x when x == SystemColors.WindowText => Color.White,
                var x when x == SystemColors.ControlText => Color.White,
                var x when x == SystemColors.GrayText => Color.FromArgb(150, 150, 155),
                var x when x == SystemColors.Highlight => Color.FromArgb(0, 120, 215),
                var x when x == SystemColors.HighlightText => Color.White,
                var x when x == SystemColors.Control => Color.FromArgb(45, 45, 48),
                var x when x == SystemColors.ControlDark => Color.FromArgb(70, 70, 75),
                var x when x == SystemColors.ControlLight => Color.FromArgb(70, 70, 75),
                var x when x == SystemColors.ControlLightLight => Color.FromArgb(60, 60, 65),
                var x when x == SystemColors.ActiveCaption => Color.FromArgb(45, 45, 48),
                var x when x == SystemColors.Info => Color.FromArgb(50, 50, 55),
                var x when x == SystemColors.InfoText => Color.White,
                _ => lightColor
            };
        }
    }
}
