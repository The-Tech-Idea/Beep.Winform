using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Popup content panel that renders items as clickable pill buttons in a
    /// wrap-flow grid layout. Used by <see cref="RoundedPillPopupHostForm"/>.
    /// Reference: Tags picker UX — pills arranged in rows, click to select/toggle.
    /// </summary>
    internal sealed class PillGridPopupContent : Control, IPopupContentPanel
    {
        public event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;
        public event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;
        public event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;
        public event EventHandler ApplyClicked;
        public event EventHandler CancelClicked;

        private ComboBoxPopupModel _model;
        private readonly FlowLayoutPanel _pillPanel;
        private readonly BeepTextBox _searchBox;
        private readonly List<PillButton> _pills = new List<PillButton>();
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.RoundedPill();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private int _keyboardFocusIndex = -1;

        public PillGridPopupContent()
        {
            DoubleBuffered = true;
            TabStop = true;

            _searchBox = new BeepTextBox
            {
                Dock = DockStyle.Bottom,
                PlaceholderText = "Search tags...",
                Visible = false,
                Height = 34
            };
            _searchBox.TextChanged += (s, e) =>
                SearchTextChanged?.Invoke(this, new ComboBoxSearchChangedEventArgs(_searchBox.Text));
            _searchBox.KeyDown += OnSearchKeyDown;

            _pillPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10, 10, 10, 10)
            };

            // Add search first (Bottom), then pills (Fill) — WinForms dock order
            Controls.Add(_searchBox);
            Controls.Add(_pillPanel);
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.RoundedPill();
            _searchBox.PlaceholderText = _profile.SearchPlaceholder;
            _searchBox.Height = _profile.SearchBoxHeight;

            if (_profile.SearchPlacement == ComboBoxPopupHostProfile.SearchPlacementMode.Bottom)
            {
                _searchBox.Dock = DockStyle.Bottom;
                Controls.SetChildIndex(_searchBox, 0);
                Controls.SetChildIndex(_pillPanel, 1);
            }
            else
            {
                _searchBox.Dock = DockStyle.Top;
                Controls.SetChildIndex(_searchBox, 0);
                Controls.SetChildIndex(_pillPanel, 1);
            }
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _themeTokens = tokens ?? ComboBoxThemeTokens.Fallback();
            BackColor = _themeTokens.PopupBackColor;
            _pillPanel.BackColor = _themeTokens.PopupBackColor;
            _searchBox.BackColor = _themeTokens.PopupBackColor;
            _searchBox.ForeColor = _themeTokens.ForeColor;

            foreach (var pill in _pills)
                pill.ApplyThemeTokens(_themeTokens);
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _model = model;
            _searchBox.Visible = model.ShowSearchBox || _profile.ForceSearchVisible;

            _pillPanel.SuspendLayout();
            _pillPanel.Controls.Clear();
            _pills.Clear();

            if (model.FilteredRows != null)
            {
                ComboBoxPopupRowModel stateRow = null;
                foreach (var row in model.FilteredRows)
                {
                    if (ComboBoxPopupRowBehavior.IsStateRow(row))
                    {
                        stateRow = row;
                        continue;
                    }

                    if (!ComboBoxPopupRowBehavior.IsSelectable(row))
                        continue;

                    var pill = new PillButton(row, _themeTokens, _profile);
                    pill.PillClicked += OnPillClicked;
                    _pillPanel.Controls.Add(pill);
                    _pills.Add(pill);
                }

                if (_pills.Count == 0 && stateRow != null)
                {
                    var info = new Label
                    {
                        AutoSize = false,
                        Width = Math.Max(120, _pillPanel.ClientSize.Width - 24),
                        Height = 34,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = string.IsNullOrWhiteSpace(stateRow.Text) ? "No items" : stateRow.Text,
                        ForeColor = _themeTokens.DisabledForeColor,
                        BackColor = Color.Transparent,
                        Margin = new Padding(6)
                    };
                    _pillPanel.Controls.Add(info);
                }
            }

            _pillPanel.ResumeLayout(true);
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
            if (_pills.Count == 0) { _keyboardFocusIndex = -1; return; }

            int next = Math.Max(0, Math.Min(index, _pills.Count - 1));
            _keyboardFocusIndex = next;

            for (int i = 0; i < _pills.Count; i++)
                _pills[i].SetFocused(i == next);

            KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(next));

            if (_keyboardFocusIndex >= 0 && _keyboardFocusIndex < _pills.Count)
            {
                try { _pillPanel.ScrollControlIntoView(_pills[_keyboardFocusIndex]); }
                catch { /* best-effort scroll */ }
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
            if (item == null || _pills.Count == 0) return;
            var rowModels = new List<ComboBoxPopupRowModel>(_pills.Count);
            for (int i = 0; i < _pills.Count; i++)
            {
                rowModels.Add(_pills[i].RowModel);
            }

            int index = ComboBoxPopupFocusHelper.FindRowIndexByItem(rowModels, item);
            if (index >= 0)
            {
                SetKeyboardFocusIndex(index);
            }
        }

        /// <summary>
        /// Estimates ideal popup height for the pill grid.
        /// </summary>
        public int EstimateHeight(int availableWidth)
        {
            if (_pills.Count == 0) return _profile.SearchBoxHeight + 60;

            int padding = 20; // horizontal padding
            int usable = Math.Max(80, availableWidth - padding);
            int spacing = _profile.PillSpacing > 0 ? _profile.PillSpacing : 4;
            int pillH = _profile.PillHeight > 0 ? _profile.PillHeight : 36;

            int x = 0;
            int rows = 1;

            foreach (var pill in _pills)
            {
                int w = pill.Width + spacing * 2;
                if (x + w > usable && x > 0)
                {
                    rows++;
                    x = 0;
                }
                x += w;
            }

            int content = rows * (pillH + spacing * 2) + 20;
            if (_searchBox.Visible) content += _profile.SearchBoxHeight;
            return content;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData switch
            {
                Keys.Right or Keys.Left or Keys.Up or Keys.Down or Keys.Enter => true,
                _ => base.IsInputKey(keyData)
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            HandleNavKey(e);
        }

        private void OnSearchKeyDown(object sender, KeyEventArgs e) => HandleNavKey(e);

        private void HandleNavKey(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Down:
                    MoveFocus(1); e.Handled = true; break;
                case Keys.Left:
                case Keys.Up:
                    MoveFocus(-1); e.Handled = true; break;
                case Keys.Home:
                    SetKeyboardFocusIndex(0); e.Handled = true; break;
                case Keys.End:
                    SetKeyboardFocusIndex(_pills.Count - 1); e.Handled = true; break;
                case Keys.Enter:
                    CommitFocused(); e.Handled = true; e.SuppressKeyPress = true; break;
            }
        }

        private void MoveFocus(int delta)
        {
            SetKeyboardFocusIndex(ComboBoxPopupNavigationHelper.ShiftIndex(_keyboardFocusIndex, delta, _pills.Count));
        }

        private void CommitFocused()
        {
            if (_keyboardFocusIndex >= 0 && _keyboardFocusIndex < _pills.Count)
                OnPillClicked(this, _pills[_keyboardFocusIndex].RowModel);
        }

        private void OnPillClicked(object sender, ComboBoxPopupRowModel row)
        {
            if (!ComboBoxPopupRowBehavior.IsSelectable(row)) return;
            bool close = !(_model?.IsMultiSelect ?? false);
            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(row, close));
        }

        // ──────────────────────────────────────────────────────────────────
        // Pill Button — renders a single item as a rounded pill
        // ──────────────────────────────────────────────────────────────────

        internal sealed class PillButton : Control
        {
            public ComboBoxPopupRowModel RowModel { get; }
            private ComboBoxThemeTokens _tokens;
            private readonly ComboBoxPopupHostProfile _profile;
            private bool _hovered;
            private bool _focused;

            public event EventHandler<ComboBoxPopupRowModel> PillClicked;

            public PillButton(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens, ComboBoxPopupHostProfile profile)
            {
                RowModel = model;
                _tokens = tokens;
                _profile = profile;
                DoubleBuffered = true;
                Cursor = ComboBoxPopupRowBehavior.IsSelectable(model) ? Cursors.Hand : Cursors.Default;
                TabStop = false;

                int h = profile.PillHeight > 0 ? profile.PillHeight : 36;
                Height = h;
                int spacing = profile.PillSpacing > 0 ? profile.PillSpacing : 4;
                Margin = new Padding(spacing);

                var textSize = TextRenderer.MeasureText(model.Text ?? "", Font, Size.Empty, TextFormatFlags.NoPadding);
                Width = textSize.Width + 28;
            }

            public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
            {
                _tokens = tokens;
                Invalidate();
            }

            public void SetFocused(bool f)
            {
                if (_focused != f) { _focused = f; Invalidate(); }
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                PillClicked?.Invoke(this, RowModel);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                _hovered = true;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                _hovered = false;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = ClientRectangle;
                rect.Inflate(-1, -1);
                if (rect.Width <= 0 || rect.Height <= 0) return;

                int radius = Math.Min(rect.Height / 2, 18);

                Color back, border, fore;

                if (RowModel.IsSelected || RowModel.IsChecked)
                {
                    back = _tokens.PopupRowSelectedColor;
                    border = _tokens.FocusBorderColor;
                    fore = _tokens.SelectedForeColor;
                }
                else if (_hovered)
                {
                    back = _tokens.PopupRowHoverColor;
                    border = Color.FromArgb(180, _tokens.PopupSeparatorColor);
                    fore = _tokens.ForeColor;
                }
                else if (_focused)
                {
                    back = _tokens.PopupRowFocusColor;
                    border = _tokens.FocusBorderColor;
                    fore = _tokens.ForeColor;
                }
                else
                {
                    back = _tokens.PopupRowBackColor;
                    border = _tokens.PopupRowBorderColor;
                    fore = _tokens.ForeColor;
                }

                using var path = CreateRoundRectPath(rect, radius);
                using (var brush = new SolidBrush(back))
                    g.FillPath(brush, path);
                using (var pen = new Pen(border))
                    g.DrawPath(pen, path);

                TextRenderer.DrawText(g, RowModel.Text ?? "", Font, rect, fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
