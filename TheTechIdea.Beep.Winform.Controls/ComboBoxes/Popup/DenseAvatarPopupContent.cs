using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

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
        private readonly Panel _scrollPanel;
        private readonly List<AvatarRow> _rows = new List<AvatarRow>();
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.DenseList();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private int _keyboardFocusIndex = -1;

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

            _scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0, 2, 0, 2)
            };

            Controls.Add(_scrollPanel);
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
            _scrollPanel.BackColor = _themeTokens.PopupBackColor;
            _searchBox.BackColor = _themeTokens.PopupBackColor;
            _searchBox.ForeColor = _themeTokens.ForeColor;

            foreach (var row in _rows) row.ApplyThemeTokens(_themeTokens);
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _model = model;
            _searchBox.Visible = model.ShowSearchBox || _profile.ForceSearchVisible;

            _scrollPanel.SuspendLayout();
            _scrollPanel.Controls.Clear();
            _rows.Clear();

            if (model.FilteredRows != null)
            {
                int rowWidth = Math.Max(80, _scrollPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
                for (int i = model.FilteredRows.Count - 1; i >= 0; i--)
                {
                    var rowModel = model.FilteredRows[i];
                    if (rowModel.RowKind == ComboBoxPopupRowKind.Separator) continue;

                    var row = new AvatarRow(rowModel, _themeTokens, _profile) { Width = rowWidth, Dock = DockStyle.Top };
                    row.AvatarRowClicked += OnRowClicked;
                    _scrollPanel.Controls.Add(row);
                    _rows.Insert(0, row);
                }
            }

            _scrollPanel.ResumeLayout(true);
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
            int w = Math.Max(80, _scrollPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
            foreach (var r in _rows) r.Width = w;
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
            int start = _keyboardFocusIndex >= 0 ? _keyboardFocusIndex : 0;
            SetKeyboardFocusIndex(start + delta);
        }

        private void CommitFocused()
        {
            if (_keyboardFocusIndex < 0 || _keyboardFocusIndex >= _rows.Count) return;
            OnRowClicked(this, _rows[_keyboardFocusIndex].RowModel);
        }

        private void OnRowClicked(object sender, ComboBoxPopupRowModel row)
        {
            if (row == null || !row.IsEnabled) return;
            if (row.RowKind == ComboBoxPopupRowKind.GroupHeader) return;
            bool close = !(_model?.IsMultiSelect ?? false);
            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(row, close));
        }

        private void EnsureVisible(int index)
        {
            if (index < 0 || index >= _rows.Count) return;
            try { _scrollPanel.ScrollControlIntoView(_rows[index]); } catch { }
        }

        // ──────────────────────────────────────────────────────────────────
        // AvatarRow — row with circular avatar, name text, status indicator dot
        // ──────────────────────────────────────────────────────────────────

        internal sealed class AvatarRow : Control
        {
            public ComboBoxPopupRowModel RowModel { get; }
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

                int avatarSize = 28;
                int avatarX = bounds.Left + 10;
                int avatarY = bounds.Top + (bounds.Height - avatarSize) / 2;
                var avatarRect = new Rectangle(avatarX, avatarY, avatarSize, avatarSize);

                // Circular avatar
                if (!string.IsNullOrWhiteSpace(RowModel.ImagePath))
                {
                    var state = g.Save();
                    using var clipPath = new GraphicsPath();
                    clipPath.AddEllipse(avatarRect);
                    g.SetClip(clipPath, CombineMode.Intersect);
                    StyledImagePainter.Paint(g, avatarRect, RowModel.ImagePath, BeepControlStyle.Minimal);
                    g.Restore(state);
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
                int textX = avatarRect.Right + 10;
                var textRect = new Rectangle(textX, bounds.Top, bounds.Width - textX - 32, bounds.Height);
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
                int dotSize = 10;
                int dotX = bounds.Right - 20;
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
