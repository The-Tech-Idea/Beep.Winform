// ILayoutNode.cs
// Discriminated union interface for the hierarchical document-layout tree.
// Sprint 12 replaces the flat List<BeepDocumentGroup> with a tree of these nodes.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    /// <summary>
    /// Discriminator for nodes in the <see cref="ILayoutNode"/> tree.
    /// </summary>
    public enum LayoutNodeType
    {
        /// <summary>A leaf node containing a set of tabbed documents.</summary>
        Group,
        /// <summary>An intermediate node that splits its area into exactly two children.</summary>
        Split
    }

    /// <summary>
    /// Base interface for all nodes in the hierarchical document-layout tree.
    /// The tree is a binary tree: every internal node is a <see cref="SplitLayoutNode"/>
    /// and every leaf is a <see cref="GroupLayoutNode"/>.
    /// </summary>
    public interface ILayoutNode
    {
        /// <summary>Unique stable identifier for this node (GUID string).</summary>
        string NodeId { get; }

        /// <summary>Whether this node represents a split or a tab group.</summary>
        LayoutNodeType NodeType { get; }

        /// <summary>
        /// Absolute bounds within the host client area, assigned during the layout pass.
        /// Consumers should treat this as a cache — do not persist it.
        /// </summary>
        Rectangle Bounds { get; set; }

        /// <summary>
        /// Accepts a visitor, enabling double-dispatch operations (serialisation, layout, etc.).
        /// </summary>
        void Accept(ILayoutNodeVisitor visitor);
    }

    /// <summary>
    /// Visitor pattern for the layout tree.  Implement this interface to traverse
    /// the tree without casting.
    /// </summary>
    public interface ILayoutNodeVisitor
    {
        void Visit(SplitLayoutNode node);
        void Visit(GroupLayoutNode node);
    }
}
