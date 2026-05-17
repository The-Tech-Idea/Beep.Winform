// BeepDocumentHostDesigner.SetupHost.cs
// Phase 07 — commercial-grade design-time setup experience for
// BeepDocumentHost (the standalone tabbed document control).
//
// This partial adds:
//   • InitializeNewComponent — launches a quick setup on first drop so the
//     control is ready-to-use without hunting through smart-tags.
//   • ShowQuickSetupWizard   — opens DocumentSetupWizardDialog (Native MDI
//     tile is suppressed because the host is a control, not a Form). Applies
//     the result (tab style + seed sample documents + optional template) in
//     a single designer transaction.
//   • DescribeHostState      — plain-English status banner used by the smart-
//     tag so the user always knows what the host is configured for.
//
// All mutations go through DesignerTransaction + IComponentChangeService so
// Ctrl+Z reverses the entire first-drop setup.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepDocumentHostDesigner
    {
        // ── First-drop wizard ─────────────────────────────────────────────

        /// <summary>
        /// Runs ONCE when the user drops a BeepDocumentHost from the toolbox.
        /// Opens the quick setup wizard so the host is ready-to-use with zero
        /// manual property tweaking.
        /// </summary>
        public override void InitializeNewComponent(IDictionary? defaultValues)
        {
            base.InitializeNewComponent(defaultValues);
            try
            {
                // Honor the developer's "Don't show again" preference (HKCU).
                // Shared with BeepDocumentManagerDesigner so a single opt-out
                // applies to both component types.
                if (!BeepDocumentManagerDesigner.ShouldAutoOpenWizard()) return;

                if (Component is BeepDocumentHost host && host.Site != null)
                {
                    var sync = SynchronizationContext.Current;
                    if (sync != null)
                        sync.Post(_ => SafeShowQuickSetupWizard(), null);
                    else
                        SafeShowQuickSetupWizard();
                }
            }
            catch
            {
                // Toolbox drop must never fail because of the wizard.
            }
        }

        private void SafeShowQuickSetupWizard()
        {
            try { ShowQuickSetupWizard(); } catch { /* design-time guard */ }
        }

        /// <summary>
        /// Opens the unified document setup wizard scoped to a single
        /// BeepDocumentHost (no companion BeepDocumentManager needed).
        /// </summary>
        internal void ShowQuickSetupWizard()
        {
            if (_wiredHost == null) return;

            DocumentSetupMode initial = IsBrowserStyled(_wiredHost)
                ? DocumentSetupMode.BrowserTabs
                : DocumentSetupMode.TabbedDocuments;

            int existingDocs = _wiredHost.Groups?.Sum(g => g.DocumentIds?.Count ?? 0) ?? 0;

            using var dlg = new DocumentSetupWizardDialog(initial, existingDocs, hostOptions: null);
            DialogResult dr = dlg.ShowDialog();

            if (dlg.Result.DoNotShowAgain)
                BeepDocumentManagerDesigner.SetAutoOpenWizard(false);

            if (dr != DialogResult.OK || dlg.Result.ConfigureLater) return;

            ApplyQuickSetupResult(_wiredHost, dlg.Result);
        }

        private void ApplyQuickSetupResult(BeepDocumentHost host, DocumentSetupResult result)
        {
            if (DesignerHost == null) return;

            // The host is a Control, not a Form. Native MDI requires a Form.
            // If the user chose Native MDI, fall back to standard tabs and
            // tell them how to enable MDI properly.
            if (result.Mode == DocumentSetupMode.NativeMdi)
            {
                MessageBox.Show(
                    "Native MDI is a Form-level mode. Drop a BeepDocumentManager onto " +
                    "the form and run its setup wizard to enable Native MDI.",
                    "Native MDI requires BeepDocumentManager",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var txn = DesignerHost.CreateTransaction("Beep Document Host Setup");
            try
            {
                if (result.Mode == DocumentSetupMode.BrowserTabs)
                {
                    SetHostProperty(host, nameof(BeepDocumentHost.TabStyle),      DocumentTabStyle.Chrome);
                    SetHostProperty(host, nameof(BeepDocumentHost.ShowAddButton), true);
                    SetHostProperty(host, nameof(BeepDocumentHost.CloseMode),     TabCloseMode.Always);
                    SetHostProperty(host, nameof(BeepDocumentHost.TabPosition),   TabStripPosition.Top);
                }
                else // TabbedDocuments — sensible IDE defaults
                {
                    SetHostProperty(host, nameof(BeepDocumentHost.TabStyle),      DocumentTabStyle.Chrome);
                    SetHostProperty(host, nameof(BeepDocumentHost.ShowAddButton), false);
                    SetHostProperty(host, nameof(BeepDocumentHost.CloseMode),     TabCloseMode.OnHover);
                    SetHostProperty(host, nameof(BeepDocumentHost.TabPosition),   TabStripPosition.Top);
                }

                if (result.AddSampleDocuments && result.SampleDocumentCount > 0)
                {
                    SeedHostSampleDocuments(host, result.SampleDocumentCount);
                }

                txn.Commit();
            }
            catch
            {
                txn.Cancel();
                throw;
            }

            // Site any newly created panels for design-time selection.
            SiteAllDesignPanels();
            RefreshDesignerActionUI();
        }

        private void SeedHostSampleDocuments(BeepDocumentHost host, int count)
        {
            // Re-use the existing design-time document collection so that the
            // panels are persisted into DesignTimeDocuments / DesignTimeLayoutJson
            // exactly like the smart-tag "Add Design-Time Document" verb does.
            for (int i = 0; i < count; i++)
            {
                AddDesignTimeDocument();
            }
        }

        // ── Status banner ─────────────────────────────────────────────────

        /// <summary>
        /// Returns a plain-English description of the host's current
        /// configuration for the smart-tag status entry.
        /// </summary>
        internal string DescribeHostState()
        {
            if (_wiredHost is not BeepDocumentHost host) return "(unbound)";

            int docs = host.Groups?.Sum(g => g.DocumentIds?.Count ?? 0) ?? 0;
            string mode = IsBrowserStyled(host) ? "Browser Tabs" : "Tabbed Documents";

            // Empty host → highlight the one thing the user can do RIGHT NOW.
            // Empty-state messaging is identical to the manager designer so the
            // user sees the same plain-English hint whether they are reading the
            // host smart-tag or the manager smart-tag.
            if (docs == 0)
            {
                return $"{mode} · 0 docs — drop a control on the host (creates a tab) " +
                       "or right-click for 'Add Document'.";
            }

            string position = host.TabPosition.ToString();
            string close    = host.CloseMode.ToString();
            string addBtn   = host.ShowAddButton ? "+ button" : "no + button";
            return $"{mode} · {docs} doc{(docs == 1 ? "" : "s")} · {position} · close: {close} · {addBtn}";
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static bool IsBrowserStyled(BeepDocumentHost? host)
            => host != null
            && host.ShowAddButton
            && host.TabStyle == DocumentTabStyle.Chrome
            && host.CloseMode == TabCloseMode.Always;

        private IDesignerHost? DesignerHost
            => GetService(typeof(IDesignerHost)) as IDesignerHost;

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
                try { prop.SetValue(host, value); } catch { /* non-fatal */ }
            }
        }
    }
}
