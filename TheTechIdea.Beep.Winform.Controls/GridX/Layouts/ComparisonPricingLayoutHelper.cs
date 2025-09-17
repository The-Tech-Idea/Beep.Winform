using System;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Comparison/pricing style layout preset for BeepGridPro.
    /// </summary>
    public sealed class ComparisonPricingLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Sizing
            grid.RowHeight = 30;              // comfortable density
            grid.ColumnHeaderHeight = 40;     // prominent headers for plan names

            // Render flags
            grid.Render.ShowGridLines = true;                 // keep subtle lines
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = true;               // striped rows improve readability
            grid.Render.UseHeaderGradient = false;           // flat headers by default
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = true;            // strong header emphasis
            grid.Render.HeaderCellPadding = 4;

            // Ensure we show headers
            grid.ShowColumnHeaders = true;

            // Alignment heuristics (best-effort – only if per-column alignment is supported on your models)
            var columns = grid?.Columns?.ToList();
            if (columns != null && columns.Count > 0)
            {
                // First visible column: left align (feature names)
                var first = columns.FirstOrDefault(c => c.Visible);
                if (first != null)
                {
                    first.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                    first.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }

                // Other visible columns: center (plan values)
                foreach (var col in columns.Where(c => c != first && c.Visible))
                {
                    col.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    col.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
            }
        }
    }
}
