using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.CommandPalette
{
    /// <summary>
    /// A keyboard-driven command palette dialog that supports category chip filtering,
    /// keyboard-shortcut hints, SVG icons per action, and a "recent items" section.
    /// </summary>
    public class BeepCommandPaletteDialog : BeepiFormPro
    {
        // ─── Layout ───────────────────────────────────────────────────────────
        private readonly BeepPanel _headerPanel;   // search box
        private readonly BeepPanel _chipPanel;     // category filter chips
        private readonly BeepPanel _bodyPanel;     // list area
        private readonly BeepPanel _footerPanel;   // hint bar

        // ─── Controls ─────────────────────────────────────────────────────────
        private readonly BeepTextBox _searchBox;
        private readonly BeepMultiChipGroup _categoryChips;
        private readonly BeepListBox _list;
        private readonly BeepLabel _hintLabel;
        private readonly BeepLabel _emptyLabel;

        // ─── State ────────────────────────────────────────────────────────────
        private readonly List<CommandAction> _actions = new();
        private List<CommandAction> _filtered = new();
        private string _activeCategory = string.Empty;

        // Recent items: stores MRU list (most-recent-first)
        private static readonly List<string> _recentTexts = new();
        private const int MaxRecentItems = 8;

        // ─────────────────────────────────────────────────────────────────────

        public BeepCommandPaletteDialog()
        {
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            Width = 560;
            Height = 480;

            // ── Header panel (search) ───────────────────────────────────────
            _headerPanel = new BeepPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding = new Padding(8, 8, 8, 4)
            };
            _searchBox = new BeepTextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Type a command…",
                UseThemeColors = true
            };
            _headerPanel.Controls.Add(_searchBox);

            // ── Chip panel (category filter) ───────────────────────────────
            _chipPanel = new BeepPanel
            {
                Dock = DockStyle.Top,
                Height = 42,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding = new Padding(6, 4, 6, 4)
            };
            _categoryChips = new BeepMultiChipGroup
            {
                Dock = DockStyle.Fill,
                ChipStyle = ChipStyle.Pill,
                SelectionMode = ChipSelectionMode.Single,
                ShowCheckmark = false,
                ShowCloseButton = false,
                UseThemeColors = true
            };
            _chipPanel.Controls.Add(_categoryChips);

            // ── Body panel (list) ──────────────────────────────────────────
            _bodyPanel = new BeepPanel
            {
                Dock = DockStyle.Fill,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding = new Padding(4)
            };
            _list = new BeepListBox
            {
                Dock = DockStyle.Fill,
                UseThemeColors = true,
                ShowImage = true   // renders ImagePath column from SimpleItem
            };

            _emptyLabel = new BeepLabel
            {
                Dock = DockStyle.Fill,
                UseThemeColors = true,
                IsFrameless = true,
                Text = "No matching commands",
                Visible = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _bodyPanel.Controls.Add(_list);
            _bodyPanel.Controls.Add(_emptyLabel);

            // ── Footer panel (keyboard hint bar) ──────────────────────────
            _footerPanel = new BeepPanel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding = new Padding(8, 4, 8, 4)
            };
            _hintLabel = new BeepLabel
            {
                Dock = DockStyle.Fill,
                Text = "↑↓ navigate   ↵ execute   Esc close",
                UseThemeColors = true
            };
            _footerPanel.Controls.Add(_hintLabel);

            // ── Wire up events ─────────────────────────────────────────────
            _searchBox.TextChanged += (s, e) => Rebind();
            _list.DoubleClick += (s, e) => ExecuteSelected();
            _list.SelectedItemChanged += (s, e) => UpdateHintForSelection();
            _categoryChips.SelectionChanged += OnCategoryChanged;

            // ── Add controls in Z-order (bottom → top for Dock) ───────────
            Controls.Add(_bodyPanel);
            Controls.Add(_chipPanel);
            Controls.Add(_headerPanel);
            Controls.Add(_footerPanel);

            ApplyTheme();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Set (replace) the full action catalogue.</summary>
        public void SetActions(IEnumerable<CommandAction> actions)
        {
            _actions.Clear();
            _actions.AddRange(actions ?? Enumerable.Empty<CommandAction>());
            RebuildCategoryChips();
            Rebind();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Keyboard handling
        // ─────────────────────────────────────────────────────────────────────

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    ExecuteSelected();
                    return true;

                case Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return true;

                case Keys.Down:
                    _list.SelectedIndex = Math.Min(
                        (_list.SelectedIndex < 0 ? -1 : _list.SelectedIndex) + 1,
                        _filtered.Count - 1);
                    return true;

                case Keys.Up:
                    _list.SelectedIndex = Math.Max(_list.SelectedIndex - 1, 0);
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Theme propagation
        // ─────────────────────────────────────────────────────────────────────

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;

            _headerPanel.Theme = Theme;
            _chipPanel.Theme = Theme;
            _bodyPanel.Theme = Theme;
            _footerPanel.Theme = Theme;
            _searchBox.Theme = Theme;
            _categoryChips.Theme = Theme;
            _list.Theme = Theme;
            _hintLabel.Theme = Theme;

            if (_currentTheme != null && _hintLabel != null)
            {
                _hintLabel.ForeColor = _currentTheme.ForeColor != Color.Empty
                    ? Color.FromArgb(140, _currentTheme.ForeColor)
                    : ColorUtils.MapSystemColor(SystemColors.GrayText);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────

        private void RebuildCategoryChips()
        {
            // "All" chip + one chip per distinct category
            var categories = _actions
                .Select(a => a.Category?.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            var items = new BindingList<SimpleItem>();

            var allItem = new SimpleItem { Text = "All", Value = string.Empty };
            items.Add(allItem);

            foreach (var cat in categories)
                items.Add(new SimpleItem { Text = cat, Value = cat });

            _categoryChips.ListItems = items;
            _categoryChips.SelectedIndex = 0;   // select "All" by default
            _activeCategory = string.Empty;

            // Only show chip panel when there are actual categories
            _chipPanel.Visible = categories.Count > 0;
        }

        private void OnCategoryChanged(object? sender, ChipSelectionChangedEventArgs e)
        {
            _activeCategory = (e.SelectedItem?.Value as string) ?? string.Empty;
            Rebind();
        }

        private void Rebind()
        {
            string query = _searchBox.Text ?? string.Empty;

            IEnumerable<CommandAction> source = _actions;

            // Category filter
            if (!string.IsNullOrEmpty(_activeCategory))
                source = source.Where(a => string.Equals(
                    a.Category, _activeCategory, StringComparison.OrdinalIgnoreCase));

            // Search filter
            if (!string.IsNullOrWhiteSpace(query))
                source = source.Where(a =>
                    a.Text.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(a.Category) && a.Category.Contains(query, StringComparison.OrdinalIgnoreCase)));

            // When search is empty and no category active: pin favorites then recent items.
            List<CommandAction> result;
            if (string.IsNullOrWhiteSpace(query) && string.IsNullOrEmpty(_activeCategory) && _recentTexts.Count > 0)
            {
                var recentSet = new HashSet<string>(_recentTexts, StringComparer.OrdinalIgnoreCase);
                var recentIndex = _recentTexts
                    .Select((text, idx) => new { text, idx })
                    .ToDictionary(x => x.text, x => x.idx, StringComparer.OrdinalIgnoreCase);

                var favorites = source.Where(a => a.IsFavorite).ToList();
                var nonFavorites = source.Where(a => !a.IsFavorite).ToList();

                var recents = nonFavorites
                    .Where(a => recentSet.Contains(a.Text))
                    .OrderBy(a => recentIndex.TryGetValue(a.Text, out int idx) ? idx : int.MaxValue)
                    .ToList();

                var rest = nonFavorites.Where(a => !recentSet.Contains(a.Text)).ToList();
                result = favorites.Concat(recents).Concat(rest).ToList();
            }
            else
            {
                // Keep favorites near top during search/filter as well.
                result = source
                    .OrderByDescending(a => a.IsFavorite)
                    .ThenBy(a => a.Text, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            _filtered = result;

            _list.ListItems = new BindingList<SimpleItem>(_filtered.Select(a => new SimpleItem
            {
                Text = a.Text,
                ImagePath = a.IconPath,
                SubText = a.ShortcutText,   // right-aligned shortcut hint via SubText
                Value = a
            }).ToList());

            _emptyLabel.Visible = _filtered.Count == 0;
            _list.Visible = _filtered.Count > 0;

            if (_filtered.Count > 0)
            {
                _list.SelectedIndex = 0;
            }

            UpdateHintForSelection();
        }

        private void ExecuteSelected()
        {
            if (_list.SelectedItem?.Value is CommandAction action)
            {
                // Track recent
                _recentTexts.RemoveAll(t => string.Equals(t, action.Text, StringComparison.OrdinalIgnoreCase));
                _recentTexts.Insert(0, action.Text);
                if (_recentTexts.Count > MaxRecentItems)
                    _recentTexts.RemoveRange(MaxRecentItems, _recentTexts.Count - MaxRecentItems);

                action.Action?.Invoke();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void UpdateHintForSelection()
        {
            var selected = _list.SelectedItem?.Value as CommandAction;
            if (selected == null)
            {
                _hintLabel.Text = "↑↓ navigate   Tab move focus   ↵ execute   Esc close";
                return;
            }

            if (!string.IsNullOrWhiteSpace(selected.ShortcutText))
            {
                _hintLabel.Text = $"↑↓ navigate   ↵ execute   Esc close   Shortcut: {selected.ShortcutText}";
            }
            else
            {
                _hintLabel.Text = "↑↓ navigate   ↵ execute   Esc close";
            }
        }
    }
}

