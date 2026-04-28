using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
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
        private readonly List<SectionHeader> _headerPool = new List<SectionHeader>();
        private readonly List<SectionSeparator> _separatorPool = new List<SectionSeparator>();
        private readonly List<SectionListRow> _listRowPool = new List<SectionListRow>();
        private readonly List<SectionPill> _pillPool = new List<SectionPill>();
        private readonly List<FlowLayoutPanel> _pillGridPool = new List<FlowLayoutPanel>();
        private readonly List<ComboBoxPopupRowModel> _sectionItemBuffer = new List<ComboBoxPopupRowModel>();
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.SegmentedTrigger();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private int _keyboardFocusIndex = -1;
        private int _scrollOffset = 0;
        private int _totalContentHeight = 0;
        private readonly List<ISectionItem> _selectableItems = new List<ISectionItem>();
        private string _lastLayoutSignature = string.Empty;
        private int _headerPoolCursor;
        private int _separatorPoolCursor;
        private int _listRowPoolCursor;
        private int _pillPoolCursor;
        private int _pillGridPoolCursor;

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

            string signature = ComputeLayoutSignature(model.FilteredRows);
            bool requiresRebuild = !string.Equals(signature, _lastLayoutSignature, StringComparison.Ordinal);
            if (requiresRebuild)
            {
                _scrollPanel.SuspendLayout();
                _allItems.Clear();
                _sectionControls.Clear();
                _selectableItems.Clear();
                _scrollOffset = 0;
                PreparePoolsForReuse();

                if (model.FilteredRows != null && model.FilteredRows.Count > 0)
                {
                    BuildSections(model.FilteredRows);
                }
                else
                {
                    _totalContentHeight = 0;
                }

                _scrollPanel.ResumeLayout(true);
                _lastLayoutSignature = signature;
            }
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
            if (_selectableItems.Count == 0) { _keyboardFocusIndex = -1; return; }

            int next = Math.Max(0, Math.Min(index, _selectableItems.Count - 1));
            _keyboardFocusIndex = next;
            for (int i = 0; i < _selectableItems.Count; i++)
                _selectableItems[i].SetFocused(i == next);

            KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(next));

            if (next >= 0 && next < _selectableItems.Count && _selectableItems[next] is Control c)
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
            if (item == null || _allItems.Count == 0) return;
            int selectableIndex = FindSelectableIndexByItem(item);
            if (selectableIndex < 0)
            {
                return;
            }

            SetKeyboardFocusIndex(selectableIndex);

            if (selectableIndex >= 0 && selectableIndex < _selectableItems.Count && _selectableItems[selectableIndex] is Control ctrl)
            {
                EnsureControlVisible(ctrl);
            }
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
            int newVal = ComboBoxPopupMouseWheelHelper.ComputeNextOffsetFromWheel(_scrollOffset, e.Delta, _vScrollBar.Maximum);
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
                    SetKeyboardFocusIndex(_selectableItems.Count - 1);
                    e.Handled = true; break;
                case Keys.Enter: CommitFocused(); e.Handled = true; e.SuppressKeyPress = true; break;
            }
        }

        private void MoveFocus(int delta)
        {
            int selectableCount = _selectableItems.Count;
            SetKeyboardFocusIndex(ComboBoxPopupNavigationHelper.ShiftIndex(_keyboardFocusIndex, delta, selectableCount));
        }

        private void CommitFocused()
        {
            if (_keyboardFocusIndex < 0 || _keyboardFocusIndex >= _selectableItems.Count) return;
            var item = _selectableItems[_keyboardFocusIndex];
            if (item.RowModel != null) OnItemClicked(this, item.RowModel);
        }

        private void OnItemClicked(object sender, ComboBoxPopupRowModel row)
        {
            if (!ComboBoxPopupRowBehavior.IsSelectable(row)) return;
            bool close = !(_model?.IsMultiSelect ?? false);
            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(row, close));
        }

        // ── Build sections from rows ──────────────────────────────────────

        private void BuildSections(IReadOnlyList<ComboBoxPopupRowModel> rows)
        {
            int panelWidth = Math.Max(100, _scrollPanel.ClientSize.Width - 8);
            int yPos = 0;
            ComboBoxPopupRowModel currentHeader = null;
            _sectionItemBuffer.Clear();

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row == null)
                {
                    continue;
                }

                if (row.RowKind == ComboBoxPopupRowKind.GroupHeader)
                {
                    yPos = RenderSection(currentHeader, _sectionItemBuffer, panelWidth, yPos);
                    _sectionItemBuffer.Clear();
                    currentHeader = row;
                    continue;
                }

                if (row.RowKind == ComboBoxPopupRowKind.Separator)
                {
                    yPos = RenderSection(currentHeader, _sectionItemBuffer, panelWidth, yPos);
                    _sectionItemBuffer.Clear();
                    currentHeader = null;

                    var sep = AcquireSeparator();
                    sep.UpdateTheme(_themeTokens);
                    sep.Width = panelWidth;
                    sep.Left = 0;
                    sep.Top = yPos;
                    AddActiveControl(sep);
                    yPos += sep.Height;
                    continue;
                }

                _sectionItemBuffer.Add(row);
            }

            yPos = RenderSection(currentHeader, _sectionItemBuffer, panelWidth, yPos);
            _sectionItemBuffer.Clear();

            // Track total content height
            _totalContentHeight = yPos;
            _selectableItems.Clear();
            for (int i = 0; i < _allItems.Count; i++)
            {
                if (_allItems[i].IsSelectable)
                {
                    _selectableItems.Add(_allItems[i]);
                }
            }
        }

        private int RenderSection(ComboBoxPopupRowModel header, List<ComboBoxPopupRowModel> items, int panelWidth, int yPos)
        {
            if (header == null && (items == null || items.Count == 0))
            {
                return yPos;
            }

            if (header != null)
            {
                var headerCtrl = AcquireHeader();
                headerCtrl.Update(header, _themeTokens);
                headerCtrl.Width = panelWidth;
                headerCtrl.Left = 0;
                headerCtrl.Top = yPos;
                AddActiveControl(headerCtrl);
                _allItems.Add(headerCtrl);
                yPos += headerCtrl.Height;
            }

            bool usePillGrid = items != null && items.Count >= 2;
            if (usePillGrid)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(items[i].SubText))
                    {
                        usePillGrid = false;
                        break;
                    }
                }
            }

            if (usePillGrid)
            {
                var grid = AcquirePillGrid();
                grid.SuspendLayout();
                grid.Controls.Clear();
                grid.Left = 0;
                grid.Top = yPos;
                grid.Width = panelWidth;
                grid.MaximumSize = new Size(panelWidth, 200);
                grid.BackColor = _themeTokens.PopupBackColor;
                AddActiveControl(grid);

                for (int i = 0; i < items.Count; i++)
                {
                    var pill = AcquirePill();
                    pill.Update(items[i], _themeTokens);
                    grid.Controls.Add(pill);
                    _allItems.Add(pill);
                }

                grid.ResumeLayout(true);
                yPos += grid.PreferredSize.Height;
                return yPos;
            }

            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var row = AcquireListRow();
                    row.Update(items[i], _themeTokens, _profile);
                    row.Width = panelWidth;
                    row.Left = 0;
                    row.Top = yPos;
                    AddActiveControl(row);
                    _allItems.Add(row);
                    yPos += row.Height;
                }
            }

            return yPos;
        }

        private static string ComputeLayoutSignature(IReadOnlyList<ComboBoxPopupRowModel> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return "empty";
            }

            var sb = new StringBuilder(rows.Count * 12);
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row == null)
                {
                    sb.Append("null|");
                    continue;
                }

                sb.Append((int)row.RowKind).Append(':')
                  .Append(BeepComboBox.GetSimpleItemIdentity(row.SourceItem)).Append(':')
                  .Append(row.GroupName ?? string.Empty).Append(':')
                  .Append(row.Text ?? string.Empty).Append(':')
                  .Append(row.IsSelected ? '1' : '0')
                  .Append(row.IsChecked ? '1' : '0')
                  .Append(row.IsEnabled ? '1' : '0')
                  .Append('|');
            }

            return sb.ToString();
        }

        private void PreparePoolsForReuse()
        {
            _headerPoolCursor = 0;
            _separatorPoolCursor = 0;
            _listRowPoolCursor = 0;
            _pillPoolCursor = 0;
            _pillGridPoolCursor = 0;

            for (int i = 0; i < _headerPool.Count; i++) _headerPool[i].Visible = false;
            for (int i = 0; i < _separatorPool.Count; i++) _separatorPool[i].Visible = false;
            for (int i = 0; i < _listRowPool.Count; i++) _listRowPool[i].Visible = false;
            for (int i = 0; i < _pillPool.Count; i++) _pillPool[i].Visible = false;
            for (int i = 0; i < _pillGridPool.Count; i++)
            {
                _pillGridPool[i].Visible = false;
                _pillGridPool[i].Controls.Clear();
            }
        }

        private void AddActiveControl(Control control)
        {
            if (control.Parent != _scrollPanel)
            {
                _scrollPanel.Controls.Add(control);
            }

            control.Visible = true;
            _sectionControls.Add(control);
        }

        private SectionHeader AcquireHeader()
        {
            if (_headerPoolCursor >= _headerPool.Count)
            {
                _headerPool.Add(new SectionHeader());
            }

            return _headerPool[_headerPoolCursor++];
        }

        private SectionSeparator AcquireSeparator()
        {
            if (_separatorPoolCursor >= _separatorPool.Count)
            {
                _separatorPool.Add(new SectionSeparator());
            }

            return _separatorPool[_separatorPoolCursor++];
        }

        private SectionListRow AcquireListRow()
        {
            if (_listRowPoolCursor >= _listRowPool.Count)
            {
                var row = new SectionListRow();
                row.RowClicked += OnItemClicked;
                _listRowPool.Add(row);
            }

            return _listRowPool[_listRowPoolCursor++];
        }

        private SectionPill AcquirePill()
        {
            if (_pillPoolCursor >= _pillPool.Count)
            {
                var pill = new SectionPill();
                pill.PillClicked += OnItemClicked;
                _pillPool.Add(pill);
            }

            return _pillPool[_pillPoolCursor++];
        }

        private FlowLayoutPanel AcquirePillGrid()
        {
            if (_pillGridPoolCursor >= _pillGridPool.Count)
            {
                _pillGridPool.Add(new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true,
                    AutoSize = true,
                    Padding = new Padding(4, 4, 4, 8)
                });
            }

            return _pillGridPool[_pillGridPoolCursor++];
        }

        private int FindSelectableIndexByItem(SimpleItem item)
        {
            if (item == null || _selectableItems.Count == 0)
            {
                return -1;
            }

            for (int i = 0; i < _selectableItems.Count; i++)
            {
                var row = _selectableItems[i].RowModel;
                if (row == null)
                {
                    continue;
                }

                if (ReferenceEquals(row.SourceItem, item))
                {
                    return i;
                }

                if (string.Equals(BeepComboBox.GetSimpleItemIdentity(row.SourceItem), BeepComboBox.GetSimpleItemIdentity(item), StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
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
            public ComboBoxPopupRowModel RowModel { get; private set; }
            public bool IsSelectable => false;
            private ComboBoxThemeTokens _tokens;
            private string _lastThemeName = string.Empty;
            private string _lastHeaderText = string.Empty;

            public SectionHeader()
            {
                DoubleBuffered = true;
                Height = 30;
                Margin = new Padding(0, 4, 0, 0);
            }

            public void Update(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens)
            {
                string headerText = model?.GroupName ?? model?.Text ?? string.Empty;
                string themeName = tokens?.ThemeName ?? string.Empty;
                bool changed = !ReferenceEquals(RowModel, model)
                    || !string.Equals(_lastHeaderText, headerText, StringComparison.Ordinal)
                    || !string.Equals(_lastThemeName, themeName, StringComparison.Ordinal);

                RowModel = model;
                _tokens = tokens;
                _lastHeaderText = headerText;
                _lastThemeName = themeName;
                if (changed)
                {
                    Invalidate();
                }
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
            private ComboBoxThemeTokens _tokens;
            private string _lastThemeName = string.Empty;

            public SectionSeparator()
            {
                DoubleBuffered = true;
                Height = 12;
                Margin = Padding.Empty;
            }

            public void UpdateTheme(ComboBoxThemeTokens tokens)
            {
                string themeName = tokens?.ThemeName ?? string.Empty;
                bool changed = !string.Equals(_lastThemeName, themeName, StringComparison.Ordinal);
                _tokens = tokens;
                _lastThemeName = themeName;
                if (changed)
                {
                    Invalidate();
                }
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
            public ComboBoxPopupRowModel RowModel { get; private set; }
            public bool IsSelectable => RowModel.IsEnabled;
            private ComboBoxThemeTokens _tokens;
            private bool _hovered;
            private bool _focused;
            private string _lastMeasuredText = string.Empty;
            private int _lastMeasuredFontHeight = -1;
            private int _cachedMeasuredWidth = 0;
            private string _lastThemeName = string.Empty;
            private string _lastIdentity = string.Empty;
            private bool _lastEnabled;

            public event EventHandler<ComboBoxPopupRowModel> PillClicked;

            public SectionPill()
            {
                DoubleBuffered = true;
                Height = 34;
                Margin = new Padding(4);
                TabStop = false;
            }

            public void Update(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens)
            {
                string identity = BeepComboBox.GetSimpleItemIdentity(model?.SourceItem);
                string themeName = tokens?.ThemeName ?? string.Empty;
                bool enabled = model?.IsEnabled ?? false;
                bool changed = !ReferenceEquals(RowModel, model)
                    || !string.Equals(_lastIdentity, identity, StringComparison.Ordinal)
                    || !string.Equals(_lastThemeName, themeName, StringComparison.Ordinal)
                    || _lastEnabled != enabled;

                RowModel = model;
                _tokens = tokens;
                _hovered = false;
                _focused = false;
                Cursor = model.IsEnabled ? Cursors.Hand : Cursors.Default;
                Width = ResolveWidth(model.Text);
                _lastIdentity = identity;
                _lastThemeName = themeName;
                _lastEnabled = enabled;
                if (changed)
                {
                    Invalidate();
                }
            }

            private int ResolveWidth(string text)
            {
                string safeText = text ?? string.Empty;
                int fontHeight = Font?.Height ?? 0;
                if (!string.Equals(_lastMeasuredText, safeText, StringComparison.Ordinal) || _lastMeasuredFontHeight != fontHeight)
                {
                    var sz = TextRenderer.MeasureText(safeText, Font, Size.Empty, TextFormatFlags.NoPadding);
                    _cachedMeasuredWidth = sz.Width + 28;
                    _lastMeasuredText = safeText;
                    _lastMeasuredFontHeight = fontHeight;
                }

                return _cachedMeasuredWidth > 0 ? _cachedMeasuredWidth : 56;
            }

            public void SetFocused(bool f)
            {
                if (_focused == f)
                {
                    return;
                }

                _focused = f;
                Invalidate();
            }

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
                    back = _tokens.PopupBackColor;
                    border = _tokens.PopupBorderColor;
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
            public ComboBoxPopupRowModel RowModel { get; private set; }
            public bool IsSelectable => RowModel.IsEnabled;
            private ComboBoxThemeTokens _tokens;
            private ComboBoxPopupHostProfile _profile;
            private bool _hovered;
            private bool _focused;
            private string _lastThemeName = string.Empty;
            private int _lastBaseRowHeight = -1;
            private string _lastIdentity = string.Empty;
            private bool _lastEnabled;
            private bool _lastSelected;

            public event EventHandler<ComboBoxPopupRowModel> RowClicked;

            public SectionListRow()
            {
                DoubleBuffered = true;
                Margin = Padding.Empty;
                TabStop = false;
            }

            public void Update(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens, ComboBoxPopupHostProfile profile)
            {
                string identity = BeepComboBox.GetSimpleItemIdentity(model?.SourceItem);
                string themeName = tokens?.ThemeName ?? string.Empty;
                int baseRowHeight = profile?.BaseRowHeight ?? 0;
                bool enabled = model?.IsEnabled ?? false;
                bool selected = model?.IsSelected ?? false;
                bool changed = !ReferenceEquals(RowModel, model)
                    || !string.Equals(_lastIdentity, identity, StringComparison.Ordinal)
                    || !string.Equals(_lastThemeName, themeName, StringComparison.Ordinal)
                    || _lastBaseRowHeight != baseRowHeight
                    || _lastEnabled != enabled
                    || _lastSelected != selected;

                RowModel = model;
                _tokens = tokens;
                _profile = profile;
                _hovered = false;
                _focused = false;
                Cursor = model.IsEnabled ? Cursors.Hand : Cursors.Default;
                Height = model.RowKind == ComboBoxPopupRowKind.WithSubText ? 44 : profile.BaseRowHeight;
                _lastIdentity = identity;
                _lastThemeName = themeName;
                _lastBaseRowHeight = baseRowHeight;
                _lastEnabled = enabled;
                _lastSelected = selected;
                if (changed)
                {
                    Invalidate();
                }
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

                if (!string.IsNullOrWhiteSpace(RowModel.ImagePath))
                {
                    int iconSize = ComboBoxPopupIconRenderer.ComputeAdaptiveIconSize(bounds.Width, bounds.Height, minSize: 14, padding: 8);
                    var iconRect = new Rectangle(bounds.Left + 12, bounds.Top + (bounds.Height - iconSize) / 2, iconSize, iconSize);
                    ComboBoxPopupIconRenderer.PaintRowImage(g, iconRect, RowModel.ImagePath, RowModel.IsEnabled, circular: false, _tokens.DisabledBackColor);
                    textRect = new Rectangle(iconRect.Right + 8, bounds.Top, Math.Max(1, bounds.Right - iconRect.Right - 8), bounds.Height);
                }

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
                    ComboBoxPopupIconRenderer.PaintCheckIcon(g, ckRect, _tokens.FocusBorderColor, 0.95f);
                    textRect.Width = Math.Max(1, ckRect.Left - textRect.Left - 6);
                }

                TextRenderer.DrawText(g, RowModel.Text ?? "", Font, textRect, fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }
    }
}
