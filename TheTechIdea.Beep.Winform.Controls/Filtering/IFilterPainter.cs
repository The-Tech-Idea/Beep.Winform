using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.GridX.Filtering;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Hit area information for filter UI elements
    /// Used for click detection, hover effects, and interaction
    /// </summary>
    public class FilterHitArea
    {
        public string Name { get; set; } = string.Empty;
        public Rectangle Bounds { get; set; }
        public object? Tag { get; set; }
        public FilterHitAreaType Type { get; set; }
    }

    /// <summary>
    /// Types of filter hit areas
    /// </summary>
    public enum FilterHitAreaType
    {
        None,
        FilterTag,
        RemoveButton,
        EditButton,
        AddFilterButton,
        AddGroupButton,
        LogicConnector,
        FieldDropdown,
        OperatorDropdown,
        ValueInput,
        ValueDropdown,
        DragHandle,
        CollapseButton,
        SearchInput,
        ClearAllButton,
        SaveButton,
        LoadButton,
        ApplyButton
    }

    /// <summary>
    /// Layout information calculated by filter painter
    /// Contains all positioned rectangles for filter UI elements
    /// </summary>
    public class FilterLayoutInfo
    {
        public Rectangle ContainerRect { get; set; }
        public Rectangle ContentRect { get; set; }
        public Rectangle HeaderRect { get; set; }
        public Rectangle FooterRect { get; set; }
        
        // For tag pills
        public Rectangle[] TagRects { get; set; } = new Rectangle[0];
        public Rectangle[] RemoveButtonRects { get; set; } = new Rectangle[0];
        
        // For grouped rows
        public Rectangle[] RowRects { get; set; } = new Rectangle[0];
        public Rectangle[] ConnectorRects { get; set; } = new Rectangle[0];
        public Rectangle[] DragHandleRects { get; set; } = new Rectangle[0];
        
        // For dropdowns
        public Rectangle[] FieldDropdownRects { get; set; } = new Rectangle[0];
        public Rectangle[] OperatorDropdownRects { get; set; } = new Rectangle[0];
        public Rectangle[] ValueDropdownRects { get; set; } = new Rectangle[0];
        
        // Action buttons
        public Rectangle AddFilterButtonRect { get; set; }
        public Rectangle AddGroupButtonRect { get; set; }
        public Rectangle ClearAllButtonRect { get; set; }
        public Rectangle SaveButtonRect { get; set; }
        public Rectangle LoadButtonRect { get; set; }
        public Rectangle ApplyButtonRect { get; set; }
        
        // Search/quick filter
        public Rectangle SearchInputRect { get; set; }
        public Rectangle SearchIconRect { get; set; }
        
        // Badge/counter
        public Rectangle CountBadgeRect { get; set; }
    }

    /// <summary>
    /// Interface for filter painters that handle custom rendering of BeepFilter controls
    /// Provides methods for painting different filter styles with modern effects
    /// Handles layout calculations and hit area registration for interaction
    /// Colors, borders, shadows come from BeepStyling - painters only handle layout and structure
    /// </summary>
    public interface IFilterPainter
    {
        /// <summary>
        /// Calculates layout positions specific to this painter's FilterStyle and registers hit areas
        /// </summary>
        /// <param name="owner">The BeepFilter control instance being laid out</param>
        /// <param name="availableRect">Available rectangle for the filter UI</param>
        /// <returns>Layout information with all positioned elements</returns>
        FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect);

        /// <summary>
        /// Paints the filter UI with the specified style
        /// </summary>
        /// <param name="g">The graphics context to paint on</param>
        /// <param name="owner">The BeepFilter control instance being painted</param>
        /// <param name="layout">Pre-calculated layout information</param>
        void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout);

        /// <summary>
        /// Performs hit testing to determine which filter element is at the specified point
        /// </summary>
        /// <param name="point">Point to test (in control coordinates)</param>
        /// <param name="layout">Current layout information</param>
        /// <returns>Hit area information or null if no hit</returns>
        FilterHitArea? HitTest(Point point, FilterLayoutInfo layout);

        /// <summary>
        /// Gets the metrics/measurements for this painter style
        /// </summary>
        FilterPainterMetrics GetMetrics(BeepFilter owner);

        /// <summary>
        /// Whether this painter supports animations
        /// </summary>
        bool SupportsAnimations { get; }

        /// <summary>
        /// Whether this painter supports drag and drop reordering
        /// </summary>
        bool SupportsDragDrop { get; }

        /// <summary>
        /// Gets the filter style this painter implements
        /// </summary>
        FilterStyle FilterStyle { get; }
    }

    /// <summary>
    /// Optional interface for painters that provide metrics
    /// </summary>
    public interface IFilterPainterMetricsProvider
    {
        FilterPainterMetrics GetMetrics(BeepFilter owner);
    }
}
