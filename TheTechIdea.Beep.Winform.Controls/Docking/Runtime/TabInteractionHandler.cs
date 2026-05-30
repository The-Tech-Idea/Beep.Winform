using System;
using System.Collections.Generic;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Handles tab interaction events (click, close, double-click).
    /// Manages tab selection state and panel visibility.
    /// </summary>
    public class TabInteractionHandler : IDisposable
    {
        private Func<string, bool> _onActivatePanel;
        private DockLayoutTree _layoutTree;
        private Dictionary<string, TabState> _tabStates = new();
        private string _activeTabKey;
        private bool _disposed = false;

        /// <summary>
        /// Represents the state of a tab.
        /// </summary>
        public class TabState
        {
            public string PanelKey { get; set; }
            public string TabLabel { get; set; }
            public bool IsSelected { get; set; }
            public DateTime LastClickedAt { get; set; }
            public int ClickCount { get; set; }
        }

        /// <summary>
        /// Event raised when a tab is clicked.
        /// </summary>
        public event EventHandler<TabClickedEventArgs> TabClicked;

        /// <summary>
        /// Event raised when a tab's close button is clicked.
        /// </summary>
        public event EventHandler<TabClosedEventArgs> TabClosed;

        /// <summary>
        /// Event raised when a tab is double-clicked.
        /// </summary>
        public event EventHandler<TabDoubleClickedEventArgs> TabDoubleClicked;

        /// <summary>
        /// Initializes the tab interaction handler.
        /// </summary>
        /// <param name="onActivatePanel">Callback invoked to activate a panel by key.</param>
        /// <param name="layoutTree">The docking layout tree.</param>
        public TabInteractionHandler(
            Func<string, bool> onActivatePanel,
            DockLayoutTree layoutTree)
        {
            _onActivatePanel = onActivatePanel;
            _layoutTree = layoutTree ?? throw new ArgumentNullException(nameof(layoutTree));
        }

        /// <summary>
        /// Registers a panel's tab.
        /// </summary>
        public void RegisterTab(string panelKey, string tabLabel)
        {
            if (string.IsNullOrEmpty(panelKey))
                throw new ArgumentException("Panel key cannot be null or empty", nameof(panelKey));

            if (!_tabStates.ContainsKey(panelKey))
            {
                _tabStates[panelKey] = new TabState
                {
                    PanelKey = panelKey,
                    TabLabel = tabLabel ?? panelKey,
                    IsSelected = false,
                    LastClickedAt = DateTime.MinValue,
                    ClickCount = 0
                };

                Debug.WriteLine($"[TabInteractionHandler] Registered tab for panel '{panelKey}'");
            }
        }

        /// <summary>
        /// Unregisters a panel's tab.
        /// </summary>
        public void UnregisterTab(string panelKey)
        {
            if (_tabStates.Remove(panelKey))
            {
                if (_activeTabKey == panelKey)
                {
                    _activeTabKey = null;
                }

                Debug.WriteLine($"[TabInteractionHandler] Unregistered tab for panel '{panelKey}'");
            }
        }

        /// <summary>
        /// Handles a tab click.
        /// </summary>
        public void HandleTabClick(string panelKey)
        {
            if (string.IsNullOrEmpty(panelKey) || !_tabStates.ContainsKey(panelKey))
            {
                Debug.WriteLine($"[TabInteractionHandler] Tab not found: {panelKey}");
                return;
            }

            var tabState = _tabStates[panelKey];
            var now = DateTime.UtcNow;
            var timeSinceLastClick = now - tabState.LastClickedAt;

            // Detect double-click (two clicks within 300ms)
            if (timeSinceLastClick.TotalMilliseconds < 300 && tabState.ClickCount > 0)
            {
                HandleTabDoubleClick(panelKey, tabState);
                tabState.ClickCount = 0;
                return;
            }

            tabState.ClickCount++;
            tabState.LastClickedAt = now;

            // Deselect previous active tab
            if (!string.IsNullOrEmpty(_activeTabKey) && _tabStates.ContainsKey(_activeTabKey))
            {
                _tabStates[_activeTabKey].IsSelected = false;
            }

            // Select this tab
            tabState.IsSelected = true;
            _activeTabKey = panelKey;

            // Activate the panel
            _onActivatePanel?.Invoke(panelKey);

            Debug.WriteLine($"[TabInteractionHandler] Tab clicked: {panelKey}");

            TabClicked?.Invoke(this, new TabClickedEventArgs
            {
                PanelKey = panelKey,
                TabLabel = tabState.TabLabel,
                WasAlreadyActive = _activeTabKey == panelKey
            });
        }

        /// <summary>
        /// Handles a tab double-click.
        /// </summary>
        private void HandleTabDoubleClick(string panelKey, TabState tabState)
        {
            Debug.WriteLine($"[TabInteractionHandler] Tab double-clicked: {panelKey}");

            TabDoubleClicked?.Invoke(this, new TabDoubleClickedEventArgs
            {
                PanelKey = panelKey,
                TabLabel = tabState.TabLabel
            });
        }

        /// <summary>
        /// Handles a tab close button click.
        /// </summary>
        public void HandleTabClose(string panelKey)
        {
            if (string.IsNullOrEmpty(panelKey) || !_tabStates.ContainsKey(panelKey))
            {
                Debug.WriteLine($"[TabInteractionHandler] Tab not found for close: {panelKey}");
                return;
            }

            var tabState = _tabStates[panelKey];

            Debug.WriteLine($"[TabInteractionHandler] Tab close requested: {panelKey}");

            // Raise event (consumer decides if panel should be removed)
            TabClosed?.Invoke(this, new TabClosedEventArgs
            {
                PanelKey = panelKey,
                TabLabel = tabState.TabLabel
            });

            // Note: Actual removal is handled by event subscriber
            // This allows for vetoing or asking for confirmation
        }

        /// <summary>
        /// Sets the active tab.
        /// </summary>
        public void SetActiveTab(string panelKey)
        {
            if (string.IsNullOrEmpty(panelKey))
            {
                _activeTabKey = null;
                return;
            }

            if (_tabStates.ContainsKey(panelKey))
            {
                HandleTabClick(panelKey);
            }
        }

        /// <summary>
        /// Gets the active tab key.
        /// </summary>
        public string GetActiveTab() => _activeTabKey;

        /// <summary>
        /// Gets tab state for a panel.
        /// </summary>
        public TabState GetTabState(string panelKey)
        {
            _tabStates.TryGetValue(panelKey, out var state);
            return state;
        }

        /// <summary>
        /// Updates a tab's label.
        /// </summary>
        public bool UpdateTabLabel(string panelKey, string newLabel)
        {
            if (_tabStates.TryGetValue(panelKey, out var state))
            {
                state.TabLabel = newLabel ?? panelKey;
                Debug.WriteLine($"[TabInteractionHandler] Updated tab label for '{panelKey}' to '{newLabel}'");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets all tab states.
        /// </summary>
        public IReadOnlyDictionary<string, TabState> GetAllTabStates() => _tabStates;

        /// <summary>
        /// Reorders a panel's tab within its group to the given zero-based index.
        /// Follows DockPanelSuite DockPane.SetContentIndex() / Krypton WorkspaceCell ordering.
        /// Fires <see cref="TabMoved"/> if the position changed.
        /// </summary>
        /// <param name="panelKey">Key of the panel to reorder.</param>
        /// <param name="newIndex">Target tab index within the group.</param>
        /// <returns>True if the tab was actually moved.</returns>
        public bool MoveTab(string panelKey, int newIndex)
        {
            var panel = _layoutTree.GetPanel(panelKey);
            if (panel?.Group == null) return false;

            var group = panel.Group;
            int before = group.Panels.ToList().IndexOf(panel);
            group.MovePanelToIndex(panel, newIndex);
            int after = group.Panels.ToList().IndexOf(panel);

            if (before == after) return false;

            TabMoved?.Invoke(this, new TabMovedEventArgs { PanelKey = panelKey, OldIndex = before, NewIndex = after });
            Debug.WriteLine($"[TabInteractionHandler] Tab moved '{panelKey}' from {before} to {after}");
            return true;
        }

        /// <summary>Raised when a tab is reordered within its group.</summary>
        public event EventHandler<TabMovedEventArgs> TabMoved;

        /// <summary>
        /// Gets the group containing a panel.
        /// </summary>
        private DockGroup GetPanelGroup(string panelKey)
        {
            var panel = _layoutTree.GetPanel(panelKey);
            if (panel != null)
            {
                return panel.Group;
            }

            return null;
        }

        /// <summary>
        /// Gets all tab keys in a specific group.
        /// </summary>
        public List<string> GetGroupTabs(string groupKey)
        {
            var group = _layoutTree.GetGroup(groupKey);
            var tabs = new List<string>();

            if (group != null)
            {
                foreach (var panel in group.Panels)
                {
                    tabs.Add(panel.Key);
                }
            }

            return tabs;
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        public TabInteractionDiagnostics GetDiagnostics()
        {
            var tabStats = new List<TabStatDescriptor>();
            foreach (var kvp in _tabStates)
            {
                tabStats.Add(new TabStatDescriptor
                {
                    PanelKey = kvp.Key,
                    TabLabel = kvp.Value.TabLabel,
                    IsSelected = kvp.Value.IsSelected,
                    ClickCount = kvp.Value.ClickCount,
                    LastClickedAtUtc = kvp.Value.LastClickedAt
                });
            }

            return new TabInteractionDiagnostics
            {
                TotalTabs = _tabStates.Count,
                ActiveTab = _activeTabKey,
                TabStats = tabStats
            };
        }

        /// <summary>
        /// Diagnostics result for tab interaction.
        /// </summary>
        public class TabInteractionDiagnostics
        {
            public int TotalTabs { get; set; }
            public string ActiveTab { get; set; }
            public List<TabStatDescriptor> TabStats { get; set; }
        }

        /// <summary>
        /// Descriptor for a tab stat.
        /// </summary>
        public class TabStatDescriptor
        {
            public string PanelKey { get; set; }
            public string TabLabel { get; set; }
            public bool IsSelected { get; set; }
            public int ClickCount { get; set; }
            public DateTime LastClickedAtUtc { get; set; }
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _tabStates.Clear();
            _disposed = true;
            Debug.WriteLine("[TabInteractionHandler] Disposed");
        }
    }

    /// <summary>
    /// Event arguments for tab clicked.
    /// </summary>
    public class TabClickedEventArgs : EventArgs
    {
        public string PanelKey { get; set; }
        public string TabLabel { get; set; }
        public bool WasAlreadyActive { get; set; }
    }

    /// <summary>
    /// Event arguments for tab closed.
    /// </summary>
    public class TabClosedEventArgs : EventArgs
    {
        public string PanelKey { get; set; }
        public string TabLabel { get; set; }
    }

    /// <summary>
    /// Event arguments for tab double-clicked.
    /// </summary>
    public class TabDoubleClickedEventArgs : EventArgs
    {
        public string PanelKey { get; set; }
        public string TabLabel { get; set; }
    }

    /// <summary>
    /// Event arguments raised when a tab is reordered within its group.
    /// </summary>
    public class TabMovedEventArgs : EventArgs
    {
        public string PanelKey  { get; set; }
        public int    OldIndex  { get; set; }
        public int    NewIndex  { get; set; }
    }
}
