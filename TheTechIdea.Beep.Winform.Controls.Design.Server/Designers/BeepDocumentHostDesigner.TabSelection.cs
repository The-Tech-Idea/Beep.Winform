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
// This file keeps the shared document-selection path used by the host
// designer's tab-strip mouse routing and by the "Select Tab Under Cursor"
// verb. It hit-tests against every group's TabStrip in screen coordinates
// and routes SetActiveDocument manually when needed.
//
// Header clicks are handled by the runtime BeepDocumentTabStrip. The host
// designer only uses GetHitTest to let those internal implementation controls
// receive mouse input, following the same "host owns panes" shape used by
// DockPanelSuite.
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

            return TryActivateDocumentTabAt(host, screenPoint);
        }

        internal bool SelectDocumentTabFromStrip(BeepDocumentTabStrip strip, Point stripClientPoint)
        {
            if (strip == null || Component is not BeepDocumentHost host)
            {
                return false;
            }

            try
            {
                strip.CalculateTabLayout();
            }
            catch
            {
                // Stale design-time layout should not block tab selection.
            }

            if (!TryResolveTabIdFromStripPoint(strip, stripClientPoint, out string? tabId)
                || string.IsNullOrWhiteSpace(tabId))
            {
                return false;
            }

            return SelectDocumentTabById(host, strip, tabId);
        }

        private bool TryActivateDocumentTabAt(BeepDocumentHost host, Point screenPoint)
        {
            if (!TryHitTestTabStrip(host, screenPoint, out string? tabId, out BeepDocumentGroup? group))
            {
                return false;
            }

            if (string.IsNullOrEmpty(tabId))
            {
                return false;
            }

            try
            {
                return SelectDocumentTabById(host, group?.TabStrip, tabId!);
            }
            catch
            {
                return false;
            }
        }

        private bool SelectDocumentTabById(BeepDocumentHost host, BeepDocumentTabStrip? strip, string tabId)
        {
            bool activated = host.SetActiveDocument(tabId);
            strip?.ActivateTabById(tabId);

            BeepDocumentPanel? panel = host.GetPanel(tabId);
            Control selectionOwner = strip != null ? strip : host;
            SyncDesignerSelectionSticky(selectionOwner, ResolveDocumentSelection(host, panel ?? host.ActivePanel));
            return activated || panel != null;
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

            if (TryHitTestDocumentPanel(host, screenPoint, out BeepDocumentPanel? panel) && panel != null)
            {
                try
                {
                    IContainer? container = GetNestedContainer() ?? GetDesignerHost()?.Container;
                    if (container != null)
                    {
                        SiteDesignPanel(container, panel, panel.DocumentId);
                    }
                }
                catch
                {
                    // Non-fatal design-time siting path.
                }

                SyncDesignerSelectionSticky(host, ResolveDocumentSelection(host, panel));
                return;
            }

            SyncDesignerSelectionSticky(host, ResolveDocumentSelection(host, host.ActivePanel));
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

                try
                {
                    strip.CalculateTabLayout();
                }
                catch
                {
                    // Stale design-time layout should not block tab selection.
                }

                if (!TryMapScreenToStripLocal(host, strip, screenPoint, out Point local))
                {
                    continue;
                }

                if (!strip.ClientRectangle.Contains(local))
                {
                    continue;
                }

                if (TryResolveTabIdFromStripPoint(strip, local, out string? resolvedTabId))
                {
                    tabId = resolvedTabId;
                    group = grp;
                    return true;
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

        internal static bool TryResolveTabIdFromStripPoint(BeepDocumentTabStrip strip, Point local, out string? tabId)
        {
            tabId = null;

            try
            {
                var hit = strip.HitTestTab(local);
                if (hit.Hit
                    && hit.TabIndex >= 0
                    && hit.TabIndex < strip.Tabs.Count
                    && !hit.IsCloseButton
                    && !hit.IsAddButton
                    && !hit.IsScrollLeft
                    && !hit.IsScrollRight
                    && !hit.IsOverflowButton)
                {
                    tabId = strip.Tabs[hit.TabIndex].Id;
                    return !string.IsNullOrWhiteSpace(tabId);
                }
            }
            catch
            {
                // Fall through to geometry-based check.
            }

            foreach (BeepDocumentTab tab in strip.Tabs)
            {
                if (tab == null || !tab.TabRect.Contains(local))
                {
                    continue;
                }

                if (!tab.CloseRect.IsEmpty && tab.CloseRect.Contains(local))
                {
                    continue;
                }

                tabId = tab.Id;
                return !string.IsNullOrWhiteSpace(tabId);
            }

            return false;
        }

        private static bool TryMapScreenToStripLocal(BeepDocumentHost host, BeepDocumentTabStrip strip, Point screenPoint, out Point local)
        {
            local = Point.Empty;

            try
            {
                if (strip.IsHandleCreated)
                {
                    local = strip.PointToClient(screenPoint);
                    return true;
                }

                if (!host.IsHandleCreated)
                {
                    return false;
                }

                Point hostLocal = host.PointToClient(screenPoint);
                Point offset = GetOffsetFromHost(strip, host);
                local = new Point(hostLocal.X - offset.X, hostLocal.Y - offset.Y);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static Point GetOffsetFromHost(Control control, Control host)
        {
            int x = 0;
            int y = 0;

            Control? current = control;
            while (current != null && current != host)
            {
                x += current.Left;
                y += current.Top;
                current = current.Parent;
            }

            return new Point(x, y);
        }

        private static bool TryHitTestDocumentPanel(BeepDocumentHost host, Point screenPoint, out BeepDocumentPanel? hitPanel)
        {
            hitPanel = null;

            foreach (BeepDocumentGroup group in host.Groups)
            {
                foreach (string documentId in group.DocumentIds)
                {
                    BeepDocumentPanel? panel = host.GetPanel(documentId);
                    if (panel == null || !panel.Visible)
                    {
                        continue;
                    }

                    Point panelPoint;
                    try
                    {
                        panelPoint = panel.PointToClient(screenPoint);
                    }
                    catch
                    {
                        continue;
                    }

                    if (!panel.ClientRectangle.Contains(panelPoint))
                    {
                        continue;
                    }

                    if (ContainsChildControlAt(panel, screenPoint))
                    {
                        continue;
                    }

                    hitPanel = panel;
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsChildControlAt(Control parent, Point screenPoint)
        {
            foreach (Control child in parent.Controls)
            {
                if (!child.Visible)
                {
                    continue;
                }

                Point childPoint;
                try
                {
                    childPoint = child.PointToClient(screenPoint);
                }
                catch
                {
                    continue;
                }

                if (!child.ClientRectangle.Contains(childPoint))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private object ResolveDocumentSelection(BeepDocumentHost host, BeepDocumentPanel? panel)
        {
            if (panel == null)
            {
                return host;
            }

            try
            {
                IContainer? container = GetNestedContainer() ?? GetDesignerHost()?.Container;
                if (container != null)
                {
                    SiteDesignPanel(container, panel, panel.DocumentId);
                }
            }
            catch
            {
                // Non-fatal design-time siting path.
            }

            return panel.Site != null ? (object)panel : host;
        }

        private void SyncDesignerSelectionSticky(Control owner, object selectionTarget)
        {
            SyncDesignerSelection(selectionTarget);

            try
            {
                if (owner.IsHandleCreated)
                {
                    owner.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            SyncDesignerSelection(selectionTarget);
                        }
                        catch
                        {
                            // Designer host may be tearing down.
                        }
                    }));
                }
            }
            catch
            {
                // Ignore transient designer host disposal/race conditions.
            }
        }
    }
}
