using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Helpers;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Ctrl+Tab quick-switcher overlay for the Beep docking system.
    /// Lists all open docked panels in MRU order with keyboard/mouse selection,
    /// search-as-you-type filtering, and themed chrome matching the docking palette.
    /// </summary>
    internal sealed class BeepDockingNavigator : Form
    {
        private readonly TextBox _search;
        private readonly ListBox _list;
        private readonly Panel _frame;
        private readonly List<DockPanel> _allPanels;
        private readonly DockingThemeColors _colors;
        private List<DockPanel> _filtered = new();

        private const int PopupWidth = 440;
        private const int PopupHeight = 340;
        private const int SearchH = 36;
        private const int Pad = 8;
        private const int ItemH = 32;

        /// <summary>Key of the panel selected by the user, or null if cancelled.</summary>
        public string SelectedPanelKey { get; private set; }

        internal void SelectNext()
        {
            if (_list.Items.Count > 0)
            {
                int n = (_list.SelectedIndex + 1) % _list.Items.Count;
                _list.SelectedIndex = n;
            }
        }

        internal void SelectPrevious()
        {
            if (_list.Items.Count > 0)
            {
                int n = (_list.SelectedIndex - 1 + _list.Items.Count) % _list.Items.Count;
                _list.SelectedIndex = n;
            }
        }

        internal void Cancel()
        {
            SelectedPanelKey = null;
            Close();
        }

        internal BeepDockingNavigator(
            IReadOnlyList<DockPanel> panels,
            DockingThemeColors colors,
            Point screenCenter)
        {
            _allPanels = panels.ToList();
            _colors = colors ?? DockingThemeColors.Default;

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Size = new Size(PopupWidth, PopupHeight);
            Location = new Point(
                screenCenter.X - PopupWidth / 2,
                screenCenter.Y - PopupHeight / 2);
            KeyPreview = true;
            BackColor = _colors.PanelBackColor;

            _frame = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(Pad),
                BackColor = _colors.PanelBackColor,
            };

            _search = new TextBox
            {
                Bounds = new Rectangle(Pad, Pad, PopupWidth - Pad * 2, SearchH),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.FixedSingle,
                Font = BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Regular),
                PlaceholderText = "Type to filter panels...",
                BackColor = _colors.PanelBackColor,
                ForeColor = _colors.PanelForeColor,
            };

            _list = new ListBox
            {
                Bounds = new Rectangle(Pad, Pad + SearchH + 4,
                    PopupWidth - Pad * 2,
                    PopupHeight - SearchH - Pad * 3 - 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                       | AnchorStyles.Left | AnchorStyles.Right,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = ItemH,
                BorderStyle = BorderStyle.None,
                BackColor = _colors.PanelBackColor,
                ForeColor = _colors.PanelForeColor,
                ScrollAlwaysVisible = false,
                IntegralHeight = false,
            };

            _frame.Controls.Add(_search);
            _frame.Controls.Add(_list);
            Controls.Add(_frame);

            _search.TextChanged += OnSearchChanged;
            _list.DrawItem += OnDrawItem;
            _list.MouseDoubleClick += (_, _) => CommitSelection();

            KeyDown += OnKeyDown;
            _search.KeyDown += OnKeyDown;
            _list.KeyDown += OnKeyDown;
            Deactivate += (_, _) => Close();

            PopulateList(string.Empty, preselectPrevious: true);
            _search.Focus();
        }

        private void OnSearchChanged(object sender, EventArgs e)
        {
            PopulateList(_search.Text, preselectPrevious: false);
        }

        private void PopulateList(string filter, bool preselectPrevious)
        {
            _filtered = string.IsNullOrWhiteSpace(filter)
                ? _allPanels.ToList()
                : _allPanels.Where(p =>
                      (p.Title ?? string.Empty).Contains(filter, StringComparison.OrdinalIgnoreCase))
                  .ToList();

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var panel in _filtered)
                _list.Items.Add(panel.Title);
            _list.EndUpdate();

            if (_filtered.Count == 0) return;

            int sel = (preselectPrevious && _filtered.Count > 1) ? 1 : 0;
            _list.SelectedIndex = sel;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    SelectedPanelKey = null;
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
                SelectedPanelKey = _filtered[idx].Key;
            Close();
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _filtered.Count) return;

            var panel = _filtered[e.Index];
            bool sel = (e.State & DrawItemState.Selected) != 0;

            Color backCol = sel ? _colors.ActiveTabBackColor : _colors.PanelBackColor;
            Color foreCol = sel ? _colors.ActiveTabForeColor : _colors.PanelForeColor;

            using var back = new SolidBrush(backCol);
            e.Graphics.FillRectangle(back, e.Bounds);

            // Active-tab indicator bar on the current panel (first in MRU order)
            if (e.Index == 0)
            {
                using var bar = new SolidBrush(_colors.ActiveTabBackColor);
                e.Graphics.FillRectangle(bar,
                    e.Bounds.Left, e.Bounds.Top + 4, 3, e.Bounds.Height - 8);
            }

            int x = e.Bounds.Left + 10;

            // Dirty dot
            if (panel.IsDirty)
            {
                using var dot = new SolidBrush(Color.OrangeRed);
                int dotY = e.Bounds.Top + (e.Bounds.Height - 8) / 2;
                e.Graphics.FillEllipse(dot, x, dotY, 8, 8);
                x += 13;
            }

            // Icon
            bool showIcon = DockingCaptionPainter.HasTabIcon(panel.IconPath);
            if (showIcon)
            {
                var iconRect = new Rectangle(x, e.Bounds.Top + (e.Bounds.Height - 16) / 2, 16, 16);
                DockingCaptionPainter.PaintTabIcon(e.Graphics, iconRect, panel.IconPath, foreCol);
                x += 18;
            }

            // Title
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var titleRect = new Rectangle(x, e.Bounds.Top, e.Bounds.Width - x - Pad, e.Bounds.Height);
            using var fmt = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
            };
            using var titleBrush = new SolidBrush(foreCol);
            e.Graphics.DrawString(panel.Title ?? "Panel",
                _list.Font ?? SystemFonts.DefaultFont, titleBrush, titleRect, fmt);
        }
    }
}

