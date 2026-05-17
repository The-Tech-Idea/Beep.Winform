// BeepDocumentManager.Presets.cs
// Phase 10 — IDisplayContainer parity polish.
//
// Matches the one-call styling ergonomic that BeepDisplayContainer.SetTabStylePreset(TabStyle)
// has exposed for years, so application code can swap between an IDE-style
// (VS Code) tab strip, a browser-style strip with a "+" button, or a
// minimal pill / underline look without poking at five properties by hand.
//
// IDisplayContainer doesn't mandate this surface — it's a quality-of-life
// extension already present on BeepDisplayContainer. Keeping the names
// matched lets the migration BeepDisplayContainer → BeepDocumentManager
// be a literal find-and-replace for that call site.
//
// Cross-references:
//   - BeepDisplayContainer.SetTabStylePreset (DisplayContainers folder)
//   - DocumentTabStyle enum (DocumentHost/BeepDocumentTab.cs)
//   - BeepDocumentHost.Properties.cs (TabStyle / ShowAddButton / CloseButtonMode)
//   - .plans/DocumentHost-MDI-Phase-10-DisplayContainerParity.md
// ─────────────────────────────────────────────────────────────────────────────
using System.ComponentModel;
using CloseMode = TheTechIdea.Beep.Winform.Controls.DocumentHost.BeepDocumentHost.CloseButtonShowMode;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public sealed partial class BeepDocumentManager
    {
        // ─────────────────────────────────────────────────────────────────
        // Presets
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies a one-shot styling preset to the underlying
        /// <see cref="BeepDocumentHost"/>. No-op when the manager is in
        /// Native MDI mode (the host does not exist).
        /// </summary>
        /// <param name="style">Target document-tab visual style.</param>
        /// <returns><c>true</c> when the preset was applied.</returns>
        public bool SetTabStylePreset(DocumentTabStyle style)
        {
            var host = GetTabbedHost();
            if (host == null) return false;

            // Each preset is a coordinated bundle so the host always reaches
            // a coherent appearance — switching from VSCode to Chrome
            // without bringing the add-button or close-button defaults
            // along would be a UX regression.
            switch (style)
            {
                case DocumentTabStyle.Chrome:
                    host.TabStyle           = DocumentTabStyle.Chrome;
                    host.TabPosition        = TabStripPosition.Top;
                    host.ShowAddButton      = true;
                    host.CloseButtonMode    = CloseMode.Always;
                    break;

                case DocumentTabStyle.VSCode:
                    host.TabStyle           = DocumentTabStyle.VSCode;
                    host.TabPosition        = TabStripPosition.Top;
                    host.ShowAddButton      = false;
                    host.CloseButtonMode    = CloseMode.OnHover;
                    break;

                case DocumentTabStyle.Underline:
                    host.TabStyle           = DocumentTabStyle.Underline;
                    host.TabPosition        = TabStripPosition.Top;
                    host.ShowAddButton      = false;
                    host.CloseButtonMode    = CloseMode.ActiveOnly;
                    break;

                case DocumentTabStyle.Pill:
                    host.TabStyle           = DocumentTabStyle.Pill;
                    host.TabPosition        = TabStripPosition.Top;
                    host.ShowAddButton      = false;
                    host.CloseButtonMode    = CloseMode.ActiveOnly;
                    break;

                case DocumentTabStyle.Flat:
                    host.TabStyle           = DocumentTabStyle.Flat;
                    host.TabPosition        = TabStripPosition.Top;
                    host.ShowAddButton      = false;
                    host.CloseButtonMode    = CloseMode.OnHover;
                    break;

                case DocumentTabStyle.Rounded:
                    host.TabStyle           = DocumentTabStyle.Rounded;
                    host.TabPosition        = TabStripPosition.Top;
                    host.ShowAddButton      = false;
                    host.CloseButtonMode    = CloseMode.ActiveOnly;
                    break;

                case DocumentTabStyle.Trapezoid:
                    host.TabStyle           = DocumentTabStyle.Trapezoid;
                    host.TabPosition        = TabStripPosition.Top;
                    host.ShowAddButton      = true;
                    host.CloseButtonMode    = CloseMode.Always;
                    break;

                default:
                    // Unknown style — just push the enum through and leave
                    // the bundle properties untouched so the host's own
                    // defaults win.
                    host.TabStyle = style;
                    break;
            }
            return true;
        }

        /// <summary>
        /// Convenience preset matching the Phase 07 wizard's
        /// <c>DocumentSetupMode.BrowserTabs</c> tile — Chrome tabs, "+" button,
        /// always-visible close buttons.
        /// </summary>
        public bool ApplyBrowserPreset() => SetTabStylePreset(DocumentTabStyle.Chrome);

        /// <summary>
        /// Convenience preset matching the Phase 07 wizard's
        /// <c>DocumentSetupMode.TabbedDocuments</c> tile — VS Code-style flat
        /// rectangular tabs, no "+" button, close-on-hover.
        /// </summary>
        public bool ApplyIdePreset() => SetTabStylePreset(DocumentTabStyle.VSCode);

        // ─────────────────────────────────────────────────────────────────
        // Designer-friendly hint property
        //
        // Mirrors the BeepDisplayContainer pattern where the developer can
        // pick a style in the Properties window and the underlying control
        // bundle changes in one click. Hidden from serialisation because
        // the underlying host already serialises TabStyle / ShowAddButton /
        // CloseButtonMode individually.
        // ─────────────────────────────────────────────────────────────────

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DocumentTabStyle CurrentTabStyle
        {
            get => GetTabbedHost()?.TabStyle ?? DocumentTabStyle.Chrome;
            set => SetTabStylePreset(value);
        }
    }
}
