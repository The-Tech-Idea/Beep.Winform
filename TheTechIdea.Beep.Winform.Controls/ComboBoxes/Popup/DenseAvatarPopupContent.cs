using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Dense popup content with circular avatar photos, name text, and colored
    /// status indicator dots. Used by <see cref="DenseListPopupHostForm"/>.
    /// Reference: "Add guests" people picker with avatar + name + status dot.
    /// </summary>
    internal sealed class DenseAvatarPopupContent : Control, IPopupContentPanel
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
        private readonly List<AvatarRow> _rows = new List<AvatarRow>();
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.DenseList();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private int _keyboardFocusIndex = -1;
        private int _scrollOffset = 0;
        private int _totalContentHeight = 0;
        private string _lastRowSignature = string.Empty;

        public DenseAvatarPopupContent()
        {
            DoubleBuffered = true;
            TabStop = true;

            _searchBox = new BeepTextBox
            {
                Dock = DockStyle.Top,
                PlaceholderText = "Add guests...",
                Visible = false,
                Height = 34
            };
            _searchBox.TextChanged += (s, e) =>
                SearchTextChanged?.Invoke(this, new ComboBoxSearchChangedEventArgs(_searchBox.Text));
            _searchBox.KeyDown += OnSearchKeyDown;

            _scrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 2, 0, 2)
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

            Controls.Add(_scrollContainer);
            Controls.Add(_searchBox);
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.DenseList();
            _searchBox.PlaceholderText = _profile.SearchPlaceholder;
            _searchBox.Height = _profile.SearchBoxHeight;
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _themeTokens = tokens ?? ComboBoxThemeTokens.Fallback();
            BackColor = _themeTokens.PopupBackColor;
            _scrollContainer.BackColor = _themeTokens.PopupBackColor;
            _scrollPanel.BackColor = _themeTokens.PopupBackColor;
            _searchBox.BackColor = _themeTokens.PopupBackColor;
            _searchBox.ForeColor = _themeTokens.ForeColor;
            if (!string.IsNullOrEmpty(_themeTokens.ThemeName))
                _vScrollBar.Theme = _themeTokens.ThemeName;

            foreach (var row in _rows) row.ApplyThemeTokens(_themeTokens);
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _model = model;
            _searchBox.Visible = model.ShowSearchBox || _profile.ForceSearchVisible;

            var filteredRows = model.FilteredRows ?? Array.Empty<ComboBoxPopupRowModel>();
            string rowSignature = ComputeRowSignature(filteredRows);
            bool rowsChanged = !string.Equals(rowSignature, _lastRowSignature, StringComparison.Ordinal);
            if (rowsChanged)
            {
                _scrollPanel.SuspendLayout();
                _scrollOffset = 0;
            }
            int renderableCount = 0;
            for (int i = 0; i < filteredRows.Length; i++)
            {
                if (filteredRows[i].RowKind != ComboBoxPopupRowKind.Separator)
                {
                    renderableCount++;
                }
            }

            if (rowsChanged && renderableCount > 0)
            {
                EnsureRowControlCount(renderableCount);
                int rowWidth = Math.Max(80, _scrollPanel.ClientSize.Width - 4);
                int yPos = 0;
                int rowIndex = 0;
                for (int i = 0; i < filteredRows.Length; i++)
                {
                    var rowModel = filteredRows[i];
                    if (rowModel.RowKind == ComboBoxPopupRowKind.Separator) continue;

                    var row = _rows[rowIndex++];
                    row.Visible = true;
                    row.Width = rowWidth;
                    row.Left = 0;
                    row.Top = yPos;
                    row.UpdateModel(rowModel);
                    yPos += row.Height;
                }

                for (int i = rowIndex; i < _rows.Count; i++)
                {
                    _rows[i].Visible = false;
                }
                _totalContentHeight = yPos;
            }
            else if (rowsChanged)
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    _rows[i].Visible = false;
                }
                _totalContentHeight = 0;
            }

            if (rowsChanged)
            {
                _scrollPanel.ResumeLayout(true);
                _lastRowSignature = rowSignature;
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
        private void OnSearchKeyDown(object sender, KeyEventArgs e) => HandleNav(e);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int w = Math.Max(80, _scrollPanel.ClientSize.Width - 4);
            foreach (var r in _rows) r.Width = w;
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
                _scrollPanel.ClientSize.Height,
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
                var row = new AvatarRow(new ComboBoxPopupRowModel(), _themeTokens, _profile);
                row.AvatarRowClicked += OnRowClicked;
                _scrollPanel.Controls.Add(row);
                _rows.Add(row);
            }
        }

        private static string ComputeRowSignature(IReadOnlyList<ComboBoxPopupRowModel> rows)
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
                  .Append(row.Text ?? string.Empty).Append(':')
                  .Append(row.IsSelected ? '1' : '0')
                  .Append(row.IsEnabled ? '1' : '0')
                  .Append('|');
            }

            return sb.ToString();
        }

        // ──────────────────────────────────────────────────────────────────
        // AvatarRow — row with circular avatar, name text, status indicator dot
        // ──────────────────────────────────────────────────────────────────

        internal sealed class AvatarRow : Control
        {
            public ComboBoxPopupRowModel RowModel { get; private set; }
            private ComboBoxThemeTokens _tokens;
            private readonly ComboBoxPopupHostProfile _profile;
            private bool _hovered;
            private bool _focused;

            public event EventHandler<ComboBoxPopupRowModel> AvatarRowClicked;

            public AvatarRow(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens, ComboBoxPopupHostProfile profile)
            {
                RowModel = model;
                _tokens = tokens;
                _profile = profile;
                DoubleBuffered = true;
                Cursor = model.IsEnabled ? Cursors.Hand : Cursors.Default;
                TabStop = false;
                Margin = Padding.Empty;

                // Group header is taller
                Height = model.RowKind == ComboBoxPopupRowKind.GroupHeader ? 24 : 38;
            }

            public void ApplyThemeTokens(ComboBoxThemeTokens tokens) { _tokens = tokens; Invalidate(); }
            public void SetKeyboardFocused(bool f) { if (_focused != f) { _focused = f; Invalidate(); } }

            public void UpdateModel(ComboBoxPopupRowModel model)
            {
                RowModel = model ?? new ComboBoxPopupRowModel();
                Cursor = ComboBoxPopupRowBehavior.IsSelectable(RowModel) ? Cursors.Hand : Cursors.Default;
                Height = RowModel.RowKind == ComboBoxPopupRowKind.GroupHeader ? 24 : 38;
                Invalidate();
            }

            protected override void OnClick(EventArgs e) { base.OnClick(e); AvatarRowClicked?.Invoke(this, RowModel); }
            protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
            protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var bounds = ClientRectangle;
                if (bounds.Width <= 4 || bounds.Height <= 4) return;

                // Group header
                if (RowModel.RowKind == ComboBoxPopupRowKind.GroupHeader)
                {
                    using var hFont = new Font(Font.FontFamily, Font.Size - 1, FontStyle.Bold);
                    var hRect = new Rectangle(bounds.Left + 12, bounds.Top, bounds.Width - 16, bounds.Height);
                    TextRenderer.DrawText(g, (RowModel.GroupName ?? RowModel.Text ?? "").ToUpperInvariant(), hFont, hRect,
                        _tokens.PopupSubTextColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    return;
                }

                if (ComboBoxPopupStateRowRenderer.TryDrawStateRow(g, bounds, RowModel, Font, _tokens.DisabledForeColor))
                {
                    return;
                }

                // Background
                Color back = _tokens.PopupBackColor;
                if (RowModel.IsSelected || RowModel.RowKind == ComboBoxPopupRowKind.Selected)
                    back = _tokens.PopupRowSelectedColor;
                else if (_focused)
                    back = _tokens.PopupRowFocusColor;
                else if (_hovered)
                    back = _tokens.PopupRowHoverColor;

                using (var bgBrush = new SolidBrush(back))
                    g.FillRectangle(bgBrush, bounds);

                int avatarInset = ComboBoxPopupIconRenderer.ScaleLogical(g, 8);
                int avatarTextGap = ComboBoxPopupIconRenderer.ScaleLogical(g, 8);
                int avatarMinSize = ComboBoxPopupIconRenderer.ScaleLogical(g, 22);
                int avatarPadding = ComboBoxPopupIconRenderer.ScaleLogical(g, 10);
                int avatarSize = ComboBoxPopupIconRenderer.ComputeAdaptiveIconSize(bounds.Width, bounds.Height, avatarMinSize, avatarPadding);
                int avatarX = bounds.Left + avatarInset;
                int avatarY = bounds.Top + (bounds.Height - avatarSize) / 2;
                var avatarRect = new Rectangle(avatarX, avatarY, avatarSize, avatarSize);

                // Circular avatar
                if (!string.IsNullOrWhiteSpace(RowModel.ImagePath))
                {
                    ComboBoxPopupIconRenderer.PaintRowImage(g, avatarRect, RowModel.ImagePath, RowModel.IsEnabled, circular: true, _tokens.DisabledBackColor);
                }
                else
                {
                    // Fallback: colored circle with initial letter
                    Color avatarColor = GetAvatarColor(RowModel.Text);
                    using var avatarBrush = new SolidBrush(avatarColor);
                    g.FillEllipse(avatarBrush, avatarRect);

                    string initial = !string.IsNullOrEmpty(RowModel.Text) ? RowModel.Text[0].ToString().ToUpper() : "?";
                    using var initialFont = new Font(Font.FontFamily, 10, FontStyle.Bold);
                    TextRenderer.DrawText(g, initial, initialFont, avatarRect, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }

                // Avatar ring for selected
                if (RowModel.IsSelected || RowModel.RowKind == ComboBoxPopupRowKind.Selected)
                {
                    using var ringPen = new Pen(_tokens.FocusBorderColor, 2f);
                    g.DrawEllipse(ringPen, avatarRect.Left - 1, avatarRect.Top - 1, avatarRect.Width + 2, avatarRect.Height + 2);
                }

                // Name text
                int textX = avatarRect.Right + avatarTextGap;
                int statusRightInset = ComboBoxPopupIconRenderer.ScaleLogical(g, 10);
                int statusDotSize = ComboBoxPopupIconRenderer.ScaleLogical(g, 10);
                var textRect = new Rectangle(textX, bounds.Top, Math.Max(1, bounds.Right - statusRightInset - statusDotSize - textX - avatarTextGap), bounds.Height);
                Color fore = RowModel.IsEnabled ? _tokens.ForeColor : _tokens.DisabledForeColor;

                if (RowModel.IsSelected)
                {
                    using var boldFont = new Font(Font, FontStyle.Bold);
                    TextRenderer.DrawText(g, RowModel.Text ?? "", boldFont, textRect, fore,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
                else
                {
                    TextRenderer.DrawText(g, RowModel.Text ?? "", Font, textRect, fore,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }

                // Status indicator dot (right side)
                int dotSize = statusDotSize;
                int dotX = bounds.Right - statusRightInset - dotSize;
                int dotY = bounds.Top + (bounds.Height - dotSize) / 2;
                Color dotColor = GetStatusColor(RowModel.SubText);

                using var dotBrush = new SolidBrush(dotColor);
                g.FillEllipse(dotBrush, dotX, dotY, dotSize, dotSize);

                // Checkmark in dot if selected
                if (RowModel.IsSelected || RowModel.RowKind == ComboBoxPopupRowKind.Selected)
                {
                    using var ckPen = new Pen(Color.White, 1.5f);
                    int cx = dotX + dotSize / 2;
                    int cy = dotY + dotSize / 2;
                    g.DrawLine(ckPen, cx - 2, cy, cx - 1, cy + 2);
                    g.DrawLine(ckPen, cx - 1, cy + 2, cx + 3, cy - 2);
                }
            }

            /// <summary>
            /// Maps SubText to a status dot color.
            /// Supported: "online", "offline", "away", "busy", "active" or colors like "#4CAF50".
            /// If SubText is null/empty, uses a default neutral gray.
            /// </summary>
            private static Color GetStatusColor(string status)
            {
                if (string.IsNullOrWhiteSpace(status))
                    return Color.FromArgb(180, 180, 195); // default gray dot

                string s = status.Trim().ToLowerInvariant();
                return s switch
                {
                    "online" or "active" => Color.FromArgb(76, 175, 80),     // green
                    "offline"            => Color.FromArgb(158, 158, 158),    // gray
                    "away"               => Color.FromArgb(255, 152, 0),      // orange
                    "busy" or "dnd"      => Color.FromArgb(244, 67, 54),      // red
                    _ when s.StartsWith("#") => ColorFromHex(s),
                    _ => Color.FromArgb(103, 137, 211)                        // blue-ish default
                };
            }

            private static Color ColorFromHex(string hex)
            {
                try { return ColorTranslator.FromHtml(hex); }
                catch { return Color.FromArgb(180, 180, 195); }
            }

            private static Color GetAvatarColor(string name)
            {
                if (string.IsNullOrEmpty(name)) return Color.FromArgb(158, 158, 158);
                // Simple hash-based color from name
                int hash = 0;
                for (int i = 0; i < name.Length; i++)
                    hash = (hash * 31 + name[i]) & 0x7FFFFFFF;
                Color[] palette =
                {
                    Color.FromArgb(66, 133, 244),   // blue
                    Color.FromArgb(219, 68, 55),     // red
                    Color.FromArgb(244, 180, 0),     // yellow
                    Color.FromArgb(15, 157, 88),     // green
                    Color.FromArgb(171, 71, 188),    // purple
                    Color.FromArgb(0, 172, 193),     // teal
                    Color.FromArgb(255, 112, 67),    // deep orange
                };
                return palette[hash % palette.Length];
            }
        }
    }
}
