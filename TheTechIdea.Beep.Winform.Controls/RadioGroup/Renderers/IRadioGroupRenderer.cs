using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Interface for rendering different styles of radio group items
    /// </summary>
    public interface IRadioGroupRenderer
    {
        /// <summary>
        /// Gets the name of this renderer Style
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
        /// Gets or sets the active selection mode contract from owner control.
        /// Renderers must use this value instead of runtime reflection.
        /// </summary>
        bool AllowMultipleSelection { get; set; }
        
        /// <summary>
        /// Gets or sets the BeepControlStyle for this renderer
        /// </summary>
        BeepControlStyle ControlStyle { get; set; }
        
        /// <summary>
        /// Gets or sets whether to use theme colors (true) or style colors (false)
        /// </summary>
        bool UseThemeColors { get; set; }

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

        /// <summary>
        /// Releases any cached GDI+ resources (Fonts, Pens, Brushes).
        /// Called by BeepRadioGroup.Dispose().
        /// </summary>
        void Cleanup();
    }

    /// <summary>
    /// Optional interface for renderers that draw their own WCAG 2.2-compliant focus ring.
    /// When a renderer implements this, BeepRadioGroup delegates focus painting to it
    /// instead of drawing a generic focus rectangle.
    /// </summary>
    public interface IFocusAwareRenderer
    {
        /// <summary>
        /// Draws a WCAG 2.2 SC 2.4.11 compliant focus indicator for the given item.
        /// Minimum 2px outline, minimum 3:1 contrast against adjacent colors.
        /// </summary>
        void DrawFocusRing(Graphics graphics, Rectangle itemRectangle, RadioItemState state);
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
        /// Whether the item is in a validation error state.
        /// Renderers draw an error border / indicator when true.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Animation progress for pressed/selection transitions (0.0 = start, 1.0 = complete).
        /// Set by BeepRadioGroup's animation timer. Renderers interpolate visual properties
        /// (overlay alpha, geometry offset) using this value.
        /// </summary>
        public float AnimationProgress { get; set; }

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
                IsSelected        = IsSelected,
                IsHovered         = IsHovered,
                IsFocused         = IsFocused,
                IsEnabled         = IsEnabled,
                IsPressed         = IsPressed,
                IsError           = IsError,
                AnimationProgress = AnimationProgress,
                Index             = Index,
                Tag               = Tag
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
        /// Checkbox-Style square selection
        /// </summary>
        Checkbox,

        /// <summary>
        /// Material Design Style
        /// </summary>
        Material,

        /// <summary>
        /// Card-based selection (modern web Style)
        /// </summary>
        Card,

        /// <summary>
        /// Chip/pill Style selection
        /// </summary>
        Chip,

        /// <summary>
        /// Modern flat design Style
        /// </summary>
        Flat,

        /// <summary>
        /// Button-group Style
        /// </summary>
        Button,

        /// <summary>
        /// Toggle switch Style
        /// </summary>
        Toggle,
        
        /// <summary>
        /// iOS-style segmented control (connected buttons)
        /// </summary>
        Segmented,
        
        /// <summary>
        /// Pill-shaped buttons (fully rounded capsules)
        /// </summary>
        Pill,
        
        /// <summary>
        /// Large touch-friendly tiles (Windows 8/10 style)
        /// </summary>
        Tile,

        /// <summary>
        /// Custom renderer
        /// </summary>
        Custom
    }
}