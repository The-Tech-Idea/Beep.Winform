using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Editors
{
    /// <summary>
    /// Provides data for custom draw events in BeepTree.
    /// Set Handled = true to prevent default drawing.
    /// Call DefaultDraw() to perform default drawing within your custom handler.
    /// </summary>
    public abstract class BeepTreeCustomDrawEventArgs : EventArgs
    {
        /// <summary>The Graphics object to draw on.</summary>
        public Graphics Graphics { get; }

        /// <summary>The bounds of the area being drawn.</summary>
        public Rectangle Bounds { get; }

        /// <summary>Set to true to indicate that the event was handled and default drawing should be skipped.</summary>
        public bool Handled { get; set; }

        protected BeepTreeCustomDrawEventArgs(Graphics graphics, Rectangle bounds)
        {
            Graphics = graphics;
            Bounds = bounds;
        }

        /// <summary>
        /// Performs the default drawing operation.
        /// </summary>
        public abstract void DefaultDraw();
    }

    /// <summary>
    /// Provides data for the CustomDrawNode event.
    /// </summary>
    public class BeepTreeCustomDrawNodeEventArgs : BeepTreeCustomDrawEventArgs
    {
        /// <summary>The node being drawn.</summary>
        public SimpleItem Node { get; }

        /// <summary>The node's layout information.</summary>
        public NodeInfo NodeInfo { get; }

        /// <summary>Whether the node is selected.</summary>
        public bool IsSelected { get; }

        /// <summary>Whether the node is hovered.</summary>
        public bool IsHovered { get; }

        /// <summary>The default draw action.</summary>
        private readonly Action _defaultDrawAction;

        public BeepTreeCustomDrawNodeEventArgs(Graphics graphics, Rectangle bounds, SimpleItem node, NodeInfo nodeInfo, bool isSelected, bool isHovered, Action defaultDrawAction)
            : base(graphics, bounds)
        {
            Node = node;
            NodeInfo = nodeInfo;
            IsSelected = isSelected;
            IsHovered = isHovered;
            _defaultDrawAction = defaultDrawAction;
        }

        public override void DefaultDraw()
        {
            _defaultDrawAction?.Invoke();
        }
    }

    /// <summary>
    /// Provides data for the CustomDrawNodeBackground event.
    /// </summary>
    public class BeepTreeCustomDrawNodeBackgroundEventArgs : BeepTreeCustomDrawEventArgs
    {
        /// <summary>The node being drawn.</summary>
        public SimpleItem Node { get; }

        /// <summary>Whether the node is selected.</summary>
        public bool IsSelected { get; }

        /// <summary>Whether the node is hovered.</summary>
        public bool IsHovered { get; }

        /// <summary>The default draw action.</summary>
        private readonly Action _defaultDrawAction;

        public BeepTreeCustomDrawNodeBackgroundEventArgs(Graphics graphics, Rectangle bounds, SimpleItem node, bool isSelected, bool isHovered, Action defaultDrawAction)
            : base(graphics, bounds)
        {
            Node = node;
            IsSelected = isSelected;
            IsHovered = isHovered;
            _defaultDrawAction = defaultDrawAction;
        }

        public override void DefaultDraw()
        {
            _defaultDrawAction?.Invoke();
        }
    }

    /// <summary>
    /// Provides data for the CustomDrawCell event.
    /// </summary>
    public class BeepTreeCustomDrawCellEventArgs : BeepTreeCustomDrawEventArgs
    {
        /// <summary>The node being drawn.</summary>
        public SimpleItem Node { get; }

        /// <summary>The column being drawn.</summary>
        public BeepTreeColumn Column { get; }

        /// <summary>The column index.</summary>
        public int ColumnIndex { get; }

        /// <summary>The cell value.</summary>
        public object Value { get; }

        /// <summary>Whether the node is selected.</summary>
        public bool IsSelected { get; }

        /// <summary>The default draw action.</summary>
        private readonly Action _defaultDrawAction;

        public BeepTreeCustomDrawCellEventArgs(Graphics graphics, Rectangle bounds, SimpleItem node, BeepTreeColumn column, int columnIndex, object value, bool isSelected, Action defaultDrawAction)
            : base(graphics, bounds)
        {
            Node = node;
            Column = column;
            ColumnIndex = columnIndex;
            Value = value;
            IsSelected = isSelected;
            _defaultDrawAction = defaultDrawAction;
        }

        public override void DefaultDraw()
        {
            _defaultDrawAction?.Invoke();
        }
    }

    /// <summary>
    /// Provides data for the CustomDrawColumnHeader event.
    /// </summary>
    public class BeepTreeCustomDrawColumnHeaderEventArgs : BeepTreeCustomDrawEventArgs
    {
        /// <summary>The column being drawn.</summary>
        public BeepTreeColumn Column { get; }

        /// <summary>The column index.</summary>
        public int ColumnIndex { get; }

        /// <summary>The default draw action.</summary>
        private readonly Action _defaultDrawAction;

        public BeepTreeCustomDrawColumnHeaderEventArgs(Graphics graphics, Rectangle bounds, BeepTreeColumn column, int columnIndex, Action defaultDrawAction)
            : base(graphics, bounds)
        {
            Column = column;
            ColumnIndex = columnIndex;
            _defaultDrawAction = defaultDrawAction;
        }

        public override void DefaultDraw()
        {
            _defaultDrawAction?.Invoke();
        }
    }
}
