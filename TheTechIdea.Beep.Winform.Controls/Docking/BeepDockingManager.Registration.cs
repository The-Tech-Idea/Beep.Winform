using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Membership: which panels this manager knows about, and how they enter and leave.
    /// </summary>
    /// <remarks>
    /// Registration is the boundary between "a control that exists" and "a panel the docking system
    /// is responsible for". Everything downstream — layout, persistence, perspectives, validation —
    /// assumes a registered panel has a key, a group and a state that agree with each other, so the
    /// cost of getting registration wrong is paid everywhere else.
    /// <para>
    /// The <c>Notify*</c> members belong here rather than with the events: they are how a panel
    /// tells the manager that something about <i>it</i> changed, which is the manager's membership
    /// bookkeeping reacting, not an event being raised for consumers.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>
        /// Adds a new docking panel to the manager, layout tree, and a matching
        /// <see cref="BeepDockspace"/> on the host form. The dockspace is the persistent runtime
        /// view that hosts the panel (same model as designer-created panels). If no matching
        /// dockspace exists yet on the host form, the panel is hosted directly on the host form
        /// (legacy path) and a warning is written to the debug log.
        /// </summary>
        public DockPanel AddPanel(string panelKey, string title, DockPosition dockPosition, Control content)
        {
            if (string.IsNullOrWhiteSpace(panelKey))
                throw new ArgumentException("Panel key cannot be null or empty", nameof(panelKey));

            if (_panelsByKey.ContainsKey(panelKey))
                throw new InvalidOperationException($"Panel with key '{panelKey}' already exists");

            InitializeSubsystems();

            // Create the visual panel — it IS a Panel control, not a Component
            var panel = new DockPanel
            {
                Key = panelKey,
                Title = title ?? "Panel",
                DockPosition = dockPosition,
                Content = content,
                Manager = this
            };

            _panelsByKey[panelKey] = panel;
            _layoutTree.RegisterPanel(panel);
            ApplyThemeToPanel(panel);

            var group = GetOrCreateGroupAtPosition(dockPosition);
            group.AddPanel(panel);

            // Invalidate as soon as the tree changes, not after the layout has been applied.
            // ApplyLayout below consumes the controller's cached result; with the invalidation
            // afterwards it consumed one computed before this panel existed, found no bounds for
            // it, and skipped it - leaving the control at its default 200x100 while the engine
            // reported the correct rectangle. Joining an existing stack showed it most clearly,
            // because adding the first panel to an edge happened to be masked by the host form's
            // own layout pass re-triggering a recalculation.
            _layoutController?.InvalidateLayout();

            if (_hostForm != null && !IsDesignHosted)
            {
                // Preferred path: place the panel inside a matching BeepDockspace, exactly like
                // designer-created panels. The dockspace's LayoutPanels / OnLayout will size it.
                var dockspace = FindOrCreateDockspaceAt(_hostForm, dockPosition);
                if (dockspace != null)
                {
                    if (panel.Parent != dockspace)
                    {
                        panel.Parent?.Controls.Remove(panel);
                        dockspace.Controls.Add(panel);
                    }
                    panel.ShowCaption = true;
                    panel.Visible = true;
                    dockspace.LayoutPanels();
                    dockspace.Invalidate();
                }
                else
                {
                    // Legacy fallback: no dockspace on the host yet. Add to host form and let the
                    // layout engine position the panel. This path is rare (dockspaces are normally
                    // designer-created); we keep it so callers without a dockspace still work.
                    Debug.WriteLine($"[BeepDockingManager] AddPanel('{panelKey}'): no BeepDockspace for {dockPosition}; falling back to host-form hosting.");
                    _hostForm.Controls.Add(panel);
                    panel.BringToFront();
                }

                // Position panels + create/place edge splitters via the layout engine.
                ApplyLayout();
            }

            // Register tab for interaction handling
            _tabHandler?.RegisterTab(panelKey, title ?? "Panel");

            OnPanelAdded(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel added: {panelKey}");
            return panel;
        }

        /// <summary>
        /// Registers a DockPanel that was created by the WinForms designer and
        /// already exists on the host form. This is the design-time equivalent of
        /// <see cref="AddPanel"/> without creating a second control instance.
        /// </summary>
        internal bool RegisterExistingPanel(DockPanel panel)
        {
            if (panel == null || IsDesignHosted || _disposed)
                return false;

            if (string.IsNullOrWhiteSpace(panel.Key))
                return false;

            InitializeSubsystems();

            if (_panelsByKey.TryGetValue(panel.Key, out var existing))
                return ReferenceEquals(existing, panel);

            _panelsByKey[panel.Key] = panel;
            ApplyThemeToPanel(panel);

            if (_layoutTree.GetPanel(panel.Key) == null)
                _layoutTree.RegisterPanel(panel);

            var group = GetOrCreateGroupAtPosition(panel.DockPosition);
            group.AddPanel(panel);

            // The designer has already placed this panel inside its dockspace (or directly on
            // the host form for legacy paths). We do NOT reparent — the dockspace's
            // LayoutPanels() / OnLayout is the authoritative layout for its child panels.
            if (panel.Parent is BeepDockspace dockspace)
            {
                dockspace.LayoutPanels();
                dockspace.Invalidate();
            }
            else if (_hostForm != null && panel.Parent == _hostForm)
            {
                panel.BringToFront();
                ApplyLayout();
            }

            _tabHandler?.RegisterTab(panel.Key, panel.Title ?? "Panel");
            _layoutController?.InvalidateLayout();
            OnPanelAdded(panel);

            Debug.WriteLine($"[BeepDockingManager] Existing designer panel registered: {panel.Key}");
            return true;
        }

        /// <summary>
        /// Unregisters a designer-created DockPanel without disposing the control.
        /// Used when a panel's Manager property is changed in generated code or at runtime.
        /// </summary>
        internal void UnregisterExistingPanel(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (!_panelsByKey.TryGetValue(panel.Key, out var existing) || !ReferenceEquals(existing, panel))
                return;

            if (panel.State == DockPanelState.Floating)
                CloseFloatWindowFor(panel);
            else if (panel.State == DockPanelState.AutoHidden)
                DetachFromAutoHideStrip(panel);

            panel.Group?.RemovePanel(panel);
            _tabHandler?.UnregisterTab(panel.Key);

            _panelsByKey.Remove(panel.Key);
            _layoutTree.UnregisterPanel(panel.Key);
            _layoutController?.InvalidateLayout();
            // Reflow so the orphaned edge splitter (keyed by group id) is disposed by SyncSplitters.
            RecalculateLayout();
            OnPanelRemoved(panel);

            Debug.WriteLine($"[BeepDockingManager] Existing designer panel unregistered: {panel.Key}");
        }

        private void RegisterDesignerCreatedPanels(Control root)
        {
            if (root == null || IsDesignHosted)
                return;

            // The designer (and any host-form code) has already placed DockPanel children inside
            // BeepDockspace containers. We respect that structure: the dockspace stays in
            // hostForm.Controls and arranges its own child panels. We just walk every panel we
            // can find and add it to the layout tree.
            foreach (var panel in EnumerateDockPanels(root).OrderBy(p => p.TabIndex).ToList())
            {
                if (ReferenceEquals(panel.Manager, this))
                    RegisterExistingPanel(panel);
            }
        }

        /// <summary>
        /// Removes a panel from the manager, layout tree, and host form Controls.
        /// </summary>
        public bool RemovePanel(string panelKey)
        {
            if (!_panelsByKey.TryGetValue(panelKey, out var panel))
                return false;

            if (panel.State == DockPanelState.Floating)
                CloseFloatWindowFor(panel);
            else if (panel.State == DockPanelState.AutoHidden)
                DetachFromAutoHideStrip(panel);

            panel.Group?.RemovePanel(panel);

            _tabHandler?.UnregisterTab(panelKey);

            // Remove from parent (dockspace or host form).
            DetachPanelFromParent(panel);

            panel.Dispose();

            _panelsByKey.Remove(panelKey);
            _closedPanels.Remove(panelKey);   // also clean up closed store in case panel was reopened then removed
            _layoutTree.UnregisterPanel(panelKey);
            RemoveMrPanel(panelKey);

            _layoutController?.InvalidateLayout();
            // Reflow so remaining panels reclaim the freed space and SyncSplitters disposes
            // any now-orphaned edge splitter (splitters are keyed by group id, not panel key).
            RecalculateLayout();
            OnPanelRemoved(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel removed: {panelKey}");
            return true;
        }

        internal void NotifyPanelDockPositionChanged(DockPanel panel, DockPosition oldPosition)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (!_panelsByKey.TryGetValue(panel.Key, out var existing) || !ReferenceEquals(existing, panel))
                return;

            // Position metadata for floating/auto-hidden panels is applied when they re-dock.
            if (panel.State != DockPanelState.Docked && panel.State != DockPanelState.Hidden)
                return;

            panel.Group?.RemovePanel(panel);
            var group = GetOrCreateGroupAtPosition(panel.DockPosition);
            group.AddPanel(panel);

            if (_hostForm != null && panel.Parent == _hostForm)
                panel.BringToFront();

            _layoutController?.InvalidateLayout();
            RecalculateLayout();   // engine repositions panels + reconciles splitters
        }

        internal void NotifyPanelTitleChanged(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            _tabHandler?.UpdateTabLabel(panel.Key, panel.Title ?? "Panel");

            // Repaint the parent dockspace header so the tab label updates immediately.
            if (panel.Parent is BeepDockspace dockspace)
                dockspace.Invalidate();

            // Update float window caption if the panel is floating.
            if (_floatWindowsByKey.TryGetValue(panel.Key, out var fw) && fw != null && !fw.IsDisposed)
                fw.Text = panel.Title ?? panel.Key;
        }

        internal void NotifyPanelPreferredSizeChanged(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (!_panelsByKey.TryGetValue(panel.Key, out var existing) || !ReferenceEquals(existing, panel))
                return;

            if (_hostForm != null && panel.Parent == _hostForm)
                panel.Invalidate();

            _layoutController?.InvalidateLayout();
            RecalculateLayout();
        }
    }
}
