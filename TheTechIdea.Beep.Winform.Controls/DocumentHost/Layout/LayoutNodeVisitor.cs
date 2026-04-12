// LayoutNodeVisitor.cs
// Concrete visitor helpers for the ILayoutNode tree.
// ─────────────────────────────────────────────────────────────────────────────
using System;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    /// <summary>
    /// Abstract base visitor that provides no-op implementations of both
    /// <see cref="ILayoutNodeVisitor"/> methods.  Subclasses override only
    /// the nodes they care about.
    /// </summary>
    public abstract class LayoutNodeVisitorBase : ILayoutNodeVisitor
    {
        /// <inheritdoc/>
        public virtual void Visit(SplitLayoutNode node) { }

        /// <inheritdoc/>
        public virtual void Visit(GroupLayoutNode node) { }
    }

    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Lightweight visitor built from two optional delegates — useful for quick
    /// one-off tree walks without creating a dedicated class.
    /// </summary>
    public sealed class DelegateLayoutNodeVisitor : ILayoutNodeVisitor
    {
        private readonly Action<SplitLayoutNode>? _onSplit;
        private readonly Action<GroupLayoutNode>? _onGroup;

        /// <param name="onSplit">Called for every <see cref="SplitLayoutNode"/>.  May be null.</param>
        /// <param name="onGroup">Called for every <see cref="GroupLayoutNode"/>.  May be null.</param>
        public DelegateLayoutNodeVisitor(
            Action<SplitLayoutNode>? onSplit = null,
            Action<GroupLayoutNode>? onGroup = null)
        {
            _onSplit = onSplit;
            _onGroup = onGroup;
        }

        /// <inheritdoc/>
        public void Visit(SplitLayoutNode node) => _onSplit?.Invoke(node);

        /// <inheritdoc/>
        public void Visit(GroupLayoutNode node) => _onGroup?.Invoke(node);
    }

    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Pre-order depth-first visitor that automatically recurses into
    /// <see cref="SplitLayoutNode"/> children after the node is visited.
    /// Subclasses override <see cref="Visit(SplitLayoutNode)"/> and/or
    /// <see cref="Visit(GroupLayoutNode)"/> — calling <c>base.Visit(...)</c>
    /// triggers the recursive descent.
    /// </summary>
    public abstract class RecursiveLayoutNodeVisitor : ILayoutNodeVisitor
    {
        /// <inheritdoc/>
        /// <remarks>Recursion into children happens after this method returns.</remarks>
        public virtual void Visit(SplitLayoutNode node)
        {
            node.First?.Accept(this);
            node.Second?.Accept(this);
        }

        /// <inheritdoc/>
        public virtual void Visit(GroupLayoutNode node) { }
    }
}
