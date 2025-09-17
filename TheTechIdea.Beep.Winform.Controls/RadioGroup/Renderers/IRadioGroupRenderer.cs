using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Interface for rendering different styles of radio group items
    /// </summary>
    public interface IRadioGroupRenderer
    {
        /// <summary>
        /// Gets the name of this renderer style
        /// </summary>
        string StyleName { get; }

        /// <summary>
        /// Gets the display name for this renderer
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets whether this renderer supports multiple selection
        /// </summary>
        bool SupportsMultipleSelection { get; }

        /// <summary>
        /// Initializes the renderer with the parent control and theme
        /// </summary>
        void Initialize(BaseControl owner, IBeepTheme theme);

        /// <summary>
        /// Renders a single radio group item
        /// </summary>
        /// <param name="graphics">Graphics context to draw on</param>
        /// <param name="item">The item to render</param>
        /// <param name="rectangle">The rectangle to render within</param>
        /// <param name="state">The current state of the item</param>
        void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state);

        /// <summary>
        /// Calculates the minimum size needed for an item
        /// </summary>
        /// <param name="item">The item to measure</param>
        /// <param name="graphics">Graphics context for measurements</param>
        /// <returns>The minimum size needed</returns>
        Size MeasureItem(SimpleItem item, Graphics graphics);

        /// <summary>
        /// Gets the content area within an item rectangle (excluding margins, borders, etc.)
        /// </summary>
        /// <param name="itemRectangle">The full item rectangle</param>
        /// <returns>The content area rectangle</returns>
        Rectangle GetContentArea(Rectangle itemRectangle);

        /// <summary>
        /// Gets the selector area within an item rectangle (for the radio button/checkbox)
        /// </summary>
        /// <param name="itemRectangle">The full item rectangle</param>
        /// <returns>The selector area rectangle</returns>
        Rectangle GetSelectorArea(Rectangle itemRectangle);

        /// <summary>
        /// Called when the theme changes to update appearance
        /// </summary>
        void UpdateTheme(IBeepTheme theme);

        /// <summary>
        /// Renders any additional decorations or effects for the entire group
        /// </summary>
        /// <param name="graphics">Graphics context to draw on</param>
        /// <param name="groupRectangle">The rectangle of the entire group</param>
        /// <param name="items">All items in the group</param>
        /// <param name="itemRectangles">Rectangles for all items</param>
        /// <param name="states">States for all items</param>
        void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, 
            List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states);
    }

    /// <summary>
    /// Represents the state of a radio group item
    /// </summary>
    public class RadioItemState
    {
        /// <summary>
        /// Whether the item is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Whether the item is hovered
        /// </summary>
        public bool IsHovered { get; set; }

        /// <summary>
        /// Whether the item is focused
        /// </summary>
        public bool IsFocused { get; set; }

        /// <summary>
        /// Whether the item is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Whether the item is pressed/active
        /// </summary>
        public bool IsPressed { get; set; }

        /// <summary>
        /// The index of the item in the group
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Additional custom state data
        /// </summary>
        public object Tag { get; set; }

        public RadioItemState()
        {
        }

        public RadioItemState(bool isSelected, bool isHovered = false, bool isFocused = false, bool isEnabled = true)
        {
            IsSelected = isSelected;
            IsHovered = isHovered;
            IsFocused = isFocused;
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Creates a copy of this state
        /// </summary>
        public RadioItemState Clone()
        {
            return new RadioItemState
            {
                IsSelected = IsSelected,
                IsHovered = IsHovered,
                IsFocused = IsFocused,
                IsEnabled = IsEnabled,
                IsPressed = IsPressed,
                Index = Index,
                Tag = Tag
            };
        }
    }

    /// <summary>
    /// Optional interface for renderers that handle images
    /// </summary>
    public interface IImageAwareRenderer
    {
        Size MaxImageSize { get; set; }
    }

    /// <summary>
    /// Enumeration of common render styles
    /// </summary>
    public enum RadioGroupRenderStyle
    {
        /// <summary>
        /// Traditional circular radio buttons
        /// </summary>
        Circular,

        /// <summary>
        /// Checkbox-style square selection
        /// </summary>
        Checkbox,

        /// <summary>
        /// Material Design style
        /// </summary>
        Material,

        /// <summary>
        /// Card-based selection (modern web style)
        /// </summary>
        Card,

        /// <summary>
        /// Chip/pill style selection
        /// </summary>
        Chip,

        /// <summary>
        /// Modern flat design style
        /// </summary>
        Flat,

        /// <summary>
        /// Button-group style
        /// </summary>
        Button,

        /// <summary>
        /// Toggle switch style
        /// </summary>
        Toggle,

        /// <summary>
        /// Custom renderer
        /// </summary>
        Custom
    }
}