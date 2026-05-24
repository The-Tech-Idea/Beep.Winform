using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Interop;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Applies calculated layout bounds to MDI child windows.
    /// Maps abstract layout rectangles to concrete window positioning and visibility.
    /// </summary>
    public class MdiPanelPositioner
    {
        private readonly IntPtr _mdiClientHwnd;
        private readonly IDockingPainter _painter;
        private Dictionary<string, IntPtr> _panelToWindowMap = new Dictionary<string, IntPtr>();
        private Dictionary<IntPtr, string> _windowToPanelMap = new Dictionary<IntPtr, string>();
        private Dictionary<string, PanelWindowState> _windowStates = new Dictionary<string, PanelWindowState>();

        /// <summary>
        /// Constructs the positioner for a specific MDI client window.
        /// </summary>
        public MdiPanelPositioner(IntPtr mdiClientHwnd, IDockingPainter painter)
        {
            if (mdiClientHwnd == IntPtr.Zero)
                throw new ArgumentException("MDI client window handle cannot be null", nameof(mdiClientHwnd));

            _mdiClientHwnd = mdiClientHwnd;
            _painter = painter ?? throw new ArgumentNullException(nameof(painter));
        }

        /// <summary>
        /// Applies a calculated layout to all panels.
        /// Creates windows for new panels, repositions existing ones, and manages visibility.
        /// </summary>
        public void ApplyLayout(Dictionary<string, Rectangle> panelBounds, List<DockPanel> allPanels)
        {
            if (panelBounds == null)
                throw new ArgumentNullException(nameof(panelBounds));

            if (allPanels == null)
                throw new ArgumentNullException(nameof(allPanels));

            // First pass: Create/update windows for visible panels
            foreach (var panelKey in panelBounds.Keys)
            {
                var panel = allPanels.FirstOrDefault(p => p.Key == panelKey);
                if (panel == null)
                    continue;

                var bounds = panelBounds[panelKey];

                if (!_panelToWindowMap.ContainsKey(panelKey))
                {
                    // Create new window for this panel
                    CreateWindowForPanel(panel, bounds);
                }
                else
                {
                    // Reposition existing window
                    RepositionWindow(panelKey, bounds, panel);
                }

                // Ensure window is visible
                ShowWindow(panelKey);
            }

            // Second pass: Hide panels not in the layout
            var visiblePanelKeys = new HashSet<string>(panelBounds.Keys);
            foreach (var panelKey in _panelToWindowMap.Keys.ToList())
            {
                if (!visiblePanelKeys.Contains(panelKey))
                {
                    HideWindow(panelKey);
                }
            }

            // Update Z-order so panels appear in logical order
            UpdateZOrder(panelBounds.Keys.ToList());
        }

        /// <summary>
        /// Creates a new MDI child window for a panel.
        /// </summary>
        private void CreateWindowForPanel(DockPanel panel, Rectangle bounds)
        {
            if (panel.Content == null)
            {
                Debug.WriteLine($"[MdiPanelPositioner] Panel '{panel.Key}' has no content control");
                return;
            }

            try
            {
                // Use SendMessage with WM_MDICREATE to create MDI child window
                var createStruct = new MdiNativeApi.MDICREATESTRUCT
                {
                    szClass = Marshal.StringToHGlobalUni("STATIC"),  // Placeholder class
                    szTitle = Marshal.StringToHGlobalUni(panel.Title ?? "Panel"),
                    x = bounds.X,
                    y = bounds.Y,
                    cx = bounds.Width,
                    cy = bounds.Height,
                    style = 0
                };

                var hwnd = MdiNativeApi.SendMessage(
                    _mdiClientHwnd,
                    MdiConstants.WM_MDICREATE,
                    IntPtr.Zero,
                    ref createStruct
                );

                // Clean up allocated strings
                Marshal.FreeHGlobal(createStruct.szClass);
                Marshal.FreeHGlobal(createStruct.szTitle);

                if (hwnd == IntPtr.Zero)
                {
                    Debug.WriteLine($"[MdiPanelPositioner] Failed to create window for panel '{panel.Key}'");
                    return;
                }

                _panelToWindowMap[panel.Key] = hwnd;
                _windowToPanelMap[hwnd] = panel.Key;
                _windowStates[panel.Key] = new PanelWindowState
                {
                    PanelKey = panel.Key,
                    WindowHandle = hwnd,
                    IsVisible = true,
                    LastBounds = bounds,
                    IsActive = false
                };

                // Parent the content control to the window
                if (panel.Content is Control contentCtrl)
                {
                    // Reparent the control if needed
                    if (contentCtrl.Parent == null)
                    {
                        contentCtrl.Dock = DockStyle.Fill;
                        // TODO: Parent to hwnd via SetParent (requires control hosting)
                    }
                }

                Debug.WriteLine($"[MdiPanelPositioner] Created window for panel '{panel.Key}' at ({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height})");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MdiPanelPositioner] Error creating window for panel '{panel.Key}': {ex.Message}");
            }
        }

        /// <summary>
        /// Repositions an existing window to new bounds.
        /// </summary>
        private void RepositionWindow(string panelKey, Rectangle newBounds, DockPanel panel)
        {
            if (!_panelToWindowMap.TryGetValue(panelKey, out var hwnd))
                return;

            // Only reposition if bounds changed
            if (_windowStates.TryGetValue(panelKey, out var state) && state.LastBounds == newBounds)
                return;

            bool success = MdiNativeApi.SetWindowPos(
                hwnd,
                IntPtr.Zero,
                newBounds.X,
                newBounds.Y,
                newBounds.Width,
                newBounds.Height,
                MdiConstants.SWP_NOZORDER | MdiConstants.SWP_NOACTIVATE
            );

            if (success)
            {
                if (_windowStates.ContainsKey(panelKey))
                {
                    _windowStates[panelKey].LastBounds = newBounds;
                }

                Debug.WriteLine($"[MdiPanelPositioner] Repositioned panel '{panelKey}' to ({newBounds.X}, {newBounds.Y}, {newBounds.Width}, {newBounds.Height})");
            }
            else
            {
                Debug.WriteLine($"[MdiPanelPositioner] Failed to reposition panel '{panelKey}'");
            }
        }

        /// <summary>
        /// Shows a window if it's hidden.
        /// </summary>
        public void ShowWindow(string panelKey)
        {
            if (!_panelToWindowMap.TryGetValue(panelKey, out var hwnd))
                return;

            if (_windowStates.TryGetValue(panelKey, out var state) && state.IsVisible)
                return;

            MdiNativeApi.ShowWindow(hwnd, MdiConstants.SW_SHOW);

            if (_windowStates.ContainsKey(panelKey))
            {
                _windowStates[panelKey].IsVisible = true;
            }

            Debug.WriteLine($"[MdiPanelPositioner] Showed panel '{panelKey}'");
        }

        /// <summary>
        /// Hides a window without destroying it.
        /// </summary>
        public void HideWindow(string panelKey)
        {
            if (!_panelToWindowMap.TryGetValue(panelKey, out var hwnd))
                return;

            if (_windowStates.TryGetValue(panelKey, out var state) && !state.IsVisible)
                return;

            MdiNativeApi.ShowWindow(hwnd, MdiConstants.SW_HIDE);

            if (_windowStates.ContainsKey(panelKey))
            {
                _windowStates[panelKey].IsVisible = false;
            }

            Debug.WriteLine($"[MdiPanelPositioner] Hid panel '{panelKey}'");
        }

        /// <summary>
        /// Activates (brings to front) a specific panel window.
        /// </summary>
        public void ActivatePanel(string panelKey)
        {
            if (!_panelToWindowMap.TryGetValue(panelKey, out var hwnd))
                return;

            // Deactivate previous active panel
            foreach (var kvp in _windowStates.Where(x => x.Value.IsActive).ToList())
            {
                kvp.Value.IsActive = false;
            }

            // Use SendMessage WM_MDIACTIVATE to activate the child
            MdiNativeApi.SendMessage(_mdiClientHwnd, MdiConstants.WM_MDIACTIVATE, hwnd, IntPtr.Zero);

            if (_windowStates.ContainsKey(panelKey))
            {
                _windowStates[panelKey].IsActive = true;
            }

            Debug.WriteLine($"[MdiPanelPositioner] Activated panel '{panelKey}'");
        }

        /// <summary>
        /// Manages Z-order so panels appear in a logical order.
        /// Later panels in the list appear in front.
        /// </summary>
        private void UpdateZOrder(List<string> orderedPanelKeys)
        {
            // Get visible windows and arrange them
            var visibleWindows = orderedPanelKeys
                .Where(key => _panelToWindowMap.ContainsKey(key))
                .Select(key => _panelToWindowMap[key])
                .ToList();

            // Move each window to top in order (later ones end up on top)
            foreach (var hwnd in visibleWindows)
            {
                MdiNativeApi.SetWindowPos(
                    hwnd,
                    IntPtr.Zero,
                    0, 0, 0, 0,
                    MdiConstants.SWP_NOMOVE | MdiConstants.SWP_NOSIZE | MdiConstants.SWP_NOACTIVATE
                );
            }
        }

        /// <summary>
        /// Destroys a panel's window and removes it from tracking.
        /// </summary>
        public void DestroyPanel(string panelKey)
        {
            if (!_panelToWindowMap.TryGetValue(panelKey, out var hwnd))
                return;

            MdiNativeApi.DestroyWindow(hwnd);

            _panelToWindowMap.Remove(panelKey);
            _windowToPanelMap.Remove(hwnd);
            _windowStates.Remove(panelKey);

            Debug.WriteLine($"[MdiPanelPositioner] Destroyed panel '{panelKey}'");
        }

        /// <summary>
        /// Gets the window handle for a panel, or null if not created.
        /// </summary>
        public IntPtr? GetPanelWindow(string panelKey)
        {
            if (_panelToWindowMap.TryGetValue(panelKey, out var hwnd))
                return hwnd;
            return null;
        }

        /// <summary>
        /// Gets the panel key for a window handle, or null if not found.
        /// </summary>
        public string GetPanelForWindow(IntPtr hwnd)
        {
            if (_windowToPanelMap.TryGetValue(hwnd, out var panelKey))
                return panelKey;
            return null;
        }

        /// <summary>
        /// Gets diagnostic information about window state.
        /// </summary>
        public PositionerDiagnostics GetDiagnostics()
        {
            return new PositionerDiagnostics
            {
                TotalManagedPanels = _panelToWindowMap.Count,
                VisiblePanels = _windowStates.Count(s => s.Value.IsVisible),
                HiddenPanels = _windowStates.Count(s => !s.Value.IsVisible),
                ActivePanel = _windowStates.FirstOrDefault(s => s.Value.IsActive).Key,
                ManagedWindows = _panelToWindowMap.Values.ToList(),
                PanelStates = new Dictionary<string, PanelWindowState>(_windowStates)
            };
        }
    }

    /// <summary>
    /// Tracks window state for a single panel.
    /// </summary>
    public class PanelWindowState
    {
        public string PanelKey { get; set; }
        public IntPtr WindowHandle { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActive { get; set; }
        public Rectangle LastBounds { get; set; }
    }

    /// <summary>
    /// Diagnostic information from the positioner.
    /// </summary>
    public class PositionerDiagnostics
    {
        public int TotalManagedPanels { get; set; }
        public int VisiblePanels { get; set; }
        public int HiddenPanels { get; set; }
        public string ActivePanel { get; set; }
        public List<IntPtr> ManagedWindows { get; set; } = new List<IntPtr>();
        public Dictionary<string, PanelWindowState> PanelStates { get; set; } = new Dictionary<string, PanelWindowState>();
    }
}
