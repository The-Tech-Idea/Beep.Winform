// BeepCommandPalettePopup.cs
// Combined command palette (Ctrl+Shift+P) and Go-to-File (Ctrl+P) popup.
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
    /// <summary>Selects the data source shown in <see cref="BeepCommandPalettePopup"/>.</summary>
    public enum CommandPaletteMode
    {
        /// <summary>Fuzzy search over all registered commands.</summary>
        Commands,

        /// <summary>Fuzzy search over currently open document tabs.</summary>
        GoToFile
    }

    /// <summary>
    /// Floating popup for the command palette (Ctrl+Shift+P) and Go-to-File (Ctrl+P) modes.
    /// Show via <see cref="BeepDocumentHost.ShowCommandPalette"/>.
    /// </summary>
    internal sealed class BeepCommandPalettePopup : Form
    {
        // ── Controls ─────────────────────────────────────────────────────────

        private readonly TextBox _search;
        private readonly Panel   _listHost;

        // ── Data ─────────────────────────────────────────────────────────────

        private readonly BeepCommandRegistry              _registry;
        private readonly IReadOnlyList<BeepDocumentTab>   _tabs;
        private readonly IBeepTheme?                      _theme;
        private readonly CommandPaletteMode               _mode;

        private List<BeepCommandEntry> _filteredCmds = new();
        private List<BeepDocumentTab>  _filteredTabs = new();
        private int                    _selectedIndex;

        // ── Results ───────────────────────────────────────────────────────────

        /// <summary>The document tab id to activate (GoToFile mode), or null if cancelled.</summary>
        internal string? SelectedDocumentId { get; private set; }

        /// <summary>When true, open the document in a new split pane (GoToFile + Ctrl+Enter).</summary>
        internal bool OpenInSplit { get; private set; }

        /// <summary>The command chosen (Commands mode), or null if cancelled.</summary>
        internal BeepCommandEntry? SelectedCommand { get; private set; }

        // ── Layout constants ──────────────────────────────────────────────────

        private const int PopupWidth  = 520;
        private const int PopupHeight = 400;
        private const int ModeH       = 22;
        private const int SearchH     = 38;
        private const int Pad         = 10;
        private const int ItemH       = 36;
        private const int MaxVisible  = 8;

        // ════════════════════════════════════════════════════════════════════
        // Constructor
        // ════════════════════════════════════════════════════════════════════

        internal BeepCommandPalettePopup(
            BeepCommandRegistry            registry,
            IReadOnlyList<BeepDocumentTab> tabs,
            IBeepTheme?                    theme,
            CommandPaletteMode             mode,
            Point                          screenPosition)
        {
            _registry = registry;
            _tabs     = tabs;
            _theme    = theme;
            _mode     = mode;

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            StartPosition   = FormStartPosition.Manual;
            Size            = new Size(PopupWidth, PopupHeight);
            Location        = screenPosition;
            KeyPreview      = true;
            BackColor       = ThemeAwareColor(_theme?.PanelBackColor, SystemColors.Window);

            // Mode hint label
            var modeLabel = new Label
            {
                Bounds    = new Rectangle(Pad, Pad, PopupWidth - Pad * 2, ModeH),
                Text      = mode == CommandPaletteMode.Commands
                                ? "Command Palette  (Ctrl+Shift+P)"
                                : "Go to File  (Ctrl+P  ·  Ctrl+Enter = open in split)",
                ForeColor = theme?.SecondaryTextColor ?? ThemeAwareGrayText(_theme?.PanelBackColor),
                Font      = BeepFontManager.GetCachedFont("Segoe UI", 9f, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                AutoSize  = false,
            };

            // Search box
            _search = new TextBox
            {
                Bounds          = new Rectangle(Pad, Pad + ModeH + 2, PopupWidth - Pad * 2, SearchH),
                Anchor          = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle     = BorderStyle.FixedSingle,
                Font            = BeepFontManager.GetCachedFont("Segoe UI", 12f, FontStyle.Regular),
                PlaceholderText = mode == CommandPaletteMode.Commands
                                      ? "Type command name…"
                                      : "Type file name…",
                BackColor       = ThemeAwareColor(_theme?.BackgroundColor, SystemColors.Window),
                ForeColor       = ThemeAwareColor(_theme?.ForeColor, SystemColors.WindowText),
            };

            // Owner-draw list panel
            int listTop = Pad + ModeH + 2 + SearchH + 6;
            _listHost = new Panel
            {
                Bounds    = new Rectangle(Pad, listTop, PopupWidth - Pad * 2, PopupHeight - listTop - Pad),
                Anchor    = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = ThemeAwareColor(_theme?.PanelBackColor, SystemColors.Window),
            };

            Controls.Add(modeLabel);
            Controls.Add(_search);
            Controls.Add(_listHost);

            _search.TextChanged  += (_, _) => RefreshList();
            _listHost.Paint      += OnListPaint;
            _listHost.MouseMove  += OnListMouseMove;
            _listHost.MouseClick += OnListMouseClick;
            KeyDown              += OnKeyDown;
            _search.KeyDown      += OnKeyDown;

            Deactivate += (_, _) => Close();

            RefreshList();
            _search.Focus();
        }

        // ── List refresh ──────────────────────────────────────────────────────

        private void RefreshList()
        {
            string q = _search.Text;
            if (_mode == CommandPaletteMode.Commands)
            {
                _filteredCmds = _registry.FuzzySearch(q).ToList();
                _filteredTabs.Clear();
            }
            else
            {
                _filteredTabs = FuzzyFilterTabs(q);
                _filteredCmds.Clear();
            }
            _selectedIndex = 0;
            _listHost.Invalidate();
        }

        private List<BeepDocumentTab> FuzzyFilterTabs(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return _tabs.OrderByDescending(t => t.IsActive).ToList();

            return _tabs
                .Select(t => (Tab: t, Score: FuzzyScore(t.Title, q)))
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Tab)
                .ToList();
        }

        private static int FuzzyScore(string source, string query)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(query)) return 0;
            if (source.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                return 1000 + (100 - Math.Min(source.Length, 100));
            int ci = source.IndexOf(query, StringComparison.OrdinalIgnoreCase);
            if (ci >= 0) return 500 + (100 - Math.Min(ci, 100));
            int qi = 0, si = 0, gaps = 0;
            string sl = source.ToLowerInvariant(), ql = query.ToLowerInvariant();
            while (si < sl.Length && qi < ql.Length)
            { if (sl[si] == ql[qi]) qi++; else gaps++; si++; }
            return qi < ql.Length ? 0 : Math.Max(1, 200 - gaps);
        }

        private int ResultCount  => _mode == CommandPaletteMode.Commands
                                        ? _filteredCmds.Count
                                        : _filteredTabs.Count;
        private int VisibleCount => Math.Min(ResultCount, MaxVisible);

        // ── Owner-draw list ───────────────────────────────────────────────────

        private void OnListPaint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color back    = ThemeAwareColor(_theme?.PanelBackColor, SystemColors.Window);
            Color fore    = ThemeAwareColor(_theme?.ForeColor, SystemColors.WindowText);
            Color selBack = ThemeAwareColor(_theme?.PrimaryColor, SystemColors.Highlight);
            Color selFore = ThemeAwareColor(_theme?.BackgroundColor, SystemColors.HighlightText);
            Color dim     = ThemeAwareGrayText(_theme?.PanelBackColor);
            Color sep     = _theme?.BorderColor        ?? Color.FromArgb(40, fore);

            var titleFont = BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Regular);
            var subFont   = BeepFontManager.GetCachedFont("Segoe UI",  9f, FontStyle.Regular);

            int count = VisibleCount;
            for (int i = 0; i < count; i++)
            {
                bool sel      = i == _selectedIndex;
                var  itemRect = new Rectangle(0, i * ItemH, _listHost.Width, ItemH);

                using var backBr = new SolidBrush(sel ? selBack : back);
                g.FillRectangle(backBr, itemRect);

                string title, sub = string.Empty;
                if (_mode == CommandPaletteMode.Commands)
                {
                    var cmd = _filteredCmds[i];
                    title = cmd.Title;
                    sub   = string.IsNullOrEmpty(cmd.Shortcut)
                                ? cmd.Category
                                : $"{cmd.Category}  ·  {cmd.Shortcut}";
                }
                else
                {
                    var tab = _filteredTabs[i];
                    title = tab.Title;
                    if (tab.IsActive)   sub = "active";
                    if (tab.IsModified) sub += (sub.Length > 0 ? "  ·  " : "") + "modified";
                }

                using var titleBr = new SolidBrush(sel ? selFore : fore);
                g.DrawString(title, titleFont, titleBr,
                    new RectangleF(12, itemRect.Top + 4, itemRect.Width - 120, 20));

                if (!string.IsNullOrEmpty(sub))
                {
                    using var subBr = new SolidBrush(sel ? Color.FromArgb(200, selFore) : dim);
                    var sz = g.MeasureString(sub, subFont);
                    g.DrawString(sub, subFont, subBr,
                        new PointF(itemRect.Right - sz.Width - 8,
                                   itemRect.Top + (ItemH - sz.Height) / 2));
                }

                if (i < count - 1)
                {
                    using var sepPen = new Pen(sep);
                    g.DrawLine(sepPen, 4, itemRect.Bottom - 1, _listHost.Width - 4, itemRect.Bottom - 1);
                }
            }

            if (ResultCount == 0)
            {
                using var emptyBr = new SolidBrush(dim);
                g.DrawString("No results", titleFont, emptyBr, new PointF(12, 10));
            }
        }

        private void OnListMouseMove(object? sender, MouseEventArgs e)
        {
            int idx = e.Y / ItemH;
            if (idx >= 0 && idx < VisibleCount && idx != _selectedIndex)
            {
                _selectedIndex = idx;
                _listHost.Invalidate();
            }
        }

        private void OnListMouseClick(object? sender, MouseEventArgs e)
        {
            int idx = e.Y / ItemH;
            if (idx >= 0 && idx < VisibleCount)
            {
                _selectedIndex = idx;
                CommitSelection(openInSplit: false);
            }
        }

        // ── Keyboard ──────────────────────────────────────────────────────────

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    Close();
                    break;

                case Keys.Enter:
                    e.Handled = true;
                    CommitSelection(openInSplit: e.Control && _mode == CommandPaletteMode.GoToFile);
                    break;

                case Keys.Down:
                    if (_selectedIndex < VisibleCount - 1)
                    {
                        _selectedIndex++;
                        _listHost.Invalidate();
                    }
                    e.Handled = true;
                    break;

                case Keys.Up:
                    if (_selectedIndex > 0)
                    {
                        _selectedIndex--;
                        _listHost.Invalidate();
                    }
                    e.Handled = true;
                    break;

                case Keys.Tab:
                {
                    int cnt = Math.Max(1, VisibleCount);
                    int dir = e.Shift ? -1 : 1;
                    _selectedIndex = (_selectedIndex + dir + cnt) % cnt;
                    _listHost.Invalidate();
                    e.Handled          = true;
                    e.SuppressKeyPress = true;
                    break;
                }
            }
        }

        private void CommitSelection(bool openInSplit)
        {
            int idx = _selectedIndex;
            if (_mode == CommandPaletteMode.Commands)
            {
                if (idx >= 0 && idx < _filteredCmds.Count)
                {
                    SelectedCommand = _filteredCmds[idx];
                    _registry.RecordUsage(SelectedCommand.Id);
                }
            }
            else
            {
                if (idx >= 0 && idx < _filteredTabs.Count)
                {
                    SelectedDocumentId = _filteredTabs[idx].Id;
                    OpenInSplit        = openInSplit;
                }
            }
            Close();
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
            return TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.GrayText);
        }

        private static bool IsDarkBackground(Color c) => c.GetBrightness() < 0.5;

        private static Color Sc(Color lightColor)
        {
            return TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(lightColor);
        }
    }
}
