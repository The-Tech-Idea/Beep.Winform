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
    /// Popup content that renders items in grouped sections. Groups whose items
    /// have no sub-text are shown as a **pill-button grid** (like "Top categories":
    /// TV, Movies, Originals, Kids). Other groups are shown as a vertical list.
    /// A "See more" link appears at the bottom if items were truncated.
    /// Used by <see cref="SegmentedTriggerPopupHostForm"/>.
    /// </summary>
    internal sealed class GroupedSectionsPopupContent : Control, IPopupContentPanel
    {
        public event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;
        public event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;
        public event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;
        public event EventHandler ApplyClicked;
        public event EventHandler CancelClicked;

        private ComboBoxPopupModel _model;
        private readonly BeepTextBox _searchBox;
        private readonly Panel _scrollContainer;
        private readonly Panel _scrollPanel;
        private readonly BeepScrollBar _vScrollBar;
        private readonly ComboBoxPopupFooter _footer;
        private readonly List<ISectionItem> _allItems = new List<ISectionItem>();
        private readonly List<Control> _sectionControls = new List<Control>();
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.SegmentedTrigger();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private int _keyboardFocusIndex = -1;
        private int _scrollOffset = 0;
        private int _totalContentHeight = 0;

        public GroupedSectionsPopupContent()
        {
            DoubleBuffered = true;
            TabStop = true;

            _searchBox = new BeepTextBox
            {
                Dock = DockStyle.Top,
                PlaceholderText = "Filter list...",
                Visible = false,
                Height = 34
            };
            _searchBox.TextChanged += (s, e) =>
                SearchTextChanged?.Invoke(this, new ComboBoxSearchChangedEventArgs(_searchBox.Text));
            _searchBox.KeyDown += OnSearchKeyDown;

            _footer = new ComboBoxPopupFooter { Visible = false };
            _footer.ApplyClicked += (s, e) => ApplyClicked?.Invoke(this, EventArgs.Empty);
            _footer.CancelClicked += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);
            _footer.PrimaryActionClicked += (s, e) => ApplyClicked?.Invoke(this, EventArgs.Empty);

            _scrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(4, 4, 4, 4)
            };

            _scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false
            };
            _scrollPanel.MouseWheel += OnListPanelMouseWheel;

            _vScrollBar = new BeepScrollBar
            {
                Dock = DockStyle.Right,
                Width = 14,
                ScrollOrientation = Orientation.Vertical,
                Minimum = 0,
                Value = 0,
                Visible = false
            };
            _vScrollBar.ValueChanged += OnScrollBarValueChanged;

            _scrollContainer.Controls.Add(_scrollPanel);
            _scrollContainer.Controls.Add(_vScrollBar);

            Controls.Add(_footer);
            Controls.Add(_scrollContainer);
            Controls.Add(_searchBox);
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.SegmentedTrigger();
            _searchBox.PlaceholderText = _profile.SearchPlaceholder;
            _searchBox.Height = _profile.SearchBoxHeight;
            _footer.ApplyProfile(_profile);
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _themeTokens = tokens ?? ComboBoxThemeTokens.Fallback();
            BackColor = _themeTokens.PopupBackColor;
            _scrollContainer.BackColor = _themeTokens.PopupBackColor;
            _scrollPanel.BackColor = _themeTokens.PopupBackColor;
            _searchBox.BackColor = _themeTokens.PopupBackColor;
            _searchBox.ForeColor = _themeTokens.ForeColor;
            _footer.BackColor = _themeTokens.PopupBackColor;
            _footer.ApplyThemeTokens(_themeTokens);
            if (!string.IsNullOrEmpty(_themeTokens.ThemeName))
                _vScrollBar.Theme = _themeTokens.ThemeName;
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _model = model;
            _searchBox.Visible = model.ShowSearchBox || _profile.ForceSearchVisible;
            _footer.Visible = model.ShowFooter || _profile.ForceFooterVisible;
            _footer.Setup(model.ShowApplyCancel, model.ShowSelectAll, model.UsePrimaryActionFooter, model.PrimaryActionText);
            if (model.IsMultiSelect && model.AllRows != null)
            {
                int count = 0;
                foreach (var r in model.AllRows)
                    if (r.IsSelected || r.IsChecked) count++;
                _footer.UpdateSelectedCount(count);
            }

            _scrollPanel.SuspendLayout();
            _scrollPanel.Controls.Clear();
            _allItems.Clear();
            _sectionControls.Clear();
            _scrollOffset = 0;

            if (model.FilteredRows != null && model.FilteredRows.Count > 0)
            {
                BuildSections(model.FilteredRows);
            }
            else
            {
                _totalContentHeight = 0;
            }

            _scrollPanel.ResumeLayout(true);
            UpdateScrollBar();
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
            var selectable = _allItems.Where(i => i.IsSelectable).ToList();
            if (selectable.Count == 0) { _keyboardFocusIndex = -1; return; }

            int next = Math.Max(0, Math.Min(index, selectable.Count - 1));
            _keyboardFocusIndex = next;
            for (int i = 0; i < selectable.Count; i++)
                selectable[i].SetFocused(i == next);

            KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(next));

            if (next >= 0 && next < selectable.Count && selectable[next] is Control c)
            {
                EnsureControlVisible(c);
            }
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

        public void FocusItem(SimpleItem item)
        {
            // Specialized panel — no-op; chip-click scroll is handled by default content.
        }

        protected override bool IsInputKey(Keys keyData) => keyData switch
        {
            Keys.Up or Keys.Down or Keys.Home or Keys.End
                or Keys.PageUp or Keys.PageDown or Keys.Enter => true,
            _ => base.IsInputKey(keyData)
        };

        protected override void OnKeyDown(KeyEventArgs e) { base.OnKeyDown(e); HandleNav(e); }
        private void OnSearchKeyDown(object sender, KeyEventArgs e) => HandleNav(e);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollBar();
            ApplyScrollOffset();
        }

        private void UpdateScrollBar()
        {
            int viewportHeight = _scrollPanel.ClientSize.Height;
            if (_totalContentHeight > viewportHeight)
            {
                _vScrollBar.Maximum = _totalContentHeight - viewportHeight;
                _vScrollBar.LargeChange = viewportHeight;
                _vScrollBar.SmallChange = Math.Max(1, _profile.BaseRowHeight);
                _vScrollBar.Value = Math.Min(_scrollOffset, _vScrollBar.Maximum);
                _vScrollBar.Visible = true;
            }
            else
            {
                _vScrollBar.Visible = false;
                _scrollOffset = 0;
            }
        }

        private void OnScrollBarValueChanged(object sender, EventArgs e)
        {
            _scrollOffset = _vScrollBar.Value;
            ApplyScrollOffset();
        }

        private void OnListPanelMouseWheel(object sender, MouseEventArgs e)
        {
            if (!_vScrollBar.Visible) return;
            int delta = -e.Delta / 4;
            int newVal = Math.Max(0, Math.Min(_vScrollBar.Maximum, _scrollOffset + delta));
            if (newVal != _scrollOffset)
            {
                _scrollOffset = newVal;
                _vScrollBar.Value = newVal;
                ApplyScrollOffset();
            }
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void ApplyScrollOffset()
        {
            int yPos = -_scrollOffset;
            foreach (var ctrl in _sectionControls)
            {
                ctrl.Top = yPos;
                yPos += ctrl.Height;
            }
        }

        private void EnsureControlVisible(Control c)
        {
            if (!_vScrollBar.Visible) return;

            int ctrlTop = c.Top + _scrollOffset;
            int ctrlBottom = ctrlTop + c.Height;
            int viewportHeight = _scrollPanel.ClientSize.Height;

            if (ctrlTop < _scrollOffset)
            {
                _scrollOffset = ctrlTop;
            }
            else if (ctrlBottom > _scrollOffset + viewportHeight)
            {
                _scrollOffset = ctrlBottom - viewportHeight;
            }
            else
            {
                return;
            }

            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, _vScrollBar.Maximum));
            _vScrollBar.Value = _scrollOffset;
            ApplyScrollOffset();
        }

        private void HandleNav(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down: MoveFocus(1); e.Handled = true; break;
                case Keys.Up: MoveFocus(-1); e.Handled = true; break;
                case Keys.PageDown: MoveFocus(6); e.Handled = true; break;
                case Keys.PageUp: MoveFocus(-6); e.Handled = true; break;
                case Keys.Home: SetKeyboardFocusIndex(0); e.Handled = true; break;
                case Keys.End:
                    SetKeyboardFocusIndex(_allItems.Count(i => i.IsSelectable) - 1);
                    e.Handled = true; break;
                case Keys.Enter: CommitFocused(); e.Handled = true; e.SuppressKeyPress = true; break;
            }
        }

        private void MoveFocus(int delta)
        {
            int start = _keyboardFocusIndex >= 0 ? _keyboardFocusIndex : 0;
            SetKeyboardFocusIndex(start + delta);
        }

        private void CommitFocused()
        {
            var selectable = _allItems.Where(i => i.IsSelectable).ToList();
            if (_keyboardFocusIndex < 0 || _keyboardFocusIndex >= selectable.Count) return;
            var item = selectable[_keyboardFocusIndex];
            if (item.RowModel != null) OnItemClicked(this, item.RowModel);
        }

        private void OnItemClicked(object sender, ComboBoxPopupRowModel row)
        {
            if (row == null || !row.IsEnabled) return;
            bool close = !(_model?.IsMultiSelect ?? false);
            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(row, close));
        }

        // ── Build sections from rows ──────────────────────────────────────

        private void BuildSections(IReadOnlyList<ComboBoxPopupRowModel> rows)
        {
            int panelWidth = Math.Max(100, _scrollPanel.ClientSize.Width - 8);
            var controls = new List<Control>();
            int yPos = 0;

            // Group rows by contiguous sections: GroupHeader → items until next header/separator
            var sections = new List<(ComboBoxPopupRowModel Header, List<ComboBoxPopupRowModel> Items)>();
            (ComboBoxPopupRowModel Header, List<ComboBoxPopupRowModel> Items) current = (null, new List<ComboBoxPopupRowModel>());

            foreach (var row in rows)
            {
                if (row.RowKind == ComboBoxPopupRowKind.GroupHeader)
                {
                    if (current.Items.Count > 0 || current.Header != null)
                        sections.Add(current);
                    current = (row, new List<ComboBoxPopupRowModel>());
                }
                else if (row.RowKind == ComboBoxPopupRowKind.Separator)
                {
                    if (current.Items.Count > 0 || current.Header != null)
                        sections.Add(current);
                    current = (null, new List<ComboBoxPopupRowModel>());
                    // Add a separator control
                    var sep = new SectionSeparator(_themeTokens) { Width = panelWidth, Left = 0, Top = yPos };
                    controls.Add(sep);
                    yPos += sep.Height;
                }
                else
                {
                    current.Items.Add(row);
                }
            }
            if (current.Items.Count > 0 || current.Header != null)
                sections.Add(current);

            // Render each section
            foreach (var section in sections)
            {
                // Section header
                if (section.Header != null)
                {
                    var headerCtrl = new SectionHeader(section.Header, _themeTokens)
                    {
                        Width = panelWidth, Left = 0, Top = yPos
                    };
                    controls.Add(headerCtrl);
                    _allItems.Add(headerCtrl);
                    yPos += headerCtrl.Height;
                }

                // Decide: if all items in section are short text (no images, no subtext), use pill grid
                bool usePillGrid = section.Items.Count > 0 &&
                    section.Items.All(r => string.IsNullOrWhiteSpace(r.SubText));

                if (usePillGrid && section.Items.Count >= 2)
                {
                    // Pill grid for this section
                    var grid = new FlowLayoutPanel
                    {
                        Left = 0, Top = yPos,
                        Width = panelWidth,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        AutoSize = true,
                        MaximumSize = new Size(panelWidth, 200),
                        Padding = new Padding(4, 4, 4, 8)
                    };
                    grid.BackColor = _themeTokens.PopupBackColor;

                    foreach (var item in section.Items)
                    {
                        var pill = new SectionPill(item, _themeTokens);
                        pill.PillClicked += OnItemClicked;
                        grid.Controls.Add(pill);
                        _allItems.Add(pill);
                    }
                    controls.Add(grid);
                    // Estimate grid height (will auto-size)
                    yPos += grid.PreferredSize.Height;
                }
                else
                {
                    // Vertical list for this section
                    foreach (var item in section.Items)
                    {
                        var row = new SectionListRow(item, _themeTokens, _profile)
                        {
                            Width = panelWidth, Left = 0, Top = yPos
                        };
                        row.RowClicked += OnItemClicked;
                        controls.Add(row);
                        _allItems.Add(row);
                        yPos += row.Height;
                    }
                }
            }

            // Track total content height
            _totalContentHeight = yPos;

            // Add controls to panel (no longer using Dock)
            foreach (var ctrl in controls)
            {
                _scrollPanel.Controls.Add(ctrl);
                _sectionControls.Add(ctrl);
            }
        }

        // ── Interface for unified focus tracking ──────────────────────────

        private interface ISectionItem
        {
            bool IsSelectable { get; }
            ComboBoxPopupRowModel RowModel { get; }
            void SetFocused(bool focused);
        }

        // ── Section Header ────────────────────────────────────────────────

        private sealed class SectionHeader : Control, ISectionItem
        {
            public ComboBoxPopupRowModel RowModel { get; }
            public bool IsSelectable => false;
            private readonly ComboBoxThemeTokens _tokens;

            public SectionHeader(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens)
            {
                RowModel = model;
                _tokens = tokens;
                DoubleBuffered = true;
                Height = 30;
                Margin = new Padding(0, 4, 0, 0);
            }

            public void SetFocused(bool focused) { }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                var bounds = ClientRectangle;
                using var bgBrush = new SolidBrush(Color.FromArgb(30, _tokens.FocusBorderColor));
                var bgRect = new Rectangle(bounds.Left + 4, bounds.Top, bounds.Width - 8, bounds.Height);
                g.FillRectangle(bgBrush, bgRect);

                // Accent bar on left
                using var accentBrush = new SolidBrush(_tokens.FocusBorderColor);
                g.FillRectangle(accentBrush, bounds.Left + 4, bounds.Top + 4, 3, bounds.Height - 8);

                // Header text
                using var headerFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
                var textRect = new Rectangle(bounds.Left + 14, bounds.Top, bounds.Width - 18, bounds.Height);
                TextRenderer.DrawText(g, RowModel.GroupName ?? RowModel.Text ?? "", headerFont, textRect,
                    _tokens.PopupGroupHeaderFore, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        // ── Section Separator ─────────────────────────────────────────────

        private sealed class SectionSeparator : Control
        {
            private readonly ComboBoxThemeTokens _tokens;

            public SectionSeparator(ComboBoxThemeTokens tokens)
            {
                _tokens = tokens;
                DoubleBuffered = true;
                Height = 12;
                Margin = Padding.Empty;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                int y = Height / 2;
                using var pen = new Pen(Color.FromArgb(100, _tokens.PopupSeparatorColor));
                e.Graphics.DrawLine(pen, 8, y, Width - 8, y);
            }
        }

        // ── Section Pill Button (for pill-grid sections) ──────────────────

        private sealed class SectionPill : Control, ISectionItem
        {
            public ComboBoxPopupRowModel RowModel { get; }
            public bool IsSelectable => RowModel.IsEnabled;
            private readonly ComboBoxThemeTokens _tokens;
            private bool _hovered;
            private bool _focused;

            public event EventHandler<ComboBoxPopupRowModel> PillClicked;

            public SectionPill(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens)
            {
                RowModel = model;
                _tokens = tokens;
                DoubleBuffered = true;
                Cursor = model.IsEnabled ? Cursors.Hand : Cursors.Default;
                Height = 34;
                Margin = new Padding(4);
                TabStop = false;

                using var g = CreateGraphics();
                var sz = TextRenderer.MeasureText(g, model.Text ?? "", Font);
                Width = sz.Width + 28;
            }

            public void SetFocused(bool f) { if (_focused != f) { _focused = f; Invalidate(); } }

            protected override void OnClick(EventArgs e) { base.OnClick(e); PillClicked?.Invoke(this, RowModel); }
            protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
            protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = ClientRectangle;
                rect.Inflate(-1, -1);
                if (rect.Width <= 0 || rect.Height <= 0) return;

                int r = Math.Min(rect.Height / 2, 6);
                Color back, border, fore;

                if (RowModel.IsSelected)
                {
                    back = _tokens.FocusBorderColor;
                    border = _tokens.FocusBorderColor;
                    fore = Color.White;
                }
                else if (_focused)
                {
                    back = _tokens.PopupRowFocusColor;
                    border = Color.FromArgb(160, _tokens.FocusBorderColor);
                    fore = _tokens.ForeColor;
                }
                else if (_hovered)
                {
                    back = _tokens.PopupRowHoverColor;
                    border = Color.FromArgb(140, _tokens.PopupSeparatorColor);
                    fore = _tokens.ForeColor;
                }
                else
                {
                    back = Color.FromArgb(240, 244, 250);
                    border = Color.FromArgb(220, 224, 232);
                    fore = _tokens.ForeColor;
                }

                using var path = CreateRoundRect(rect, r);
                using (var brush = new SolidBrush(back)) g.FillPath(brush, path);
                using (var pen = new Pen(border)) g.DrawPath(pen, path);

                TextRenderer.DrawText(g, RowModel.Text ?? "", Font, rect, fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            private static GraphicsPath CreateRoundRect(Rectangle rect, int radius)
            {
                var path = new GraphicsPath();
                int d = radius * 2;
                if (d > rect.Width) d = rect.Width;
                if (d > rect.Height) d = rect.Height;
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                return path;
            }
        }

        // ── Section List Row (for list sections) ──────────────────────────

        private sealed class SectionListRow : Control, ISectionItem
        {
            public ComboBoxPopupRowModel RowModel { get; }
            public bool IsSelectable => RowModel.IsEnabled;
            private readonly ComboBoxThemeTokens _tokens;
            private readonly ComboBoxPopupHostProfile _profile;
            private bool _hovered;
            private bool _focused;

            public event EventHandler<ComboBoxPopupRowModel> RowClicked;

            public SectionListRow(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens, ComboBoxPopupHostProfile profile)
            {
                RowModel = model;
                _tokens = tokens;
                _profile = profile;
                DoubleBuffered = true;
                Cursor = model.IsEnabled ? Cursors.Hand : Cursors.Default;
                Height = model.RowKind == ComboBoxPopupRowKind.WithSubText ? 44 : (profile.BaseRowHeight);
                Margin = Padding.Empty;
                TabStop = false;
            }

            public void SetFocused(bool f) { if (_focused != f) { _focused = f; Invalidate(); } }

            protected override void OnClick(EventArgs e) { base.OnClick(e); RowClicked?.Invoke(this, RowModel); }
            protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
            protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                var bounds = ClientRectangle;
                if (bounds.Width <= 4 || bounds.Height <= 4) return;

                Color fore = RowModel.IsEnabled ? _tokens.ForeColor : _tokens.DisabledForeColor;
                Color back = _tokens.PopupBackColor;

                if (RowModel.IsSelected || RowModel.RowKind == ComboBoxPopupRowKind.Selected)
                    back = _tokens.PopupRowSelectedColor;
                else if (_focused)
                    back = _tokens.PopupRowFocusColor;
                else if (_hovered)
                    back = _tokens.PopupRowHoverColor;

                using (var brush = new SolidBrush(back))
                    g.FillRectangle(brush, bounds);

                // Separator line at bottom
                if (_profile.ShowRowSeparators)
                {
                    using var sepPen = new Pen(Color.FromArgb(60, _tokens.PopupSeparatorColor));
                    g.DrawLine(sepPen, bounds.Left + 12, bounds.Bottom - 1, bounds.Right - 12, bounds.Bottom - 1);
                }

                var textRect = new Rectangle(bounds.Left + 14, bounds.Top, bounds.Width - 28, bounds.Height);

                // Sub-arrow for expandable items (→)
                if (!string.IsNullOrEmpty(RowModel.SubText) && RowModel.SubText == ">")
                {
                    var arrowRect = new Rectangle(bounds.Right - 24, bounds.Top + (bounds.Height - 12) / 2, 10, 12);
                    using var arrowPen = new Pen(_tokens.PopupSubTextColor, 1.5f);
                    g.DrawLine(arrowPen, arrowRect.Left, arrowRect.Top, arrowRect.Right, arrowRect.Top + arrowRect.Height / 2);
                    g.DrawLine(arrowPen, arrowRect.Right, arrowRect.Top + arrowRect.Height / 2, arrowRect.Left, arrowRect.Bottom);
                    textRect.Width -= 18;
                }

                // Checkmark for selected
                if (_profile.ShowCheckmarkForSelected && (RowModel.IsSelected || RowModel.RowKind == ComboBoxPopupRowKind.Selected))
                {
                    var ckRect = new Rectangle(bounds.Right - 24, bounds.Top + (bounds.Height - 14) / 2, 14, 14);
                    using var ckPen = new Pen(_tokens.FocusBorderColor, 2f);
                    g.DrawLine(ckPen, ckRect.Left + 2, ckRect.Top + 7, ckRect.Left + 5, ckRect.Bottom - 2);
                    g.DrawLine(ckPen, ckRect.Left + 5, ckRect.Bottom - 2, ckRect.Right - 2, ckRect.Top + 2);
                    textRect.Width = Math.Max(1, ckRect.Left - textRect.Left - 6);
                }

                TextRenderer.DrawText(g, RowModel.Text ?? "", Font, textRect, fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }
    }
}
