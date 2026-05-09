using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Search;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Tokens;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        private void InitializeSearchControls()
        {
            _searchBox.Name = "RibbonSearchBox";
            _searchBox.ToolTipText = "Search commands (Ctrl+F)";
            _searchBox.BorderStyle = BorderStyle.FixedSingle;
            _searchBox.TextChanged += (_, __) =>
            {
                if (_searchMode != RibbonSearchMode.Off)
                {
                    RunSearch(_searchBox.Text);
                }
            };
            _searchBox.Enter += (_, __) =>
            {
                if (_showSearchHistorySuggestions && string.IsNullOrWhiteSpace(_searchBox.Text))
                {
                    ApplySearchHistorySuggestions();
                }
            };
            _searchBox.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Down)
                {
                    if (string.IsNullOrWhiteSpace(_searchBox.Text) && _showSearchHistorySuggestions)
                    {
                        ApplySearchHistorySuggestions();
                    }
                    MoveSearchSelection(1);
                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.KeyCode == Keys.Up)
                {
                    MoveSearchSelection(-1);
                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.KeyCode == Keys.Escape)
                {
                    HideSearchResultsDropDown();
                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    if (TryExecuteSelectedSearchResult())
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    RunSearch(_searchBox.Text);
                    e.SuppressKeyPress = true;
                }
            };

            _searchResultsButton.Name = "RibbonSearchResultsButton";
            _searchResultsButton.ToolTipText = "Search results";
            _searchResultsButton.Visible = false;
            _searchResultsButton.DropDownClosed += (_, __) => _searchResultSelectionIndex = -1;
            EnsureSearchControls();
        }

        private void ApplySearchAccessibility()
        {
            _searchResultsButton.AccessibleName = "Ribbon search results";
            _searchResultsButton.AccessibleDescription = "Opens ranked search results and recent queries.";
            _searchResultsButton.AccessibleRole = AccessibleRole.ButtonDropDown;
            _searchResultsButton.AccessibleDefaultActionDescription = "Open search results";

            if (_searchBox.Control != null)
            {
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    _searchBox.Control,
                    "Ribbon search box",
                    "Search commands in the ribbon. Press Enter to execute the top result.",
                    AccessibleRole.Text);
                _searchBox.Control.TabStop = _searchMode != RibbonSearchMode.Off;
                _searchBox.Control.RightToLeft = _ribbonRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            }
        }

        private async void RunSearch(string? rawQuery)
        {
            try
            {
                await RunSearchAsync(rawQuery);
            }
            catch
            {
                // Keep ribbon stable if search fails.
            }
        }

        private async Task RunSearchAsync(string? rawQuery)
        {
            var sw = Stopwatch.StartNew();
            int version = ++_searchRequestVersion;
            string query = rawQuery?.Trim() ?? string.Empty;

            if (_searchMode == RibbonSearchMode.Off)
            {
                ApplySearchResults([]);
                RaiseSearchExecuted(query, 0, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
                return;
            }

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                if (_showSearchHistorySuggestions)
                {
                    ApplySearchHistorySuggestions();
                }
                else
                {
                    ApplySearchResults([]);
                }
                RaiseSearchExecuted(query, 0, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
                return;
            }

            if (_searchMode == RibbonSearchMode.SmartService && _searchProvider != null)
            {
                try
                {
                    var results = await _searchProvider.SearchAsync(query, _commandLookup.Values.Where(n => n.IsVisible));
                    if (version != _searchRequestVersion) return;
                    var list = results?.ToList() ?? [];
                    ApplySearchResults(list);
                    RecordSearchQuery(query);
                    RaiseSearchExecuted(query, list.Count, providerUsed: true, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
                    return;
                }
                catch
                {
                    if (version != _searchRequestVersion) return;
                    var fallback = QueryLocalSearch(query);
                    ApplySearchResults(fallback);
                    RecordSearchQuery(query);
                    RaiseSearchExecuted(query, fallback.Count, providerUsed: true, providerFailed: true, usedLocalFallback: true, sw.ElapsedMilliseconds);
                    return;
                }
            }

            var localResults = QueryLocalSearch(query);
            ApplySearchResults(localResults);
            RecordSearchQuery(query);
            RaiseSearchExecuted(query, localResults.Count, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
        }

        private void RunLocalSearch(string? rawQuery)
        {
            var sw = Stopwatch.StartNew();
            string query = rawQuery?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                if (_showSearchHistorySuggestions)
                {
                    ApplySearchHistorySuggestions();
                }
                else
                {
                    ApplySearchResults([]);
                }
                RaiseSearchExecuted(query, 0, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
                return;
            }

            var results = QueryLocalSearch(query);
            ApplySearchResults(results);
            RecordSearchQuery(query);
            RaiseSearchExecuted(query, results.Count, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
        }

        private List<SimpleItem> QueryLocalSearch(string query)
        {
            var map = new Dictionary<string, SimpleItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in _commandLookup.Values)
            {
                AddSearchCandidate(map, node);
            }

            if (_searchIncludeBackstage)
            {
                foreach (var node in EnumerateBackstageNodes())
                {
                    AddSearchCandidate(map, node);
                }
            }

            return RibbonSearchIndex.RankCommands(query, map.Values, _searchCommandUsage, _searchMaxResults, ResolveSearchScoreBoost);
        }

        public RibbonSearchBenchmarkReport RunLocalSearchBenchmark(IEnumerable<string> queries, bool? includeBackstage = null)
        {
            var map = new Dictionary<string, SimpleItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in _commandLookup.Values)
            {
                AddSearchCandidate(map, node);
            }

            bool includeBackstageNodes = includeBackstage ?? _searchIncludeBackstage;
            if (includeBackstageNodes)
            {
                foreach (var node in EnumerateBackstageNodes())
                {
                    AddSearchCandidate(map, node);
                }
            }

            return RibbonSearchBenchmark.Run(queries, map.Values, _searchCommandUsage, _searchMaxResults, ResolveSearchScoreBoost);
        }

        public RibbonAccessibilityAuditReport RunAccessibilityAudit()
        {
            return RibbonAccessibilityAudit.Audit(this);
        }

        private IEnumerable<SimpleItem> EnumerateBackstageNodes()
        {
            foreach (var root in _backstageItems)
            {
                foreach (var node in EnumerateNodeTree(root))
                {
                    yield return node;
                }
            }

            foreach (var root in _backstageRecentItems)
            {
                foreach (var node in EnumerateNodeTree(root))
                {
                    yield return node;
                }
            }

            foreach (var root in _backstagePinnedItems)
            {
                foreach (var node in EnumerateNodeTree(root))
                {
                    yield return node;
                }
            }
        }

        private static IEnumerable<SimpleItem> EnumerateNodeTree(SimpleItem root)
        {
            if (root == null)
            {
                yield break;
            }

            var stack = new Stack<SimpleItem>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!current.IsVisible)
                {
                    continue;
                }

                yield return current;
                for (int i = current.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push(current.Children[i]);
                }
            }
        }

        private static void AddSearchCandidate(IDictionary<string, SimpleItem> map, SimpleItem node)
        {
            if (!IsSearchableNode(node))
            {
                return;
            }

            string key = GetMergeKey(node);
            if (string.IsNullOrWhiteSpace(key))
            {
                key = GetCommandKey(node);
            }

            if (!map.ContainsKey(key))
            {
                map[key] = node;
            }
        }

        private static bool IsSearchableNode(SimpleItem node)
        {
            if (node.IsSeparator || !node.IsVisible)
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

        public void SetSearchCategoryBoost(string categoryKey, int scoreBoost)
        {
            if (string.IsNullOrWhiteSpace(categoryKey))
            {
                return;
            }

            _searchCategoryBoosts[categoryKey.Trim()] = Math.Clamp(scoreBoost, -200, 300);
        }

        public void ClearSearchCategoryBoosts()
        {
            _searchCategoryBoosts.Clear();
        }

        private int ResolveSearchScoreBoost(SimpleItem item)
        {
            int boost = 0;
            if (_searchScoreBoostProvider != null)
            {
                boost += _searchScoreBoostProvider(item);
            }

            string categoryKey = item.Category.ToString();
            if (!string.IsNullOrWhiteSpace(categoryKey) &&
                _searchCategoryBoosts.TryGetValue(categoryKey, out int categoryBoost))
            {
                boost += categoryBoost;
            }

            if (!string.IsNullOrWhiteSpace(item.GroupName) &&
                _searchCategoryBoosts.TryGetValue($"group:{item.GroupName}", out int groupBoost))
            {
                boost += groupBoost;
            }

            return boost;
        }

        private void RaiseSearchExecuted(string query, int resultCount, bool providerUsed, bool providerFailed, bool usedLocalFallback, long durationMs = 0)
        {
            SearchExecuted?.Invoke(this, new RibbonSearchExecutedEventArgs(query, _searchMode, resultCount, providerUsed, providerFailed, usedLocalFallback));
            _searchTelemetry?.OnSearchExecuted(new RibbonSearchTelemetryEvent
            {
                Query = query,
                Mode = _searchMode,
                ResultCount = resultCount,
                ProviderUsed = providerUsed,
                ProviderFailed = providerFailed,
                UsedLocalFallback = usedLocalFallback,
                DurationMs = Math.Max(0, durationMs),
                ExecutedAtUtc = DateTime.UtcNow
            });
        }

        private void ApplySearchResults(List<SimpleItem> results)
        {
            _searchResults.Clear();
            _searchResultsButton.DropDownItems.Clear();
            _searchResultSelectionIndex = -1;

            if (results.Count == 0)
            {
                _searchResultsButton.Text = "Find";
                return;
            }

            _searchResults.AddRange(results);
            _searchResultsButton.Text = $"Find ({results.Count})";

            foreach (var command in results)
            {
                var item = new ToolStripMenuItem(GetDisplayText(command), CreateCommandImage(command.ImagePath, true))
                {
                    ToolTipText = BuildToolTip(command),
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography)
                };
                RibbonAccessibilityHelper.ApplyCommandAccessibility(item, command, GetDisplayText(command), AccessibleRole.MenuItem);
                item.Click += (_, __) => InvokeSearchResult(command, item);
                item.MouseEnter += (_, __) => _searchResultSelectionIndex = _searchResultsButton.DropDownItems.IndexOf(item);
                _searchResultsButton.DropDownItems.Add(item);
            }
        }

        private void ApplySearchHistorySuggestions()
        {
            _searchResults.Clear();
            _searchResultsButton.DropDownItems.Clear();
            _searchResultSelectionIndex = -1;

            if (_searchHistory.Count == 0)
            {
                _searchResultsButton.Text = "Find";
                HideSearchResultsDropDown();
                return;
            }

            _searchResultsButton.Text = $"Recent ({_searchHistory.Count})";
            foreach (var query in _searchHistory.Take(Math.Min(10, _searchHistory.Count)))
            {
                var item = new ToolStripMenuItem(query)
                {
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                    ToolTipText = "Run recent search"
                };
                item.AccessibleName = $"Recent query {query}";
                item.AccessibleDescription = "Execute this recent ribbon search query.";
                item.AccessibleRole = AccessibleRole.MenuItem;
                item.Click += (_, __) =>
                {
                    _searchBox.Text = query;
                    _searchBox.SelectionStart = _searchBox.Text.Length;
                    RunSearch(query);
                };
                item.MouseUp += (_, e) =>
                {
                    if (e.Button != MouseButtons.Right)
                    {
                        return;
                    }

                    if (RemoveSearchHistoryItem(query))
                    {
                        ApplySearchHistorySuggestions();
                    }
                };
                item.MouseEnter += (_, __) => _searchResultSelectionIndex = _searchResultsButton.DropDownItems.IndexOf(item);
                _searchResultsButton.DropDownItems.Add(item);
            }

            _searchResultsButton.DropDownItems.Add(new ToolStripSeparator());

            var manage = new ToolStripMenuItem("Manage history...")
            {
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };
            manage.AccessibleName = "Manage search history";
            manage.AccessibleDescription = "Open the dialog to remove or clear search history entries.";
            manage.AccessibleRole = AccessibleRole.MenuItem;
            manage.Click += (_, __) => ShowSearchHistoryManager();
            _searchResultsButton.DropDownItems.Add(manage);

            var clear = new ToolStripMenuItem("Clear history")
            {
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };
            clear.AccessibleName = "Clear search history";
            clear.AccessibleDescription = "Remove all saved search queries.";
            clear.AccessibleRole = AccessibleRole.MenuItem;
            clear.Click += (_, __) =>
            {
                ClearSearchHistory();
                ApplySearchHistorySuggestions();
            };
            _searchResultsButton.DropDownItems.Add(clear);
        }

        private void ShowSearchHistoryManager()
        {
            using var dialog = new Form
            {
                Text = "Search History",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(460, 340),
                MinimizeBox = false,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                BackColor = _theme.GroupBack,
                ForeColor = _theme.Text,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };

            var list = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text
            };
            list.Items.AddRange(_searchHistory.Cast<object>().ToArray());

            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(6, 4, 6, 4),
                BackColor = _theme.GroupBack
            };

            var close = new Button
            {
                Text = "Close",
                Width = 88,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text
            };
            close.FlatAppearance.BorderColor = _theme.GroupBorder;
            close.Click += (_, __) => dialog.Close();

            var clear = new Button
            {
                Text = "Clear All",
                Width = 88,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text
            };
            clear.FlatAppearance.BorderColor = _theme.GroupBorder;
            clear.Click += (_, __) =>
            {
                ClearSearchHistory();
                list.Items.Clear();
                ApplySearchHistorySuggestions();
            };

            var remove = new Button
            {
                Text = "Remove",
                Width = 88,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text
            };
            remove.FlatAppearance.BorderColor = _theme.GroupBorder;
            remove.Click += (_, __) =>
            {
                if (list.SelectedItem is not string selected)
                {
                    return;
                }

                if (RemoveSearchHistoryItem(selected))
                {
                    list.Items.Remove(selected);
                    ApplySearchHistorySuggestions();
                }
            };

            footer.Controls.Add(close);
            footer.Controls.Add(clear);
            footer.Controls.Add(remove);

            dialog.Controls.Add(list);
            dialog.Controls.Add(footer);

            var owner = FindForm();
            if (owner != null)
            {
                dialog.ShowDialog(owner);
            }
            else
            {
                dialog.ShowDialog();
            }
        }

        public void ClearSearchHistory()
        {
            _searchHistory.Clear();
            _searchCommandUsage.Clear();
        }

        public bool RemoveSearchHistoryItem(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return false;
            }

            int index = _searchHistory.FindIndex(x => x.Equals(query.Trim(), StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return false;
            }

            _searchHistory.RemoveAt(index);
            return true;
        }

        public void SaveSearchHistoryTo(string file)
        {
            try
            {
                File.WriteAllLines(file, _searchHistory);
            }
            catch
            {
            }
        }

        public void LoadSearchHistoryFrom(string file)
        {
            try
            {
                if (!File.Exists(file))
                {
                    return;
                }

                var lines = File.ReadAllLines(file)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(l => l.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(_searchHistoryLimit)
                    .ToList();

                _searchHistory.Clear();
                _searchHistory.AddRange(lines);
            }
            catch
            {
            }
        }

        private void RecordSearchQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return;
            }

            string normalized = query.Trim();
            if (normalized.Length < 2)
            {
                return;
            }

            int existingIndex = _searchHistory.FindIndex(q => q.Equals(normalized, StringComparison.OrdinalIgnoreCase));
            if (existingIndex >= 0)
            {
                _searchHistory.RemoveAt(existingIndex);
            }

            _searchHistory.Insert(0, normalized);
            while (_searchHistory.Count > _searchHistoryLimit)
            {
                _searchHistory.RemoveAt(_searchHistory.Count - 1);
            }
        }

        private void RecordSearchCommandUsage(SimpleItem command)
        {
            string key = GetCommandKey(command);
            if (_searchCommandUsage.TryGetValue(key, out int score))
            {
                _searchCommandUsage[key] = Math.Min(200, score + 1);
            }
            else
            {
                _searchCommandUsage[key] = 1;
            }
        }

        private void InvokeSearchResult(SimpleItem command, ToolStripItem source)
        {
            RecordSearchQuery(_searchBox.Text);
            RecordSearchCommandUsage(command);
            HideSearchResultsDropDown();
            RaiseCommandInvoked(command, source);
        }

        private bool TryExecuteSelectedSearchResult()
        {
            if (_searchResultsButton.DropDownItems.Count == 0)
            {
                return false;
            }

            int index = _searchResultSelectionIndex;
            if (index < 0)
            {
                index = 0;
            }

            if (index >= _searchResultsButton.DropDownItems.Count)
            {
                return false;
            }

            if (_searchResults.Count > index)
            {
                var command = _searchResults[index];
                var source = _searchResultsButton.DropDownItems[index];
                InvokeSearchResult(command, source);
                return true;
            }

            if (_searchResultsButton.DropDownItems[index] is ToolStripMenuItem menuItem)
            {
                menuItem.PerformClick();
                HideSearchResultsDropDown();
                return true;
            }

            return true;
        }

        private void MoveSearchSelection(int delta)
        {
            if (_searchResultsButton.DropDownItems.Count == 0)
            {
                return;
            }

            if (!_searchResultsButton.DropDown.Visible)
            {
                _searchResultsButton.ShowDropDown();
            }

            int count = _searchResultsButton.DropDownItems.Count;
            if (_searchResultSelectionIndex < 0)
            {
                _searchResultSelectionIndex = delta > 0 ? 0 : count - 1;
            }
            else
            {
                _searchResultSelectionIndex += delta;
                if (_searchResultSelectionIndex < 0)
                {
                    _searchResultSelectionIndex = count - 1;
                }
                else if (_searchResultSelectionIndex >= count)
                {
                    _searchResultSelectionIndex = 0;
                }
            }

            if (_searchResultSelectionIndex >= 0 && _searchResultSelectionIndex < count)
            {
                _searchResultsButton.DropDownItems[_searchResultSelectionIndex].Select();
            }
        }

        private void HideSearchResultsDropDown()
        {
            if (_searchResultsButton.DropDown.Visible)
            {
                _searchResultsButton.HideDropDown();
            }
            _searchResultSelectionIndex = -1;
        }
    }
}
