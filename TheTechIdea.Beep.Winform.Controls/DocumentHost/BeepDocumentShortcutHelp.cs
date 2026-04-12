// BeepDocumentShortcutHelp.cs
// Searchable keyboard shortcut help dialog for BeepDocumentHost.
// Shows all registered commands grouped by category with export support.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Searchable, category-grouped display of all keyboard shortcuts registered
    /// in <see cref="BeepCommandRegistry"/>.  Opened via Ctrl+K H or the command palette.
    /// </summary>
    internal sealed class BeepDocumentShortcutHelp : Form
    {
        // ── Controls ─────────────────────────────────────────────────────────

        private readonly TextBox _search;
        private readonly Panel   _listHost;
        private readonly Button  _btnExportJson;
        private readonly Button  _btnExportCsv;

        // ── Data ─────────────────────────────────────────────────────────────

        private readonly BeepCommandRegistry _registry;
        private readonly IBeepTheme?         _theme;

        private List<(string Category, List<BeepCommandEntry> Commands)> _groups = new();

        // ── Layout constants ──────────────────────────────────────────────────

        private const int PopupWidth  = 560;
        private const int PopupHeight = 500;
        private const int TitleH      = 26;
        private const int SearchH     = 36;
        private const int Pad         = 10;
        private const int ItemH       = 30;
        private const int CatH        = 22;
        private const int BtnH        = 28;

        internal BeepDocumentShortcutHelp(
            BeepCommandRegistry registry,
            IBeepTheme?         theme,
            Point               screenPosition)
        {
            _registry = registry;
            _theme    = theme;

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            StartPosition   = FormStartPosition.Manual;
            Size            = new Size(PopupWidth, PopupHeight);
            Location        = screenPosition;
            KeyPreview      = true;
            BackColor       = theme?.PanelBackColor ?? SystemColors.Window;

            // Title bar
            var titleLabel = new Label
            {
                Bounds    = new Rectangle(Pad, Pad, PopupWidth - 80, TitleH),
                Text      = "Keyboard Shortcuts",
                Font      = BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = theme?.ForeColor ?? SystemColors.WindowText,
                BackColor = Color.Transparent,
                AutoSize  = false,
            };

            var btnClose = new Button
            {
                Bounds     = new Rectangle(PopupWidth - 36, Pad, 26, TitleH),
                Text       = "✕",
                FlatStyle  = FlatStyle.Flat,
                Font       = BeepFontManager.GetCachedFont("Segoe UI", 9f, FontStyle.Regular),
                ForeColor  = theme?.ForeColor ?? SystemColors.WindowText,
                BackColor  = Color.Transparent,
                DialogResult = DialogResult.Cancel,
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (_, _) => Close();

            // Search
            _search = new TextBox
            {
                Bounds          = new Rectangle(Pad, Pad + TitleH + 4, PopupWidth - Pad * 2, SearchH),
                BorderStyle     = BorderStyle.FixedSingle,
                Font            = BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Regular),
                PlaceholderText = "Search shortcuts…",
                BackColor       = theme?.BackgroundColor ?? SystemColors.Window,
                ForeColor       = theme?.ForeColor       ?? SystemColors.WindowText,
            };

            // Scrollable list panel
            int listTop = Pad + TitleH + 4 + SearchH + 6;
            int listH   = PopupHeight - listTop - Pad - BtnH - 8;
            _listHost = new Panel
            {
                Bounds     = new Rectangle(Pad, listTop, PopupWidth - Pad * 2, listH),
                AutoScroll = true,
                BackColor  = theme?.PanelBackColor ?? SystemColors.Window,
            };

            // Export buttons
            int btnY = PopupHeight - Pad - BtnH;
            _btnExportJson = new Button
            {
                Bounds     = new Rectangle(Pad, btnY, 110, BtnH),
                Text       = "Export JSON",
                FlatStyle  = FlatStyle.Flat,
                Font       = BeepFontManager.GetCachedFont("Segoe UI", 9f, FontStyle.Regular),
                BackColor  = theme?.PrimaryColor ?? SystemColors.Highlight,
                ForeColor  = theme?.BackgroundColor ?? SystemColors.HighlightText,
            };
            _btnExportJson.FlatAppearance.BorderSize = 0;

            _btnExportCsv = new Button
            {
                Bounds     = new Rectangle(Pad + 120, btnY, 110, BtnH),
                Text       = "Export CSV",
                FlatStyle  = FlatStyle.Flat,
                Font       = BeepFontManager.GetCachedFont("Segoe UI", 9f, FontStyle.Regular),
                BackColor  = theme?.PrimaryColor ?? SystemColors.Highlight,
                ForeColor  = theme?.BackgroundColor ?? SystemColors.HighlightText,
            };
            _btnExportCsv.FlatAppearance.BorderSize = 0;

            Controls.Add(titleLabel);
            Controls.Add(btnClose);
            Controls.Add(_search);
            Controls.Add(_listHost);
            Controls.Add(_btnExportJson);
            Controls.Add(_btnExportCsv);

            _search.TextChanged      += (_, _) => RefreshList();
            _btnExportJson.Click     += OnExportJson;
            _btnExportCsv.Click      += OnExportCsv;
            KeyDown                  += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            RefreshList();
            _search.Focus();
        }

        // ── List refresh ──────────────────────────────────────────────────────

        private void RefreshList()
        {
            string q   = _search.Text.Trim();
            var    all = _registry.GetAll();

            IEnumerable<BeepCommandEntry> source = string.IsNullOrEmpty(q)
                ? all
                : all.Where(c =>
                    c.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    (c.Shortcut ?? string.Empty).Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    c.Category.Contains(q, StringComparison.OrdinalIgnoreCase));

            _groups = source
                .GroupBy(c => c.Category)
                .OrderBy(g => g.Key)
                .Select(g => (g.Key, g.OrderBy(c => c.Title).ToList()))
                .ToList();

            RebuildListPanel();
        }

        private void RebuildListPanel()
        {
            _listHost.SuspendLayout();
            _listHost.Controls.Clear();

            Color fore    = _theme?.ForeColor          ?? SystemColors.WindowText;
            Color dim     = _theme?.SecondaryTextColor ?? SystemColors.GrayText;
            Color catBack = _theme?.PrimaryColor       ?? SystemColors.Highlight;
            Color catFore = _theme?.BackgroundColor    ?? SystemColors.HighlightText;
            Color accent  = _theme?.PrimaryColor       ?? SystemColors.Highlight;
            Color itemBk  = _theme?.PanelBackColor     ?? SystemColors.Window;

            var catFont  = BeepFontManager.GetCachedFont("Segoe UI",  9f, FontStyle.Bold);
            var nameFont = BeepFontManager.GetCachedFont("Segoe UI", 10f, FontStyle.Regular);
            var kbFont   = BeepFontManager.GetCachedFont("Segoe UI",  9f, FontStyle.Regular);

            int y = 0;
            int w = _listHost.ClientSize.Width;

            foreach (var (cat, cmds) in _groups)
            {
                // Category header
                var catLbl = new Label
                {
                    Bounds    = new Rectangle(0, y, w, CatH),
                    Text      = "  " + cat,
                    Font      = catFont,
                    BackColor = Color.FromArgb(30, accent),
                    ForeColor = fore,
                    AutoSize  = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                };
                _listHost.Controls.Add(catLbl);
                y += CatH;

                foreach (var cmd in cmds)
                {
                    var itemPnl = new Panel
                    {
                        Bounds    = new Rectangle(0, y, w, ItemH),
                        BackColor = itemBk,
                    };

                    var nameLbl = new Label
                    {
                        Bounds    = new Rectangle(8, 0, w - 160, ItemH),
                        Text      = cmd.Title,
                        Font      = nameFont,
                        ForeColor = fore,
                        AutoSize  = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        BackColor = Color.Transparent,
                    };

                    var kbLbl = new Label
                    {
                        Bounds    = new Rectangle(w - 155, 0, 148, ItemH),
                        Text      = cmd.Shortcut ?? string.Empty,
                        Font      = kbFont,
                        ForeColor = string.IsNullOrEmpty(cmd.Shortcut) ? dim : accent,
                        AutoSize  = false,
                        TextAlign = ContentAlignment.MiddleRight,
                        BackColor = Color.Transparent,
                    };

                    itemPnl.Controls.Add(nameLbl);
                    itemPnl.Controls.Add(kbLbl);

                    // Separator
                    itemPnl.Paint += (s, e) =>
                    {
                        using var pen = new Pen(Color.FromArgb(20, fore));
                        e.Graphics.DrawLine(pen, 0, ItemH - 1, w, ItemH - 1);
                    };

                    _listHost.Controls.Add(itemPnl);
                    y += ItemH;
                }
            }

            if (_groups.Count == 0)
            {
                var emptyLbl = new Label
                {
                    Bounds    = new Rectangle(0, 0, _listHost.Width, 40),
                    Text      = "No shortcuts match your search.",
                    Font      = nameFont,
                    ForeColor = dim,
                    AutoSize  = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent,
                };
                _listHost.Controls.Add(emptyLbl);
            }

            // Set the virtual height so AutoScroll works
            _listHost.AutoScrollMinSize = new Size(0, y);
            _listHost.ResumeLayout(true);
        }

        // ── Export ────────────────────────────────────────────────────────────

        private void OnExportJson(object? sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Title      = "Export Shortcuts",
                Filter     = "JSON files (*.json)|*.json",
                FileName   = "beep-shortcuts.json",
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var sb = new StringBuilder();
            sb.AppendLine("[");
            var all = _registry.GetAll();
            for (int i = 0; i < all.Count; i++)
            {
                var c = all[i];
                sb.AppendLine("  {");
                sb.AppendLine($"    \"id\": \"{Escape(c.Id)}\",");
                sb.AppendLine($"    \"title\": \"{Escape(c.Title)}\",");
                sb.AppendLine($"    \"category\": \"{Escape(c.Category)}\",");
                sb.AppendLine($"    \"shortcut\": \"{Escape(c.Shortcut ?? string.Empty)}\"");
                sb.Append("  }");
                sb.AppendLine(i < all.Count - 1 ? "," : string.Empty);
            }
            sb.AppendLine("]");
            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
        }

        private void OnExportCsv(object? sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Title    = "Export Shortcuts",
                Filter   = "CSV files (*.csv)|*.csv",
                FileName = "beep-shortcuts.csv",
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var sb = new StringBuilder();
            sb.AppendLine("Id,Title,Category,Shortcut");
            foreach (var c in _registry.GetAll())
                sb.AppendLine($"\"{c.Id}\",\"{c.Title}\",\"{c.Category}\",\"{c.Shortcut ?? string.Empty}\"");
            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
        }

        private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
