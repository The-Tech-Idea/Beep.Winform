using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal class ComboBoxPopupContent : Control, IPopupContentPanel
    {
        public event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;
        public event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;
        public event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;
        public event EventHandler ApplyClicked;
        public event EventHandler CancelClicked;
        public event EventHandler SelectAllClicked;
        public event EventHandler ClearAllClicked;
        
        private ComboBoxPopupModel _model;
        private readonly BeepTextBox _searchBox;
        private readonly FlowLayoutPanel _listPanel;
        private readonly ComboBoxPopupFooter _footer;
        private readonly List<ComboBoxPopupRow> _rows = new List<ComboBoxPopupRow>();
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.OutlineDefault();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private int _keyboardFocusIndex = -1;

        public ComboBoxPopupContent()
        {
            DoubleBuffered = true;
            TabStop = true;
            
            _searchBox = new BeepTextBox { Dock = DockStyle.Top, PlaceholderText = "Search...", Visible = false, Height = 32 };
            _searchBox.TextChanged += (s, e) => SearchTextChanged?.Invoke(this, new ComboBoxSearchChangedEventArgs(_searchBox.Text));
            _searchBox.KeyDown += SearchBoxOnKeyDown;
            Controls.Add(_searchBox);

            _listPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            _listPanel.Padding = Padding.Empty;
            _listPanel.Margin = Padding.Empty;
            Controls.Add(_listPanel);
            
            _footer = new ComboBoxPopupFooter { Visible = false };
            _footer.ApplyClicked += (s, e) => ApplyClicked?.Invoke(this, EventArgs.Empty);
            _footer.CancelClicked += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);
            _footer.SelectAllClicked += (s, e) => SelectAllClicked?.Invoke(this, EventArgs.Empty);
            _footer.ClearAllClicked += (s, e) => ClearAllClicked?.Invoke(this, EventArgs.Empty);
            Controls.Add(_footer);
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _model = model;
            
            _searchBox.Visible = model.ShowSearchBox || _profile.ForceSearchVisible;
            _footer.Visible = model.ShowFooter || _profile.ForceFooterVisible;
            _footer.Setup(model.ShowApplyCancel, model.ShowSelectAll);
            if (model.IsMultiSelect && model.AllRows != null)
            {
                int count = 0;
                foreach (var r in model.AllRows)
                    if (r.IsSelected || r.IsChecked) count++;
                _footer.UpdateSelectedCount(count);
            }
            
            _listPanel.Controls.Clear();
            _rows.Clear();
            if (model.FilteredRows != null)
            {
                foreach (var rowModel in model.FilteredRows)
                {
                    var row = new ComboBoxPopupRow { Width = Math.Max(100, _listPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4), Dock = DockStyle.Top };
                    row.ApplyProfile(_profile);
                    row.ApplyThemeTokens(_themeTokens);
                    row.SetModel(rowModel);
                    row.RowCommitted += OnRowCommitted;
                    _listPanel.Controls.Add(row);
                    _rows.Add(row);
                }
            }

            SetKeyboardFocusIndex(model.KeyboardFocusIndex >= 0 ? model.KeyboardFocusIndex : 0);
        }

        public void FocusSearchBox()
        {
            if (!_searchBox.Visible || !_searchBox.Enabled)
            {
                Focus();
                return;
            }

            _searchBox.Focus();
            _searchBox.SelectionStart = _searchBox.Text?.Length ?? 0;
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.OutlineDefault();
            _searchBox.PlaceholderText = _profile.SearchPlaceholder ?? "Search...";
            _searchBox.Height = _profile.SearchBoxHeight;
            _searchBox.Dock = _profile.SearchPlacement == ComboBoxPopupHostProfile.SearchPlacementMode.Bottom
                ? DockStyle.Bottom
                : DockStyle.Top;
            _footer.ApplyProfile(_profile);
            _listPanel.Padding = new Padding(_profile.ListHorizontalPadding, _profile.ListVerticalPadding, _profile.ListHorizontalPadding, _profile.ListVerticalPadding);

            if (_profile.SearchPlacement == ComboBoxPopupHostProfile.SearchPlacementMode.Bottom)
            {
                Controls.SetChildIndex(_footer, 0);
                Controls.SetChildIndex(_searchBox, 1);
            }
            else
            {
                Controls.SetChildIndex(_searchBox, 0);
                Controls.SetChildIndex(_footer, 2);
            }

            foreach (var row in _rows)
            {
                row.ApplyProfile(_profile);
            }
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _themeTokens = tokens ?? ComboBoxThemeTokens.Fallback();
            BackColor = _themeTokens.PopupBackColor;
            _listPanel.BackColor = _themeTokens.PopupBackColor;
            _searchBox.BackColor = _themeTokens.PopupBackColor;
            _searchBox.ForeColor = _themeTokens.ForeColor;
            _footer.BackColor = _themeTokens.PopupBackColor;
            _footer.ApplyThemeTokens(_themeTokens);

            foreach (var row in _rows)
            {
                row.ApplyThemeTokens(_themeTokens);
            }
        }

        public void UpdateSearchText(string text)
        {
            string safe = text ?? string.Empty;
            if (!string.Equals(_searchBox.Text, safe, StringComparison.Ordinal))
            {
                _searchBox.Text = safe;
            }
        }

        public void SetKeyboardFocusIndex(int index)
        {
            if (_rows.Count == 0)
            {
                _keyboardFocusIndex = -1;
                return;
            }

            int next = Math.Max(0, Math.Min(index, _rows.Count - 1));
            _keyboardFocusIndex = next;
            for (int i = 0; i < _rows.Count; i++)
            {
                _rows[i].SetKeyboardFocused(i == next);
            }

            KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(next));
            EnsureRowVisible(next);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData switch
            {
                Keys.Up => true,
                Keys.Down => true,
                Keys.PageUp => true,
                Keys.PageDown => true,
                Keys.Home => true,
                Keys.End => true,
                Keys.Enter => true,
                _ => base.IsInputKey(keyData)
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_rows.Count == 0)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Down:
                    MoveFocus(1);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    MoveFocus(-1);
                    e.Handled = true;
                    break;
                case Keys.Home:
                    SetKeyboardFocusIndex(0);
                    e.Handled = true;
                    break;
                case Keys.End:
                    SetKeyboardFocusIndex(_rows.Count - 1);
                    e.Handled = true;
                    break;
                case Keys.PageDown:
                    MoveFocus(6);
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                    MoveFocus(-6);
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    CommitFocusedRow();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int width = Math.Max(100, _listPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
            foreach (var row in _rows)
            {
                row.Width = width;
            }
        }

        private void SearchBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    MoveFocus(1);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    MoveFocus(-1);
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    CommitFocusedRow();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.PageDown:
                    MoveFocus(6);
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                    MoveFocus(-6);
                    e.Handled = true;
                    break;
                case Keys.Home:
                    SetKeyboardFocusIndex(0);
                    e.Handled = true;
                    break;
                case Keys.End:
                    SetKeyboardFocusIndex(_rows.Count - 1);
                    e.Handled = true;
                    break;
            }
        }

        private void MoveFocus(int delta)
        {
            if (_rows.Count == 0)
            {
                return;
            }

            int start = _keyboardFocusIndex >= 0 ? _keyboardFocusIndex : 0;
            SetKeyboardFocusIndex(start + delta);
        }

        private void CommitFocusedRow()
        {
            if (_keyboardFocusIndex < 0 || _keyboardFocusIndex >= _rows.Count)
            {
                return;
            }

            ComboBoxPopupRow row = _rows[_keyboardFocusIndex];
            var model = row.Model;
            if (model == null)
            {
                return;
            }

            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(model, !_model.IsMultiSelect));
        }

        private void OnRowCommitted(object sender, ComboBoxPopupRowModel rowModel)
        {
            if (rowModel == null)
            {
                return;
            }

            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(rowModel, !_model.IsMultiSelect));
        }

        private void EnsureRowVisible(int index)
        {
            if (index < 0 || index >= _rows.Count)
            {
                return;
            }

            try
            {
                _listPanel.ScrollControlIntoView(_rows[index]);
            }
            catch
            {
                // Best-effort scrolling only.
            }
        }
    }
}
