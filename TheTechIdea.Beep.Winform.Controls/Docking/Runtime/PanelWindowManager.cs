using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Manages the lifecycle and associations between DockPanels and MDI windows.
    /// Coordinates panel creation, activation, and removal.
    /// </summary>
    public class PanelWindowManager
    {
        private readonly DockLayoutTree _layoutTree;
        private readonly MdiPanelPositioner _positioner;
        private readonly WindowChrome _chrome;

        private Dictionary<string, WindowDescriptor> _managedWindows = new Dictionary<string, WindowDescriptor>();
        private string _activePanel = null;

        public event EventHandler<PanelEventArgs> PanelCreated;
        public event EventHandler<PanelEventArgs> PanelDestroyed;
        public event EventHandler<PanelEventArgs> PanelActivated;
        public event EventHandler<PanelEventArgs> PanelHidden;

        public PanelWindowManager(DockLayoutTree layoutTree, MdiPanelPositioner positioner, WindowChrome chrome)
        {
            _layoutTree = layoutTree ?? throw new ArgumentNullException(nameof(layoutTree));
            _positioner = positioner ?? throw new ArgumentNullException(nameof(positioner));
            _chrome = chrome ?? throw new ArgumentNullException(nameof(chrome));
        }

        /// <summary>
        /// Creates a new managed panel window.
        /// </summary>
        public bool CreatePanel(DockPanel panel, IntPtr mdiClientHwnd)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            if (_managedWindows.ContainsKey(panel.Key))
            {
                Debug.WriteLine($"[PanelWindowManager] Panel '{panel.Key}' already managed");
                return false;
            }

            try
            {
                var descriptor = new WindowDescriptor
                {
                    PanelKey = panel.Key,
                    Panel = panel,
                    CreatedAt = DateTime.UtcNow,
                    State = WindowLifecycleState.Created
                };

                _managedWindows[panel.Key] = descriptor;

                PanelCreated?.Invoke(this, new PanelEventArgs { PanelKey = panel.Key, Panel = panel });

                Debug.WriteLine($"[PanelWindowManager] Created panel '{panel.Key}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PanelWindowManager] Error creating panel '{panel.Key}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes a panel from management and destroys its window.
        /// </summary>
        public bool DestroyPanel(string panelKey)
        {
            if (!_managedWindows.TryGetValue(panelKey, out var descriptor))
                return false;

            try
            {
                // Deactivate if this is the active panel
                if (_activePanel == panelKey)
                {
                    _activePanel = null;
                }

                _positioner.DestroyPanel(panelKey);
                _managedWindows.Remove(panelKey);

                PanelDestroyed?.Invoke(this, new PanelEventArgs { PanelKey = panelKey, Panel = descriptor.Panel });

                Debug.WriteLine($"[PanelWindowManager] Destroyed panel '{panelKey}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PanelWindowManager] Error destroying panel '{panelKey}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Activates (brings to front) a panel.
        /// </summary>
        public bool ActivatePanel(string panelKey)
        {
            if (!_managedWindows.ContainsKey(panelKey))
                return false;

            try
            {
                _positioner.ActivatePanel(panelKey);

                if (_activePanel != panelKey)
                {
                    var previousActive = _activePanel;
                    _activePanel = panelKey;

                    // Update group's active panel
                    var panel = _layoutTree.GetPanel(panelKey);
                    if (panel?.Group != null)
                    {
                        panel.Group.ActivePanel = panel;
                    }
                }

                PanelActivated?.Invoke(this, new PanelEventArgs { PanelKey = panelKey });

                Debug.WriteLine($"[PanelWindowManager] Activated panel '{panelKey}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PanelWindowManager] Error activating panel '{panelKey}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Hides a panel without destroying it.
        /// </summary>
        public bool HidePanel(string panelKey)
        {
            if (!_managedWindows.ContainsKey(panelKey))
                return false;

            try
            {
                _positioner.HideWindow(panelKey);

                if (_managedWindows.TryGetValue(panelKey, out var descriptor))
                {
                    descriptor.State = WindowLifecycleState.Hidden;
                }

                if (_activePanel == panelKey)
                {
                    _activePanel = null;
                }

                PanelHidden?.Invoke(this, new PanelEventArgs { PanelKey = panelKey });

                Debug.WriteLine($"[PanelWindowManager] Hid panel '{panelKey}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PanelWindowManager] Error hiding panel '{panelKey}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Shows a hidden panel.
        /// </summary>
        public bool ShowPanel(string panelKey)
        {
            if (!_managedWindows.ContainsKey(panelKey))
                return false;

            try
            {
                _positioner.ShowWindow(panelKey);

                if (_managedWindows.TryGetValue(panelKey, out var descriptor))
                {
                    descriptor.State = WindowLifecycleState.Visible;
                }

                Debug.WriteLine($"[PanelWindowManager] Showed panel '{panelKey}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PanelWindowManager] Error showing panel '{panelKey}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the active panel key, if any.
        /// </summary>
        public string GetActivePanel() => _activePanel;

        /// <summary>
        /// Gets all managed panel keys.
        /// </summary>
        public List<string> GetManagedPanels() => _managedWindows.Keys.ToList();

        /// <summary>
        /// Gets the state of a managed panel.
        /// </summary>
        public WindowDescriptor GetPanelDescriptor(string panelKey)
        {
            if (_managedWindows.TryGetValue(panelKey, out var descriptor))
                return descriptor;
            return null;
        }

        /// <summary>
        /// Processes a tab click event.
        /// </summary>
        public void HandleTabClick(string panelKey)
        {
            if (panelKey != null && _managedWindows.ContainsKey(panelKey))
            {
                ActivatePanel(panelKey);
            }
        }

        /// <summary>
        /// Processes a close button click event.
        /// </summary>
        public void HandleTabCloseClick(string panelKey)
        {
            if (panelKey != null && _managedWindows.ContainsKey(panelKey))
            {
                // Raise event and let caller decide whether to destroy or hide
                var descriptor = _managedWindows[panelKey];
                Debug.WriteLine($"[PanelWindowManager] Close requested for panel '{panelKey}'");
            }
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        public ManagerDiagnostics GetDiagnostics()
        {
            return new ManagerDiagnostics
            {
                TotalManagedPanels = _managedWindows.Count,
                VisiblePanels = _managedWindows.Count(x => x.Value.State == WindowLifecycleState.Visible),
                HiddenPanels = _managedWindows.Count(x => x.Value.State == WindowLifecycleState.Hidden),
                ActivePanel = _activePanel,
                ManagedPanels = _managedWindows.Values.ToList()
            };
        }
    }

    /// <summary>
    /// Descriptor for a managed panel window.
    /// </summary>
    public class WindowDescriptor
    {
        public string PanelKey { get; set; }
        public DockPanel Panel { get; set; }
        public IntPtr WindowHandle { get; set; }
        public WindowLifecycleState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActivatedAt { get; set; }

        public override string ToString() => $"{PanelKey} ({State})";
    }

    /// <summary>
    /// Lifecycle states for managed windows.
    /// </summary>
    public enum WindowLifecycleState
    {
        Created,
        Visible,
        Hidden,
        Destroyed
    }

    /// <summary>
    /// Event arguments for panel events.
    /// </summary>
    public class PanelEventArgs : EventArgs
    {
        public string PanelKey { get; set; }
        public DockPanel Panel { get; set; }
    }

    /// <summary>
    /// Diagnostic information from the panel manager.
    /// </summary>
    public class ManagerDiagnostics
    {
        public int TotalManagedPanels { get; set; }
        public int VisiblePanels { get; set; }
        public int HiddenPanels { get; set; }
        public string ActivePanel { get; set; }
        public List<WindowDescriptor> ManagedPanels { get; set; } = new List<WindowDescriptor>();
    }
}
