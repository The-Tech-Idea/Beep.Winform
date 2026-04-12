// BeepDocumentDockConstraints.cs
// Declarative dock constraints for BeepDocumentHost.
// Set via BeepDocumentHost.DockConstraints to restrict docking operations.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Defines size and zone restrictions for docking operations inside a
    /// <see cref="BeepDocumentHost"/>.
    /// </summary>
    public sealed class DockConstraints
    {
        // ── Pane size limits ──────────────────────────────────────────────────

        /// <summary>Minimum width (logical pixels) of any split pane.  Default 80.</summary>
        public int MinPaneWidth  { get; set; } = 80;

        /// <summary>Minimum height (logical pixels) of any split pane.  Default 60.</summary>
        public int MinPaneHeight { get; set; } = 60;

        /// <summary>
        /// Maximum width (logical pixels) of any split pane.
        /// Use <see cref="int.MaxValue"/> (default) to impose no limit.
        /// </summary>
        public int MaxPaneWidth  { get; set; } = int.MaxValue;

        /// <summary>
        /// Maximum height (logical pixels) of any split pane.
        /// Use <see cref="int.MaxValue"/> (default) to impose no limit.
        /// </summary>
        public int MaxPaneHeight { get; set; } = int.MaxValue;

        // ── Zone restrictions ─────────────────────────────────────────────────

        /// <summary>
        /// Set of dock zones that are disabled for this host.
        /// Drop actions targeting a disabled zone are silently ignored.
        /// </summary>
        public HashSet<DockZone> DisabledZones { get; } = new HashSet<DockZone>();

        // ── Cross-host option ─────────────────────────────────────────────────

        /// <summary>
        /// When false, this host refuses to accept documents dragged in from other hosts
        /// even if <see cref="BeepDocumentHost.AllowDragBetweenHosts"/> is true.
        /// Default is true.
        /// </summary>
        public bool AllowIncomingTransfer { get; set; } = true;

        // ── Convenience factory methods ───────────────────────────────────────

        /// <summary>Returns a <see cref="DockConstraints"/> with no restrictions.</summary>
        public static DockConstraints Unrestricted() => new DockConstraints();

        /// <summary>
        /// Returns a <see cref="DockConstraints"/> that disables all directional split zones,
        /// allowing only Centre (tab merge) docking.
        /// </summary>
        public static DockConstraints TabMergeOnly()
        {
            var c = new DockConstraints();
            c.DisabledZones.Add(DockZone.Left);
            c.DisabledZones.Add(DockZone.Right);
            c.DisabledZones.Add(DockZone.Top);
            c.DisabledZones.Add(DockZone.Bottom);
            return c;
        }

        /// <summary>
        /// Returns a <see cref="DockConstraints"/> that disables all docking (read-only host).
        /// </summary>
        public static DockConstraints ReadOnly()
        {
            var c = new DockConstraints();
            c.DisabledZones.Add(DockZone.Centre);
            c.DisabledZones.Add(DockZone.Left);
            c.DisabledZones.Add(DockZone.Right);
            c.DisabledZones.Add(DockZone.Top);
            c.DisabledZones.Add(DockZone.Bottom);
            c.AllowIncomingTransfer = false;
            return c;
        }

        /// <summary>Returns true if the given zone is permitted by these constraints.</summary>
        public bool IsZoneAllowed(DockZone zone)
            => zone == DockZone.None || !DisabledZones.Contains(zone);
    }
}
