using TheTechIdea.Beep.Winform.Controls.Diagnostics;
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

            // Resolve against the real commands rather than trusting the caller's string. This method
            // accepted anything: AddCommandToQuickAccess("NoSuchCommand") returned true and put the
            // literal text in the key list beside genuine keys, where it survived a save/load round
            // trip and could never resolve to a command. A true return meant only "the string was not
            // already in the list".
            string? resolved = ResolveQuickAccessKey(commandKey, strict: true);
            if (resolved == null)
            {
                BeepLog.Warn(this, $"add '{commandKey}' to the quick access toolbar",
                             "no command with that key or display text exists in the ribbon");
                return false;
            }

            if (_quickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase)) return false;

            _quickAccessCommandKeys.Add(resolved);
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
            catch (Exception ex)
            {
                // A save that fails silently loses the user's toolbar with no sign anything happened -
                // and the caller has no return value to tell it apart from success either.
                BeepLog.Failure(this, $"save the quick access toolbar to '{file}'", ex);
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
            catch (Exception ex)
            {
                BeepLog.Failure(this, $"load the quick access toolbar from '{file}'", ex);
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

        private string? ResolveQuickAccessKey(string token) => ResolveQuickAccessKey(token, strict: false);

        /// <summary>
        /// Maps a key or a command's display text onto a real command key.
        /// </summary>
        /// <param name="strict">
        /// When true, an unmatched token returns null instead of itself. The lenient behaviour is kept
        /// for loading a saved file, where the ribbon may not be built yet and the key has to be taken
        /// on trust; it is wrong for a caller adding a command by name, which is how an arbitrary
        /// string ended up in the toolbar.
        /// </param>
        private string? ResolveQuickAccessKey(string token, bool strict)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            // Nothing to resolve against yet - during load, before the commands are built.
            if (_commandLookup.Count == 0) return strict ? null : token;

            if (_commandLookup.ContainsKey(token)) return token;

            var match = _commandLookup.FirstOrDefault(kv =>
                string.Equals(GetDisplayText(kv.Value), token, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(match.Key)) return match.Key;

            return strict ? null : token;
        }
    }
}
