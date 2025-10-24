using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public enum NavigationButtonState
    {
        Normal,
        Hovered,
        Pressed,
        Disabled
    }
    public enum NavigationButtonType
    {
        First,
        Previous,
        Next,
        Last,
        AddNew,
        Delete,
        Save,
        Cancel
    }
    public enum navigationPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    public enum navigationAlignment
    {
        Left,
        Center,
        Right
    }
    /// <summary>
    /// Grid Style presets inspired by popular JavaScript frameworks and design systems
    /// </summary>
    //public enum BeepGridStyle
    //{
    //    /// <summary>
    //    /// Default Beep grid styling
    //    /// </summary>
    //    Default,

    //    /// <summary>
    //    /// Clean, minimal styling with subtle borders
    //    /// </summary>
    //    Clean,

    //    /// <summary>
    //    /// Bootstrap-inspired table styling with striped rows
    //    /// </summary>
    //    Bootstrap,

    //    /// <summary>
    //    /// Material Design table styling
    //    /// </summary>
    //    Material,

    //    /// <summary>
    //    /// Modern flat design with minimal borders
    //    /// </summary>
    //    Flat,


    //    /// <summary>
    //    /// Compact styling for dense data display
    //    /// </summary>
    //    Compact,

    //    /// <summary>
    //    /// Professional corporate styling
    //    /// </summary>
    //    Corporate,

    //    /// <summary>
    //    /// Minimalist styling with focus on content
    //    /// </summary>
    //    Minimal,

    //    /// <summary>
    //    /// Card-based styling for modern UIs
    //    /// </summary>
    //    Card,

    //    /// <summary>
    //    /// Borderless modern styling
    //    /// </summary>
    //    Borderless
    //}
    public enum navigationStyle
    {
        None,
        /// <summary>
        /// Classic Windows Forms Style with standard buttons
        /// </summary>
        Standard,
        
        /// <summary>
        /// Modern flat design with Material Design principles
        /// </summary>
        Material,
        
        /// <summary>
        /// Compact space-saving design similar to DevExpress
        /// </summary>
        Compact,
        
        /// <summary>
        /// Minimal design with icon-only buttons
        /// </summary>
        Minimal,
        
        /// <summary>
        /// Bootstrap-inspired navigation with primary/secondary button styles
        /// </summary>
        Bootstrap,
        
        /// <summary>
        /// Fluent Design (Microsoft) with acrylic effects and modern spacing
        /// </summary>
        Fluent,
        
        /// <summary>
        /// Ant Design inspired clean and professional look
        /// </summary>
        AntDesign,
        
        /// <summary>
        /// Telerik/Kendo UI inspired professional grid navigation
        /// </summary>
        Telerik,
        
        /// <summary>
        /// AG Grid inspired modern web grid navigation
        /// </summary>
        AGGrid,
        
        /// <summary>
        /// DataTables (jQuery) inspired pagination Style
        /// </summary>
        DataTables,
        
        /// <summary>
        /// Card-based modern navigation
        /// </summary>
        Card,
        
        /// <summary>
        /// Flat minimal design inspired by Tailwind CSS
        /// </summary>
        Tailwind
    }
}