using System;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Filtering.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Factory for creating filter painters based on FilterStyle
    /// </summary>
    public static class FilterPainterFactory
    {
        /// <summary>
        /// Creates a filter painter for the specified style
        /// </summary>
        /// <param name="style">Filter style to create painter for</param>
        /// <param name="controlStyle">BeepControlStyle instance (currently unused, reserved for future use)</param>
        /// <returns>Filter painter instance</returns>
        public static IFilterPainter CreatePainter(FilterStyle style, BeepControlStyle controlStyle)
        {
            // Note: controlStyle parameter is currently unused but kept for API compatibility
            // Painters access styling through the BeepFilter owner parameter
            
            return style switch
            {
                FilterStyle.TagPills => new TagPillsFilterPainter(),
                FilterStyle.GroupedRows => new GroupedRowsFilterPainter(),
                FilterStyle.InlineRow => new InlineRowFilterPainter(),
                FilterStyle.QuickSearch => new QuickSearchFilterPainter(),
                FilterStyle.QueryBuilder => new QueryBuilderFilterPainter(),
                FilterStyle.DropdownMultiSelect => new DropdownMultiSelectFilterPainter(),
                FilterStyle.SidebarPanel => new SidebarPanelFilterPainter(),
                FilterStyle.AdvancedDialog => new AdvancedDialogFilterPainter(),
                
                _ => new TagPillsFilterPainter() // Default fallback
            };
        }

        /// <summary>
        /// Checks if a painter is fully implemented for the given style
        /// </summary>
        /// <param name="style">Filter style to check</param>
        /// <returns>True if fully implemented, false if using fallback</returns>
        public static bool IsFullyImplemented(FilterStyle style)
        {
            return style switch
            {
                FilterStyle.TagPills => true,
                FilterStyle.GroupedRows => true,
                FilterStyle.InlineRow => true,
                FilterStyle.QuickSearch => true,
                FilterStyle.QueryBuilder => true,
                FilterStyle.DropdownMultiSelect => true,
                FilterStyle.SidebarPanel => true,
                FilterStyle.AdvancedDialog => true,
                _ => false
            };
        }

        /// <summary>
        /// Gets a description of what the filter style does
        /// </summary>
        public static string GetStyleDescription(FilterStyle style)
        {
            return style switch
            {
                FilterStyle.TagPills => "Horizontal row of tag chips with X buttons - simple and clean",
                FilterStyle.GroupedRows => "Filter rows with AND/OR logic connectors - structured and powerful",
                FilterStyle.InlineRow => "Compact single-line filter rows - space-efficient",
                FilterStyle.QuickSearch => "Single search bar with instant filtering - quick and easy",
                FilterStyle.QueryBuilder => "Visual logic tree with If/Else conditions - complex queries",
                FilterStyle.DropdownMultiSelect => "Dropdown multi-select with checkboxes - user-friendly",
                FilterStyle.SidebarPanel => "Vertical sidebar with collapsible categories - e-commerce style",
                FilterStyle.AdvancedDialog => "Modal or slide-in panel with multiple sections - feature-rich",
                _ => "Unknown filter style"
            };
        }
    }
}
