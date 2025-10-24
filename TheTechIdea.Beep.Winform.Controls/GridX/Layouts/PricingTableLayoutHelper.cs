using System.Linq;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Pricing table-Style layout preset.
    /// - Taller headers to allow plan badges/images (painter may draw images if provided by columns)
    /// - First (feature) column left-aligned and sticky
    /// - Plan columns centered (headers and cells)
    /// - No row stripes, minimal grid lines
    /// - Subtle elevation/gradient on headers for hierarchy
    /// </summary>
    public sealed class PricingTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Dimensions
            grid.RowHeight = 28;                  // comfortable for feature lists
            grid.ColumnHeaderHeight = 56;         // room for icon/badge + title
            grid.ShowColumnHeaders = true;

            // Render flags (structure only, not colors)
            grid.Render.ShowGridLines = false;    // pricing tables are usually clean
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = true; // subtle hierarchy
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = true; // emphasize plan names
            grid.Render.HeaderCellPadding = 6;    // more breathing room
            grid.Render.UseElevation = true;      // slight elevation for header
            grid.Render.CardStyle = false;
            grid.Render.ShowSortIndicators = false; // pricing tables rarely sortable by columns

            // Alignment heuristics
            var cols = grid.Columns?.Where(c => c.Visible).ToList();
            if (cols == null || cols.Count == 0) return;

            // Identify the feature column (first visible non-system column)
            var featureCol = cols.FirstOrDefault(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID);

            foreach (var c in cols)
            {
                bool isSystem = c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID;
                if (isSystem)
                {
                    c.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = ContentAlignment.MiddleCenter;
                    continue;
                }

                var name = (c.ColumnCaption ?? c.ColumnName ?? string.Empty).ToLowerInvariant();
                bool isPrice = name.Contains("price") || name.Contains("cost") || name.Contains("amount") || name.Contains("monthly") || name.Contains("year");
                bool isAction = name.Contains("action") || name.Contains("buy") || name.Contains("select") || name.Contains("subscribe") || name.Contains("upgrade");

                if (c == featureCol)
                {
                    // Feature names on the left, sticky for better scanning
                    c.HeaderTextAlignment = ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = ContentAlignment.MiddleLeft;
                    c.Sticked = true; // keep features visible when scrolling horizontally
                }
                else if (isPrice || isAction)
                {
                    c.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = ContentAlignment.MiddleCenter;
                }
                else
                {
                    // Default: center plan columns
                    c.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = ContentAlignment.MiddleCenter;
                }
            }
        }
    }
}
