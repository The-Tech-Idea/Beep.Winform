using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.DragDrop
{
    /// <summary>
    /// What a drop at the current cursor location would do.
    /// </summary>
    internal enum DockDropKind
    {
        /// <summary>No valid target — the panel would float.</summary>
        Float,

        /// <summary>Dock against an edge of the whole dock-site (host form).</summary>
        DockSiteEdge,

        /// <summary>Split an existing group along one of its edges (Phase 03 engine).</summary>
        GroupEdge,

        /// <summary>Stack into an existing group as a tab at <see cref="DockDropResult.InsertIndex"/>.</summary>
        GroupCenterStack
    }

    /// <summary>
    /// Resolved drop target for an in-flight drag. Produced by <c>DockTargetResolver</c> on every
    /// mouse-move and consumed by <c>DockDragController</c> on commit. Immutable snapshot — a new
    /// instance is produced each move.
    /// </summary>
    internal sealed class DockDropResult
    {
        /// <summary>The kind of drop the cursor currently resolves to.</summary>
        public DockDropKind Kind { get; init; } = DockDropKind.Float;

        /// <summary>Edge/position for <see cref="DockDropKind.DockSiteEdge"/> or <see cref="DockDropKind.GroupEdge"/>.</summary>
        public DockPosition Position { get; init; } = DockPosition.Floating;

        /// <summary>Target group for group-edge / center-stack drops; null for site-edge or float.</summary>
        public DockGroup TargetGroup { get; init; }

        /// <summary>Tab insert index for <see cref="DockDropKind.GroupCenterStack"/> (-1 = append).</summary>
        public int InsertIndex { get; init; } = -1;

        /// <summary>Screen-space preview rectangle the ghost should snap to (empty = follow cursor).</summary>
        public Rectangle PreviewBounds { get; init; }

        /// <summary>True when this drop docks the panel somewhere (i.e. not a plain float).</summary>
        public bool IsDock => Kind != DockDropKind.Float;

        /// <summary>A float result with an optional follow-the-cursor preview rectangle.</summary>
        public static DockDropResult Float(Rectangle previewBounds) => new DockDropResult
        {
            Kind = DockDropKind.Float,
            Position = DockPosition.Floating,
            PreviewBounds = previewBounds
        };
    }
}
