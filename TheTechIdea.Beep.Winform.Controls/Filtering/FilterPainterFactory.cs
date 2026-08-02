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

    }
}
