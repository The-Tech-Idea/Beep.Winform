using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public enum SortDirection
    {
        None,
        Ascending,
        Descending
    }

    // Add enum for grid styles
    public enum BeepGridStyle
    {
        Classic,
        Modern,
        Flat,
        Material,
        Dark
    }
    /// <summary>
    /// Enum for modern gradient types
    /// </summary>
    public enum ModernGradientType
    {
        None,
        Linear,
        Radial,
        Conic,
        Mesh,
        Subtle
    }
    public enum BeepShapeType
    {
        Line,
        Rectangle,
        Ellipse,
        Triangle,
        Star,
        Diamond,
        Pentagon,
        Hexagon,
        Octagon
    }
    #region Supporting Enums and Classes
    public enum ShapeFillStyle
    {
        None,
        Solid,
        Gradient,
        Hatch
    }

    public enum ResizeHandle
    {
        None = -1,
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3
    }
    #endregion
    #region Supporting Classes and Enums
    public class DockItemState
    {
        public SimpleItem Item { get; set; }
        public float CurrentScale { get; set; } = 1.0f;
        public float TargetScale { get; set; } = 1.0f;
        public bool IsHovered { get; set; }
        public bool IsSelected { get; set; }
    }

    public enum DockPosition
    {
        Top,
        Bottom,
        Left,
        Right,
        Center
    }

    public enum DockOrientation
    {
        Horizontal,
        Vertical
    }
    #endregion

}
