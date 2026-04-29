using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Ultra-minimal popup content — text-only rows, no icons, no checkmarks,
    /// no borders between rows, no footer. Relies on popup shadow for visual
    /// separation. Used by <see cref="MinimalBorderlessPopupHostForm"/>.
    /// Reference: floating "Select your speciality" panel with clean text rows.
    /// </summary>
    internal sealed class MinimalCleanPopupContent : Control, IPopupContentPanel
    {
        public event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;
        public event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;
        public event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;
        public event EventHandler ApplyClicked;
        public event EventHandler CancelClicked;

        private ComboBoxPopupModel _model;
        private readonly Panel _scrollContainer;
        private readonly Panel _listPanel;
        private readonly BeepScrollBar _vScrollBar;
        private readonly List<MinimalRow> _rows = new List<MinimalRow>();
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.MinimalBorderless();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private int _keyboardFocusIndex = -1;
        private int _scrollOffset = 0;
        private int _totalContentHeight = 0;

        public MinimalCleanPopupContent()
        {
            DoubleBuffered = true;
            TabStop = true;

            _scrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(4, 8, 4, 8)
            };

            _listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false
            };
            _listPanel.MouseWheel += OnListPanelMouseWheel;

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

            _scrollContainer.Controls.Add(_listPanel);
            _scrollContainer.Controls.Add(_vScrollBar);
            Controls.Add(_scrollContainer);
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.MinimalBorderless();
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _themeTokens = tokens ?? ComboBoxThemeTokens.Fallback();
            BackColor = _themeTokens.BackColor;
            _scrollContainer.BackColor = _themeTokens.BackColor;
            _listPanel.BackColor = _themeTokens.BackColor;
            if (!string.IsNullOrEmpty(_themeTokens.ThemeName))
                _vScrollBar.Theme = _themeTokens.ThemeName;
            foreach (var row in _rows) row.ApplyThemeTokens(_themeTokens);
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _model = model;

            _listPanel.SuspendLayout();
            _scrollOffset = 0;

            var filteredRows = model.FilteredRows ?? Array.Empty<ComboBoxPopupRowModel>();
            if (filteredRows.Count > 0)
            {
                int rowWidth = Math.Max(80, _listPanel.ClientSize.Width - 8);
                int yPos = 0;

                EnsureRowControlCount(filteredRows.Count);
                for (int i = 0; i < filteredRows.Count; i++)
                {
                    var row = _rows[i];
                    row.Visible = true;
                    row.Width = rowWidth;
                    row.Left = 0;
                    row.Top = yPos;
                    row.UpdateModel(filteredRows[i]);
                    yPos += row.Height;
                }

                for (int i = filteredRows.Count; i < _rows.Count; i++)
                {
                    _rows[i].Visible = false;
                }
                _totalContentHeight = yPos;
            }
            else
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    _rows[i].Visible = false;
                }
                _totalContentHeight = 0;
            }

            _listPanel.ResumeLayout(true);
            UpdateScrollBar();
            SetKeyboardFocusIndex(model.KeyboardFocusIndex >= 0 ? model.KeyboardFocusIndex : 0);
        }

        public void UpdateSearchText(string text)
        {
            // Minimal variant has no search box — no-op
        }

        public void SetKeyboardFocusIndex(int index)
        {
            if (_rows.Count == 0) { _keyboardFocusIndex = -1; return; }
            int next = Math.Max(0, Math.Min(index, _rows.Count - 1));
            _keyboardFocusIndex = next;
            for (int i = 0; i < _rows.Count; i++)
                _rows[i].SetKeyboardFocused(i == next);
            KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(next));
            EnsureVisible(next);
        }

        public void FocusSearchBox()
        {
            // No search box — just focus the control itself for keyboard nav
            Focus();
        }

        public void FocusItem(SimpleItem item)
        {
            if (item == null || _rows.Count == 0) return;
            var rowModels = new List<ComboBoxPopupRowModel>(_rows.Count);
            for (int i = 0; i < _rows.Count; i++)
            {
                rowModels.Add(_rows[i].RowModel);
            }

            int index = ComboBoxPopupFocusHelper.FindRowIndexByItem(rowModels, item);
            if (index >= 0)
            {
                SetKeyboardFocusIndex(index);
                EnsureVisible(index);
            }
        }

        protected override bool IsInputKey(Keys keyData) => keyData switch
        {
            Keys.Up or Keys.Down or Keys.Home or Keys.End
                or Keys.PageUp or Keys.PageDown or Keys.Enter => true,
            _ => base.IsInputKey(keyData)
        };

        protected override void OnKeyDown(KeyEventArgs e) { base.OnKeyDown(e); HandleNav(e); }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int w = Math.Max(80, _listPanel.ClientSize.Width - 8);
            foreach (var r in _rows) r.Width = w;
            UpdateScrollBar();
            ApplyScrollOffset();
        }

        private void UpdateScrollBar()
        {
            int viewportHeight = _listPanel.ClientSize.Height;
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
            foreach (var row in _rows)
            {
                row.Top = yPos;
                yPos += row.Height;
            }
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
                case Keys.End: SetKeyboardFocusIndex(_rows.Count - 1); e.Handled = true; break;
                case Keys.Enter: CommitFocused(); e.Handled = true; e.SuppressKeyPress = true; break;
            }
        }

        private void MoveFocus(int delta)
        {
            SetKeyboardFocusIndex(ComboBoxPopupNavigationHelper.ShiftIndex(_keyboardFocusIndex, delta, _rows.Count));
        }

        private void CommitFocused()
        {
            if (_keyboardFocusIndex < 0 || _keyboardFocusIndex >= _rows.Count) return;
            OnRowClicked(this, _rows[_keyboardFocusIndex].RowModel);
        }

        private void OnRowClicked(object sender, ComboBoxPopupRowModel row)
        {
            if (!ComboBoxPopupRowBehavior.IsSelectable(row)) return;
            bool close = !(_model?.IsMultiSelect ?? false);
            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(row, close));
        }

        private void EnsureVisible(int index)
        {
            if (index < 0 || index >= _rows.Count) return;
            if (!_vScrollBar.Visible) return;

            var row = _rows[index];
            int rowTop = 0;
            for (int i = 0; i < index; i++)
                rowTop += _rows[i].Height;
            int next = ComboBoxPopupScrollHelper.ComputeAdjustedScrollOffset(
                _scrollOffset,
                rowTop,
                row.Height,
                _listPanel.ClientSize.Height,
                _vScrollBar.Maximum);
            if (next == _scrollOffset)
            {
                return;
            }
            _scrollOffset = next;
            _vScrollBar.Value = _scrollOffset;
            ApplyScrollOffset();
        }

        private void EnsureRowControlCount(int requiredCount)
        {
            while (_rows.Count < requiredCount)
            {
                var row = new MinimalRow(new ComboBoxPopupRowModel(), _themeTokens, _profile);
                row.MinimalRowClicked += OnRowClicked;
                _listPanel.Controls.Add(row);
                _rows.Add(row);
            }
        }

        // ──────────────────────────────────────────────────────────────────
        // MinimalRow — renders text-only with very subtle hover, no icons
        // ──────────────────────────────────────────────────────────────────

        internal sealed class MinimalRow : Control
        {
            public ComboBoxPopupRowModel RowModel { get; private set; }
            private ComboBoxThemeTokens _tokens;
            private readonly ComboBoxPopupHostProfile _profile;
            private bool _hovered;
            private bool _focused;

            public event EventHandler<ComboBoxPopupRowModel> MinimalRowClicked;

            public MinimalRow(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens, ComboBoxPopupHostProfile profile)
            {
                RowModel = model;
                _tokens = tokens;
                _profile = profile;
                DoubleBuffered = true;
                Cursor = ComboBoxPopupRowBehavior.IsSelectable(model)
                    ? Cursors.Hand : Cursors.Default;
                TabStop = false;
                Margin = Padding.Empty;

                Height = model.RowKind switch
                {
                    ComboBoxPopupRowKind.GroupHeader => 28,
                    ComboBoxPopupRowKind.Separator => 8,
                    _ => profile.BaseRowHeight
                };
            }

            public void UpdateModel(ComboBoxPopupRowModel model)
            {
                RowModel = model ?? new ComboBoxPopupRowModel();
                Cursor = ComboBoxPopupRowBehavior.IsSelectable(RowModel) ? Cursors.Hand : Cursors.Default;
                Height = RowModel.RowKind switch
                {
                    ComboBoxPopupRowKind.GroupHeader => 28,
                    ComboBoxPopupRowKind.Separator => 8,
                    _ => _profile.BaseRowHeight
                };
                Invalidate();
            }

            public void ApplyThemeTokens(ComboBoxThemeTokens tokens) { _tokens = tokens; Invalidate(); }
            public void SetKeyboardFocused(bool f) { if (_focused != f) { _focused = f; Invalidate(); } }

            protected override void OnClick(EventArgs e) { base.OnClick(e); MinimalRowClicked?.Invoke(this, RowModel); }
            protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
            protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var bounds = ClientRectangle;
                if (bounds.Width <= 4 || bounds.Height <= 4) return;

                // Separator — thin subtle line
                if (RowModel.RowKind == ComboBoxPopupRowKind.Separator)
                {
                    int y = bounds.Top + bounds.Height / 2;
                    using var pen = new Pen(_tokens.PopupSeparatorColor);
                    g.DrawLine(pen, bounds.Left + 16, y, bounds.Right - 16, y);
                    return;
                }

                // Group header — small caps, muted
                if (RowModel.RowKind == ComboBoxPopupRowKind.GroupHeader)
                {
                    var hRect = new Rectangle(bounds.Left + 16, bounds.Top, bounds.Width - 32, bounds.Height);
                    using var hFont = new Font(Font.FontFamily, Font.Size - 1f, FontStyle.Regular);
                    TextRenderer.DrawText(g, (RowModel.GroupName ?? RowModel.Text ?? "").ToUpperInvariant(), hFont, hRect,
                        _tokens.PopupMutedForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    return;
                }

                if (ComboBoxPopupStateRowRenderer.TryDrawStateRow(g, bounds, RowModel, Font, _tokens.DisabledForeColor))
                {
                    return;
                }

                // Normal row — ultra-clean with rounded hover rect
                var rowRect = new Rectangle(bounds.Left + 4, bounds.Top + 1, bounds.Width - 8, bounds.Height - 2);
                int radius = 6;

                Color back = Color.Transparent;
                if (RowModel.IsSelected || RowModel.RowKind == ComboBoxPopupRowKind.Selected)
                    back = _tokens.PopupRowSelectedColor;
                else if (_focused)
                    back = _tokens.PopupRowFocusColor;
                else if (_hovered)
                    back = _tokens.PopupRowHoverColor;

                if (back != Color.Transparent)
                {
                    using var path = CreateRoundRect(rowRect, radius);
                    using var brush = new SolidBrush(back);
                    g.FillPath(brush, path);
                }

                // Text — generous left padding, clean sans-serif
                var textRect = new Rectangle(rowRect.Left + 16, rowRect.Top, rowRect.Width - 24, rowRect.Height);
                Color fore = !RowModel.IsEnabled
                    ? _tokens.DisabledForeColor
                    : (RowModel.IsSelected ? _tokens.FocusBorderColor : _tokens.ForeColor);

                Font textFont = RowModel.IsSelected ? new Font(Font, FontStyle.Bold) : Font;
                TextRenderer.DrawText(g, RowModel.Text ?? "", textFont, textRect, fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                if (RowModel.IsSelected && textFont != Font) textFont.Dispose();
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
    }
}
