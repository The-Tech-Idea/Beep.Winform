// BeepDocumentManagerDesigner.ViewMode.cs
// Phase 01 + Phase 07 — BeepDocumentHost / BrowserTabs clarity and
// commercial-grade setup experience.
//
// Public surface used by the main partial:
//   • ApplyTabbedViewMode    (with optional Chrome / browser styling)
//   • ApplyBrowserTabsMode   (Tabbed + ShowAddButton + Chrome style + always-close)
//   • ShowSetupWizard        (modal DocumentSetupWizardDialog)
//   • ApplySetupResult       (applies a result without re-showing the wizard)
//   • SeedSampleDocuments    (adds N starter documents transactionally)
//   • DescribeViewMode       (human-readable status banner)
//
// Every mutation goes through DesignerTransaction + IComponentChangeService so
// Ctrl+Z reverses the entire setup, including auto-wired companion components.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed partial class BeepDocumentManagerDesigner
    {
        // ══════════════════════════════════════════════════════════════════
        // Mode A — Tabbed Documents (default IDE-style)
        // ══════════════════════════════════════════════════════════════════

        internal void ApplyTabbedViewMode(bool showInfo)
            => ApplyTabbedViewModeCore(browserStyle: false, showInfo);

        // ══════════════════════════════════════════════════════════════════
        // Mode B — Browser Tabs (Chrome + Add button + Always close)
        // ══════════════════════════════════════════════════════════════════

        internal void ApplyBrowserTabsMode(bool showInfo)
            => ApplyTabbedViewModeCore(browserStyle: true, showInfo);

        private void ApplyTabbedViewModeCore(bool browserStyle, bool showInfo)
        {
            if (_manager == null || DesignerHost == null) return;

            // Auto-create or reuse existing BeepDocumentHost
            BeepDocumentHost? host = EnsureBeepDocumentHostExists(out string hostStatus, out bool hostAutoCreated);
            if (host == null) return;
            EnsureManagerUsesHost(host);

            if (browserStyle)
            {
                ApplyBrowserStyleToHost(host);
            }

            if (showInfo)
            {
                string hostName = host.Site?.Name ?? "BeepDocumentHost";
                string createdNote = hostAutoCreated
                    ? $"Auto-created '{hostName}' on the root form (Dock = Fill).\n\n"
                    : $"Using existing '{hostName}'.\n\n";
                string msg = (browserStyle
                        ? "Browser Tabs mode applied. "
                        : "Tabbed Documents mode applied. ") +
                      createdNote +
                      (browserStyle
                        ? "Chrome tab style, new-tab button visible, close button always on."
                        : "Tabs, splits, floating windows, pinned tabs, and themes are available.");

                HideSmartTag(); // Dismiss panel before opening modal.
                MessageBox.Show(msg,
                    browserStyle ? "Use Browser Tabs" : "Use Tabbed Documents",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            RefreshPanel();
        }

        // ══════════════════════════════════════════════════════════════════
        // Legacy Native MDI entry point (kept for binary/source compatibility).
        // ══════════════════════════════════════════════════════════════════

        internal void ApplyNativeMdiViewMode(bool showInfo)
        {
            ApplyTabbedViewModeCore(browserStyle: false, showInfo: false);

            if (showInfo)
            {
                HideSmartTag();
                MessageBox.Show(
                    "The designer now uses a single BeepDocumentHost surface so tabs, headers, and tab contents are selectable and serialized in Designer.cs.\n\n" +
                    "Native MDI is a runtime-only strategy and is not exposed by the design-time setup.",
                    "Tabbed Document Host",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            RefreshPanel();
        }

        // ══════════════════════════════════════════════════════════════════
        // Setup wizard — top-level entry point
        // ══════════════════════════════════════════════════════════════════

        internal void ShowSetupWizard()
        {
            if (_manager == null) return;

            // Hot-fix 2026-05-17: dismiss the smart-tag panel before showing
            // the modal wizard. Otherwise the action panel stays painted on
            // top of the design surface once the wizard closes, hiding the
            // very view it just configured.
            HideSmartTag();

            DocumentSetupMode initial = _manager.Host switch
            {
                BeepDocumentHost h when IsBrowserStyledHost(h) => DocumentSetupMode.BrowserTabs,
                BeepDocumentHost                                => DocumentSetupMode.TabbedDocuments,
                _                                               => DocumentSetupMode.TabbedDocuments
            };

            // Reflect the *current* state to the wizard:
            //   – existing-doc count so the wizard can skip sample seeding,
            //   – the list of BeepDocumentHost candidates so the user can pick.
            int existingDocs = 0;
            var current = _manager.Host;
            if (current != null)
                existingDocs = current.Groups?.Sum(g => g.DocumentIds?.Count ?? 0) ?? 0;

            var hostOptions = EnumerateHostsOnSurface();

            using var dlg = new DocumentSetupWizardDialog(initial, existingDocs, hostOptions);
            DialogResult dr = dlg.ShowDialog();

            // Always honor "Don't show again" — even on Cancel — so a developer
            // who closes the wizard via the X / Configure Later still gets their
            // preference saved when they explicitly ticked the box.
            if (dlg.Result.DoNotShowAgain)
                SetAutoOpenWizard(false);

            if (dr != DialogResult.OK || dlg.Result.ConfigureLater) return;

            ApplySetupResult(dlg.Result);
        }

        /// <summary>
        /// Enumerates every BeepDocumentHost currently on the design surface.
        /// Returns null if the wizard's multi-host picker isn't needed
        /// (0 or 1 hosts → designer auto-resolves).
        /// </summary>
        private IReadOnlyList<HostPickerOption>? EnumerateHostsOnSurface()
        {
            if (DesignerHost?.Container == null) return null;

            var list = DesignerHost.Container.Components
                .OfType<BeepDocumentHost>()
                .Select(h => new HostPickerOption(
                    h.Site?.Name ?? h.Name ?? h.GetType().Name,
                    h))
                .ToList();

            return list.Count >= 2 ? list : null;
        }

        /// <summary>
        /// When the wizard's multi-host picker is in play, the chosen host
        /// short-circuits <see cref="ResolveBeepDocumentHost"/> for the
        /// duration of <see cref="ApplySetupResult"/>.
        /// </summary>
        private BeepDocumentHost? _pinnedHostForSetup;

        /// <summary>
        /// Applies a wizard result without re-showing the dialog. Used by both
        /// the smart-tag wizard verb and the InitializeNewComponent first-drop
        /// hook.
        /// </summary>
        internal void ApplySetupResult(DocumentSetupResult result)
        {
            if (_manager == null || DesignerHost == null) return;

            _pinnedHostForSetup = result.SelectedHostComponent as BeepDocumentHost;
            _suppressImmediateHostSync = true;

            using var txn = DesignerHost.CreateTransaction("Beep Document Area Setup");
            try
            {
                switch (result.Mode)
                {
                    case DocumentSetupMode.TabbedDocuments:
                        ApplyTabbedViewMode(showInfo: false);
                        break;
                    case DocumentSetupMode.BrowserTabs:
                        ApplyBrowserTabsMode(showInfo: false);
                        break;
                    case DocumentSetupMode.NativeMdi:
                        ApplyTabbedViewMode(showInfo: false);
                        break;
                }

                if (result.AddSampleDocuments)
                {
                    SeedSampleDocuments(result.SampleDocumentCount);
                }

                if (result.ApplyTemplate)
                {
                    ApplyLayoutTemplate(result.TemplateId);
                }

                txn.Commit();
            }
            catch
            {
                txn.Cancel();
                throw;
            }
            finally
            {
                _suppressImmediateHostSync = false;
                _pinnedHostForSetup = null;
            }

            ScheduleManagerDocumentsHostSync();

            RefreshPanel();
            HideSmartTag();
        }

        // ══════════════════════════════════════════════════════════════════
        // Sample-document seeding
        // ══════════════════════════════════════════════════════════════════

        internal void SeedSampleDocuments(int count)
        {
            if (_manager?.Host == null) return;
            if (count <= 0) return;

            int startIndex = _manager.DesignTimeDocuments.Count + 1;
            int addedViaHostDesigner = 0;
            for (int i = 0; i < count; i++)
            {
                if (!TryAddDocumentViaHostDesigner($"Document {startIndex + i}", activate: i == count - 1, selectSurface: i == count - 1))
                {
                    break;
                }

                addedViaHostDesigner++;
            }

            if (addedViaHostDesigner == count)
            {
                return;
            }

            // Phase 04: seeded sample documents must round-trip through
            // serialization, so they are appended to DesignTimeDocuments
            // (with change-service notification) and re-applied to the view.
            // The whole batch runs inside MutateDesignTimeDocuments so it
            // participates in the outer "Beep Document Area Setup"
            // transaction created by ApplySetupResult.
            MutateDesignTimeDocuments("Seed Sample Documents", docs =>
            {
                for (int i = addedViaHostDesigner; i < count; i++)
                {
                    docs.Add(DesignTimeDocumentCoordinator.CreateDetachedDescriptor(
                        Guid.NewGuid().ToString("N"),
                        "Document " + (startIndex + i)));
                }
            });
        }

        internal void ApplyLayoutTemplate(string templateId)
        {
            // The runtime template catalogue lives in
            // BeepDocumentHost.Templates.cs. Map wizard ids to the closest
            // runtime templates that are available today. Phase 05 will grow
            // this table.
            var host = _manager?.Host;
            if (host == null) return;

            switch (templateId)
            {
                case "Visual Studio (3 panes)":
                    if (!TryInvokeHostTemplate(host, "ApplyVisualStudioLayout"))
                    {
                        TryInvokeHostTemplate(host, "ApplyThreePaneLayout");
                    }
                    break;
                case "Two-pane editor":
                    TryInvokeHostTemplate(host, "ApplyTwoPaneLayout");
                    break;
                case "Browser (single tab)":
                    // Browser already implied by ApplyBrowserStyleToHost; nothing extra.
                    break;
                case "Empty":
                default:
                    // no-op
                    break;
            }
        }

        private static bool TryInvokeHostTemplate(BeepDocumentHost host, string methodName)
        {
            var m = host.GetType().GetMethod(methodName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                Type.EmptyTypes);
            if (m == null) return false;
            try { m.Invoke(host, null); return true; }
            catch { return false; }
        }

        // ══════════════════════════════════════════════════════════════════
        // Status banner — plain-English current configuration
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Returns a one-line human-readable description of the manager's
        /// current setup. When something is missing or empty, the line ends
        /// with an actionable hint that points at the verb or smart-tag item
        /// the user should click — commercial MDI platforms (DevExpress
        /// XtraTabbedView, Telerik RadDocking) do the same so the developer
        /// never has to guess.
        ///   "Tabbed Documents · 3 docs · Top tabs · Splits allowed"
        ///   "Tabbed Documents · 0 docs — drop a control or click 'Add Document'."
        ///   "Browser Tabs · 1 doc · Top tabs · + button"
        ///   "Tabbed Documents · Host not assigned — drop a BeepDocumentHost or re-run the Setup Wizard."
        ///   "Not configured — click 'Setup Wizard…' to start."
        /// Always safe to call.
        /// </summary>
        internal string DescribeViewMode()
        {
            if (_manager == null) return "(unbound)";

            var host = _manager.Host;
            if (host == null)
                return "Not configured — click 'Setup Wizard…' to start.";

            int docs = host.Groups?.Sum(g => g.DocumentIds?.Count ?? 0) ?? 0;
            string position = host.TabPosition.ToString();
            bool browser = IsBrowserStyledHost(host);
            string label = browser ? "Browser Tabs" : "Tabbed Documents";

            // Empty host → tell the user exactly which two things they can do.
            if (docs == 0)
            {
                return $"{label} · 0 docs — drop a control on the host (creates a tab) " +
                       "or click 'Add Document'.";
            }

            string flags =
                (browser ? "+ button" : (host.AllowSplit ? "Splits allowed" : "No splits"));
            return $"{label} · {docs} doc{(docs == 1 ? "" : "s")} · {position} tabs · {flags}";
        }

        // ══════════════════════════════════════════════════════════════════
        // Helpers
        // ══════════════════════════════════════════════════════════════════

        private static bool IsBrowserStyledHost(BeepDocumentHost? host)
            => host != null
            && host.ShowAddButton
            && host.TabStyle == DocumentTabStyle.Chrome
            && host.CloseMode == TabCloseMode.Always;

        private void ApplyBrowserStyleToHost(BeepDocumentHost host)
        {
            if (DesignerHost == null) return;
            using var txn = DesignerHost.CreateTransaction("Apply Browser Style");
            try
            {
                SetHostProperty(host, nameof(BeepDocumentHost.TabStyle),       DocumentTabStyle.Chrome);
                SetHostProperty(host, nameof(BeepDocumentHost.ShowAddButton),  true);
                SetHostProperty(host, nameof(BeepDocumentHost.CloseMode),      TabCloseMode.Always);
                SetHostProperty(host, nameof(BeepDocumentHost.TabPosition),    TabStripPosition.Top);
                txn.Commit();
            }
            catch { txn.Cancel(); }
        }

        private void SetHostProperty(BeepDocumentHost host, string propertyName, object? value)
        {
            var prop = TypeDescriptor.GetProperties(host)[propertyName];
            if (prop == null) return;
            var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            try
            {
                changeSvc?.OnComponentChanging(host, prop);
                object? oldValue = prop.GetValue(host);
                prop.SetValue(host, value);
                changeSvc?.OnComponentChanged(host, prop, oldValue, value);
            }
            catch
            {
                try { prop.SetValue(host, value); } catch { /* fall-through; non-fatal at design time */ }
            }
        }

        private void EnsureManagerUsesHost(BeepDocumentHost host)
        {
            if (_manager == null || ReferenceEquals(_manager.Host, host))
            {
                return;
            }

            if (!_manager.Hosts.Contains(host))
            {
                var hostsProp = TypeDescriptor.GetProperties(_manager)[nameof(BeepDocumentManager.Hosts)];
                var changeSvcForHosts = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                changeSvcForHosts?.OnComponentChanging(_manager, hostsProp);
                _manager.Hosts.Add(host);
                changeSvcForHosts?.OnComponentChanged(_manager, hostsProp, null, null);
            }

            PropertyDescriptor? prop = TypeDescriptor.GetProperties(_manager)[nameof(BeepDocumentManager.Host)];
            if (prop == null || prop.IsReadOnly)
            {
                return;
            }

            var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            object? oldValue = prop.GetValue(_manager);
            changeSvc?.OnComponentChanging(_manager, prop);
            prop.SetValue(_manager, host);
            changeSvc?.OnComponentChanged(_manager, prop, oldValue, host);
        }

        private BeepDocumentHost? ResolveBeepDocumentHost(out string status)
        {
            // 1. Pinned (wizard explicit pick) wins.
            if (_pinnedHostForSetup != null)
            {
                status = "Using the BeepDocumentHost selected in the wizard.";
                return _pinnedHostForSetup;
            }

            status = "No BeepDocumentHost found on the design surface.";
            if (DesignerHost?.Container == null) return null;

            var hosts = DesignerHost.Container.Components
                .OfType<BeepDocumentHost>()
                .ToList();

            if (hosts.Count == 1)
            {
                status = "Auto-wired to the single BeepDocumentHost on the surface.";
                return hosts[0];
            }
            if (hosts.Count > 1)
            {
                status = "Multiple BeepDocumentHost controls were found — please set the manager Host property manually " +
                         "or re-open the Setup Wizard to pick one.";
                return null;
            }
            return null;
        }

        /// <summary>
        /// Hot-fix 2026-05-17 — single-component-drop UX.
        ///
        /// Returns the BeepDocumentHost the manager should bind to.
        /// If one already exists on the surface it is reused; otherwise
        /// the designer creates one, drops it on the root Form, docks it
        /// Fill, and sends it to back so docked siblings (BeepMenuBar at
        /// top, status bars at bottom) keep their edges.
        ///
        /// This means dropping a single BeepDocumentManager → wizard → OK
        /// produces a fully wired MDI surface with NO manual control drops
        /// required. Matches DevExpress XtraDockManager, Telerik RadDock,
        /// and Syncfusion DockingManager behaviour.
        ///
        /// Returns null only when the root component isn't a Form (e.g. a
        /// UserControl) — the caller surfaces a helpful explanation in
        /// that one edge case.
        /// </summary>
        private BeepDocumentHost? EnsureBeepDocumentHostExists(out string status, out bool autoCreated)
        {
            autoCreated = false;

            // 1. Reuse existing (pinned pick or singleton on surface).
            var existing = ResolveBeepDocumentHost(out status);
            if (existing != null) return existing;

            // 2. No host found and ResolveBeepDocumentHost decided not to
            //    auto-pick (e.g. multiple hosts). Don't override that — we
            //    only auto-create when the surface has zero hosts.
            if (DesignerHost?.Container == null) return null;
            int existingHostCount = DesignerHost.Container.Components
                .OfType<BeepDocumentHost>()
                .Count();
            if (existingHostCount > 0) return null;

            // 3. Need a Form root to add a Control child to.
            if (DesignerHost.RootComponent is not Form rootForm)
            {
                status = "Root component is not a Form, so a BeepDocumentHost cannot be auto-created. " +
                         "Drop one manually.";
                return null;
            }

            using var txn = DesignerHost.CreateTransaction("Create BeepDocumentHost");
            try
            {
                var host = (BeepDocumentHost)DesignerHost.CreateComponent(typeof(BeepDocumentHost), GetNextHostComponentName());
                var changeSvc    = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                var controlsProp = TypeDescriptor.GetProperties(rootForm)["Controls"];

                changeSvc?.OnComponentChanging(rootForm, controlsProp);
                rootForm.Controls.Add(host);
                changeSvc?.OnComponentChanged(rootForm, controlsProp, null, null);

                // Dock = Fill via the property descriptor so the change
                // service notifies serializers — the InitializeComponent
                // codegen will then write the Dock = Fill line.
                var dockProp = TypeDescriptor.GetProperties(host)[nameof(Control.Dock)];
                if (dockProp != null)
                {
                    var oldDock = dockProp.GetValue(host);
                    changeSvc?.OnComponentChanging(host, dockProp);
                    dockProp.SetValue(host, DockStyle.Fill);
                    changeSvc?.OnComponentChanged(host, dockProp, oldDock, DockStyle.Fill);
                }

                // Send to back so it doesn't obscure docked siblings
                // (menubar at top, status bar at bottom) that should claim
                // their edges first.
                try { host.SendToBack(); } catch { /* design-time; non-fatal */ }

                txn.Commit();
                autoCreated = true;
                status = $"Auto-created '{host.Site?.Name ?? "BeepDocumentHost"}' on '{rootForm.Site?.Name ?? rootForm.Name}'.";
                return host;
            }
            catch
            {
                txn.Cancel();
                status = "Failed to auto-create BeepDocumentHost (design-time error).";
                return null;
            }
        }

        private void SetIsMdiContainer(Form form, bool value)
        {
            if (form.IsMdiContainer == value) return;
            PropertyDescriptor? prop = TypeDescriptor.GetProperties(form)[nameof(Form.IsMdiContainer)];
            if (prop == null) return;

            var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            try
            {
                changeSvc?.OnComponentChanging(form, prop);
                prop.SetValue(form, value);
                changeSvc?.OnComponentChanged(form, prop, !value, value);
            }
            catch
            {
                form.IsMdiContainer = value;
            }
        }

        private void RefreshPanel()
        {
            if (GetService(typeof(DesignerActionUIService)) is DesignerActionUIService svc)
            {
                svc.Refresh(Component);
            }
        }
    }
}
