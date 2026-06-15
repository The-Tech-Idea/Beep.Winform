using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Customization;
using TheTechIdea.Beep.Winform.Controls.Search;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        public void BeginMergeScope()
        {
            _mergeBaseSnapshot.Clear();
            _mergeBaseSnapshot.AddRange(CloneNodeList(_commandItems));
            _isMerged = false;
        }

        public List<SimpleItem> GetCommandModelSnapshot()
        {
            return CloneNodeList(_commandItems);
        }

        public void MergeFrom(BeepRibbonControl source, RibbonMergeMode mode = RibbonMergeMode.AppendTabs)
        {
            if (source == null) return;
            MergeFrom(source.GetCommandModelSnapshot(), mode);
        }

        public void MergeFrom(IEnumerable<SimpleItem>? sourceTabs, RibbonMergeMode mode = RibbonMergeMode.AppendTabs)
        {
            if (sourceTabs == null) return;
            var incoming = CloneNodeList(sourceTabs);
            if (incoming.Count == 0) return;

            if (_mergeBaseSnapshot.Count == 0)
            {
                _mergeBaseSnapshot.AddRange(CloneNodeList(_commandItems));
            }

            List<SimpleItem> baseModel = _isMerged
                ? CloneNodeList(_commandItems)
                : CloneNodeList(_mergeBaseSnapshot);

            List<SimpleItem> result = mode switch
            {
                RibbonMergeMode.Replace => incoming,
                RibbonMergeMode.MergeByTabName => MergeByName(baseModel, incoming),
                _ => AppendTabs(baseModel, incoming)
            };

            ReplaceCommandItems(result);
            _isMerged = true;
            RibbonMerged?.Invoke(this, new RibbonMergedEventArgs(mode, incoming.Count, result.Count));
        }

        public void EndMergeScope()
        {
            if (_mergeBaseSnapshot.Count == 0)
            {
                _isMerged = false;
                return;
            }

            ReplaceCommandItems(CloneNodeList(_mergeBaseSnapshot));
            _mergeBaseSnapshot.Clear();
            _isMerged = false;
            RibbonMerged?.Invoke(this, new RibbonMergedEventArgs(RibbonMergeMode.Replace, 0, _commandItems.Count));
        }

        public bool ShowCustomizeRibbonDialog(IWin32Window? owner = null)
        {
            if ((_personalizationOptions & (RibbonPersonalizationOptions.RibbonTabs | RibbonPersonalizationOptions.RibbonGroups | RibbonPersonalizationOptions.CommandOrder | RibbonPersonalizationOptions.QuickAccess)) == 0)
            {
                return false;
            }

            EnsureCustomizationDefaultsCaptured();
            var initialState = BuildCustomizationDialogState();
            using var dialog = new RibbonCustomizationDialog(initialState)
            {
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };

            bool applied = false;

            dialog.ApplyRequested += (_, e) =>
            {
                ApplyCustomizationState(e.State, RibbonCustomizationAction.Apply);
                applied = true;
            };

            dialog.ResetRequested += (_, __) =>
            {
                ResetCustomizationToDefault();
                applied = true;
                dialog.LoadState(BuildCustomizationDialogState());
            };

            dialog.CancelRequested += (_, __) =>
            {
                RibbonCustomizationCanceled?.Invoke(this, EventArgs.Empty);
            };

            if (owner != null)
            {
                dialog.ShowDialog(owner);
            }
            else
            {
                dialog.ShowDialog();
            }

            return applied;
        }

        public RibbonCustomizationDialogState CaptureCustomizationState()
        {
            return BuildCustomizationDialogState().DeepClone();
        }

        public void ApplyCustomizationState(RibbonCustomizationDialogState state, RibbonCustomizationAction action = RibbonCustomizationAction.Apply)
        {
            ApplyCustomizationDialogState(state);
            RibbonCustomizationApplied?.Invoke(this,
                new RibbonCustomizationAppliedEventArgs(action, _commandItems.Count, _quickAccessCommandKeys.Count));
        }

        public void ResetCustomizationToDefault()
        {
            EnsureCustomizationDefaultsCaptured();
            if (_defaultCustomizationSnapshot == null)
            {
                return;
            }

            _pendingCustomizationState = null;
            _quickAccessCommandKeys.Clear();
            if (_defaultQuickAccessSnapshot != null)
            {
                foreach (var key in _defaultQuickAccessSnapshot)
                {
                    if (!string.IsNullOrWhiteSpace(key) &&
                        !_quickAccessCommandKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
                    {
                        _quickAccessCommandKeys.Add(key);
                    }
                }
            }

            ReplaceCommandItems(CloneNodeList(_defaultCustomizationSnapshot));
            RibbonCustomizationReset?.Invoke(this, EventArgs.Empty);
            RibbonCustomizationApplied?.Invoke(this,
                new RibbonCustomizationAppliedEventArgs(RibbonCustomizationAction.Reset, _commandItems.Count, _quickAccessCommandKeys.Count));
        }

        public void MarkCurrentCustomizationAsDefault()
        {
            if (_commandItems.Count == 0)
            {
                _defaultCustomizationSnapshot = null;
                _defaultQuickAccessSnapshot = null;
                return;
            }

            _defaultCustomizationSnapshot = CloneNodeList(_commandItems);
            _defaultQuickAccessSnapshot = [.. _quickAccessCommandKeys];
        }

        private void EnsureCustomizationDefaultsCaptured()
        {
            if (_defaultCustomizationSnapshot != null)
            {
                return;
            }

            if (_commandItems.Count == 0)
            {
                return;
            }

            _defaultCustomizationSnapshot = CloneNodeList(_commandItems);
            _defaultQuickAccessSnapshot = [.. _quickAccessCommandKeys];
        }

        private RibbonCustomizationDialogState BuildCustomizationDialogState()
        {
            var state = new RibbonCustomizationDialogState();

            foreach (var tabNode in _commandItems)
            {
                var tabModel = new RibbonCustomizationTabModel
                {
                    TabKey = GetMergeKey(tabNode),
                    Text = GetDisplayText(tabNode),
                    Visible = tabNode.IsVisible
                };

                foreach (var groupNode in tabNode.Children)
                {
                    tabModel.Groups.Add(new RibbonCustomizationGroupModel
                    {
                        GroupKey = GetMergeKey(groupNode),
                        Text = GetDisplayText(groupNode),
                        Visible = groupNode.IsVisible
                    });
                }

                state.Tabs.Add(tabModel);
            }

            var commandMap = new Dictionary<string, RibbonCustomizationCommandModel>(StringComparer.OrdinalIgnoreCase);
            foreach (var tabNode in _commandItems)
            {
                string tabKey = GetMergeKey(tabNode);
                string tabText = GetDisplayText(tabNode);
                foreach (var groupNode in tabNode.Children)
                {
                    string groupKey = GetMergeKey(groupNode);
                    string groupText = GetDisplayText(groupNode);
                    AddCustomizationCommands(commandMap, tabKey, tabText, groupKey, groupText, groupNode.Children);
                }
            }

            state.AvailableCommands = commandMap.Values
                .OrderBy(c => c.Text, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            foreach (var key in _quickAccessCommandKeys)
            {
                string? resolved = ResolveQuickAccessKey(key);
                if (!string.IsNullOrWhiteSpace(resolved) &&
                    !state.QuickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                {
                    state.QuickAccessCommandKeys.Add(resolved);
                }
            }

            return state;
        }

        private static void AddCustomizationCommands(
            IDictionary<string, RibbonCustomizationCommandModel> commandMap,
            string tabKey,
            string tabText,
            string groupKey,
            string groupText,
            IEnumerable<SimpleItem> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.IsSeparator)
                {
                    continue;
                }

                if (CanCustomizeCommand(node))
                {
                    string commandKey = GetCommandKey(node);
                    if (!commandMap.ContainsKey(commandKey))
                    {
                        commandMap[commandKey] = new RibbonCustomizationCommandModel
                        {
                            CommandKey = commandKey,
                            Text = GetDisplayText(node),
                            TabKey = tabKey,
                            GroupKey = groupKey,
                            TabText = tabText,
                            GroupText = groupText
                        };
                    }
                }

                if (node.Children.Count > 0)
                {
                    AddCustomizationCommands(commandMap, tabKey, tabText, groupKey, groupText, node.Children);
                }
            }
        }

        private static bool CanCustomizeCommand(SimpleItem node)
        {
            if (node.IsSeparator)
            {
                return false;
            }

            if (node.Children.Count == 0)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(node.ActionID) ||
                   !string.IsNullOrWhiteSpace(node.ReferenceID) ||
                   !string.IsNullOrWhiteSpace(node.MethodName);
        }

        private void ApplyCustomizationDialogState(RibbonCustomizationDialogState state)
        {
            if (state == null)
            {
                return;
            }

            _pendingCustomizationState = null;
            _quickAccessCommandKeys.Clear();
            foreach (var key in state.QuickAccessCommandKeys)
            {
                string? resolved = ResolveQuickAccessKey(key);
                if (!string.IsNullOrWhiteSpace(resolved) &&
                    !_quickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                {
                    _quickAccessCommandKeys.Add(resolved);
                }
            }

            var tabStates = state.Tabs
                .Select((tab, tabIndex) => new RibbonTabState
                {
                    TabKey = tab.TabKey,
                    Visible = tab.Visible,
                    Order = tabIndex,
                    Groups = tab.Groups
                        .Select((group, groupIndex) => new RibbonGroupState
                        {
                            GroupKey = group.GroupKey,
                            Visible = group.Visible,
                            Order = groupIndex
                        })
                        .ToList()
                })
                .ToList();

            ApplyTabStates(tabStates);
            BuildFromSimpleItems();
        }

        public void SaveCustomizationTo(string file)
        {
            try
            {
                var state = new RibbonCustomizationState
                {
                    LayoutMode = _layoutMode,
                    Density = _density,
                    SearchMode = _searchMode,
                    SearchIncludeBackstage = _searchIncludeBackstage,
                    SearchMaxResults = _searchMaxResults,
                    EnableKeyTips = _enableKeyTips,
                    QuickAccessAboveRibbon = _quickAccessAboveRibbon,
                    IsMinimized = _isMinimized,
                    ShowMinimizedPopupOnTabClick = _showMinimizedPopupOnTabClick,
                    BackstageSelectedIndex = _activeBackstageIndex,
                    QuickAccessCommandKeys = [.. _quickAccessCommandKeys],
                    Tabs = CaptureTabStates()
                };
                var json = System.Text.Json.JsonSerializer.Serialize(state, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(file, json);
            }
            catch
            {
            }
        }

        public void LoadCustomizationFrom(string file)
        {
            try
            {
                if (!File.Exists(file)) return;
                var json = File.ReadAllText(file);
                var state = System.Text.Json.JsonSerializer.Deserialize<RibbonCustomizationState>(json);
                if (state == null) return;

                LayoutMode = state.LayoutMode;
                Density = state.Density;
                SearchMode = state.SearchMode;
                SearchIncludeBackstage = state.SearchIncludeBackstage;
                SearchMaxResults = state.SearchMaxResults <= 0 ? 12 : state.SearchMaxResults;
                EnableKeyTips = state.EnableKeyTips;
                QuickAccessAboveRibbon = state.QuickAccessAboveRibbon;
                ShowMinimizedPopupOnTabClick = state.ShowMinimizedPopupOnTabClick;
                if (_allowMinimize)
                {
                    IsMinimized = state.IsMinimized;
                }

                _quickAccessCommandKeys.Clear();
                if (state.QuickAccessCommandKeys != null)
                {
                    foreach (var entry in state.QuickAccessCommandKeys)
                    {
                        string? resolved = ResolveQuickAccessKey(entry);
                        if (!string.IsNullOrWhiteSpace(resolved) &&
                            !_quickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                        {
                            _quickAccessCommandKeys.Add(resolved);
                        }
                    }
                }

                if (_commandItems.Count > 0)
                {
                    EnsureCustomizationDefaultsCaptured();
                    ApplyTabStates(state.Tabs);
                    _pendingCustomizationState = null;
                    BuildFromSimpleItems();
                }
                else
                {
                    _pendingCustomizationState = state;
                    RebuildQuickAccessToolbar();
                }

                if (state.BackstageSelectedIndex >= 0 && state.BackstageSelectedIndex < _backstageNavList.Items.Count)
                {
                    _backstageNavList.SelectedIndex = state.BackstageSelectedIndex;
                }
            }
            catch
            {
            }
        }

        public void SaveThemeTokensTo(string file)
        {
            try
            {
                var tokens = RibbonThemeTokens.FromTheme(_theme);
                var json = System.Text.Json.JsonSerializer.Serialize(tokens, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(file, json);
            }
            catch
            {
            }
        }

        public void LoadThemeTokensFrom(string file)
        {
            try
            {
                if (!File.Exists(file)) return;
                var json = File.ReadAllText(file);
                var tokens = System.Text.Json.JsonSerializer.Deserialize<RibbonThemeTokens>(json);
                if (tokens == null) return;
                RibbonThemeProvider = tokens.ToTheme(_theme);
            }
            catch
            {
            }
        }
    }
}
