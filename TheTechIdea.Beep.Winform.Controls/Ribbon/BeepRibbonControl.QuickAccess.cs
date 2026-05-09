using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        public bool AddCommandToQuickAccess(SimpleItem command)
        {
            if (command == null) return false;
            return AddCommandToQuickAccess(GetCommandKey(command));
        }

        public bool AddCommandToQuickAccess(string commandKey)
        {
            if (string.IsNullOrWhiteSpace(commandKey)) return false;
            if ((_personalizationOptions & RibbonPersonalizationOptions.QuickAccess) == 0) return false;
            if (_quickAccessCommandKeys.Contains(commandKey, StringComparer.OrdinalIgnoreCase)) return false;

            _quickAccessCommandKeys.Add(commandKey);
            RebuildQuickAccessToolbar();
            return true;
        }

        public bool RemoveCommandFromQuickAccess(SimpleItem command)
        {
            if (command == null) return false;
            return RemoveCommandFromQuickAccess(GetCommandKey(command));
        }

        public bool RemoveCommandFromQuickAccess(string commandKey)
        {
            if (string.IsNullOrWhiteSpace(commandKey)) return false;
            int index = _quickAccessCommandKeys.FindIndex(k => k.Equals(commandKey, StringComparison.OrdinalIgnoreCase));
            if (index < 0) return false;
            _quickAccessCommandKeys.RemoveAt(index);
            RebuildQuickAccessToolbar();
            return true;
        }

        public bool MoveQuickAccessCommand(string commandKey, int newIndex)
        {
            if (string.IsNullOrWhiteSpace(commandKey)) return false;
            int oldIndex = _quickAccessCommandKeys.FindIndex(k => k.Equals(commandKey, StringComparison.OrdinalIgnoreCase));
            if (oldIndex < 0) return false;
            if (newIndex < 0 || newIndex >= _quickAccessCommandKeys.Count) return false;
            if (oldIndex == newIndex) return true;

            var item = _quickAccessCommandKeys[oldIndex];
            _quickAccessCommandKeys.RemoveAt(oldIndex);
            _quickAccessCommandKeys.Insert(newIndex, item);
            RebuildQuickAccessToolbar();
            return true;
        }

        public void SaveQuickAccessTo(string file)
        {
            try
            {
                File.WriteAllLines(file, _quickAccessCommandKeys);
            }
            catch
            {
            }
        }

        public void LoadQuickAccessFrom(string file)
        {
            try
            {
                if (!File.Exists(file)) return;
                var lines = File.ReadAllLines(file);
                _quickAccessCommandKeys.Clear();
                foreach (var l in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    string? resolved = ResolveQuickAccessKey(l);
                    if (!string.IsNullOrWhiteSpace(resolved) &&
                        !_quickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                    {
                        _quickAccessCommandKeys.Add(resolved);
                    }
                }
                RebuildQuickAccessToolbar();
            }
            catch
            {
            }
        }

        private void ApplyQuickAccessPlacement()
        {
            _quickAccess.Dock = _quickAccessAboveRibbon ? DockStyle.Top : DockStyle.Bottom;
            if (_isMinimized)
            {
                ApplyMinimizedState();
            }
            PerformLayout();
            Invalidate();
        }

        private void RebuildQuickAccessToolbar()
        {
            _quickAccess.SuspendLayout();
            try
            {
                var qatItems = _commandMap.Keys.Where(i => i.Owner == _quickAccess).ToList();
                foreach (var qatItem in qatItems)
                {
                    _commandMap.Remove(qatItem);
                }

                var existingKeys = _quickAccessCommandKeys
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                _quickAccessCommandKeys.Clear();
                _quickAccessCommandKeys.AddRange(existingKeys);

                _quickAccess.Items.Clear();
                _quickAccess.Items.Add(_backstageButton);

                for (int i = 0; i < _quickAccessCommandKeys.Count; i++)
                {
                    var resolvedKey = ResolveQuickAccessKey(_quickAccessCommandKeys[i]);
                    if (string.IsNullOrWhiteSpace(resolvedKey)) continue;
                    _quickAccessCommandKeys[i] = resolvedKey;

                    if (!_commandLookup.TryGetValue(resolvedKey, out var command)) continue;
                    var button = new ToolStripButton(GetDisplayText(command), CreateCommandImage(command.ImagePath, true))
                    {
                        AutoSize = true,
                        DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        Font = BeepThemesManager.ToFont(_theme.CommandTypography)
                    };
                    ConfigureCommandItem(button, command);
                    button.Click += (_, __) => RaiseCommandInvoked(command, button);
                    _quickAccess.Items.Add(button);
                    _commandMap[button] = command;
                }

                if (_searchMode != RibbonSearchMode.Off)
                {
                    _searchBox.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
                    _searchResultsButton.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
                    _quickAccess.Items.Add(new ToolStripSeparator());
                    _quickAccess.Items.Add(_searchBox);
                    _quickAccess.Items.Add(_searchResultsButton);
                }
            }
            finally
            {
                _quickAccess.ResumeLayout();
                ApplySearchAccessibility();
                ApplyPaneTabOrder();
                RefreshKeyTipsVisibility();
            }
        }

        private string? ResolveQuickAccessKey(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            if (_commandLookup.Count == 0) return token;
            if (_commandLookup.ContainsKey(token)) return token;

            var match = _commandLookup.FirstOrDefault(kv =>
                string.Equals(GetDisplayText(kv.Value), token, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(match.Key) ? token : match.Key;
        }
    }
}
