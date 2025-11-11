using System;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Filter interaction patterns - defines HOW filters work, not their visual appearance
    /// Visual styling (colors, borders, shadows) comes from BeepStyling and BeepControlStyle
    /// </summary>
    public enum FilterStyle
    {
        /// <summary>
        /// Simple tag/chip pills with removable X buttons (like image 1)
        /// - Horizontal row of tag pills
        /// - Click X to remove
        /// - Click tag to edit
        /// </summary>
        TagPills,

        /// <summary>
        /// Grouped filter rows with AND/OR logic (like images 2 and 4)
        /// - Visual grouping with indentation
        /// - AND/OR connectors between groups
        /// - Add filter/Add group buttons
        /// - Nested subgroups supported
        /// </summary>
        GroupedRows,

        /// <summary>
        /// Query builder with visual logic branches (like image 3)
        /// - If/Else conditions
        /// - Visual tree/branches showing logic flow
        /// - Prompt vs Logic toggle
        /// - Complex nested conditions
        /// </summary>
        QueryBuilder,

        /// <summary>
        /// Dropdown-based multi-select filters (like image 2 - "Omitted" dropdown)
        /// - Field selector dropdown
        /// - Operator dropdown (is, is any of, etc.)
        /// - Value dropdown/multi-select with checkboxes
        /// - Shows selected count badge
        /// </summary>
        DropdownMultiSelect,

        /// <summary>
        /// Inline filter row (single line per condition)
        /// - Column | Operator | Value in one row
        /// - Minimal spacing, compact layout
        /// - Quick add/remove buttons
        /// </summary>
        InlineRow,

        /// <summary>
        /// Sidebar panel filters (e-commerce style)
        /// - Vertical list of filter categories
        /// - Checkboxes/toggles for each option
        /// - Collapsible sections
        /// - Apply/Clear buttons at bottom
        /// </summary>
        SidebarPanel,

        /// <summary>
        /// Quick search bar with instant filtering
        /// - Single search input
        /// - Column selector dropdown
        /// - Real-time filtering as you type
        /// - Active filter count badge
        /// </summary>
        QuickSearch,

        /// <summary>
        /// Advanced dialog with multiple sections
        /// - Modal popup or slide-in panel
        /// - Tabbed sections for different filter types
        /// - Save/Load filter configurations
        /// - Preview results count
        /// </summary>
        AdvancedDialog
    }

    /// <summary>
    /// Filter display mode - determines how filters are shown
    /// </summary>
    public enum FilterDisplayMode
    {
        /// <summary>
        /// Always visible
        /// </summary>
        AlwaysVisible,

        /// <summary>
        /// Collapsible/expandable
        /// </summary>
        Collapsible,

        /// <summary>
        /// Show on hover
        /// </summary>
        OnHover,

        /// <summary>
        /// Modal popup
        /// </summary>
        Modal,

        /// <summary>
        /// Slide-in panel
        /// </summary>
        SlideIn
    }

    /// <summary>
    /// Filter position relative to the parent control
    /// </summary>
    public enum FilterPosition
    {
        /// <summary>
        /// Top of parent control
        /// </summary>
        Top,

        /// <summary>
        /// Bottom of parent control
        /// </summary>
        Bottom,

        /// <summary>
        /// Left side of parent control
        /// </summary>
        Left,

        /// <summary>
        /// Right side of parent control
        /// </summary>
        Right,

        /// <summary>
        /// Floating/overlay
        /// </summary>
        Floating,

        /// <summary>
        /// Embedded inline
        /// </summary>
        Inline
    }
}
