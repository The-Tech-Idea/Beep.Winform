using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Design system table preset with checkbox selection, subtle separators, and alignment heuristics.
    /// </summary>
    public sealed class DesignSystemTableLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Sizing
            grid.RowHeight = 40;
            grid.ColumnHeaderHeight = 48;
            grid.ShowColumnHeaders = true;

            // Ensure selection checkbox rendering is on
            grid.ShowCheckBox = true;

            // Render flags
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = false;
            grid.Render.UseHeaderGradient = false;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = false;
            grid.Render.HeaderCellPadding = 8;
            grid.Render.UseElevation = false;
            grid.Render.CardStyle = false;

            var cols = grid?.Columns?.ToList();
            if (cols == null || cols.Count == 0) return;

            // Make sure checkbox column is visible if exists
            var chk = cols.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (chk != null) chk.Visible = true;

            // Alignments
            var firstVisible = cols.FirstOrDefault(c => c.Visible && !c.IsSelectionCheckBox);
            if (firstVisible != null)
            {
                firstVisible.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                firstVisible.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            }

            foreach (var c in cols.Where(x => x.Visible))
            {
                var name = (c.ColumnCaption ?? c.ColumnName ?? string.Empty).ToLowerInvariant();

                if (c.IsSelectionCheckBox)
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else if (name.Contains("status"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else if (name.Contains("type"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else if (name.Contains("sku") || name.Contains("id"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else if (name.Contains("price") || name.Contains("usd") || name.Contains("amount") || name.Contains("total"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
                else if (name.Contains("contact") || name.Contains("company") || name.Contains("customer"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }
                else if (name.Contains("action"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else if (c != firstVisible)
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }
            }
        }
    }
}
