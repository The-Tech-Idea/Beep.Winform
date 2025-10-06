using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    internal static class LayoutCommon
    {
        /// <summary>
        /// Applies generic alignment heuristics in a domain-agnostic way.
        /// - First visible non-system column is left aligned
        /// - System columns (checkbox/rownum) are centered
        /// - Obvious action/numeric/status columns are centered
        /// </summary>
        public static void ApplyAlignmentHeuristics(BeepGridPro grid)
        {
            if (grid?.Data?.Columns == null || grid.Data.Columns.Count == 0) return;

            var cols = grid.Data.Columns.Where(c => c.Visible).ToList();
            if (cols.Count == 0) return;

            var firstVis = cols.FirstOrDefault(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID);

            foreach (var c in cols)
            {
                bool isSystem = c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID;
                if (isSystem)
                {
                    c.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = ContentAlignment.MiddleCenter;
                    continue;
                }

                // Obvious names that should be centered
                var name = (c.ColumnCaption ?? c.ColumnName ?? string.Empty).ToLowerInvariant();
                bool isCenterCandidate = name.Contains("status") || name.Contains("state") ||
                                         name.Contains("action") || name.Contains("actions") ||
                                         name.Contains("price") || name.Contains("total") ||
                                         name.Contains("qty") || name.Contains("quantity");

                if (firstVis != null && c == firstVis)
                {
                    c.HeaderTextAlignment = ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = ContentAlignment.MiddleLeft;
                }
                else if (isCenterCandidate)
                {
                    c.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = ContentAlignment.MiddleCenter;
                }
                else
                {
                    // Default left for readability
                    c.HeaderTextAlignment = ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = ContentAlignment.MiddleLeft;
                }
            }
        }
    }
}
