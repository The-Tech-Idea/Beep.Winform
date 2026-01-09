using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Painters
{
    /// <summary>
    /// State information for an accordion item
    /// </summary>
    public struct AccordionItemState
    {
        public bool IsHovered;
        public bool IsSelected;
        public bool IsExpanded;
        public bool IsChild;
        public int Level; // Nesting level (0 = root, 1 = child, etc.)
    }

    /// <summary>
    /// Rendering options for accordion painters
    /// </summary>
    public class AccordionRenderOptions
    {
        public IBeepTheme Theme { get; set; }
        public bool UseThemeColors { get; set; } = true;
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;
        public AccordionMenus.Helpers.AccordionStyle AccordionStyle { get; set; } = AccordionMenus.Helpers.AccordionStyle.Material3;
        public int ItemHeight { get; set; } = 40;
        public int ChildItemHeight { get; set; } = 32;
        public int HeaderHeight { get; set; } = 48;
        public int IndentationWidth { get; set; } = 20;
        public int Spacing { get; set; } = 2;
        public int Padding { get; set; } = 8;
        public int BorderRadius { get; set; } = 8;
        public int HighlightWidth { get; set; } = 4;
        public bool IsCollapsed { get; set; } = false;
        public Font HeaderFont { get; set; }
        public Font ItemFont { get; set; }
        public Font ChildItemFont { get; set; }
    }

    /// <summary>
    /// Interface for accordion menu painters
    /// </summary>
    public interface IAccordionPainter
    {
        /// <summary>
        /// Paint the accordion background
        /// </summary>
        void PaintAccordionBackground(Graphics g, Rectangle bounds, AccordionRenderOptions options);

        /// <summary>
        /// Paint the header/title area
        /// </summary>
        void PaintHeader(Graphics g, Rectangle bounds, string title, AccordionRenderOptions options);

        /// <summary>
        /// Paint a menu item (header item)
        /// </summary>
        void PaintItem(Graphics g, Rectangle bounds, SimpleItem item, AccordionItemState state, AccordionRenderOptions options);

        /// <summary>
        /// Paint a child menu item
        /// </summary>
        void PaintChildItem(Graphics g, Rectangle bounds, SimpleItem item, AccordionItemState state, AccordionRenderOptions options);

        /// <summary>
        /// Paint the expand/collapse icon
        /// </summary>
        void PaintExpanderIcon(Graphics g, Rectangle bounds, bool isExpanded, AccordionRenderOptions options);

        /// <summary>
        /// Paint connector line from parent to child
        /// </summary>
        void PaintConnectorLine(Graphics g, Point start, Point end, AccordionRenderOptions options);
    }
}
