using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Customization
{
    public sealed class RibbonCustomizationDialog : Form
    {
        private readonly CheckedListBox _tabsList = new() { Dock = DockStyle.Fill, CheckOnClick = true, IntegralHeight = false };
        private readonly CheckedListBox _groupsList = new() { Dock = DockStyle.Fill, CheckOnClick = true, IntegralHeight = false };
        private readonly ListBox _availableCommandsList = new() { Dock = DockStyle.Fill };
        private readonly ListBox _quickAccessList = new() { Dock = DockStyle.Fill };
        private readonly TextBox _commandSearchBox = new() { Dock = DockStyle.Fill };
        private readonly ComboBox _commandTabFilter = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox _commandGroupFilter = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

        private readonly Button _tabUpButton = new() { Text = "Up", Width = 82 };
        private readonly Button _tabDownButton = new() { Text = "Down", Width = 82 };
        private readonly Button _groupUpButton = new() { Text = "Up", Width = 82 };
        private readonly Button _groupDownButton = new() { Text = "Down", Width = 82 };
        private readonly Button _addQuickAccessButton = new() { Text = "Add >", Width = 82 };
        private readonly Button _removeQuickAccessButton = new() { Text = "< Remove", Width = 82 };
        private readonly Button _quickAccessUpButton = new() { Text = "Up", Width = 82 };
        private readonly Button _quickAccessDownButton = new() { Text = "Down", Width = 82 };
        private readonly Button _okButton = new() { Text = "OK", Width = 88 };
        private readonly Button _applyButton = new() { Text = "Apply", Width = 88 };
        private readonly Button _resetButton = new() { Text = "Reset", Width = 88 };
        private readonly Button _cancelButton = new() { Text = "Cancel", Width = 88 };

        private readonly Dictionary<string, RibbonCustomizationCommandModel> _commandLookup = new(StringComparer.OrdinalIgnoreCase);
        private RibbonCustomizationDialogState _state;
        private bool _isBinding;

        public event EventHandler<RibbonCustomizationStateEventArgs>? ApplyRequested;
        public event EventHandler? ResetRequested;
        public event EventHandler? CancelRequested;

        public RibbonCustomizationDialog(RibbonCustomizationDialogState state)
        {
            _state = state?.DeepClone() ?? new RibbonCustomizationDialogState();
            InitializeUi();
            BindState(_state);
        }

        public void LoadState(RibbonCustomizationDialogState state)
        {
            _state = state?.DeepClone() ?? new RibbonCustomizationDialogState();
            BindState(_state);
        }

        private void InitializeUi()
        {
            SuspendLayout();
            Text = "Customize Ribbon";
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            Width = 920;
            Height = 620;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            var pages = new TabControl { Dock = DockStyle.Fill };
            var ribbonPage = new TabPage("Ribbon");
            var quickAccessPage = new TabPage("Quick Access Toolbar");
            pages.TabPages.Add(ribbonPage);
            pages.TabPages.Add(quickAccessPage);
            root.Controls.Add(pages, 0, 0);

            BuildRibbonPage(ribbonPage);
            BuildQuickAccessPage(quickAccessPage);

            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(8)
            };
            footer.Controls.AddRange([_cancelButton, _applyButton, _okButton, _resetButton]);
            root.Controls.Add(footer, 0, 1);

            AcceptButton = _okButton;
            CancelButton = _cancelButton;

            _tabsList.SelectedIndexChanged += (_, __) => PopulateGroupsForSelectedTab();
            _tabsList.ItemCheck += TabsList_ItemCheck;
            _groupsList.ItemCheck += GroupsList_ItemCheck;

            _tabUpButton.Click += (_, __) => MoveSelectedTab(-1);
            _tabDownButton.Click += (_, __) => MoveSelectedTab(1);
            _groupUpButton.Click += (_, __) => MoveSelectedGroup(-1);
            _groupDownButton.Click += (_, __) => MoveSelectedGroup(1);
            _commandSearchBox.TextChanged += (_, __) =>
            {
                if (_isBinding) return;
                PopulateAvailableCommands();
            };
            _commandTabFilter.SelectedIndexChanged += (_, __) =>
            {
                if (_isBinding) return;
                PopulateCommandGroupFilter();
                PopulateAvailableCommands();
            };
            _commandGroupFilter.SelectedIndexChanged += (_, __) =>
            {
                if (_isBinding) return;
                PopulateAvailableCommands();
            };
            _addQuickAccessButton.Click += (_, __) => AddSelectedCommandToQuickAccess();
            _removeQuickAccessButton.Click += (_, __) => RemoveSelectedQuickAccessCommand();
            _quickAccessUpButton.Click += (_, __) => MoveSelectedQuickAccess(-1);
            _quickAccessDownButton.Click += (_, __) => MoveSelectedQuickAccess(1);

            _okButton.Click += (_, __) =>
            {
                RaiseApplyRequested();
                DialogResult = DialogResult.OK;
                Close();
            };
            _applyButton.Click += (_, __) => RaiseApplyRequested();
            _resetButton.Click += (_, __) => ResetRequested?.Invoke(this, EventArgs.Empty);
            _cancelButton.Click += (_, __) =>
            {
                CancelRequested?.Invoke(this, EventArgs.Empty);
                DialogResult = DialogResult.Cancel;
                Close();
            };

            ResumeLayout();
        }

        private void BuildRibbonPage(TabPage page)
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 430
            };
            page.Controls.Add(split);

            split.Panel1.Controls.Add(CreateListPane("Tabs", _tabsList, _tabUpButton, _tabDownButton));
            split.Panel2.Controls.Add(CreateListPane("Groups", _groupsList, _groupUpButton, _groupDownButton));
        }

        private void BuildQuickAccessPage(TabPage page)
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            page.Controls.Add(layout);

            layout.Controls.Add(new Label { Text = "Available commands", Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 8, 0, 8) }, 0, 0);
            layout.Controls.Add(new Label { Text = "Quick Access Toolbar", Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 8, 0, 8) }, 2, 0);
            layout.Controls.Add(BuildCommandFilterPane(), 0, 1);
            layout.Controls.Add(_availableCommandsList, 0, 2);

            var centerButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(8, 24, 8, 8)
            };
            centerButtons.Controls.AddRange([_addQuickAccessButton, _removeQuickAccessButton]);
            layout.Controls.Add(centerButtons, 1, 2);

            var quickAccessPane = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            quickAccessPane.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            quickAccessPane.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            quickAccessPane.Controls.Add(_quickAccessList, 0, 0);

            var quickAccessButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(0, 8, 0, 0)
            };
            quickAccessButtons.Controls.AddRange([_quickAccessUpButton, _quickAccessDownButton]);
            quickAccessPane.Controls.Add(quickAccessButtons, 0, 1);

            layout.Controls.Add(quickAccessPane, 2, 2);
        }

        private Control BuildCommandFilterPane()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(0, 0, 0, 8)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            panel.Controls.Add(new Label { Text = "Category", Dock = DockStyle.Fill, AutoSize = true }, 0, 0);
            panel.Controls.Add(new Label { Text = "Group", Dock = DockStyle.Fill, AutoSize = true }, 1, 0);
            panel.Controls.Add(new Label { Text = "Search", Dock = DockStyle.Fill, AutoSize = true }, 2, 0);

            panel.Controls.Add(_commandTabFilter, 0, 1);
            panel.Controls.Add(_commandGroupFilter, 1, 1);
            panel.Controls.Add(_commandSearchBox, 2, 1);
            return panel;
        }

        private static Control CreateListPane(string title, Control listControl, params Button[] buttons)
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0, 8, 0, 8)
            };
            layout.Controls.Add(label, 0, 0);
            layout.Controls.Add(listControl, 0, 1);

            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(0, 8, 0, 0)
            };
            foreach (var button in buttons)
            {
                footer.Controls.Add(button);
            }
            layout.Controls.Add(footer, 0, 2);
            return layout;
        }

        private void BindState(RibbonCustomizationDialogState state)
        {
            _isBinding = true;
            try
            {
                _commandLookup.Clear();
                foreach (var command in state.AvailableCommands)
                {
                    if (!_commandLookup.ContainsKey(command.CommandKey))
                    {
                        _commandLookup[command.CommandKey] = command;
                    }
                }

                PopulateTabs(-1);
                PopulateCommandTabFilter();
                PopulateCommandGroupFilter();
                PopulateAvailableCommands();
                PopulateQuickAccess(-1);
            }
            finally
            {
                _isBinding = false;
            }
        }

        private void PopulateTabs(int selectedIndex)
        {
            _tabsList.Items.Clear();
            for (int i = 0; i < _state.Tabs.Count; i++)
            {
                var tab = _state.Tabs[i];
                int addedIndex = _tabsList.Items.Add(tab);
                _tabsList.SetItemChecked(addedIndex, tab.Visible);
            }

            if (_tabsList.Items.Count == 0)
            {
                _groupsList.Items.Clear();
                return;
            }

            int index = selectedIndex >= 0 && selectedIndex < _tabsList.Items.Count ? selectedIndex : 0;
            _tabsList.SelectedIndex = index;
            PopulateGroupsForSelectedTab();
        }

        private void PopulateGroupsForSelectedTab()
        {
            _groupsList.Items.Clear();
            var tab = GetSelectedTab();
            if (tab == null)
            {
                return;
            }

            for (int i = 0; i < tab.Groups.Count; i++)
            {
                var group = tab.Groups[i];
                int addedIndex = _groupsList.Items.Add(group);
                _groupsList.SetItemChecked(addedIndex, group.Visible);
            }
        }

        private void PopulateAvailableCommands()
        {
            _availableCommandsList.Items.Clear();
            IEnumerable<RibbonCustomizationCommandModel> query = _state.AvailableCommands;

            string? tabKey = GetSelectedFilterKey(_commandTabFilter);
            if (!string.IsNullOrWhiteSpace(tabKey))
            {
                query = query.Where(c => string.Equals(c.TabKey, tabKey, StringComparison.OrdinalIgnoreCase));
            }

            string? groupKey = GetSelectedFilterKey(_commandGroupFilter);
            if (!string.IsNullOrWhiteSpace(groupKey))
            {
                query = query.Where(c => string.Equals(c.GroupKey, groupKey, StringComparison.OrdinalIgnoreCase));
            }

            string search = (_commandSearchBox.Text ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    ContainsIgnoreCase(c.Text, search) ||
                    ContainsIgnoreCase(c.TabText, search) ||
                    ContainsIgnoreCase(c.GroupText, search) ||
                    ContainsIgnoreCase(c.CommandKey, search));
            }

            foreach (var command in query.OrderBy(c => c.Text, StringComparer.CurrentCultureIgnoreCase))
            {
                _availableCommandsList.Items.Add(command);
            }
        }

        private void PopulateCommandTabFilter()
        {
            _commandTabFilter.BeginUpdate();
            try
            {
                _commandTabFilter.Items.Clear();
                _commandTabFilter.Items.Add(new CommandFilterItem(string.Empty, "All categories"));

                foreach (var tab in _state.Tabs
                    .OrderBy(t => t.Text, StringComparer.CurrentCultureIgnoreCase))
                {
                    _commandTabFilter.Items.Add(new CommandFilterItem(tab.TabKey, tab.Text));
                }

                _commandTabFilter.SelectedIndex = _commandTabFilter.Items.Count > 0 ? 0 : -1;
            }
            finally
            {
                _commandTabFilter.EndUpdate();
            }
        }

        private void PopulateCommandGroupFilter()
        {
            string? selectedGroupKey = GetSelectedFilterKey(_commandGroupFilter);
            string? selectedTabKey = GetSelectedFilterKey(_commandTabFilter);
            IEnumerable<RibbonCustomizationCommandModel> source = _state.AvailableCommands;
            if (!string.IsNullOrWhiteSpace(selectedTabKey))
            {
                source = source.Where(c => string.Equals(c.TabKey, selectedTabKey, StringComparison.OrdinalIgnoreCase));
            }

            _commandGroupFilter.BeginUpdate();
            try
            {
                _commandGroupFilter.Items.Clear();
                _commandGroupFilter.Items.Add(new CommandFilterItem(string.Empty, "All groups"));

                foreach (var group in source
                    .GroupBy(c => c.GroupKey, StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                    {
                        var first = g.First();
                        string text = string.IsNullOrWhiteSpace(first.GroupText) ? first.GroupKey : first.GroupText;
                        return new CommandFilterItem(first.GroupKey, text);
                    })
                    .OrderBy(g => g.Text, StringComparer.CurrentCultureIgnoreCase))
                {
                    _commandGroupFilter.Items.Add(group);
                }

                int index = 0;
                if (!string.IsNullOrWhiteSpace(selectedGroupKey))
                {
                    for (int i = 0; i < _commandGroupFilter.Items.Count; i++)
                    {
                        if (_commandGroupFilter.Items[i] is CommandFilterItem item &&
                            string.Equals(item.Key, selectedGroupKey, StringComparison.OrdinalIgnoreCase))
                        {
                            index = i;
                            break;
                        }
                    }
                }

                _commandGroupFilter.SelectedIndex = _commandGroupFilter.Items.Count > 0 ? index : -1;
            }
            finally
            {
                _commandGroupFilter.EndUpdate();
            }
        }

        private static string? GetSelectedFilterKey(ComboBox comboBox)
        {
            return comboBox.SelectedItem is CommandFilterItem item
                ? item.Key
                : null;
        }

        private static bool ContainsIgnoreCase(string value, string search)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.Contains(search, StringComparison.OrdinalIgnoreCase);
        }

        private void PopulateQuickAccess(int selectedIndex)
        {
            _quickAccessList.Items.Clear();
            foreach (var commandKey in _state.QuickAccessCommandKeys)
            {
                if (_commandLookup.TryGetValue(commandKey, out var command))
                {
                    _quickAccessList.Items.Add(command);
                }
                else
                {
                    _quickAccessList.Items.Add(new RibbonCustomizationCommandModel
                    {
                        CommandKey = commandKey,
                        Text = commandKey
                    });
                }
            }

            if (_quickAccessList.Items.Count == 0)
            {
                return;
            }

            if (selectedIndex < 0 || selectedIndex >= _quickAccessList.Items.Count)
            {
                selectedIndex = _quickAccessList.Items.Count - 1;
            }
            _quickAccessList.SelectedIndex = selectedIndex;
        }

        private RibbonCustomizationTabModel? GetSelectedTab()
        {
            return _tabsList.SelectedItem as RibbonCustomizationTabModel;
        }

        private void TabsList_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (_isBinding) return;
            if (e.Index < 0 || e.Index >= _tabsList.Items.Count) return;
            if (_tabsList.Items[e.Index] is RibbonCustomizationTabModel tab)
            {
                tab.Visible = e.NewValue == CheckState.Checked;
            }
        }

        private void GroupsList_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (_isBinding) return;
            if (e.Index < 0 || e.Index >= _groupsList.Items.Count) return;
            if (_groupsList.Items[e.Index] is RibbonCustomizationGroupModel group)
            {
                group.Visible = e.NewValue == CheckState.Checked;
            }
        }

        private void MoveSelectedTab(int direction)
        {
            int index = _tabsList.SelectedIndex;
            if (index < 0) return;
            int target = index + direction;
            if (target < 0 || target >= _state.Tabs.Count) return;

            var tab = _state.Tabs[index];
            _state.Tabs.RemoveAt(index);
            _state.Tabs.Insert(target, tab);
            PopulateTabs(target);
        }

        private void MoveSelectedGroup(int direction)
        {
            var tab = GetSelectedTab();
            if (tab == null) return;

            int index = _groupsList.SelectedIndex;
            if (index < 0) return;
            int target = index + direction;
            if (target < 0 || target >= tab.Groups.Count) return;

            var group = tab.Groups[index];
            tab.Groups.RemoveAt(index);
            tab.Groups.Insert(target, group);
            PopulateGroupsForSelectedTab();
            _groupsList.SelectedIndex = target;
        }

        private void AddSelectedCommandToQuickAccess()
        {
            if (_availableCommandsList.SelectedItem is not RibbonCustomizationCommandModel selected)
            {
                return;
            }

            if (_state.QuickAccessCommandKeys.Contains(selected.CommandKey, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            _state.QuickAccessCommandKeys.Add(selected.CommandKey);
            PopulateQuickAccess(_state.QuickAccessCommandKeys.Count - 1);
        }

        private void RemoveSelectedQuickAccessCommand()
        {
            if (_quickAccessList.SelectedItem is not RibbonCustomizationCommandModel selected)
            {
                return;
            }

            int index = _state.QuickAccessCommandKeys.FindIndex(k => k.Equals(selected.CommandKey, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return;
            }

            _state.QuickAccessCommandKeys.RemoveAt(index);
            PopulateQuickAccess(index);
        }

        private void MoveSelectedQuickAccess(int direction)
        {
            if (_quickAccessList.SelectedItem is not RibbonCustomizationCommandModel selected)
            {
                return;
            }

            int oldIndex = _state.QuickAccessCommandKeys.FindIndex(k => k.Equals(selected.CommandKey, StringComparison.OrdinalIgnoreCase));
            if (oldIndex < 0)
            {
                return;
            }

            int targetIndex = oldIndex + direction;
            if (targetIndex < 0 || targetIndex >= _state.QuickAccessCommandKeys.Count)
            {
                return;
            }

            string key = _state.QuickAccessCommandKeys[oldIndex];
            _state.QuickAccessCommandKeys.RemoveAt(oldIndex);
            _state.QuickAccessCommandKeys.Insert(targetIndex, key);
            PopulateQuickAccess(targetIndex);
        }

        private void RaiseApplyRequested()
        {
            ApplyRequested?.Invoke(this, new RibbonCustomizationStateEventArgs(_state.DeepClone()));
        }

        private sealed class CommandFilterItem
        {
            public CommandFilterItem(string key, string text)
            {
                Key = key ?? string.Empty;
                Text = string.IsNullOrWhiteSpace(text) ? Key : text;
            }

            public string Key { get; }
            public string Text { get; }

            public override string ToString() => Text;
        }
    }
}
