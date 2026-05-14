// BeepDocumentHost.DockRails.cs
// Partial class: dock-rail registration and layout integration.
//
// BeepDocumentHost coordinates with BeepDockManager via two entry points:
//   • RegisterDockRail(edge, rail)   — called by BeepDockManager when a rail
//     is first needed for an edge.
//   • UnregisterDockRail(edge)       — called by BeepDockManager on dispose or
//     Host reassignment.
//
// Layout integration — called from RecalculateLayout() in Layout.cs:
//   • ShrinkForDockRails(fullBounds)  → Rectangle    (shrink the document area)
//   • PositionDockRails(fullBounds)                  (position each rail in the margin)
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────────

        private readonly Dictionary<DockEdge, BeepDockRail> _dockRails =
            new();

        // ─────────────────────────────────────────────────────────────────────
        // Registration API (called by BeepDockManager)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called by <see cref="BeepDockManager"/> to attach a rail to this host.
        /// The rail is added to the host's Controls and participates in layout.
        /// </summary>
        internal void RegisterDockRail(DockEdge edge, BeepDockRail rail)
        {
            if (_dockRails.TryGetValue(edge, out var existing) && ReferenceEquals(existing, rail))
                return;

            // Remove any previous rail on this edge
            UnregisterDockRail(edge);

            _dockRails[edge] = rail;
            rail.ApplyTheme(_currentTheme);
            Controls.Add(rail);
            rail.BringToFront();

            RecalculateLayout();
        }

        /// <summary>
        /// Called by <see cref="BeepDockManager"/> when a rail is removed from this host.
        /// </summary>
        internal void UnregisterDockRail(DockEdge edge)
        {
            if (!_dockRails.TryGetValue(edge, out var rail)) return;

            Controls.Remove(rail);
            _dockRails.Remove(edge);
            RecalculateLayout();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Layout helpers — called from RecalculateLayout() in Layout.cs
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns <paramref name="fullBounds"/> shrunk by the pixel extent of all
        /// registered pinned rails so the document content area does not overlap them.
        /// </summary>
        internal Rectangle ShrinkForDockRails(Rectangle fullBounds)
        {
            if (_dockRails.Count == 0) return fullBounds;

            var r = fullBounds;
            foreach (var (edge, rail) in _dockRails)
            {
                if (!rail.Visible) continue;
                int sz = rail.RailPixelSize;
                if (sz <= 0) continue;

                switch (edge)
                {
                    case DockEdge.Left:
                        r = new Rectangle(r.X + sz, r.Y, r.Width - sz, r.Height);
                        break;
                    case DockEdge.Right:
                        r = new Rectangle(r.X, r.Y, r.Width - sz, r.Height);
                        break;
                    case DockEdge.Top:
                        r = new Rectangle(r.X, r.Y + sz, r.Width, r.Height - sz);
                        break;
                    case DockEdge.Bottom:
                        r = new Rectangle(r.X, r.Y, r.Width, r.Height - sz);
                        break;
                }
            }
            return r;
        }

        /// <summary>
        /// Explicitly positions each registered rail in the margin between
        /// <paramref name="fullBounds"/> and the (already-shrunk) content area.
        /// Idempotent — safe to call every layout cycle.
        /// </summary>
        internal void PositionDockRails(Rectangle fullBounds)
        {
            if (_dockRails.Count == 0) return;

            // We need to compute each rail's position from scratch.
            // Rails are stacked outward from the content area; they do not nest.
            // For simplicity, each rail owns its computed margin independently
            // (multiple rails on the same edge are not supported in V1 — one per edge).

            // Compute cumulative insets as we position each rail.
            int insetLeft = 0, insetRight = 0, insetTop = 0, insetBottom = 0;

            // Left
            if (_dockRails.TryGetValue(DockEdge.Left, out var leftRail) && leftRail.Visible)
            {
                int sz = leftRail.RailPixelSize;
                leftRail.SetBounds(
                    fullBounds.Left,
                    fullBounds.Top,
                    sz,
                    fullBounds.Height);
                leftRail.BringToFront();
                insetLeft = sz;
            }

            // Right
            if (_dockRails.TryGetValue(DockEdge.Right, out var rightRail) && rightRail.Visible)
            {
                int sz = rightRail.RailPixelSize;
                rightRail.SetBounds(
                    fullBounds.Right - sz,
                    fullBounds.Top,
                    sz,
                    fullBounds.Height);
                rightRail.BringToFront();
                insetRight = sz;
            }

            // Top (uses left/right insets to avoid corners)
            if (_dockRails.TryGetValue(DockEdge.Top, out var topRail) && topRail.Visible)
            {
                int sz = topRail.RailPixelSize;
                topRail.SetBounds(
                    fullBounds.Left  + insetLeft,
                    fullBounds.Top,
                    fullBounds.Width - insetLeft - insetRight,
                    sz);
                topRail.BringToFront();
                insetTop = sz;
            }

            // Bottom (uses left/right insets to avoid corners)
            if (_dockRails.TryGetValue(DockEdge.Bottom, out var bottomRail) && bottomRail.Visible)
            {
                int sz = bottomRail.RailPixelSize;
                bottomRail.SetBounds(
                    fullBounds.Left  + insetLeft,
                    fullBounds.Bottom - sz,
                    fullBounds.Width - insetLeft - insetRight,
                    sz);
                bottomRail.BringToFront();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Theming — called from ApplyTheme in main BeepDocumentHost.cs
        // ─────────────────────────────────────────────────────────────────────

        private void ApplyThemeToDockRails()
        {
            foreach (var rail in _dockRails.Values)
                rail.ApplyTheme(_currentTheme);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Dispose hook — called from BeepDocumentHost.Dispose
        // ─────────────────────────────────────────────────────────────────────

        private void DisposeDockRails()
        {
            foreach (var edge in _dockRails.Keys.ToList())
                UnregisterDockRail(edge);
        }
    }
}
