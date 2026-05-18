// BeepDocumentHostDesigner.PanelSiting.cs
// Phase 02 — design-time selection fix.
//
// At runtime, BeepDocumentHost.AddDocument(...) creates BeepDocumentPanel instances
// directly (new BeepDocumentPanel(id, title)) and adds them to the internal
// _contentArea panel. Those panels are NEVER sited as designer components, which is
// why clicking a tab header at design time did nothing in the Properties window —
// ISelectionService.SetSelectedComponents silently no-ops for un-sited components.
//
// This partial adds the missing piece: after every document panel action, the
// designer walks the host's groups and registers every panel with the host's
// INestedContainer. Sited panels:
//   - get their own BeepDocumentPanelDesigner instance (selection adorners, snap lines,
//     property-grid filtering already implemented),
//   - appear in the component tree under the host (DevExpress/Telerik pattern),
//   - participate in undo/redo and IComponentChangeService notifications,
//   - are automatically disposed when the host is removed from the design surface.
//
// The partial is intentionally small and isolated so the monolithic
// BeepDocumentHostDesigner.cs can be refactored further (Phase 03) without touching
// this fix.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepDocumentHostDesigner
    {
        // ── Tracking ──────────────────────────────────────────────────────────
        // Panels we have sited via INestedContainer so we can undo our work on
        // Dispose without yanking components the user may have added by hand.
        private readonly HashSet<BeepDocumentPanel> _sitedPanels = new HashSet<BeepDocumentPanel>();

        // Phase 04: set to true while the designer is being torn down so
        // UnsiteAllDesignPanels can skip the explicit nested.Remove(panel)
        // calls. The host's runtime Dispose will dispose nested children
        // automatically through the INestedContainer's own lifecycle, and
        // skipping the explicit removals here prevents IComponentChange-
        // Service.ComponentRemoved events from polluting the design
        // surface's undo stack with synthetic entries the user can't
        // sensibly redo.
        private bool _isUnsiting;

        // Sanitises a DocumentId into a valid C# identifier suffix so the nested
        // component name is stable, readable, and unique inside the host's
        // INestedContainer. Example: "doc-12ab" → "doc_12ab".
        private static readonly Regex s_invalidNameChars = new Regex(@"[^A-Za-z0-9_]", RegexOptions.Compiled);

        // ── Public entry points (called from main partial) ────────────────────

        /// <summary>
        /// Site every <see cref="BeepDocumentPanel"/> that the host currently knows
        /// about as a nested designer component. Safe to call repeatedly.
        /// Internal so <see cref="BeepDocumentManagerDesigner"/> can trigger siting
        /// after it mutates documents through the manager's design-time collection.
        /// </summary>
        internal void SiteAllDesignPanels()
        {
            if (_wiredHost == null) return;
            if (!IsDesignTimeHost(_wiredHost)) return;

            // Prefer INestedContainer (panels appear under host in component tree).
            // Fall back to root IContainer so panels are still sited and selectable.
            IContainer? container = GetNestedContainer() ?? GetDesignerHost()?.Container;
            if (container == null) return;

            foreach (var group in _wiredHost.Groups)
            {
                foreach (string documentId in group.DocumentIds)
                {
                    BeepDocumentPanel? panel = _wiredHost.GetPanel(documentId);
                    if (panel == null) continue;
                    SiteDesignPanel(container, panel, documentId);
                }
            }
        }

        /// <summary>
        /// Remove every panel we sited so they don't leak in the designer container
        /// after the host is removed from the design surface.
        /// </summary>
        /// <remarks>
        /// Phase 04: when called from <c>Dispose</c> (i.e. while
        /// <see cref="_isUnsiting"/> is true) we skip the explicit
        /// <c>nested.Remove(panel)</c> calls. The host's
        /// <see cref="INestedContainer"/> disposes its children automatically
        /// as part of the host's runtime <c>Dispose</c>, and skipping the
        /// explicit removal prevents synthetic
        /// <see cref="IComponentChangeService.ComponentRemoved"/> events
        /// from polluting the undo stack with entries the user can't sensibly
        /// reverse.
        /// </remarks>
        private void UnsiteAllDesignPanels()
        {
            if (_sitedPanels.Count == 0) return;

            if (!_isUnsiting)
            {
                foreach (BeepDocumentPanel panel in _sitedPanels.ToList())
                {
                    try
                    {
                        if (panel.Site?.Container is IContainer container)
                        {
                            container.Remove(panel);
                        }
                    }
                    catch { /* designer may already be tearing down */ }
                }
            }

            _sitedPanels.Clear();
        }

        // ── Internals ─────────────────────────────────────────────────────────

        /// <summary>
        /// Resolves the host's <see cref="INestedContainer"/> if available.
        /// The container is the standard WinForms mechanism for exposing sub-components
        /// (used by SplitContainer.Panel1/Panel2, ToolStripDropDown items, etc.).
        /// When unavailable, <see cref="SiteAllDesignPanels"/> falls back to the
        /// root <see cref="IContainer"/> so panels are still sited and selectable.
        /// </summary>
        private INestedContainer? GetNestedContainer()
        {
            if (_wiredHost?.Site == null) return null;
            return _wiredHost.Site.GetService(typeof(INestedContainer)) as INestedContainer;
        }

        private void SiteDesignPanel(IContainer container, BeepDocumentPanel panel, string documentId)
        {
            if (panel == null) return;
            if (panel.Site != null)
            {
                _sitedPanels.Add(panel);
                return;
            }

            string baseName = BuildPanelComponentName(documentId);
            string name = baseName;
            int suffix = 1;

            // IContainer.Add throws ArgumentException on duplicate names. Loop
            // until we find a free slot — designer container is per-host so the
            // base name should be unique on the first try.
            while (true)
            {
                try
                {
                    container.Add(panel, name);
                    _sitedPanels.Add(panel);
                    panel.SetDocumentTitleSafe(); // keep header text in sync after siting
                    return;
                }
                catch (ArgumentException)
                {
                    suffix++;
                    name = baseName + "_" + suffix;
                    if (suffix > 32) return; // give up; never throw out of design-time
                }
                catch
                {
                    return; // any other failure is non-fatal
                }
            }
        }

        private static string BuildPanelComponentName(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return "documentPanel";
            }
            string cleaned = s_invalidNameChars.Replace(documentId, "_");
            if (cleaned.Length == 0 || char.IsDigit(cleaned[0]))
            {
                cleaned = "doc_" + cleaned;
            }
            return cleaned;
        }

        /// <summary>
        /// Returns true when the host is sited in a design surface (LicenseManager
        /// reports Designtime). We never want to site panels at runtime.
        /// </summary>
        private static bool IsDesignTimeHost(BeepDocumentHost host)
        {
            if (host.Site?.DesignMode == true) return true;
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }
    }

    // ── BeepDocumentPanel siting helper ──────────────────────────────────────
    // The panel sometimes resets its DocumentTitle when re-sited (because Name
    // changes); the static helper here is a no-op extension that callers can use
    // without touching the runtime BeepDocumentPanel class. If the future runtime
    // adds a "Name vs DocumentTitle" guard, replace with the real call.
    internal static class BeepDocumentPanelDesignerExtensions
    {
        public static void SetDocumentTitleSafe(this BeepDocumentPanel panel)
        {
            // Reserved for future use — placeholder keeps the public surface
            // stable while Phase 02 settles. No-op today.
            _ = panel;
        }
    }
}
