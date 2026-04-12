// LayoutNodeExtensions.cs
// Extension methods for querying and traversing ILayoutNode trees.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    /// <summary>
    /// Extension methods that add tree-traversal and query capabilities to
    /// any <see cref="ILayoutNode"/>.
    /// </summary>
    public static class LayoutNodeExtensions
    {
        // ─────────────────────────────────────────────────────────────────────
        // Traversal
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns every node in the subtree rooted at <paramref name="node"/>
        /// in pre-order (node, then children left-to-right).
        /// </summary>
        public static IEnumerable<ILayoutNode> AllNodes(this ILayoutNode node)
        {
            ArgumentNullException.ThrowIfNull(node);
            var stack = new Stack<ILayoutNode>();
            stack.Push(node);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                if (current is SplitLayoutNode split)
                {
                    // Push second first so first is processed first
                    if (split.Second != null) stack.Push(split.Second);
                    if (split.First  != null) stack.Push(split.First);
                }
            }
        }

        /// <summary>
        /// Returns all <see cref="GroupLayoutNode"/> leaf nodes in the subtree
        /// (left-to-right order).
        /// </summary>
        public static IEnumerable<GroupLayoutNode> AllLeaves(this ILayoutNode node)
        {
            ArgumentNullException.ThrowIfNull(node);
            foreach (var n in node.AllNodes())
                if (n is GroupLayoutNode g)
                    yield return g;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Queries
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Returns the number of <see cref="GroupLayoutNode"/> leaves.</summary>
        public static int LeafCount(this ILayoutNode node)
        {
            ArgumentNullException.ThrowIfNull(node);
            int count = 0;
            foreach (var _ in node.AllLeaves()) count++;
            return count;
        }

        /// <summary>Returns the maximum depth of the subtree (a single leaf = depth 1).</summary>
        public static int Depth(this ILayoutNode node)
        {
            ArgumentNullException.ThrowIfNull(node);
            return node is SplitLayoutNode split
                ? 1 + Math.Max(
                    split.First  != null ? split.First.Depth()  : 0,
                    split.Second != null ? split.Second.Depth() : 0)
                : 1;
        }

        /// <summary>
        /// Finds the first node with <paramref name="nodeId"/> in the subtree,
        /// or <c>null</c> if not found.
        /// </summary>
        public static ILayoutNode? FindNode(this ILayoutNode root, string nodeId)
        {
            ArgumentNullException.ThrowIfNull(root);
            if (string.IsNullOrEmpty(nodeId)) return null;
            foreach (var n in root.AllNodes())
                if (n.NodeId == nodeId) return n;
            return null;
        }

        /// <summary>
        /// Finds the parent <see cref="SplitLayoutNode"/> of <paramref name="target"/>
        /// in the subtree rooted at <paramref name="root"/>, or <c>null</c> if
        /// <paramref name="target"/> is the root or is not found.
        /// </summary>
        public static SplitLayoutNode? FindParent(this ILayoutNode root, ILayoutNode target)
        {
            ArgumentNullException.ThrowIfNull(root);
            ArgumentNullException.ThrowIfNull(target);
            foreach (var n in root.AllNodes())
            {
                if (n is SplitLayoutNode split
                    && (ReferenceEquals(split.First, target)
                     || ReferenceEquals(split.Second, target)))
                    return split;
            }
            return null;
        }

        /// <summary>
        /// Returns <c>true</c> if the subtree contains a node with the given
        /// <paramref name="nodeId"/>.
        /// </summary>
        public static bool ContainsNode(this ILayoutNode root, string nodeId)
            => root.FindNode(nodeId) != null;

        /// <summary>
        /// Returns <c>true</c> if any leaf in the subtree contains
        /// <paramref name="documentId"/> in its <see cref="GroupLayoutNode.DocumentIds"/>.
        /// </summary>
        public static bool ContainsDocument(this ILayoutNode root, string documentId)
        {
            ArgumentNullException.ThrowIfNull(root);
            if (string.IsNullOrEmpty(documentId)) return false;
            foreach (var leaf in root.AllLeaves())
                if (leaf.DocumentIds.Contains(documentId)) return true;
            return false;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Structural helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns <c>true</c> when the node is a <see cref="GroupLayoutNode"/>
        /// with no documents.
        /// </summary>
        public static bool IsEmptyLeaf(this ILayoutNode node)
            => node is GroupLayoutNode g && g.IsEmpty;

        /// <summary>
        /// Performs a quick structural sanity check on the subtree:
        /// a <see cref="SplitLayoutNode"/> must have non-null <c>First</c> and <c>Second</c>.
        /// Does not validate document uniqueness.
        /// </summary>
        public static bool IsStructurallyValid(this ILayoutNode node)
        {
            ArgumentNullException.ThrowIfNull(node);
            foreach (var n in node.AllNodes())
            {
                if (n is SplitLayoutNode split
                    && (split.First == null || split.Second == null))
                    return false;
            }
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Cloning
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a deep clone of the subtree.  <see cref="ILayoutNode.Bounds"/>
        /// values are copied; document IDs are copied by value.
        /// </summary>
        public static ILayoutNode DeepClone(this ILayoutNode node)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (node is GroupLayoutNode g)
            {
                var clone = new GroupLayoutNode(g.NodeId, g.GroupId,
                    new System.Collections.Generic.List<string>(g.DocumentIds),
                    g.SelectedDocumentId)
                {
                    Bounds = g.Bounds
                };
                return clone;
            }

            if (node is SplitLayoutNode s)
            {
                var clone = new SplitLayoutNode(
                    s.First.DeepClone(),
                    s.Second.DeepClone(),
                    s.Orientation,
                    s.Ratio)
                {
                    Bounds = s.Bounds
                };
                return clone;
            }

            throw new NotSupportedException($"Unknown node type: {node.GetType().Name}");
        }
    }
}
