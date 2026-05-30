using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.DragDrop
{
    /// <summary>
    /// Commit surface the <see cref="DockDragController"/> uses to apply a drop. Implemented by
    /// <c>BeepDockingManager</c> (in its <c>DragDrop</c> partial) so the controller stays free of
    /// layout-tree and window plumbing. Each commit method may be vetoed (return false) by the
    /// host's cancelable docking events.
    /// </summary>
    internal interface IDockDragHost
    {
        /// <summary>The dock-site form guides/ghost are shown over.</summary>
        Form HostForm { get; }

        /// <summary>Resolved docking palette for the ghost.</summary>
        DockingThemeColors DockingColors { get; }

        /// <summary>Current group bounds (in host-form client coords) used for group-edge detection.</summary>
        IReadOnlyDictionary<string, Rectangle> GroupBounds { get; }

        /// <summary>Looks up a DockGroup by its string id for group-edge resolution.</summary>
        DockGroup GetGroup(string groupId);

        /// <summary>Floats the panel (tear-out). Returns false if vetoed.</summary>
        bool CommitFloat(DockPanel panel);

        /// <summary>Docks the panel against a dock-site edge. Returns false if vetoed.</summary>
        bool CommitDockSiteEdge(DockPanel panel, DockPosition position);

        /// <summary>Stacks the panel as a tab in the center/document group. Returns false if vetoed.</summary>
        bool CommitCenterStack(DockPanel panel, DockGroup targetGroup, int insertIndex);

        /// <summary>Splits an existing group along one edge. Returns false if vetoed.</summary>
        bool CommitGroupEdge(DockPanel panel, DockGroup targetGroup, DockPosition position);
    }
}
