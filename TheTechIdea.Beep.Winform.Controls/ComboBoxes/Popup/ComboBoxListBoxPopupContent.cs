using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Unified popup content that hosts a BeepListBox, replacing legacy popup row renderers.
    /// </summary>
    internal sealed class ComboBoxListBoxPopupContent : Control, IPopupContentPanel
    {
        public event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;
        public event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;
        public event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;
        public event EventHandler ApplyClicked;
        public event EventHandler CancelClicked;

        private readonly BeepTextBox _searchBox;
        private readonly BeepListBox _listBox;
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.OutlineDefault();
        private ComboBoxThemeTokens _tokens = ComboBoxThemeTokens.Fallback();
        private readonly Dictionary<string, ComboBoxPopupRowModel> _rowByIdentity = new(StringComparer.OrdinalIgnoreCase);
        private bool _suspendEvents;
        private bool _isMultiSelect;
        private IReadOnlyList<ComboBoxPopupRowModel> _activeRows = Array.Empty<ComboBoxPopupRowModel>();

        public ComboBoxListBoxPopupContent()
        {
            DoubleBuffered = true;
            TabStop = true;

            _searchBox = new BeepTextBox
            {
                Dock = DockStyle.Top,
                Height = 32,
                Visible = false
            };
            _searchBox.TextChanged += OnSearchBoxTextChanged;
            Controls.Add(_searchBox);

            _listBox = new BeepListBox
            {
                Dock = DockStyle.Fill,
                IsChild = true,
                ShowSearch = false,
                MultiSelect = false,
                ShowGroups = true,
                CollapsibleGroups = true,
                PersistCollapsedGroups = true,
                ShowCheckBox = false,
                ShowImage = true,
                ListBoxType = ListBoxType.CommandList
            };
            _listBox.ItemClicked += OnListItemClicked;
            _listBox.CheckedItemsChanged += OnCheckedItemsChanged;
            _listBox.SelectionChanged += OnSelectionChanged;
            _listBox.KeyDown += OnListKeyDown;
            Controls.Add(_listBox);
        }

        internal void SetListBoxType(ListBoxType listBoxType)
        {
            _listBox.ListBoxType = listBoxType;
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.OutlineDefault();
            _searchBox.PlaceholderText = _profile.SearchPlaceholder ?? "Search...";
            _searchBox.Height = Math.Max(24, _profile.SearchBoxHeight);
            _searchBox.Dock = _profile.SearchPlacement == ComboBoxPopupHostProfile.SearchPlacementMode.Bottom
                ? DockStyle.Bottom
                : DockStyle.Top;
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _tokens = tokens ?? ComboBoxThemeTokens.Fallback();
            BackColor = _tokens.PopupBackColor;
            _searchBox.BackColor = _tokens.PopupBackColor;
            _searchBox.ForeColor = _tokens.ForeColor;
            _listBox.BackColor = _tokens.PopupBackColor;
            _listBox.ForeColor = _tokens.ForeColor;
            _listBox.SelectionBackColor = _tokens.PopupRowSelectedColor;
            _listBox.SelectionBorderColor = _tokens.FocusBorderColor;
            _listBox.FocusOutlineColor = _tokens.FocusBorderColor;
            _listBox.HoverBackColor = _tokens.PopupRowHoverColor;
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            model ??= ComboBoxPopupModel.Empty();
            _activeRows = model.FilteredRows ?? Array.Empty<ComboBoxPopupRowModel>();
            _isMultiSelect = model.IsMultiSelect;
            bool hasSelectableRows = _activeRows.Any(IsSelectableRow);
            bool hasGroupRows = model.HasGroupHeaders && hasSelectableRows;
            _searchBox.Visible = (model.ShowSearchBox || _profile.ForceSearchVisible) && hasSelectableRows;

            _suspendEvents = true;
            try
            {
                if (_searchBox.Visible && !string.Equals(_searchBox.Text, model.SearchText ?? string.Empty, StringComparison.Ordinal))
                {
                    _searchBox.Text = model.SearchText ?? string.Empty;
                }

                _rowByIdentity.Clear();
                var listItems = new BindingList<SimpleItem>();
                foreach (var row in _activeRows)
                {
                    var mapped = MapRowToListItem(row);
                    listItems.Add(mapped);
                    _rowByIdentity[GetIdentity(mapped)] = row;
                }

                _listBox.ShowCheckBox = _isMultiSelect;
                _listBox.MultiSelect = _isMultiSelect;
                _listBox.SelectionMode = _isMultiSelect ? SelectionModeEnum.MultiSimple : SelectionModeEnum.Single;
                _listBox.ShowGroups = hasGroupRows;
                _listBox.CollapsibleGroups = hasGroupRows;
                _listBox.ListItems = listItems;

                if (_isMultiSelect)
                {
                    foreach (var item in listItems)
                    {
                        if (_rowByIdentity.TryGetValue(GetIdentity(item), out var row) && row.IsChecked)
                        {
                            _listBox.SetItemCheckbox(item, true);
                        }
                    }
                }
                else
                {
                    int selectedIndex = -1;
                    for (int i = 0; i < listItems.Count; i++)
                    {
                        if (_rowByIdentity.TryGetValue(GetIdentity(listItems[i]), out var row) && row.IsSelected)
                        {
                            selectedIndex = i;
                            break;
                        }
                    }

                    if (selectedIndex >= 0)
                    {
                        _listBox.SelectedIndex = selectedIndex;
                    }
                    else if (model.KeyboardFocusIndex >= 0 && model.KeyboardFocusIndex < listItems.Count)
                    {
                        _listBox.SelectedIndex = model.KeyboardFocusIndex;
                    }
                }
            }
            finally
            {
                _suspendEvents = false;
            }
        }

        private static bool IsSelectableRow(ComboBoxPopupRowModel row)
        {
            if (row == null)
            {
                return false;
            }

            return row.RowKind != ComboBoxPopupRowKind.EmptyState
                   && row.RowKind != ComboBoxPopupRowKind.LoadingState
                   && row.RowKind != ComboBoxPopupRowKind.NoResults
                   && row.RowKind != ComboBoxPopupRowKind.GroupHeader
                   && row.RowKind != ComboBoxPopupRowKind.Separator;
        }

        public void UpdateSearchText(string text)
        {
            string safe = text ?? string.Empty;
            if (_searchBox.Visible)
            {
                if (!string.Equals(_searchBox.Text, safe, StringComparison.Ordinal))
                {
                    _searchBox.Text = safe;
                }
            }
            else
            {
                _listBox.SearchText = safe;
            }
        }

        public void SetKeyboardFocusIndex(int index)
        {
            if (_listBox.ListItems == null || _listBox.ListItems.Count == 0)
            {
                return;
            }

            int safe = Math.Max(0, Math.Min(index, _listBox.ListItems.Count - 1));
            _listBox.SelectedIndex = safe;
            _listBox.EnsureIndexVisible(safe);
            KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(safe));
        }

        public void FocusSearchBox()
        {
            if (_searchBox.Visible)
            {
                _searchBox.Focus();
                _searchBox.SelectionStart = _searchBox.Text?.Length ?? 0;
            }
            else
            {
                _listBox.Focus();
            }
        }

        public void FocusItem(SimpleItem item)
        {
            if (item == null || _listBox.ListItems == null || _listBox.ListItems.Count == 0)
            {
                return;
            }

            string target = GetIdentity(item);
            for (int i = 0; i < _listBox.ListItems.Count; i++)
            {
                if (string.Equals(GetIdentity(_listBox.ListItems[i]), target, StringComparison.OrdinalIgnoreCase))
                {
                    _listBox.SelectedIndex = i;
                    _listBox.EnsureIndexVisible(i);
                    KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(i));
                    return;
                }
            }
        }

        private void OnSearchBoxTextChanged(object sender, EventArgs e)
        {
            if (_suspendEvents)
            {
                return;
            }

            SearchTextChanged?.Invoke(this, new ComboBoxSearchChangedEventArgs(_searchBox.Text ?? string.Empty));
        }

        private void OnListItemClicked(object sender, SimpleItem item)
        {
            if (_suspendEvents || item == null)
            {
                return;
            }

            if (!_rowByIdentity.TryGetValue(GetIdentity(item), out var row) || !row.IsEnabled)
            {
                return;
            }

            if (_isMultiSelect)
            {
                bool isChecked = _listBox.GetItemCheckbox(item);
                RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(BuildCommittedRow(row, isChecked), closePopup: false));
                return;
            }

            RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(BuildCommittedRow(row, false), closePopup: true));
        }

        private void OnCheckedItemsChanged(object sender, ListBoxCheckedChangedEventArgs e)
        {
            if (_suspendEvents || !_isMultiSelect || e?.Item == null)
            {
                return;
            }

            if (_rowByIdentity.TryGetValue(GetIdentity(e.Item), out var row) && row.IsEnabled)
            {
                RowCommitted?.Invoke(this, new ComboBoxRowCommittedEventArgs(BuildCommittedRow(row, e.IsChecked), closePopup: false));
            }
        }

        private void OnSelectionChanged(object sender, ListBoxSelectionChangedEventArgs e)
        {
            if (_suspendEvents || e?.CurrentItem == null)
            {
                return;
            }

            var listItems = _listBox.ListItems;
            if (listItems == null)
            {
                return;
            }

            int idx = listItems.IndexOf(e.CurrentItem);
            if (idx >= 0)
            {
                KeyboardFocusChanged?.Invoke(this, new ComboBoxKeyboardFocusChangedEventArgs(idx));
            }
        }

        private void OnListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _listBox.SelectedItem != null)
            {
                OnListItemClicked(this, _listBox.SelectedItem);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private static ComboBoxPopupRowModel BuildCommittedRow(ComboBoxPopupRowModel source, bool checkedState)
            => new ComboBoxPopupRowModel
            {
                SourceItem = source.SourceItem,
                RowKind = source.RowKind,
                Text = source.Text,
                SubText = source.SubText,
                TrailingText = source.TrailingText,
                TrailingValueText = source.TrailingValueText,
                ImagePath = source.ImagePath,
                GroupName = source.GroupName,
                LayoutPreset = source.LayoutPreset,
                IsSelected = source.IsSelected,
                IsEnabled = source.IsEnabled,
                IsKeyboardFocused = source.IsKeyboardFocused,
                IsCheckable = source.IsCheckable,
                IsChecked = checkedState,
                MatchStart = source.MatchStart,
                MatchLength = source.MatchLength,
                ListIndex = source.ListIndex
            };

        private static SimpleItem MapRowToListItem(ComboBoxPopupRowModel row)
        {
            if (row?.SourceItem != null)
            {
                var item = row.SourceItem;
                if (!string.IsNullOrWhiteSpace(row.Text)) item.Text = row.Text;
                if (!string.IsNullOrWhiteSpace(row.SubText)) item.SubText = row.SubText;
                if (!string.IsNullOrWhiteSpace(row.ImagePath)) item.ImagePath = row.ImagePath;
                if (!string.IsNullOrWhiteSpace(row.GroupName)) item.GroupName = row.GroupName;
                item.IsSeparator = row.RowKind == ComboBoxPopupRowKind.Separator;
                item.IsEnabled = row.IsEnabled;
                item.IsCheckable = row.IsCheckable;
                item.IsChecked = row.IsChecked;
                item.BadgeText = !string.IsNullOrWhiteSpace(row.TrailingValueText) ? row.TrailingValueText : row.TrailingText;
                return item;
            }

            return new SimpleItem
            {
                GuidId = $"popup::{row?.ListIndex ?? 0}",
                Text = row?.Text ?? string.Empty,
                SubText = row?.SubText ?? string.Empty,
                GroupName = row?.GroupName ?? string.Empty,
                IsSeparator = row?.RowKind == ComboBoxPopupRowKind.Separator,
                IsEnabled = row?.IsEnabled ?? false,
                IsCheckable = row?.IsCheckable ?? false,
                IsChecked = row?.IsChecked ?? false,
                BadgeText = row?.TrailingValueText ?? row?.TrailingText
            };
        }

        private static string GetIdentity(SimpleItem item)
        {
            if (item == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(item.GuidId)) return item.GuidId;
            if (item.ID != 0) return item.ID.ToString();
            return !string.IsNullOrWhiteSpace(item.Text) ? item.Text : (item.Name ?? string.Empty);
        }
    }
}
