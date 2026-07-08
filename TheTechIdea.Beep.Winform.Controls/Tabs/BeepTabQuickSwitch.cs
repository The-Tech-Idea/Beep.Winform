// BeepTabQuickSwitch.cs
// Keyboard-driven tab picker popup for BeepTabs in Documents / Workspace mode.
// Show via BeepTabs.ShowQuickSwitch() or the Ctrl+P shortcut.
// Provides type-to-filter + arrow-key / Tab-key navigation over open tabs
// ordered by Most-Recently-Used (MRU) — mirrors BeepDocumentQuickSwitch.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Tabs
{
    // ── Entry type ────────────────────────────────────────────────────────────

    /// <summary>
    /// Minimal display record for one tab in the quick-switch list.
    /// Produced by BeepTabs and consumed exclusively by BeepTabQuickSwitch.
    /// </summary>
    internal readonly struct BeepTabQuickSwitchEntry
    {
        internal int    TabIndex   { get; init; }
        internal string Title      { get; init; }
        internal bool   IsDirty    { get; init; }
        internal bool   IsPinned   { get; init; }
        internal bool   IsSelected { get; init; }

        public override string ToString() => Title;
    }

    // ── Popup form ────────────────────────────────────────────────────────────

    /// <summary>
    /// Lightweight floating popup for quickly switching between open tabs.
    /// Tabs are presented in MRU order with the second entry pre-selected
    /// (the previous tab, matching common IDE behaviour).
    /// </summary>
    internal sealed class BeepTabQuickSwitch : Form
    {
        // ── Controls ──────────────────────────────────────────────────────────

        private readonly TextBox _search;
        private readonly ListBox _list;
        private readonly Panel   _frame;

        // ── Data ──────────────────────────────────────────────────────────────

        private readonly IReadOnlyList<BeepTabQuickSwitchEntry> _allEntries;
        private readonly IBeepTheme?                             _currentTheme;
        private List<BeepTabQuickSwitchEntry> _filtered = new();

        // ── Result ────────────────────────────────────────────────────────────

        /// <summary>
        /// The tab index chosen by the user, or <c>-1</c> if cancelled.
        /// </summary>
        internal int SelectedTabIndex { get; private set; } = -1;

        // ── Layout constants (DPI-scaled at construction) ─────────────────────

        private readonly int PopupWidth;
        private readonly int PopupHeight;
        private readonly int SearchH;
        private readonly int Pad;
        private readonly int ItemH;

        // ═════════════════════════════════════════════════════════════════════
        // Constructor
        // ═════════════════════════════════════════════════════════════════════

        /// <param name="entries">
        ///   Tabs in MRU order (most recent first). The currently active tab
        ///   should be at index 0; the popup pre-selects index 1 so the user
        ///   reaches the previous tab immediately.
        /// </param>
        /// <param name="theme">Optional theme for colours.</param>
        /// <param name="screenPosition">Top-left corner in screen coordinates.</param>
        internal BeepTabQuickSwitch(
            IReadOnlyList<BeepTabQuickSwitchEntry> entries,
            IBeepTheme?                             theme,
            Point                                   screenPosition)
        {
            _allEntries   = entries;
            _currentTheme = theme;

            // DPI-scale layout constants using a temporary control for DPI context
            var dpi = this;
            PopupWidth  = DpiScalingHelper.ScaleValue(440, dpi);
            PopupHeight = DpiScalingHelper.ScaleValue(340, dpi);
            SearchH     = DpiScalingHelper.ScaleValue(36, dpi);
            Pad         = DpiScalingHelper.ScaleValue(8, dpi);
            ItemH       = DpiScalingHelper.ScaleValue(32, dpi);

            // ── Form setup ────────────────────────────────────────────────────
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            StartPosition   = FormStartPosition.Manual;
            Size            = new Size(PopupWidth, PopupHeight);
            Location        = screenPosition;
            KeyPreview      = true;
            BackColor       = ThemeColor(_currentTheme?.PanelBackColor, SystemColors.Window);

            // ── Outer frame ───────────────────────────────────────────────────
            _frame = new Panel
            {
                Dock      = DockStyle.Fill,
                Padding   = new Padding(Pad),
                BackColor = ThemeColor(_currentTheme?.PanelBackColor, SystemColors.Window),
            };

            // ── Search box ────────────────────────────────────────────────────
            _search = new TextBox
            {
                Bounds          = new Rectangle(Pad, Pad, PopupWidth - Pad * 2, SearchH),
                Anchor          = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle     = BorderStyle.FixedSingle,
                Font            = BeepThemesManager.ToFont(theme?.BodyMedium) ?? SystemFonts.DefaultFont,
                PlaceholderText = "Type to filter tabs…",
                BackColor       = ThemeColor(_currentTheme?.BackgroundColor, SystemColors.Window),
                ForeColor       = ThemeColor(_currentTheme?.ForeColor,       SystemColors.WindowText),
            };

            // ── Results list ──────────────────────────────────────────────────
            _list = new ListBox
            {
                Bounds              = new Rectangle(Pad, Pad + SearchH + 4,
                                          PopupWidth  - Pad * 2,
                                          PopupHeight - SearchH - Pad * 3 - 4),
                Anchor              = AnchorStyles.Top | AnchorStyles.Bottom
                                    | AnchorStyles.Left | AnchorStyles.Right,
                DrawMode            = DrawMode.OwnerDrawFixed,
                ItemHeight          = ItemH,
                BorderStyle         = BorderStyle.None,
                BackColor           = ThemeColor(_currentTheme?.PanelBackColor, SystemColors.Window),
                ForeColor           = ThemeColor(_currentTheme?.ForeColor,       SystemColors.WindowText),
                ScrollAlwaysVisible = false,
                IntegralHeight      = false,
            };

            _frame.Controls.Add(_search);
            _frame.Controls.Add(_list);
            Controls.Add(_frame);

            // ── Wire events ───────────────────────────────────────────────────
            _search.TextChanged        += OnSearchChanged;
            _list.DrawItem             += OnDrawItem;
            _list.MouseDoubleClick     += (_, _) => CommitSelection();

            KeyDown          += OnKeyDown;
            _search.KeyDown  += OnKeyDown;
            _list.KeyDown    += OnKeyDown;
            Deactivate       += (_, _) => Close();

            // ── Populate: pre-select the second entry (previous MRU tab) ─────
            PopulateList(string.Empty, preselectPrevious: true);
            _search.Focus();
        }

        // ── Search filter ──────────────────────────────────────────────────────

        private void OnSearchChanged(object? sender, EventArgs e)
        {
            PopulateList(_search.Text, preselectPrevious: false);
        }

        private void PopulateList(string filter, bool preselectPrevious)
        {
            _filtered = string.IsNullOrWhiteSpace(filter)
                ? _allEntries.ToList()
                : _allEntries.Where(e =>
                      e.Title.Contains(filter, StringComparison.OrdinalIgnoreCase))
                  .ToList();

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var entry in _filtered)
                _list.Items.Add(entry.Title);
            _list.EndUpdate();

            if (_filtered.Count == 0) return;

            // Without a filter, pre-select index 1 (previous tab); with a
            // filter, pre-select the first match.
            int sel = (preselectPrevious && _filtered.Count > 1) ? 1 : 0;
            _list.SelectedIndex = sel;
        }

        // ── Keyboard handling ──────────────────────────────────────────────────

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    SelectedTabIndex = -1;
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
                    // Tab (with or without Shift) cycles through the list.
                    if (_list.Items.Count > 0)
                    {
                        int n = e.Shift
                            ? (_list.SelectedIndex - 1 + _list.Items.Count) % _list.Items.Count
                            : (_list.SelectedIndex + 1) % _list.Items.Count;
                        _list.SelectedIndex = n;
                    }
                    e.Handled        = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void CommitSelection()
        {
            int idx = _list.SelectedIndex;
            if (idx >= 0 && idx < _filtered.Count)
                SelectedTabIndex = _filtered[idx].TabIndex;
            Close();
        }

        // ── Owner-draw list items ──────────────────────────────────────────────

        private void OnDrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _filtered.Count) return;

            BeepTabQuickSwitchEntry entry = _filtered[e.Index];
            bool sel = (e.State & DrawItemState.Selected) != 0;

            Color backCol = sel
                ? ThemeColor(_currentTheme?.PrimaryColor, SystemColors.Highlight)
                : ThemeColor(_currentTheme?.PanelBackColor, SystemColors.Window);
            Color foreCol = sel
                ? ThemeColor(_currentTheme?.BackgroundColor, SystemColors.HighlightText)
                : ThemeColor(_currentTheme?.ForeColor, SystemColors.WindowText);

            using var back = new SolidBrush(backCol);
            e.Graphics.FillRectangle(back, e.Bounds);

            // Left indicator bar for the currently open tab
            if (entry.IsSelected)
            {
                Color barCol = _currentTheme?.PrimaryColor ?? Color.DodgerBlue;
                using var bar = new SolidBrush(barCol);
                e.Graphics.FillRectangle(bar,
                    e.Bounds.Left, e.Bounds.Top + 4, 3, e.Bounds.Height - 8);
            }

            int x = e.Bounds.Left + 10;

            // Dirty dot
            if (entry.IsDirty)
            {
                using var dot = new SolidBrush(Color.OrangeRed);
                int dotY = e.Bounds.Top + (e.Bounds.Height - 8) / 2;
                e.Graphics.FillEllipse(dot, x, dotY, 8, 8);
                x += 13;
            }

            // Pin marker (small filled circle in accent colour)
            if (entry.IsPinned)
            {
                Color pinCol = sel
                    ? Color.FromArgb(200, foreCol)
                    : (_currentTheme?.PrimaryColor ?? Color.SteelBlue);
                using var pin = new SolidBrush(pinCol);
                int pinY = e.Bounds.Top + (e.Bounds.Height - 8) / 2;
                e.Graphics.FillEllipse(pin, x, pinY, 6, 8);
                x += 11;
            }

            // Title text
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var titleRect = new Rectangle(x, e.Bounds.Top, e.Bounds.Width - x - Pad, e.Bounds.Height);
            using var fmt = new StringFormat
            {
                Alignment     = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
                FormatFlags   = StringFormatFlags.NoWrap,
            };
            using var titleBrush = new SolidBrush(foreCol);
            e.Graphics.DrawString(entry.Title, _list.Font ?? SystemFonts.DefaultFont, titleBrush, titleRect, fmt);
        }

        // ── Colour helpers ─────────────────────────────────────────────────────

        private static Color ThemeColor(Color? themed, Color fallback)
            => themed.HasValue && themed.Value != Color.Empty ? themed.Value : fallback;
    }
}
