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
    /// Popup content that renders each item as a rounded card with subtle border
    /// and shadow on hover. Used by <see cref="FilledSoftPopupHostForm"/>.
    /// Each row is an inset card with rounded corners — not a flat list row.
    /// Reference: filled/soft dropdown variants with card-style rows.
    /// </summary>
    internal sealed class CardRowPopupContent : Control, IPopupContentPanel
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
        private readonly List<CardRow> _cards = new List<CardRow>();
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.FilledSoft();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private int _keyboardFocusIndex = -1;
        private int _scrollOffset = 0;
        private int _totalContentHeight = 0;

        public CardRowPopupContent()
        {
            DoubleBuffered = true;
            TabStop = true;

            _searchBox = new BeepTextBox
            {
                Dock = DockStyle.Top,
                PlaceholderText = "Find...",
                Visible = false,
                Height = 34
            };
            _searchBox.TextChanged += (s, e) =>
                SearchTextChanged?.Invoke(this, new ComboBoxSearchChangedEventArgs(_searchBox.Text));
            _searchBox.KeyDown += OnSearchKeyDown;

            _scrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(6, 6, 6, 6)
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

            Controls.Add(_searchBox);
            Controls.Add(_scrollContainer);
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.FilledSoft();
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

            foreach (var card in _cards) card.ApplyThemeTokens(_themeTokens);
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _model = model;
            _searchBox.Visible = model.ShowSearchBox || _profile.ForceSearchVisible;

            _scrollPanel.SuspendLayout();
            _scrollPanel.Controls.Clear();
            _cards.Clear();
            _scrollOffset = 0;

            if (model.FilteredRows != null)
            {
                int cardWidth = Math.Max(80, _scrollPanel.ClientSize.Width - 8);
                int yPos = 0;

                foreach (var rowModel in model.FilteredRows)
                {
                    var card = new CardRow(rowModel, _themeTokens, _profile)
                    {
                        Width = cardWidth,
                        Left = 0,
                        Top = yPos
                    };
                    card.CardClicked += OnCardClicked;
                    _scrollPanel.Controls.Add(card);
                    _cards.Add(card);
                    yPos += card.Height + card.Margin.Vertical;
                }
                _totalContentHeight = yPos;
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
            if (_cards.Count == 0) { _keyboardFocusIndex = -1; return; }
            int next = Math.Max(0, Math.Min(index, _cards.Count - 1));
            _keyboardFocusIndex = next;
            for (int i = 0; i < _cards.Count; i++)
                _cards[i].SetKeyboardFocused(i == next);
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int w = Math.Max(80, _scrollPanel.ClientSize.Width - 8);
            foreach (var c in _cards) c.Width = w;
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
            foreach (var card in _cards)
            {
                card.Top = yPos;
                yPos += card.Height + card.Margin.Vertical;
            }
        }

        private void OnSearchKeyDown(object sender, KeyEventArgs e) => HandleNav(e);

        private void HandleNav(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down: MoveFocus(1); e.Handled = true; break;
                case Keys.Up: MoveFocus(-1); e.Handled = true; break;
                case Keys.PageDown: MoveFocus(6); e.Handled = true; break;
                case Keys.PageUp: MoveFocus(-6); e.Handled = true; break;
                case Keys.Home: SetKeyboardFocusIndex(0); e.Handled = true; break;
                case Keys.End: SetKeyboardFocusIndex(_cards.Count - 1); e.Handled = true; break;
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
            if (_keyboardFocusIndex < 0 || _keyboardFocusIndex >= _cards.Count) return;
            OnCardClicked(this, _cards[_keyboardFocusIndex].RowModel);
        }

        private void OnCardClicked(object sender, ComboBoxPopupRowModel row)
        {
            if (row == null || !row.IsEnabled) return;
            if (row.RowKind == ComboBoxPopupRowKind.GroupHeader || row.RowKind == ComboBoxPopupRowKind.Separator) return;
            bool close = !(_model?.IsMultiSelect ?? false);
            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(row, close));
        }

        private void EnsureVisible(int index)
        {
            if (index < 0 || index >= _cards.Count) return;
            if (!_vScrollBar.Visible) return;

            var card = _cards[index];
            int cardTop = 0;
            for (int i = 0; i < index; i++)
                cardTop += _cards[i].Height + _cards[i].Margin.Vertical;
            int cardBottom = cardTop + card.Height;
            int viewportHeight = _scrollPanel.ClientSize.Height;

            if (cardTop < _scrollOffset)
            {
                _scrollOffset = cardTop;
            }
            else if (cardBottom > _scrollOffset + viewportHeight)
            {
                _scrollOffset = cardBottom - viewportHeight;
            }
            else
            {
                return;
            }

            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, _vScrollBar.Maximum));
            _vScrollBar.Value = _scrollOffset;
            ApplyScrollOffset();
        }

        // ──────────────────────────────────────────────────────────────────
        // CardRow — renders one item as a rounded card with border and shadow
        // ──────────────────────────────────────────────────────────────────

        internal sealed class CardRow : Control
        {
            public ComboBoxPopupRowModel RowModel { get; }
            private ComboBoxThemeTokens _tokens;
            private readonly ComboBoxPopupHostProfile _profile;
            private bool _hovered;
            private bool _focused;

            public event EventHandler<ComboBoxPopupRowModel> CardClicked;

            public CardRow(ComboBoxPopupRowModel model, ComboBoxThemeTokens tokens, ComboBoxPopupHostProfile profile)
            {
                RowModel = model;
                _tokens = tokens;
                _profile = profile;
                DoubleBuffered = true;
                Cursor = model.IsEnabled ? Cursors.Hand : Cursors.Default;
                TabStop = false;

                int baseH = model.RowKind == ComboBoxPopupRowKind.WithSubText ? 52
                          : model.RowKind == ComboBoxPopupRowKind.GroupHeader ? 28
                          : model.RowKind == ComboBoxPopupRowKind.Separator ? 10
                          : (profile.BaseRowHeight + 8);
                Height = baseH;
                Margin = new Padding(0, 2, 0, 2);
            }

            public void ApplyThemeTokens(ComboBoxThemeTokens tokens) { _tokens = tokens; Invalidate(); }

            public void SetKeyboardFocused(bool f) { if (_focused != f) { _focused = f; Invalidate(); } }

            protected override void OnClick(EventArgs e) { base.OnClick(e); CardClicked?.Invoke(this, RowModel); }
            protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
            protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var bounds = ClientRectangle;
                if (bounds.Width <= 4 || bounds.Height <= 4) return;

                // Separator row — draw a thin line
                if (RowModel.RowKind == ComboBoxPopupRowKind.Separator)
                {
                    int y = bounds.Top + bounds.Height / 2;
                    using var pen = new Pen(Color.FromArgb(60, _tokens.PopupSeparatorColor), 1f) { DashStyle = DashStyle.Dot };
                    g.DrawLine(pen, bounds.Left + 12, y, bounds.Right - 12, y);
                    return;
                }

                // Group header — bold text, no card
                if (RowModel.RowKind == ComboBoxPopupRowKind.GroupHeader)
                {
                    using var headerFont = new Font(Font, FontStyle.Bold);
                    TextRenderer.DrawText(g, RowModel.GroupName ?? RowModel.Text ?? "", headerFont, bounds,
                        _tokens.PopupGroupHeaderFore, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    return;
                }

                // Card inset: 4px from each edge
                var cardRect = Rectangle.Inflate(bounds, -4, -1);
                int radius = 8;

                // Card colors
                Color cardBack, cardBorder;
                if (!RowModel.IsEnabled)
                {
                    cardBack = _tokens.DisabledBackColor;
                    cardBorder = Color.FromArgb(80, _tokens.BorderColor);
                }
                else if (RowModel.IsSelected || RowModel.RowKind == ComboBoxPopupRowKind.Selected)
                {
                    cardBack = _tokens.PopupRowSelectedColor;
                    cardBorder = _tokens.FocusBorderColor;
                }
                else if (_focused)
                {
                    cardBack = _tokens.PopupRowFocusColor;
                    cardBorder = Color.FromArgb(160, _tokens.FocusBorderColor);
                }
                else if (_hovered)
                {
                    cardBack = _tokens.PopupRowHoverColor;
                    cardBorder = Color.FromArgb(120, _tokens.HoverBorderColor);
                }
                else
                {
                    cardBack = Color.FromArgb(252, 253, 255);
                    cardBorder = Color.FromArgb(80, _tokens.PopupSeparatorColor);
                }

                // Draw card shadow on hover
                if (_hovered && RowModel.IsEnabled)
                {
                    var shadowRect = cardRect;
                    shadowRect.Offset(0, 2);
                    using var shadowPath = CreateRoundRect(shadowRect, radius);
                    using var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0));
                    g.FillPath(shadowBrush, shadowPath);
                }

                // Draw card
                using var cardPath = CreateRoundRect(cardRect, radius);
                using (var brush = new SolidBrush(cardBack))
                    g.FillPath(brush, cardPath);
                using (var pen = new Pen(cardBorder))
                    g.DrawPath(pen, cardPath);

                // Content layout inside card
                var contentRect = new Rectangle(cardRect.Left + 12, cardRect.Top, cardRect.Width - 24, cardRect.Height);
                Color fore = RowModel.IsEnabled ? _tokens.ForeColor : _tokens.DisabledForeColor;

                // Icon
                if (!string.IsNullOrWhiteSpace(RowModel.ImagePath))
                {
                    int imgSize = 20;
                    var iconRect = new Rectangle(contentRect.Left, contentRect.Top + (contentRect.Height - imgSize) / 2, imgSize, imgSize);
                    StyledImagePainter.Paint(g, iconRect, RowModel.ImagePath, BeepControlStyle.Minimal);
                    contentRect = new Rectangle(iconRect.Right + 8, contentRect.Top, contentRect.Width - imgSize - 8, contentRect.Height);
                }

                // Checkmark for selected single-select
                if (_profile.ShowCheckmarkForSelected && (RowModel.IsSelected || RowModel.RowKind == ComboBoxPopupRowKind.Selected))
                {
                    var checkRect = new Rectangle(cardRect.Right - 28, cardRect.Top + (cardRect.Height - 14) / 2, 14, 14);
                    using var checkPen = new Pen(_tokens.FocusBorderColor, 2f);
                    g.DrawLine(checkPen, checkRect.Left + 2, checkRect.Top + 7, checkRect.Left + 5, checkRect.Bottom - 2);
                    g.DrawLine(checkPen, checkRect.Left + 5, checkRect.Bottom - 2, checkRect.Right - 2, checkRect.Top + 2);
                    contentRect.Width = Math.Max(1, checkRect.Left - contentRect.Left - 6);
                }

                // Text
                if (RowModel.RowKind == ComboBoxPopupRowKind.WithSubText && !string.IsNullOrEmpty(RowModel.SubText))
                {
                    var titleRect = new Rectangle(contentRect.Left, contentRect.Top + 6, contentRect.Width, contentRect.Height / 2);
                    var subRect = new Rectangle(contentRect.Left, titleRect.Bottom - 2, contentRect.Width, contentRect.Height - titleRect.Height - 4);
                    TextRenderer.DrawText(g, RowModel.Text ?? "", Font, titleRect, fore,
                        TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                    TextRenderer.DrawText(g, RowModel.SubText, Font, subRect, _tokens.PopupSubTextColor,
                        TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
                else
                {
                    TextRenderer.DrawText(g, RowModel.Text ?? "", Font, contentRect, fore,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
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
