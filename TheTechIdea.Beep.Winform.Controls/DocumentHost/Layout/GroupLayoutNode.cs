// GroupLayoutNode.cs
// Leaf node in the layout tree — owns a set of tabbed documents in a single pane.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    /// <summary>
    /// A leaf node in the layout tree that maps 1:1 to a <see cref="BeepDocumentGroup"/>.
    /// Contains an ordered collection of document IDs and tracks which document is active.
    /// </summary>
    public sealed class GroupLayoutNode : ILayoutNode
    {
        // ─────────────────────────────────────────────────────────────────────
        // ILayoutNode
        // ─────────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public string NodeId { get; }

        /// <inheritdoc/>
        public LayoutNodeType NodeType => LayoutNodeType.Group;

        /// <inheritdoc/>
        public Rectangle Bounds { get; set; }

        // ─────────────────────────────────────────────────────────────────────
        // Group identity
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// References the <see cref="BeepDocumentGroup.GroupId"/> that this
        /// node represents in the live control hierarchy.
        /// Equal to <see cref="NodeId"/> for a new group; may differ after serialisation
        /// restore if the runtime assigned a fresh GUID.
        /// </summary>
        public string GroupId { get; set; }

        // ─────────────────────────────────────────────────────────────────────
        // Documents
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Ordered list of document IDs that belong to this group.
        /// Matches the visual tab order.
        /// </summary>
        public List<string> DocumentIds { get; } = new List<string>();

        /// <summary>
        /// ID of the document that is currently active (selected) in this group.
        /// Null if the group is empty.
        /// </summary>
        public string? SelectedDocumentId { get; set; }

        /// <summary><c>true</c> when <see cref="DocumentIds"/> is empty.</summary>
        public bool IsEmpty => DocumentIds.Count == 0;

        // ─────────────────────────────────────────────────────────────────────
        // Construction
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Creates a new group node with a fresh GUID as its node and group ID.</summary>
        public GroupLayoutNode()
        {
            NodeId  = Guid.NewGuid().ToString();
            GroupId = NodeId;
        }

        /// <summary>Creates a group node that corresponds to an existing runtime group.</summary>
        public GroupLayoutNode(string groupId)
        {
            NodeId  = groupId ?? Guid.NewGuid().ToString();
            GroupId = NodeId;
        }

        /// <summary>
        /// Deserialization constructor — preserves IDs from a saved layout snapshot.
        /// </summary>
        internal GroupLayoutNode(string nodeId, string groupId,
                                  List<string>? documentIds = null,
                                  string? selectedDocumentId = null)
        {
            NodeId               = nodeId  ?? Guid.NewGuid().ToString();
            GroupId              = groupId ?? NodeId;
            SelectedDocumentId   = selectedDocumentId;
            if (documentIds != null)
                DocumentIds.AddRange(documentIds);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Visitor
        // ─────────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void Accept(ILayoutNodeVisitor visitor) => visitor.Visit(this);
    }
}
