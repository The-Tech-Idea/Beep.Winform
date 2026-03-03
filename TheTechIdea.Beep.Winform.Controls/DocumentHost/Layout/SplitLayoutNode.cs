// SplitLayoutNode.cs
// Intermediate layout-tree node representing a two-way split.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    /// <summary>
    /// A node in the layout tree that divides its <see cref="ILayoutNode.Bounds"/>
    /// into exactly two child nodes separated by a draggable splitter.
    /// </summary>
    public sealed class SplitLayoutNode : ILayoutNode
    {
        // ─────────────────────────────────────────────────────────────────────
        // ILayoutNode
        // ─────────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public string NodeId { get; }

        /// <inheritdoc/>
        public LayoutNodeType NodeType => LayoutNodeType.Split;

        /// <inheritdoc/>
        public Rectangle Bounds { get; set; }

        // ─────────────────────────────────────────────────────────────────────
        // Split state
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// <c>Horizontal</c> means children are side-by-side (left | right).
        /// <c>Vertical</c> means children are stacked (top / bottom).
        /// </summary>
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        /// <summary>
        /// Split ratio for the FIRST child (0.0–1.0).
        /// The second child receives the remaining 1−Ratio share.
        /// </summary>
        public float Ratio
        {
            get => _ratio;
            set => _ratio = Math.Max(0.1f, Math.Min(0.9f, value));
        }
        private float _ratio = 0.5f;

        // ─────────────────────────────────────────────────────────────────────
        // Children
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>First child (left/top depending on orientation).</summary>
        public ILayoutNode First  { get; set; }

        /// <summary>Second child (right/bottom depending on orientation).</summary>
        public ILayoutNode Second { get; set; }

        // ─────────────────────────────────────────────────────────────────────
        // Construction
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Creates a new split node with the given children.</summary>
        public SplitLayoutNode(ILayoutNode first, ILayoutNode second,
                               Orientation orientation = Orientation.Horizontal,
                               float ratio = 0.5f)
        {
            NodeId      = Guid.NewGuid().ToString();
            First       = first  ?? throw new ArgumentNullException(nameof(first));
            Second      = second ?? throw new ArgumentNullException(nameof(second));
            Orientation = orientation;
            Ratio       = ratio;
        }

        /// <summary>Deserialization constructor — call only from layout migration/restore.</summary>
        internal SplitLayoutNode(string nodeId, ILayoutNode first, ILayoutNode second,
                                  Orientation orientation, float ratio)
        {
            NodeId      = nodeId ?? Guid.NewGuid().ToString();
            First       = first  ?? throw new ArgumentNullException(nameof(first));
            Second      = second ?? throw new ArgumentNullException(nameof(second));
            Orientation = orientation;
            Ratio       = ratio;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Visitor
        // ─────────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void Accept(ILayoutNodeVisitor visitor) => visitor.Visit(this);

        // ─────────────────────────────────────────────────────────────────────
        // Layout helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Divides <see cref="Bounds"/> into rectangles for <see cref="First"/> and
        /// <see cref="Second"/> based on <see cref="Orientation"/> and
        /// <see cref="Ratio"/>, accounting for the splitter bar thickness.
        /// </summary>
        public (Rectangle FirstBounds, Rectangle SplitterBounds, Rectangle SecondBounds)
            ComputeChildBounds(int splitterThickness)
        {
            if (Orientation == Orientation.Horizontal)
            {
                int firstW     = (int)(Bounds.Width * Ratio);
                int splitterX  = Bounds.X + firstW;
                return (
                    new Rectangle(Bounds.X,    Bounds.Y, firstW,                 Bounds.Height),
                    new Rectangle(splitterX,   Bounds.Y, splitterThickness,      Bounds.Height),
                    new Rectangle(splitterX + splitterThickness, Bounds.Y,
                                  Bounds.Width - firstW - splitterThickness,     Bounds.Height)
                );
            }
            else // Vertical — stacked top/bottom
            {
                int firstH     = (int)(Bounds.Height * Ratio);
                int splitterY  = Bounds.Y + firstH;
                return (
                    new Rectangle(Bounds.X, Bounds.Y,    Bounds.Width, firstH),
                    new Rectangle(Bounds.X, splitterY,   Bounds.Width, splitterThickness),
                    new Rectangle(Bounds.X, splitterY + splitterThickness,
                                  Bounds.Width, Bounds.Height - firstH - splitterThickness)
                );
            }
        }
    }
}
