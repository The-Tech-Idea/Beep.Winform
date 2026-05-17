// BeepDocumentHostDesigner.TabSelection.cs
// Phase 11 — Design-Time Tab Selection.
//
// The runtime BeepDocumentTabStrip is a non-sited child of BeepDocumentHost,
// so the WinForms designer normally swallows mouse clicks that land on it
// and treats them as "select the parent host" gestures. As a result a user
// could never click a tab at design time to switch the active document or
// route subsequent toolbox drops to a specific document panel — defeating
// the whole purpose of designing a multi-document MDI surface visually.
//
// Two pieces of plumbing live in this file:
//
//   • GetHitTest(Point) — overrides the parent designer hit-test to claim
//     pass-through ("let the runtime control receive this click") for any
//     screen coordinate that lands on a tab body, the add button, the
//     scroll buttons, the overflow button, or the group-header strip. All
//     other hits stay with the designer so dragging the host itself still
//     selects and moves it normally.
//
//   • EnsureActiveDocumentSelected() — a defensive fallback used by the
//     verb "Select Tab Under Cursor" and by the tab-strip mouse pre-filter
//     installed in Initialize(). If the WinForms designer host still
//     intercepts the click (some MDI Tools surfaces do), we hit-test the
//     cursor against every group's TabStrip in screen coordinates and
//     route SetActiveDocument(tabId) manually. The ActiveDocumentChanged
//     handler installed in BeepDocumentHostDesigner.cs (Phase 11 wire-up)
//     then promotes the corresponding BeepDocumentPanel to the primary
//     selection so the property grid swaps automatically.
//
// Together these two paths give the developer a click-to-select-tab UX
// identical to DevExpress XtraTabbedView and Telerik RadDocking.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepDocumentHostDesigner
    {
        // ── Designer hit-test ────────────────────────────────────────────────

        /// <summary>
        /// Phase 11 — claims pass-through for clicks that land on a tab body
        /// (or its associated buttons) so the runtime <see cref="BeepDocumentTabStrip"/>
        /// receives the message and raises <c>TabSelected</c>. The host's
        /// <c>OnTabSelected</c> then activates the matching document panel and
        /// fires <c>ActiveDocumentChanged</c>, which the designer forwards into
        /// the ISelectionService selection (see <c>OnHostActiveDocumentChanged</c>).
        /// </summary>
        /// <param name="point">
        /// Mouse position in <b>screen</b> coordinates, as documented for
        /// <see cref="ControlDesigner.GetHitTest(Point)"/>. Returning <c>true</c>
        /// means "this point belongs to the runtime control, not to me".
        /// </param>
        protected override bool GetHitTest(Point point)
        {
            if (Component is not BeepDocumentHost host)
            {
                return false;
            }

            if (TryHitTestTabStrip(host, point, out _, out _))
            {
                return true;
            }

            return base.GetHitTest(point);
        }

        // ── Public helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Phase 11 — Hit-tests the supplied screen point against every group's
        /// tab strip and, when it lands on a tab body, activates that document
        /// on the host. Used by the smart-tag verb "Select Tab Under Cursor"
        /// as a defensive fallback when the WinForms designer intercepts the
        /// click before it can reach the strip's own mouse pipeline.
        /// </summary>
        /// <returns>
        /// <c>true</c> when a tab was activated, <c>false</c> otherwise. The
        /// resulting ActiveDocumentChanged event drives selection promotion.
        /// </returns>
        internal bool ActivateTabAt(Point screenPoint)
        {
            if (Component is not BeepDocumentHost host)
            {
                return false;
            }

            if (!TryHitTestTabStrip(host, screenPoint, out string? tabId, out _))
            {
                return false;
            }

            if (string.IsNullOrEmpty(tabId))
            {
                return false;
            }

            try
            {
                return host.SetActiveDocument(tabId!);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Phase 11 — Promotes the panel under the supplied screen point to
        /// the primary designer selection. When no tab is under the cursor,
        /// falls back to selecting the currently active panel (or the host).
        /// </summary>
        internal void SelectDocumentAt(Point screenPoint)
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            if (ActivateTabAt(screenPoint))
            {
                // ActivateTabAt → host.SetActiveDocument → ActiveDocumentChanged
                // → OnHostActiveDocumentChanged will do the selection sync.
                return;
            }

            object selection = host.ActivePanel is { } activePanel && activePanel.Site != null
                ? (object)activePanel
                : host;
            SyncDesignerSelection(selection);
        }

        // ── Internal hit-test pipeline ───────────────────────────────────────

        /// <summary>
        /// Tests a screen point against every visible tab strip on the host.
        /// Returns the tab id and group when the point falls on a tab body,
        /// the add button, a scroll button, the overflow button, or any
        /// group-header rectangle. Out parameters are populated only on hit.
        /// </summary>
        private static bool TryHitTestTabStrip(
            BeepDocumentHost host,
            Point screenPoint,
            out string? tabId,
            out BeepDocumentGroup? group)
        {
            tabId = null;
            group = null;

            if (host == null)
            {
                return false;
            }

            foreach (BeepDocumentGroup grp in host.Groups)
            {
                if (grp?.TabStrip is not BeepDocumentTabStrip strip)
                {
                    continue;
                }

                if (!strip.IsHandleCreated)
                {
                    continue;
                }

                Point local;
                try
                {
                    local = strip.PointToClient(screenPoint);
                }
                catch
                {
                    continue;
                }

                if (!strip.ClientRectangle.Contains(local))
                {
                    continue;
                }

                // Any click on the tab strip area should be allowed through —
                // the strip itself routes the message to tab vs. button vs.
                // overflow / scroll using its existing OnMouseDown pipeline.
                // We still try to resolve the tab id so designer verbs can
                // act on a specific tab without re-running hit-test code.
                foreach (BeepDocumentTab tab in strip.Tabs)
                {
                    if (tab != null && tab.TabRect.Contains(local))
                    {
                        tabId = tab.Id;
                        group = grp;
                        return true;
                    }
                }

                // No specific tab hit, but the click is on the tab-strip
                // surface (add button, scroll, overflow, header). Still
                // return true so GetHitTest forwards the message to the
                // strip's own button/overflow handlers.
                group = grp;
                return true;
            }

            return false;
        }
    }
}
