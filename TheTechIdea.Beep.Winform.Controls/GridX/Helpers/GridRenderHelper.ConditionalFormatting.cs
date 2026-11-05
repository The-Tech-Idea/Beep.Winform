using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridRenderHelper
    {
        /// <summary>
        /// Applies a matching conditional rule, then the active search highlight.
        /// Selection and the focused row win over both so the user's current
        /// position is never hidden.
        /// </summary>
        internal (Color Back, Color Fore, FontStyle? FontStyle) ApplyConditionalFormatting(
            BeepRowConfig row, int rowIndex, BeepColumnConfig column, BeepCellConfig cell,
            Color back, Color fore, bool isSelectedRow, bool isActiveRow)
        {
            if (isSelectedRow || isActiveRow) return (back, fore, null);

            FontStyle? fontStyle = null;
            var style = _grid.ConditionalFormatting.Resolve(row, rowIndex, column);
            if (style != null)
            {
                if (style.BackColor.HasValue) back = style.BackColor.Value;
                if (style.ForeColor.HasValue) fore = style.ForeColor.Value;
                fontStyle = style.FontStyle;
            }

            // Search highlight is a background tint over the whole matching
            // cell; it never changes the value or the conditional foreground.
            string query = _grid.ToolbarState?.SearchText ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(query))
            {
                string text = Convert.ToString(cell?.CellValue) ?? string.Empty;
                if (text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    back = _grid.ChromeState.ActiveBackground;
            }

            return (back, fore, fontStyle);
        }
    }
}

