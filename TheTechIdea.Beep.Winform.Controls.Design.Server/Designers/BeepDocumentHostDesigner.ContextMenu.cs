// BeepDocumentHostDesigner.ContextMenu.cs
// Phase 03 — split out of BeepDocumentHostDesigner.cs.
//
// Design-time right-click context menu for BeepDocumentHost, plus the
// supporting MouseUp/MouseDown surface wiring on the host itself and on
// every nested BeepDocumentTabStrip child.
//
// Surface wiring is auto-discovered: WireDesignContextMenuSurfaces walks
// host.Controls during Initialize() and HookDesignContextMenuSurface is
// re-invoked through Host_ControlAdded/Removed so tabs added later are
// also pickable. UnhookDesignContextMenuSurface clears event handlers
// during Dispose to prevent designer leaks.
//
// IsDesignContextMenuSurface gates which controls we attach to (host,
// tab strips, direct children) so we never compete with the standard
// Properties grid context-menu of unrelated controls.
//
// Left-click on a tab strip hit-tests the tab and routes ISelectionService
// to the corresponding BeepDocumentPanel — same UX as DevExpress
// XtraTabbedView's design-time tab selection.
//
// Right-click builds a contextual menu via BuildDesignContextMenu and
// themes it with ApplyThemeToDesignMenu against the host's current
// BackColor/ForeColor. The menu is disposed when it closes.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepDocumentHostDesigner
    {
        // ── Host child-tracking for menu surface wiring ──────────────────────

        private void Host_ControlAdded(object? sender, ControlEventArgs e)
        {
            HookDesignContextMenuSurface(e.Control);

            // Site any BeepDocumentPanel that is added at design time (e.g. by
            // BeepDocumentManager via the view) so it becomes selectable.
            if (e.Control is BeepDocumentPanel panel && _wiredHost != null)
            {
                IContainer? container = GetNestedContainer() ?? GetDesignerHost()?.Container;
                if (container != null)
                {
                    SiteDesignPanel(container, panel, panel.DocumentId);
                }
            }
        }

        private void Host_ControlRemoved(object? sender, ControlEventArgs e)
        {
            UnhookDesignContextMenuSurface(e.Control);

            // Unsite the panel when it is removed from the host so the designer
            // container does not hold a stale reference.
            if (e.Control is BeepDocumentPanel panel)
            {
                _sitedPanels.Remove(panel);
                try
                {
                    if (panel.Site?.Container is IContainer container)
                        container.Remove(panel);
                }
                catch { /* designer may already be tearing down */ }
            }
        }

        private void WireDesignContextMenuSurfaces(BeepDocumentHost host)
        {
            HookDesignContextMenuSurface(host);
            foreach (Control child in host.Controls)
            {
                HookDesignContextMenuSurface(child);
            }
        }

        private void HookDesignContextMenuSurface(Control control)
        {
            if (!IsDesignContextMenuSurface(control) || !_contextMenuSurfaces.Add(control))
            {
                return;
            }

            control.MouseUp += DesignContextSurface_MouseUp;

            // Left-click on a tab header → select that document's panel in the Properties window.
            if (control is BeepDocumentTabStrip)
                control.MouseDown += DesignTabStrip_MouseDown;
        }

        private void UnhookDesignContextMenuSurface(Control control)
        {
            if (!_contextMenuSurfaces.Remove(control))
            {
                return;
            }

            control.MouseUp -= DesignContextSurface_MouseUp;

            if (control is BeepDocumentTabStrip)
                control.MouseDown -= DesignTabStrip_MouseDown;
        }

        private static bool IsDesignContextMenuSurface(Control control)
            => control is BeepDocumentHost or BeepDocumentTabStrip || control.Parent is BeepDocumentHost;

        // ── Mouse handlers ──────────────────────────────────────────────────

        /// <summary>
        /// Left-click on a tab header selects the corresponding <see cref="BeepDocumentPanel"/>
        /// in the Visual Studio Properties window — identical to DevExpress XtraTabbedView behaviour.
        /// </summary>
        private void DesignTabStrip_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left
                || sender is not BeepDocumentTabStrip strip
                || Component is not BeepDocumentHost host)
                return;

            var hit = strip.HitTestTab(e.Location);
            if (!hit.Hit || hit.TabIndex < 0 || hit.TabIndex >= strip.Tabs.Count)
                return;

            if (hit.IsCloseButton || hit.IsAddButton || hit.IsScrollLeft || hit.IsScrollRight || hit.IsOverflowButton)
                return;

            string tabId = strip.Tabs[hit.TabIndex].Id;
            if (string.IsNullOrWhiteSpace(tabId)) return;

            var panel = host.GetPanel(tabId);
            if (panel == null) return;

            GetSelectionService()?.SetSelectedComponents(
                new object[] { panel },
                SelectionTypes.Replace);
        }

        private void DesignContextSurface_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || Component is not BeepDocumentHost host)
            {
                return;
            }

            Control surface = sender as Control ?? host;
            if (surface is BeepDocumentTabStrip strip
                && strip.ActiveTabIndex >= 0
                && strip.ActiveTabIndex < strip.Tabs.Count)
            {
                string tabId = strip.Tabs[strip.ActiveTabIndex].Id;
                if (!string.IsNullOrWhiteSpace(tabId))
                {
                    host.SetActiveDocument(tabId);
                }
            }

            ShowDesignContextMenu(host, surface, e.Location);
        }

        // ── Menu construction & theming ─────────────────────────────────────

        private void ShowDesignContextMenu(BeepDocumentHost host, Control surface, Point location)
        {
            var menu = BuildDesignContextMenu(host);
            ApplyThemeToDesignMenu(host, menu);
            menu.Closed += (s, e) => menu.Dispose();
            menu.Show(surface, location);
        }

        private ContextMenuStrip BuildDesignContextMenu(BeepDocumentHost host)
        {
            var menu = new ContextMenuStrip { ShowImageMargin = false };
            bool hasActiveDocument = !string.IsNullOrWhiteSpace(host.ActiveDocumentId);
            DocumentDockState dockState = GetActiveDocumentDockState();
            bool isPinned = hasActiveDocument
                && FindDesignTimeDocument(host.DesignTimeDocuments, host.ActiveDocumentId!)?.IsPinned == true;

            if (hasActiveDocument)
            {
                string title = GetActiveDocumentTitle();
                menu.Items.Add(new ToolStripMenuItem($"Active Document: {title}") { Enabled = false });
                menu.Items.Add(new ToolStripMenuItem($"Dock State: {dockState}") { Enabled = false });
                menu.Items.Add(new ToolStripSeparator());
            }

            menu.Items.Add(CreateDesignMenuItem("Add Document", AddDesignTimeDocument));
            menu.Items.Add(CreateDesignMenuItem("Layout Assistant\u2026", ShowLayoutAssistant));
            menu.Items.Add(CreateDesignMenuItem("Edit Documents\u2026", () =>
            {
                if (Component is BeepDocumentHost currentHost)
                {
                    EditDesignTimeDocuments(currentHost);
                }
            }));

            if (hasActiveDocument)
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateDesignMenuItem("Rename Active Document\u2026", RenameActiveDesignTimeDocumentWithPrompt));
                menu.Items.Add(CreateDesignMenuItem("Select Active Document Surface", SelectActiveDocumentSurface));
                menu.Items.Add(CreateDesignMenuItem("Close Active Document", CloseActiveDesignTimeDocument));
                menu.Items.Add(CreateDesignMenuItem(
                    dockState == DocumentDockState.Floating ? "Dock Back" : "Float",
                    dockState == DocumentDockState.Floating ? DockBackActiveDesignTimeDocument : FloatActiveDesignTimeDocument,
                    dockState is DocumentDockState.Docked or DocumentDockState.Floating));
                menu.Items.Add(CreateDesignMenuItem(
                    isPinned ? "Unpin Active Document" : "Pin Active Document",
                    () => SetActiveDocumentPinned(!isPinned),
                    dockState != DocumentDockState.None));

                if (dockState == DocumentDockState.AutoHide)
                {
                    menu.Items.Add(CreateDesignMenuItem("Restore Auto Hide", RestoreAutoHideActiveDesignTimeDocument));
                }

                if (dockState == DocumentDockState.Docked)
                {
                    menu.Items.Add(BuildAutoHideMenu());
                }
            }

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(CreateDesignMenuItem("Split With New Document Right", () => CreateSplitDesignTimeDocument(horizontal: true)));
            menu.Items.Add(CreateDesignMenuItem("Split With New Document Down", () => CreateSplitDesignTimeDocument(horizontal: false)));
            menu.Items.Add(CreateDesignMenuItem("Merge All Groups", MergeAllDesignTimeGroups, host.Groups.Count > 1));
            menu.Items.Add(BuildLayoutPresetMenu());

            if (CanReopenLastClosedDesignTimeDocument)
            {
                menu.Items.Add(CreateDesignMenuItem("Reopen Last Closed", ReopenLastClosedDesignTimeDocument));
            }

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(CreateDesignMenuItem("View Layout Tree\u2026", () => ShowLayoutTreeDialog(host)));
            menu.Items.Add(CreateDesignMenuItem("Set Group Tab Position\u2026", () => EditGroupTabPositions(host), host.Groups.Count > 1));

            return menu;
        }

        private ToolStripMenuItem BuildAutoHideMenu()
        {
            var menuItem = new ToolStripMenuItem("Auto Hide");
            foreach (AutoHideSide side in Enum.GetValues(typeof(AutoHideSide)))
            {
                menuItem.DropDownItems.Add(CreateDesignMenuItem($"{GetAutoHideLabel(side)}", () => AutoHideActiveDesignTimeDocument(side)));
            }

            return menuItem;
        }

        private ToolStripMenuItem BuildLayoutPresetMenu()
        {
            var menuItem = new ToolStripMenuItem("Quick Layout Presets");
            foreach (LayoutPreset preset in GetLayoutPresetOrder())
            {
                menuItem.DropDownItems.Add(CreateDesignMenuItem(GetLayoutPresetDisplayName(preset), () => ApplyDesignTimeLayoutPreset(preset)));
            }

            return menuItem;
        }

        private static ToolStripMenuItem CreateDesignMenuItem(string text, Action action, bool enabled = true)
        {
            var menuItem = new ToolStripMenuItem(text) { Enabled = enabled };
            menuItem.Click += (s, e) => action();
            return menuItem;
        }

        private void ApplyThemeToDesignMenu(BeepDocumentHost host, ContextMenuStrip menu)
        {
            menu.BackColor = host.BackColor;
            menu.ForeColor = host.ForeColor;
            menu.Renderer = new DocumentHostDesignerMenuRenderer(host.BackColor, host.ForeColor);

            foreach (ToolStripItem item in menu.Items)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.DropDownOpening += (s, e) =>
                    {
                        if (menuItem.DropDown is ContextMenuStrip dropDown)
                        {
                            ApplyThemeToDesignMenu(host, dropDown);
                        }
                    };
                }
            }
        }
    }
}
