using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Popup content with a selected-chips area at the top, a dashed separator,
    /// and a checkbox list below. Used by MultiChipCompact and MultiChipSearch.
    /// Reference images: category picker with chips + checkboxes.
    /// </summary>
    internal sealed class ChipHeaderPopupContent : Control, IPopupContentPanel
    {
        public event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;
        public event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;
        public event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;
        public event EventHandler ApplyClicked;
        public event EventHandler CancelClicked;

        private ComboBoxPopupModel _model;
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.MultiChipCompact();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();

        // Child controls — laid out in dock order (bottom first, then top, then fill)
        private readonly BeepTextBox _searchBox;
        private readonly FlowLayoutPanel _chipArea;
        private readonly DashedSeparatorPanel _separator;
        private readonly FlowLayoutPanel _listPanel;
        private readonly ComboBoxPopupFooter _footer;

        private readonly List<PopupChip> _chips = new List<PopupChip>();
        private readonly List<ComboBoxPopupRow> _rows = new List<ComboBoxPopupRow>();
        private int _keyboardFocusIndex = -1;

        public ChipHeaderPopupContent()
        {
            DoubleBuffered = true;
            TabStop = true;

            // Search box at top (optional)
            _searchBox = new BeepTextBox
            {
                Dock = DockStyle.Top,
                PlaceholderText = "Search and select...",
                Visible = false,
                Height = 34
            };
            _searchBox.TextChanged += (s, e) =>
                SearchTextChanged?.Invoke(this, new ComboBoxSearchChangedEventArgs(_searchBox.Text));
            _searchBox.KeyDown += OnSearchKeyDown;

            // Chip area — shows selected items as removable chips
            _chipArea = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                MaximumSize = new Size(0, 80),
                MinimumSize = new Size(0, 0),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(8, 6, 8, 2),
                Visible = false
            };

            // Dashed separator between chips and list
            _separator = new DashedSeparatorPanel
            {
                Dock = DockStyle.Top,
                Height = 14,
                Visible = false
            };

            // Checkbox row list
            _listPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };

            // Footer with Select All / Clear All or Apply / Cancel
            _footer = new ComboBoxPopupFooter { Visible = false };
            _footer.ApplyClicked += (s, e) => ApplyClicked?.Invoke(this, EventArgs.Empty);
            _footer.CancelClicked += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);
            _footer.SelectAllClicked += OnSelectAllClicked;
            _footer.ClearAllClicked += OnClearAllClicked;

            // Add controls in correct dock order:
            // Footer (Bottom) → ListPanel (Fill) → Separator (Top) → ChipArea (Top) → SearchBox (Top)
            Controls.Add(_footer);
            Controls.Add(_listPanel);
            Controls.Add(_separator);
            Controls.Add(_chipArea);
            Controls.Add(_searchBox);
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.MultiChipCompact();
            _searchBox.PlaceholderText = _profile.SearchPlaceholder;
            _searchBox.Height = _profile.SearchBoxHeight;
            _chipArea.MaximumSize = new Size(0, _profile.ChipAreaMaxHeight > 0 ? _profile.ChipAreaMaxHeight : 80);
            _footer.ApplyProfile(_profile);
            _listPanel.Padding = new Padding(
                _profile.ListHorizontalPadding, _profile.ListVerticalPadding,
                _profile.ListHorizontalPadding, _profile.ListVerticalPadding);
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _themeTokens = tokens ?? ComboBoxThemeTokens.Fallback();
            BackColor = _themeTokens.PopupBackColor;
            _chipArea.BackColor = _themeTokens.PopupBackColor;
            _listPanel.BackColor = _themeTokens.PopupBackColor;
            _searchBox.BackColor = _themeTokens.PopupBackColor;
            _searchBox.ForeColor = _themeTokens.ForeColor;
            _separator.SeparatorColor = _themeTokens.PopupSeparatorColor;
            _footer.BackColor = _themeTokens.PopupBackColor;
            _footer.ApplyThemeTokens(_themeTokens);

            foreach (var chip in _chips) chip.ApplyThemeTokens(_themeTokens);
            foreach (var row in _rows) row.ApplyThemeTokens(_themeTokens);
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _model = model;

            // Search box visibility
            _searchBox.Visible = model.ShowSearchBox || _profile.ForceSearchVisible;

            // Footer
            _footer.Visible = model.ShowFooter || _profile.ForceFooterVisible;
            _footer.Setup(model.ShowApplyCancel, model.ShowSelectAll);
            if (model.IsMultiSelect && model.AllRows != null)
            {
                int count = 0;
                foreach (var r in model.AllRows)
                    if (r.IsSelected || r.IsChecked) count++;
                _footer.UpdateSelectedCount(count);
            }

            // Rebuild chips from all checked items
            RebuildChips(model);

            // Separator visible when chips are present
            _separator.Visible = _chipArea.Visible && _profile.ShowDashedSeparator;

            // Rebuild checkbox row list from filtered rows
            _listPanel.SuspendLayout();
            _listPanel.Controls.Clear();
            _rows.Clear();

            if (model.FilteredRows != null)
            {
                int listWidth = Math.Max(100,
                    _listPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
                foreach (var rowModel in model.FilteredRows)
                {
                    var row = new ComboBoxPopupRow
                    {
                        Width = listWidth,
                        Dock = DockStyle.Top
                    };
                    row.ApplyProfile(_profile);
                    row.ApplyThemeTokens(_themeTokens);
                    row.SetModel(rowModel);
                    row.RowCommitted += OnRowCommitted;
                    _listPanel.Controls.Add(row);
                    _rows.Add(row);
                }
            }

            _listPanel.ResumeLayout(true);
            SetKeyboardFocusIndex(model.KeyboardFocusIndex >= 0 ? model.KeyboardFocusIndex : 0);
        }

        public void UpdateSearchText(string text)
        {
            string safe = text ?? string.Empty;
            if (!string.Equals(_searchBox.Text, safe, StringComparison.Ordinal))
                _searchBox.Text = safe;
        }

        public void SetKeyboardFocusIndex(int index)
        {
            if (_rows.Count == 0) { _keyboardFocusIndex = -1; return; }

            int next = Math.Max(0, Math.Min(index, _rows.Count - 1));
            _keyboardFocusIndex = next;
            for (int i = 0; i < _rows.Count; i++)
                _rows[i].SetKeyboardFocused(i == next);

            KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(next));
            EnsureRowVisible(next);
        }

        public void FocusSearchBox()
        {
            if (_searchBox.Visible && _searchBox.Enabled)
            {
                _searchBox.Focus();
                _searchBox.SelectionStart = _searchBox.Text?.Length ?? 0;
                return;
            }
            Focus();
        }

        /// <summary>
        /// Estimates ideal height.
        /// </summary>
        public int EstimateHeight(ComboBoxPopupModel model)
        {
            int h = 0;
            if (model.ShowSearchBox || _profile.ForceSearchVisible) h += _profile.SearchBoxHeight;

            // Chips area — rough estimate
            int checkedCount = model.AllRows?.Count(r => r.IsChecked) ?? 0;
            if (checkedCount > 0 && _profile.ShowChipArea)
            {
                h += Math.Min(42 + (checkedCount > 4 ? 30 : 0), _profile.ChipAreaMaxHeight);
                if (_profile.ShowDashedSeparator) h += 14;
            }

            // Rows
            int rowCount = model.FilteredRows?.Count ?? 0;
            h += rowCount * _profile.BaseRowHeight;

            // Footer
            if (model.ShowFooter || _profile.ForceFooterVisible) h += _profile.FooterHeight;

            return h;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData switch
            {
                Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown
                    or Keys.Home or Keys.End or Keys.Enter => true,
                _ => base.IsInputKey(keyData)
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            HandleNavKey(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int width = Math.Max(100, _listPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
            foreach (var row in _rows) row.Width = width;
        }

        // ── Chip Area ─────────────────────────────────────────────────────

        private void RebuildChips(ComboBoxPopupModel model)
        {
            _chipArea.SuspendLayout();
            _chipArea.Controls.Clear();
            _chips.Clear();

            var allRows = model.AllRows;
            if (allRows == null || !_profile.ShowChipArea)
            {
                _chipArea.Visible = false;
                _chipArea.ResumeLayout(true);
                return;
            }

            foreach (var row in allRows)
            {
                if (!row.IsChecked) continue;
                var chip = new PopupChip(row, _themeTokens);
                chip.CloseClicked += OnChipCloseClicked;
                _chipArea.Controls.Add(chip);
                _chips.Add(chip);
            }

            _chipArea.Visible = _chips.Count > 0;
            _chipArea.ResumeLayout(true);
        }

        private void OnChipCloseClicked(object sender, ComboBoxPopupRowModel row)
        {
            // Toggle off the checked item
            var toggled = new ComboBoxPopupRowModel
            {
                SourceItem = row.SourceItem,
                RowKind = row.RowKind,
                Text = row.Text,
                SubText = row.SubText,
                ImagePath = row.ImagePath,
                GroupName = row.GroupName,
                IsSelected = false,
                IsEnabled = row.IsEnabled,
                IsKeyboardFocused = row.IsKeyboardFocused,
                IsCheckable = row.IsCheckable,
                IsChecked = false,
                ListIndex = row.ListIndex
            };
            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(toggled, closePopup: false));
        }

        // ── Row List ──────────────────────────────────────────────────────

        private void OnRowCommitted(object sender, ComboBoxPopupRowModel rowModel)
        {
            if (rowModel == null) return;
            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(rowModel, closePopup: false));
        }

        // ── Select All / Clear All ────────────────────────────────────────

        private void OnSelectAllClicked(object sender, EventArgs e)
        {
            if (_model?.AllRows == null) return;
            foreach (var row in _model.AllRows)
            {
                if (row.IsCheckable && !row.IsChecked && row.IsEnabled
                    && row.RowKind != ComboBoxPopupRowKind.GroupHeader
                    && row.RowKind != ComboBoxPopupRowKind.Separator)
                {
                    var toggled = new ComboBoxPopupRowModel
                    {
                        SourceItem = row.SourceItem, RowKind = row.RowKind,
                        Text = row.Text, SubText = row.SubText,
                        ImagePath = row.ImagePath, GroupName = row.GroupName,
                        IsSelected = true, IsEnabled = row.IsEnabled,
                        IsCheckable = row.IsCheckable, IsChecked = true,
                        ListIndex = row.ListIndex
                    };
                    RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(toggled, closePopup: false));
                }
            }
        }

        private void OnClearAllClicked(object sender, EventArgs e)
        {
            if (_model?.AllRows == null) return;
            foreach (var row in _model.AllRows)
            {
                if (row.IsCheckable && row.IsChecked && row.IsEnabled)
                {
                    var toggled = new ComboBoxPopupRowModel
                    {
                        SourceItem = row.SourceItem, RowKind = row.RowKind,
                        Text = row.Text, SubText = row.SubText,
                        ImagePath = row.ImagePath, GroupName = row.GroupName,
                        IsSelected = false, IsEnabled = row.IsEnabled,
                        IsCheckable = row.IsCheckable, IsChecked = false,
                        ListIndex = row.ListIndex
                    };
                    RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(toggled, closePopup: false));
                }
            }
        }

        // ── Keyboard navigation ───────────────────────────────────────────

        private void OnSearchKeyDown(object sender, KeyEventArgs e) => HandleNavKey(e);

        private void HandleNavKey(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down: MoveFocus(1); e.Handled = true; break;
                case Keys.Up: MoveFocus(-1); e.Handled = true; break;
                case Keys.PageDown: MoveFocus(6); e.Handled = true; break;
                case Keys.PageUp: MoveFocus(-6); e.Handled = true; break;
                case Keys.Home: SetKeyboardFocusIndex(0); e.Handled = true; break;
                case Keys.End: SetKeyboardFocusIndex(_rows.Count - 1); e.Handled = true; break;
                case Keys.Enter:
                    CommitFocusedRow(); e.Handled = true; e.SuppressKeyPress = true; break;
            }
        }

        private void MoveFocus(int delta)
        {
            int start = _keyboardFocusIndex >= 0 ? _keyboardFocusIndex : 0;
            SetKeyboardFocusIndex(start + delta);
        }

        private void CommitFocusedRow()
        {
            if (_keyboardFocusIndex < 0 || _keyboardFocusIndex >= _rows.Count) return;
            var row = _rows[_keyboardFocusIndex];
            if (row.Model != null)
                RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(row.Model, closePopup: false));
        }

        private void EnsureRowVisible(int index)
        {
            if (index < 0 || index >= _rows.Count) return;
            try { _listPanel.ScrollControlIntoView(_rows[index]); }
            catch { /* best-effort */ }
        }

        // ──────────────────────────────────────────────────────────────────
        // Dashed Separator — thin panel that draws a horizontal dashed line
        // ──────────────────────────────────────────────────────────────────

        internal sealed class DashedSeparatorPanel : Control
        {
            public Color SeparatorColor { get; set; } = Color.FromArgb(180, 180, 195);

            public DashedSeparatorPanel()
            {
                DoubleBuffered = true;
                Height = 14;
                SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var g = e.Graphics;
                int y = Height / 2;
                using var pen = new Pen(SeparatorColor, 1f) { DashStyle = DashStyle.Dash };
                g.DrawLine(pen, 12, y, Width - 12, y);
            }
        }

        // ──────────────────────────────────────────────────────────────────
        // PopupChip — renders one selected item as a removable chip
        // ──────────────────────────────────────────────────────────────────

        internal sealed class PopupChip : Control
        {
            public ComboBoxPopupRowModel RowModel { get; }
            private ComboBoxThemeTokens _tokens;
            private bool _hoverClose;
            private Rectangle _closeRect;

            public event EventHandler<ComboBoxPopupRowModel> CloseClicked;

            public PopupChip(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens)
            {
                RowModel = model;
                _tokens = tokens;
                DoubleBuffered = true;
                Height = 28;
                Margin = new Padding(3, 3, 3, 3);
                Cursor = Cursors.Default;

                using var g = CreateGraphics();
                var textSize = TextRenderer.MeasureText(g, model.Text ?? "", Font);
                Width = textSize.Width + 30; // text + close button + padding
            }

            public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
            {
                _tokens = tokens;
                Invalidate();
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                bool wasHover = _hoverClose;
                _hoverClose = _closeRect.Contains(e.Location);
                if (wasHover != _hoverClose) { Cursor = _hoverClose ? Cursors.Hand : Cursors.Default; Invalidate(); }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                if (_hoverClose) { _hoverClose = false; Cursor = Cursors.Default; Invalidate(); }
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                if (_hoverClose)
                    CloseClicked?.Invoke(this, RowModel);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = ClientRectangle;
                rect.Inflate(-1, -1);
                if (rect.Width <= 0 || rect.Height <= 0) return;

                int radius = Math.Min(rect.Height / 2, 12);
                Color chipBack = _tokens.FocusBorderColor;
                Color chipFore = Color.White;

                using var path = CreateRoundRectPath(rect, radius);
                using (var brush = new SolidBrush(chipBack))
                    g.FillPath(brush, path);

                // Close button rect
                int closeSize = 12;
                _closeRect = new Rectangle(
                    rect.Right - closeSize - 6,
                    rect.Top + (rect.Height - closeSize) / 2,
                    closeSize, closeSize);

                // Text
                var textRect = new Rectangle(rect.Left + 8, rect.Top, _closeRect.Left - rect.Left - 10, rect.Height);
                TextRenderer.DrawText(g, RowModel.Text ?? "", Font, textRect, chipFore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                // Close × icon
                Color closeFore = _hoverClose ? Color.White : Color.FromArgb(200, Color.White);
                using var closePen = new Pen(closeFore, 1.5f);
                int cx = _closeRect.Left + _closeRect.Width / 2;
                int cy = _closeRect.Top + _closeRect.Height / 2;
                int off = 3;
                g.DrawLine(closePen, cx - off, cy - off, cx + off, cy + off);
                g.DrawLine(closePen, cx + off, cy - off, cx - off, cy + off);
            }

            private static GraphicsPath CreateRoundRectPath(Rectangle rect, int radius)
            {
                var path = new GraphicsPath();
                int d = radius * 2;
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                return path;
            }
        }
    }
}
